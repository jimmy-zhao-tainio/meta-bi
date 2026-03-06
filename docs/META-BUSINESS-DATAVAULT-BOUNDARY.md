# MetaBusinessDataVault boundary

## Purpose

`MetaBusinessDataVault` is the sanctioned Business Data Vault model in the BI stack.

It exists after raw integration and before downstream warehouse and analysis design.

At this layer the sanctioned model should be specific enough to describe real Business Vault structures, not just name them and rely on generator-side conventions.

## Design inspiration and sources

This boundary is informed by Data Vault material used as source material, not as a dependency to import blindly.

Verified references used in this draft:

- Microsoft Fabric Data Vault article (business-vault and same-as discussion): <https://techcommunity.microsoft.com/blog/analyticsonazure/implementing-data-vault-2-0-on-fabric-data-warehouse/4227078>
- Redgate, The Business Data Vault: <https://www.red-gate.com/blog/data-vault-series-the-business-data-vault/>
- Redgate, Data Vault 2.0 Modeling Basics: <https://www.red-gate.com/blog/data-vault-series-data-vault-2-0-modeling-basics>
- Varigence reference values for Data Vault structures: <https://docs.varigence.com/bimlflex/reference-documentation/metadata-static-values.html>
- dv2.org 2023 notes on effectivity / multi-active / record tracking satellites: <https://www.dv2.org/2023/>

## Modeling stance

This model is not in architecture space.

It is a specific implementation-facing sanctioned model. If generated DDL or other emitted artifacts need the metadata, the sanctioned model should carry that metadata explicitly.

That means the model represents, where applicable:

- hash key column names
- reference column names
- business key column names
- load timestamp column names
- record source column names
- hashdiff column names
- PIT reference column names
- bridge reference/path/depth columns when those helper structures require them

## Business Vault structure families

### BusinessHub

A business hub carries the Business Vault hub structure itself, including the mandatory technical columns the emitted table needs.

The sanctioned model therefore carries:

- `Name`
- `HubKind`
- `HubKeyColumnName`
- `LoadTimestampColumnName`
- `RecordSourceColumnName`

Business key composition is carried explicitly through `BusinessHubKeyPart` rows.

### BusinessHubKeyPart

A hub key part represents one component of the business key carried by the hub.

The sanctioned model carries:

- `Name`
- `BusinessKeyColumnName`
- `Ordinal`

### BusinessLink

A business link carries the Business Vault link structure and its mandatory technical columns.

The sanctioned model carries:

- `Name`
- `LinkKind`
- `LinkKeyColumnName`
- `LoadTimestampColumnName`
- `RecordSourceColumnName`

### BusinessLinkEnd

A business link end represents one participating business hub in a business link.

The sanctioned model carries:

- `Ordinal`
- `RoleName` when the end is role-qualified
- `LinkReferenceColumnName`
- optional `IsDrivingKey` when downstream effectivity semantics need that distinction

### BusinessHubSatellite

A business hub satellite represents descriptive or rule-driven history attached to a business hub.

The sanctioned model carries:

- `Name`
- `SatelliteKind`
- `ParentHubKeyColumnName`
- `LoadTimestampColumnName`
- `RecordSourceColumnName`
- `HashDiffColumnName` when the satellite kind uses a hashdiff

Payload structure is represented explicitly through `BusinessHubSatelliteAttribute` rows.

### BusinessLinkSatellite

A business link satellite represents descriptive or rule-driven history attached to a business link.

The sanctioned model carries:

- `Name`
- `SatelliteKind`
- `ParentLinkKeyColumnName`
- `LoadTimestampColumnName`
- `RecordSourceColumnName`
- `HashDiffColumnName` when the satellite kind uses a hashdiff

Payload structure is represented explicitly through `BusinessLinkSatelliteAttribute` rows.

### Point-in-time structures

PIT tables are helper structures in Business Vault and therefore need first-class representation.

The sanctioned model carries:

- `BusinessPointInTime`
  - `Name`
  - `HubKeyColumnName`
  - `AsOfTimestampColumnName`
- `BusinessPointInTimeHubSatellite`
  - `Ordinal`
  - `ReferenceColumnName`
- `BusinessPointInTimeLinkSatellite`
  - `Ordinal`
  - `ReferenceColumnName`

### Bridge structures

Bridge tables are helper structures for traversal and performance and need first-class representation.

The sanctioned model carries:

- `BusinessBridge`
  - `Name`
  - `BridgeKind`
  - `RootHubKeyColumnName`
  - optional `AsOfTimestampColumnName`
  - optional `DepthColumnName`
  - optional `PathColumnName`
- `BusinessBridgeHub`
  - `Ordinal`
  - `RoleName`
  - `BridgeReferenceColumnName`
- `BusinessBridgeLink`
  - `Ordinal`
  - `RoleName`
  - `BridgeReferenceColumnName`

## Link and satellite variants

The checked sources support link-level and satellite-level variants beyond the plain raw pattern.

The sanctioned model therefore keeps explicit kind properties where the physical structure family is the same but the subtype matters:

- `BusinessLink.LinkKind`
- `BusinessHubSatellite.SatelliteKind`
- `BusinessLinkSatellite.SatelliteKind`
- `BusinessBridge.BridgeKind`
- `BusinessHub.HubKind`

This keeps the model specific without exploding every subtype into a duplicate top-level entity when the structural family is still the same.

## What MetaBusinessDataVault should not own

It should not own:

- source-system extraction detail
- raw-vault lineage semantics that belong in `MetaRawDataVault`
- downstream dimensional or semantic-model artifacts
- measure definitions
- runtime execution details

Those belong in raw, transform, warehouse, analysis, or operations layers.

## Relationship to other sanctioned models

### MetaRawDataVault -> MetaBusinessDataVault

Raw Vault provides the integrated historical substrate.

Business Vault introduces business logic, mastering, helper structures, and business-driven reshaping on top of that substrate.

### MetaBusiness -> MetaBusinessDataVault

`MetaBusiness` should anchor why Business Vault structures exist.

Business Vault should not be built only because raw source structure exists. It should be built because business processes, org scope, and business intent justify it.

### MetaBusinessDataVault -> downstream analytical models

Business Vault should later provide the stronger basis for:

- suggested analytical groupings
- warehouse-serving structures
- analysis/semantic model design

## Current sanctioned direction

- keep `meta-datavault` as the tool family
- keep `MetaRawDataVault` as the raw sanctioned model
- keep `MetaBusinessDataVault` as the business sanctioned model beside it

That keeps the tool family unified while making the model intent explicit.
