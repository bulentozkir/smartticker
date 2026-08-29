using System.Text.Json;
using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

public static class SettingsJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
    };

    public static string Serialize(SmartTickerSettings settings) =>
        JsonSerializer.Serialize(settings.Normalize(), Options);
}
