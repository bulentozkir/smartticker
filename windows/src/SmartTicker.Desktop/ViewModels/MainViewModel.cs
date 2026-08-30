using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;
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

    [ObservableProperty]
    public partial string QuoteLine { get; set; } = "PRICES  •  Add an authorized webpage or feed in Settings  •  Refresh: 1 min";

    [ObservableProperty]
    public partial string NewsLine { get; set; } = "NEWS  •  Add RSS/Atom feeds  •  Refresh: 5 min  •  Delayed data — not investment advice";

    [ObservableProperty]
    public partial bool IsPaused { get; set; }

    [ObservableProperty]
    public partial string NewSymbol { get; set; } = string.Empty;

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
        catch (Exception exception) when (exception is HttpRequestException or InvalidOperationException or TaskCanceledException)
        {
            ReportImportFailure(
                "the starter quotes from GitHub",
                [exception is TaskCanceledException ? "The download timed out." : exception.Message]);
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

    public IReadOnlyList<SourcePreset> SourceAlternatives => KnownSourceCatalog.All;

    public IReadOnlyList<int> RowCountOptions { get; } = Enumerable.Range(1, 8).ToArray();

    public IReadOnlyList<int> ScrollSpeedOptions { get; } = [20, 30, 40, 50, 65, 80, 100, 120];

    public ObservableCollection<TickerSubscription> Subscriptions { get; } = [];

    public ObservableCollection<TickerLane> VisiblePriceRows { get; } = [];

    public ObservableCollection<TickerLane> VisibleNewsRows { get; } = [];

    public ObservableCollection<CssSelectorSuggestion> SelectorSuggestions { get; } = [];

    public ObservableCollection<CssSelectorSuggestion> NewsSelectorSuggestions { get; } = [];

    public ObservableCollection<QuoteSnapshot> LatestQuotes { get; } = [];

    public ObservableCollection<NewsSnapshot> LatestNews { get; } = [];

    [ObservableProperty]
    public partial double WindowHeight { get; set; } = TickerLayoutCalculator.NaturalHeight(1, 1);

    [ObservableProperty]
    public partial bool ShowPriceLine { get; set; } = true;

    [ObservableProperty]
    public partial bool ShowNewsLine { get; set; } = true;

    [ObservableProperty]
    public partial string BackgroundColorHex { get; set; } = SmartTickerSettings.DefaultBackgroundColor;

    [ObservableProperty]
    public partial double BackgroundOpacity { get; set; } = SmartTickerSettings.DefaultOpacity;

    // The suggestion list is shared, so only the row that started the discovery renders it.
    [ObservableProperty]
    public partial SelectorKind DiscoveryTarget { get; set; } = SelectorKind.Price;

    public bool ShowPriceMatches => DiscoveryTarget == SelectorKind.Price;

    public bool ShowChangeMatches => DiscoveryTarget == SelectorKind.Change;

    public bool ShowExtendedMatches => DiscoveryTarget == SelectorKind.ExtendedPrice;

    public bool ShowExtendedChangeMatches => DiscoveryTarget == SelectorKind.ExtendedChange;

    partial void OnDiscoveryTargetChanged(SelectorKind value)
    {
        OnPropertyChanged(nameof(ShowPriceMatches));
        OnPropertyChanged(nameof(ShowChangeMatches));
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
    public partial int PriceRefreshSeconds { get; set; } = SmartTickerSettings.DefaultPriceRefreshSeconds;

    [ObservableProperty]
    public partial int NewsRefreshSeconds { get; set; } = SmartTickerSettings.DefaultNewsRefreshSeconds;

    partial void OnPriceRefreshSecondsChanged(int value) => SaveSettings();

    partial void OnNewsRefreshSecondsChanged(int value) => SaveSettings();

    private TickerLayout Layout =>
        TickerLayoutCalculator.Calculate(WindowHeight, PriceRowCount, NewsRowCount, ShowPriceLine, ShowNewsLine);

    public bool IsPriceVisible => ShowPriceLine;

    public bool IsNewsVisible => ShowNewsLine && Layout.ShowNews;

    // Alpha on the background keeps the desktop visible through the bar while the text stays crisp.
    public IBrush BackgroundBrush
    {
        get
        {
            var color = Color.Parse(HexColor.TryNormalize(BackgroundColorHex, out var normalized)
                ? normalized
                : SmartTickerSettings.DefaultBackgroundColor);
            var alpha = (byte)Math.Round(Math.Clamp(BackgroundOpacity, SmartTickerSettings.MinimumOpacity, SmartTickerSettings.MaximumOpacity) * 255);
            return new SolidColorBrush(Color.FromArgb(alpha, color.R, color.G, color.B));
        }
    }

    public string BackgroundOpacityText => $"{BackgroundOpacity * 100:0}%";

    public IBrush SymbolBrush => ToBrush(SymbolColorHex, SmartTickerSettings.DefaultSymbolColor);

    public IBrush ExtendedPriceBrush => ToBrush(ExtendedPriceColorHex, SmartTickerSettings.DefaultExtendedPriceColor);

    public IBrush PriceBrush => ToBrush(PriceColorHex, SmartTickerSettings.DefaultPriceColor);

    public IBrush NewsBrush => ToBrush(NewsColorHex, SmartTickerSettings.DefaultNewsColor);

    public IBrush NewsAlternateBrush => ToBrush(NewsColor2Hex, SmartTickerSettings.DefaultNewsColor2);

    public IBrush NewsBrush3 => ToBrush(NewsColor3Hex, SmartTickerSettings.DefaultNewsColor3);

    public IBrush NewsBrush4 => ToBrush(NewsColor4Hex, SmartTickerSettings.DefaultNewsColor4);

    private IReadOnlyList<IBrush> NewsBrushCycle => [NewsBrush, NewsAlternateBrush, NewsBrush3, NewsBrush4];

    public IBrush PriceUpBrush => ToBrush(PriceUpColorHex, SmartTickerSettings.DefaultPriceUpColor);

    public IBrush PriceDownBrush => ToBrush(PriceDownColorHex, SmartTickerSettings.DefaultPriceDownColor);

    private static IBrush ToBrush(string hex, string fallback) =>
        new SolidColorBrush(Color.Parse(HexColor.TryNormalize(hex, out var normalized) ? normalized : fallback));

    private readonly IPriceSelectorDiscovery? _selectorDiscovery;
    private readonly INewsSelectorDiscovery? _newsSelectorDiscovery;
    private readonly IQuoteFetcher? _quoteFetcher;
    private readonly INewsFetcher? _newsFetcher;
    private readonly ISettingsStore? _settingsStore;
    private readonly ILinkLauncher? _linkLauncher;
    private readonly IStarterSettingsSource? _starterSettings;
    private readonly SemaphoreSlim _priceRefreshGate = new(1, 1);
    private readonly SemaphoreSlim _newsRefreshGate = new(1, 1);
    private readonly NewsRepeatFilter _newsRepeatFilter = new();
    private SourceAcknowledgementLedger _acknowledgements = new();
    private readonly CancellationTokenSource _lifetimeCancellation = new();
    private bool _isApplyingSettings;

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
        IStarterSettingsSource? starterSettings = null)
    {
        _starterSettings = starterSettings;
        _selectorDiscovery = selectorDiscovery;
        _quoteFetcher = quoteFetcher;
        _newsSelectorDiscovery = newsSelectorDiscovery;
        _settingsStore = settingsStore;
        _newsFetcher = newsFetcher;
        _linkLauncher = linkLauncher;
        SelectedSource = SourceAlternatives[0];
        LoadSettings();
    }

    public string SourceSummary => string.Join("  •  ", SourceAlternatives.Select(source => source.Name));

    public string SelectedSourcePrefix => SelectedSource?.UrlPrefix ?? string.Empty;

    public string SelectedSourcePolicy => SelectedSource?.PolicySummary ?? string.Empty;

    public string SelectedSourceGuidance => SelectedSource?.Guidance ?? string.Empty;

    public string SelectedSourcePolicyColor =>
        SelectedSource?.CollectionPolicy == CollectionPolicy.RequiresWrittenPermission ? "#D9822B" : "#70E1A1";

    public string? CurrentSourceHost => SourceAcknowledgementLedger.HostOf(NewSourceUrl);

    public bool RequiresAcknowledgement =>
        CurrentSourceHost is not null && !_acknowledgements.IsAcknowledged(NewSourceUrl);

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
    private void AcknowledgeSource()
    {
        if (_acknowledgements.Acknowledge(NewSourceUrl))
        {
            EntryMessage = $"Access to {CurrentSourceHost} confirmed. This is remembered for that site.";
            RaiseAcknowledgementChanged();
            SaveSettings();
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
    private async Task AddSubscriptionAsync()
    {
        if (RequiresAcknowledgement)
        {
            EntryMessage = $"Confirm your access rights for {CurrentSourceHost} before adding this entry.";
            return;
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
            ExtendedCssSelector = string.IsNullOrWhiteSpace(NewExtendedCssSelector) ? null : NewExtendedCssSelector.Trim(),
            ExtendedChangeCssSelector = string.IsNullOrWhiteSpace(NewExtendedChangeCssSelector)
                ? null
                : NewExtendedChangeCssSelector.Trim(),
            ChangeCssSelector = string.IsNullOrWhiteSpace(NewChangeCssSelector) ? null : NewChangeCssSelector.Trim(),
        };

        if (EditingSubscription is { } editing)
        {
            var index = Subscriptions.IndexOf(editing);
            Subscriptions[index] = subscription!.WithNewsRepeatLimit(NewNewsRepeatLimit);
            _newsRepeatFilter.Forget(editing.Id);
            EntryMessage = $"Updated {subscription!.Symbol} from {subscription.SourceName}.";
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
    private void RemoveSubscription(TickerSubscription? subscription)
    {
        if (subscription is not null)
        {
            Subscriptions.Remove(subscription);
            var quote = LatestQuotes.FirstOrDefault(item => item.SubscriptionId == subscription.Id);
            if (quote is not null)
            {
                LatestQuotes.Remove(quote);
            }

            var news = LatestNews.FirstOrDefault(item => item.SubscriptionId == subscription.Id);
            if (news is not null)
            {
                LatestNews.Remove(news);
            }

            _newsRepeatFilter.Forget(subscription.Id);

            UpdateTickerLines();
            SaveSettings();
        }
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

    [RelayCommand]
    public async Task RefreshPricesAsync()
    {
        if (_quoteFetcher is null || IsPaused ||
            !await _priceRefreshGate.WaitAsync(0, _lifetimeCancellation.Token))
        {
            return;
        }

        try
        {
            var priceSubscriptions = Subscriptions.Where(item => item.CollectPrice).ToArray();
            foreach (var subscription in priceSubscriptions)
            {
                var snapshot = await _quoteFetcher.FetchAsync(subscription, _lifetimeCancellation.Token);
                var previous = LatestQuotes.FirstOrDefault(item => item.SubscriptionId == subscription.Id);
                if (previous is not null)
                {
                    LatestQuotes.Remove(previous);
                }

                LatestQuotes.Add(snapshot);
            }

            UpdatePriceRows();
        }
        catch (OperationCanceledException) when (_lifetimeCancellation.IsCancellationRequested)
        {
        }
        finally
        {
            _priceRefreshGate.Release();
        }
    }

    [RelayCommand]
    public async Task RefreshNewsAsync()
    {
        if (_newsFetcher is null || IsPaused ||
            !await _newsRefreshGate.WaitAsync(0, _lifetimeCancellation.Token))
        {
            return;
        }

        try
        {
            var newsSubscriptions = Subscriptions.Where(item => item.CollectNews).ToArray();
            foreach (var subscription in newsSubscriptions)
            {
                var snapshot = await _newsFetcher.FetchAsync(subscription, _lifetimeCancellation.Token);
                if (snapshot.Success)
                {
                    snapshot = snapshot with
                    {
                        Headlines = _newsRepeatFilter.Filter(
                            subscription.Id,
                            snapshot.Headlines,
                            subscription.NewsRepeatLimit),
                    };
                }

                var previous = LatestNews.FirstOrDefault(item => item.SubscriptionId == subscription.Id);
                if (previous is not null)
                {
                    LatestNews.Remove(previous);
                }

                LatestNews.Add(snapshot);
            }

            UpdateNewsRows();
            UpdatePriceRows();
        }
        catch (OperationCanceledException) when (_lifetimeCancellation.IsCancellationRequested)
        {
        }
        finally
        {
            _newsRefreshGate.Release();
        }
    }

    [RelayCommand]
    private Task DiscoverSelectorsAsync() => DiscoverForAsync(SelectorKind.Price);

    [RelayCommand]
    private Task DiscoverChangeSelectorsAsync() => DiscoverForAsync(SelectorKind.Change);

    [RelayCommand]
    private Task DiscoverExtendedSelectorsAsync() => DiscoverForAsync(SelectorKind.ExtendedPrice);

    [RelayCommand]
    private Task DiscoverExtendedChangeSelectorsAsync() => DiscoverForAsync(SelectorKind.ExtendedChange);

    private static string DescribeKind(SelectorKind kind) => kind switch
    {
        SelectorKind.Change => "price change",
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
        catch (Exception exception) when (exception is HttpRequestException or InvalidOperationException or TaskCanceledException)
        {
            DiscoveryMessage = exception is TaskCanceledException
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
        catch (Exception exception) when (exception is HttpRequestException or InvalidOperationException or TaskCanceledException)
        {
            NewsDiscoveryMessage = exception is TaskCanceledException ? "News selector discovery timed out." : exception.Message;
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
        catch (Exception exception) when (exception is HttpRequestException or InvalidOperationException or TaskCanceledException)
        {
            ValidationMessage = exception is TaskCanceledException
                ? "The page did not respond in time."
                : exception.Message;
        }
        finally
        {
            IsValidating = false;
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
        OnPropertyChanged(nameof(StatusGlyph));
        OnPropertyChanged(nameof(StatusColor));
        OnPropertyChanged(nameof(StatusText));
        UpdateVisibleRows();
    }

    private void UpdateTickerLines()
    {
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

        WindowHeight = TickerLayoutCalculator.NaturalHeight(value, NewsRowCount, ShowPriceLine, ShowNewsLine);
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

        WindowHeight = TickerLayoutCalculator.NaturalHeight(PriceRowCount, value, ShowPriceLine, ShowNewsLine);
        UpdateVisibleRows();
        SaveSettings();
    }

    partial void OnWindowHeightChanged(double value)
    {
        OnPropertyChanged(nameof(IsNewsVisible));
        UpdateVisibleRows();
    }

    partial void OnShowPriceLineChanged(bool value)
    {
        WindowHeight = TickerLayoutCalculator.NaturalHeight(PriceRowCount, NewsRowCount, value, ShowNewsLine);
        OnPropertyChanged(nameof(IsPriceVisible));
        OnPropertyChanged(nameof(IsNewsVisible));
        UpdateVisibleRows();
        SaveSettings();
    }

    partial void OnShowNewsLineChanged(bool value)
    {
        WindowHeight = TickerLayoutCalculator.NaturalHeight(PriceRowCount, NewsRowCount, ShowPriceLine, value);
        OnPropertyChanged(nameof(IsNewsVisible));
        UpdateVisibleRows();
        SaveSettings();
    }

    partial void OnBackgroundColorHexChanged(string value) => ApplyColorChange(nameof(BackgroundBrush));

    partial void OnBackgroundOpacityChanged(double value)
    {
        OnPropertyChanged(nameof(BackgroundOpacityText));
        ApplyColorChange(nameof(BackgroundBrush));
    }

    // News brushes are baked into segments, so the rows must be rebuilt.
    partial void OnNewsColorHexChanged(string value)
    {
        OnPropertyChanged(nameof(NewsBrush));
        UpdateNewsRows();
        SaveSettings();
    }

    partial void OnNewsColor2HexChanged(string value)
    {
        OnPropertyChanged(nameof(NewsAlternateBrush));
        UpdateNewsRows();
        SaveSettings();
    }

    partial void OnNewsColor3HexChanged(string value)
    {
        OnPropertyChanged(nameof(NewsBrush3));
        UpdateNewsRows();
        SaveSettings();
    }

    partial void OnNewsColor4HexChanged(string value)
    {
        OnPropertyChanged(nameof(NewsBrush4));
        UpdateNewsRows();
        SaveSettings();
    }

    // Price row brushes are baked into segments, so the rows must be rebuilt.
    partial void OnSymbolColorHexChanged(string value)
    {
        OnPropertyChanged(nameof(SymbolBrush));
        UpdatePriceRows();
        SaveSettings();
    }

    partial void OnPriceColorHexChanged(string value)
    {
        OnPropertyChanged(nameof(PriceBrush));
        UpdatePriceRows();
        SaveSettings();
    }

    partial void OnExtendedPriceColorHexChanged(string value)
    {
        OnPropertyChanged(nameof(ExtendedPriceBrush));
        UpdatePriceRows();
        SaveSettings();
    }

    partial void OnPriceUpColorHexChanged(string value)
    {
        OnPropertyChanged(nameof(PriceUpBrush));
        UpdatePriceRows();
        SaveSettings();
    }

    partial void OnPriceDownColorHexChanged(string value)
    {
        OnPropertyChanged(nameof(PriceDownBrush));
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
            VisiblePriceRows.Clear();
            return;
        }

        var rows = Subscriptions
            .Where(item => item.CollectPrice)
            .Select(item =>
            {
                var quote = LatestQuotes.FirstOrDefault(snapshot => snapshot.SubscriptionId == item.Id);
                var marker = HasNoNews(item) ? NoNewsMarker : string.Empty;
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

                return new TickerSegment(runs, item.SourceUri);
            })
            .ToArray();
        ReplaceVisibleRows(
            VisiblePriceRows,
            rows,
            PriceRowCount,
            PriceScrollSpeed,
            IsPaused,
            layout.RowHeight,
            layout.PriceFontSize,
            "Add an authorized webpage in Settings");
    }

    private void UpdateNewsRows()
    {
        var layout = Layout;
        if (!layout.ShowNews)
        {
            VisibleNewsRows.Clear();
            return;
        }

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
            layout.NewsFontSize,
            "Add a news source in Settings");
    }

    // A headline-less entry is marked in the price row rather than shown as a news error.
    private bool HasNoNews(TickerSubscription item)
    {
        if (!item.CollectNews)
        {
            return true;
        }

        var news = LatestNews.FirstOrDefault(snapshot => snapshot.SubscriptionId == item.Id);
        return news is not null && (!news.Success || news.Headlines.Count == 0);
    }

    private static TickerSegment Tint(TickerSegment segment, IBrush brush) =>
        new(segment.Runs.Select(run => run with { Brush = brush }).ToArray(), segment.Link);

    private IEnumerable<TickerSegment> BuildNewsSegments(TickerSubscription item)
    {
        var news = LatestNews.FirstOrDefault(snapshot => snapshot.SubscriptionId == item.Id);
        if (news is not { Success: true })
        {
            return [];
        }

        return news.Headlines.Select(headline => new TickerSegment(
            $"{item.Symbol} — {headline.Title}",
            headline.Url ?? item.SourceUri));
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
        target.Clear();
        if (source.Count == 0)
        {
            target.Add(new TickerLane(
                [new TickerSegment(emptyMessage, null)], pixelsPerSecond, isPaused, rowHeight, fontSize));
            return;
        }

        var count = Math.Min(Math.Clamp(rowCount, 1, 8), source.Count);
        var rows = Enumerable.Range(0, count).Select(_ => new List<TickerSegment>()).ToArray();
        for (var index = 0; index < source.Count; index++)
        {
            rows[index % count].Add(source[index]);
        }

        foreach (var row in rows)
        {
            target.Add(new TickerLane(row, pixelsPerSecond, isPaused, rowHeight, fontSize));
        }
    }

    public void Dispose()
    {
        SaveSettings();
        _lifetimeCancellation.Cancel();
        _lifetimeCancellation.Dispose();
        _priceRefreshGate.Dispose();
        _newsRefreshGate.Dispose();
        (_selectorDiscovery as IDisposable)?.Dispose();
        (_newsSelectorDiscovery as IDisposable)?.Dispose();
        (_quoteFetcher as IDisposable)?.Dispose();
        (_newsFetcher as IDisposable)?.Dispose();
    }

    private void ClearEntryForm()
    {
        EditingSubscription = null;
        NewSymbol = string.Empty;
        SelectedSource = SourceAlternatives[0];
        NewSourceUrlSuffix = string.Empty;
        NewCssSelector = string.Empty;
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
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
        {
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
            Subscriptions.Clear();
            foreach (var subscription in settings.Subscriptions)
            {
                Subscriptions.Add(subscription);
            }

            PriceRowCount = settings.PriceRowCount;
            NewsRowCount = settings.NewsRowCount;
            PriceScrollSpeed = settings.PriceScrollSpeed;
            NewsScrollSpeed = settings.NewsScrollSpeed;
            ShowPriceLine = settings.ShowPriceLine;
            ShowNewsLine = settings.ShowNewsLine;
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
            PriceRefreshSeconds = settings.PriceRefreshSeconds;
            NewsRefreshSeconds = settings.NewsRefreshSeconds;
            Language = settings.Language;
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
        ShowPriceLine = ShowPriceLine,
        ShowNewsLine = ShowNewsLine,
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
        PriceRefreshSeconds = PriceRefreshSeconds,
        NewsRefreshSeconds = NewsRefreshSeconds,
        Language = Language,
    };

    public string ExportSettingsJson() => SettingsJson.Serialize(CurrentSettings());

    /// <summary>Validates untrusted JSON and only replaces the live settings when every check passes.</summary>
    public SettingsImportResult ImportSettingsJson(string? json)
    {
        var result = SettingsImportValidator.Validate(json);
        if (!result.Success)
        {
            return result;
        }

        ApplySettings(result.Settings!);
        SaveSettings();
        UpdateTickerLines();
        RaiseAcknowledgementChanged();
        EntryMessage = $"Imported {Subscriptions.Count} entr{(Subscriptions.Count == 1 ? "y" : "ies")} and applied the saved appearance.";
        return result;
    }

    private void SaveSettings()
    {
        if (_settingsStore is null || _isApplyingSettings)
        {
            return;
        }

        try
        {
            _settingsStore.Save(CurrentSettings());
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            EntryMessage = $"Settings could not be saved: {exception.Message}";
        }
    }

    private static string FormatCurrency(string? currency) =>
        string.IsNullOrWhiteSpace(currency) ? string.Empty : $" {currency}";
}
