namespace SmartTicker.Core.Services;

/// <summary>Registers the app to launch when the user logs in. The OS is the source of truth.</summary>
public interface IStartupRegistration
{
    bool IsSupported { get; }

    bool IsEnabled { get; }

    void SetEnabled(bool enabled);
}
