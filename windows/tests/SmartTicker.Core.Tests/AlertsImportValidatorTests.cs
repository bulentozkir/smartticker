using SmartTicker.Core.Models;
using SmartTicker.Core.Services;

namespace SmartTicker.Core.Tests.Services;

public class AlertsImportValidatorTests
{
    private static string Serialize(AlertSettings settings) => AlertsJson.Serialize(settings);

    private static AlertRule Rule(string symbol = "MSFT") => new()
    {
        Id = Guid.NewGuid(),
        SubscriptionId = Guid.NewGuid(),
        Symbol = symbol,
        Comparison = AlertComparison.GreaterThan,
        Threshold = 100m,
    };

    [Fact]
    public void Validate_RejectsEmptyInput()
    {
        Assert.False(AlertsImportValidator.Validate(null).Success);
        Assert.False(AlertsImportValidator.Validate("   ").Success);
    }

    [Fact]
    public void Validate_RejectsMalformedJson()
    {
        var result = AlertsImportValidator.Validate("{ not json ");

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("not valid JSON"));
    }

    [Fact]
    public void Validate_AcceptsAnEmptyRuleSet()
    {
        var result = AlertsImportValidator.Validate(Serialize(AlertSettings.Default));

        Assert.True(result.Success);
        Assert.Empty(result.Settings!.Rules);
    }

    [Fact]
    public void Validate_RoundTripsRules()
    {
        var original = AlertSettings.Default with { Rules = [Rule("AAPL")], BuzzCount = 7, BlinkSeconds = 90 };

        var result = AlertsImportValidator.Validate(Serialize(original));

        Assert.True(result.Success);
        Assert.Single(result.Settings!.Rules);
        Assert.Equal("AAPL", result.Settings.Rules[0].Symbol);
        Assert.Equal(7, result.Settings.BuzzCount);
        Assert.Equal(90, result.Settings.BlinkSeconds);
    }

    [Fact]
    public void Validate_RejectsARuleWithoutASymbol()
    {
        var json = Serialize(AlertSettings.Default with { Rules = [Rule() with { Symbol = "  " }] });

        var result = AlertsImportValidator.Validate(json);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("has no symbol"));
    }

    [Fact]
    public void Validate_RejectsAnEmptyId()
    {
        var json = Serialize(AlertSettings.Default with { Rules = [Rule() with { Id = Guid.Empty }] });

        var result = AlertsImportValidator.Validate(json);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("has no id"));
    }

    [Fact]
    public void Validate_RejectsDuplicateIds()
    {
        var rule = Rule();
        var json = Serialize(AlertSettings.Default with { Rules = [rule, rule with { Symbol = "AAPL" }] });

        var result = AlertsImportValidator.Validate(json);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("repeats the id"));
    }

    [Fact]
    public void Validate_RejectsAnUnknownComparison()
    {
        var json = Serialize(AlertSettings.Default with { Rules = [Rule() with { Comparison = (AlertComparison)42 }] });

        var result = AlertsImportValidator.Validate(json);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("unknown comparison"));
    }

    [Fact]
    public void Validate_RejectsAnEndBeforeTheStart()
    {
        var rule = Rule() with
        {
            StartsOn = new DateTimeOffset(2026, 5, 1, 0, 0, 0, TimeSpan.Zero),
            EndsOn = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
        };
        var result = AlertsImportValidator.Validate(Serialize(AlertSettings.Default with { Rules = [rule] }));

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("ends before it starts"));
    }

    [Fact]
    public void Validate_ClampsOutOfRangeBuzzAndBlink()
    {
        var json = Serialize(AlertSettings.Default with { BuzzCount = 999, BlinkSeconds = 1 });

        var result = AlertsImportValidator.Validate(json);

        Assert.True(result.Success);
        Assert.Equal(AlertSettings.MaximumBuzzCount, result.Settings!.BuzzCount);
        Assert.Equal(AlertSettings.MinimumBlinkSeconds, result.Settings.BlinkSeconds);
    }
}
