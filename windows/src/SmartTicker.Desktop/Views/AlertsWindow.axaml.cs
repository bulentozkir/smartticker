using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SmartTicker.Desktop.Views;

public partial class AlertsWindow : Window
{
    public AlertsWindow()
    {
        InitializeComponent();
    }

    private void ShowHelp(object? sender, RoutedEventArgs e) =>
        ExceptionSafety.Run(() => HelpWindow.Open(this));
}
