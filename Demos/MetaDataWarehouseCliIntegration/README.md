# MetaDataWarehouse CLI Integration

This demo authors a small sanctioned `MetaDataWarehouse` workspace through CLI commands, converts it to a current `MetaSql` workspace using the checked-in default `MetaDataWarehouseImplementation` policy workspace, and deploys the result to local SQL Server through the manifest-driven `meta-sql` path.

The workflow is modeled in:

```text
MetaDataWarehouseCliIntegration.MetaMesh
```

Run commands from the mesh folder. `--workspace` is omitted because `meta-mesh`
defaults to the current directory:

```powershell
$env:META_BI_DEMO_MASTER_SQL = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_BI_DW_SQL = "Server=.;Database=DataWarehouseCliIntegration;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

cd MetaDataWarehouseCliIntegration.MetaMesh
meta-mesh show
meta-mesh run --operation cleanup
meta-mesh run --operation build-and-deploy-warehouse
```

## Operations

- `cleanup`: drops the demo database and removes generated workspace folders.
- `build-and-deploy-warehouse`: authors the warehouse model, projects it to
  `MetaSql`, creates a deploy plan, deploys SQL Server objects, and creates a
  verification deploy plan.

The final verification plan should report no SQL changes after deployment.

## Output

- `MetaDataWarehouseCliIntegrationWorkspace`
- `CurrentMetaSqlWorkspace`
- `MetaSqlDeployManifest`
- `MetaSqlVerifyManifest`
- SQL Server database `DataWarehouseCliIntegration`
