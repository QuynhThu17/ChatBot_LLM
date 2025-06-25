namespace ChatBot_LLM.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Mật khẩu để demo (sau có thể hash)
        public string Role { get; set; } = "User";
    }
}
