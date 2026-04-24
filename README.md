# meta-bi

`meta-bi` is the BI stack that sits on top of the generic `meta` foundation.

This repository contains BI-oriented sanctioned models, CLIs, and docs:

- `MetaSchema.*`
- `MetaDataType.*`
- `MetaDataTypeConversion.*`
- `MetaTransformScript.*`
- `MetaTransformBinding.*`
- `MetaDataVault.*`
- `MetaSql.Workspace`
- `MetaSql.Core`

It also contains BI architecture notes in `docs/`.

## Dependency Boundary

This repository now consumes the generic foundation through an internal NuGet package boundary instead of source-level project references.

Direct foundation package dependency:

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
dotnet build MetaTransform\Script\Tests\MetaTransformScript.Tests.csproj
dotnet build MetaTransform\Binding\Cli\MetaTransformBinding.Cli.csproj
```

This keeps BI work from silently editing foundation code and makes the boundary explicit.

Build the installer:

```cmd
dotnet build MetaBi\Installer\MetaBi.Installer.csproj
```

Then install the packaged BI CLIs (`meta-schema`, `meta-data-type`, `meta-data-type-conversion`, `meta-convert`, `meta-datavault-raw`, `meta-datavault-business`, `meta-pipeline`, `meta-transform-script`, `meta-transform-binding`) into `%LOCALAPPDATA%\meta\bin` and add that directory to your user `PATH`:

```cmd
MetaBi\Installer\bin\publish\win-x64\install-meta-bi.exe
```

The installer copies published CLI payloads from the current `meta-bi` checkout into `%LOCALAPPDATA%\meta\bin`.
If install reports missing binaries, publish the CLI projects first (for example `dotnet publish MetaPipeline\Cli\MetaPipeline.Cli.csproj -c Debug -r win-x64`, `dotnet publish MetaTransform\Script\Cli\MetaTransformScript.Cli.csproj -c Debug -r win-x64`, and `dotnet publish MetaTransform\Binding\Cli\MetaTransformBinding.Cli.csproj -c Debug -r win-x64`).

## Intent

The long-term repo boundary is:

- `meta`: generic foundation (`Meta.Core`, `meta`, `MetaWeave`, `meta-weave`, generic metamodels)
- `meta-bi`: sanctioned BI models and BI-specific CLIs/tooling

## Included Build Entry Points

Top-level solution files:
- `MetaSchema.sln`
- `MetaDataType.sln`
- `MetaDataTypeConversion.sln`
- `MetaDataVault.sln`

Project-first stacks (built via `.csproj` entry points):
- `MetaTransform\Script\Cli\MetaTransformScript.Cli.csproj`
- `MetaTransform\Script\Tests\MetaTransformScript.Tests.csproj`
- `MetaTransform\Binding\Cli\MetaTransformBinding.Cli.csproj`
- `MetaPipeline\Cli\MetaPipeline.Cli.csproj`
- `MetaPipeline\Tests\MetaPipeline.Tests.csproj`
- `MetaSql\Cli\MetaSql.Cli.csproj`
- `MetaSql\Tests\MetaSql.Tests.csproj`

## CLI Guide

`meta-bi` ships these operator-facing CLIs:

- `meta-schema`
- `meta-data-type`
- `meta-data-type-conversion`
- `meta-convert`
- `meta-datavault-raw`
- `meta-datavault-business`
- `meta-pipeline`
- `meta-sql`
- `meta-transform-script`
- `meta-transform-binding`

### meta-schema

Purpose:
- extract a sanctioned `MetaSchema` workspace from a live SQL Server schema

Command surface:
- `meta-schema help`
- `meta-schema extract sqlserver --new-workspace <path> --connection-env <name> --system <name> (--schema <name> | --all-schemas) (--table <name> | --all-tables)`

Example:

`--connection-env` names the shell-visible environment variable that contains the SQL Server connection string.

```cmd
meta-schema extract sqlserver --new-workspace .\MetaSchema.Workspace --connection-env META_SQL_DEV --system MySystem --schema dbo --all-tables
```

### meta-data-type

Purpose:
- bootstrap sanctioned `MetaDataType` workspaces

Command surface:
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

Command surface:
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
- perform cross-model conversions handled by conversion glue code

Command surface:
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

Command surface:
- `meta-datavault-raw --new-workspace <path>`
- `meta-datavault-raw add-*`

`add-*` commands:
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

Command surface:
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

Command surface:
- `meta-sql deploy-plan --source-workspace <path> --connection-env <name> --out <path> [--approve-drop-table <schema.table>] [--approve-drop-column <schema.table.column>] [--approve-truncate-column <schema.table.column>] [--approval-file <path>]`
- `meta-sql deploy --manifest-workspace <path> --source-workspace <path> --connection-env <name>`

Behavior summary:
- `deploy-plan` extracts live schema when the target database exists, otherwise treats live as truly empty and writes a deploy manifest against that empty live state
- `deploy-plan` and `deploy` always operate on the full source workspace and full live database; filtered subset deploy is not supported
- destructive actions require exact object-scoped approvals
- `deploy` executes only the manifest after source/live fingerprint validation
- when the manifest expects a missing target database, `deploy` creates it first and refuses if it already exists
- schema creation is explicit in the manifest (`AddSchema`), not inferred while rendering table DDL

Examples:

`--connection-env` names the shell-visible environment variable that contains the target SQL Server connection string.

```cmd
meta-sql deploy-plan --source-workspace .\CurrentMetaSql.Workspace --connection-env META_SQL_DEV --out .\out\deploy-manifest
meta-sql deploy --manifest-workspace .\out\deploy-manifest --source-workspace .\CurrentMetaSql.Workspace --connection-env META_SQL_DEV
```

### meta-pipeline

Purpose:
- execute one bound `MetaTransformScript` and write its row stream
- keep stage 1 centered on SQL Server source read, bounded row buffering, and SQL Server target write

Command surface:
- `meta-pipeline help`
- `meta-pipeline execute sqlserver --transform-workspace <path> --binding-workspace <path> --script <name> --source-connection-env <name> --target-connection-env <name> [--target <sql-identifier>] [--batch-size <n>]`

Behavior summary:
- `execute sqlserver` resolves one explicitly named transform binding from the binding workspace
- the command emits the transform SQL body from the transform workspace, executes that query against the source SQL Server connection, buffers rows in bounded batches, and bulk-copies each batch into the bound target table on the target SQL Server connection
- `--source-connection-env` and `--target-connection-env` name shell-visible environment variables; the command resolves them to connection strings at runtime
- `--script` is always required because stage 1 runs one transform script per execution
- if the selected binding contains multiple targets, `--target` is required
- stage 1 execution supports parameterless transform scripts and one selected target per run

Example:

```cmd
meta-pipeline execute sqlserver --transform-workspace .\TransformWS --binding-workspace .\BindingWS --source-connection-env META_PIPELINE_SOURCE --target-connection-env META_PIPELINE_TARGET --script dbo.v_customer_load
```

### meta-transform-script

`MetaTransformScript` provides a canonical, semantically round-trippable SQL `VIEW` syntax model for a supported bounded SQL surface. It imports supported SQL into canonical workspace form, emits semantically equivalent SQL back out, and proves stability through `SQL -> workspace -> SQL -> workspace` plus `meta instance diff`.

Purpose:
- author and maintain a sanctioned `MetaTransformScript` workspace for the supported SQL `VIEW` body subset
- import supported SQL from files or inline code into canonical workspace form
- emit semantically equivalent SQL back out of that workspace
- prove the core invariant `SQL -> workspace -> SQL -> workspace` with `meta instance diff`
- serve as the authored syntax substrate for later binding, type inference, and validation layers

Command surface:
- `meta-transform-script help`
- `meta-transform-script from sql-file --path <file.sql> --target <sql-identifier> (--new-workspace <path> | --workspace <path>)`
- `meta-transform-script from sql-code --code <sql> --target <sql-identifier> (--new-workspace <path> | --workspace <path>) [--name <name>]`
- `meta-transform-script to sql-path [--workspace <path>] --out <path>`
- `meta-transform-script to sql-code [--workspace <path>] [--name <name>]`

What the model is:
- this is a canonical syntax model with typed workspace rows for supported SQL structure
- the modeled truth is the supported SQL view body, rooted in the `SelectStatement` family
- `CREATE VIEW` wrapper syntax is treated as an import/export envelope, not as the primary modeled truth
- wrapper details captured in the model are:
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

Model entity list:
- script and statement spine:
  - `TransformScript`
  - `TransformScriptSelectStatementLink`
  - `SelectStatement`
  - `StatementWithCtesAndXmlNamespaces`
  - `QueryExpression`
  - `QuerySpecification`
  - `CommonTableExpression`
- source/table side:
  - `FromClause`
  - `TableReference`
  - `NamedTableReference`
  - `QualifiedJoin`
  - `QueryDerivedTable`
  - `PivotedTableReference`
  - `UnpivotedTableReference`
  - `FullTextTableReference`
- expression side:
  - `SelectElement`
  - `ScalarExpression`
  - `BooleanExpression`
  - `FunctionCall`
  - `CaseExpression`
  - `ColumnReferenceExpression`
  - `ScalarSubquery`
  - `OverClause`
  - `WindowDefinition`
- support entities:
  - `Identifier`
  - `MultiPartIdentifier`
  - `SchemaObjectName`
  - `Literal`
  - `DataTypeReference`
- structural helper entities:
  - `*Link` rows carry optional or structured relationships explicitly
  - `*Item` rows preserve ordered collections explicitly

These entities and helper rows are what the workspace persists for the supported SQL structure.

Import behavior:
- `from sql-file` accepts one `.sql` file per command invocation
- each import command requires explicit `--target <sql-identifier>`
- use `--new-workspace` for the first script and `--workspace` to append subsequent scripts one-by-one
- a `.sql` file may contain:
  - one supported bare `SELECT`
  - one or more supported `CREATE VIEW ... AS ... GO` statements
  - batches with leading `SET ...` statements
- explicit view column lists in `CREATE VIEW` are captured and emitted back out
- `from sql-code` imports SQL text directly, requires explicit `--target`, and optionally takes `--name` when the input is a bare `SELECT`

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
  - composite and grand-total grouping shapes covered by the corpus
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
  - windowed aggregate and analytic functions covered by the corpus
  - percentile analytic/window functions with `WITHIN GROUP` such as `PERCENTILE_CONT` and `PERCENTILE_DISC`
- scalar/value expression families:
  - column references
  - multipart identifiers
  - string, integer, numeric, money, binary, `NULL`, and `MAX` literal families covered by the corpus
  - arithmetic binary expressions
  - unary expressions
  - parenthesized expressions
  - `CASE`
  - `COALESCE`
  - `NULLIF`
  - `IIF`
  - ordinary function calls
  - parameterless/system-call style expressions covered by the corpus
  - `CAST`
  - `TRY_CAST`
  - `CONVERT`
  - `TRY_CONVERT`
  - `PARSE`
  - `TRY_PARSE`
  - parameterized data type references
  - primary-expression collation
  - `AT TIME ZONE`
  - sequence/global expression cases covered by the corpus
- subqueries:
  - scalar subqueries
  - correlated subqueries in the supported expression and predicate forms exercised by the corpus
- XML-in-view support:
  - `WITH XMLNAMESPACES`
  - XML method-style calls as exercised in the reference corpus, for example `.value(...)`, `.query(...)`, and `.exist(...)`
  - XML `nodes(...)` table sources

Data type alias note:
- import accepts selected SQL Server type aliases in supported type-reference positions
- aliases are normalized to sanctioned canonical SQL type names in the model and emitter output
- examples: `integer -> int`, `sysname -> nvarchar(128)`, `character varying -> varchar`, `double precision -> float`, `national character varying -> nvarchar`

Detailed parser/emitter checklist:
- the exact implemented and verified surface is tracked in [docs/META-TRANSFORM-SCRIPT-PARSER-STATUS.md](docs/META-TRANSFORM-SCRIPT-PARSER-STATUS.md)
- open items there are ordinary parser/emitter/model/import-shaping gaps

Unsupported or Excluded Surface:
- `OPENJSON`
- `OPENROWSET`
- `OPENQUERY`
- provider/ad-hoc external-source wrapper forms such as `OPENROWSET` provider and `OPENDATASOURCE` reference cases
- `CHANGETABLE`
- the ODBC escape-surface reference case
- `CREATE VIEW` wrapper options
- `WITH CHECK OPTION`
- materialized view syntax

Reference corpus status:
- `MetaTransform\Script\Reference\Corpus` contains the broader SQL corpus used to pressure the importer/emitter
- the reference-corpus round-trip demo uses the supported subset of that corpus and excludes the unsupported surfaces listed above
- the exact supported parser/emitter surface and proof cases are tracked in `docs/META-TRANSFORM-SCRIPT-PARSER-STATUS.md`
- the proof point is `meta instance diff` reporting no differences between the original and round-tripped workspaces

Reference corpus demo commands:

```cmd
cd Samples\Demos\MetaTransformScriptReferenceCorpusCliIntegration
call cleanup.cmd

