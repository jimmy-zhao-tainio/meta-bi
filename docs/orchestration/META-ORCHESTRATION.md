# MetaOrchestration

`MetaOrchestration` owns dependency planning, policy-facing access analysis, and lock-aware run plans across `MetaPipeline` units.

It does not own SQL parsing, SQL binding, pipeline task execution, operational DB evidence, calendars, or external orchestrator execution. The current bounded slice automatically derives task/object effects, infers the safe dependency graph from already-bound transform profiles, records determinism/synchronization issues when the model cannot prove safe ordering, and creates a run plan when policy is complete.

## Stage 1 Contract

Inputs:

- a `MetaPipeline` workspace
- the `MetaTransformScript` workspace used by its transform steps
- the `MetaTransformBinding` workspace for those scripts

The analyzer consumes binding-shaped profiles:

- pipeline references
- ordered transform task profiles
- object reads
- object writes
- object read/write mutations
- object reset writes

`ObjectAccess` remains raw evidence. It is not the final orchestration semantic contract.

The analyzer derives `TaskObjectEffect` rows from that evidence:

- access direction
- write effect
- access purpose
- whether the effect participates in data dependency inference
- whether the task is a published producer
- whether synchronization is required
- conservative lock mode

The product word is `Profile`, not `Fact`, to avoid BI fact-table ambiguity.

## Validity

Dependency ordering, write determinism, and runtime synchronization are separate concerns.

`OrchestrationPlan.DagStatus` is:

- `Complete` when the task dependency graph has no blocking dependency issue
- `Invalid` when the dependency graph itself cannot be trusted

`OrchestrationPlan.DeterminismStatus` is:

- `Deterministic`
- `RequiresExplicitOrdering`
- `Invalid`

`OrchestrationPlan.SynchronizationStatus` is:

- `Complete`
- `HasConstraints`
- `RequiresPolicy`

Invalid and policy-requiring workspaces are still saved so the issues can be reviewed, corrected, and re-run by users or automation. Policy corrections are also modeled in the workspace rather than passed as ad-hoc command memory.

## Automatic Inference

Dependency inference is automatic for the safe modeled cases. It uses derived semantics rather than raw access kinds, so orchestration is not a hand-written schedule pasted on top of the pipeline.

The safe automatic case is published-producer to dependency-consumer:

- pipeline A writes object X
- pipeline B reads object X
- infer A before B

The actual model records `TaskDependency` first, then projects cross-pipeline data edges to `PipelineDependency` for overview.

A lone reset write is allowed when no other pipeline touches that object. A same-pipeline reset followed by a write is treated as an isolated replace sequence, and consumers wait for the effective producer task.

The boundary is deliberate: when a dependency, write order, or lock behavior can be inferred safely from modeled profiles, `MetaOrchestration` writes it. When it cannot, it writes a modeled issue or requires an explicit policy row instead of smuggling a scheduler guess into runtime code.

That makes the ordinary modeled path automatic in practice: bound transform tasks and modeled executable tasks become task profiles, safe producer/consumer relationships become dependencies, and complete policy becomes a runnable plan. Dynamic SQL or legacy opaque procedural SQL needs explicit guarantees before orchestration can rely on it.

The analyzer distinguishes:

- same-target append writers, which can leave the DAG complete while recording synchronization constraints
- replacement mixed with append, which leaves the DAG complete but requires explicit write-order policy
- same-table mutations, which require determinism/synchronization policy without creating artificial dependency cycles
- unsafe shared reset, which blocks the dependency graph

The analyzer blocks the DAG when:

- a reset/destructive write intersects with another pipeline touching the same object and no safe order is inferable
- existing dependencies form a cycle
- a pipeline step references a missing script or binding
- a pipeline step references a known non-executable helper script, currently scalar function definitions modeled as `ScriptObjectScalarFunction`

Scalar function definitions can live in the transform workspace used by pipelines. They are helper objects for SQL expressions, not data-moving statements, so they contribute no object accesses by themselves. If a pipeline task references one directly, the analyzer emits `NonExecutableTransformScript` instead of the generic missing/unsupported script issue. When an executable transform calls a modeled same-workspace scalar function, binding can surface table reads from the supported return-expression body, so those reads participate in normal producer-to-consumer dependency inference.

## Workspace Shape

The inference model records:

- `OrchestrationPlan`
- `PipelineReference`
- `DataObject`
- `TaskAccessProfile`
- `ObjectAccess`
- `PipelineObjectAccess`
- `TaskObjectEffect`
- `TaskDependency`
- `PipelineDependency`
- `DependencyIssue`
- `DependencyIssuePipeline`

The policy and run-plan model records:

