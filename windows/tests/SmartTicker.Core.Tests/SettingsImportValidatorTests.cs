using SmartTicker.Core.Models;
using SmartTicker.Core.Services;

namespace SmartTicker.Core.Tests;

public class SettingsImportValidatorTests
{
    private const string ValidJson = """
    {
      "version": 1,
      "subscriptions": [
        {
          "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
          "symbol": "MSFT",
          "sourceName": "Yahoo Finance",
          "sourceUri": "https://finance.yahoo.com/quote/MSFT",
          "collectPrice": true,
          "collectNews": true,
          "cssSelector": null,
          "newsCssSelector": "a.titles",
          "newsRepeatLimit": 5
        }
      ],
      "priceRowCount": 2,
      "newsRowCount": 1,
      "priceScrollSpeed": 50,
      "newsScrollSpeed": 40,
      "acknowledgedSources": ["finance.yahoo.com"],
      "showPriceLine": true,
      "showNewsLine": false,
      "backgroundColor": "#10151D",
      "priceColor": "#70E1A1",
      "newsColor": "#D8DEE9",
      "priceUpColor": "#3FB950",
      "priceDownColor": "#F85149"
    }
    """;

    [Fact]
    public void Validate_AcceptsAnExportedFile()
    {
        var result = SettingsImportValidator.Validate(ValidJson);

        Assert.True(result.Success);
        Assert.Empty(result.Errors);

        var settings = result.Settings!;
        Assert.Equal(2, settings.PriceRowCount);
        Assert.False(settings.ShowNewsLine);
        Assert.Equal("#F85149", settings.PriceDownColor);
        Assert.Equal(["finance.yahoo.com"], settings.AcknowledgedSources);

        var subscription = Assert.Single(settings.Subscriptions);
        Assert.Equal("MSFT", subscription.Symbol);
        Assert.Equal("a.titles", subscription.NewsCssSelector);
        Assert.Equal(5, subscription.NewsRepeatLimit);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_RejectsEmptyInput(string? json)
    {
        var result = SettingsImportValidator.Validate(json);

        Assert.False(result.Success);
        Assert.Contains("empty", Assert.Single(result.Errors));
    }

    [Fact]
    public void Validate_RejectsMalformedJsonWithLocation()
    {
        var result = SettingsImportValidator.Validate("{ \"version\": 1, }");

        Assert.False(result.Success);
        var error = Assert.Single(result.Errors);
        Assert.Contains("not valid JSON", error);
        Assert.Contains("line", error);
    }

    [Fact]
    public void Validate_RejectsANonObjectRoot()
    {
        var result = SettingsImportValidator.Validate("[1, 2, 3]");

        Assert.False(result.Success);
        Assert.Contains("top level", Assert.Single(result.Errors));
    }

    [Fact]
    public void Validate_RequiresAVersion()
    {
        var result = SettingsImportValidator.Validate("{ \"priceRowCount\": 1 }");

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("'version' is missing"));
    }

