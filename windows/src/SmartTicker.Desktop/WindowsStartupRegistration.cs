#if WINDOWS
using System;
using System.Runtime.InteropServices;
using SmartTicker.Core.Services;
using SmartTicker.Infrastructure.Startup;
using Windows.ApplicationModel;

namespace SmartTicker.Desktop;

/// <summary>
/// Uses the package startup-task extension for MSIX installs and the per-user Run key for
/// unpackaged installs. Registry writes made by a packaged app can be virtualized and therefore
/// cannot reliably register a Windows startup app.
/// </summary>
internal sealed class WindowsStartupRegistration : IStartupRegistration
{
    private const string TaskId = "SmartTickerStartupTask";

    private readonly StartupTask? _startupTask;
    private readonly IStartupRegistration? _unpackagedRegistration;

    private WindowsStartupRegistration(StartupTask startupTask)
    {
        _startupTask = startupTask;
        MigrateLegacyRunKeyRegistration();
    }

    private WindowsStartupRegistration(IStartupRegistration unpackagedRegistration)
    {
        _unpackagedRegistration = unpackagedRegistration;
    }

    public bool IsSupported => _startupTask is not null || _unpackagedRegistration?.IsSupported == true;

    public bool IsEnabled => _startupTask is not null
        ? IsEnabledState(_startupTask.State)
        : _unpackagedRegistration?.IsEnabled == true;

    public static IStartupRegistration Create()
    {
        if (!HasPackageIdentity())
        {
            return new WindowsStartupRegistration(new WindowsRunKeyStartupRegistration());
        }

        try
        {
            var startupTask = StartupTask.GetAsync(TaskId).AsTask().GetAwaiter().GetResult();
            return new WindowsStartupRegistration(startupTask);
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            return new UnsupportedPackagedStartupRegistration(exception.Message);
        }
    }

    public void SetEnabled(bool enabled)
    {
        if (_startupTask is null)
        {
            _unpackagedRegistration?.SetEnabled(enabled);
            return;
        }

        if (!enabled)
        {
            _startupTask.Disable();
            return;
        }

        EnablePackagedTask(_startupTask);
    }

    private static bool HasPackageIdentity()
    {
        try
        {
            _ = Package.Current.Id.Name;
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (COMException)
        {
            return false;
        }
    }

    private static bool IsEnabledState(StartupTaskState state) =>
        state is StartupTaskState.Enabled or StartupTaskState.EnabledByPolicy;

    private static void EnablePackagedTask(StartupTask startupTask)
    {
        if (IsEnabledState(startupTask.State))
        {
            return;
        }

        if (startupTask.State == StartupTaskState.DisabledByUser)
        {
            throw new InvalidOperationException(
                "Windows disabled SmartTicker in Startup apps. Re-enable it in Settings > Apps > Startup.");
        }

        if (startupTask.State == StartupTaskState.DisabledByPolicy)
        {
            throw new InvalidOperationException("Windows policy prevents SmartTicker from starting at sign-in.");
        }

        var state = startupTask.RequestEnableAsync().AsTask().GetAwaiter().GetResult();
        if (!IsEnabledState(state))
        {
            throw new InvalidOperationException("Windows did not enable SmartTicker in Startup apps.");
        }
    }

    private void MigrateLegacyRunKeyRegistration()
    {
        var legacyRegistration = new WindowsRunKeyStartupRegistration();
        if (!legacyRegistration.IsEnabled || _startupTask is null)
        {
            return;
        }

        if (_startupTask.State == StartupTaskState.Disabled)
        {
            EnablePackagedTask(_startupTask);
        }

        if (IsEnabledState(_startupTask.State))
        {
            legacyRegistration.SetEnabled(false);
        }
    }

    private sealed class UnsupportedPackagedStartupRegistration(string reason) : IStartupRegistration
    {
        public bool IsSupported => false;

        public bool IsEnabled => false;

        public void SetEnabled(bool enabled) => throw new InvalidOperationException(
            $"This SmartTicker package has no usable startup task: {reason}");
    }
}
#endif