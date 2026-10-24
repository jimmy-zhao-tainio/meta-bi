# Modeled SQL value conversion

MetaDataTypeConversion owns the standard target representation selected by a
mapping and the authored implementation used when a caller explicitly requests
a value expression for that mapping. There remains exactly one mapping for a
source data type and target type system. A compatibility path is evidence for
validation, never an executable or lossless conversion plan.

## Model and generation boundary

`ConversionImplementation.SqlConversionTemplate` is an optional natural object
reference to a reusable `SqlConversionTemplate`. A template has an identity, an
explicit `Dialect` (`SqlServer`) and `Function` (`Cast` or `Convert`). These are
closed supported domain values, not SQL fragments or a placeholder language.
Both supported functions have strict SQL Server failure semantics. TRY
functions, PARSE, style and culture inputs are unsupported and are not silently
substituted. Additional implementations can reference the same template.

The fixed template operands are the input scalar expression and the selected
mapping's target type with explicit facets. `SqlValueConversionRequest` supplies
the expression, source type, target system and `SqlServerTypeFacets`. The
MetaDataType catalog supplies the selected target type's name and system; the
caller cannot override it with an unrelated destination type. Length/MAX,
precision and scale are typed API values, not encoded templates. No parameter
defaults are invented: length types require a length or supported MAX; decimal
types require precision and scale; temporal precision types require scale;
float requires precision. Inapplicable, missing and out-of-range facets fail.
Callers supply facets appropriate to their intended representation; generation
does not infer capacity from data or prove that conversion is lossless.

`MetaDataTypeConversionService.GenerateSqlExpression` selects exactly one
mapping and interprets its implementation. Its result includes the selected
mapping and a `SqlScalarExpression` with a natural root reference and its
`MetaTransformScriptModel`. MetaTransformScript owns scalar parsing, existing
CAST/CONVERT call nodes, parameterized type references and SQL rendering. No
parallel expression AST or text substitution engine was introduced. Scalar
fragment parsing requires the end of input, so aliases, query tails, multiple
expressions and additional statements are rejected.

`Direct` remains a pass-through: choosing `meta:String -> nvarchar` for physical
storage does not itself insert a runtime cast. `Structural` has no scalar SQL
fallback and fails explicit value generation. Neither implementation may be
given a SQL template. Other implementations without a template, dangling
templates, unsupported functions and dialects produce identified diagnostics.
Mapping-only resolution does not require an executable SQL implementation.

## Production consumers

The production value-generation entry point is the modeled
`meta-data-type-conversion sql-expression` command. It calls the owner service
and displays the generated SQL through the normal presenter. The public service
also returns the syntax graph for callers that need structured composition.

The existing DataWarehouseToSql, RawDataVaultToSql and BusinessDataVaultToSql
converters execute their packaged weaves. They use `dataTypes` and
`typeConversions` to choose physical column types and generate DDL; none requests
a mapped runtime value expression. Their physical mapping behavior is preserved.
SchemaToRawDataVault uses physical type mapping as well. The existing transform
pattern weave substitutes explicitly authored pattern values. TransformScriptToSql
and the MetaTransformScript emitter render authored CAST/CONVERT syntax; that
syntax ownership is appropriate. Binding continues to record compatibility
evidence without changing authored SQL or inserting conversion calls.

## Concrete example and authoring

The built-in mapping
`MetaDataTypeConversion:mapping:sqlserver:int-to-nvarchar` selects
`sqlserver:type:nvarchar`. It now references implementation `SqlCast`, whose
template is `sqlserver:template:strict-cast` (`SqlServer`, `Cast`).

From the repository root, after building the CLI:

```powershell
& ./MetaDataTypeConversion/Cli/bin/Release/net8.0/meta-data-type-conversion.exe sql-expression `
  --workspace ./MetaDataTypeConversion/Workspace `
  --source-data-type sqlserver:type:int --target-data-type-system SqlServer `
  --input-expression '([s].[Value] + 2) * 3' --length 24
```

The result is `CAST(([s].[Value] + 2) * 3 AS nvarchar(24))`. Selecting the other
authored template on the implementation changes the result to
`CONVERT(nvarchar(24), ([s].[Value] + 2) * 3)` without changing generator code:

```powershell
meta instance relationship set ConversionImplementation `
  MetaDataTypeConversion:implementation:sql-cast `
  --to SqlConversionTemplate sqlserver:template:strict-convert `
  --workspace <your-conversion-workspace>
```

