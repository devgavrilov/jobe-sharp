using System;
using System.IO;
using JobeSharp.Languages.Versions;

namespace JobeSharp.Languages.Abstract
{
    internal abstract class LanguageBase : ILanguage
    {
        public abstract string Name { get; }

        public Lazy<string> Version { get; }

        public Lazy<bool> IsInstalled { get; }

        protected abstract IVersionProvider VersionProvider { get; }

        protected LanguageBase()
        {
            Version = new Lazy<string>(() => VersionProvider.GetVersion());
            IsInstalled = new Lazy<bool>(() => VersionProvider.CheckAnyVersionExistence());
        }

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