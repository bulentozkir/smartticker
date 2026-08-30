namespace SmartTicker.Desktop.Localization;

/// <summary>Every user-visible menu and status string, in one language.</summary>
public sealed class UiStrings
{
    public required string MenuQuotes { get; init; }

    public string MenuAlerts { get; init; } = "Alerts";

    public required string MenuAppSettings { get; init; }

    public required string MenuShowPriceLine { get; init; }

    public required string MenuShowNewsLine { get; init; }

    public required string MenuRefreshPrices { get; init; }

    public required string MenuRefreshNews { get; init; }

    public required string MenuPauseResume { get; init; }

    public required string MenuLanguage { get; init; }

    public required string MenuAbout { get; init; }

    public required string MenuExit { get; init; }

    public required string StatusPaused { get; init; }

    public required string StatusWorking { get; init; }

    public required string EmptyPriceLine { get; init; }

    public required string EmptyNewsLine { get; init; }

    public required string Loading { get; init; }

    public required string Unavailable { get; init; }

    public required string TitleQuotes { get; init; }

    public required string TitleAppSettings { get; init; }

    public required string TitleAbout { get; init; }
}
