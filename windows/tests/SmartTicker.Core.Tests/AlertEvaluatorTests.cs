using SmartTicker.Core.Models;
using SmartTicker.Core.Services;

namespace SmartTicker.Core.Tests;

public sealed class AlertEvaluatorTests
{
    private static readonly DateTimeOffset Now = new(2026, 8, 30, 12, 0, 0, TimeSpan.Zero);

    private static AlertRule Rule(
        AlertComparison comparison = AlertComparison.GreaterThanOrEqual,
        decimal threshold = 100m,
        DateTimeOffset? starts = null,
        DateTimeOffset? ends = null,
        bool enabled = true) => new()
        {
            Id = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Symbol = "MSFT",
            Comparison = comparison,
            Threshold = threshold,
            StartsOn = starts,
            EndsOn = ends,
            Enabled = enabled,
        };

    [Theory]
    [InlineData(AlertComparison.LessThan, 99, true)]
    [InlineData(AlertComparison.LessThan, 100, false)]
    [InlineData(AlertComparison.LessThanOrEqual, 100, true)]
    [InlineData(AlertComparison.GreaterThan, 101, true)]
    [InlineData(AlertComparison.GreaterThan, 100, false)]
    [InlineData(AlertComparison.GreaterThanOrEqual, 100, true)]
    [InlineData(AlertComparison.EqualTo, 100, true)]
    [InlineData(AlertComparison.EqualTo, 100.01, false)]
    [InlineData(AlertComparison.NotEqualTo, 100.01, true)]
    public void Matches_HonoursEachComparison(AlertComparison comparison, decimal price, bool expected)
    {
        Assert.Equal(expected, AlertEvaluator.Matches(Rule(comparison), price));
    }

    [Fact]
    public void ShouldFire_IsFalseBeforeTheStartDate()
    {
        var rule = Rule(starts: Now.AddDays(1));

        Assert.False(AlertEvaluator.ShouldFire(rule, 150m, Now));
    }

    [Fact]
    public void ShouldFire_IsFalseAfterTheEndDate()
    {
        var rule = Rule(ends: Now.AddDays(-1));

        Assert.False(AlertEvaluator.ShouldFire(rule, 150m, Now));
    }

    [Fact]
    public void ShouldFire_IsTrueWhenTheRuleNeverExpires()
    {
        var rule = Rule();

        Assert.True(AlertEvaluator.ShouldFire(rule, 150m, Now.AddYears(5)));
    }

    [Fact]
    public void ShouldFire_IsFalseWhenDisabled()
    {
        var rule = Rule(enabled: false);

        Assert.False(AlertEvaluator.ShouldFire(rule, 150m, Now));
    }

    [Fact]
    public void ShouldFire_IsTrueInsideTheWindow()
    {
        var rule = Rule(starts: Now.AddDays(-1), ends: Now.AddDays(1));

        Assert.True(AlertEvaluator.ShouldFire(rule, 150m, Now));
    }

    [Fact]
    public void HasExpired_OnlyWhenAnEndDatePassed()
    {
        Assert.False(AlertEvaluator.HasExpired(Rule(), Now));
        Assert.True(AlertEvaluator.HasExpired(Rule(ends: Now.AddSeconds(-1)), Now));
    }

    [Fact]
    public void Normalize_ClampsTheBlinkDuration()
    {
        Assert.Equal(
            AlertSettings.MinimumBlinkSeconds,
            (AlertSettings.Default with { BlinkSeconds = 0 }).Normalize().BlinkSeconds);
        Assert.Equal(
            AlertSettings.MaximumBlinkSeconds,
            (AlertSettings.Default with { BlinkSeconds = 100000 }).Normalize().BlinkSeconds);
    }

    [Fact]
    public void Defaults_MatchTheRequestedBehaviour()
    {
        Assert.True(AlertSettings.Default.SoundEnabled);
        Assert.Equal(60, AlertSettings.DefaultBlinkSeconds);
        Assert.Equal(15, AlertSettings.DefaultBuzzCount);
        Assert.Equal(15, AlertSettings.Default.BuzzCount);
    }

    [Fact]
    public void StateProperties_DistinguishEnabledFromDisabled()
    {
        var enabled = Rule();
        var disabled = Rule(enabled: false);

        Assert.NotEqual(enabled.StateGlyph, disabled.StateGlyph);
        Assert.NotEqual(enabled.StateColor, disabled.StateColor);
        Assert.Equal("Enabled", enabled.StateText);
        Assert.Equal("Disabled", disabled.StateText);
    }

    // The button names the action, so it must read the opposite of the current state.
    [Fact]
    public void ToggleActionText_NamesTheAction()
    {
        Assert.Equal("Disable", Rule().ToggleActionText);
        Assert.Equal("Enable", Rule(enabled: false).ToggleActionText);
    }

    [Theory]
    [InlineData(0, AlertSettings.MinimumBuzzCount)]
    [InlineData(999, AlertSettings.MaximumBuzzCount)]
    [InlineData(3, 3)]
    public void Normalize_ClampsTheBuzzCount(int supplied, int expected)
    {
        Assert.Equal(expected, (AlertSettings.Default with { BuzzCount = supplied }).Normalize().BuzzCount);
    }
}
