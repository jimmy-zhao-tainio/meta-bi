# meta-bi

`meta-bi` is the BI stack that sits on top of the generic `meta` foundation.

This repository currently contains BI-oriented sanctioned models, CLIs, and docs:

- `MetaSchema.*`
- `MetaDataType.*`
- `MetaDataTypeConversion.*`
- `MetaDataVault.*`

It also contains BI architecture notes in `docs/`.

## Current dependency boundary

This repository now consumes the generic foundation through an internal NuGet package boundary instead of source-level project references.

Current direct foundation package dependency:

- `Meta.Core`

Additional foundation packages available from the same internal feed when BI projects need them:

- `Meta.Adapters`
- `MetaWeave.Core`
- `MetaFabric.Core`

Before restore/build, add a package source that points at the packed output from `meta`:

```cmd
dotnet nuget add source C:pathtometa.nupkg --name meta-foundation-internal
```

Then pack the foundation repo and build this repo:

```cmd
cd C:pathtometa
pack-internal.cmd

cd C:pathtometa-bi
dotnet build MetaSchema.sln
dotnet build MetaDataType.sln
dotnet build MetaDataTypeConversion.sln
dotnet build MetaDataVault.sln
```

This keeps BI work from silently editing foundation code and makes the boundary explicit.

Build the installer:

```cmd
dotnet build MetaBi.InstallerMetaBi.Installer.csproj
```

Then install the BI CLIs (`meta-schema`, `meta-data-type`, `meta-data-type-conversion`, `meta-datavault-raw`, `meta-datavault-business`) into `%LOCALAPPDATA%metabin` and add that directory to your user `PATH`:

```cmd
MetaBi.Installerbinpublishwin-x64install-meta-bi.exe
```

## Intent

The long-term repo boundary is:

- `meta`: generic foundation (`Meta.Core`, `meta`, `MetaWeave`, `meta-weave`, `MetaFabric`, `meta-fabric`, generic metamodels)
- `meta-bi`: sanctioned BI models and BI-specific CLIs/tooling

## Included solution files

- `MetaSchema.sln`
- `MetaDataType.sln`
- `MetaDataTypeConversion.sln`
- `MetaDataVault.sln`

## Current BI Weave and Fabric Example

Flat anchors:
- `WeavesWeave-MetaBusiness-MetaBusinessDataVault`
- `WeavesWeave-MetaBusiness-MetaBusinessDataVault-HubObject-Commerce`
- `WeavesWeave-MetaBusiness-MetaBusinessDataVault-LinkRelationship-Commerce`

Scoped seam:
- `WeavesWeave-MetaBusiness-MetaBusinessDataVault-LinkHubParticipant-Commerce`
- `FabricsFabric-Suggest-MetaBusiness-MetaBusinessDataVault-LinkHubParticipant-Commerce`
- `FabricsFabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkHubParticipant-Commerce`

The current proved path is:

- `BusinessHub.Name` -> `BusinessObject.Name` through flat weave
- `BusinessLink.Name` -> `BusinessRelationship.Name` through flat weave
- `BusinessLinkHub.RoleName` -> `BusinessRelationshipParticipant.RoleName` through fabric-scoped weave validation
- `BusinessHubKeyPart.Name` -> `BusinessKeyPart.Name` through path-scoped fabric validation

Current BI fabric samples therefore prove both:

- shared-parent scope:
  - `FabricsFabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkHubParticipant-Commerce`
- multi-hop path scope:
  - `FabricsFabric-Scoped-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce`
## Current Business Data Vault Materialization

The Data Vault tool family is now split into two CLIs:

- `meta-datavault-raw`
- `meta-datavault-business`

- `check-business-materialization`
- `materialize-business`

Preflight:

```cmd

meta-datavault-business check-business-materialization --business-workspace C:pathtoMetaBusiness.WorkspacesSampleBusinessCommerceRepeatedKeyPart --bdv-workspace C:pathtoMetaDataVault.WorkspacesSampleBusinessDataVaultCommerceRepeatedKeyPart --implementation-workspace C:pathtoMetaDataVault.WorkspacesMetaDataVaultImplementation --weave-workspace C:pathtoWeavesWeave-MetaBusiness-MetaBusinessDataVault-HubObject-Commerce-RepeatedKeyPart --weave-workspace C:pathtoWeavesWeave-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce --weave-workspace C:pathtoWeavesWeave-MetaBusiness-MetaBusinessDataVault-LinkRelationship-Commerce-RepeatedKeyPart --weave-workspace C:pathtoWeavesWeave-MetaBusiness-MetaBusinessDataVault-LinkHubParticipant-Commerce-RepeatedKeyPart --fabric-workspace C:pathtoFabricsFabric-Scoped-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce --fabric-workspace C:pathtoFabricsFabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkHubParticipant-Commerce-RepeatedKeyPart
```

```text
OK: business datavault materialization contract
Business Workspace: C:pathtoMetaBusiness.WorkspacesSampleBusinessCommerceRepeatedKeyPart
BusinessDataVault Workspace: C:pathtoMetaDataVault.WorkspacesSampleBusinessDataVaultCommerceRepeatedKeyPart
Implementation Workspace: C:pathtoMetaDataVault.WorkspacesMetaDataVaultImplementation
Weaves: 4
Fabrics: 2
FlatAnchors: 2/2
ScopedAnchors: 2/2
```

First-step materialization:

