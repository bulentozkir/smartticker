using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using SmartTicker.Desktop.ViewModels;
using SmartTicker.Infrastructure.Launching;

namespace SmartTicker.Desktop.Views;

internal enum ConfigFileKind
{
    Settings,
    Alerts,
}

internal static class EditConfigFileWorkflow
{
    private static readonly FilePickerFileType JsonFileType = new("SmartTicker settings")
    {
        Patterns = ["*.json"],
        MimeTypes = ["application/json"],
    };

    public static async Task RunAsync(Window owner, MainViewModel viewModel, ConfigFileKind kind)
    {
        // A fresh profile may not have written the file yet, so it is flushed before it can be opened.
        if (kind == ConfigFileKind.Settings)
        {
            viewModel.PersistSettings();
        }
        else
        {
            viewModel.PersistAlerts();
        }

        var path = kind == ConfigFileKind.Settings
            ? viewModel.SettingsStoreLocation
            : viewModel.AlertStoreLocation;
        if (!File.Exists(path))
        {
            viewModel.EntryMessage = $"{path} does not exist yet, so it cannot be edited.";
            return;
        }

        while (true)
        {
            var choice = await AskAsync(owner, kind, path);
            if (choice == EditConfigChoice.Cancel)
            {
                return;
            }

            if (choice == EditConfigChoice.Export)
            {
                if (kind == ConfigFileKind.Settings)
                {
                    await SampleConfigImportWorkflow.ExportSettingsAsync(owner, viewModel);
                }
                else
                {
                    await ExportAlertsAsync(owner, viewModel);
                }

                continue;
            }

            if (!new LocalConfigFileLauncher().TryOpen(path))
            {
                viewModel.EntryMessage =
                    $"{Path.GetFileName(path)} could not be opened. Assign a text editor to .json files and try again.";
            }

            return;
        }
    }

    private static async Task ExportAlertsAsync(Window owner, MainViewModel viewModel)
    {
        try
        {
            var file = await owner.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export existing SmartTicker alert rules",
                SuggestedFileName = "smartticker-alerts.json",
                DefaultExtension = "json",
                ShowOverwritePrompt = true,
                FileTypeChoices = [JsonFileType],
            });

            if (file is null)
            {
                return;
            }

            await using var stream = await file.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(viewModel.ExportAlertsJson());
            viewModel.EntryMessage = $"Existing alert rules exported to {file.Name}.";
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            viewModel.EntryMessage = $"Existing alert rules could not be exported: {exception.Message}";
        }
    }

    private static async Task<EditConfigChoice> AskAsync(Window owner, ConfigFileKind kind, string path)
    {
        var subject = kind == ConfigFileKind.Settings ? "app config" : "alert rules";
        var choice = EditConfigChoice.Cancel;
        var export = new Button { Content = "Export existing config...", Padding = new Thickness(16, 8) };
        var open = new Button { Content = "Open in text editor", Padding = new Thickness(16, 8) };
        var cancel = new Button { Content = "Cancel", Padding = new Thickness(16, 8) };
        var dialog = new Window
        {
            Title = kind == ConfigFileKind.Settings ? "Edit Current App Config" : "Edit Current Alert Rules",
            Width = 700,
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
                        Text = "For advanced users",
                        FontSize = 22,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = Brush.Parse("#E6EDF3"),
                    },
                    new TextBlock
                    {
                        Text = $"Export the existing {subject} first. Editing this file by hand can break it, and there is no undo.",
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = Brush.Parse("#F0A45D"),
                    },
                    new TextBlock
                    {
                        Text = "SmartTicker reloads the file as soon as you save it. If the JSON is malformed or fails schema validation, the change is rejected, the current configuration is kept, and you must import a valid export taken before editing.",
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = Brush.Parse("#C9D1D9"),
                    },
                    new TextBlock
                    {
                        Text = path,
                        TextWrapping = TextWrapping.Wrap,
                        FontSize = 11,
                        Foreground = Brush.Parse("#8B949E"),
                    },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 8,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Children = { cancel, export, open },
                    },
                },
            },
        };

        cancel.Click += (_, _) => dialog.Close();
        export.Click += (_, _) =>
        {
            choice = EditConfigChoice.Export;
            dialog.Close();
        };
        open.Click += (_, _) =>
        {
            choice = EditConfigChoice.Open;
            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return choice;
    }

    private enum EditConfigChoice
    {
        Cancel,
        Export,
        Open,
    }
}
