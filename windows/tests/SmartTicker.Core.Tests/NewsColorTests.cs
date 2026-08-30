using SmartTicker.Core.Models;
using SmartTicker.Core.Services;

namespace SmartTicker.Core.Tests;

public sealed class NewsColorTests
{
    [Fact]
    public void Defaults_AreTheRequestedFourColors()
    {
        Assert.Equal("#FFFFFF", SmartTickerSettings.DefaultNewsColor);
        Assert.Equal("#00E5FF", SmartTickerSettings.DefaultNewsColor2);
        Assert.Equal("#A3E635", SmartTickerSettings.DefaultNewsColor3);
        Assert.Equal("#79C0FF", SmartTickerSettings.DefaultNewsColor4);
    }

    // A repeat inside the cycle would make two headlines in one rotation indistinguishable.
    [Fact]
    public void Defaults_AreAllDistinct()
    {
        string[] cycle =
        [
            SmartTickerSettings.DefaultNewsColor,
            SmartTickerSettings.DefaultNewsColor2,
            SmartTickerSettings.DefaultNewsColor3,
            SmartTickerSettings.DefaultNewsColor4,
        ];

        Assert.Equal(cycle.Length, cycle.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [Fact]
    public void UpgradeDefaults_ReplacesTheRetiredSingleNewsColor()
    {
        var upgraded = (SmartTickerSettings.Default with { NewsColor = "#D8DEE9" }).UpgradeDefaults();

        Assert.Equal(SmartTickerSettings.DefaultNewsColor, upgraded.NewsColor);
    }

    [Fact]
    public void UpgradeDefaults_KeepsAChosenNewsColor()
    {
        var upgraded = (SmartTickerSettings.Default with { NewsColor = "#ABCDEF" }).UpgradeDefaults();

        Assert.Equal("#ABCDEF", upgraded.NewsColor);
    }

    [Fact]
    public void Import_ReadsAllFourNewsColors()
    {
        var result = SettingsImportValidator.Validate(
            """{"version":1,"subscriptions":[],"priceRowCount":1,"newsRowCount":1,"priceScrollSpeed":50,"newsScrollSpeed":40,"newsColor":"#111111","newsColor2":"#222222","newsColor3":"#333333","newsColor4":"#444444"}""");

        Assert.Empty(result.Errors);
        Assert.Equal("#111111", result.Settings!.NewsColor);
        Assert.Equal("#222222", result.Settings.NewsColor2);
        Assert.Equal("#333333", result.Settings.NewsColor3);
        Assert.Equal("#444444", result.Settings.NewsColor4);
    }

    [Fact]
    public void Import_DefaultsTheExtraColorsWhenAbsent()
    {
        var result = SettingsImportValidator.Validate(
            """{"version":1,"subscriptions":[],"priceRowCount":1,"newsRowCount":1,"priceScrollSpeed":50,"newsScrollSpeed":40}""");

        Assert.Empty(result.Errors);
        Assert.Equal(SmartTickerSettings.DefaultNewsColor2, result.Settings!.NewsColor2);
        Assert.Equal(SmartTickerSettings.DefaultNewsColor3, result.Settings.NewsColor3);
        Assert.Equal(SmartTickerSettings.DefaultNewsColor4, result.Settings.NewsColor4);
    }
}
