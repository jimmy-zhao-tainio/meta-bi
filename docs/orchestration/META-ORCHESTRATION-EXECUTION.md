# MetaOrchestration Execution

This note captures the first process-based runtime slice after run planning.

`MetaOrchestration` should execute a run plan, not an inferred plan directly and not a loose set of whole pipelines. Run-plan rows in the orchestration workspace are the contract between planning and runtime.

## Ownership Boundary

`MetaOrchestration` owns:

- reading a `RunPlan`
- deciding which planned tasks are eligible to start
- enforcing planned task locks
- applying explicit orchestration policy
- supervising external task workers
- reporting orchestration-level progress and failure

`MetaPipeline` owns:

- executing one transform-backed pipeline step
- resolving the modeled step into bound `MetaTransformScript` and `MetaTransformBinding` rows
- running SQL or row movement
- recording pipeline operational evidence
- enforcing task-level timeout and connection settings

The runtime boundary should remain:

```text
MetaOrchestration supervises.
MetaPipeline executes.
```

## Why Not Whole-Pipeline Processes

Starting `meta-pipeline execute --pipeline <name>` for each pipeline is too coarse once orchestration is task-level.

Example:

```text
P1.T1
P1.T2 depends on P2.T1
P1.T3

P2.T1
P2.T2
```

If orchestration starts whole pipelines, it cannot express `P1.T2` waiting for `P2.T1` without either over-serializing the whole pipelines or risking incorrect execution. The runtime unit therefore needs to be a pipeline step.

## Required MetaPipeline Slice

Add a small task-grain command before implementing orchestration execution:

```cmd
meta-pipeline execute-step ^
  --workspace .\PipelineWS ^
  --pipeline CustomerLoad ^
  --step-name load-customers ^
  --transform-workspace .\TransformWS ^
  --binding-workspace .\BindingWS ^
  [--data-type-conversion-workspace .\DataTypeConversionWS] ^
  [--pipeline-db-connection-env META_PIPELINE_OPERATIONAL_SQL]
```

Rules:

- execute exactly one transform-backed `PipelineTask`
- do not traverse predecessor or successor tasks
- use the same realization rules as normal pipeline execution
- keep MetaPipeline operational DB evidence
- fail when the selected step is not transform-backed
- fail when the selected step/binding/script cannot be resolved

This keeps process-based orchestration possible without weakening the task graph.

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
  [--max-degree-of-parallelism 4]
```

The command refreshes deterministic run-plan rows from current workspace state before execution. It does not infer SQL access or bind SQL on the fly.

## Runtime Loop

The first process-based runtime can be simple:

1. Load the orchestration workspace.
2. Validate there is exactly one ready `RunPlan`.
3. Keep pending, running, completed, failed, and skipped planned-task sets.
4. Start any pending task whose dependency conditions are satisfied and whose planned locks are compatible with currently running tasks.
5. Launch one `meta-pipeline execute-step` process per started task.
6. Stream or summarize child process output according to console mode.
7. Record outcomes as child processes finish.
8. Skip downstream tasks whose dependency conditions cannot be satisfied.
9. Continue unrelated viable run-plan paths.
10. Return nonzero when any planned task failed or any required downstream task was skipped.

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
- skip only tasks whose dependency conditions are not satisfied
- let `OnFailure` dependency branches run when their predecessor failed
- treat unchosen failure branches after successful predecessors as normal skipped branches
- do not retry
- do not resume
- do not repair partial pipeline state
- do not continue tasks blocked by failed predecessors
- report skipped blocked tasks separately from failed tasks

Retries, resume, partial rerun, and manual recovery are later orchestration-runtime features.

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
- retries
- resumability
- queue storage
- resource optimization
- run-plan mutation during execution
- direct SQL execution owned by MetaOrchestration

## Acceptance Criteria For First Runtime Slice

- `meta-pipeline execute-step` runs exactly one modeled transform-backed step.
- `meta-orchestration execute` refreshes and consumes run-plan rows from an orchestration workspace.
- execution launches `meta-pipeline execute-step` child processes, not whole-pipeline execution.
- execution starts ready tasks by traversing the dependency graph.
- execution honors planned task locks.
- execution honors `--max-degree-of-parallelism`.
- failed tasks block only their downstream dependents.
- unrelated viable paths continue by default.
- `OnFailure` dependency branches run after a planned task failure.
- unchosen failure branches are skipped without making the run plan execution fail.
- progress is compact and readable.
- no SQL binding/parsing/execution logic is duplicated in MetaOrchestration.
- resource policy remains explicitly out of scope except for the global throttle.
