using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Views;

public partial class AppSettingsWindow : Window
{
    // Settings files are a few kilobytes; anything larger is not one and should not be read into memory.
    private const long MaximumImportBytes = 1024 * 1024;

    private static readonly FilePickerFileType JsonFileType = new("SmartTicker settings")
    {
        Patterns = ["*.json"],
        MimeTypes = ["application/json"],
    };

    private bool _sourceValidationRunning;

    public AppSettingsWindow()
    {
        InitializeComponent();
    }

    private void ShowHelp(object? sender, RoutedEventArgs e) => HelpWindow.Open(this);

    private void OpenQuoteGroups(object? sender, RoutedEventArgs e) => QuoteGroupsWindow.Open(this, DataContext);

    private async void ValidateAllSources(object? sender, RoutedEventArgs e)
    {
        if (_sourceValidationRunning || DataContext is not MainViewModel viewModel)
        {
            return;
        }

        _sourceValidationRunning = true;
        var button = sender as Button;
        if (button is not null)
        {
            button.IsEnabled = false;
        }

        try
        {
            var reviews = viewModel.GetPendingSourcePermissionReviews();
            for (var index = 0; index < reviews.Count; index++)
            {
                var review = reviews[index];
                var dialog = new SourcePermissionWindow
                {
                    DataContext = review,
                    Title = $"Review source {index + 1} of {reviews.Count}",
                };
                var decision = await dialog.ShowDialog<SourcePermissionDecision>(this);
                if (decision == SourcePermissionDecision.Cancel)
                {
                    viewModel.SourceValidationStatus = "Source validation cancelled.";
                    return;
                }

                if (decision == SourcePermissionDecision.Approve)
                {
                    viewModel.ApproveSourcePermission(review);
                }
            }

            await viewModel.ValidateAllSourcesAsync();
        }
        finally
        {
            if (button is not null)
            {
                button.IsEnabled = true;
            }

            _sourceValidationRunning = false;
        }
    }

    private async void ExportSettings(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        try
        {
            var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export SmartTicker settings",
                SuggestedFileName = "smartticker-settings.json",
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
            await writer.WriteAsync(viewModel.ExportSettingsJson());
            viewModel.EntryMessage = $"Settings exported to {file.Name}.";
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            viewModel.EntryMessage = $"Settings could not be exported: {exception.Message}";
        }
    }

    private async void ImportSettings(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        try
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Import SmartTicker settings",
                AllowMultiple = false,
                FileTypeFilter = [JsonFileType],
            });

            if (files.Count == 0)
            {
                return;
            }

            var file = files[0];
            var properties = await file.GetBasicPropertiesAsync();
            if (properties.Size > MaximumImportBytes)
            {
                viewModel.ReportImportFailure(
                    file.Name,
                    [$"The file is {properties.Size:N0} bytes. A SmartTicker settings file is only a few kilobytes, so this one was not read."]);
                return;
            }

            string json;
            await using (var stream = await file.OpenReadAsync())
            using (var reader = new StreamReader(stream))
            {
                json = await reader.ReadToEndAsync();
            }

            var result = viewModel.ImportSettingsJson(json);
            if (!result.Success)
            {
                viewModel.ReportImportFailure(file.Name, result.Errors);
                return;
            }

            viewModel.ReportImportSuccess(file.Name, result.Settings!.Subscriptions.Length);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            viewModel.ReportImportFailure("the selected file", [exception.Message]);
        }
    }

    private async void ExportAlerts(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        try
        {
            var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export SmartTicker alert rules",
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
            viewModel.EntryMessage = $"Alert rules exported to {file.Name}.";
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            viewModel.EntryMessage = $"Alert rules could not be exported: {exception.Message}";
        }
    }

    private async void ImportAlerts(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        try
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Import SmartTicker alert rules",
                AllowMultiple = false,
                FileTypeFilter = [JsonFileType],
            });

            if (files.Count == 0)
            {
                return;
            }

            var file = files[0];
            var properties = await file.GetBasicPropertiesAsync();
            if (properties.Size > MaximumImportBytes)
            {
                viewModel.ReportImportFailure(
                    file.Name,
                    [$"The file is {properties.Size:N0} bytes. A SmartTicker alerts file is only a few kilobytes, so this one was not read."]);
                return;
            }

            string json;
            await using (var stream = await file.OpenReadAsync())
            using (var reader = new StreamReader(stream))
            {
                json = await reader.ReadToEndAsync();
            }

            var result = viewModel.ImportAlertsJson(json);
            if (!result.Success)
            {
                viewModel.ReportImportFailure(file.Name, result.Errors);
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            viewModel.ReportImportFailure("the selected file", [exception.Message]);
        }
    }
}
