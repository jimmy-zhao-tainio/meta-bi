# SQL conversion snippets and consumer investigation

No pre-existing consumer was found that constructs mapping-driven SQL
value-conversion expressions. The investigation compared this branch with
`5cee491`, searched production references to MetaDataTypeConversion and inspected
the actual packaged weaves. This change delivers conversion metadata and a
metadata-instantiation helper. It does not integrate snippets into an existing
BI execution flow or replace an existing SQL conversion policy.

## Existing behavior traced

| Existing consumer | Selection and actual responsibility | Does the implementation choose a SQL value expression? |
| --- | --- | --- |
| `MetaDataType/Core/MetaDataTypeWorkspaceService.cs`, `CreateWorkspace` | Supplies the sanctioned type/type-system catalog. | No. |
| `MetaDataTypeConversion/Core/MetaDataTypeConversionService.cs`, `Resolve` and `ResolveCompatibility` | Selects one mapping per source/system, or returns compatibility evidence through the existing graph traversal. The existing `resolve` command reports that selection. | No SQL expression is constructed. |
| `MetaConvert/DataWarehouseToSql/DataWarehouseToSqlConverter.cs`, `ConvertToMetaSql` | Executes the packaged DataWarehouseToSql weave with `dataTypes` and `typeConversions`. `TableColumn` and `TableColumnDataTypeDetail` lower Meta types to SQL Server type IDs and carry type details. Already-SQL Server types retain their identity. | Direct implementation identity gates eligible type mappings; it does not select a SQL conversion expression. |
| `MetaConvert/DataVaultToSql/ToMetaSql.cs`, `Converter.ConvertAsync` | Executes RawDataVaultToSql or BusinessDataVaultToSql. Their `LoweredTypes` relations select physical types. Raw uses direct SQL Server mappings then identity fallback; Business uses direct Meta-to-SQL Server mappings. | No. `DefaultExpressionSql` values are copied from the implementation workspace, independently of mapping implementation selection. |
| `MetaTransform/Binding/TransformBindingValidationService.cs`, `ValidateTargetColumnTypeConformance` and `AppendTargetColumnTypeAssessment` | Calls `ResolveCompatibility`, validates length/precision/scale and records exact or sanctioned-conversion evidence. | No. Authored SQL remains unchanged. |
| `MetaPipeline/Core/SqlServer/SqlServerBulkCopyRowStreamWriter.cs`, `ResolveRuntimeTargetDataTypeId`, `ResolveSqlServerClrType`, `CoerceValue` | Resolves a target representation when necessary, chooses a CLR storage type and coerces batch values for `SqlBulkCopy`. Already-target-system types bypass mapping. | It performs CLR value coercion, including `Convert.ChangeType` and CLR parsing, rather than constructing SQL. The selected implementation is not consulted for that coercion. Replacing it with snippets would require a new SQL execution workflow, outside this change. |
| `MetaConvert/SchemaToDataVault/RawDataVaultFromMetaSchemaService.cs`, `MaterializeWithReport`, and SchemaToRawDataVault `Field`/`FieldDataTypeDetail` | Executes with only `schema` and `options`; copies schema field type IDs/details. | It does not consume MetaDataTypeConversion. |
| `MetaTransform/Script/Sql/MetaTransformScriptSqlEmitter.Expressions.Calls.cs` and `MetaConvert/TransformScriptToSql/TransformScriptToSqlConverter.cs` | Render CAST, CONVERT and other already-authored syntax nodes. The packaged TransformScriptToSql weave accepts only the `transform` source workspace. | SQL syntax rendering, not mapping-specific conversion policy. |

All 77 modeled relations/transformations across DataWarehouseToSql,
RawDataVaultToSql, BusinessDataVaultToSql and SchemaToRawDataVault were emitted
with `meta-weave emit-relation` / `emit-transformation` and inspected. Their
CAST/CONVERT-family calls are `TRY_CONVERT(int, ...Ordinal)` evaluations used to
order metadata. They do not implement conversions of business values selected
by DataTypeMapping. Test reference converters were excluded as production
integration evidence.

