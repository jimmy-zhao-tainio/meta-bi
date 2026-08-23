# AdventureWorks full BI stack

## A complete BI stack built by an agent

The AdventureWorks stack was fully designed and built by an agent using the
public Meta and meta-bi command-line tools. Its starting inputs were a live
`AdventureWorks2022` database and a short
[business request](BUSINESS-REQUIREMENTS.md) for sales trends, channel and
product analysis, geography, salesperson quotas, and visible data-quality
risks.

The agent made the architectural decisions that turn those inputs into a BI
system. It selected sales order, sales line, and salesperson quota as separate
fact grains; kept tax and freight at order grain; created shared date, product,
geography, customer, channel, and salesperson dimensions; derived the
Online/Store channel from source evidence; and declined to publish gross margin
because AdventureWorks lacks complete historical cost coverage.

The implementation is preserved as models rather than summarized after the
fact. Every generated workspace was created through its owning CLI. The demo's
MetaMesh workspace names 27 participating workspaces, 29 operations, and 614
ordered executable steps covering discovery, model authoring, conversion,
deployment, transformation import and binding, pipeline construction,
execution, verification, and analytical deployment. The generated results are
checked in alongside that construction record.

## What the agent built

Each product owns a concrete part of the resulting stack:

| Engineering concern | AdventureWorks result | Modeled capability |
| --- | --- | --- |
| Discover the live source | 6 schemas, 71 tables, 20 views, 744 fields, and 90 relationships | `meta-schema` extracts a reviewable `MetaSchema` source contract. |
| Preserve source history | 71 Raw hubs, 90 links, 71 satellites, and 341 satellite attributes | `meta-datavault-raw` and `meta-convert` turn the source contract into `MetaRawDataVault`, then into 232 deployable `MetaSql` tables through an explicit implementation model. |
| Establish business meaning | 14 Business hubs, 19 links, 14 satellites, stable business names, and the Online/Store derivation | `meta-datavault-business` authors `MetaBusinessDataVault` concepts separately from the source-shaped Raw layer. |
| Fix analytical grain | 3 facts, 8 conformed dimensions, 10 warehouse measures, 40 dimension attributes, and 19 fact-to-dimension relationships | `meta-data-warehouse` authors `MetaDataWarehouse`; `meta-convert` projects it through an implementation model into an 11-table physical warehouse. |
| Make transformations provable | 53 source-to-Raw, 47 Raw-to-Business, and 20 Business-to-warehouse transformations, each strictly bound | `meta-transform-script` models the T-SQL program; `meta-transform-binding`, `MetaSchema`, `MetaDataType`, and `MetaDataTypeConversion` prove its reads, writes, target shape, and type compatibility. |
| Turn evidence into quality checks | 167 promoted checks covering orphan joins, multiplicity expansion, duplicate risk, and outer-join null expansion | `meta-data-quality` derives reviewable `MetaDataQuality` candidates and `meta-convert` emits executable quality SQL. |
| Operate the load | 3 pipelines containing 120 transformation tasks, plus one inferred orchestration plan over the same 120 tasks | `meta-pipeline` executes and records work; `meta-orchestration` derives and governs runtime dependencies. |
| Publish portable analytics | 11 analytical tables, 72 attributes, 11 base measures, 3 hierarchies, and 19 relationships | `meta-analytics` carries engine-neutral `MetaAnalytics`; `meta-convert` and `meta-tabular` produce and operate the Analysis Services model. |
| Model the conversion itself | One requirement and 24 executable Analytics-to-Tabular transformations | `meta-weave` executes a `MetaWeave` correspondence whose WeaveScript projection is readable and editable modeled transformation. |

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
cd AdventureWorksBiStackDemo.MetaMesh

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

The verification operation checks Raw and Business Vault counts, warehouse fact grains, source-to-warehouse measures, deployed Data Quality results, and the latest task-level pipeline evidence. The expected static AdventureWorks result is 31,465 orders, 121,317 lines, 163 quota rows, and 167 promoted Data Quality checks with no findings.

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

Cleanup drops `AdventureWorksRawVault`, `AdventureWorksBusinessVault`, `AdventureWorksAnalytics`, and `AdventureWorksMetaPipeline`, then removes transient pipeline execution evidence under `Runs\ops\RunArtifacts`. It keeps the checked-in modeled workspaces. It does not remove the source database or an external Tabular database.
