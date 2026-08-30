using SmartTicker.Core.Services;

namespace SmartTicker.Infrastructure.Startup;

/// <summary>
/// Freedesktop autostart: a .desktop file in the autostart directory is launched at login by GNOME,
/// KDE, XFCE and other compliant desktops.
/// </summary>
public sealed class XdgAutostartRegistration : IStartupRegistration
{
    private const string EntryFileName = "smartticker.desktop";

    private readonly string _autostartDirectory;
    private readonly string? _executablePath;

    public XdgAutostartRegistration(string? autostartDirectory = null, string? executablePath = null)
    {
        _autostartDirectory = autostartDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "autostart");
        _executablePath = executablePath ?? Environment.ProcessPath;
    }

    public string FilePath => Path.Combine(_autostartDirectory, EntryFileName);

    public bool IsSupported => !string.IsNullOrWhiteSpace(_executablePath);

    public bool IsEnabled => File.Exists(FilePath);

    public void SetEnabled(bool enabled)
    {
        if (!enabled)
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }

            return;
        }

        if (!IsSupported)
        {
            throw new InvalidOperationException("The executable path could not be determined.");
        }

        Directory.CreateDirectory(_autostartDirectory);
        var entry = string.Join(
            '\n',
            "[Desktop Entry]",
            "Type=Application",
            "Name=SmartTicker",
            "Comment=Desktop price and news ticker",
            $"Exec=\"{_executablePath}\"",
            "Icon=smartticker",
            "Terminal=false",
            "X-GNOME-Autostart-enabled=true",
            string.Empty);
        File.WriteAllText(FilePath, entry);
    }
}
