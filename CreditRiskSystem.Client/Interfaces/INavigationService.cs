using System;
using System.Threading.Tasks;
using CreditRiskSystem.Client.ViewModels;

namespace CreditRiskSystem.Client.Interfaces;

public interface INavigationService
{
    Task NavigateTo<TViewModel>(object parameter = null) where TViewModel : ViewModelBase;
    void GoBack();
    
}