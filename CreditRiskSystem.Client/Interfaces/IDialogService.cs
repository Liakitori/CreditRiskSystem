using System.Collections.Generic;
using System.Threading.Tasks;

namespace CreditRiskSystem.Client.Interfaces;

public interface IDialogService
{
    Task ShowMessageAsync(string title, string message);
    Task<string[]?> ShowOpenFileDialogAsync(string title, List<(string Name, List<string> Extensions)> filters);
    Task<string?> ShowSaveFileDialogAsync(string title, string initialFileName, List<(string Name, List<string> Extensions)> filters);
}