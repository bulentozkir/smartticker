using SmartTicker.Core.Models;
using SmartTicker.Core.Services;

namespace SmartTicker.Core.Tests;

public sealed class BackgroundOpacityTests
{
    [Theory]
    [InlineData(0.0, SmartTickerSettings.MinimumOpacity)]
    [InlineData(2.5, SmartTickerSettings.MaximumOpacity)]
    [InlineData(0.6, 0.6)]
    public void Normalize_ClampsOpacityIntoTheLegibleRange(double supplied, double expected)
    {
        var settings = (SmartTickerSettings.Default with { BackgroundOpacity = supplied }).Normalize();

        Assert.Equal(expected, settings.BackgroundOpacity, 3);
    }

    [Fact]
    public void Normalize_ReplacesNonFiniteOpacityWithTheDefault()
    {
        var settings = (SmartTickerSettings.Default with { BackgroundOpacity = double.NaN }).Normalize();

        Assert.Equal(SmartTickerSettings.DefaultOpacity, settings.BackgroundOpacity, 3);
    }

    [Fact]
    public void Import_ReadsBackgroundOpacity()
    {
        var result = SettingsImportValidator.Validate(
            """{"version":1,"subscriptions":[],"priceRowCount":1,"newsRowCount":1,"priceScrollSpeed":50,"newsScrollSpeed":40,"backgroundOpacity":0.65}""");

        Assert.Empty(result.Errors);
        Assert.Equal(0.65, result.Settings!.BackgroundOpacity, 3);
    }

    [Fact]
    public void Import_RejectsAnOpacityOutsideTheRange()
    {
        var result = SettingsImportValidator.Validate(
            """{"version":1,"subscriptions":[],"priceRowCount":1,"newsRowCount":1,"priceScrollSpeed":50,"newsScrollSpeed":40,"backgroundOpacity":9}""");

        Assert.Contains(result.Errors, error => error.Contains("backgroundOpacity", StringComparison.Ordinal));
    }

    [Fact]
    public void Import_DefaultsOpacityWhenAbsent()
    {
        var result = SettingsImportValidator.Validate(
            """{"version":1,"subscriptions":[],"priceRowCount":1,"newsRowCount":1,"priceScrollSpeed":50,"newsScrollSpeed":40}""");

        Assert.Empty(result.Errors);
        Assert.Equal(SmartTickerSettings.DefaultOpacity, result.Settings!.BackgroundOpacity, 3);
    }
}
