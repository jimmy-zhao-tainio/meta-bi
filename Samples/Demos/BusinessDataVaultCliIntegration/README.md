# Business Data Vault CLI Integration

This demo still shows CLI-based authoring of a sanctioned `MetaBusinessDataVault` workspace.

What it no longer does:

- it does not generate a `SqlModel` workspace
- `meta-datavault-business generate-metasql` is currently a stub

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

- `Workspace`

The script stops after authoring the Business Data Vault workspace and prints a note that `generate-metasql` is currently a stub.
