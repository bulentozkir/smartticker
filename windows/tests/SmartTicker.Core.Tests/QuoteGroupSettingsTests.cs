using SmartTicker.Core.Models;
using SmartTicker.Core.Services;

namespace SmartTicker.Core.Tests;

public sealed class QuoteGroupSettingsTests
{
    [Fact]
    public void Normalize_PreservesEmptyGroupsAndInfersLegacyAssignments()
    {
        var assigned = new TickerSubscription(
            Guid.NewGuid(),
            "MSFT",
            "Example",
            new Uri("https://example.com/MSFT"),
            CollectPrice: true,
            CollectNews: false)
        {
            GroupName = "Legacy",
        };
        var settings = SmartTickerSettings.Default with
        {
            Subscriptions = [assigned],
            QuoteGroupNames = ["Watchlist", "watchlist", " "],
        };

        var normalized = settings.Normalize();

        Assert.Equal(["Watchlist", "Legacy"], normalized.QuoteGroupNames);
    }

    [Fact]
    public void Import_PreservesAnEmptyGroupDefinition()
    {
        var result = SettingsImportValidator.Validate(
            """{"version":1,"quoteGroups":["Watchlist"],"subscriptions":[]}""");

        Assert.True(result.Success);
        Assert.Equal(["Watchlist"], result.Settings!.QuoteGroupNames);
    }

    [Theory]
    [InlineData("[\"Tech\",\"tech\"]")]
    [InlineData("[\"   \"]")]
    [InlineData("[42]")]
    public void Import_RejectsInvalidGroupDefinitions(string groups)
    {
        var result = SettingsImportValidator.Validate($"{{\"version\":1,\"quoteGroups\":{groups}}}");

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("quoteGroups", StringComparison.Ordinal));
    }

    [Fact]
    public void Normalize_DropsHiddenNewsQuotesThatNoLongerExist()
    {
        var kept = Guid.NewGuid();
        var subscription = new TickerSubscription(
            kept,
            "MSFT",
            "Example",
            new Uri("https://example.com/MSFT"),
            CollectPrice: true,
            CollectNews: true);
        var settings = SmartTickerSettings.Default with
        {
            Subscriptions = [subscription],
            HiddenNewsQuotes = [kept, kept, Guid.NewGuid()],
        };

        Assert.Equal([kept], settings.Normalize().HiddenNewsQuotes);
    }

    [Fact]
    public void Import_ReadsHiddenNewsQuotes()
    {
        var id = Guid.NewGuid();
        var json = $$"""
        {
          "version": 1,
          "subscriptions": [
            {
              "id": "{{id}}",
              "symbol": "MSFT",
              "sourceUri": "https://example.com/MSFT",
              "collectPrice": true,
              "collectNews": true
            }
          ],
          "hiddenNewsQuotes": ["{{id}}"]
        }
        """;

        var result = SettingsImportValidator.Validate(json);

        Assert.True(result.Success, string.Join(Environment.NewLine, result.Errors));
        Assert.Equal([id], result.Settings!.HiddenNewsQuotes);
    }

    [Fact]
    public void Import_RejectsAnInvalidHiddenNewsQuote()
    {
        var result = SettingsImportValidator.Validate("{\"version\":1,\"hiddenNewsQuotes\":[\"not-a-guid\"]}");

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("hiddenNewsQuotes", StringComparison.Ordinal));
    }
}