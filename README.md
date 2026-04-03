# meta-bi

`meta-bi` is the BI stack that sits on top of the generic `meta` foundation.

This repository currently contains BI-oriented sanctioned models, CLIs, and docs:

- `MetaSchema.*`
- `MetaDataType.*`
- `MetaDataTypeConversion.*`
- `MetaTransformScript.*`
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

Then install the packaged BI CLIs (`meta-schema`, `meta-data-type`, `meta-data-type-conversion`, `meta-convert`, `meta-datavault-raw`, `meta-datavault-business`) into `%LOCALAPPDATA%metabin` and add that directory to your user `PATH`:

```cmd
MetaBi\Installer\bin\publish\win-x64\install-meta-bi.exe
```

`meta-transform-script` is not yet added to that installer flow. During active development, run it from `MetaTransformScript\Cli\bin\Debug\net8.0` or put that debug bin on your `PATH`.

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
- `meta-transform-script`

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

### meta-transform-script

`MetaTransformScript` provides a canonical, semantically round-trippable SQL `VIEW` syntax model for a supported bounded SQL surface. It can import supported SQL into canonical workspace form, emit semantically equivalent SQL back out, and prove round-trip stability with `meta instance diff`.

Purpose:
- author and maintain a sanctioned `MetaTransformScript` workspace for the supported SQL `VIEW` body subset
- import supported SQL from files, folders, or inline code into canonical workspace form
- emit semantically equivalent SQL back out of that workspace
- prove the core invariant `SQL -> workspace -> SQL -> workspace` with `meta instance diff`

Current command surface:
- `meta-transform-script help`
- `meta-transform-script from sql-path --path <path> --new-workspace <path>`
- `meta-transform-script from sql-code --code <sql> --new-workspace <path> [--name <name>]`
- `meta-transform-script to sql-path [--workspace <path>] --out <path>`
- `meta-transform-script to sql-code [--workspace <path>] [--name <name>]`

What the model is:
- this is a canonical syntax model, not a blob store for SQL text
- the modeled truth is the supported SQL view body, rooted in the `SelectStatement` family
- `CREATE VIEW` wrapper syntax is treated as an import/export envelope, not as the primary modeled truth
- wrapper details currently captured in the model are:
  - view schema identifier
  - view object identifier
  - explicit view column list
- round-trip is semantic, not trivia-preserving:
  - original whitespace
  - comments
  - token offsets
  - exact file formatting
  are not part of the contract
- binding, type inference, target validation, and lineage are follow-on layers built on top of this syntax model; they do not replace it

Import behavior:
- `from sql-path` accepts either:
  - one `.sql` file
  - one folder of `.sql` files
- a single `.sql` file may contain:
  - one supported bare `SELECT`
  - one or more supported `CREATE VIEW ... AS ... GO` statements
  - batches with leading `SET ...` statements
- explicit view column lists in `CREATE VIEW` are captured and emitted back out
- `from sql-code` imports SQL text directly and optionally takes `--name` when the input is a bare `SELECT`

Export behavior:
- `to sql-code` emits the modeled view body only
- `to sql-path` emits `CREATE VIEW ... AS ... GO`
- if `--out` ends with `.sql`, all scripts are emitted into one combined file
- otherwise `--out` is treated as a target folder
- folder export preserves the original `SourcePath` file names when possible so a re-import can round-trip deterministically

Supported SQL surface today:
- query roots and composition:
  - `SELECT`
  - parenthesized query expressions
  - common table expressions (`WITH ... AS (...)`)
  - `WITH XMLNAMESPACES`
  - `UNION`
  - `UNION ALL`
  - `EXCEPT`
  - `INTERSECT`
- projection:
  - named select items
  - aliases
  - `*`
  - qualified `alias.*`
- table sources:
  - named table references
  - aliases
  - qualified joins
  - unqualified joins
  - `CROSS APPLY`
  - `OUTER APPLY`
  - query-derived tables
  - inline `VALUES`
  - parenthesized joins
  - schema-object function table references
  - built-in table functions covered by the reference corpus
  - `PIVOT`
  - `UNPIVOT`
  - `TABLESAMPLE`
  - full-text table references such as `CONTAINSTABLE` / `FREETEXTTABLE`
- filtering and predicate forms:
  - boolean `AND` / `OR`
  - `NOT`
  - parenthesized boolean expressions
  - scalar comparison predicates
  - `IS NULL` / `IS NOT NULL`
  - `BETWEEN`
  - `IN (...)`
  - `IN (subquery)`
  - `EXISTS`
  - subquery comparison predicates with `ANY` / `ALL`
  - `LIKE`
  - `IS DISTINCT FROM`
  - full-text predicates such as `CONTAINS` / `FREETEXT`
- grouping and aggregation:
  - `GROUP BY`
  - `HAVING`
  - aggregate function calls in projection and other supported expression positions
  - `GROUPING SETS`
  - `ROLLUP`
  - `CUBE`
  - composite and grand-total grouping shapes covered by the current corpus
- ordering and row limiting:
  - query-level `ORDER BY`
  - `TOP`
  - `OFFSET ... FETCH`
  - sort order per element
