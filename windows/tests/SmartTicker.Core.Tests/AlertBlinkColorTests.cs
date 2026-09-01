using SmartTicker.Core.Models;
using SmartTicker.Core.Services;

namespace SmartTicker.Core.Tests;

public sealed class AlertBlinkColorTests
{
    [Fact]
    public void Default_IsMagenta()
    {
        Assert.Equal("#FF00FF", SmartTickerSettings.DefaultAlertBlinkColor);
        Assert.Equal(SmartTickerSettings.DefaultAlertBlinkColor, SmartTickerSettings.Default.AlertBlinkColor);
    }

    [Fact]
    public void Import_ReadsAChosenColor()
    {
        var result = SettingsImportValidator.Validate("""{"version":1,"alertBlinkColor":"#12AB34"}""");

        Assert.True(result.Success);
        Assert.Equal("#12AB34", result.Settings!.AlertBlinkColor);
    }

    [Fact]
    public void Import_DefaultsToMagentaWhenColorIsAbsent()
    {
        var result = SettingsImportValidator.Validate("""{"version":1}""");

        Assert.True(result.Success);
        Assert.Equal(SmartTickerSettings.DefaultAlertBlinkColor, result.Settings!.AlertBlinkColor);
    }

    [Fact]
    public void Import_RejectsAnInvalidColor()
    {
        var result = SettingsImportValidator.Validate("""{"version":1,"alertBlinkColor":"magenta-ish"}""");

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("alertBlinkColor", StringComparison.Ordinal));
    }
}