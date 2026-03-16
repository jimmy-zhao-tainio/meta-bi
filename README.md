# meta-bi

`meta-bi` is the BI stack that sits on top of the generic `meta` foundation.

This repository currently contains BI-oriented sanctioned models, CLIs, and docs:

- `MetaSchema.*`
- `MetaDataType.*`
- `MetaDataTypeConversion.*`
- `MetaDataVault.*`
- `SqlModel.Workspaces`
- `MetaSql.Core`

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

## MetaSql Status

The original `MetaSql` attempt has been archived under:

```text
Archive/MetaSql.Legacy
```

It was a DDL-centered deployment experiment and is not the active direction anymore.

The reboot now starts from a sanctioned canonical `SqlModel` instead:

- `SqlModel.Workspaces`
- `MetaSql.Core`

Current canonical object families include:

- databases and schemas
- tables and columns
- primary keys and primary key columns
- foreign keys and foreign key columns
- indexes and index columns

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

Both CLIs bootstrap empty sanctioned workspaces directly with `--new-workspace <path>`.

Examples:

```cmd
meta-datavault-raw --new-workspace C:\path\to\NewRawDataVault
meta-datavault-business --new-workspace C:\path\to\NewBusinessDataVault
```

Business-only commands:

- `check-business-materialization`
- `materialize-business`
- `generate-metasql` (currently a stub)
- explicit `add-*` authoring commands for hubs, links, references, satellites, PITs, bridges, and their child rows

Example authoring flow:

```cmd
meta-datavault-business --new-workspace C:\path\to\NewBusinessDataVault
meta-datavault-business add-hub --workspace C:\path\to\NewBusinessDataVault --id Customer --name Customer
meta-datavault-business add-hub-key-part --workspace C:\path\to\NewBusinessDataVault --id CustomerIdentifier --hub Customer --name Identifier --data-type-id meta:type:String --ordinal 1
meta-datavault-business add-link --workspace C:\path\to\NewBusinessDataVault --id CustomerOrder --name CustomerOrder
meta-datavault-business add-link-hub --workspace C:\path\to\NewBusinessDataVault --id CustomerOrderCustomer --link CustomerOrder --hub Customer --ordinal 1 --role-name Customer
```

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

## Current Data Vault MetaSql Command Status

`generate-metasql` is currently a stub in both Data Vault CLIs:

- `meta-datavault-raw generate-metasql`
- `meta-datavault-business generate-metasql`

The old DataVault-to-Sql projection code has been removed. These commands stay in the CLI surface only as placeholders for future work and currently do not generate any `SqlModel` workspace.

Current active direction:

- author and materialize sanctioned Data Vault workspaces
- keep `SqlModel` as a sanctioned canonical SQL model in `MetaSql.Core`
- reboot schema deployment from model-native `SqlModel`, not from the removed DV projection path

See also:

- `docs/META-DATAVAULT-MATERIALIZATION-NOTE.md`
- `docs/BDV-BUSINESS-COLUMN-INTENT-NOTE.md`






