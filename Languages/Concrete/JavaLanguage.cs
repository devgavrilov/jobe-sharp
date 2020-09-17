using System;
using System.IO;
using System.Text.RegularExpressions;
using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Versions;

namespace JobeSharp.Languages.Concrete
{
    internal class JavaLanguage : LanguageBase, ICompiled
    {
        public override string Name => "java";
        
        protected override IVersionProvider VersionProvider => 
            new CommandRegexVersionProvider("java -version", new Regex("version \"([\\d.]+)\""));

        public string GetCompilationCommand(ExecutionTask task)
        {
            return $"javac {task.SourceFileName}";
        }

        public string GetRunCommand(ExecutionTask task)
        {
            return $"java {task.SourceFileName.Replace(".java", "")}";
        }
    }
}