meta-transform-script from sql-file --path SourceViews\001_basic_select\view.sql --target dbo.v_basic --new-workspace MetaTransformScriptReferenceCorpusWorkspace

pushd MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script to sql-path --out ..\RoundTrippedViews
meta-transform-script to sql-path --out ..\RoundTrippedViews.sql
meta-transform-script to sql-code --name dbo.v_window_functions
popd

meta-transform-script from sql-file --path RoundTrippedViews\view.sql --target dbo.v_basic --new-workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta instance diff MetaTransformScriptReferenceCorpusWorkspace MetaTransformScriptReferenceCorpusRoundTripWorkspace

pushd MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script to sql-code --name dbo.v_xml_namespaces_and_methods
popd
```

Captured output excerpt from `Samples\Demos\MetaTransformScriptReferenceCorpusCliIntegration\run.output`:

```text
> meta-transform-script from sql-file --path SourceViews\001_basic_select\view.sql --target dbo.v_basic --new-workspace MetaTransformScriptReferenceCorpusWorkspace
OK: Created MetaTransformScriptReferenceCorpusWorkspace
Import:
  Scripts: 32

> meta-transform-script from sql-file --path RoundTrippedViews\view.sql --target dbo.v_basic --new-workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
OK: Created MetaTransformScriptReferenceCorpusRoundTripWorkspace
Import:
  Scripts: 32

