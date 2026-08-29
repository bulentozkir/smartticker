using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

public interface ISettingsStore
{
    string FilePath { get; }

    SmartTickerSettings Load();

    void Save(SmartTickerSettings settings);
}