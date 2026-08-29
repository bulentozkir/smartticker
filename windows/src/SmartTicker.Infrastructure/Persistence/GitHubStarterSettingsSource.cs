using SmartTicker.Core.Services;
using SmartTicker.Infrastructure.Networking;

namespace SmartTicker.Infrastructure.Persistence;

public sealed class GitHubStarterSettingsSource : IStarterSettingsSource, IDisposable
{
    private readonly PublicHtmlClient _client = new();

    public Uri Location { get; } = new(
        "https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/samples/smartticker-settings.sample.json");

    public Task<string> DownloadAsync(CancellationToken cancellationToken = default) =>
        _client.GetStringAsync(Location, cancellationToken);

    public void Dispose() => _client.Dispose();
}
