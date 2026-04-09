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

### Query-boundary and rowset composition

- SELECT
- UNION
- UNION ALL
- INTERSECT
- EXCEPT
- Select scalar expression
- SELECT *
- SELECT alias.*
- GROUP BY
- HAVING
- Expression grouping specification

### Name resolution and expression traversal

- Column reference
- Comparison predicate
- BETWEEN
- IN
- EXISTS
- LIKE
- LIKE ESCAPE
- IS NULL
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
- AT TIME ZONE
- COLLATE

## Partial

### Function-rowset binding

- Schema object table-valued function
  Current support: requires script-supplied column alias list
- Table-valued function alias
  Current support: follows the currently supported TVF shapes
- Table-valued function column alias list
  Current support: this is the sanctioned shape-enabler for current TVF binding

### Recursive and aggregate semantics

- Recursive common table expression
  Current support: rowset shape can stabilize from explicit CTE column aliases or anchor-branch names; deeper recursive semantics are still deferred
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

## Deferred

### Query modifiers and wrappers

- DISTINCT
- TOP
- ORDER BY
- OFFSET
- FETCH
- Query parentheses

### Additional sources

- OPENJSON
- OPENROWSET
- OPENQUERY
- Ad hoc data source
- CHANGETABLE
- Built-in table-valued function
- Global table-valued function
- Join parentheses

### Predicate and function families not yet bound explicitly

- DISTINCT predicate
- Full-text predicate
- GROUP BY ALL
- ROLLUP
- CUBE
- GROUPING SETS
- Grand total grouping
- Composite grouping specification
- GROUPING
- GROUPING_ID
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
- EXTRACT
- Parameterless call
- WITH XMLNAMESPACES
- Default XML namespace
- XML methods
- XML NODES
- PIVOT
- UNPIVOT
- TABLESAMPLE
- CONTAINS
- FREETEXT
- CONTAINSTABLE
- FREETEXTTABLE
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

- rowset and scope binding is real for named sources, derived tables, CTEs, joins, set operations, correlated subqueries, and basic grouped queries
- expression traversal is broad enough for current binding work, but window semantics, pivoting, XML, full-text, and most source-special forms are still deferred
- some rows that appear in the SQL syntax reference are intentionally not tracked here as standalone binding targets because they belong to syntax ownership or later semantic layers
