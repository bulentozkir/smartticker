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
    }

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