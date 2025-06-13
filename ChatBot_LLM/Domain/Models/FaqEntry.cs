using ChatBot_LLM.Domain.Models;
using ChatBot_LLM.Infrastructure.Services;

namespace ChatBot_LLM.Domain.Models
{
    public class FAQEntry
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }

}