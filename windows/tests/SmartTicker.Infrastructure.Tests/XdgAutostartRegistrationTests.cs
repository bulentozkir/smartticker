using SmartTicker.Infrastructure.Startup;

namespace SmartTicker.Infrastructure.Tests;

public class XdgAutostartRegistrationTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(), "smartticker-autostart-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }

        GC.SuppressFinalize(this);
    }

    private XdgAutostartRegistration CreateRegistration(string executable = "/opt/smartticker/SmartTicker.Desktop") =>
        new(_directory, executable);

    [Fact]
    public void IsEnabled_IsFalseBeforeAnythingIsWritten()
    {
        Assert.False(CreateRegistration().IsEnabled);
    }

    [Fact]
    public void SetEnabled_CreatesTheDesktopEntryAndTheDirectory()
    {
        var registration = CreateRegistration();

        registration.SetEnabled(true);

        Assert.True(registration.IsEnabled);
        Assert.True(File.Exists(registration.FilePath));
        Assert.EndsWith("smartticker.desktop", registration.FilePath);
    }

    [Fact]
    public void SetEnabled_WritesAValidDesktopEntry()
    {
        var registration = CreateRegistration();

        registration.SetEnabled(true);

        var lines = File.ReadAllLines(registration.FilePath);
        Assert.Equal("[Desktop Entry]", lines[0]);
        Assert.Contains("Type=Application", lines);
        Assert.Contains("Name=SmartTicker", lines);
        Assert.Contains("Terminal=false", lines);
        Assert.Contains("X-GNOME-Autostart-enabled=true", lines);
    }

    [Fact]
    public void SetEnabled_QuotesTheExecutableSoSpacedPathsSurvive()
    {
        var registration = CreateRegistration("/home/me/My Apps/SmartTicker.Desktop");

        registration.SetEnabled(true);

        Assert.Contains("Exec=\"/home/me/My Apps/SmartTicker.Desktop\"", File.ReadAllLines(registration.FilePath));
    }

    [Fact]
    public void SetEnabled_False_RemovesTheEntry()
    {
        var registration = CreateRegistration();
        registration.SetEnabled(true);

        registration.SetEnabled(false);

        Assert.False(registration.IsEnabled);
        Assert.False(File.Exists(registration.FilePath));
    }

    [Fact]
    public void SetEnabled_False_IsHarmlessWhenNothingIsRegistered()
    {
        var registration = CreateRegistration();

        registration.SetEnabled(false);

        Assert.False(registration.IsEnabled);
    }

    [Fact]
    public void SetEnabled_True_IsIdempotent()
    {
        var registration = CreateRegistration();

        registration.SetEnabled(true);
        registration.SetEnabled(true);

        Assert.True(registration.IsEnabled);
        Assert.Single(Directory.GetFiles(_directory));
    }

    [Fact]
    public void IsSupported_IsFalseWithoutAnExecutablePath()
    {
        Assert.False(new XdgAutostartRegistration(_directory, string.Empty).IsSupported);
    }
}
