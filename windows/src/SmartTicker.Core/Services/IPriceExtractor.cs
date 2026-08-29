using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

public interface IPriceExtractor
{
    PriceExtractionResult Extract(string html, TickerSource source);
}