using System.Text.Json.Serialization;

namespace JobeSharp.DTO
{
    public class FileDto
    {
        [JsonPropertyName("file_contents")]
        public string FileContents { get; set; }
    }
}