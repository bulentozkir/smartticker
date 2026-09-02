namespace SmartTicker.Infrastructure.Persistence;

internal static class LocalDataPath
{
    private const string DataDirectoryEnvironmentVariable = "SMARTTICKER_DATA_DIRECTORY";

    public static string For(string fileName)
    {
        var configuredDirectory = Environment.GetEnvironmentVariable(DataDirectoryEnvironmentVariable);
        var directory = string.IsNullOrWhiteSpace(configuredDirectory)
            ? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SmartTicker")
            : Path.GetFullPath(configuredDirectory);
        return Path.Combine(directory, fileName);
    }
}