using System;
using System.IO;
using JobeSharp.Languages.Abstract;
using JobeSharp.Sandbox;
using JobeSharp.Services;

namespace JobeSharp.Languages
{
    internal class LanguageExecutor : IDisposable
    {
        private string WorkTempDirectory { get; } =
            Path.Combine(Path.GetTempPath(), "jobe", "executions", Guid.NewGuid().ToString());
        
        private ExecutionTask Task { get; }
        private FileCache FileCache { get; }

        public LanguageExecutor(ExecutionTask task, FileCache fileCache)
        {
            Directory.CreateDirectory(WorkTempDirectory);

            Task = task;
            FileCache = fileCache;
            Task.ExecuteOptions.WorkingDirectory = WorkTempDirectory;
        }

        public ExecutionResult Execute()
        {
            Task.SourceFileName = Task.Language.GetCorrectSourceFileName(Task);
            LoadFiles();
            return Task.Language switch
            {
                IInterpreted interpreted => ExecuteInterpreted(interpreted),
                ICompiled compiled => ExecuteCompiled(compiled),
                _ => throw new NotImplementedException()
            };
        }

        private void LoadFiles()
        {
            foreach (var (id, path) in Task.CachedFilesIdPath)
            {
                File.WriteAllBytes(Path.Combine(WorkTempDirectory, path), FileCache.ReadBytes(id));
            }
        }

        private ExecutionResult ExecuteInterpreted(IInterpreted interpreted)
        {
            var scriptFilePath = Path.Combine(WorkTempDirectory, Task.SourceFileName);
            File.WriteAllText(Path.Combine(WorkTempDirectory, Task.SourceFileName), Task.SourceCode);

            Task.ExecuteOptions.StdIn = Task.Input;
            Task.Language.CorrectExecutionOptions(Task.ExecuteOptions);
            
            var executeArguments = string.Join(" ", Task.ExecuteArguments ?? new [] { "" });
            var runCommand = interpreted.GetRunnableCommandOfScript(scriptFilePath, executeArguments);
            return new RunExecutionResult(SandboxExecutor.Execute(runCommand, Task.ExecuteOptions));
        }

        private ExecutionResult ExecuteCompiled(ICompiled compiled)
        {
            File.WriteAllText(Path.Combine(WorkTempDirectory, Task.SourceFileName), Task.SourceCode);

            Task.ExecuteOptions.StdIn = Task.Input;
            Task.Language.CorrectExecutionOptions(Task.ExecuteOptions);

            var linkArguments = string.Join(" ", Task.LinkArguments ?? new [] { "" });
            var compileArguments = string.Join(" ", Task.CompileArguments ?? new [] { "" });
            var compilationCommand = compiled.GetCompilationCommand(Task, linkArguments, compileArguments);
            
            var compileExecutionResult = new CompileExecutionResult(SandboxExecutor.Execute(compilationCommand, Task.ExecuteOptions));
            if (!compileExecutionResult.IsSuccess)
            {
                return compileExecutionResult;
            }
            
            var executeArguments = string.Join(" ", Task.ExecuteArguments ?? new [] { "" });
            var runCommand = compiled.GetRunCommand(Task, executeArguments);
            return new RunExecutionResult(SandboxExecutor.Execute(runCommand, Task.ExecuteOptions));
        }

        public void Dispose()
        {
            if (Task.Debug)
            {
                return;
            }
            
            Directory.Delete(WorkTempDirectory, recursive: true);
        }
    }
}