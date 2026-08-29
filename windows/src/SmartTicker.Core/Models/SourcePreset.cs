namespace SmartTicker.Core.Models;

public sealed record SourcePreset(
    string Name,
    Uri? HomePage,
    CollectionPolicy CollectionPolicy,
    string Guidance)
{
    public string UrlPrefix => HomePage?.AbsoluteUri ?? string.Empty;

    public string PolicySummary => CollectionPolicy switch
    {
        CollectionPolicy.RequiresWrittenPermission => "Written permission required",
        CollectionPolicy.CheckSitePolicy => "Check site policy",
        _ => "Source you provide",
    };

    public string ComposeUrl(string? suffix)
    {
        var value = suffix?.Trim() ?? string.Empty;
        return HomePage is null
            ? value
            : HomePage.AbsoluteUri + value.TrimStart('/');
    }

    public bool TryGetSuffix(Uri sourceUri, out string suffix)
    {
        suffix = string.Empty;
        if (HomePage is null)
        {
            return false;
        }

        var source = sourceUri.AbsoluteUri;
        var prefix = HomePage.AbsoluteUri;
        if (!source.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        suffix = source[prefix.Length..];
        return true;
    }
}