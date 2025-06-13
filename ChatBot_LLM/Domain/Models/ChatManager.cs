using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ChatManager
{
    private List<ChatSession> _chatHistory;
    private string _savePath = "chat_history.json";

    public ChatManager()
    {
        _chatHistory = LoadChatHistory() ?? new List<ChatSession>();
    }

    public void StartNewChat()
    {
        _chatHistory.Add(new ChatSession { Id = Guid.NewGuid(), Messages = new List<string>(), StartTime = DateTime.Now });
        SaveChatHistory();
    }

    public void AddMessage(Guid sessionId, string message)
    {
        var session = _chatHistory.Find(c => c.Id == sessionId);
        if (session != null)
        {
            session.Messages.Add(message);
            SaveChatHistory();
        }
    }

    public List<string> GetCurrentChat(Guid sessionId)
    {
        var session = _chatHistory.Find(c => c.Id == sessionId);
        return session?.Messages ?? new List<string>();
    }

    public void ClearCurrentChat(Guid sessionId)
    {
        var session = _chatHistory.Find(c => c.Id == sessionId);
        if (session != null)
        {
            session.Messages.Clear();
            SaveChatHistory();
        }
    }

    private List<ChatSession> LoadChatHistory()
    {
        if (File.Exists(_savePath))
        {
            string json = File.ReadAllText(_savePath);
            return JsonSerializer.Deserialize<List<ChatSession>>(json);
        }
        return null;
    }

    private void SaveChatHistory()
    {
        string json = JsonSerializer.Serialize(_chatHistory);
        File.WriteAllText(_savePath, json);
    }
}

public class ChatSession
{
    public Guid Id { get; set; }
    public List<string> Messages { get; set; }
    public DateTime StartTime { get; set; }
}