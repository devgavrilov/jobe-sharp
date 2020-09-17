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
            Task = task;
            FileCache = fileCache;

            Directory.CreateDirectory(WorkTempDirectory);
        }

        public ExecutionResult Execute()
        {
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

            var command = interpreted.GetRunnableCommandOfScript(scriptFilePath);
            
            return new RunExecutionResult(SandboxExecutor.Execute(command));
        }

        private ExecutionResult ExecuteCompiled(ICompiled compiled)
        {
            var sourceCodePath = Path.Combine(WorkTempDirectory, Task.SourceFileName);
            File.WriteAllText(Path.Combine(WorkTempDirectory, Task.SourceFileName), Task.SourceCode);

            var compilationCommand = compiled.GetCompilationCommandBySourceCode(sourceCodePath, Path.Combine(WorkTempDirectory, compiled.CompiledFileName));
            
            var compileExecutionResult = new CompileExecutionResult(SandboxExecutor.Execute(compilationCommand));
            if (!compileExecutionResult.IsSuccess)
            {
                return compileExecutionResult;
            }

            return new RunExecutionResult(
                SandboxExecutor.Execute(Path.Combine(WorkTempDirectory, compiled.CompiledFileName)));
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