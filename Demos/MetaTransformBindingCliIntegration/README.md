MetaTransformBinding CLI integration sample.

This demo imports two SQL view definitions into a generated `MetaTransformScript`
workspace, then binds those scripts against the tracked `SchemaWS` workspace to
produce a generated `MetaTransformBinding` workspace.

The workflow is modeled in:

```text
MetaTransformBindingCliIntegration.MetaMesh
```

Run commands from the mesh folder. `--workspace` is omitted because `meta-mesh`
defaults to the current directory:

```powershell
cd MetaTransformBindingCliIntegration.MetaMesh
meta-mesh show
meta-mesh run --operation cleanup
meta-mesh run --operation bind-transforms
```

Source tables in `SchemaWS`:

- `sales.Customer`
- `sales.Order`
- `sales.Invoice`

Target tables in `SchemaWS`:

- `sales.CustomerOrderSummary`
- `reporting.InvoiceWindow`

`bind-transforms` runs:

- `meta-transform-script from sql-file` for `SourceViews\001_customer_order_summary\view.sql`
- `meta-transform-script from sql-file` for `SourceViews\002_invoice_window\view.sql`
- `meta-transform-binding bind` with `--source-schema SchemaWS`, `--target-schema SchemaWS`, and `--execute-system MetaTransformBindingCliIntegration`

`meta-transform-binding bind` resolves source identifiers against source schema
workspaces and target identifiers against the target schema workspace. It
hard-fails on missing source or target tables, ambiguous identifiers, source
column subset mismatches, and target structural mismatches.
