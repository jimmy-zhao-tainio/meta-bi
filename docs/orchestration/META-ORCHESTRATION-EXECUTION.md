# MetaOrchestration Execution

This note captures the first process-based runtime slice after run planning.

`MetaOrchestration` should execute a run plan, not an inferred plan directly and not a loose set of whole pipelines. Run-plan rows in the orchestration workspace are the contract between planning and runtime.

For the current plain-English state-machine contract, critique, and section-by-section implementation comparison, see `META-ORCHESTRATION-STATE-LOGIC.md`.

## Runtime Scope

`meta-orchestration execute` is a short-lived supervisor command, not a daemon and not a resumable distributed scheduler.

Its job is to:

- refresh and execute one run plan
- start `meta-pipeline` worker processes
- grant eligible tasks based on dependencies, locks, and retry policy
- handle worker/process failures while the supervisor is alive
- capture bounded run evidence and worker diagnostics
- exit with a final status

If `meta-orchestration.exe` itself crashes, the run is a hard stop. The supported behavior is best-effort evidence: a run journal, worker logs, task/grant events already flushed, and final exception details when the process can write them. Do not design automatic supervisor-restart resume, durable grant replay, or a MetaOrchestration operational database into the first runtime shape.

The supervisor should log its own managed exceptions with a compact runtime state snapshot when the run journal exists. It should also record catchable shutdown signals such as Ctrl+C, SIGTERM, and SIGHUP where the platform exposes them, then cancel the live run and dispose workers. Non-catchable termination such as SIGKILL, hard power loss, or force-kill from the OS cannot be logged reliably.

## Ownership Boundary

`MetaOrchestration` owns:

- reading a `RunPlan`
- deciding which planned tasks are eligible to start
- enforcing planned task locks
- applying explicit orchestration policy
- supervising external task workers
- reporting orchestration-level progress and failure

`MetaPipeline` owns:

- loading and preserving one modeled pipeline process context
- exposing task readiness to orchestration through a worker protocol
- executing granted transform-backed pipeline tasks inside that pipeline context
- resolving modeled tasks into bound `MetaTransformScript` and `MetaTransformBinding` rows
- running SQL or row movement
- recording pipeline operational evidence
- enforcing task-level timeout and connection settings

The runtime boundary should remain:

```text
MetaOrchestration supervises.
MetaPipeline executes.
```

## Why Not One-Shot Step Processes

Starting `meta-pipeline execute --pipeline <name>` for each pipeline is too coarse once orchestration is task-level.
Starting `meta-pipeline execute-step` for each task goes too far the other way: it erases the pipeline process context between tasks.

Example:

```text
P1.T1
P1.T2 depends on P2.T1
P1.T3

P2.T1
P2.T2
```

If orchestration starts whole pipelines, it cannot express `P1.T2` waiting for `P2.T1` without either over-serializing the whole pipelines or risking incorrect execution.
If orchestration starts one process per step, `P1.T1`, `P1.T2`, and `P1.T3` are no longer one living pipeline execution.

The runtime boundary is therefore:

```text
one MetaPipeline worker process per pipeline
one StartPipeline activation handshake before task work
one TaskReady/GrantTask/StopPipeline handshake per pipeline task boundary
MetaOrchestration owns cross-pipeline task synchronization
```

## Required MetaPipeline Slice

Add a small pipeline-worker command before implementing orchestration execution:

```cmd
meta-pipeline execute-worker ^
  --workspace .\PipelineWS ^
  --pipeline CustomerLoad ^
  --transform-workspace .\TransformWS ^
  --binding-workspace .\BindingWS ^
  --control-pipe <pipe-name> ^
  [--data-type-conversion-workspace .\DataTypeConversionWS] ^
  [--pipeline-db-connection-env META_PIPELINE_OPERATIONAL_SQL]
```

Rules:

- load the modeled pipeline once
- emit `WorkerOnline` with the exact executable version before any task event
- emit `WorkerReady`, then wait for orchestration to send `StartPipeline`
- emit `PipelineStarted` only after `StartPipeline` names the active pipeline
- emit a `TaskReady` event before each transform-backed `PipelineTask`
- wait for orchestration to grant `GrantTask`, stop the pipeline worker with `StopPipeline`, or close the failed path with `FailPipeline`
- use the same realization rules as normal pipeline execution
- keep MetaPipeline operational DB evidence
- fail when any task/binding/script cannot be resolved
- keep the named pipe control protocol separate from stdout/stderr diagnostics

This keeps process-based orchestration possible without erasing pipeline context.

## Worker Protocol

