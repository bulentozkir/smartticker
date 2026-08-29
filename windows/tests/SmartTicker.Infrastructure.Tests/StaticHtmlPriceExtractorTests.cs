using SmartTicker.Core.Models;
using SmartTicker.Infrastructure.Extraction;

namespace SmartTicker.Infrastructure.Tests;

public sealed class StaticHtmlPriceExtractorTests
{
    private static readonly TickerSource Source = new(
        "TEST",
        "Test",
        new Uri("https://example.com/quote"),
        Currency: "USD");

    [Fact]
    public void Extract_UsesExplicitSelectorFirst()
    {
        const string html = "<html><span class='price'>$1,234.56</span><meta itemprop='price' content='999.00'></html>";
        var source = Source with { CssSelector = ".price" };

        var result = new StaticHtmlPriceExtractor().Extract(html, source);

        Assert.True(result.Success, result.Message);
        Assert.Equal(1234.56m, result.Price);
        Assert.Equal("CSS selector", result.Method);
    }

    [Fact]
    public void Extract_ReadsJsonLdPrice()
    {
        const string html = """
            <script type="application/ld+json">
            { "@type": "Offer", "price": "183.42", "priceCurrency": "USD" }
            </script>
            """;

        var result = new StaticHtmlPriceExtractor().Extract(html, Source);

        Assert.True(result.Success, result.Message);
        Assert.Equal(183.42m, result.Price);
        Assert.Equal("Structured data", result.Method);
    }

    [Fact]
    public void Extract_RejectsAmbiguousStructuredPrices()
    {
        const string html = "<meta itemprop='price' content='10.00'><meta property='og:price:amount' content='11.00'>";

        var result = new StaticHtmlPriceExtractor().Extract(html, Source);

        Assert.False(result.Success);
        Assert.Contains("multiple", result.Message, StringComparison.OrdinalIgnoreCase);
    }
}