namespace SmartTicker.Core.Models;

public sealed record PriceExtractionResult(
    bool Success,
    decimal? Price,
    string? Currency,
    string Method,
    string Message)
{
    public static PriceExtractionResult Found(decimal price, string? currency, string method) =>
        new(true, price, currency, method, "Price candidate found. Verify it against the source page.");

    public static PriceExtractionResult Failed(string message) =>
        new(false, null, null, "None", message);
}