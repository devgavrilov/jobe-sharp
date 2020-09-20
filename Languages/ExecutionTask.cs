using System.Collections.Generic;
using JobeSharp.Languages.Abstract;
using JobeSharp.Sandbox;

namespace JobeSharp.Languages
{
    public class ExecutionTask
    {
        public ILanguage Language { get; set; }
        public string SourceCode { get; set; }
        public string SourceFileName { get; set; }
        public string Input { get; set; }
        public Dictionary<string, string> CachedFilesIdPath { get; set; }
        public bool Debug { get; set; }
        
        public string[] LinkArguments { get; set; }
        public string[] CompileArguments { get; set; }
        public string[] ExecuteArguments { get; set; }
        
        public ExecuteOptions ExecuteOptions { get; set; } = new ExecuteOptions
        {
            FileSizeKb = 20 * 1024,
            StreamSizeKb = 2 * 1024,
            CpuTimeSeconds = 5,
            TotalMemoryKb = 200 * 1024,
            NumberOfProcesses = 20,
        };
    }
}