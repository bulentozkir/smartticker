using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using SmartTicker.Core.Models;

namespace SmartTicker.Infrastructure.Extraction;

public sealed partial class StaticHtmlNewsSelectorAnalyzer
{
    private const int AncestorDepth = 4;

    public IReadOnlyList<CssSelectorSuggestion> Analyze(string html, int maximumSuggestions = 5)
    {
        if (string.IsNullOrWhiteSpace(html) || maximumSuggestions < 1)
        {
            return [];
        }

        var document = new HtmlParser().ParseDocument(html);
        var suggestions = new List<CssSelectorSuggestion>();
        foreach (var anchor in document.QuerySelectorAll("a[href]"))
        {
            var headline = Normalize(anchor.TextContent);
            if (headline.Length is < 12 or > 240)
            {
                continue;
            }

            var (confidence, reason) = Score(anchor);
            var selector = CreateSelector(anchor);
            if (confidence > 0 && selector is not null)
            {
                suggestions.Add(new CssSelectorSuggestion(selector, headline, confidence, reason));
            }
        }

        return suggestions
            .OrderByDescending(item => item.Confidence)
            .ThenBy(item => item.Selector.Length)
            .DistinctBy(item => item.Selector, StringComparer.Ordinal)
            .Take(maximumSuggestions)
            .ToArray();
    }

    private static (int Confidence, string Reason) Score(IElement anchor)
    {
        var identity = string.Join(' ',
            anchor.ClassName,
            anchor.GetAttribute("data-testid"),
            anchor.GetAttribute("data-field"),
            anchor.GetAttribute("aria-label"),
            string.Join(' ', Ancestors(anchor).Select(item => item.ClassName)));

        return identity.Contains("headline", StringComparison.OrdinalIgnoreCase)
            ? (95, "Link identifier contains 'headline'")
            : identity.Contains("title", StringComparison.OrdinalIgnoreCase)
                ? (82, "Link identifier contains 'title'")
                : anchor.ParentElement?.LocalName is "h1" or "h2" or "h3"
                    ? (78, "Headline link inside a heading")
                    : identity.Contains("news", StringComparison.OrdinalIgnoreCase) ||
                      identity.Contains("story", StringComparison.OrdinalIgnoreCase)
                        ? (70, "Link appears in a news or story element")
                        : (0, string.Empty);
    }

    private static IEnumerable<IElement> Ancestors(IElement anchor)
    {
        var ancestor = anchor.ParentElement;
        for (var depth = 0; depth < AncestorDepth && ancestor is not null; depth++)
        {
            yield return ancestor;
            ancestor = ancestor.ParentElement;
        }
    }

    private static string? CreateSelector(IElement anchor)
    {
        foreach (var attributeName in new[] { "data-testid", "data-field" })
        {
            if (anchor.GetAttribute(attributeName) is { Length: > 0 } value)
            {
                return $"a[{attributeName}=\"{EscapeAttribute(value)}\"]";
            }
        }

        var className = anchor.ClassList.FirstOrDefault(value =>
            SimpleIdentifierPattern().IsMatch(value) &&
            (value.Contains("headline", StringComparison.OrdinalIgnoreCase) ||
             value.Contains("title", StringComparison.OrdinalIgnoreCase) ||
             value.Contains("story", StringComparison.OrdinalIgnoreCase)));
        if (className is not null)
        {
            return $"a.{className}";
        }

        // Headline links are often bare, so fall back to the nearest container that names itself.
        foreach (var ancestor in Ancestors(anchor))
        {
            var containerClass = ancestor.ClassList.FirstOrDefault(value =>
                SimpleIdentifierPattern().IsMatch(value) &&
                (value.Contains("headline", StringComparison.OrdinalIgnoreCase) ||
                 value.Contains("news", StringComparison.OrdinalIgnoreCase) ||
                 value.Contains("story", StringComparison.OrdinalIgnoreCase)));
            if (containerClass is not null)
            {
                return $"{ancestor.LocalName}.{containerClass} a";
            }
        }

        return null;
    }

    private static string Normalize(string value) =>
        WhitespacePattern().Replace(value, " ").Trim();

    private static string EscapeAttribute(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal);

    [GeneratedRegex(@"^[A-Za-z_][A-Za-z0-9_-]*$", RegexOptions.CultureInvariant)]
    private static partial Regex SimpleIdentifierPattern();

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
    private static partial Regex WhitespacePattern();
}