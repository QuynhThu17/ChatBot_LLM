using ChatBot_LLM.Domain.Models;
using ChatBot_LLM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatBot_LLM.Infrastructure.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> ValidateUser(string username, string password)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }

    }
}
