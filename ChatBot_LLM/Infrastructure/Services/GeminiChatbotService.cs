using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatBot_LLM.Interfaces;
using ChatBot_LLM.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace ChatBot_LLM.Infrastructure.Services
{
    public class GeminiChatbotService : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly FAQService _faqService;

        public GeminiChatbotService(HttpClient httpClient, IConfiguration configuration, FAQService faqService)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"]!;
            _faqService = faqService;
        }

        public async Task<string> GetAnswerAsync(string question, string context = "")
        {
            var matchedContext = await _faqService.FindContextAsync(question);

            var fullContext = !string.IsNullOrWhiteSpace(matchedContext) ? matchedContext : context;

            var prompt = $"Bạn là trợ lý ảo. Dựa vào thông tin sau: {fullContext}. Hãy trả lời câu hỏi sau một cách thân thiện: {question}";

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);

            if (doc.RootElement.TryGetProperty("error", out var error))
            {
                var message = error.TryGetProperty("message", out var msg) ? msg.GetString() : "Không rõ lỗi";
                return $"❗ Lỗi từ Gemini API: {message}";
            }

            if (doc.RootElement.TryGetProperty("candidates", out var candidates))
            {
                try
                {
                    return candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString()
                           ?? "⚠️ Không có nội dung trả lời.";
                }
                catch
                {
                    return "⚠️ Định dạng phản hồi không đúng.";
                }
            }

            return "⚠️ Không nhận được phản hồi hợp lệ từ API Gemini.";
        }
    }
}
