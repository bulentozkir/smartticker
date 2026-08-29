using SmartTicker.Infrastructure.Extraction;

namespace SmartTicker.Infrastructure.Tests;

public sealed class StaticHtmlNewsExtractorTests
{
    [Fact]
    public void Extract_UsesExplicitSelectorAndDeduplicates()
    {
        const string html = """
            <a class="headline" href="/1">Markets gain after earnings reports</a>
            <a class="headline" href="/2">Markets gain after earnings reports</a>
            <a class="headline" href="/3">Technology shares lead the afternoon rally</a>
            <a class="other" href="/4">Ignored navigation link text</a>
            """;

        var result = new StaticHtmlNewsExtractor().Extract(html, "a.headline");

        Assert.True(result.Success);
        Assert.Equal(2, result.Headlines.Count);
        Assert.Contains(result.Headlines, item => item.Title == "Markets gain after earnings reports");
        Assert.DoesNotContain(result.Headlines, item => item.Title == "Ignored navigation link text");
    }

    [Fact]
    public void Extract_ResolvesRelativeHeadlineLinksAgainstPage()
    {
        const string html = "<a class='headline' href='/news/story-1'>Central bank holds rates steady</a>";

        var result = new StaticHtmlNewsExtractor().Extract(
            html, "a.headline", new Uri("https://example.com/quote/MSFT"));

        var headline = Assert.Single(result.Headlines);
        Assert.Equal("https://example.com/news/story-1", headline.Url?.AbsoluteUri);
    }

    [Fact]
    public void Extract_RejectsNonWebSchemeLinks()
    {
        const string html = "<a class='headline' href='javascript:alert(1)'>Central bank holds rates steady</a>";

        var result = new StaticHtmlNewsExtractor().Extract(
            html, "a.headline", new Uri("https://example.com/"));

        var headline = Assert.Single(result.Headlines);
        Assert.Null(headline.Url);
    }

    [Fact]
    public void Extract_FallsBackToHeuristicWhenNoSelector()
    {
        const string html = "<h2><a href='/a'>Central bank holds interest rates steady</a></h2>";

        var result = new StaticHtmlNewsExtractor().Extract(html, null);

        Assert.True(result.Success);
        Assert.Equal("Central bank holds interest rates steady", Assert.Single(result.Headlines).Title);
    }

    [Fact]
    public void Extract_ReportsFailureWhenSelectorMatchesNothing()
    {
        var result = new StaticHtmlNewsExtractor().Extract("<div>no news</div>", "a.headline");

        Assert.False(result.Success);
        Assert.Empty(result.Headlines);
        Assert.Contains("did not match", result.Message);
    }

    [Fact]
    public void Extract_ReportsInvalidSelector()
    {
        var result = new StaticHtmlNewsExtractor().Extract("<a class='headline' href='/a'>Some headline text</a>", "a[[[");

        Assert.False(result.Success);
        Assert.Contains("not valid CSS", result.Message);
    }
}
