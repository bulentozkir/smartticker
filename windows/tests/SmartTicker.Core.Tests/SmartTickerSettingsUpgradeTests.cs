using SmartTicker.Core.Models;

namespace SmartTicker.Core.Tests;

public sealed class SmartTickerSettingsUpgradeTests
{
    [Theory]
    [InlineData("#70E1A1")]
    [InlineData("#79C0FF")]
    [InlineData("#79c0ff")]
    public void UpgradeDefaults_ReplacesEveryRetiredPriceColor(string stored)
    {
        var settings = SmartTickerSettings.Default with { PriceColor = stored };

        Assert.Equal(SmartTickerSettings.DefaultPriceColor, settings.UpgradeDefaults().PriceColor);
    }

    [Fact]
    public void UpgradeDefaults_KeepsAColorThePersonChose()
    {
        var settings = SmartTickerSettings.Default with { PriceColor = "#FF00FF" };

        Assert.Equal("#FF00FF", settings.UpgradeDefaults().PriceColor);
    }

    [Fact]
    public void DefaultPriceColor_DiffersFromTheRisingChangeColor()
    {
        Assert.NotEqual(SmartTickerSettings.DefaultPriceUpColor, SmartTickerSettings.DefaultPriceColor);
        Assert.NotEqual(SmartTickerSettings.DefaultPriceDownColor, SmartTickerSettings.DefaultPriceColor);
    }

    // The three price-row runs are drawn side by side, so none may share a colour.
    [Fact]
    public void PriceRowDefaults_AreAllDistinct()
    {
        string[] colors =
        [
            SmartTickerSettings.DefaultSymbolColor,
            SmartTickerSettings.DefaultPriceColor,
            SmartTickerSettings.DefaultExtendedPriceColor,
            SmartTickerSettings.DefaultPriceUpColor,
            SmartTickerSettings.DefaultPriceDownColor,
        ];

        Assert.Equal(colors.Length, colors.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }
}
