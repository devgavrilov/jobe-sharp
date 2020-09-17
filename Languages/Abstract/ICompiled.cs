namespace JobeSharp.Languages.Abstract
{
    internal interface ICompiled
    {
        string GetCompilationCommand(ExecutionTask task);

        string GetRunCommand(ExecutionTask task);
    }
}