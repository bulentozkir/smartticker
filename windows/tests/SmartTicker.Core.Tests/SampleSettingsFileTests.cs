using SmartTicker.Core.Models;
using SmartTicker.Core.Services;

namespace SmartTicker.Core.Tests;

public sealed class SampleSettingsFileTests
{
    private static string SamplePath()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "samples")))
        {
            directory = directory.Parent;
        }

        Assert.NotNull(directory);
        return Path.Combine(directory!.FullName, "samples", "smartticker-settings.sample.json");
    }

    [Fact]
    public void SampleFile_PassesImportValidation()
    {
        var result = SettingsImportValidator.Validate(File.ReadAllText(SamplePath()));

        Assert.True(result.Success, string.Join(Environment.NewLine, result.Errors));
        Assert.Equal(20, result.Settings!.Subscriptions.Length);
        Assert.All(result.Settings.Subscriptions, item => Assert.True(item.CollectPrice && item.CollectNews));
        Assert.Equal(20, result.Settings.Subscriptions.Select(item => item.Id).Distinct().Count());
        Assert.All(result.Settings.Subscriptions, item => Assert.False(string.IsNullOrWhiteSpace(item.CssSelector)));
        Assert.All(result.Settings.Subscriptions, item => Assert.False(string.IsNullOrWhiteSpace(item.NewsCssSelector)));
        Assert.True(result.Settings.AllowWebsiteCookiesAndCrossHostRedirects);
        Assert.Empty(result.Settings.AcknowledgedSources);
    }

    // A colour pinned here overrides the app default on import, so the sample must not fall behind.
    [Fact]
    public void SampleFile_UsesTheCurrentDefaultColors()
    {
        var settings = SettingsImportValidator.Validate(File.ReadAllText(SamplePath())).Settings;

        Assert.NotNull(settings);
        Assert.Equal(SmartTickerSettings.DefaultBackgroundColor, settings!.BackgroundColor);
        Assert.Equal(SmartTickerSettings.DefaultSymbolColor, settings.SymbolColor);
        Assert.Equal(SmartTickerSettings.DefaultPriceColor, settings.PriceColor);
        Assert.Equal(SmartTickerSettings.DefaultExtendedPriceColor, settings.ExtendedPriceColor);
        Assert.Equal(SmartTickerSettings.DefaultNewsColor, settings.NewsColor);
        Assert.Equal(SmartTickerSettings.DefaultNewsColor2, settings.NewsColor2);
        Assert.Equal(SmartTickerSettings.DefaultNewsColor3, settings.NewsColor3);
        Assert.Equal(SmartTickerSettings.DefaultNewsColor4, settings.NewsColor4);
        Assert.Equal(SmartTickerSettings.DefaultPriceUpColor, settings.PriceUpColor);
        Assert.Equal(SmartTickerSettings.DefaultPriceDownColor, settings.PriceDownColor);
    }
}
