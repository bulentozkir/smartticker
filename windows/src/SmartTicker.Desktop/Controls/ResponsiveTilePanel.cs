using System;
using Avalonia;
using Avalonia.Controls;

namespace SmartTicker.Desktop.Controls;

public sealed class ResponsiveTilePanel : Panel
{
    public static readonly StyledProperty<double> MinimumTileWidthProperty =
        AvaloniaProperty.Register<ResponsiveTilePanel, double>(nameof(MinimumTileWidth), 360);

    public static readonly StyledProperty<double> MaximumTileWidthProperty =
        AvaloniaProperty.Register<ResponsiveTilePanel, double>(nameof(MaximumTileWidth), 560);

    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<ResponsiveTilePanel, double>(nameof(Spacing), 12);

    public double MinimumTileWidth
    {
        get => GetValue(MinimumTileWidthProperty);
        set => SetValue(MinimumTileWidthProperty, value);
    }

    public double MaximumTileWidth
    {
        get => GetValue(MaximumTileWidthProperty);
        set => SetValue(MaximumTileWidthProperty, value);
    }

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = double.IsFinite(availableSize.Width)
            ? availableSize.Width
            : Math.Max(MinimumTileWidth, MaximumTileWidth);
        var layout = Calculate(width);
        var height = 0d;
        var rowHeight = 0d;
        var column = 0;
        foreach (var child in Children)
        {
            child.Measure(new Size(layout.TileWidth, double.PositiveInfinity));
            rowHeight = Math.Max(rowHeight, child.DesiredSize.Height);
            column++;
            if (column == layout.Columns)
            {
                height += rowHeight + Spacing;
                rowHeight = 0;
                column = 0;
            }
        }

        if (column > 0)
        {
            height += rowHeight;
        }
        else if (height > 0)
        {
            height -= Spacing;
        }

        return new Size(width, Math.Max(0, height));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var layout = Calculate(finalSize.Width);
        var y = 0d;
        for (var start = 0; start < Children.Count; start += layout.Columns)
        {
            var count = Math.Min(layout.Columns, Children.Count - start);
            var rowHeight = 0d;
            for (var index = 0; index < count; index++)
            {
                rowHeight = Math.Max(rowHeight, Children[start + index].DesiredSize.Height);
            }

            for (var index = 0; index < count; index++)
            {
                var x = index * (layout.TileWidth + Spacing);
                Children[start + index].Arrange(new Rect(x, y, layout.TileWidth, rowHeight));
            }

            y += rowHeight + Spacing;
        }

        return finalSize;
    }

    public TileLayout Calculate(double availableWidth)
    {
        var spacing = Math.Max(0, Spacing);
        var minimum = Math.Max(240, MinimumTileWidth);
        var maximum = Math.Max(minimum, MaximumTileWidth);
        var width = Math.Max(0, availableWidth);
        var columns = Math.Max(1, (int)Math.Floor((width + spacing) / (minimum + spacing)));
        var tileWidth = columns == 1
            ? Math.Min(maximum, width)
            : Math.Min(maximum, (width - spacing * (columns - 1)) / columns);
        return new TileLayout(columns, Math.Max(0, tileWidth));
    }
}

public readonly record struct TileLayout(int Columns, double TileWidth);