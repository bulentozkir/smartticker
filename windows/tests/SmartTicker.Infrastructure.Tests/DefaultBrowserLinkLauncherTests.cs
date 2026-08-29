using SmartTicker.Infrastructure.Launching;

namespace SmartTicker.Infrastructure.Tests;

public sealed class DefaultBrowserLinkLauncherTests
{
    [Theory]
    [InlineData("file:///C:/Windows/System32/cmd.exe")]
    [InlineData("ms-settings:privacy")]
    [InlineData("ftp://example.com/payload")]
    public void TryOpen_RefusesNonWebSchemes(string url)
    {
        var launched = new DefaultBrowserLinkLauncher().TryOpen(new Uri(url));

        Assert.False(launched);
    }
}
