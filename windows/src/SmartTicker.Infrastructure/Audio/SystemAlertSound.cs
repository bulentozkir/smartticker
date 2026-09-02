using SmartTicker.Core.Services;

namespace SmartTicker.Infrastructure.Audio;

public sealed class SystemAlertSound : IAlertSound
{
    private const int ToneHertz = 880;
    private const int ToneMilliseconds = 180;
    private const int GapMilliseconds = 120;

    public void Buzz(int times)
    {
        if (times < 1)
        {
            return;
        }

        // Console.Beep blocks for the tone duration, so the buzz runs off the UI thread.
        _ = Task.Run(() =>
        {
            try
            {
                for (var index = 0; index < times; index++)
                {
                    if (index > 0)
                    {
                        Thread.Sleep(GapMilliseconds);
                    }

                    PlayOnce();
                }
            }
            catch (Exception exception) when (exception is not
                (OutOfMemoryException or StackOverflowException or AccessViolationException or System.Runtime.InteropServices.SEHException))
            {
                System.Diagnostics.Trace.TraceError(exception.ToString());
            }
        });
    }

    private static void PlayOnce()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                Console.Beep(ToneHertz, ToneMilliseconds);
            }
            else
            {
                // The terminal bell is the only dependency-free option on Linux and macOS.
                Console.Out.Write('\a');
                Console.Out.Flush();
            }
        }
        catch (Exception exception) when (exception is PlatformNotSupportedException or IOException)
        {
        }
    }
}
