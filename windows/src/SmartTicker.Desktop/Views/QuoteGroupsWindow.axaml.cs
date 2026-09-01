using Avalonia.Controls;
using Avalonia.Interactivity;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Views;

public partial class QuoteGroupsWindow : Window
{
    public QuoteGroupsWindow()
    {
        InitializeComponent();
        Opened += (_, _) => (DataContext as MainViewModel)?.PrepareQuoteGroupManager();
    }

    public static void Open(Window owner, object? dataContext)
    {
        var window = new QuoteGroupsWindow { DataContext = dataContext };
        window.Show(owner);
        window.Activate();
    }

    private void ShowHelp(object? sender, RoutedEventArgs e) => HelpWindow.Open(this);

    private void CloseWindow(object? sender, RoutedEventArgs e) => Close();
}