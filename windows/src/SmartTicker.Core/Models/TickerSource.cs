namespace SmartTicker.Core.Models;

public sealed record TickerSource(
    string Symbol,
    string DisplayName,
    Uri PageUri,
    string? CssSelector = null,
    string? Currency = null)
{
    public static bool TryCreate(
        string symbol,
        string displayName,
        string pageUrl,
        string? cssSelector,
        string? currency,
        out TickerSource? source,
        out string? error)
    {
        source = null;
        error = null;

        if (string.IsNullOrWhiteSpace(symbol))
        {
            error = "Ticker symbol is required.";
            return false;
        }

        if (!Uri.TryCreate(pageUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
        {
            error = "A valid HTTP or HTTPS page URL is required.";
            return false;
        }

        if (!string.IsNullOrEmpty(uri.UserInfo))
        {
            error = "URLs containing credentials are not accepted.";
            return false;
        }

        source = new TickerSource(
            symbol.Trim().ToUpperInvariant(),
            string.IsNullOrWhiteSpace(displayName) ? symbol.Trim().ToUpperInvariant() : displayName.Trim(),
            uri,
            string.IsNullOrWhiteSpace(cssSelector) ? null : cssSelector.Trim(),
            string.IsNullOrWhiteSpace(currency) ? null : currency.Trim().ToUpperInvariant());
        return true;
    }
}