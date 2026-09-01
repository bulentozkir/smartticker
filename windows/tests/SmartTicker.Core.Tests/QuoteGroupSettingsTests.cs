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
}