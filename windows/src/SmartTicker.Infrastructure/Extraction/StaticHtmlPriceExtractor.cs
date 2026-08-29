using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using SmartTicker.Core.Models;
using SmartTicker.Core.Services;

namespace SmartTicker.Infrastructure.Extraction;

public sealed partial class StaticHtmlPriceExtractor : IPriceExtractor
{
    private static readonly string[] PriceMetaSelectors =
    [
        "meta[property='product:price:amount']",
        "meta[property='og:price:amount']",
        "meta[itemprop='price']",
    ];

    public PriceExtractionResult Extract(string html, TickerSource source)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return PriceExtractionResult.Failed("The page returned no HTML.");
        }

        var document = new HtmlParser().ParseDocument(html);

        if (source.CssSelector is not null)
        {
            try
            {
                var selected = document.QuerySelector(source.CssSelector);
                if (selected is null)
                {
                    return PriceExtractionResult.Failed("The configured CSS selector did not match an element.");
                }

                if (TryParsePrice(selected.TextContent, out var selectedPrice))
                {
                    return PriceExtractionResult.Found(selectedPrice, source.Currency, "CSS selector");
                }

                return PriceExtractionResult.Failed("The selected element did not contain an unambiguous price.");
            }
            catch (DomException)
            {
                return PriceExtractionResult.Failed("The configured CSS selector is invalid.");
            }
        }

        var structuredCandidates = ReadJsonLdCandidates(document)
            .Concat(ReadMetaCandidates(document))
            .Distinct()
            .ToArray();

        if (structuredCandidates.Length == 1)
        {
            return PriceExtractionResult.Found(structuredCandidates[0], source.Currency, "Structured data");
        }

        if (structuredCandidates.Length > 1)
        {
            return PriceExtractionResult.Failed("The page exposes multiple structured price values. Add a CSS selector.");
        }

        return PriceExtractionResult.Failed(
            "No reliable static-HTML price was found. Add a CSS selector or choose an authorized feed; JavaScript rendering is not used.");
    }

    private static IEnumerable<decimal> ReadMetaCandidates(IDocument document)
    {
        foreach (var selector in PriceMetaSelectors)
        {
            var value = document.QuerySelector(selector)?.GetAttribute("content");
            if (TryParsePrice(value, out var price))
            {
                yield return price;
            }
        }
    }

    private static IEnumerable<decimal> ReadJsonLdCandidates(IDocument document)
    {
        foreach (var script in document.QuerySelectorAll("script[type='application/ld+json']"))
        {
            JsonDocument json;
            try
            {
                json = JsonDocument.Parse(script.TextContent);
            }
            catch (JsonException)
            {
                continue;
            }

            using (json)
            {
                foreach (var price in FindPriceProperties(json.RootElement))
                {
                    yield return price;
                }
            }
        }
    }

    private static IEnumerable<decimal> FindPriceProperties(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("price") && TryParsePrice(property.Value.ToString(), out var price))
                {
                    yield return price;
                }

                foreach (var nested in FindPriceProperties(property.Value))
                {
                    yield return nested;
                }
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                foreach (var nested in FindPriceProperties(item))
                {
                    yield return nested;
                }
            }
        }
    }

    private static bool TryParsePrice(string? text, out decimal price)
    {
        price = default;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var match = PricePattern().Match(text.Replace('\u00a0', ' ').Trim());
        if (!match.Success)
        {
            return false;
        }

        var normalized = match.Groups[1].Value.Replace(",", string.Empty, StringComparison.Ordinal);
        return decimal.TryParse(
            normalized,
            NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture,
            out price) && price >= 0;
    }

    [GeneratedRegex(@"(?<![\d.,])([+-]?\d{1,3}(?:,\d{3})*(?:\.\d+)?|[+-]?\d+(?:\.\d+)?)(?![\d.,])", RegexOptions.CultureInvariant)]
    private static partial Regex PricePattern();
}