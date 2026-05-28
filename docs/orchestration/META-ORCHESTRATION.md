# MetaOrchestration

`MetaOrchestration` owns dependency planning, policy-facing access analysis, and lock-aware run plans across `MetaPipeline` units.

It does not own SQL parsing, SQL binding, pipeline task execution, operational DB evidence, calendars, or external orchestrator execution. The current bounded slice derives task/object effects, infers data dependencies, records determinism/synchronization issues from already-bound transform profiles, and can create a run plan when policy is complete.

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

Invalid and policy-requiring workspaces are still saved so the issues can be reviewed, corrected, and re-run by users or agents. Policy corrections are also modeled in the workspace rather than passed as ad-hoc command memory.

## Automatic Inference

Dependency inference uses derived semantics rather than raw access kinds.

The safe automatic case is published-producer to dependency-consumer:

- pipeline A writes object X
- pipeline B reads object X
- infer A before B

The actual model records `TaskDependency` first, then projects cross-pipeline data edges to `PipelineDependency` for overview.

A lone reset write is allowed when no other pipeline touches that object. A same-pipeline reset followed by a write is treated as an isolated replace sequence, and consumers wait for the effective producer task.

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
- `RunPlan`
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

## Run Planning

`refresh-run-plan` builds a lock-aware topological plan from `TaskDependency`, active `TaskOrderingResolution` rows, `TaskObjectEffect.LockMode`, and active `LockCompatibilityPolicy` rows.

The run-plan builder:

- requires `DagStatus=Complete`
- fails when any issue still blocks the DAG
- fails when determinism or synchronization policy is still required
- respects intra-pipeline serial task edges
- respects cross-pipeline producer-to-consumer task edges
- writes `RunPlan`, dependency-ordered `PlannedTask` rows, and per-task `PlannedTaskLock` rows
- records reasons on planned tasks and locks, including lock policy source when a policy applies

The initial lock compatibility is conservative:

- `SharedRead` can overlap with `SharedRead`
- `AppendWrite` can overlap with `AppendWrite` only under a matching `Append`/`Append` `AllowConcurrent` policy
- replacement, mutation, reset, and exclusive-style writes do not overlap with other same-object access

## Execution

`execute` refreshes deterministic run-plan rows from the current orchestration workspace state, then consumes those rows. It does not infer SQL access, bind SQL, or execute pipeline logic itself. It repeatedly starts ready `PlannedTask` rows whose dependency conditions are satisfied and whose planned locks are compatible with currently running tasks, launching `meta-pipeline execute-step` workers up to `--max-degree-of-parallelism`.

Execution continues viable DAG paths by default. A failed task marks only that task failed; downstream `OnSuccess` branches are skipped as blocked, downstream `OnFailure` branches become eligible, and unrelated planned paths continue. An `OnFailure` branch after a successful predecessor is skipped as an unchosen branch rather than a run failure. The command returns nonzero when any task failed or any required success branch was blocked.

Failure handlers are not post-run action hooks. A handler pipeline is part of the same run plan and runs through `meta-pipeline execute-step` like any other planned task.

## CLI

`MetaOrchestration` does not currently have an empty `init` command. The root `--new-workspace` command creates the orchestration workspace by inferring from already-bound pipeline profiles. Run planning is a resolution/planning pass inside that same workspace.

```cmd
meta-orchestration --pipeline-workspace .\PipelineWS --transform-workspace .\TransformWS --binding-workspace .\BindingWS --new-workspace .\OrchestrationWS
meta-orchestration list-issues --workspace .\OrchestrationWS
meta-orchestration add-order --workspace .\OrchestrationWS --from-task RefreshStage.load --to-task AppendStage.load --object dbo.Stage --reason "Refresh before append."
meta-orchestration add-dependency --workspace .\OrchestrationWS --from-task RefreshStage.load --to-task FailureHandler.record --condition failure --reason "Record failed refresh."
meta-orchestration allow-concurrent-append --workspace .\OrchestrationWS --object dbo.Stage --reason "Append-only stage writers can overlap."
meta-orchestration set-lock-policy --workspace .\OrchestrationWS --object dbo.Stage --left-effect Mutation --right-effect Mutation --behavior serialize --reason "Stage access should not overlap."
meta-orchestration refresh-run-plan --workspace .\OrchestrationWS
meta-orchestration inspect-run-plan --workspace .\OrchestrationWS
meta-orchestration execute --workspace .\OrchestrationWS --pipeline-workspace .\PipelineWS --transform-workspace .\TransformWS --binding-workspace .\BindingWS --max-degree-of-parallelism 4
```

Workspace creation returns nonzero when `DagStatus` is `Invalid`, while still writing the orchestration workspace with issue rows. A complete DAG may still have determinism or synchronization issues that must be handled before automatic parallel run planning. `refresh-run-plan` returns nonzero until those policy gaps are resolved.

## Runtime Boundary

The current runtime is process-based and local. It starts `meta-pipeline` child processes, but it does not yet own worker pools, distributed locks, calendars, retries, resumability, resource pools, or placement optimization.

More runtime detail is captured in [META-ORCHESTRATION-EXECUTION.md](META-ORCHESTRATION-EXECUTION.md). The key boundary is that orchestration executes planned task-grain work by supervising `meta-pipeline execute-step` child processes. It should not duplicate MetaPipeline transform execution logic.
