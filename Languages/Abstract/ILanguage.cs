﻿namespace JobeSharp.Languages.Abstract
{
    public interface ILanguage
    {
        string Name { get; }
        string Version { get; }
        bool IsInstalled { get; }

        ExecutionResult Execute(ExecutionTask executionTask);
    }
}