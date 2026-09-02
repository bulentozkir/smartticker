using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SmartTicker.Core.Models;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => ExceptionSafety.Run(() =>
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.ConfirmAlertRemoval = ConfirmAlertRemovalAsync;
            }
        });
    }

    private void ShowHelp(object? sender, RoutedEventArgs e) =>
        ExceptionSafety.Run(() => HelpWindow.Open(this));

    private void OpenQuoteGroups(object? sender, RoutedEventArgs e) =>
        ExceptionSafety.Run(() => QuoteGroupsWindow.Open(this, DataContext));

    private async void ImportSampleConfig(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            await ExceptionSafety.RunAsync(
                () => SampleConfigImportWorkflow.RunAsync(this, viewModel),
                exception => viewModel.ReportRecoverableError("Importing the sample config", exception));
        }
    }

    private Task<bool> ConfirmAlertRemovalAsync(string symbol, int count) =>
        ConfirmDialog.ShowAsync(
            this,
            "Alert rules",
            $"{symbol} has {count} alert rule(s). Delete them?",
            "Delete rules");

    private void EditSubscription(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: TickerSubscription subscription } &&
            DataContext is MainViewModel viewModel)
        {
            ExceptionSafety.Run(
                () => viewModel.EditSubscriptionCommand.Execute(subscription),
                exception => viewModel.ReportRecoverableError("Editing quote", exception));
        }
    }

    private void RemoveSubscription(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: TickerSubscription subscription } &&
            DataContext is MainViewModel viewModel)
        {
            ExceptionSafety.Run(
                () => viewModel.RemoveSubscriptionCommand.Execute(subscription),
                exception => viewModel.ReportRecoverableError("Removing quote", exception));
        }
    }
}