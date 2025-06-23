using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatBot_LLM.Infrastructure.Services
{
    public class EmbeddingRetrievalService
    {
        private readonly HttpClient _httpClient;
        private readonly string _backendUrl = "http://127.0.0.1:8000"; // Địa chỉ Python API backend

        public EmbeddingRetrievalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<RetrievedFaq>> GetRelevantFaqsAsync(string question, int topK = 5)
        {
            var requestBody = new
            {
                question = question,
                top_k = topK
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_backendUrl}/retrieve", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<RetrieveResponse>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.Results ?? new List<RetrievedFaq>();
        }

        public class RetrieveResponse
        {
            public List<RetrievedFaq> Results { get; set; }
        }

        public class RetrievedFaq
        {
            public int Id { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
            public double Similarity { get; set; }
        }
    }
}