Reproduce the key packaged mapping reads, for example:

```powershell
meta-weave emit-relation --workspace MetaConvert/Weaves/RawDataVaultToSql --direction forward --name LoweredTypes
meta-weave emit-relation --workspace MetaConvert/Weaves/BusinessDataVaultToSql --direction forward --name LoweredTypes
meta-weave emit-transformation --workspace MetaConvert/Weaves/DataWarehouseToSql --direction forward --name TableColumn
meta-weave emit-transformation --workspace MetaConvert/Weaves/SchemaToRawDataVault --direction forward --name Field
```

## Scope correction and preserved behavior

The added `sql-expression` command, handler, runtime binding, options,
command-model instances, command-specific execution tests and usage instructions
have been removed. CLI sources and command metadata now match `5cee491` exactly;
the existing help test verifies the removed command is absent. Create, check,
resolve and help remain. The added method was also removed from the existing
`IMetaDataTypeConversionService` interface consumed by BI components.

The branch had changed the existing `int-to-nvarchar` mapping from Direct to
SqlExpression. That selection is restored. Materializing the original and
current workspaces and comparing all 69 mapping IDs, source IDs, target IDs and
implementation IDs showed no differences. This matters because the Raw weave
uses implementation identity when selecting physical mappings. No defect in
that pre-existing behavior is claimed or redesigned here.

The reusable SqlExpression implementation and snippet definitions are available
as metadata; none of the pre-existing built-in mappings is redirected to them.
Tests explicitly author temporary snippet selections. They establish metadata
capability, not production BI adoption. Existing catalog, converter, weave and
bulk-copy sources are unchanged from the starting commit. Compatibility
traversal remains unchanged; the binding regression proves SQL is not rewritten.

## Authored instance data

`ConversionImplementation.SqlConversionSnippet` is a natural reference to a
reusable `SqlConversionSnippet`. Its `TransformScript` property holds authored,
parameterized text using TransformScript syntax. The actual built-in snippet
instances are:

| Snippet ID | TransformScript |
| --- | --- |
| `sqlserver:snippet:cast` | `CAST($(value) AS $(type))` |
| `sqlserver:snippet:convert` | `CONVERT($(type), $(value))` |

The `MetaDataTypeConversion:implementation:sql-expression` instance references
`sqlserver:snippet:cast`. Each snippet owns two binding rows:

| Binding name | Referenced SqlConversionParameter ID |
| --- | --- |
| `value` | `MetaDataTypeConversion:parameter:source-expression` |
| `type` | `MetaDataTypeConversion:parameter:destination-type` |

The binding rows have natural snippet and parameter references. Their IDs are
`sqlserver:binding:cast-value`, `cast-type`, `convert-value` and `convert-type`
under the same `sqlserver:binding:` prefix. They and the literal expressions
are authored instance data in the sanctioned C# workspace. No Cast/Convert
selector or expression-registration branch remains.

Only MetaTransformPattern's `$(name)` syntax is reused. There are no Pattern,
PatternInstance or Weave dependencies. Placeholder names are case-insensitive,
allow surrounding whitespace, and support the established `$$(name)` escape.
Substitution is a single pass; inserted SQL is not rescanned. Missing, duplicate,
unused and unsupported bindings produce identified errors.

## Helper role and input preparation

The concrete `MetaDataTypeConversionService.GenerateSqlExpression` helper
instantiates an explicitly selected authored snippet. No pre-existing BI
operation calls it, and there is no new command or execution workflow for it.
The scalar-fragment parser, type formatter and SQL renderer are retained because
they directly support preparing bound SQL fragments and checking the resulting
metadata expression. No helper test is evidence of BI pipeline integration.

