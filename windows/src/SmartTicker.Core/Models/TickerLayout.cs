namespace SmartTicker.Core.Models;

public sealed record TickerLayout(
    double RowHeight,
    double PriceFontSize,
    double NewsFontSize,
    bool ShowNews);
