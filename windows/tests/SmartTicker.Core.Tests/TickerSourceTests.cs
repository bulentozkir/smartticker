using SmartTicker.Core.Models;

namespace SmartTicker.Core.Tests;

public sealed class TickerSourceTests
{
    [Fact]
    public void TryCreate_NormalizesValidSource()
    {
        var valid = TickerSource.TryCreate(
            " msft ",
            "Microsoft",
            "https://example.com/quote/msft",
            ".price",
            "usd",
            out var source,
            out var error);

        Assert.True(valid, error);
        Assert.NotNull(source);
        Assert.Equal("MSFT", source.Symbol);
        Assert.Equal("USD", source.Currency);
    }

    [Theory]
    [InlineData("file:///c:/secret.txt")]
    [InlineData("https://user:password@example.com/quote")]
    [InlineData("not-a-url")]
    public void TryCreate_RejectsUnsafeOrInvalidUrl(string url)
    {
        var valid = TickerSource.TryCreate("MSFT", "Microsoft", url, null, null, out _, out var error);

        Assert.False(valid);
        Assert.NotNull(error);
    }

    [Fact]
    public void Subscription_AllowsSameTickerFromDifferentSources()
    {
        var firstValid = TickerSubscription.TryCreate(
            "MSFT", "Source A", "https://example.com/a/MSFT", true, false, null, out var first, out _);
        var secondValid = TickerSubscription.TryCreate(
            "MSFT", "Source B", "https://example.com/b/MSFT", false, true, null, out var second, out _);

        Assert.True(firstValid);
        Assert.True(secondValid);
        Assert.Equal(first!.Symbol, second!.Symbol);
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void Subscription_RequiresPriceOrNewsSelection()
    {
        var valid = TickerSubscription.TryCreate(
            "MSFT", "Source", "https://example.com/MSFT", false, false, null, out _, out var error);

        Assert.False(valid);
        Assert.Equal("Select price, news, or both.", error);
    }

    [Fact]
    public void Subscription_UpdatePreservesIdentityAndSeparateSelectors()
    {
        TickerSubscription.TryCreate(
            "MSFT", "Source", "https://example.com/MSFT", true, false, ".old-price",
            out var original, out _);

        var valid = TickerSubscription.TryUpdate(
            original!, " msft ", "Updated", "https://example.com/new/MSFT", true, true,
            ".price", "a.headline", out var updated, out var error);

        Assert.True(valid, error);
        Assert.Equal(original!.Id, updated!.Id);
        Assert.Equal(".price", updated.CssSelector);
        Assert.Equal("a.headline", updated.NewsCssSelector);
        Assert.True(updated.CollectNews);
    }

    [Fact]
    public void SourcePreset_ComposesPrefixAndSuffix()
    {
        var preset = new SourcePreset(
            "Yahoo Finance",
            new Uri("https://finance.yahoo.com/"),
            CollectionPolicy.CheckSitePolicy,
            "Test");

        var url = preset.ComposeUrl("/quote/MSFT/");

        Assert.Equal("https://finance.yahoo.com/quote/MSFT/", url);
        Assert.True(preset.TryGetSuffix(new Uri(url), out var suffix));
        Assert.Equal("quote/MSFT/", suffix);
    }

    [Fact]
    public void SourcePreset_CustomOptionUsesCompleteUrl()
    {
        var preset = new SourcePreset("Custom URL", null, CollectionPolicy.CheckSitePolicy, "Test");

        var url = preset.ComposeUrl(" https://example.com/quote/MSFT ");

        Assert.Equal("https://example.com/quote/MSFT", url);
    }
}