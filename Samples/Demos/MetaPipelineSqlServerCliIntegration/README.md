# MetaPipeline SQL Server CLI Integration

This demo creates one local SQL Server database with two tables:

- `dbo.SourceCustomer`
- `dbo.TargetCustomer`

The pipeline imports one transform script, binds it against the extracted schema workspace, creates a `MetaPipeline` workspace, adds a pipeline row, and runs `meta-pipeline execute`.

The modeled target write is `InsertRows`; the current SQL Server runtime realizes it with bulk copy.

Run:

```cmd
call run.cmd
```
