using JobeSharp.Sandbox;

namespace JobeSharp.Languages.Abstract
{
    public interface ILanguage
    {
        string Name { get; }
        string Version { get; }
        bool IsInstalled { get; }

        string GetCorrectSourceFileName(ExecutionTask task);
        void CorrectExecutionOptions(ExecuteOptions executeOptions);
    }
}