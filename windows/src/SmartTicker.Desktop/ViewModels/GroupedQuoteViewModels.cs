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
    string Symbol,
    string Headline,
    string StatusText,
    Uri? SourceUri,
    IBrush SymbolForeground,
    IBrush HeadlineForeground);

public sealed class StaticNewsGroup : ObservableObject
{
    public const string AllQuotesFilter = "All quotes";

    private readonly IReadOnlyList<StaticNewsRow> _allRows;
    private readonly Action<string, string> _filterChanged;
    private string _selectedQuote;

    public StaticNewsGroup(
        string name,
        IReadOnlyList<StaticNewsRow> rows,
        string? selectedQuote,
        Action<string, string> filterChanged)
    {
        Name = name;
        _allRows = rows;
        _filterChanged = filterChanged;
        FilterOptions =
        [
            AllQuotesFilter,
            .. rows.Select(row => row.Symbol).Distinct(StringComparer.OrdinalIgnoreCase),
        ];
        _selectedQuote = FilterOptions.FirstOrDefault(option =>
            string.Equals(option, selectedQuote, StringComparison.OrdinalIgnoreCase)) ?? AllQuotesFilter;
    }

    public string Name { get; }

    public string DisplayName => string.IsNullOrEmpty(Name) ? "UNGROUPED" : Name.ToUpperInvariant();

    public IReadOnlyList<string> FilterOptions { get; }

    public string SelectedQuote
    {
        get => _selectedQuote;
        set
        {
            var normalized = FilterOptions.FirstOrDefault(option =>
                string.Equals(option, value, StringComparison.OrdinalIgnoreCase)) ?? AllQuotesFilter;
            if (SetProperty(ref _selectedQuote, normalized))
            {
                OnPropertyChanged(nameof(Rows));
                OnPropertyChanged(nameof(CountText));
                _filterChanged(Name, normalized);
            }
        }
    }

    public IReadOnlyList<StaticNewsRow> Rows => SelectedQuote == AllQuotesFilter
        ? _allRows
        : _allRows.Where(row =>
            string.Equals(row.Symbol, SelectedQuote, StringComparison.OrdinalIgnoreCase)).ToArray();

    public string CountText => Rows.Count == _allRows.Count
        ? $"{_allRows.Count} headline{(_allRows.Count == 1 ? string.Empty : "s")}"
        : $"{Rows.Count} of {_allRows.Count} headlines";
}

public sealed record QuoteGroupSummary(string Name, int QuoteCount, string Symbols)
{
    public string CountText => $"{QuoteCount} quote{(QuoteCount == 1 ? string.Empty : "s")}";

    public string SymbolsDisplay => string.IsNullOrWhiteSpace(Symbols) ? "No quotes assigned" : Symbols;
}