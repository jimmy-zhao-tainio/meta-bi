# Source

- Corpus source repository: archived
  [`databricks/spark-sql-perf`](https://web.archive.org/web/20250101005949/https://github.com/databricks/spark-sql-perf)
  (original URL: `https://github.com/databricks/spark-sql-perf`)
- Source folder: `src/main/resources/tpcds_2_4`
- Upstream copyright: Copyright 2015 Databricks Inc.
- Upstream license: Apache License 2.0
- Imported slice in this demo: q01-q99
- Variant mapping used for single-id coverage:
  - `q14` -> `q14a.sql`
  - `q23` -> `q23a.sql`
  - `q24` -> `q24a.sql`
  - `q39` -> `q39a.sql`

# Normalization Applied

- Wrapped each query in `CREATE VIEW tpcds.v_qNN AS ...`.
- Replaced `LIMIT n` with `OFFSET 0 ROWS FETCH NEXT n ROWS ONLY`.
- Replaced `cast(...) +/- interval 'n' (day|month|year)` with `dateadd(<unit>, +/-n, cast(...))`.

# SQL Server fixture adaptation

The 2026-11-19 fixture regeneration additionally applies these explicit changes:

- `SUBSTR` becomes SQL Server `SUBSTRING`, and sample standard deviation
  `STDDEV_SAMP` becomes `STDEV`.
- Backtick-delimited aliases use square brackets with the same alias text.
- Views with unrestricted `ORDER BY` gain `OFFSET 0 ROWS`, preserving all rows.
- Previously unnamed output expressions receive explicit aliases. Repeated output
  names in q39 and q64 receive distinct aliases for their second period.
- q36, q70 and q86 repeat their `GROUPING` expression inside an ordering `CASE`,
  where SQL Server cannot resolve the sibling output alias `lochierarchy`.
- q74's repeated `ORDER BY 1, 1, 1` becomes `ORDER BY 1`.

All 99 resulting views compile against `SourceSchema.sql`. That source DDL was
authored from the former source-table snapshot, preserving its modeled facets.
It is checked in so future extraction has an explicit SQL input. SQL Server
determines all expected output types during regeneration.

SQL Server references for these adaptations:

- [CREATE VIEW](https://learn.microsoft.com/en-us/sql/t-sql/statements/create-view-transact-sql)
- [ORDER BY and OFFSET](https://learn.microsoft.com/en-us/sql/t-sql/queries/select-order-by-clause-transact-sql)
- [STDEV](https://learn.microsoft.com/en-us/sql/t-sql/functions/stdev-transact-sql)

# Scope

- This demo covers SQL import/round-trip, schema-aware Binding and SQL
  Server-derived output contracts for `MetaTransformScript`.
- It is not a claim of benchmark-fidelity execution semantics.
- It reports no TPC benchmark metric or result.
- `SchemaWS` is regenerated from the checked-in SQL through the `regenerate-schema`
  mesh operation and retained as the demo's expected contract.

The MetaTransformScript corpus is derived from TPC-DS and is not comparable to
published TPC-DS results: the corpus is modified and does not comply with the
TPC-DS Specification. See the repository
[`THIRD-PARTY-NOTICES.md`](../../THIRD-PARTY-NOTICES.md).
