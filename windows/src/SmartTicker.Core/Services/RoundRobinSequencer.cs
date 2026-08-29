namespace SmartTicker.Core.Services;

public static class RoundRobinSequencer
{
    /// <summary>Takes one item from each group per pass so no single group dominates the order.</summary>
    public static IReadOnlyList<T> Interleave<T>(IReadOnlyList<IReadOnlyList<T>> groups)
    {
        var sequenced = new List<T>();
        if (groups.Count == 0)
        {
            return sequenced;
        }

        var longest = groups.Max(group => group.Count);
        for (var round = 0; round < longest; round++)
        {
            foreach (var group in groups)
            {
                if (round < group.Count)
                {
                    sequenced.Add(group[round]);
                }
            }
        }

        return sequenced;
    }
}
