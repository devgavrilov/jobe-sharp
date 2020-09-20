using System.Text.RegularExpressions;
using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Versions;
using JobeSharp.Sandbox;

namespace JobeSharp.Languages.Concrete
{
    internal class Python3Language : InterpretedLanguage
    {
        public override string Name => "python3";

        protected override IVersionProvider VersionProvider => 
            new CommandRegexVersionProvider("python3 --version", new Regex("Python ([0-9._]*)"));

        protected override RunExecutionResult Run(ExecutionTask executionTask)
        {
            executionTask.ExecuteOptions.StdIn = executionTask.Input;
            executionTask.ExecuteOptions.TotalMemoryKb = 600 * 1024;

            var executeCommand = $"python3 -BE {GetSourceFilePath(executionTask)} {executionTask.GetExecuteArguments()}";
            return new RunExecutionResult(SandboxExecutor.Execute(executeCommand, executionTask.ExecuteOptions));
        }
    }
}