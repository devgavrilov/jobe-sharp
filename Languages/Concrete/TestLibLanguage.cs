using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Versions;
using JobeSharp.Sandbox;

namespace JobeSharp.Languages.Concrete
{
    internal class TestLibInput
    {
        [JsonPropertyName("input")]
        public string Input { get; set; }

        [JsonPropertyName("output")]
        public string Output { get; set; }
        
        [JsonPropertyName("answer")]
        public string Answer { get; set; }
    }
    
    internal class TestLibResult
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }
        
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
    
    internal class TestLibLanguage : CompiledLanguage
    {
        public override string Name => "testlib";
        
        protected override IVersionProvider VersionProvider => 
            new CommandRegexVersionProvider("gcc -v", new Regex("gcc version ([\\d.]+)"));

        protected override CompileExecutionResult Compile(ExecutionTask executionTask)
        {
            var compileCommand = $"g++ {executionTask.GetCompileArguments("-Wall -Werror -x c++")} -o checker.o {GetSourceFilePath(executionTask)} {executionTask.GetLinkArguments()}";
            
            return new CompileExecutionResult(
                SandboxExecutor.Execute(compileCommand, executionTask.ExecuteOptions));
        }

        protected override RunExecutionResult Run(ExecutionTask executionTask)
        {
            var input = JsonSerializer.Deserialize<TestLibInput>(executionTask.Input);
            
            File.WriteAllText(Path.Combine(executionTask.WorkTempDirectory, "prog.in"), input.Input);
            File.WriteAllText(Path.Combine(executionTask.WorkTempDirectory, "prog.out"), input.Output);
            File.WriteAllText(Path.Combine(executionTask.WorkTempDirectory, "prog.ans"), input.Answer);

            var runResult = SandboxExecutor.Execute($"checker.o {executionTask.GetExecuteArguments()} prog.in prog.out prog.ans prog.res",
                executionTask.ExecuteOptions);

            var result = new TestLibResult
            {
                Code = runResult.ExitCode,
                Message = File.ReadAllText(Path.Combine(executionTask.WorkTempDirectory, "prog.res"))
            };
            
            return new RunExecutionResult(exitCode: 0, output: JsonSerializer.Serialize(result), error: "");
        }
    }
}