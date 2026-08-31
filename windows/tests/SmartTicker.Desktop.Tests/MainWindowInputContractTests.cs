using System.Xml.Linq;

namespace SmartTicker.Desktop.Tests;

public sealed class MainWindowInputContractTests
{
    [Fact]
    public void TickerWindow_IsPassiveAndUsesExplicitResizeSurfaces()
    {
        var window = LoadMainWindowXaml().Root!;

        Assert.Equal("True", (string?)window.Attribute("CanResize"));
        Assert.Equal("False", (string?)window.Attribute("ShowActivated"));
        Assert.Equal("None", (string?)window.Attribute("WindowDecorations"));

        var resizeSurfaces = window
            .Descendants()
            .Where(element => (string?)element.Attribute("PointerPressed") == "BeginWindowResize")
            .ToArray();
        Assert.Equal(8, resizeSurfaces.Length);
        Assert.Equal(
            ["East", "North", "NorthEast", "NorthWest", "South", "SouthEast", "SouthWest", "West"],
            resizeSurfaces.Select(element => (string?)element.Attribute("Tag")).Order());
    }

    [Fact]
    public void OnlyExplicitGripStartsWindowDrag()
    {
        var dragSurface = Assert.Single(
            LoadMainWindowXaml().Descendants(),
            element => (string?)element.Attribute("PointerPressed") == "BeginWindowDrag");

        Assert.Equal("SizeAll", (string?)dragSurface.Attribute("Cursor"));

        var code = File.ReadAllText(MainWindowPath("MainWindow.axaml.cs"));
        Assert.DoesNotContain("override void OnPointerPressed", code);
    }

    private static XDocument LoadMainWindowXaml() => XDocument.Load(MainWindowPath("MainWindow.axaml"));

    private static string MainWindowPath(string fileName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null &&
               !Directory.Exists(Path.Combine(directory.FullName, "src", "SmartTicker.Desktop")))
        {
            directory = directory.Parent;
        }

        Assert.NotNull(directory);
        return Path.Combine(directory!.FullName, "src", "SmartTicker.Desktop", "Views", fileName);
    }
}