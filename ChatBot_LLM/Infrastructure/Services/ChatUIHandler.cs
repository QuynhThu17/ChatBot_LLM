using ChatBot_LLM.Domain.Models;
using ChatBot_LLM.Infrastructure.Services;

public class ChatUIHandler
{
    private readonly ChatManager _chatManager;
    private Guid _currentSessionId;
    private readonly ChatHistoryService _chatHistoryService;

    public ChatUIHandler(ChatManager chatManager, ChatHistoryService chatHistoryService)
    {
        _chatManager = chatManager;
        _chatHistoryService = chatHistoryService;
        _currentSessionId = Guid.NewGuid(); // Tạo SessionId mới khi khởi tạo
    }

    public async Task StartNewChat()
    {
        // Không lưu lịch sử ngay, chỉ tạo SessionId mới
        _currentSessionId = Guid.NewGuid();
        _chatManager.ClearCurrentChat(_currentSessionId);
        _chatManager.StartNewChat(); // Chỉ khởi tạo trong ChatManager (nếu cần)
    }

    public async Task AddMessage(string message, string role = "user")
    {
        _chatManager.AddMessage(_currentSessionId, message);
        await _chatHistoryService.AddAsync(new ChatHistory
        {
            SessionId = _currentSessionId.ToString(),
            Role = role,
            Content = message,
            Timestamp = DateTime.Now
        });
    }

    public string GetCurrentSessionId()
    {
        return _currentSessionId.ToString();
    }
}