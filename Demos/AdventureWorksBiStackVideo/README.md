# AdventureWorks full BI stack

This demo builds and runs a complete modeled BI stack from the live `AdventureWorks2022` OLTP database:

```text
AdventureWorks2022
  -> Raw Data Vault
  -> Business Data Vault
  -> dimensional warehouse
  -> Data Quality
  -> MetaAnalytics
  -> MetaTabular
```

Every generated model, transform, binding, pipeline, orchestration plan, deployment manifest, and analytics workspace is produced by its sanctioned CLI. `AdventureWorksBiStackVideo.MetaMesh` records the complete workflow as named operations; generated results live under `Runs` and can be recreated.

Read [BUSINESS-REQUIREMENTS.md](BUSINESS-REQUIREMENTS.md) for the requested analytical outcome and [FULL-STACK-DESIGN.md](FULL-STACK-DESIGN.md) for the grains, layer ownership, workspace graph, and acceptance contract.

## Prerequisites

- the `meta` and `meta-bi` CLIs on `PATH`;
- a local SQL Server instance with `AdventureWorks2022` restored;
- permission to create the four demo-owned databases;
- Analysis Services Tabular only when running the optional Tabular deployment and processing operations.

`SOURCE-SETUP.md` covers the source database. `prepare-adventureworks-db.ps1` can download and restore the official backup on a local development machine.

## Environment

The examples below use the local default SQL Server instance:

```powershell
$env:AW_SOURCE_SQL = "Server=.;Database=AdventureWorks2022;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:AW_ADMIN_SQL = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:AW_RDV_SQL = "Server=.;Database=AdventureWorksRawVault;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:AW_BDV_SQL = "Server=.;Database=AdventureWorksBusinessVault;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:AW_DW_SQL = "Server=.;Database=AdventureWorksAnalytics;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:AW_PIPELINE_SQL = "Server=.;Database=AdventureWorksMetaPipeline;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
```

## Build the stack

Run from the mesh workspace:

```powershell
cd AdventureWorksBiStackVideo.MetaMesh

meta-mesh run --operation validate-source
meta-mesh run --operation sync-source-schema

meta-mesh run --operation create-raw-vault
meta-mesh run --operation deploy-raw-vault
meta-mesh run --operation create-source-raw-transforms
meta-mesh run --operation create-source-raw-pipeline

meta-mesh run --operation create-business-vault
meta-mesh run --operation deploy-business-vault
meta-mesh run --operation create-raw-business-transforms
meta-mesh run --operation create-raw-business-pipeline

meta-mesh run --operation create-dimensional-warehouse
meta-mesh run --operation deploy-dimensional-warehouse
meta-mesh run --operation create-business-warehouse-transforms
meta-mesh run --operation create-business-warehouse-pipeline

meta-mesh run --operation create-orchestration
meta-mesh run --operation deploy-pipeline-runtime
meta-mesh run --operation create-data-quality
meta-mesh run --operation deploy-data-quality
meta-mesh run --operation create-analytics
meta-mesh run --operation create-tabular
```

The three transform boundaries are strictly bound to modeled source and target schemas before their pipelines are authored. Orchestration is inferred from those pipeline tasks and binding effects.

## Run and verify

Execute the inferred graph across all three pipelines, then reconcile the result to the source:

```powershell
meta-mesh run --operation run-etl
meta-mesh run --operation verify-stack
```

For an inspected layer-by-layer run, use `load-raw-vault`, `load-business-vault`, and `load-dimensional-warehouse` instead of `run-etl`.

The verification operation checks Raw and Business Vault counts, warehouse fact grains, source-to-warehouse measures, deployed Data Quality results, and the latest task-level pipeline evidence. The expected static AdventureWorks result is 31,465 orders, 121,317 lines, 163 quota rows, and 176 promoted Data Quality checks with no findings.

## Tabular runtime

The MetaAnalytics and MetaTabular workspaces are complete without Analysis Services. The runtime operations target the local `.\TABULAR` instance and deploy the model as `AdventureWorksSales`:

```powershell
meta-mesh run --operation deploy-tabular
meta-mesh run --operation process-tabular
```

`deploy-tabular` replaces the target database and deploys metadata without processing. `process-tabular` performs the full refresh. `drop-tabular` removes the deployed database.

The deployment operation also grants the local `NT Service\MSOLAP$TABULAR` service account read access to `AdventureWorksAnalytics`. The verified deployment contains 11 tables, 72 columns, 11 measures, and 19 relationships and completes a full refresh on `.\TABULAR`. A direct model query returns Sales Amount `109846381.425`, Total Due `123216786.1159`, and Quota Amount `95714000`, matching the relational verification.

## Cleanup

```powershell
meta-mesh run --operation cleanup
```

Cleanup drops `AdventureWorksRawVault`, `AdventureWorksBusinessVault`, `AdventureWorksAnalytics`, and `AdventureWorksMetaPipeline`, then removes `Runs`. It does not remove the source database or an external Tabular database.
