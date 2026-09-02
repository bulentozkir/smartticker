using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Views;

internal static class SampleConfigImportWorkflow
{
    private static readonly FilePickerFileType JsonFileType = new("SmartTicker settings")
    {
        Patterns = ["*.json"],
        MimeTypes = ["application/json"],
    };

    public static async Task RunAsync(Window owner, MainViewModel viewModel)
    {
        while (true)
        {
            var choice = await AskAsync(owner, viewModel.StarterSourceUri);
            if (choice == SampleConfigImportChoice.Cancel)
            {
                return;
            }

            if (choice == SampleConfigImportChoice.ExportExisting)
            {
                await ExportSettingsAsync(owner, viewModel);
                continue;
            }

            await viewModel.LoadStarterQuotesCommand.ExecuteAsync(null);
            return;
        }
    }

    public static async Task<bool> ExportSettingsAsync(Window owner, MainViewModel viewModel)
    {
        try
        {
            var file = await owner.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export existing SmartTicker config",
                SuggestedFileName = "smartticker-settings.json",
                DefaultExtension = "json",
                ShowOverwritePrompt = true,
                FileTypeChoices = [JsonFileType],
            });

            if (file is null)
            {
                return false;
            }

            await using var stream = await file.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(viewModel.ExportSettingsJson());
            viewModel.EntryMessage = $"Existing config exported to {file.Name}.";
            return true;
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            viewModel.EntryMessage = $"Existing config could not be exported: {exception.Message}";
            return false;
        }
    }

    private static async Task<SampleConfigImportChoice> AskAsync(Window owner, string sourceUri)
    {
        var choice = SampleConfigImportChoice.Cancel;
        var export = new Button { Content = "Export existing config...", Padding = new Thickness(16, 8) };
        var import = new Button { Content = "Import Sample Quotes Config", Padding = new Thickness(16, 8) };
        var cancel = new Button { Content = "Cancel", Padding = new Thickness(16, 8) };
        var dialog = new Window
        {
            Title = "Replace SmartTicker config?",
            Width = 680,
            SizeToContent = SizeToContent.Height,
            CanResize = false,
            Topmost = true,
            Background = Brush.Parse("#0D1117"),
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Thickness(24),
                Spacing = 14,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Are you sure?",
                        FontSize = 22,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = Brush.Parse("#E6EDF3"),
                    },
                    new TextBlock
                    {
                        Text = "This downloads the published sample config from the internet and replaces your existing quotes, quote groups, source approvals, view, appearance, and other app settings. Alert rules are stored separately and will not be deleted.",
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = Brush.Parse("#F0A45D"),
                    },
                    new TextBlock
                    {
                        Text = "Exporting first is optional. It saves your current config to a local file so you can import it again later.",
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = Brush.Parse("#C9D1D9"),
                    },
                    new TextBlock
                    {
                        Text = sourceUri,
                        TextWrapping = TextWrapping.Wrap,
                        FontSize = 11,
                        Foreground = Brush.Parse("#8B949E"),
                    },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 8,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Children = { cancel, export, import },
                    },
                },
            },
        };
        WindowReachability.Attach(dialog);

        cancel.Click += (_, _) => ExceptionSafety.Run(dialog.Close);
        export.Click += (_, _) =>
        {
            choice = SampleConfigImportChoice.ExportExisting;
            ExceptionSafety.Run(dialog.Close);
        };
        import.Click += (_, _) =>
        {
            choice = SampleConfigImportChoice.ImportSample;
            ExceptionSafety.Run(dialog.Close);
        };

        await dialog.ShowDialog(owner);
        return choice;
    }

    private enum SampleConfigImportChoice
    {
        Cancel,
        ExportExisting,
        ImportSample,
    }
}