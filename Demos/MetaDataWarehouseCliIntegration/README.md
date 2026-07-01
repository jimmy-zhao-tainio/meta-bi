# MetaDataWarehouse CLI Integration

This demo authors a small sanctioned `MetaDataWarehouse` workspace through CLI commands, converts it to a current `MetaSql` workspace using the checked-in default `MetaDataWarehouseImplementation` policy workspace, and deploys the result to local SQL Server through the manifest-driven `meta-sql` path.

## Commands

Run from this directory:

```cmd
run.cmd
```

Remove generated workspaces:

```cmd
cleanup.cmd
```

## Output

- `MetaDataWarehouseCliIntegrationWorkspace`
- `CurrentMetaSqlWorkspace`
- `MetaSqlDeployManifest`
- `MetaSqlVerifyManifest`
- SQL Server database `DataWarehouseCliIntegration`
