using Microsoft.Extensions.Hosting;

namespace PipelineSchedulR.Tests.Mocks.HostApplicationLifetime;

internal class HostApplicationLifetimeMock : IHostApplicationLifetime, IDisposable
{
    internal readonly CancellationTokenSource _cancellationTokenSourceStarted = new();
    internal readonly CancellationTokenSource _cancellationTokenSourceStopped = new();
    internal readonly CancellationTokenSource _cancellationTokenSourceStopping = new();

    public CancellationToken ApplicationStarted => _cancellationTokenSourceStarted.Token;
    public CancellationToken ApplicationStopping => _cancellationTokenSourceStopped.Token;
    public CancellationToken ApplicationStopped => _cancellationTokenSourceStopping.Token;

    public void StartApplication()
    {
        _cancellationTokenSourceStarted.Cancel();
    }
    public void StopApplication()
    {
        _cancellationTokenSourceStopping.Cancel();
    }
    public void Dispose()
    {
        _cancellationTokenSourceStopped.Cancel();
        _cancellationTokenSourceStarted.Dispose();
        _cancellationTokenSourceStopped.Dispose();
        _cancellationTokenSourceStopping.Dispose();
    }
}
