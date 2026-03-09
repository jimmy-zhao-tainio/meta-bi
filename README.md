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
dotnet nuget add source C:\path\to\meta\.nupkg --name meta-foundation-internal
```

Then pack the foundation repo and build this repo:

```cmd
cd C:\path\to\meta
pack-internal.cmd

cd C:\path\to\meta-bi
dotnet build MetaSchema.sln
dotnet build MetaDataType.sln
dotnet build MetaDataTypeConversion.sln
dotnet build MetaDataVault.sln
```

This keeps BI work from silently editing foundation code and makes the boundary explicit.

Build the installer:

```cmd
dotnet build MetaBi.Installer\MetaBi.Installer.csproj
```

Then install the BI CLIs (`meta-schema`, `meta-data-type`, `meta-data-type-conversion`, `meta-datavault`) into `%LOCALAPPDATA%\meta\bin` and add that directory to your user `PATH`:

```cmd
MetaBi.Installer\bin\publish\win-x64\install-meta-bi.exe
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
- `Weaves\Weave-MetaBusiness-MetaBusinessDataVault`
- `Weaves\Weave-MetaBusiness-MetaBusinessDataVault-HubObject-Commerce`
- `Weaves\Weave-MetaBusiness-MetaBusinessDataVault-LinkRelationship-Commerce`

Scoped seam:
- `Weaves\Weave-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce`
- `Fabrics\Fabric-Suggest-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce`
- `Fabrics\Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce`

The current proved path is:

- `BusinessHub.Name` -> `BusinessObject.Name` through flat weave
- `BusinessLink.Name` -> `BusinessRelationship.Name` through flat weave
- `BusinessLinkEnd.RoleName` -> `BusinessRelationshipParticipant.RoleName` through fabric-scoped weave validation
- `BusinessHubKeyPart.Name` -> `BusinessKeyPart.Name` through path-scoped fabric validation

Current BI fabric samples therefore prove both:

- shared-parent scope:
  - `Fabrics\Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce`
- multi-hop path scope:
  - `Fabrics\Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce`
## Current Business Data Vault Materialization

`meta-datavault` now has both:

- `check-business-materialization`
- `materialize-business`

Preflight:

```cmd
meta-datavault check-business-materialization --business-workspace C:\path\to\MetaBusiness.Workspaces\SampleBusinessCommerceRepeatedKeyPart --bdv-workspace C:\path\to\MetaDataVault.Workspaces\SampleBusinessDataVaultCommerceRepeatedKeyPart --implementation-workspace C:\path\to\MetaDataVault.Workspaces\MetaDataVaultImplementation --weave-workspace C:\path\to\Weaves\Weave-MetaBusiness-MetaBusinessDataVault-HubObject-Commerce-RepeatedKeyPart --weave-workspace C:\path\to\Weaves\Weave-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce --weave-workspace C:\path\to\Weaves\Weave-MetaBusiness-MetaBusinessDataVault-LinkRelationship-Commerce-RepeatedKeyPart --weave-workspace C:\path\to\Weaves\Weave-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce-RepeatedKeyPart --fabric-workspace C:\path\to\Fabrics\Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce --fabric-workspace C:\path\to\Fabrics\Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce-RepeatedKeyPart
```

```text
OK: business datavault materialization contract
Business Workspace: C:\path\to\MetaBusiness.Workspaces\SampleBusinessCommerceRepeatedKeyPart
BusinessDataVault Workspace: C:\path\to\MetaDataVault.Workspaces\SampleBusinessDataVaultCommerceRepeatedKeyPart
Implementation Workspace: C:\path\to\MetaDataVault.Workspaces\MetaDataVaultImplementation
Weaves: 4
Fabrics: 2
FlatAnchors: 2/2
ScopedAnchors: 2/2
```

First-step materialization:

