using SmartTicker.Core.Models;
using SmartTicker.Infrastructure.Extraction;

namespace SmartTicker.Infrastructure.Tests;

public sealed class SelectorKindDiscoveryTests
{
    // The close and after-hours values share a class, so only the container tells them apart.
    private const string CnbcHtml = """
        <div class="QuoteStrip-container">
          <div class="QuoteStrip-extendedDataContainer">
            <span class="QuoteStrip-lastPrice">513.05</span>
            <span class="QuoteStrip-changeDown">-0.09%</span>
          </div>
          <div class="QuoteStrip-dataContainer">
            <span class="QuoteStrip-lastPrice">513.53</span>
            <span class="QuoteStrip-changePercent">+0.25%</span>
          </div>
        </div>
        """;

    private const string TradingEconomicsHtml = """
        <div id="ctl00_ContentPlaceHolder1_ctl00_PanelPrice">
          <span id="market_last">4,454.08</span>
          <span id="market_daily_Pchg">-0.42%</span>
        </div>
        """;

    private readonly StaticHtmlSelectorAnalyzer _analyzer = new();

    [Fact]
    public void Change_FindsThePercentAndIgnoresTheAfterHoursOne()
    {
        var suggestions = _analyzer.Analyze(CnbcHtml, SelectorKind.Change);

        var match = Assert.Single(suggestions, item => item.Selector.Contains("changePercent", StringComparison.Ordinal));
        Assert.Equal("+0.25%", match.SampleValue);
        Assert.DoesNotContain(suggestions, item => item.SampleValue == "-0.09%");
    }

    [Fact]
    public void Change_FindsTheTradingEconomicsPercent()
    {
        var suggestions = _analyzer.Analyze(TradingEconomicsHtml, SelectorKind.Change);

        var match = Assert.Single(suggestions);
        Assert.Contains("market_daily_Pchg", match.Selector, StringComparison.Ordinal);
        Assert.Equal("-0.42%", match.SampleValue);
    }

    [Fact]
    public void ExtendedPrice_ScopesTheSharedClassToTheAfterHoursContainer()
    {
        var suggestions = _analyzer.Analyze(CnbcHtml, SelectorKind.ExtendedPrice);

        var match = Assert.Single(suggestions);
        Assert.Contains("QuoteStrip-extendedDataContainer", match.Selector, StringComparison.Ordinal);
        Assert.Contains("QuoteStrip-lastPrice", match.Selector, StringComparison.Ordinal);
        Assert.Equal("513.05", match.SampleValue);
    }

    [Fact]
    public void ExtendedChange_FindsOnlyTheAfterHoursPercent()
    {
        var suggestions = _analyzer.Analyze(CnbcHtml, SelectorKind.ExtendedChange);

        var match = Assert.Single(suggestions);
        Assert.Equal("-0.09%", match.SampleValue);
        Assert.Contains("QuoteStrip-extendedDataContainer", match.Selector, StringComparison.Ordinal);
    }

    [Fact]
    public void Price_KeepsItsOriginalBehaviour()
    {
        var suggestions = _analyzer.Analyze(TradingEconomicsHtml, SelectorKind.Price);

        Assert.Contains(suggestions, item => item.Selector.Contains("market_last", StringComparison.Ordinal));
    }
}
