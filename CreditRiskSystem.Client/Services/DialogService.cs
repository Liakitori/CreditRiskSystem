using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CreditRiskSystem.Client.Interfaces;

namespace CreditRiskSystem.Client.Services;

public class DialogService : IDialogService
{
    private readonly Window _owner;

    public DialogService(Window owner)
    {
        _owner = owner;
    }

    public async Task ShowMessageAsync(string title, string message)
    {
        var dialog = new Window
        {
            Title = title,
            Content = new TextBlock { Text = message },
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        await dialog.ShowDialog(_owner);
    }

    public async Task<string[]?> ShowOpenFileDialogAsync(string title, List<(string Name, List<string> Extensions)> filters)
    {
        var dialog = new OpenFileDialog
        {
            Title = title,
            Filters = filters.Select(f => new FileDialogFilter { Name = f.Name, Extensions = f.Extensions }).ToList()
        };

        return await dialog.ShowAsync(_owner);
    }

    public async Task<string?> ShowSaveFileDialogAsync(string title, string initialFileName, List<(string Name, List<string> Extensions)> filters)
    {
        var dialog = new SaveFileDialog
        {
            Title = title,
            InitialFileName = initialFileName,
            Filters = filters.Select(f => new FileDialogFilter { Name = f.Name, Extensions = f.Extensions }).ToList()
        };

        return await dialog.ShowAsync(_owner);
    }
}