```cmd
meta-datavault materialize-business --business-workspace C:\path\to\MetaBusiness.Workspaces\SampleBusinessCommerceRepeatedKeyPart --bdv-workspace C:\path\to\MetaDataVault.Workspaces\SampleBusinessDataVaultCommerceRepeatedKeyPart --implementation-workspace C:\path\to\MetaDataVault.Workspaces\MetaDataVaultImplementation --weave-workspace C:\path\to\Weaves\Weave-MetaBusiness-MetaBusinessDataVault-HubObject-Commerce-RepeatedKeyPart --weave-workspace C:\path\to\Weaves\Weave-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce --weave-workspace C:\path\to\Weaves\Weave-MetaBusiness-MetaBusinessDataVault-LinkRelationship-Commerce-RepeatedKeyPart --weave-workspace C:\path\to\Weaves\Weave-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce-RepeatedKeyPart --fabric-workspace C:\path\to\Fabrics\Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce --fabric-workspace C:\path\to\Fabrics\Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce-RepeatedKeyPart --new-workspace C:\path\to\Output\MaterializedBusinessDataVault
```

```text
OK: business datavault materialized
Business Workspace: C:\path\to\MetaBusiness.Workspaces\SampleBusinessCommerceRepeatedKeyPart
BusinessDataVault Workspace: C:\path\to\MetaDataVault.Workspaces\SampleBusinessDataVaultCommerceRepeatedKeyPart
Implementation Workspace: C:\path\to\MetaDataVault.Workspaces\MetaDataVaultImplementation
Path: C:\path\to\Output\MaterializedBusinessDataVault
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

`meta-datavault` now has a first SQL pass:

```cmd
meta-datavault generate-sql --workspace C:\path\to\Output\MaterializedBusinessDataVault --implementation-workspace C:\path\to\MetaDataVault.Workspaces\MetaDataVaultImplementation --data-type-conversion-workspace C:\path\to\MetaDataTypeConversion.Workspaces\MetaDataTypeConversion --out C:\path\to\Output\Sql
```

```text
OK: business datavault sql generated
Workspace: C:\path\to\Output\MaterializedBusinessDataVault
Implementation Workspace: C:\path\to\MetaDataVault.Workspaces\MetaDataVaultImplementation
DataTypeConversion Workspace: C:\path\to\MetaDataTypeConversion.Workspaces\MetaDataTypeConversion
Path: C:\path\to\Output\Sql
Files: 9
Tables: 9
BusinessHubs: 3
BusinessLinks: 2
BusinessHubSatellites: 1
BusinessLinkSatellites: 1
BusinessPointInTimes: 1
BusinessBridges: 1
```

Current SQL scope:

- `BusinessHub`
- `BusinessLink`
- `BusinessHubSatellite`
- `BusinessLinkSatellite`
- `BusinessPointInTime`
- `BusinessBridge`

Current SQL constraints and supported semantics:

- `BusinessLink.LinkKind` must currently be `standard`
- `BusinessLinkEnd.IsDrivingKey` is not yet emitted and therefore causes SQL generation to fail fast if present
- `BusinessHubSatellite.SatelliteKind` and `BusinessLinkSatellite.SatelliteKind` currently support `standard` and `multi-active`; `multi-active` requires explicit satellite key-part rows, while `standard` must not declare them
- `BusinessBridge.BridgeKind` must currently be `standard`
- `BusinessPointInTime` currently supports only `standard` satellite references; point-in-time references to `multi-active` satellites fail fast
- bridge SQL generation requires an explicit ordered path from `AnchorHub` through `BusinessBridgeLink` and `BusinessBridgeHub` rows; inconsistent paths fail fast
- `MetaDataVaultImplementation` must provide the required technical column defaults for the currently supported SQL surface; missing required defaults fail fast instead of being silently omitted

Helper-structure example workspace:

- `MetaDataVault.Workspaces/SampleBusinessDataVaultCommerceHelpers`

See also:

- `docs/META-DATAVAULT-MATERIALIZATION-NOTE.md`
- `docs/BDV-BUSINESS-COLUMN-INTENT-NOTE.md`



## SQL Demo

For a checked-in Business Data Vault SQL demo with workspaces, instance data, generated SQL, and plain `cmd` scripts, see:

- `Samples/Demos/BusinessDataVaultSql`
