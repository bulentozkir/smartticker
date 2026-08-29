namespace SmartTicker.Core.Models;

/// <summary>The interface languages SmartTicker ships translations for.</summary>
public static class AppLanguages
{
    public const string Default = "en";

    public static IReadOnlyList<string> Supported { get; } =
        ["en", "ar", "de", "el", "es", "fr", "hi", "id", "it", "ja", "ko", "nl", "pt", "ru", "tr", "zh"];

    public static bool IsSupported(string? code) =>
        code is not null && Supported.Contains(code.Trim().ToLowerInvariant());

    public static string Normalize(string? code) =>
        IsSupported(code) ? code!.Trim().ToLowerInvariant() : Default;
}
