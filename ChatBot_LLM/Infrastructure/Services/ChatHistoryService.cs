using ChatBot_LLM.Domain.Models;
using ChatBot_LLM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatBot_LLM.Infrastructure.Services
{
    public class ChatHistoryService
    {
        private readonly ApplicationDbContext _context;

        public ChatHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatHistory>> GetBySessionIdAsync(string sessionId)
        {
            return await _context.ChatHistories
                .Where(h => h.SessionId == sessionId)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();
        }

        public async Task AddAsync(ChatHistory message)
        {
            _context.ChatHistories.Add(message);
            await _context.SaveChangesAsync();
        }
    }
}
