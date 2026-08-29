using SmartTicker.Core.Models;
using SmartTicker.Core.Services;

namespace SmartTicker.Core.Tests;

public sealed class NewsRepeatFilterTests
{
    private static NewsHeadline[] Headlines(params string[] titles) =>
        titles.Select(title => new NewsHeadline(title, null)).ToArray();

    [Fact]
    public void Filter_RetiresHeadlineAfterConfiguredNumberOfShowings()
    {
        var filter = new NewsRepeatFilter();
        var id = Guid.NewGuid();
        var headlines = Headlines("Markets close higher on tech gains");

        for (var showing = 1; showing <= 5; showing++)
        {
            Assert.Single(filter.Filter(id, headlines, 5));
        }

        Assert.Empty(filter.Filter(id, headlines, 5));
    }

    [Fact]
    public void Filter_HonoursPerEntryLimit()
    {
        var filter = new NewsRepeatFilter();
        var id = Guid.NewGuid();
        var headlines = Headlines("Central bank holds rates steady");

        Assert.Single(filter.Filter(id, headlines, 2));
        Assert.Single(filter.Filter(id, headlines, 2));
        Assert.Empty(filter.Filter(id, headlines, 2));
    }

    [Fact]
    public void Filter_CountsEachSubscriptionIndependently()
    {
        var filter = new NewsRepeatFilter();
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();
        var headlines = Headlines("Shared headline across two sources");

        filter.Filter(first, headlines, 1);

        Assert.Empty(filter.Filter(first, headlines, 1));
        Assert.Single(filter.Filter(second, headlines, 1));
    }

    [Fact]
    public void Filter_KeepsNewHeadlinesWhenOthersAreRetired()
    {
        var filter = new NewsRepeatFilter();
        var id = Guid.NewGuid();
        filter.Filter(id, Headlines("Original headline about markets"), 1);

        var visible = filter.Filter(id, Headlines("Original headline about markets", "Fresh headline about rates"), 1);

        Assert.Equal("Fresh headline about rates", Assert.Single(visible).Title);
    }

    [Fact]
    public void Forget_ResetsCountsForSubscription()
    {
        var filter = new NewsRepeatFilter();
        var id = Guid.NewGuid();
        var headlines = Headlines("Quarterly earnings beat expectations");
        filter.Filter(id, headlines, 1);

        filter.Forget(id);

        Assert.Single(filter.Filter(id, headlines, 1));
    }

    [Fact]
    public void Subscription_DefaultsToFiveShowingsAndClamps()
    {
        TickerSubscription.TryCreate(
            "MSFT", "Example", "https://example.com/quote/MSFT", false, true, null, "a.headline",
            out var subscription, out _);

        Assert.Equal(5, subscription!.NewsRepeatLimit);
        Assert.Equal(1, subscription.WithNewsRepeatLimit(0).NewsRepeatLimit);
        Assert.Equal(100, subscription.WithNewsRepeatLimit(500).NewsRepeatLimit);
        Assert.Equal(12, subscription.WithNewsRepeatLimit(12).NewsRepeatLimit);
    }
}
