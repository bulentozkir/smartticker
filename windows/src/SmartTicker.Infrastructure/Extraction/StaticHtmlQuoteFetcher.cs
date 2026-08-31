using SmartTicker.Core.Models;
using SmartTicker.Core.Services;
using SmartTicker.Infrastructure.Networking;

namespace SmartTicker.Infrastructure.Extraction;

public sealed class StaticHtmlQuoteFetcher : IQuoteFetcher, IDisposable
{
    private readonly PublicHtmlClient _client;
    private readonly StaticHtmlPriceExtractor _extractor = new();
    private readonly StaticHtmlChangeExtractor _changeExtractor = new();

    public StaticHtmlQuoteFetcher(WebsiteAccessPolicy? accessPolicy = null)
    {
        _client = new PublicHtmlClient(accessPolicy);
    }

    public async Task<QuoteSnapshot> FetchAsync(
        TickerSubscription subscription,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var html = await _client.GetStringAsync(subscription.SourceUri, cancellationToken);
            return ExtractSnapshot(subscription, html, DateTimeOffset.UtcNow);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception) when (exception is HttpRequestException or InvalidOperationException)
        {
            return new QuoteSnapshot(
                subscription.Id,
                subscription.Symbol,
                subscription.SourceName,
                null,
                null,
                DateTimeOffset.UtcNow,
                false,
                exception.Message);
        }
    }

    public void Dispose() => _client.Dispose();

    internal QuoteSnapshot ExtractSnapshot(
        TickerSubscription subscription,
        string html,
        DateTimeOffset observedAt)
    {
        var source = new TickerSource(
            subscription.Symbol,
            subscription.Symbol,
            subscription.SourceUri,
            subscription.CssSelector);
        var extraction = _extractor.Extract(html, source);
        var preMarket = ExtractSession(
            html,
            subscription,
            subscription.PreMarketCssSelector,
            subscription.PreMarketChangeCssSelector);
        var extended = ExtractSession(
            html,
            subscription,
            subscription.ExtendedCssSelector,
            subscription.ExtendedChangeCssSelector);
        return new QuoteSnapshot(
            subscription.Id,
            subscription.Symbol,
            subscription.SourceName,
            extraction.Price,
            extraction.Currency,
            observedAt,
            extraction.Success,
            extraction.Message,
            extraction.Success ? ExtractChange(html, subscription) : null,
            extended.Price,
            extended.ChangePercent,
            preMarket.Price,
            preMarket.ChangePercent);
    }

    private decimal? ExtractChange(string html, TickerSubscription subscription) =>
        string.IsNullOrWhiteSpace(subscription.ChangeCssSelector)
            ? _changeExtractor.Extract(html)
            : _changeExtractor.Extract(html, subscription.ChangeCssSelector);

    private (decimal? Price, decimal? ChangePercent) ExtractSession(
        string html,
        TickerSubscription subscription,
        string? priceSelector,
        string? changeSelector)
    {
        if (string.IsNullOrWhiteSpace(priceSelector))
        {
            return (null, null);
        }

        var source = new TickerSource(
            subscription.Symbol,
            subscription.Symbol,
            subscription.SourceUri,
            priceSelector);
        var extraction = _extractor.Extract(html, source);
        if (!extraction.Success)
        {
            return (null, null);
        }

        var change = string.IsNullOrWhiteSpace(changeSelector)
            ? null
            : _changeExtractor.Extract(html, changeSelector);
        return (extraction.Price, change);
    }
}