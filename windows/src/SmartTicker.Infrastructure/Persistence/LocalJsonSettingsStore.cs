using System.Text.Json;
using SmartTicker.Core.Models;
using SmartTicker.Core.Services;

namespace SmartTicker.Infrastructure.Persistence;

public sealed class LocalJsonSettingsStore : ISettingsStore
{
    public LocalJsonSettingsStore(string? filePath = null)
    {
        FilePath = filePath ?? LocalDataPath.For("settings.json");
    }

    public string FilePath { get; }

    public SmartTickerSettings Load()
    {
        if (!File.Exists(FilePath))
        {
            return SmartTickerSettings.Default;
        }

        using var stream = File.OpenRead(FilePath);
        return (JsonSerializer.Deserialize<SmartTickerSettings>(stream, SettingsJson.Options)
            ?? SmartTickerSettings.Default).Normalize();
    }

    public void Save(SmartTickerSettings settings)
    {
        var directory = Path.GetDirectoryName(FilePath)
            ?? throw new InvalidOperationException("The settings path does not have a parent directory.");
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