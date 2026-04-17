# Source

- Corpus source repository: `https://github.com/databricks/spark-sql-perf`
- Source folder: `src/main/resources/tpcds_2_4`
- License observed in source repo: Apache-2.0
- Imported slice in this demo: q01-q20 (with q14 mapped from source file `q14a.sql`)

# Normalization Applied

- Wrapped each query in `CREATE VIEW tpcds.v_qNN AS ...`.
- Replaced `LIMIT n` with `OFFSET 0 ROWS FETCH NEXT n ROWS ONLY`.
- Replaced `cast(...) + interval 'n' day` with `dateadd(day, n, cast(...))`.

# Scope

- This demo is a parser/import corpus slice for `MetaTransformScript`.
- It is not a claim of benchmark-fidelity execution semantics.
