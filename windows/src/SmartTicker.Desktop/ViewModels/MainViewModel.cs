using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartTicker.Application.Sources;
using SmartTicker.Core.Models;
using SmartTicker.Core.Services;
using SmartTicker.Desktop.Localization;

namespace SmartTicker.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase, IDisposable
{
    // Flags a price entry whose subscription does not collect news.
    private const string NoNewsMarker = "⊗ ";

    // Flags a price entry with a live alert.
    private const string AlertMarker = "❗ ";

    [ObservableProperty]
    public partial string QuoteLine { get; set; } = "PRICES  •  Add an authorized webpage or feed in Settings  •  Refresh: 1 min";

    [ObservableProperty]
    public partial string NewsLine { get; set; } = "NEWS  •  Add RSS/Atom feeds  •  Refresh: 5 min  •  Delayed data — not investment advice";

    [ObservableProperty]
    public partial bool IsPaused { get; set; }

    [ObservableProperty]
    public partial string NewSymbol { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewGroupName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewSourceName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewSourceUrl { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewSourceUrlSuffix { get; set; } = string.Empty;

    [ObservableProperty]
    public partial SourcePreset? SelectedSource { get; set; }

    [ObservableProperty]
    public partial string NewCssSelector { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewNewsCssSelector { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewPreMarketCssSelector { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewPreMarketChangeCssSelector { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewExtendedCssSelector { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewExtendedChangeCssSelector { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewChangeCssSelector { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int NewNewsRepeatLimit { get; set; } = TickerSubscription.DefaultNewsRepeatLimit;

    [ObservableProperty]
    public partial bool NewCollectPrice { get; set; } = true;

    [ObservableProperty]
    public partial bool NewCollectNews { get; set; }

    [ObservableProperty]
    public partial string EntryMessage { get; set; } = "Enter every ticker and source URL manually. Duplicate symbols are allowed.";

    [ObservableProperty]
    public partial string DiscoveryMessage { get; set; } = "Selector discovery reads public static HTML only.";

    [ObservableProperty]
    public partial string NewsDiscoveryMessage { get; set; } = "News discovery reads public static HTML only.";

    [ObservableProperty]
    public partial string ValidationMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ImportStatusIcon { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ImportStatusMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ImportStatusColor { get; set; } = "#D8DEE9";

    [ObservableProperty]
    public partial bool HasImportStatus { get; set; }

    [ObservableProperty]
    public partial bool HasImportProblems { get; set; }

    public ObservableCollection<string> ImportProblems { get; } = [];

    // Shown until the first entry exists, so a fresh install is not just an empty bar.
    public bool ShowStarterPrompt => Subscriptions.Count == 0;

    public string StarterSourceUri => _starterSettings?.Location.AbsoluteUri ?? string.Empty;

    [ObservableProperty]
    public partial bool IsLoadingStarter { get; set; }

    [RelayCommand]
    private async Task LoadStarterQuotesAsync()
    {
        if (_starterSettings is null)
        {
            ReportImportFailure("the starter file", ["Downloading is unavailable in the designer."]);
            return;
        }

        try
        {
            IsLoadingStarter = true;
            var json = await _starterSettings.DownloadAsync(_lifetimeCancellation.Token);
            var result = ImportSettingsJson(json);
            if (result.Success)
            {
                ReportImportSuccess("the starter quotes from GitHub", result.Settings!.Subscriptions.Length);
                await RefreshPricesAsync();
                await RefreshNewsAsync();
            }
            else
            {
                ReportImportFailure("the starter quotes from GitHub", result.Errors);
            }
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            ReportImportFailure(
                "the starter quotes from GitHub",
            [exception is OperationCanceledException ? "The download was cancelled or timed out." : exception.Message]);
        }
        finally
        {
            IsLoadingStarter = false;
        }
    }

    public void ReportImportSuccess(string fileName, int entryCount)
    {
        ImportProblems.Clear();
        HasImportProblems = false;
        ImportStatusIcon = "✓";
        ImportStatusColor = "#3FB950";
        ImportStatusMessage =
            $"\"{fileName}\" replaced your configuration. {entryCount} entr{(entryCount == 1 ? "y is" : "ies are")} now active.";
        HasImportStatus = true;
    }

    public void ReportImportFailure(string fileName, IReadOnlyList<string> problems)
    {
        ImportProblems.Clear();
        foreach (var problem in problems)
        {
            ImportProblems.Add(problem);
        }

        HasImportProblems = ImportProblems.Count > 0;
        ImportStatusIcon = "✕";
        ImportStatusColor = "#F85149";
        ImportStatusMessage = $"\"{fileName}\" was rejected. Your current settings are unchanged.";
        HasImportStatus = true;
    }

    [ObservableProperty]
    public partial bool IsValidating { get; set; }

    [ObservableProperty]
    public partial bool IsValidatingAllSources { get; set; }

    [ObservableProperty]
    public partial string SourceValidationStatus { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool HasSourceValidationProblems { get; set; }

    public ObservableCollection<string> SourceValidationProblems { get; } = [];

    [ObservableProperty]
    public partial string Language { get; set; } = AppLanguages.Default;

    [ObservableProperty]
    public partial bool IsDiscovering { get; set; }

    [ObservableProperty]
    public partial bool IsDiscoveringNews { get; set; }

    [ObservableProperty]
    public partial TickerSubscription? EditingSubscription { get; set; }

    [ObservableProperty]
    public partial int PriceScrollSpeed { get; set; } = 50;

    [ObservableProperty]
    public partial int NewsScrollSpeed { get; set; } = 40;

    [ObservableProperty]
    public partial int PriceRowCount { get; set; } = 1;

    [ObservableProperty]
    public partial int NewsRowCount { get; set; } = 1;

    [ObservableProperty]
    public partial int ScrollingViewFontSize { get; set; } = SmartTickerSettings.DefaultScrollingViewFontSize;

    [ObservableProperty]
    public partial int StaticViewFontSize { get; set; } = SmartTickerSettings.DefaultStaticViewFontSize;

    public IReadOnlyList<SourcePreset> SourceAlternatives => KnownSourceCatalog.All;

    public IReadOnlyList<int> RowCountOptions { get; } = Enumerable.Range(1, 8).ToArray();

    public IReadOnlyList<int> ScrollSpeedOptions { get; } = [20, 30, 40, 50, 65, 80, 100, 120];

    public ObservableCollection<TickerSubscription> Subscriptions { get; } = [];

    public ObservableCollection<TickerLane> VisiblePriceRows { get; } = [];

    public ObservableCollection<StaticQuoteGroup> StaticQuoteGroups { get; } = [];

    public ObservableCollection<StaticNewsGroup> StaticNewsGroups { get; } = [];

    public ObservableCollection<QuoteGroupSummary> QuoteGroups { get; } = [];

    public ObservableCollection<TickerLane> VisibleNewsRows { get; } = [];

    public ObservableCollection<CssSelectorSuggestion> SelectorSuggestions { get; } = [];

    public ObservableCollection<CssSelectorSuggestion> NewsSelectorSuggestions { get; } = [];

    public ObservableCollection<QuoteSnapshot> LatestQuotes { get; } = [];

    public ObservableCollection<NewsSnapshot> LatestNews { get; } = [];

    [ObservableProperty]
    public partial double WindowWidth { get; set; } = SmartTickerSettings.DefaultScrollingWindowSize.Width;

    [ObservableProperty]
    public partial double WindowHeight { get; set; } = TickerLayoutCalculator.NaturalHeight(1, 1);

    [ObservableProperty]
    public partial int ScrollingWindowWidth { get; set; } = SmartTickerSettings.DefaultScrollingWindowSize.Width;

    [ObservableProperty]
    public partial int ScrollingWindowHeight { get; set; } = SmartTickerSettings.DefaultScrollingWindowSize.Height;

    [ObservableProperty]
    public partial int StaticPricesWindowWidth { get; set; } = SmartTickerSettings.DefaultStaticPricesWindowSize.Width;

    [ObservableProperty]
    public partial int StaticPricesWindowHeight { get; set; } = SmartTickerSettings.DefaultStaticPricesWindowSize.Height;

    [ObservableProperty]
    public partial int StaticNewsWindowWidth { get; set; } = SmartTickerSettings.DefaultStaticNewsWindowSize.Width;

    [ObservableProperty]
    public partial int StaticNewsWindowHeight { get; set; } = SmartTickerSettings.DefaultStaticNewsWindowSize.Height;

    [ObservableProperty]
    public partial bool ShowPriceLine { get; set; } = true;

    [ObservableProperty]
    public partial bool ShowNewsLine { get; set; }

    [ObservableProperty]
    public partial bool UseStaticGroupedView { get; set; }

    [ObservableProperty]
    public partial QuoteGroupSummary? SelectedQuoteGroup { get; set; }

    [ObservableProperty]
    public partial string ManagedGroupName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial TickerSubscription? SelectedGroupQuote { get; set; }

    [ObservableProperty]
    public partial string GroupManagerMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool LaunchAtLogin { get; set; }

    public bool IsLaunchAtLoginSupported => _startupRegistration?.IsSupported ?? false;

    [ObservableProperty]
    public partial bool AllowWebsiteCookiesAndCrossHostRedirects { get; set; }

    [ObservableProperty]
    public partial string BackgroundColorHex { get; set; } = SmartTickerSettings.DefaultBackgroundColor;

    [ObservableProperty]
    public partial double BackgroundOpacity { get; set; } = SmartTickerSettings.DefaultOpacity;

    // The suggestion list is shared, so only the row that started the discovery renders it.
    [ObservableProperty]
    public partial SelectorKind DiscoveryTarget { get; set; } = SelectorKind.Price;

    public bool ShowPriceMatches => DiscoveryTarget == SelectorKind.Price;

    public bool ShowChangeMatches => DiscoveryTarget == SelectorKind.Change;

    public bool ShowPreMarketMatches => DiscoveryTarget == SelectorKind.PreMarketPrice;

    public bool ShowPreMarketChangeMatches => DiscoveryTarget == SelectorKind.PreMarketChange;

    public bool ShowExtendedMatches => DiscoveryTarget == SelectorKind.ExtendedPrice;

    public bool ShowExtendedChangeMatches => DiscoveryTarget == SelectorKind.ExtendedChange;

    partial void OnDiscoveryTargetChanged(SelectorKind value)
    {
        OnPropertyChanged(nameof(ShowPriceMatches));
        OnPropertyChanged(nameof(ShowChangeMatches));
        OnPropertyChanged(nameof(ShowPreMarketMatches));
        OnPropertyChanged(nameof(ShowPreMarketChangeMatches));
        OnPropertyChanged(nameof(ShowExtendedMatches));
        OnPropertyChanged(nameof(ShowExtendedChangeMatches));
    }

    [ObservableProperty]
    public partial string SymbolColorHex { get; set; } = SmartTickerSettings.DefaultSymbolColor;

    [ObservableProperty]
    public partial string ExtendedPriceColorHex { get; set; } = SmartTickerSettings.DefaultExtendedPriceColor;

    [ObservableProperty]
    public partial string PriceColorHex { get; set; } = SmartTickerSettings.DefaultPriceColor;

    [ObservableProperty]
    public partial string NewsColorHex { get; set; } = SmartTickerSettings.DefaultNewsColor;

    [ObservableProperty]
    public partial string NewsColor2Hex { get; set; } = SmartTickerSettings.DefaultNewsColor2;

    [ObservableProperty]
    public partial string NewsColor3Hex { get; set; } = SmartTickerSettings.DefaultNewsColor3;

    [ObservableProperty]
    public partial string NewsColor4Hex { get; set; } = SmartTickerSettings.DefaultNewsColor4;

    [ObservableProperty]
    public partial string PriceUpColorHex { get; set; } = SmartTickerSettings.DefaultPriceUpColor;

    [ObservableProperty]
    public partial string PriceDownColorHex { get; set; } = SmartTickerSettings.DefaultPriceDownColor;

    [ObservableProperty]
    public partial string AlertBlinkColorHex { get; set; } = SmartTickerSettings.DefaultAlertBlinkColor;

    [ObservableProperty]
    public partial int PriceRefreshSeconds { get; set; } = SmartTickerSettings.DefaultPriceRefreshSeconds;

    [ObservableProperty]
    public partial int NewsRefreshSeconds { get; set; } = SmartTickerSettings.DefaultNewsRefreshSeconds;

    partial void OnPriceRefreshSecondsChanged(int value) => SaveSettings();

    partial void OnNewsRefreshSecondsChanged(int value) => SaveSettings();

    private TickerLayout Layout =>
        TickerLayoutCalculator.Calculate(WindowHeight, PriceRowCount, NewsRowCount, ShowPriceLine, ShowNewsLine);

    public bool IsPriceVisible => ShowPriceLine;

    public bool IsScrollingPriceView => ShowPriceLine && !UseStaticGroupedView;

    public bool IsStaticGroupedPriceView => ShowPriceLine && UseStaticGroupedView;

    public bool IsScrollingTickerView => !UseStaticGroupedView;

    public bool IsStaticTableTickerView => UseStaticGroupedView;

    public bool IsScrollingPricesOnlyView => !UseStaticGroupedView && !ShowNewsLine;

    public bool IsScrollingPricesWithNewsView => !UseStaticGroupedView && ShowNewsLine;

    public bool IsStaticPricesOnlyView => UseStaticGroupedView && !ShowNewsLine;

    public bool IsStaticPricesWithNewsView => UseStaticGroupedView && ShowNewsLine;

    public bool IsScrollingNewsView => IsNewsVisible && !UseStaticGroupedView;

    public bool IsStaticGroupedNewsView => ShowNewsLine && UseStaticGroupedView;

    public double MinimumMainWindowHeight => UseStaticGroupedView
        ? SmartTickerSettings.MinimumStaticPricesWindowHeight
        : RequiredScrollingWindowHeight;

    public bool IsStaticTickerContentVisible => UseStaticGroupedView && ShowPriceLine;

    public bool HasQuoteGroups => QuoteGroups.Count > 0;

    public IReadOnlyList<string> GroupNameOptions => QuoteGroups.Select(group => group.Name).ToArray();

    public bool HasSelectedQuoteGroup => SelectedQuoteGroup is not null;

    public bool HasSelectedGroupQuote => SelectedGroupQuote is not null;

    public bool CanAssociateGroupQuote => SelectedQuoteGroup is not null && SelectedGroupQuote is not null;

    public bool CanUngroupSelectedQuote => !string.IsNullOrWhiteSpace(SelectedGroupQuote?.GroupName);

    public bool HasStaticQuoteGroups => StaticQuoteGroups.Count > 0;

    public bool HasStaticNewsGroups => StaticNewsGroups.Count > 0;

    public bool IsNewsVisible => ShowNewsLine && Layout.ShowNews;

    private IBrush _backgroundBrush = ToBackgroundBrush(
        SmartTickerSettings.DefaultBackgroundColor,
        SmartTickerSettings.DefaultOpacity);
    private IBrush _symbolBrush = ToBrush(SmartTickerSettings.DefaultSymbolColor, SmartTickerSettings.DefaultSymbolColor);
    private IBrush _extendedPriceBrush = ToBrush(
        SmartTickerSettings.DefaultExtendedPriceColor,
        SmartTickerSettings.DefaultExtendedPriceColor);
    private IBrush _priceBrush = ToBrush(SmartTickerSettings.DefaultPriceColor, SmartTickerSettings.DefaultPriceColor);
    private readonly IBrush[] _newsBrushCycle =
    [
        ToBrush(SmartTickerSettings.DefaultNewsColor, SmartTickerSettings.DefaultNewsColor),
        ToBrush(SmartTickerSettings.DefaultNewsColor2, SmartTickerSettings.DefaultNewsColor2),
        ToBrush(SmartTickerSettings.DefaultNewsColor3, SmartTickerSettings.DefaultNewsColor3),
        ToBrush(SmartTickerSettings.DefaultNewsColor4, SmartTickerSettings.DefaultNewsColor4),
    ];
    private IBrush _priceUpBrush = ToBrush(SmartTickerSettings.DefaultPriceUpColor, SmartTickerSettings.DefaultPriceUpColor);
    private IBrush _priceDownBrush = ToBrush(SmartTickerSettings.DefaultPriceDownColor, SmartTickerSettings.DefaultPriceDownColor);
    private IBrush _alertBlinkBrush = ToBrush(
        SmartTickerSettings.DefaultAlertBlinkColor,
        SmartTickerSettings.DefaultAlertBlinkColor);

    // Alpha on the background keeps the desktop visible through the bar while the text stays crisp.
    public IBrush BackgroundBrush => _backgroundBrush;

    public string BackgroundOpacityText => $"{BackgroundOpacity * 100:0}%";

    public IBrush SymbolBrush => _symbolBrush;

    public IBrush ExtendedPriceBrush => _extendedPriceBrush;

    public IBrush PriceBrush => _priceBrush;

    public IBrush NewsBrush => _newsBrushCycle[0];

    public IBrush NewsAlternateBrush => _newsBrushCycle[1];

    public IBrush NewsBrush3 => _newsBrushCycle[2];

    public IBrush NewsBrush4 => _newsBrushCycle[3];

    private IReadOnlyList<IBrush> NewsBrushCycle => _newsBrushCycle;

    public IBrush PriceUpBrush => _priceUpBrush;

    public IBrush PriceDownBrush => _priceDownBrush;

    public IBrush AlertBlinkBrush => _alertBlinkBrush;

    private static readonly IBrush AlertFlashTextBrush = new ImmutableSolidColorBrush(0xFF000000u);

    private const int ChangeBlinkSeconds = 3;

    // Fixed brown so a refreshed price or a brand-new headline reads differently from a fired alert.
    private static readonly IBrush ChangeBlinkBrush = new ImmutableSolidColorBrush(0xFF8B4513u);

    private static readonly IBrush ChangeBlinkTextBrush = new ImmutableSolidColorBrush(0xFFFFFFFFu);

    private static IBrush ToBrush(string hex, string fallback) =>
        new SolidColorBrush(Color.Parse(HexColor.TryNormalize(hex, out var normalized) ? normalized : fallback));

    private static IBrush ToBackgroundBrush(string hex, double opacity)
    {
        var color = Color.Parse(HexColor.TryNormalize(hex, out var normalized)
            ? normalized
            : SmartTickerSettings.DefaultBackgroundColor);
        var alpha = (byte)Math.Round(Math.Clamp(
            opacity,
            SmartTickerSettings.MinimumOpacity,
            SmartTickerSettings.MaximumOpacity) * 255);
        return new SolidColorBrush(Color.FromArgb(alpha, color.R, color.G, color.B));
    }

    private readonly IPriceSelectorDiscovery? _selectorDiscovery;
    private readonly INewsSelectorDiscovery? _newsSelectorDiscovery;
    private readonly IQuoteFetcher? _quoteFetcher;
    private readonly INewsFetcher? _newsFetcher;
    private readonly ISettingsStore? _settingsStore;
    private readonly ILinkLauncher? _linkLauncher;
    private readonly IStarterSettingsSource? _starterSettings;
    private readonly IAlertStore? _alertStore;
    private readonly IAlertSound? _alertSound;
    private readonly IStartupRegistration? _startupRegistration;
    private readonly WebsiteAccessPolicy _websiteAccessPolicy;
    private readonly DispatcherTimer _blinkTimer = new() { Interval = TimeSpan.FromMilliseconds(500) };
    private readonly DispatcherTimer _configReloadTimer = new() { Interval = TimeSpan.FromMilliseconds(400) };
    private FileSystemWatcher? _settingsWatcher;
    private FileSystemWatcher? _alertsWatcher;
    private bool _settingsFileChanged;
    private bool _alertsFileChanged;
    private int _configReloadAttempts;
    private DateTimeOffset _lastSelfWrite;
    private readonly Dictionary<Guid, DateTimeOffset> _blinkingUntil = [];
    private readonly Dictionary<Guid, DateTimeOffset> _priceChangeBlinkUntil = [];
    private readonly Dictionary<(Guid SubscriptionId, string Headline), DateTimeOffset> _newHeadlineBlinkUntil = [];
    private readonly Dictionary<Guid, QuoteSnapshot> _latestQuotesBySubscription = [];
    private readonly Dictionary<Guid, NewsSnapshot> _latestNewsBySubscription = [];
    private readonly Dictionary<Guid, string> _priceRefreshErrors = [];
    private readonly Dictionary<Guid, string> _newsRefreshErrors = [];
    private readonly AlertArmingState _arming = new();
    private bool _blinkOn;
    private readonly RefreshWorkCoordinator _refreshCoordinator = new(4);
    private int _refreshGeneration;
    private readonly NewsRepeatFilter _newsRepeatFilter = new();
    private readonly HashSet<Guid> _hiddenNewsQuotes = [];
    private readonly List<string> _quoteGroupNames = [];
    private SourceAcknowledgementLedger _acknowledgements = new();
    private readonly CancellationTokenSource _lifetimeCancellation = new();
    private bool _isApplyingSettings;
    private bool _isCapturingWindowSize;
    private bool _isApplyingConfiguredWindowSize;
    private volatile bool _isDisposed;
    private bool _settingsPersistenceBlocked;

    public MainViewModel()
        : this(null, null, null, null)
    {
    }

    public MainViewModel(
        IPriceSelectorDiscovery? selectorDiscovery,
        IQuoteFetcher? quoteFetcher,
        INewsSelectorDiscovery? newsSelectorDiscovery = null,
        ISettingsStore? settingsStore = null,
        INewsFetcher? newsFetcher = null,
        ILinkLauncher? linkLauncher = null,
        IStarterSettingsSource? starterSettings = null,
        IAlertStore? alertStore = null,
        IAlertSound? alertSound = null,
        IStartupRegistration? startupRegistration = null,
        WebsiteAccessPolicy? websiteAccessPolicy = null)
    {
        _alertStore = alertStore;
        _alertSound = alertSound;
        _startupRegistration = startupRegistration;
        _websiteAccessPolicy = websiteAccessPolicy ?? new WebsiteAccessPolicy();
        _starterSettings = starterSettings;
        _selectorDiscovery = selectorDiscovery;
        _quoteFetcher = quoteFetcher;
        _newsSelectorDiscovery = newsSelectorDiscovery;
        _settingsStore = settingsStore;
        _newsFetcher = newsFetcher;
        _linkLauncher = linkLauncher;
        LatestQuotes.CollectionChanged += OnLatestQuotesChanged;
        LatestNews.CollectionChanged += OnLatestNewsChanged;
        _blinkTimer.Tick += (_, _) => RunSafely("Blink update", OnBlinkTick);
        SelectedSource = SourceAlternatives[0];
        LoadSettings();
        LoadAlerts();
        StartWatchingConfigFiles();
    }

    private void OnLatestQuotesChanged(object? sender, NotifyCollectionChangedEventArgs change) =>
        UpdateSnapshotIndex(change, _latestQuotesBySubscription, snapshot => snapshot.SubscriptionId);

    private void OnLatestNewsChanged(object? sender, NotifyCollectionChangedEventArgs change) =>
        UpdateSnapshotIndex(change, _latestNewsBySubscription, snapshot => snapshot.SubscriptionId);

    private static void UpdateSnapshotIndex<TSnapshot>(
        NotifyCollectionChangedEventArgs change,
        Dictionary<Guid, TSnapshot> index,
        Func<TSnapshot, Guid> subscriptionId)
        where TSnapshot : class
    {
        if (change.Action == NotifyCollectionChangedAction.Reset)
        {
            index.Clear();
            return;
        }

        if (change.OldItems is not null)
        {
            foreach (var snapshot in change.OldItems.OfType<TSnapshot>())
            {
                index.Remove(subscriptionId(snapshot));
            }
        }

        if (change.NewItems is not null)
        {
            foreach (var snapshot in change.NewItems.OfType<TSnapshot>())
            {
                index[subscriptionId(snapshot)] = snapshot;
            }
        }
    }

    private QuoteSnapshot? LatestQuoteFor(Guid subscriptionId) =>
        _latestQuotesBySubscription.GetValueOrDefault(subscriptionId);

    private NewsSnapshot? LatestNewsFor(Guid subscriptionId) =>
        _latestNewsBySubscription.GetValueOrDefault(subscriptionId);

    public string SourceSummary => string.Join("  •  ", SourceAlternatives.Select(source => source.Name));

    public string SelectedSourcePrefix => SelectedSource?.UrlPrefix ?? string.Empty;

    public string SelectedSourcePolicy => SelectedSource?.PolicySummary ?? string.Empty;

    public string SelectedSourceGuidance => SelectedSource?.Guidance ?? string.Empty;

    public string SelectedSourcePolicyColor =>
        SelectedSource?.CollectionPolicy == CollectionPolicy.RequiresWrittenPermission ? "#D9822B" : "#70E1A1";

    public string? CurrentSourceHost => SourceAcknowledgementLedger.HostOf(NewSourceUrl);

    public bool RequiresAcknowledgement =>
        !AllowWebsiteCookiesAndCrossHostRedirects &&
        CurrentSourceHost is not null &&
        !_acknowledgements.IsAcknowledged(NewSourceUrl);

    public string AcknowledgementText => CurrentSourceHost is null
        ? string.Empty
        : $"Confirm once for {CurrentSourceHost}: you have reviewed its terms and robots rules and are permitted to collect this data.";

    public bool IsEditing => EditingSubscription is not null;

    public string EntryActionText => IsEditing ? "Save changes" : "Add independent entry";

    public string StatusGlyph => IsPaused ? "Ⅱ" : "▶";

    public string StatusColor => IsPaused ? "#F2B84B" : "#70E1A1";

    public string StatusText => IsPaused ? Text.StatusPaused : Text.StatusWorking;

    public UiStrings Text => Translations.For(Language);

    public IReadOnlyList<LanguageOption> LanguageOptions => Translations.Options;

    [RelayCommand]
    private void SetLanguage(string? code) => Language = AppLanguages.Normalize(code);

    [RelayCommand]
    private void SetTickerView(string? mode) =>
        RunSafely("Changing view", () => ApplyTickerView(mode));

    private void ApplyTickerView(string? mode)
    {
        var (useStaticView, showNews) = mode switch
        {
            "scrolling-prices" => (false, false),
            "scrolling-prices-news" => (false, true),
            "static-prices" => (true, false),
            "static-prices-news" => (true, true),
            _ => (UseStaticGroupedView, ShowNewsLine),
        };

        ShowPriceLine = true;
        if (!showNews)
        {
            ShowNewsLine = false;
        }

        UseStaticGroupedView = useStaticView;
        if (showNews)
        {
            ShowNewsLine = true;
        }
    }

    [RelayCommand]
    private void AcknowledgeSource()
    {
        if (_acknowledgements.Acknowledge(NewSourceUrl))
        {
            SyncApprovedSourceHosts();
            EntryMessage = $"Access to {CurrentSourceHost} confirmed. This is remembered for that site.";
            RaiseAcknowledgementChanged();
            SaveSettings();
        }
    }

    public void PrepareQuoteGroupManager()
    {
        RefreshQuoteGroups();
        SelectedQuoteGroup ??= QuoteGroups.FirstOrDefault();
        if (SelectedGroupQuote is not null &&
            !Subscriptions.Any(item => item.Id == SelectedGroupQuote.Id))
        {
            SelectedGroupQuote = null;
        }

        GroupManagerMessage = QuoteGroups.Count == 0
            ? "Create a group, then select a quote to associate with it."
            : "Select a group on the left and a quote on the right.";
    }

    partial void OnSelectedQuoteGroupChanged(QuoteGroupSummary? value)
    {
        ManagedGroupName = value?.Name ?? string.Empty;
        OnPropertyChanged(nameof(HasSelectedQuoteGroup));
        OnPropertyChanged(nameof(CanAssociateGroupQuote));
    }

    partial void OnSelectedGroupQuoteChanged(TickerSubscription? value)
    {
        OnPropertyChanged(nameof(HasSelectedGroupQuote));
        OnPropertyChanged(nameof(CanAssociateGroupQuote));
        OnPropertyChanged(nameof(CanUngroupSelectedQuote));
    }

    [RelayCommand]
    private void CreateQuoteGroup()
    {
        if (!TryGetManagedGroupName(out var name))
        {
            return;
        }

        var existing = QuoteGroups.FirstOrDefault(group =>
            string.Equals(group.Name, name, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            SelectedQuoteGroup = existing;
            GroupManagerMessage = $"The group {existing.Name} already exists.";
            return;
        }

        _quoteGroupNames.Add(name);
        RefreshQuoteGroups();
        SelectedQuoteGroup = QuoteGroups.First(group =>
            string.Equals(group.Name, name, StringComparison.OrdinalIgnoreCase));
        SaveSettings();
        GroupManagerMessage = $"Created {name}. Select a quote and choose Associate.";
    }

    [RelayCommand]
    private void UpdateQuoteGroup()
    {
        if (SelectedQuoteGroup is not { } selected)
        {
            GroupManagerMessage = "Select a group first.";
            return;
        }

        if (!TryGetManagedGroupName(out var replacement))
        {
            return;
        }

        if (string.Equals(selected.Name, replacement, StringComparison.Ordinal))
        {
            GroupManagerMessage = "The group name is unchanged.";
            return;
        }

        var duplicate = QuoteGroups.FirstOrDefault(group =>
            !string.Equals(group.Name, selected.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(group.Name, replacement, StringComparison.OrdinalIgnoreCase));
        if (duplicate is not null)
        {
            GroupManagerMessage = $"The group {duplicate.Name} already exists. Associate quotes with it instead.";
            return;
        }

        var definitionIndex = _quoteGroupNames.FindIndex(name =>
            string.Equals(name, selected.Name, StringComparison.OrdinalIgnoreCase));
        if (definitionIndex < 0)
        {
            GroupManagerMessage = $"The group {selected.Name} no longer exists.";
            return;
        }

        _quoteGroupNames[definitionIndex] = replacement;
        for (var index = 0; index < Subscriptions.Count; index++)
        {
            if (string.Equals(Subscriptions[index].GroupName, selected.Name, StringComparison.OrdinalIgnoreCase))
            {
                ReplaceSubscriptionGroup(index, replacement);
            }
        }

        UpdateTickerLines();
        SaveSettings();
        SelectedQuoteGroup = QuoteGroups.FirstOrDefault(group =>
            string.Equals(group.Name, replacement, StringComparison.OrdinalIgnoreCase));
        GroupManagerMessage = $"Updated {selected.Name} to {replacement}.";
    }

    [RelayCommand]
    private void DeleteQuoteGroup()
    {
        if (SelectedQuoteGroup is not { } selected)
        {
            GroupManagerMessage = "Select a group first.";
            return;
        }

        _quoteGroupNames.RemoveAll(name =>
            string.Equals(name, selected.Name, StringComparison.OrdinalIgnoreCase));
        var changed = 0;
        for (var index = 0; index < Subscriptions.Count; index++)
        {
            if (string.Equals(Subscriptions[index].GroupName, selected.Name, StringComparison.OrdinalIgnoreCase))
            {
                ReplaceSubscriptionGroup(index, null);
                changed++;
            }
        }

        UpdateTickerLines();
        SaveSettings();
        SelectedQuoteGroup = QuoteGroups.FirstOrDefault();
        GroupManagerMessage = $"Deleted {selected.Name} and ungrouped {changed} quote{(changed == 1 ? string.Empty : "s")}.";
    }

    [RelayCommand]
    private void AssociateSelectedQuote()
    {
        if (SelectedQuoteGroup is not { } group || SelectedGroupQuote is not { } quote)
        {
            GroupManagerMessage = "Select one group and one quote first.";
            return;
        }

        var index = FindSubscriptionIndex(quote.Id);
        if (index < 0)
        {
            GroupManagerMessage = $"The quote {quote.Symbol} no longer exists.";
            return;
        }

        var previousGroup = Subscriptions[index].GroupName;
        if (string.Equals(previousGroup, group.Name, StringComparison.OrdinalIgnoreCase))
        {
            GroupManagerMessage = $"{quote.Symbol} is already in {group.Name}.";
            return;
        }

        ReplaceSubscriptionGroup(index, group.Name);
        UpdateTickerLines();
        SaveSettings();
        GroupManagerMessage = string.IsNullOrWhiteSpace(previousGroup)
            ? $"Associated {quote.Symbol} with {group.Name}."
            : $"Moved {quote.Symbol} from {previousGroup} to {group.Name}.";
    }

    [RelayCommand]
    private void UngroupSelectedQuote()
    {
        if (SelectedGroupQuote is not { } quote)
        {
            GroupManagerMessage = "Select a quote first.";
            return;
        }

        var index = FindSubscriptionIndex(quote.Id);
        if (index < 0 || string.IsNullOrWhiteSpace(Subscriptions[index].GroupName))
        {
            GroupManagerMessage = $"{quote.Symbol} is already Ungrouped.";
            return;
        }

        var previousGroup = Subscriptions[index].GroupName;
        ReplaceSubscriptionGroup(index, null);
        UpdateTickerLines();
        SaveSettings();
        GroupManagerMessage = $"Removed {quote.Symbol} from {previousGroup}.";
    }

    private bool TryGetManagedGroupName(out string groupName)
    {
        if (!TickerSubscription.TryNormalizeGroupName(ManagedGroupName, out var normalized, out var error) ||
            normalized is null)
        {
            groupName = string.Empty;
            GroupManagerMessage = error ?? "Enter a group name.";
            return false;
        }

        groupName = normalized;
        return true;
    }

    private int FindSubscriptionIndex(Guid id)
    {
        for (var index = 0; index < Subscriptions.Count; index++)
        {
            if (Subscriptions[index].Id == id)
            {
                return index;
            }
        }

        return -1;
    }

    private void RefreshQuoteGroups()
    {
        var selectedName = SelectedQuoteGroup?.Name;
        foreach (var assignedName in Subscriptions
            .Where(item => !string.IsNullOrWhiteSpace(item.GroupName))
            .Select(item => item.GroupName!)
            .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!_quoteGroupNames.Contains(assignedName, StringComparer.OrdinalIgnoreCase))
            {
                _quoteGroupNames.Add(assignedName);
            }
        }

        var summaries = _quoteGroupNames.Select(name =>
        {
            var members = Subscriptions
                .Where(item => string.Equals(item.GroupName, name, StringComparison.OrdinalIgnoreCase))
                .ToArray();
            return new QuoteGroupSummary(
                name,
                members.Length,
                string.Join(", ", members.Select(item => item.Symbol)));
        }).ToArray();

        SelectedQuoteGroup = null;
        QuoteGroups.Clear();
        foreach (var summary in summaries)
        {
            QuoteGroups.Add(summary);
        }

        SelectedQuoteGroup = QuoteGroups.FirstOrDefault(group =>
            string.Equals(group.Name, selectedName, StringComparison.OrdinalIgnoreCase));
        OnPropertyChanged(nameof(HasQuoteGroups));
        OnPropertyChanged(nameof(GroupNameOptions));
    }

    public void MoveQuoteGroup(string sourceName, string targetName, bool placeAfter)
    {
        if (string.Equals(sourceName, targetName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var source = Subscriptions
            .Where(item => string.Equals(GroupKey(item), sourceName, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        var remaining = Subscriptions
            .Where(item => !string.Equals(GroupKey(item), sourceName, StringComparison.OrdinalIgnoreCase))
            .ToList();
        var targetIndexes = remaining
            .Select((item, index) => (item, index))
            .Where(pair => string.Equals(GroupKey(pair.item), targetName, StringComparison.OrdinalIgnoreCase))
            .Select(pair => pair.index)
            .ToArray();
        if (source.Length == 0 || targetIndexes.Length == 0)
        {
            return;
        }

        var sourceDefinitionIndex = _quoteGroupNames.FindIndex(name =>
            string.Equals(name, sourceName, StringComparison.OrdinalIgnoreCase));
        if (sourceDefinitionIndex >= 0)
        {
            var movedDefinition = _quoteGroupNames[sourceDefinitionIndex];
            _quoteGroupNames.RemoveAt(sourceDefinitionIndex);
            var targetDefinitionIndex = _quoteGroupNames.FindIndex(name =>
                string.Equals(name, targetName, StringComparison.OrdinalIgnoreCase));
            if (targetDefinitionIndex >= 0)
            {
                _quoteGroupNames.Insert(
                    targetDefinitionIndex + (placeAfter ? 1 : 0),
                    movedDefinition);
            }
            else
            {
                _quoteGroupNames.Add(movedDefinition);
            }
        }

        var insertionIndex = placeAfter ? targetIndexes[^1] + 1 : targetIndexes[0];
        remaining.InsertRange(insertionIndex, source);
        Subscriptions.Clear();
        foreach (var subscription in remaining)
        {
            Subscriptions.Add(subscription);
        }

        UpdateTickerLines();
        SaveSettings();
    }

    private static string GroupKey(TickerSubscription subscription) =>
        string.IsNullOrWhiteSpace(subscription.GroupName) ? string.Empty : subscription.GroupName;

    private string ResolveExistingGroupName(string groupName) =>
        _quoteGroupNames.FirstOrDefault(candidate =>
            string.Equals(candidate, groupName, StringComparison.OrdinalIgnoreCase))
        ?? groupName;

    private void ReplaceSubscriptionGroup(int index, string? groupName)
    {
        var original = Subscriptions[index];
        var updated = original with { GroupName = groupName };
        Subscriptions[index] = updated;
        if (EditingSubscription?.Id == original.Id)
        {
            EditingSubscription = updated;
            NewGroupName = groupName ?? string.Empty;
        }

        if (SelectedGroupQuote?.Id == original.Id)
        {
            SelectedGroupQuote = updated;
        }

        if (AlertSubscription?.Id == original.Id)
        {
            AlertSubscription = updated;
        }
    }

    [RelayCommand]
    private void OpenLink(Uri? link)
    {
        if (link is not null)
        {
            _linkLauncher?.TryOpen(link);
        }
    }

    [RelayCommand]
    private void TogglePause()
    {
        IsPaused = !IsPaused;
        NewsLine = IsPaused
            ? "PAUSED  •  Right-click to resume"
            : "NEWS  •  Add RSS/Atom feeds  •  Refresh: 5 min  •  Delayed data — not investment advice";
    }

    [RelayCommand]
    private Task AddSubscriptionAsync() =>
        RunSafelyAsync(IsEditing ? "Updating quote" : "Adding quote", AddSubscriptionCoreAsync);

    private async Task AddSubscriptionCoreAsync()
    {
        if (EditingSubscription is { } currentEdit && FindSubscriptionIndex(currentEdit.Id) < 0)
        {
            EditingSubscription = null;
            EntryMessage = "The quote changed outside this form. Select Edit and try again.";
            return;
        }

        if (RequiresAcknowledgement)
        {
            EntryMessage = $"Confirm your access rights for {CurrentSourceHost} before adding this entry.";
            return;
        }

        if (!TickerSubscription.TryNormalizeGroupName(NewGroupName, out var groupName, out var groupError))
        {
            EntryMessage = groupError!;
            return;
        }

        if (groupName is not null)
        {
            groupName = ResolveExistingGroupName(groupName);
        }

        var valid = EditingSubscription is { } original
            ? TickerSubscription.TryUpdate(
                original, NewSymbol, NewSourceName, NewSourceUrl, NewCollectPrice, NewCollectNews,
                NewCssSelector, NewNewsCssSelector, out var subscription, out var error)
            : TickerSubscription.TryCreate(
                NewSymbol, NewSourceName, NewSourceUrl, NewCollectPrice, NewCollectNews,
                NewCssSelector, NewNewsCssSelector, out subscription, out error);
        if (!valid)
        {
            EntryMessage = error ?? "The source could not be added.";
            return;
        }

        subscription = subscription! with
        {
            GroupName = groupName,
            PreMarketCssSelector = string.IsNullOrWhiteSpace(NewPreMarketCssSelector) ? null : NewPreMarketCssSelector.Trim(),
            PreMarketChangeCssSelector = string.IsNullOrWhiteSpace(NewPreMarketChangeCssSelector)
                ? null
                : NewPreMarketChangeCssSelector.Trim(),
            ExtendedCssSelector = string.IsNullOrWhiteSpace(NewExtendedCssSelector) ? null : NewExtendedCssSelector.Trim(),
            ExtendedChangeCssSelector = string.IsNullOrWhiteSpace(NewExtendedChangeCssSelector)
                ? null
                : NewExtendedChangeCssSelector.Trim(),
            ChangeCssSelector = string.IsNullOrWhiteSpace(NewChangeCssSelector) ? null : NewChangeCssSelector.Trim(),
        };

        if (EditingSubscription is { } editing)
        {
            var index = FindSubscriptionIndex(editing.Id);
            Subscriptions[index] = subscription!.WithNewsRepeatLimit(NewNewsRepeatLimit);
            _newsRepeatFilter.Forget(editing.Id);
            EntryMessage = $"Updated {subscription!.Symbol} from {subscription.SourceName}.";
            await ReconcileAlertsAfterRenameAsync(editing, subscription!.Symbol);
        }
        else
        {
            subscription = subscription!.WithNewsRepeatLimit(NewNewsRepeatLimit);
            Subscriptions.Add(subscription);
            EntryMessage = $"Added {subscription.Symbol} from {subscription.SourceName}. Duplicate ticker entries remain independent.";
        }

        ClearEntryForm();
        UpdateTickerLines();
        SaveSettings();
        if (subscription.CollectPrice)
        {
            await RefreshPricesAsync();
        }

        if (subscription.CollectNews)
        {
            await RefreshNewsAsync();
        }
    }

    [RelayCommand]
    private void EditSubscription(TickerSubscription? subscription)
    {
        if (subscription is null)
        {
            return;
        }

        EditingSubscription = subscription;
        NewSymbol = subscription.Symbol;
        NewGroupName = subscription.GroupName ?? string.Empty;
        var preset = SourceAlternatives.FirstOrDefault(source => source.TryGetSuffix(subscription.SourceUri, out _));
        if (preset is null)
        {
            preset = SourceAlternatives.First(source => source.HomePage is null);
            SelectedSource = preset;
            NewSourceUrlSuffix = subscription.SourceUri.AbsoluteUri;
            NewSourceName = subscription.SourceName;
        }
        else
        {
            preset.TryGetSuffix(subscription.SourceUri, out var suffix);
            SelectedSource = preset;
            NewSourceUrlSuffix = suffix;
        }
        NewCssSelector = subscription.CssSelector ?? string.Empty;
        NewPreMarketCssSelector = subscription.PreMarketCssSelector ?? string.Empty;
        NewPreMarketChangeCssSelector = subscription.PreMarketChangeCssSelector ?? string.Empty;
        NewExtendedCssSelector = subscription.ExtendedCssSelector ?? string.Empty;
        NewExtendedChangeCssSelector = subscription.ExtendedChangeCssSelector ?? string.Empty;
        NewChangeCssSelector = subscription.ChangeCssSelector ?? string.Empty;
        NewNewsCssSelector = subscription.NewsCssSelector ?? string.Empty;
        NewNewsRepeatLimit = subscription.NewsRepeatLimit;
        NewCollectPrice = subscription.CollectPrice;
        NewCollectNews = subscription.CollectNews;
        SelectorSuggestions.Clear();
        NewsSelectorSuggestions.Clear();
        EntryMessage = $"Editing {subscription.Symbol} from {subscription.SourceName}.";
    }

    [RelayCommand]
    private void CancelEdit()
    {
        ClearEntryForm();
        EntryMessage = "Edit cancelled. Duplicate symbols are allowed.";
    }

    [RelayCommand]
    private Task RemoveSubscriptionAsync(TickerSubscription? subscription) =>
        RunSafelyAsync("Removing quote", () => RemoveSubscriptionCoreAsync(subscription));

    private async Task RemoveSubscriptionCoreAsync(TickerSubscription? subscription)
    {
        if (subscription is null)
        {
            return;
        }

        var alertCount = CountAlertsFor(subscription.Id);
        if (alertCount > 0)
        {
            var remove = ConfirmAlertRemoval is null ||
                await ConfirmAlertRemoval(subscription.Symbol, alertCount);
            if (remove)
            {
                DropAlertsFor(subscription.Id);
            }
        }

        var index = FindSubscriptionIndex(subscription.Id);
        if (index < 0)
        {
            EntryMessage = $"{subscription.Symbol} was already removed.";
            return;
        }

        subscription = Subscriptions[index];
        Subscriptions.RemoveAt(index);
        _hiddenNewsQuotes.Remove(subscription.Id);
        if (SelectedGroupQuote?.Id == subscription.Id)
        {
            SelectedGroupQuote = null;
        }

        var quote = LatestQuoteFor(subscription.Id);
        if (quote is not null)
        {
            LatestQuotes.Remove(quote);
        }

        var news = LatestNewsFor(subscription.Id);
        if (news is not null)
        {
            LatestNews.Remove(news);
        }

        _newsRepeatFilter.Forget(subscription.Id);

        UpdateTickerLines();
        SaveSettings();
    }

    [RelayCommand]
    private void MoveSubscriptionUp(TickerSubscription? subscription) => MoveSubscription(subscription, -1);

    [RelayCommand]
    private void MoveSubscriptionDown(TickerSubscription? subscription) => MoveSubscription(subscription, 1);

    private void MoveSubscription(TickerSubscription? subscription, int offset)
    {
        if (subscription is null)
        {
            return;
        }

        var index = Subscriptions.IndexOf(subscription);
        var target = index + offset;
        if (index < 0 || target < 0 || target >= Subscriptions.Count)
        {
            return;
        }

        Subscriptions.Move(index, target);
        UpdateTickerLines();
        SaveSettings();
    }

    public Task RefreshPricesAsync() => RefreshPricesCoreAsync(null);

    private async Task RefreshPricesCoreAsync(IReadOnlyCollection<Guid>? subscriptionIds)
    {
        if (_quoteFetcher is null || IsPaused || _isDisposed)
        {
            return;
        }

        try
        {
            var generation = Volatile.Read(ref _refreshGeneration);
            var requested = subscriptionIds?.ToHashSet();
            var priceSubscriptions = Subscriptions
                .Where(item =>
                    item.CollectPrice &&
                    CanAccessSource(item.SourceUri) &&
                    (requested is null || requested.Contains(item.Id)))
                .ToArray();
            var work = priceSubscriptions
                .Select(subscription =>
                {
                    var lease = _refreshCoordinator.TryAcquire(RefreshStream.Prices, subscription.Id);
                    return lease is null ? null : FetchPriceAsync(subscription, lease);
                })
                .Where(task => task is not null)
                .Select(task => task!)
                .ToArray();
            if (work.Length == 0)
            {
                return;
            }

            var results = await Task.WhenAll(work).ConfigureAwait(false);
            _lifetimeCancellation.Token.ThrowIfCancellationRequested();
            await RunOnUiThreadAsync(() => CommitPriceResults(results, generation)).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (_lifetimeCancellation.IsCancellationRequested)
        {
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            await RunOnUiThreadAsync(() => ReportRecoverableError("Price refresh", exception))
                .ConfigureAwait(false);
        }
    }

    private async Task<PriceFetchResult> FetchPriceAsync(
        TickerSubscription subscription,
        IDisposable lease)
    {
        using (lease)
        {
            try
            {
                var snapshot = await _quoteFetcher!
                    .FetchAsync(subscription, _lifetimeCancellation.Token)
                    .ConfigureAwait(false);
                return new PriceFetchResult(subscription, snapshot, null);
            }
            catch (OperationCanceledException) when (_lifetimeCancellation.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
            {
                return new PriceFetchResult(subscription, null, exception);
            }
        }
    }

    private sealed record PriceFetchResult(
        TickerSubscription Subscription,
        QuoteSnapshot? Snapshot,
        Exception? Error);

    private void CommitPriceResults(IReadOnlyList<PriceFetchResult> results, int generation)
    {
        if (_isDisposed || IsPaused || generation != Volatile.Read(ref _refreshGeneration))
        {
            return;
        }

        var changeBlinkUntil = DateTimeOffset.Now.AddSeconds(ChangeBlinkSeconds);
        var changed = false;
        var renderedStateChanged = false;
        var successfulRefresh = false;
        foreach (var result in results)
        {
            var subscription = Subscriptions.FirstOrDefault(item => item.Id == result.Subscription.Id);
            if (subscription is null || subscription != result.Subscription)
            {
                continue;
            }

            var previous = LatestQuoteFor(subscription.Id);
            var failure = result.Error?.Message ?? (result.Snapshot is { Success: false } failed ? failed.Status : null);
            if (failure is not null)
            {
                renderedStateChanged |= !_priceRefreshErrors.TryGetValue(subscription.Id, out var oldError) ||
                    oldError != failure;
                _priceRefreshErrors[subscription.Id] = failure;
                EntryMessage = $"Price refresh failed: {failure}";
                if (previous is { Success: true })
                {
                    continue;
                }

                var failedSnapshot = result.Snapshot ?? new QuoteSnapshot(
                    subscription.Id,
                    subscription.Symbol,
                    subscription.SourceName,
                    null,
                    null,
                    DateTimeOffset.UtcNow,
                    false,
                    failure);
                ReplaceQuoteSnapshot(previous, failedSnapshot);
                renderedStateChanged = true;
                continue;
            }

            var snapshot = result.Snapshot!;
            renderedStateChanged |= _priceRefreshErrors.Remove(subscription.Id);
            if (HasPriceChanged(previous, snapshot))
            {
                _priceChangeBlinkUntil[subscription.Id] = changeBlinkUntil;
                changed = true;
            }

            ReplaceQuoteSnapshot(previous, snapshot);
            renderedStateChanged = true;
            successfulRefresh = true;
        }

        if (changed)
        {
            StartBlinking();
        }

        if (renderedStateChanged)
        {
            UpdatePriceRows();
        }

        if (successfulRefresh)
        {
            EvaluateAlerts();
        }
    }

    private void ReplaceQuoteSnapshot(QuoteSnapshot? previous, QuoteSnapshot current)
    {
        if (previous is not null)
        {
            LatestQuotes.Remove(previous);
        }

        LatestQuotes.Add(current);
    }

    public async Task RefreshPricesSafelyAsync(string operation)
    {
        try
        {
            await RefreshPricesAsync();
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            ReportRecoverableError(operation, exception);
        }
    }

    internal async Task RefreshPriceSubscriptionsSafelyAsync(
        string operation,
        IReadOnlyCollection<Guid> subscriptionIds)
    {
        try
        {
            await RefreshPricesCoreAsync(subscriptionIds);
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            ReportRecoverableError(operation, exception);
        }
    }

    public Task RefreshNewsAsync() => RefreshNewsCoreAsync(null);

    private async Task RefreshNewsCoreAsync(IReadOnlyCollection<Guid>? subscriptionIds)
    {
        if (_newsFetcher is null || IsPaused || _isDisposed)
        {
            return;
        }

        try
        {
            var generation = Volatile.Read(ref _refreshGeneration);
            var requested = subscriptionIds?.ToHashSet();
            var newsSubscriptions = Subscriptions
                .Where(item =>
                    item.CollectNews &&
                    CanAccessSource(item.SourceUri) &&
                    (requested is null || requested.Contains(item.Id)))
                .ToArray();
            var work = newsSubscriptions
                .Select(subscription =>
                {
                    var lease = _refreshCoordinator.TryAcquire(RefreshStream.News, subscription.Id);
                    return lease is null ? null : FetchNewsAsync(subscription, lease);
                })
                .Where(task => task is not null)
                .Select(task => task!)
                .ToArray();
            if (work.Length == 0)
            {
                return;
            }

            var results = await Task.WhenAll(work).ConfigureAwait(false);
            _lifetimeCancellation.Token.ThrowIfCancellationRequested();
            await RunOnUiThreadAsync(() => CommitNewsResults(results, generation)).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (_lifetimeCancellation.IsCancellationRequested)
        {
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            await RunOnUiThreadAsync(() => ReportRecoverableError("News refresh", exception))
                .ConfigureAwait(false);
        }
    }

    private async Task<NewsFetchResult> FetchNewsAsync(
        TickerSubscription subscription,
        IDisposable lease)
    {
        using (lease)
        {
            try
            {
                var snapshot = await _newsFetcher!
                    .FetchAsync(subscription, _lifetimeCancellation.Token)
                    .ConfigureAwait(false);
                return new NewsFetchResult(subscription, snapshot, null);
            }
            catch (OperationCanceledException) when (_lifetimeCancellation.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
            {
                return new NewsFetchResult(subscription, null, exception);
            }
        }
    }

    private sealed record NewsFetchResult(
        TickerSubscription Subscription,
        NewsSnapshot? Snapshot,
        Exception? Error);

    private void CommitNewsResults(IReadOnlyList<NewsFetchResult> results, int generation)
    {
        if (_isDisposed || IsPaused || generation != Volatile.Read(ref _refreshGeneration))
        {
            return;
        }

        var changeBlinkUntil = DateTimeOffset.Now.AddSeconds(ChangeBlinkSeconds);
        var changed = false;
        var renderedStateChanged = false;
        foreach (var result in results)
        {
            var subscription = Subscriptions.FirstOrDefault(item => item.Id == result.Subscription.Id);
            if (subscription is null || subscription != result.Subscription)
            {
                continue;
            }

            var previous = LatestNewsFor(subscription.Id);
            var failure = result.Error?.Message ?? (result.Snapshot is { Success: false } failed ? failed.Status : null);
            if (failure is not null)
            {
                renderedStateChanged |= !_newsRefreshErrors.TryGetValue(subscription.Id, out var oldError) ||
                    oldError != failure;
                _newsRefreshErrors[subscription.Id] = failure;
                EntryMessage = $"News refresh failed: {failure}";
                if (previous is { Success: true })
                {
                    continue;
                }

                var failedSnapshot = result.Snapshot ?? new NewsSnapshot(
                    subscription.Id,
                    subscription.Symbol,
                    subscription.SourceName,
                    [],
                    DateTimeOffset.UtcNow,
                    false,
                    failure);
                ReplaceNewsSnapshot(previous, failedSnapshot);
                renderedStateChanged = true;
                continue;
            }

            var snapshot = result.Snapshot!;
            _newsRefreshErrors.Remove(subscription.Id);
            snapshot = snapshot with
            {
                Headlines = _newsRepeatFilter.Filter(
                    subscription.Id,
                    snapshot.Headlines,
                    subscription.NewsRepeatLimit),
            };
            foreach (var headline in NewHeadlinesSince(previous, snapshot))
            {
                _newHeadlineBlinkUntil[(subscription.Id, headline)] = changeBlinkUntil;
                changed = true;
            }

            ReplaceNewsSnapshot(previous, snapshot);
            renderedStateChanged = true;
        }

        if (changed)
        {
            StartBlinking();
        }

        if (renderedStateChanged)
        {
            UpdateNewsRows();
            UpdatePriceRows();
        }
    }

    private void ReplaceNewsSnapshot(NewsSnapshot? previous, NewsSnapshot current)
    {
        if (previous is not null)
        {
            LatestNews.Remove(previous);
        }

        LatestNews.Add(current);
    }

    public async Task RefreshNewsSafelyAsync(string operation)
    {
        try
        {
            await RefreshNewsAsync();
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            ReportRecoverableError(operation, exception);
        }
    }

    internal async Task RefreshNewsSubscriptionsSafelyAsync(
        string operation,
        IReadOnlyCollection<Guid> subscriptionIds)
    {
        try
        {
            await RefreshNewsCoreAsync(subscriptionIds);
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            ReportRecoverableError(operation, exception);
        }
    }

    private static async Task RunOnUiThreadAsync(Action action)
    {
        if (Avalonia.Application.Current is null || Dispatcher.UIThread.CheckAccess())
        {
            action();
            return;
        }

        await Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Background);
    }

    public void ReportRecoverableError(string operation, Exception exception)
    {
        EntryMessage = $"{operation} failed: {exception.Message}";
    }

    [RelayCommand]
    private Task DiscoverSelectorsAsync() => DiscoverForAsync(SelectorKind.Price);

    [RelayCommand]
    private Task DiscoverChangeSelectorsAsync() => DiscoverForAsync(SelectorKind.Change);

    [RelayCommand]
    private Task DiscoverPreMarketSelectorsAsync() => DiscoverForAsync(SelectorKind.PreMarketPrice);

    [RelayCommand]
    private Task DiscoverPreMarketChangeSelectorsAsync() => DiscoverForAsync(SelectorKind.PreMarketChange);

    [RelayCommand]
    private Task DiscoverExtendedSelectorsAsync() => DiscoverForAsync(SelectorKind.ExtendedPrice);

    [RelayCommand]
    private Task DiscoverExtendedChangeSelectorsAsync() => DiscoverForAsync(SelectorKind.ExtendedChange);

    private static string DescribeKind(SelectorKind kind) => kind switch
    {
        SelectorKind.Change => "price change",
        SelectorKind.PreMarketPrice => "pre-market price",
        SelectorKind.PreMarketChange => "pre-market change",
        SelectorKind.ExtendedPrice => "after-hours price",
        SelectorKind.ExtendedChange => "after-hours change",
        _ => "price",
    };

    private async Task DiscoverForAsync(SelectorKind kind)
    {
        SelectorSuggestions.Clear();
        DiscoveryTarget = kind;
        var label = DescribeKind(kind);
        if (_selectorDiscovery is null)
        {
            DiscoveryMessage = "Selector discovery is unavailable in the designer.";
            return;
        }

        if (!Uri.TryCreate(NewSourceUrl, UriKind.Absolute, out var uri))
        {
            DiscoveryMessage = "Enter a valid source URL before discovery.";
            return;
        }

        if (!CanAccessSource(uri))
        {
            DiscoveryMessage = "Approve this website before requesting selector discovery.";
            return;
        }

        try
        {
            IsDiscovering = true;
            DiscoveryMessage = $"Inspecting public static HTML for the {label}…";
            var suggestions = await _selectorDiscovery.DiscoverAsync(uri, kind);
            foreach (var suggestion in suggestions)
            {
                SelectorSuggestions.Add(suggestion);
            }

            DiscoveryMessage = suggestions.Count == 0
                ? $"No reliable {label} selector was found. The page may require JavaScript or a manual selector."
                : $"Found {suggestions.Count} possible {label} selector(s). Test the selected value before saving.";
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            DiscoveryMessage = exception is OperationCanceledException
                ? "Selector discovery timed out."
                : exception.Message;
        }
        finally
        {
            IsDiscovering = false;
        }
    }

    [RelayCommand]
    private void UseSelectorSuggestion(CssSelectorSuggestion? suggestion)
    {
        if (suggestion is null)
        {
            return;
        }

        switch (DiscoveryTarget)
        {
            case SelectorKind.Change:
                NewChangeCssSelector = suggestion.Selector;
                break;
            case SelectorKind.PreMarketPrice:
                NewPreMarketCssSelector = suggestion.Selector;
                break;
            case SelectorKind.PreMarketChange:
                NewPreMarketChangeCssSelector = suggestion.Selector;
                break;
            case SelectorKind.ExtendedPrice:
                NewExtendedCssSelector = suggestion.Selector;
                break;
            case SelectorKind.ExtendedChange:
                NewExtendedChangeCssSelector = suggestion.Selector;
                break;
            default:
                NewCssSelector = suggestion.Selector;
                break;
        }

        DiscoveryMessage = $"Selected {suggestion.Selector} as the {DescribeKind(DiscoveryTarget)} selector. Verify it with the source before saving.";
    }

    [RelayCommand]
    private async Task DiscoverNewsSelectorsAsync()
    {
        NewsSelectorSuggestions.Clear();
        if (_newsSelectorDiscovery is null)
        {
            NewsDiscoveryMessage = "News selector discovery is unavailable in the designer.";
            return;
        }

        if (!Uri.TryCreate(NewSourceUrl, UriKind.Absolute, out var uri))
        {
            NewsDiscoveryMessage = "Enter a valid source URL before discovery.";
            return;
        }

        if (!CanAccessSource(uri))
        {
            NewsDiscoveryMessage = "Approve this website before requesting news selector discovery.";
            return;
        }

        try
        {
            IsDiscoveringNews = true;
            NewsDiscoveryMessage = "Inspecting public static HTML for headline links…";
            var suggestions = await _newsSelectorDiscovery.DiscoverAsync(uri);
            foreach (var suggestion in suggestions)
            {
                NewsSelectorSuggestions.Add(suggestion);
            }

            NewsDiscoveryMessage = suggestions.Count == 0
                ? "No reliable news selector was found. The page may require JavaScript or a manual selector."
                : $"Found {suggestions.Count} possible news selector(s). Verify one before saving.";
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            NewsDiscoveryMessage = exception is OperationCanceledException ? "News selector discovery timed out." : exception.Message;
        }
        finally
        {
            IsDiscoveringNews = false;
        }
    }

    [RelayCommand]
    private void UseNewsSelectorSuggestion(CssSelectorSuggestion? suggestion)
    {
        if (suggestion is null)
        {
            return;
        }

        NewNewsCssSelector = suggestion.Selector;
        NewsDiscoveryMessage = $"Selected news selector {suggestion.Selector}. Verify it before saving.";
    }

    [RelayCommand]
    private async Task ValidateSourceAsync()
    {
        // The symbol only labels the result, so a placeholder keeps the URL testable before it is typed.
        var symbol = string.IsNullOrWhiteSpace(NewSymbol) ? "TEST" : NewSymbol;
        if (!TickerSubscription.TryCreate(
                symbol,
                NewSourceName,
                NewSourceUrl,
                NewCollectPrice,
                NewCollectNews,
                NewCssSelector,
                NewNewsCssSelector,
                out var probe,
                out var error))
        {
            ValidationMessage = error ?? "The entry is not valid.";
            return;
        }

        if (!CanAccessSource(probe!.SourceUri))
        {
            ValidationMessage = "Approve this website before validating it.";
            return;
        }

        try
        {
            IsValidating = true;
            ValidationMessage = $"Requesting {probe!.SourceUri.Host}…";
            var results = new List<string>();

            if (probe.CollectPrice && _quoteFetcher is not null)
            {
                var quote = await _quoteFetcher.FetchAsync(probe, _lifetimeCancellation.Token);
                results.Add(quote switch
                {
                    { Success: true, Price: { } price } => $"price {price:N2}",
                    _ => $"no price ({quote.Status})",
                });
            }

            if (probe.CollectNews && _newsFetcher is not null)
            {
                var news = await _newsFetcher.FetchAsync(probe, _lifetimeCancellation.Token);
                results.Add(news is { Success: true, Headlines.Count: > 0 }
                    ? $"{news.Headlines.Count} headline(s)"
                    : $"no headlines ({news.Status})");
            }

            ValidationMessage = results.Count == 0
                ? "Validation is unavailable in the designer."
                : $"{probe.SourceUri.Host} → {string.Join("; ", results)}";
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            ValidationMessage = exception is OperationCanceledException
                ? "The page did not respond in time."
                : exception.Message;
        }
        finally
        {
            IsValidating = false;
        }
    }

    public IReadOnlyList<SourcePermissionReview> GetPendingSourcePermissionReviews() =>
        AllowWebsiteCookiesAndCrossHostRedirects
            ? []
            : [.. Subscriptions
            .GroupBy(item => item.SourceUri.Host, StringComparer.OrdinalIgnoreCase)
            .Where(group => !_acknowledgements.IsAcknowledged(group.First().SourceUri.AbsoluteUri))
            .Select(group =>
            {
                var first = group.First();
                var preset = KnownSourceCatalog.All.FirstOrDefault(item =>
                    item.HomePage is not null &&
                    string.Equals(item.HomePage.Host, group.Key, StringComparison.OrdinalIgnoreCase));
                return new SourcePermissionReview(
                    first.SourceUri,
                    group.Key,
                    string.Join(", ", group.Select(item => item.SourceName).Distinct(StringComparer.OrdinalIgnoreCase)),
                    string.Join(", ", group.Select(item => item.Symbol).Distinct(StringComparer.OrdinalIgnoreCase)),
                    preset?.PolicySummary ?? "Review required",
                    preset?.Guidance ?? "Review this website's terms, privacy policy, robots rules, and automated-access policy.");
            })];

    public void ApproveSourcePermission(SourcePermissionReview review)
    {
        if (_acknowledgements.Acknowledge(review.SourceUri.AbsoluteUri))
        {
            SyncApprovedSourceHosts();
            RaiseAcknowledgementChanged();
            SaveSettings();
        }
    }

    public async Task ValidateAllSourcesAsync()
    {
        if (IsValidatingAllSources)
        {
            return;
        }

        var subscriptions = Subscriptions.ToArray();
        SourceValidationProblems.Clear();
        HasSourceValidationProblems = false;
        if (subscriptions.Length == 0)
        {
            SourceValidationStatus = "There are no configured sources to validate.";
            return;
        }

        var passed = 0;
        var failed = 0;
        var skipped = 0;
        try
        {
            IsValidatingAllSources = true;
            for (var index = 0; index < subscriptions.Length; index++)
            {
                var subscription = subscriptions[index];
                SourceValidationStatus = $"Validating {index + 1} of {subscriptions.Length}: {subscription.Symbol}…";
                if (!CanAccessSource(subscription.SourceUri))
                {
                    skipped++;
                    SourceValidationProblems.Add($"{subscription.Symbol}: source permission was not approved.");
                    continue;
                }

                var problems = new List<string>();
                if (subscription.CollectPrice)
                {
                    if (_quoteFetcher is null)
                    {
                        problems.Add("price validation is unavailable");
                    }
                    else
                    {
                        var quote = await _quoteFetcher.FetchAsync(subscription, _lifetimeCancellation.Token);
                        if (!quote.Success || quote.Price is null)
                        {
                            problems.Add($"price: {quote.Status}");
                        }
                    }
                }

                if (subscription.CollectNews)
                {
                    if (_newsFetcher is null)
                    {
                        problems.Add("news validation is unavailable");
                    }
                    else
                    {
                        var news = await _newsFetcher.FetchAsync(subscription, _lifetimeCancellation.Token);
                        if (!news.Success || news.Headlines.Count == 0)
                        {
                            problems.Add($"news: {news.Status}");
                        }
                    }
                }

                if (problems.Count == 0)
                {
                    passed++;
                }
                else
                {
                    failed++;
                    SourceValidationProblems.Add($"{subscription.Symbol}: {string.Join("; ", problems)}");
                }
            }

            HasSourceValidationProblems = SourceValidationProblems.Count > 0;
            SourceValidationStatus = $"Validation complete: {passed} passed, {failed} failed, {skipped} skipped.";
        }
        catch (OperationCanceledException) when (_lifetimeCancellation.IsCancellationRequested)
        {
            SourceValidationStatus = "Source validation was cancelled because SmartTicker is closing.";
        }
        finally
        {
            IsValidatingAllSources = false;
        }
    }

    partial void OnEditingSubscriptionChanged(TickerSubscription? value)
    {
        OnPropertyChanged(nameof(IsEditing));
        OnPropertyChanged(nameof(EntryActionText));
    }

    partial void OnSelectedSourceChanged(SourcePreset? value)
    {
        NewSourceName = value?.Name ?? string.Empty;
        OnPropertyChanged(nameof(SelectedSourcePrefix));
        OnPropertyChanged(nameof(SelectedSourcePolicy));
        OnPropertyChanged(nameof(SelectedSourceGuidance));
        OnPropertyChanged(nameof(SelectedSourcePolicyColor));
        RebuildSourceUrl();
    }

    partial void OnNewSourceUrlSuffixChanged(string value) => RebuildSourceUrl();

    partial void OnNewSourceUrlChanged(string value) => RaiseAcknowledgementChanged();

    private void RaiseAcknowledgementChanged()
    {
        OnPropertyChanged(nameof(CurrentSourceHost));
        OnPropertyChanged(nameof(RequiresAcknowledgement));
        OnPropertyChanged(nameof(AcknowledgementText));
    }

    partial void OnIsPausedChanged(bool value)
    {
        Interlocked.Increment(ref _refreshGeneration);
        OnPropertyChanged(nameof(StatusGlyph));
        OnPropertyChanged(nameof(StatusColor));
        OnPropertyChanged(nameof(StatusText));
        UpdateVisibleRows();
    }

    private void UpdateTickerLines()
    {
        RefreshQuoteGroups();
        var priceEntries = Subscriptions.Where(item => item.CollectPrice).ToArray();
        var newsEntries = Subscriptions.Where(item => item.CollectNews).ToArray();

        QuoteLine = priceEntries.Length == 0
            ? "PRICES  •  Add an authorized webpage in Settings  •  Refresh: 1 min"
            : "PRICES  •  " + string.Join("  •  ", priceEntries.Select(item => $"{item.Symbol} — {item.SourceName}"));
        NewsLine = newsEntries.Length == 0
            ? "NEWS  •  Add RSS/Atom feeds in Settings  •  Refresh: 5 min"
            : "NEWS  •  " + string.Join("  •  ", newsEntries.Select(item => $"{item.Symbol} — {item.SourceName}"));
        OnPropertyChanged(nameof(ShowStarterPrompt));
        UpdateVisibleRows();
    }

    partial void OnLanguageChanged(string value)
    {
        OnPropertyChanged(nameof(Text));
        OnPropertyChanged(nameof(StatusText));
        UpdatePriceRows();
        UpdateNewsRows();
        SaveSettings();
    }

    partial void OnPriceRowCountChanged(int value)
    {
        var clamped = Math.Clamp(value, 1, 8);
        if (value != clamped)
        {
            PriceRowCount = clamped;
            return;
        }

        EnsureMainWindowCanShowSelectedView();
        UpdateVisibleRows();
        SaveSettings();
    }

    partial void OnNewsRowCountChanged(int value)
    {
        var clamped = Math.Clamp(value, 1, 8);
        if (value != clamped)
        {
            NewsRowCount = clamped;
            return;
        }

        EnsureMainWindowCanShowSelectedView();
        UpdateVisibleRows();
        SaveSettings();
    }

    partial void OnScrollingViewFontSizeChanged(int value)
    {
        var clamped = Math.Clamp(
            value,
            SmartTickerSettings.MinimumViewFontSize,
            SmartTickerSettings.MaximumViewFontSize);
        if (value != clamped)
        {
            ScrollingViewFontSize = clamped;
            return;
        }

        EnsureMainWindowCanShowSelectedView();
        UpdatePriceRows();
        UpdateNewsRows();
        SaveSettings();
    }

    partial void OnStaticViewFontSizeChanged(int value)
    {
        var clamped = Math.Clamp(
            value,
            SmartTickerSettings.MinimumViewFontSize,
            SmartTickerSettings.MaximumViewFontSize);
        if (value != clamped)
        {
            StaticViewFontSize = clamped;
            return;
        }

        SaveSettings();
    }

    partial void OnScrollingWindowWidthChanged(int value) =>
        ApplyWindowDimension(
            value,
            SmartTickerSettings.MinimumWindowWidth,
            SmartTickerSettings.MaximumWindowWidth,
            newValue => ScrollingWindowWidth = newValue,
            isActive: !UseStaticGroupedView,
            isWidth: true);

    partial void OnScrollingWindowHeightChanged(int value) =>
        ApplyWindowDimension(
            value,
            RequiredScrollingWindowHeight,
            SmartTickerSettings.MaximumScrollingWindowHeight,
            newValue => ScrollingWindowHeight = newValue,
            isActive: !UseStaticGroupedView,
            isWidth: false);

    partial void OnStaticPricesWindowWidthChanged(int value) =>
        ApplyWindowDimension(
            value,
            SmartTickerSettings.MinimumWindowWidth,
            SmartTickerSettings.MaximumWindowWidth,
            newValue => StaticPricesWindowWidth = newValue,
            isActive: UseStaticGroupedView,
            isWidth: true);

    partial void OnStaticPricesWindowHeightChanged(int value) =>
        ApplyWindowDimension(
            value,
            SmartTickerSettings.MinimumStaticPricesWindowHeight,
            SmartTickerSettings.MaximumStaticWindowHeight,
            newValue => StaticPricesWindowHeight = newValue,
            isActive: UseStaticGroupedView,
            isWidth: false);

    partial void OnStaticNewsWindowWidthChanged(int value) =>
        ApplyWindowDimension(
            value,
            SmartTickerSettings.MinimumWindowWidth,
            SmartTickerSettings.MaximumWindowWidth,
            newValue => StaticNewsWindowWidth = newValue,
            isActive: false,
            isWidth: true);

    partial void OnStaticNewsWindowHeightChanged(int value) =>
        ApplyWindowDimension(
            value,
            SmartTickerSettings.MinimumStaticNewsWindowHeight,
            SmartTickerSettings.MaximumStaticWindowHeight,
            newValue => StaticNewsWindowHeight = newValue,
            isActive: false,
            isWidth: false);

    private void ApplyWindowDimension(
        int value,
        int minimum,
        int maximum,
        Action<int> assignClamped,
        bool isActive,
        bool isWidth)
    {
        var clamped = Math.Clamp(value, minimum, maximum);
        if (value != clamped)
        {
            assignClamped(clamped);
            return;
        }

        if (isActive)
        {
            if (isWidth)
            {
                WindowWidth = value;
            }
            else
            {
                WindowHeight = value;
            }
        }

        if (!_isCapturingWindowSize)
        {
            SaveSettings();
        }
    }

    internal void CaptureMainWindowSize(double width, double height)
    {
        var capturedWidth = Math.Clamp(
            (int)Math.Round(width),
            SmartTickerSettings.MinimumWindowWidth,
            SmartTickerSettings.MaximumWindowWidth);
        var minimumHeight = UseStaticGroupedView
            ? SmartTickerSettings.MinimumStaticPricesWindowHeight
            : SmartTickerSettings.MinimumScrollingWindowHeight;
        var maximumHeight = UseStaticGroupedView
            ? SmartTickerSettings.MaximumStaticWindowHeight
            : SmartTickerSettings.MaximumScrollingWindowHeight;
        var capturedHeight = Math.Clamp((int)Math.Round(height), minimumHeight, maximumHeight);

        _isCapturingWindowSize = true;
        try
        {
            WindowWidth = capturedWidth;
            WindowHeight = capturedHeight;
            if (UseStaticGroupedView)
            {
                StaticPricesWindowWidth = capturedWidth;
                StaticPricesWindowHeight = capturedHeight;
            }
            else
            {
                ScrollingWindowWidth = capturedWidth;
                ScrollingWindowHeight = capturedHeight;
            }
        }
        finally
        {
            _isCapturingWindowSize = false;
        }
    }

    internal void CaptureStaticNewsWindowSize(double width, double height)
    {
        _isCapturingWindowSize = true;
        try
        {
            StaticNewsWindowWidth = Math.Clamp(
                (int)Math.Round(width),
                SmartTickerSettings.MinimumWindowWidth,
                SmartTickerSettings.MaximumWindowWidth);
            StaticNewsWindowHeight = Math.Clamp(
                (int)Math.Round(height),
                SmartTickerSettings.MinimumStaticNewsWindowHeight,
                SmartTickerSettings.MaximumStaticWindowHeight);
        }
        finally
        {
            _isCapturingWindowSize = false;
        }
    }

    private void ApplyConfiguredMainWindowSize()
    {
        _isApplyingConfiguredWindowSize = true;
        try
        {
            WindowWidth = UseStaticGroupedView ? StaticPricesWindowWidth : ScrollingWindowWidth;
            WindowHeight = UseStaticGroupedView ? StaticPricesWindowHeight : ScrollingWindowHeight;
        }
        finally
        {
            _isApplyingConfiguredWindowSize = false;
        }

        EnsureMainWindowCanShowSelectedView();
    }

    private int RequiredScrollingWindowHeight => Math.Max(
        SmartTickerSettings.MinimumScrollingWindowHeight,
        (int)Math.Ceiling(TickerLayoutCalculator.MinimumHeight(
            PriceRowCount,
            NewsRowCount,
            ShowPriceLine,
            ShowNewsLine,
            ScrollingViewFontSize + 6)));

    private void EnsureMainWindowCanShowSelectedView()
    {
        OnPropertyChanged(nameof(MinimumMainWindowHeight));
        var minimum = (int)Math.Ceiling(MinimumMainWindowHeight);
        if (WindowHeight >= minimum)
        {
            return;
        }

        _isApplyingConfiguredWindowSize = true;
        try
        {
            WindowHeight = minimum;
            if (UseStaticGroupedView)
            {
                StaticPricesWindowHeight = minimum;
            }
            else
            {
                ScrollingWindowHeight = minimum;
            }
        }
        finally
        {
            _isApplyingConfiguredWindowSize = false;
        }
    }

    partial void OnWindowHeightChanged(double value)
    {
        OnPropertyChanged(nameof(IsNewsVisible));
        OnPropertyChanged(nameof(IsScrollingNewsView));
        if (!_isApplyingSettings && !_isApplyingConfiguredWindowSize)
        {
            UpdateVisibleRows();
        }
    }

    partial void OnShowPriceLineChanged(bool value)
    {
        EnsureMainWindowCanShowSelectedView();
        OnPropertyChanged(nameof(IsPriceVisible));
        OnPropertyChanged(nameof(IsScrollingPriceView));
        OnPropertyChanged(nameof(IsStaticGroupedPriceView));
        OnPropertyChanged(nameof(IsStaticTickerContentVisible));
        OnPropertyChanged(nameof(IsNewsVisible));
        OnPropertyChanged(nameof(IsScrollingNewsView));
        UpdateVisibleRows();
        SaveSettings();
    }

    partial void OnShowNewsLineChanged(bool value)
    {
        EnsureMainWindowCanShowSelectedView();
        OnPropertyChanged(nameof(IsNewsVisible));
        OnPropertyChanged(nameof(IsScrollingNewsView));
        OnPropertyChanged(nameof(IsStaticGroupedNewsView));
        OnPropertyChanged(nameof(IsStaticTickerContentVisible));
        OnPropertyChanged(nameof(IsScrollingPricesOnlyView));
        OnPropertyChanged(nameof(IsScrollingPricesWithNewsView));
        OnPropertyChanged(nameof(IsStaticPricesOnlyView));
        OnPropertyChanged(nameof(IsStaticPricesWithNewsView));
        UpdateVisibleRows();
        SaveSettings();
    }

    partial void OnUseStaticGroupedViewChanged(bool value)
    {
        ApplyConfiguredMainWindowSize();
        OnPropertyChanged(nameof(MinimumMainWindowHeight));
        OnPropertyChanged(nameof(IsScrollingPriceView));
        OnPropertyChanged(nameof(IsStaticGroupedPriceView));
        OnPropertyChanged(nameof(IsScrollingTickerView));
        OnPropertyChanged(nameof(IsStaticTableTickerView));
        OnPropertyChanged(nameof(IsScrollingNewsView));
        OnPropertyChanged(nameof(IsStaticGroupedNewsView));
        OnPropertyChanged(nameof(IsStaticTickerContentVisible));
        OnPropertyChanged(nameof(IsScrollingPricesOnlyView));
        OnPropertyChanged(nameof(IsScrollingPricesWithNewsView));
        OnPropertyChanged(nameof(IsStaticPricesOnlyView));
        OnPropertyChanged(nameof(IsStaticPricesWithNewsView));
        UpdateVisibleRows();
        SaveSettings();
    }

    partial void OnLaunchAtLoginChanged(bool value)
    {
        if (_isApplyingSettings || _startupRegistration is null)
        {
            return;
        }

        try
        {
            _startupRegistration.SetEnabled(value);
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            EntryMessage = $"Startup setting could not be changed: {exception.Message}";
            RevertLaunchAtLogin();
            return;
        }

        SaveSettings();
    }

    private void RevertLaunchAtLogin()
    {
        try
        {
            _isApplyingSettings = true;
            LaunchAtLogin = _startupRegistration?.IsEnabled ?? false;
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
        }
        finally
        {
            _isApplyingSettings = false;
        }
    }

    partial void OnAllowWebsiteCookiesAndCrossHostRedirectsChanged(bool value)
    {
        _websiteAccessPolicy.AllowCookiesAndCrossHostRedirects = value;
        RaiseAcknowledgementChanged();
        if (!value)
        {
            var inaccessible = Subscriptions
                .Where(item => !_acknowledgements.IsAcknowledged(item.SourceUri.AbsoluteUri))
                .Select(item => item.Id)
                .ToHashSet();
            foreach (var quote in LatestQuotes.Where(item => inaccessible.Contains(item.SubscriptionId)).ToArray())
            {
                LatestQuotes.Remove(quote);
            }

            foreach (var news in LatestNews.Where(item => inaccessible.Contains(item.SubscriptionId)).ToArray())
            {
                LatestNews.Remove(news);
            }

            UpdatePriceRows();
            UpdateNewsRows();
        }

        SaveSettings();
    }

    private bool CanAccessSource(Uri sourceUri) =>
        AllowWebsiteCookiesAndCrossHostRedirects ||
        _acknowledgements.IsAcknowledged(sourceUri.AbsoluteUri);

    private void SyncApprovedSourceHosts() =>
        _websiteAccessPolicy.ReplaceApprovedHosts(_acknowledgements.ToArray());

    partial void OnBackgroundColorHexChanged(string value)
    {
        _backgroundBrush = ToBackgroundBrush(value, BackgroundOpacity);
        ApplyColorChange(nameof(BackgroundBrush));
    }

    partial void OnBackgroundOpacityChanged(double value)
    {
        _backgroundBrush = ToBackgroundBrush(BackgroundColorHex, value);
        OnPropertyChanged(nameof(BackgroundOpacityText));
        ApplyColorChange(nameof(BackgroundBrush));
    }

    // News brushes are baked into segments, so the rows must be rebuilt.
    partial void OnNewsColorHexChanged(string value)
    {
        _newsBrushCycle[0] = ToBrush(value, SmartTickerSettings.DefaultNewsColor);
        OnPropertyChanged(nameof(NewsBrush));
        UpdateNewsRows();
        SaveSettings();
    }

    partial void OnNewsColor2HexChanged(string value)
    {
        _newsBrushCycle[1] = ToBrush(value, SmartTickerSettings.DefaultNewsColor2);
        OnPropertyChanged(nameof(NewsAlternateBrush));
        UpdateNewsRows();
        SaveSettings();
    }

    partial void OnNewsColor3HexChanged(string value)
    {
        _newsBrushCycle[2] = ToBrush(value, SmartTickerSettings.DefaultNewsColor3);
        OnPropertyChanged(nameof(NewsBrush3));
        UpdateNewsRows();
        SaveSettings();
    }

    partial void OnNewsColor4HexChanged(string value)
    {
        _newsBrushCycle[3] = ToBrush(value, SmartTickerSettings.DefaultNewsColor4);
        OnPropertyChanged(nameof(NewsBrush4));
        UpdateNewsRows();
        SaveSettings();
    }

    // Price row brushes are baked into segments, so the rows must be rebuilt.
    partial void OnSymbolColorHexChanged(string value)
    {
        _symbolBrush = ToBrush(value, SmartTickerSettings.DefaultSymbolColor);
        OnPropertyChanged(nameof(SymbolBrush));
        UpdatePriceRows();
        SaveSettings();
    }

    partial void OnPriceColorHexChanged(string value)
    {
        _priceBrush = ToBrush(value, SmartTickerSettings.DefaultPriceColor);
        OnPropertyChanged(nameof(PriceBrush));
        UpdatePriceRows();
        SaveSettings();
    }

    partial void OnExtendedPriceColorHexChanged(string value)
    {
        _extendedPriceBrush = ToBrush(value, SmartTickerSettings.DefaultExtendedPriceColor);
        OnPropertyChanged(nameof(ExtendedPriceBrush));
        UpdatePriceRows();
        SaveSettings();
    }

    partial void OnPriceUpColorHexChanged(string value)
    {
        _priceUpBrush = ToBrush(value, SmartTickerSettings.DefaultPriceUpColor);
        OnPropertyChanged(nameof(PriceUpBrush));
        UpdatePriceRows();
        SaveSettings();
    }

    partial void OnPriceDownColorHexChanged(string value)
    {
        _priceDownBrush = ToBrush(value, SmartTickerSettings.DefaultPriceDownColor);
        OnPropertyChanged(nameof(PriceDownBrush));
        UpdatePriceRows();
        SaveSettings();
    }

    partial void OnAlertBlinkColorHexChanged(string value)
    {
        _alertBlinkBrush = ToBrush(value, SmartTickerSettings.DefaultAlertBlinkColor);
        OnPropertyChanged(nameof(AlertBlinkBrush));
        UpdatePriceRows();
        SaveSettings();
    }

    private void ApplyColorChange(string brushName)
    {
        OnPropertyChanged(brushName);
        SaveSettings();
    }

    [RelayCommand]
    private void ResetColors()
    {
        BackgroundColorHex = SmartTickerSettings.DefaultBackgroundColor;
        BackgroundOpacity = SmartTickerSettings.DefaultOpacity;
        SymbolColorHex = SmartTickerSettings.DefaultSymbolColor;
        PriceColorHex = SmartTickerSettings.DefaultPriceColor;
        ExtendedPriceColorHex = SmartTickerSettings.DefaultExtendedPriceColor;
        NewsColorHex = SmartTickerSettings.DefaultNewsColor;
        NewsColor2Hex = SmartTickerSettings.DefaultNewsColor2;
        NewsColor3Hex = SmartTickerSettings.DefaultNewsColor3;
        NewsColor4Hex = SmartTickerSettings.DefaultNewsColor4;
        PriceUpColorHex = SmartTickerSettings.DefaultPriceUpColor;
        PriceDownColorHex = SmartTickerSettings.DefaultPriceDownColor;
        AlertBlinkColorHex = SmartTickerSettings.DefaultAlertBlinkColor;
    }

    partial void OnPriceScrollSpeedChanged(int value)
    {
        var clamped = Math.Clamp(value, 10, 200);
        if (value != clamped)
        {
            PriceScrollSpeed = clamped;
            return;
        }

        UpdatePriceRows();
        SaveSettings();
    }

    partial void OnNewsScrollSpeedChanged(int value)
    {
        var clamped = Math.Clamp(value, 10, 200);
        if (value != clamped)
        {
            NewsScrollSpeed = clamped;
            return;
        }

        UpdateNewsRows();
        SaveSettings();
    }

    private void UpdateVisibleRows()
    {
        UpdatePriceRows();
        UpdateNewsRows();
    }

    private void UpdatePriceRows()
    {
        var layout = Layout;
        if (!ShowPriceLine)
        {
            return;
        }

        UpdateStaticQuoteGroups();

        var rows = Subscriptions
            .Where(item => item.CollectPrice)
            .Select(item =>
            {
                var quote = LatestQuoteFor(item.Id);
                var alerting = IsAlerting(item.Id);
                var marker = alerting ? AlertMarker : HasNoNews(item) ? NoNewsMarker : string.Empty;
                var value = quote switch
                {
                    { Success: true, Price: { } price } => $"{price:N2}{FormatCurrency(quote.Currency)}",
                    { Success: false } => Text.Unavailable,
                    _ => Text.Loading,
                };

                var runs = new List<TickerRun>
                {
                    new($"{marker}{item.Symbol} ", SymbolBrush),
                    new(value, PriceBrush),
                };

                if (quote is { Success: true, ChangePercent: { } percent })
                {
                    runs.Add(new(
                        $" ({percent:+0.00;-0.00;0.00}%)",
                        percent < 0 ? PriceDownBrush : PriceUpBrush));
                }

                if (quote is { Success: true, PreMarketPrice: { } preMarket })
                {
                    runs.Add(new($"  {preMarket:N2}{FormatCurrency(quote.Currency)}", ExtendedPriceBrush));
                    if (quote.PreMarketChangePercent is { } preMarketPercent)
                    {
                        runs.Add(new(
                            $" ({preMarketPercent:+0.00;-0.00;0.00}%)",
                            preMarketPercent < 0 ? PriceDownBrush : PriceUpBrush));
                    }
                }

                if (quote is { Success: true, ExtendedPrice: { } extended })
                {
                    runs.Add(new($"  {extended:N2}{FormatCurrency(quote.Currency)}", ExtendedPriceBrush));
                    if (quote.ExtendedChangePercent is { } extendedPercent)
                    {
                        runs.Add(new(
                            $" ({extendedPercent:+0.00;-0.00;0.00}%)",
                            extendedPercent < 0 ? PriceDownBrush : PriceUpBrush));
                    }
                }

                return new TickerSegment(runs, item.SourceUri)
                {
                    Highlight = alerting
                        ? _blinkOn
                            ? new TickerHighlight(AlertBlinkBrush, AlertFlashTextBrush)
                            : new TickerHighlight(AlertFlashTextBrush, AlertBlinkBrush)
                        : _blinkOn && IsPriceChanged(item.Id)
                            ? new TickerHighlight(ChangeBlinkBrush, ChangeBlinkTextBrush)
                            : null,
                };
            })
            .ToArray();
        ReplaceVisibleRows(
            VisiblePriceRows,
            rows,
            PriceRowCount,
            PriceScrollSpeed,
            IsPaused,
            layout.RowHeight,
            ScrollingViewFontSize,
            "Add an authorized webpage in Settings");
    }

    private void UpdateStaticQuoteGroups()
    {
        var groups = Subscriptions
            .Where(item => item.CollectPrice)
            .GroupBy(
                GroupKey,
                StringComparer.OrdinalIgnoreCase)
            .Select(group => new StaticQuoteGroup(
                group.Key,
                group.Select(BuildStaticQuoteRow).ToArray()))
            .ToArray();

        for (var index = 0; index < groups.Length; index++)
        {
            var existingIndex = IndexOfGroup(StaticQuoteGroups, groups[index].Name, index);
            if (existingIndex < 0)
            {
                StaticQuoteGroups.Insert(index, groups[index]);
                continue;
            }

            if (existingIndex != index)
            {
                StaticQuoteGroups.Move(existingIndex, index);
            }

            StaticQuoteGroups[index].UpdateRows(groups[index].Rows);
        }

        while (StaticQuoteGroups.Count > groups.Length)
        {
            StaticQuoteGroups.RemoveAt(StaticQuoteGroups.Count - 1);
        }

        OnPropertyChanged(nameof(HasStaticQuoteGroups));
    }

    private StaticQuoteRow BuildStaticQuoteRow(TickerSubscription subscription)
    {
        var quote = LatestQuoteFor(subscription.Id);
        var alerting = IsAlerting(subscription.Id);
        var marker = alerting ? AlertMarker : HasNoNews(subscription) ? NoNewsMarker : string.Empty;
        var lastText = quote switch
        {
            { Success: true, Price: { } price } => $"{price:N2}",
            { Success: false } => Text.Unavailable,
            _ => Text.Loading,
        };
        var changeText = "—";
        var percentText = "—";
        var changeBrush = PriceBrush;
        if (quote is { Success: true, Price: { } last, ChangePercent: { } percent })
        {
            var divisor = 1m + percent / 100m;
            if (divisor != 0m)
            {
                var change = last - last / divisor;
                changeText = $"{change:+0.00;-0.00;0.00}";
            }

            percentText = $"{percent:+0.00;-0.00;0.00}%";
            changeBrush = percent < 0 ? PriceDownBrush : PriceUpBrush;
        }

        var sessionParts = new List<string>();
        if (quote is { Success: true, PreMarketPrice: { } preMarket })
        {
            sessionParts.Add($"Pre-market {preMarket:N2}{FormatPercent(quote.PreMarketChangePercent)}");
        }

        if (quote is { Success: true, ExtendedPrice: { } extended })
        {
            sessionParts.Add($"After-hours {extended:N2}{FormatPercent(quote.ExtendedChangePercent)}");
        }

        IBrush background = Brushes.Transparent;
        var symbolBrush = SymbolBrush;
        var lastBrush = PriceBrush;
        if (alerting)
        {
            var highlight = _blinkOn
                ? new TickerHighlight(AlertBlinkBrush, AlertFlashTextBrush)
                : new TickerHighlight(AlertFlashTextBrush, AlertBlinkBrush);
            background = highlight.Background;
            symbolBrush = highlight.Foreground;
            lastBrush = highlight.Foreground;
            changeBrush = highlight.Foreground;
        }
        else if (_blinkOn && IsPriceChanged(subscription.Id))
        {
            background = ChangeBlinkBrush;
            symbolBrush = ChangeBlinkTextBrush;
            lastBrush = ChangeBlinkTextBrush;
            changeBrush = ChangeBlinkTextBrush;
        }

        return new StaticQuoteRow(
            subscription.Id,
            $"{marker}{subscription.Symbol}",
            lastText,
            changeText,
            percentText,
            quote?.Status ?? Text.Loading,
            string.Join(" | ", sessionParts),
            subscription.SourceUri,
            background,
            symbolBrush,
            lastBrush,
            changeBrush);
    }

    private static string FormatPercent(decimal? percent) => percent is { } value
        ? $" ({value:+0.00;-0.00;0.00}%)"
        : string.Empty;

    private void UpdateNewsRows()
    {
        var layout = Layout;
        if (!ShowNewsLine)
        {
            return;
        }

        UpdateStaticNewsGroups();

        var groups = Subscriptions
            .Where(item => item.CollectNews)
            .Select(item => (IReadOnlyList<TickerSegment>)BuildNewsSegments(item).ToArray())
            .ToArray();
        var rows = RoundRobinSequencer.Interleave(groups);
        // Colours cycle over the interleaved order, so neighbouring headlines always differ.
        var cycle = NewsBrushCycle;
        var tinted = rows
            .Select((segment, index) => Tint(segment, cycle[index % cycle.Count]))
            .ToArray();
        ReplaceVisibleRows(
            VisibleNewsRows,
            tinted,
            NewsRowCount,
            NewsScrollSpeed,
            IsPaused,
            layout.RowHeight,
            ScrollingViewFontSize,
            "Add a news source in Settings");
    }

    private void UpdateStaticNewsGroups()
    {
        var colorIndex = 0;
        var groups = new List<StaticNewsGroup>();
        foreach (var group in Subscriptions
                     .Where(item => item.CollectNews)
                     .GroupBy(GroupKey, StringComparer.OrdinalIgnoreCase))
        {
            var quoteRows = group
                .Select(item => (IReadOnlyList<StaticNewsRow>)BuildStaticNewsRows(item))
                .ToArray();
            var rows = RoundRobinSequencer.Interleave(quoteRows)
                .Select(row =>
                {
                    row.HeadlineForeground = NewsBrushCycle[colorIndex++ % NewsBrushCycle.Count];
                    return row;
                })
                .ToArray();
            groups.Add(new StaticNewsGroup(
                group.Key,
                rows,
                _hiddenNewsQuotes,
                SetStaticNewsQuoteVisibility));
        }

        for (var index = 0; index < groups.Count; index++)
        {
            var existingIndex = IndexOfGroup(StaticNewsGroups, groups[index].Name, index);
            if (existingIndex < 0)
            {
                StaticNewsGroups.Insert(index, groups[index]);
                continue;
            }

            if (existingIndex != index)
            {
                StaticNewsGroups.Move(existingIndex, index);
            }

            StaticNewsGroups[index].UpdateRows(groups[index].AllRows, _hiddenNewsQuotes);
        }

        while (StaticNewsGroups.Count > groups.Count)
        {
            StaticNewsGroups.RemoveAt(StaticNewsGroups.Count - 1);
        }

        OnPropertyChanged(nameof(HasStaticNewsGroups));
    }

    private static int IndexOfGroup<TGroup>(
        IReadOnlyList<TGroup> groups,
        string name,
        int startIndex)
        where TGroup : class
    {
        for (var index = startIndex; index < groups.Count; index++)
        {
            var groupName = groups[index] switch
            {
                StaticQuoteGroup quoteGroup => quoteGroup.Name,
                StaticNewsGroup newsGroup => newsGroup.Name,
                _ => null,
            };
            if (string.Equals(groupName, name, StringComparison.OrdinalIgnoreCase))
            {
                return index;
            }
        }

        return -1;
    }

    private IReadOnlyList<StaticNewsRow> BuildStaticNewsRows(TickerSubscription subscription)
    {
        var news = LatestNewsFor(subscription.Id);
        if (news is { Success: true, Headlines.Count: > 0 })
        {
            var rows = new List<StaticNewsRow>(news.Headlines.Count);
            foreach (var headline in news.Headlines)
            {
                rows.Add(new StaticNewsRow(
                    subscription.Id,
                    subscription.Symbol,
                    subscription.SourceName,
                    headline.Title,
                    news.Status,
                    headline.Url ?? subscription.SourceUri,
                    SymbolBrush,
                    NewsBrush)
                {
                    Background = _blinkOn && IsNewHeadline(subscription.Id, headline.Title)
                        ? ChangeBlinkBrush
                        : Brushes.Transparent,
                });
            }

            return rows;
        }

        var status = news?.Status ?? Text.Loading;
        return
        [
            new StaticNewsRow(
                subscription.Id,
                subscription.Symbol,
                subscription.SourceName,
                status,
                status,
                subscription.SourceUri,
                SymbolBrush,
                NewsBrush),
        ];
    }

    private void SetStaticNewsQuoteVisibility(Guid subscriptionId, bool isShown)
    {
        var changed = isShown
            ? _hiddenNewsQuotes.Remove(subscriptionId)
            : _hiddenNewsQuotes.Add(subscriptionId);
        if (changed)
        {
            SaveSettings();
        }
    }

    // A headline-less entry is marked in the price row rather than shown as a news error.
    private bool HasNoNews(TickerSubscription item)
    {
        if (!item.CollectNews)
        {
            return true;
        }

        var news = LatestNewsFor(item.Id);
        return news is not null && (!news.Success || news.Headlines.Count == 0);
    }

    public ObservableCollection<AlertRule> AlertRules { get; } = [];

    /// <summary>Set by the window so the view model can ask before discarding alert rules.</summary>
    public Func<string, int, Task<bool>>? ConfirmAlertRemoval { get; set; }

    [ObservableProperty]
    public partial bool AlertSoundEnabled { get; set; } = true;

    [ObservableProperty]
    public partial int AlertBlinkSeconds { get; set; } = AlertSettings.DefaultBlinkSeconds;

    [ObservableProperty]
    public partial int AlertBuzzCount { get; set; } = AlertSettings.DefaultBuzzCount;

    [ObservableProperty]
    public partial string AlertMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial TickerSubscription? AlertSubscription { get; set; }

    [ObservableProperty]
    public partial AlertComparison AlertComparisonChoice { get; set; } = AlertComparison.GreaterThanOrEqual;

    [ObservableProperty]
    public partial string AlertThresholdText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial DateTimeOffset? AlertStartsOn { get; set; }

    [ObservableProperty]
    public partial DateTimeOffset? AlertEndsOn { get; set; }

    [ObservableProperty]
    public partial bool AlertNeverExpires { get; set; } = true;

    [ObservableProperty]
    public partial AlertRule? EditingAlertRule { get; set; }

    public bool IsEditingAlertRule => EditingAlertRule is not null;

    public string AlertSubmitText => EditingAlertRule is null ? "Add rule" : "Update rule";

    partial void OnEditingAlertRuleChanged(AlertRule? value)
    {
        OnPropertyChanged(nameof(IsEditingAlertRule));
        OnPropertyChanged(nameof(AlertSubmitText));
    }

    public IReadOnlyList<AlertComparison> ComparisonOptions { get; } = Enum.GetValues<AlertComparison>();

    public string AlertStoreLocation => _alertStore?.FilePath ?? "(not available in the designer)";

    public string SettingsStoreLocation => _settingsStore?.FilePath ?? "(not available in the designer)";

    public void PersistSettings() => SaveSettings();

    public void PersistAlerts() => SaveAlerts();

    /// <summary>Applies a file edited outside SmartTicker; the file is already current, so it is not written back.</summary>
    public SettingsImportResult ApplyEditedSettingsJson(string? json)
    {
        var result = SettingsImportValidator.Validate(json);
        if (!result.Success)
        {
            ReportEditedConfigRejected("settings.json", result.Errors);
            return result;
        }

        ApplySettings(result.Settings!);
        _settingsPersistenceBlocked = false;
        UpdateTickerLines();
        RaiseAcknowledgementChanged();
        ReportImportSuccess("the edited settings.json", result.Settings!.Subscriptions.Length);
        return result;
    }

    public AlertsImportResult ApplyEditedAlertsJson(string? json)
    {
        var result = ImportAlertsJson(json, persist: false);
        if (!result.Success)
        {
            ReportEditedConfigRejected("alerts.json", result.Errors);
        }

        return result;
    }

    private void ReportEditedConfigRejected(string fileName, IReadOnlyList<string> problems)
    {
        ReportImportFailure(
            fileName,
            [
                .. problems,
                "Correct the file, or restore a valid export with Import settings\u2026 or Import alert rules\u2026.",
            ]);
    }

    private void StartWatchingConfigFiles()
    {
        _configReloadTimer.Tick += (_, _) => RunSafely("Configuration reload", ReloadChangedConfigFiles);
        _settingsWatcher = CreateConfigWatcher(_settingsStore?.FilePath, () => _settingsFileChanged = true);
        _alertsWatcher = CreateConfigWatcher(_alertStore?.FilePath, () => _alertsFileChanged = true);
    }

    private FileSystemWatcher? CreateConfigWatcher(string? filePath, Action onChanged)
    {
        var directory = string.IsNullOrWhiteSpace(filePath) ? null : Path.GetDirectoryName(filePath);
        var fileName = string.IsNullOrWhiteSpace(filePath) ? null : Path.GetFileName(filePath);
        if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(fileName) || !Directory.Exists(directory))
        {
            return null;
        }

        try
        {
            var watcher = new FileSystemWatcher(directory, fileName)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName,
            };

            // Editors save through temp files and renames, so every event restarts one debounce window.
            void Queue(object? sender, FileSystemEventArgs args)
            {
                if (_isDisposed)
                {
                    return;
                }

                try
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        if (_isDisposed)
                        {
                            return;
                        }

                        RunSafely("Configuration file change", () =>
                        {
                            onChanged();
                            _configReloadAttempts = 0;
                            _configReloadTimer.Stop();
                            _configReloadTimer.Start();
                        });
                    });
                }
                catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
                {
                }
            }

            watcher.Changed += Queue;
            watcher.Created += Queue;
            watcher.Renamed += (sender, args) => Queue(sender, args);
            watcher.EnableRaisingEvents = true;
            return watcher;
        }
        catch (Exception exception)
            when (exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            return null;
        }
    }

    private void ReloadChangedConfigFiles()
    {
        if (_isDisposed)
        {
            return;
        }

        _configReloadTimer.Stop();

        // SmartTicker's own saves raise the same events; only an outside edit should be re-imported.
        if (DateTimeOffset.Now - _lastSelfWrite < TimeSpan.FromSeconds(1))
        {
            _settingsFileChanged = false;
            _alertsFileChanged = false;
            return;
        }

        if (_settingsFileChanged)
        {
            if (!TryReadConfigFile(_settingsStore?.FilePath, out var settingsJson))
            {
                RetryConfigReload();
                return;
            }

            _settingsFileChanged = false;
            ApplyEditedSettingsJson(settingsJson);
        }

        if (_alertsFileChanged)
        {
            if (!TryReadConfigFile(_alertStore?.FilePath, out var alertsJson))
            {
                RetryConfigReload();
                return;
            }

            _alertsFileChanged = false;
            ApplyEditedAlertsJson(alertsJson);
        }
    }

    private void RetryConfigReload()
    {
        if (++_configReloadAttempts > 5)
        {
            _settingsFileChanged = false;
            _alertsFileChanged = false;
            EntryMessage = "An edited configuration file stayed locked by another program, so it was not reloaded.";
            return;
        }

        _configReloadTimer.Start();
    }

    private static bool TryReadConfigFile(string? filePath, out string json)
    {
        json = string.Empty;
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return false;
        }

        try
        {
            json = File.ReadAllText(filePath);
            return true;
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            return false;
        }
    }

    private bool _isApplyingAlerts;

    private void LoadAlerts()
    {
        if (_alertStore is null)
        {
            return;
        }

        try
        {
            var alerts = _alertStore.Load();
            _isApplyingAlerts = true;
            try
            {
                AlertSoundEnabled = alerts.SoundEnabled;
                AlertBlinkSeconds = alerts.BlinkSeconds;
                AlertBuzzCount = alerts.BuzzCount;
                AlertRules.Clear();
                foreach (var rule in alerts.Rules)
                {
                    AlertRules.Add(rule);
                }
            }
            finally
            {
                _isApplyingAlerts = false;
            }
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            AlertMessage = $"Alert rules could not be loaded: {exception.Message}";
            EntryMessage = AlertMessage;
        }
    }

    private void SaveAlerts()
    {
        if (_alertStore is null || _isApplyingAlerts)
        {
            return;
        }

        try
        {
            _alertStore.Save(new AlertSettings
            {
                Rules = [.. AlertRules],
                SoundEnabled = AlertSoundEnabled,
                BlinkSeconds = AlertBlinkSeconds,
                BuzzCount = AlertBuzzCount,
            });
            _lastSelfWrite = DateTimeOffset.Now;
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            AlertMessage = $"Alert rules could not be saved: {exception.Message}";
            EntryMessage = AlertMessage;
        }
    }

    partial void OnAlertSoundEnabledChanged(bool value) => SaveAlerts();

    partial void OnAlertBlinkSecondsChanged(int value) => SaveAlerts();

    partial void OnAlertBuzzCountChanged(int value) => SaveAlerts();

    [RelayCommand]
    private void SaveAlertRule()
    {
        if (AlertSubscription is not { } subscription)
        {
            AlertMessage = "Choose the quote this alert watches.";
            return;
        }

        if (!decimal.TryParse(AlertThresholdText, NumberStyles.Number, CultureInfo.InvariantCulture, out var threshold))
        {
            AlertMessage = "Enter a numeric threshold, for example 250.50.";
            return;
        }

        var starts = AlertStartsOn;
        var ends = AlertNeverExpires ? null : AlertEndsOn;
        if (starts is { } from && ends is { } to && to < from)
        {
            AlertMessage = "The end date cannot be before the start date.";
            return;
        }

        if (EditingAlertRule is { } editing)
        {
            var index = AlertRules.IndexOf(editing);
            if (index < 0)
            {
                ClearAlertForm();
                AlertMessage = "That rule no longer exists.";
                return;
            }

            AlertRules[index] = editing with
            {
                SubscriptionId = subscription.Id,
                Symbol = subscription.Symbol,
                Comparison = AlertComparisonChoice,
                Threshold = threshold,
                StartsOn = starts,
                EndsOn = ends,
            };

            // The condition changed, so a rule already sitting in its fired state must re-arm.
            _arming.Rearm(editing.Id);
            ClearAlertForm();
            SaveAlerts();
            AlertMessage = $"Updated alert for {subscription.Symbol}.";
            EvaluateAlerts();
            return;
        }

        AlertRules.Add(new AlertRule
        {
            Id = Guid.NewGuid(),
            SubscriptionId = subscription.Id,
            Symbol = subscription.Symbol,
            Comparison = AlertComparisonChoice,
            Threshold = threshold,
            StartsOn = starts,
            EndsOn = ends,
        });
        ClearAlertForm();
        SaveAlerts();
        AlertMessage = $"Added alert for {subscription.Symbol}.";
        EvaluateAlerts();
    }

    [RelayCommand]
    private void EditAlertRule(AlertRule? rule)
    {
        if (rule is null || !AlertRules.Contains(rule))
        {
            return;
        }

        AlertSubscription = Subscriptions.FirstOrDefault(item => item.Id == rule.SubscriptionId);
        AlertComparisonChoice = rule.Comparison;
        AlertThresholdText = rule.Threshold.ToString(CultureInfo.InvariantCulture);
        AlertStartsOn = rule.StartsOn;
        AlertNeverExpires = rule.EndsOn is null;
        AlertEndsOn = rule.EndsOn;
        EditingAlertRule = rule;
        AlertMessage = $"Editing the {rule.Symbol} alert.";
    }

    [RelayCommand]
    private void CancelAlertEdit()
    {
        ClearAlertForm();
        AlertMessage = string.Empty;
    }

    private void ClearAlertForm()
    {
        EditingAlertRule = null;
        AlertThresholdText = string.Empty;
        AlertStartsOn = null;
        AlertEndsOn = null;
        AlertNeverExpires = true;
    }

    [RelayCommand]
    private void RemoveAlertRule(AlertRule? rule)
    {
        if (rule is null)
        {
            return;
        }

        AlertRules.Remove(rule);
        _arming.Rearm(rule.Id);
        if (EditingAlertRule == rule)
        {
            ClearAlertForm();
        }

        SaveAlerts();
        AlertMessage = $"Removed alert for {rule.Symbol}.";
    }

    [RelayCommand]
    private void ToggleAlertRule(AlertRule? rule)
    {
        if (rule is null)
        {
            return;
        }

        var index = AlertRules.IndexOf(rule);
        if (index < 0)
        {
            return;
        }

        var updated = rule with { Enabled = !rule.Enabled };
        AlertRules[index] = updated;

        // Re-arm so a re-enabled rule can fire again against the price it is already breaching.
        _arming.Rearm(rule.Id);
        if (!updated.Enabled &&
            !AlertRules.Any(other => other.SubscriptionId == rule.SubscriptionId && _arming.IsFiring(other.Id)))
        {
            _blinkingUntil.Remove(rule.SubscriptionId);
        }

        SaveAlerts();
        AlertMessage = updated.Enabled
            ? $"Enabled alert for {updated.Symbol}."
            : $"Disabled alert for {updated.Symbol}.";
        // Runs last so a rule that fires immediately reports that instead of the toggle message.
        EvaluateAlerts();
        UpdatePriceRows();
    }

    public int CountAlertsFor(Guid subscriptionId) =>
        AlertRules.Count(rule => rule.SubscriptionId == subscriptionId);

    private void DropAlertsFor(Guid subscriptionId)
    {
        foreach (var rule in AlertRules.Where(rule => rule.SubscriptionId == subscriptionId).ToArray())
        {
            AlertRules.Remove(rule);
            _arming.Rearm(rule.Id);
        }

        _blinkingUntil.Remove(subscriptionId);
        SaveAlerts();
    }

    // Rules store the symbol for display, so a renamed quote must carry the new one.
    private void RenameAlertsFor(Guid subscriptionId, string symbol)
    {
        for (var index = 0; index < AlertRules.Count; index++)
        {
            if (AlertRules[index].SubscriptionId == subscriptionId && AlertRules[index].Symbol != symbol)
            {
                AlertRules[index] = AlertRules[index] with { Symbol = symbol };
            }
        }

        SaveAlerts();
    }

    private void EvaluateAlerts()
    {
        if (AlertRules.Count == 0)
        {
            return;
        }

        var now = DateTimeOffset.Now;
        var fired = new List<AlertRule>();
        foreach (var rule in AlertRules)
        {
            var quote = LatestQuoteFor(rule.SubscriptionId);
            if (quote is not { Success: true, Price: { } price })
            {
                _arming.Rearm(rule.Id);
                continue;
            }

            if (_arming.ShouldNotify(rule, price, now))
            {
                fired.Add(rule);
            }
        }

        if (fired.Count == 0)
        {
            return;
        }

        var until = now.AddSeconds(AlertBlinkSeconds);
        foreach (var rule in fired)
        {
            _blinkingUntil[rule.SubscriptionId] = until;
        }

        AlertMessage = fired.Count == 1
            ? $"Alert fired: {fired[0].Summary}"
            : $"{fired.Count} alerts fired.";

        if (AlertSoundEnabled)
        {
            _alertSound?.Buzz(AlertBuzzCount);
        }

        _blinkOn = true;
        if (!_blinkTimer.IsEnabled)
        {
            _blinkTimer.Start();
        }

        UpdatePriceRows();
    }

    private void StartBlinking()
    {
        _blinkOn = true;
        if (!_blinkTimer.IsEnabled)
        {
            _blinkTimer.Start();
        }
    }

    private static bool HasPriceChanged(QuoteSnapshot? previous, QuoteSnapshot current) =>
        previous is { Success: true, Price: { } earlier } &&
        current is { Success: true, Price: { } latest } &&
        earlier != latest;

    private static IEnumerable<string> NewHeadlinesSince(NewsSnapshot? previous, NewsSnapshot current)
    {
        // Without a previous successful sync nothing qualifies as new, so a first load never blinks.
        if (previous is not { Success: true } || current is not { Success: true })
        {
            return [];
        }

        var known = previous.Headlines.Select(headline => headline.Title).ToHashSet(StringComparer.Ordinal);
        return current.Headlines
            .Select(headline => headline.Title)
            .Where(title => !known.Contains(title))
            .Distinct(StringComparer.Ordinal);
    }

    private void OnBlinkTick()
    {
        if (_isDisposed)
        {
            return;
        }

        var updatePrices = _blinkingUntil.Count > 0 || _priceChangeBlinkUntil.Count > 0;
        var updateNews = _newHeadlineBlinkUntil.Count > 0;
        var now = DateTimeOffset.Now;
        foreach (var key in _blinkingUntil.Where(pair => pair.Value <= now).Select(pair => pair.Key).ToArray())
        {
            _blinkingUntil.Remove(key);
        }

        foreach (var key in _priceChangeBlinkUntil.Where(pair => pair.Value <= now).Select(pair => pair.Key).ToArray())
        {
            _priceChangeBlinkUntil.Remove(key);
        }

        foreach (var key in _newHeadlineBlinkUntil.Where(pair => pair.Value <= now).Select(pair => pair.Key).ToArray())
        {
            _newHeadlineBlinkUntil.Remove(key);
        }

        if (_blinkingUntil.Count == 0 && _priceChangeBlinkUntil.Count == 0 && _newHeadlineBlinkUntil.Count == 0)
        {
            _blinkTimer.Stop();
            _blinkOn = false;
        }
        else
        {
            _blinkOn = !_blinkOn;
        }

        if (updatePrices)
        {
            UpdatePriceRows();
        }

        if (updateNews)
        {
            UpdateNewsRows();
        }
    }

    private bool IsAlerting(Guid subscriptionId) => _blinkingUntil.ContainsKey(subscriptionId);

    private bool IsPriceChanged(Guid subscriptionId) => _priceChangeBlinkUntil.ContainsKey(subscriptionId);

    private bool IsNewHeadline(Guid subscriptionId, string headline) =>
        _newHeadlineBlinkUntil.ContainsKey((subscriptionId, headline));

    private async Task ReconcileAlertsAfterRenameAsync(TickerSubscription original, string newSymbol)
    {
        if (original.Symbol == newSymbol)
        {
            return;
        }

        var alertCount = CountAlertsFor(original.Id);
        if (alertCount == 0)
        {
            return;
        }

        if (ConfirmAlertRemoval is not null && await ConfirmAlertRemoval(original.Symbol, alertCount))
        {
            DropAlertsFor(original.Id);
            EntryMessage += $" Removed {alertCount} alert rule(s).";
            return;
        }

        RenameAlertsFor(original.Id, newSymbol);
    }


    private static TickerSegment Tint(TickerSegment segment, IBrush brush) =>
        segment with { Runs = segment.Runs.Select(run => run with { Brush = brush }).ToArray() };

    private IEnumerable<TickerSegment> BuildNewsSegments(TickerSubscription item)
    {
        var news = LatestNewsFor(item.Id);
        if (news is not { Success: true })
        {
            return [];
        }

        return news.Headlines.Select(headline => new TickerSegment(
            $"{item.Symbol} — {headline.Title}",
            headline.Url ?? item.SourceUri)
        {
            Highlight = _blinkOn && IsNewHeadline(item.Id, headline.Title)
                ? new TickerHighlight(ChangeBlinkBrush, ChangeBlinkTextBrush)
                : null,
        });
    }

    private static void ReplaceVisibleRows(
        ObservableCollection<TickerLane> target,
        IReadOnlyList<TickerSegment> source,
        int rowCount,
        int pixelsPerSecond,
        bool isPaused,
        double rowHeight,
        double fontSize,
        string emptyMessage)
    {
        var lanes = new List<IReadOnlyList<TickerSegment>>();
        if (source.Count == 0)
        {
            lanes.Add([new TickerSegment(emptyMessage, null)]);
        }
        else
        {
            var count = Math.Min(Math.Clamp(rowCount, 1, 8), source.Count);
            var rows = Enumerable.Range(0, count).Select(_ => new List<TickerSegment>()).ToArray();
            for (var index = 0; index < source.Count; index++)
            {
                rows[index % count].Add(source[index]);
            }

            lanes.AddRange(rows);
        }

        // Reuse the existing lanes so the marquee controls survive the refresh and keep scrolling.
        while (target.Count > lanes.Count)
        {
            target.RemoveAt(target.Count - 1);
        }

        for (var index = 0; index < lanes.Count; index++)
        {
            if (index < target.Count)
            {
                target[index].Update(lanes[index], pixelsPerSecond, isPaused, rowHeight, fontSize);
            }
            else
            {
                target.Add(new TickerLane(lanes[index], pixelsPerSecond, isPaused, rowHeight, fontSize));
            }
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        Interlocked.Increment(ref _refreshGeneration);
        SaveSettings();
        ExceptionSafety.Run(_blinkTimer.Stop);
        ExceptionSafety.Run(_configReloadTimer.Stop);
        ExceptionSafety.Run(() => _settingsWatcher?.Dispose());
        ExceptionSafety.Run(() => _alertsWatcher?.Dispose());
        ExceptionSafety.Run(_lifetimeCancellation.Cancel);
        ExceptionSafety.Run(() => (_selectorDiscovery as IDisposable)?.Dispose());
        ExceptionSafety.Run(() => (_newsSelectorDiscovery as IDisposable)?.Dispose());
        ExceptionSafety.Run(() => (_quoteFetcher as IDisposable)?.Dispose());
        ExceptionSafety.Run(() => (_newsFetcher as IDisposable)?.Dispose());
    }

    private void RunSafely(string operation, Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            ReportRecoverableError(operation, exception);
        }
    }

    private async Task RunSafelyAsync(string operation, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            ReportRecoverableError(operation, exception);
        }
    }

    private void ClearEntryForm()
    {
        EditingSubscription = null;
        NewSymbol = string.Empty;
        NewGroupName = string.Empty;
        SelectedSource = SourceAlternatives[0];
        NewSourceUrlSuffix = string.Empty;
        NewCssSelector = string.Empty;
        NewPreMarketCssSelector = string.Empty;
        NewPreMarketChangeCssSelector = string.Empty;
        NewNewsCssSelector = string.Empty;
        NewExtendedCssSelector = string.Empty;
        NewExtendedChangeCssSelector = string.Empty;
        NewChangeCssSelector = string.Empty;
        NewNewsRepeatLimit = TickerSubscription.DefaultNewsRepeatLimit;
        NewCollectPrice = true;
        NewCollectNews = false;
        SelectorSuggestions.Clear();
        NewsSelectorSuggestions.Clear();
    }

    private void RebuildSourceUrl()
    {
        NewSourceUrl = SelectedSource?.ComposeUrl(NewSourceUrlSuffix) ?? NewSourceUrlSuffix.Trim();
    }

    private void LoadSettings()
    {
        if (_settingsStore is null)
        {
            UpdateVisibleRows();
            return;
        }

        try
        {
            ApplySettings(_settingsStore.Load().UpgradeDefaults());
            EntryMessage = Subscriptions.Count == 0
                ? "Enter every ticker and source URL manually. Duplicate symbols are allowed."
                : $"Loaded {Subscriptions.Count} configured entr{(Subscriptions.Count == 1 ? "y" : "ies")} from local settings.";
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            _settingsPersistenceBlocked = true;
            EntryMessage = $"Settings could not be loaded: {exception.Message}";
        }

        UpdateTickerLines();
        RaiseAcknowledgementChanged();
    }

    private void ApplySettings(SmartTickerSettings settings)
    {
        try
        {
            _isApplyingSettings = true;
            _acknowledgements = new SourceAcknowledgementLedger(settings.AcknowledgedSources);
            SyncApprovedSourceHosts();
            _quoteGroupNames.Clear();
            _quoteGroupNames.AddRange(settings.QuoteGroupNames);
            _hiddenNewsQuotes.Clear();
            _hiddenNewsQuotes.UnionWith(settings.HiddenNewsQuotes);
            Subscriptions.Clear();
            foreach (var subscription in settings.Subscriptions)
            {
                var groupName = subscription.GroupName is null
                    ? null
                    : ResolveExistingGroupName(subscription.GroupName);
                Subscriptions.Add(subscription with { GroupName = groupName });
            }

            PriceRowCount = settings.PriceRowCount;
            NewsRowCount = settings.NewsRowCount;
            PriceScrollSpeed = settings.PriceScrollSpeed;
            NewsScrollSpeed = settings.NewsScrollSpeed;
            ScrollingViewFontSize = settings.ScrollingViewFontSize;
            StaticViewFontSize = settings.StaticViewFontSize;
            ScrollingWindowWidth = settings.ScrollingWindowSize.Width;
            ScrollingWindowHeight = settings.ScrollingWindowSize.Height;
            StaticPricesWindowWidth = settings.StaticPricesWindowSize.Width;
            StaticPricesWindowHeight = settings.StaticPricesWindowSize.Height;
            StaticNewsWindowWidth = settings.StaticNewsWindowSize.Width;
            StaticNewsWindowHeight = settings.StaticNewsWindowSize.Height;
            // Every selectable View mode includes prices; migrate the retired hidden-price state.
            ShowPriceLine = true;
            ShowNewsLine = settings.ShowNewsLine;
            UseStaticGroupedView = settings.UseStaticGroupedView;
            // The OS wins: the user may have switched autostart off outside the app.
            LaunchAtLogin = _startupRegistration?.IsEnabled ?? settings.LaunchAtLogin;
            AllowWebsiteCookiesAndCrossHostRedirects = settings.AllowWebsiteCookiesAndCrossHostRedirects;
            BackgroundColorHex = settings.BackgroundColor;
            BackgroundOpacity = settings.BackgroundOpacity;
            SymbolColorHex = settings.SymbolColor;
            ExtendedPriceColorHex = settings.ExtendedPriceColor;
            PriceColorHex = settings.PriceColor;
            NewsColorHex = settings.NewsColor;
            NewsColor2Hex = settings.NewsColor2;
            NewsColor3Hex = settings.NewsColor3;
            NewsColor4Hex = settings.NewsColor4;
            PriceUpColorHex = settings.PriceUpColor;
            PriceDownColorHex = settings.PriceDownColor;
            AlertBlinkColorHex = settings.AlertBlinkColor;
            PriceRefreshSeconds = settings.PriceRefreshSeconds;
            NewsRefreshSeconds = settings.NewsRefreshSeconds;
            Language = settings.Language;
            ApplyConfiguredMainWindowSize();
        }
        finally
        {
            _isApplyingSettings = false;
        }
    }

    private SmartTickerSettings CurrentSettings() => new(
        SmartTickerSettings.CurrentVersion,
        Subscriptions.ToArray(),
        PriceRowCount,
        NewsRowCount,
        PriceScrollSpeed,
        NewsScrollSpeed)
    {
        AcknowledgedSources = _acknowledgements.ToArray(),
        QuoteGroupNames = _quoteGroupNames.ToArray(),
        HiddenNewsQuotes = _hiddenNewsQuotes.ToArray(),
        ShowPriceLine = ShowPriceLine,
        ShowNewsLine = ShowNewsLine,
        UseStaticGroupedView = UseStaticGroupedView,
        LaunchAtLogin = LaunchAtLogin,
        AllowWebsiteCookiesAndCrossHostRedirects = AllowWebsiteCookiesAndCrossHostRedirects,
        BackgroundColor = BackgroundColorHex,
        BackgroundOpacity = BackgroundOpacity,
        SymbolColor = SymbolColorHex,
        ExtendedPriceColor = ExtendedPriceColorHex,
        PriceColor = PriceColorHex,
        NewsColor = NewsColorHex,
        NewsColor2 = NewsColor2Hex,
        NewsColor3 = NewsColor3Hex,
        NewsColor4 = NewsColor4Hex,
        PriceUpColor = PriceUpColorHex,
        PriceDownColor = PriceDownColorHex,
        AlertBlinkColor = AlertBlinkColorHex,
        PriceRefreshSeconds = PriceRefreshSeconds,
        NewsRefreshSeconds = NewsRefreshSeconds,
        ScrollingViewFontSize = ScrollingViewFontSize,
        StaticViewFontSize = StaticViewFontSize,
        ScrollingWindowSize = new WindowSizeSettings(ScrollingWindowWidth, ScrollingWindowHeight),
        StaticPricesWindowSize = new WindowSizeSettings(StaticPricesWindowWidth, StaticPricesWindowHeight),
        StaticNewsWindowSize = new WindowSizeSettings(StaticNewsWindowWidth, StaticNewsWindowHeight),
        Language = Language,
    };

    public string ExportSettingsJson() => SettingsJson.Serialize(CurrentSettings());

    public string ExportAlertsJson() => AlertsJson.Serialize(new AlertSettings
    {
        Rules = [.. AlertRules],
        SoundEnabled = AlertSoundEnabled,
        BlinkSeconds = AlertBlinkSeconds,
        BuzzCount = AlertBuzzCount,
    });

    /// <summary>Validates untrusted JSON and only replaces the live alerts when every check passes.</summary>
    public AlertsImportResult ImportAlertsJson(string? json) => ImportAlertsJson(json, persist: true);

    private AlertsImportResult ImportAlertsJson(string? json, bool persist)
    {
        var result = AlertsImportValidator.Validate(json);
        if (!result.Success)
        {
            return result;
        }

        var imported = result.Settings!;

        // Subscription ids differ between machines, so rules are re-attached by symbol where possible.
        var relinked = 0;
        var orphaned = 0;
        var rules = new List<AlertRule>(imported.Rules.Length);
        foreach (var rule in imported.Rules)
        {
            var match = Subscriptions.FirstOrDefault(item => item.Id == rule.SubscriptionId)
                ?? Subscriptions.FirstOrDefault(item =>
                    string.Equals(item.Symbol, rule.Symbol, StringComparison.OrdinalIgnoreCase));

            if (match is null)
            {
                orphaned++;
                rules.Add(rule);
                continue;
            }

            if (match.Id != rule.SubscriptionId)
            {
                relinked++;
            }

            rules.Add(rule with { SubscriptionId = match.Id, Symbol = match.Symbol });
        }

        try
        {
            _isApplyingAlerts = true;
            AlertRules.Clear();
            foreach (var rule in rules)
            {
                AlertRules.Add(rule);
            }

            AlertSoundEnabled = imported.SoundEnabled;
            AlertBlinkSeconds = imported.BlinkSeconds;
            AlertBuzzCount = imported.BuzzCount;
        }
        finally
        {
            _isApplyingAlerts = false;
        }

        _arming.Clear();
        ClearAlertForm();
        if (persist)
        {
            SaveAlerts();
        }

        var notes = new List<string> { $"Imported {rules.Count} alert rule{(rules.Count == 1 ? string.Empty : "s")}" };
        if (relinked > 0)
        {
            notes.Add($"{relinked} re-linked by symbol");
        }

        if (orphaned > 0)
        {
            notes.Add($"{orphaned} match no configured quote and will not fire");
        }

        AlertMessage = string.Join(", ", notes) + ".";
        EntryMessage = AlertMessage;
        return result;
    }

    /// <summary>Validates untrusted JSON and only replaces the live settings when every check passes.</summary>
    public SettingsImportResult ImportSettingsJson(string? json)
    {
        var result = SettingsImportValidator.Validate(json);
        if (!result.Success)
        {
            return result;
        }

        ApplySettings(result.Settings!);
        _settingsPersistenceBlocked = false;
        SaveSettings();
        UpdateTickerLines();
        RaiseAcknowledgementChanged();
        EntryMessage = $"Imported {Subscriptions.Count} entr{(Subscriptions.Count == 1 ? "y" : "ies")} and applied the saved appearance.";
        return result;
    }

    private void SaveSettings()
    {
        if (_settingsStore is null || _isApplyingSettings || _settingsPersistenceBlocked)
        {
            return;
        }

        try
        {
            _settingsStore.Save(CurrentSettings());
            _lastSelfWrite = DateTimeOffset.Now;
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            EntryMessage = $"Settings could not be saved: {exception.Message}";
        }
    }

    private static string FormatCurrency(string? currency) =>
        string.IsNullOrWhiteSpace(currency) ? string.Empty : $" {currency}";
}
