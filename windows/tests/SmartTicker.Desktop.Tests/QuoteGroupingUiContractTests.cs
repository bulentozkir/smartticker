using System.Xml.Linq;
using SmartTicker.Desktop.Controls;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Tests;

public sealed class QuoteGroupingUiContractTests
{
    [Fact]
    public void QuoteEditor_ExposesEditableGroupLookupAndManager()
    {
        var document = LoadView("SettingsWindow.axaml");

        var groupLookup = Assert.Single(document.Descendants(), element =>
            element.Name.LocalName == "ComboBox" &&
            (string?)element.Attribute("Text") == "{Binding NewGroupName, Mode=TwoWay}");
        Assert.Equal("{Binding GroupNameOptions}", (string?)groupLookup.Attribute("ItemsSource"));
        Assert.Equal("True", (string?)groupLookup.Attribute("IsEditable"));
        Assert.Contains(document.Descendants(), element =>
            (string?)element.Attribute("Click") == "OpenQuoteGroups");
        Assert.Contains(document.Descendants(), element =>
            (string?)element.Attribute("Text") == "{Binding GroupNameDisplay}");
    }

    [Fact]
    public void AppSettings_ExposesStaticGroupedViewMode()
    {
        var document = LoadView("AppSettingsWindow.axaml");

        Assert.Contains(document.Descendants(), element =>
            (string?)element.Attribute("IsChecked") == "{Binding UseStaticGroupedView}");
    }

    [Fact]
    public void AppSettings_ExposesAlertBlinkColorPicker()
    {
        var document = LoadView("AppSettingsWindow.axaml");

        Assert.Contains(document.Descendants(), element =>
            element.Name.LocalName == "ColorPicker" &&
            (string?)element.Attribute("Color") ==
                "{Binding AlertBlinkColorHex, Mode=TwoWay, Converter={x:Static conv:HexColorConverter.Instance}}");
        Assert.Contains(document.Descendants(), element =>
            element.Name.LocalName == "TextBox" &&
            (string?)element.Attribute("Text") == "{Binding AlertBlinkColorHex}");
    }

    [Fact]
    public void MainWindow_StaticTableHasExpectedColumnsAndGroupSource()
    {
        var document = LoadView("MainWindow.axaml");
        var textValues = document.Descendants()
            .Select(element => (string?)element.Attribute("Text"))
            .Where(value => value is not null)
            .ToArray();

        Assert.Contains("Symbol", textValues);
        Assert.Contains("Last", textValues);
        Assert.Contains("Chg", textValues);
        Assert.Contains("Chg%", textValues);
        Assert.Contains(document.Descendants(), element =>
            (string?)element.Attribute("ItemsSource") == "{Binding StaticQuoteGroups}");
        Assert.DoesNotContain(document.Descendants(), element =>
            (string?)element.Attribute("ItemsSource") == "{Binding StaticNewsGroups}");
        Assert.Contains(document.Descendants(), element =>
            (string?)element.Attribute("IsVisible") == "{Binding IsScrollingNewsView}");

        var quoteColumns = document.Descendants()
            .Where(element => (string?)element.Attribute("ColumnDefinitions") == "2*,*,*,*")
            .ToArray();
        Assert.Equal(2, quoteColumns.Length);
        Assert.All(quoteColumns, grid => Assert.StartsWith("12,", (string?)grid.Attribute("Margin")));
        Assert.Single(
            document.Descendants(),
            element => element.Name.LocalName == nameof(ResponsiveTilePanel));
    }

    [Fact]
    public void MainWindow_StaticGroupsStretchAndSupportDragDrop()
    {
        var documents = new[] { LoadView("MainWindow.axaml"), LoadView("StaticNewsWindow.axaml") };
        var allowDrop = documents.SelectMany(document => document.Descendants())
            .Where(element => element.Attributes().Any(attribute =>
                attribute.Name.LocalName.EndsWith("AllowDrop", StringComparison.Ordinal) &&
                attribute.Value == "True"))
            .ToArray();

        Assert.Equal(2, allowDrop.Length);
        Assert.All(allowDrop, group =>
        {
            Assert.Contains(group.Attributes(), attribute =>
                attribute.Name.LocalName.EndsWith("DragOver", StringComparison.Ordinal) &&
                attribute.Value == "GroupDragOver");
            Assert.Contains(group.Attributes(), attribute =>
                attribute.Name.LocalName.EndsWith("Drop", StringComparison.Ordinal) &&
                attribute.Value == "GroupDrop");
        });
        Assert.All(documents, document => Assert.True(document.Descendants().Count(element =>
            element.Name.LocalName == "Style" &&
            (string?)element.Attribute("Selector") == "ContentPresenter") >= 1));
        Assert.All(documents, document => Assert.Single(
            document.Descendants(),
            element => (string?)element.Attribute("PointerPressed") == "BeginGroupDrag"));
    }

