namespace ChatBot_LLM.Domain.Models
{
    public class ChatMessage
    {
        public string Role { get; set; } = "user"; // hoặc "assistant"

        public int Id { get; set; }
        public int ChatHistoryId { get; set; }
        public string Sender { get; set; } = ""; // "User" hoặc "Bot"
        public string Content { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
