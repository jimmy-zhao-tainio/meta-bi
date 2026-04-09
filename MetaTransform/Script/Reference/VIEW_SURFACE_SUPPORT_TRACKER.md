# SQL VIEW Surface Support Tracker

Legend:
- `M` = representable in the current `MetaTransformScript` model
- `R` = parser plus emitter roundtrip verified
- `S` = `MetaTransformScriptSqlService` import/export audited
- `-` = not yet claimed

## View Definition

| Label | M | R | S |
| --- | --- | --- | --- |
| CREATE VIEW | Y | Y | Y |
| VIEW column list | Y | Y | Y |

## Query Structure

| Label | M | R | S |
| --- | --- | --- | --- |
| SELECT | Y | Y | Y |
| DISTINCT | Y | Y | Y |
| TOP | Y | Y | Y |
| ORDER BY | Y | Y | Y |
| OFFSET | Y | Y | Y |
| FETCH | Y | Y | Y |
| Query parentheses | Y | Y | Y |
| UNION | Y | Y | Y |
| UNION ALL | Y | Y | Y |
| INTERSECT | Y | Y | Y |
| EXCEPT | Y | Y | Y |

## Common Table Expressions

| Label | M | R | S |
| --- | --- | --- | --- |
| WITH | Y | Y | Y |
| Common table expression | Y | Y | Y |
| Recursive common table expression | Y | Y | Y |
| CTE column list | Y | Y | Y |

## Table-Valued Functions

| Label | M | R | S |
| --- | --- | --- | --- |
| Schema object table-valued function | Y | Y | Y |
| Built-in table-valued function | Y | Y | Y |
| Global table-valued function | Y | Y | Y |
| Table-valued function alias | Y | Y | Y |
| Table-valued function column alias list | Y | Y | Y |

## FROM Sources

| Label | M | R | S |
| --- | --- | --- | --- |
| Named table reference | Y | Y | Y |
| Table alias | Y | Y | Y |
| Derived table | Y | Y | Y |
| Inline VALUES | Y | Y | Y |
| OPENJSON | Y | - | - |
| OPENROWSET | - | - | - |
| OPENQUERY | Y | - | - |
| Ad hoc data source | - | - | - |
| CHANGETABLE | Y | - | - |

## Joins And Lateral Sources

| Label | M | R | S |
| --- | --- | --- | --- |
| INNER JOIN | Y | Y | Y |
| LEFT OUTER JOIN | Y | Y | Y |
| RIGHT OUTER JOIN | Y | Y | Y |
| FULL OUTER JOIN | Y | Y | Y |
| CROSS JOIN | Y | Y | Y |
| CROSS APPLY | Y | Y | Y |
| OUTER APPLY | Y | Y | Y |
| Join parentheses | Y | Y | Y |

## Projection

| Label | M | R | S |
| --- | --- | --- | --- |
| Select scalar expression | Y | Y | Y |
| SELECT * | Y | Y | Y |
| SELECT alias.* | Y | Y | Y |

## Predicates

| Label | M | R | S |
| --- | --- | --- | --- |
| Comparison predicate | Y | Y | Y |
| BETWEEN | Y | Y | Y |
| IN | Y | Y | Y |
| EXISTS | Y | Y | Y |
| LIKE | Y | Y | Y |
| LIKE ESCAPE | Y | Y | Y |
| IS NULL | Y | Y | Y |
| DISTINCT predicate | Y | Y | Y |
| Full-text predicate | Y | Y | Y |
| Negated predicate | Y | Y | Y |
| Boolean AND | Y | Y | Y |
| Boolean OR | Y | Y | Y |
| Boolean NOT | Y | Y | Y |
| Parenthesized boolean expression | Y | Y | Y |
| Subquery comparison predicate | Y | Y | Y |

## Grouping

| Label | M | R | S |
| --- | --- | --- | --- |
| GROUP BY | Y | Y | Y |
| HAVING | Y | Y | Y |
| GROUP BY ALL | Y | Y | Y |
| ROLLUP | Y | Y | Y |
| CUBE | Y | Y | Y |
| GROUPING SETS | Y | Y | Y |
| Grand total grouping | Y | Y | Y |
| Composite grouping specification | Y | Y | Y |
| Expression grouping specification | Y | Y | Y |

## Aggregate Functions

| Label | M | R | S |
| --- | --- | --- | --- |
| AVG | Y | Y | Y |
| COUNT | Y | Y | Y |
| COUNT_BIG | Y | Y | Y |
| SUM | Y | Y | Y |
| MIN | Y | Y | Y |
| MAX | Y | Y | Y |
| CHECKSUM_AGG | Y | Y | Y |
| STRING_AGG | Y | Y | Y |
| STDEV | Y | Y | Y |
| STDEVP | Y | Y | Y |
| VAR | Y | Y | Y |
| VARP | Y | Y | Y |
| APPROX_COUNT_DISTINCT | Y | Y | Y |

## Grouping Functions

| Label | M | R | S |
| --- | --- | --- | --- |
| GROUPING | Y | Y | Y |
| GROUPING_ID | Y | Y | Y |

## Windowing

