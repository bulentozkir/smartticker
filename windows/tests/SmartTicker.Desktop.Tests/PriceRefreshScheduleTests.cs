using SmartTicker.Desktop.Views;

namespace SmartTicker.Desktop.Tests;

public sealed class PriceRefreshScheduleTests
{
    [Fact]
    public void SixtyQuotesOverThirtySeconds_RefreshesTwoUniqueQuotesPerSecond()
    {
        var schedule = new PriceRefreshSchedule();
        var subscriptions = Enumerable.Range(0, 60).Select(_ => Guid.NewGuid()).ToArray();

        var batches = Enumerable.Range(0, 30)
            .Select(_ => schedule.NextBatch(subscriptions, 30).ToArray())
            .ToArray();

        Assert.All(batches, batch => Assert.Equal(2, batch.Length));
        Assert.Equal(subscriptions, batches.SelectMany(batch => batch));
        Assert.Equal(batches[0], schedule.NextBatch(subscriptions, 30));
    }

    [Fact]
    public void FewerQuotesThanSeconds_SpreadsThemAcrossTheWholeInterval()
    {
        var schedule = new PriceRefreshSchedule();
        var subscriptions = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToArray();

        var batches = Enumerable.Range(0, 30)
            .Select(_ => schedule.NextBatch(subscriptions, 30).ToArray())
            .ToArray();

        Assert.Equal([0, 6, 12, 18, 24], batches
            .Select((batch, slot) => (batch, slot))
            .Where(item => item.batch.Length > 0)
            .Select(item => item.slot));
        Assert.Equal(subscriptions, batches.SelectMany(batch => batch));
    }

    [Fact]
    public void ChangedInputs_RestartAtTheFirstSlot()
    {
        var schedule = new PriceRefreshSchedule();
        var subscriptions = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToArray();
        _ = schedule.NextBatch(subscriptions, 30);
        _ = schedule.NextBatch(subscriptions, 30);

        var changed = subscriptions.Reverse().ToArray();

        Assert.Equal([changed[0]], schedule.NextBatch(changed, 30));
    }
}