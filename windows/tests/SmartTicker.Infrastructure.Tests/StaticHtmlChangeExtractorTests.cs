using SmartTicker.Infrastructure.Extraction;

namespace SmartTicker.Infrastructure.Tests;

public sealed class StaticHtmlChangeExtractorTests
{
    [Fact]
    public void Extract_ReadsCnbcChangeAndFollowsDocumentOrder()
    {
        const string html = """
            <div class="QuoteStrip-lastPriceStripContainer">
                <span class="QuoteStrip-lastPrice">16.68</span>
                <span class="QuoteStrip-changeUp"><span>(+1.15%)</span></span>
            </div>
            <div class="QuoteStrip-lastPriceStripContainer">
                <span class="QuoteStrip-lastPrice">16.49</span>
                <span class="QuoteStrip-changeDown"><span>(-1.61%)</span></span>
            </div>
            """;

        Assert.Equal(1.15m, new StaticHtmlChangeExtractor().Extract(html));
    }

    [Fact]
    public void Extract_ReadsTradingEconomicsPercentChange()
    {
        const string html = """
            <span id="market_last">4,454.08</span>
            <span id="market_daily_Pchg">-3.18%</span>
            """;

        Assert.Equal(-3.18m, new StaticHtmlChangeExtractor().Extract(html));
    }

    [Fact]
    public void Extract_ReturnsNullWhenNoPercentIsPresent()
    {
        Assert.Null(new StaticHtmlChangeExtractor().Extract("<div class='price'>513.53</div>"));
    }
}
