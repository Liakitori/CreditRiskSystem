using Avalonia.Controls;
using CreditRiskSystem.Common.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using CreditRiskSystem.Client.Interfaces;
using CreditRiskSystem.Client.Services;

namespace CreditRiskSystem.Client.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    [Reactive] public ViewModelBase CurrentViewModel { get; set; }

    public MainWindowViewModel(INavigationService navigationService, MainViewModel mainViewModel)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        CurrentViewModel = mainViewModel;
        (navigationService as NavigationService).SetMainViewModel(this);
    }
}








    /*private readonly HttpClient _httpClient;
    private readonly Window _parentWindow;

    [Reactive] public string Result { get; set; }

    public ReactiveCommand<Unit, Unit> UploadFileCommand { get; }
    public ReactiveCommand<Unit, Unit> DownloadPdfCommand { get; }
    public ReactiveCommand<Unit, Unit> DownloadJsonCommand { get; }

    public MainWindowViewModel(HttpClient httpClient, Window parentWindow)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _parentWindow = parentWindow ?? throw new ArgumentNullException(nameof(parentWindow));
        UploadFileCommand = ReactiveCommand.CreateFromTask(UploadFileAsync);
        DownloadPdfCommand = ReactiveCommand.CreateFromTask(DownloadPdfAsync);
        DownloadJsonCommand = ReactiveCommand.CreateFromTask(DownloadJsonAsync);
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
                    var sb = new StringBuilder();

                    // Модели кредитного риска
                    sb.AppendLine("=== Модели кредитного риска ===");
                    sb.AppendLine($"Altman Z-score: {riskResult.AltmanZScore:F2} ({riskResult.AltmanRiskLevel})");
                    sb.AppendLine($"Springate: {riskResult.SpringateScore:F2} ({riskResult.SpringateRiskLevel})");
                    sb.AppendLine($"Fulmer: {riskResult.FulmerScore:F2} ({riskResult.FulmerRiskLevel})");
                    sb.AppendLine($"Ohlson O-score: {riskResult.OhlsonOScore:F2} (Вероятность: {riskResult.OhlsonProbability:F2})");
                    sb.AppendLine($"Zmijewski: {riskResult.ZmijewskiScore:F2} (Вероятность: {riskResult.ZmijewskiProbability:F2})");
                    sb.AppendLine($"Общая оценка кредитного риска: {riskResult.OverallRiskAssessment}");
                    sb.AppendLine();

                    // Рентабельность
                    sb.AppendLine("=== Рентабельность ===");
                    sb.AppendLine($"Рентабельность объема продаж (Р1): {riskResult.Р1:F2}%");
                    sb.AppendLine($"Бухгалтерская рентабельность (Р2): {riskResult.Р2:F2}%");
                    sb.AppendLine($"Чистая рентабельность (Р3): {riskResult.Р3:F2}%");
                    sb.AppendLine($"Экономическая рентабельность (Р4): {riskResult.Р4:F2}%");
                    sb.AppendLine($"Рентабельность собственного капитала (Р5): {riskResult.Р5:F2}%");
                    sb.AppendLine($"Валовая рентабельность (Р6): {riskResult.Р6:F2}%");
                    sb.AppendLine($"Рентабельность реализованной продукции (Р7): {riskResult.Р7:F2}%");
                    sb.AppendLine();

                    // Деловая активность
                    sb.AppendLine("=== Деловая активность ===");
                    sb.AppendLine($"Общая оборачиваемость капитала (ДА1): {riskResult.ДА1:F2} оборотов");
                    sb.AppendLine($"Оборачиваемость оборотных средств (ДА2): {riskResult.ДА2:F2} оборотов");
                    sb.AppendLine($"Отдача нематериальных активов (ДА3): {riskResult.ДА3:F2} оборотов");
                    sb.AppendLine($"Фондоотдача (ДА4): {riskResult.ДА4:F2} оборотов");
                    sb.AppendLine($"Отдача собственного капитала (ДА5): {riskResult.ДА5:F2} оборотов");
                    sb.AppendLine($"Оборачиваемость средств в расчетах (ДА6): {riskResult.ДА6:F2} оборотов");
                    sb.AppendLine($"Оборачиваемость кредиторской задолженности (ДА7): {riskResult.ДА7:F2} оборотов");
                    sb.AppendLine($"Оборачиваемость материальных средств (ДА8): {riskResult.ДА8:F2} дней");
                    sb.AppendLine($"Оборачиваемость денежных средств (ДА9): {riskResult.ДА9:F2} дней");
                    sb.AppendLine($"Срок погашения дебиторской задолженности (ДА10): {riskResult.ДА10:F2} дней");
                    sb.AppendLine($"Срок погашения кредиторской задолженности (ДА11): {riskResult.ДА11:F2} дней");
                    sb.AppendLine();

                    // Финансовая устойчивость
                    sb.AppendLine("=== Финансовая устойчивость ===");
                    sb.AppendLine($"Коэффициент капитализации (ФУ1): {riskResult.ФУ1:F2}");
                    sb.AppendLine($"Собственный капитал в обороте (ФУ2): {riskResult.ФУ2:F2} тыс. руб.");
                    sb.AppendLine($"Обеспеченность запасов собственными источниками (ФУ3): {riskResult.ФУ3:F2}");
                    sb.AppendLine($"Коэффициент автономии (ФУ4): {riskResult.ФУ4:F2}");
                    sb.AppendLine($"Коэффициент финансирования (ФУ5): {riskResult.ФУ5:F2}");
                    sb.AppendLine($"Коэффициент финансовой устойчивости (ФУ6): {riskResult.ФУ6:F2}");
                    sb.AppendLine($"Коэффициент маневренности (ФУ7): {riskResult.ФУ7:F2}");
                    sb.AppendLine($"Коэффициент мобилизации (ФУ8): {riskResult.ФУ8:F2}");
                    sb.AppendLine();

                    // Платёжеспособность
                    sb.AppendLine("=== Платёжеспособность ===");
                    sb.AppendLine($"Общий показатель платежеспособности (П1): {riskResult.П1:F2}");
                    sb.AppendLine($"Коэффициент абсолютной ликвидности (П2): {riskResult.П2:F2}");
                    sb.AppendLine($"Коэффициент быстрой ликвидности (П3): {riskResult.П3:F2}");
                    sb.AppendLine($"Коэффициент текущей ликвидности (П4): {riskResult.П4:F2}");
                    sb.AppendLine($"Коэффициент маневренности функционирующего капитала (П5): {riskResult.П5:F2}");
                    sb.AppendLine($"Доля оборотных средств в активах (П6): {riskResult.П6:F2}");
                    sb.AppendLine($"Коэффициент обеспеченности собственными средствами (П7): {riskResult.П7:F2}");
                    sb.AppendLine($"Коэффициент обеспеченности обязательств активами (П8): {riskResult.П8:F2}");

                    *//*Result = $"Altman Z-score: {riskResult.AltmanZScore:F2} ({riskResult.AltmanRiskLevel})\n" +
                             $"Springate: {riskResult.SpringateScore:F2} ({riskResult.SpringateRiskLevel})\n" +
                             $"Fulmer: {riskResult.FulmerScore:F2} ({riskResult.FulmerRiskLevel})\n" +
                             $"Ohlson O-score: {riskResult.OhlsonOScore:F2} (Вероятность: {riskResult.OhlsonProbability:F2})\n" +
                             $"Zmijewski: {riskResult.ZmijewskiScore:F2} (Вероятность: {riskResult.ZmijewskiProbability:F2})\n" +
                             $"Общая оценка кредитного риска: {riskResult.OverallRiskAssessment}";*//*
                    Result = sb.ToString();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Result = $"Ошибка сервера: {response.ReasonPhrase}\nДетали: {errorContent}";
                    *//*Result = $"Ошибка: {response.ReasonPhrase}";*//*
                }
            }
            catch (Exception ex)
            {
                Result = $"Ошибка при обработке файла: {ex.Message}";
            }
        }
    }

    private async Task DownloadPdfAsync()
    {
        try
        {
            // Отправляем GET-запрос к эндпоинту для получения PDF
            var response = await _httpClient.GetAsync("api/FinancialData/download/pdf");
            if (response.IsSuccessStatusCode)
            {
                // Сохраняем файл на диск
                var saveDialog = new SaveFileDialog
                {
                    Title = "Сохранить PDF",
                    InitialFileName = "result.pdf",
                    Filters = { new FileDialogFilter { Name = "PDF файлы", Extensions = { "pdf" } } }
                };

                var path = await saveDialog.ShowAsync(_parentWindow);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    using var fileStream = File.Create(path);
                    await response.Content.CopyToAsync(fileStream);
                }
            }
            else
            {
                Result = $"Ошибка: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            Result = $"Ошибка при скачивании PDF: {ex.Message}";
        }
    }

    private async Task DownloadJsonAsync()
    {
        try
        {
            // Отправляем GET-запрос к эндпоинту для получения JSON
            var response = await _httpClient.GetAsync("api/FinancialData/download/json");
            if (response.IsSuccessStatusCode)
            {
                // Сохраняем файл на диск
                var saveDialog = new SaveFileDialog
                {
                    Title = "Сохранить JSON",
                    InitialFileName = "result.json",
                    Filters = { new FileDialogFilter { Name = "JSON файлы", Extensions = { "json" } } }
                };

                var path = await saveDialog.ShowAsync(_parentWindow);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    using var fileStream = File.Create(path);
                    await response.Content.CopyToAsync(fileStream);
                }
            }
            else
            {
                Result = $"Ошибка: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            Result = $"Ошибка при скачивании JSON: {ex.Message}";
        }
    }*/