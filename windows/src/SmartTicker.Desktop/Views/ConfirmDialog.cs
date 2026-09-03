using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;

namespace SmartTicker.Desktop.Views;

/// <summary>Avalonia has no built-in message box, so the confirmation is a small owned window.</summary>
internal static class ConfirmDialog
{
    public static async Task<bool> ShowAsync(
        Window owner,
        string title,
        string message,
        string confirmText,
        string cancelText = "Keep them")
    {
        var result = false;
        var confirm = new Button { Content = confirmText, Padding = new Avalonia.Thickness(18, 8) };
        var cancel = new Button { Content = cancelText, Padding = new Avalonia.Thickness(18, 8) };
        var dialog = new Window
        {
            Title = title,
            Width = 460,
            SizeToContent = SizeToContent.Height,
            CanResize = false,
            Topmost = true,
            ShowInTaskbar = false,
            Background = Avalonia.Media.Brush.Parse("#0D1117"),
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(24),
                Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = message,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        Foreground = Avalonia.Media.Brush.Parse("#E6EDF3"),
                    },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 10,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Children = { cancel, confirm },
                    },
                },
            },
        };
        WindowReachability.Attach(dialog);
        WindowReachability.KeepDialogInFront(dialog);

        confirm.Click += (_, _) =>
        {
            result = true;
            ExceptionSafety.Run(dialog.Close);
        };
        cancel.Click += (_, _) => ExceptionSafety.Run(dialog.Close);

        await dialog.ShowDialog(owner);
        return result;
    }
}
