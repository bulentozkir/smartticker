using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using SmartTicker.Core.Services;

namespace SmartTicker.Infrastructure.Networking;

internal sealed record WebsiteConsentForm(
    Uri ActionUri,
    IReadOnlyList<KeyValuePair<string, string>> HiddenFields,
    WebsiteConsentOption Accept,
    WebsiteConsentOption Reject,
    string Title,
    string Summary)
{
    private static readonly string[] AcceptTerms = ["accept", "agree", "allow", "approve"];
    private static readonly string[] RejectTerms = ["reject", "decline", "deny", "refuse", "necessary only"];

    public static bool TryParse(Uri pageUri, string html, out WebsiteConsentForm? consentForm)
    {
        consentForm = null;
        if (string.IsNullOrWhiteSpace(html))
        {
            return false;
        }

        var document = new HtmlParser().ParseDocument(html);
        var title = Normalize(document.Title);
        foreach (var form in document.QuerySelectorAll("form"))
        {
            if (!string.Equals(form.GetAttribute("method"), "post", StringComparison.OrdinalIgnoreCase) ||
                form.QuerySelector("input[type=password]") is not null)
            {
                continue;
            }

            var summary = Normalize(form.TextContent);
            var context = $"{title} {summary}";
            if (!context.Contains("privacy", StringComparison.OrdinalIgnoreCase) ||
                !(context.Contains("cookie", StringComparison.OrdinalIgnoreCase) ||
                  context.Contains("consent", StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var options = form.QuerySelectorAll("button, input[type=submit]")
                .Select(ToOption)
                .Where(option => option is not null)
                .Cast<WebsiteConsentOption>()
                .ToArray();
            var accept = options.FirstOrDefault(option => Matches(option, AcceptTerms));
            var reject = options.FirstOrDefault(option => Matches(option, RejectTerms));
            if (accept is null || reject is null || accept == reject)
            {
                continue;
            }

            var action = form.GetAttribute("action");
            Uri actionUri;
            try
            {
                actionUri = string.IsNullOrWhiteSpace(action) ? pageUri : new Uri(pageUri, action);
            }
            catch (UriFormatException)
            {
                continue;
            }

            var hiddenFields = form.QuerySelectorAll("input[type=hidden][name]")
                .Select(input => new KeyValuePair<string, string>(
                    input.GetAttribute("name")!,
                    input.GetAttribute("value") ?? string.Empty))
                .ToArray();
            consentForm = new WebsiteConsentForm(
                actionUri,
                hiddenFields,
                accept,
                reject,
                string.IsNullOrWhiteSpace(title) ? "Website privacy choices" : title,
                summary.Length <= 1600 ? summary : summary[..1597] + "...");
            return true;
        }

        return false;
    }

    public IReadOnlyList<KeyValuePair<string, string>> CreatePayload(WebsiteConsentDecision decision)
    {
        var option = decision == WebsiteConsentDecision.Accept ? Accept : Reject;
        return [.. HiddenFields, new KeyValuePair<string, string>(option.Name, option.Value)];
    }

    private static WebsiteConsentOption? ToOption(IElement element)
    {
        if (element.LocalName == "button" &&
            element.GetAttribute("type") is { } buttonType &&
            !string.Equals(buttonType, "submit", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var name = element.GetAttribute("name")?.Trim();
        var value = element.GetAttribute("value")?.Trim();
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var label = Normalize(element.LocalName == "input" ? value : element.TextContent);
        return new WebsiteConsentOption(
            string.IsNullOrWhiteSpace(label) ? value : label,
            name,
            value);
    }

    private static bool Matches(WebsiteConsentOption option, IEnumerable<string> terms)
    {
        var value = $"{option.Label} {option.Name} {option.Value}";
        return terms.Any(term => value.Contains(term, StringComparison.OrdinalIgnoreCase));
    }

    private static string Normalize(string? value) =>
        Regex.Replace(value ?? string.Empty, @"\s+", " ").Trim();
}

internal sealed record WebsiteConsentOption(string Label, string Name, string Value);