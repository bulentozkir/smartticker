namespace SmartTicker.Desktop.Tests;

public sealed class ExceptionSafetyTests
{
    [Fact]
    public void Run_ContainsAndReportsARecoverableException()
    {
        Exception? reported = null;

        ExceptionSafety.Run(
            () => throw new InvalidOperationException("recoverable"),
            exception => reported = exception);

        Assert.IsType<InvalidOperationException>(reported);
    }

    [Fact]
    public async Task RunAsync_ContainsAndReportsARecoverableException()
    {
        Exception? reported = null;

        await ExceptionSafety.RunAsync(
            () => Task.FromException(new IOException("recoverable")),
            exception => reported = exception);

        Assert.IsType<IOException>(reported);
    }

    [Fact]
    public void Run_ContainsARecoverableReporterFailure()
    {
        ExceptionSafety.Run(
            () => throw new InvalidOperationException("operation"),
            _ => throw new IOException("reporter"));
    }

    [Fact]
    public void Run_DoesNotSwallowAnUnrecoverableFailure()
    {
        Assert.Throws<OutOfMemoryException>(() =>
            ExceptionSafety.Run(() => throw new OutOfMemoryException("fatal")));
    }
}