Inspect the installed `meta ... --help` before authoring. The checked-in C#
workspace and MetaCli command instances were regenerated with the modeled CLIs;
generator-assigned local variable renumbering is incidental to that projection.

## Compatibility and migration

Mapping cardinality, targets and compatibility traversal are unchanged. The
existing `int-to-nvarchar` mapping deliberately changes implementation from
`Direct` to `SqlCast`, because this same-system text-normalization mapping now
has an explicit value-generation realization. `Resolve` reports that new
implementation; no resolution path changes. All other built-in mappings retain
their implementation references. Old authored workspaces retain their own
implementation choices rather than receiving an implicit CAST fallback.

The model adds one entity and one optional relationship. Existing persisted
workspaces need their model contract updated before loading through the new
typed model. Regenerate disposable workspaces with `create`. For retained
authored workspaces, use `meta model add-entity SqlConversionTemplate`, add the
required `Dialect` and `Function` properties, and add the optional
`ConversionImplementation -> SqlConversionTemplate` relationship through the
modeled CLI. Then author template/implementation instances and explicitly
select affected mappings. Apply schema changes to disposable SQL resources
before retained installations. Do not hand-edit generated model files.

## Acceptance evidence

`SqlValueConversionTests` covers authored-template changes, compound expressions,
parameterized target types, strict-function rejection, missing implementations,
unsupported dialects, no multi-hop execution and no default facets.
`SqlValueConversionWorkspaceTests` round-trips XML and C# template relationships
and generated syntax, then invokes the production CLI before and after an
authored template change. `SqlValueConversionSqlServerTests` exercises the SQL
workspace surface in a uniquely named disposable database and executes generated
SQL, checks decimal precision/scale and verifies strict conversion failure. Its
test-only model name is changed to match the disposable SQL database and restored
on materialization for typed loading, as required by the foundation's SQL surface.

Service regression tests retain mapping/compatibility coverage and explicitly
reject competing mappings to the same system. The numeric-to-Unicode binding
test also proves validation leaves stored SQL unchanged and adds no CAST or
CONVERT nodes.

## Verification commands

Run from the repository root with the .NET SDK on PATH. Release builds completed
with zero warnings and errors for these projects:

```powershell
$projects = @(
  'MetaDataTypeConversion/Tests/MetaDataTypeConversion.Tests.csproj',
  'Tests/Unit/MetaBi.Unit.Tests.csproj',
  'MetaTransform/Script/Tests/MetaTransformScript.Tests.csproj',
  'MetaDataWarehouse/Tests/MetaDataWarehouse.Tests.csproj',
  'MetaDataVault/Tests/MetaDataVault.Tests.csproj'
)
foreach ($project in $projects) {
  dotnet build $project -c Release -m:1 -nr:false --nologo --verbosity quiet
  if ($LASTEXITCODE -ne 0) { throw "Build failed: $project" }
}
```

The exact focused checks are:

```powershell
./eng/test.ps1 -Profile Unit -Configuration Release
dotnet test MetaDataTypeConversion/Tests/MetaDataTypeConversion.Tests.csproj -c Release --no-build --no-restore --nologo
dotnet test MetaTransform/Script/Tests/MetaTransformScript.Tests.csproj -c Release --no-build --no-restore --nologo --filter 'FullyQualifiedName~SqlCorpusExpressionRoundTripTests|FullyQualifiedName~TransformBindingTests|FullyQualifiedName~TransformBindingHardeningTests|FullyQualifiedName~TransformScriptToSqlWeaveTests'
dotnet test MetaDataWarehouse/Tests/MetaDataWarehouse.Tests.csproj -c Release --no-build --no-restore --nologo --filter FullyQualifiedName~DataWarehouseToSqlConverterTests
dotnet test MetaDataVault/Tests/MetaDataVault.Tests.csproj -c Release --no-build --no-restore --nologo --filter FullyQualifiedName~ConvertToMetaSqlTests
git diff --check
git diff --cached --check
```

On 2026-10-24 the final unit gate passed all 78 tests at 855 ms summed test
execution. Earlier cold executions exceeded its one-second timing budget; the
gate and budget were retained unchanged. Conversion passed all 46 tests,
including actual SQL Server 2022 persistence and execution. The focused transform
suite passed 210 tests, the warehouse suite passed 7 tests and the vault suite
passed 29 tests. Both Git whitespace checks passed. Full repository
Integration-profile execution was not run; the commands above are the scoped
regression evidence.
