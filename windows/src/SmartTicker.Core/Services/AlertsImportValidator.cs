using System.Text.Json;
using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

public sealed record AlertsImportResult(bool Success, AlertSettings? Settings, IReadOnlyList<string> Errors)
{
    public static AlertsImportResult Failed(params string[] errors) => new(false, null, errors);

    public static AlertsImportResult Succeeded(AlertSettings settings) => new(true, settings, []);
}

/// <summary>
/// Rejects an imported alerts file outright rather than silently dropping bad rules, so a partially
/// understood file never quietly replaces working alerts.
/// </summary>
public static class AlertsImportValidator
{
    public static AlertsImportResult Validate(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return AlertsImportResult.Failed("The file is empty.");
        }

        AlertSettings? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<AlertSettings>(json, SettingsJson.Options);
        }
        catch (JsonException exception)
        {
            return AlertsImportResult.Failed($"The file is not valid JSON: {exception.Message}");
        }

        if (parsed is null)
        {
            return AlertsImportResult.Failed("The file does not contain an alerts object.");
        }

        var errors = new List<string>();
        var rules = parsed.Rules ?? [];
        var seen = new HashSet<Guid>();
        for (var index = 0; index < rules.Length; index++)
        {
            var rule = rules[index];
            var label = $"Rule {index + 1}";
            if (rule is null)
            {
                errors.Add($"{label} is empty.");
                continue;
            }

            if (rule.Id == Guid.Empty)
            {
                errors.Add($"{label} has no id.");
            }
            else if (!seen.Add(rule.Id))
            {
                errors.Add($"{label} repeats the id {rule.Id}.");
            }

            if (rule.SubscriptionId == Guid.Empty)
            {
                errors.Add($"{label} has no subscriptionId.");
            }

            if (string.IsNullOrWhiteSpace(rule.Symbol))
            {
                errors.Add($"{label} has no symbol.");
            }

            if (!Enum.IsDefined(rule.Comparison))
            {
                errors.Add($"{label} has an unknown comparison '{rule.Comparison}'.");
            }

            if (rule.StartsOn is { } from && rule.EndsOn is { } to && to < from)
            {
                errors.Add($"{label} ends before it starts.");
            }
        }

        return errors.Count > 0
            ? new AlertsImportResult(false, null, errors)
            : AlertsImportResult.Succeeded(parsed.Normalize());
    }
}
