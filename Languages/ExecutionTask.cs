using System.Collections.Generic;
using JobeSharp.Sandbox;

namespace JobeSharp.Languages
{
    public class ExecutionTask
    {
        public string SourceCode { get; set; }
        public string SourceFileName { get; set; }
        public string Input { get; set; }
        public Dictionary<string, string> CachedFilesIdPath { get; set; }
        public bool Debug { get; set; }
        
        public string[] LinkArguments { get; set; }
        public string[] CompileArguments { get; set; }
        public string[] InterpreterArguments { get; set; }        
        public string[] RunArgs { get; set; }
        
        public string WorkTempDirectory { get; set; }

        public string GetLinkArguments(string defaultArguments = "")
        {
            return string.Join(" ", LinkArguments ?? new [] { defaultArguments });
        }
        
        public string GetCompileArguments(string defaultArguments = "")
        {
            return string.Join(" ", CompileArguments ?? new [] { defaultArguments });
        }
        
        public string GetInterpreterArguments(string defaultArguments = "")
        {
            return string.Join(" ", InterpreterArguments ?? new [] { defaultArguments });
        }
        
        public string GetRunArguments(string defaultArguments = "")
        {
            return string.Join(" ", RunArgs ?? new [] { defaultArguments });
        }
        
        public ExecuteOptions ExecuteOptions { get; } = new ExecuteOptions
        {
            FileSizeKb = 20 * 1024,
            StreamSizeKb = 2 * 1024,
            CpuTimeSeconds = 5,
            TotalMemoryKb = 200 * 1024,
            NumberOfProcesses = 20,
        };
    }
}