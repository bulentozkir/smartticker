namespace SmartTicker.Core.Services;

public interface ILinkLauncher
{
    bool TryOpen(Uri uri);
}
