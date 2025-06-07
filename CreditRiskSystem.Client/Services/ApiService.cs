using CreditRiskSystem.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CreditRiskSystem.Client.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private string _token;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetToken(string token)
        {
            _token = token;
            _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token)
                ? null
                : new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Username = username, Password = password });
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result.Token;
        }

        public async Task RegisterAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", new { Username = username, Password = password });
            response.EnsureSuccessStatusCode();
        }

        public async Task<RiskAssessmentResult> UploadFileAsync(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(stream), "file", Path.GetFileName(filePath));
            var response = await _httpClient.PostAsync("api/FinancialData/upload", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<RiskAssessmentResult>();
        }

        public async Task<Stream> DownloadPdfAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/FinancialData/download/pdf/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> DownloadJsonAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/FinancialData/download/json/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<List<RiskAssessmentResult>> GetHistoryAsync()
        {
            var response = await _httpClient.GetAsync("api/FinancialData/history");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<RiskAssessmentResult>>();
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
    }
}