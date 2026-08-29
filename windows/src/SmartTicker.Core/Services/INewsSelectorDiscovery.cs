using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

public interface INewsSelectorDiscovery
{
    Task<IReadOnlyList<CssSelectorSuggestion>> DiscoverAsync(
        Uri pageUri,
        CancellationToken cancellationToken = default);
}