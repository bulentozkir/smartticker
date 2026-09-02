using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Views;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _refreshTimer = new(DispatcherPriority.Background)
    {
        Interval = TimeSpan.FromSeconds(1),
    };
    private readonly DispatcherTimer _windowSizeSaveTimer = new(DispatcherPriority.Background)
    {
        Interval = TimeSpan.FromMilliseconds(300),
    };
    private readonly StaggeredRefreshSchedule _priceRefreshSchedule = new();
    private readonly StaggeredRefreshSchedule _newsRefreshSchedule = new();
    private MainViewModel? _observedViewModel;
    private string? _draggedGroupName;
    private StaticNewsWindow? _staticNewsWindow;
    private bool _staticNewsSyncQueued;
    private bool _newsRefreshesFirst;
    private bool _isClosing;
    private Size? _pendingMainWindowSize;

    public MainWindow()
    {
        InitializeComponent();
        WindowReachability.Attach(this);
        _refreshTimer.Tick += OnRefreshTimerTick;
        _windowSizeSaveTimer.Tick += (_, _) => RunSafely("Saving window size", () =>
        {
            _windowSizeSaveTimer.Stop();
            if (_pendingMainWindowSize is { } size && ViewModel is { } viewModel)
            {
                _pendingMainWindowSize = null;
                viewModel.CaptureMainWindowSize(size.Width, size.Height);
                viewModel.PersistSettings();
            }
        });
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
            WindowReachability.EnsureReachable(this);
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
                _pendingMainWindowSize = e.NewSize;
                _windowSizeSaveTimer.Stop();
                _windowSizeSaveTimer.Start();
            }
        });
        Closing += (_, _) => _isClosing = true;
        Closed += (_, _) =>
        {
            _isClosing = true;
            RunSafely("Closing static News", CloseStaticNewsWindow);
            RunSafely("Releasing pointer capture", ReleasePointerCapture);
            RunSafely("Stopping refresh timers", StopFlowTimers);
            RunSafely("Saving final window size", () =>
            {
                _windowSizeSaveTimer.Stop();
                _pendingMainWindowSize = null;
                ViewModel?.CaptureMainWindowSize(Bounds.Width, Bounds.Height);
                ViewModel?.PersistSettings();
            });
            RunSafely("Closing SmartTicker", () => ViewModel?.Dispose());
        };
    }

    private MainViewModel? ViewModel => DataContext as MainViewModel;

    private void OnRefreshTimerTick(object? sender, EventArgs e) =>
        RunSafely("Scheduling data refresh", () => StartNextRefreshSlots(
            "Automatic price refresh",
            "Automatic News refresh"));

    private void StartNextRefreshSlots(string priceOperation, string newsOperation)
    {
        if (_isClosing || ViewModel is not { IsPaused: false } viewModel)
        {
            return;
        }

        var priceIds = viewModel.Subscriptions
            .Where(subscription => subscription.CollectPrice)
            .Select(subscription => subscription.Id)
            .ToArray();
        var newsIds = viewModel.Subscriptions
            .Where(subscription => subscription.CollectNews)
            .Select(subscription => subscription.Id)
            .ToArray();
        var priceBatch = _priceRefreshSchedule.NextBatch(priceIds, viewModel.PriceRefreshSeconds);
        var newsBatch = _newsRefreshSchedule.NextBatch(newsIds, viewModel.NewsRefreshSeconds);

        if (_newsRefreshesFirst)
        {
            StartNewsBatch(viewModel, newsOperation, newsBatch);
            StartPriceBatch(viewModel, priceOperation, priceBatch);
        }
        else
        {
            StartPriceBatch(viewModel, priceOperation, priceBatch);
            StartNewsBatch(viewModel, newsOperation, newsBatch);
        }

        _newsRefreshesFirst = !_newsRefreshesFirst;
    }

    private static void StartPriceBatch(
        MainViewModel viewModel,
        string operation,
        IReadOnlyCollection<Guid> batch)
    {
        if (batch.Count > 0)
        {
            _ = viewModel.RefreshPriceSubscriptionsSafelyAsync(operation, batch);
        }
    }

    private static void StartNewsBatch(
        MainViewModel viewModel,
        string operation,
        IReadOnlyCollection<Guid> batch)
    {
        if (batch.Count > 0)
        {
            _ = viewModel.RefreshNewsSubscriptionsSafelyAsync(operation, batch);
        }
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

    private void RefreshPrices(object? sender, RoutedEventArgs e) =>
        RunSafely("Starting manual price refresh", () =>
        {
            if (ViewModel is not { IsPaused: false })
            {
                return;
            }

            _refreshTimer.Stop();
            _priceRefreshSchedule.Reset();
            var ids = ViewModel.Subscriptions
                .Where(subscription => subscription.CollectPrice)
                .Select(subscription => subscription.Id)
                .ToArray();
            StartPriceBatch(
                ViewModel,
                "Manual price refresh",
                _priceRefreshSchedule.NextBatch(ids, ViewModel.PriceRefreshSeconds));
            _refreshTimer.Start();
        });

    private void RefreshNews(object? sender, RoutedEventArgs e) =>
        RunSafely("Starting manual News refresh", () =>
        {
            if (ViewModel is not { IsPaused: false } viewModel)
            {
                return;
            }

            _refreshTimer.Stop();
            _newsRefreshSchedule.Reset();
            var ids = viewModel.Subscriptions
                .Where(subscription => subscription.CollectNews)
                .Select(subscription => subscription.Id)
                .ToArray();
            StartNewsBatch(
                viewModel,
                "Manual News refresh",
                _newsRefreshSchedule.NextBatch(ids, viewModel.NewsRefreshSeconds));
            _refreshTimer.Start();
        });

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
        _refreshTimer.Stop();

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
        ResetRefreshSchedules();
        if (!viewModel.IsPaused)
        {
            _refreshTimer.Start();
            StartNextRefreshSlots("Initial price refresh", "Initial News refresh");
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e) =>
        RunSafely("Applying a setting change", () => ApplyViewModelPropertyChange(e));

    private void ApplyViewModelPropertyChange(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.PriceRefreshSeconds))
        {
            _priceRefreshSchedule.Reset();
        }

        if (e.PropertyName == nameof(MainViewModel.NewsRefreshSeconds))
        {
            _newsRefreshSchedule.Reset();
        }

        if (e.PropertyName == nameof(MainViewModel.IsPaused) && ViewModel is { } viewModel)
        {
            if (viewModel.IsPaused)
            {
                _refreshTimer.Stop();
            }
            else if (!_isClosing)
            {
                ResetRefreshSchedules();
                _refreshTimer.Start();
            }
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

    private void ResetRefreshSchedules()
    {
        _priceRefreshSchedule.Reset();
        _newsRefreshSchedule.Reset();
    }

    private void StopFlowTimers()
    {
        _refreshTimer.Stop();
        ResetRefreshSchedules();
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
            if (ViewModel is { } viewModel)
            {
                newsWindow.Width = viewModel.StaticNewsWindowWidth;
                newsWindow.Height = viewModel.StaticNewsWindowHeight;
            }
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