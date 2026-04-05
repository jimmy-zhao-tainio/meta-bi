# MetaTransformScript Errata

Current hardening backlog captured from an `Unsupported` / `not supported` sweep across `MetaTransformScript`.

This note is for future parser-plumbing and hardening passes.

It is intentionally not the same thing as parser status:

- parser status says what is currently proven
- this note says where explicit gaps and hard-fail edges still exist

It should stay practical and current.

## Purpose

Use this note when:

- deciding which unsupported edges deserve first-class support next
- separating real feature gaps from integrity-guard exceptions

Do not treat every item here as equally urgent.

## Parser Gaps

These are explicit parser/import hard-fails on the current mainline code path.

- `CREATE VIEW` names with more than two identifier parts
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptSqlParser.Document.cs`
- parenthesized scalar expressions
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptSqlParser.Expressions.cs`
- unsupported `CROSS ...` table-reference forms beyond the currently handled ones
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptSqlParser.Table.cs`
- parenthesized table-reference forms beyond query-derived tables and inline `VALUES` tables
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptSqlParser.Table.cs`
- schema object names with more than two parts
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptSqlParser.Table.cs`
- some data type names in the special-expression parser
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptSqlParser.SpecialExpressions.cs`
- `GROUP BY ALL`
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptSqlParser.Query.cs`

## Import-Shaping Gaps

These are import-shaping limits in the current parser/import flow.

- bare `SELECT` file/folder import on `sql-path`
  - bare `SELECT` is currently accepted only on `sql-code` with an explicit name
- `CREATE VIEW ... WITH <view options>`
  - `MetaTransformScript/Sql/MetaTransformScriptSqlService.cs`
- `WITH CHECK OPTION`
  - `MetaTransformScript/Sql/MetaTransformScriptSqlService.cs`
- materialized-view syntax
  - `MetaTransformScript/Sql/MetaTransformScriptSqlService.cs`
- mixed bare `SELECT` and `CREATE VIEW` top-level shapes in one logical import source
  - `MetaTransformScript/Sql/MetaTransformScriptSqlService.cs`
- non-`SET` auxiliary batches before/around supported statements
  - `MetaTransformScript/Sql/MetaTransformScriptSqlService.cs`

## Emitter Hardening Gaps

These are real emitter-side support gaps rather than parser-only gaps.

- ODBC-escape `LIKE` predicates
  - `MetaTransformScript/Sql/MetaTransformScriptSqlEmitter.Boolean.cs`
- `GROUP BY ALL`
  - `MetaTransformScript/Sql/MetaTransformScriptSqlEmitter.Query.cs`
- distributed aggregation grouping specifications
  - `MetaTransformScript/Sql/MetaTransformScriptSqlEmitter.Query.cs`
- approximate `TOP`
  - `MetaTransformScript/Sql/MetaTransformScriptSqlEmitter.Query.cs`
- approximate `OFFSET/FETCH`
  - `MetaTransformScript/Sql/MetaTransformScriptSqlEmitter.Query.cs`
- `FunctionCall.WithArrayWrapper=true`
  - `MetaTransformScript/Sql/MetaTransformScriptSqlEmitter.Expressions.Calls.cs`
- `FunctionCall.UniqueRowFilter`
  - this is the important one for aggregate-level `DISTINCT`
  - `MetaTransformScript/Sql/MetaTransformScriptSqlEmitter.Expressions.Calls.cs`

## Integrity Guards

There are many emitter exceptions of the form:

- `Unsupported MetaTransformScript ... id ...`
- `Unsupported MetaTransformScript ... type ...`

Treat those primarily as integrity guards, not automatic backlog items.

They become real work only when:

- a supported corpus/example hits them
- or a new supported feature would require them to become reachable

## Suggested Priority

If work resumes on hardening, the practical order is:

1. Add aggregate-level `DISTINCT` support in parser and emitter.
2. Decide whether broader batch-envelope shapes beyond `SET` should be owned or explicitly rejected long-term.
3. Tackle remaining table-reference and scalar-expression gaps only when a supported example needs them.

## Not Errata

These were explicit concerns earlier, but they are now proven and should not be tracked as open errata:

- recursive named-column CTE shape
- numeric window frame delimiters
- true nested scalar subqueries inside subqueries
- grouping-set family support (`GROUPING SETS`, `ROLLUP`, `CUBE`)
- inline `VALUES` tables
- `TABLESAMPLE`
- `IS DISTINCT FROM`
- ordinary full-text predicates (`CONTAINS`, `FREETEXT`)
- signed, scientific, binary, and `NULL` literal forms used by the current corpus
- `PARSE(...)`, `TRY_PARSE(...)`, and `CURRENT_TIMESTAMP`
- `AT TIME ZONE`
- join-parenthesized table references
- `NEXT VALUE FOR` and global variables such as `@@SPID`
