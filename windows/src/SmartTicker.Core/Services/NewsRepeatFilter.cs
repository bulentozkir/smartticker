using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

/// <summary>Counts one "showing" per news refresh in which a headline appears.</summary>
public sealed class NewsRepeatFilter
{
    private readonly Dictionary<Guid, Dictionary<string, int>> _shownCounts = [];

    public IReadOnlyList<NewsHeadline> Filter(
        Guid subscriptionId,
        IReadOnlyList<NewsHeadline> headlines,
        int repeatLimit)
    {
        var limit = Math.Max(1, repeatLimit);
        if (!_shownCounts.TryGetValue(subscriptionId, out var seen))
        {
            seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            _shownCounts[subscriptionId] = seen;
        }

        var visible = new List<NewsHeadline>();
        foreach (var headline in headlines)
        {
            var alreadyShown = seen.GetValueOrDefault(headline.Title);
            if (alreadyShown >= limit)
            {
                continue;
            }

            seen[headline.Title] = alreadyShown + 1;
            visible.Add(headline);
        }

        return visible;
    }

    public void Forget(Guid subscriptionId) => _shownCounts.Remove(subscriptionId);
}
