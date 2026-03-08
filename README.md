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

The remaining unresolved seam is:

- `BusinessHubKeyPart` -> `BusinessKeyPart`

That case is multi-hop rather than shared-parent and is not yet covered by current `MetaFabric`.
