using SmartTicker.Core.Models;
using SmartTicker.Infrastructure.Persistence;

namespace SmartTicker.Infrastructure.Tests;

public sealed class LocalJsonAlertStoreTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), $"st-alerts-{Guid.NewGuid():N}.json");

    public void Dispose()
    {
        if (File.Exists(_path))
        {
            File.Delete(_path);
        }
    }

    [Fact]
    public void Load_ReturnsDefaultsWhenTheFileIsMissing()
    {
        var store = new LocalJsonAlertStore(_path);

        var alerts = store.Load();

        Assert.Empty(alerts.Rules);
        Assert.True(alerts.SoundEnabled);
    }

    [Fact]
    public void SaveThenLoad_RoundTripsEveryField()
    {
        var store = new LocalJsonAlertStore(_path);
        var rule = new AlertRule
        {
            Id = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Symbol = "PPLT",
            Comparison = AlertComparison.LessThan,
            Threshold = 1234.5m,
            StartsOn = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            EndsOn = null,
            Enabled = false,
        };

        store.Save(new AlertSettings { Rules = [rule], SoundEnabled = false, BlinkSeconds = 120, BuzzCount = 9 });
        var loaded = store.Load();

        var actual = Assert.Single(loaded.Rules);
        Assert.Equal(rule.Id, actual.Id);
        Assert.Equal(rule.SubscriptionId, actual.SubscriptionId);
        Assert.Equal("PPLT", actual.Symbol);
        Assert.Equal(AlertComparison.LessThan, actual.Comparison);
        Assert.Equal(1234.5m, actual.Threshold);
        Assert.Equal(rule.StartsOn, actual.StartsOn);
        Assert.Null(actual.EndsOn);
        Assert.False(actual.Enabled);
        Assert.False(loaded.SoundEnabled);
        Assert.Equal(120, loaded.BlinkSeconds);
        Assert.Equal(9, loaded.BuzzCount);
    }

    // A hand-edited or truncated file must not stop the ticker from starting.
    [Fact]
    public void Load_FallsBackWhenTheFileIsCorrupt()
    {
        File.WriteAllText(_path, "{ this is not json");

        var alerts = new LocalJsonAlertStore(_path).Load();

        Assert.Empty(alerts.Rules);
    }

    [Fact]
    public void AlertsUseTheirOwnFile()
    {
        Assert.EndsWith("alerts.json", new LocalJsonAlertStore().FilePath, StringComparison.Ordinal);
    }
}