The local worker interface is now a dedicated named pipe control channel:

- named pipe: orchestration commands and worker protocol events
- child stdout: diagnostics only
- child stderr: diagnostics and human-readable failure context

Frames are line-delimited tab-separated fields. The current command kinds are:

- `StartPipeline`: activate the pipeline id/name before any task boundary is exposed
- `GrantTask`: grant the `TaskReady` task with command id, grant id, and attempt number
- `StopPipeline`: stop the pipeline worker at the current `TaskReady` task because orchestration cannot legally continue that pipeline path
- `FailPipeline`: close the active pipeline path after orchestration decides the failure is terminal

The current event kinds are:

- `WorkerOnline`: worker reached the runtime contract and reports exact executable version
- `WorkerReady`: worker initialized and can receive assignments
- `PipelineStarted`: the requested pipeline context is active
- `TaskReady`: worker has reached the next pipeline task boundary
- `GrantAccepted`: worker committed the grant command
- `TaskStarted`: granted task attempt started
- `TaskSucceeded`: granted task attempt completed successfully
- `TaskFailed`: granted task attempt completed unsuccessfully
- `Closed`: parent-side synthetic event when the worker process exits
- `ProtocolFault`: parent-side synthetic event for malformed worker control messages

Important constraints:

- worker-mode stdout/stderr must not carry control messages
- parent must continuously drain stdout and stderr to avoid diagnostics backpressure deadlocks
- every event must carry stable pipeline and task identifiers
- pipeline workers do not understand orchestration dependency semantics; they only wait at task boundaries and execute granted work
- the named pipe is a local supervision protocol, not a durable distributed runtime store

## Worker Architecture

The target keeps the same ownership boundary and continues hardening the explicit local control channel for one short-lived execute invocation.

### Process Shape

`meta-orchestration` starts `meta-pipeline.exe` as a long-lived worker host:

```cmd
meta-pipeline worker ^
  --workspace .\PipelineWS ^
  --control-channel <channel-name> ^
  [--pipeline-db-connection-env META_PIPELINE_OPERATIONAL_SQL]
```

The pipeline worker process loads the `MetaPipeline` workspace once, advertises the pipelines and task boundaries it can execute, then waits for orchestration grants. It should shut down only when orchestration sends a normal drain/stop command, when all assigned pipeline work is complete, or when a fatal worker failure occurs.

`meta-orchestration` owns the worker lifecycle while that execute process is alive:

- create one orchestration run id
- acquire an exclusive execution lease for the orchestration workspace
- create one control channel per worker process
- start the worker process with workspace path and channel name
- wait for `WorkerOnline`
- start the specific `MetaPipeline.Pipeline` entities needed by the current run plan
- grant work only when dependencies, locks, and resources allow it
- drain and close workers after all reachable work is complete
- terminate workers on orchestration cancellation, timeout, or fatal protocol violation
- exit when the run is complete or when the supervisor itself reaches a fatal condition

### Control Channel

The current local transport is a dedicated duplex named pipe. A Unix-domain socket can serve the same role on platforms that support it.

Do not use stdout/stdin for the production control plane. Keep them for human diagnostics and normal process output. A dedicated channel makes the worker contract explicit and avoids mixing CLI prose with machine messages.

Minimum transport requirements:

- duplex messages
- framed payloads
- cancellation/close detection
- parent can apply read/write timeouts
- worker identity is tied to the process orchestration started
- exact `meta`/`meta-bi` version is exchanged before task messages

JSON is not the product metadata format, but using a framed transport encoding for runtime messages is acceptable if the message contract is modeled in C# and not treated as durable metadata. A compact binary or length-prefixed UTF-8 message format is also fine. The important part is that the contract is typed and owned by the runtime boundary.

The first handshake should be deliberately boring: the worker sends its exact `meta`/`meta-bi` version and orchestration requires it to match the expected executable version. No protocol negotiation, partial compatibility matrix, or feature discovery is needed for the first production cut. Version mismatch fails before any `StartPipeline` command.

### Runtime Contract

Messages from pipeline worker to orchestration:

