using System.Text.RegularExpressions;
using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Versions;

namespace JobeSharp.Languages.Concrete
{
    internal class CppLanguage : LanguageBase, ICompiled
    {
        public override string Name => "cpp";
        
        protected override IVersionProvider VersionProvider => 
            new CommandRegexVersionProvider("gcc -v", new Regex("gcc version ([\\d.]+)"));

        public string GetCompilationCommand(ExecutionTask task, string linkArguments, string compileArguments)
        {
            if (string.IsNullOrWhiteSpace(compileArguments))
            {
                compileArguments =  "-Wall -Werror";
            }
            
            return $"g++ {compileArguments} -o main.o {task.SourceFileName} {linkArguments}";
        }

        public string GetRunCommand(ExecutionTask task, string executeArguments)
        {
            return $"main.o {executeArguments}";
        }
    }
}