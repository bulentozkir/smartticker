using SmartTicker.Core.Models;
using SmartTicker.Core.Services;
using SmartTicker.Infrastructure.Networking;

namespace SmartTicker.Infrastructure.Extraction;

public sealed class StaticHtmlNewsSelectorDiscovery : INewsSelectorDiscovery, IDisposable
{
    private readonly PublicHtmlClient _client;
    private readonly StaticHtmlNewsSelectorAnalyzer _analyzer = new();

    public StaticHtmlNewsSelectorDiscovery(WebsiteAccessPolicy? accessPolicy = null)
    {
        _client = new PublicHtmlClient(accessPolicy);
    }

    public async Task<IReadOnlyList<CssSelectorSuggestion>> DiscoverAsync(
        Uri pageUri,
        CancellationToken cancellationToken = default)
    {
        var html = await _client.GetStringAsync(pageUri, cancellationToken);
        return _analyzer.Analyze(html);
    }

    public void Dispose() => _client.Dispose();
}