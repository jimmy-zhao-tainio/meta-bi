# MetaRawDataVault boundary

## Purpose

`MetaRawDataVault` is the sanctioned raw Data Vault model in the BI stack.

It should be specific enough to describe a real raw vault design, not just hint at one and leave core structure hidden inside generator code.

At this layer the sanctioned model should make the required raw-vault table structure explicit:

- hubs
- links
- hub satellites
- link satellites
- source lineage structures needed to anchor those vault objects back to extracted source schema metadata

## Modeling stance

This model is not in architecture space.

It is closer to `MetaSchema`: specific, implementation-facing, and detailed enough that a generator should not need to guess the mandatory technical columns of the raw vault tables it emits.

The rule is:

- if generation requires the metadata, the model should carry it explicitly

That means the sanctioned model represents, for each raw vault table family, the technical columns that every generated table needs:

- hash key column names
- business key column names
- parent key reference column names
- load timestamp column names
- record source column names
- hashdiff column names when the satellite kind requires them

## Raw table families represented

### RawHub

A raw hub represents a business key entry point.

The sanctioned model therefore carries:

- `Name`
- `HubKeyColumnName`
- `LoadTimestampColumnName`
- `RecordSourceColumnName`

Business key composition is represented explicitly through `RawHubKeyPart` rows rather than inferred from naming conventions.

### RawHubKeyPart

A hub key part represents one component of the hub business key.

The sanctioned model carries:

- `BusinessKeyColumnName`
- `Ordinal`
- relationship to the source field that supplies that key part

### RawHubSatellite

A raw hub satellite represents descriptive history attached to a hub.

The sanctioned model carries:

- `Name`
- `SatelliteKind`
- `ParentHubKeyColumnName`
- `LoadTimestampColumnName`
- `RecordSourceColumnName`
- `HashDiffColumnName` when the satellite kind uses a hashdiff

Payload structure is represented explicitly through `RawHubSatelliteAttribute` rows.

### RawLink

A raw link represents an asserted relationship between hubs.

The sanctioned model carries:

- `Name`
- `LinkKind`
- `LinkKeyColumnName`
- `LoadTimestampColumnName`
- `RecordSourceColumnName`

### RawLinkEnd

A link end represents one participating hub in a raw link.

The sanctioned model carries:

- `Ordinal`
- `RoleName` when the end is role-qualified
- `LinkReferenceColumnName`
- optional `IsDrivingKey` when effectivity-style downstream semantics need that distinction

### RawLinkSatellite

A raw link satellite represents descriptive history attached to a link.

The sanctioned model carries:

- `Name`
- `SatelliteKind`
- `ParentLinkKeyColumnName`
- `LoadTimestampColumnName`
- `RecordSourceColumnName`
- `HashDiffColumnName` when the satellite kind uses a hashdiff

Payload structure is represented explicitly through `RawLinkSatelliteAttribute` rows.

## Source-side anchoring

The model still carries source schema entities:

- `SourceSystem`
- `SourceSchema`
- `SourceTable`
- `SourceField`
- `SourceTableRelationship`
- `SourceTableRelationshipField`

Those are not there to duplicate `MetaSchema` conceptually. They are there so a raw vault design can stay anchored to concrete extracted source structures and so generated lineage stays explicit.

## What MetaRawDataVault should not own

It should not own:

- business-process semantics
- business mastering rules
- PIT and bridge helper structures
- dimensional or semantic-serving artifacts
- runtime loading/orchestration details

Those belong in business-vault, warehouse, analysis, or operations layers.
