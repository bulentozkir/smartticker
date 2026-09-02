using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SmartTicker.Desktop;

internal static class ExceptionSafety
{
    public static bool IsRecoverable(Exception exception) => exception is not
        (OutOfMemoryException or StackOverflowException or AccessViolationException or SEHException);

    public static void Run(Action action, Action<Exception>? report = null)
    {
        try
        {
            action();
        }
        catch (Exception exception) when (IsRecoverable(exception))
        {
            Report(exception, report);
        }
    }

    public static async Task RunAsync(Func<Task> action, Action<Exception>? report = null)
    {
        try
        {
            await action();
        }
        catch (Exception exception) when (IsRecoverable(exception))
        {
            Report(exception, report);
        }
    }

    private static void Report(Exception exception, Action<Exception>? report)
    {
        try
        {
            report?.Invoke(exception);
        }
        catch (Exception reportingException) when (IsRecoverable(reportingException))
        {
            TraceSafely(reportingException);
        }

        TraceSafely(exception);
    }

    private static void TraceSafely(Exception exception)
    {
        try
        {
            Trace.TraceError(exception.ToString());
        }
        catch (Exception traceException) when (IsRecoverable(traceException))
        {
        }
    }
}