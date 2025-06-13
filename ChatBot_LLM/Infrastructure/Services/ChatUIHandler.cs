using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBot_LLM.Infrastructure.Services;
using ChatBot_LLM.Domain.Models;
using Microsoft.AspNetCore.Http;

public class ChatUIHandler
{
    private readonly ChatManager _chatManager;
    private Guid _currentSessionId;
    private readonly ChatHistoryService _chatHistoryService;

    public ChatUIHandler(ChatManager chatManager, ChatHistoryService chatHistoryService)
    {
        _chatManager = chatManager;
        _chatHistoryService = chatHistoryService;
        _currentSessionId = Guid.NewGuid();
        _chatManager.StartNewChat();
    }

    public async Task StartNewChat()
    {
        // Lưu lịch sử phiên hiện tại
        var currentMessages = _chatManager.GetCurrentChat(_currentSessionId);
        foreach (var message in currentMessages)
        {
            await _chatHistoryService.AddAsync(new ChatHistory
            {
                SessionId = _currentSessionId.ToString(),
                Role = currentMessages.IndexOf(message) % 2 == 0 ? "user" : "assistant",
                Content = message,
                Timestamp = DateTime.Now
            });
        }

        // Xóa và tạo phiên mới
        _chatManager.ClearCurrentChat(_currentSessionId);
        _currentSessionId = Guid.NewGuid();
        _chatManager.StartNewChat();

        // Thêm tin nhắn chào mừng
        var welcomeMessage = new ChatHistory
        {
            SessionId = _currentSessionId.ToString(),
            Role = "assistant",
            Content = "Chào bạn! Tôi là trợ lý AI. Hãy hỏi tôi bất cứ điều gì.",
            Timestamp = DateTime.Now
        };
        await _chatHistoryService.AddAsync(welcomeMessage);
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

    public List<string> GetCurrentChat()
    {
        return _chatManager.GetCurrentChat(_currentSessionId);
    }

    public string GetCurrentSessionId()
    {
        return _currentSessionId.ToString();
    }
}