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
using CreditRiskSystem.Client.Models;

namespace CreditRiskSystem.Client.ViewModels
{
    public class HistoryViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private readonly IDialogService _dialogService;

        [Reactive] public ObservableCollection<CalculationSummaryViewModel> Calculations { get; set; }

        public HistoryViewModel(IApiService apiService, IDialogService dialogService)
        {
            _apiService = apiService;
            _dialogService = dialogService;
            Calculations = [];
            
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
                    Calculations.Add(new CalculationSummaryViewModel(
                        result.Id,
                        result.CalculatedAt,
                        result.OverallRiskAssessment,
                        _apiService,
                        _dialogService
                    ));
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync("Ошибка", $"Не удалось загрузить историю: {ex.Message}");
            }
        }
    }
}