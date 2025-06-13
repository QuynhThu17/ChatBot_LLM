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
                .OrderBy(ch => ch.Timestamp)
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
                    Title = g.Where(ch => ch.Role == "user")
                             .OrderBy(ch => ch.Timestamp)
                             .Select(ch => ch.Content)
                             .FirstOrDefault() != null
                        ? (g.Where(ch => ch.Role == "user")
                            .OrderBy(ch => ch.Timestamp)
                            .Select(ch => ch.Content)
                            .FirstOrDefault().Length > 50
                            ? g.Where(ch => ch.Role == "user")
                               .OrderBy(ch => ch.Timestamp)
                               .Select(ch => ch.Content)
                               .FirstOrDefault().Substring(0, 50) + "..."
                            : g.Where(ch => ch.Role == "user")
                               .OrderBy(ch => ch.Timestamp)
                               .Select(ch => ch.Content)
                               .FirstOrDefault())
                        : "Cuộc trò chuyện chưa có tiêu đề",
                    LastMessageTime = g.Max(ch => ch.Timestamp)
                })
                .OrderByDescending(s => s.LastMessageTime)
                .ToListAsync();
            return sessions;
        }

        public async Task UpdateSessionTitleAsync(string sessionId, string title)
        {
            using var context = _contextFactory.CreateDbContext();
            var sessionMessages = await context.ChatHistories
                .Where(ch => ch.SessionId == sessionId)
                .ToListAsync();

            if (sessionMessages.Any())
            {
                await context.SaveChangesAsync();
            }
        }
    }

    public class ChatSessionSummary
    {
        public string SessionId { get; set; }
        public string Title { get; set; }
        public DateTime LastMessageTime { get; set; }
    }
}