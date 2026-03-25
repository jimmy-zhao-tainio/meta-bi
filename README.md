# meta-bi

`meta-bi` is the BI stack that sits on top of the generic `meta` foundation.

This repository currently contains BI-oriented sanctioned models, CLIs, and docs:

- `MetaSchema.*`
- `MetaDataType.*`
- `MetaDataTypeConversion.*`
- `MetaDataVault.*`
- `MetaSql.Workspace`
- `MetaSql.Core`

It also contains BI architecture notes in `docs/`.

## Current dependency boundary

This repository now consumes the generic foundation through an internal NuGet package boundary instead of source-level project references.

Current direct foundation package dependency:

- `Meta.Core`

Additional foundation packages available from the same internal feed when BI projects need them:

- `Meta.Adapters`
- `MetaWeave.Core`

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
- `meta`: generic foundation (`Meta.Core`, `meta`, `MetaWeave`, `meta-weave`, generic metamodels)
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

The reboot now starts from a sanctioned canonical `MetaSql` model instead:

- `MetaSql.Workspace`
- `MetaSql.Core`

Current canonical object families include:

- databases and schemas
- tables and columns
- primary keys and primary key columns
- foreign keys and foreign key columns
- indexes and index columns

## Current Data Vault CLI Status

The Data Vault tool family is split into two CLIs:

- `meta-datavault-raw`
- `meta-datavault-business`

Both CLIs bootstrap empty sanctioned workspaces directly with `--new-workspace <path>`, and `meta-datavault-business` supports explicit `add-*` authoring commands for business vault structures.

Legacy weave/fabric-based materialization commands and related sample workspaces were removed from the active repo direction.

`generate-metasql` remains a CLI stub in both Data Vault CLIs:

- `meta-datavault-raw generate-metasql`
- `meta-datavault-business generate-metasql`

Active direction:

- author sanctioned Data Vault workspaces
- project to sanctioned `MetaSql`
- plan/deploy through the manifest-driven `MetaSql` pipeline






