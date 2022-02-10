using Newtonsoft.Json;

namespace JobeSharp.DTO
{
    public class ResultDto
    {
        [JsonProperty("run_id")]
        public string RunId { get; set; }
        
        [JsonProperty("outcome")]
        public long Outcome { get; set; }
        
        [JsonProperty("cmpinfo")]
        public string CmpInfo { get; set; }
        
        [JsonProperty("stdout")]
        public string StdOut { get; set; }
        
        [JsonProperty("stderr")]
        public string StdErr { get; set; }
    }
}