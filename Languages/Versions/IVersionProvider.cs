namespace JobeSharp.Languages.Versions
{
    internal interface IVersionProvider
    {
        public string GetVersion();
        public bool CheckAnyVersionExistence();
    }
}