using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Controls;

public sealed class MarqueeText : UserControl
{
    private const double CopyGap = 40;
    private const double TargetPixelsPerFrame = 2.5;

    public static readonly StyledProperty<IReadOnlyList<TickerSegment>?> SegmentsProperty =
        AvaloniaProperty.Register<MarqueeText, IReadOnlyList<TickerSegment>?>(nameof(Segments));

    public static readonly StyledProperty<ICommand?> LinkCommandProperty =
        AvaloniaProperty.Register<MarqueeText, ICommand?>(nameof(LinkCommand));

    public static readonly StyledProperty<int> PixelsPerSecondProperty =
        AvaloniaProperty.Register<MarqueeText, int>(nameof(PixelsPerSecond), 50);

    public static readonly StyledProperty<bool> IsPausedProperty =
        AvaloniaProperty.Register<MarqueeText, bool>(nameof(IsPaused));

    public static readonly StyledProperty<IBrush?> TextBrushProperty =
        AvaloniaProperty.Register<MarqueeText, IBrush?>(nameof(TextBrush), Brushes.White);

    // Null falls back to TextBrush at reduced opacity; an explicit brush renders fully opaque.
    public static readonly StyledProperty<IBrush?> SeparatorBrushProperty =
        AvaloniaProperty.Register<MarqueeText, IBrush?>(nameof(SeparatorBrush));

    public static readonly StyledProperty<double> TickerFontSizeProperty =
        AvaloniaProperty.Register<MarqueeText, double>(nameof(TickerFontSize), 14);

    public static readonly StyledProperty<FontWeight> TickerFontWeightProperty =
        AvaloniaProperty.Register<MarqueeText, FontWeight>(nameof(TickerFontWeight), FontWeight.Normal);

    private readonly Canvas _canvas = new() { ClipToBounds = true };
    private readonly DispatcherTimer _timer = new() { Interval = AnimationIntervalFor(50) };
    private readonly Stopwatch _clock = new();
    private readonly List<Control> _copies = [];
    private readonly Cursor _handCursor = new(StandardCursorType.Hand);
    private double _contentWidth;
    private double _contentHeight;
    private double _origin;
    private bool _isAttached;

    public MarqueeText()
    {
        ClipToBounds = true;
        Content = _canvas;
        _timer.Tick += (sender, args) => ExceptionSafety.Run(() => OnAnimationTick(sender, args));
        SizeChanged += (_, _) => ExceptionSafety.Run(RebuildCopies);
        AttachedToVisualTree += OnAttachedToVisualTree;
        DetachedFromVisualTree += OnDetachedFromVisualTree;
    }

    public IReadOnlyList<TickerSegment>? Segments
    {
        get => GetValue(SegmentsProperty);
        set => SetValue(SegmentsProperty, value);
    }

    public ICommand? LinkCommand
    {
        get => GetValue(LinkCommandProperty);
        set => SetValue(LinkCommandProperty, value);
    }

    public int PixelsPerSecond
    {
        get => GetValue(PixelsPerSecondProperty);
        set => SetValue(PixelsPerSecondProperty, value);
    }

    public bool IsPaused
    {
        get => GetValue(IsPausedProperty);
        set => SetValue(IsPausedProperty, value);
    }

    public IBrush? TextBrush
    {
        get => GetValue(TextBrushProperty);
        set => SetValue(TextBrushProperty, value);
    }

    public IBrush? SeparatorBrush
    {
        get => GetValue(SeparatorBrushProperty);
        set => SetValue(SeparatorBrushProperty, value);
    }

    public double TickerFontSize
    {
        get => GetValue(TickerFontSizeProperty);
        set => SetValue(TickerFontSizeProperty, value);
    }

    public FontWeight TickerFontWeight
    {
        get => GetValue(TickerFontWeightProperty);
        set => SetValue(TickerFontWeightProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SegmentsProperty ||
            change.Property == TextBrushProperty ||
            change.Property == SeparatorBrushProperty ||
            change.Property == TickerFontSizeProperty ||
            change.Property == TickerFontWeightProperty)
        {
            ExceptionSafety.Run(() => Dispatcher.UIThread.Post(
                () => ExceptionSafety.Run(RebuildCopies),
                DispatcherPriority.Loaded));
        }

        if (change.Property == PixelsPerSecondProperty)
        {
            _timer.Interval = AnimationIntervalFor(PixelsPerSecond);
            _clock.Restart();
        }

        if (change.Property == IsPausedProperty)
        {
            UpdateAnimationState();
        }
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        ExceptionSafety.Run(() =>
        {
            _isAttached = true;
            RebuildCopies();
            UpdateAnimationState();
        });
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        ExceptionSafety.Run(() =>
        {
            _isAttached = false;
            _timer.Stop();
            _clock.Stop();
        });
    }

    private void UpdateAnimationState()
    {
        if (!_isAttached || IsPaused || _copies.Count == 0 || Bounds.Width <= 0)
        {
            _timer.Stop();
            _clock.Stop();
            return;
        }

        _clock.Restart();
        if (!_timer.IsEnabled)
        {
            _timer.Start();
        }
    }

