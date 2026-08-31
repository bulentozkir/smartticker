using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace SmartTicker.Desktop.Views;

[SupportedOSPlatform("windows")]
internal static class WindowsPassiveWindow
{
    private const int ExtendedStyleIndex = -20;
    private static readonly nint NoActivateStyle = 0x08000000;

    public static void MakeNonActivating(nint windowHandle)
    {
        var styles = GetWindowLongPtr(windowHandle, ExtendedStyleIndex);
        _ = SetWindowLongPtr(windowHandle, ExtendedStyleIndex, styles | NoActivateStyle);
    }

    public static void ReleasePointerCapture() => _ = ReleaseCapture();

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
    private static extern nint GetWindowLongPtr(nint windowHandle, int index);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    private static extern nint SetWindowLongPtr(nint windowHandle, int index, nint newValue);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ReleaseCapture();
}