using SmartTicker.Core.Models;
using SmartTicker.Infrastructure.Extraction;

namespace SmartTicker.Infrastructure.Tests;

public sealed class ExtendedPriceExtractionTests
{
    // Mirrors the live CNBC quote strip: the after-hours price appears first in the document.
    private const string CnbcHtml = """
        <div class="QuoteStrip-container">
          <div class="QuoteStrip-extendedDataContainer QuoteStrip-dataContainer">
            <span class="QuoteStrip-lastPrice">513.05</span>
            <span class="QuoteStrip-changeDown">(-0.09%)</span>
          </div>
          <div class="QuoteStrip-dataContainer QuoteStrip-extendedHours">
            <span class="QuoteStrip-lastPrice">513.53</span>
            <span class="QuoteStrip-changeUp">(+0.25%)</span>
          </div>
        </div>
        """;

    private static TickerSource Source(string selector) =>
        new("MSFT", "MSFT", new Uri("https://www.cnbc.com/quotes/MSFT"), selector);

    [Fact]
    public void CloseSelector_SkipsTheAfterHoursBlock()
    {
        var result = new StaticHtmlPriceExtractor().Extract(
            CnbcHtml,
            Source(".QuoteStrip-dataContainer:not(.QuoteStrip-extendedDataContainer) .QuoteStrip-lastPrice"));

        Assert.True(result.Success, result.Message);
        Assert.Equal(513.53m, result.Price);
    }

    [Fact]
    public void ExtendedSelector_ReadsTheAfterHoursPrice()
    {
        var result = new StaticHtmlPriceExtractor().Extract(
            CnbcHtml,
            Source(".QuoteStrip-extendedDataContainer .QuoteStrip-lastPrice"));

        Assert.True(result.Success, result.Message);
        Assert.Equal(513.05m, result.Price);
    }

    [Fact]
    public void ExtendedChangeSelector_ReadsTheAfterHoursChange()
    {
        var change = new StaticHtmlChangeExtractor().Extract(
            CnbcHtml,
            ".QuoteStrip-extendedDataContainer [class*=changeUp],.QuoteStrip-extendedDataContainer [class*=changeDown]");

        Assert.Equal(-0.09m, change);
    }

    [Fact]
    public void YahooUsesSeparateSelectorsForCloseAndPostMarket()
    {
        const string html = """
            <span data-testid="qsp-price">513.53</span>
            <span data-testid="qsp-post-price">513.06</span>
            """;
        var extractor = new StaticHtmlPriceExtractor();

        Assert.Equal(513.53m, extractor.Extract(html, Source("[data-testid=\"qsp-price\"]")).Price);
        Assert.Equal(513.06m, extractor.Extract(html, Source("[data-testid=\"qsp-post-price\"]")).Price);
    }

    [Fact]
    public void YahooSnapshotIncludesClosePreMarketAndPostMarketValues()
    {
        const string html = """
            <section class="primary">
              <span data-testid="qsp-price">513.53</span>
            </section>
            <section class="secondary">
              <span data-testid="qsp-pre-price">516.20</span>
              <span data-testid="qsp-pre-price-change-percent">(+0.52%)</span>
              <span data-testid="qsp-post-price">513.06</span>
              <span data-testid="qsp-post-price-change-percent">(-0.09%)</span>
            </section>
            """;
        var subscription = new TickerSubscription(
            Guid.NewGuid(),
            "MSFT",
            "Yahoo Finance",
            new Uri("https://finance.yahoo.com/quote/MSFT/"),
            true,
            false,
            "[data-testid=\"qsp-price\"]")
        {
            PreMarketCssSelector = "section.secondary span[data-testid=\"qsp-pre-price\"]",
            PreMarketChangeCssSelector = "section.secondary span[data-testid=\"qsp-pre-price-change-percent\"]",
            ExtendedCssSelector = "section.secondary span[data-testid=\"qsp-post-price\"]",
            ExtendedChangeCssSelector = "section.secondary span[data-testid=\"qsp-post-price-change-percent\"]",
        };
        using var fetcher = new StaticHtmlQuoteFetcher();

        var snapshot = fetcher.ExtractSnapshot(subscription, html, DateTimeOffset.UtcNow);

        Assert.Equal(513.53m, snapshot.Price);
        Assert.Equal(516.20m, snapshot.PreMarketPrice);
        Assert.Equal(0.52m, snapshot.PreMarketChangePercent);
        Assert.Equal(513.06m, snapshot.ExtendedPrice);
        Assert.Equal(-0.09m, snapshot.ExtendedChangePercent);
    }
}
