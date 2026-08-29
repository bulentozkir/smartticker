using SmartTicker.Infrastructure.Extraction;

namespace SmartTicker.Infrastructure.Tests;

public sealed class StaticHtmlNewsSelectorAnalyzerTests
{
    [Fact]
    public void Analyze_FindsReusableHeadlineLinkSelector()
    {
        const string html = """
            <article><a class="headline" href="/one">Markets gain after earnings reports</a></article>
            <article><a class="headline" href="/two">Technology shares lead the afternoon rally</a></article>
            """;

        var suggestions = new StaticHtmlNewsSelectorAnalyzer().Analyze(html);

        var suggestion = Assert.Single(suggestions);
        Assert.Equal("a.headline", suggestion.Selector);
        Assert.Equal(95, suggestion.Confidence);
        Assert.Contains("Markets gain", suggestion.SampleValue);
    }

    [Fact]
    public void Analyze_RejectsGenericNavigationLinks()
    {
        const string html = "<nav><a href='/markets'>Browse all market sections</a></nav>";

        var suggestions = new StaticHtmlNewsSelectorAnalyzer().Analyze(html);

        Assert.Empty(suggestions);
    }

    [Fact]
    public void Analyze_FindsBareHeadlineLinksThroughTheirContainer()
    {
        const string html = """
            <div class="list-group-item indc_news_stream te-stream-repeater">
                <b><a href="/commodity/gold/news/579154">Gold slips as the hawkish tone lifts rate bets</a></b>
                <div class="comment more">Gold prices fell toward 4,560 an ounce on Friday.</div>
            </div>
            """;

        var suggestions = new StaticHtmlNewsSelectorAnalyzer().Analyze(html);

        var suggestion = Assert.Single(suggestions);
        Assert.Equal("div.indc_news_stream a", suggestion.Selector);
        Assert.Contains("Gold slips", suggestion.SampleValue);
    }
}