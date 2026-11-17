# Raw Data Vault From MetaSchema CLI Integration

This demo prepares a small SQL Server Customer/Orders source, extracts it as a
`MetaSchema` workspace, converts that schema to `MetaRawDataVault`, converts the
raw vault model to `MetaSql`, deploys it, and creates a verification deploy
manifest.

Set the SQL connection variables in the caller shell:

```powershell
$env:META_BI_DEMO_MASTER_SQL = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_BI_SCHEMA_SOURCE_SQL = "Server=.;Database=RawDataVaultSchemaSourceDemo;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_BI_RAW_DV_FROM_SCHEMA_SQL = "Server=.;Database=RawDataVaultFromMetaSchemaCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
```

Run from the mesh folder:

```powershell
cd Demos\RawDataVaultFromMetaSchemaCliIntegration\RawDataVaultFromMetaSchemaCliIntegration.MetaMesh
meta-mesh run --operation cleanup
meta-mesh run --operation build-from-meta-schema
```

The source has two tables, native type facets, primary keys and a foreign key.
Its extraction and conversion exercise schema-driven hubs, satellites and a link.
The final deploy manifest must report no remaining changes.

The former source was the entire generated Business Vault example. Applying the
default Raw naming patterns to that already-prefixed vault compounded names and
exceeded SQL Server's 128-character identifier limit. That limit remains enforced;
this example does not introduce implicit name truncation or claim that the default
naming policy supports arbitrary nested vault schemas. Such inputs require an
appropriate authored implementation/naming policy.

Generated outputs are ignored:

- `MetaSchemaWorkspace`
- `RawDataVaultFromMetaSchemaCliIntegrationWorkspace`
- `CurrentMetaSqlWorkspace`
- `MetaSqlDeployManifest`
- `MetaSqlVerifyManifest`
