using System.Xml.Linq;
using SmartTicker.Desktop.Views;

namespace SmartTicker.Desktop.Tests;

public sealed class HelpContentContractTests
{
    [Fact]
    public void CompleteGuide_IsEmbeddedInDesktopAssembly()
    {
        var assembly = typeof(HelpWindow).Assembly;
        var resourceName = Assert.Single(
            assembly.GetManifestResourceNames(),
            name => name.EndsWith(".HELPME.md", StringComparison.Ordinal));

        using var stream = assembly.GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);
        using var reader = new StreamReader(stream!);
        var guide = reader.ReadToEnd();
        var source = File.ReadAllText(RepositoryPath("HELPME.md"));

        Assert.Equal(source, guide);
        Assert.Contains("# SmartTicker Help", guide);
        Assert.Contains("## Quick navigation", guide);
        Assert.Contains("[Quotes](#quotes)", guide);
        Assert.Contains("[App Settings](#app-settings)", guide);
        Assert.Contains("[Alert rules](#alert-rules)", guide);
        Assert.Contains("[Troubleshooting](#troubleshooting)", guide);
        Assert.Contains("## Quotes", guide);
        Assert.Contains("## App Settings", guide);
        Assert.Contains("## Alert rules", guide);
        Assert.Contains("## Troubleshooting", guide);
    }

    [Fact]
    public void EveryConfigurationSurface_OpensHelpWindow()
    {
        var windowNames = new[] { "MainWindow", "SettingsWindow", "AppSettingsWindow", "AlertsWindow" };
        foreach (var windowName in windowNames)
        {
            var window = XDocument.Load(DesktopPath("Views", $"{windowName}.axaml"));
            var helpItem = Assert.Single(
                window.Descendants(),
                element => (string?)element.Attribute("Click") == "ShowHelp");

            var label = (string?)helpItem.Attribute("Header") ?? (string?)helpItem.Attribute("Content");
            Assert.NotNull(label);
        }
    }

    [Fact]
    public void HelpWindow_UsesPublishedRawGuide()
    {
        var code = File.ReadAllText(DesktopPath("Views", "HelpWindow.axaml.cs"));

        Assert.Contains(
            "https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/HELPME.md",
            code);
    }

    private static string DesktopPath(params string[] parts)
        => RepositoryPath(["windows", "src", "SmartTicker.Desktop", .. parts]);

    private static string RepositoryPath(params string[] parts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null &&
               !Directory.Exists(Path.Combine(directory.FullName, "windows", "src", "SmartTicker.Desktop")))
        {
            directory = directory.Parent;
        }

        Assert.NotNull(directory);
        return Path.Combine([directory!.FullName, .. parts]);
    }
}