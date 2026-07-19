# AdventureWorks full BI stack

This document is the implementation contract for the AdventureWorks MetaMesh demo. The business request in `BUSINESS-REQUIREMENTS.md` is the authority. Generated workspaces under `Runs` are disposable results, not authored product truth.

## Analytical scope

The first complete release has three fact grains:

- **Sales order**: one row per `Sales.SalesOrderHeader`. This owns order count, subtotal, tax, freight, total due, status, channel, and order/due/ship dates.
- **Sales line**: one row per `Sales.SalesOrderDetail`. This owns product, quantity, unit price, discount amount, and line sales amount.
- **Salesperson quota**: one row per salesperson and quota period from `Sales.SalesPersonQuotaHistory`.

The facts share modeled dimensions for date, product hierarchy, geography and territory, customer/store channel, and salesperson. Tax and freight are not copied to line grain. Gross margin is not published in this release: the source has historical cost rows for only 293 of 504 products, so a complete historical margin measure cannot be stated without an explicit business policy for missing cost.

## Source scope

`sync-source-schema` extracts the live `AdventureWorks2022` database as the source contract. The Raw Data Vault model remains source-faithful. The first ETL slice loads the source structures needed by the analytical scope:

- `Sales.SalesOrderHeader`
- `Sales.SalesOrderDetail`
- `Sales.Customer`
- `Sales.Store`
- `Sales.SalesPerson`
- `Sales.SalesPersonQuotaHistory`
- `Sales.SalesTerritory`
- `Sales.SpecialOfferProduct`
- `Production.Product`
- `Production.ProductSubcategory`
- `Production.ProductCategory`
- `Person.Person`
- `Person.Address`
- `Person.StateProvince`
- `Person.CountryRegion`
- `HumanResources.Employee`

The source schema is extracted in full so schema drift remains visible even when a table is outside the first load slice.
`HumanResources.Employee` contributes its business key and relationships to Person and SalesPerson. Its descriptive satellite is deliberately not loaded because the analytical scope needs employee identity, not HR payload such as birth date, leave balances, or login details.

## Layer ownership

### Raw Data Vault

The Raw Data Vault is mechanically generated from the source contract. Source business keys become hubs, source foreign-key relationships become links, and descriptive source fields become satellites. Load SQL preserves source values, uses SHA-256 hashes stored as 32-byte binary values, and applies insert-only Data Vault behavior:

- hubs and links insert only unseen hash keys;
- satellites compare the latest hash difference and append only changed versions;
- hash input is length-delimited and null-aware;
- no transform uses MD5 or invents replacement values for source nulls.

### Business Data Vault

The Business Data Vault contains the business structures needed by the analytical scope. It uses stable business names such as SalesOrder, SalesOrderLine, SalesPersonQuota, Product, Customer, Store, SalesPerson, SalesTerritory, Address, StateProvince, and CountryRegion. It carries curated descriptive satellites and explicit links between those concepts. `SalesChannel` is derived as `Online` or `Store` from modeled source evidence.

### Dimensional warehouse

The warehouse owns presentation grain and additive behavior. It contains:

- Date, Product, Geography, Customer, Salesperson, SalesTerritory, and SalesChannel dimensions;
- SalesOrder, SalesLine, and SalespersonQuota facts;
- role-playing Order Date, Due Date, Ship Date, and Quota Period relationships;
- calendar attributes on Date; fiscal attributes remain outside this release because the OLTP source and business requirements do not declare a fiscal calendar;
- product category/subcategory/model attributes;
- country, state/province, city, postal code, and territory attributes;
- separate order-level and line-level measures so aggregation is unambiguous.

### Data Quality

MetaDataQuality derives candidates from the bound Business-to-Warehouse transforms. The generated checks provide modeled evidence for missing references, duplicate-looking fact grains, nullability mismatches, and other supported transform findings. Promotion is explicit and generated SQL is deployed separately from warehouse construction.

### Analytics

MetaAnalytics describes the business-facing model and converts to MetaTabular. The model exposes:

- sales amount, order subtotal, tax, freight, total due, discount amount, order quantity, order count, average unit price, average order value, and quota amount;
- calendar date browsing;
- product, geography, customer/store, salesperson, territory, and channel slicing;
- separate Sales Orders, Sales Lines, and Salesperson Quotas tables so measures retain their grain.

