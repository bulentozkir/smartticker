using SmartTicker.Core.Services;

namespace SmartTicker.Core.Tests;

public sealed class SourceAcknowledgementLedgerTests
{
    [Fact]
    public void Acknowledge_AppliesToWholeHostOnlyOnce()
    {
        var ledger = new SourceAcknowledgementLedger();

        Assert.False(ledger.IsAcknowledged("https://finance.yahoo.com/quote/MSFT"));
        Assert.True(ledger.Acknowledge("https://finance.yahoo.com/quote/MSFT"));

        Assert.True(ledger.IsAcknowledged("https://finance.yahoo.com/quote/AAPL"));
    }

    [Fact]
    public void Acknowledge_DoesNotLeakAcrossDifferentSites()
    {
        var ledger = new SourceAcknowledgementLedger();
        ledger.Acknowledge("https://finance.yahoo.com/quote/MSFT");

        Assert.False(ledger.IsAcknowledged("https://www.cnbc.com/quotes/MSFT"));
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("file:///C:/secret.txt")]
    [InlineData("")]
    public void Acknowledge_RejectsNonWebUrls(string url)
    {
        var ledger = new SourceAcknowledgementLedger();

        Assert.False(ledger.Acknowledge(url));
        Assert.False(ledger.IsAcknowledged(url));
    }

    [Fact]
    public void Ledger_RoundTripsThroughStoredHosts()
    {
        var ledger = new SourceAcknowledgementLedger();
        ledger.Acknowledge("https://www.marketwatch.com/investing/stock/msft");

        var restored = new SourceAcknowledgementLedger(ledger.ToArray());

        Assert.True(restored.IsAcknowledged("https://www.marketwatch.com/investing/stock/aapl"));
    }
}