- `WorkerOnline`: exact executable version, process id, worker id, loaded pipeline workspace fingerprint
- `WorkerReady`: worker is initialized and can receive assignments
- `PipelineCatalog`: pipeline id/name and executable task boundary list for the loaded workspace
- `PipelineStarted`: pipeline id/name, pipeline instance id, and first task boundary
- `TaskReady`: pipeline id, pipeline task id, task ordinal, task name
- `GrantAccepted`: command id, grant id, attempt number, pipeline id, pipeline task id
- `TaskStarted`: grant id, attempt number, pipeline id, pipeline task id
- `TaskSucceeded`: grant id, row counts, batches, operational run/task ids
- `TaskFailed`: grant id, attempt number, failure stage, failure class, retry hint, failure message, operational run/task ids
- `RetryScheduled`: failed grant id, next grant id, next attempt number, delay, retry reason
- `GrantRangeSucceeded`: range grant id and completed task grant ids
- `GrantRangeFailed`: range grant id, failed task grant id, failure stage, failure message
- `PipelineCompleted`: pipeline id when the worker reaches the end of a serial pipeline
- `PipelineStopped`: pipeline id after orchestration stopped it at a boundary
- `PipelineFailed`: pipeline id after orchestration marks the pipeline failed and releases that pipeline context
- `WorkerDrained`: no assigned work remains
- `WorkerFaulted`: worker-level unrecoverable fault
- `Heartbeat`: worker health and currently executing grant id, if any
- `Diagnostic`: structured non-control diagnostics, already subject to worker-side size/rate limits

Messages from orchestration to pipeline worker:

- `InitializeRun`: orchestration run id and execution settings
- `StartPipeline`: pipeline id/name to activate from the loaded workspace, plus an optional resume task id
- `GrantTask`: command id, grant id, optional previous grant id, attempt number, pipeline id, pipeline task id
- `GrantRange`: range grant id plus an ordered contiguous list of task grants for one active pipeline
- `StopPipeline`: stop at the current task boundary because orchestration has determined that pipeline path is blocked or no longer selected
- `FailPipeline`: mark the active pipeline failed after an execution failure when orchestration will not grant a failure branch
- `DrainWorker`: finish current granted task, then stop accepting new assignments
- `CancelGrant`: cancel current granted task if the underlying execution supports cancellation
- `TerminateWorker`: abort process after timeout/fatal orchestration shutdown

There is still no `Skip` command. Blocked, not-selected, and skipped are orchestration outcome classifications. Pipeline workers only know task boundaries, grants, stops, and execution outcomes.

Command rationale:

- `InitializeRun` separates process startup from run context.
- `StartPipeline` makes the active `MetaPipeline.Pipeline` explicit before task work starts and carries the resume task boundary for replacement workers.
- `GrantTask` is the unit of orchestration permission, lock ownership, retry attempt, and task evidence.
- `GrantRange` is only a round-trip optimization over contiguous grants; it does not change task semantics.
- `StopPipeline` keeps blocked/not-selected decisions in orchestration while letting the worker stop cleanly at a boundary.
- `FailPipeline` is an orchestration decision after task failure, retry exhaustion, or unrecoverable worker state.
- `DrainWorker` supports graceful shutdown without accepting more assignments.
- `CancelGrant` gives a cooperative cancellation path before hard termination.
- `TerminateWorker` is the last resort for fatal protocol errors, timeout, shutdown, or version mismatch.

Event rationale:

- `WorkerOnline` proves the child process reached the runtime contract and carries the exact version check.
- `WorkerReady` separates transport/process readiness from pipeline activation.
- `PipelineCatalog` lets orchestration verify that pipeline task ids still match the run plan.
- `PipelineStarted` confirms the worker has an active pipeline context.
- `TaskReady` is the synchronization point where orchestration decides grant, stop, wait, or fail.
- `GrantAccepted` distinguishes a command written to the channel from a grant committed by the worker.
- `TaskStarted`, `TaskSucceeded`, and `TaskFailed` are the task attempt lifecycle.
- `RetryScheduled` records retry intent before the next grant is issued.
- range events summarize range grants while preserving per-task events.
- pipeline terminal events close the active pipeline context.
- worker terminal/fault events close or fault the process context.
- `Heartbeat` prevents silent-live worker hangs.
- `Diagnostic` carries bounded operational evidence without becoming control flow.

### Pipeline Activation

Starting `meta-pipeline.exe` loads a worker process and the pipeline workspace. It does not by itself begin executing a particular `MetaPipeline.Pipeline` entity.

Orchestration should explicitly send `StartPipeline` before any task grants. This matters because one worker host may eventually be able to execute more than one pipeline entity from the same workspace, sequentially or through a future worker-pool model. The active pipeline context should include:

- pipeline id and name
- pipeline workspace fingerprint
- transform/binding workspace fingerprints used for this run
- orchestration run id
- pipeline execution instance id
- first executable task boundary

After `PipelineStarted`, the worker emits `TaskReady` for the first orchestration-visible task boundary. Orchestration then grants work, stops the pipeline, or leaves it waiting until dependencies/resources allow progress.

