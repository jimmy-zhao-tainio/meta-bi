# Raw Data Vault From MetaSchema CLI Integration

This demo prepares the Business Data Vault demo database, extracts it as a
`MetaSchema` workspace, converts that schema to `MetaRawDataVault`, converts the
raw vault model to `MetaSql`, deploys it, and creates a verification deploy
manifest.

Set the SQL connection variables in the caller shell:

```powershell
$env:META_BI_DEMO_MASTER_SQL = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_BI_BUSINESS_DV_SQL = "Server=.;Database=BusinessDataVaultCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_BI_RAW_DV_FROM_SCHEMA_SQL = "Server=.;Database=RawDataVaultFromMetaSchemaCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
```

Run from the mesh folder:

```powershell
cd Demos\RawDataVaultFromMetaSchemaCliIntegration\RawDataVaultFromMetaSchemaCliIntegration.MetaMesh
meta-mesh run --operation cleanup
meta-mesh run --operation build-from-meta-schema
```

The build operation composes the source Business Data Vault mesh before
extracting schema, so the source database is recreated from the modeled
Business Data Vault demo first.

Generated outputs are ignored:

- `MetaSchemaWorkspace`
- `RawDataVaultFromMetaSchemaCliIntegrationWorkspace`
- `CurrentMetaSqlWorkspace`
- `MetaSqlDeployManifest`
- `MetaSqlVerifyManifest`
