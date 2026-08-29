using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using SmartTicker.Core.Models;

namespace SmartTicker.Infrastructure.Extraction;

public sealed partial class StaticHtmlNewsExtractor
{
    public sealed record Result(IReadOnlyList<NewsHeadline> Headlines, bool Success, string Message);

    public Result Extract(string html, string? cssSelector, Uri? baseUri = null, int maximumHeadlines = 12)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return new Result([], false, "The source returned an empty document.");
        }

        var document = new HtmlParser().ParseDocument(html);

        if (!string.IsNullOrWhiteSpace(cssSelector))
        {
            IHtmlCollection<IElement> matches;
            try
            {
                matches = document.QuerySelectorAll(cssSelector);
            }
            catch (DomException)
            {
                return new Result([], false, $"The selector '{cssSelector}' is not valid CSS.");
            }

            var selected = Collect(matches, baseUri, maximumHeadlines);
            return selected.Count > 0
                ? new Result(selected, true, $"Read {selected.Count} headline(s).")
                : new Result([], false, $"The selector '{cssSelector}' did not match any headline text.");
        }

        var discovered = Collect(document.QuerySelectorAll("a[href]").Where(IsLikelyHeadline), baseUri, maximumHeadlines);
        return discovered.Count > 0
            ? new Result(discovered, true, $"Read {discovered.Count} headline(s) automatically.")
            : new Result([], false, "No headlines were found. Set a news CSS selector for this source.");
    }

    private static List<NewsHeadline> Collect(IEnumerable<IElement> elements, Uri? baseUri, int maximumHeadlines)
    {
        var headlines = new List<NewsHeadline>();
        foreach (var element in elements)
        {
            var headline = Normalize(element.TextContent);
            if (headline.Length is >= 12 and <= 240 &&
                !headlines.Any(item => string.Equals(item.Title, headline, StringComparison.OrdinalIgnoreCase)))
            {
                headlines.Add(new NewsHeadline(headline, ResolveLink(element, baseUri)));
                if (headlines.Count == maximumHeadlines)
                {
                    break;
                }
            }
        }

        return headlines;
    }

    private static Uri? ResolveLink(IElement element, Uri? baseUri)
    {
        var href = element.GetAttribute("href")
            ?? element.Closest("a[href]")?.GetAttribute("href")
            ?? element.QuerySelector("a[href]")?.GetAttribute("href");
        if (string.IsNullOrWhiteSpace(href))
        {
            return null;
        }

        Uri? resolved;
        if (baseUri is null)
        {
            _ = Uri.TryCreate(href, UriKind.Absolute, out resolved);
        }
        else
        {
            _ = Uri.TryCreate(baseUri, href, out resolved);
        }

        return resolved is not null &&
               (resolved.Scheme == Uri.UriSchemeHttp || resolved.Scheme == Uri.UriSchemeHttps)
            ? resolved
            : null;
    }

    private static bool IsLikelyHeadline(IElement anchor)
    {
        var identity = string.Join(' ',
            anchor.ClassName,
            anchor.GetAttribute("data-testid"),
            anchor.GetAttribute("data-field"),
            anchor.GetAttribute("aria-label"),
            anchor.ParentElement?.ClassName);
        return identity.Contains("headline", StringComparison.OrdinalIgnoreCase) ||
               identity.Contains("title", StringComparison.OrdinalIgnoreCase) ||
               identity.Contains("story", StringComparison.OrdinalIgnoreCase) ||
               identity.Contains("news", StringComparison.OrdinalIgnoreCase) ||
               anchor.ParentElement?.LocalName is "h1" or "h2" or "h3";
    }

    private static string Normalize(string value) => WhitespacePattern().Replace(value, " ").Trim();

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
    private static partial Regex WhitespacePattern();
}
