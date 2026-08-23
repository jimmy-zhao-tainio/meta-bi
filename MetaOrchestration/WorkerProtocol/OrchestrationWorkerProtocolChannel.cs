using System.IO.Pipes;
using System.Text;

namespace MetaOrchestration.WorkerProtocol;

public sealed class OrchestrationWorkerProtocolChannel : IAsyncDisposable, IDisposable
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);
    private readonly Stream stream;
    private readonly StreamReader reader;
    private readonly StreamWriter writer;
    private bool disposed;

    private OrchestrationWorkerProtocolChannel(Stream stream)
    {
        this.stream = stream;
        reader = new StreamReader(stream, Utf8NoBom, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true);
        writer = new StreamWriter(stream, Utf8NoBom, bufferSize: 4096, leaveOpen: true)
        {
            AutoFlush = true
        };
    }

    public static NamedPipeServerStream CreateServerPipe(string pipeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pipeName);
        return new NamedPipeServerStream(
            pipeName,
            PipeDirection.InOut,
            maxNumberOfServerInstances: 1,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous);
    }

    public static OrchestrationWorkerProtocolChannel FromConnectedStream(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        return new OrchestrationWorkerProtocolChannel(stream);
    }

    public static async Task<OrchestrationWorkerProtocolChannel> ConnectClientAsync(
        string pipeName,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pipeName);
        if (timeout is { } configuredTimeout && configuredTimeout < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "Timeout must be non-negative. Use zero or null for no timeout.");
        }

        var client = new NamedPipeClientStream(
            ".",
            pipeName,
            PipeDirection.InOut,
            PipeOptions.Asynchronous);
        try
        {
            if (timeout is null || timeout.Value == TimeSpan.Zero)
            {
                await client.ConnectAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                using var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCancellation.CancelAfter(timeout.Value);
                await client.ConnectAsync(timeoutCancellation.Token).ConfigureAwait(false);
            }

            return new OrchestrationWorkerProtocolChannel(client);
        }
        catch
        {
            await client.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    public async Task<string?> ReadLineAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        return await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task WriteEventAsync(
        WorkerProtocolEvent workerEvent,
        CancellationToken cancellationToken = default) =>
        WriteLineAsync(OrchestrationWorkerProtocol.EncodeEvent(workerEvent), cancellationToken);

    public Task WriteCommandAsync(
        WorkerProtocolCommand command,
        CancellationToken cancellationToken = default) =>
        WriteLineAsync(OrchestrationWorkerProtocol.EncodeCommand(command), cancellationToken);

    public async Task WriteLineAsync(string line, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        await writer.WriteLineAsync(line.AsMemory(), cancellationToken).ConfigureAwait(false);
        await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        writer.Dispose();
        reader.Dispose();
        stream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        await writer.DisposeAsync().ConfigureAwait(false);
        reader.Dispose();
        await stream.DisposeAsync().ConfigureAwait(false);
    }
}
