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
        Assert.All(result.Settings.Subscriptions, item => Assert.False(string.IsNullOrWhiteSpace(item.GroupName)));
        Assert.False(result.Settings.ShowNewsLine);
        Assert.False(result.Settings.UseStaticGroupedView);
        Assert.True(result.Settings.AllowWebsiteCookiesAndCrossHostRedirects);
        Assert.Empty(result.Settings.AcknowledgedSources);

        Assert.Equal(
            new Dictionary<string, int>
            {
                ["Mega-Cap Tech"] = 6,
                ["Precious Metals"] = 8,
                ["Industrial Metals"] = 1,
                ["US Indices"] = 2,
                ["Rates"] = 1,
                ["ETFs"] = 2,
            },
            result.Settings.Subscriptions
                .GroupBy(item => item.GroupName!)
                .ToDictionary(group => group.Key, group => group.Count()));

        var yahoo = result.Settings.Subscriptions
            .Where(item => item.SourceName == "Yahoo Finance")
            .ToArray();
        Assert.Equal(15, yahoo.Length);
        Assert.All(yahoo, item =>
        {
            Assert.Equal("[data-testid=\"qsp-price\"]", item.CssSelector);
            Assert.Equal("section.primary span[data-testid=\"qsp-price-change-percent\"]", item.ChangeCssSelector);
            Assert.Equal("section.secondary span[data-testid=\"qsp-pre-price\"]", item.PreMarketCssSelector);
            Assert.Equal("section.secondary span[data-testid=\"qsp-pre-price-change-percent\"]", item.PreMarketChangeCssSelector);
            Assert.Equal("section.secondary span[data-testid=\"qsp-post-price\"]", item.ExtendedCssSelector);
            Assert.Equal("section.secondary span[data-testid=\"qsp-post-price-change-percent\"]", item.ExtendedChangeCssSelector);
        });

        var tradingEconomics = result.Settings.Subscriptions
            .Where(item => item.SourceName == "Trading Economics")
            .ToArray();
        Assert.Equal(5, tradingEconomics.Length);
        Assert.All(tradingEconomics, item =>
        {
            Assert.Null(item.PreMarketCssSelector);
            Assert.Null(item.PreMarketChangeCssSelector);
            Assert.Null(item.ExtendedCssSelector);
            Assert.Null(item.ExtendedChangeCssSelector);
        });
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
        Assert.Equal(SmartTickerSettings.DefaultAlertBlinkColor, settings.AlertBlinkColor);
    }
}