> meta instance diff MetaTransformScriptReferenceCorpusWorkspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
Instance diff: no differences.
Rows: left=4996, right=4996  Properties: left=8348, right=8348
NotIn: left-not-in-right=0, right-not-in-left=0
```

### meta-transform-binding

`MetaTransformBinding` is the binding contract layer on top of `MetaTransformScript`.

Purpose:
- bind all transform scripts in a transform workspace into an explicit binding workspace (`rowsets`, `columns`, source/target SQL identifiers)
- validate source and target contracts against explicit schema workspaces in the same command
- fail hard on contract mismatches and persist explicit validation link rows in the resulting workspace

Command surface:
- `meta-transform-binding help`
- `meta-transform-binding bind --transform-workspace <path> --source-schema <path> [--source-schema <path> ...] --target-schema <path> --execute-system <name> --new-workspace <path> [--execute-system-default-schema-name <schema>] [--ignore-target-columns <col[,col...]>]`

Behavior summary:
- `bind` reads the target SQL identifier from `TransformScript.TargetSqlIdentifier`
- source identifiers resolve against source schema workspaces; target identifiers resolve against the target schema workspace
- source identifier resolution is explicit:
  - `system.schema.table` resolves directly
  - `schema.table` resolves as `<execute-system>.<schema>.<table>`
  - `table` resolves as `<execute-system>.<execute-system-default-schema-name>.<table>`
- `--execute-system` is required and must be represented in provided source schema workspaces when one/two-part source identifiers exist
- if any one-part source identifier exists, `--execute-system-default-schema-name` is required
- each source/target schema workspace must contain exactly one system
- `bind` enforces target write-contract shape using non-identity target fields
- `bind` processes all transform scripts in the transform workspace
- `--ignore-target-columns` excludes named non-identity target columns from target conformance checks; unknown names fail explicitly
- bind is atomic: if binding or validation fails, no output workspace is created
- scale proof is included in `Samples\Demos\MetaTransformScriptTpcDsCliIntegration\run.cmd`, which imports and binds TPC-DS `q01`-`q99` in one workspace run

Examples:

```cmd
meta-transform-binding bind --transform-workspace .\TransformWS --source-schema .\SourceSchemaWS --target-schema .\TargetSchemaWS --execute-system WarehouseDb --new-workspace .\BindingWS

meta-transform-binding bind --transform-workspace .\TransformWS --source-schema .\SalesSchemaWS --source-schema .\ReferenceSchemaWS --target-schema .\WarehouseSchemaWS --execute-system WarehouseDb --execute-system-default-schema-name dbo --new-workspace .\BindingWS --ignore-target-columns LoadUtc,RunId
```

See also:
- `Samples\Demos\MetaTransformBindingCliIntegration\run.cmd`
- `Samples\Demos\MetaTransformBindingCliIntegration\README.md`
- `Samples\Demos\MetaTransformScriptTpcDsCliIntegration\run.cmd`


