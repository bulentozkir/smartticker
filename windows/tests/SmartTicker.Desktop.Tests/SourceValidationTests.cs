using Avalonia.Media;
using SmartTicker.Core.Models;
using SmartTicker.Core.Services;
using SmartTicker.Desktop.ViewModels;
using SmartTicker.Desktop.Views;
using SmartTicker.Infrastructure.Persistence;

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
        var loadedBrush = Assert.IsType<SolidColorBrush>(viewModel.AlertBlinkBrush);
        Assert.Equal(Color.Parse("#12AB34"), loadedBrush.Color);
        Assert.Same(loadedBrush, viewModel.AlertBlinkBrush);

        viewModel.AlertBlinkColorHex = "#3456CD";

        Assert.NotSame(loadedBrush, viewModel.AlertBlinkBrush);
        Assert.Same(viewModel.AlertBlinkBrush, viewModel.AlertBlinkBrush);
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
        var store = new TestSettingsStore(settings);
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: store,
            newsFetcher: new InterleavedNewsFetcher());

        await viewModel.RefreshNewsAsync();

        var group = Assert.Single(viewModel.StaticNewsGroups);
        Assert.Equal(["MSFT · Example", "AAPL · Example"], group.QuoteFilters.Select(filter => filter.Label));
        Assert.All(group.QuoteFilters, filter => Assert.True(filter.IsShown));
        Assert.Equal("All quotes", group.FilterSummary);
        Assert.Equal(
            ["MSFT", "AAPL", "MSFT", "AAPL", "MSFT", "AAPL"],
            group.Rows.Select(row => row.Symbol));

        group.QuoteFilters.Single(filter => filter.SubscriptionId == microsoft.Id).IsShown = false;

        Assert.All(group.Rows, row => Assert.Equal("AAPL", row.Symbol));
        Assert.Equal("3 of 6 headlines", group.CountText);
        Assert.Equal("AAPL", group.FilterSummary);
        Assert.Equal([microsoft.Id], store.Saved!.HiddenNewsQuotes);

        group.QuoteFilters.Single(filter => filter.SubscriptionId == apple.Id).IsShown = false;
        Assert.Empty(group.Rows);
        Assert.Equal("No quotes", group.FilterSummary);
        Assert.Equal([microsoft.Id, apple.Id], store.Saved.HiddenNewsQuotes);

        group.QuoteFilters.Single(filter => filter.SubscriptionId == apple.Id).IsShown = true;
        Assert.All(group.Rows, row => Assert.Equal("AAPL", row.Symbol));
        Assert.Equal([microsoft.Id], store.Saved.HiddenNewsQuotes);

        await viewModel.RefreshNewsAsync();

        var refreshed = Assert.Single(viewModel.StaticNewsGroups);
        Assert.False(refreshed.QuoteFilters.Single(filter => filter.SubscriptionId == microsoft.Id).IsShown);
        Assert.True(refreshed.QuoteFilters.Single(filter => filter.SubscriptionId == apple.Id).IsShown);
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
        Assert.Equal(["Metals", "Rates", "Tech"], store.Saved.QuoteGroupNames);
    }

    [Fact]
    public void QuoteGroupManager_CreatesUpdatesAndDeletesEmptyGroups()
    {
        var quote = Subscription("MSFT", "Example", "https://example.com/MSFT");
        var store = new TestSettingsStore(SmartTickerSettings.Default with { Subscriptions = [quote] });
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: store);
        viewModel.PrepareQuoteGroupManager();
        viewModel.ManagedGroupName = "Tech";

        viewModel.CreateQuoteGroupCommand.Execute(null);

        var created = Assert.Single(viewModel.QuoteGroups);
        Assert.Equal("Tech", created.Name);
        Assert.Equal(0, created.QuoteCount);
        Assert.Equal(["Tech"], store.Saved!.QuoteGroupNames);
        viewModel.SelectedGroupQuote = quote;
        viewModel.AssociateSelectedQuoteCommand.Execute(null);
        Assert.Equal("Tech", Assert.Single(viewModel.Subscriptions).GroupName);

        viewModel.ManagedGroupName = "Leaders";

        viewModel.UpdateQuoteGroupCommand.Execute(null);

        var updated = Assert.Single(viewModel.QuoteGroups);
        Assert.Equal("Leaders", updated.Name);
        Assert.Equal(1, updated.QuoteCount);
        Assert.Equal("Leaders", Assert.Single(viewModel.Subscriptions).GroupName);
        Assert.Equal(["Leaders"], store.Saved.QuoteGroupNames);

        viewModel.DeleteQuoteGroupCommand.Execute(null);

        Assert.Empty(viewModel.QuoteGroups);
        Assert.Single(viewModel.Subscriptions);
        Assert.Null(viewModel.Subscriptions[0].GroupName);
        Assert.Empty(store.Saved.QuoteGroupNames);
    }

    [Fact]
    public void QuoteGroupManager_AssociationMovesAQuoteBetweenGroups()
    {
        var quote = Subscription("MSFT", "Example", "https://example.com/MSFT");
        var store = new TestSettingsStore(SmartTickerSettings.Default with
        {
            Subscriptions = [quote],
            QuoteGroupNames = ["Tech", "Leaders"],
        });
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: store);
        viewModel.PrepareQuoteGroupManager();
        viewModel.SelectedGroupQuote = quote;
        viewModel.SelectedQuoteGroup = viewModel.QuoteGroups.Single(group => group.Name == "Tech");

        viewModel.AssociateSelectedQuoteCommand.Execute(null);

        Assert.Equal("Tech", Assert.Single(viewModel.Subscriptions).GroupName);
        Assert.Equal(1, viewModel.QuoteGroups.Single(group => group.Name == "Tech").QuoteCount);
        Assert.Equal(0, viewModel.QuoteGroups.Single(group => group.Name == "Leaders").QuoteCount);

        viewModel.SelectedQuoteGroup = viewModel.QuoteGroups.Single(group => group.Name == "Leaders");

        viewModel.AssociateSelectedQuoteCommand.Execute(null);

        Assert.Equal("Leaders", Assert.Single(viewModel.Subscriptions).GroupName);
        Assert.Equal(0, viewModel.QuoteGroups.Single(group => group.Name == "Tech").QuoteCount);
        Assert.Equal(1, viewModel.QuoteGroups.Single(group => group.Name == "Leaders").QuoteCount);
        Assert.Equal("Leaders", Assert.Single(store.Saved!.Subscriptions).GroupName);

        viewModel.UngroupSelectedQuoteCommand.Execute(null);

        Assert.Null(Assert.Single(viewModel.Subscriptions).GroupName);
        Assert.Equal(0, viewModel.QuoteGroups.Single(group => group.Name == "Leaders").QuoteCount);
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
                QuoteGroupNames = ["Tech", "Metals", "Empty"],
            }));
        var importedStore = new TestSettingsStore(SmartTickerSettings.Default);
        using var importer = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: importedStore);

        var result = importer.ImportSettingsJson(exporter.ExportSettingsJson());

        Assert.True(result.Success);
        Assert.Equal(["Tech", "Metals"], importer.Subscriptions.Select(item => item.GroupName));
        Assert.Equal(["Tech", "Metals", "Empty"], importer.GroupNameOptions);
        Assert.Equal(["Tech", "Metals", "Empty"], importedStore.Saved!.QuoteGroupNames);
        Assert.Equal(["Tech", "Metals"], importedStore.Saved!.Subscriptions.Select(item => item.GroupName));
    }

    [Fact]
    public async Task PriceRefresh_BlinksOnlyAQuoteWhosePriceChangedSinceTheLastSync()
    {
        var subscription = Subscription("MSFT", "Example", "https://example.com/MSFT");
        var settings = SmartTickerSettings.Default with
        {
            Subscriptions = [subscription],
            AcknowledgedSources = ["example.com"],
            UseStaticGroupedView = true,
        };
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: new ChangingQuoteFetcher(100m, 101m),
            settingsStore: new TestSettingsStore(settings));

        await viewModel.RefreshPricesAsync();

        Assert.Equal(Brushes.Transparent, Assert.Single(viewModel.StaticQuoteGroups).Rows.Single().Background);

        await viewModel.RefreshPricesAsync();

        var changed = Assert.Single(viewModel.StaticQuoteGroups).Rows.Single();
        Assert.Equal(Color.Parse("#8B4513"), Assert.IsAssignableFrom<ISolidColorBrush>(changed.Background).Color);

        await viewModel.RefreshPricesAsync();

        var unchanged = Assert.Single(viewModel.StaticQuoteGroups).Rows.Single();
        Assert.Equal(Color.Parse("#8B4513"), Assert.IsAssignableFrom<ISolidColorBrush>(unchanged.Background).Color);
    }

    [Fact]
    public async Task NewsRefresh_BlinksOnlyHeadlinesThatArrivedSinceTheLastSync()
    {
        var subscription = Subscription("MSFT", "Example", "https://example.com/MSFT");
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
            newsFetcher: new GrowingNewsFetcher());

        await viewModel.RefreshNewsAsync();

        Assert.All(
            Assert.Single(viewModel.StaticNewsGroups).Rows,
            row => Assert.Equal(Brushes.Transparent, row.Background));

        await viewModel.RefreshNewsAsync();

        var rows = Assert.Single(viewModel.StaticNewsGroups).Rows;
        Assert.Equal(["First", "Second"], rows.Select(row => row.Headline));
        Assert.Equal(Brushes.Transparent, rows[0].Background);
        Assert.Equal(Color.Parse("#8B4513"), Assert.IsAssignableFrom<ISolidColorBrush>(rows[1].Background).Color);
    }

    [Fact]
    public void ApplyEditedSettingsJson_AppliesAValidInPlaceEditWithoutWritingItBack()
    {
        var store = new TestSettingsStore(SmartTickerSettings.Default with
        {
            Subscriptions = [Subscription("MSFT", "Example", "https://example.com/MSFT")],
        });
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: store);

        var result = viewModel.ApplyEditedSettingsJson(
            """{"version":1,"subscriptions":[],"quoteGroups":["Edited"],"priceRowCount":3}""");

        Assert.True(result.Success);
        Assert.Empty(viewModel.Subscriptions);
        Assert.Equal(["Edited"], viewModel.GroupNameOptions);
        Assert.Equal(3, viewModel.PriceRowCount);
        // The file on disk is already the source of truth, so nothing is written back over the edit.
        Assert.Null(store.Saved);
    }

    [Fact]
    public void ApplyEditedSettingsJson_RejectsAMalformedEditAndKeepsTheCurrentConfiguration()
    {
        var store = new TestSettingsStore(SmartTickerSettings.Default with
        {
            Subscriptions = [Subscription("MSFT", "Example", "https://example.com/MSFT")],
        });
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: store);

        var result = viewModel.ApplyEditedSettingsJson("{ not json ");

        Assert.False(result.Success);
        Assert.Equal(["MSFT"], viewModel.Subscriptions.Select(item => item.Symbol));
        Assert.True(viewModel.HasImportProblems);
        Assert.Contains("settings.json", viewModel.ImportStatusMessage);
        Assert.Contains(viewModel.ImportProblems, problem => problem.Contains("restore a valid export"));
        Assert.Null(store.Saved);
    }

    [Fact]
    public async Task PriceRefresh_ContainsProviderExceptions()
    {
        var subscription = Subscription("MSFT", "Example", "https://example.com/MSFT");
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: new ThrowingQuoteFetcher(),
            settingsStore: new TestSettingsStore(SmartTickerSettings.Default with
            {
                Subscriptions = [subscription],
                AcknowledgedSources = ["example.com"],
            }));

        await viewModel.RefreshPricesAsync();

        Assert.Contains("Price refresh failed", viewModel.EntryMessage);
        Assert.Contains("price provider failed", viewModel.EntryMessage);
    }

    [Fact]
    public async Task ScheduledPriceRefresh_RequestsOnlyItsBatchAndUpdatesTheLaneOnce()
    {
        var first = Subscription("MSFT", "Example", "https://example.com/MSFT");
        var second = Subscription("AAPL", "Example", "https://example.com/AAPL");
        var third = Subscription("NVDA", "Example", "https://example.com/NVDA");
        var fetcher = new SuccessfulQuoteFetcher();
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: fetcher,
            settingsStore: new TestSettingsStore(SmartTickerSettings.Default with
            {
                Subscriptions = [first, second, third],
                AcknowledgedSources = ["example.com"],
            }));
        var lane = Assert.Single(viewModel.VisiblePriceRows);
        var segmentNotifications = 0;
        lane.PropertyChanged += (_, change) =>
        {
            if (change.PropertyName == nameof(TickerLane.Segments))
            {
                segmentNotifications++;
            }
        };

        await viewModel.RefreshPriceSubscriptionsSafelyAsync(
            "Scheduled price refresh",
            [first.Id, third.Id]);

        Assert.Equal([first.Id, third.Id], fetcher.Requests);
        Assert.Equal([first.Id, third.Id], viewModel.LatestQuotes.Select(snapshot => snapshot.SubscriptionId));
        Assert.Equal(1, segmentNotifications);
    }

    [Fact]
    public async Task ScheduledPriceRefresh_KeepsRenderedSnapshotUntilReplacementIsReady()
    {
        var subscription = Subscription("MSFT", "Example", "https://example.com/MSFT");
        var fetcher = new HoldingSecondQuoteFetcher();
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: fetcher,
            settingsStore: new TestSettingsStore(SmartTickerSettings.Default with
            {
                Subscriptions = [subscription],
                AcknowledgedSources = ["example.com"],
            }));
        await viewModel.RefreshPricesAsync();
        var originalSnapshot = Assert.Single(viewModel.LatestQuotes);
        var originalSegments = Assert.Single(viewModel.VisiblePriceRows).Segments;

        var refresh = viewModel.RefreshPriceSubscriptionsSafelyAsync(
            "Scheduled price refresh",
            [subscription.Id]);
        await fetcher.SecondRequestStarted.Task.WaitAsync(TimeSpan.FromSeconds(2));

        Assert.Same(originalSnapshot, Assert.Single(viewModel.LatestQuotes));
        Assert.Same(originalSegments, Assert.Single(viewModel.VisiblePriceRows).Segments);

        fetcher.ReleaseSecondRequest.TrySetResult();
        await refresh;

        Assert.Equal(101m, Assert.Single(viewModel.LatestQuotes).Price);
        Assert.NotSame(originalSegments, Assert.Single(viewModel.VisiblePriceRows).Segments);
    }

    [Fact]
    public async Task ScheduledPriceRefresh_SpreadsSixtyQuotesAcrossThirtyConsolidatedBatches()
    {
        var subscriptions = Enumerable.Range(0, 60)
            .Select(index => Subscription(
                $"Q{index:00}",
                "Example",
                $"https://example.com/Q{index:00}"))
            .ToArray();
        var fetcher = new SuccessfulQuoteFetcher();
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: fetcher,
            settingsStore: new TestSettingsStore(SmartTickerSettings.Default with
            {
                Subscriptions = subscriptions,
                AcknowledgedSources = ["example.com"],
                PriceRefreshSeconds = 30,
            }));
        var lane = Assert.Single(viewModel.VisiblePriceRows);
        var segmentNotifications = 0;
        lane.PropertyChanged += (_, change) =>
        {
            if (change.PropertyName == nameof(TickerLane.Segments))
            {
                segmentNotifications++;
            }
        };
        var schedule = new PriceRefreshSchedule();
        var subscriptionIds = subscriptions.Select(subscription => subscription.Id).ToArray();

        for (var slot = 0; slot < 30; slot++)
        {
            var batch = schedule.NextBatch(subscriptionIds, 30);
            Assert.Equal(2, batch.Count);
            await viewModel.RefreshPriceSubscriptionsSafelyAsync("Scheduled price refresh", batch);
        }

        Assert.Equal(subscriptionIds, fetcher.Requests);
        Assert.Equal(subscriptionIds, viewModel.LatestQuotes.Select(snapshot => snapshot.SubscriptionId));
        Assert.Equal(30, segmentNotifications);
    }

    [Fact]
    public async Task NewsRefresh_ContainsProviderExceptions()
    {
        var subscription = Subscription("MSFT", "Example", "https://example.com/MSFT");
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: new TestSettingsStore(SmartTickerSettings.Default with
            {
                Subscriptions = [subscription],
                AcknowledgedSources = ["example.com"],
            }),
            newsFetcher: new ThrowingNewsFetcher());

        await viewModel.RefreshNewsAsync();

        Assert.Contains("News refresh failed", viewModel.EntryMessage);
        Assert.Contains("news provider failed", viewModel.EntryMessage);
    }

    [Fact]
    public async Task DisposeDuringPriceRefresh_DoesNotEscapeAnObjectDisposedException()
    {
        var subscription = Subscription("MSFT", "Example", "https://example.com/MSFT");
        var fetcher = new CancellableQuoteFetcher();
        var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: fetcher,
            settingsStore: new TestSettingsStore(SmartTickerSettings.Default with
            {
                Subscriptions = [subscription],
                AcknowledgedSources = ["example.com"],
            }));
        var refresh = viewModel.RefreshPricesAsync();
        await fetcher.Started.Task.WaitAsync(TimeSpan.FromSeconds(2));

        viewModel.Dispose();

        await refresh;
    }

    [Fact]
    public async Task ProviderResultAfterDispose_IsIgnoredEvenWhenProviderIgnoresCancellation()
    {
        var subscription = Subscription("MSFT", "Example", "https://example.com/MSFT");
        var fetcher = new DelayedQuoteFetcher();
        var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: fetcher,
            settingsStore: new TestSettingsStore(SmartTickerSettings.Default with
            {
                Subscriptions = [subscription],
                AcknowledgedSources = ["example.com"],
            }));
        var refresh = viewModel.RefreshPricesAsync();
        await fetcher.Started.Task.WaitAsync(TimeSpan.FromSeconds(2));

        viewModel.Dispose();
        fetcher.Release.TrySetResult();
        await refresh;

        Assert.Empty(viewModel.LatestQuotes);
    }

    [Fact]
    public async Task ProviderResultForRemovedQuote_IsIgnored()
    {
        var subscription = Subscription("MSFT", "Example", "https://example.com/MSFT");
        var fetcher = new DelayedQuoteFetcher();
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: fetcher,
            settingsStore: new TestSettingsStore(SmartTickerSettings.Default with
            {
                Subscriptions = [subscription],
                AcknowledgedSources = ["example.com"],
            }));
        var refresh = viewModel.RefreshPricesAsync();
        await fetcher.Started.Task.WaitAsync(TimeSpan.FromSeconds(2));

        await viewModel.RemoveSubscriptionCommand.ExecuteAsync(subscription);
        fetcher.Release.TrySetResult();
        await refresh;

        Assert.Empty(viewModel.Subscriptions);
        Assert.Empty(viewModel.LatestQuotes);
    }

    [Fact]
    public async Task UpdateAfterExternalRemoval_ReportsStaleEditorInsteadOfThrowing()
    {
        var subscription = Subscription("MSFT", "Example", "https://example.com/MSFT");
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: new TestSettingsStore(SmartTickerSettings.Default with
            {
                Subscriptions = [subscription],
            }));
        viewModel.EditSubscriptionCommand.Execute(subscription);
        var imported = viewModel.ApplyEditedSettingsJson("""{"version":1,"subscriptions":[]}""");

        await viewModel.AddSubscriptionCommand.ExecuteAsync(null);

        Assert.True(imported.Success);
        Assert.Empty(viewModel.Subscriptions);
        Assert.Contains("changed outside this form", viewModel.EntryMessage);
    }

    [Fact]
    public void ViewSwitch_ContainsSettingsStoreFailures()
    {
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: new ThrowingSaveSettingsStore(SmartTickerSettings.Default));

        viewModel.SetTickerViewCommand.Execute("static-prices-news");

        Assert.True(viewModel.IsStaticPricesWithNewsView);
        Assert.Contains("Settings could not be saved", viewModel.EntryMessage);
    }

    [Fact]
    public void AlertStoreLoadFailure_IsReportedWithoutEscapingStartup()
    {
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            alertStore: new ThrowingAlertStore(throwOnLoad: true));

        Assert.Contains("Alert rules could not be loaded", viewModel.EntryMessage);
    }

    [Fact]
    public void AlertStoreSaveFailure_IsReportedWithoutEscapingPropertyChange()
    {
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            alertStore: new ThrowingAlertStore(throwOnLoad: false));

        viewModel.AlertBuzzCount = 9;

        Assert.Contains("Alert rules could not be saved", viewModel.EntryMessage);
    }

    [Fact]
    public void MalformedStartupSettings_AreNotOverwrittenByViewChangesOrShutdown()
    {
        var directory = Path.Combine(Path.GetTempPath(), "SmartTicker.Tests", Guid.NewGuid().ToString("N"));
        var path = Path.Combine(directory, "settings.json");
        const string malformed = "{ not valid json";
        Directory.CreateDirectory(directory);
        File.WriteAllText(path, malformed);

        try
        {
            var viewModel = new MainViewModel(
                selectorDiscovery: null,
                quoteFetcher: null,
                settingsStore: new LocalJsonSettingsStore(path));

            Assert.Contains("Settings could not be loaded", viewModel.EntryMessage);
            viewModel.SetTickerViewCommand.Execute("static-prices-news");
            viewModel.Dispose();

            Assert.Equal(malformed, File.ReadAllText(path));
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public async Task QuoteMutationsAndViewSwitches_RemainStableAcrossRepeatedTransitions()
    {
        var store = new TestSettingsStore(SmartTickerSettings.Default with
        {
            AllowWebsiteCookiesAndCrossHostRedirects = true,
        });
        using var viewModel = new MainViewModel(
            selectorDiscovery: null,
            quoteFetcher: null,
            settingsStore: store);
        var modes = new[]
        {
            "scrolling-prices",
            "scrolling-prices-news",
            "static-prices",
            "static-prices-news",
        };

        for (var iteration = 0; iteration < 20; iteration++)
        {
            viewModel.SelectedSource = viewModel.SourceAlternatives.Single(source => source.HomePage is null);
            viewModel.NewSymbol = $"Q{iteration}";
            viewModel.NewSourceUrlSuffix = $"https://example.com/Q{iteration}";
            viewModel.NewCollectPrice = true;
            viewModel.NewCollectNews = true;
            await viewModel.AddSubscriptionCommand.ExecuteAsync(null);
            var added = Assert.Single(viewModel.Subscriptions);

            viewModel.SetTickerViewCommand.Execute(modes[iteration % modes.Length]);
            viewModel.EditSubscriptionCommand.Execute(added);
            viewModel.NewSymbol = $"Q{iteration}U";
            await viewModel.AddSubscriptionCommand.ExecuteAsync(null);
            var updated = Assert.Single(viewModel.Subscriptions);
            Assert.Equal($"Q{iteration}U", updated.Symbol);

            viewModel.SetTickerViewCommand.Execute(modes[(iteration + 1) % modes.Length]);
            await viewModel.RemoveSubscriptionCommand.ExecuteAsync(updated);
            Assert.Empty(viewModel.Subscriptions);
        }

        Assert.NotNull(store.Saved);
        Assert.DoesNotContain("failed", viewModel.EntryMessage, StringComparison.OrdinalIgnoreCase);
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

    private sealed class ThrowingSaveSettingsStore(SmartTickerSettings settings) : ISettingsStore
    {
        public string FilePath => string.Empty;

        public SmartTickerSettings Load() => settings;

        public void Save(SmartTickerSettings saved) => throw new InvalidOperationException("disk unavailable");
    }

    private sealed class ThrowingAlertStore(bool throwOnLoad) : IAlertStore
    {
        public string FilePath => string.Empty;

        public AlertSettings Load() => throwOnLoad
            ? throw new InvalidOperationException("alert store unavailable")
            : AlertSettings.Default;

        public void Save(AlertSettings settings) => throw new InvalidOperationException("alert store unavailable");
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

    private sealed class ThrowingQuoteFetcher : IQuoteFetcher
    {
        public Task<QuoteSnapshot> FetchAsync(
            TickerSubscription subscription,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("price provider failed");
    }

    private sealed class ThrowingNewsFetcher : INewsFetcher
    {
        public Task<NewsSnapshot> FetchAsync(
            TickerSubscription subscription,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("news provider failed");
    }

    private sealed class CancellableQuoteFetcher : IQuoteFetcher
    {
        public TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task<QuoteSnapshot> FetchAsync(
            TickerSubscription subscription,
            CancellationToken cancellationToken = default)
        {
            Started.TrySetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            throw new InvalidOperationException("The cancellation wait unexpectedly completed.");
        }
    }

    private sealed class DelayedQuoteFetcher : IQuoteFetcher
    {
        public TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task<QuoteSnapshot> FetchAsync(
            TickerSubscription subscription,
            CancellationToken cancellationToken = default)
        {
            Started.TrySetResult();
            await Release.Task;
            return new QuoteSnapshot(
                subscription.Id,
                subscription.Symbol,
                subscription.SourceName,
                100m,
                "USD",
                DateTimeOffset.UtcNow,
                true,
                "ok");
        }
    }

    private sealed class ChangingQuoteFetcher(params decimal[] prices) : IQuoteFetcher
    {
        private readonly Queue<decimal> _prices = new(prices);

        public Task<QuoteSnapshot> FetchAsync(
            TickerSubscription subscription,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new QuoteSnapshot(
                subscription.Id,
                subscription.Symbol,
                subscription.SourceName,
                _prices.Count > 1 ? _prices.Dequeue() : _prices.Peek(),
                "USD",
                DateTimeOffset.UtcNow,
                true,
                "ok"));
    }

    private sealed class HoldingSecondQuoteFetcher : IQuoteFetcher
    {
        private int _requestCount;

        public TaskCompletionSource SecondRequestStarted { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource ReleaseSecondRequest { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task<QuoteSnapshot> FetchAsync(
            TickerSubscription subscription,
            CancellationToken cancellationToken = default)
        {
            var price = 100m;
            if (Interlocked.Increment(ref _requestCount) == 2)
            {
                SecondRequestStarted.TrySetResult();
                await ReleaseSecondRequest.Task.WaitAsync(cancellationToken);
                price = 101m;
            }

            return new QuoteSnapshot(
                subscription.Id,
                subscription.Symbol,
                subscription.SourceName,
                price,
                "USD",
                DateTimeOffset.UtcNow,
                true,
                "ok");
        }
    }

    private sealed class GrowingNewsFetcher : INewsFetcher
    {
        private int _calls;

        public Task<NewsSnapshot> FetchAsync(
            TickerSubscription subscription,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new NewsSnapshot(
                subscription.Id,
                subscription.Symbol,
                subscription.SourceName,
                (_calls++ == 0 ? new[] { "First" } : ["First", "Second"])
                    .Select(title => new NewsHeadline(title, new Uri("https://example.com/news")))
                    .ToArray(),
                DateTimeOffset.UtcNow,
                true,
                "ok"));
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