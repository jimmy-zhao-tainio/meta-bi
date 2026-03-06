# MetaDataVaultImplementation boundary

## Purpose

`MetaDataVaultImplementation` is the sanctioned model for fixed implementation details of Data Vault structures.

It exists so `MetaRawDataVault` and `MetaBusinessDataVault` can stay structural and semantic, while fixed physical conventions live in one explicit sanctioned workspace.

## Modeling stance

This model is not generic pattern metadata.

It is specific implementation metadata, with one entity per Data Vault table type that the platform intends to emit.

That means the sanctioned implementation workspace should carry concrete properties such as:

- hash key column names and data types
- parent hash key column names and data types
- load timestamp column names and data types
- record source column names and data types
- hashdiff column names and data types
- PIT snapshot/reference column naming
- Bridge path/depth/effective-date column naming

## Table-type entities

The current sanctioned surface is:

- `RawHubImplementation`
- `RawLinkImplementation`
- `RawHubSatelliteImplementation`
- `RawLinkSatelliteImplementation`
- `BusinessHubImplementation`
- `BusinessLinkImplementation`
- `BusinessHubSatelliteImplementation`
- `BusinessLinkSatelliteImplementation`
- `BusinessPointInTimeImplementation`
- `BusinessBridgeImplementation`

Each entity represents one emitted table family and lists the fixed implementation details that tooling should apply for that family.

## Why this is explicit

A generic `TableType` + `Convention` model would push too much interpretation back into tooling.

For Data Vault, the sanctioned models are meant to be specific enough that a generator can read the implementation workspace directly and know:

- which mandatory technical columns exist
- what those columns are called
- what their physical data types are

## Relationship to other sanctioned models

- `MetaRawDataVault` and `MetaBusinessDataVault` define the structure and semantics of vault tables.
- `MetaDataVaultImplementation` defines the fixed physical column conventions for those table types.
- `MetaBusinessKey` defines explicit business-key intent and should be bound to `MetaSchema` before raw-vault materialization.

## What this model should not own

It should not own:

- source extraction structure
- business process semantics
- vault table membership or payload design
- cross-model bindings
- runtime execution concerns
