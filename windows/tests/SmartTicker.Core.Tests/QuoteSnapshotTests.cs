using SmartTicker.Core.Models;

namespace SmartTicker.Core.Tests;

public sealed class QuoteSnapshotTests
{
    [Fact]
    public void Snapshot_KeepsDuplicateSymbolsSeparatedBySubscription()
    {
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();
        var observedAt = DateTimeOffset.UtcNow;

        var first = new QuoteSnapshot(firstId, "MSFT", "Source A", 100m, "USD", observedAt, true, "OK");
        var second = new QuoteSnapshot(secondId, "MSFT", "Source B", 101m, "USD", observedAt, true, "OK");

        Assert.Equal(first.Symbol, second.Symbol);
        Assert.NotEqual(first.SubscriptionId, second.SubscriptionId);
        Assert.NotEqual(first.Price, second.Price);
    }
}