namespace SmartTicker.Core.Services;

/// <summary>Records, per website host, that the user confirmed they may collect from it.</summary>
public sealed class SourceAcknowledgementLedger
{
    private readonly HashSet<string> _hosts;

    public SourceAcknowledgementLedger(IEnumerable<string>? acknowledgedHosts = null)
    {
        _hosts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var host in acknowledgedHosts ?? [])
        {
            if (!string.IsNullOrWhiteSpace(host))
            {
                _hosts.Add(host.Trim());
            }
        }
    }

    public static string? HostOf(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            ? uri.Host
            : null;

    public bool IsAcknowledged(string? url)
    {
        var host = HostOf(url);
        return host is not null && _hosts.Contains(host);
    }

    public bool Acknowledge(string? url)
    {
        var host = HostOf(url);
        if (host is null)
        {
            return false;
        }

        _hosts.Add(host);
        return true;
    }

    public string[] ToArray() => [.. _hosts.Order(StringComparer.OrdinalIgnoreCase)];
}
