# Transform Binding Coverage

Seeded from [VIEW_SURFACE_REFERENCE.md](../../Script/Reference/VIEW_SURFACE_REFERENCE.md), but organized around binding concerns rather than syntax inventory.

Status meanings:
- `Supported` = binding behavior is implemented for the current sanctioned shapes
- `Partial` = binding works, but with important limits that should be called out explicitly
- `Deferred` = modeled syntax exists, but binding coverage is not claimed yet
- `Not applicable` = not a standalone binding concern in this tracker

Name resolution and expression traversal are distinct from result-type validation.
Schema-aware Binding requires proven result types for every written output. An
expression can be traversable while its result typing is unsupported; validation
then rejects it with `TargetWriteValueTypeNotResolved` (or the mutation binding
diagnostic `MutationWriteValueTypeNotResolved`). The destination never supplies
the missing expression type.

## SQL Server result contracts

The selected MetaDataType catalog resolves both source types and expression
result types through modeled SQL Server membership. SQL type names select
catalog instances; IDs may be arbitrary. These are SQL Server language rules,
independent of authored source-to-destination conversion policy.

- Numeric arithmetic applies SQL Server precedence and decimal precision/scale
  rules, including the precision-38 scale reduction. Integer constants use their
  minimum decimal precision in decimal expressions. Constants beyond `int` and
  decimal-point constants are `numeric`, rather than `bigint` or `decimal`.
- SUM, AVG, MIN, MAX, COUNT, COUNT_BIG, APPROX_COUNT_DISTINCT, STDEV, STDEVP, VAR
  and VARP have result typing for their supported inputs. Aggregates retain
  possible NULL results. GROUPING, GROUPING_ID and the ranking functions
  ROW_NUMBER, RANK, DENSE_RANK and NTILE have their SQL Server result types.
- CASE and COALESCE reconcile supported numeric or character branches, including
  differing facets. ISNULL keeps the first argument's type, and NULLIF keeps its
  first argument's type with a nullable result.
- ROUND, ABS, SUBSTRING, single-argument trimming/case functions, character `+`
  and text-only CONCAT have bounded result typing. CONCAT with nontext arguments
  remains unsupported because its implicit formatting lengths are not modeled.
- CAST/CONVERT retain their declared result types and SQL Server facet defaults,
  including MAX. A cast preserves an aggregate/subquery's possible NULL result.
- Derived tables, CTEs, scalar subqueries and supported set-operation inputs
  propagate result contracts. Set operations reconcile supported numeric or
  character types and facets. Outer joins and OUTER APPLY introduce nullability
  on their null-extended side.

This is bounded type analysis, not SQL Server compilation or value execution.
Mixed numeric/character implicit conversions, date arithmetic, other function
families, and general PIVOT/UNPIVOT or TVF result typing remain unsupported.
Subtotal grouping conservatively treats its grouping keys as nullable; individual
grouping-set participation is not yet enumerated. Other advanced grouping and
recursive-query semantics remain partial. Unknown schema
facets retain their existing validation behavior. Final expression/target type
IDs must match; the different-type destination policy is unchanged.

## Supported

### Source admission and scope

- Named table reference
- Table alias
- Derived table
- Inline VALUES
- Common table expression
- CTE column list
- Recursive common table expression
  Current support: rowset-shape stabilization in sanctioned cases
- INNER JOIN
- LEFT OUTER JOIN
- RIGHT OUTER JOIN
- FULL OUTER JOIN
- CROSS JOIN
- TABLESAMPLE
  Current support: sample-number and repeat-seed expressions are traversed for name resolution; rowset shape is unchanged in Binding
- CROSS APPLY
- OUTER APPLY
- Join parentheses

### Query-boundary and rowset composition

- SELECT
- UNION
- UNION ALL
- INTERSECT
- EXCEPT
- Select scalar expression
- GROUP BY
- GROUP BY ALL
- HAVING
- Expression grouping specification

### Query modifiers and wrappers

- DISTINCT
- TOP
- ORDER BY
- OFFSET
- FETCH
- Query parentheses
  Current support: query-level modifiers and wrappers are traversed for name binding, including `TOP ... WITH TIES` ordering requirements and output-alias references in query-level `ORDER BY`; these constructs do not mutate rowset shape in Binding

### Structural schema conformance

- Schema-aware Binding validates resolved source and target contracts.
  Schema-free name/scope binding does not establish schema or type conformance.

