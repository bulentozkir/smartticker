using SmartTicker.Core.Models;
using SmartTicker.Core.Services;
using SmartTicker.Infrastructure.Networking;

namespace SmartTicker.Infrastructure.Extraction;

public sealed class StaticHtmlNewsFetcher : INewsFetcher, IDisposable
{
    private readonly PublicHtmlClient _client = new();
    private readonly StaticHtmlNewsExtractor _extractor = new();

    public async Task<NewsSnapshot> FetchAsync(
        TickerSubscription subscription,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var html = await _client.GetStringAsync(subscription.SourceUri, cancellationToken);
            var extraction = _extractor.Extract(html, subscription.NewsCssSelector, subscription.SourceUri);
            return new NewsSnapshot(
                subscription.Id,
                subscription.Symbol,
                subscription.SourceName,
                extraction.Headlines,
                DateTimeOffset.UtcNow,
                extraction.Success,
                extraction.Message);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception) when (exception is HttpRequestException or InvalidOperationException)
        {
            return new NewsSnapshot(
                subscription.Id,
                subscription.Symbol,
                subscription.SourceName,
                [],
                DateTimeOffset.UtcNow,
                false,
                exception.Message);
        }
    }

    public void Dispose() => _client.Dispose();
}
