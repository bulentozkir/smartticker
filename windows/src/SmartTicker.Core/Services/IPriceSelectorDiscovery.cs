using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

public interface IPriceSelectorDiscovery
{
    Task<IReadOnlyList<CssSelectorSuggestion>> DiscoverAsync(
        Uri pageUri,
        SelectorKind kind = SelectorKind.Price,
        CancellationToken cancellationToken = default);
}