### Name resolution and expression traversal

- Column reference
  Current support: source references persist as `ColumnReference`; resolved DML target references persist as `TargetColumnReference` with their exact syntax and schema-field evidence
- Comparison predicate
- BETWEEN
- IN
- EXISTS
- LIKE
- LIKE ESCAPE
- IS NULL
- DISTINCT predicate
- Full-text predicate (`CONTAINS`, `FREETEXT`)
- Negated predicate
- Boolean AND
- Boolean OR
- Boolean NOT
- Parenthesized boolean expression
- Subquery comparison predicate
- Binary expression
- Unary expression
- Parenthesized scalar expression
- Scalar subquery
- Searched CASE
- Simple CASE
- COALESCE
- NULLIF
- IIF
- CHOOSE
- CAST
- TRY_CAST
- CONVERT
- TRY_CONVERT
- PARSE
- TRY_PARSE
- LEFT
- RIGHT
- EXTRACT
- NEXT VALUE FOR
- Parameterless call (`CURRENT_TIMESTAMP`)
- AT TIME ZONE
- COLLATE
- Inline TVF parameter references (`@Param`)
  Current support: inline TVF wrapper parameters are treated as scalar inputs in expression binding and are not inferred as source-rowset columns

## Partial

### Function-rowset binding

- Schema object table-valued function
  Current support: binds with script-supplied column aliases when present; without aliases, rowset columns can be inferred from bound column references and `SELECT *` remains a Validate-time shape concern
- Built-in table-valued function
- Global table-valued function
  Current support: sanctioned global TVF shapes infer rowsets by function name (`OPENJSON`, `GENERATE_SERIES`, `STRING_SPLIT`); unknown/global shapes bind as schema-deferred rowsets and infer columns from bound references
- OPENJSON
  Current support: infers default `key` / `value` / `type` rowset shape from the script call; `WITH (...)` schema-driven shape is not yet modeled in binding
- CONTAINSTABLE
- FREETEXTTABLE
  Current support: binds table-form full-text references as function rowsets with sanctioned `KEY` / `RANK` columns and traverses the search-condition expression; table-name/column-argument schema conformance remains a Validate concern
- Table-valued function alias
  Current support: follows the currently supported TVF shapes
- Table-valued function column alias list
  Current support: aliases are honored when provided; they are no longer required for binding to proceed
- PIVOT
- UNPIVOT
  Current support: binds pivot/unpivot rowset shape when input shape is syntax-derived (for example from a derived table or CTE projection); direct base-source input remains an explicit binding gap because full source shape is not available from syntax alone

### XML traversal and sources

- WITH XMLNAMESPACES
- Default XML namespace
- XML methods
- XML NODES
  Current support: function-call targets are traversed for multi-part receiver binding (for example `s.XmlPayload.value(...)`), and XML nodes table references bind as function rowsets when script column aliases are present; XQuery/namespace semantic validation is deferred

### Recursive and aggregate semantics

- SELECT *
- SELECT alias.*
  Current support: expands from syntax-derived rowsets such as derived tables, CTEs, and other bound query boundaries; base-source star expansion is deferred to Validate because Binding does not derive source schema shape
- Recursive common table expression
  Current support: rowset shape can stabilize from explicit CTE column aliases or anchor-branch names; deeper recursive semantics are still deferred
- GROUPING SETS
- ROLLUP
- CUBE
- Composite grouping specification
- Grand total grouping
- AVG
- COUNT
- COUNT_BIG
- SUM
- MIN
- MAX
- CHECKSUM_AGG
- STRING_AGG
- STDEV
- STDEVP
- VAR
- VARP
- APPROX_COUNT_DISTINCT
  Current support: aggregate arguments are traversed and supported aggregates have SQL Server result typing as listed above. CHECKSUM_AGG and STRING_AGG result typing remains unsupported.
- GROUPING
- GROUPING_ID
  Current support: advanced grouping specifications are traversed for grouped-rowset visibility, and grouping functions have SQL Server result types. Full grouping semantics remain partial.

### Window and ordered-set traversal

