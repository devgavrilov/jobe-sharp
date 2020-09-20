using System.IO;
using JobeSharp.Languages.Versions;

namespace JobeSharp.Languages.Abstract
{
    internal abstract class LanguageBase : ILanguage
    {
        public abstract string Name { get; }

        public string Version => VersionProvider.GetVersion();

        public bool IsInstalled => VersionProvider.CheckAnyVersionExistence();

        protected abstract IVersionProvider VersionProvider { get; }

        public virtual ExecutionResult Execute(ExecutionTask executionTask)
        {
            File.WriteAllText(GetSourceFilePath(executionTask), executionTask.SourceCode);
            return new RunExecutionResult(exitCode: 0, output: "", error: "");
        }

        protected virtual string GetSourceFilePath(ExecutionTask executionTask)
        {
            return Path.Combine(executionTask.WorkTempDirectory, executionTask.SourceFileName);
        }
    }
}