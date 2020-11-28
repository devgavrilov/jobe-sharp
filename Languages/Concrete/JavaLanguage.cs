using System;
using System.IO;
using System.Text.RegularExpressions;
using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Versions;
using JobeSharp.Sandbox;

namespace JobeSharp.Languages.Concrete
{
    internal class JavaLanguage : CompiledLanguage
    {
        public override string Name => "java";
        
        protected override IVersionProvider VersionProvider => 
            new CommandRegexVersionProvider("java -version", new Regex("version \"([\\d.]+)\""));

        private Regex ClassNameParseRegex { get; } = new Regex(
            "(^|\\W)public\\s+class\\s+(\\w+)[^{]*\\{.*?(public\\s+static|static\\s+public)\\s+void\\s+main\\s*\\(\\s*String",
            RegexOptions.Multiline | RegexOptions.Singleline);

        protected override string GetSourceFilePath(ExecutionTask executionTask)
        {
            return Path.Combine(executionTask.WorkTempDirectory, $"{TryParseMainClassNameFromSource(executionTask)}.java");
        }

        private string TryParseMainClassNameFromSource(ExecutionTask executionTask)
        {
            var result = ClassNameParseRegex.Match(executionTask.SourceCode);

            return result.Success ? result.Groups[2].Value : executionTask.SourceFileName.Replace(".java", "");
        }

        protected override CompileExecutionResult Compile(ExecutionTask executionTask)
        {
            executionTask.ExecuteOptions.NumberOfProcesses = Math.Max(executionTask.ExecuteOptions.NumberOfProcesses, 256);
            executionTask.ExecuteOptions.TotalMemoryKb = 0;
            
            var compileCommand = $"javac {executionTask.GetCompileArguments()} {GetSourceFilePath(executionTask)}";
            
            return new CompileExecutionResult(
                SandboxExecutor.Execute(compileCommand, executionTask.ExecuteOptions));
        }

        protected override RunExecutionResult Run(ExecutionTask executionTask)
        {
            executionTask.ExecuteOptions.StdIn = executionTask.Input;
            executionTask.ExecuteOptions.NumberOfProcesses = Math.Max(executionTask.ExecuteOptions.NumberOfProcesses, 256);
            executionTask.ExecuteOptions.TotalMemoryKb = 0;

            var compiledFilePath = Path.GetFileName(GetSourceFilePath(executionTask).Replace(".java", ""));
            
            return new RunExecutionResult(
                SandboxExecutor.Execute($"java {executionTask.GetInterpreterArguments("-Xrs -Xss8m -Xmx200m")} {compiledFilePath} {executionTask.GetRunArguments()}", executionTask.ExecuteOptions));
        }
    }
}