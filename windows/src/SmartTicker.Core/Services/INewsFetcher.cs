using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

public interface INewsFetcher
{
    Task<NewsSnapshot> FetchAsync(
        TickerSubscription subscription,
        CancellationToken cancellationToken = default);
}
