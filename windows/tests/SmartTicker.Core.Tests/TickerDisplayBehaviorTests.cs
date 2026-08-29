using SmartTicker.Core.Models;

namespace SmartTicker.Core.Tests;

public sealed class TickerDisplayBehaviorTests
{
    [Fact]
    public void Subscription_PreservesIndependentPriceAndNewsChoices()
    {
        var priceValid = TickerSubscription.TryCreate(
            "MSFT", "Price Source", "https://example.com/price", true, false, null, out var price, out _);
        var newsValid = TickerSubscription.TryCreate(
            "MSFT", "News Source", "https://example.com/news", false, true, null, out var news, out _);

        Assert.True(priceValid);
        Assert.True(newsValid);
        Assert.True(price!.CollectPrice);
        Assert.False(price.CollectNews);
        Assert.False(news!.CollectPrice);
        Assert.True(news.CollectNews);
    }
}