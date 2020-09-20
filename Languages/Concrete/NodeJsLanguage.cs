using System.Text.RegularExpressions;
using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Versions;
using JobeSharp.Sandbox;

namespace JobeSharp.Languages.Concrete
{
    internal class NodeJsLanguage : InterpretedLanguage
    {
        public override string Name => "nodejs";

        protected override IVersionProvider VersionProvider => 
            new CommandRegexVersionProvider("nodejs -v", new Regex("v([\\d.]+)"));

        protected override RunExecutionResult Run(ExecutionTask executionTask)
        {
            executionTask.ExecuteOptions.StdIn = executionTask.Input;

            var executeCommand = $"nodejs {GetSourceFilePath(executionTask)} {executionTask.GetExecuteArguments()}";
            return new RunExecutionResult(SandboxExecutor.Execute(executeCommand, executionTask.ExecuteOptions));
        }
    }
}