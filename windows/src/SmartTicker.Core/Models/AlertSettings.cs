namespace SmartTicker.Core.Models;

/// <summary>Alert rules live in their own file so they survive a settings import.</summary>
public sealed record AlertSettings
{
    public const int CurrentVersion = 1;
    public const int MinimumBlinkSeconds = 5;
    public const int MaximumBlinkSeconds = 900;
    public const int DefaultBlinkSeconds = 60;
    public const int MinimumBuzzCount = 1;
    public const int MaximumBuzzCount = 20;
    public const int DefaultBuzzCount = 15;

    public int BuzzCount { get; init; } = DefaultBuzzCount;

    public int Version { get; init; } = CurrentVersion;

    public AlertRule[] Rules { get; init; } = [];

    public bool SoundEnabled { get; init; } = true;

    public int BlinkSeconds { get; init; } = DefaultBlinkSeconds;

    public static AlertSettings Default => new();

    public AlertSettings Normalize() => this with
    {
        Version = CurrentVersion,
        Rules = Rules ?? [],
        BlinkSeconds = Math.Clamp(BlinkSeconds, MinimumBlinkSeconds, MaximumBlinkSeconds),
        BuzzCount = Math.Clamp(BuzzCount, MinimumBuzzCount, MaximumBuzzCount),
    };
}
