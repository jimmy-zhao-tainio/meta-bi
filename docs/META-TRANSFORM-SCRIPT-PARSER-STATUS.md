# MetaTransformScript Parser Status

Current status of the SQL parser front end for `MetaTransformScript`.

This note is intentionally narrow:

- what the parser currently handles
- what the current verified corpus coverage is
- what the current integration proof status is
- what is explicitly unsupported right now

It should stay honest and small.

Remaining gaps are now ordinary parser, emitter, model, or import-shaping errata. They are not hidden parser delegation.

## Parser target

The parser targets the canonical `MetaTransformScript` model directly.

Current intended flow:

- `SQL text -> lexer/parser -> MetaTransformScript workspace model -> emitter -> semantically equivalent SQL`

Current implementation reality:

- `MetaTransformScript` no longer uses ScriptDOM in its import/read path
- `MetaTransformScript` no longer has a ScriptDOM package reference
- tests no longer depend on a second parser implementation as the product read path

## Implemented parser surface

The following are implemented in the parser and exercised through the current tests and service path:

- `CREATE VIEW ... AS ...` envelope for supported body import
- `CREATE VIEW` column lists
- bare `SELECT` bodies when an explicit script name is supplied
- `SET`-only batches on import
- `GO`-split import batches
- `SELECT DISTINCT`
- simple select list
- `*` and qualified `alias.*`
- named table references and aliases
- `INNER`, `LEFT OUTER`, `RIGHT OUTER`, `FULL OUTER`, `CROSS JOIN`
- `CROSS APPLY`, `OUTER APPLY`
- `PIVOT`
- `UNPIVOT`
- derived tables
- inline `VALUES` tables
- join-parenthesized table references
- common table expressions
- CTE column lists
- recursive CTE shape when the member queries stay inside the current supported query surface
- `WHERE`
- boolean combinations: `AND`, `OR`, `NOT`, parentheses
- comparisons
- `BETWEEN`
- `IN (...)`
- `IN (subquery)`
- `LIKE`
- `IS DISTINCT FROM`
- `IS NULL`, `IS NOT NULL`
- scalar subqueries
- nested scalar subqueries inside subqueries
- `EXISTS`
- `ALL` / `ANY` subquery comparisons
- `CONTAINS`
- `FREETEXT`
- simple arithmetic with `+`
- unary `+` and `-`
- simple `CASE`
- generic function calls
- `CHOOSE` as a generic function call
- `PARSE`
- `TRY_PARSE`
- `COALESCE`
- `NULLIF`
- `IIF`
- `CAST`
- `TRY_CAST`
- `CONVERT`
- `TRY_CONVERT`
- `CURRENT_TIMESTAMP`
- `NEXT VALUE FOR`
- global variables such as `@@SPID`
- signed, scientific, binary, and `NULL` literal forms used by the current corpus
- primary-expression `COLLATE`
- `AT TIME ZONE`
- `WITH XMLNAMESPACES (...)` alias form
- `WITH XMLNAMESPACES (DEFAULT ...)`
- method-call targets such as `s.XmlPayload.value(...)`
- searched `CASE`
- `GROUP BY`
- `GROUPING SETS`
- `ROLLUP`
- `CUBE`
- `HAVING`
- `UNION`, `UNION ALL`, `EXCEPT`, `INTERSECT`
- query-parenthesized set-op inputs such as `(SELECT ...) UNION ALL (SELECT ...)`
- `TOP`, `PERCENT`, `WITH TIES`
- `TABLESAMPLE`
- query-level `ORDER BY`
- `OFFSET ... ROWS`
- `FETCH NEXT/FIRST ... ROWS ONLY`
- window `OVER (...)`
- named `WINDOW` clause definitions
- `PARTITION BY`
- window `ORDER BY`
- current frame forms used by the corpus:
  - `ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW`
- numeric delimiters such as `ROWS 2 PRECEDING`
- numeric BETWEEN forms such as `ROWS BETWEEN 1 PRECEDING AND 1 FOLLOWING`
- simple named-window references

## Verified corpus coverage

Currently covered by parser/emitter round-trip tests:

- `001_basic_select.sql`
- `002_select_star.sql`
- `003_join_variants.sql`
- `004_apply_sources.sql`
- `005_pivot.sql`
- `006_unpivot.sql`
- `007_where_predicates.sql`
- `008_group_by_having.sql`
- `009_grouping_sets.sql`
- `010_rollup_cube.sql`
- `011_subqueries_and_correlation.sql`
- `012_subquery_predicates.sql`
- `013_set_operations.sql`
- `014_value_expressions.sql`
- `015_window_functions.sql`
- `016_named_window.sql`
- `017_cte.sql`
- `018_ordering_and_top.sql`
- `019_offset_fetch.sql`
- `020_xml_namespaces_and_methods.sql`
- `021_inline_values.sql`
- `023_table_sample.sql`
- `040_view_column_list.sql`
- `024_query_parentheses.sql`
- `025_distinct_predicate.sql`
- `027_fulltext.sql`
- `029_literals_and_special_calls.sql`
- `030_time_zone_extract.sql`
- `031_join_parentheses.sql`
- `036_sequence_and_globals.sql`
- `041_xml_namespaces_default.sql`
- `042_cte_column_list.sql`
- `043_recursive_cte_column_list.sql`
- `044_window_frame_offsets.sql`
- `045_nested_subqueries.sql`

## Current integration proof status

- `dotnet test MetaTransformScript\Tests\MetaTransformScript.Tests.csproj` currently passes
- the smaller CLI integration demo currently passes and `meta instance diff` reports no differences
- the larger reference-corpus CLI demo currently passes for the supported 32-script demo set
- `SQL -> workspace -> SQL -> workspace` is currently stable on that demo set, and `meta instance diff` reports no differences

## Explicitly unsupported right now

These are rejected explicitly rather than guessed:

- bare `SELECT` file/folder import on `sql-path` without `CREATE VIEW` wrappers
- `GROUP BY ALL`
- unsupported parenthesized table-reference forms outside the currently supported derived-table shape
- unsupported parenthesized scalar expressions
- unsupported or malformed non-`SET` auxiliary batches in the import path
