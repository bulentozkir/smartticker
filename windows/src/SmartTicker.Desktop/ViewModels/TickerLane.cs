using System;
using System.Collections.Generic;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartTicker.Desktop.ViewModels;

public sealed record TickerRun(string Text, IBrush? Brush = null);

/// <summary>Fills the segment background so a fired alert stays visible while the row scrolls.</summary>
public sealed record TickerHighlight(IBrush Background, IBrush Foreground);

public sealed record TickerSegment(IReadOnlyList<TickerRun> Runs, Uri? Link)
{
    public TickerSegment(string text, Uri? link)
        : this([new TickerRun(text)], link)
    {
    }

    public TickerHighlight? Highlight { get; init; }
}

/// <summary>
/// Mutable so a refresh updates the existing lane instead of replacing it; replacing the item would
/// make the ItemsControl rebuild the marquee control and restart the scroll.
/// </summary>
public sealed partial class TickerLane : ObservableObject
{
    public TickerLane(
        IReadOnlyList<TickerSegment> segments,
        int pixelsPerSecond,
        bool isPaused,
        double rowHeight,
        double fontSize)
    {
        Segments = segments;
        PixelsPerSecond = pixelsPerSecond;
        IsPaused = isPaused;
        RowHeight = rowHeight;
        FontSize = fontSize;
    }

    [ObservableProperty]
    public partial IReadOnlyList<TickerSegment> Segments { get; set; }

    [ObservableProperty]
    public partial int PixelsPerSecond { get; set; }

    [ObservableProperty]
    public partial bool IsPaused { get; set; }

    [ObservableProperty]
    public partial double RowHeight { get; set; }

    [ObservableProperty]
    public partial double FontSize { get; set; }

    public void Update(
        IReadOnlyList<TickerSegment> segments,
        int pixelsPerSecond,
        bool isPaused,
        double rowHeight,
        double fontSize)
    {
        Segments = segments;
        PixelsPerSecond = pixelsPerSecond;
        IsPaused = isPaused;
        RowHeight = rowHeight;
        FontSize = fontSize;
    }
}