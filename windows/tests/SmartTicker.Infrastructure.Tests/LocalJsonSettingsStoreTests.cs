using SmartTicker.Core.Models;
using SmartTicker.Infrastructure.Persistence;

namespace SmartTicker.Infrastructure.Tests;

public sealed class LocalJsonSettingsStoreTests
{
    [Fact]
    public void SaveAndLoad_RoundTripsSubscriptionsAndDisplaySettings()
    {
        var directory = Path.Combine(Path.GetTempPath(), "SmartTicker.Tests", Guid.NewGuid().ToString("N"));
        var path = Path.Combine(directory, "settings.json");

        try
        {
            TickerSubscription.TryCreate(
                "MSFT", "Example", "https://example.com/quote/MSFT", true, true,
                ".price", "a.headline", out var subscription, out _);
            subscription = subscription!.WithNewsRepeatLimit(9);
            var expected = new SmartTickerSettings(1, [subscription!], 3, 2, 65, 30);
            var store = new LocalJsonSettingsStore(path);

            store.Save(expected);
            var actual = store.Load();

            Assert.Equal(3, actual.PriceRowCount);
            Assert.Equal(2, actual.NewsRowCount);
            Assert.Equal(65, actual.PriceScrollSpeed);
            Assert.Equal(30, actual.NewsScrollSpeed);
            var restored = Assert.Single(actual.Subscriptions);
            Assert.Equal(subscription, restored);
            Assert.Equal(9, restored.NewsRepeatLimit);
            Assert.Contains("\"priceScrollSpeed\": 65", File.ReadAllText(path));
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
    }

    [Fact]
    public void Load_MissingFileReturnsDefaults()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "settings.json");

        var settings = new LocalJsonSettingsStore(path).Load();

        Assert.Equal(SmartTickerSettings.CurrentVersion, settings.Version);
        Assert.Empty(settings.Subscriptions);
        Assert.Equal(50, settings.PriceScrollSpeed);
    }
}