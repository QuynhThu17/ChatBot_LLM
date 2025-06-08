namespace ChatBot_LLM.Domain.Models
{
    public class ChatMessage
    {
        public string Role { get; set; } = "user"; // hoặc "assistant"
        public string Content { get; set; } = string.Empty;
    }
}
