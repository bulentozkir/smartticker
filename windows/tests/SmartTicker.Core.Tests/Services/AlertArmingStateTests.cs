using SmartTicker.Core.Models;
using SmartTicker.Core.Services;

namespace SmartTicker.Core.Tests.Services;

public class AlertArmingStateTests
{
    private static readonly DateTimeOffset Now = new(2026, 3, 2, 15, 0, 0, TimeSpan.Zero);

    private static AlertRule Rule(bool enabled = true) => new()
    {
        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        SubscriptionId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        Symbol = "MSFT",
        Comparison = AlertComparison.GreaterThan,
        Threshold = 100m,
        Enabled = enabled,
    };

    [Fact]
    public void ShouldNotify_IsTrueOnlyOnTheRisingEdge()
    {
        var state = new AlertArmingState();

        Assert.True(state.ShouldNotify(Rule(), 150m, Now));
        Assert.False(state.ShouldNotify(Rule(), 150m, Now));
        Assert.False(state.ShouldNotify(Rule(), 160m, Now));
    }

    [Fact]
    public void ShouldNotify_FiresAgainAfterTheConditionClears()
    {
        var state = new AlertArmingState();

        Assert.True(state.ShouldNotify(Rule(), 150m, Now));
        Assert.False(state.ShouldNotify(Rule(), 50m, Now));
        Assert.True(state.ShouldNotify(Rule(), 150m, Now));
    }

    [Fact]
    public void ShouldNotify_FiresAgainAfterDisableThenEnable_WhenNoRefreshHappensInBetween()
    {
        var state = new AlertArmingState();
        Assert.True(state.ShouldNotify(Rule(), 150m, Now));

        // The user toggles the rule off and back on without a price refresh in between.
        state.Rearm(Rule().Id);

        Assert.True(state.ShouldNotify(Rule(), 150m, Now));
    }

    [Fact]
    public void ShouldNotify_DisabledRuleNeverNotifiesAndReArms()
    {
        var state = new AlertArmingState();
        Assert.True(state.ShouldNotify(Rule(), 150m, Now));

        Assert.False(state.ShouldNotify(Rule(enabled: false), 150m, Now));
        Assert.True(state.IsArmed(Rule().Id));
    }

    [Fact]
    public void IsFiring_TracksTheCurrentMatchedState()
    {
        var state = new AlertArmingState();
        Assert.False(state.IsFiring(Rule().Id));

        state.ShouldNotify(Rule(), 150m, Now);
        Assert.True(state.IsFiring(Rule().Id));

        state.ShouldNotify(Rule(), 10m, Now);
        Assert.False(state.IsFiring(Rule().Id));
    }
}
