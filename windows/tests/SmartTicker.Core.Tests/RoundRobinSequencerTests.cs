using SmartTicker.Core.Services;

namespace SmartTicker.Core.Tests;

public sealed class RoundRobinSequencerTests
{
    private static IReadOnlyList<IReadOnlyList<string>> Groups(params string[][] groups) =>
        groups.Select(group => (IReadOnlyList<string>)group).ToArray();

    [Fact]
    public void Interleave_AlternatesBetweenTickers()
    {
        var sequenced = RoundRobinSequencer.Interleave(
            Groups(["A1", "A2", "A3"], ["B1", "B2", "B3"]));

        Assert.Equal(["A1", "B1", "A2", "B2", "A3", "B3"], sequenced);
    }

    [Fact]
    public void Interleave_KeepsRemainderWhenOneTickerHasFewerHeadlines()
    {
        var sequenced = RoundRobinSequencer.Interleave(
            Groups(["A1", "A2", "A3", "A4"], ["B1"]));

        Assert.Equal(["A1", "B1", "A2", "A3", "A4"], sequenced);
    }

    [Fact]
    public void Interleave_CyclesAcrossThreeTickers()
    {
        var sequenced = RoundRobinSequencer.Interleave(
            Groups(["A1", "A2"], ["B1", "B2"], ["C1", "C2"]));

        Assert.Equal(["A1", "B1", "C1", "A2", "B2", "C2"], sequenced);
    }

    [Fact]
    public void Interleave_SkipsEmptyGroups()
    {
        var sequenced = RoundRobinSequencer.Interleave(
            Groups(["A1", "A2"], [], ["C1"]));

        Assert.Equal(["A1", "C1", "A2"], sequenced);
    }

    [Fact]
    public void Interleave_HandlesNoGroups()
    {
        Assert.Empty(RoundRobinSequencer.Interleave(Groups()));
    }
}
