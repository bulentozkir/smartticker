using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SmartTicker.Desktop;

internal static class WindowsProcessEfficiency
{
    // PROCESS_INFORMATION_CLASS.ProcessPowerThrottling
    private const int ProcessPowerThrottling = 4;
    private const uint PowerThrottlingCurrentVersion = 1;
    private const uint PowerThrottlingExecutionSpeed = 0x1;

    public static void ApplyToCurrentProcess()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        using var process = Process.GetCurrentProcess();
        TryApply(
            () => process.PriorityClass = ProcessPriorityClass.Idle,
            "setting Low process priority");
        TryApply(
            () => EnableEfficiencyMode(process.Handle),
            "enabling Efficiency mode");
    }

    private static void TryApply(Action action, string operation)
    {
        try
        {
            action();
        }
        catch (Exception exception) when (ExceptionSafety.IsRecoverable(exception))
        {
            try
            {
                Trace.TraceError($"SmartTicker failed while {operation}: {exception}");
            }
            catch (Exception traceException) when (ExceptionSafety.IsRecoverable(traceException))
            {
            }
        }
    }

    private static void EnableEfficiencyMode(nint processHandle)
    {
        var state = new ProcessPowerThrottlingState
        {
            Version = PowerThrottlingCurrentVersion,
            ControlMask = PowerThrottlingExecutionSpeed,
            StateMask = PowerThrottlingExecutionSpeed,
        };
        if (!SetProcessInformation(
                processHandle,
                ProcessPowerThrottling,
                ref state,
                (uint)Marshal.SizeOf<ProcessPowerThrottlingState>()))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ProcessPowerThrottlingState
    {
        public uint Version;
        public uint ControlMask;
        public uint StateMask;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessInformation(
        nint processHandle,
        int processInformationClass,
        ref ProcessPowerThrottlingState processInformation,
        uint processInformationSize);
}