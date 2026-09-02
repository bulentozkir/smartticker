using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Views;

public partial class StaticNewsWindow : Window
{
    private string? _draggedGroupName;
    private readonly DispatcherTimer _sizeSaveTimer = new(DispatcherPriority.Background)
    {
        Interval = TimeSpan.FromMilliseconds(300),
    };
    private Size? _pendingSize;

    public StaticNewsWindow()
    {
        InitializeComponent();
        WindowReachability.Attach(this);
        _sizeSaveTimer.Tick += (_, _) => RunSafely("Saving News window size", () =>
        {
            _sizeSaveTimer.Stop();
            if (_pendingSize is { } size && ViewModel is { } viewModel)
            {
                _pendingSize = null;
                viewModel.CaptureStaticNewsWindowSize(size.Width, size.Height);
                viewModel.PersistSettings();
            }
        });
        SizeChanged += (_, e) => RunSafely("Resizing News window", () =>
        {
            _pendingSize = e.NewSize;
            _sizeSaveTimer.Stop();
            _sizeSaveTimer.Start();
        });
        Closed += (_, _) => RunSafely("Saving final News window size", () =>
        {
            _sizeSaveTimer.Stop();
            _pendingSize = null;
            ViewModel?.CaptureStaticNewsWindowSize(Bounds.Width, Bounds.Height);
            ViewModel?.PersistSettings();
        });
    }

    private MainViewModel? ViewModel => DataContext as MainViewModel;

    private void OpenStaticNews(object? sender, PointerPressedEventArgs e)
    {
        RunSafely("Opening news source", () =>
        {
            if (sender is Border { DataContext: StaticNewsRow { SourceUri: { } sourceUri } } &&
                e.ClickCount == 2 &&
                e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                e.Handled = true;
                ViewModel?.OpenLinkCommand.Execute(sourceUri);
            }
        });
    }

    private async void BeginGroupDrag(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control { DataContext: StaticNewsGroup group } control &&
            e.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
        {
            _draggedGroupName = group.Name;
            e.Handled = true;
            var data = new DataTransfer();
            data.Add(DataTransferItem.CreateText(group.Name));
            try
            {
                await DragDrop.DoDragDropAsync(e, data, DragDropEffects.Move);
            }
            catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
            {
                ViewModel?.ReportRecoverableError("Reordering News groups", exception);
            }
            finally
            {
                _draggedGroupName = null;
            }
        }
    }

    private void GroupDragOver(object? sender, DragEventArgs e)
    {
        RunSafely("Reordering News groups", () =>
        {
            var canMove = sender is Control { DataContext: StaticNewsGroup group } &&
                _draggedGroupName is not null &&
                !string.Equals(_draggedGroupName, group.Name, StringComparison.OrdinalIgnoreCase);
            e.DragEffects = canMove ? DragDropEffects.Move : DragDropEffects.None;
            e.Handled = true;
        });
    }

    private void GroupDrop(object? sender, DragEventArgs e)
    {
        RunSafely("Reordering News groups", () =>
        {
            if (sender is Control { DataContext: StaticNewsGroup group } control &&
                _draggedGroupName is { } sourceName &&
                !string.Equals(sourceName, group.Name, StringComparison.OrdinalIgnoreCase))
            {
                var placeAfter = e.GetPosition(control).X >= control.Bounds.Width / 2;
                ViewModel?.MoveQuoteGroup(sourceName, group.Name, placeAfter);
                e.DragEffects = DragDropEffects.Move;
            }

            e.Handled = true;
        });
    }

    private void RunSafely(string operation, Action action) =>
        ExceptionSafety.Run(
            action,
            exception => ViewModel?.ReportRecoverableError(operation, exception));
}