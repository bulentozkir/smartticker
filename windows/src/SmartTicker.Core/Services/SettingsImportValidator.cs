using System.Text.Json;
using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

public sealed record SettingsImportResult(SmartTickerSettings? Settings, IReadOnlyList<string> Errors)
{
    public bool Success => Settings is not null;

    private static SettingsImportResult Failed(string error) => new(null, [error]);

    internal static SettingsImportResult Rejected(string error) => Failed(error);

    internal static SettingsImportResult Rejected(IReadOnlyList<string> errors) => new(null, errors);

    internal static SettingsImportResult Accepted(SmartTickerSettings settings) => new(settings, []);
}

/// <summary>Validates untrusted settings JSON before any of it is applied.</summary>
public static class SettingsImportValidator
{
    public const int MaximumSubscriptions = 200;

    private const int MaximumReportedErrors = 25;

    private static readonly HashSet<string> RootProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "version",
        "subscriptions",
        "priceRowCount",
        "newsRowCount",
        "priceScrollSpeed",
        "backgroundOpacity",
        "newsScrollSpeed",
        "priceRefreshSeconds",
        "newsRefreshSeconds",
        "acknowledgedSources",
        "showPriceLine",
        "showNewsLine",
        "launchAtLogin",
        "backgroundColor",
        "symbolColor",
        "priceColor",
        "extendedPriceColor",
        "newsColor",
        "newsColor2",
        "newsColor3",
        "newsColor4",
        "priceUpColor",
        "priceDownColor",
        "language",
    };

    private static readonly HashSet<string> SubscriptionProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "id",
        "symbol",
        "sourceName",
        "sourceUri",
        "collectPrice",
        "collectNews",
        "cssSelector",
        "extendedCssSelector",
        "extendedChangeCssSelector",
        "changeCssSelector",
        "newsCssSelector",
        "newsRepeatLimit",
    };

    public static SettingsImportResult Validate(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return SettingsImportResult.Rejected("The file is empty, so there are no settings to import.");
        }

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(json, new JsonDocumentOptions { MaxDepth = 32 });
        }
        catch (JsonException exception)
        {
            return SettingsImportResult.Rejected(Describe(exception));
        }

        using (document)
        {
            var errors = new List<string>();
            var root = ReadObject(document.RootElement, string.Empty, errors);
            if (root is null)
            {
                return SettingsImportResult.Rejected(
                    $"The file must contain a JSON object at the top level, but it contains {Describe(document.RootElement.ValueKind)}.");
            }

            ReportUnknown(root, RootProperties, string.Empty, errors);
            ValidateVersion(root, errors);

            var settings = new SmartTickerSettings(
                SmartTickerSettings.CurrentVersion,
                ReadSubscriptions(root, errors),
                ReadInt(root, string.Empty, "priceRowCount", 1, 8, 1, errors),
                ReadInt(root, string.Empty, "newsRowCount", 1, 8, 1, errors),
                ReadInt(root, string.Empty, "priceScrollSpeed", 10, 200, 50, errors),
                ReadInt(root, string.Empty, "newsScrollSpeed", 10, 200, 40, errors))
            {
                AcknowledgedSources = ReadAcknowledgedSources(root, errors),
                PriceRefreshSeconds = ReadInt(
                    root,
                    string.Empty,
                    "priceRefreshSeconds",
                    SmartTickerSettings.MinimumRefreshSeconds,
                    SmartTickerSettings.MaximumRefreshSeconds,
                    SmartTickerSettings.DefaultPriceRefreshSeconds,
                    errors),
                NewsRefreshSeconds = ReadInt(
                    root,
                    string.Empty,
                    "newsRefreshSeconds",
                    SmartTickerSettings.MinimumRefreshSeconds,
                    SmartTickerSettings.MaximumRefreshSeconds,
                    SmartTickerSettings.DefaultNewsRefreshSeconds,
                    errors),
                ShowPriceLine = ReadBool(root, string.Empty, "showPriceLine", true, errors),
                ShowNewsLine = ReadBool(root, string.Empty, "showNewsLine", true, errors),
                LaunchAtLogin = ReadBool(root, string.Empty, "launchAtLogin", false, errors),
                BackgroundColor = ReadColor(root, "backgroundColor", SmartTickerSettings.DefaultBackgroundColor, errors),
                BackgroundOpacity = ReadFraction(
                    root,
                    "backgroundOpacity",
                    SmartTickerSettings.MinimumOpacity,
                    SmartTickerSettings.MaximumOpacity,
                    SmartTickerSettings.DefaultOpacity,
                    errors),
                SymbolColor = ReadColor(root, "symbolColor", SmartTickerSettings.DefaultSymbolColor, errors),
                ExtendedPriceColor = ReadColor(root, "extendedPriceColor", SmartTickerSettings.DefaultExtendedPriceColor, errors),
                PriceColor = ReadColor(root, "priceColor", SmartTickerSettings.DefaultPriceColor, errors),
                NewsColor = ReadColor(root, "newsColor", SmartTickerSettings.DefaultNewsColor, errors),
                NewsColor2 = ReadColor(root, "newsColor2", SmartTickerSettings.DefaultNewsColor2, errors),
                NewsColor3 = ReadColor(root, "newsColor3", SmartTickerSettings.DefaultNewsColor3, errors),
                NewsColor4 = ReadColor(root, "newsColor4", SmartTickerSettings.DefaultNewsColor4, errors),
                PriceUpColor = ReadColor(root, "priceUpColor", SmartTickerSettings.DefaultPriceUpColor, errors),
                PriceDownColor = ReadColor(root, "priceDownColor", SmartTickerSettings.DefaultPriceDownColor, errors),
                Language = ReadLanguage(root, errors),
            };

            return errors.Count > 0
                ? SettingsImportResult.Rejected(Summarize(errors))
                : SettingsImportResult.Accepted(settings.Normalize());
        }
    }

    private static string ReadLanguage(Dictionary<string, JsonElement> root, List<string> errors)
    {
        if (!root.TryGetValue("language", out var element))
        {
            return AppLanguages.Default;
        }

        if (element.ValueKind != JsonValueKind.String)
        {
            errors.Add($"'language' must be a quoted language code such as \"en\", but it is {Describe(element.ValueKind)}.");
            return AppLanguages.Default;
        }

        var code = element.GetString();
        if (!AppLanguages.IsSupported(code))
        {
            errors.Add($"'language' is \"{code}\", which SmartTicker does not translate. Supported codes: {string.Join(", ", AppLanguages.Supported)}.");
            return AppLanguages.Default;
        }

        return AppLanguages.Normalize(code);
    }

    private static void ValidateVersion(Dictionary<string, JsonElement> root, List<string> errors)
    {
        if (!root.TryGetValue("version", out var element))
        {
            errors.Add("'version' is missing. A SmartTicker settings file must declare the schema version it was written with.");
            return;
        }

        if (element.ValueKind != JsonValueKind.Number || !element.TryGetInt32(out var version))
        {
            errors.Add($"'version' must be a whole number, but it is {Describe(element.ValueKind)}.");
            return;
        }

        if (version == SmartTickerSettings.CurrentVersion)
        {
            return;
        }

        errors.Add(version > SmartTickerSettings.CurrentVersion
            ? $"'version' is {version}, but this build of SmartTicker only understands version {SmartTickerSettings.CurrentVersion}. The file was written by a newer version."
            : $"'version' is {version}, which is not a recognised SmartTicker settings version (expected {SmartTickerSettings.CurrentVersion}).");
    }

    private static TickerSubscription[] ReadSubscriptions(Dictionary<string, JsonElement> root, List<string> errors)
    {
        if (!root.TryGetValue("subscriptions", out var element))
        {
            return [];
        }

        if (element.ValueKind != JsonValueKind.Array)
        {
            errors.Add($"'subscriptions' must be a list of entries, but it is {Describe(element.ValueKind)}.");
            return [];
        }

        var count = element.GetArrayLength();
        if (count > MaximumSubscriptions)
        {
            errors.Add($"'subscriptions' contains {count} entries, which is more than the {MaximumSubscriptions} SmartTicker allows.");
            return [];
        }

        var identifiers = new HashSet<Guid>();
        var subscriptions = new List<TickerSubscription>(count);
        var index = 0;
        foreach (var item in element.EnumerateArray())
        {
            var subscription = ReadSubscription(item, $"subscriptions[{index++}]", identifiers, errors);
            if (subscription is not null)
            {
                subscriptions.Add(subscription);
            }
        }

        return subscriptions.ToArray();
    }

    private static TickerSubscription? ReadSubscription(
        JsonElement element,
        string path,
        HashSet<Guid> identifiers,
        List<string> errors)
    {
        var map = ReadObject(element, path, errors);
        if (map is null)
        {
            return null;
        }

        ReportUnknown(map, SubscriptionProperties, path, errors);

        var id = ReadId(map, path, identifiers, errors);
        var symbol = ReadString(map, path, "symbol", errors, required: true);
        var sourceName = ReadString(map, path, "sourceName", errors);
        var sourceUri = ReadSourceUri(map, path, errors);
        var collectPrice = ReadBool(map, path, "collectPrice", false, errors);
        var collectNews = ReadBool(map, path, "collectNews", false, errors);
        var cssSelector = ReadString(map, path, "cssSelector", errors);
        var extendedCssSelector = ReadString(map, path, "extendedCssSelector", errors);
        var extendedChangeCssSelector = ReadString(map, path, "extendedChangeCssSelector", errors);
        var changeCssSelector = ReadString(map, path, "changeCssSelector", errors);
        var newsCssSelector = ReadString(map, path, "newsCssSelector", errors);
        var repeatLimit = ReadInt(
            map,
            path,
            "newsRepeatLimit",
            1,
            100,
            TickerSubscription.DefaultNewsRepeatLimit,
            errors);

        if (!collectPrice && !collectNews)
        {
            errors.Add($"'{path}' has both 'collectPrice' and 'collectNews' set to false, so the entry would never show anything.");
        }

        if (id is null || symbol is null || sourceUri is null)
        {
            return null;
        }

        return new TickerSubscription(
            id.Value,
            symbol.Trim(),
            string.IsNullOrWhiteSpace(sourceName) ? sourceUri.Host : sourceName.Trim(),
            sourceUri,
            collectPrice,
            collectNews,
            string.IsNullOrWhiteSpace(cssSelector) ? null : cssSelector.Trim(),
            string.IsNullOrWhiteSpace(newsCssSelector) ? null : newsCssSelector.Trim())
        {
            NewsRepeatLimit = repeatLimit,
            ExtendedCssSelector = string.IsNullOrWhiteSpace(extendedCssSelector) ? null : extendedCssSelector.Trim(),
            ExtendedChangeCssSelector = string.IsNullOrWhiteSpace(extendedChangeCssSelector) ? null : extendedChangeCssSelector.Trim(),
            ChangeCssSelector = string.IsNullOrWhiteSpace(changeCssSelector) ? null : changeCssSelector.Trim(),
        };
    }

    private static Guid? ReadId(
        Dictionary<string, JsonElement> map,
        string path,
        HashSet<Guid> identifiers,
        List<string> errors)
    {
        if (!map.TryGetValue("id", out var element) || element.ValueKind == JsonValueKind.Null)
        {
            return Guid.NewGuid();
        }

        if (element.ValueKind != JsonValueKind.String || !Guid.TryParse(element.GetString(), out var id))
        {
            errors.Add($"'{path}.id' must be a GUID such as \"7c9e6679-7425-40de-944b-e07fc1f90ae7\".");
            return null;
        }

        if (!identifiers.Add(id))
        {
            errors.Add($"'{path}.id' repeats the identifier {id}, which is already used by an earlier entry.");
            return null;
        }

        return id;
    }

    private static Uri? ReadSourceUri(Dictionary<string, JsonElement> map, string path, List<string> errors)
    {
        var text = ReadString(map, path, "sourceUri", errors, required: true);
        if (text is null)
        {
            return null;
        }

        if (!Uri.TryCreate(text.Trim(), UriKind.Absolute, out var uri))
        {
            errors.Add($"'{path}.sourceUri' is not a valid absolute URL: \"{text}\".");
            return null;
        }

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            errors.Add($"'{path}.sourceUri' must use http or https, but it uses '{uri.Scheme}'. SmartTicker only reads web pages.");
            return null;
        }

        return uri;
    }

    private static string[] ReadAcknowledgedSources(Dictionary<string, JsonElement> root, List<string> errors)
    {
        if (!root.TryGetValue("acknowledgedSources", out var element))
        {
            return [];
        }

        if (element.ValueKind != JsonValueKind.Array)
        {
            errors.Add($"'acknowledgedSources' must be a list of host names, but it is {Describe(element.ValueKind)}.");
            return [];
        }

        var hosts = new List<string>();
        var index = 0;
        foreach (var item in element.EnumerateArray())
        {
            var path = $"acknowledgedSources[{index++}]";
            if (item.ValueKind != JsonValueKind.String)
            {
                errors.Add($"'{path}' must be a quoted host name such as \"finance.yahoo.com\".");
                continue;
            }

            var host = item.GetString();
            if (string.IsNullOrWhiteSpace(host) || Uri.CheckHostName(host) == UriHostNameType.Unknown)
            {
                errors.Add($"'{path}' is not a valid host name: \"{host}\".");
                continue;
            }

            hosts.Add(host);
        }

        return hosts.ToArray();
    }

    private static Dictionary<string, JsonElement>? ReadObject(JsonElement element, string path, List<string> errors)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            if (path.Length > 0)
            {
                errors.Add($"'{path}' must be an object, but it is {Describe(element.ValueKind)}.");
            }

            return null;
        }

        var map = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        foreach (var property in element.EnumerateObject())
        {
            if (!map.TryAdd(property.Name, property.Value))
            {
                errors.Add($"'{Field(path, property.Name)}' is declared more than once.");
            }
        }

        return map;
    }

    private static void ReportUnknown(
        Dictionary<string, JsonElement> map,
        HashSet<string> known,
        string path,
        List<string> errors)
    {
        foreach (var name in map.Keys)
        {
            if (!known.Contains(name))
            {
                errors.Add($"'{Field(path, name)}' is not a SmartTicker setting. Check the spelling or remove it.");
            }
        }
    }

    private static double ReadFraction(
        Dictionary<string, JsonElement> map,
        string name,
        double minimum,
        double maximum,
        double fallback,
        List<string> errors)
    {
        if (!map.TryGetValue(name, out var element))
        {
            return fallback;
        }

        if (element.ValueKind != JsonValueKind.Number || !element.TryGetDouble(out var value) || !double.IsFinite(value))
        {
            errors.Add($"'{name}' must be a number between {minimum} and {maximum}, but it is {Describe(element.ValueKind)}.");
            return fallback;
        }

        if (value < minimum || value > maximum)
        {
            errors.Add($"'{name}' is {value}, which is outside the allowed range {minimum}-{maximum}.");
            return fallback;
        }

        return value;
    }

    private static int ReadInt(
        Dictionary<string, JsonElement> map,
        string path,
        string name,
        int minimum,
        int maximum,
        int fallback,
        List<string> errors)
    {
        if (!map.TryGetValue(name, out var element))
        {
            return fallback;
        }

        if (element.ValueKind != JsonValueKind.Number || !element.TryGetInt32(out var value))
        {
            errors.Add($"'{Field(path, name)}' must be a whole number between {minimum} and {maximum}, but it is {Describe(element.ValueKind)}.");
            return fallback;
        }

        if (value < minimum || value > maximum)
        {
            errors.Add($"'{Field(path, name)}' is {value}, which is outside the allowed range {minimum}-{maximum}.");
            return fallback;
        }

        return value;
    }

    private static bool ReadBool(
        Dictionary<string, JsonElement> map,
        string path,
        string name,
        bool fallback,
        List<string> errors)
    {
        if (!map.TryGetValue(name, out var element))
        {
            return fallback;
        }

        if (element.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            return element.GetBoolean();
        }

        errors.Add($"'{Field(path, name)}' must be true or false, but it is {Describe(element.ValueKind)}.");
        return fallback;
    }

    private static string? ReadString(
        Dictionary<string, JsonElement> map,
        string path,
        string name,
        List<string> errors,
        bool required = false)
    {
        if (!map.TryGetValue(name, out var element) || element.ValueKind == JsonValueKind.Null)
        {
            if (required)
            {
                errors.Add($"'{Field(path, name)}' is required.");
            }

            return null;
        }

        if (element.ValueKind != JsonValueKind.String)
        {
            errors.Add($"'{Field(path, name)}' must be text in quotes, but it is {Describe(element.ValueKind)}.");
            return null;
        }

        var value = element.GetString();
        if (required && string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"'{Field(path, name)}' cannot be blank.");
            return null;
        }

        return value;
    }

    private static string ReadColor(
        Dictionary<string, JsonElement> root,
        string name,
        string fallback,
        List<string> errors)
    {
        if (!root.TryGetValue(name, out var element))
        {
            return fallback;
        }

        if (element.ValueKind != JsonValueKind.String)
        {
            errors.Add($"'{name}' must be a hex color in quotes such as \"{fallback}\", but it is {Describe(element.ValueKind)}.");
            return fallback;
        }

        if (!HexColor.TryNormalize(element.GetString(), out var color))
        {
            errors.Add($"'{name}' is \"{element.GetString()}\", which is not a hex color. Use a value such as \"{fallback}\".");
            return fallback;
        }

        return color;
    }

    private static IReadOnlyList<string> Summarize(List<string> errors)
    {
        if (errors.Count <= MaximumReportedErrors)
        {
            return errors;
        }

        var trimmed = errors.Take(MaximumReportedErrors).ToList();
        trimmed.Add($"…and {errors.Count - MaximumReportedErrors} more problems.");
        return trimmed;
    }

    private static string Field(string path, string name) => path.Length == 0 ? name : $"{path}.{name}";

    private static string Describe(JsonValueKind kind) => kind switch
    {
        JsonValueKind.Object => "an object",
        JsonValueKind.Array => "a list",
        JsonValueKind.String => "text",
        JsonValueKind.Number => "a number",
        JsonValueKind.True or JsonValueKind.False => "a true/false value",
        JsonValueKind.Null => "null",
        _ => "an unexpected value",
    };

    private static string Describe(JsonException exception)
    {
        var message = exception.Message;
        var marker = message.IndexOf(" LineNumber:", StringComparison.Ordinal);
        if (marker >= 0)
        {
            message = message[..marker];
        }

        var location = exception.LineNumber is { } line && exception.BytePositionInLine is { } position
            ? $" at line {line + 1}, position {position + 1}"
            : string.Empty;

        return $"The file is not valid JSON{location}. {message}";
    }
}
