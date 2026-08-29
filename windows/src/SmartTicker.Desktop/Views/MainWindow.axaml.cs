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
        DataContextChanged += (_, _) => ConfigureFlowTimers();
        // A fresh install shows an empty bar with no obvious next step, so the starter offer comes to the user.
        Opened += (_, _) => Dispatcher.UIThread.Post(() =>
        {
            if (ViewModel is { ShowStarterPrompt: true })
            {
                new SettingsWindow { DataContext = DataContext }.Show(this);
            }
        });
        SizeChanged += (_, e) =>
        {
            if (ViewModel is { } viewModel)
            {
                viewModel.WindowHeight = e.NewSize.Height;
            }
        };
        Closed += (_, _) =>
        {
            StopFlowTimers();
            ViewModel?.Dispose();
        };
    }

    private MainViewModel? ViewModel => DataContext as MainViewModel;

    // Without a title bar the ticker background is the only drag surface; links mark the event handled.
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!e.Handled && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }

    private void ExitApplication(object? sender, RoutedEventArgs e) => Close();

    private void ShowAbout(object? sender, RoutedEventArgs e) => new AboutWindow { DataContext = DataContext }.ShowDialog(this);

    private void OpenSettings(object? sender, RoutedEventArgs e)
    {
        var settings = new SettingsWindow { DataContext = DataContext };
        settings.Show(this);
    }

    private void OpenAppSettings(object? sender, RoutedEventArgs e)
    {
        var settings = new AppSettingsWindow { DataContext = DataContext };
        settings.Show(this);
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
}