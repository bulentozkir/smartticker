using SmartTicker.Core.Services;

namespace SmartTicker.Infrastructure.Startup;

public static class StartupRegistrationFactory
{
    public static IStartupRegistration Create() =>
        OperatingSystem.IsWindows() ? new WindowsRunKeyStartupRegistration()
        : OperatingSystem.IsLinux() ? new XdgAutostartRegistration()
        : new UnsupportedStartupRegistration();
}

/// <summary>Keeps the option visible but inert on platforms with no autostart mechanism wired up.</summary>
public sealed class UnsupportedStartupRegistration : IStartupRegistration
{
    public bool IsSupported => false;

    public bool IsEnabled => false;

    public void SetEnabled(bool enabled)
    {
    }
}
