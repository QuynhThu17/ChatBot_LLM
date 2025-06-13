using ChatBot_LLM.Domain.Models;
using ChatBot_LLM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBot_LLM.Infrastructure.Services
{
    public class ChatHistoryService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public ChatHistoryService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ChatHistory>> GetBySessionIdAsync(string sessionId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.ChatHistories
                .Where(ch => ch.SessionId == sessionId)
                .ToListAsync();
        }

        public async Task AddAsync(ChatHistory chatHistory)
        {
            using var context = _contextFactory.CreateDbContext();
            context.ChatHistories.Add(chatHistory);
            await context.SaveChangesAsync();
        }

        public async Task DeleteBySessionIdAsync(string sessionId)
        {
            using var context = _contextFactory.CreateDbContext();
            var histories = await context.ChatHistories
                .Where(ch => ch.SessionId == sessionId)
                .ToListAsync();
            context.ChatHistories.RemoveRange(histories);
            await context.SaveChangesAsync();
        }

        public async Task<List<ChatSessionSummary>> GetChatSessionsAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            var sessions = await context.ChatHistories
                .GroupBy(ch => ch.SessionId)
                .Select(g => new ChatSessionSummary
                {
                    SessionId = g.Key,
                    Title = g.OrderBy(ch => ch.Timestamp).First().Content.Length > 50
                        ? g.OrderBy(ch => ch.Timestamp).First().Content.Substring(0, 50) + "..."
                        : g.OrderBy(ch => ch.Timestamp).First().Content,
                    LastMessageTime = g.Max(ch => ch.Timestamp)
                })
                .OrderByDescending(s => s.LastMessageTime)
                .ToListAsync();
            return sessions;
        }
    }

    public class ChatSessionSummary
    {
        public string SessionId { get; set; }
        public string Title { get; set; }
        public DateTime LastMessageTime { get; set; }
    }
}