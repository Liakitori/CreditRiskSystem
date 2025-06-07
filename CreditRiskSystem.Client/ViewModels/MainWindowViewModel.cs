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

namespace CreditRiskSystem.Client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        [Reactive] public ViewModelBase CurrentViewModel { get; set; }

        public MainWindowViewModel(INavigationService navigationService, AuthorizationViewModel authorizationViewModel)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            CurrentViewModel = authorizationViewModel;
            (navigationService as NavigationService)?.SetMainViewModel(this);
        }
    }
}
