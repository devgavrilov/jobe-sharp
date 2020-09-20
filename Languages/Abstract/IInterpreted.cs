namespace JobeSharp.Languages.Abstract
{
    public interface IInterpreted
    {
        string GetRunnableCommandOfScript(string scriptFilePath, string executeArguments);
    }
}