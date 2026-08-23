# MetaOrchestration CLI Integration Demo

This demo builds a SQL Server schema, imports transform scripts, binds them, creates `MetaPipeline` workspaces, and then asks `meta-orchestration` to create orchestration DAGs, build run-plan rows, and execute planned pipeline steps from the bound read/write profiles.

The demo workflow is modeled in:

```text
MetaOrchestrationCliIntegration.MetaMesh
```

Run commands from the mesh folder. `--workspace` is omitted because `meta-mesh` defaults to the current directory:

```powershell
cd MetaOrchestrationCliIntegration.MetaMesh
```

Set the SQL connection environment variables in the caller shell:

```powershell
$env:META_ORCHESTRATION_DEMO_SQL = "Server=.;Database=MetaOrchestrationCliIntegration;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_ORCHESTRATION_DEMO_ADMIN_SQL = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
```

Inspect the mesh:

```powershell
meta-mesh show
meta-mesh operations
meta-mesh run --operation validate-environment
```

Run a clean happy path:

```powershell
meta-mesh run --operation cleanup
meta-mesh run --operation prepare-shared-workspaces
meta-mesh run --operation create-complete-run-plan
meta-mesh run --operation inspect-complete
meta-mesh run --operation execute-complete
```

The broader regression tour is split into named mesh operations:

- `create-complete-run-plan` builds `CompletePipelineWS`, which contains six pipelines that can be placed automatically:
  - stage customer rows
  - stage order rows
  - load customer dimension rows from staged customers
  - load sales fact rows from staged orders and dimension customers
  - refresh an isolated work exchange-rate table with a truncate plus load pair
  - truncate a private scratch table that no other pipeline touches
  - `CompleteOrchestrationWS` then records dependency-ordered planned tasks and planned locks for that complete graph
- `create-policy-run-plan` builds `PolicyPipelineWS`, a complete dependency DAG with same-object mutations that require explicit write-order policy before automatic run planning:
  - one pipeline updates `dbo.SharedLanding`
  - one pipeline merges `dbo.SharedLanding`
  - the demo records an explicit order plus a scoped `Mutation`/`Mutation` lock policy, then writes run-plan rows into `PolicyOrchestrationWS`
- `create-invalid-evidence` builds `InvalidPipelineWS`, which contains a blocking dependency problem plus a non-blocking synchronization note:
  - one pipeline truncates `dbo.StageCustomer` while another pipeline reads it
  - two pipelines append to `dbo.SharedLanding`, which leaves the DAG valid but records a synchronization constraint
- `execute-failure-branch` builds `FailurePipelineWS`, which demonstrates conditional success/failure DAG dependencies:
  - the run plan contains a normal stage-customer load
  - the run plan also contains an unrelated two-step path
  - the failure handler is a normal planned pipeline task
  - the demo adds an explicit failure dependency edge from the broken stage task to the handler task before run planning
  - the source table is deliberately dropped before execution
  - `meta-orchestration execute` skips only unsatisfied success branches, continues the unrelated path, and runs the modeled `FailureHandler` branch through the DAG

Run the additional scenarios:

```powershell
meta-mesh run --operation create-policy-run-plan
meta-mesh run --operation create-invalid-evidence
meta-mesh run --operation execute-failure-branch
```

`create-invalid-evidence` and `execute-failure-branch` model expected diagnostic failures directly
on their operation steps. The mesh treats exit code `4` as successful for those steps, so the
operation remains honest without wrapper script branching.

The scenario creation operations assume `prepare-shared-workspaces` has created the shared `SchemaWS`, `TransformWS`, and `BindingWS` workspaces.

Requirements:

- SQL Server available as `Server=.`
- `meta-mesh`, `meta-sql`, `meta-schema`, `meta-transform-script`, `meta-transform-binding`, `meta-pipeline`, and `meta-orchestration` available on `PATH`
