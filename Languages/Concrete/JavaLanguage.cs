using System;
using System.Text.RegularExpressions;
using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Versions;
using JobeSharp.Sandbox;

namespace JobeSharp.Languages.Concrete
{
    internal class JavaLanguage : LanguageBase, ICompiled
    {
        public override string Name => "java";
        
        protected override IVersionProvider VersionProvider => 
            new CommandRegexVersionProvider("java -version", new Regex("version \"([\\d.]+)\""));

        private Regex ClassNameParseRegex { get; } = new Regex(
            "(^|\\W)public\\s+class\\s+(\\w+)[^{]*\\{.*?(public\\s+static|static\\s+public)\\s+void\\s+main\\s*\\(\\s*String",
            RegexOptions.Multiline | RegexOptions.Singleline);

        public override string GetCorrectSourceFileName(ExecutionTask task)
        {
            return $"{TryParseMainClassNameFromSource(task)}.java";
        }

        private string TryParseMainClassNameFromSource(ExecutionTask task)
        {
            var result = ClassNameParseRegex.Match(task.SourceCode);

            return result.Success ? result.Groups[2].Value : task.SourceFileName.Replace(".java", "");
        }

        public string GetCompilationCommand(ExecutionTask task, string linkArguments, string compileArguments)
        {
            return $"javac {compileArguments} {task.SourceFileName}";
        }

        public override void CorrectExecutionOptions(ExecuteOptions executeOptions)
        {
            executeOptions.NumberOfProcesses = Math.Max(executeOptions.NumberOfProcesses, 256);
            executeOptions.TotalMemoryKb = 0;
        }

        public string GetRunCommand(ExecutionTask task, string executeArguments)
        {
            if (string.IsNullOrWhiteSpace(executeArguments))
            {
                executeArguments = "-Xrs -Xss8m -Xmx200m";
            }
            
            return $"java {task.SourceFileName.Replace(".java", "")} {executeArguments}";
        }
    }
}