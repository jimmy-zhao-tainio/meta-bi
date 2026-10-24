# SQL conversion snippets

MetaDataTypeConversion owns conversion snippets as authored instance data.
`ConversionImplementation.SqlConversionSnippet` references a reusable
`SqlConversionSnippet` with `Dialect` and `SqlText`. SQL text uses the same
`$(name)` placeholder syntax as transform-pattern text. The conversion service
has no dependency on MetaTransformPattern, its instance model or its weave.

For example, the built-in cast snippet stores this actual expression:

```sql
CAST($(value) AS $(type))
```

`SqlConversionParameterBinding` rows belong to a snippet. Each row gives a
placeholder `Name` and a natural reference to a `SqlConversionParameter`.
The built-in parameters are `SourceExpression` and `DestinationType`; the cast
and convert snippets each have `value` and `type` binding rows. Placeholder
names are authored data and can be renamed together with their bindings.
There is no SQL function selector or C# dispatch for conversion expressions.

## Substitution and syntax

The production `sql-expression` command calls
`MetaDataTypeConversionService.GenerateSqlExpression`. The service resolves
one mapping, obtains its snippet, prepares its two inputs and substitutes the
snippet's bound placeholders in one pass. Repeated placeholders receive the
same value. Names are case-insensitive, may contain the same letters, digits,
underscores, dots and hyphens as pattern placeholders, and allow surrounding
whitespace. `$$(name)` produces literal `$(name)`. Inserted SQL fragments are
never rescanned for placeholders. Missing, duplicate, unused and unsupported
bindings fail with identified diagnostics.

The source input must parse as exactly one supported SQL scalar expression.
Its rendered form is grouped in parentheses before substitution so an authored
snippet such as `CAST($(value) * 2 AS $(type))` preserves a compound operand:
`1 + 2` becomes `(1 + 2) * 2`, yielding 6 rather than 5.

The destination type comes from the selected mapping and MetaDataType catalog.
The request supplies typed length/MAX, precision and scale facets. The existing
MetaTransformScript type model and renderer produce the bound type fragment;
applicable parameters must be explicit and valid. No type defaults are invented.
The complete substituted expression is parsed into MetaTransformScript and
rendered by its existing SQL machinery. Additional statements, aliases and
query tails are rejected.

Function choice, composition, fixed style/culture arguments and failure behavior
are in `SqlText`. For example, an author can replace the expression with:

```sql
COALESCE(CONVERT($(type), $(value), 112), N'unknown')
```

Generation then produces that composition without a C# change. The built-in
snippets use strict CAST and CONVERT; TRY behavior is used only when explicitly
authored in a snippet. SQL and runtime conversion support remain bounded by
MetaTransformScript and SQL Server. Generation does not prove losslessness or
infer a safe destination capacity from values.

## Concrete example and authoring

The built-in `sqlserver:int -> sqlserver:nvarchar` mapping references
`MetaDataTypeConversion:implementation:sql-expression`, whose default snippet
is `sqlserver:snippet:cast`. From the repository root after building:

```powershell
& ./MetaDataTypeConversion/Cli/bin/Release/net8.0/meta-data-type-conversion.exe sql-expression `
  --workspace ./MetaDataTypeConversion/Workspace `
  --source-data-type sqlserver:type:int --target-data-type-system SqlServer `
  --input-expression '([s].[Value] + 2) * 3' --length 24
```

The SQL is `CAST((([s].[Value] + 2) * 3) AS nvarchar(24))`.

Inspect current CLI help before authoring. Change an existing snippet through
the modeled metadata CLI, for example:

```powershell
meta instance update SqlConversionSnippet sqlserver:snippet:cast `
  --set 'SqlText=COALESCE(CONVERT($(type), $(value), 112), N''unknown'')' `
  --workspace <your-conversion-workspace>
```

Or select another reusable snippet on the implementation:

```powershell
meta instance relationship set ConversionImplementation `
  MetaDataTypeConversion:implementation:sql-expression `
  --to SqlConversionSnippet sqlserver:snippet:convert `
  --workspace <your-conversion-workspace>
```

New snippets and their binding rows are added as instances of the same model.
There is no expression-registration step in generator code. The checked-in
C# workspace was regenerated using the metadata CLI.

## Preserved behavior and migration

One mapping per source type and target system, compatibility traversal and the
existing physical-mapping consumers remain unchanged. Direct remains a
pass-through; Structural has no scalar fallback. Binding does not insert casts
or rewrite authored SQL. Compatibility paths are not executed as conversion
programs. The warehouse and vault converters continue their existing generation
paths.

Workspaces from the previous selector-based commit must be regenerated or
migrated through the modeled CLI: replace SqlConversionTemplate/Function with
SqlConversionSnippet/SqlText, author actual SQL text and add parameter and
binding entities/instances. The built-in value implementation is now named
SqlExpression. No compatibility interpretation of the retired function values
remains. Test disposable SQL resources before retained schema upgrades.

## Verification

Release builds use the following command for the conversion, unit, transform,
warehouse and vault test projects:

```powershell
dotnet build <test-project> -c Release -m:1 -nr:false --nologo --verbosity quiet
```

Focused checks:

```powershell
./eng/test.ps1 -Profile Unit -Configuration Release
dotnet test MetaDataTypeConversion/Tests/MetaDataTypeConversion.Tests.csproj -c Release --no-build --no-restore --nologo
dotnet test MetaTransform/Script/Tests/MetaTransformScript.Tests.csproj -c Release --no-build --no-restore --nologo --filter 'FullyQualifiedName~SqlCorpusExpressionRoundTripTests|FullyQualifiedName~TransformBindingTests|FullyQualifiedName~TransformBindingHardeningTests'
dotnet test MetaDataWarehouse/Tests/MetaDataWarehouse.Tests.csproj -c Release --no-build --no-restore --nologo --filter FullyQualifiedName~DataWarehouseToSqlConverterTests
dotnet test MetaDataVault/Tests/MetaDataVault.Tests.csproj -c Release --no-build --no-restore --nologo --filter FullyQualifiedName~ConvertToMetaSqlTests
git diff --check
```

Conversion tests cover data-only expression changes, fixed arguments, renamed
and repeated placeholders, escapes, nonrecursive substitution, diagnostics,
compound operands and explicit target facets. XML and C# round trips retain
snippet text and natural binding relationships, and invoke the production CLI
before and after an authored expression change. The SQL Server test persists
the model in a uniquely named disposable database, checks natural references,
executes generated expressions, proves compound-operand grouping and verifies
strict failure plus decimal precision/scale. Its test-only model name is changed
to match the temporary database name and restored for typed materialization,
as required by the foundation SQL surface. The database is removed afterward.

On 2026-10-24 all five Release builds completed with zero warnings/errors.
The conversion suite passed 54 tests, the unit gate passed 78 tests at 148 ms
summed execution, the focused transform/binding suite passed 177 tests, warehouse
passed 7 tests and vault passed 29 tests. The first cold unit run exceeded the
one-second budget at 1155 ms; the unchanged gate passed on rerun. SQL Server 2022
persistence and execution were exercised and no test databases remained.
`git diff --check` passed. The full repository Integration profile was not run.
