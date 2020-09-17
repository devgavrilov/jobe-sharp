using JobeSharp.Languages.Versions;

namespace JobeSharp.Languages.Abstract
{
    internal abstract class LanguageBase : ILanguage
    {
        public abstract string Name { get; }

        public string Version => VersionProvider.GetVersion();

        public bool IsInstalled => VersionProvider.CheckAnyVersionExistence();

        protected abstract IVersionProvider VersionProvider { get; }
    }
}