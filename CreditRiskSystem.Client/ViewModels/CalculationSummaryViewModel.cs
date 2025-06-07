using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using CreditRiskSystem.Client.Interfaces;
using CreditRiskSystem.Client.Services;
using ReactiveUI;

namespace CreditRiskSystem.Client.Models;

public class CalculationSummaryViewModel
{
    private readonly IApiService _apiService;
    private readonly IDialogService _dialogService;
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string OverallRisk { get; set; }

    public ReactiveCommand<Guid, Unit> DownloadPdfCommand { get; }
    public ReactiveCommand<Guid, Unit> DownloadJsonCommand { get; }
    
    public CalculationSummaryViewModel(Guid id, DateTime createdAt, string overallRisk, IApiService apiService, IDialogService dialogService)
    {
        _apiService = apiService;
        _dialogService = dialogService;
        Id = id;
        CreatedAt = createdAt;
        OverallRisk = overallRisk;
        DownloadPdfCommand = ReactiveCommand.CreateFromTask<Guid>(DownloadPdfAsync);
        DownloadJsonCommand = ReactiveCommand.CreateFromTask<Guid>(DownloadJsonAsync);
    }
    private async Task DownloadPdfAsync(Guid id)
    {
        try
        {
            var stream = await _apiService.DownloadPdfAsync(id);
            var path = await _dialogService.ShowSaveFileDialogAsync("Сохранить PDF", $"result_{id}.pdf", new[] { ("PDF файлы", new[] { "pdf" }.ToList()) }.ToList());
            if (!string.IsNullOrWhiteSpace(path))
            {
                using var fileStream = File.Create(path);
                await stream.CopyToAsync(fileStream);
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Ошибка", $"Ошибка при скачивании PDF: {ex.Message}");
        }
    }

    private async Task DownloadJsonAsync(Guid id)
    {
        try
        {
            var stream = await _apiService.DownloadJsonAsync(id);
            var path = await _dialogService.ShowSaveFileDialogAsync("Сохранить JSON", $"result_{id}.json", new[] { ("JSON файлы", new[] { "json" }.ToList()) }.ToList());
            if (!string.IsNullOrWhiteSpace(path))
            {
                using var fileStream = File.Create(path);
                await stream.CopyToAsync(fileStream);
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Ошибка", $"Ошибка при скачивании JSON: {ex.Message}");
        }
    }
}