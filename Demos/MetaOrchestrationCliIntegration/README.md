# MetaOrchestration CLI Integration Demo

This demo builds a SQL Server schema, imports transform scripts, binds them, creates `MetaPipeline` workspaces, and then asks `meta-orchestration` to infer DAGs, build run-plan rows, and execute planned pipeline steps from the bound read/write profiles.

`run.cmd` is the focused happy-path demo:

- prepare the shared SQL/schema/transform/binding workspaces
- create `CompletePipelineWS`
- infer `CompleteOrchestrationWS`
- refresh and inspect the run plan
- execute the complete six-pipeline DAG

The broader regression tour is split into named scenario scripts:

- `run-complete.cmd` builds `CompletePipelineWS`, which contains six pipelines that can be placed automatically:
  - stage customer rows
  - stage order rows
  - load customer dimension rows from staged customers
  - load sales fact rows from staged orders and dimension customers
  - refresh an isolated work exchange-rate table with a truncate plus load pair
  - truncate a private scratch table that no other pipeline touches
  - `CompleteOrchestrationWS` then records dependency-ordered planned tasks and planned locks for that complete graph
- `run-policy.cmd` builds `PolicyPipelineWS`, a complete dependency DAG with same-object mutations that require explicit write-order policy before automatic run planning:
  - one pipeline updates `dbo.SharedLanding`
  - one pipeline merges `dbo.SharedLanding`
  - the demo records an explicit order plus a scoped `Mutation`/`Mutation` lock policy, then writes run-plan rows into `PolicyOrchestrationWS`
- `run-invalid.cmd` builds `InvalidPipelineWS`, which contains a blocking dependency problem plus a non-blocking synchronization note:
  - one pipeline truncates `dbo.StageCustomer` while another pipeline reads it
  - two pipelines append to `dbo.SharedLanding`, which leaves the DAG valid but records a synchronization constraint
- `run-failure.cmd` builds `FailurePipelineWS`, which demonstrates conditional success/failure DAG dependencies:
  - the run plan contains a normal stage-customer load
  - the run plan also contains an unrelated two-step path
  - the failure handler is a normal planned pipeline task
  - the demo adds an explicit failure dependency edge from the broken stage task to the handler task before run planning
  - the source table is deliberately dropped before execution
  - `meta-orchestration execute` skips only unsatisfied success branches, continues the unrelated path, and runs the modeled `FailureHandler` branch through the DAG

Run the focused execution demo:

```cmd
run.cmd
```

Run the same flow through the modeled MetaMesh operations:

```cmd
cd MetaOrchestrationCliIntegration.MetaMesh
meta-mesh show
meta-mesh run --operation prepare-shared-workspaces
meta-mesh run --operation create-complete-run-plan
meta-mesh run --operation inspect-complete
meta-mesh run --operation execute-complete
```

The MetaMesh workspace declares the demo workspaces and operation steps. `prepare-shared-workspaces`
and `create-complete-run-plan` are first-run authoring operations: run `cleanup.cmd` from the demo
folder first if generated workspace folders already exist.

Run every scenario:

```cmd
run-all.cmd
```

Advanced/manual flow:

```cmd
prepare.cmd
run-complete.cmd
run-policy.cmd
run-invalid.cmd
run-failure.cmd
```

The scenario scripts assume `prepare.cmd` has created the shared `SchemaWS`, `TransformWS`, and `BindingWS` workspaces.

Requirements:

- SQL Server available as `Server=.`
- `meta-sql`, `meta-schema`, `meta-transform-script`, `meta-transform-binding`, `meta-pipeline`, and `meta-orchestration` available on `PATH`