- `TaskOrderingResolution`
- `LockCompatibilityPolicy`
- `RetryPolicy`
- `RetryPolicyFailureClass`
- `RunPlan`
- `RunPlanRetryPolicy`
- `PlannedTask`
- `PlannedTaskLock`

These are model/instance rows in the orchestration workspace. The current persistence surface is the normal deterministic workspace format.

## Policy Rows

`TaskDependency` and `TaskOrderingResolution` both carry a dependency condition:

- `OnSuccess`
- `OnFailure`

Inferred producer/consumer dependencies are `OnSuccess`. User-authored failure branches are modeled as `OnFailure` DAG edges between normal task profiles.

`TaskOrderingResolution` records explicit predecessor/successor order between two task access profiles. It is used to resolve write determinism cases such as replacement plus append or two same-object mutators, and it can also author an explicit failure branch. The relationship may name the relevant `DataObject`, but the ordered tasks remain the primary truth.

`LockCompatibilityPolicy` records scoped lock compatibility for an object/effect interaction. It is intentionally not a blanket table switch. The row names the data object, left write effect, right write effect, lock behavior, status, and reason.

The first supported lock behaviors are:

- `Serialize`
- `AllowConcurrent`

These policies affect run planning only. They do not create data dependencies and they do not rewrite pipeline tasks.

`allow-concurrent-append` is a convenience command that creates an `Append`/`Append` `LockCompatibilityPolicy` with `AllowConcurrent`.

`set-lock-policy` is the generic scoped command. For example, conditional inferred-member repair can be represented as `ConditionalKeyedUpsert`/`ConditionalKeyedUpsert` with `Serialize` until a future model can prove a unique-key and atomic-upsert contract.

`AllowConcurrent` is currently accepted only for `Append`/`Append`. Other same-object interactions can be explicitly serialized, but concurrent keyed-upsert or mutation allowance needs a stronger modeled safety contract first.

`RetryPolicy` records orchestration retry truth for a plan. It names the policy kind, attempt budget, delay/backoff values, whether read-only and write-effecting tasks are retry-safe by default, status, and reason.

`RetryPolicyFailureClass` records which failure classes the policy may retry. A failure class omitted from the policy is not retryable by that policy.

`RunPlanRetryPolicy` assigns the default retry policy to a run plan. Retry policy is not a CLI-only runtime knob; execution resolves it from the orchestration workspace after refreshing the run plan.

## Run Planning

`refresh-run-plan` writes lock-aware run-plan rows from declared task access profiles, `TaskObjectEffect.LockMode`, and active `LockCompatibilityPolicy` rows. It does not dependency-order the tasks. `TaskDependency` and active `TaskOrderingResolution` rows remain graph edges that execution evaluates at runtime.

The run-plan builder:

- requires `DagStatus=Complete`
- fails when any issue still blocks the DAG
- fails when determinism or synchronization policy is still required
- preserves dependency edges as runtime graph constraints
- writes `RunPlan`, default `RunPlanRetryPolicy`, stable `PlannedTask` rows, and per-task `PlannedTaskLock` rows
- records reasons on planned tasks and locks, including lock policy source when a policy applies

If a plan has no active `RetryPolicy`, run planning creates a conservative `DefaultRetryPolicy`: three attempts, bounded exponential backoff, retry for read-only tasks by default, no write-task retry by default, and transient failure classes such as SQL/connectivity failures and retryable worker-reported failures. If more than one active retry policy exists for the plan, run planning fails until the policy is made unambiguous.

The initial lock compatibility is conservative:

- `SharedRead` can overlap with `SharedRead`
- `AppendWrite` can overlap with `AppendWrite` only under a matching `Append`/`Append` `AllowConcurrent` policy
- replacement, mutation, reset, and exclusive-style writes do not overlap with other same-object access

## Execution

`execute` takes an exclusive execution lease for the orchestration workspace, refreshes deterministic run-plan rows from the current orchestration workspace state, then consumes those rows. It does not infer SQL access, bind SQL, or execute pipeline logic itself. It launches one `meta-pipeline execute-worker` process per participating pipeline with a dedicated named pipe control channel, checks the worker's exact executable version, sends `StartPipeline` after `WorkerReady`, receives task `TaskReady` events only after `PipelineStarted`, grants only `PlannedTask` rows whose dependency conditions are satisfied and whose planned locks are compatible with currently running tasks, and stops a worker when its next serial pipeline task is blocked. `--max-degree-of-parallelism` limits concurrently granted tasks, not pipeline process count. Transform and binding workspaces are required only when planned tasks include transform-backed `MetaPipeline` steps; executable-only pipeline workers do not require those workspace arguments.

