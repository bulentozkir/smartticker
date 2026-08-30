using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

/// <summary>
/// Tracks which rules are already in their fired state so a rule that stays true does not notify on
/// every refresh. Disabling or editing a rule must re-arm it, otherwise it can never fire again.
/// </summary>
public sealed class AlertArmingState
{
    private readonly HashSet<Guid> _matched = [];

    public bool IsArmed(Guid ruleId) => !_matched.Contains(ruleId);

    /// <summary>True only on the transition from not-matching to matching.</summary>
    public bool ShouldNotify(AlertRule rule, decimal price, DateTimeOffset now)
    {
        if (!AlertEvaluator.ShouldFire(rule, price, now))
        {
            _matched.Remove(rule.Id);
            return false;
        }

        return _matched.Add(rule.Id);
    }

    public void Rearm(Guid ruleId) => _matched.Remove(ruleId);

    public bool IsFiring(Guid ruleId) => _matched.Contains(ruleId);

    public void Clear() => _matched.Clear();
}
