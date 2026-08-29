using System.ComponentModel;
using System.Diagnostics;
using SmartTicker.Core.Services;

namespace SmartTicker.Infrastructure.Launching;

public sealed class DefaultBrowserLinkLauncher : ILinkLauncher
{
    public bool TryOpen(Uri uri)
    {
        // Ticker links can originate from remote pages, so only web schemes may reach the shell.
        if (!uri.IsAbsoluteUri ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return false;
        }

        try
        {
            using var process = Process.Start(new ProcessStartInfo(uri.AbsoluteUri)
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
