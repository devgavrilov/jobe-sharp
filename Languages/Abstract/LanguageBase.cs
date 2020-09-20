using JobeSharp.Languages.Versions;
using JobeSharp.Sandbox;

namespace JobeSharp.Languages.Abstract
{
    internal abstract class LanguageBase : ILanguage
    {
        public abstract string Name { get; }

        public string Version => VersionProvider.GetVersion();

        public bool IsInstalled => VersionProvider.CheckAnyVersionExistence();

        protected abstract IVersionProvider VersionProvider { get; }

        public virtual string GetCorrectSourceFileName(ExecutionTask task)
        {
            return task.SourceFileName;
        }

        public virtual void CorrectExecutionOptions(ExecuteOptions executeOptions)
        {
        }
    }
}