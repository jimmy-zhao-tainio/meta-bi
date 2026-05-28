# Raw Data Vault CLI Integration

This demo shows CLI-based authoring of a sanctioned `MetaRawDataVault` workspace, generation of a current `MetaSql` workspace, and manifest-driven deployment to local SQL Server.

## Commands

Run from this directory:

```cmd
run.cmd
```

Remove the generated workspace:

```cmd
cleanup.cmd
```

## What the demo authors

- source systems, schemas, tables, fields, and datatype details
- raw hubs and hub key parts
- raw links and link hubs
- raw hub satellites and link satellites

## Output

- `RawDataVaultCliIntegrationWorkspace`
- `CurrentMetaSqlWorkspace`
- `MetaSqlDeployManifest`
- `MetaSqlVerifyManifest`
