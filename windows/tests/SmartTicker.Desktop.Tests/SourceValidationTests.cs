using SmartTicker.Core.Models;
using SmartTicker.Core.Services;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Tests;

public sealed class SourceValidationTests
{
    [Fact]
    public void LoadSettings_AppliesWebsiteAccessSettingToSharedPolicy()
    {
        var store = new TestSettingsStore(SmartTickerSettings.Default with
        {
            AllowWebsiteCookiesAndCrossHostRedirects = true,
        });
        var policy = new WebsiteAccessPolicy();
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: store,
            websiteAccessPolicy: policy);

        Assert.True(viewModel.AllowWebsiteCookiesAndCrossHostRedirects);
        Assert.True(policy.AllowCookiesAndCrossHostRedirects);

        viewModel.AllowWebsiteCookiesAndCrossHostRedirects = false;

        Assert.False(policy.AllowCookiesAndCrossHostRedirects);
        Assert.False(store.Saved!.AllowWebsiteCookiesAndCrossHostRedirects);
    }

    [Fact]
    public void GetPendingSourcePermissionReviews_GroupsEntriesByHost()
    {
        var settings = SmartTickerSettings.Default with
        {
            Subscriptions =
            [
                Subscription("MSFT", "Yahoo Finance", "https://finance.yahoo.com/quote/MSFT/"),
                Subscription("AAPL", "Yahoo Finance", "https://finance.yahoo.com/quote/AAPL/"),
                Subscription("Gold", "Trading Economics", "https://tradingeconomics.com/commodity/gold"),
            ],
            AcknowledgedSources = ["tradingeconomics.com"],
        };
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: new TestSettingsStore(settings));

        var review = Assert.Single(viewModel.GetPendingSourcePermissionReviews());

        Assert.Equal("finance.yahoo.com", review.Host);
        Assert.Contains("MSFT", review.Symbols);
        Assert.Contains("AAPL", review.Symbols);
        Assert.Equal("Written permission required", review.PolicySummary);
    }

    [Fact]
    public async Task ValidateAllSourcesAsync_TestsApprovedEntriesAndSkipsOthers()
    {
        var yahoo = Subscription("MSFT", "Yahoo Finance", "https://finance.yahoo.com/quote/MSFT/");
        var tradingEconomics = Subscription(
            "Gold",
            "Trading Economics",
            "https://tradingeconomics.com/commodity/gold");
        var settings = SmartTickerSettings.Default with
        {
            Subscriptions = [yahoo, tradingEconomics],
            AcknowledgedSources = ["finance.yahoo.com"],
        };
        var quoteFetcher = new SuccessfulQuoteFetcher();
        var newsFetcher = new SuccessfulNewsFetcher();
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: quoteFetcher,
            settingsStore: new TestSettingsStore(settings),
            newsFetcher: newsFetcher);

        await viewModel.ValidateAllSourcesAsync();

        Assert.Equal([yahoo.Id], quoteFetcher.Requests);
        Assert.Equal([yahoo.Id], newsFetcher.Requests);
        Assert.Equal("Validation complete: 1 passed, 0 failed, 1 skipped.", viewModel.SourceValidationStatus);
        Assert.Contains(viewModel.SourceValidationProblems, item =>
            item == "Gold: source permission was not approved.");
    }

    [Fact]
    public async Task WebsiteAccessToggle_ControlsRefreshAndPerHostReview()
    {
        var yahoo = Subscription("MSFT", "Yahoo Finance", "https://finance.yahoo.com/quote/MSFT/");
        var settings = SmartTickerSettings.Default with { Subscriptions = [yahoo] };
        var quoteFetcher = new SuccessfulQuoteFetcher();
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: quoteFetcher,
            settingsStore: new TestSettingsStore(settings));

        await viewModel.RefreshPricesAsync();

        Assert.Empty(quoteFetcher.Requests);
        Assert.Single(viewModel.GetPendingSourcePermissionReviews());

        viewModel.AllowWebsiteCookiesAndCrossHostRedirects = true;
        await viewModel.RefreshPricesAsync();

        Assert.Equal([yahoo.Id], quoteFetcher.Requests);
        Assert.Empty(viewModel.GetPendingSourcePermissionReviews());
    }

    private static TickerSubscription Subscription(string symbol, string sourceName, string sourceUri) =>
        new(
            Guid.NewGuid(),
            symbol,
            sourceName,
            new Uri(sourceUri),
            CollectPrice: true,
            CollectNews: true,
            CssSelector: ".price",
            NewsCssSelector: "a.news");

    private sealed class TestSettingsStore(SmartTickerSettings settings) : ISettingsStore
    {
        public string FilePath => string.Empty;

        public SmartTickerSettings? Saved { get; private set; }

        public SmartTickerSettings Load() => settings;

        public void Save(SmartTickerSettings saved) => Saved = saved;
    }

    private sealed class SuccessfulQuoteFetcher : IQuoteFetcher
    {
        public List<Guid> Requests { get; } = [];

        public Task<QuoteSnapshot> FetchAsync(
            TickerSubscription subscription,
            CancellationToken cancellationToken = default)
        {
            Requests.Add(subscription.Id);
            return Task.FromResult(new QuoteSnapshot(
                subscription.Id,
                subscription.Symbol,
                subscription.SourceName,
                100m,
                "USD",
                DateTimeOffset.UtcNow,
                true,
                "ok"));
        }
    }

    private sealed class SuccessfulNewsFetcher : INewsFetcher
    {
        public List<Guid> Requests { get; } = [];

        public Task<NewsSnapshot> FetchAsync(
            TickerSubscription subscription,
            CancellationToken cancellationToken = default)
        {
            Requests.Add(subscription.Id);
            return Task.FromResult(new NewsSnapshot(
                subscription.Id,
                subscription.Symbol,
                subscription.SourceName,
                [new NewsHeadline("Headline", new Uri("https://example.com/news"))],
                DateTimeOffset.UtcNow,
                true,
                "ok"));
        }
    }
}