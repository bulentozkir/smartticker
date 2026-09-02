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
    public void AppSettings_DoesNotDuplicateViewOrGroupManagementCommands()
    {
        var document = LoadView("AppSettingsWindow.axaml");

        Assert.DoesNotContain(document.Descendants(), element =>
            (string?)element.Attribute("IsChecked") == "{Binding UseStaticGroupedView}");
        Assert.DoesNotContain(document.Descendants(), element =>
            (string?)element.Attribute("Click") == "OpenQuoteGroups");
    }

    [Fact]
    public void SampleConfigImport_IsOfferedFromBothWindowsBehindAnExportFirstConfirmation()
    {
        foreach (var windowName in new[] { "SettingsWindow", "AppSettingsWindow" })
        {
            var document = LoadView($"{windowName}.axaml");
            var button = Assert.Single(document.Descendants(), element =>
                (string?)element.Attribute("Click") == "ImportSampleConfig");

            Assert.Equal("Import Sample Quotes Config", (string?)button.Attribute("Content"));
            Assert.Equal("{Binding !IsLoadingStarter}", (string?)button.Attribute("IsEnabled"));
            Assert.Contains(
                "SampleConfigImportWorkflow.RunAsync(this, viewModel)",
                File.ReadAllText(ViewPath($"{windowName}.axaml.cs")));
        }

        var workflow = File.ReadAllText(ViewPath("SampleConfigImportWorkflow.cs"));

        Assert.Contains("Are you sure?", workflow);
        Assert.Contains("downloads the published sample config from the internet", workflow);
        Assert.Contains("replaces your existing quotes", workflow);
        Assert.Contains("Export existing config...", workflow);
        Assert.Contains("Import Sample Quotes Config", workflow);
        Assert.Contains("Cancel", workflow);
        Assert.Contains("ExportSettingsJson()", workflow);
        Assert.Contains("LoadStarterQuotesCommand.ExecuteAsync(null)", workflow);
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
    public void AppSettings_ExposesSeparateScrollingAndStaticFontSizes()
    {
        var settings = LoadView("AppSettingsWindow.axaml");
        var mainWindow = LoadView("MainWindow.axaml");
        var newsWindow = LoadView("StaticNewsWindow.axaml");

        var fontSizeInputs = settings.Descendants()
            .Where(element => element.Name.LocalName == "NumericUpDown")
            .Where(element => (string?)element.Attribute("Value") is
                "{Binding ScrollingViewFontSize}" or "{Binding StaticViewFontSize}")
            .ToArray();
        Assert.Equal(2, fontSizeInputs.Length);
        Assert.All(fontSizeInputs, input =>
        {
            Assert.Equal("9", (string?)input.Attribute("Minimum"));
            Assert.Equal("24", (string?)input.Attribute("Maximum"));
            Assert.Equal("1", (string?)input.Attribute("Increment"));
        });

        const string staticBinding =
            "{Binding $parent[Window].((vm:MainViewModel)DataContext).StaticViewFontSize}";
        Assert.Equal(4, mainWindow.Descendants().Count(element =>
            (string?)element.Attribute("FontSize") == staticBinding));
        Assert.Equal(2, newsWindow.Descendants().Count(element =>
            (string?)element.Attribute("FontSize") == staticBinding));
        Assert.Equal(2, mainWindow.Descendants().Count(element =>
            (string?)element.Attribute("TickerFontSize") == "{Binding FontSize}"));
    }

    [Fact]
    public void AppSettings_ExposesThreePersistedWindowSizePairs()
    {
        var settings = LoadView("AppSettingsWindow.axaml");
        var mainWindow = LoadView("MainWindow.axaml");
        var newsWindow = LoadView("StaticNewsWindow.axaml");
        var values = settings.Descendants()
            .Where(element => element.Name.LocalName == "NumericUpDown")
            .Select(element => (string?)element.Attribute("Value"))
            .ToArray();

        Assert.Contains("{Binding ScrollingWindowWidth}", values);
        Assert.Contains("{Binding ScrollingWindowHeight}", values);
        Assert.Contains("{Binding StaticPricesWindowWidth}", values);
        Assert.Contains("{Binding StaticPricesWindowHeight}", values);
        Assert.Contains("{Binding StaticNewsWindowWidth}", values);
        Assert.Contains("{Binding StaticNewsWindowHeight}", values);
        Assert.Equal("{Binding WindowWidth}", (string?)mainWindow.Root!.Attribute("Width"));
        Assert.Equal("{Binding WindowHeight}", (string?)mainWindow.Root.Attribute("Height"));
        Assert.Equal("{Binding MinimumMainWindowHeight}", (string?)mainWindow.Root.Attribute("MinHeight"));
        Assert.Equal("{Binding StaticNewsWindowWidth}", (string?)newsWindow.Root!.Attribute("Width"));
        Assert.Equal("{Binding StaticNewsWindowHeight}", (string?)newsWindow.Root.Attribute("Height"));
    }

    [Fact]
    public void EveryWindowUsesTheSharedReachabilityGuard()
    {
        foreach (var fileName in new[]
                 {
                     "AboutWindow.axaml.cs",
                     "AlertsWindow.axaml.cs",
                     "AppSettingsWindow.axaml.cs",
                     "HelpWindow.axaml.cs",
                     "MainWindow.axaml.cs",
                     "QuoteGroupsWindow.axaml.cs",
                     "SettingsWindow.axaml.cs",
                     "SourcePermissionWindow.axaml.cs",
                     "StaticNewsWindow.axaml.cs",
                     "WebsiteConsentWindow.axaml.cs",
                 })
        {
            Assert.Contains("WindowReachability.Attach(this);", File.ReadAllText(ViewPath(fileName)));
        }

        foreach (var fileName in new[]
                 {
                     "ConfirmDialog.cs",
                     "EditConfigFileWorkflow.cs",
                     "SampleConfigImportWorkflow.cs",
                 })
        {
            Assert.Contains("WindowReachability.Attach(dialog);", File.ReadAllText(ViewPath(fileName)));
        }
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
        Assert.Equal("{Binding StaticNewsWindowWidth}", (string?)newsWindow.Root.Attribute("Width"));
        Assert.Equal("{Binding StaticNewsWindowHeight}", (string?)newsWindow.Root.Attribute("Height"));
        Assert.Equal("Manual", (string?)newsWindow.Root.Attribute("WindowStartupLocation"));
        Assert.Contains(newsWindow.Descendants(), element =>
            (string?)element.Attribute("ItemsSource") == "{Binding StaticNewsGroups}");
        Assert.Contains(newsWindow.Descendants(), element =>
            (string?)element.Attribute("ItemsSource") == "{Binding QuoteFilters}");
        Assert.Contains(newsWindow.Descendants(), element =>
            element.Name.LocalName == "CheckBox" &&
            (string?)element.Attribute("IsChecked") == "{Binding IsShown, Mode=TwoWay}");
        // The multi-select list lives in a flyout so the filter itself stays one line tall.
        Assert.Contains(newsWindow.Descendants(), element => element.Name.LocalName == "Flyout");
        Assert.Contains(newsWindow.Descendants(), element =>
            (string?)element.Attribute("Text") == "{Binding FilterSummary}");
        Assert.DoesNotContain(newsWindow.Descendants(), element =>
            element.Name.LocalName == "WrapPanel");
        Assert.DoesNotContain(newsWindow.Descendants(), element =>
            (string?)element.Attribute("Click") == "ShowHelp");
        Assert.Contains(mainWindow.Descendants(), element =>
            (string?)element.Attribute("Click") == "OpenStaticNewsWindow");
        Assert.Contains("SyncStaticNewsWindow();", source);
        Assert.Contains("PositionStaticNewsWindow(newsWindow);", source);
        Assert.Contains("newsWindow.Show();", source);
    }

    [Fact]
    public void AppSettings_OffersDirectConfigFileEditingBehindAnExportFirstWarning()
    {
        var document = LoadView("AppSettingsWindow.axaml");
        var handlers = document.Descendants()
            .Select(element => (string?)element.Attribute("Click"))
            .Where(value => value is not null)
            .ToArray();

        Assert.Contains("EditAppConfig", handlers);
        Assert.Contains("EditAlertRules", handlers);

        var code = File.ReadAllText(ViewPath("AppSettingsWindow.axaml.cs"));
        Assert.Contains("EditConfigFileWorkflow.RunAsync(this, viewModel, ConfigFileKind.Settings)", code);
        Assert.Contains("EditConfigFileWorkflow.RunAsync(this, viewModel, ConfigFileKind.Alerts)", code);

        var workflow = File.ReadAllText(ViewPath("EditConfigFileWorkflow.cs"));
        Assert.Contains("For advanced users", workflow);
        Assert.Contains("Export existing config...", workflow);
        Assert.Contains("Open in text editor", workflow);
        Assert.Contains("Cancel", workflow);
        Assert.Contains("reloads the file as soon as you save it", workflow);
        Assert.Contains("LocalConfigFileLauncher().TryOpen(path)", workflow);
    }

    [Fact]
    public void ResponsiveTilePanel_FillsAvailableWidthWithoutLeftoverGaps()
    {
        var panel = new ResponsiveTilePanel
        {
            MinimumTileWidth = 380,
            Spacing = 12,
        };

        Assert.Equal(new TileLayout(1, 360), panel.Calculate(360));
        Assert.Equal(new TileLayout(1, 700), panel.Calculate(700));
        Assert.Equal(new TileLayout(2, 434), panel.Calculate(880));
        Assert.Equal(new TileLayout(5, 410.4), panel.Calculate(2100));
        Assert.Equal(new TileLayout(2, 1044), panel.Calculate(2100, 2));
        Assert.Equal(new TileLayout(1, 2100), panel.Calculate(2100, 1));
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
    public void MainWindow_EventSinksAreContainedAndCompanionSyncIsDeferred()
    {
        var app = File.ReadAllText(ViewPath("../App.axaml.cs"));
        var mainWindow = File.ReadAllText(ViewPath("MainWindow.axaml.cs"));

        Assert.Contains("ShutdownMode.OnMainWindowClose", app);
        Assert.Contains("Dispatcher.UIThread.UnhandledException", app);
        Assert.Contains("Dispatcher.UIThread.InvokeAsync", app);
        Assert.Contains("RefreshPriceSubscriptionsSafelyAsync", mainWindow);
        Assert.Contains("StaggeredRefreshSchedule", mainWindow);
        Assert.Contains("Interval = TimeSpan.FromSeconds(1)", mainWindow);
        Assert.Contains("RefreshNewsSubscriptionsSafelyAsync", mainWindow);
        Assert.Contains("_newsRefreshSchedule", mainWindow);
        Assert.Contains("QueueStaticNewsWindowSync();", mainWindow);
        Assert.Contains("RunSafely(\"Applying a setting change\"", mainWindow);
        Assert.Contains("RunSafely(\"Scheduling data refresh\"", mainWindow);
        Assert.DoesNotContain("async void OnRefreshTimerTick", mainWindow);
        Assert.Contains("ExceptionSafety.Run", File.ReadAllText(ViewPath("../Controls/MarqueeText.cs")));
        Assert.Contains("ExceptionSafety.RunAsync", File.ReadAllText(ViewPath("AppSettingsWindow.axaml.cs")));
        Assert.Contains("ExceptionSafety.RunAsync", File.ReadAllText(ViewPath("SettingsWindow.axaml.cs")));
        Assert.DoesNotContain("_ = viewModel.RefreshPricesAsync();", mainWindow);
        Assert.DoesNotContain("_ = viewModel.RefreshNewsAsync();", mainWindow);
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