using System.Text.Json;
using SmartTicker.Core.Models;
using SmartTicker.Core.Services;

namespace SmartTicker.Infrastructure.Persistence;

public sealed class LocalJsonAlertStore : IAlertStore
{
    public LocalJsonAlertStore(string? filePath = null)
    {
        FilePath = filePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SmartTicker",
            "alerts.json");
    }

    public string FilePath { get; }

    public AlertSettings Load()
    {
        if (!File.Exists(FilePath))
        {
            return AlertSettings.Default;
        }

        try
        {
            using var stream = File.OpenRead(FilePath);
            return (JsonSerializer.Deserialize<AlertSettings>(stream, SettingsJson.Options)
                ?? AlertSettings.Default).Normalize();
        }
        catch (JsonException)
        {
            // A corrupt alert file must not stop the ticker from starting.
            return AlertSettings.Default;
        }
    }

    public void Save(AlertSettings settings)
    {
        var directory = Path.GetDirectoryName(FilePath)
            ?? throw new InvalidOperationException("The alert path does not have a parent directory.");
        Directory.CreateDirectory(directory);

        var temporaryPath = FilePath + ".tmp";
        try
        {
            using (var stream = new FileStream(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                JsonSerializer.Serialize(stream, settings.Normalize(), SettingsJson.Options);
                stream.Flush(true);
            }

            File.Move(temporaryPath, FilePath, true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }
}
