using SmartTicker.Core.Models;
using SmartTicker.Desktop.Localization;

namespace SmartTicker.Desktop.Tests;

public sealed class TranslationsTests
{
    [Fact]
    public void EverySupportedLanguageHasAnOptionAndStrings()
    {
        foreach (var code in AppLanguages.Supported)
        {
            Assert.Contains(Translations.Options, option => option.Code == code);

            var strings = Translations.For(code);
            Assert.False(string.IsNullOrWhiteSpace(strings.MenuQuotes), code);
            Assert.False(string.IsNullOrWhiteSpace(strings.MenuLanguage), code);
            Assert.False(string.IsNullOrWhiteSpace(strings.MenuExit), code);
            Assert.False(string.IsNullOrWhiteSpace(strings.MenuHelp), code);
            Assert.False(string.IsNullOrWhiteSpace(strings.StatusPaused), code);
            Assert.False(string.IsNullOrWhiteSpace(strings.TitleAppSettings), code);
        }
    }

    [Fact]
    public void OptionsAndSupportedCodesAgree()
    {
        Assert.Equal(
            AppLanguages.Supported.OrderBy(code => code, StringComparer.Ordinal),
            Translations.Options.Select(option => option.Code).OrderBy(code => code, StringComparer.Ordinal));
    }

    [Fact]
    public void UnknownLanguageFallsBackToEnglish()
    {
        Assert.Equal(Translations.For("en").MenuExit, Translations.For("xx").MenuExit);
    }
}
