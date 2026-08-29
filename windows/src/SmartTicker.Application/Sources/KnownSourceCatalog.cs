using SmartTicker.Core.Models;

namespace SmartTicker.Application.Sources;

public static class KnownSourceCatalog
{
    public static IReadOnlyList<SourcePreset> All { get; } =
    [
        new(
            "Yahoo Finance",
            new Uri("https://finance.yahoo.com/"),
            CollectionPolicy.RequiresWrittenPermission,
            "Yahoo's terms prohibit automated collection without prior permission. Open in a browser or configure an authorized feed."),
        new(
            "CNBC",
            new Uri("https://www.cnbc.com/"),
            CollectionPolicy.CheckSitePolicy,
            "Use only public pages or feeds whose current terms and robots directives allow automated access."),
        new(
            "Trading Economics",
            new Uri("https://tradingeconomics.com/"),
            CollectionPolicy.CheckSitePolicy,
            "Prefer a documented Trading Economics API or authorized feed; Google sign-in does not authorize crawling."),
        new(
            "Custom URL",
            null,
            CollectionPolicy.CheckSitePolicy,
            "Enter a complete public HTTP or HTTPS URL and verify that automated access is permitted."),
    ];
}