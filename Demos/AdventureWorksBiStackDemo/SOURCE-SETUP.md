# AdventureWorks source setup

The stack starts from the live `AdventureWorks2022` OLTP database. It does not use Microsoft's separate AdventureWorks data warehouse sample or copy an existing analytical model.

Official sources:

- [AdventureWorks sample databases](https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure)
- [SQL Server sample releases](https://github.com/microsoft/sql-server-samples/releases/tag/adventureworks)

Restore `AdventureWorks2022.bak` through your normal SQL Server tooling, or run the local setup helper:

```powershell
$env:AW_SQL_SERVER = "."
$env:AW_SOURCE_DATABASE = "AdventureWorks2022"
.\prepare-adventureworks-db.ps1
```

Set `AW_RESTORE_REPLACE=1` when the helper should replace an existing database. `AW_BACKUP_URL` and `AW_RUN_ROOT` override the download source and local backup cache when needed.

Set the source and administrative connections used by the mesh:

```powershell
$env:AW_SOURCE_SQL = "Server=.;Database=AdventureWorks2022;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:AW_ADMIN_SQL = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
```

Then verify and synchronize the source contract:

```powershell
cd AdventureWorksBiStackDemo.MetaMesh
meta-mesh run --operation validate-source
meta-mesh run --operation sync-source-schema
```

The synchronization extracts the complete live database schema into `Runs\source\AdventureWorks2022\Schema`, so source drift remains visible even though the first ETL slice uses only the tables required by the analytical scope.
