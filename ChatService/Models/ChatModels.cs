using System.Text.Json.Serialization;

namespace ChatService.Models
{
    public class ChatModel
    {
        public string Question { get; set; } = string.Empty;
    }

    public class ChatResponse
    {
        public string Answer { get; set; }
        [JsonPropertyName("text_time")]
        public DateTime TextTime { get; set; } = DateTime.Now;
    }
}
