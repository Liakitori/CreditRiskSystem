using CreditRiskSystem.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CreditRiskSystem.Client.Services
{
    public interface IApiService
    {
        Task<string> LoginAsync(string username, string password);
        Task RegisterAsync(string username, string password);
        Task<RiskAssessmentResult> UploadFileAsync(string filePath);
        Task<Stream> DownloadPdfAsync(Guid id);
        Task<Stream> DownloadJsonAsync(Guid id);
        Task<List<RiskAssessmentResult>> GetHistoryAsync();
        void SetToken(string token);
    }
}