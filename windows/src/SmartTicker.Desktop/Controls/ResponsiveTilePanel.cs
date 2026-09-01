using System;
using Avalonia;
using Avalonia.Controls;

namespace SmartTicker.Desktop.Controls;

public sealed class ResponsiveTilePanel : Panel
{
    public static readonly StyledProperty<double> MinimumTileWidthProperty =
        AvaloniaProperty.Register<ResponsiveTilePanel, double>(nameof(MinimumTileWidth), 360);

    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<ResponsiveTilePanel, double>(nameof(Spacing), 12);

    public double MinimumTileWidth
    {
        get => GetValue(MinimumTileWidthProperty);
        set => SetValue(MinimumTileWidthProperty, value);
    }

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = double.IsFinite(availableSize.Width) ? availableSize.Width : MinimumTileWidth;
        var spacing = Math.Max(0, Spacing);
        var layout = Calculate(width, Children.Count);
        var columnHeights = new double[layout.Columns];
        foreach (var child in Children)
        {
            child.Measure(new Size(layout.TileWidth, double.PositiveInfinity));
            var column = ShortestColumn(columnHeights);
            columnHeights[column] += child.DesiredSize.Height + spacing;
        }

        var height = 0d;
        foreach (var columnHeight in columnHeights)
        {
            height = Math.Max(height, columnHeight);
        }

        return new Size(width, Math.Max(0, height - spacing));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var spacing = Math.Max(0, Spacing);
        var layout = Calculate(finalSize.Width, Children.Count);
        var columnHeights = new double[layout.Columns];
        foreach (var child in Children)
        {
            var column = ShortestColumn(columnHeights);
            var height = child.DesiredSize.Height;
            child.Arrange(new Rect(
                column * (layout.TileWidth + spacing),
                columnHeights[column],
                layout.TileWidth,
                height));
            columnHeights[column] += height + spacing;
        }

        return finalSize;
    }

    public TileLayout Calculate(double availableWidth) => Calculate(availableWidth, int.MaxValue);

    public TileLayout Calculate(double availableWidth, int tileCount)
    {
        var spacing = Math.Max(0, Spacing);
        var minimum = Math.Max(240, MinimumTileWidth);
        var width = Math.Max(0, availableWidth);
        var columns = Math.Max(1, (int)Math.Floor((width + spacing) / (minimum + spacing)));
        columns = Math.Min(columns, Math.Max(1, tileCount));
        return new TileLayout(columns, Math.Max(0, (width - spacing * (columns - 1)) / columns));
    }

    // Each tile starts under the shortest column, so a short group leaves no gap beneath it.
    private static int ShortestColumn(double[] columnHeights)
    {
        var shortest = 0;
        for (var index = 1; index < columnHeights.Length; index++)
        {
            if (columnHeights[index] < columnHeights[shortest])
            {
                shortest = index;
            }
        }

        return shortest;
    }
}

public readonly record struct TileLayout(int Columns, double TileWidth);