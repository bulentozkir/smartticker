using System;
using System.Collections.Generic;

namespace SmartTicker.Desktop.Views;

internal sealed class StaggeredRefreshSchedule
{
    private Guid[] _subscriptionIds = [];
    private int _intervalSeconds;
    private int _nextSlot;

    public IReadOnlyList<Guid> NextBatch(IReadOnlyList<Guid> subscriptionIds, int intervalSeconds)
    {
        intervalSeconds = Math.Max(1, intervalSeconds);
        if (_intervalSeconds != intervalSeconds || !Matches(subscriptionIds))
        {
            _subscriptionIds = [.. subscriptionIds];
            _intervalSeconds = intervalSeconds;
            _nextSlot = 0;
        }

        var slot = _nextSlot;
        _nextSlot = (_nextSlot + 1) % _intervalSeconds;
        if (_subscriptionIds.Length == 0)
        {
            return [];
        }

        var batch = new List<Guid>();
        for (var index = 0; index < _subscriptionIds.Length; index++)
        {
            if ((long)index * _intervalSeconds / _subscriptionIds.Length == slot)
            {
                batch.Add(_subscriptionIds[index]);
            }
        }

        return batch;
    }

    public void Reset()
    {
        _subscriptionIds = [];
        _intervalSeconds = 0;
        _nextSlot = 0;
    }

    private bool Matches(IReadOnlyList<Guid> subscriptionIds)
    {
        if (_subscriptionIds.Length != subscriptionIds.Count)
        {
            return false;
        }

        for (var index = 0; index < _subscriptionIds.Length; index++)
        {
            if (_subscriptionIds[index] != subscriptionIds[index])
            {
                return false;
            }
        }

        return true;
    }
}