`--worker-event-timeout-seconds` is an opt-in timeout for silent worker protocol periods. `0` or omission means no worker-event timeout. It does not apply while a worker is parked at `TaskReady` waiting for orchestration. It does apply during startup/activation and while a grant is running when configured. `--worker-activation-timeout-seconds` can override startup/activation silence; omitted follows `--worker-event-timeout-seconds`, while `0` disables activation timeout. Running-grant silence marks the active task failed/unknown before terminating the worker path only when the configured timeout is reached.

Execution writes a local run journal, bounded worker log artifacts, and lease record under the configured run artifacts root, or under the default user-local `meta\orchestration` operational directory. These are operational evidence files, not modeled workspace metadata. Multiple `meta-orchestration` processes can execute different orchestration workspaces at the same time, but a second execution against the same workspace fails while the lease is active.

Execution continues viable DAG paths by default. A failed task attempt is evaluated against the run-plan retry policy before it becomes a terminal task failure. When retry is allowed after a worker-reported `TaskFailed`, orchestration releases the failed attempt's locks, records retry evidence, and sends a new `GrantTask` with a new grant id and incremented attempt number. When retry is allowed after worker loss or a running-grant timeout, orchestration starts a replacement worker and sends `StartPipeline` with the failed task id as the resume boundary so earlier same-pipeline tasks are not replayed. When retry is not allowed, the task is marked failed and the pipeline worker path is closed. Downstream `OnSuccess` branches are blocked, downstream `OnFailure` branches become eligible, and unrelated planned paths continue. An `OnFailure` branch after a successful predecessor is treated as a non-selected branch rather than a run failure. The command returns nonzero when any task failed terminally or any required success branch was blocked.

Failure handlers are not post-run action hooks. A handler pipeline is part of the same run plan and runs through the same pipeline-worker protocol as any other planned task.

## CLI

`MetaOrchestration` does not currently have an empty `init` command. The `create` command creates the orchestration workspace from modeled pipeline profiles; dependency inference is part of that creation operation rather than a separate user intention. Transform-backed pipeline steps carry transform and binding workspace paths in the pipeline model; executable process steps are included as dependency-neutral task profiles. Run planning is a resolution/planning pass inside that same workspace, so the common path is model -> orchestration workspace -> refreshed run plan -> execution.

```cmd
meta-orchestration create --pipeline-workspace .\PipelineWS --output-xml .\OrchestrationWS
meta-orchestration list-issues --workspace .\OrchestrationWS
meta-orchestration add-order --workspace .\OrchestrationWS --from-task RefreshStage.load --to-task AppendStage.load --object dbo.Stage --reason "Refresh before append."
meta-orchestration add-dependency --workspace .\OrchestrationWS --from-task RefreshStage.load --to-task FailureHandler.record --condition failure --reason "Record failed refresh."
meta-orchestration allow-concurrent-append --workspace .\OrchestrationWS --object dbo.Stage --reason "Append-only stage writers can overlap."
meta-orchestration set-lock-policy --workspace .\OrchestrationWS --object dbo.Stage --left-effect Mutation --right-effect Mutation --behavior serialize --reason "Stage access should not overlap."
meta-orchestration refresh-run-plan --workspace .\OrchestrationWS
meta-orchestration inspect-run-plan --workspace .\OrchestrationWS
meta-orchestration execute --workspace .\OrchestrationWS --pipeline-workspace .\PipelineWS --max-degree-of-parallelism 4 --run-artifacts-root .\TestRuns
```

Workspace creation returns nonzero when `DagStatus` is `Invalid`, while still writing the orchestration workspace with issue rows. A complete DAG may still have determinism or synchronization issues that must be handled before automatic parallel run planning. `refresh-run-plan` returns nonzero until those policy gaps are resolved.

## Runtime Boundary

The current runtime is process-based and local. It starts `meta-pipeline` child processes, but those children are pipeline workers, not one-shot task processes. MetaPipeline preserves pipeline context inside the worker; MetaOrchestration owns cross-pipeline task synchronization through named-pipe worker events and grants. Local task-attempt retry after `TaskFailed`, retryable worker-loss replacement, and running-grant timeout retry exist through modeled retry policy. `meta-orchestration execute` is intentionally short-lived: if the supervisor process itself crashes, the run is a hard stop and the run artifacts/logs are used for diagnosis or manual rerun decisions. Worker liveness policy, worker pools, distributed locks, calendars, manual rerun support, resource pools, and placement optimization remain future work.

More runtime detail is captured in [META-ORCHESTRATION-EXECUTION.md](META-ORCHESTRATION-EXECUTION.md). The key boundary is that orchestration executes planned task-grain work by coordinating `meta-pipeline execute-worker` child processes. It should not duplicate MetaPipeline transform execution logic.
