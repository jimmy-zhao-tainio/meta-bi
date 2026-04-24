# MetaPipeline SQL Server CLI Integration

This demo creates one local SQL Server database with two tables:

- `dbo.SourceCustomer`
- `dbo.TargetCustomer`

The pipeline imports one transform script, binds it against the extracted schema workspace, and runs `meta-pipeline transfer sqlserver` to bulk-copy rows from source to target.

Run:

```cmd
call run.cmd
```