- OVER
- PARTITION BY
- Window ORDER BY
- ROWS frame
- RANGE frame
- Named WINDOW clause
- ROW_NUMBER
- RANK
- DENSE_RANK
- NTILE
- LEAD
- LAG
- FIRST_VALUE
- LAST_VALUE
- PERCENT_RANK
- CUME_DIST
- PERCENTILE_CONT
- PERCENTILE_DISC
  Current support: function arguments, `WITHIN GROUP` orderings, `OVER` partitions/orderings/frame offsets, and named window-definition expressions are traversed for name resolution; full window semantics and explicit window-name validation are still deferred

### Validation seam inside `MetaTransformBinding`

- Source identifier resolution
- Source column-subset schema conformance
- Target identifier resolution
- Final output rowset structural target conformance
- Catalog-backed SQL Server expression/target type conformance
  Schema type validity is checked independently by MetaSchema's `SqlServerSchemaTypeValidator`. Every source and target field must resolve through the selected MetaDataType catalog and its modeled DataTypeSystem relationship to a system named `SqlServer`. `bind --data-type-workspace <workspace>` selects that catalog for schema validation and literal/CAST/CONVERT result typing; omission loads the packaged authored workspace. A supplied catalog is authoritative and is never supplemented from compiled seeds. Matching validated type IDs produce an exact assessment. The resulting expression type ID must match the destination type ID. Binding validates the supplied script against schema contracts; it does not prescribe how the script is produced. Missing required type IDs are also rejected by typed workspace authoring/loading.
  Current support: validation appends explicit validation link rows inside `MetaTransformBinding` and fails hard on any mismatch by default; `--allow-partial` is an explicit corpus/discovery mode that skips failed transform scripts and saves only successfully bound/validated objects, with optional TSV diagnostics through `--partial-report`; target validation supports explicit strict target-column exclusion via `--ignore-target-columns` (ignored names must exist as non-identity target fields), supports optional sparse target-column exclusion via `--ignore-target-columns-if-present`, and persists each applied ignore as `ValidationTargetIgnoredColumn`. For mutation statements, the schema-backed public bind flow derives and validates explicit `Write` facts for INSERT, UPDATE, and every supported MERGE action (UPDATE, INSERT, and DELETE), maps every `WriteValue` to its writable target field, retains scalar-expression and concrete syntax origins, fails when required INSERT fields are omitted, and proves each write value has an exact resulting type including nullability and available length, precision, and scale. `Delete`, `MergeDelete`, and `Truncate` persist their exact non-value effects. Mutation predicates, MERGE `ON`, and every optional `WHEN` search condition persist source reads as `ColumnReference` facts and target reads as `TargetColumnReference` facts. The internal schema-free structural projection used by test fixtures does not claim those strict facts. SELECT output validation likewise requires type evidence for every mapped column and rejects an unresolved result. The persisted type outcome is `ValidationTargetColumnTypeExact`. Unknown schema facets retain their existing behavior.

## Deferred

### Query modifiers and wrappers

### Predicate and function families not yet bound explicitly

- ODBC scalar function escape

## Out Of Scope (Not Planned)

- OPENROWSET
- OPENQUERY
- Ad hoc data source
- CHANGETABLE

## Not Applicable

These belong to syntax, wrapping, or later semantic layers, not to binding as a standalone coverage target.

- CREATE VIEW
- VIEW column list
- One-part name
- Two-part name
- Three-part name
- Four-part name
- Cross-database name
- CURRENT_TIMESTAMP
- NEXT VALUE FOR
- Integer literal
- Numeric literal
- Real literal
- String literal
- Binary literal
- NULL literal
- MAX literal
- bigint
- int
- smallint
- tinyint
- bit
- decimal
- numeric
- money
- smallmoney
- float
- real
- date
- time
- datetime
- datetime2
- datetimeoffset
- char
- varchar
- nchar
- nvarchar
- binary
- varbinary
- uniqueidentifier
- sql_variant
- xml
- geography
- geometry
- hierarchyid

## Current Reading

If you want the shortest honest summary of Binding today, it is:

- rowset and scope binding is real for named sources, derived tables, CTEs, joins, set operations, correlated subqueries, and grouped queries including current sanctioned advanced grouping forms and `GROUP BY ALL`
- expression traversal is broad enough for current binding work, including current sanctioned window and ordered-set clauses; Binding now persists unresolved source/target contracts, and the first Validate slice appends explicit validation rows inside the same semantic workspace
- some rows that appear in the SQL syntax reference are intentionally not tracked here as standalone binding targets because they belong to syntax ownership or later semantic layers