    [Fact]
    public void Validate_RejectsANewerSchemaVersion()
    {
        var result = SettingsImportValidator.Validate("{ \"version\": 99 }");

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("newer version"));
    }

    [Fact]
    public void Validate_RejectsUnknownProperties()
    {
        var result = SettingsImportValidator.Validate("{ \"version\": 1, \"priceColour\": \"#FFFFFF\" }");

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("'priceColour' is not a SmartTicker setting"));
    }

    [Fact]
    public void Validate_RejectsOutOfRangeNumbers()
    {
        var result = SettingsImportValidator.Validate("{ \"version\": 1, \"priceRowCount\": 40 }");

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("outside the allowed range 1-8"));
    }

    [Fact]
    public void Validate_RejectsWrongTypes()
    {
        var result = SettingsImportValidator.Validate("{ \"version\": 1, \"showNewsLine\": \"yes\", \"newsRowCount\": \"two\" }");

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("'showNewsLine' must be true or false"));
        Assert.Contains(result.Errors, error => error.Contains("'newsRowCount' must be a whole number"));
    }

    [Fact]
    public void Validate_RejectsAnInvalidColor()
    {
        var result = SettingsImportValidator.Validate("{ \"version\": 1, \"priceColor\": \"bright green\" }");

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("not a hex color"));
    }

    [Fact]
    public void Validate_RejectsANonWebSourceUri()
    {
        var json = """
        {
          "version": 1,
          "subscriptions": [
            { "symbol": "X", "sourceUri": "file:///C:/secrets.txt", "collectPrice": true, "collectNews": false }
          ]
        }
        """;

        var result = SettingsImportValidator.Validate(json);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("must use http or https"));
    }

    [Fact]
    public void Validate_RejectsASubscriptionMissingRequiredFields()
    {
        var json = """
        {
          "version": 1,
          "subscriptions": [ { "collectPrice": true } ]
        }
        """;

        var result = SettingsImportValidator.Validate(json);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("'subscriptions[0].symbol' is required"));
        Assert.Contains(result.Errors, error => error.Contains("'subscriptions[0].sourceUri' is required"));
    }

    [Fact]
    public void Validate_RejectsASubscriptionThatCollectsNothing()
    {
        var json = """
        {
          "version": 1,
          "subscriptions": [
            { "symbol": "X", "sourceUri": "https://example.com", "collectPrice": false, "collectNews": false }
          ]
        }
        """;

        var result = SettingsImportValidator.Validate(json);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("would never show anything"));
    }

    [Fact]
    public void Validate_RejectsDuplicateSubscriptionIds()
    {
        var json = """
        {
          "version": 1,
          "subscriptions": [
            { "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7", "symbol": "A", "sourceUri": "https://example.com", "collectPrice": true, "collectNews": false },
            { "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7", "symbol": "B", "sourceUri": "https://example.com", "collectPrice": true, "collectNews": false }
          ]
        }
        """;

        var result = SettingsImportValidator.Validate(json);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("repeats the identifier"));
    }

    [Fact]
    public void Validate_RejectsAnInvalidAcknowledgedSource()
    {
        var result = SettingsImportValidator.Validate("{ \"version\": 1, \"acknowledgedSources\": [\"not a host\"] }");

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("not a valid host name"));
    }

    [Fact]
    public void Validate_RejectsDuplicateProperties()
    {
        var result = SettingsImportValidator.Validate("{ \"version\": 1, \"priceRowCount\": 1, \"priceRowCount\": 2 }");

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("declared more than once"));
    }

    [Fact]
    public void Validate_ReportsEveryProblemAtOnce()
    {
        var result = SettingsImportValidator.Validate("{ \"version\": 1, \"priceRowCount\": 40, \"newsScrollSpeed\": 5, \"priceColor\": \"nope\" }");

        Assert.False(result.Success);
        Assert.Equal(3, result.Errors.Count);
    }

    [Fact]
    public void Validate_RejectsARefreshIntervalOutsideTheAllowedRange()
    {
        var tooFast = SettingsImportValidator.Validate("{ \"version\": 1, \"priceRefreshSeconds\": 5 }");
        var tooSlow = SettingsImportValidator.Validate("{ \"version\": 1, \"newsRefreshSeconds\": 3600 }");

        Assert.False(tooFast.Success);
        Assert.Contains(tooFast.Errors, error => error.Contains("'priceRefreshSeconds' is 5, which is outside the allowed range 30-300"));
        Assert.False(tooSlow.Success);
        Assert.Contains(tooSlow.Errors, error => error.Contains("'newsRefreshSeconds' is 3600, which is outside the allowed range 30-300"));
    }

    [Fact]
    public void Validate_RoundTripsSerializedSettings()
    {
        var original = SmartTickerSettings.Default with
        {
            PriceRowCount = 3,
            NewsScrollSpeed = 120,
            PriceRefreshSeconds = 45,
            AllowWebsiteCookiesAndCrossHostRedirects = true,
        };

        var result = SettingsImportValidator.Validate(SettingsJson.Serialize(original));

        Assert.True(result.Success);
        Assert.Equal(3, result.Settings!.PriceRowCount);
        Assert.Equal(120, result.Settings.NewsScrollSpeed);
        Assert.Equal(45, result.Settings.PriceRefreshSeconds);
        Assert.True(result.Settings.AllowWebsiteCookiesAndCrossHostRedirects);
    }
}
