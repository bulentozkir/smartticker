namespace SmartTicker.Core.Services;

public interface IAlertSound
{
    /// <summary>Plays the buzz without blocking the caller.</summary>
    void Buzz(int times);
}
