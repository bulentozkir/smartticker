namespace SmartTicker.Core.Models;

using System.Text.Json.Serialization;

public sealed record TickerSubscription(
    Guid Id,
    string Symbol,
    string SourceName,
    Uri SourceUri,
    bool CollectPrice,
    bool CollectNews,
    string? CssSelector = null,
    string? NewsCssSelector = null)
{
    public const int DefaultNewsRepeatLimit = 5;

    // Extended-hours markup differs per site, so the selectors are supplied rather than guessed.
    public string? ExtendedCssSelector { get; init; }

    public string? ExtendedChangeCssSelector { get; init; }

    public int NewsRepeatLimit { get; init; } = DefaultNewsRepeatLimit;

    public TickerSubscription WithNewsRepeatLimit(int limit) =>
        this with { NewsRepeatLimit = Math.Clamp(limit, 1, 100) };

    [JsonIgnore]
    public string PriceSelectorDisplay => string.IsNullOrWhiteSpace(CssSelector) ? "Automatic" : CssSelector;

    [JsonIgnore]
    public string NewsSelectorDisplay => string.IsNullOrWhiteSpace(NewsCssSelector) ? "Automatic" : NewsCssSelector;

    public static bool TryCreate(
        string symbol,
        string sourceName,
        string sourceUrl,
        bool collectPrice,
        bool collectNews,
        string? cssSelector,
        string? newsCssSelector,
        out TickerSubscription? subscription,
        out string? error)
    {
        subscription = null;
        error = null;

        if (!collectPrice && !collectNews)
        {
            error = "Select price, news, or both.";
            return false;
        }

        if (!TickerSource.TryCreate(symbol, symbol, sourceUrl, cssSelector, null, out var source, out error))
        {
            return false;
        }

        subscription = new TickerSubscription(
            Guid.NewGuid(),
            source!.Symbol,
            string.IsNullOrWhiteSpace(sourceName) ? source.PageUri.Host : sourceName.Trim(),
            source.PageUri,
            collectPrice,
            collectNews,
            source.CssSelector,
            collectNews && !string.IsNullOrWhiteSpace(newsCssSelector) ? newsCssSelector.Trim() : null);
        return true;
    }

    public static bool TryCreate(
        string symbol,
        string sourceName,
        string sourceUrl,
        bool collectPrice,
        bool collectNews,
        string? cssSelector,
        out TickerSubscription? subscription,
        out string? error) =>
        TryCreate(
            symbol,
            sourceName,
            sourceUrl,
            collectPrice,
            collectNews,
            cssSelector,
            null,
            out subscription,
            out error);

    public static bool TryUpdate(
        TickerSubscription original,
        string symbol,
        string sourceName,
        string sourceUrl,
        bool collectPrice,
        bool collectNews,
        string? cssSelector,
        string? newsCssSelector,
        out TickerSubscription? subscription,
        out string? error)
    {
        if (!TryCreate(
                symbol,
                sourceName,
                sourceUrl,
                collectPrice,
                collectNews,
                cssSelector,
                newsCssSelector,
                out var candidate,
                out error))
        {
            subscription = null;
            return false;
        }

        subscription = candidate! with { Id = original.Id };
        return true;
    }
}