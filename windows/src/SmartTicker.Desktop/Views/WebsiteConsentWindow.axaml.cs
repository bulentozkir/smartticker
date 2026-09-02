using Avalonia.Controls;
using Avalonia.Interactivity;
using SmartTicker.Core.Services;

namespace SmartTicker.Desktop.Views;

public partial class WebsiteConsentWindow : Window
{
    public WebsiteConsentWindow()
    {
        InitializeComponent();
    }

    private void Accept(object? sender, RoutedEventArgs e) =>
        ExceptionSafety.Run(() => Close(WebsiteConsentDecision.Accept));

    private void Reject(object? sender, RoutedEventArgs e) =>
        ExceptionSafety.Run(() => Close(WebsiteConsentDecision.Reject));

    private void Cancel(object? sender, RoutedEventArgs e) =>
        ExceptionSafety.Run(() => Close(WebsiteConsentDecision.Cancel));
}