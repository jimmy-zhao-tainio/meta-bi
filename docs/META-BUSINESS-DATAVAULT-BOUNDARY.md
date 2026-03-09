# MetaBusinessDataVault boundary

## Purpose

`MetaBusinessDataVault` is the sanctioned Business Data Vault model in the BI stack.

It should describe Business Vault structures and semantics explicitly, while leaving fixed technical naming conventions to a separate sanctioned implementation workspace.

## Modeling stance

This model owns:

- business hubs
- business links
- business hub satellites
- business link satellites
- PIT structures
- Bridge structures
- the structural and semantic relations between them

It does **not** own house-fixed technical naming such as fixed hash-key, load timestamp, record source, hashdiff, PIT reference, or bridge path/depth column names. Those belong in `MetaDataVaultImplementation`.

## Business Vault structure families

### BusinessHub

Represents a business-vault hub structure.

The sanctioned model carries:

- `Name`
- optional `Description`

### BusinessHubKeyPart

Represents one component of the hub business key.

The sanctioned model carries:

- `Name`
- `DataTypeId`
- `Ordinal`

Additional usage-specific detail such as length or precision belongs in `BusinessHubKeyPartDataTypeDetail` rows.

### BusinessLink

Represents a business-vault link structure.

The sanctioned model carries:

- `Name`

### BusinessLinkHub

Represents one participating hub in a business link.

The sanctioned model carries:

- `Ordinal`
- `RoleName`

### BusinessHubSatellite

Represents a business-vault hub satellite.

The sanctioned model carries:

- `Name`
- `SatelliteKind`

Payload structure is represented through:

- `BusinessHubSatelliteAttribute`
- `BusinessHubSatelliteKeyPart`

Those child rows own their own `DataTypeId` values, with optional local detail rows for variable physical declarations.

The additional key-part rows matter for multi-active or similarly qualified satellite structures where payload rows are not uniquely identified by parent key alone.

### BusinessLinkSatellite

Represents a business-vault link satellite.

The sanctioned model carries:

- `Name`
- `SatelliteKind`

Payload structure is represented through:

- `BusinessLinkSatelliteAttribute`
- `BusinessLinkSatelliteKeyPart`

Those child rows own their own `DataTypeId` values, with optional local detail rows for variable physical declarations.

The additional key-part rows matter for multi-active or similarly qualified satellite structures where payload rows are not uniquely identified by parent key alone.

### BusinessPointInTime

Represents a PIT helper structure over a business hub.

Included satellites are represented explicitly through:

- `BusinessPointInTimeHubSatellite`
- `BusinessPointInTimeLinkSatellite`

Additional arbitrary PIT date or timestamp contexts can be represented through:

- `BusinessPointInTimeStamp`
- `BusinessPointInTimeStampDataTypeDetail`

### BusinessBridge

Represents a bridge helper structure.

The sanctioned model carries:

- `Name`
- required `AnchorHub`

Participating hubs and links are represented explicitly through:

- `BusinessBridgeHub`
- `BusinessBridgeLink`

Bridge output columns are represented explicitly through:

- `BusinessBridgeHubKeyPartProjection`
- `BusinessBridgeHubSatelliteAttributeProjection`
- `BusinessBridgeLinkSatelliteAttributeProjection`

## Variant handling

The sanctioned model keeps explicit kind properties only where the structure family is the same but the subtype still matters:

- `BusinessHubSatellite.SatelliteKind`
- `BusinessLinkSatellite.SatelliteKind`
- bridge semantics

## Relationship to other sanctioned models

### MetaRawDataVault -> MetaBusinessDataVault

Raw Vault provides the integrated historical substrate.

Business Vault adds business logic, mastering, helper structures, and business-driven reshaping on top of that substrate.

### MetaBusiness -> MetaBusinessDataVault

`MetaBusiness` should anchor why Business Vault structures exist.

The minimum direct anchors are:

- `BusinessHub` -> `BusinessObject`
- `BusinessHubKeyPart` -> `BusinessKeyPart`
- `BusinessLink` -> `BusinessRelationship`
- `BusinessLinkHub` -> `BusinessRelationshipParticipant`

See `docs/META-BUSINESS-BUSINESSDATAVAULT-WEAVE-NOTE.md` for the current contract and limitations.

### MetaDataVaultImplementation -> MetaBusinessDataVault

Implementation details such as fixed mandatory technical column names should come from `MetaDataVaultImplementation`, not from this model.

## What MetaBusinessDataVault should not own

It should not own:

- source-system extraction detail
- fixed implementation naming policy
- downstream dimensional or semantic artifacts
- measure definitions
- runtime execution details



