# MetaOrchestration Runtime State Logic

This note states the current orchestration runtime logic in plain English before treating tests as meaningful evidence.

The goal is not to make the implementation look correct. The goal is to expose where the logic is crisp, where it is local-process hardening, and where the boundary deliberately stops.

`meta-orchestration execute` is a short-lived supervisor command. It starts pipeline workers, grants work, handles worker failures with retry/logging policy, records run evidence, and exits. If `meta-orchestration.exe` itself crashes, that run is a hard stop. The supported behavior is best-effort evidence capture through the run journal and bounded worker logs, not automatic supervisor restart or durable resume.

## Plain English Logic

### Ownership

MetaOrchestration supervises. MetaPipeline executes.

MetaOrchestration decides when a pipeline task may run. MetaPipeline reports task boundaries and executes only after orchestration grants work. A pipeline worker must not advance past a failed task or blocked boundary by itself.

### Worker Lifecycle

A worker starts in `Starting`.

The only normal worker lifecycle is:

```text
Starting -> Online -> Ready -> StartPipelineSent -> PipelineStarted
```

The worker may close from any non-closed lifecycle state. Closed is terminal for that worker instance. A closed worker is not online, not ready, not activated, and not allowed to emit more lifecycle or task events.

`Closed` and `ProtocolFault` are supervisor-side synthetic events. A worker must not emit either name over the control channel.

`WorkerOnline` must carry the exact executable version expected by the supervisor.

`WorkerReady` means the worker is initialized but has not started an active pipeline context.

If a worker closes after `WorkerOnline` but before `WorkerReady`, it has passed version handshake but has no pipeline context. The worker instance can be abandoned and replaced from the beginning without touching task state.

`StartPipelineSent` means orchestration has sent the activation command.

`PipelineStarted` means the worker has accepted that command and may now emit `TaskReady`.

If `StartPipeline` cannot be delivered after `WorkerReady`, no pipeline context exists. The worker instance can be abandoned and replaced from the beginning without touching task state.

Pre-work replacement is bounded per pipeline plus resume boundary. If the same pipeline repeatedly loses workers before activation, or repeatedly loses workers before a grant can be delivered at the same task boundary, orchestration fails fast instead of spawning workers forever.

Worker loss meaning is a state-machine decision. Runtime supplies the worker name plus loss facts such as exit code and orchestration stop intent; `ApplyWorkerLoss` returns the action category (`CloseOnly`, replacement from beginning, replacement at ready task, active-grant loss, block remaining after failure, or fail unresolved). Runtime executes side effects after that decision.

Worker non-response meaning is also a state-machine decision. Runtime supplies the worker name and elapsed protocol time; `ResolveWorkerTimeout` returns the timeout category, expected event or command, active task id when relevant, whether the worker is waiting for orchestration, and whether unresolved pipeline work remains. Runtime may terminate a process and write journal evidence, but it must not rediscover lifecycle state from side dictionaries.

### Task And Grant Lifecycle

A task starts as `Pending`.

A worker can emit `TaskReady` only after its worker is `PipelineStarted`, only for a pending task, and only when the same worker has no other ready task or active grant.

After `TaskReady`, orchestration can issue a grant:

```text
Pending -> Ready -> GrantIssued
```

If the worker disconnects after `TaskReady` but before `GrantTask` is delivered, no work attempt has begun. The task returns to pending and a replacement worker starts at that same task boundary:

```text
Ready -> Pending
```

That ready-boundary replacement is also bounded per task boundary. The repeated failure is a supervisor/runtime failure, not a consumed task attempt.

This is true whether grant delivery fails immediately, the process has already exited before grant can be attempted, or the ready task is waiting on dependencies or locks and the worker exits while still ungranted.

A grant is not a hint. It is the unit of permission, evidence, retry, and lock ownership. A grant must have:

- command id
- grant id
- attempt number
- pipeline id/name
- task id

The worker may acknowledge the grant:

```text
GrantIssued -> GrantAccepted
```

The worker may start execution:

```text
GrantIssued -> Running
GrantAccepted -> Running
```

