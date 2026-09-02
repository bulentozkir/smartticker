using Avalonia.Controls;
using Avalonia.Interactivity;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Views;

public partial class QuoteGroupsWindow : Window
{
    public QuoteGroupsWindow()
    {
        InitializeComponent();
        WindowReachability.Attach(this);
        Opened += (_, _) => ExceptionSafety.Run(
            () => (DataContext as MainViewModel)?.PrepareQuoteGroupManager(),
            exception => (DataContext as MainViewModel)?.ReportRecoverableError("Opening Quote Groups", exception));
    }

    public static void Open(Window owner, object? dataContext)
    {
        var window = new QuoteGroupsWindow { DataContext = dataContext };
        window.Show(owner);
        window.Activate();
    }

    private void ShowHelp(object? sender, RoutedEventArgs e) =>
        ExceptionSafety.Run(() => HelpWindow.Open(this));

    private void CloseWindow(object? sender, RoutedEventArgs e) => ExceptionSafety.Run(Close);
}