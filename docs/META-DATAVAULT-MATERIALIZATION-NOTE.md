# MetaDataVault materialization note

## Current split

`meta-datavault` has two distinct responsibilities:

1. native Data Vault workspace authoring and management
2. sanctioned materialization from other metadata workspaces

Those are both valid.

## Current implemented steps

The first implemented Business Vault materialization steps are:

- a sanctioned contract check
- a first-step BDV materializer that physicalizes table names into a new workspace

Contract check:

```cmd
meta-datavault check-business-materialization --business-workspace <path> --bdv-workspace <path> --implementation-workspace <path> --weave-workspace <path> [--weave-workspace <path> ...] --fabric-workspace <path> [--fabric-workspace <path> ...]
```

This command validates that the input set is coherent and complete enough for Business Data Vault materialization.

## Current sanctioned inputs

The current Business Data Vault materialization contract consumes:

- one `MetaBusiness` workspace
- one `MetaBusinessDataVault` workspace
- one `MetaDataVaultImplementation` workspace
- one or more `MetaWeave` workspaces
- one or more `MetaFabric` workspaces

## Why each input exists

### MetaBusiness

Owns business-side intent:

- `BusinessObject`
- `BusinessKey`
- `BusinessKeyPart`
- `BusinessRelationship`
- `BusinessRelationshipParticipant`

### MetaBusinessDataVault

Owns the sanctioned Business Data Vault structure family:

- hubs
- links
- satellites
- PIT
- bridge

### MetaDataVaultImplementation

Owns fixed implementation defaults that do not belong in the sanctioned BDV model itself:

- table name patterns
- fixed mandatory technical column names
- datatype defaults for those columns

### MetaWeave

Owns flat direct anchors such as:

- `BusinessHub.Name -> BusinessObject.Name`
- `BusinessLink.Name -> BusinessRelationship.Name`

### MetaFabric

Owns scoped child-row consistency over weave workspaces, such as:

- `BusinessLinkEnd.RoleName -> BusinessRelationshipParticipant.RoleName`
- `BusinessHubKeyPart.Name -> BusinessKeyPart.Name`

where the child binding is only deterministic under a scoped parent binding.

## Current required anchor set

Flat anchors:

- `BusinessHub.Name -> BusinessObject.Name`
- `BusinessLink.Name -> BusinessRelationship.Name`

Scoped anchors:

- `BusinessLinkEnd.RoleName -> BusinessRelationshipParticipant.RoleName`
- `BusinessHubKeyPart.Name -> BusinessKeyPart.Name`

The contract check currently requires all four.

## Current materialize-business step

```cmd
meta-datavault materialize-business --business-workspace <path> --bdv-workspace <path> --implementation-workspace <path> --weave-workspace <path> [--weave-workspace <path> ...] --fabric-workspace <path> [--fabric-workspace <path> ...] --new-workspace <path>
```

This command currently:

- reruns the sanctioned contract gate
- clones the sanctioned `MetaBusinessDataVault` intent workspace into a new workspace
- applies the implementation `TableNamePattern` values to table-bearing BDV rows
- keeps semantic row `Id` values stable while physicalizing `Name`

## What this does not do yet

The current materializer does not yet:

- derive new BDV rows beyond the sanctioned intent workspace
- embed implementation column defaults into the BDV workspace model
- generate SSDT artifacts

## Current SQL generation step

```cmd
meta-datavault generate-sql --workspace <materialized-bdv-workspace> --implementation-workspace <path> --data-type-conversion-workspace <path> --out <path>
```

This first SQL pass consumes:

- one materialized `MetaBusinessDataVault` workspace
- one `MetaDataVaultImplementation` workspace
- one `MetaDataTypeConversion` workspace

It currently emits plain SQL scripts for:

- `BusinessHub`
- `BusinessLink`
- `BusinessHubSatellite`
- `BusinessLinkSatellite`
- `BusinessPointInTime`
- `BusinessBridge`

## Why this is still useful

This gives downstream tooling a physicalized BDV workspace without hardcoding the table naming rules in generators.

If the sanctioned input set is incomplete or inconsistent, the failure still happens before materialized output is written.

## Current proved sample set

The current sanctioned repeated-key-part and helper-structure sample set is:

- `MetaBusiness.Workspaces/SampleBusinessCommerceRepeatedKeyPart`
- `MetaDataVault.Workspaces/SampleBusinessDataVaultCommerceRepeatedKeyPart`
- `MetaDataVault.Workspaces/SampleBusinessDataVaultCommerceHelpers`
- `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-HubObject-Commerce-RepeatedKeyPart`
- `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce`
- `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-LinkRelationship-Commerce-RepeatedKeyPart`
- `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce-RepeatedKeyPart`
- `Fabrics/Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce`
- `Fabrics/Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce-RepeatedKeyPart`

## Current typing position

Business-derived BDV columns are now typed directly on the BDV rows that cause those SQL columns to exist:

- `BusinessHubKeyPart`
- `BusinessHubSatelliteAttribute`
- `BusinessHubSatelliteKeyPart`
- `BusinessLinkSatelliteAttribute`
- `BusinessLinkSatelliteKeyPart`

Those rows own `DataTypeId`, and optional local `...DataTypeDetail` rows carry variable details such as length or precision.

That keeps:

- `MetaBusiness` focused on business meaning
- `MetaBusinessDataVault` responsible for the business-driven columns it materializes
- `MetaDataVaultImplementation` responsible for standardized technical DV columns

See also:

- `docs/BDV-BUSINESS-COLUMN-INTENT-NOTE.md`


