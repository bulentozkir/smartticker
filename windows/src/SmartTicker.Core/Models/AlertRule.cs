namespace SmartTicker.Core.Models;

public sealed record AlertRule
{
    public required Guid Id { get; init; }

    /// <summary>The quote this rule watches. Rules are orphaned when its subscription is removed.</summary>
    public required Guid SubscriptionId { get; init; }

    public required string Symbol { get; init; }

    public required AlertComparison Comparison { get; init; }

    public required decimal Threshold { get; init; }

    /// <summary>Null means the rule is active immediately.</summary>
    public DateTimeOffset? StartsOn { get; init; }

    /// <summary>Null means the rule never expires.</summary>
    public DateTimeOffset? EndsOn { get; init; }

    public bool Enabled { get; init; } = true;

    public string ComparisonText => Comparison switch
    {
        AlertComparison.LessThan => "<",
        AlertComparison.LessThanOrEqual => "<=",
        AlertComparison.GreaterThan => ">",
        AlertComparison.GreaterThanOrEqual => ">=",
        AlertComparison.EqualTo => "=",
        _ => "!=",
    };

    public string ScheduleText => (StartsOn, EndsOn) switch
    {
        (null, null) => "Never expires",
        ({ } from, null) => $"From {from:yyyy-MM-dd}",
        (null, { } to) => $"Until {to:yyyy-MM-dd}",
        ({ } from, { } to) => $"{from:yyyy-MM-dd} to {to:yyyy-MM-dd}",
    };

    public string Summary => $"{Symbol} {ComparisonText} {Threshold:0.####}";

    public string StateGlyph => Enabled ? "●" : "○";

    public string StateText => Enabled ? "Enabled" : "Disabled";

    public string StateColor => Enabled ? "#3FB950" : "#6E7681";

    /// <summary>The button names the action, not the current state.</summary>
    public string ToggleActionText => Enabled ? "Disable" : "Enable";

    public double RowOpacity => Enabled ? 1.0 : 0.55;
}
