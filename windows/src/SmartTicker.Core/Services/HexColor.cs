namespace SmartTicker.Core.Services;

public static class HexColor
{
    /// <summary>Accepts #RGB, #RRGGBB, or #AARRGGBB with or without the leading hash.</summary>
    public static bool TryNormalize(string? value, out string normalized)
    {
        normalized = string.Empty;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var digits = value.Trim().TrimStart('#');
        if (digits.Length is not (3 or 6 or 8) || !digits.All(Uri.IsHexDigit))
        {
            return false;
        }

        if (digits.Length == 3)
        {
            digits = string.Concat(digits.Select(digit => new string(digit, 2)));
        }

        normalized = "#" + digits.ToUpperInvariant();
        return true;
    }
}
