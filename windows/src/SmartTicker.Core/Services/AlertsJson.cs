using System.Text.Json;
using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

public static class AlertsJson
{
    public static string Serialize(AlertSettings settings) =>
        JsonSerializer.Serialize(settings.Normalize(), SettingsJson.Options);
}
