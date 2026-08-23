# MetaTransformScript Status

Remaining gaps are ordinary parser, emitter, model, or import-shaping errata. They are not hidden parser delegation.

## Implemented And Verified

- [x] ScriptDOM removed from `MetaTransformScript`
- [x] SQL import/read path uses the `MetaTransformScript` parser
- [x] SQL emit path uses the `MetaTransformScript` emitter
- [x] `SQL -> workspace -> SQL -> workspace` is stable on the current supported demo set
- [x] `meta instance diff` reports no differences on the current supported demo set
- [x] `dotnet test MetaTransform\Script\Tests\MetaTransformScript.Tests.csproj` passes

- [x] `CREATE VIEW ... AS ...` envelope
- [x] inline `CREATE FUNCTION ... RETURNS TABLE ... AS RETURN (...)` envelope
- [x] scalar `CREATE FUNCTION` envelope for bodies reducible to one `RETURN` scalar expression or scalar `RETURN SELECT`/subquery expression
- [x] `CREATE VIEW` column lists
- [x] `SET`-only batches on import
- [x] `GO`-split import batches
- [x] unsupported `CREATE VIEW` wrapper syntax is rejected explicitly
- [x] unsupported non-`SET` auxiliary batches are rejected explicitly
- [x] bare `SELECT` import through `sql-code` with explicit name
- [x] generalized `TransformScript` root link to modeled `TSqlStatement`
- [x] bare mutation statement import with explicit name for `sql-code`
- [x] bare mutation statement import from `sql-file` with file-stem script name
- [x] bounded `INSERT` syntax:
  - `INSERT INTO <target> (<columns>) SELECT ...`
  - `INSERT INTO <target> (<columns>) VALUES (...)`
- [x] bounded `UPDATE` syntax:
  - target object and optional alias
  - `SET` assignments
  - optional `FROM`
  - optional `WHERE`
- [x] bounded `DELETE` syntax:
  - target object/alias
  - optional joined `FROM`
  - optional `WHERE`
- [x] bounded `TRUNCATE TABLE`
- [x] bounded `MERGE` syntax:
  - required SQL Server terminating semicolon
  - optional CTE prefix
  - `TOP (expression) [PERCENT]`
  - target table `WITH (...)` hints for currently modeled hint forms
  - target object and optional alias
  - table-reference `USING`
  - `ON` predicate
  - `WHEN MATCHED [AND ...]` update/delete
  - repeated `WHEN MATCHED` and `WHEN NOT MATCHED BY SOURCE` clauses under SQL Server's two-clause/update-delete constraints
  - `WHEN NOT MATCHED [BY TARGET] [AND ...]` insert
  - `WHEN NOT MATCHED BY SOURCE [AND ...]` update/delete
  - `OUTPUT` with output select elements and optional `INTO`
  - `OPTION (...)` query hints for currently modeled hint forms

- [x] `SELECT DISTINCT`
- [x] aggregate-level `DISTINCT` such as `COUNT(DISTINCT ...)`
- [x] `TOP`, `PERCENT`, `WITH TIES`
- [x] query-level `ORDER BY`
- [x] `OFFSET/FETCH`
- [x] set operations: `UNION`, `UNION ALL`, `EXCEPT`, `INTERSECT`
- [x] query parentheses

- [x] named table references and aliases
- [x] SQL Server named-table `WITH (...)` hints in `FROM`/`JOIN` sources (modeled via `SqlHint`)
- [x] joins: `INNER`, `LEFT`, `RIGHT`, `FULL`, `CROSS`
- [x] `CROSS APPLY`, `OUTER APPLY`
- [x] derived tables
- [x] inline `VALUES` tables
- [x] join-parenthesized table references
- [x] `PIVOT`
- [x] `UNPIVOT`
- [x] `TABLESAMPLE`

- [x] common table expressions
- [x] CTE column lists
- [x] recursive CTE shape inside the current supported query surface

- [x] comparisons
- [x] SQL Server not-equal operators `<>` and `!=` (emitted canonically as `<>`)
- [x] `BETWEEN`
- [x] `NOT BETWEEN`
- [x] `IN (...)`
- [x] `NOT IN (...)`
- [x] `IN (subquery)`
- [x] `NOT IN (subquery)`
- [x] `LIKE`
- [x] `NOT LIKE`
- [x] `LIKE ... ESCAPE ...`
- [x] `IS NULL`
- [x] `IS NOT NULL`
- [x] `IS DISTINCT FROM`
- [x] `EXISTS`
- [x] `ALL` / `ANY` subquery comparisons
- [x] boolean `AND` / `OR` / `NOT`
- [x] boolean parentheses
- [x] full-text predicates: `CONTAINS`, `FREETEXT`

