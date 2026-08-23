# MetaPipeline SQL Server CLI Integration

This demo creates one local SQL Server source/target database with tables for the current executable `MetaPipeline`/`MetaTransformScript` surface:

- `dbo.SourceCustomer`
- `dbo.TargetCustomer`
- `dbo.InsertSourceCustomer`
- `dbo.InsertTargetCustomer`
- `dbo.UpdateCustomer`
- `dbo.MergeSourceCustomer`
- `dbo.MergeCustomer`
- `dbo.DeleteCustomer`

It also creates a local SQL Server operational database named `MetaPipeline` if it does not already exist.
That operational database is not dropped or truncated by the demo cleanup, so repeated runs keep MetaPipeline execution history.

The pipeline imports six transform scripts, binds them against the extracted schema workspace, creates a `MetaPipeline` workspace, adds one pipeline row, appends six serial transform-backed tasks, initializes the operational DB with `meta-pipeline create-pipeline-db`, and runs `meta-pipeline execute` with operational DB recording enabled.

Target tables include platform columns `AuditId bigint` and `InsertDateTime2 datetime2(7)` with SQL Server defaults backed by `SESSION_CONTEXT(N'MetaPipeline.AuditId')` and `SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc')`. The transform binding ignores those target-only columns, so transform scripts stay focused on business columns while the operational MetaPipeline DB supplies the task audit context.

The modeled pipeline demonstrates:

- `TRUNCATE TABLE` as a mutation transform task.
- `INSERT ... SELECT` as a mutation transform task.
- `UPDATE` as a mutation transform task.
- `MERGE` as a mutation transform task.
- `DELETE` as a mutation transform task.
- `SELECT` materialized through adjacent `InsertRows`.

Mutation tasks execute directly through the execution connection. The final SELECT task writes the resulting row stream through `InsertRows`; the current SQL Server runtime realizes insert rows with bulk copy.
The demo leaves target tables and operational evidence in SQL Server for inspection instead of printing verification queries back to the console. Check the target tables for platform defaults and the `MetaPipeline` operational DB for task `AuditId`, task-level timeout setting, row-count evidence, and failure details.

The workflow is modeled in:

```text
MetaPipelineSqlServerCliIntegration.MetaMesh
```

Run commands from the mesh folder. `--workspace` is omitted because `meta-mesh`
defaults to the current directory:

```powershell
$env:META_PIPELINE_DEMO_EXECUTION_SQL = "Server=.;Database=MetaPipelineSqlServerCliIntegration;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_PIPELINE_DEMO_TARGET_SQL = "Server=.;Database=MetaPipelineSqlServerCliIntegration;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_PIPELINE_DEMO_PIPELINE_DB_ADMIN_SQL = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_PIPELINE_DEMO_OPERATIONAL_SQL = "Server=.;Database=MetaPipeline;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

cd MetaPipelineSqlServerCliIntegration.MetaMesh
meta-mesh show
meta-mesh run --operation cleanup
meta-mesh run --operation build-and-execute-pipeline
```

Operations:
- `cleanup`: drops the demo execution database and removes generated workspaces.
  It intentionally preserves the `MetaPipeline` operational database.
- `build-and-execute-pipeline`: creates source SQL objects, extracts schema,
  imports and binds transform scripts, authors the pipeline, initializes the
  operational DB, and executes the customer load.
