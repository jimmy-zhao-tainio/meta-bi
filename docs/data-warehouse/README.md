# MetaDataWarehouse

## Purpose

This folder records the strict dimensional-modeling stance for the `MetaDataWarehouse` sanctioned model.

`MetaDataWarehouse` should model declared dimensional design truth. It should not pretend to prove every loaded row is semantically honest. Runtime truth is checked through bindings, transform execution, and data-quality rules.

## Layer Boundary

- `MetaDataWarehouse` owns declared dimensional intent: table type, grain, keys, dimensions, measures, historization, and sanctioned constraints.
- Logical attribute and measure `DataTypeId` values name Meta-system semantic data types, not SQL Server types.
- `MetaDataWarehouseImplementation` owns SQL realization policy: schemas, table/column/key naming, surrogate keys, SCD technical columns, platform columns, defaults, indexes, and SQL type realization policy.
- `MetaTransformScript` owns SQL movement and remains flexible enough to do strange things.
- `MetaTransformBinding` owns output/target shape claims and can validate that a transform maps to declared warehouse columns.
- `MetaDataQuality` owns row-level proofs such as uniqueness, null checks, referential coverage, and grain violations in actual data.
- `MetaPipeline` executes bound scripts and records evidence. It does not enforce Kimball semantics.

## Strict Stance

The sanctioned authored model should be Kimball-strict by default:

- one fact table has one declared grain
- fact measures belong to that grain
- dimension references are explicit
- snapshot/fact/dimension/bridge table types are not just labels; they carry constraints
- nonconformant or legacy warehouse tables can be imported/assessed, but should not be blessed as sanctioned dimensional design

## Authoring Boundary

The CLI should read as concept authoring: add a warehouse, add a dimension, add attributes, mark a dimension as slowly changing, add a fact, declare grain, add dimensional roles, and add measures.

Do not ask users to author logical rows for surrogate keys, effective-from/effective-to/current/hash columns, platform columns, or SQL datatype facets. Those are implementation policy and conversion concerns.

## Table-Type Notes

- [Base dimension table](table-types/dimension.md)
- [Slowly changing dimension behavior](table-types/slowly-changing-dimension.md)
- [Conformed dimension table](table-types/conformed-dimension.md)
- [Fact-to-dimension relationship roles](table-types/fact-dimension-relationship.md)
- [Degenerate dimension](table-types/degenerate-dimension.md)
- [Junk dimension table](table-types/junk-dimension.md)
- [Mini-dimension table](table-types/mini-dimension.md)
- [Outrigger dimension table](table-types/outrigger-dimension.md)
- [Bridge table](table-types/bridge-table.md)
- [Base fact table](table-types/fact-table.md)
- [Transaction fact table](table-types/transaction-fact.md)
- [Periodic snapshot fact table](table-types/periodic-snapshot-fact.md)
- [Accumulating snapshot fact table](table-types/accumulating-snapshot-fact.md)
- [Factless fact table](table-types/factless-fact.md)
- [Aggregate fact table](table-types/aggregate-fact.md)

## Current Artifacts

- Logical workspace: `MetaDataWarehouse/Workspaces/MetaDataWarehouse`
- Default implementation workspace: `MetaDataWarehouse/Workspaces/MetaDataWarehouseImplementation`
- Sample workspace: `MetaDataWarehouse/Workspaces/SampleDataWarehouseCommerce`
- Authoring CLI: `meta-data-warehouse`
- SQL projection: `meta-convert data-warehouse-to-sql`