- [x] scalar subqueries
- [x] nested scalar subqueries
- [x] parenthesized scalar expressions
- [x] unary `+` / `-`
- [x] simple arithmetic with `+`
- [x] arithmetic with `-`, `*`, `/`, `%`
- [x] `CASE`
- [x] `COALESCE`
- [x] `NULLIF`
- [x] `IIF`
- [x] `LEFT`
- [x] `RIGHT`
- [x] generic function calls
- [x] table-valued function references with optional aliases
- [x] `PARSE`
- [x] `TRY_PARSE`
- [x] `CAST`
- [x] `TRY_CAST`
- [x] `CONVERT`
- [x] `TRY_CONVERT`
- [x] SQL Server national string literals such as `N'...'`
- [x] `CURRENT_TIMESTAMP`
- [x] `NEXT VALUE FOR`
- [x] global variables such as `@@SPID`
- [x] signed literals used by the corpus
- [x] scientific notation literals used by the corpus
- [x] leading-dot numeric and scientific literals such as `.5`, `.5E2`, and `.5e-2`
- [x] binary literals used by the corpus
- [x] `NULL` literal expressions
- [x] primary-expression `COLLATE`
- [x] `AT TIME ZONE`

- [x] MetaDataType sanctioned SQL Server types used by the current parser/emitter type path
- [x] SQL Server `TIMESTAMP`/`ROWVERSION` type aliases (modeled canonically as `binary(8)`)
- [x] SQL Server single-quoted select aliases (emitted canonically as identifier aliases)

- [x] `GROUP BY`
- [x] `GROUPING SETS`
- [x] `ROLLUP`
- [x] `CUBE`
- [x] `HAVING`
- [x] `GROUP BY ALL`

- [x] window `OVER (...)`
- [x] named `WINDOW` clause
- [x] `PARTITION BY`
- [x] window `ORDER BY`
- [x] numeric window frame delimiters used by the current corpus
- [x] analytic/window functions such as `FIRST_VALUE`, `LAST_VALUE`, `CUME_DIST`, and `PERCENT_RANK`
- [x] percentile analytic/window functions with `WITHIN GROUP` such as `PERCENTILE_CONT` and `PERCENTILE_DISC`

- [x] `WITH XMLNAMESPACES (...)`
- [x] `WITH XMLNAMESPACES (DEFAULT ...)`
- [x] XML method-call targets such as `.value(...)`, `.query(...)`, `.exist(...)`
- [x] XML `nodes(...)` table sources

- [x] schema object names with more than two parts
- [x] reference corpus coverage through `051_cross_database_names.sql`
- [x] reference corpus coverage for `052_arithmetic_operators.sql`
- [x] reference corpus coverage for `053_negated_predicates.sql`
- [x] reference corpus coverage for `054_like_escape.sql`
- [x] reference corpus coverage for `055_xml_nodes.sql`
- [x] reference corpus coverage for `056_analytic_window_functions.sql`
- [x] reference corpus coverage for `057_percentile_within_group.sql`

## Mainline Support Gaps We Likely Should Close

- [ ] distributed aggregation grouping specifications in emitter
- [ ] advanced SQL Server `MERGE` hint/query-option forms beyond the current modeled `SqlHint` shape, kept as modeled `MetaTransformScript` syntax rather than a pipeline-owned write primitive
- [ ] semantic validation that `WHEN NOT MATCHED BY SOURCE` actions do not reference source aliases is not implemented; the parser models the search/action expressions structurally but does not resolve source-vs-target column ownership
- [ ] broader non-MERGE mutation statement options such as `OUTPUT`, `TOP`, variable assignment update forms, and `DEFAULT VALUES`
- [ ] expression-lowerable procedural scalar UDF bodies, for example local `DECLARE`, `SET`, `IF ... SET`, and `RETURN` lowered into one deterministic scalar expression graph

## Deliberate Non-Support

- [x] bare `SELECT` import without explicit `--name` stays unsupported
- [x] mixed bare `SELECT` and `CREATE VIEW` shapes in one logical import source stay unsupported
- [x] non-`SET` auxiliary batches before/around supported statements stay unsupported
- [x] ODBC-escape `LIKE` predicates stay unsupported
- [x] arbitrary `CREATE VIEW ... WITH <view options>` stays unsupported unless explicitly whitelisted
- [x] `WITH CHECK OPTION` stays unsupported unless explicitly justified
- [x] materialized-view syntax stays unsupported
- [x] multistatement table-valued `CREATE FUNCTION` wrappers stay unsupported
- [x] procedural scalar UDF bodies stay unsupported unless they are added as a generic expression-lowering subset

## Support Only If Justified By Real Use Case

- [ ] broader unsupported `CROSS ...` table-reference forms
- [ ] parenthesized table-reference forms beyond the current supported shapes
- [ ] `FunctionCall.WithArrayWrapper=true`
