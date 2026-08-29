using SmartTicker.Infrastructure.Extraction;

namespace SmartTicker.Infrastructure.Tests;

public sealed class StaticHtmlSelectorAnalyzerTests
{
    [Fact]
    public void Analyze_RanksSchemaPriceFirst()
    {
        const string html = """
            <html>
              <meta itemprop="price" content="412.35">
              <span class="last-quote">411.20</span>
            </html>
            """;

        var suggestions = new StaticHtmlSelectorAnalyzer().Analyze(html);

        Assert.NotEmpty(suggestions);
        Assert.Equal("meta[itemprop=\"price\"]", suggestions[0].Selector);
        Assert.Equal(100, suggestions[0].Confidence);
        Assert.Equal("412.35", suggestions[0].SampleValue);
    }

    [Fact]
    public void Analyze_RejectsGenericNumbers()
    {
        const string html = "<div class='article-date'>2026</div><div>123.45</div>";

        var suggestions = new StaticHtmlSelectorAnalyzer().Analyze(html);

        Assert.Empty(suggestions);
    }

    [Fact]
    public void Analyze_SuggestsRepeatedSelectorsUsingTheFirstMatch()
    {
        const string html = "<span class='price'>10.00</span><span class='price'>11.00</span>";

        var suggestions = new StaticHtmlSelectorAnalyzer().Analyze(html);

        var suggestion = Assert.Single(suggestions);
        Assert.Equal("span.price", suggestion.Selector);
        Assert.Equal("10.00", suggestion.SampleValue);
        Assert.Contains("matches 2 elements", suggestion.Reason);
    }

    [Fact]
    public void Analyze_PrefersTheQuoteElementOverItsContainer()
    {
        const string html = """
            <div class="QuotePageTabs">Overview 505.33 Profile News</div>
            <span class="QuoteStrip-lastPrice">513.05</span>
            <span class="QuoteStrip-lastPrice">513.53</span>
            """;

        var suggestions = new StaticHtmlSelectorAnalyzer().Analyze(html);

        Assert.Contains(suggestions, item => item.Selector == "span.QuoteStrip-lastPrice");
        Assert.DoesNotContain(suggestions, item => item.Selector.Contains("QuotePageTabs", StringComparison.Ordinal));
    }
}