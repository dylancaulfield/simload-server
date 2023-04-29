namespace SimLoad.Common;

public interface ICancellationTokenProvider
{
    CancellationToken? CancellationToken { get; }
}

public class CancellationTokenProvider : ICancellationTokenProvider, IDisposable
{
    private CancellationTokenSource? _cancellationTokenSource;

    public CancellationTokenProvider(CancellationTokenSource? cancellationTokenSource)
    {
        _cancellationTokenSource = cancellationTokenSource;
    }

    public CancellationToken? CancellationToken => _cancellationTokenSource?.Token ?? null;

    public void Dispose()
    {
        _cancellationTokenSource!.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
    }
}