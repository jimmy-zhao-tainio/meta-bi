# MetaRawDataVault boundary

## Purpose

`MetaRawDataVault` is the sanctioned raw Data Vault model in the BI stack.

It should describe raw-vault structure and semantics explicitly, while leaving fixed technical naming conventions to a separate sanctioned implementation workspace.

## Modeling stance

This model is not in architecture space.

It is specific and implementation-facing, but it should not carry house-fixed technical column names when those names are standard by table type and belong to implementation policy rather than vault structure.

That means `MetaRawDataVault` owns:

- source anchoring
- raw hubs
- raw links
- raw hub satellites
- raw link satellites
- business-key part structure
- payload attribute structure
- link-end structure and semantics

And it does **not** own fixed technical naming such as:

- hub hash-key column names
- link hash-key column names
- load timestamp column names
- record source column names
- hashdiff column names
- link reference column names

Those belong in `MetaDataVaultImplementation`.

## Raw table families represented

### RawHub

Represents a raw hub and its source-table anchor.

### RawHubKeyPart

Represents one component of the hub business key.

The sanctioned model carries:

- `Name`
- `Ordinal`
- relationship to the source field that supplies that key part

### RawHubSatellite

Represents a raw hub satellite.

The sanctioned model carries:

- `Name`
- `SatelliteKind`
- relationship to the parent raw hub
- relationship to the source table that supplies the payload

Payload structure is carried through `RawHubSatelliteAttribute` rows.

### RawLink

Represents a raw link.

The sanctioned model carries:

- `Name`
- `LinkKind`
- relationship to the source table relationship it comes from

### RawLinkEnd

Represents one participating hub in a raw link.

The sanctioned model carries:

- `Ordinal`
- `RoleName`
- optional `IsDrivingKey`

### RawLinkSatellite

Represents a raw link satellite.

The sanctioned model carries:

- `Name`
- `SatelliteKind`
- relationship to the parent raw link
- relationship to the source table that supplies the payload

Payload structure is carried through `RawLinkSatelliteAttribute` rows.

## Source-side anchoring

The model still carries source schema entities:

- `SourceSystem`
- `SourceSchema`
- `SourceTable`
- `SourceField`
- `SourceTableRelationship`
- `SourceTableRelationshipField`

These keep the raw vault design anchored to concrete extracted source structures.

## Business-key input

`MetaRawDataVault` no longer permits heuristic business-key selection during `MetaSchema` conversion.

Business-key intent must come from a separate sanctioned `MetaBusinessKey` workspace and be bound to `MetaSchema` explicitly.

## What MetaRawDataVault should not own

It should not own:

- business-process semantics
- business-vault helper structures like PIT and Bridge
- fixed implementation naming policy
- downstream dimensional or semantic artifacts
- runtime execution details
