# Third-Party Notices

This repository is licensed under the Apache License 2.0. The following
third-party material or externally obtained sample data has additional
attribution.

## MetaTransformScript corpus derived from TPC-DS

The SQL files under
`Demos/MetaTransformScriptTpcDsCliIntegration/SourceViews` and their generated
workspace derivatives are adapted from the `q01`-`q99` query slice in
the archived
[`databricks/spark-sql-perf`](https://web.archive.org/web/20250101005949/https://github.com/databricks/spark-sql-perf)
repository, folder `src/main/resources/tpcds_2_4`. The original repository URL
was `https://github.com/databricks/spark-sql-perf`.

Copyright 2015 Databricks Inc.

The upstream project is licensed under the Apache License 2.0. A copy of that
license is included as this repository's [`LICENSE`](LICENSE). The imported SQL
was modified as documented in the corpus
[`SOURCE.md`](Demos/MetaTransformScriptTpcDsCliIntegration/SOURCE.md), including
SQL Server view wrappers and syntax normalization. No upstream `NOTICE` text was
located in the archived source inventory reviewed in August 2026.

TPC-DS is a trademark of the Transaction Processing Performance Council. The
MetaTransformScript corpus is derived from TPC-DS and is not comparable to
published TPC-DS results: it is modified for parser and binding coverage, does
not comply with the TPC-DS Specification, and reports no TPC benchmark metric.
This disclosure follows the TPC's
[Fair Use of TPC Benchmarks](https://www.tpc.org/tpc_documents_current_versions/pdf/tpc_fair_use_quick_reference_v1.0.0.pdf)
guidance for derived work.

## AdventureWorks

The AdventureWorks demo can download Microsoft's AdventureWorks sample database
from the official
[`microsoft/sql-server-samples`](https://github.com/microsoft/sql-server-samples)
releases. Those samples are provided under the MIT License. The database backup
is not redistributed in this repository; locally generated models and demo
outputs are authored by this project against the downloaded sample database.
