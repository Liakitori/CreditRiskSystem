using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CreditRiskSystem.Client.ViewModels;
using CreditRiskSystem.Client.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
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
            // Регистрируем HttpClient с именем "ServerApi"
            services.AddHttpClient("ServerApi", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7148");
                //Debug.WriteLine($"HttpClient configured with BaseAddress: {client.BaseAddress}");
            });
            // Регистрируем MainWindowViewModel
            services.AddSingleton<MainWindowViewModel>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("ServerApi");
                //Debug.WriteLine($"Creating MainWindowViewModel with HttpClient BaseAddress: {httpClient.BaseAddress}");
                return new MainWindowViewModel(httpClient);
            });

            var serviceProvider = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

    }
}