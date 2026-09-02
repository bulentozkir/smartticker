using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartTicker.Desktop.ViewModels;

public sealed partial class StaticQuoteRow : ObservableObject
{
    public StaticQuoteRow(
        Guid subscriptionId,
        string symbol,
        string lastText,
        string changeText,
        string changePercentText,
        string statusText,
        string sessionSummary,
        Uri sourceUri,
        IBrush background,
        IBrush symbolForeground,
        IBrush lastForeground,
        IBrush changeForeground)
    {
        SubscriptionId = subscriptionId;
        Symbol = symbol;
        LastText = lastText;
        ChangeText = changeText;
        ChangePercentText = changePercentText;
        StatusText = statusText;
        SessionSummary = sessionSummary;
        SourceUri = sourceUri;
        Background = background;
        SymbolForeground = symbolForeground;
        LastForeground = lastForeground;
        ChangeForeground = changeForeground;
    }

    [ObservableProperty]
    public partial Guid SubscriptionId { get; set; }

    [ObservableProperty]
    public partial string Symbol { get; set; }

    [ObservableProperty]
    public partial string LastText { get; set; }

    [ObservableProperty]
    public partial string ChangeText { get; set; }

    [ObservableProperty]
    public partial string ChangePercentText { get; set; }

    [ObservableProperty]
    public partial string StatusText { get; set; }

    [ObservableProperty]
    public partial string SessionSummary { get; set; }

    [ObservableProperty]
    public partial Uri SourceUri { get; set; }

    [ObservableProperty]
    public partial IBrush Background { get; set; }

    [ObservableProperty]
    public partial IBrush SymbolForeground { get; set; }

    [ObservableProperty]
    public partial IBrush LastForeground { get; set; }

    [ObservableProperty]
    public partial IBrush ChangeForeground { get; set; }

    public void UpdateFrom(StaticQuoteRow updated)
    {
        Symbol = updated.Symbol;
        LastText = updated.LastText;
        ChangeText = updated.ChangeText;
        ChangePercentText = updated.ChangePercentText;
        StatusText = updated.StatusText;
        SessionSummary = updated.SessionSummary;
        SourceUri = updated.SourceUri;
        Background = updated.Background;
        SymbolForeground = updated.SymbolForeground;
        LastForeground = updated.LastForeground;
        ChangeForeground = updated.ChangeForeground;
    }
}

public sealed class StaticQuoteGroup : ObservableObject
{
    public StaticQuoteGroup(string name, IReadOnlyList<StaticQuoteRow> rows)
    {
        Name = name;
        UpdateRows(rows);
    }

    public string Name { get; }

    public string DisplayName => string.IsNullOrEmpty(Name) ? "UNGROUPED" : Name.ToUpperInvariant();

    public ObservableCollection<StaticQuoteRow> Rows { get; } = [];

    public string CountText => $"{Rows.Count} quote{(Rows.Count == 1 ? string.Empty : "s")}";

    public void UpdateRows(IReadOnlyList<StaticQuoteRow> updated)
    {
        var countChanged = Rows.Count != updated.Count;
        for (var index = 0; index < updated.Count; index++)
        {
            var existingIndex = IndexOf(Rows, updated[index].SubscriptionId, index);
            if (existingIndex < 0)
            {
                Rows.Insert(index, updated[index]);
                continue;
            }

            if (existingIndex != index)
            {
                Rows.Move(existingIndex, index);
            }

            Rows[index].UpdateFrom(updated[index]);
        }

        while (Rows.Count > updated.Count)
        {
            Rows.RemoveAt(Rows.Count - 1);
        }

        if (countChanged)
        {
            OnPropertyChanged(nameof(CountText));
        }
    }

    private static int IndexOf(
        IReadOnlyList<StaticQuoteRow> rows,
        Guid subscriptionId,
        int startIndex)
    {
        for (var index = startIndex; index < rows.Count; index++)
        {
            if (rows[index].SubscriptionId == subscriptionId)
            {
                return index;
            }
        }

        return -1;
    }
}

