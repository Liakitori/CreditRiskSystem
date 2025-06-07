using CreditRiskSystem.Client.Interfaces;
using CreditRiskSystem.Client.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace CreditRiskSystem.Client.ViewModels
{
    public class AuthorizationViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private readonly INavigationService _navigationService;

        [Reactive] public string Username { get; set; }
        [Reactive] public string Password { get; set; }
        [Reactive] public string ErrorMessage { get; set; }

        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

        public AuthorizationViewModel(IApiService apiService, INavigationService navigationService)
        {
            _apiService = apiService;
            _navigationService = navigationService;

            LoginCommand = ReactiveCommand.CreateFromTask(LoginAsync);
            RegisterCommand = ReactiveCommand.CreateFromTask(RegisterAsync);
        }

        private async Task LoginAsync()
        {
            try
            {
                var token = await _apiService.LoginAsync(Username, Password);
                _apiService.SetToken(token);
                await _navigationService.NavigateTo<MainViewModel>();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private async Task RegisterAsync()
        {
            try
            {
                await _apiService.RegisterAsync(Username, Password);
                var token = await _apiService.LoginAsync(Username, Password);
                _apiService.SetToken(token);
                await _navigationService.NavigateTo<MainViewModel>();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }


}