If orchestration is replacing a worker after retryable worker loss, `StartPipeline` carries the task id where the replacement must resume. The worker must validate that task id against the loaded pipeline plan and emit `TaskReady` for that boundary. It must not execute earlier tasks in the same pipeline. Empty resume task id means normal activation at the first executable task.

### Grant Ranges

`GrantRange` is an optimization, not a different semantic model. A range grant is legal only when orchestration has already proved that every task in the contiguous serial segment can run without another orchestration decision point between them.

`GrantRange` can be used when:

- all tasks in the range belong to one active pipeline
- the tasks are contiguous in the modeled pipeline order
- all predecessor dependency conditions are already satisfied
- locks/resources for the whole range can be acquired up front
- no task inside the range is a known cross-pipeline synchronization boundary
- no task inside the range has an unresolved conditional branch decision

The range command should carry:

- one range grant id
- ordered task grant ids
- pipeline id
- first and last pipeline task id
- lock/resource claim summary

The worker still emits per-task `TaskStarted`, `TaskSucceeded`, and `TaskFailed` events. If any task in the range fails, the worker stops executing the range, emits `GrantRangeFailed`, and waits for orchestration to decide whether to grant a modeled failure branch, stop the pipeline, fail the pipeline, drain the worker, or terminate the worker.

Semantically:

```text
GrantRange(T1,T2,T3) == GrantTask(T1), GrantTask(T2), GrantTask(T3)
```

The only difference is fewer round trips across the control channel.

### Failure Commands And Behavior

After a `TaskFailed` event, the worker must not advance to the next task automatically. It enters an awaiting-decision state at the failed task boundary.

Orchestration can then send:

- `GrantTask` for a retry of the same failed task attempt when retry policy allows it
- `GrantTask` or `GrantRange` for a modeled `OnFailure` branch that is eligible
- `StopPipeline` when the pipeline path is blocked or not selected but the failure is handled elsewhere
- `FailPipeline` when the pipeline is terminally failed for this run
- `DrainWorker` when no more work should be assigned to that worker after current terminal handling
- `CancelGrant` when cooperative cancellation is supported and the grant is still running
- `TerminateWorker` for fatal protocol errors, hard timeouts, or process shutdown

Failure behavior:

- task execution failure is reported by MetaPipeline as `TaskFailed`
- retryable `TaskFailed` attempts receive a new `GrantTask` with a new grant id, previous grant id, and incremented attempt number
- orchestration classifies downstream tasks as blocked, not selected, or eligible failure-branch work
- a failed grant releases locks/resources before new grants are evaluated
- `FailPipeline` releases the active pipeline context but keeps the worker process alive if it can safely accept another pipeline
- process exit while a grant is running is treated as grant failure unless a terminal event for that grant was already received
- if retry policy allows the failure, orchestration starts a replacement worker with `StartPipeline` carrying the failed task id as the resume boundary
- replacement workers must emit `TaskReady` for that resume boundary; earlier same-pipeline tasks must not be replayed
- process exit while idle after `DrainWorker` or after all active pipelines are terminal is normal

### Scheduler Shape

Local orchestration now maintains explicit in-memory state machines for the live supervisor. Legal movement is represented as transition rows, not scattered `if` statements:

```text
StateTransition<CurrentState, Trigger, NextState>
```

There are separate transition tables for worker lifecycle and task/grant lifecycle. The side dictionaries still carry process, lock, retry-delay, and output payloads, but task and worker state changes go through the transition applicator.

- workers: starting, online, ready, draining, exited, faulted
- pipelines: unassigned, assigned, ready, running, blocked, completed, failed
- tasks: pending, ready, grant issued, grant accepted, running, retry scheduled, succeeded, failed, blocked
- grants: issued, accepted, running, completed, failed, cancelled, timed out
- locks/resources: held by grant id, released on terminal task outcome
- workspace lease: acquired, renewed, released, stale, stolen only by explicit recovery

The first implemented state machines are local and process-owned. They reject duplicate ready tasks on one worker, `TaskReady` before `PipelineStarted`, terminal task events before `TaskStarted`, blocked transitions from active grants, and replacement-worker retry without the active-grant -> retry-scheduled -> pending resume path.

The scheduler loop should be event-driven:

1. consume worker events
2. update runtime state
3. release locks/resources from terminal grants
4. evaluate newly ready tasks against dependency outcomes and resource availability
5. issue grants or stop/drain workers
6. repeat until all reachable tasks are terminal and all workers are drained

This is not topological ordering. It is repeated readiness evaluation over runtime state.

### Logical Liveness Guards

