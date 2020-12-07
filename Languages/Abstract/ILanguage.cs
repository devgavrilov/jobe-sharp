using System;

namespace JobeSharp.Languages.Abstract
{
    public interface ILanguage
    {
        string Name { get; }
        Lazy<string> Version { get; }
        Lazy<bool> IsInstalled { get; }

        ExecutionResult Execute(ExecutionTask executionTask);
    }
}