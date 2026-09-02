using SmartTicker.Desktop.Views;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Tests;

public sealed class StaggeredRefreshScheduleTests
{
    [Fact]
    public void SixtyQuotesOverThirtySeconds_RefreshesTwoUniqueQuotesPerSecond()
    {
        var schedule = new StaggeredRefreshSchedule();
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
        var schedule = new StaggeredRefreshSchedule();
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
        var schedule = new StaggeredRefreshSchedule();
        var subscriptions = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToArray();
        _ = schedule.NextBatch(subscriptions, 30);
        _ = schedule.NextBatch(subscriptions, 30);

        var changed = subscriptions.Reverse().ToArray();

        Assert.Equal([changed[0]], schedule.NextBatch(changed, 30));
    }
}

public sealed class RefreshWorkCoordinatorTests
{
    [Fact]
    public void Admission_IsBoundedDeduplicatedAndImmediatelyReusable()
    {
        var coordinator = new RefreshWorkCoordinator(4);
        var subscriptions = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToArray();
        var leases = subscriptions
            .Take(4)
            .Select(id => coordinator.TryAcquire(RefreshStream.Prices, id))
            .ToArray();

        Assert.All(leases, Assert.NotNull);
        Assert.Equal(4, coordinator.ActiveCount);
        Assert.Null(coordinator.TryAcquire(RefreshStream.Prices, subscriptions[0]));
        Assert.Null(coordinator.TryAcquire(RefreshStream.News, subscriptions[4]));

        leases[0]!.Dispose();

        using var replacement = coordinator.TryAcquire(RefreshStream.News, subscriptions[4]);
        Assert.NotNull(replacement);
        Assert.Equal(4, coordinator.ActiveCount);

        foreach (var lease in leases.Skip(1))
        {
            lease!.Dispose();
        }
    }

    [Fact]
    public void SameSubscription_CanRefreshPricesAndNewsIndependently()
    {
        var coordinator = new RefreshWorkCoordinator(4);
        var subscriptionId = Guid.NewGuid();

        using var price = coordinator.TryAcquire(RefreshStream.Prices, subscriptionId);
        using var news = coordinator.TryAcquire(RefreshStream.News, subscriptionId);

        Assert.NotNull(price);
        Assert.NotNull(news);
        Assert.Equal(2, coordinator.ActiveCount);
    }
}