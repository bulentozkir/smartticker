using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

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
    Guid SubscriptionId,
    string Symbol,
    string SourceName,
    string Headline,
    string StatusText,
    Uri? SourceUri,
    IBrush SymbolForeground,
    IBrush HeadlineForeground);

public sealed class StaticNewsQuoteFilter : ObservableObject
{
    private readonly Action<bool> _visibilityChanged;
    private bool _isShown;

    public StaticNewsQuoteFilter(
        Guid subscriptionId,
        string symbol,
        string sourceName,
        bool isShown,
        Action<bool> visibilityChanged)
    {
        SubscriptionId = subscriptionId;
        Label = $"{symbol} · {sourceName}";
        _isShown = isShown;
        _visibilityChanged = visibilityChanged;
    }

    public Guid SubscriptionId { get; }

    public string Label { get; }

    public bool IsShown
    {
        get => _isShown;
        set
        {
            if (SetProperty(ref _isShown, value))
            {
                _visibilityChanged(value);
            }
        }
    }
}

public sealed class StaticNewsGroup : ObservableObject
{
    private readonly IReadOnlyList<StaticNewsRow> _allRows;
    private readonly Action<string, Guid, bool> _filterChanged;

    public StaticNewsGroup(
        string name,
        IReadOnlyList<StaticNewsRow> rows,
        IReadOnlySet<Guid>? hiddenQuotes,
        Action<string, Guid, bool> filterChanged)
    {
        Name = name;
        _allRows = rows;
        _filterChanged = filterChanged;
        QuoteFilters = rows
            .GroupBy(row => row.SubscriptionId)
            .Select(group =>
            {
                var row = group.First();
                return new StaticNewsQuoteFilter(
                    row.SubscriptionId,
                    row.Symbol,
                    row.SourceName,
                    hiddenQuotes?.Contains(row.SubscriptionId) != true,
                    isShown => OnQuoteVisibilityChanged(row.SubscriptionId, isShown));
            })
            .ToArray();
    }

    public string Name { get; }

    public string DisplayName => string.IsNullOrEmpty(Name) ? "UNGROUPED" : Name.ToUpperInvariant();

    public IReadOnlyList<StaticNewsQuoteFilter> QuoteFilters { get; }

    public IReadOnlyList<StaticNewsRow> Rows
    {
        get
        {
            var shown = QuoteFilters
                .Where(filter => filter.IsShown)
                .Select(filter => filter.SubscriptionId)
                .ToHashSet();
            return _allRows.Where(row => shown.Contains(row.SubscriptionId)).ToArray();
        }
    }

    public string CountText => Rows.Count == _allRows.Count
        ? $"{_allRows.Count} headline{(_allRows.Count == 1 ? string.Empty : "s")}"
        : $"{Rows.Count} of {_allRows.Count} headlines";

    private void OnQuoteVisibilityChanged(Guid subscriptionId, bool isShown)
    {
        OnPropertyChanged(nameof(Rows));
        OnPropertyChanged(nameof(CountText));
        _filterChanged(Name, subscriptionId, isShown);
    }
}

public sealed record QuoteGroupSummary(string Name, int QuoteCount, string Symbols)
{
    public string CountText => $"{QuoteCount} quote{(QuoteCount == 1 ? string.Empty : "s")}";

    public string SymbolsDisplay => string.IsNullOrWhiteSpace(Symbols) ? "No quotes assigned" : Symbols;
}