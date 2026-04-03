# meta-bi

`meta-bi` is the BI stack that sits on top of the generic `meta` foundation.

This repository currently contains BI-oriented sanctioned models, CLIs, and docs:

- `MetaSchema.*`
- `MetaDataType.*`
- `MetaDataTypeConversion.*`
- `MetaTransform.Workspace`
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
dotnet build MetaBi\Installer\MetaBi.Installer.csproj
```

Then install the BI CLIs (`meta-schema`, `meta-data-type`, `meta-data-type-conversion`, `meta-convert`, `meta-datavault-raw`, `meta-datavault-business`) into `%LOCALAPPDATA%metabin` and add that directory to your user `PATH`:

```cmd
MetaBi\Installer\bin\publish\win-x64\install-meta-bi.exe
```

## Intent

The long-term repo boundary is:

- `meta`: generic foundation (`Meta.Core`, `meta`, `MetaWeave`, `meta-weave`, generic metamodels)
- `meta-bi`: sanctioned BI models and BI-specific CLIs/tooling

## Included solution files

- `MetaSchema.sln`
- `MetaDataType.sln`
- `MetaDataTypeConversion.sln`
- `MetaDataVault.sln`

## CLI Guide

`meta-bi` currently ships these operator-facing CLIs:

- `meta-schema`
- `meta-data-type`
- `meta-data-type-conversion`
- `meta-convert`
- `meta-datavault-raw`
- `meta-datavault-business`
- `meta-sql`

### meta-schema

Purpose:
- extract a sanctioned `MetaSchema` workspace from a live SQL Server schema

Current command surface:
- `meta-schema help`
- `meta-schema extract sqlserver --new-workspace <path> --connection <connectionString> --system <name> (--schema <name> | --all-schemas) (--table <name> | --all-tables)`

Example:

```cmd
meta-schema extract sqlserver --new-workspace .\MetaSchema.Workspace --connection "<connectionString>" --system MySystem --schema dbo --all-tables
```

### meta-data-type

Purpose:
- bootstrap sanctioned `MetaDataType` workspaces

Current command surface:
- `meta-data-type help`
- `meta-data-type init --new-workspace <path>`

Example:

```cmd
meta-data-type init --new-workspace .\MetaDataType.Workspace
```

### meta-data-type-conversion

Purpose:
- author and validate sanctioned type-conversion workspaces
- resolve one source data type through the sanctioned conversion graph

Current command surface:
- `meta-data-type-conversion help`
- `meta-data-type-conversion init --new-workspace <path>`
- `meta-data-type-conversion check --workspace <path>`
- `meta-data-type-conversion resolve --workspace <path> --source-data-type <id>`

Examples:

```cmd
meta-data-type-conversion check --workspace .\MetaDataTypeConversion.Workspace
meta-data-type-conversion resolve --workspace .\MetaDataTypeConversion.Workspace --source-data-type meta:type:String
```

### meta-convert

Purpose:
- perform cross-model conversions owned by conversion glue code

Current command surface:
- `meta-convert help`
- `meta-convert schema-to-raw-datavault --source-workspace <path> --new-workspace <path> [--ignore-field-name <name>]... [--ignore-field-suffix <suffix>]... [--include-views] [--verbose]`
- `meta-convert raw-datavault-to-sql [--workspace <path>] --implementation-workspace <path> --database-name <name> --out <path>`
- `meta-convert business-datavault-to-sql [--workspace <path>] --implementation-workspace <path> --database-name <name> --out <path>`

Projection note:
- `raw-datavault-to-sql` and `business-datavault-to-sql` take physical schema ownership from the sanctioned `MetaDataVaultImplementation` workspace and do not accept a schema override.

Example:

```cmd
meta-convert schema-to-raw-datavault --source-workspace .\MetaSchema.Workspace --new-workspace .\MetaRawDataVault.Workspace
meta-convert raw-datavault-to-sql --workspace .\MetaRawDataVault.Workspace --implementation-workspace .\MetaDataVault\Workspaces\MetaDataVaultImplementation --database-name MyVault --out .\out\CurrentMetaSql.Workspace
meta-convert business-datavault-to-sql --workspace .\MetaBusinessDataVault.Workspace --implementation-workspace .\MetaDataVault\Workspaces\MetaDataVaultImplementation --database-name MyBusinessVault --out .\out\CurrentMetaSql.Workspace
```

### meta-datavault-raw

Purpose:
- author sanctioned raw Data Vault workspaces

Current command surface:
- `meta-datavault-raw --new-workspace <path>`
- `meta-datavault-raw add-*`

Current `add-*` commands:
- `add-source-system`
- `add-source-schema`
- `add-source-table`
- `add-source-field`
- `add-source-field-data-type-detail`
- `add-source-table-relationship`
- `add-source-table-relationship-field`
- `add-hub`
- `add-hub-key-part`
- `add-hub-satellite`
- `add-hub-satellite-attribute`
- `add-link`
- `add-link-hub`
- `add-link-satellite`
- `add-link-satellite-attribute`

Examples:

```cmd
meta-datavault-raw --new-workspace .\MetaRawDataVault.Workspace
```

### meta-datavault-business

Purpose:
- author sanctioned business Data Vault workspaces

Current command surface:
- `meta-datavault-business --new-workspace <path>`
- `meta-datavault-business add-*`
- typed business authoring commands take optional datatype facets inline via `--length`, `--precision`, and `--scale`; the CLI persists the underlying detail rows for you

Representative `add-*` families:
- `add-bridge*`
- `add-hub*`
- `add-link*`
- `add-hierarchical-link*`
- `add-reference*`
- `add-same-as-link*`
- `add-point-in-time*`

Example:

```cmd
meta-datavault-business --new-workspace .\MetaBusinessDataVault.Workspace
```

### meta-sql

Purpose:
- plan and apply manifest-driven SQL Server deployment from sanctioned `MetaSql` workspaces

Current command surface:
- `meta-sql deploy-plan --source-workspace <path> --connection-string <value> --out <path> [--approve-drop-table <schema.table>] [--approve-drop-column <schema.table.column>] [--approve-truncate-column <schema.table.column>] [--approval-file <path>]`
- `meta-sql deploy --manifest-workspace <path> --source-workspace <path> --connection-string <value>`

Behavior summary:
- `deploy-plan` extracts live schema when the target database exists, otherwise treats live as truly empty and writes a deploy manifest against that empty live state
- `deploy-plan` and `deploy` always operate on the full source workspace and full live database; filtered subset deploy is not supported
- destructive actions require exact object-scoped approvals
- `deploy` executes only the manifest after source/live fingerprint validation
- when the manifest expects a missing target database, `deploy` creates it first and refuses if it already exists
- schema creation is explicit in the manifest (`AddSchema`), not inferred while rendering table DDL

Examples:

```cmd
meta-sql deploy-plan --source-workspace .\CurrentMetaSql.Workspace --connection-string "<connectionString>" --out .\out\deploy-manifest
meta-sql deploy --manifest-workspace .\out\deploy-manifest --source-workspace .\CurrentMetaSql.Workspace --connection-string "<connectionString>"
```


## Active Models

Current active BI model families include:

- `MetaSchema`
- `MetaDataType`
- `MetaDataTypeConversion`
- `MetaTransform`
- `MetaRawDataVault`
- `MetaBusinessDataVault`
- `MetaSql`

`MetaTransform` is currently introduced as a sanctioned workspace model only. CLI and generated tooling are not added yet.

## Current Projection Status

- `meta-convert raw-datavault-to-sql` is operational:
  it converts sanctioned raw DV to a current `MetaSql` workspace and does not query any live database.
- `meta-convert business-datavault-to-sql` is operational:
  it converts sanctioned business DV to a current `MetaSql` workspace, applies sanctioned business-type lowering, and does not query any live database.






