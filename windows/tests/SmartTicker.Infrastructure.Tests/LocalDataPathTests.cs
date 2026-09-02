using SmartTicker.Infrastructure.Persistence;

namespace SmartTicker.Infrastructure.Tests;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class DataDirectoryEnvironmentCollection
{
    public const string Name = "Data directory environment";
}

[Collection(DataDirectoryEnvironmentCollection.Name)]
public sealed class LocalDataPathTests
{
    [Fact]
    public void DefaultStores_UseConfiguredDataDirectory()
    {
        const string variable = "SMARTTICKER_DATA_DIRECTORY";
        var original = Environment.GetEnvironmentVariable(variable);
        var directory = Path.Combine(Path.GetTempPath(), "SmartTicker.Tests", Guid.NewGuid().ToString("N"));

        try
        {
            Environment.SetEnvironmentVariable(variable, directory);

            Assert.Equal(Path.Combine(directory, "settings.json"), new LocalJsonSettingsStore().FilePath);
            Assert.Equal(Path.Combine(directory, "alerts.json"), new LocalJsonAlertStore().FilePath);
        }
        finally
        {
            Environment.SetEnvironmentVariable(variable, original);
        }
    }
}