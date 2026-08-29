namespace SmartTicker.Core.Models;

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
    public const string DefaultNewsColor = "#D8DEE9";
    public const string DefaultPriceUpColor = "#3FB950";
    public const string DefaultPriceDownColor = "#F85149";

    public const int MinimumRefreshSeconds = 30;
    public const int MaximumRefreshSeconds = 300;
    public const int DefaultPriceRefreshSeconds = 60;
    public const int DefaultNewsRefreshSeconds = 300;

    public int PriceRefreshSeconds { get; init; } = DefaultPriceRefreshSeconds;

    public int NewsRefreshSeconds { get; init; } = DefaultNewsRefreshSeconds;

    public string[] AcknowledgedSources { get; init; } = [];

    public bool ShowPriceLine { get; init; } = true;

    public bool ShowNewsLine { get; init; } = true;

    public string BackgroundColor { get; init; } = DefaultBackgroundColor;

    public string SymbolColor { get; init; } = DefaultSymbolColor;

    public string ExtendedPriceColor { get; init; } = DefaultExtendedPriceColor;

    public string PriceColor { get; init; } = DefaultPriceColor;

    public string NewsColor { get; init; } = DefaultNewsColor;

    public string PriceUpColor { get; init; } = DefaultPriceUpColor;

    public string PriceDownColor { get; init; } = DefaultPriceDownColor;

    public string Language { get; init; } = AppLanguages.Default;

    public static SmartTickerSettings Default => new(
        CurrentVersion,
        [],
        1,
        1,
        50,
        40);

    public SmartTickerSettings UpgradeDefaults() =>
        RetiredPriceColors.Contains(PriceColor, StringComparer.OrdinalIgnoreCase)
            ? this with { PriceColor = DefaultPriceColor }
            : this;

    public SmartTickerSettings Normalize() => this with
    {
        Version = CurrentVersion,
        Subscriptions = Subscriptions ?? [],
        AcknowledgedSources = AcknowledgedSources ?? [],
        PriceRowCount = Math.Clamp(PriceRowCount, 1, 8),
        NewsRowCount = Math.Clamp(NewsRowCount, 1, 8),
        PriceScrollSpeed = Math.Clamp(PriceScrollSpeed, 10, 200),
        NewsScrollSpeed = Math.Clamp(NewsScrollSpeed, 10, 200),
        PriceRefreshSeconds = Math.Clamp(PriceRefreshSeconds, MinimumRefreshSeconds, MaximumRefreshSeconds),
        NewsRefreshSeconds = Math.Clamp(NewsRefreshSeconds, MinimumRefreshSeconds, MaximumRefreshSeconds),
        Language = AppLanguages.Normalize(Language),
    };
}