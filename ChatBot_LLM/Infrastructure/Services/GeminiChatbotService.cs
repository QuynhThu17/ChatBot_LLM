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
            // Hiển thị thông báo "Đang suy nghĩ..." trong khi bot xử lý câu trả lời
            Console.WriteLine("Đang suy nghĩ...");

            // Tìm kiếm ngữ cảnh phù hợp từ hệ thống FAQ
            var matchedContext = await _faqService.FindContextAsync(question);
            var fullContext = !string.IsNullOrWhiteSpace(matchedContext) ? matchedContext : context;

            // Tạo prompt cho API Gemini
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

            // Gửi yêu cầu đến API Gemini
            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);

            // Kiểm tra lỗi từ Gemini API
            if (doc.RootElement.TryGetProperty("error", out var error))
            {
                var message = error.TryGetProperty("message", out var msg) ? msg.GetString() : "Không rõ lỗi";
                return $"❗ Lỗi từ Gemini API: {message}";
            }

            // Lấy câu trả lời từ API Gemini và định dạng lại
            if (doc.RootElement.TryGetProperty("candidates", out var candidates))
            {
                try
                {
                    var answer = candidates[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    // Định dạng câu trả lời với các dòng mới cho dễ đọc
                    return FormatAnswer(answer);
                }
                catch
                {
                    return "⚠️ Định dạng phản hồi không đúng.";
                }
            }

            return "⚠️ Không nhận được phản hồi hợp lệ từ API Gemini.";
        }

        // Định dạng câu trả lời với các dòng mới, sau mỗi dấu chấm
        private string FormatAnswer(string answer)
        {
            return answer?.Replace(". ", ".\n") ?? "⚠️ Không có nội dung trả lời.";
        }
    }
}