    private void RebuildCopies()
    {
        var hadContent = _copies.Count > 0 && _contentWidth > 1;
        var previousOrigin = _origin;
        _canvas.Children.Clear();
        _copies.Clear();
        if (Segments is not { Count: > 0 })
        {
            UpdateAnimationState();
            return;
        }

        var first = CreateCopy();
        first.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        _contentWidth = Math.Max(1, first.DesiredSize.Width);
        _contentHeight = first.DesiredSize.Height;
        var cycleWidth = _contentWidth + CopyGap;
        var copyCount = Math.Max(2, (int)Math.Ceiling(Math.Max(Bounds.Width, 1) / cycleWidth) + 2);

        _copies.Add(first);
        _canvas.Children.Add(first);
        for (var index = 1; index < copyCount; index++)
        {
            var copy = CreateCopy();
            _copies.Add(copy);
            _canvas.Children.Add(copy);
        }

        // A refresh (price tick or alert blink) rebuilds the copies, so the crawl position is carried
        // over; resetting it here would restart the scroll on every update.
        _origin = hadContent
            ? Math.Clamp(previousOrigin, -cycleWidth, Math.Max(0, Bounds.Width))
            : Math.Max(0, Bounds.Width / 2);
        PositionCopies(cycleWidth);
        UpdateAnimationState();
    }

    private Control CreateCopy()
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center,
        };

        var segments = Segments ?? [];
        for (var index = 0; index < segments.Count; index++)
        {
            if (index > 0)
            {
                panel.Children.Add(CreateBlock("  ◆  ", SeparatorBrush is null ? 0.55 : 1, SeparatorBrush ?? TextBrush));
            }

            panel.Children.Add(CreateSegmentBlock(segments[index]));
        }

        return panel;
    }

    private Control CreateSegmentBlock(TickerSegment segment)
    {
        var highlightForeground = segment.Highlight?.Foreground;
        Control content;
        if (segment.Runs.Count == 1)
        {
            content = CreateBlock(segment.Runs[0].Text, 1, highlightForeground ?? segment.Runs[0].Brush ?? TextBrush);
        }
        else
        {
            var pair = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
            };
            foreach (var run in segment.Runs)
            {
                pair.Children.Add(CreateBlock(run.Text, 1, highlightForeground ?? run.Brush ?? TextBrush));
            }

            content = pair;
        }

        if (segment.Highlight is { } highlight)
        {
            // Padding is identical in both blink phases so the row does not shift as it flips.
            content = new Border
            {
                Background = highlight.Background,
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(8, 1),
                VerticalAlignment = VerticalAlignment.Center,
                Child = content,
            };
        }

        if (segment.Link is not { } link)
        {
            return content;
        }

        content.Cursor = _handCursor;
        ToolTip.SetTip(content, $"Double-click to open {link.AbsoluteUri}");

        content.PointerPressed += (_, args) =>
        {
            if (!args.GetCurrentPoint(content).Properties.IsLeftButtonPressed)
            {
                return;
            }

            args.Handled = true;
            if (args.ClickCount == 2 && LinkCommand?.CanExecute(link) == true)
            {
                ExceptionSafety.Run(() => LinkCommand.Execute(link));
            }
        };
        return content;
    }

    private TextBlock CreateBlock(string text, double opacity, IBrush? brush) => new()
    {
        Text = text,
        Foreground = brush,
        Opacity = opacity,
        FontFamily = new FontFamily("Inter"),
        FontSize = TickerFontSize,
        FontWeight = TickerFontWeight,
        TextWrapping = TextWrapping.NoWrap,
        VerticalAlignment = VerticalAlignment.Center,
    };

    private void OnAnimationTick(object? sender, EventArgs e)
    {
        var elapsedSeconds = Math.Min(_clock.Elapsed.TotalSeconds, 0.1);
        _clock.Restart();
        if (IsPaused || _copies.Count == 0 || Bounds.Width <= 0)
        {
            return;
        }

        var cycleWidth = _contentWidth + CopyGap;
        _origin -= Math.Clamp(PixelsPerSecond, 10, 200) * elapsedSeconds;
        while (_origin <= -cycleWidth)
        {
            _origin += cycleWidth;
        }

        PositionCopies(cycleWidth);
    }

    private void PositionCopies(double cycleWidth)
    {
        var top = Math.Max(0, (Bounds.Height - _contentHeight) / 2);
        for (var index = 0; index < _copies.Count; index++)
        {
            Canvas.SetLeft(_copies[index], _origin + index * cycleWidth);
            Canvas.SetTop(_copies[index], top);
        }
    }

    internal static TimeSpan AnimationIntervalFor(int pixelsPerSecond)
    {
        var speed = Math.Clamp(pixelsPerSecond, 10, 200);
        var milliseconds = Math.Clamp(TargetPixelsPerFrame * 1000 / speed, 33, 100);
        return TimeSpan.FromMilliseconds(milliseconds);
    }
}