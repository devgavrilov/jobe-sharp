namespace JobeSharp.Languages.Abstract
{
    public interface ICompiled
    {
        string CompiledFileName { get; }
        
        string GetCompilationCommandBySourceCode(string sourceFilePath, string compiledFilePath);
    }
}