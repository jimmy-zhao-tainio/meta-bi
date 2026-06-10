# AdventureWorks source setup

Use the official Microsoft AdventureWorks sample database downloads to restore a local SQL Server database.

Primary references:

- Microsoft Learn AdventureWorks sample databases: https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure
- Microsoft SQL Server samples releases: https://github.com/microsoft/sql-server-samples/releases/tag/adventureworks

For this demo, default to the plain OLTP `AdventureWorks2022` database because SQL Server 2022 is the local developer target. This is intentionally not the `AdventureWorksDW` data warehouse sample.

The demo itself starts from the restored SQL database, not from the backup file, not from sample project files, and not from hand-written schema metadata.

## Restore

Restore the `.bak` through SSMS, Azure Data Studio, the MSSQL extension, or `sqlcmd`.

The scaffold includes `prepare-adventureworks-db.cmd`, which downloads the official `AdventureWorks2022.bak`, restores it to the local SQL Server default instance, and verifies source data.

If the local SQL Server differs from the defaults, set the environment variables first:

```cmd
set AW_SQL_SERVER=localhost
set AW_SOURCE_DATABASE=AdventureWorks2022
set AW_RESTORE_REPLACE=1
```

Then run:

```cmd
prepare-adventureworks-db.cmd
```

Set the source connection variables with `00-env.cmd` or override them before running checks:

```cmd
set AW_SQL_SERVER=localhost
set AW_SOURCE_DATABASE=AdventureWorks2022
set AW_SOURCE_SQL=Server=localhost;Database=AdventureWorks2022;Trusted_Connection=True;TrustServerCertificate=True;
```

Then run:

```cmd
01-check-source.cmd
```

## First product command in the recording

After the readiness check passes, the recorded agent run should begin product work by extracting the source schema from the live SQL database:

```cmd
meta-schema extract sqlserver --new-workspace SourceSchemaWS --connection-env AW_SOURCE_SQL --system AdventureWorks --all-schemas --all-tables
```

The scaffold version writes to `Runs\source-schema\SourceSchemaWS` and exists only as a reference/preflight command:

```cmd
03-extract-source-schema.cmd
```

For the main video, do not pre-create `SourceSchemaWS`. Let the recorded agent create it inside its generated run folder.

## Tables expected by the first slice

The first video slice expects these source tables to exist:

- `Sales.SalesOrderHeader`
- `Sales.SalesOrderDetail`
- `Sales.Customer`
- `Sales.Store`
- `Sales.SalesPerson`
- `Sales.SalesTerritory`
- `Sales.SalesPersonQuotaHistory`
- `Production.Product`
- `Production.ProductSubcategory`
- `Production.ProductCategory`
- `Person.Person`
- `Person.Address`
- `Person.StateProvince`
- `Person.CountryRegion`
- `HumanResources.Employee`
