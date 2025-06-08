using ChatBot_LLM.Domain.Models;

namespace ChatBot_LLM.Interfaces 
{
    public interface IChatbotService
    {
        Task<string> GetAnswerAsync(string question, string context = "");
    }
}
    