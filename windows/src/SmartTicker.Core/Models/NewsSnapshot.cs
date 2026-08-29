namespace SmartTicker.Core.Models;

public sealed record NewsSnapshot(
    Guid SubscriptionId,
    string Symbol,
    string SourceName,
    IReadOnlyList<NewsHeadline> Headlines,
    DateTimeOffset ObservedAt,
    bool Success,
    string Status);
