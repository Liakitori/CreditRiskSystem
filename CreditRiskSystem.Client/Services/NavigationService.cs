using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CreditRiskSystem.Client.Interfaces;
using CreditRiskSystem.Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CreditRiskSystem.Client.Services;

public class NavigationService : INavigationService
{
    private MainWindowViewModel? _mainWindowViewModel;
    private readonly Stack<ViewModelBase> _navigationStack = new Stack<ViewModelBase>();
    public event Action<ViewModelBase> ViewModelChanged;

    public void SetMainViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }

    public async Task NavigateTo<TViewModel>(object parameter = null) where TViewModel : ViewModelBase
    {
        if (_mainWindowViewModel == null)
            throw new InvalidOperationException("MainViewModel not set");

        var viewModel = ServiceProvider.Services.GetService<TViewModel>();
        if (viewModel == null)
            throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} not registered");

        _navigationStack.Push(_mainWindowViewModel.CurrentViewModel);
        _mainWindowViewModel.CurrentViewModel = viewModel;
        ViewModelChanged?.Invoke(viewModel);
    }

    public void GoBack()
    {
        if (_navigationStack.Count <= 0) return;

        var previousViewModel = _navigationStack.Pop();
        _mainWindowViewModel.CurrentViewModel = previousViewModel;
        ViewModelChanged?.Invoke(previousViewModel);
    }
}