The worker may finish only from `Running`:

```text
Running -> Succeeded
Running -> Failed
Running -> Ready, for same-worker retry
Running -> RetryScheduled, for replacement-worker retry
```

Every worker-reported active-grant event must carry the same command id, grant id, and attempt number as the active grant. Missing evidence is a protocol violation. Wrong evidence is a protocol violation.

### Supervisor Failure Paths

The supervisor may observe a failure without a terminal worker event. Examples:

- worker process exits while a grant is active
- worker stops responding while a grant is active
- supervisor terminates a worker after a timeout

Those paths are not worker-reported `TaskFailed` events. They are supervisor-observed failures.

The supervisor must resolve the active grant exactly once:

```text
GrantIssued -> Failed
GrantAccepted -> Failed
Running -> Failed
```

or, when retry policy allows replacement:

```text
GrantIssued -> RetryScheduled -> Pending
GrantAccepted -> RetryScheduled -> Pending
Running -> RetryScheduled -> Pending
```

Replacement workers must be started with `StartPipeline` carrying the failed task id as the resume boundary. They must emit `TaskReady` for that task. Earlier same-pipeline tasks must not replay.

When a worker itself reports a non-retry `TaskFailed`, that terminal task evidence is already recorded. Orchestration may send `FailPipeline` to make the worker fail its pipeline context, but sending that command is cleanup, not the state transition that proves task failure. The worker is not closed merely because orchestration sent `FailPipeline`; it may still report `PipelineFailed` before closing. If the control channel is already gone, the lost `FailPipeline` command is journal evidence, not a reason to crash the supervisor.

### Blocking

Blocking is an orchestration decision.

Pending or ready tasks can be marked blocked. Active grants cannot be blocked directly. The active grant must first reach a terminal failure or retry state. This prevents hiding active work under a downstream classification.

When a downstream worker is waiting at `TaskReady` and a dependency result makes that task impossible to run, orchestration records the blocked task outcome before sending `StopPipeline`. The stop command is cleanup for the worker's pipeline context. If the worker has already disconnected, the lost stop command is journal evidence, not a reason to undo or crash the blocked-task resolution.

### Runtime Bookkeeping

`OrchestrationRuntimeKernel` owns logical runtime state:

- pending task ids
- ready task boundaries and retry delays
- active grant ids, command ids, attempts, and running worker names
- running planned-task locks
- task outcomes
- stopped pipeline markers
- worker/task transition state through `OrchestrationExecutionStateMachine`

`MetaOrchestrationRuntimeService` owns transport and side effects:

- process handles
- named-pipe event reads and command writes
- journal/log artifact writes
- observer callbacks
- replacement process startup and termination

After each event, command, timeout, and replacement, kernel projection checks assert:

- every ready task corresponds to a ready task in the transition state
- every running task corresponds to an active grant in the transition state
- every active grant matches grant id, command id, and attempt evidence in the transition state
- closed workers do not appear as live ready/running owners
- every non-closed kernel worker still has a live transport projection supplied by the service: an event read, ready task, or active grant

## Critique

### Crisp Parts

The worker lifecycle is now small enough to reason about.

The task/grant lifecycle has explicit transition rows.

Worker-reported grant events now require evidence. This is essential because otherwise the supervisor can accidentally join a worker event to the wrong active grant.

Replacement-worker retry has a local resume boundary. That avoids replaying prior same-pipeline tasks in the live-supervisor case.

### Weak Parts

The in-process kernel now owns ready/running/retry/outcome/lock bookkeeping, so the service no longer has parallel pending/ready/running/grant dictionaries. This removes the most obvious shadow-state bug source and is intentionally scoped to one live `meta-orchestration execute` invocation.

Supervisor-observed failure meaning now resolves through `OrchestrationRuntimeKernel.ResolveSupervisorObservedFailure`: retry evaluation, terminal outcome, retry scheduling, replacement resume boundary, and downstream blocking are logical kernel decisions. The service still records result/log evidence and starts replacement processes as side effects.

