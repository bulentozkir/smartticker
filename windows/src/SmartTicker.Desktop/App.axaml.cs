using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SmartTicker.Core.Services;
using SmartTicker.Desktop.ViewModels;
using SmartTicker.Desktop.Views;
using SmartTicker.Infrastructure.Audio;
using SmartTicker.Infrastructure.Extraction;
using SmartTicker.Infrastructure.Launching;
using SmartTicker.Infrastructure.Persistence;
using SmartTicker.Infrastructure.Startup;

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
            var websiteAccessPolicy = new WebsiteAccessPolicy();
            var mainWindow = new MainWindow();
            websiteAccessPolicy.ConsentPrompt = (request, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var dialog = new WebsiteConsentWindow { DataContext = request };
                return dialog.ShowDialog<WebsiteConsentDecision>(mainWindow);
            };
            mainWindow.DataContext = new MainViewModel(
                new StaticHtmlPriceSelectorDiscovery(websiteAccessPolicy),
                new StaticHtmlQuoteFetcher(websiteAccessPolicy),
                new StaticHtmlNewsSelectorDiscovery(websiteAccessPolicy),
                new LocalJsonSettingsStore(),
                new StaticHtmlNewsFetcher(websiteAccessPolicy),
                new DefaultBrowserLinkLauncher(),
                new GitHubStarterSettingsSource(websiteAccessPolicy),
                new LocalJsonAlertStore(),
                new SystemAlertSound(),
                StartupRegistrationFactory.Create(),
                websiteAccessPolicy);
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}