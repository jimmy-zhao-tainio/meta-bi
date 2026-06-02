using System.Runtime.InteropServices;

namespace MetaOrchestration.Core;

internal sealed class OrchestrationSupervisorSignalScope : IDisposable
{
    private readonly OrchestrationRunJournal journal;
    private readonly OrchestrationSupervisorRunState state;
    private readonly CancellationTokenSource cancellation = new();
    private readonly List<IDisposable> signalRegistrations = [];
    private ConsoleCancelEventHandler? cancelKeyHandler;
    private EventHandler? processExitHandler;
    private int completed;
    private int disposed;

    private OrchestrationSupervisorSignalScope(
        OrchestrationRunJournal journal,
        OrchestrationSupervisorRunState state)
    {
        this.journal = journal;
        this.state = state;
    }

    public CancellationToken CancellationToken => cancellation.Token;

    public static OrchestrationSupervisorSignalScope Register(
        OrchestrationRunJournal journal,
        OrchestrationSupervisorRunState state)
    {
        var scope = new OrchestrationSupervisorSignalScope(journal, state);
        scope.RegisterHandlers();
        return scope;
    }

    public void MarkCompleted() =>
        Interlocked.Exchange(ref completed, 1);

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed, 1) != 0)
        {
            return;
        }

        if (cancelKeyHandler is not null)
        {
            try
            {
                Console.CancelKeyPress -= cancelKeyHandler;
            }
            catch (Exception)
            {
                // Best-effort signal cleanup.
            }
        }

        if (processExitHandler is not null)
        {
            AppDomain.CurrentDomain.ProcessExit -= processExitHandler;
        }

        foreach (var registration in signalRegistrations)
        {
            registration.Dispose();
        }

        cancellation.Dispose();
    }

    private void RegisterHandlers()
    {
        cancelKeyHandler = (_, args) =>
        {
            args.Cancel = true;
            RecordSupervisorSignal("ConsoleCancelKey", args.SpecialKey.ToString(), cancelRun: true);
        };
        try
        {
            Console.CancelKeyPress += cancelKeyHandler;
        }
        catch (Exception ex)
        {
            journal.WriteEvent("SupervisorSignalRegistrationFailed", "ConsoleCancelKey", ex.Message);
        }

        processExitHandler = (_, _) =>
        {
            if (Volatile.Read(ref completed) != 0)
            {
                return;
            }

            journal.WriteEvent("SupervisorProcessExit", "ProcessExit", "process is exiting before the run completed");
            journal.WriteEvent("SupervisorState", "ProcessExit", state.Describe());
        };
        AppDomain.CurrentDomain.ProcessExit += processExitHandler;

        RegisterPosixSignal(PosixSignal.SIGTERM);
        RegisterPosixSignal(PosixSignal.SIGHUP);
    }

    private void RegisterPosixSignal(PosixSignal signal)
    {
        try
        {
            signalRegistrations.Add(PosixSignalRegistration.Create(
                signal,
                context =>
                {
                    context.Cancel = true;
                    RecordSupervisorSignal(signal.ToString(), "POSIX signal received", cancelRun: true);
                }));
        }
        catch (PlatformNotSupportedException)
        {
            // Windows and some hosts do not expose POSIX signal registration.
        }
        catch (Exception ex)
        {
            journal.WriteEvent("SupervisorSignalRegistrationFailed", signal.ToString(), ex.Message);
        }
    }

    private void RecordSupervisorSignal(
        string signalName,
        string detail,
        bool cancelRun)
    {
        if (Volatile.Read(ref completed) != 0)
        {
            return;
        }

        journal.WriteEvent("SupervisorSignal", signalName, detail);
        journal.WriteEvent("SupervisorState", signalName, state.Describe());
        if (cancelRun)
        {
            try
            {
                cancellation.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // Process is already winding down.
            }
        }
    }
}
