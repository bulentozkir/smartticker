using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using SmartTicker.Core.Models;

namespace SmartTicker.Infrastructure.Extraction;

public sealed partial class StaticHtmlSelectorAnalyzer
{
    public IReadOnlyList<CssSelectorSuggestion> Analyze(string html, int maximumSuggestions = 5)
    {
        if (string.IsNullOrWhiteSpace(html) || maximumSuggestions < 1)
        {
            return [];
        }

        var document = new HtmlParser().ParseDocument(html);
        var suggestions = new List<CssSelectorSuggestion>();

        foreach (var element in document.QuerySelectorAll("*") )
        {
            if (element.LocalName is "script" or "style" or "noscript")
            {
                continue;
            }

            var candidateText = element.GetAttribute("content") ?? element.TextContent;
            var text = candidateText.Trim();
            var valueMatch = PricePattern().Match(text);
            if (!valueMatch.Success || !decimal.TryParse(
                    valueMatch.Groups[1].Value.Replace(",", string.Empty, StringComparison.Ordinal),
                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture,
                    out var price) || price < 0)
            {
                continue;
            }

            // A container's text concatenates its descendants, so a number found there is rarely the quote.
            if (valueMatch.Value.Length * 2 < text.Length)
            {
                continue;
            }

            var (score, reason) = Score(element);
            if (score == 0 || CreateSelector(element, document) is not { } candidate)
            {
                continue;
            }

            var confidence = candidate.Matches == 1 ? score : Math.Max(1, score - 20);
            var detail = candidate.Matches == 1
                ? reason
                : $"{reason}; matches {candidate.Matches} elements, the first is used";

            suggestions.Add(new CssSelectorSuggestion(
                candidate.Selector,
                valueMatch.Value,
                Math.Min(confidence, 100),
                detail));
        }

        return suggestions
            .OrderByDescending(item => item.Confidence)
            .ThenBy(item => item.Selector.Length)
            .DistinctBy(item => item.Selector, StringComparer.Ordinal)
            .Take(maximumSuggestions)
            .ToArray();
    }

    private static (int Score, string Reason) Score(IElement element)
    {
        var itemProp = element.GetAttribute("itemprop") ?? string.Empty;
        var property = element.GetAttribute("property") ?? string.Empty;
        var identity = string.Join(' ',
            element.Id,
            element.ClassName,
            element.GetAttribute("data-testid"),
            element.GetAttribute("data-field"),
            element.GetAttribute("aria-label"));

        if (itemProp.Equals("price", StringComparison.OrdinalIgnoreCase))
        {
            return (100, "Schema price field");
        }

        if (property.Contains("price", StringComparison.OrdinalIgnoreCase))
        {
            return (95, "Price metadata");
        }

        if (identity.Contains("price", StringComparison.OrdinalIgnoreCase))
        {
            return (85, "Element identifier contains 'price'");
        }

        if (identity.Contains("last", StringComparison.OrdinalIgnoreCase) ||
            identity.Contains("quote", StringComparison.OrdinalIgnoreCase))
        {
            return (65, "Element identifier resembles a quote value");
        }

        return (0, string.Empty);
    }

    // Prefers a selector that matches exactly one element, but a repeated one still beats no suggestion.
    private static (string Selector, int Matches)? CreateSelector(IElement element, IDocument document)
    {
        var candidates = new List<string>();
        foreach (var attributeName in new[] { "itemprop", "data-testid", "data-field", "property" })
        {
            if (element.GetAttribute(attributeName) is { Length: > 0 } value)
            {
                candidates.Add($"{element.LocalName}[{attributeName}=\"{EscapeAttribute(value)}\"]");
            }
        }

        if (!string.IsNullOrWhiteSpace(element.Id))
        {
            candidates.Add($"{element.LocalName}[id=\"{EscapeAttribute(element.Id)}\"]");
        }

        foreach (var className in element.ClassList.Where(IsSimpleIdentifier))
        {
            candidates.Add($"{element.LocalName}.{className}");
        }

        (string Selector, int Matches)? repeated = null;
        foreach (var selector in candidates)
        {
            var matches = CountMatches(selector, document);
            if (matches == 1)
            {
                return (selector, 1);
            }

            if (matches > 1)
            {
                repeated ??= (selector, matches);
            }
        }

        return repeated;
    }

    private static int CountMatches(string selector, IDocument document)
    {
        try
        {
            return document.QuerySelectorAll(selector).Length;
        }
        catch (DomException)
        {
            return 0;
        }
    }

    private static bool IsSimpleIdentifier(string value) => SimpleIdentifierPattern().IsMatch(value);

    private static string EscapeAttribute(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal);

    [GeneratedRegex(@"^[A-Za-z_][A-Za-z0-9_-]*$", RegexOptions.CultureInvariant)]
    private static partial Regex SimpleIdentifierPattern();

    [GeneratedRegex(@"(?<![\d.,])(?:[$€£¥]\s*)?([+-]?\d{1,3}(?:,\d{3})*(?:\.\d+)?|[+-]?\d+(?:\.\d+)?)(?![\d.,])", RegexOptions.CultureInvariant)]
    private static partial Regex PricePattern();
}