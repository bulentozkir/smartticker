using System.Xml.Linq;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using SmartTicker.Core.Models;
using SmartTicker.Desktop.Controls;
using SmartTicker.Desktop.Localization;
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
        Assert.Contains("[Group quotes](#group-quotes)", guide);
        Assert.Contains("[Scrolling or static view](#choose-scrolling-or-static-quote-view)", guide);
        Assert.Contains("[App Settings](#app-settings)", guide);
        Assert.Contains("[Alert rules](#alert-rules)", guide);
        Assert.Contains("[Troubleshooting](#troubleshooting)", guide);
        Assert.Contains("News opens automatically in a separate", guide);
        Assert.Contains("View > Open static news window", guide);
        Assert.Contains("Left-to-right scroll: Prices with News", guide);
        Assert.Contains("headlines are interleaved by quote", guide);
        Assert.Contains("Show news for", guide);
        Assert.Contains("### Change highlights", guide);
        Assert.Contains("### Edit the configuration files in place", guide);
        Assert.Contains("Edit Current App Config", guide);
        Assert.Contains("responsive tiles laid out from left to right", guide);
        Assert.Contains("Drag the dotted handle", guide);
        Assert.Contains("## Quotes", guide);
        Assert.Contains("## App Settings", guide);
        Assert.Contains("## Alert rules", guide);
        Assert.Contains("## Troubleshooting", guide);
    }

    [Fact]
    public void EverySupportedLanguage_HasACompleteDistinctEmbeddedGuideAndLocalizedChrome()
    {
        var english = File.ReadAllText(RepositoryPath("HELPME.md"));
        var expectedStructure = StructureOf(english);

        foreach (var language in AppLanguages.Supported)
        {
            var path = language == AppLanguages.Default
                ? RepositoryPath("HELPME.md")
                : RepositoryPath("help", $"HELPME.{language}.md");
            Assert.True(File.Exists(path), $"Missing Help translation for '{language}': {path}");
            var source = File.ReadAllText(path);

            Assert.Equal(source, HelpWindow.ReadEmbeddedHelp(language));
            Assert.Equal(expectedStructure, StructureOf(source));
            Assert.Contains("SmartTicker", source);
            Assert.Contains("1.0.3", source);
            Assert.Contains("settings.json", source);
            Assert.Contains("alerts.json", source);
            Assert.Contains("SMARTTICKER_DATA_DIRECTORY", source);
            Assert.Contains("Left-to-right scroll: Prices with News", source);
            Assert.Contains("https://github.com/bulentozkir/smartticker/issues", source);
            Assert.DoesNotContain("**Reload**", source, StringComparison.OrdinalIgnoreCase);

            var strings = HelpLocalization.For(language);
            Assert.All(
                new[]
                {
                    strings.Title,
                    strings.Subtitle,
                    strings.Navigation,
                    strings.CheckingOnline,
                    strings.OnlineLoaded,
                    strings.OfflineLoaded,
                },
                value => Assert.False(string.IsNullOrWhiteSpace(value)));
            var expectedPath = language == AppLanguages.Default
                ? "/bulentozkir/smartticker/refs/heads/main/HELPME.md"
                : $"/bulentozkir/smartticker/refs/heads/main/help/HELPME.{language}.md";
            Assert.Equal(expectedPath, HelpWindow.HelpUriFor(language).AbsolutePath);
            if (language != AppLanguages.Default)
            {
                Assert.NotEqual(english, source);
                Assert.Contains(
                    $"https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/help/HELPME.{language}.md",
                    source);
                Assert.DoesNotContain(
                    "https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/HELPME.md",
                    source);
            }
        }

        var resources = typeof(HelpWindow).Assembly.GetManifestResourceNames()
            .Where(name => name.Contains(".HELPME", StringComparison.OrdinalIgnoreCase) &&
                name.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        Assert.Equal(AppLanguages.Supported.Count, resources.Length);
    }

    [Fact]
    public void OpenHelp_ReloadsEmbeddedContentWhenTheAppLanguageChanges()
    {
        var code = File.ReadAllText(DesktopPath("Views", "HelpWindow.axaml.cs"));

        Assert.Contains("PropertyChanged += OnViewModelPropertyChanged", code);
        Assert.Contains("nameof(MainViewModel.Language)", code);
        Assert.Contains("ReloadHelp();", code);
        Assert.Contains("RenderHelp(ReadEmbeddedHelp(language), language);", code);
        Assert.Contains("generation != Volatile.Read(ref _loadGeneration)", code);
    }

    [Fact]
    public void EveryConfigurationSurface_OpensHelpWindow()
    {
        var windowNames = new[]
        {
            "MainWindow", "SettingsWindow", "AppSettingsWindow", "AlertsWindow", "QuoteGroupsWindow",
        };
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
        var window = XDocument.Load(DesktopPath("Views", "HelpWindow.axaml"));
        var code = File.ReadAllText(DesktopPath("Views", "HelpWindow.axaml.cs"));

        Assert.Equal("True", (string?)window.Root!.Attribute("Topmost"));
        Assert.Contains("window.Activate();", code);
        Assert.Equal(
            "https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/HELPME.md",
            HelpWindow.HelpUriFor("en").AbsoluteUri);
        Assert.Equal(
            "https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/help/HELPME.de.md",
            HelpWindow.HelpUriFor("de").AbsoluteUri);
        Assert.Contains("HelpUriFor(language)", code);
    }

    [Fact]
    public void MarkdownRenderer_FormatsHeadingsTablesCodeAndLinks()
    {
        const string markdown = """
        # Guide

        ## Configure

        Read **carefully** and open [support](https://example.com).

        | Name | Value |
        | --- | --- |
        | Mode | Static |

        ```text
        sample
        ```
        """;
        Uri? opened = null;

        var rendered = MarkdownHelpRenderer.Render(markdown, _ => { }, uri => opened = uri);
        var controls = Descendants(rendered.Content).ToArray();

        Assert.Equal(["Guide", "Configure"], rendered.Headings.Select(item => item.Title));
        Assert.Contains(controls, control => control is Grid { ColumnDefinitions.Count: 2 });
        Assert.Contains(controls, control => control is SelectableTextBlock { Text: var text } && text?.Contains("sample") == true);
        var link = Assert.Single(controls.OfType<Button>(), button => Equals(button.Content, "support"));

        link.Command?.Execute(link.CommandParameter);
        link.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));

        Assert.Equal(new Uri("https://example.com"), opened);
    }

    [Fact]
    public void MarkdownRenderer_RendersCompleteGuideWithNavigatorTargets()
    {
        var source = File.ReadAllText(RepositoryPath("HELPME.md"));

        var rendered = MarkdownHelpRenderer.Render(source, _ => { }, _ => { });

        Assert.True(rendered.Headings.Count >= 40);
        Assert.Contains(rendered.Headings, heading => heading.Anchor == "main-ticker-controls");
        Assert.Contains(rendered.Headings, heading => heading.Anchor == "group-quotes");
        Assert.Contains(rendered.Headings, heading => heading.Anchor == "troubleshooting");
        Assert.True(Descendants(rendered.Content).OfType<Grid>().Count() >= 10);
    }

    [Fact]
    public void MarkdownRenderer_PreservesCombiningMarksInLocalizedAnchors()
    {
        const string markdown = "## सहायता खोलें";

        var rendered = MarkdownHelpRenderer.Render(markdown, _ => { }, _ => { });

        Assert.Equal("सहायता-खोलें", Assert.Single(rendered.Headings).Anchor);
    }

    [Fact]
    public void HelpWindow_UsesFormattedContentAndSectionNavigation()
    {
        var document = XDocument.Load(DesktopPath("Views", "HelpWindow.axaml"));

        Assert.Contains(document.Descendants(), element => (string?)element.Attribute("Name") == "NavigationPanel");
        Assert.Contains(document.Descendants(), element => (string?)element.Attribute("Name") == "HelpContentHost");
        Assert.DoesNotContain(document.Descendants(), element => (string?)element.Attribute("Name") == "HelpText");
        Assert.DoesNotContain(document.Descendants(), element =>
            (string?)element.Attribute("Content") is "Reload" or "Open online" or "Close");
    }

    private static IEnumerable<Control> Descendants(Control control)
    {
        yield return control;
        var children = control switch
        {
            Panel panel => panel.Children.Cast<Control>(),
            Border { Child: { } child } => [child],
            ScrollViewer { Content: Control child } => [child],
            ContentControl { Content: Control child } => [child],
            TextBlock { Inlines: { } inlines } => inlines
                .OfType<InlineUIContainer>()
                .Select(container => container.Child),
            _ => [],
        };
        foreach (var child in children)
        {
            foreach (var descendant in Descendants(child))
            {
                yield return descendant;
            }
        }
    }

    private static string DesktopPath(params string[] parts)
        => RepositoryPath(["windows", "src", "SmartTicker.Desktop", .. parts]);

    private static (int H1, int H2, int H3, int Fences, int Tables) StructureOf(string markdown) =>
        (
            Regex.Matches(markdown, "(?m)^# ").Count,
            Regex.Matches(markdown, "(?m)^## ").Count,
            Regex.Matches(markdown, "(?m)^### ").Count,
            Regex.Matches(markdown, "(?m)^```").Count,
            Regex.Matches(markdown, "(?m)^\\| ---").Count);

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