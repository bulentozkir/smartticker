using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;

namespace SmartTicker.Infrastructure.Extraction;

public sealed partial class StaticHtmlChangeExtractor
{
    private static readonly string[] ChangeSelectors =
    [
        "[data-field*='ChangePercent' i]",
        "[data-testid*='change-percent' i]",
        "[data-test*='CHANGE_PERCENT' i]",
        "[class*='changePercent' i]",
        "[class*='change-percent' i]",
        "[class*='percentChange' i]",
        "[id*='changePercent' i]",

        // Up/down variants are one selector so the match follows document order, like the price does.
        "[class*='changeUp' i],[class*='changeDown' i]",
        "[id*='pch' i]",
    ];

    public decimal? Extract(string html, string? cssSelector = null)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return null;
        }

        var document = new HtmlParser().ParseDocument(html);
        var selectors = string.IsNullOrWhiteSpace(cssSelector)
            ? ChangeSelectors
            : [cssSelector, .. ChangeSelectors];

        foreach (var selector in selectors)
        {
            try
            {
                foreach (var element in document.QuerySelectorAll(selector))
                {
                    var text = element.GetAttribute("value") ?? element.TextContent;
                    if (TryParsePercent(text, out var percent))
                    {
                        return percent;
                    }
                }
            }
            catch (AngleSharp.Dom.DomException)
            {
                // A malformed selector should not stop the remaining candidates.
            }
        }

        return null;
    }

    public static bool TryParsePercent(string? text, out decimal percent)
    {
        percent = default;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var match = TrailingPercentPattern().Match(text);
        if (!match.Success)
        {
            match = LeadingPercentPattern().Match(text);
        }

        if (!match.Success)
        {
            return false;
        }

        var number = match.Groups[1].Value.Replace(',', '.');
        if (!decimal.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out percent))
        {
            return false;
        }

        // Some sources mark direction with a glyph rather than a sign.
        if (percent > 0 && (text.Contains('−') || text.Contains('▼') || text.Contains("-")))
        {
            percent = -percent;
        }

        return true;
    }

    [GeneratedRegex(@"([+-]?\d{1,3}(?:[.,]\d+)?)\s*%", RegexOptions.CultureInvariant)]
    private static partial Regex TrailingPercentPattern();

    [GeneratedRegex(@"%\s*([+-]?\d{1,3}(?:[.,]\d+)?)", RegexOptions.CultureInvariant)]
    private static partial Regex LeadingPercentPattern();
}
