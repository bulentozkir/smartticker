using SmartTicker.Core.Models;
using SmartTicker.Core.Services;
using SmartTicker.Infrastructure.Networking;

namespace SmartTicker.Infrastructure.Extraction;

public sealed class StaticHtmlPriceSelectorDiscovery : IPriceSelectorDiscovery, IDisposable
{
    private readonly PublicHtmlClient _client;
    private readonly StaticHtmlSelectorAnalyzer _analyzer = new();

    public StaticHtmlPriceSelectorDiscovery(WebsiteAccessPolicy? accessPolicy = null)
    {
        _client = new PublicHtmlClient(accessPolicy);
    }

    public async Task<IReadOnlyList<CssSelectorSuggestion>> DiscoverAsync(
        Uri pageUri,
        SelectorKind kind = SelectorKind.Price,
        CancellationToken cancellationToken = default)
    {
        var html = await _client.GetStringAsync(pageUri, cancellationToken);
        return _analyzer.Analyze(html, kind);
    }

    public void Dispose() => _client.Dispose();
}