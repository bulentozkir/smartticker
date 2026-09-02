using System;
using System.Collections.Generic;
using System.Threading;

namespace SmartTicker.Desktop.ViewModels;

internal enum RefreshStream
{
    Prices,
    News,
}

internal sealed class RefreshWorkCoordinator
{
    private readonly object _sync = new();
    private readonly int _maximumConcurrency;
    private readonly HashSet<(RefreshStream Stream, Guid SubscriptionId)> _active = [];

    public RefreshWorkCoordinator(int maximumConcurrency)
    {
        _maximumConcurrency = Math.Max(1, maximumConcurrency);
    }

    public int ActiveCount
    {
        get
        {
            lock (_sync)
            {
                return _active.Count;
            }
        }
    }

    public IDisposable? TryAcquire(RefreshStream stream, Guid subscriptionId)
    {
        lock (_sync)
        {
            var key = (stream, subscriptionId);
            if (_active.Count >= _maximumConcurrency || !_active.Add(key))
            {
                return null;
            }

            return new Lease(this, key);
        }
    }

    private void Release((RefreshStream Stream, Guid SubscriptionId) key)
    {
        lock (_sync)
        {
            _active.Remove(key);
        }
    }

    private sealed class Lease(
        RefreshWorkCoordinator owner,
        (RefreshStream Stream, Guid SubscriptionId) key) : IDisposable
    {
        private RefreshWorkCoordinator? _owner = owner;

        public void Dispose() => Interlocked.Exchange(ref _owner, null)?.Release(key);
    }
}