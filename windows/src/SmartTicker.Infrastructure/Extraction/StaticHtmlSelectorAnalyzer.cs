using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using SmartTicker.Core.Models;

namespace SmartTicker.Infrastructure.Extraction;

public sealed partial class StaticHtmlSelectorAnalyzer
{
    private static readonly string[] ExtendedMarkers =
    [
        "extended", "afterhour", "after-hour", "aftermarket", "after-market",
        "postmarket", "post-market", "post-price", "post-trade", "posttrade", "qsp-post",
    ];

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
        var identity = Identity(element);

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

    private static string Identity(IElement element) => string.Join(' ',
        element.Id,
        element.ClassName,
        element.GetAttribute("data-testid"),
        element.GetAttribute("data-field"),
        element.GetAttribute("aria-label"));

    public IReadOnlyList<CssSelectorSuggestion> Analyze(string html, SelectorKind kind, int maximumSuggestions = 5)
    {
        if (kind == SelectorKind.Price)
        {
            return Analyze(html, maximumSuggestions);
        }

        if (string.IsNullOrWhiteSpace(html) || maximumSuggestions < 1)
        {
            return [];
        }

        var document = new HtmlParser().ParseDocument(html);
        var wantsPercent = kind is SelectorKind.Change or SelectorKind.ExtendedChange;
        var wantsExtended = kind is SelectorKind.ExtendedPrice or SelectorKind.ExtendedChange;
        var suggestions = new List<CssSelectorSuggestion>();

        foreach (var element in document.QuerySelectorAll("*"))
        {
            if (element.LocalName is "script" or "style" or "noscript")
            {
                continue;
            }

            var text = (element.GetAttribute("content") ?? element.TextContent).Trim();
            var match = wantsPercent ? PercentPattern().Match(text) : PricePattern().Match(text);
            if (!match.Success || match.Value.Length * 2 < text.Length)
            {
                continue;
            }

            // Identifiers like "QuoteStrip-changeDown" contain "quote", so a percent must not pass as a price.
            if (!wantsPercent && PercentPattern().IsMatch(text))
            {
                continue;
            }

            var scope = FindExtendedScope(element);
            if ((scope is not null) != wantsExtended)
            {
                continue;
            }

            var (score, reason) = ScoreFor(kind, element);
            if (score == 0 || CreateSelector(element, document) is not { } candidate)
            {
                continue;
            }

            var selector = candidate.Selector;
            var matches = candidate.Matches;

            // The after-hours value often shares its class with the close price, so the container disambiguates it.
            if (wantsExtended && scope is not null && !ReferenceEquals(scope, element) &&
                CreateSelector(scope, document) is { } container)
            {
                var scoped = $"{container.Selector} {candidate.Selector}";
                var scopedMatches = CountMatches(scoped, document);
                if (scopedMatches > 0)
                {
                    selector = scoped;
                    matches = scopedMatches;
                }
            }

            var confidence = matches == 1 ? score : Math.Max(1, score - 20);
            var detail = matches == 1
                ? reason
                : $"{reason}; matches {matches} elements, the first is used";

            suggestions.Add(new CssSelectorSuggestion(selector, match.Value, Math.Min(confidence, 100), detail));
        }

        return suggestions
            .OrderByDescending(item => item.Confidence)
            .ThenBy(item => item.Selector.Length)
            .DistinctBy(item => item.Selector, StringComparer.Ordinal)
            .Take(maximumSuggestions)
            .ToArray();
    }

    private static IElement? FindExtendedScope(IElement element)
    {
        var current = element;
        for (var depth = 0; depth < 5 && current is not null; depth++)
        {
            var identity = Identity(current);
            if (ExtendedMarkers.Any(marker => identity.Contains(marker, StringComparison.OrdinalIgnoreCase)))
            {
                return current;
            }

            current = current.ParentElement;
        }

        return null;
    }

    private static (int Score, string Reason) ScoreFor(SelectorKind kind, IElement element)
    {
        var identity = Identity(element);
        if (kind is SelectorKind.Change or SelectorKind.ExtendedChange)
        {
            if (identity.Contains("changepercent", StringComparison.OrdinalIgnoreCase) ||
                identity.Contains("change-percent", StringComparison.OrdinalIgnoreCase) ||
                identity.Contains("percentchange", StringComparison.OrdinalIgnoreCase) ||
                identity.Contains("pchg", StringComparison.OrdinalIgnoreCase) ||
                identity.Contains("pch", StringComparison.OrdinalIgnoreCase))
            {
                return (95, "Identifier names a percent change");
            }

            return identity.Contains("change", StringComparison.OrdinalIgnoreCase) ||
                   identity.Contains("chg", StringComparison.OrdinalIgnoreCase)
                ? (80, "Identifier contains 'change'")
                : (0, string.Empty);
        }

        if (identity.Contains("price", StringComparison.OrdinalIgnoreCase))
        {
            return (90, "Identifier contains 'price'");
        }

        return identity.Contains("last", StringComparison.OrdinalIgnoreCase) ||
               identity.Contains("quote", StringComparison.OrdinalIgnoreCase)
            ? (70, "Identifier resembles a quote value")
            : (0, string.Empty);
    }

    private static string EscapeAttribute(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal);

    [GeneratedRegex(@"^[A-Za-z_][A-Za-z0-9_-]*$", RegexOptions.CultureInvariant)]
    private static partial Regex SimpleIdentifierPattern();

    [GeneratedRegex(@"(?<![\d.,])(?:[$€£¥]\s*)?([+-]?\d{1,3}(?:,\d{3})*(?:\.\d+)?|[+-]?\d+(?:\.\d+)?)(?![\d.,])", RegexOptions.CultureInvariant)]
    private static partial Regex PricePattern();

    [GeneratedRegex(@"[+-]?\d{1,3}(?:,\d{3})*(?:\.\d+)?\s*%|[+-]?\d+(?:\.\d+)?\s*%", RegexOptions.CultureInvariant)]
    private static partial Regex PercentPattern();
}