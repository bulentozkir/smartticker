using SmartTicker.Core.Models;

namespace SmartTicker.Core.Services;

public interface IAlertStore
{
    string FilePath { get; }

    AlertSettings Load();

    void Save(AlertSettings settings);
}
