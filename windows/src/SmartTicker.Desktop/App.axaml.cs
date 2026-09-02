using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
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
            desktop.ShutdownMode = Avalonia.Controls.ShutdownMode.OnMainWindowClose;
            var websiteAccessPolicy = new WebsiteAccessPolicy();
            var mainWindow = new MainWindow();
            Dispatcher.UIThread.UnhandledException += (_, args) =>
            {
                if (!ExceptionSafety.IsRecoverable(args.Exception))
                {
                    return;
                }

                args.Handled = true;
                try
                {
                    if (mainWindow.DataContext is MainViewModel viewModel)
                    {
                        viewModel.ReportRecoverableError("An unexpected UI operation failed", args.Exception);
                    }
                }
                catch (Exception reportingException) when (ExceptionSafety.IsRecoverable(reportingException))
                {
                    System.Diagnostics.Trace.TraceError(reportingException.ToString());
                }
            };
            websiteAccessPolicy.ConsentPrompt = (request, cancellationToken) =>
                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var dialog = new WebsiteConsentWindow { DataContext = request };
                    return await dialog.ShowDialog<WebsiteConsentDecision>(mainWindow);
                });
            try
            {
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
            }
            catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
            {
                var fallback = new MainViewModel();
                fallback.ReportRecoverableError("SmartTicker startup", exception);
                mainWindow.DataContext = fallback;
            }

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}