# MetaPipeline Model Sketch

This is the broader model sketch behind the initial sanctioned `MetaPipeline` XML model.

The goal is to name the broad shape early enough that the runtime work does not accidentally turn the first SQL Server bulk insert into the whole product.

## Identity

`MetaPipeline` is the executable data-moving model for `meta-bi`.

It should describe declared pipeline work.
It should not become the intelligent orchestrator.

The orchestrator may later decide which pipeline to run, when to run it, which target to run it for, which partitions or tenants to include, and whether approvals or dependency gates are satisfied.
`MetaPipeline` should execute the declared work honestly and report what happened.

Stage 1 keeps the data-flow center deliberately narrow: one pipeline has a serial task chain.
Each transform-backed task executes one bound `TransformScript`; SELECT-kind transforms may be followed immediately by their `InsertRows` target write.
Only executable statement-backed transform scripts are valid task selections. Scalar function definitions are helper objects inside `MetaTransformScript`, so `MetaPipeline` rejects them as transform steps rather than treating them as mutation scripts.
Broader orchestration should still run multiple pipelines for independent units of work rather than turning one pipeline into a scheduler.

## Core Shape

The model center is:

| Concept | Meaning |
| --- | --- |
| `Pipeline` | A named executable definition made of declared tasks. |
| `PipelineTask` | The ordered task backbone. Concrete task meaning comes from related task-detail entities. |
| `TaskDependency` | An explicit ordering or data dependency between tasks inside the pipeline. |
| `RowStream` | The main in-memory/batched data carrier between read/transform and write tasks. |
| `RowStreamShape` | The column contract for a row stream. |
| `ConnectionReference` | A shell environment variable name that resolves to a connection string at runtime. |

The first implemented slice already proves part of this:

| Current runtime piece | Model concept |
| --- | --- |
| `TransformScript` + `TransformBinding` selection | Transform execution task nucleus |
| `PipelineRowStreamShape` | `RowStreamShape` |
| `PipelineDataBatch` | Bounded row-stream buffer |
| `IPipelineTargetWriteOperation` | Target write operation |
| `InsertRowsTargetWriteTask` | Logical insert-rows target write task. |
| `SqlServerBulkInsertTargetWriteOperation` | Current SQL Server physical realization of insert rows. |
| `MetaPipelineExecutionResult` | Minimal in-memory execution evidence |
| `MetaPipeline` operational DB | Optional persisted runtime evidence for runs, task runs, metrics, logs, and failures. |

## Pipeline

A `Pipeline` should be a plain executable definition:

| Field | Intent |
| --- | --- |
| `Name` | Stable pipeline identity. |
| `Description` | Human explanation, not runtime logic. |
| `TaskList` | Ordered task declarations. Ordered collections must remain explicit. |
| `TaskDependencyList` | Optional explicit dependencies when simple order is not enough. |
| `ConnectionReferenceList` | Named environment-variable references used by tasks. |
| `ParameterList` | Runtime values the orchestrator or caller must provide, when needed. |
| `EvidenceContract` | The minimum runtime facts the pipeline promises to report. |

The pipeline can declare structure.
It should not decide scheduling, dependency freshness, approval workflow, tenant fan-out, or environment promotion by itself.

## Task

Every task should have the same backbone:

| Field | Intent |
| --- | --- |
| `Name` | Stable task identity inside the pipeline. |
| Task detail entity | The concrete task family, for example `TransformExecutionTask` or `TargetWriteTask`. |
| `InputList` | Declared inputs such as row streams, target objects, source objects, or run values. |
| `OutputList` | Declared outputs such as row streams, target effects, evidence, or watermarks. |
| `DependsOnTaskList` | Explicit dependency references. |
| Failure behavior | A concrete policy entity when non-default behavior is needed. |
| `EvidenceContract` | Task-level counters, timings, fingerprints, and failure classification. |

This keeps task execution boring.
Different ETL semantics live in concrete task and operation entities, not in a string-valued `Kind` field or a single clever executor.

## Data Carriers

The first data carrier should remain `RowStream`.

| Carrier | Meaning |
| --- | --- |
| `RowStream` | Rows with a declared `RowStreamShape`, read in bounded buffers. |
| `DataArtifact` | A named external/staged artifact such as a file, object-store object, or staging table. |
| `ControlValue` | A scalar runtime value such as a watermark, source artifact identity, or row count. |
| `Evidence` | Runtime facts emitted by a task or pipeline run. |

Stage 1 is database-first, so `RowStream` is the important carrier now.
Other carriers can be modeled later when file, API, message, or object-store sources become real.

## Task Families

The stage 1 model keeps task families narrow and executable.

| Family | What it covers | Example task kinds |
| --- | --- | --- |
| Transform execution | Producing a row stream from sanctioned transform logic. | execute `TransformScript`, bind target, emit row-stream shape |
| Target write | Moving buffered rows into a target. | `InsertRowsTargetWriteTask` |
| Validation | Checking modeled shape and contract before execute. | pipeline graph validation, row-stream shape drift guardrail |

## Target Write Surface

`MetaPipeline` stage 1 models one target-write primitive:

| Operation | Meaning |
| --- | --- |
| `InsertRowsTargetWriteTask` | Insert the buffered row stream into the target. |

Mini-DML primitives are out of scope in `MetaPipeline`.
SQL mutation semantics belong to modeled `MetaTransformScript` SQL statements.

## Validation and Quality

Validation should be a task family, not a side effect sprinkled through writers.

