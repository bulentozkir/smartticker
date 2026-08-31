using System.Net;

namespace SmartTicker.Core.Services;

public sealed class WebsiteAccessPolicy
{
    private readonly object _approvedHostsLock = new();
    private readonly SemaphoreSlim _consentPromptGate = new(1, 1);
    private HashSet<string> _approvedHosts = new(StringComparer.OrdinalIgnoreCase);

    public bool AllowCookiesAndCrossHostRedirects { get; set; }

    public CookieContainer SessionCookies { get; } = new();

    public Func<WebsiteConsentRequest, CancellationToken, Task<WebsiteConsentDecision>>? ConsentPrompt { get; set; }

    public bool AllowsWebsiteSession(Uri sourceUri)
    {
        if (AllowCookiesAndCrossHostRedirects)
        {
            return true;
        }

        lock (_approvedHostsLock)
        {
            return _approvedHosts.Contains(sourceUri.Host);
        }
    }

    public void ReplaceApprovedHosts(IEnumerable<string> hosts)
    {
        lock (_approvedHostsLock)
        {
            _approvedHosts = new HashSet<string>(
                hosts.Where(host => !string.IsNullOrWhiteSpace(host)).Select(host => host.Trim()),
                StringComparer.OrdinalIgnoreCase);
        }
    }

    public async Task<WebsiteConsentDecision> RequestConsentAsync(
        WebsiteConsentRequest request,
        CancellationToken cancellationToken)
    {
        await _consentPromptGate.WaitAsync(cancellationToken);
        try
        {
            return ConsentPrompt is null
                ? WebsiteConsentDecision.Cancel
                : await ConsentPrompt(request, cancellationToken);
        }
        finally
        {
            _consentPromptGate.Release();
        }
    }
}

public sealed record WebsiteConsentRequest(
    Uri SourceUri,
    Uri ConsentUri,
    string Title,
    string Summary,
    string AcceptLabel,
    string RejectLabel);

public enum WebsiteConsentDecision
{
    Cancel,
    Reject,
    Accept,
}