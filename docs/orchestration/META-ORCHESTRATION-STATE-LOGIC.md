# MetaOrchestration Runtime State Logic

This note states the current orchestration runtime logic in plain English before treating tests as meaningful evidence.

`meta-orchestration execute` is a short-lived supervisor command. It starts pipeline worker processes, exchanges typed protocol messages over named pipes, grants work, handles worker failures with retry/logging policy while the supervisor is alive, records run evidence, and exits. If `meta-orchestration.exe` itself crashes, that run is a hard stop. The supported behavior is best-effort evidence through the run journal and bounded worker logs, not automatic supervisor restart or durable resume.

## Ownership

MetaOrchestration supervises. MetaPipeline executes.

`MetaOrchestrationRuntimeService` owns side effects only:

- start and terminate worker processes
- read worker protocol events
- send worker protocol commands
- write run journal and log references
- project task results and blocked results for the public runtime result
- submit observed facts as `RuntimeEvent`
- execute emitted `RuntimeAction`

The runtime core owns orchestration meaning:

- `MetaOrchestrationRuntimeKernel.RegisterEvent(RuntimeEvent)` is the single event-in/action-out boundary.
- `RuntimeState` owns orchestration state and bookkeeping.
- `ExecutionStateReducer.Apply(...)` decides task, worker, and grant lifecycle transitions.
- `ActivationStateReducer.Apply(...)` decides pipeline/worker activation transitions.
- Bookkeeping components mutate only through intention-revealing methods and check invariants close to mutation.

The service must not choose the next task state, worker state, activation state, grant state, retry state, lock state, or outcome state. If the service observes a fact, it emits an event. If the kernel wants an effect, it emits an action.

## Events And Actions

Runtime inputs are explicit `RuntimeEvent` records. Important events include:

- `SchedulerTick`
- `WorkerOnline`
- `WorkerReady`
- `StartPipelineAcknowledged`
- `PipelineStarted`
- `TaskReady`
- `GrantAccepted`
- `GrantDeliveryFailed`
- `TaskStarted`
- `TaskSucceeded`
- `TaskFailed`
- `WorkerClosed`
- `WorkerTimedOut`
- `SupervisorFailureObserved`
- `PipelineStopRequested`

Runtime outputs are explicit `RuntimeAction` records. Important actions include:

- `StartWorker`
- `SendStartPipeline`
- `IssueGrant`
- `SendStopPipeline`
- `MarkPipelineFailed`
- `ScheduleRetry`
- `RecordTaskCompletion`
- `RecordBlockedTasks`
- `WriteJournalEntry`
- `NotifyObserver`
- `PublishSnapshot`

Actions are emitted after the kernel has already made coherent state/bookkeeping changes. For example, `IssueGrant` is emitted only after the ready task has been removed from the ready queue, the running grant has been recorded, and required locks have been acquired.

## Worker And Activation Logic

A worker instance normally moves through this lifecycle:

```text
Starting -> Online -> Ready -> StartPipelineSent -> PipelineStarted -> Closed
```

`Closed` is terminal for that worker instance. A closed worker is not online, not ready, not activated, and not allowed to emit more task events.

A pipeline activation normally moves through:

```text
Inactive -> StartRequested -> Starting -> Online -> Ready -> Active
```

Activation can also be parked for capacity or replacement resume boundaries. Parking is kernel-owned bookkeeping, not a service-local skip rule.

If a worker dies before pipeline context exists, the kernel may replace it from the beginning subject to bounded replacement attempts. If a worker dies after a ready task but before a grant is delivered, the task returns to pending and a replacement worker resumes at that task boundary. If a worker dies with an active grant, the kernel resolves failure or retry according to retry policy and grant evidence.

## Task, Grant, Retry, And Lock Logic

A task starts as `Pending`.

The normal task/grant flow is:

```text
Pending -> Ready -> GrantIssued -> GrantAccepted -> Running -> Succeeded
```

Failure may produce either a terminal failure or a retry schedule:

```text
Running -> Failed
Running -> RetryScheduled -> Ready
```

A grant is the unit of permission, evidence, retry, and lock ownership. Worker-reported grant events must match the active grant id, command id, and attempt number. Missing or mismatched evidence is a protocol violation.

Locks are acquired for a grant and released with that grant. A lock-conflicted ready task remains ready and receives no grant until compatible lock state exists. A completed, failed, blocked, or stopped task must not remain pending, ready, running, locked, or retry-scheduled.

## Dependency Logic

The kernel does not precompute a fixed execution order. It repeatedly reacts to events and current state.

A ready task can receive a grant only when dependencies are satisfied and locks can be acquired. If a predecessor has not completed, the task waits. If a predecessor completed with an outcome that does not satisfy the dependency condition, downstream work is blocked and the kernel emits blocked-result and stop-pipeline actions.

Worker activation prefers pipelines that can currently make progress. When worker capacity is spare and no progressable inactive pipeline remains, a dependency-waiting pipeline can be activated so it can reach a ready boundary. When capacity is full and a waiting ready worker is occupying a slot needed by a progressable inactive pipeline, the kernel may park that worker through explicit capacity-deferral bookkeeping.

## Bookkeeping Invariants

`RuntimeState` owns the mutable logical containers. No raw mutable dictionaries, lists, or sets are exposed as runtime API.

Core invariants:

- a task cannot be both pending and ready
- a task cannot have two active grants
- a grant cannot be accepted, started, completed, or failed by the wrong worker/evidence
- locks acquired for a grant must be released with that grant
- a completed or blocked task cannot remain pending, ready, running, locked, or retry scheduled
- a stopped pipeline cannot receive new work
- a retry entry has an owner, due time, attempt number, and previous grant id
- a capacity-deferred worker has a pending or parked resume boundary, not loose service-side state
- worker replacement attempts are bounded per pipeline plus resume boundary

## Current Critique

The runtime now has a much better control point than the removed broad imperative internals: events enter the kernel, reducers decide lifecycle transitions, bookkeeping mutates through controlled components, and typed actions leave the kernel.

The design is still intentionally local-supervisor oriented. It does not pretend to be a durable distributed scheduler. Future production work should focus on operational storage, run search, metrics, alerting, retention cleanup, and cross-machine locking without moving orchestration transition ownership back into the service.

## Verification Anchors

Current regression anchors:

- `MetaOrchestrationRuntimeCoreSkeletonTests` for reducer surfaces, architecture boundaries, grant/lock/retry/dependency invariants, and service source checks.
- `MetaOrchestrationAnalysisServiceTests` runtime integration cases for fake worker process/named-pipe behavior, retries, worker loss, timeouts, protocol violations, capacity, and graph fuzzing.
- Full `MetaOrchestration.Tests` must pass after runtime changes.