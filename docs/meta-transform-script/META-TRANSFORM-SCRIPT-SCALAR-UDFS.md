# MetaTransformScript Scalar UDF Support

`MetaTransformScript` supports scalar SQL Server UDFs only when the body can be represented as modeled syntax, not as an opaque module blob.

## Current Supported Surface

Supported scalar function wrappers:

```sql
CREATE FUNCTION dbo.fnExample
(
    @value int
)
RETURNS int
AS
BEGIN
    RETURN @value + 1;
END
GO
```

The importer models:

- function identity through `TransformScript` plus `ScriptObjectScalarFunction`
- schema and object identifiers
- ordered parameters and parameter data types
- scalar return data type
- one body expression through the existing `ScalarExpression` graph

The supported body must reduce to one scalar result:

- `AS RETURN <scalar expression>`
- `AS BEGIN RETURN <scalar expression>; END`
- scalar `RETURN SELECT ...` / scalar subquery shapes supported by the current expression parser

Export emits a deterministic canonical function wrapper. It does not preserve original whitespace, comments, or function-body formatting.

## Binding Behavior

Scalar function definitions are not rowsets, so binding a transform workspace does not try to infer output columns for the function definition itself.
The binding workspace keeps a `TransformBinding` row for the scalar function identity, but does not add source/target rowsets or binding targets for that definition.

Scalar function call sites inside views and statements still bind their argument expressions. For example, `dbo.fnTidBK(s.CreatedAt)` binds `s.CreatedAt` as a source column argument and does not treat `dbo` as a column target.
When a call resolves to a modeled scalar function in the same transform workspace, binding also walks the function return expression so table reads inside a supported scalar subquery body become visible to downstream dependency analysis.

## Pipeline and Orchestration Behavior

Scalar function definitions are helper objects, not pipeline transform steps.
`MetaPipeline` rejects a selected scalar function definition as non-executable, and `MetaOrchestration` records a blocking `NonExecutableTransformScript` issue if an existing pipeline workspace already references one directly.

Views or mutation statements that call scalar functions remain normal pipeline/orchestration inputs. Their dependency profile comes from the bound statement, bound call arguments, and same-workspace scalar function return-expression body sources.

## Explicit Non-Support

These shapes currently fail as `UnsupportedFunctionWrapper` so corpus import loops can skip/report them separately from parser bugs:

- multistatement table-valued functions such as `RETURNS @Output TABLE`
- procedural scalar UDF bodies with local control flow or assignment statements such as `DECLARE`, `SET`, and `IF`
- function options not deliberately modeled by the importer

## fnTidBK-Style Stretch

The observed `dbo.fnTidBK` body is not impossible, but it should be added as a generic expression-lowering subset, not as a special case for that function name or file.

A defensible subset would accept only side-effect-free local scalar bodies:

- `DECLARE @name <type>`
- `SET @name = <scalar expression>`
- `IF <boolean expression> SET @name = <scalar expression>`
- `RETURN <scalar expression>`

That subset can lower local variable state into one expression graph. For the observed pattern:

```sql
SET @seconds = <calc>;
IF @seconds = 0
    SET @seconds = 86400;
RETURN CONVERT(VARCHAR(25), @seconds);
```

the modeled result can be equivalent to:

```sql
RETURN CONVERT(VARCHAR(25), CASE WHEN <calc> = 0 THEN 86400 ELSE <calc> END);
```

That is a small compiler-style lowering pass, but it needs expression cloning/substitution so generated model rows are owned once and remain deterministic. It should still reject loops, cursors, dynamic SQL, table access, multi-branch procedural programs, and side-effecting statements until those constructs have an explicit model boundary.
