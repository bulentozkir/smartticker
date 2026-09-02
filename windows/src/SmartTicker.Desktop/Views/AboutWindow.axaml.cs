using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SmartTicker.Infrastructure.Launching;

namespace SmartTicker.Desktop.Views;

public partial class AboutWindow : Window
{
    private static readonly Uri LicenseUri =
        new("https://polyformproject.org/licenses/noncommercial/1.0.0");

    public AboutWindow()
    {
        InitializeComponent();
        VersionText.Text = $"Version {ResolveVersion()}";
    }

    private static string ResolveVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? assembly.GetName().Version?.ToString()
            ?? "1.0.3";
    }

    private void OpenLicense(object? sender, RoutedEventArgs e) =>
        ExceptionSafety.Run(() => new DefaultBrowserLinkLauncher().TryOpen(LicenseUri));

    private void CloseWindow(object? sender, RoutedEventArgs e) => ExceptionSafety.Run(Close);
}
