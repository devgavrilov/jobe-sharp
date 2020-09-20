using System.Text.RegularExpressions;
using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Versions;

namespace JobeSharp.Languages.Concrete
{
    internal class NodeJsLanguage : LanguageBase, IInterpreted
    {
        public override string Name => "nodejs";

        protected override IVersionProvider VersionProvider => 
            new CommandRegexVersionProvider("nodejs -v", new Regex("v([\\d.]+)"));

        public string GetRunnableCommandOfScript(string scriptFilePath, string executeArguments)
        {
            return $"nodejs {scriptFilePath} {executeArguments}";
        }
    }
}