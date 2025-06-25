namespace ChatBot_LLM.Infrastructure.Services
{
    public class UserSessionService
    {
        public string? Username { get; private set; }
        public string? Role { get; private set; }

        public void SetUser(string username, string role)
        {
            Username = username;
            Role = role;
        }

        public void Clear()
        {
            Username = null;
            Role = null;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(Username);
    }

}
