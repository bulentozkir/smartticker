using SmartTicker.Core.Services;

namespace SmartTicker.Core.Tests;

public sealed class TickerLayoutCalculatorTests
{
    [Fact]
    public void Calculate_ShowsNewsWhenBothBandsFit()
    {
        var layout = TickerLayoutCalculator.Calculate(
            TickerLayoutCalculator.NaturalHeight(1, 1), 1, 1);

        Assert.True(layout.ShowNews);
        Assert.Equal(TickerLayoutCalculator.NaturalRowHeight, layout.RowHeight);
    }

    [Fact]
    public void Calculate_HidesNewsWhenHeightCannotFitBothBands()
    {
        var layout = TickerLayoutCalculator.Calculate(45, 1, 1);

        Assert.False(layout.ShowNews);
    }

    [Fact]
    public void Calculate_RestoresNewsWhenWindowGrowsBack()
    {
        Assert.False(TickerLayoutCalculator.Calculate(45, 1, 1).ShowNews);
        Assert.True(TickerLayoutCalculator.Calculate(120, 1, 1).ShowNews);
    }

    [Fact]
    public void Calculate_MoreRowsNeedMoreHeightBeforeNewsAppears()
    {
        Assert.False(TickerLayoutCalculator.Calculate(120, 3, 3).ShowNews);
        Assert.True(TickerLayoutCalculator.Calculate(160, 3, 3).ShowNews);
    }

    [Fact]
    public void Calculate_GrowsFontWithRowHeight()
    {
        var small = TickerLayoutCalculator.Calculate(92, 1, 1);
        var large = TickerLayoutCalculator.Calculate(220, 1, 1);

        Assert.True(large.RowHeight > small.RowHeight);
        Assert.True(large.PriceFontSize > small.PriceFontSize);
        Assert.True(large.NewsFontSize > small.NewsFontSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    [InlineData(4000)]
    public void Calculate_KeepsRowHeightAndFontsWithinBounds(double availableHeight)
    {
        var layout = TickerLayoutCalculator.Calculate(availableHeight, 2, 2);

        Assert.InRange(
            layout.RowHeight,
            TickerLayoutCalculator.MinimumRowHeight,
            TickerLayoutCalculator.MaximumRowHeight);
        Assert.InRange(layout.PriceFontSize, 10, 24);
        Assert.InRange(layout.NewsFontSize, 9, 22);
    }
}