The supervisor must not wait for a worker message when the current state already proves no useful message can arrive. These cases should fail fast as runtime/protocol faults, not as SQL task failures:

- a worker process fails to start after earlier workers were already started
- a worker exits before its first `TaskReady`
- a worker exits after `TaskReady` but before orchestration can grant or stop the task
- orchestration attempts to send `GrantTask` or `StopPipeline` after the worker control channel has closed
- worker emits `TaskReady` for a task that is not pending
- worker emits a second `TaskReady` while already waiting at a task boundary
- worker emits `TaskReady` while an earlier grant is still running
- worker emits `GrantAccepted`, `TaskStarted`, `TaskSucceeded`, or `TaskFailed` without a matching active grant
- worker emits a terminal event for a task other than the granted task
- worker emits an event for a task id that is not in the run plan
- all live workers are waiting at `TaskReady`, no task can be granted, and no task is running that could change dependency outcomes
- all worker processes have closed while pending/running/ready runtime state remains
- worker stays silent past the worker-event timeout during activation or while a grant is running

The current local runtime explicitly validates the command/event state before accepting worker events. If worker startup fails partway through, already-started workers are disposed. If a worker exits before a grant is sent, orchestration will not mark the task running first and then wait for a terminal event that cannot arrive. When every live worker is already `TaskReady` and no command can be sent, it reports the blocking predecessor state rather than waiting forever on the control pipe. Examples include dependency cycles, a predecessor not present in the run plan, or a predecessor trapped behind another ready boundary.

Local execution also has opt-in worker-event timeouts. Workers parked at `TaskReady` do not count as silent because they are intentionally waiting for orchestration. Startup/activation silence and running-grant silence count only when the matching timeout is configured. `0` or omission means no timeout. `--worker-activation-timeout-seconds` can override startup/activation silence; omitted follows `--worker-event-timeout-seconds`, while `0` disables activation timeout. Running-grant silence terminates the worker and records the active task as failed/unknown only when the configured timeout is reached. If retry policy allows the timeout failure class and the task is retry-safe, orchestration starts a replacement worker at the same task boundary; otherwise it blocks the remaining serial pipeline path and lets unrelated viable paths continue.

Cooperative cancellation and drain timeouts can be added as live-supervisor behavior. Durable supervisor restart/recovery is out of scope for this runtime shape.

### Workspace Execution Lease

Multiple `meta-orchestration.exe` processes must be allowed to coexist. They must not execute the same orchestration workspace at the same time.

Execution should acquire an exclusive lease before refreshing/saving the run plan or starting workers. The lease is scoped to the canonical orchestration workspace identity:

- resolved absolute workspace path
- workspace model identity when available
- run-plan fingerprint after refresh

Local first implementation:

- use an exclusive operational lock file keyed by a stable hash of the canonical workspace path, plus an in-process guard for same-process concurrency
- write a small operational lease record outside the modeled XML workspace, for diagnostics and stale-lock recovery
- hold the lease until all workers are drained/terminated and the final run outcome is recorded
- read-only commands such as `inspect`, `list-issues`, and `inspect-run-plan` do not require the execution lease

This lease is operational evidence, not modeled orchestration truth. It should not be written as normal `instances/*.xml` metadata. A simple local lock file is enough for the short-lived supervisor shape.

### Run Artifacts And Evidence

Runtime state remains in-process. Automatic same-task resume is limited to a live supervisor replacing a failed worker process during the same execute invocation.

Run evidence is stored in a per-run artifact directory outside modeled workspaces. Do not write run attempts, worker diagnostics, or logs into the normal MetaOrchestration workspace and then import them back as metadata.

The run artifacts should record:

- orchestration run id
- orchestration workspace identity and execution lease holder
- run-plan fingerprint
- worker process lifecycle
- active pipeline instances
- task grants
- range grants
- task outcomes
- blocked/not-selected outcomes
- lock/resource acquisitions and releases
- protocol faults

This evidence is not a replacement for the modeled orchestration workspace and not a source for SQL or dependency truth. If the supervisor crashes, these artifacts are for diagnosis and manual rerun decisions, not automatic replay.

### Production Runtime Features

The runtime should include the ordinary supervisor features expected from a serious command-line data platform. These are operational capabilities around the modeled run plan; they are not new metadata truth and do not imply a long-running service.

Run identity and evidence:

- stable orchestration run id for every execute invocation
- sanitized command line, exact executable versions, machine name, process id, and user identity when available
- orchestration workspace fingerprint, run-plan fingerprint, pipeline workspace fingerprint, transform workspace fingerprint, and binding workspace fingerprint
- per-run artifact directory outside the modeled workspaces
- flushed event journal for diagnosis and manual rerun decisions
- configurable retention for run artifacts, worker diagnostics, and event history

