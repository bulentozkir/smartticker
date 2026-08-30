using SmartTicker.Core.Models;
using SmartTicker.Core.Services;
using SmartTicker.Infrastructure.Networking;

namespace SmartTicker.Infrastructure.Extraction;

public sealed class StaticHtmlQuoteFetcher : IQuoteFetcher, IDisposable
{
    private readonly PublicHtmlClient _client = new();
    private readonly StaticHtmlPriceExtractor _extractor = new();
    private readonly StaticHtmlChangeExtractor _changeExtractor = new();

    public async Task<QuoteSnapshot> FetchAsync(
        TickerSubscription subscription,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var html = await _client.GetStringAsync(subscription.SourceUri, cancellationToken);
            var source = new TickerSource(
                subscription.Symbol,
                subscription.Symbol,
                subscription.SourceUri,
                subscription.CssSelector);
            var extraction = _extractor.Extract(html, source);
            var extended = ExtractExtended(html, subscription);
            return new QuoteSnapshot(
                subscription.Id,
                subscription.Symbol,
                subscription.SourceName,
                extraction.Price,
                extraction.Currency,
                DateTimeOffset.UtcNow,
                extraction.Success,
                extraction.Message,
                extraction.Success ? ExtractChange(html, subscription) : null,
                extended.Price,
                extended.ChangePercent);
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

    private decimal? ExtractChange(string html, TickerSubscription subscription) =>
        string.IsNullOrWhiteSpace(subscription.ChangeCssSelector)
            ? _changeExtractor.Extract(html)
            : _changeExtractor.Extract(html, subscription.ChangeCssSelector);

    private (decimal? Price, decimal? ChangePercent) ExtractExtended(string html, TickerSubscription subscription)
    {
        if (string.IsNullOrWhiteSpace(subscription.ExtendedCssSelector))
        {
            return (null, null);
        }

        var source = new TickerSource(
            subscription.Symbol,
            subscription.Symbol,
            subscription.SourceUri,
            subscription.ExtendedCssSelector);
        var extraction = _extractor.Extract(html, source);
        if (!extraction.Success)
        {
            return (null, null);
        }

        var change = string.IsNullOrWhiteSpace(subscription.ExtendedChangeCssSelector)
            ? null
            : _changeExtractor.Extract(html, subscription.ExtendedChangeCssSelector);
        return (extraction.Price, change);
    }
}