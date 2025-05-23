using CreditRiskSystem.Common.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CreditRiskSystem.Client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly HttpClient _httpClient;

        [Reactive] public double WorkingCapital { get; set; }
        [Reactive] public double TotalAssets { get; set; }
        [Reactive] public double RetainedEarnings { get; set; }
        [Reactive] public double EBIT { get; set; }
        [Reactive] public double MarketValueOfEquity { get; set; }
        [Reactive] public double TotalLiabilities { get; set; }
        [Reactive] public double Revenue { get; set; }
        [Reactive] public string Result { get; set; }

        public ReactiveCommand<Unit, Unit> AssessRiskCommand { get; }
        public MainWindowViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            //Debug.WriteLine($"MainWindowViewModel initialized with BaseAddress: {_httpClient.BaseAddress}"); // Диагностика

            // Настраиваем команду для расчета риска
            AssessRiskCommand = ReactiveCommand.CreateFromTask(AssessRiskAsync);
        }

        private async Task AssessRiskAsync()
        {
            try
            {
                //Debug.WriteLine($"Sending request to: {_httpClient.BaseAddress?.ToString() ?? "null"}api/FinancialData"); // Диагностика
                var data = new FinancialData
                {
                    WorkingCapital = WorkingCapital,
                    TotalAssets = TotalAssets,
                    RetainedEarnings = RetainedEarnings,
                    EBIT = EBIT,
                    MarketValueOfEquity = MarketValueOfEquity,
                    TotalLiabilities = TotalLiabilities,
                    Revenue = Revenue
                };

                var response = await _httpClient.PostAsJsonAsync("api/FinancialData", data);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RiskAssessmentResult>();
                    Result = $"Altman Z-score: {result.AltmanZScore:F2}\n" +
                             $"Уровень риска: {result.RiskLevel}\n" +
                             $"Рекомендации: {result.Recommendations}";
                    //Debug.WriteLine($"Request successful: {Result}"); // Диагностика
                }
                else
                {
                    Result = $"Ошибка: {response.ReasonPhrase}";
                    //Debug.WriteLine($"Request failed: {response.ReasonPhrase}"); // Диагностика
                }
            }
            catch (Exception ex)
            {
                Result = $"Ошибка: {ex.Message}";
                //Debug.WriteLine($"Exception: {ex}"); // Диагностика
            }
        }

    }
}