Only inputs required by the selected snippet's binding rows are resolved.
The source input parses as one supported scalar expression and is parenthesized
to preserve compound operands. The destination input resolves the mapped target
catalog type and explicit length/MAX, precision and scale only when bound.
Repeated bindings to one input reuse its prepared value. No type defaults or
implicit TRY conversions are introduced.

For example, with only a source-expression binding, this fixed-type snippet:

```sql
CONVERT(nvarchar(8), $(value), 112)
```

instantiates for `@source` as `CONVERT(nvarchar(8), (@source), 112)` without a
separate destination length, destination facets or target catalog entry.
A destination-only snippet likewise does not parse an unused source expression.

Changing authored `TransformScript` to:

```sql
COALESCE(CONVERT($(type), $(value), 112), N'unknown')
```

changes the instantiated expression, including composition and the fixed style
argument, without a C# conversion branch. An authored `CAST($(value) * 2 AS
$(type))` groups source `1 + 2` as `(1 + 2) * 2`; SQL Server execution returns 6.

## Verification

All scoped test projects are built before testing, using
`dotnet build <test-project> -c Release -m:1 -nr:false --nologo --verbosity quiet`.
The exact regression commands are:

```powershell
./eng/test.ps1 -Profile Unit -Configuration Release
dotnet test MetaDataTypeConversion/Tests/MetaDataTypeConversion.Tests.csproj -c Release --no-build --no-restore --nologo
dotnet test MetaTransform/Script/Tests/MetaTransformScript.Tests.csproj -c Release --no-build --no-restore --nologo --filter 'FullyQualifiedName~SqlCorpusExpressionRoundTripTests|FullyQualifiedName~TransformBindingTests|FullyQualifiedName~TransformBindingHardeningTests'
dotnet test MetaDataWarehouse/Tests/MetaDataWarehouse.Tests.csproj -c Release --no-build --no-restore --nologo --filter FullyQualifiedName~DataWarehouseToSqlConverterTests
dotnet test MetaDataVault/Tests/MetaDataVault.Tests.csproj -c Release --no-build --no-restore --nologo --filter FullyQualifiedName~ConvertToMetaSqlTests
dotnet test MetaPipeline/Tests/MetaPipeline.Tests.csproj -c Release --no-build --no-restore --nologo --filter FullyQualifiedName~SqlServerBulkCopyRowStreamWriterTests
git diff --exit-code 5cee491 -- MetaDataTypeConversion/Cli MetaDataType MetaConvert MetaPipeline
git diff --check
```

Conversion coverage includes source-only and destination-only preparation,
data-only changes, composed/fixed arguments, new snippet identities, binding
errors, repeated/escaped names, compound expressions and parameterized types.
XML/C# round trips retain natural snippet/binding references and instantiated
syntax. SQL Server coverage persists the model in a uniquely named disposable
database, executes helper-produced expressions, verifies strict failure and
precision/scale, and removes the database afterward. It is helper/surface
verification rather than a production-pipeline witness.

Limitations: no existing BI SQL value-conversion consumer was found or changed;
SQL syntax support remains bounded by MetaTransformScript. Persisted workspaces
require a modeled schema migration for the added snippet and binding entities.
Retained installations were not upgraded. Full repository Integration-profile
execution is not claimed by the focused commands above.
The existing bulk-copy tests selected above cover construction and input
validation; they are not live bulk-copy or CLR-coercion execution coverage.

Results on 2026-10-25: all six scoped test-project Release builds passed with
zero warnings/errors. Conversion passed 55 tests, unit passed 79 tests at 164 ms
summed execution, transform/binding passed 177 tests, warehouse passed 7 tests,
vault passed 29 tests and bulk-copy construction passed 2 tests. The first unit
run passed its assertions but exceeded the one-second timing budget at 3663 ms;
the unchanged gate passed on rerun. Actual SQL Server 2022 workspace persistence
and helper-expression execution passed; no disposable test databases remained.
Baseline CLI/consumer comparisons, the 69-mapping comparison, help/metadata
absence checks and both Git whitespace checks passed.