public sealed partial class StaticNewsRow : ObservableObject
{
    public StaticNewsRow(
        Guid subscriptionId,
        string symbol,
        string sourceName,
        string headline,
        string statusText,
        Uri? sourceUri,
        IBrush symbolForeground,
        IBrush headlineForeground)
    {
        SubscriptionId = subscriptionId;
        Symbol = symbol;
        SourceName = sourceName;
        Headline = headline;
        StatusText = statusText;
        SourceUri = sourceUri;
        SymbolForeground = symbolForeground;
        HeadlineForeground = headlineForeground;
    }

    [ObservableProperty]
    public partial Guid SubscriptionId { get; set; }

    [ObservableProperty]
    public partial string Symbol { get; set; }

    [ObservableProperty]
    public partial string SourceName { get; set; }

    [ObservableProperty]
    public partial string Headline { get; set; }

    [ObservableProperty]
    public partial string StatusText { get; set; }

    [ObservableProperty]
    public partial Uri? SourceUri { get; set; }

    [ObservableProperty]
    public partial IBrush SymbolForeground { get; set; }

    [ObservableProperty]
    public partial IBrush HeadlineForeground { get; set; }

    [ObservableProperty]
    public partial IBrush Background { get; set; } = Brushes.Transparent;

    public void UpdateFrom(StaticNewsRow updated)
    {
        Symbol = updated.Symbol;
        SourceName = updated.SourceName;
        Headline = updated.Headline;
        StatusText = updated.StatusText;
        SourceUri = updated.SourceUri;
        SymbolForeground = updated.SymbolForeground;
        HeadlineForeground = updated.HeadlineForeground;
        Background = updated.Background;
    }
}

public sealed class StaticNewsQuoteFilter : ObservableObject
{
    private readonly Action<bool> _visibilityChanged;
    private bool _isShown;
    private string _symbol;
    private string _label;

    public StaticNewsQuoteFilter(
        Guid subscriptionId,
        string symbol,
        string sourceName,
        bool isShown,
        Action<bool> visibilityChanged)
    {
        SubscriptionId = subscriptionId;
        _symbol = symbol;
        _label = $"{symbol} · {sourceName}";
        _isShown = isShown;
        _visibilityChanged = visibilityChanged;
    }

    public Guid SubscriptionId { get; }

    public string Symbol => _symbol;

    public string Label => _label;

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

    public void Update(string symbol, string sourceName)
    {
        if (SetProperty(ref _symbol, symbol, nameof(Symbol)))
        {
            OnPropertyChanged(nameof(Label));
        }

        SetProperty(ref _label, $"{symbol} · {sourceName}", nameof(Label));
    }
}

public sealed class StaticNewsGroup : ObservableObject
{
    private readonly List<StaticNewsRow> _allRows = [];
    private readonly Action<Guid, bool> _filterChanged;

    public StaticNewsGroup(
        string name,
        IReadOnlyList<StaticNewsRow> rows,
        IReadOnlySet<Guid>? hiddenQuotes,
        Action<Guid, bool> filterChanged)
    {
        Name = name;
        _filterChanged = filterChanged;
        UpdateRows(rows, hiddenQuotes);
    }

    public string Name { get; }

    public string DisplayName => string.IsNullOrEmpty(Name) ? "UNGROUPED" : Name.ToUpperInvariant();

    public ObservableCollection<StaticNewsQuoteFilter> QuoteFilters { get; } = [];

    public string FilterSummary
    {
        get
        {
            var shown = QuoteFilters.Count(filter => filter.IsShown);
            return shown switch
            {
                0 => "No quotes",
                1 => QuoteFilters.First(filter => filter.IsShown).Symbol,
                _ when shown == QuoteFilters.Count => "All quotes",
                _ => $"{shown} of {QuoteFilters.Count} quotes",
            };
        }
    }

    public ObservableCollection<StaticNewsRow> Rows { get; } = [];

    internal IReadOnlyList<StaticNewsRow> AllRows => _allRows;

    public string CountText => Rows.Count == _allRows.Count
        ? $"{_allRows.Count} headline{(_allRows.Count == 1 ? string.Empty : "s")}"
        : $"{Rows.Count} of {_allRows.Count} headlines";

