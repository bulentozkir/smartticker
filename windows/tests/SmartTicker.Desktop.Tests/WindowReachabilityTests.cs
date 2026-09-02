using Avalonia;
using SmartTicker.Desktop.Views;

namespace SmartTicker.Desktop.Tests;

public sealed class WindowReachabilityTests
{
    private static readonly PixelRect Primary = new(0, 0, 1920, 1080);

    [Theory]
    [InlineData(-500, -200, 32, 32)]
    [InlineData(2500, 1400, 1888, 1048)]
    public void ClampTopLeft_KeepsTheDragCornerPositiveAndReachable(
        int x,
        int y,
        int expectedX,
        int expectedY)
    {
        var result = WindowReachability.ClampTopLeft(
            new PixelPoint(x, y),
            [Primary],
            Primary);

        Assert.Equal(new PixelPoint(expectedX, expectedY), result);
    }

    [Fact]
    public void ClampTopLeft_PreservesAReachablePositiveSecondaryScreenPosition()
    {
        var secondary = new PixelRect(1920, 0, 1920, 1080);
        var position = new PixelPoint(2100, 100);

        var result = WindowReachability.ClampTopLeft(position, [Primary, secondary], secondary);

        Assert.Equal(position, result);
    }

    [Fact]
    public void ClampTopLeft_DoesNotPlaceAWindowOnANegativeOnlyScreen()
    {
        var leftScreen = new PixelRect(-1920, 0, 1920, 1080);

        var result = WindowReachability.ClampTopLeft(
            new PixelPoint(-500, 100),
            [leftScreen, Primary],
            leftScreen);

        Assert.Equal(new PixelPoint(32, 100), result);
    }
}