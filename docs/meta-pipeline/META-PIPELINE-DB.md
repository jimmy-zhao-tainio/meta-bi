# MetaPipeline Operational DB

## Purpose

The MetaPipeline operational DB is the runtime evidence store for pipeline execution.

It stores:

- pipeline run start/completion/status
- task run status
- row, batch, column, task, rows-affected, and duration metrics
- audit-relevant warning/error run logs
- high-volume informational diagnostic logs
- failure records for validation, configuration, connection, runtime, and unexpected failures

It does not store sanctioned model truth.
The modeled metadata surfaces remain XML workspaces, SQL workspaces, and C#
workspaces.
XML is one deterministic workspace surface, not a semantic authority above SQL or C#.

## Bootstrap

The CLI creates or updates the SQL Server operational database:

```text
meta-pipeline create-pipeline-db --pipeline-db-connection-env META_PIPELINE_SQLSERVER --pipeline-db-name MetaPipeline
```

`META_PIPELINE_SQLSERVER` must contain a SQL Server connection string with permission to create the database and schema.
`--pipeline-db-name` defaults to `MetaPipeline`.
Connection strings are read from environment variables only and are not written into model XML or the operational DB.

The bootstrap is idempotent and creates schema `MetaPipeline` with:

- `SchemaVersion`
- `PipelineRun`
- `TaskRun`
- `RunMetric`
- `RunLog`
- `RunDiagnosticsLog`
- `RunFailure`

Operational tables include date-oriented indexes for common operator queries:

- `PipelineRun.StartedAtUtc`
- `PipelineRun.CompletedAtUtc`
- `PipelineRun.Status, StartedAtUtc`
- `TaskRun.StartedAtUtc`
- `TaskRun.CompletedAtUtc`
- `RunLog.LoggedAtUtc`
- `RunDiagnosticsLog.LoggedAtUtc`
- `RunFailure.OccurredAtUtc`

Execution commands do not create or initialize the operational DB.
If the database is unavailable or the schema has not been bootstrapped, execution fails before pipeline work starts with a `Next:` helper telling the operator to initialize the operational DB.

## Execution Recording

Recording is opt-in per run:

```text
meta-pipeline execute --workspace .\PipelineWS --pipeline CustomerLoad --transform-workspace .\TransformWS --binding-workspace .\BindingWS --pipeline-db-connection-env META_PIPELINE_DB
```

```text
meta-pipeline execute-sqlserver --transform-workspace .\TransformWS --binding-workspace .\BindingWS --script dbo.v_customer_load --execution-connection-env EXECUTION_DB --target-connection-env TARGET_DB --pipeline-db-connection-env META_PIPELINE_DB
```

When enabled, the CLI starts a run record before modeled validation/runtime resolution so configuration and connection failures can be recorded.
Successful and failed runtime results are completed from `MetaPipelineExecutionResult` and its task results.
For SQL mutation tasks, row-count metrics use SQL Server rows affected where SQL Server reports a value.
Explicit `TimeoutSeconds` settings are stored on `TaskRun`; omitted timeout means no SQL command timeout.
Pipeline, TransformScript, and Binding workspace paths and selected model identities are recorded on `PipelineRun` and `TaskRun`. Supplying mutually consistent workspaces is an operational requirement; execution does not infer freshness from workspace content. Operational databases created by older versions may retain an unused `RunFingerprint` table and its historical rows. MetaPipeline does not drop or write that retired table.
The target operational database and `MetaPipeline` schema must already exist.

## Pruning

Operational evidence grows over time and must be maintained deliberately in production.
The CLI provides explicit retention maintenance:

```text
meta-pipeline prune-pipeline-db --pipeline-db-connection-env META_PIPELINE_DB --retention-days 30
```

Use `--dry-run` to inspect the prune scope without deleting rows.
Only `RunDiagnosticsLog` rows for completed runs with `CompletedAtUtc` older than the cutoff are eligible.
The lineage/audit tables are preserved:

- `PipelineRun`
- `TaskRun`
- `RunMetric`
- `RunLog`
- `RunFailure`

Audit ids are also preserved.
Running or incomplete runs are not touched by this command.
`meta-pipeline` does not install SQL Agent jobs; operators should schedule this command using their normal operations tooling.

## Boundary

The operational DB is not:

- scheduling
- orchestration
- dependency freshness
- checkpointing
- watermarks
- resumability
- metadata truth
- a replacement for XML workspaces

Those concerns must be modeled or owned explicitly elsewhere before they become sanctioned product behavior.
