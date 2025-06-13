using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

public class ChatUIHandler
{
    private ChatManager _chatManager;
    private Guid _currentSessionId;
    private IHttpContextAccessor _httpContextAccessor;

    public ChatUIHandler(ChatManager chatManager, IHttpContextAccessor httpContextAccessor)
    {
        _chatManager = chatManager;
        _httpContextAccessor = httpContextAccessor;
        _currentSessionId = Guid.NewGuid();
        _chatManager.StartNewChat();
    }

    public void StartNewChat()
    {
        _chatManager.ClearCurrentChat(_currentSessionId);
        _currentSessionId = Guid.NewGuid();
        _chatManager.StartNewChat();
    }

    public void AddMessage(string message)
    {
        _chatManager.AddMessage(_currentSessionId, message);
    }

    public List<string> GetCurrentChat()
    {
        return _chatManager.GetCurrentChat(_currentSessionId);
    }
}