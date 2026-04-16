# Transform Binding Coverage

Seeded from [VIEW_SURFACE_REFERENCE.md](/c:/Users/jimmy/Desktop/meta-bi/MetaTransform/Script/Reference/VIEW_SURFACE_REFERENCE.md), but organized around binding concerns rather than syntax inventory.

Status meanings:
- `Supported` = binding behavior is implemented for the current sanctioned shapes
- `Partial` = binding works, but with important limits that should be called out explicitly
- `Deferred` = modeled syntax exists, but binding coverage is not claimed yet
- `Not applicable` = not a standalone binding concern in this tracker

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

- Not applicable in Binding itself
  Current boundary: Binding persists source and target SQL identifiers plus derived rowsets, but schema comparison belongs to Validate

### Name resolution and expression traversal

- Column reference
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
- NEXT VALUE FOR
- Parameterless call (`CURRENT_TIMESTAMP`)
- AT TIME ZONE
- COLLATE
- Inline TVF parameter references (`@Param`)
  Current support: inline TVF wrapper parameters are treated as scalar inputs in expression binding and are not inferred as source-rowset columns

## Partial

### Function-rowset binding

- Schema object table-valued function
  Current support: requires script-supplied column alias list
- Built-in table-valued function
- Global table-valued function
  Current support: sanctioned global TVF shapes currently infer rowsets by function name (`OPENJSON`, `GENERATE_SERIES`, `STRING_SPLIT`); other global TVFs remain explicit binding gaps
- OPENJSON
  Current support: infers default `key` / `value` / `type` rowset shape from the script call; `WITH (...)` schema-driven shape is not yet modeled in binding
- CONTAINSTABLE
- FREETEXTTABLE
  Current support: binds table-form full-text references as function rowsets with sanctioned `KEY` / `RANK` columns and traverses the search-condition expression; table-name/column-argument schema conformance remains a Validate concern
- Table-valued function alias
  Current support: follows the currently supported TVF shapes
- Table-valued function column alias list
  Current support: this is the sanctioned shape-enabler for current TVF binding
- PIVOT
- UNPIVOT
  Current support: binds pivot/unpivot rowset shape when input shape is syntax-derived (for example from a derived table or CTE projection); direct base-source input remains an explicit binding gap because full source shape is not available from syntax alone

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
  Current support: aggregate arguments are traversed and grouped-query rowset binding exists, but full aggregate output/type semantics are not yet the point of this layer
- GROUPING
- GROUPING_ID
  Current support: advanced grouping specifications are traversed for grouped-rowset visibility, and grouping-function arguments bind through the aggregate call path; full aggregate/output/type semantics are still deferred

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
  Current support: validation appends explicit validation link rows inside `MetaTransformBinding` and fails hard on any mismatch; target validation currently checks structural count/name and skips SQL identity columns only

## Deferred

### Query modifiers and wrappers

### Additional sources

- OPENJSON
- OPENROWSET
- OPENQUERY
- Ad hoc data source
- CHANGETABLE

### Predicate and function families not yet bound explicitly

- EXTRACT
- WITH XMLNAMESPACES
- Default XML namespace
- XML methods
- XML NODES
- TABLESAMPLE
- ODBC scalar function escape

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
