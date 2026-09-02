using SmartTicker.Desktop.Controls;
using SmartTicker.Desktop.ViewModels;

namespace SmartTicker.Desktop.Tests;

public sealed class CpuEfficiencyTests
{
    [Theory]
    [InlineData(10, 100)]
    [InlineData(40, 62.5)]
    [InlineData(50, 50)]
    [InlineData(200, 33)]
    public void MarqueeCadence_UsesBoundedPixelDistance(int speed, double expectedMilliseconds)
    {
        Assert.Equal(expectedMilliseconds, MarqueeText.AnimationIntervalFor(speed).TotalMilliseconds);
    }

    [Fact]
    public void LaneUpdate_NotifiesOnlyWhenRenderedSegmentsChange()
    {
        var link = new Uri("https://example.com/quote");
        var lane = new TickerLane(
            [new TickerSegment("MSFT 100.00", link)],
            pixelsPerSecond: 50,
            isPaused: false,
            rowHeight: 24,
            fontSize: 14);
        var changedProperties = new List<string?>();
        lane.PropertyChanged += (_, change) => changedProperties.Add(change.PropertyName);

        lane.Update(
            [new TickerSegment("MSFT 100.00", new Uri(link.AbsoluteUri))],
            pixelsPerSecond: 50,
            isPaused: false,
            rowHeight: 24,
            fontSize: 14);

        Assert.DoesNotContain(nameof(TickerLane.Segments), changedProperties);

        lane.Update(
            [new TickerSegment("MSFT 101.00", link)],
            pixelsPerSecond: 50,
            isPaused: false,
            rowHeight: 24,
            fontSize: 14);

        Assert.Contains(nameof(TickerLane.Segments), changedProperties);
    }
}