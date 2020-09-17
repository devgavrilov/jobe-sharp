using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JobeSharp.DTO
{
    public class RunDto
    {
        [JsonPropertyName("run_spec")]
        public RunSpecDto RunSpec { get; set; }
    }
        
    public class RunSpecDto
    {
        [JsonPropertyName("language_id")]
        public string LanguageName { get; set; }
        
        [JsonPropertyName("sourcecode")]
        public string SourceCode { get; set; }
        
        [JsonPropertyName("sourcefilename")]
        public string SourceFileName { get; set; }
        
        [JsonPropertyName("input")]
        public string Input { get; set; }
        
        [JsonPropertyName("file_list")]
        public string[][] FileList { get; set; }
        
        [JsonPropertyName("parameters")]
        public Dictionary<string, string> Parameters { get; set; }
        
        [JsonPropertyName("debug")]
        public bool Debug { get; set; }
    }
}