using Avalonia.Media;
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
    public void LoadSettings_MigratesLegacyHiddenPricesToTheSelectedViewMode()
    {
        var store = new TestSettingsStore(SmartTickerSettings.Default with
        {
            ShowPriceLine = false,
            ShowNewsLine = false,
        });
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: store);

        Assert.True(viewModel.ShowPriceLine);
        Assert.True(viewModel.IsScrollingPricesOnlyView);
    }

    [Fact]
    public void AlertBlinkColor_LoadsSavesAndResetsWithAppearanceSettings()
    {
        var store = new TestSettingsStore(SmartTickerSettings.Default with
        {
            AlertBlinkColor = "#12AB34",
        });
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: store);

        Assert.Equal("#12AB34", viewModel.AlertBlinkColorHex);
        Assert.Equal(Color.Parse("#12AB34"), Assert.IsType<SolidColorBrush>(viewModel.AlertBlinkBrush).Color);

        viewModel.AlertBlinkColorHex = "#3456CD";

        Assert.Equal("#3456CD", store.Saved!.AlertBlinkColor);
        var exported = SettingsImportValidator.Validate(viewModel.ExportSettingsJson());
        Assert.True(exported.Success);
        Assert.Equal("#3456CD", exported.Settings!.AlertBlinkColor);

        viewModel.ResetColorsCommand.Execute(null);

        Assert.Equal(SmartTickerSettings.DefaultAlertBlinkColor, viewModel.AlertBlinkColorHex);
        Assert.Equal(SmartTickerSettings.DefaultAlertBlinkColor, store.Saved.AlertBlinkColor);
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
        var policy = new WebsiteAccessPolicy();
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: new TestSettingsStore(settings),
            websiteAccessPolicy: policy);

        var review = Assert.Single(viewModel.GetPendingSourcePermissionReviews());

        Assert.Equal("finance.yahoo.com", review.Host);
        Assert.Contains("MSFT", review.Symbols);
        Assert.Contains("AAPL", review.Symbols);
        Assert.Equal("Written permission required", review.PolicySummary);
        Assert.True(policy.AllowsWebsiteSession(new Uri("https://tradingeconomics.com/commodity/gold")));
        Assert.False(policy.AllowsWebsiteSession(review.SourceUri));

        viewModel.ApproveSourcePermission(review);

        Assert.True(policy.AllowsWebsiteSession(review.SourceUri));
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

    [Fact]
    public async Task RefreshPrices_RendersPreMarketBeforePostMarket()
    {
        var subscription = Subscription("MSFT", "Yahoo Finance", "https://finance.yahoo.com/quote/MSFT/") with
        {
            CollectNews = false,
        };
        var settings = SmartTickerSettings.Default with
        {
            Subscriptions = [subscription],
            AcknowledgedSources = ["finance.yahoo.com"],
        };
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: new SessionQuoteFetcher(),
            settingsStore: new TestSettingsStore(settings));

        await viewModel.RefreshPricesAsync();

        var text = string.Concat(viewModel.VisiblePriceRows[0].Segments[0].Runs.Select(run => run.Text));
        Assert.Equal("⊗ MSFT 100.00 USD (+1.00%)  101.00 USD (+0.50%)  99.00 USD (-0.25%)", text);
    }

    [Fact]
    public async Task RefreshPrices_StaticViewGroupsRowsAndDerivesChangeAmount()
    {
        var microsoft = Subscription("MSFT", "Example", "https://example.com/MSFT") with
        {
            GroupName = "Mag 7",
            CollectNews = false,
        };
        var gold = Subscription("GOLD", "Example", "https://example.com/GOLD") with
        {
            GroupName = "Precious Metals",
            CollectNews = false,
        };
        var settings = SmartTickerSettings.Default with
        {
            Subscriptions = [microsoft, gold],
            AcknowledgedSources = ["example.com"],
            UseStaticGroupedView = true,
        };
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: new GroupedQuoteFetcher(),
            settingsStore: new TestSettingsStore(settings));

        await viewModel.RefreshPricesAsync();

        Assert.True(viewModel.IsStaticGroupedPriceView);
        Assert.Empty(viewModel.VisiblePriceRows);
        Assert.Equal(["Mag 7", "Precious Metals"], viewModel.StaticQuoteGroups.Select(group => group.Name));
        var row = Assert.Single(viewModel.StaticQuoteGroups[0].Rows);
        Assert.Equal("⊗ MSFT", row.Symbol);
        Assert.Equal("100.00", row.LastText);
        Assert.Equal("+9.09", row.ChangeText);
        Assert.Equal("+10.00%", row.ChangePercentText);
    }

    [Fact]
    public void SetTickerView_SwitchesImmediatelyAndPersistsBothModes()
    {
        var subscription = Subscription("MSFT", "Example", "https://example.com/MSFT") with
        {
            GroupName = "Mag 7",
            CollectNews = false,
        };
        var store = new TestSettingsStore(SmartTickerSettings.Default with
        {
            Subscriptions = [subscription],
            AcknowledgedSources = ["example.com"],
        });
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: store);

        viewModel.SetTickerViewCommand.Execute("static-prices-news");

        Assert.True(viewModel.IsStaticTableTickerView);
        Assert.Single(viewModel.StaticQuoteGroups);
        Assert.Empty(viewModel.VisiblePriceRows);
        Assert.True(store.Saved!.UseStaticGroupedView);

        viewModel.SetTickerViewCommand.Execute("scrolling-prices");

        Assert.True(viewModel.IsScrollingTickerView);
        Assert.False(viewModel.ShowNewsLine);
        Assert.Empty(viewModel.StaticQuoteGroups);
        Assert.Single(viewModel.VisiblePriceRows);
        Assert.False(store.Saved!.UseStaticGroupedView);
    }

    [Theory]
    [InlineData("scrolling-prices", false, false)]
    [InlineData("scrolling-prices-news", false, true)]
    [InlineData("static-prices", true, false)]
    [InlineData("static-prices-news", true, true)]
    public void SetTickerView_MapsEveryMenuChoiceToOnePersistedMode(
        string mode,
        bool expectedStatic,
        bool expectedNews)
    {
        var store = new TestSettingsStore(SmartTickerSettings.Default with
        {
            UseStaticGroupedView = !expectedStatic,
            ShowNewsLine = !expectedNews,
        });
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: store);

        viewModel.SetTickerViewCommand.Execute(mode);

        Assert.True(viewModel.ShowPriceLine);
        Assert.Equal(expectedStatic, viewModel.UseStaticGroupedView);
        Assert.Equal(expectedNews, viewModel.ShowNewsLine);
        Assert.Equal(expectedStatic, store.Saved!.UseStaticGroupedView);
        Assert.Equal(expectedNews, store.Saved.ShowNewsLine);
        Assert.Single(
            new[]
            {
                viewModel.IsScrollingPricesOnlyView,
                viewModel.IsScrollingPricesWithNewsView,
                viewModel.IsStaticPricesOnlyView,
                viewModel.IsStaticPricesWithNewsView,
            },
            selected => selected);
    }

    [Fact]
    public async Task StaticView_GroupsNewsWithoutCreatingAMarquee()
    {
        var subscription = Subscription("MSFT", "Example", "https://example.com/MSFT") with
        {
            GroupName = "Mag 7",
        };
        var settings = SmartTickerSettings.Default with
        {
            Subscriptions = [subscription],
            AcknowledgedSources = ["example.com"],
            UseStaticGroupedView = true,
            ShowNewsLine = true,
        };
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: new TestSettingsStore(settings),
            newsFetcher: new SuccessfulNewsFetcher());

        await viewModel.RefreshNewsAsync();

        Assert.True(viewModel.IsStaticGroupedNewsView);
        Assert.Empty(viewModel.VisibleNewsRows);
        var group = Assert.Single(viewModel.StaticNewsGroups);
        Assert.Equal("Mag 7", group.Name);
        var row = Assert.Single(group.Rows);
        Assert.Equal("MSFT", row.Symbol);
        Assert.Equal("Headline", row.Headline);
    }

    [Fact]
    public async Task StaticNews_InterleavesQuotesAndRetainsPerGroupFilterAcrossRefreshes()
    {
        var microsoft = Subscription("MSFT", "Example", "https://example.com/MSFT") with { GroupName = "Tech" };
        var apple = Subscription("AAPL", "Example", "https://example.com/AAPL") with { GroupName = "Tech" };
        var settings = SmartTickerSettings.Default with
        {
            Subscriptions = [microsoft, apple],
            AcknowledgedSources = ["example.com"],
            UseStaticGroupedView = true,
            ShowNewsLine = true,
        };
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: new TestSettingsStore(settings),
            newsFetcher: new InterleavedNewsFetcher());

        await viewModel.RefreshNewsAsync();

        var group = Assert.Single(viewModel.StaticNewsGroups);
        Assert.Equal(["All quotes", "MSFT", "AAPL"], group.FilterOptions);
        Assert.Equal(
            ["MSFT", "AAPL", "MSFT", "AAPL", "MSFT", "AAPL"],
            group.Rows.Select(row => row.Symbol));

        group.SelectedQuote = "AAPL";

        Assert.All(group.Rows, row => Assert.Equal("AAPL", row.Symbol));
        Assert.Equal("3 of 6 headlines", group.CountText);

        await viewModel.RefreshNewsAsync();

        var refreshed = Assert.Single(viewModel.StaticNewsGroups);
        Assert.Equal("AAPL", refreshed.SelectedQuote);
        Assert.All(refreshed.Rows, row => Assert.Equal("AAPL", row.Symbol));
    }

    [Fact]
    public void MoveQuoteGroup_ReordersAllEntriesAsAStableBlockAndPersists()
    {
        var firstTech = Subscription("MSFT", "Example", "https://example.com/MSFT") with { GroupName = "Tech" };
        var metals = Subscription("GOLD", "Example", "https://example.com/GOLD") with { GroupName = "Metals" };
        var secondTech = Subscription("AAPL", "Example", "https://example.com/AAPL") with { GroupName = "Tech" };
        var rates = Subscription("US10Y", "Example", "https://example.com/US10Y") with { GroupName = "Rates" };
        var store = new TestSettingsStore(SmartTickerSettings.Default with
        {
            Subscriptions = [firstTech, metals, secondTech, rates],
        });
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: store);

        viewModel.MoveQuoteGroup("Tech", "Rates", placeAfter: true);

        Assert.Equal(["GOLD", "US10Y", "MSFT", "AAPL"], viewModel.Subscriptions.Select(item => item.Symbol));
        Assert.Equal(["MSFT", "AAPL"], viewModel.Subscriptions.Where(item => item.GroupName == "Tech").Select(item => item.Symbol));
        Assert.Equal(viewModel.Subscriptions, store.Saved!.Subscriptions);
    }

    [Fact]
    public void QuoteGroupManager_RenamesMergesAndUngroupsWithoutRemovingQuotes()
    {
        var first = Subscription("MSFT", "Example", "https://example.com/MSFT") with { GroupName = "Tech" };
        var second = Subscription("AAPL", "Example", "https://example.com/AAPL") with { GroupName = "Leaders" };
        var store = new TestSettingsStore(SmartTickerSettings.Default with { Subscriptions = [first, second] });
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: store);
        viewModel.EditSubscriptionCommand.Execute(first);
        viewModel.PrepareQuoteGroupManager();
        Assert.Equal(["Tech", "Leaders"], viewModel.GroupNameOptions);
        viewModel.SelectedQuoteGroup = viewModel.QuoteGroups.Single(group => group.Name == "Tech");
        viewModel.ManagedGroupName = "leaders";

        viewModel.RenameQuoteGroupCommand.Execute(null);

        Assert.Single(viewModel.QuoteGroups);
        Assert.All(viewModel.Subscriptions, item => Assert.Equal("Leaders", item.GroupName));
        Assert.Equal("Leaders", viewModel.EditingSubscription!.GroupName);
        Assert.Equal("Leaders", viewModel.NewGroupName);
        Assert.Contains("Merged", viewModel.GroupManagerMessage);

        viewModel.ClearQuoteGroupCommand.Execute(null);

        Assert.Empty(viewModel.QuoteGroups);
        Assert.Equal(2, viewModel.Subscriptions.Count);
        Assert.All(viewModel.Subscriptions, item => Assert.Null(item.GroupName));
        Assert.Null(viewModel.EditingSubscription!.GroupName);
        Assert.Empty(viewModel.NewGroupName);
        Assert.NotNull(store.Saved);
    }

    [Fact]
    public void SettingsExportImport_PreservesQuoteGroupsAndLookupOptions()
    {
        var first = Subscription("MSFT", "Example", "https://example.com/MSFT") with { GroupName = "Tech" };
        var second = Subscription("GOLD", "Example", "https://example.com/GOLD") with { GroupName = "Metals" };
        using var exporter = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: new TestSettingsStore(SmartTickerSettings.Default with
            {
                Subscriptions = [first, second],
            }));
        var importedStore = new TestSettingsStore(SmartTickerSettings.Default);
        using var importer = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: importedStore);

        var result = importer.ImportSettingsJson(exporter.ExportSettingsJson());

        Assert.True(result.Success);
        Assert.Equal(["Tech", "Metals"], importer.Subscriptions.Select(item => item.GroupName));
        Assert.Equal(["Tech", "Metals"], importer.GroupNameOptions);
        Assert.Equal(["Tech", "Metals"], importedStore.Saved!.Subscriptions.Select(item => item.GroupName));
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

    private sealed class SessionQuoteFetcher : IQuoteFetcher
    {
        public Task<QuoteSnapshot> FetchAsync(
            TickerSubscription subscription,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new QuoteSnapshot(
                subscription.Id,
                subscription.Symbol,
                subscription.SourceName,
                100m,
                "USD",
                DateTimeOffset.UtcNow,
                true,
                "ok",
                ChangePercent: 1m,
                ExtendedPrice: 99m,
                ExtendedChangePercent: -0.25m,
                PreMarketPrice: 101m,
                PreMarketChangePercent: 0.5m));
    }

    private sealed class GroupedQuoteFetcher : IQuoteFetcher
    {
        public Task<QuoteSnapshot> FetchAsync(
            TickerSubscription subscription,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new QuoteSnapshot(
                subscription.Id,
                subscription.Symbol,
                subscription.SourceName,
                subscription.Symbol == "MSFT" ? 100m : 200m,
                "USD",
                DateTimeOffset.UtcNow,
                true,
                "ok",
                ChangePercent: subscription.Symbol == "MSFT" ? 10m : -5m));
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

    private sealed class InterleavedNewsFetcher : INewsFetcher
    {
        public Task<NewsSnapshot> FetchAsync(
            TickerSubscription subscription,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new NewsSnapshot(
                subscription.Id,
                subscription.Symbol,
                subscription.SourceName,
                Enumerable.Range(1, 3)
                    .Select(index => new NewsHeadline(
                        $"{subscription.Symbol} headline {index}",
                        new Uri($"https://example.com/{subscription.Symbol}/{index}")))
                    .ToArray(),
                DateTimeOffset.UtcNow,
                true,
                "ok"));
    }
}