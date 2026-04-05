# MetaTransformScript Status

- [x] ScriptDOM removed from `MetaTransformScript`
- [x] SQL import/read path uses the `MetaTransformScript` parser
- [x] SQL emit path uses the `MetaTransformScript` emitter
- [x] `SQL -> workspace -> SQL -> workspace` is stable on the current supported demo set
- [x] `meta instance diff` reports no differences on the current supported demo set
- [x] `dotnet test MetaTransformScript\Tests\MetaTransformScript.Tests.csproj` passes

- [x] `CREATE VIEW ... AS ...` envelope
- [x] `CREATE VIEW` column lists
- [x] `SET`-only batches on import
- [x] `GO`-split import batches
- [x] bare `SELECT` import through `sql-code` with explicit name
- [ ] bare `SELECT` file/folder import through `sql-path`

- [x] `SELECT DISTINCT`
- [x] aggregate-level `DISTINCT` such as `COUNT(DISTINCT ...)`
- [x] `TOP`, `PERCENT`, `WITH TIES`
- [x] query-level `ORDER BY`
- [x] `OFFSET/FETCH`
- [x] set operations: `UNION`, `UNION ALL`, `EXCEPT`, `INTERSECT`
- [x] query parentheses

- [x] named table references and aliases
- [x] joins: `INNER`, `LEFT`, `RIGHT`, `FULL`, `CROSS`
- [x] `CROSS APPLY`, `OUTER APPLY`
- [x] derived tables
- [x] inline `VALUES` tables
- [x] join-parenthesized table references
- [x] `PIVOT`
- [x] `UNPIVOT`
- [x] `TABLESAMPLE`
- [ ] broader unsupported `CROSS ...` table-reference forms
- [ ] parenthesized table-reference forms beyond the current supported shapes

- [x] common table expressions
- [x] CTE column lists
- [x] recursive CTE shape inside the current supported query surface

- [x] comparisons
- [x] `BETWEEN`
- [x] `IN (...)`
- [x] `IN (subquery)`
- [x] `LIKE`
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
- [x] `CASE`
- [x] `COALESCE`
- [x] `NULLIF`
- [x] `IIF`
- [x] generic function calls
- [x] `PARSE`
- [x] `TRY_PARSE`
- [x] `CAST`
- [x] `TRY_CAST`
- [x] `CONVERT`
- [x] `TRY_CONVERT`
- [x] `CURRENT_TIMESTAMP`
- [x] `NEXT VALUE FOR`
- [x] global variables such as `@@SPID`
- [x] signed literals used by the corpus
- [x] scientific notation literals used by the corpus
- [x] binary literals used by the corpus
- [x] `NULL` literal expressions
- [x] primary-expression `COLLATE`
- [x] `AT TIME ZONE`
- [ ] broader scalar-expression hardening beyond the current supported corpus

- [x] `GROUP BY`
- [x] `GROUPING SETS`
- [x] `ROLLUP`
- [x] `CUBE`
- [x] `HAVING`
- [x] `GROUP BY ALL`
- [ ] distributed aggregation grouping specifications in emitter

- [x] window `OVER (...)`
- [x] named `WINDOW` clause
- [x] `PARTITION BY`
- [x] window `ORDER BY`
- [x] numeric window frame delimiters used by the current corpus

- [x] `WITH XMLNAMESPACES (...)`
- [x] `WITH XMLNAMESPACES (DEFAULT ...)`
- [x] XML method-call targets such as `.value(...)`, `.query(...)`, `.exist(...)`

- [x] reference corpus coverage through `045_nested_subqueries.sql`
- [x] reference corpus coverage for `046_aggregate_distinct.sql`
- [x] reference corpus coverage for `047_parenthesized_scalar_expressions.sql`
- [x] reference corpus coverage for `048_group_by_all.sql`

- [ ] `CREATE VIEW` names with more than two identifier parts
- [ ] schema object names with more than two parts
- [ ] broader data type names beyond the current supported set
- [ ] `CREATE VIEW ... WITH <view options>`
- [ ] `WITH CHECK OPTION`
- [ ] materialized-view syntax
- [ ] mixed bare `SELECT` and `CREATE VIEW` shapes in one logical import source
- [ ] non-`SET` auxiliary batches before/around supported statements
- [ ] ODBC-escape `LIKE` predicates
- [ ] `FunctionCall.WithArrayWrapper=true`
