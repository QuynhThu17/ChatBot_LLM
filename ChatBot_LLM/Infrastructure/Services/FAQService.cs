using ChatBot_LLM.Domain.Models;
using ChatBot_LLM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatBot_LLM.Infrastructure.Services
{
    public class FAQService
    {
        private readonly ApplicationDbContext _context;

        public FAQService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FaqEntry>> GetAllAsync() =>
            await _context.FAQs.ToListAsync();

        public async Task<string?> FindContextAsync(string userQuestion)
        {
            var all = await GetAllAsync();
            return all
                .OrderByDescending(f => Similarity(f.Question, userQuestion))
                .FirstOrDefault()?.Answer;
        }

        private double Similarity(string a, string b)
        {
            var tokensA = a.ToLower().Split(" ");
            var tokensB = b.ToLower().Split(" ");
            return tokensA.Intersect(tokensB).Count();
        }

        public async Task AddAsync(FaqEntry faq)
        {
            _context.FAQs.Add(faq);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FaqEntry faq)
        {
            _context.FAQs.Update(faq);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var faq = await _context.FAQs.FindAsync(id);
            if (faq != null)
            {
                _context.FAQs.Remove(faq);
                await _context.SaveChangesAsync();
            }
        }
    }
}
