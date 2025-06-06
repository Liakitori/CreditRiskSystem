using Avalonia.Controls;
using CreditRiskSystem.Common.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive;
using System.Threading.Tasks;

namespace CreditRiskSystem.Client.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly HttpClient _httpClient;
        private readonly Window _parentWindow;

        [Reactive] public string Result { get; set; }

        public ReactiveCommand<Unit, Unit> UploadFileCommand { get; }

        public MainWindowViewModel(HttpClient httpClient, Window parentWindow)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _parentWindow = parentWindow ?? throw new ArgumentNullException(nameof(parentWindow));
            UploadFileCommand = ReactiveCommand.CreateFromTask(UploadFileAsync);
        }

        private async Task UploadFileAsync()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите файл Excel",
                Filters = { new FileDialogFilter { Name = "Excel", Extensions = { "xlsx" } } }
            };

            var result = await dialog.ShowAsync(_parentWindow);
            if (result != null && result.Length > 0)
            {
                try
                {
                    var filePath = result[0];
                    using var stream = File.OpenRead(filePath);
                    var content = new MultipartFormDataContent();
                    content.Add(new StreamContent(stream), "file", Path.GetFileName(filePath));

                    var response = await _httpClient.PostAsync("api/FinancialData/upload", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var riskResult = await response.Content.ReadFromJsonAsync<RiskAssessmentResult>();
                        Result = $"Altman Z-score: {riskResult.AltmanZScore:F2} ({riskResult.AltmanRiskLevel})\n" +
                                 $"Springate: {riskResult.SpringateScore:F2} ({riskResult.SpringateRiskLevel})\n" +
                                 $"Fulmer: {riskResult.FulmerScore:F2} ({riskResult.FulmerRiskLevel})\n" +
                                 $"Ohlson O-score: {riskResult.OhlsonOScore:F2} (Вероятность: {riskResult.OhlsonProbability:F2})\n" +
                                 $"Zmijewski: {riskResult.ZmijewskiScore:F2} (Вероятность: {riskResult.ZmijewskiProbability:F2})\n" +
                                 $"Общая оценка кредитного риска: {riskResult.OverallRiskAssessment}";
                    }
                    else
                    {
                        Result = $"Ошибка: {response.ReasonPhrase}";
                    }
                }
                catch (Exception ex)
                {
                    Result = $"Ошибка: {ex.Message}";
                }
            }
        }
    }
}