The model should support:

| Validation level | Examples |
| --- | --- |
| Structural | required columns, type compatibility, length/precision/scale fit, nullability |
| Contract | unique key, reference readiness, duplicate detection, source schema version |
| Business | domain checks, value dependencies, reconciliation, control totals |
| Runtime | row count thresholds, reject thresholds, warning vs error severity |

Stage 1 already performs minimal structural shape checks.
Broader validation should become declared validation tasks.

## Runtime State and Evidence

Stage 1 runtime evidence is returned in-memory through `MetaPipelineExecutionResult`.
When enabled by CLI option, the same execution evidence is also written to the SQL Server operational DB.

`MetaPipeline` does not persist run logs, run status, checkpoints, or watermarks to sanctioned XML artifacts.
The operational DB is runtime infrastructure, not sanctioned model truth.

## Connection Handling

Connection strings are not stored in sanctioned metadata or artifacts.

`ConnectionReference` values name shell-visible environment variables.
Runtime resolves those names to connection strings immediately before execution.
Errors must name the missing or empty variable, not print the secret value.

## Orchestration Boundary

`MetaPipeline` can declare internal task order.

The orchestrator owns:

| Orchestrator concern | Reason |
| --- | --- |
| Schedules and triggers | These decide when work should happen. |
| Cross-pipeline dependency freshness | This needs global state and policy. |
| Tenant/partition fan-out decisions | These choose many executions from one definition. |
| Approval workflow | This is governance and operator interaction. |
| Resource queues and priorities | These are platform scheduling concerns. |
| Environment promotion | This is release/deployment concern. |

The pipeline may expose metadata the orchestrator can use.
It should not become the scheduler brain.

## Stage 1 Model Nucleus

The smallest honest model nucleus is:

| Model piece | Current status |
| --- | --- |
| `Pipeline` | Modeled as an explicit row added to a `MetaPipeline` workspace. |
| `TransformExecutionTask` | Modeled; runtime executes each serial task through explicit `TransformScript` and `TransformBinding` selection. |
| `ExecutableTask` | Modeled; runtime executes an external process and records success/failure from the real exit code. |
| `RowStreamShape` | Implemented in core runtime. |
| `RowStreamBatch` | Implemented as `PipelineDataBatch`. |
| `TargetWriteTask` | Modeled and backed by `IPipelineTargetWriteOperation`. |
| `InsertRowsTargetWriteTask` | Modeled concrete target write operation. SQL Server currently realizes insert via bulk copy. |
| Run evidence | Stage 1 returns in-memory execution results and can optionally record operational DB evidence. |

The initial instance CLI supports:

```text
meta-pipeline new-workspace <path>
meta-pipeline add-pipeline --workspace <path> --name <name> [--description <text>]
meta-pipeline add-step --workspace <path> --pipeline <name> --script <name-or-id> --transform-workspace <path> --binding-workspace <path> --execution-connection-env <name> [--step-name <name>] [--binding <id>] [--target-connection-env <name>] [--target <sql-identifier>] [--target-write <insert-rows>] [--batch-size <n>] [--timeout-seconds <n>]
meta-pipeline add-executable-step --workspace <path> --pipeline <name> --executable <path> [--step-name <name>] [--arguments <text>] [--working-directory <path>] [--success-exit-code <n>] [--timeout-seconds <n>]
meta-pipeline inspect --workspace <path>
meta-pipeline create-pipeline-db --pipeline-db-connection-env <name> [--pipeline-db-name <name>]
meta-pipeline prune-pipeline-db --pipeline-db-connection-env <name> --retention-days <days> [--dry-run]
meta-pipeline execute --workspace <path> --pipeline <name> [--transform-workspace <path>] [--binding-workspace <path>] [--pipeline-db-connection-env <name>]
```

`execute` resolves the pipeline's serial `PipelineTask` chain, resolves modeled connection references through environment variables, and executes each transform-backed task in order.
Executable tasks do not use transform or binding workspaces; runtime success is determined by their modeled success exit code.
For SELECT-kind scripts, the transform task uses its execution connection to read and must be followed immediately by one `InsertRowsTargetWriteTask` with a target connection.
For INSERT/UPDATE/DELETE/TRUNCATE/MERGE scripts, the transform task executes directly through the execution connection and must not feed a target-write task.
The CLI takes connection environment variable names only; it derives the modeled connection reference name from the env name rather than exposing a separate `--...-ref` argument.
`TimeoutSeconds` is optional; omitted means no SQL command timeout. Timeout evidence is recorded on `TaskRun`, and SHA-256 workspace fingerprints are recorded in `RunFingerprint`.
The current failure policy is `StopOnFirstFailure`, and pipeline-wide transaction task primitives are intentionally unsupported.
`prune-pipeline-db` prunes only old `RunDiagnosticsLog` rows; `PipelineRun`, `TaskRun`, `RunLog`, metrics, fingerprints, failures, and audit ids remain available for audit lineage.

## Anti-Patterns

Do not encode broad ETL concerns as one giant `PipelineOptions` blob.

Do not introduce ad-hoc JSON or blob artifacts as product truth.

Do not hide destructive target behavior inside a friendly operation name.

Do not infer lineage or dependencies from SQL text as product truth.

Do not store connection strings in pipeline metadata.

Do not make orchestration intelligence part of the plain executor.

Do not model every [`../meta-load/META-LOAD.md`](../meta-load/META-LOAD.md) concern as a first-class class before there is a concrete task that needs it.
