using System.Text.RegularExpressions;
using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Versions;
using JobeSharp.Sandbox;

namespace JobeSharp.Languages.Concrete
{
    internal class PascalLanguage : CompiledLanguage
    {
        public override string Name => "pascal";
        
        protected override IVersionProvider VersionProvider => 
            new CommandRegexVersionProvider("fpc -h", new Regex("version\\s(.*?\\[.*?\\])"));

        protected override CompileExecutionResult Compile(ExecutionTask executionTask)
        {
            var compileCommand = $"fpc {executionTask.GetCompileArguments()} -omain.o {GetSourceFilePath(executionTask)} {executionTask.GetLinkArguments()}";
            
            return new CompileExecutionResult(
                SandboxExecutor.Execute(compileCommand, executionTask.ExecuteOptions));
        }

        protected override RunExecutionResult Run(ExecutionTask executionTask)
        {
            executionTask.ExecuteOptions.StdIn = executionTask.Input;
            
            return new RunExecutionResult(
                SandboxExecutor.Execute($"main.o {executionTask.GetRunArguments()}", executionTask.ExecuteOptions));
        }
    }
}