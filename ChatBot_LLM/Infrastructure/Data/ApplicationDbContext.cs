using System.Collections.Generic;
using ChatBot_LLM.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatBot_LLM.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FaqEntry> FAQs => Set<FaqEntry>();
        public DbSet<ChatHistory> ChatHistories => Set<ChatHistory>();
        public DbSet<User> Users { get; set; }


    }
}
