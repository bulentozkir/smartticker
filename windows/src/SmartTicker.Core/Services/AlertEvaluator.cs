using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

public static class AlertEvaluator
{
    public static bool IsWithinSchedule(AlertRule rule, DateTimeOffset now) =>
        (rule.StartsOn is not { } from || now >= from) &&
        (rule.EndsOn is not { } to || now <= to);

    public static bool Matches(AlertRule rule, decimal price) => rule.Comparison switch
    {
        AlertComparison.LessThan => price < rule.Threshold,
        AlertComparison.LessThanOrEqual => price <= rule.Threshold,
        AlertComparison.GreaterThan => price > rule.Threshold,
        AlertComparison.GreaterThanOrEqual => price >= rule.Threshold,
        AlertComparison.EqualTo => price == rule.Threshold,
        _ => price != rule.Threshold,
    };

    public static bool ShouldFire(AlertRule rule, decimal price, DateTimeOffset now) =>
        rule.Enabled && IsWithinSchedule(rule, now) && Matches(rule, price);

    public static bool HasExpired(AlertRule rule, DateTimeOffset now) =>
        rule.EndsOn is { } to && now > to;
}
