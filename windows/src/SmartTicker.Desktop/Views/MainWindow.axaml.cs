using System;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Views;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _priceRefreshTimer = new();
    private readonly DispatcherTimer _newsRefreshTimer = new();
    private MainViewModel? _observedViewModel;
    private string? _draggedGroupName;
    private StaticNewsWindow? _staticNewsWindow;
    private bool _staticNewsSyncQueued;
    private bool _isClosing;

    public MainWindow()
    {
        InitializeComponent();
        _priceRefreshTimer.Tick += OnPriceRefreshTimerTick;
        _newsRefreshTimer.Tick += OnNewsRefreshTimerTick;
        DataContextChanged += (_, _) => RunSafely("Applying window data", () =>
        {
            ConfigureFlowTimers();
            if (IsVisible)
            {
                QueueStaticNewsWindowSync();
            }
        });
        Opened += (_, _) => RunSafely("Opening SmartTicker", () =>
        {
            ConfigurePassiveWindow();
            SyncStaticNewsWindow();
            ExceptionSafety.Run(
                () => Dispatcher.UIThread.Post(() => RunSafely("Opening Quotes", () =>
                {
                    if (!_isClosing && ViewModel is { ShowStarterPrompt: true })
                    {
                        new SettingsWindow { DataContext = DataContext }.Show(this);
                    }
                })),
                exception => ReportRecoverableError("Queueing the Quotes window", exception));
        });
        Deactivated += (_, _) => RunSafely("Releasing pointer capture", ReleasePointerCapture);
        PointerReleased += (_, _) => RunSafely("Releasing pointer capture", ReleasePointerCapture);
        SizeChanged += (_, e) => RunSafely("Resizing SmartTicker", () =>
        {
            if (ViewModel is { } viewModel)
            {
                viewModel.WindowHeight = e.NewSize.Height;
            }
        });
        Closing += (_, _) => _isClosing = true;
        Closed += (_, _) =>
        {
            _isClosing = true;
            RunSafely("Closing static News", CloseStaticNewsWindow);
            RunSafely("Releasing pointer capture", ReleasePointerCapture);
            RunSafely("Stopping refresh timers", StopFlowTimers);
            RunSafely("Closing SmartTicker", () => ViewModel?.Dispose());
        };
    }

    private MainViewModel? ViewModel => DataContext as MainViewModel;

    private async void OnPriceRefreshTimerTick(object? sender, EventArgs e)
    {
        await ExceptionSafety.RunAsync(
            async () =>
            {
                if (!_isClosing && ViewModel is { IsPaused: false } viewModel)
                {
                    await viewModel.RefreshPricesSafelyAsync("Automatic price refresh");
                }
            },
            exception => ReportRecoverableError("Automatic price refresh", exception));
    }

    private async void OnNewsRefreshTimerTick(object? sender, EventArgs e)
    {
        await ExceptionSafety.RunAsync(
            async () =>
            {
                if (!_isClosing && ViewModel is { IsPaused: false } viewModel)
                {
                    await viewModel.RefreshNewsSafelyAsync("Automatic news refresh");
                }
            },
            exception => ReportRecoverableError("Automatic news refresh", exception));
    }

    private void BeginWindowDrag(object? sender, PointerPressedEventArgs e)
    {
        RunSafely("Moving SmartTicker", () =>
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                e.Handled = true;
                ReleasePointerCapture();
                BeginMoveDrag(e);
            }
        });
    }

    private void BeginWindowResize(object? sender, PointerPressedEventArgs e)
    {
        RunSafely("Resizing SmartTicker", () =>
        {
            if (sender is Control { Tag: string edgeName } &&
                e.GetCurrentPoint(this).Properties.IsLeftButtonPressed &&
                Enum.TryParse<WindowEdge>(edgeName, out var edge))
            {
                e.Handled = true;
                ReleasePointerCapture();
                BeginResizeDrag(edge, e);
            }
        });
    }

    private void OpenStaticQuote(object? sender, PointerPressedEventArgs e)
    {
        RunSafely("Opening quote source", () =>
        {
            if (sender is Border { DataContext: StaticQuoteRow row } &&
                e.ClickCount == 2 &&
                e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                e.Handled = true;
                ViewModel?.OpenLinkCommand.Execute(row.SourceUri);
            }
        });
    }

    private async void BeginGroupDrag(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control &&
            e.GetCurrentPoint(control).Properties.IsLeftButtonPressed &&
            TryGetGroupName(control.DataContext, out var groupName))
        {
            _draggedGroupName = groupName;
            e.Handled = true;
            var data = new DataTransfer();
            data.Add(DataTransferItem.CreateText(groupName));
            try
            {
                await DragDrop.DoDragDropAsync(e, data, DragDropEffects.Move);
            }
            catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
            {
                ReportRecoverableError("Reordering groups", exception);
            }
            finally
            {
                _draggedGroupName = null;
            }
        }
    }

    private void GroupDragOver(object? sender, DragEventArgs e)
    {
        RunSafely("Reordering groups", () =>
        {
            var canMove = sender is Control control &&
                _draggedGroupName is not null &&
                TryGetGroupName(control.DataContext, out var targetName) &&
                !string.Equals(_draggedGroupName, targetName, StringComparison.OrdinalIgnoreCase);
            e.DragEffects = canMove ? DragDropEffects.Move : DragDropEffects.None;
            e.Handled = true;
        });
    }

    private void GroupDrop(object? sender, DragEventArgs e)
    {
        RunSafely("Reordering groups", () =>
        {
            if (sender is Control control &&
                _draggedGroupName is { } sourceName &&
                TryGetGroupName(control.DataContext, out var targetName) &&
                !string.Equals(sourceName, targetName, StringComparison.OrdinalIgnoreCase))
            {
                var placeAfter = e.GetPosition(control).X >= control.Bounds.Width / 2;
                ViewModel?.MoveQuoteGroup(sourceName, targetName, placeAfter);
                e.DragEffects = DragDropEffects.Move;
            }

            e.Handled = true;
        });
    }

    private static bool TryGetGroupName(object? value, out string groupName)
    {
        groupName = value switch
        {
            StaticQuoteGroup quoteGroup => quoteGroup.Name,
            StaticNewsGroup newsGroup => newsGroup.Name,
            _ => null!,
        };
        return groupName is not null;
    }

    private void ConfigurePassiveWindow()
    {
        if (OperatingSystem.IsWindows() && TryGetPlatformHandle() is { } handle)
        {
            WindowsPassiveWindow.MakeNonActivating(handle.Handle);
        }
    }

    private static void ReleasePointerCapture()
    {
        if (OperatingSystem.IsWindows())
        {
            WindowsPassiveWindow.ReleasePointerCapture();
        }
    }

    private void ExitApplication(object? sender, RoutedEventArgs e) => RunSafely("Closing SmartTicker", Close);

    private void ShowHelp(object? sender, RoutedEventArgs e) => RunSafely("Opening Help", () => HelpWindow.Open(this));

    private void ShowAbout(object? sender, RoutedEventArgs e) =>
        RunSafely("Opening About", () => new AboutWindow { DataContext = DataContext }.ShowDialog(this));

    private void OpenSettings(object? sender, RoutedEventArgs e)
    {
        RunSafely("Opening Quotes", () =>
        {
            var settings = new SettingsWindow { DataContext = DataContext };
            settings.Show(this);
        });
    }

    private void OpenQuoteGroups(object? sender, RoutedEventArgs e) =>
        RunSafely("Opening Quote Groups", () => QuoteGroupsWindow.Open(this, DataContext));

    private void OpenStaticNewsWindow(object? sender, RoutedEventArgs e) =>
        RunSafely("Opening static News", ShowStaticNewsWindow);

    private void OpenAppSettings(object? sender, RoutedEventArgs e)
    {
        RunSafely("Opening App Settings", () =>
        {
            var settings = new AppSettingsWindow { DataContext = DataContext };
            settings.Show(this);
        });
    }

    private void OpenAlerts(object? sender, RoutedEventArgs e)
    {
        RunSafely("Opening Alerts", () =>
        {
            var alerts = new AlertsWindow { DataContext = DataContext };
            alerts.Show(this);
        });
    }

    private void ConfigureFlowTimers()
    {
        _priceRefreshTimer.Stop();
        _newsRefreshTimer.Stop();

        if (_observedViewModel is not null)
        {
            _observedViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _observedViewModel = ViewModel;
        if (ViewModel is not { } viewModel)
        {
            return;
        }

        viewModel.PropertyChanged += OnViewModelPropertyChanged;
        ApplyRefreshIntervals();
        _priceRefreshTimer.Start();
        _newsRefreshTimer.Start();
        _ = viewModel.RefreshPricesSafelyAsync("Initial price refresh");
        _ = viewModel.RefreshNewsSafelyAsync("Initial news refresh");
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e) =>
        RunSafely("Applying a setting change", () => ApplyViewModelPropertyChange(e));

    private void ApplyViewModelPropertyChange(PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MainViewModel.PriceRefreshSeconds) or nameof(MainViewModel.NewsRefreshSeconds))
        {
            ApplyRefreshIntervals();
        }

        if (e.PropertyName is nameof(MainViewModel.UseStaticGroupedView) or nameof(MainViewModel.ShowNewsLine))
        {
            QueueStaticNewsWindowSync();
        }
    }

    private void QueueStaticNewsWindowSync()
    {
        if (_staticNewsSyncQueued || _isClosing)
        {
            return;
        }

        _staticNewsSyncQueued = true;
        ExceptionSafety.Run(
            () => Dispatcher.UIThread.Post(() => RunSafely("Synchronizing static News", () =>
            {
                _staticNewsSyncQueued = false;
                if (!_isClosing)
                {
                    SyncStaticNewsWindow();
                }
            }), DispatcherPriority.Loaded),
            exception =>
            {
                _staticNewsSyncQueued = false;
                ReportRecoverableError("Queueing static News", exception);
            });
    }

    // Reassigning Interval restarts the countdown, so a running timer picks the new value up immediately.
    private void ApplyRefreshIntervals()
    {
        if (ViewModel is not { } viewModel)
        {
            return;
        }

        _priceRefreshTimer.Interval = TimeSpan.FromSeconds(viewModel.PriceRefreshSeconds);
        _newsRefreshTimer.Interval = TimeSpan.FromSeconds(viewModel.NewsRefreshSeconds);
    }

    private void StopFlowTimers()
    {
        _priceRefreshTimer.Stop();
        _newsRefreshTimer.Stop();
        if (_observedViewModel is not null)
        {
            _observedViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            _observedViewModel = null;
        }
    }

    private void SyncStaticNewsWindow()
    {
        if (ViewModel is { UseStaticGroupedView: true, ShowNewsLine: true })
        {
            ShowStaticNewsWindow();
        }
        else
        {
            CloseStaticNewsWindow();
        }
    }

    private void ShowStaticNewsWindow()
    {
        if (_staticNewsWindow is not null ||
            ViewModel is not { UseStaticGroupedView: true, ShowNewsLine: true })
        {
            return;
        }

        try
        {
            var newsWindow = new StaticNewsWindow { DataContext = DataContext };
            newsWindow.Closed += (_, _) =>
            {
                if (ReferenceEquals(_staticNewsWindow, newsWindow))
                {
                    _staticNewsWindow = null;
                }
            };
            _staticNewsWindow = newsWindow;
            PositionStaticNewsWindow(newsWindow);
            newsWindow.Show();
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            _staticNewsWindow = null;
            ViewModel.EntryMessage = $"Static news window could not open: {exception.Message}";
        }
    }

    private void CloseStaticNewsWindow()
    {
        var newsWindow = _staticNewsWindow;
        _staticNewsWindow = null;
        newsWindow?.Close();
    }

    private void RunSafely(string operation, Action action) =>
        ExceptionSafety.Run(action, exception => ReportRecoverableError(operation, exception));

    private void ReportRecoverableError(string operation, Exception exception) =>
        ViewModel?.ReportRecoverableError(operation, exception);

    private void PositionStaticNewsWindow(StaticNewsWindow newsWindow)
    {
        const int margin = 24;
        var screens = Screens;
        var mainScreen = screens?.ScreenFromWindow(this) ?? screens?.Primary;
        if (screens is null || mainScreen is null)
        {
            return;
        }

        var targetScreen = screens.All.FirstOrDefault(screen => screen != mainScreen) ?? mainScreen;
        var workArea = targetScreen.WorkingArea;
        var width = Math.Min(
            Math.Max(1, workArea.Width - margin * 2),
            (int)Math.Ceiling(newsWindow.Width * targetScreen.Scaling));
        var height = Math.Min(
            Math.Max(1, workArea.Height - margin * 2),
            (int)Math.Ceiling(newsWindow.Height * targetScreen.Scaling));

        if (targetScreen != mainScreen)
        {
            newsWindow.Position = new PixelPoint(workArea.X + margin, workArea.Y + margin);
            return;
        }

        var mainWidth = (int)Math.Ceiling(Bounds.Width * mainScreen.Scaling);
        var mainHeight = (int)Math.Ceiling(Bounds.Height * mainScreen.Scaling);
        var candidates = new[]
        {
            new PixelPoint(Position.X, Position.Y + mainHeight + margin),
            new PixelPoint(Position.X + mainWidth + margin, Position.Y),
            new PixelPoint(Position.X, Position.Y - height - margin),
            new PixelPoint(Position.X - width - margin, Position.Y),
        };
        PixelPoint? position = candidates
            .Where(candidate => Fits(workArea, candidate, width, height))
            .Select(candidate => (PixelPoint?)candidate)
            .FirstOrDefault();
        if (position is null)
        {
            position = new PixelPoint(
                Math.Max(workArea.X + margin, workArea.Right - width - margin),
                Math.Max(workArea.Y + margin, workArea.Bottom - height - margin));
        }

        newsWindow.Position = position.Value;
    }

    private static bool Fits(PixelRect area, PixelPoint position, int width, int height) =>
        position.X >= area.X &&
        position.Y >= area.Y &&
        position.X + width <= area.Right &&
        position.Y + height <= area.Bottom;
}