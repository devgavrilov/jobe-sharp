namespace JobeSharp.Languages.Abstract
{
    internal interface ICompiled
    {
        string GetCompilationCommand(ExecutionTask task, string linkArguments, string compileArguments);

        string GetRunCommand(ExecutionTask task, string executeArguments);
    }
}