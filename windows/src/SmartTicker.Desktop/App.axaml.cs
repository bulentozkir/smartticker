using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SmartTicker.Desktop.ViewModels;
using SmartTicker.Desktop.Views;
using SmartTicker.Infrastructure.Extraction;
using SmartTicker.Infrastructure.Launching;
using SmartTicker.Infrastructure.Persistence;

namespace SmartTicker.Desktop;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(
                    new StaticHtmlPriceSelectorDiscovery(),
                    new StaticHtmlQuoteFetcher(),
                    new StaticHtmlNewsSelectorDiscovery(),
                    new LocalJsonSettingsStore(),
                    new StaticHtmlNewsFetcher(),
                    new DefaultBrowserLinkLauncher()),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}