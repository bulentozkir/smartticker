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
        DataContextChanged += (_, _) =>
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.ConfirmAlertRemoval = ConfirmAlertRemovalAsync;
            }
        };
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
            viewModel.EditSubscriptionCommand.Execute(subscription);
        }
    }

    private void RemoveSubscription(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: TickerSubscription subscription } &&
            DataContext is MainViewModel viewModel)
        {
            viewModel.RemoveSubscriptionCommand.Execute(subscription);
        }
    }
}