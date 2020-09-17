using System.IO;
using System.Text.RegularExpressions;
using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Versions;

namespace JobeSharp.Languages.Concrete
{
    internal class CLanguage : LanguageBase, ICompiled
    {
        public override string Name => "c";
        
        protected override IVersionProvider VersionProvider => 
            new CommandRegexVersionProvider("gcc -v", new Regex("gcc version ([\\d.]+)"));

        public string GetCompilationCommand(ExecutionTask task)
        {
            return $"gcc -Wall -Werror -std=c99 -x c -o {GetCompiledFilePath(task)} {task.SourceFileName}";
        }

        public string GetRunCommand(ExecutionTask task)
        {
            return GetCompiledFilePath(task);
        }

        private string GetCompiledFilePath(ExecutionTask task)
        {
            return Path.Combine("main.o");
        }
    }
}