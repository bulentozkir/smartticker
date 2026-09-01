using System.ComponentModel;
using System.Diagnostics;

namespace SmartTicker.Infrastructure.Launching;

/// <summary>Opens SmartTicker's own JSON config files; ticker content can never supply the path.</summary>
public sealed class LocalConfigFileLauncher
{
    public bool TryOpen(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) ||
            !string.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase) ||
            !File.Exists(filePath))
        {
            return false;
        }

        try
        {
            using var process = Process.Start(new ProcessStartInfo(filePath)
            {
                UseShellExecute = true,
            });
            return true;
        }
        catch (Exception exception)
            when (exception is Win32Exception or InvalidOperationException or PlatformNotSupportedException)
        {
            return false;
        }
    }
}
