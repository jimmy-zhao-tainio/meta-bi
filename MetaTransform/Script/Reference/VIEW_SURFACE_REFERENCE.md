# SQL VIEW Surface Reference

## View Definition

- CREATE VIEW
- VIEW column list

## Query Structure

- SELECT
- DISTINCT
- TOP
- ORDER BY
- OFFSET
- FETCH
- Query parentheses
- UNION
- UNION ALL
- INTERSECT
- EXCEPT

## Common Table Expressions

- WITH
- Common table expression
- Recursive common table expression
- CTE column list

## Table-Valued Functions

- Schema object table-valued function
- Built-in table-valued function
- Global table-valued function
- Table-valued function alias
- Table-valued function column alias list

## FROM Sources

- Named table reference
- Table alias
- Derived table
- Inline VALUES
- OPENJSON
- OPENROWSET
- OPENQUERY
- Ad hoc data source
- CHANGETABLE

## Joins And Lateral Sources

- INNER JOIN
- LEFT OUTER JOIN
- RIGHT OUTER JOIN
- FULL OUTER JOIN
- CROSS JOIN
- CROSS APPLY
- OUTER APPLY
- Join parentheses

## Projection

- Select scalar expression
- SELECT *
- SELECT alias.*

## Predicates

- Comparison predicate
- BETWEEN
- IN
- EXISTS
- LIKE
- LIKE ESCAPE
- IS NULL
- DISTINCT predicate
- Full-text predicate
- Negated predicate
- Boolean AND
- Boolean OR
- Boolean NOT
- Parenthesized boolean expression
- Subquery comparison predicate

## Grouping

- GROUP BY
- HAVING
- GROUP BY ALL
- ROLLUP
- CUBE
- GROUPING SETS
- Grand total grouping
- Composite grouping specification
- Expression grouping specification

## Aggregate Functions

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

## Grouping Functions

- GROUPING
- GROUPING_ID

## Windowing

- OVER
- PARTITION BY
- Window ORDER BY
- ROWS frame
- RANGE frame
- Named WINDOW clause

## Analytic Functions

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

## Scalar Expressions

- Column reference
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
- AT TIME ZONE
- COLLATE
- Parameterless call

## Literals

- Integer literal
- Numeric literal
- Real literal
- String literal
- Binary literal
- NULL literal
- MAX literal

## XML

- WITH XMLNAMESPACES
- Default XML namespace
- XML methods
- XML NODES

## Pivoting

- PIVOT
- UNPIVOT

## Sampling

- TABLESAMPLE

## Full-Text

- CONTAINS
- FREETEXT
- CONTAINSTABLE
- FREETEXTTABLE

## Object Naming

- One-part name
- Two-part name
- Three-part name
- Four-part name
- Cross-database name

## Built-In Globals And Special Calls

- CURRENT_TIMESTAMP
- NEXT VALUE FOR

## ODBC Surface

- ODBC scalar function escape

## Data Types

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
