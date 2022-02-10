using System.Text.Json.Serialization;

namespace JobeSharp.DTO
{
    public class ResultDto
    {
        [JsonPropertyName("run_id")]
        public string RunId { get; set; }
        
        [JsonPropertyName("outcome")]
        public long Outcome { get; set; }
        
        [JsonPropertyName("cmpinfo")]
        public string CmpInfo { get; set; }
        
        [JsonPropertyName("stdout")]
        public string StdOut { get; set; }
        
        [JsonPropertyName("stderr")]
        public string StdErr { get; set; }
    }
}