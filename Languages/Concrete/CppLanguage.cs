using System.Text.RegularExpressions;
using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Versions;
using JobeSharp.Sandbox;

namespace JobeSharp.Languages.Concrete
{
    internal class CppLanguage : CompiledLanguage
    {
        public override string Name => "cpp";
        
        protected override IVersionProvider VersionProvider => 
            new CommandRegexVersionProvider("gcc -v", new Regex("gcc version ([\\d.]+)"));

        protected override CompileExecutionResult Compile(ExecutionTask executionTask)
        {
            var compileCommand = $"g++ {executionTask.GetCompileArguments("-Wall -Werror -x c++")} -o main.o {GetSourceFilePath(executionTask)} {executionTask.GetLinkArguments()}";
            
            return new CompileExecutionResult(
                SandboxExecutor.Execute(compileCommand, executionTask.ExecuteOptions));
        }

        protected override RunExecutionResult Run(ExecutionTask executionTask)
        {
            return new RunExecutionResult(
                SandboxExecutor.Execute($"main.o {executionTask.GetExecuteArguments()}", executionTask.ExecuteOptions));
        }
    }
}