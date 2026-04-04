# MetaTransformScript Owned Parser Status

Current status of the owned SQL parser front end for `MetaTransformScript`.

This note is intentionally narrow:

- what the owned parser currently handles
- what still routes through ScriptDOM temporarily
- what is explicitly unsupported right now

It should stay honest and small.

## Owned parser target

The owned parser targets the canonical `MetaTransformScript` model directly.

Current intended flow:

- `SQL text -> owned lexer/parser -> MetaTransformScript workspace model -> owned emitter -> semantically equivalent SQL`

Temporary migration reality:

- CLI/service import still routes through ScriptDOM in `MetaTransformScriptSqlService`
- ScriptDOM is still used as a comparison oracle in tests while the owned parser replaces the read path slice by slice

## Implemented owned-parser surface

The following are implemented through the owned parser and verified against the current importer on the reference corpus:

- `CREATE VIEW ... AS ...` envelope for supported body import
- bare `SELECT` view bodies
- `SELECT DISTINCT`
- simple select list
- `*` and qualified `alias.*`
- named table references and aliases
- `INNER`, `LEFT OUTER`, `RIGHT OUTER`, `FULL OUTER`, `CROSS JOIN`
- `CROSS APPLY`, `OUTER APPLY`
- `PIVOT`
- `UNPIVOT`
- derived tables
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
- `IS NULL`, `IS NOT NULL`
- scalar subqueries
- nested scalar subqueries inside subqueries
- `EXISTS`
- `ALL` / `ANY` subquery comparisons
- simple arithmetic with `+`
- simple `CASE`
- generic function calls
- `CHOOSE` as a generic function call
- `COALESCE`
- `NULLIF`
- `IIF`
- `CAST`
- `TRY_CAST`
- `CONVERT`
- `TRY_CONVERT`
- primary-expression `COLLATE`
- `WITH XMLNAMESPACES (...)` alias form
- `WITH XMLNAMESPACES (DEFAULT ...)`
- method-call targets such as `s.XmlPayload.value(...)`
- searched `CASE`
- `GROUP BY`
- `HAVING`
- `UNION`, `UNION ALL`, `EXCEPT`, `INTERSECT`
- query-parenthesized set-op inputs such as `(SELECT ...) UNION ALL (SELECT ...)`
- `TOP`, `PERCENT`, `WITH TIES`
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

## Verified corpus files

Currently verified against the ScriptDOM importer oracle:

- `001_basic_select.sql`
- `002_select_star.sql`
- `003_join_variants.sql`
- `004_apply_sources.sql`
- `005_pivot.sql`
- `006_unpivot.sql`
- `007_where_predicates.sql`
- `008_group_by_having.sql`
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
- `024_query_parentheses.sql`
- `041_xml_namespaces_default.sql`
- `042_cte_column_list.sql`
- `043_recursive_cte_column_list.sql`
- `044_window_frame_offsets.sql`
- `045_nested_subqueries.sql`

## Explicitly unsupported right now

These are rejected explicitly by the owned parser rather than guessed:

- `CREATE VIEW` column lists
- `GROUP BY ALL`
- unsupported parenthesized table-reference forms outside the currently supported derived-table shape
- unsupported or malformed `GO`-batch input in the owned parser path

## Known temporary gap

- The public import/read path still uses ScriptDOM.
- The owned parser is verified through tests, but it is not yet the CLI/service import implementation.
