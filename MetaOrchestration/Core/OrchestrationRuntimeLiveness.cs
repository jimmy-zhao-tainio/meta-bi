using MetaOrchestration.WorkerProtocol;

namespace MetaOrchestration.Core;

internal static class OrchestrationRuntimeLiveness
{
    public static void ValidateWorkerEvent(
        string workerName,
        string eventKind,
        string taskId,
        ISet<string> pendingTaskIds,
        IReadOnlyDictionary<string, string> readyTaskIdByWorkerName,
        IReadOnlyDictionary<string, string> runningTaskIdByWorkerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(eventKind);

        switch (eventKind)
        {
            case WorkerEventKinds.TaskReady:
                if (string.IsNullOrWhiteSpace(taskId))
                {
                    throw new InvalidOperationException(
                        $"Pipeline worker '{workerName}' emitted {WorkerEventKinds.TaskReady} without a task id.");
                }

                if (!pendingTaskIds.Contains(taskId))
                {
                    throw new InvalidOperationException(
                        $"Pipeline worker '{workerName}' emitted {WorkerEventKinds.TaskReady} for task '{taskId}', but that task is not pending. The worker is waiting for a command orchestration must not send.");
                }

                if (readyTaskIdByWorkerName.TryGetValue(workerName, out var alreadyReadyTaskId))
                {
                    throw new InvalidOperationException(
                        $"Pipeline worker '{workerName}' emitted {WorkerEventKinds.TaskReady} for task '{taskId}' while it is already waiting at task '{alreadyReadyTaskId}'.");
                }

                if (runningTaskIdByWorkerName.TryGetValue(workerName, out var runningTaskId))
                {
                    throw new InvalidOperationException(
                        $"Pipeline worker '{workerName}' emitted {WorkerEventKinds.TaskReady} for task '{taskId}' while grant '{runningTaskId}' is still running.");
                }

                return;

            case WorkerEventKinds.GrantAccepted:
            case WorkerEventKinds.TaskStarted:
            case WorkerEventKinds.TaskSucceeded:
            case WorkerEventKinds.TaskFailed:
                if (string.IsNullOrWhiteSpace(taskId))
                {
                    throw new InvalidOperationException(
                        $"Pipeline worker '{workerName}' emitted {eventKind} without a task id.");
                }

                if (!runningTaskIdByWorkerName.TryGetValue(workerName, out var grantedTaskId))
                {
                    throw new InvalidOperationException(
                        $"Pipeline worker '{workerName}' emitted {eventKind} for task '{taskId}', but orchestration has no active grant for that worker.");
                }

                if (!string.Equals(grantedTaskId, taskId, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"Pipeline worker '{workerName}' emitted {eventKind} for task '{taskId}', but the active grant is '{grantedTaskId}'.");
                }

                return;

            case WorkerEventKinds.Closed:
                return;

            default:
                throw new InvalidOperationException(
                    $"Pipeline worker '{workerName}' emitted unsupported event '{eventKind}'.");
        }
    }
}
