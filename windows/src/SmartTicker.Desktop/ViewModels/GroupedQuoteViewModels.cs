using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace SmartTicker.Desktop.ViewModels;

public sealed record StaticQuoteRow(
    Guid SubscriptionId,
    string Symbol,
    string LastText,
    string ChangeText,
    string ChangePercentText,
    string StatusText,
    string SessionSummary,
    Uri SourceUri,
    IBrush Background,
    IBrush SymbolForeground,
    IBrush LastForeground,
    IBrush ChangeForeground);

public sealed record StaticQuoteGroup(string Name, IReadOnlyList<StaticQuoteRow> Rows)
{
    public string DisplayName => string.IsNullOrEmpty(Name) ? "UNGROUPED" : Name.ToUpperInvariant();

    public string CountText => $"{Rows.Count} quote{(Rows.Count == 1 ? string.Empty : "s")}";
}

public sealed record StaticNewsRow(
    string Symbol,
    string Headline,
    string StatusText,
    Uri? SourceUri,
    IBrush SymbolForeground,
    IBrush HeadlineForeground);

public sealed record StaticNewsGroup(string Name, IReadOnlyList<StaticNewsRow> Rows)
{
    public string DisplayName => string.IsNullOrEmpty(Name) ? "UNGROUPED" : Name.ToUpperInvariant();

    public string CountText => $"{Rows.Count} headline{(Rows.Count == 1 ? string.Empty : "s")}";
}

public sealed record QuoteGroupSummary(string Name, int QuoteCount, string Symbols)
{
    public string CountText => $"{QuoteCount} quote{(QuoteCount == 1 ? string.Empty : "s")}";
}