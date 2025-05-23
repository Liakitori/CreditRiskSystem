using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CreditRiskSystem.Client.ViewModels;
using CreditRiskSystem.Client.Views;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddHttpClient<MainWindowViewModel>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001"); // Укажи порт твоего сервера
            });

            services.AddSingleton<MainWindowViewModel>();

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
        /*public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }*/

    }
}