using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace SmartTicker.Desktop.Views;

internal static class WindowReachability
{
    private const int MinimumCoordinate = 32;
    private const int ReachableCornerSize = 32;
    private static readonly ConditionalWeakTable<Window, GuardState> States = new();

    public static void Attach(Window window)
    {
        var state = States.GetValue(window, _ => new GuardState());
        if (state.IsAttached)
        {
            return;
        }

        state.IsAttached = true;
        window.Opened += (_, _) => ExceptionSafety.Run(() => EnsureReachable(window, state));
        window.PositionChanged += (_, _) => ExceptionSafety.Run(() => QueueEnsureReachable(window, state));
    }

    public static void EnsureReachable(Window window)
    {
        var state = States.GetValue(window, _ => new GuardState { IsAttached = true });
        EnsureReachable(window, state);
    }

    internal static PixelPoint ClampTopLeft(
        PixelPoint position,
        IReadOnlyList<PixelRect> workingAreas,
        PixelRect preferredArea)
    {
        var usableAreas = workingAreas
            .Where(CanHostReachableCorner)
            .ToArray();
        if (usableAreas.Length == 0)
        {
            return new PixelPoint(
                Math.Max(MinimumCoordinate, position.X),
                Math.Max(MinimumCoordinate, position.Y));
        }

        var currentArea = usableAreas
            .Select(area => (PixelRect?)area)
            .FirstOrDefault(area => IsReachable(position, area!.Value));
        if (currentArea is not null)
        {
            return position;
        }

        var target = CanHostReachableCorner(preferredArea)
            ? preferredArea
            : usableAreas[0];
        var minimumX = Math.Max(MinimumCoordinate, target.X);
        var minimumY = Math.Max(MinimumCoordinate, target.Y);
        var maximumX = Math.Max(minimumX, target.Right - ReachableCornerSize);
        var maximumY = Math.Max(minimumY, target.Bottom - ReachableCornerSize);
        return new PixelPoint(
            Math.Clamp(position.X, minimumX, maximumX),
            Math.Clamp(position.Y, minimumY, maximumY));
    }

    private static void EnsureReachable(Window window, GuardState state)
    {
        if (state.IsAdjusting || window.Screens is not { } screens || screens.All.Count == 0)
        {
            return;
        }

        var preferred = screens.ScreenFromPoint(window.Position)?.WorkingArea ??
            screens.ScreenFromWindow(window)?.WorkingArea ??
            screens.Primary?.WorkingArea ??
            screens.All[0].WorkingArea;
        var position = ClampTopLeft(
            window.Position,
            screens.All.Select(screen => screen.WorkingArea).ToArray(),
            preferred);
        if (position == window.Position)
        {
            return;
        }

        state.IsAdjusting = true;
        try
        {
            window.Position = position;
        }
        finally
        {
            state.IsAdjusting = false;
        }
    }

    private static void QueueEnsureReachable(Window window, GuardState state)
    {
        if (state.IsQueued)
        {
            return;
        }

        state.IsQueued = true;
        Dispatcher.UIThread.Post(() => ExceptionSafety.Run(() =>
        {
            state.IsQueued = false;
            EnsureReachable(window, state);
        }), DispatcherPriority.Background);
    }

    private static bool CanHostReachableCorner(PixelRect area) =>
        area.Right >= MinimumCoordinate + ReachableCornerSize &&
        area.Bottom >= MinimumCoordinate + ReachableCornerSize;

    private static bool IsReachable(PixelPoint position, PixelRect area)
    {
        var minimumX = Math.Max(MinimumCoordinate, area.X);
        var minimumY = Math.Max(MinimumCoordinate, area.Y);
        return position.X >= minimumX &&
            position.Y >= minimumY &&
            position.X <= area.Right - ReachableCornerSize &&
            position.Y <= area.Bottom - ReachableCornerSize;
    }

    private sealed class GuardState
    {
        public bool IsAttached { get; set; }

        public bool IsAdjusting { get; set; }

        public bool IsQueued { get; set; }
    }
}