Tabular deployment and processing are separate operations because they require an Analysis Services environment. The authored analytics and Tabular workspaces remain complete when that external service is unavailable.

## Workspace graph

All generated workspaces live under `Runs`:

```text
Runs/
  source/AdventureWorks2022/Schema
  rdv/RawDataVault
  rdv/Sql
  rdv/DeployManifest
  rdv/Schema
  rdv/Transforms
  rdv/Binding
  bdv/BusinessDataVault
  bdv/Sql
  bdv/DeployManifest
  bdv/Schema
  bdv/Transforms
  bdv/Binding
  dw/Warehouse
  dw/Sql
  dw/DeployManifest
  dw/Schema
  dw/Transforms
  dw/Binding
  dq/DataQuality
  dq/DataQuality.sql
  analytics/Analytics
  analytics/Tabular
  ops/Pipelines
  ops/Orchestration
  ops/RunArtifacts
```

Authored transform SQL and verification queries live outside `Runs` so they are versioned inputs.

## Environment contract

- `AW_SOURCE_SQL`: `AdventureWorks2022` SQL Server connection.
- `AW_ADMIN_SQL`: SQL Server administrative connection, normally `master`; deployment operations use it to create the demo-owned databases.
- `AW_RDV_SQL`: `AdventureWorksRawVault` connection.
- `AW_BDV_SQL`: `AdventureWorksBusinessVault` connection.
- `AW_DW_SQL`: `AdventureWorksAnalytics` connection.
- `AW_PIPELINE_SQL`: connection to the `AdventureWorksMetaPipeline` operational database used for pipeline run evidence.
MetaMesh validates explicit `{env:...}` arguments before starting an operation. Connection references stored inside the MetaPipeline and MetaTabular workspaces are resolved later by those product CLIs. Set `AW_RDV_SQL`, `AW_BDV_SQL`, `AW_DW_SQL`, and `AW_PIPELINE_SQL` before ETL execution, and set `AW_DW_SQL` before Tabular deployment; MetaMesh does not yet discover those embedded references during validation.

## Operation contract

Build and deployment operations are separate from execution:

1. `validate-source`
2. `sync-source-schema`
3. `create-raw-vault`
4. `deploy-raw-vault`
5. `create-source-raw-transforms`
6. `create-source-raw-pipeline`
7. `create-business-vault`
8. `deploy-business-vault`
9. `create-raw-business-transforms`
10. `create-raw-business-pipeline`
11. `create-dimensional-warehouse`
12. `deploy-dimensional-warehouse`
13. `create-business-warehouse-transforms`
14. `create-business-warehouse-pipeline`
15. `create-orchestration`
16. `deploy-pipeline-runtime`
17. `create-data-quality`
18. `deploy-data-quality`
19. `create-analytics`
20. `create-tabular`

ETL can be run as one inferred graph with `run-etl`, or inspected layer by layer with `load-raw-vault`, `load-business-vault`, and `load-dimensional-warehouse`. Running both forms deliberately repeats the ETL and is useful for proving insert-only and merge idempotency; it is not required for a normal load.

Tabular runtime operations target the local `.\TABULAR` instance and the modeled `AdventureWorksSales` database:

1. `deploy-tabular` deploys metadata without processing and grants the local Tabular service account read access to the warehouse.
2. `process-tabular` processes the deployed database.
3. `drop-tabular` removes the deployed database.

`verify-stack` reconciles the Data Vault path, warehouse facts and measures, Data Quality results, and latest pipeline evidence to the source. `cleanup` drops only the four demo-owned SQL Server databases and removes `Runs`; it does not remove `AdventureWorks2022` or an optional external Tabular database.

Composite operations may invoke these named operations, but no step receives hidden ordering or product semantics from its position in a script. Transform binding and inferred orchestration provide the modeled dependency evidence.

## Acceptance

A clean run must prove:

- every generated workspace is produced by its sanctioned CLI;
- every transform imports into MetaTransformScript and strictly binds to modeled source and target schemas;
- each ETL transition has its own modeled pipeline;
- orchestration is inferred from pipeline and binding evidence;
- repeat execution does not duplicate hubs, links, or unchanged satellites;
- warehouse fact counts and core measures reconcile to AdventureWorks source queries;
- DQ SQL is generated from promoted modeled candidates;
- the analytics and Tabular workspaces expose the requested business questions;
- any unavailable external Tabular runtime is reported as an environment limitation, not represented as a successful deployment.