```cmd
meta-datavault-business materialize-business --business-workspace C:pathtoMetaBusiness.WorkspacesSampleBusinessCommerceRepeatedKeyPart --bdv-workspace C:pathtoMetaDataVault.WorkspacesSampleBusinessDataVaultCommerceRepeatedKeyPart --implementation-workspace C:pathtoMetaDataVault.WorkspacesMetaDataVaultImplementation --weave-workspace C:pathtoWeavesWeave-MetaBusiness-MetaBusinessDataVault-HubObject-Commerce-RepeatedKeyPart --weave-workspace C:pathtoWeavesWeave-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce --weave-workspace C:pathtoWeavesWeave-MetaBusiness-MetaBusinessDataVault-LinkRelationship-Commerce-RepeatedKeyPart --weave-workspace C:pathtoWeavesWeave-MetaBusiness-MetaBusinessDataVault-LinkHubParticipant-Commerce-RepeatedKeyPart --fabric-workspace C:pathtoFabricsFabric-Scoped-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce --fabric-workspace C:pathtoFabricsFabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkHubParticipant-Commerce-RepeatedKeyPart --new-workspace C:pathtoOutputMaterializedBusinessDataVault
```

```text
OK: business datavault materialized
Business Workspace: C:pathtoMetaBusiness.WorkspacesSampleBusinessCommerceRepeatedKeyPart
BusinessDataVault Workspace: C:pathtoMetaDataVault.WorkspacesSampleBusinessDataVaultCommerceRepeatedKeyPart
Implementation Workspace: C:pathtoMetaDataVault.WorkspacesMetaDataVaultImplementation
Path: C:pathtoOutputMaterializedBusinessDataVault
Model: MetaBusinessDataVault
MaterializedTables: 5
BusinessHubs: 3
BusinessLinks: 2
BusinessHubSatellites: 0
BusinessLinkSatellites: 0
BusinessPointInTimes: 0
BusinessBridges: 0
```

Current scope of `materialize-business`:

- validates the sanctioned Business/BDV/Implementation/Weave/Fabric input set first
- writes a new `MetaBusinessDataVault` workspace
- applies the sanctioned implementation `TableNamePattern`s to BDV table-bearing rows
- keeps semantic row `Id` values stable while physicalizing `Name`

## Current Business Data Vault SQL Generation

`meta-datavault-business` now has a first SQL pass:

```cmd
meta-datavault-business generate-sql --workspace C:\path\to\Output\MaterializedBusinessDataVault --implementation-workspace C:\path\to\MetaDataVault.Workspaces\MetaDataVaultImplementation --data-type-conversion-workspace C:\path\to\MetaDataTypeConversion.Workspaces\MetaDataTypeConversion --out C:\path\to\Output\Sql
```

```text
OK: business datavault sql generated
Workspace: C:\path\to\Output\MaterializedBusinessDataVault
Implementation Workspace: C:\path\to\MetaDataVault.Workspaces\MetaDataVaultImplementation
DataTypeConversion Workspace: C:\path\to\MetaDataTypeConversion.Workspaces\MetaDataTypeConversion
Path: C:\path\to\Output\Sql
Files: 11
Tables: 11
BusinessHubs: 3
BusinessLinks: 2
BusinessHubSatellites: 1
BusinessLinkSatellites: 1
BusinessReferences: 1
BusinessReferenceSatellites: 1
BusinessPointInTimes: 1
BusinessBridges: 1
```

Current SQL scope:

- `BusinessHub`
- `BusinessLink`
- `BusinessSameAsLink`
- `BusinessHierarchicalLink`
- `BusinessReference`
- `BusinessHubSatellite`
- `BusinessLinkSatellite`
- `BusinessSameAsLinkSatellite`
- `BusinessHierarchicalLinkSatellite`
- `BusinessReferenceSatellite`
- `BusinessPointInTime`
- `BusinessBridge`

Current SQL constraints and supported semantics:

- Standard links, same-as links, and hierarchical links are modeled as separate sanctioned entities and generate separate SQL structures
- `AuditId` is part of the sanctioned implementation baseline and is emitted on every generated DV table in the current SQL path
- `BusinessHubSatellite.SatelliteKind` and `BusinessLinkSatellite.SatelliteKind` currently support `standard` and `multi-active`; `multi-active` requires explicit satellite key-part rows, while `standard` must not declare them
- `BusinessPointInTime` currently supports only the baseline snapshot/reference contract; point-in-time references to `multi-active` satellites fail fast and explicit `BusinessPointInTimeStamp` rows are modeled but not yet emitted to SQL
- `BusinessPointInTime` must reference at least one hub or link satellite, ordinals must be unique across those references, hub satellites must belong to the point-in-time parent hub, and link satellites must connect to that hub
- bridge SQL generation requires an explicit ordered path from `AnchorHub` through `BusinessBridgeLink` and `BusinessBridgeHub` rows; inconsistent paths fail fast
- bridge SQL generation also requires explicit projection rows (`BusinessBridgeHubKeyPartProjection`, `BusinessBridgeHubSatelliteAttributeProjection`, `BusinessBridgeLinkSatelliteAttributeProjection`); projected columns are typed from the referenced BDV rows and their local `...DataTypeDetail`
- `MetaDataVaultImplementation` must provide the required technical column defaults for the currently supported SQL surface; missing required defaults fail fast instead of being silently omitted

Helper-structure example workspace:

- `MetaDataVault.Workspaces/SampleBusinessDataVaultCommerceHelpers`

See also:

- `docs/META-DATAVAULT-MATERIALIZATION-NOTE.md`
- `docs/BDV-BUSINESS-COLUMN-INTENT-NOTE.md`

## SQL Demo

For a checked-in Business Data Vault SQL demo with workspaces, instance data, generated SQL, and plain `cmd` scripts, see:

- `Samples/Demos/BusinessDataVaultSql`

