using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using EduSync.API.Services.Interfaces;

namespace EduSync.API.Services
{
    public class AIModelService : IAIModelService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly int _maxTokens;
        private readonly double _temperature;

        public AIModelService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = configuration["AIModel:ApiKey"] ?? throw new ArgumentNullException("AIModel:ApiKey");
            _model = configuration["AIModel:Model"] ?? "gpt-4";
            _maxTokens = configuration.GetValue<int>("AIModel:MaxTokens", 2000);
            _temperature = configuration.GetValue<double>("AIModel:Temperature", 0.7);

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                max_tokens = _maxTokens,
                temperature = _temperature
            };

            var response = await SendRequestAsync(requestBody);
            return ExtractResponseContent(response);
        }

        public async Task<string> AnalyzeTextAsync(string text)
        {
            var prompt = $"Analyze the following text and provide insights:\n\n{text}";
            return await GenerateResponseAsync(prompt);
        }

        public async Task<string> GenerateFeedbackAsync(string assessment, string studentResponse)
        {
            var prompt = $"Given the assessment criteria:\n{assessment}\n\nAnd the student's response:\n{studentResponse}\n\nProvide constructive feedback:";
            return await GenerateResponseAsync(prompt);
        }

        public async Task<string> SummarizeTextAsync(string text)
        {
            var prompt = $"Summarize the following text concisely:\n\n{text}";
            return await GenerateResponseAsync(prompt);
        }

        private async Task<string> SendRequestAsync(object requestBody)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private string ExtractResponseContent(string responseJson)
        {
            using var document = JsonDocument.Parse(responseJson);
            var root = document.RootElement;
            var choices = root.GetProperty("choices");
            var firstChoice = choices[0];
            var message = firstChoice.GetProperty("message");
            return message.GetProperty("content").GetString() ?? string.Empty;
        }
    }
} 