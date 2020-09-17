using System;
using System.IO;
using JobeSharp.Languages.Abstract;
using JobeSharp.Sandbox;

namespace JobeSharp.Languages
{
    internal class LanguageExecutor : IDisposable
    {
        private string WorkTempDirectory { get; } =
            Path.Combine(Path.GetTempPath(), "jobe", "executions", Guid.NewGuid().ToString());
        
        private ExecutionTask Task { get; }

        public LanguageExecutor(ExecutionTask task)
        {
            Task = task;

            Directory.CreateDirectory(WorkTempDirectory);
        }

        public ExecutionResult Execute()
        {
            return Task.Language switch
            {
                IInterpreted interpreted => ExecuteInterpreted(interpreted),
                ICompiled compiled => ExecuteCompiled(compiled),
                _ => throw new NotImplementedException()
            };
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
            Directory.Delete(WorkTempDirectory, recursive: true);
        }
    }
}