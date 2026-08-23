# Raw Data Vault CLI Integration

This demo authors a sanctioned `MetaRawDataVault` workspace, converts it to
`MetaSql`, deploys it to SQL Server, and creates a verification deploy manifest.

Set the SQL connection variables in the caller shell:

```powershell
$env:META_BI_DEMO_MASTER_SQL = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_BI_RAW_DV_SQL = "Server=.;Database=RawDataVaultCliIntegration;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
```

Run from the mesh folder:

```powershell
cd Demos\RawDataVaultCliIntegration\RawDataVaultCliIntegration.MetaMesh
meta-mesh run --operation cleanup
meta-mesh run --operation build-and-deploy-raw-data-vault
```

The operation authors:

- independent typed fields and datatype details;
- raw hubs and hub key parts;
- raw links and named link roles;
- raw hub satellites and link satellites;
- a generated `MetaSql` workspace and SQL deployment manifests.

Generated outputs are ignored:

- `RawDataVaultCliIntegrationWorkspace`
- `CurrentMetaSqlWorkspace`
- `MetaSqlDeployManifest`
- `MetaSqlVerifyManifest`