Protocol safety:

- exact executable-version handshake before any `StartPipeline`
- idempotent command ids for `StartPipeline`, `GrantTask`, `GrantRange`, `CancelGrant`, `DrainWorker`, and `FailPipeline`
- duplicate event handling by event id and grant id
- monotonic worker sequence numbers so orchestration can detect gaps
- explicit protocol violation outcomes that fail only the affected worker/pipeline unless the run cannot continue

Worker supervision:

- process start timeout, online timeout, heartbeat timeout, task start timeout, task execution timeout, drain timeout, and termination timeout
- heartbeat records include worker state, active pipeline, active grant, and last completed event sequence
- graceful shutdown path: stop issuing grants, cancel if policy allows, drain worker, then terminate only after timeout
- worker cleanup when the live supervisor cancels, times out, or exits normally
- bounded stderr/stdout capture for diagnostics without backpressure deadlocks
- worker restart is allowed only when no grant is active or when policy says the active grant can be retried safely

Retry policy:

- retry is a first-production-cut feature, not an unsupported placeholder
- retry policy is orchestration model data: `RetryPolicy`, `RetryPolicyFailureClass`, and `RunPlanRetryPolicy`
- retry decisions are orchestration decisions, never automatic worker advancement
- each retry is a new grant id with an attempt number and optional previous grant id
- default policy is conservative but real: max attempts, delay/backoff, and retryable failure classes
- retryable failure classes include transient SQL/connectivity failure, worker crash before terminal task event when the task is declared retry-safe, heartbeat/task timeout when policy allows cancellation/termination, and explicitly retryable worker-reported failures
- non-retryable failure classes include version mismatch, malformed protocol, invalid workspace/run plan, missing task ids, deterministic binding/model errors, exhausted retry budget, and tasks not declared retry-safe when effects could be duplicated
- retries reacquire locks/resources for the new grant attempt
- retry evidence records original grant, new grant, attempt number, failure class, delay, and final exhaustion reason
- range retry is decomposed to the failed/current task boundary unless the whole range is declared retry-safe
- retry policy can start as a run-level default with task-level overrides added later

Run isolation:

- one execution lease per orchestration workspace
- one run artifact root per orchestration run
- no reports or operational evidence written into modeled workspace folders
- workers receive only the workspace paths and environment variables needed for their assigned work
- secrets are passed by environment variable names or process environment, not serialized into workspace metadata or durable run events

Policy hooks:

- retry policy is explicit and enforced by orchestration, with retry-safe task/effect rules protecting non-idempotent work
- cancellation policy distinguishes cooperative cancellation from hard termination
- failure policy distinguishes task failure, pipeline failure, worker fault, and supervisor fault
- resource policy can start with global parallelism but must remain separate from dependency and lock semantics
- lock/resource acquisitions are tied to grant ids and released on terminal grant outcomes

Observability:

- compact attached-console progress
- quiet machine-friendly output for redirected/headless runs
- structured diagnostics for workers, grants, pipelines, and run summary
- final summary includes succeeded, failed, blocked, not-selected, cancelled, and timed-out counts
- metrics hooks for task duration, wait duration, queue depth, active grants, worker restarts, cancellation count, retry count, and timeout count

Log handling:

- control messages and logs must use separate channels or separate typed message classes with independent budgets
- stdout/stderr may be captured for diagnostics, but never as the production control plane
- every worker gets bounded log capture: maximum line length, maximum bytes per task attempt, maximum bytes per worker, and maximum bytes per run
- high-volume worker diagnostics are written to per-run artifact files with rotation, not inserted one row per line into an operational database
- the run journal records log file references, byte counts, truncation flags, severity counts, and selected failure excerpts
- repeated identical diagnostics should be coalesced with counts
- noisy diagnostics should be rate-limited with explicit dropped/truncated counters
- attached console output stays compact; full worker logs are opt-in through diagnostics bundle/export commands
- log writes are batched and flushed on interval/terminal events so a failure storm does not overload SQL Server or file storage
- secrets and connection strings are redacted before logs become durable evidence
- log retention is capped by run age and total size, with cleanup outside modeled workspaces

Operational commands should eventually cover:

- inspect recent run artifacts
- show the current lease holder for a workspace
- request cancellation of a run
- request drain of a run
- clear a stale local lease through an explicit recovery command
- export a diagnostics bundle for a run
- validate that all referenced workspaces and executable versions are compatible before execution starts

