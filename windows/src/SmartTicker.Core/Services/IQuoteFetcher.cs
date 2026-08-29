using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

public interface IQuoteFetcher
{
    Task<QuoteSnapshot> FetchAsync(
        TickerSubscription subscription,
        CancellationToken cancellationToken = default);
}