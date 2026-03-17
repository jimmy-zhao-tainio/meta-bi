# Raw Data Vault CLI Integration

This demo still shows CLI-based authoring of a sanctioned `MetaRawDataVault` workspace.

What it no longer does:

- it does not generate a `MetaSql` workspace
- `meta-datavault-raw generate-metasql` is currently a stub

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

- `Workspace`

The script stops after authoring the Raw Data Vault workspace and prints a note that `generate-metasql` is currently a stub.
