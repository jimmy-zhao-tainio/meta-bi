# AdventureWorks clean BI stack run plan

## Policy

This is a clean accepted replay candidate. Failed diagnostic attempts are disposable. If a product, model, environment, or worker issue blocks the run, halt, fix outside this folder, and start a new clean run.

## Stack Path

```text
AdventureWorks2022 -> AdventureWorksRawVault -> AdventureWorksBusinessVault -> AdventureWorksMetaDemo -> AdventureWorksMetaDemoTabular
```

RDV and BDV are required persisted layers. The DW/mart is populated from BDV-backed transforms. Tabular is the final analytics proof and must not replace the modeled table-load orchestration proof.

## Stage Scripts

- `stages\00-source-readiness.cmd`
- `stages\01-extract-source-schema.cmd`
- `stages\02-rdv-model-sql-deploy.cmd`
- `stages\03-bdv-model-sql-deploy.cmd`
- `stages\04-load-product-vault-slice.cmd`
- `stages\05-load-sales-vault-slice.cmd`
- `stages\06-build-bdv-mart-dq-orchestration.cmd`
- `stages\07-author-process-tabular.cmd`

## Target Databases

- Source: `AdventureWorks2022`
- RDV: `AdventureWorksRawVault`
- BDV: `AdventureWorksBusinessVault`
- Mart: `AdventureWorksMetaDemo`
- Tabular: `AdventureWorksMetaDemoTabular` on `%AW_TABULAR_SERVER%`

## Gate Evidence

Source gate:

- Live SQL Server source is reachable through `AW_SOURCE_SQL`.
- Source schema workspace exists at `source\AdventureWorks2022\Schema`.
- Extracted source counts are recorded in the stage log.

RDV gate:

- Raw vault workspace exists at `rdv\AdventureWorksRawVault\RawVault`.
- RDV SQL workspace exists at `rdv\AdventureWorksRawVault\Sql`.
- Product and Sales RDV target tables are deployed.
- Product and Sales RDV transform scripts are authored/imported before strict binding.
- RDV load row-count evidence is recorded.

BDV gate:

- Business vault workspace exists at `bdv\AdventureWorksBusinessVault\BusinessVault`.
- BDV SQL workspace exists at `bdv\AdventureWorksBusinessVault\Sql`.
- Product and Sales BDV target tables are deployed.
- Product and Sales BDV transform scripts are authored/imported before strict binding.
- BDV load row-count evidence is recorded.

DW/mart and DQ gate:

- Mart tables are created in `AdventureWorksMetaDemo`.
- Mart transforms read from `AdventureWorksBusinessVault`, not directly from `AdventureWorks2022`.
- Mart strict binding succeeds against BDV source schema and mart target schema.
- DQ candidates are generated from modeled transform structure and binding evidence, promoted, converted to SQL, and deployed.
- `dq.v_DataQualityReview` reports the review result.

Pipeline/orchestration gate:

- `ops\Pipeline` contains one transform-backed pipeline per table-producing transform.
- Expected transform-backed task counts: RDV Product 8, RDV Sales 21, BDV Product 2, BDV Sales 20, DW/Mart 5, total 56.
- `TransformExecutionTask.xml` exists and no executable wrapper is used as the primary ETL proof.
- `ops\Orchestration` is inferred from modeled pipeline/binding access profiles.
- `inspect-run-plan` shows 56 planned table-load tasks and model-derived dependencies.
- `meta-orchestration execute` completes the 56-task load run.

Tabular gate:

- `analytics\Analytics` is authored as portable analytical intent over the mart.
- `analytics\Tabular` is generated from `MetaAnalytics` and patched with import partitions that read the mart tables.
- `meta-tabular deploy --no-process` and `meta-tabular process` succeed against `%AW_TABULAR_SERVER%` / `%AW_TABULAR_DATABASE%`.
- `analytics\TabularProof` runs a DAX proof for fact row count, sales amount, and order quantity.

## Boundary

The accepted claim is the modeled BI path through RDV, BDV, DW/mart, automatic DQ, transform-backed orchestration, and Tabular. If Tabular is unavailable locally, the run is partial at the analytics target gate with an environment blocker; the ETL/orchestration result remains valid but not a complete recording run.
