using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CreditRiskSystem.Client.ViewModels;
using CreditRiskSystem.Client.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace CreditRiskSystem.Client;
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        ServiceProvider.Initialize();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = ServiceProvider.Services.GetService<MainWindow>();
            desktop.MainWindow.DataContext = ServiceProvider.Services.GetService<MainWindowViewModel>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}