    [Fact]
    public void StaticNews_UsesSeparateMovableWindowAndCanBeReopened()
    {
        var mainWindow = LoadView("MainWindow.axaml");
        var newsWindow = LoadView("StaticNewsWindow.axaml");
        var source = File.ReadAllText(ViewPath("MainWindow.axaml.cs"));

        Assert.Equal("True", (string?)newsWindow.Root!.Attribute("CanResize"));
        Assert.Equal("True", (string?)newsWindow.Root.Attribute("Topmost"));
        Assert.Equal("SmartTicker News", (string?)newsWindow.Root.Attribute("Title"));
        Assert.Equal("680", (string?)newsWindow.Root.Attribute("Width"));
        Assert.Equal("340", (string?)newsWindow.Root.Attribute("Height"));
        Assert.Equal("Manual", (string?)newsWindow.Root.Attribute("WindowStartupLocation"));
        Assert.Contains(newsWindow.Descendants(), element =>
            (string?)element.Attribute("ItemsSource") == "{Binding StaticNewsGroups}");
        Assert.Contains(newsWindow.Descendants(), element =>
            (string?)element.Attribute("ItemsSource") == "{Binding FilterOptions}");
        Assert.Contains(newsWindow.Descendants(), element =>
            (string?)element.Attribute("SelectedItem") == "{Binding SelectedQuote, Mode=TwoWay}");
        Assert.DoesNotContain(newsWindow.Descendants(), element =>
            (string?)element.Attribute("Click") == "ShowHelp");
        Assert.Contains(mainWindow.Descendants(), element =>
            (string?)element.Attribute("Click") == "OpenStaticNewsWindow");
        Assert.Contains("SyncStaticNewsWindow();", source);
        Assert.Contains("PositionStaticNewsWindow(newsWindow);", source);
        Assert.Contains("newsWindow.Show();", source);
    }

    [Fact]
    public void ResponsiveTilePanel_UsesAvailableWidthBeforeVerticalOverflow()
    {
        var panel = new ResponsiveTilePanel
        {
            MinimumTileWidth = 380,
            MaximumTileWidth = 560,
            Spacing = 12,
        };

        Assert.Equal(new TileLayout(1, 360), panel.Calculate(360));
        Assert.Equal(new TileLayout(1, 560), panel.Calculate(700));
        Assert.Equal(new TileLayout(2, 434), panel.Calculate(880));
        Assert.Equal(new TileLayout(5, 410.4), panel.Calculate(2100));
    }

    [Fact]
    public void MainWindow_ViewMenuSelectsExactlyOneTickerMode()
    {
        var document = LoadView("MainWindow.axaml");
        var viewItems = document.Descendants()
            .Where(element => (string?)element.Attribute("GroupName") == "TickerView")
            .ToArray();

        Assert.Equal(4, viewItems.Length);
        Assert.All(viewItems, item => Assert.Equal("Radio", (string?)item.Attribute("ToggleType")));
        Assert.Equal(
            ["scrolling-prices", "scrolling-prices-news", "static-prices", "static-prices-news"],
            viewItems.Select(item => (string?)item.Attribute("CommandParameter")));
        Assert.Equal(
            [
                "{Binding IsScrollingPricesOnlyView, Mode=OneWay}",
                "{Binding IsScrollingPricesWithNewsView, Mode=OneWay}",
                "{Binding IsStaticPricesOnlyView, Mode=OneWay}",
                "{Binding IsStaticPricesWithNewsView, Mode=OneWay}",
            ],
            viewItems.Select(item => (string?)item.Attribute("IsChecked")));

        using var viewModel = new MainViewModel();
        Assert.True(viewModel.IsScrollingPricesOnlyView);

        viewModel.SetTickerViewCommand.Execute("static-prices");

        Assert.True(viewModel.IsStaticPricesOnlyView);
    }

    [Fact]
    public void GroupManager_ExposesCrudAndOneGroupAssociationWorkflow()
    {
        var document = LoadView("QuoteGroupsWindow.axaml");
        Assert.Equal("True", (string?)document.Root!.Attribute("Topmost"));
        Assert.Contains(document.Descendants(), element =>
            element.Name.LocalName == "TextBox" &&
            (string?)element.Attribute("Text") == "{Binding ManagedGroupName, Mode=TwoWay}");
        var commands = document.Descendants()
            .Select(element => (string?)element.Attribute("Command"))
            .Where(value => value is not null)
            .ToArray();

        Assert.Contains("{Binding CreateQuoteGroupCommand}", commands);
        Assert.Contains("{Binding UpdateQuoteGroupCommand}", commands);
        Assert.Contains("{Binding DeleteQuoteGroupCommand}", commands);
        Assert.Contains("{Binding AssociateSelectedQuoteCommand}", commands);
        Assert.Contains("{Binding UngroupSelectedQuoteCommand}", commands);
        Assert.Contains(document.Descendants(), element =>
            element.Name.LocalName == "ListBox" &&
            (string?)element.Attribute("ItemsSource") == "{Binding Subscriptions}" &&
            (string?)element.Attribute("SelectedItem") == "{Binding SelectedGroupQuote, Mode=TwoWay}");
        Assert.Contains(document.Descendants(), element =>
            (string?)element.Attribute("Text") == "{Binding GroupNameDisplay}");
    }

    private static XDocument LoadView(string name) => XDocument.Load(ViewPath(name));

    private static string ViewPath(string name)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null &&
               !Directory.Exists(Path.Combine(directory.FullName, "src", "SmartTicker.Desktop")))
        {
            directory = directory.Parent;
        }

        Assert.NotNull(directory);
        return Path.Combine(directory!.FullName, "src", "SmartTicker.Desktop", "Views", name);
    }
}