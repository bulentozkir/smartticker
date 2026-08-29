using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

public static class TickerLayoutCalculator
{
    public const double MinimumRowHeight = 20;
    public const double MaximumRowHeight = 56;
    public const double NaturalRowHeight = 26;

    // Window border plus the vertical padding of the visible bands.
    private const double ChromeWithNews = 12;
    private const double ChromeWithoutNews = 7;

    public static double NaturalHeight(
        int priceRowCount,
        int newsRowCount,
        bool priceEnabled = true,
        bool newsEnabled = true)
    {
        var rows = (priceEnabled ? ClampRows(priceRowCount) : 0) + (newsEnabled ? ClampRows(newsRowCount) : 0);
        var chrome = priceEnabled && newsEnabled ? ChromeWithNews : ChromeWithoutNews;
        return chrome + Math.Max(1, rows) * NaturalRowHeight;
    }

    public static TickerLayout Calculate(
        double availableHeight,
        int priceRowCount,
        int newsRowCount,
        bool priceEnabled = true,
        bool newsEnabled = true)
    {
        var priceRows = priceEnabled ? ClampRows(priceRowCount) : 0;
        var newsRows = newsEnabled ? ClampRows(newsRowCount) : 0;

        // News is dropped rather than squeezed once both bands can no longer meet the minimum row height.
        var showNews = newsRows > 0 &&
            (priceRows == 0 ||
             availableHeight >= ChromeWithNews + (priceRows + newsRows) * MinimumRowHeight);

        var chrome = priceRows > 0 && showNews ? ChromeWithNews : ChromeWithoutNews;
        var visibleRows = Math.Max(1, priceRows + (showNews ? newsRows : 0));

        var rowHeight = Math.Clamp(
            (availableHeight - chrome) / visibleRows,
            MinimumRowHeight,
            MaximumRowHeight);

        return new TickerLayout(
            Math.Round(rowHeight, 1),
            Math.Round(Math.Clamp(rowHeight * 0.55, 10, 24), 1),
            Math.Round(Math.Clamp(rowHeight * 0.52, 9, 22), 1),
            showNews);
    }

    private static int ClampRows(int value) => Math.Clamp(value, 1, 8);
}
