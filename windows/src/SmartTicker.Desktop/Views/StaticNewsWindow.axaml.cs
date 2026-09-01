using System;
using Avalonia.Controls;
using Avalonia.Input;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Views;

public partial class StaticNewsWindow : Window
{
    private string? _draggedGroupName;

    public StaticNewsWindow()
    {
        InitializeComponent();
    }

    private MainViewModel? ViewModel => DataContext as MainViewModel;

    private void OpenStaticNews(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border { DataContext: StaticNewsRow { SourceUri: { } sourceUri } } &&
            e.ClickCount == 2 &&
            e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            e.Handled = true;
            ViewModel?.OpenLinkCommand.Execute(sourceUri);
        }
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
            finally
            {
                _draggedGroupName = null;
            }
        }
    }

    private void GroupDragOver(object? sender, DragEventArgs e)
    {
        var canMove = sender is Control { DataContext: StaticNewsGroup group } &&
            _draggedGroupName is not null &&
            !string.Equals(_draggedGroupName, group.Name, StringComparison.OrdinalIgnoreCase);
        e.DragEffects = canMove ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }

    private void GroupDrop(object? sender, DragEventArgs e)
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
    }
}