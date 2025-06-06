using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CreditRiskSystem.Client.ViewModels;
using CreditRiskSystem.Client.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace CreditRiskSystem.Client
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();
            services.AddHttpClient("ServerApi", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7148");
            });

            // Регистрируем MainWindowViewModel
            services.AddSingleton<MainWindowViewModel>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("ServerApi");
                var mainWindow = new MainWindow(); // Создаем окно
                var viewModel = new MainWindowViewModel(httpClient, mainWindow);
                mainWindow.DataContext = viewModel; // Устанавливаем DataContext для окна
                return viewModel;
            });

            // Регистрируем MainWindow как singleton
            services.AddSingleton<MainWindow>(provider =>
            {
                var viewModel = provider.GetRequiredService<MainWindowViewModel>();
                var mainWindow = new MainWindow
                {
                    DataContext = viewModel
                };
                return mainWindow;
            });

            var serviceProvider = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = serviceProvider.GetRequiredService<MainWindow>();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}