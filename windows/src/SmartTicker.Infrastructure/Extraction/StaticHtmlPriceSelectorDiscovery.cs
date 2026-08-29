using SmartTicker.Core.Models;
using SmartTicker.Core.Services;
using SmartTicker.Infrastructure.Networking;

namespace SmartTicker.Infrastructure.Extraction;

public sealed class StaticHtmlPriceSelectorDiscovery : IPriceSelectorDiscovery, IDisposable
{
    private readonly PublicHtmlClient _client = new();
    private readonly StaticHtmlSelectorAnalyzer _analyzer = new();

    public async Task<IReadOnlyList<CssSelectorSuggestion>> DiscoverAsync(
        Uri pageUri,
        CancellationToken cancellationToken = default)
    {
        var html = await _client.GetStringAsync(pageUri, cancellationToken);
        return _analyzer.Analyze(html);
    }

    public void Dispose() => _client.Dispose();
}