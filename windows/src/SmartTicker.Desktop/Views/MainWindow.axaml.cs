using System;
using System.ComponentModel;
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

    public MainWindow()
    {
        InitializeComponent();
        _priceRefreshTimer.Tick += async (_, _) =>
        {
            if (ViewModel is { IsPaused: false } viewModel)
            {
                await viewModel.RefreshPricesAsync();
            }
        };
        _newsRefreshTimer.Tick += async (_, _) =>
        {
            if (ViewModel is { IsPaused: false } viewModel)
            {
                await viewModel.RefreshNewsAsync();
            }
        };
        DataContextChanged += (_, _) =>
        {
            ConfigureFlowTimers();
            if (IsVisible)
            {
                SyncStaticNewsWindow();
            }
        };
        Opened += (_, _) =>
        {
            ConfigurePassiveWindow();
            SyncStaticNewsWindow();
            // A fresh install shows an empty bar with no obvious next step, so the starter offer comes to the user.
            Dispatcher.UIThread.Post(() =>
            {
                if (ViewModel is { ShowStarterPrompt: true })
                {
                    new SettingsWindow { DataContext = DataContext }.Show(this);
                }
            });
        };
        Deactivated += (_, _) => ReleasePointerCapture();
        PointerReleased += (_, _) => ReleasePointerCapture();
        SizeChanged += (_, e) =>
        {
            if (ViewModel is { } viewModel)
            {
                viewModel.WindowHeight = e.NewSize.Height;
            }
        };
        Closed += (_, _) =>
        {
            CloseStaticNewsWindow();
            ReleasePointerCapture();
            StopFlowTimers();
            ViewModel?.Dispose();
        };
    }

    private MainViewModel? ViewModel => DataContext as MainViewModel;

    private void BeginWindowDrag(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            e.Handled = true;
            ReleasePointerCapture();
            BeginMoveDrag(e);
        }
    }

    private void BeginWindowResize(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control { Tag: string edgeName } &&
            e.GetCurrentPoint(this).Properties.IsLeftButtonPressed &&
            Enum.TryParse<WindowEdge>(edgeName, out var edge))
        {
            e.Handled = true;
            ReleasePointerCapture();
            BeginResizeDrag(edge, e);
        }
    }

    private void OpenStaticQuote(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border { DataContext: StaticQuoteRow row } &&
            e.ClickCount == 2 &&
            e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            e.Handled = true;
            ViewModel?.OpenLinkCommand.Execute(row.SourceUri);
        }
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
            finally
            {
                _draggedGroupName = null;
            }
        }
    }

    private void GroupDragOver(object? sender, DragEventArgs e)
    {
        var canMove = sender is Control control &&
            _draggedGroupName is not null &&
            TryGetGroupName(control.DataContext, out var targetName) &&
            !string.Equals(_draggedGroupName, targetName, StringComparison.OrdinalIgnoreCase);
        e.DragEffects = canMove ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }

    private void GroupDrop(object? sender, DragEventArgs e)
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

    private void ExitApplication(object? sender, RoutedEventArgs e) => Close();

    private void ShowHelp(object? sender, RoutedEventArgs e) => HelpWindow.Open(this);

    private void ShowAbout(object? sender, RoutedEventArgs e) => new AboutWindow { DataContext = DataContext }.ShowDialog(this);

    private void OpenSettings(object? sender, RoutedEventArgs e)
    {
        var settings = new SettingsWindow { DataContext = DataContext };
        settings.Show(this);
    }

    private void OpenQuoteGroups(object? sender, RoutedEventArgs e) => QuoteGroupsWindow.Open(this, DataContext);

    private void OpenStaticNewsWindow(object? sender, RoutedEventArgs e) => ShowStaticNewsWindow();

    private void OpenAppSettings(object? sender, RoutedEventArgs e)
    {
        var settings = new AppSettingsWindow { DataContext = DataContext };
        settings.Show(this);
    }

    private void OpenAlerts(object? sender, RoutedEventArgs e)
    {
        var alerts = new AlertsWindow { DataContext = DataContext };
        alerts.Show(this);
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
        _ = viewModel.RefreshPricesAsync();
        _ = viewModel.RefreshNewsAsync();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MainViewModel.PriceRefreshSeconds) or nameof(MainViewModel.NewsRefreshSeconds))
        {
            ApplyRefreshIntervals();
        }

        if (e.PropertyName is nameof(MainViewModel.UseStaticGroupedView) or nameof(MainViewModel.ShowNewsLine))
        {
            SyncStaticNewsWindow();
        }
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
            newsWindow.Show();
        }
        catch (Exception exception)
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
}