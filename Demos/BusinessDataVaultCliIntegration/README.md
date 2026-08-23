# Business Data Vault CLI Integration

This demo authors a sanctioned `MetaBusinessDataVault` workspace, converts it to
`MetaSql`, deploys it to SQL Server, and creates a verification deploy manifest.

Set the SQL connection variables in the caller shell:

```powershell
$env:META_BI_DEMO_MASTER_SQL = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_BI_BUSINESS_DV_SQL = "Server=.;Database=BusinessDataVaultCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
```

Run from the mesh folder:

```powershell
cd Demos\BusinessDataVaultCliIntegration\BusinessDataVaultCliIntegration.MetaMesh
meta-mesh run --operation cleanup
meta-mesh run --operation build-and-deploy-business-data-vault
```

The operation authors:

- business hubs, links, same-as links, and hierarchical links;
- references and satellites;
- PIT and bridge helper rows;
- a generated `MetaSql` workspace and SQL deployment manifests.

Generated outputs are ignored:

- `BusinessDataVaultCliIntegrationWorkspace`
- `CurrentMetaSqlWorkspace`
- `MetaSqlDeployManifest`
- `MetaSqlVerifyManifest`
