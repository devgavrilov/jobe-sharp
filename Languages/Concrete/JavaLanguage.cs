using System.Text.RegularExpressions;
using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Versions;

namespace JobeSharp.Languages.Concrete
{
    internal class JavaLanguage : LanguageBase
    {
        public override string Name => "java";
        
        protected override IVersionProvider VersionProvider => 
            new CommandRegexVersionProvider("java -version", new Regex("version \"([\\d.]+)\""));
    }
}