using ChatBot_LLM.Domain.Models;
using ChatBot_LLM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // Added for List<T>
using System.Threading.Tasks;     // Added for Task

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
            // 1. Add input validation for sessionId
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                // You can choose to:
                // a) Return an empty list (safe default)
                // Console.WriteLine("Warning: GetBySessionIdAsync called with null or empty sessionId.");
                return new List<ChatHistory>();
                // b) Throw an ArgumentNullException if sessionId is strictly required
                // throw new ArgumentNullException(nameof(sessionId), "SessionId cannot be null or empty.");
            }

            try
            {
                return await _context.ChatHistories
                    .Where(h => h.SessionId == sessionId)
                    .OrderBy(h => h.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex) // Catch general exceptions during DB access
            {
                // Log the exception details (e.g., using ILogger)
                Console.WriteLine($"Error retrieving chat history for SessionId '{sessionId}': {ex.Message}");
                // Depending on your error handling strategy, you might:
                // a) Re-throw the exception (if the caller is expected to handle it)
                throw;
                // b) Return an empty list (to allow the UI to continue without crashing)
                // return new List<ChatHistory>();
            }
        }

        public async Task AddAsync(ChatHistory message)
        {
            // 2. Add input validation for the message object itself
            if (message == null)
            {
                // Console.WriteLine("Warning: AddAsync called with a null ChatHistory object.");
                return; // Silently exit if message is null, or throw ArgumentNullException
            }
            // Optional: Basic validation for essential properties of ChatHistory
            if (string.IsNullOrWhiteSpace(message.SessionId) || string.IsNullOrWhiteSpace(message.Content))
            {
                Console.WriteLine("Warning: ChatHistory object has missing essential properties (SessionId or Content).");
                // throw new ArgumentException("ChatHistory object must have SessionId and Content.");
                return;
            }

            try
            {
                _context.ChatHistories.Add(message);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx) // Catch specific Entity Framework update exceptions
            {
                Console.WriteLine($"Database update error adding chat history: {dbEx.Message}");
                // Inspect dbEx.InnerException for more specific details (e.g., SQL Server errors)
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");
                }
                throw; // Re-throw to inform the caller about the failure
            }
            catch (Exception ex) // Catch any other unexpected exceptions
            {
                Console.WriteLine($"Generic error adding chat history: {ex.Message}");
                throw;
            }
        }

        // OPTIONAL: Add an UpdateAsync method if you plan to update existing ChatHistory records (e.g., adding a title)
        public async Task UpdateAsync(ChatHistory message)
        {
            if (message == null)
            {
                // Console.WriteLine("Warning: UpdateAsync called with a null ChatHistory object.");
                return;
            }

            try
            {
                // Ensure the entity is being tracked or attach it
                _context.ChatHistories.Update(message);
                // Or if it's not being tracked:
                // _context.Entry(message).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database update error updating chat history: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Generic error updating chat history: {ex.Message}");
                throw;
            }
        }
    }
}