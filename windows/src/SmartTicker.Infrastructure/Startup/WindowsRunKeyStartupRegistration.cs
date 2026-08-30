using System.Runtime.Versioning;
using Microsoft.Win32;
using SmartTicker.Core.Services;

namespace SmartTicker.Infrastructure.Startup;

/// <summary>
/// Per-user Run key. Chosen over a Startup-folder shortcut because it needs no COM interop and is
/// straightforward to read back for the checkbox state.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class WindowsRunKeyStartupRegistration : IStartupRegistration
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "SmartTicker";

    private readonly string? _executablePath;

    public WindowsRunKeyStartupRegistration(string? executablePath = null)
    {
        _executablePath = executablePath ?? Environment.ProcessPath;
    }

    public bool IsSupported => !string.IsNullOrWhiteSpace(_executablePath);

    public bool IsEnabled
    {
        get
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath);
            return key?.GetValue(ValueName) is string value && !string.IsNullOrWhiteSpace(value);
        }
    }

    public void SetEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath, writable: true)
            ?? throw new InvalidOperationException("The Run registry key could not be opened.");

        if (!enabled)
        {
            key.DeleteValue(ValueName, throwOnMissingValue: false);
            return;
        }

        if (!IsSupported)
        {
            throw new InvalidOperationException("The executable path could not be determined.");
        }

        // Quoted so a path containing spaces is still launched as a single argument.
        key.SetValue(ValueName, $"\"{_executablePath}\"", RegistryValueKind.String);
    }
}
