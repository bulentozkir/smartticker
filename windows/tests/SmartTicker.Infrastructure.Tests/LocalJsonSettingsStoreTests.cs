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
            subscription = subscription!.WithNewsRepeatLimit(9) with { GroupName = "Mega-Cap Tech" };
            var expected = new SmartTickerSettings(1, [subscription!], 3, 2, 65, 30)
            {
                UseStaticGroupedView = true,
                AlertBlinkColor = "#123ABC",
                QuoteGroupNames = ["Mega-Cap Tech", "Empty"],
            };
            var store = new LocalJsonSettingsStore(path);

            store.Save(expected);
            var actual = store.Load();

            Assert.Equal(3, actual.PriceRowCount);
            Assert.Equal(2, actual.NewsRowCount);
            Assert.Equal(65, actual.PriceScrollSpeed);
            Assert.Equal(30, actual.NewsScrollSpeed);
            Assert.True(actual.UseStaticGroupedView);
            Assert.Equal("#123ABC", actual.AlertBlinkColor);
            Assert.Equal(["Mega-Cap Tech", "Empty"], actual.QuoteGroupNames);
            var restored = Assert.Single(actual.Subscriptions);
            Assert.Equal(subscription, restored);
            Assert.Equal("Mega-Cap Tech", restored.GroupName);
            Assert.Equal(9, restored.NewsRepeatLimit);
            Assert.Contains("\"priceScrollSpeed\": 65", File.ReadAllText(path));
            Assert.Contains("\"alertBlinkColor\": \"#123ABC\"", File.ReadAllText(path));
            Assert.Contains("\"quoteGroups\"", File.ReadAllText(path));
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
        Assert.True(settings.ShowPriceLine);
        Assert.False(settings.ShowNewsLine);
        Assert.False(settings.UseStaticGroupedView);
    }

    [Fact]
    public void Load_SettingsWithoutAlertBlinkColorDefaultsToMagenta()
    {
        var directory = Path.Combine(Path.GetTempPath(), "SmartTicker.Tests", Guid.NewGuid().ToString("N"));
        var path = Path.Combine(directory, "settings.json");
        Directory.CreateDirectory(directory);

        try
        {
            File.WriteAllText(
                path,
                """{"version":1,"subscriptions":[],"priceRowCount":1,"newsRowCount":1,"priceScrollSpeed":50,"newsScrollSpeed":40}""");

            var settings = new LocalJsonSettingsStore(path).Load();

            Assert.Equal(SmartTickerSettings.DefaultAlertBlinkColor, settings.AlertBlinkColor);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }
}