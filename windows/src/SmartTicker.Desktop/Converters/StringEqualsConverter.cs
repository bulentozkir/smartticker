using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace SmartTicker.Desktop.Converters;

/// <summary>Marks the menu item whose parameter matches the bound value.</summary>
public sealed class StringEqualsConverter : IValueConverter
{
    public static readonly StringEqualsConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        string.Equals(value as string, parameter as string, StringComparison.OrdinalIgnoreCase);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        BindingOperations.DoNothing;
}
