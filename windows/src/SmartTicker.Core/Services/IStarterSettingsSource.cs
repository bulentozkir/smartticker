namespace SmartTicker.Core.Services;

/// <summary>Fetches the published starter configuration so a new install has something to show.</summary>
public interface IStarterSettingsSource
{
    Uri Location { get; }

    Task<string> DownloadAsync(CancellationToken cancellationToken = default);
}
