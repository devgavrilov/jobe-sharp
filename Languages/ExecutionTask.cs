using System.Collections.Generic;
using JobeSharp.Languages.Abstract;

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
    }
}