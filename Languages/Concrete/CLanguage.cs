using System.Text.RegularExpressions;
using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Versions;

namespace JobeSharp.Languages.Concrete
{
    internal class CLanguage : LanguageBase, ICompiled
    {
        public override string Name => "c";

        public string CompiledFileName => "main.o";
        
        protected override IVersionProvider VersionProvider => 
            new CommandRegexVersionProvider("gcc -v", new Regex("gcc version ([\\d.]+)"));

        public string GetCompilationCommandBySourceCode(string sourceFilePath, string compiledFilePath)
        {
            return $"gcc -Wall -Werror -std=c99 -x c -o {compiledFilePath} {sourceFilePath}";
        }
    }
}