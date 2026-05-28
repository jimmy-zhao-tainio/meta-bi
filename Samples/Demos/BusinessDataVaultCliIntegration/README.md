# Business Data Vault CLI Integration

This demo shows CLI-based authoring of a sanctioned `MetaBusinessDataVault` workspace, generation of a current `MetaSql` workspace, and manifest-driven deployment to local SQL Server.

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

- business hubs, links, same-as links, and hierarchical links
- references and satellites
- PIT and bridge helper rows

## Output

- `BusinessDataVaultCliIntegrationWorkspace`
- `CurrentMetaSqlWorkspace`
- `MetaSqlDeployManifest`
- `MetaSqlVerifyManifest`
