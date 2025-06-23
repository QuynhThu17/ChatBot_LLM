using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatBot_LLM.Interfaces;
using ChatBot_LLM.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace ChatBot_LLM.Infrastructure.Services
{
    public class GeminiChatbotService : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly EmbeddingRetrievalService _embeddingService;
        private readonly ChatHistoryService _chatHistoryService;

        public GeminiChatbotService(HttpClient httpClient, IConfiguration configuration, EmbeddingRetrievalService embeddingService, ChatHistoryService chatHistoryService)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"]!;
            _embeddingService = embeddingService;
            _chatHistoryService = chatHistoryService;
        }

        public async Task<string> GetAnswerAsync(string question, string context = "")
        {
            // Lấy top 5 câu hỏi tương tự từ RAG (Python API)
            var retrievedFaqs = await _embeddingService.GetRelevantFaqsAsync(question);

            string knowledgeBase = string.Join("\n\n", retrievedFaqs.Select(f =>
                $"Câu hỏi: {f.Question}\nTrả lời: {f.Answer}"));

            // Lấy lịch sử chat gần nhất từ ChatHistory (ví dụ lấy 5 tin nhắn cuối)
            var sessionId = context; // context bây giờ dùng tạm để truyền sessionId vào
            var chatHistory = await _chatHistoryService.GetBySessionIdAsync(sessionId);

            var recentMessages = chatHistory
                .OrderByDescending(ch => ch.Timestamp)
                .Take(10)
                .OrderBy(ch => ch.Timestamp) // đảo lại đúng thứ tự thời gian
                .Select(ch => $"{ch.Role.ToUpper()}: {ch.Content}");

            string chatMemory = string.Join("\n", recentMessages);

            // Tổng hợp prompt cuối cùng
            var prompt = $@"Bạn là trợ lý ảo hỗ trợ khách hàng. 
            Dưới đây là dữ liệu tri thức nội bộ:
            {knowledgeBase}

            Đây là lịch sử hội thoại gần đây với khách hàng:
            {chatMemory}

            Bây giờ, hãy trả lời câu hỏi mới nhất một cách chính xác, thân thiện:
            {question}";

            // Gọi Gemini như cũ
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
