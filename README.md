# meta-bi

`meta-bi` is the BI stack that sits on top of the generic `meta` foundation.

This repository currently contains BI-oriented sanctioned models, CLIs, and docs:

- `MetaSchema.*`
- `MetaType.*`
- `MetaTypeConversion.*`
- `MetaDataVault.*`

It also contains BI architecture notes in `docs/`.

## Current dependency boundary

This repository now consumes the generic foundation through an internal NuGet package boundary instead of source-level project references.

Current direct foundation package dependency:

- `Meta.Core`

Additional foundation packages available from the same internal feed when BI projects need them:

- `Meta.Adapters`
- `MetaWeave.Core`

Before restore/build, add a package source that points at the packed output from `isomorphic-metadata`:

```cmd
dotnet nuget add source C:\path\to\isomorphic-metadata\.nupkg --name meta-foundation-internal
```

Then pack the foundation repo and build this repo:

```cmd
cd C:\path\to\isomorphic-metadata
pack-internal.cmd

cd C:\path\to\meta-bi
dotnet build MetaSchema.sln
dotnet build MetaType.sln
dotnet build MetaTypeConversion.sln
dotnet build MetaDataVault.sln
```

This keeps BI work from silently editing foundation code and makes the boundary explicit.

Install the BI CLIs (`meta-schema`, `meta-type`, `meta-type-conversion`, `meta-datavault`) into `%LOCALAPPDATA%\meta\bin` and add that directory to your user `PATH`:

```cmd
install-meta-bi.cmd
```

## Intent

The long-term repo boundary is:

- `isomorphic-metadata`: generic foundation (`Meta.Core`, `meta`, `MetaWeave`, `meta-weave`, generic metamodels)
- `meta-bi`: sanctioned BI models and BI-specific CLIs/tooling

## Included solution files

- `MetaSchema.sln`
- `MetaType.sln`
- `MetaTypeConversion.sln`
- `MetaDataVault.sln`
