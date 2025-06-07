using System;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using CreditRiskSystem.Client.Interfaces;
using CreditRiskSystem.Client.Services;
using CreditRiskSystem.Client.ViewModels;
using CreditRiskSystem.Client.Views;

namespace CreditRiskSystem.Client;

public static class ServiceProvider
{
    private static IServiceProvider? _serviceProvider;

    public static IServiceProvider Services
    {
        get => _serviceProvider ?? throw new InvalidOperationException("ServiceProvider not initialized");
        private set => _serviceProvider = value;
    }

    public static void Initialize()
    {
        var services = new ServiceCollection();

        // Регистрация Http клиента
        services.AddHttpClient("ServerApi", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7148");
        });

        // Регистрация сервисов
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<IDialogService>(provider => 
            new DialogService(provider.GetRequiredService<MainWindow>()));

        // Регистрация MainWindowViewModel
        services.AddSingleton<MainWindowViewModel>();

        // Регистрация остальных ViewModel-ей
        RegisterViewModels(services, Assembly.GetExecutingAssembly());

        // Построение провайдера сервисов
        Services = services.BuildServiceProvider();

        // Настройка NavigationService
        var navigationService = Services.GetRequiredService<INavigationService>() as NavigationService;
        navigationService?.SetMainViewModel(Services.GetRequiredService<MainWindowViewModel>());
    }
    private static void RegisterViewModels(IServiceCollection sc, Assembly asm)
    {
        foreach (var t in asm.GetTypes())
        {
            if (!t.Name.EndsWith("ViewModel") || t.IsAbstract) continue;

            if (t == typeof(MainWindowViewModel) || t == typeof(MainViewModel))
            {
                if (t == typeof(MainViewModel))
                {
                    sc.AddSingleton(t, provider =>
                    {
                        var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient("ServerApi");
                        var dialogService = provider.GetRequiredService<IDialogService>();
                        return new MainViewModel(httpClient, dialogService);
                    });
                }
                else
                {
                    sc.AddSingleton(t);
                }
            }
            else
            {
                sc.AddTransient(t);
            }
        }
    }
    /*private static void RegisterViewModels(IServiceCollection sc, Assembly asm)
    {
        foreach (var t in asm.GetTypes())
        {
            if (!t.Name.EndsWith("ViewModel") || t.IsAbstract) continue;

            if (t == typeof(MainWindowViewModel))
                sc.AddSingleton(t);
            else
                sc.AddTransient(t);
        }
    }*/
}