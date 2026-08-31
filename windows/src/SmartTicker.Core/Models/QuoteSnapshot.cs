namespace SmartTicker.Core.Models;

public sealed record QuoteSnapshot(
    Guid SubscriptionId,
    string Symbol,
    string SourceName,
    decimal? Price,
    string? Currency,
    DateTimeOffset ObservedAt,
    bool Success,
    string Status,
    decimal? ChangePercent = null,
    decimal? ExtendedPrice = null,
    decimal? ExtendedChangePercent = null,
    decimal? PreMarketPrice = null,
    decimal? PreMarketChangePercent = null);