Worker-reported non-retry failure still has cleanup logic outside the kernel. The latest regression tests cover the two dangerous cases: a cooperative worker reporting `PipelineFailed` after `FailPipeline`, and a worker disconnecting before `FailPipeline` can be delivered.

Blocked-task decisions now come from the runtime kernel. The service still sends `StopPipeline` as cleanup; if that control channel is already gone, the lost command remains journal evidence and does not undo the blocked outcome.

Ready-before-grant worker loss is now a kernel decision plus transition: the machine returns replacement at the ready task boundary and applies `ReadyWorkerLost`. Worker replacement and abandoned process cleanup remain runtime side effects around that decision.

Pre-activation worker loss is worker-lifecycle-only. It has no task state transition, but the machine now returns replacement from the beginning when the worker has no established pipeline context. Replacement and abandoned process cleanup remain runtime side effects around worker closure. The local runtime bounds repeated pre-work replacements so one bad pipeline path does not spawn workers forever.

Online-before-ready worker loss is also worker-lifecycle-only. The machine now returns replacement from the beginning when the worker closes after `WorkerOnline` and before pipeline context exists, and it is bounded by the same local pre-work replacement policy. Silent startup and activation are now classified by `ResolveWorkerTimeout`, but they remain timeout/liveness failures until bounded startup retry policy is modeled.

The current state machine is in-process only, by design. It is not a durable operational state machine and should not grow supervisor-restart recovery unless the product scope changes. A supervisor crash stops the run.

Heartbeat-style retry is still not wired through the same retry handling as worker crash and running-grant timeout. That is a live-supervisor worker liveness feature, not supervisor restart recovery.

The outer protocol fuzzing is still young. The kernel can be fuzzed millions of times cheaply, but real process/named-pipe fuzzing needs a smaller, slower harness and is gated by an environment variable for heavier local runs.

### Things That Must Stay True

There must be no dependency-level or topological execution order baked into runtime execution.

There must be no runtime path where a worker terminal event changes task state without matching active grant evidence.

There must be no runtime path where a closed worker remains logically online or ready.

There must be no direct block transition from active grant states.

There must be no replacement-worker retry without an explicit resume task boundary.

There must be no stdout/stderr control-plane fallback.

There must be no runtime lifecycle-predicate shortcut for worker loss or worker timeout meaning. Runtime executes decisions returned by the state-machine kernel.

## Implementation Comparison

### Worker Lifecycle

Expected logic:

```text
Starting -> Online -> Ready -> StartPipelineSent -> PipelineStarted
Closed is terminal.
```

Current implementation:

- `OrchestrationExecutionStateMachine.WorkerTransitions` declares exactly these forward lifecycle transitions.
- `WorkerClosed` exists from every non-closed worker state.
- `WorkerIsOnline`, `WorkerIsReady`, `WorkerStartPipelineSent`, and `WorkerPipelineStarted` explicitly return false for `Closed`.
- `AcceptWorkerLifecycleEvent` and `ValidateTaskEvent` reject events after `Closed`.
- The named-pipe reader treats worker-emitted `Closed` and `ProtocolFault` as protocol faults because those event kinds are reserved for the supervisor.
- `ApplyWorkerLoss` owns worker-loss classification. It closes the lost worker instance and returns whether runtime should close only, replace from the beginning, replace at a ready task boundary, resolve an active grant, block remaining pipeline tasks after failure, or fail unresolved work.
- `ResolveWorkerTimeout` owns worker non-response classification. It returns whether the worker is awaiting `WorkerOnline`, awaiting `WorkerReady`, awaiting `PipelineStarted`, awaiting the first task boundary, waiting for a grant, timing out an active grant, or already resolved.
- Runtime tolerates a broken control channel while sending `StartPipeline` by journaling `StartPipelineCommandLost` and executing the machine's replacement-from-beginning decision.
- Runtime tolerates worker close after `WorkerOnline` but before `WorkerReady` by executing the machine's replacement-from-beginning decision.
- Runtime no longer calls worker lifecycle predicates to describe timeout state; it consumes the timeout decision and performs process termination or supervisor-observed task failure side effects.

