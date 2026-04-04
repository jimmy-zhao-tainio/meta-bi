# MetaTransformScript Errata

Current hardening backlog captured from an `Unsupported` / `not supported` sweep across `MetaTransformScript`.

This note is for future parser-plumbing and hardening passes.

It is intentionally not the same thing as parser status:

- parser status says what is currently proven
- this note says where explicit gaps and hard-fail edges still exist

It should stay practical and current.

## Purpose

Use this note when:

- replacing the public read/import path with the owned parser
- deciding which unsupported edges deserve first-class support next
- separating real feature gaps from integrity-guard exceptions

Do not treat every item here as equally urgent.

## Owned Parser Gaps

These are explicit owned-parser hard-fails on the current mainline code path.

- `CREATE VIEW` column lists
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptOwnedSqlParser.Document.cs`
- `CREATE VIEW` names with more than two identifier parts
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptOwnedSqlParser.Document.cs`
- `GO`-separated batches in the owned parser path
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptOwnedSqlParser.Infrastructure.cs`
- parenthesized scalar expressions
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptOwnedSqlParser.Expressions.cs`
- unsupported `CROSS ...` table-reference forms beyond the currently handled ones
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptOwnedSqlParser.Table.cs`
- parenthesized table-reference forms beyond query-derived tables
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptOwnedSqlParser.Table.cs`
- schema object names with more than two parts
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptOwnedSqlParser.Table.cs`
- some data type names in the special-expression parser
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptOwnedSqlParser.SpecialExpressions.cs`
- `MAX` data type parameters
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptOwnedSqlParser.SpecialExpressions.cs`
- `GROUP BY ALL`
  - `MetaTransformScript/Sql/Parsing/MetaTransformScriptOwnedSqlParser.Query.cs`

## Public Import / Wrapper Gaps

These are still explicitly unsupported in the ScriptDOM-backed service import path.

- `CREATE VIEW ... WITH <view options>`
  - `MetaTransformScript/Sql/MetaTransformScriptSqlService.cs`
- `WITH CHECK OPTION`
  - `MetaTransformScript/Sql/MetaTransformScriptSqlService.cs`
- materialized-view syntax
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

1. Replace the public import/read path with the owned parser.
2. Add aggregate-level `DISTINCT` support in parser and emitter.
3. Add owned-parser support for `GO` batches and wrapper shapes already tolerated by the service path.
4. Decide whether `CREATE VIEW` column lists belong in the owned parser boundary or remain wrapper-only.
5. Tackle remaining table-reference and scalar-expression gaps only when a supported example needs them.

## Not Errata

These were explicit concerns earlier, but they are now proven and should not be tracked as open errata:

- recursive named-column CTE shape
- numeric window frame delimiters
- true nested scalar subqueries inside subqueries
