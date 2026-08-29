using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace SmartTicker.Desktop.ViewModels;

public sealed record TickerRun(string Text, IBrush? Brush = null);

public sealed record TickerSegment(IReadOnlyList<TickerRun> Runs, Uri? Link)
{
    public TickerSegment(string text, Uri? link)
        : this([new TickerRun(text)], link)
    {
    }
}

public sealed record TickerLane(
    IReadOnlyList<TickerSegment> Segments,
    int PixelsPerSecond,
    bool IsPaused,
    double RowHeight,
    double FontSize);