    private void OnQuoteVisibilityChanged(Guid subscriptionId, bool isShown)
    {
        ReconcileVisibleRows();
        OnPropertyChanged(nameof(CountText));
        OnPropertyChanged(nameof(FilterSummary));
        _filterChanged(subscriptionId, isShown);
    }

    public void UpdateRows(IReadOnlyList<StaticNewsRow> updated, IReadOnlySet<Guid>? hiddenQuotes)
    {
        for (var index = 0; index < updated.Count; index++)
        {
            var existingIndex = IndexOf(_allRows, updated[index], index);
            if (existingIndex < 0)
            {
                _allRows.Insert(index, updated[index]);
                continue;
            }

            var existing = _allRows[existingIndex];
            if (existingIndex != index)
            {
                _allRows.RemoveAt(existingIndex);
                _allRows.Insert(index, existing);
            }

            existing.UpdateFrom(updated[index]);
        }

        while (_allRows.Count > updated.Count)
        {
            _allRows.RemoveAt(_allRows.Count - 1);
        }

        var filterRows = updated
            .GroupBy(row => row.SubscriptionId)
            .Select(group => group.First())
            .ToArray();
        for (var index = 0; index < filterRows.Length; index++)
        {
            var row = filterRows[index];
            var existingIndex = IndexOf(QuoteFilters, row.SubscriptionId, index);
            if (existingIndex < 0)
            {
                QuoteFilters.Insert(index, new StaticNewsQuoteFilter(
                    row.SubscriptionId,
                    row.Symbol,
                    row.SourceName,
                    hiddenQuotes?.Contains(row.SubscriptionId) != true,
                    isShown => OnQuoteVisibilityChanged(row.SubscriptionId, isShown)));
                continue;
            }

            if (existingIndex != index)
            {
                QuoteFilters.Move(existingIndex, index);
            }

            QuoteFilters[index].Update(row.Symbol, row.SourceName);
        }

        while (QuoteFilters.Count > filterRows.Length)
        {
            QuoteFilters.RemoveAt(QuoteFilters.Count - 1);
        }

        ReconcileVisibleRows();
        OnPropertyChanged(nameof(CountText));
        OnPropertyChanged(nameof(FilterSummary));
    }

    private void ReconcileVisibleRows()
    {
        var shown = QuoteFilters
            .Where(filter => filter.IsShown)
            .Select(filter => filter.SubscriptionId)
            .ToHashSet();
        var updated = _allRows.Where(row => shown.Contains(row.SubscriptionId)).ToArray();
        for (var index = 0; index < updated.Length; index++)
        {
            var existingIndex = IndexOf(Rows, updated[index], index);
            if (existingIndex < 0)
            {
                Rows.Insert(index, updated[index]);
            }
            else if (existingIndex != index)
            {
                Rows.Move(existingIndex, index);
            }
        }

        while (Rows.Count > updated.Length)
        {
            Rows.RemoveAt(Rows.Count - 1);
        }
    }

    private static int IndexOf(
        IReadOnlyList<StaticNewsRow> rows,
        StaticNewsRow target,
        int startIndex)
    {
        for (var index = startIndex; index < rows.Count; index++)
        {
            if (rows[index].SubscriptionId == target.SubscriptionId &&
                rows[index].Headline == target.Headline)
            {
                return index;
            }
        }

        return -1;
    }

    private static int IndexOf(
        IReadOnlyList<StaticNewsQuoteFilter> filters,
        Guid subscriptionId,
        int startIndex)
    {
        for (var index = startIndex; index < filters.Count; index++)
        {
            if (filters[index].SubscriptionId == subscriptionId)
            {
                return index;
            }
        }

        return -1;
    }
}

public sealed record QuoteGroupSummary(string Name, int QuoteCount, string Symbols)
{
    public string CountText => $"{QuoteCount} quote{(QuoteCount == 1 ? string.Empty : "s")}";

    public string SymbolsDisplay => string.IsNullOrWhiteSpace(Symbols) ? "No quotes assigned" : Symbols;
}