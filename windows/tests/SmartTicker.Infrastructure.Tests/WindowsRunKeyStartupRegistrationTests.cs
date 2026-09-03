using System.Runtime.Versioning;
using SmartTicker.Infrastructure.Startup;

namespace SmartTicker.Infrastructure.Tests;

[SupportedOSPlatform("windows")]
public class WindowsRunKeyStartupRegistrationTests
{
    [Fact]
    public void IsDisabledByUser_RecognizesWindowsDisabledState()
    {
        Assert.True(WindowsRunKeyStartupRegistration.IsDisabledByUser([3, 0, 0, 0]));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(new byte[] { })]
    [InlineData(new byte[] { 2, 0, 0, 0 })]
    public void IsDisabledByUser_DoesNotRejectMissingOrEnabledState(byte[]? value)
    {
        Assert.False(WindowsRunKeyStartupRegistration.IsDisabledByUser(value));
    }
}