Comparison: aligned for the local in-process runtime.

Remaining concern: worker-instance identity is currently run-local evidence in the journal and logs. That is acceptable for a short-lived supervisor.

### Task Ready And Grant Issue

Expected logic:

```text
TaskReady is allowed only for pending tasks after PipelineStarted.
GrantTask requires command id, grant id, and positive attempt number.
```

Current implementation:

- `ValidateTaskReady` requires `PipelineStarted`.
- `ValidateTaskReady` rejects non-pending tasks, duplicate ready tasks on one worker, and ready while another grant is active.
- `MarkGrantIssued` rejects missing grant id, command id, and non-positive attempt numbers.
- `ReadyWorkerLost` returns `Ready` tasks to `Pending` when a ready worker disconnects before `GrantTask` delivery.

Comparison: aligned.

Remaining concern: these projection checks are local runtime assertions. They are meant to prevent live-run logic errors, not to support supervisor restart.

### Worker-Reported Active Grant Events

Expected logic:

```text
GrantAccepted, TaskStarted, TaskSucceeded, and TaskFailed must match active grant evidence.
```

Current implementation:

- `ValidateActiveGrantEvent` checks that the worker has exactly one matching active grant.
- `ValidateGrantEvidence` requires and compares grant id, command id, and attempt number.
- Runtime routes `GrantAccepted`, `TaskStarted`, `TaskSucceeded`, `TaskFailed`, and same-worker retry through evidence-aware kernel methods.

Comparison: aligned after the latest tightening.

Remaining concern: malformed or semantically odd but well-formed diagnostic/heartbeat payloads are not deeply validated yet.

### Supervisor-Observed Failures

Expected logic:

```text
Supervisor-observed failure resolves active grants through supervisor failure or replacement retry.
```

Current implementation:

- Worker close or timeout while a task is active is handled as supervisor-observed failure.
- An active-grant timeout terminates the process and closes the kernel worker before resolving the supervisor-observed task failure or replacement retry.
- Retryable cases use `ReplacementRetryScheduled`, close the old worker, return the task to pending, and start a replacement worker at the failed task id.
- Non-retry cases use supervisor failure and block remaining same-pipeline tasks.
- Worker-reported non-retry `TaskFailed` records the failed attempt, keeps the worker lifecycle open, sends `FailPipeline`, accepts later `PipelineFailed`, and tolerates a broken control channel by journaling `FailPipelineCommandLost`.

Comparison: partially aligned.

Remaining concern: this is local live-supervisor recovery. The service still performs side effects around the kernel decision: result recording, log evidence, command writes, worker termination, and replacement startup.

### Blocking

Expected logic:

```text
Only pending/ready tasks can be blocked. Active grants must first become failed or retry scheduled.
```

Current implementation:

- `Blocked` transitions exist only from `Pending` and `Ready`.
- Runtime block helpers call `MarkBlocked`.
- Runtime sends `StopPipeline` after recording blocked task results, and tolerates a broken control channel by journaling `StopPipelineCommandLost`.

Comparison: aligned.

Remaining concern: block helpers are projection-checked in the local runtime. If the supervisor process crashes, the run stops and the journal/logs are the recovery evidence.

### Runtime Scheduling

Expected logic:

```text
Scheduler consumes events, updates state, releases locks, evaluates readiness, issues grants, repeats.
It must not compute a topological execution order.
Due timeout deadlines must wake immediately; they must not disappear because the deadline is already in the past.
```

Current implementation:

- Run plan rows are still loaded in deterministic ordinal order for stable data access.
- Runtime grants only after readiness and lock checks at event time.
- If `GrantTask` cannot be delivered to a ready worker, runtime asks `ApplyWorkerLoss`; the machine applies `ReadyWorkerLost` and returns replacement at the ready task boundary. Runtime then removes local ready payload, restores pending projection, abandons the old worker instance, and starts a replacement worker with the task id as the resume boundary.
- If a ready worker is already exited before grant can be attempted, runtime asks the same machine decision instead of deferring forever or later treating the close as unresolved work.
- If `StartPipeline` cannot be delivered after `WorkerReady`, runtime asks `ApplyWorkerLoss`; the machine returns replacement from the beginning.
- If an online worker closes before `WorkerReady`, the closed-worker path asks `ApplyWorkerLoss`; the machine returns replacement from the beginning.
- Worker-event timeout scheduling asks `ResolveWorkerTimeout`. Workers waiting at `TaskReady` for orchestration are excluded from protocol-timeout deadlines; active-grant timeouts use the decision task id; unresolved-pipeline timeout failures use the decision's unresolved-work flag.
- Those three pre-work replacement paths share a bounded replacement counter keyed by pipeline plus resume boundary. Exceeding the limit fails the run with a supervisor error instead of allowing an unbounded restart loop.
- There is no wave/rank execution order in the execution loop.
- Worker-event timeout wake construction now includes already-due deadlines and returns a completed wake task for them.

Comparison: aligned in principle.

Remaining concern: deterministic iteration order can still make test output look ordered. Tests should inspect behavior and dependencies, not infer semantics from list order.

### Test Strategy

Expected logic:

```text
High-volume fuzz the kernel.
Truth-table every derived predicate.
Low-volume fuzz the real process/protocol boundary.
Check projections between runtime dictionaries and the kernel.
```

Current implementation:

- Kernel fuzzer covers lifecycle crashes, active-grant crashes, retries, illegal events, and protocol encode/decode.
- Kernel tests directly assert worker-loss decisions for ready-boundary loss, pre-activation loss, and active-grant loss.
- Kernel tests directly assert worker-timeout decisions for before-online, before-ready, before-pipeline-started, before-first-task-boundary, ready-waiting-for-grant, active-grant timeout, and resolved-pipeline cases.
- Worker predicate truth table exists.
- Fake-worker process tests cover malformed protocol, missing active-grant evidence, silent workers, worker crashes, and replacement resume.
- The outer named-pipe protocol fuzzer now covers duplicate lifecycle events, events before activation, terminal events before running, wrong grant/command/attempt evidence, duplicate ready events, and worker-emitted reserved synthetic events.
- Runtime-kernel projection invariants check pending, ready, scheduled retry, active worker, active grant, and lock state against state-machine task snapshots.
- Runtime-kernel projection invariants check non-closed worker snapshots against live event/ready/running transport projections supplied by the service, preventing dead process instances from remaining logically active in the kernel.
- Direct `OrchestrationRuntimeKernel` tests cover dependency blocking, supervisor crash retry with replacement resume boundary, missing live transport projection rejection, and randomized success/failure/retry/block scenarios.
- A generated multi-pipeline named-pipe graph fuzzer creates layered DAGs, predicts final-task readiness from modeled dependencies, then executes through real fake worker processes.
- Regression tests cover non-retry worker-reported failure followed by cooperative `PipelineFailed`, and non-retry worker-reported failure followed by immediate control-channel disconnect.
- Regression tests cover dependency-blocked downstream `TaskReady` followed by immediate control-channel disconnect before `StopPipeline` can be delivered.
- Regression tests cover ready worker disconnect before `GrantTask` delivery and assert replacement resumes at the same task id without recording a failed attempt.
- Regression tests cover ready workers that have already exited before grant is attempted, including a downstream ready task waiting on an unfinished dependency.
- Regression tests cover worker disconnect before `StartPipeline` delivery and assert replacement starts from the beginning, while the delivered-but-silent activation timeout remains fail-fast.
- Regression tests cover worker disconnect after `WorkerOnline` but before `WorkerReady` and assert replacement starts from the beginning, while never-online timeout remains fail-fast.
- Regression tests cover repeated pre-work worker losses for online-before-ready, ready-before-`StartPipeline`, and ready-before-`GrantTask` branches; each fails fast after the bounded replacement limit instead of eventually succeeding after arbitrary worker churn.

Comparison: improving, but not complete.

Remaining concern: fuzzing covers local live-supervisor behavior. That is the intended scope for `meta-orchestration execute`; supervisor crash recovery is out of scope.