### Failure Rules

- worker process exits before `WorkerOnline`: worker startup failure
- worker exits while holding a grant: granted task failed unless a terminal task event was already received
- worker exits while holding a range grant: the current task grant fails and the range fails
- worker stops at a blocked task after `StopPipeline`: normal blocked pipeline path
- worker receives `GrantTask` before `StartPipeline`: protocol violation
- worker receives `GrantRange` containing non-contiguous tasks: protocol violation
- heartbeat timeout while idle: worker fault, restart may be possible if no grant is held
- heartbeat timeout while running: granted task timeout/failure; orchestration decides whether retries exist
- executable version mismatch: fail before assignments
- malformed message: fatal protocol violation for that worker
- retry budget exhausted: task failed with retry-exhausted evidence and downstream dependency handling continues from that failed outcome
- log budget exhausted: continue execution, truncate/coalesce diagnostics, and record the truncation evidence unless the logging subsystem itself becomes unavailable in a way that threatens control-channel progress

### First Production Cut

The first production-worthy cut should implement:

- named-pipe control channel
- typed protocol messages and exact executable-version handshake
- exclusive orchestration workspace execution lease
- one worker process per participating pipeline, started by orchestration
- explicit `StartPipeline` before task grants
- replacement-worker resume boundary through `StartPipeline` task id
- worker lifecycle state in `MetaOrchestration.Core`
- grant ids for every task execution
- range grant ids for `GrantRange`
- idempotent grant command handling
- worker heartbeat and timeout handling
- first-class retry policy with grant attempt numbers, retry-safe classification, backoff, exhaustion evidence, and metrics
- bounded local run/event evidence in the run artifact directory
- sanitized run fingerprints for every participating workspace and executable
- explicit drain and stop behavior
- explicit cancellation and failure commands
- bounded diagnostics/log capture with per-task, per-worker, and per-run budgets plus rotation/truncation evidence
- final run summary with failed, blocked, not-selected, cancelled, and timed-out counts
- stdout/stderr reserved for diagnostics
- no distributed workers, no supervisor-restart resume

Distributed workers and supervisor-restart resumability are out of scope for `meta-orchestration execute`.

## Orchestration Execute Command

First CLI shape:

```cmd
meta-orchestration execute ^
  --workspace .\OrchestrationWS ^
  --pipeline-workspace .\PipelineWS ^
  --transform-workspace .\TransformWS ^
  --binding-workspace .\BindingWS ^
  [--data-type-conversion-workspace .\DataTypeConversionWS] ^
  [--pipeline-db-connection-env META_PIPELINE_OPERATIONAL_SQL] ^
  [--max-degree-of-parallelism 4] ^
  [--run-artifacts-root .\TestRuns] ^
  [--worker-event-timeout-seconds 0]
  [--worker-activation-timeout-seconds 0]
  [--worker-control-pipe-connect-timeout-seconds 0]
```

The command takes an exclusive execution lease for the orchestration workspace, refreshes deterministic run-plan rows from current workspace state, writes an operational run journal, and then executes. It does not infer SQL access or bind SQL on the fly.

