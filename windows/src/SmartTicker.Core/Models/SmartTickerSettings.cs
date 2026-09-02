using System.Text.Json.Serialization;

namespace SmartTicker.Core.Models;

public sealed record WindowSizeSettings(int Width, int Height);

public sealed record SmartTickerSettings(
    int Version,
    TickerSubscription[] Subscriptions,
    int PriceRowCount,
    int NewsRowCount,
    int PriceScrollSpeed,
    int NewsScrollSpeed)
{
    public const int CurrentVersion = 1;

    public const string DefaultBackgroundColor = "#10151D";
    public const string DefaultSymbolColor = "#79C0FF";
    public const string DefaultPriceColor = "#FFA657";
    public const string DefaultExtendedPriceColor = "#00E5FF";

    // Defaults from earlier builds; an unchanged value is upgraded on load rather than left looking stale.
    public static IReadOnlyList<string> RetiredPriceColors { get; } = ["#70E1A1", "#79C0FF", "#00E5FF"];
    public const string DefaultNewsColor = "#FFFFFF";
    public const string DefaultNewsColor2 = "#00E5FF";
    public const string DefaultNewsColor3 = "#A3E635";
    public const string DefaultNewsColor4 = "#79C0FF";

    // The single off-white news colour predates the alternating pair.
    public static IReadOnlyList<string> RetiredNewsColors { get; } = ["#D8DEE9"];
    public const string DefaultPriceUpColor = "#3FB950";
    public const string DefaultPriceDownColor = "#F85149";
    public const string DefaultAlertBlinkColor = "#FF00FF";

    public const int MinimumRefreshSeconds = 30;
    public const int MaximumRefreshSeconds = 300;
    public const int DefaultPriceRefreshSeconds = 60;
    public const int DefaultNewsRefreshSeconds = 300;

    public const int MinimumViewFontSize = 9;
    public const int MaximumViewFontSize = 24;
    public const int DefaultScrollingViewFontSize = 14;
    public const int DefaultStaticViewFontSize = 13;

    public const int MinimumWindowWidth = 420;
    public const int MaximumWindowWidth = 7680;
    public const int MinimumScrollingWindowHeight = 50;
    public const int MaximumScrollingWindowHeight = 900;
    public const int MinimumStaticPricesWindowHeight = 420;
    public const int MinimumStaticNewsWindowHeight = 240;
    public const int MaximumStaticWindowHeight = 4320;

    public static WindowSizeSettings DefaultScrollingWindowSize { get; } = new(980, 64);
    public static WindowSizeSettings DefaultStaticPricesWindowSize { get; } = new(980, 420);
    public static WindowSizeSettings DefaultStaticNewsWindowSize { get; } = new(680, 340);

    // Below this the ticker stops being legible against a busy desktop.
    public const double MinimumOpacity = 0.2;
    public const double MaximumOpacity = 1.0;
    public const double DefaultOpacity = 1.0;

    public double BackgroundOpacity { get; init; } = DefaultOpacity;

    public int PriceRefreshSeconds { get; init; } = DefaultPriceRefreshSeconds;

    public int NewsRefreshSeconds { get; init; } = DefaultNewsRefreshSeconds;

    public int ScrollingViewFontSize { get; init; } = DefaultScrollingViewFontSize;

    public int StaticViewFontSize { get; init; } = DefaultStaticViewFontSize;

    public WindowSizeSettings ScrollingWindowSize { get; init; } = DefaultScrollingWindowSize;

    public WindowSizeSettings StaticPricesWindowSize { get; init; } = DefaultStaticPricesWindowSize;

    public WindowSizeSettings StaticNewsWindowSize { get; init; } = DefaultStaticNewsWindowSize;

    public string[] AcknowledgedSources { get; init; } = [];

    [JsonPropertyName("quoteGroups")]
    public string[] QuoteGroupNames { get; init; } = [];

    [JsonPropertyName("hiddenNewsQuotes")]
    public Guid[] HiddenNewsQuotes { get; init; } = [];

    public bool ShowPriceLine { get; init; } = true;

    public bool ShowNewsLine { get; init; }

    public bool UseStaticGroupedView { get; init; }

    /// <summary>Mirrors the OS registration; the OS remains authoritative if the two disagree.</summary>
    public bool LaunchAtLogin { get; init; }

    public bool AllowWebsiteCookiesAndCrossHostRedirects { get; init; }

    public string BackgroundColor { get; init; } = DefaultBackgroundColor;

    public string SymbolColor { get; init; } = DefaultSymbolColor;

    public string ExtendedPriceColor { get; init; } = DefaultExtendedPriceColor;

    public string PriceColor { get; init; } = DefaultPriceColor;

    public string NewsColor { get; init; } = DefaultNewsColor;

    public string NewsColor2 { get; init; } = DefaultNewsColor2;

    public string NewsColor3 { get; init; } = DefaultNewsColor3;

    public string NewsColor4 { get; init; } = DefaultNewsColor4;

    public string PriceUpColor { get; init; } = DefaultPriceUpColor;

    public string PriceDownColor { get; init; } = DefaultPriceDownColor;

    public string AlertBlinkColor { get; init; } = DefaultAlertBlinkColor;

    public string Language { get; init; } = AppLanguages.Default;

    public static SmartTickerSettings Default => new(
        CurrentVersion,
        [],
        1,
        1,
        50,
        40);

    public SmartTickerSettings UpgradeDefaults()
    {
        var upgraded = this;
        if (RetiredPriceColors.Contains(upgraded.PriceColor, StringComparer.OrdinalIgnoreCase))
        {
            upgraded = upgraded with { PriceColor = DefaultPriceColor };
        }

        if (RetiredNewsColors.Contains(upgraded.NewsColor, StringComparer.OrdinalIgnoreCase))
        {
            upgraded = upgraded with { NewsColor = DefaultNewsColor };
        }

        return upgraded;
    }

    public SmartTickerSettings Normalize() => this with
    {
        Version = CurrentVersion,
        Subscriptions = Subscriptions ?? [],
        AcknowledgedSources = AcknowledgedSources ?? [],
        QuoteGroupNames = NormalizeQuoteGroupNames(QuoteGroupNames, Subscriptions),
        HiddenNewsQuotes = NormalizeHiddenNewsQuotes(HiddenNewsQuotes, Subscriptions),
        PriceRowCount = Math.Clamp(PriceRowCount, 1, 8),
        NewsRowCount = Math.Clamp(NewsRowCount, 1, 8),
        BackgroundOpacity = double.IsFinite(BackgroundOpacity)
            ? Math.Clamp(BackgroundOpacity, MinimumOpacity, MaximumOpacity)
            : DefaultOpacity,
        PriceScrollSpeed = Math.Clamp(PriceScrollSpeed, 10, 200),
        NewsScrollSpeed = Math.Clamp(NewsScrollSpeed, 10, 200),
        ScrollingViewFontSize = Math.Clamp(ScrollingViewFontSize, MinimumViewFontSize, MaximumViewFontSize),
        StaticViewFontSize = Math.Clamp(StaticViewFontSize, MinimumViewFontSize, MaximumViewFontSize),
        ScrollingWindowSize = NormalizeWindowSize(
            ScrollingWindowSize,
            DefaultScrollingWindowSize,
            MinimumScrollingWindowHeight,
            MaximumScrollingWindowHeight),
        StaticPricesWindowSize = NormalizeWindowSize(
            StaticPricesWindowSize,
            DefaultStaticPricesWindowSize,
            MinimumStaticPricesWindowHeight,
            MaximumStaticWindowHeight),
        StaticNewsWindowSize = NormalizeWindowSize(
            StaticNewsWindowSize,
            DefaultStaticNewsWindowSize,
            MinimumStaticNewsWindowHeight,
            MaximumStaticWindowHeight),
        PriceRefreshSeconds = Math.Clamp(PriceRefreshSeconds, MinimumRefreshSeconds, MaximumRefreshSeconds),
        NewsRefreshSeconds = Math.Clamp(NewsRefreshSeconds, MinimumRefreshSeconds, MaximumRefreshSeconds),
        Language = AppLanguages.Normalize(Language),
    };

    private static WindowSizeSettings NormalizeWindowSize(
        WindowSizeSettings? size,
        WindowSizeSettings fallback,
        int minimumHeight,
        int maximumHeight) => new(
            Math.Clamp(size?.Width ?? fallback.Width, MinimumWindowWidth, MaximumWindowWidth),
            Math.Clamp(size?.Height ?? fallback.Height, minimumHeight, maximumHeight));

    private static string[] NormalizeQuoteGroupNames(
        IEnumerable<string>? names,
        IEnumerable<TickerSubscription>? subscriptions)
    {
        var normalized = new List<string>();
        foreach (var name in (names ?? []).Concat((subscriptions ?? []).Select(item => item.GroupName)))
        {
            if (TickerSubscription.TryNormalizeGroupName(name, out var groupName, out _) &&
                groupName is not null &&
                !normalized.Contains(groupName, StringComparer.OrdinalIgnoreCase))
            {
                normalized.Add(groupName);
            }
        }

        return normalized.ToArray();
    }

    // A hidden quote is dropped once its entry is gone, so a deleted quote cannot linger in the file.
    private static Guid[] NormalizeHiddenNewsQuotes(
        IEnumerable<Guid>? hidden,
        IEnumerable<TickerSubscription>? subscriptions)
    {
        var known = (subscriptions ?? []).Select(item => item.Id).ToHashSet();
        return (hidden ?? []).Where(known.Contains).Distinct().ToArray();
    }
}