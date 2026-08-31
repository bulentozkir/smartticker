using Avalonia.Controls;
using Avalonia.Interactivity;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Views;

public partial class SourcePermissionWindow : Window
{
    public SourcePermissionWindow()
    {
        InitializeComponent();
    }

    private void Approve(object? sender, RoutedEventArgs e) => Close(SourcePermissionDecision.Approve);

    private void Skip(object? sender, RoutedEventArgs e) => Close(SourcePermissionDecision.Skip);

    private void Cancel(object? sender, RoutedEventArgs e) => Close(SourcePermissionDecision.Cancel);
}