| Label | M | R | S |
| --- | --- | --- | --- |
| OVER | Y | Y | Y |
| PARTITION BY | Y | Y | Y |
| Window ORDER BY | Y | Y | Y |
| ROWS frame | Y | Y | Y |
| RANGE frame | Y | Y | Y |
| Named WINDOW clause | Y | Y | Y |

## Analytic Functions

| Label | M | R | S |
| --- | --- | --- | --- |
| ROW_NUMBER | Y | Y | Y |
| RANK | Y | Y | Y |
| DENSE_RANK | Y | Y | Y |
| NTILE | Y | Y | Y |
| LEAD | Y | Y | Y |
| LAG | Y | Y | Y |
| FIRST_VALUE | Y | Y | Y |
| LAST_VALUE | Y | Y | Y |
| PERCENT_RANK | Y | Y | Y |
| CUME_DIST | Y | Y | Y |
| PERCENTILE_CONT | Y | Y | Y |
| PERCENTILE_DISC | Y | Y | Y |

## Scalar Expressions

| Label | M | R | S |
| --- | --- | --- | --- |
| Column reference | Y | Y | Y |
| Binary expression | Y | Y | Y |
| Unary expression | Y | Y | Y |
| Parenthesized scalar expression | Y | Y | Y |
| Scalar subquery | Y | Y | Y |
| Searched CASE | Y | Y | Y |
| Simple CASE | Y | Y | Y |
| COALESCE | Y | Y | Y |
| NULLIF | Y | Y | Y |
| IIF | Y | Y | Y |
| CHOOSE | Y | Y | Y |
| CAST | Y | Y | Y |
| TRY_CAST | Y | Y | Y |
| CONVERT | Y | Y | Y |
| TRY_CONVERT | Y | Y | Y |
| PARSE | Y | Y | Y |
| TRY_PARSE | Y | Y | Y |
| LEFT | Y | Y | Y |
| RIGHT | Y | Y | Y |
| EXTRACT | Y | Y | Y |
| AT TIME ZONE | Y | Y | Y |
| COLLATE | Y | Y | Y |
| Parameterless call | Y | Y | Y |

## Literals

| Label | M | R | S |
| --- | --- | --- | --- |
| Integer literal | Y | Y | Y |
| Numeric literal | Y | Y | Y |
| Real literal | Y | Y | Y |
| String literal | Y | Y | Y |
| Binary literal | Y | Y | Y |
| NULL literal | Y | Y | Y |
| MAX literal | Y | Y | Y |

## XML

| Label | M | R | S |
| --- | --- | --- | --- |
| WITH XMLNAMESPACES | Y | Y | Y |
| Default XML namespace | Y | Y | Y |
| XML methods | Y | Y | Y |
| XML NODES | Y | Y | Y |

## Pivoting

| Label | M | R | S |
| --- | --- | --- | --- |
| PIVOT | Y | Y | Y |
| UNPIVOT | Y | Y | Y |

## Sampling

| Label | M | R | S |
| --- | --- | --- | --- |
| TABLESAMPLE | Y | Y | Y |

## Full-Text

| Label | M | R | S |
| --- | --- | --- | --- |
| CONTAINS | Y | Y | Y |
| FREETEXT | Y | Y | Y |
| CONTAINSTABLE | Y | Y | Y |
| FREETEXTTABLE | Y | Y | Y |

## Object Naming

| Label | M | R | S |
| --- | --- | --- | --- |
| One-part name | Y | Y | Y |
| Two-part name | Y | Y | Y |
| Three-part name | Y | Y | Y |
| Four-part name | Y | Y | Y |
| Cross-database name | Y | Y | Y |

## Built-In Globals And Special Calls

| Label | M | R | S |
| --- | --- | --- | --- |
| CURRENT_TIMESTAMP | Y | Y | Y |
| NEXT VALUE FOR | Y | Y | Y |

## ODBC Surface

| Label | M | R | S |
| --- | --- | --- | --- |
| ODBC scalar function escape | - | - | - |

## Data Types

| Label | M | R | S |
| --- | --- | --- | --- |
| bigint | Y | Y | Y |
| int | Y | Y | Y |
| smallint | Y | Y | Y |
| tinyint | Y | Y | Y |
| bit | Y | Y | Y |
| decimal | Y | Y | Y |
| numeric | Y | Y | Y |
| money | Y | Y | Y |
| smallmoney | Y | Y | Y |
| float | Y | Y | Y |
| real | Y | Y | Y |
| date | Y | Y | Y |
| time | Y | Y | Y |
| datetime | Y | Y | Y |
| datetime2 | Y | Y | Y |
| datetimeoffset | Y | Y | Y |
| char | Y | Y | Y |
| varchar | Y | Y | Y |
| nchar | Y | Y | Y |
| nvarchar | Y | Y | Y |
| binary | Y | Y | Y |
| varbinary | Y | Y | Y |
| uniqueidentifier | Y | Y | Y |
| sql_variant | Y | Y | Y |
| xml | Y | Y | Y |
| geography | Y | Y | Y |
| geometry | Y | Y | Y |
| hierarchyid | Y | Y | Y |
