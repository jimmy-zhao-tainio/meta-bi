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

# Scope

- This demo is a parser/import corpus slice for `MetaTransformScript`.
- It is not a claim of benchmark-fidelity execution semantics.
- It reports no TPC benchmark metric or result.
- `SchemaWS` is checked in as a one-off schema snapshot for this corpus and reused as the demo schema contract.

The MetaTransformScript corpus is derived from TPC-DS and is not comparable to
published TPC-DS results: the corpus is modified and does not comply with the
TPC-DS Specification. See the repository
[`THIRD-PARTY-NOTICES.md`](../../THIRD-PARTY-NOTICES.md).
