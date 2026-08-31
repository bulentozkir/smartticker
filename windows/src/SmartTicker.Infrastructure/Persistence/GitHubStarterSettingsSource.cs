using SmartTicker.Core.Services;
using SmartTicker.Infrastructure.Networking;

namespace SmartTicker.Infrastructure.Persistence;

public sealed class GitHubStarterSettingsSource : IStarterSettingsSource, IDisposable
{
    private readonly PublicHtmlClient _client;

    public GitHubStarterSettingsSource(WebsiteAccessPolicy? accessPolicy = null)
    {
        _client = new PublicHtmlClient(accessPolicy);
    }

    public Uri Location { get; } = new(
        "https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/samples/smartticker-settings.sample.json");

    public Task<string> DownloadAsync(CancellationToken cancellationToken = default) =>
        _client.GetJsonAsync(Location, cancellationToken);

    public void Dispose() => _client.Dispose();
}
