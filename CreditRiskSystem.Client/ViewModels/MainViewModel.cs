using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using CreditRiskSystem.Client.Interfaces;
using CreditRiskSystem.Common.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CreditRiskSystem.Client.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;

    [Reactive] public string Result { get; set; }

    public ReactiveCommand<Unit, Unit> UploadFileCommand { get; }
    public ReactiveCommand<Unit, Unit> DownloadPdfCommand { get; }
    public ReactiveCommand<Unit, Unit> DownloadJsonCommand { get; }

    public MainViewModel(HttpClient httpClient, IDialogService dialogService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        UploadFileCommand = ReactiveCommand.CreateFromTask(UploadFileAsync);
        DownloadPdfCommand = ReactiveCommand.CreateFromTask(DownloadPdfAsync);
        DownloadJsonCommand = ReactiveCommand.CreateFromTask(DownloadJsonAsync);
    }

    private async Task UploadFileAsync()
    {
        var filters = new List<(string Name, List<string> Extensions)>
        {
            ("Excel", new List<string> { "xlsx" })
        };

        var result = await _dialogService.ShowOpenFileDialogAsync("Выберите файл Excel", filters);
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
                    var sb = new StringBuilder();

                    sb.AppendLine("=== Модели кредитного риска ===");
                    sb.AppendLine($"Altman Z-score: {riskResult.AltmanZScore:F2} ({riskResult.AltmanRiskLevel})");
                    sb.AppendLine($"Springate: {riskResult.SpringateScore:F2} ({riskResult.SpringateRiskLevel})");
                    sb.AppendLine($"Fulmer: {riskResult.FulmerScore:F2} ({riskResult.FulmerRiskLevel})");
                    sb.AppendLine($"Ohlson O-score: {riskResult.OhlsonOScore:F2} (Вероятность: {riskResult.OhlsonProbability:F2})");
                    sb.AppendLine($"Zmijewski: {riskResult.ZmijewskiScore:F2} (Вероятность: {riskResult.ZmijewskiProbability:F2})");
                    sb.AppendLine($"Общая оценка кредитного риска: {riskResult.OverallRiskAssessment}");
                    sb.AppendLine();

                    // Добавьте остальные категории (Рентабельность, Деловая активность и т.д.) по аналогии из закомментированного кода

                    Result = sb.ToString();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await _dialogService.ShowMessageAsync("Ошибка", $"Ошибка сервера: {response.ReasonPhrase}\nДетали: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync("Ошибка", $"Ошибка при обработке файла: {ex.Message}");
            }
        }
    }

    private async Task DownloadPdfAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/FinancialData/download/pdf");
            if (response.IsSuccessStatusCode)
            {
                var filters = new List<(string Name, List<string> Extensions)>
                {
                    ("PDF файлы", new List<string> { "pdf" })
                };

                var path = await _dialogService.ShowSaveFileDialogAsync("Сохранить PDF", "result.pdf", filters);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    using var fileStream = File.Create(path);
                    await response.Content.CopyToAsync(fileStream);
                }
            }
            else
            {
                await _dialogService.ShowMessageAsync("Ошибка", $"Ошибка: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Ошибка", $"Ошибка при скачивании PDF: {ex.Message}");
        }
    }

    private async Task DownloadJsonAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/FinancialData/download/json");
            if (response.IsSuccessStatusCode)
            {
                var filters = new List<(string Name, List<string> Extensions)>
                {
                    ("JSON файлы", new List<string> { "json" })
                };

                var path = await _dialogService.ShowSaveFileDialogAsync("Сохранить JSON", "result.json", filters);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    using var fileStream = File.Create(path);
                    await response.Content.CopyToAsync(fileStream);
                }
            }
            else
            {
                await _dialogService.ShowMessageAsync("Ошибка", $"Ошибка: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Ошибка", $"Ошибка при скачивании JSON: {ex.Message}");
        }
    }
}