The current local worker bridge captures non-protocol worker stdout/stderr into bounded per-run artifact files under `logs\`. The run journal records each worker log path and a terminal summary with captured bytes, dropped bytes, and truncation flags. These files are operational evidence and are not written into modeled workspace folders.

## Runtime Loop

The first process-based runtime can be simple:

1. Resolve the orchestration workspace path, run id, run artifact root, and execution lease.
2. Load the orchestration workspace.
3. Validate there is exactly one ready `RunPlan`.
4. Keep pending, running, completed, failed, and blocked planned-task sets.
5. Start any pending task whose dependency conditions are satisfied and whose planned locks are compatible with currently running tasks.
6. Launch one `meta-pipeline execute-worker` process per participating pipeline.
7. Send `StartPipeline` after `WorkerReady`.
8. Receive worker `TaskReady` events and grant only tasks whose dependencies, locks, and resource limits are satisfied.
9. On `TaskFailed`, resolve the modeled retry policy and either grant a retry attempt or close the pipeline path as terminally failed.
10. On retryable worker loss while a grant is running, start a replacement worker and send `StartPipeline` with the failed task id as the resume boundary.
11. Treat silent activation/running-grant periods past configured timeout options as worker protocol faults; retry running-grant timeouts only when policy allows it. `0` means no timeout.
12. Record task outcomes from worker events into the run journal.
13. Send `STOP` to a worker when its next pipeline task is blocked by orchestration dependency conditions.
14. Continue unrelated viable run-plan paths.
15. Return nonzero when any planned task failed terminally or any required downstream task was blocked.

The run plan has already encoded dependencies and declared lock requests. Runtime should not rediscover SQL access semantics.

## Progress Surface

Console progress should stay compact:

```text
[========------------] 8 of 24  2 running
[====================] 24 of 24
```

Attached-console mode renders one live line. Redirected/headless runs stay quiet and child output remains captured for final failure diagnostics. The default should not become a wall of interleaved process logs.

## Failure Policy

The first execution slice is conservative about recovery but not about unrelated work:

- continue viable DAG paths by default
- block only tasks whose dependency conditions are not satisfied
- let `OnFailure` dependency branches run when their predecessor failed
- treat unchosen failure branches after successful predecessors as blocked/non-selected branches
- retry only through explicit orchestration retry policy, with a new grant id per attempt and retry-safe classification
- only resume automatically at a task boundary after retryable worker loss; do not replay prior same-pipeline tasks
- do not repair partial pipeline state
- do not continue tasks blocked by failed predecessors
- report blocked tasks separately from failed tasks

Partial rerun and manual recovery can be added later. Supervisor-restart recovery is out of scope: if `meta-orchestration.exe` crashes, the run stops and the captured artifacts are used for diagnosis/rerun decisions. Retry remains part of the runtime contract because transient worker, SQL, and connectivity failures are normal operational conditions while the supervisor is alive.

## Conditional Dependencies

Next-step conditionals are dependency types on the DAG:

```cmd
meta-orchestration add-dependency ^
  --workspace .\OrchestrationWS ^
  --from-task LoadCustomers.load-customers ^
  --to-task FailureHandler.record-failure ^
  --condition failure ^
  [--reason "Record and notify after customer load failure."]
```

This records a user-authored dependency resolution row in the orchestration workspace. `refresh-run-plan` then places the handler task after its predecessor, and `execute` evaluates the dependency condition from the predecessor outcome.

The current dependency conditions are:

- `OnSuccess`: successor runs only when the predecessor succeeded
- `OnFailure`: successor runs only when the predecessor failed

Failure handlers are normal pipeline tasks in the run plan. There is no separate failure-action launcher after the run plan completes.

The first slice intentionally does not model retries, compensating transactions, OR-joins, or richer gateway semantics.

## Resource Throttling

Task dependencies answer:

```text
Is this task eligible after predecessor outcomes are known?
```

Planned locks answer:

```text
May these tasks safely overlap against the same modeled object?
```

Resource policy answers:

```text
Should these tasks actually overlap given CPU, memory, disk, network, source-system, tempdb, or SSAS pressure?
```

Do not collapse these into one concept.

The first runtime may support a global `--max-degree-of-parallelism`, but the model should leave room for explicit resource rows later:

- `ExecutionResource`
- `TaskResourceClaim`
- `ResourceConcurrencyPolicy`

A generic named-resource model is preferable to hard-coding CPU/network/disk semantics too early. Users know their bottlenecks; orchestration should give them modeled throttles.

Example future CLI shape:

```cmd
meta-orchestration add-resource --workspace .\OrchestrationWS --name DW --max-concurrent 2
meta-orchestration claim-resource --workspace .\OrchestrationWS --task LoadFactSales.load-fact-sales --resource DW --weight 1
```

Then a task can start only when:

- all dependency predecessors are complete
- planned locks are compatible
- resource capacity is available
- global execution capacity is available

## Non-Goals For First Execute Slice

- calendars
- worker pools
- distributed workers
- distributed locks
- supervisor-restart resumability
- automatic partial rerun
- queue storage
- resource optimization
- run-plan mutation during execution
- direct SQL execution owned by MetaOrchestration

## Acceptance Criteria For First Runtime Slice

- `meta-pipeline execute-worker` preserves one process context for one modeled pipeline.
- `meta-orchestration execute` refreshes and consumes run-plan rows from an orchestration workspace.
- execution launches `meta-pipeline execute-worker` child processes, not whole-pipeline fire-and-forget execution and not one-shot step execution.
- execution starts ready tasks by traversing the dependency graph.
- execution honors planned task locks.
- execution honors `--max-degree-of-parallelism`.
- failed tasks block only their downstream dependents.
- unrelated viable paths continue by default.
- `OnFailure` dependency branches run after a planned task failure.
- unchosen failure branches are blocked/non-selected without making the run plan execution fail.
- progress is compact and readable.
- no SQL binding/parsing/execution logic is duplicated in MetaOrchestration.
- resource policy remains explicitly out of scope except for the global throttle.