- windowing:
  - `OVER`
  - `PARTITION BY`
  - window `ORDER BY`
  - window frames
  - named `WINDOW` definitions
  - windowed aggregate and analytic functions covered by the current corpus
- scalar/value expression families:
  - column references
  - multipart identifiers
  - string, integer, numeric, money, binary, `NULL`, and `MAX` literal families covered by the current corpus
  - arithmetic binary expressions
  - unary expressions
  - parenthesized expressions
  - `CASE`
  - `COALESCE`
  - `NULLIF`
  - `IIF`
  - ordinary function calls
  - parameterless/system-call style expressions covered by the current corpus
  - `CAST`
  - `TRY_CAST`
  - `CONVERT`
  - `TRY_CONVERT`
  - `PARSE`
  - `TRY_PARSE`
  - parameterized data type references
  - primary-expression collation
  - `AT TIME ZONE`
  - sequence/global expression cases covered by the current corpus
- subqueries:
  - scalar subqueries
  - correlated subqueries in the supported expression and predicate forms exercised by the corpus
- XML-in-view support:
  - `WITH XMLNAMESPACES`
  - XML method-style calls as exercised in the reference corpus, for example `.value(...)`, `.query(...)`, and `.exist(...)`

Current unsupported or excluded surface:
- `OPENJSON`
- `OPENROWSET`
- `OPENQUERY`
- provider/ad-hoc external-source wrapper forms such as the current `OPENROWSET` provider and `OPENDATASOURCE` reference cases
- `CHANGETABLE`
- the current ODBC escape-surface reference case
- `CREATE VIEW` wrapper options
- `WITH CHECK OPTION`
- materialized view syntax

Reference corpus status:
- `MetaTransformScript\Reference\Corpus` contains the broader working SQL corpus used to pressure the importer/emitter
- the currently supported reference-corpus round-trip demo uses the supported subset of that corpus and excludes the unsupported surfaces listed above
- the current supported reference-corpus demo round-trips `32` scripts through `SQL -> workspace -> SQL -> workspace`
- the proof point is `meta instance diff` reporting no differences between the original and round-tripped workspaces

Reference corpus demo commands:

```cmd
cd Samples\Demos\MetaTransformScriptReferenceCorpusCliIntegration
call cleanup.cmd

meta-transform-script from sql-path --path SourceViews --new-workspace MetaTransformScriptReferenceCorpusWorkspace

pushd MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script to sql-path --out ..\RoundTrippedViews
meta-transform-script to sql-path --out ..\RoundTrippedViews.sql
meta-transform-script to sql-code --name dbo.v_window_functions
popd

meta-transform-script from sql-path --path RoundTrippedViews --new-workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta instance diff MetaTransformScriptReferenceCorpusWorkspace MetaTransformScriptReferenceCorpusRoundTripWorkspace

pushd MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script to sql-code --name dbo.v_xml_namespaces_and_methods
popd
```

The expected proof point in `run.output` is:

```text
Instance diff: no differences.
```

Smaller two-script demo:

```cmd
cd Samples\Demos\MetaTransformScriptCliIntegration
call cleanup.cmd

meta-transform-script from sql-path --path SourceViews --new-workspace MetaTransformScriptCliIntegrationWorkspace

pushd MetaTransformScriptCliIntegrationWorkspace
meta-transform-script to sql-path --out ..\RoundTrippedViews
meta-transform-script to sql-path --out ..\RoundTrippedViews.sql
meta-transform-script to sql-code --name sales.CustomerOrderSummary
popd

meta-transform-script from sql-path --path RoundTrippedViews --new-workspace MetaTransformScriptRoundTripWorkspace
```

Single-file and inline-code examples:

```cmd
meta-transform-script from sql-path --path .\Views --new-workspace .\MetaTransformScript.Workspace
meta-transform-script to sql-path --workspace .\MetaTransformScript.Workspace --out .\out\Views
meta-transform-script to sql-path --workspace .\MetaTransformScript.Workspace --out .\out\Views.sql
meta-transform-script to sql-code --workspace .\MetaTransformScript.Workspace --name dbo.v_window_functions

meta-transform-script from sql-code --code "select 1 as A" --name dbo.v_inline --new-workspace .\MetaTransformScript.Inline
meta-transform-script to sql-code --workspace .\MetaTransformScript.Inline --name dbo.v_inline
```


## Active Models

Current active BI model families include:

- `MetaSchema`
- `MetaDataType`
- `MetaDataTypeConversion`
- `MetaTransformScript`
- `MetaRawDataVault`
- `MetaBusinessDataVault`
- `MetaSql`

`MetaTransformScript` is the current sanctioned SQL-script modeling track in this repo. It models the supported SQL `VIEW` body subset, carries selected `CREATE VIEW` envelope fields needed for file-based import/export, and is exercised through the `meta-transform-script` CLI plus the demos under `Samples\Demos\MetaTransformScript*`.

## Current Projection Status

- `meta-convert raw-datavault-to-sql` is operational:
  it converts sanctioned raw DV to a current `MetaSql` workspace and does not query any live database.
- `meta-convert business-datavault-to-sql` is operational:
  it converts sanctioned business DV to a current `MetaSql` workspace, applies sanctioned business-type lowering, and does not query any live database.






