using System.Globalization;
using System.Reflection;

namespace MetaOrchestration.WorkerProtocol;

public static class OrchestrationWorkerProtocol
{
    public const string WorkerMessagePrefix = "META_PIPELINE_WORKER";
    public const string CommandMessagePrefix = "META_ORCHESTRATION";

    public static string ResolveExecutableVersion(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        return assembly
                   .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                   ?.InformationalVersion
               ?? assembly.GetName().Version?.ToString()
               ?? "unknown";
    }

    public static string EncodeEvent(WorkerProtocolEvent workerEvent)
    {
        ArgumentNullException.ThrowIfNull(workerEvent);
        return string.Join(
            "\t",
            WorkerMessagePrefix,
            workerEvent.Kind,
            Escape(workerEvent.WorkerId),
            Escape(workerEvent.PipelineId),
            Escape(workerEvent.PipelineName),
            Escape(workerEvent.TaskId),
            Escape(workerEvent.TaskName),
            Escape(workerEvent.GrantId),
            Escape(workerEvent.CommandId),
            workerEvent.AttemptNumber.ToString(CultureInfo.InvariantCulture),
            workerEvent.ExitCode.ToString(CultureInfo.InvariantCulture),
            Escape(workerEvent.ExecutableVersion),
            Escape(workerEvent.Message),
            Escape(workerEvent.FailureClass));
    }

    public static bool TryDecodeEvent(string line, out WorkerProtocolEvent workerEvent)
    {
        workerEvent = WorkerProtocolEvent.Empty;
        var parts = line.Split('\t');
        if (parts.Length == 0 ||
            !string.Equals(parts[0], WorkerMessagePrefix, StringComparison.Ordinal))
        {
            return false;
        }

        if (parts.Length < 13)
        {
            throw new InvalidOperationException($"Malformed pipeline worker event line '{line}'.");
        }

        workerEvent = new WorkerProtocolEvent(
            parts[1],
            Unescape(parts[2]),
            Unescape(parts[3]),
            Unescape(parts[4]),
            Unescape(parts[5]),
            Unescape(parts[6]),
            Unescape(parts[7]),
            Unescape(parts[8]),
            ParseInt(parts[9]),
            ParseInt(parts[10]),
            Unescape(parts[11]),
            Unescape(parts[12]),
            parts.Length >= 14 ? Unescape(parts[13]) : string.Empty);
        return true;
    }

    public static string EncodeCommand(WorkerProtocolCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        return string.Join(
            "\t",
            CommandMessagePrefix,
            command.Kind,
            Escape(command.CommandId),
            Escape(command.GrantId),
            Escape(command.PreviousGrantId),
            command.AttemptNumber.ToString(CultureInfo.InvariantCulture),
            Escape(command.PipelineId),
            Escape(command.PipelineName),
            Escape(command.TaskId),
            Escape(command.Reason));
    }

    public static bool TryDecodeCommand(string line, out WorkerProtocolCommand command)
    {
        command = WorkerProtocolCommand.Empty;
        var parts = line.Split('\t');
        if (parts.Length == 0 ||
            !string.Equals(parts[0], CommandMessagePrefix, StringComparison.Ordinal))
        {
            return false;
        }

        if (parts.Length < 9)
        {
            throw new InvalidOperationException($"Malformed orchestration worker command line '{line}'.");
        }

        var hasPipelineName = parts.Length >= 10;
        command = new WorkerProtocolCommand(
            parts[1],
            Unescape(parts[2]),
            Unescape(parts[3]),
            Unescape(parts[4]),
            ParseInt(parts[5]),
            Unescape(parts[6]),
            hasPipelineName ? Unescape(parts[7]) : string.Empty,
            hasPipelineName ? Unescape(parts[8]) : Unescape(parts[7]),
            hasPipelineName ? Unescape(parts[9]) : Unescape(parts[8]));
        return true;
    }

    private static int ParseInt(string value) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : 0;

    private static string Escape(string? value) =>
        Uri.EscapeDataString(value ?? string.Empty);

    private static string Unescape(string? value) =>
        Uri.UnescapeDataString(value ?? string.Empty);
}

public static class WorkerEventKinds
{
    public const string WorkerOnline = "WorkerOnline";
    public const string WorkerReady = "WorkerReady";
    public const string PipelineCatalog = "PipelineCatalog";
    public const string PipelineStarted = "PipelineStarted";
    public const string TaskReady = "TaskReady";
    public const string GrantAccepted = "GrantAccepted";
    public const string TaskStarted = "TaskStarted";
    public const string TaskSucceeded = "TaskSucceeded";
    public const string TaskFailed = "TaskFailed";
    public const string RetryScheduled = "RetryScheduled";
    public const string GrantRangeSucceeded = "GrantRangeSucceeded";
    public const string GrantRangeFailed = "GrantRangeFailed";
    public const string PipelineCompleted = "PipelineCompleted";
    public const string PipelineStopped = "PipelineStopped";
    public const string PipelineFailed = "PipelineFailed";
    public const string WorkerDrained = "WorkerDrained";
    public const string WorkerFaulted = "WorkerFaulted";
    public const string Heartbeat = "Heartbeat";
    public const string Diagnostic = "Diagnostic";
    public const string Closed = "Closed";
    public const string ProtocolFault = "ProtocolFault";
}

public static class WorkerCommandKinds
{
    public const string InitializeRun = "InitializeRun";
    public const string StartPipeline = "StartPipeline";
    public const string GrantTask = "GrantTask";
    public const string GrantRange = "GrantRange";
    public const string StopPipeline = "StopPipeline";
    public const string FailPipeline = "FailPipeline";
    public const string DrainWorker = "DrainWorker";
    public const string CancelGrant = "CancelGrant";
    public const string TerminateWorker = "TerminateWorker";
}

public static class WorkerFailureClasses
{
    public const string TransientSql = "TransientSql";
    public const string TransientConnectivity = "TransientConnectivity";
    public const string WorkerCrashBeforeTerminalEvent = "WorkerCrashBeforeTerminalEvent";
    public const string HeartbeatTimeout = "HeartbeatTimeout";
    public const string TaskTimeout = "TaskTimeout";
    public const string WorkerReportedRetryable = "WorkerReportedRetryable";
    public const string VersionMismatch = "VersionMismatch";
    public const string MalformedProtocol = "MalformedProtocol";
    public const string InvalidWorkspace = "InvalidWorkspace";
    public const string MissingTaskId = "MissingTaskId";
    public const string DeterministicModelError = "DeterministicModelError";
    public const string RetryBudgetExhausted = "RetryBudgetExhausted";
}

public sealed record WorkerProtocolEvent(
    string Kind,
    string WorkerId,
    string PipelineId,
    string PipelineName,
    string TaskId,
    string TaskName,
    string GrantId,
    string CommandId,
    int AttemptNumber,
    int ExitCode,
    string ExecutableVersion,
    string Message,
    string FailureClass = "")
{
    public static WorkerProtocolEvent Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        0,
        0,
        string.Empty,
        string.Empty,
        string.Empty);
}

public sealed record WorkerProtocolCommand(
    string Kind,
    string CommandId,
    string GrantId,
    string PreviousGrantId,
    int AttemptNumber,
    string PipelineId,
    string PipelineName,
    string TaskId,
    string Reason)
{
    public static WorkerProtocolCommand Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        0,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}
