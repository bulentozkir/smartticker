namespace SmartTicker.Desktop.Tests;

public sealed class WindowsProcessEfficiencyTests
{
    [Fact]
    public void Startup_AppliesLowPriorityAndEfficiencyModeBeforeAvalonia()
    {
        var program = ReadDesktopSource("Program.cs");
        var policy = ReadDesktopSource("WindowsProcessEfficiency.cs");

        var policyCall = program.IndexOf(
            "WindowsProcessEfficiency.ApplyToCurrentProcess();",
            StringComparison.Ordinal);
        var avaloniaStart = program.IndexOf(
            "BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);",
            StringComparison.Ordinal);

        Assert.True(policyCall >= 0 && policyCall < avaloniaStart);
        Assert.Contains("OperatingSystem.IsWindows()", policy);
        Assert.Contains("ProcessPriorityClass.Idle", policy);
        Assert.Contains("ProcessPowerThrottling = 4", policy);
        Assert.Contains("PowerThrottlingExecutionSpeed = 0x1", policy);
        Assert.Contains("ControlMask = PowerThrottlingExecutionSpeed", policy);
        Assert.Contains("StateMask = PowerThrottlingExecutionSpeed", policy);
        Assert.Contains("SetProcessInformation(", policy);
        Assert.Contains("ExceptionSafety.IsRecoverable", policy);
    }

    private static string ReadDesktopSource(string fileName) => File.ReadAllText(Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "src", "SmartTicker.Desktop", fileName)));
}