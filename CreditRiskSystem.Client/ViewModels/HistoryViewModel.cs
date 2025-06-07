using CreditRiskSystem.Client.Interfaces;
using CreditRiskSystem.Client.Services;
using CreditRiskSystem.Common.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace CreditRiskSystem.Client.ViewModels
{
    public class HistoryViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private readonly IDialogService _dialogService;

        [Reactive] public ObservableCollection<CalculationSummary> Calculations { get; set; }
        public ReactiveCommand<Guid, Unit> DownloadPdfCommand { get; }
        public ReactiveCommand<Guid, Unit> DownloadJsonCommand { get; }

        public HistoryViewModel(IApiService apiService, IDialogService dialogService)
        {
            _apiService = apiService;
            _dialogService = dialogService;
            Calculations = new ObservableCollection<CalculationSummary>();

            DownloadPdfCommand = ReactiveCommand.CreateFromTask<Guid>(DownloadPdfAsync);
            DownloadJsonCommand = ReactiveCommand.CreateFromTask<Guid>(DownloadJsonAsync);

            LoadCalculationsAsync();
        }

        private async void LoadCalculationsAsync()
        {
            try
            {
                var results = await _apiService.GetHistoryAsync();
                Calculations.Clear();
                foreach (var result in results)
                {
                    Calculations.Add(new CalculationSummary
                    {
                        Id = result.Id,
                        CreatedAt = result.CalculatedAt,
                        OverallRisk = result.OverallRiskAssessment
                    });
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync("Ошибка", $"Не удалось загрузить историю: {ex.Message}");
            }
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

    public class CalculationSummary
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OverallRisk { get; set; }
    }
}