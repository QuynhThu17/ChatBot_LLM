namespace ChatBot_LLM.Domain.Models
{
    public class ChatHistory
    {
        public int Id { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
