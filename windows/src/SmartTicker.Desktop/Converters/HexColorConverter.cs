using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using SmartTicker.Core.Services;

namespace SmartTicker.Desktop.Converters;

/// <summary>Bridges the stored hex string and the colour picker's Color value.</summary>
public sealed class HexColorConverter : IValueConverter
{
    public static readonly HexColorConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        HexColor.TryNormalize(value as string, out var normalized) ? Color.Parse(normalized) : Colors.Transparent;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is Color color ? $"#{color.R:X2}{color.G:X2}{color.B:X2}" : string.Empty;
}
