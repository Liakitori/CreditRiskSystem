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

namespace CreditRiskSystem.Client.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly HttpClient _httpClient;
        private readonly Window _parentWindow;

        [Reactive] public string Result1 { get; set; }
        [Reactive] public string Result2 { get; set; }
        [Reactive] public string Result3 { get; set; }
        [Reactive] public string Result4 { get; set; }
        [Reactive] public string Result5 { get; set; }

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
                        var sb1 = new StringBuilder();
                        var sb2 = new StringBuilder();
                        var sb3 = new StringBuilder();
                        var sb4 = new StringBuilder();
                        var sb5 = new StringBuilder();

                        // Модели кредитного риска
                        sb1.AppendLine("### - Модели кредитного риска");
                        sb1.AppendLine($"- **Altman Z-score:** {riskResult.AltmanZScore:F2} ({riskResult.AltmanRiskLevel})");
                        sb1.AppendLine($"- **Springate:** {riskResult.SpringateScore:F2} ({riskResult.SpringateRiskLevel})");
                        sb1.AppendLine($"- **Fulmer:** {riskResult.FulmerScore:F2} ({riskResult.FulmerRiskLevel})");
                        sb1.AppendLine($"- **Ohlson O-score:** {riskResult.OhlsonOScore:F2} (Вероятность: {riskResult.OhlsonProbability:F2})");
                        sb1.AppendLine($"- **Zmijewski:** {riskResult.ZmijewskiScore:F2} (Вероятность: {riskResult.ZmijewskiProbability:F2})");
                        sb1.AppendLine($"- **Общая оценка кредитного риска:** {riskResult.OverallRiskAssessment}");
                        sb1.AppendLine();

                        // Рентабельность
                        sb2.AppendLine("### - Рентабельность");
                        sb2.AppendLine($"- **Рентабельность объема продаж (Р1):** {riskResult.Р1:F2}%");
                        sb2.AppendLine($"- **Бухгалтерская рентабельность (Р2):** {riskResult.Р2:F2}%");
                        sb2.AppendLine($"- **Чистая рентабельность (Р3):** {riskResult.Р3:F2}%");
                        sb2.AppendLine($"- **Экономическая рентабельность (Р4):** {riskResult.Р4:F2}%");
                        sb2.AppendLine($"- **Рентабельность собственного капитала (Р5):** {riskResult.Р5:F2}%");
                        sb2.AppendLine($"- **Валовая рентабельность (Р6):** {riskResult.Р6:F2}%");
                        sb2.AppendLine($"- **Рентабельность реализованной продукции (Р7):** {riskResult.Р7:F2}%");
                        sb2.AppendLine();

                        // Деловая активность
                        sb3.AppendLine("### - Деловая активность");
                        sb3.AppendLine($"- **Общая оборачиваемость капитала (ДА1):** {riskResult.ДА1:F2} оборотов");
                        sb3.AppendLine($"- **Оборачиваемость оборотных средств (ДА2):** {riskResult.ДА2:F2} оборотов");
                        sb3.AppendLine($"- **Отдача нематериальных активов (ДА3):** {riskResult.ДА3:F2} оборотов");
                        sb3.AppendLine($"- **Фондоотдача (ДА4):** {riskResult.ДА4:F2} оборотов");
                        sb3.AppendLine($"- **Отдача собственного капитала (ДА5):** {riskResult.ДА5:F2} оборотов");
                        sb3.AppendLine($"- **Оборачиваемость средств в расчетах (ДА6):** {riskResult.ДА6:F2} оборотов");
                        sb3.AppendLine($"- **Оборачиваемость кредиторской задолженности (ДА7):** {riskResult.ДА7:F2} оборотов");
                        sb3.AppendLine($"- **Оборачиваемость материальных средств (ДА8):** {riskResult.ДА8:F2} дней");
                        sb3.AppendLine($"- **Оборачиваемость денежных средств (ДА9):** {riskResult.ДА9:F2} дней");
                        sb3.AppendLine($"- **Срок погашения дебиторской задолженности (ДА10):** {riskResult.ДА10:F2} дней");
                        sb3.AppendLine($"- **Срок погашения кредиторской задолженности (ДА11):** {riskResult.ДА11:F2} дней");
                        sb3.AppendLine();

                        // Финансовая устойчивость
                        sb4.AppendLine("### - Финансовая устойчивость");
                        sb4.AppendLine($"- **Коэффициент капитализации (ФУ1):** {riskResult.ФУ1:F2}");
                        sb4.AppendLine($"- **Собственный капитал в обороте (ФУ2):** {riskResult.ФУ2:F2} тыс. руб.");
                        sb4.AppendLine($"- **Обеспеченность запасов собственными источниками (ФУ3):** {riskResult.ФУ3:F2}");
                        sb4.AppendLine($"- **Коэффициент автономии (ФУ4):** {riskResult.ФУ4:F2}");
                        sb4.AppendLine($"- **Коэффициент финансирования (ФУ5):** {riskResult.ФУ5:F2}");
                        sb4.AppendLine($"- **Коэффициент финансовой устойчивости (ФУ6): {riskResult.ФУ6:F2}");
                        sb4.AppendLine($"- **Коэффициент маневренности (ФУ7):** {riskResult.ФУ7:F2}");
                        sb4.AppendLine($"- **Коэффициент мобилизации (ФУ8):** {riskResult.ФУ8:F2}");
                        sb4.AppendLine();

                        // Платёжеспособность
                        sb5.AppendLine("### - Платёжеспособность");
                        sb5.AppendLine($"- **Общий показатель платежеспособности (П1):** {riskResult.П1:F2}");
                        sb5.AppendLine($"- **Коэффициент абсолютной ликвидности (П2):** {riskResult.П2:F2}");
                        sb5.AppendLine($"- **Коэффициент быстрой ликвидности (П3):** {riskResult.П3:F2}");
                        sb5.AppendLine($"- **Коэффициент текущей ликвидности (П4):** {riskResult.П4:F2}");
                        sb5.AppendLine($"- **Коэффициент маневренности функционирующего капитала (П5):** {riskResult.П5:F2}");
                        sb5.AppendLine($"- **Доля оборотных средств в активах (П6):** {riskResult.П6:F2}");
                        sb5.AppendLine($"- **Коэффициент обеспеченности собственными средствами (П7):** {riskResult.П7:F2}");
                        sb5.AppendLine($"- **Коэффициент обеспеченности обязательств активами (П8):** {riskResult.П8:F2}");

                        /*Result = $"Altman Z-score: {riskResult.AltmanZScore:F2} ({riskResult.AltmanRiskLevel})\n" +
                                 $"Springate: {riskResult.SpringateScore:F2} ({riskResult.SpringateRiskLevel})\n" +
                                 $"Fulmer: {riskResult.FulmerScore:F2} ({riskResult.FulmerRiskLevel})\n" +
                                 $"Ohlson O-score: {riskResult.OhlsonOScore:F2} (Вероятность: {riskResult.OhlsonProbability:F2})\n" +
                                 $"Zmijewski: {riskResult.ZmijewskiScore:F2} (Вероятность: {riskResult.ZmijewskiProbability:F2})\n" +
                                 $"Общая оценка кредитного риска: {riskResult.OverallRiskAssessment}";*/
                        Result1 = sb1.ToString();
                        Result2 = sb2.ToString();
                        Result3 = sb3.ToString();
                        Result4 = sb4.ToString();
                        Result5 = sb5.ToString();
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Result1 = $"Ошибка сервера: {response.ReasonPhrase}\nДетали: {errorContent}";
                        /*Result = $"Ошибка: {response.ReasonPhrase}";*/
                    }
                }
                catch (Exception ex)
                {
                    Result1 = $"Ошибка при обработке файла: {ex.Message}";
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
                    Result1 = $"Ошибка: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                Result1 = $"Ошибка при скачивании PDF: {ex.Message}";
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
                    Result1 = $"Ошибка: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                Result1 = $"Ошибка при скачивании JSON: {ex.Message}";
            }
        }
    }
}