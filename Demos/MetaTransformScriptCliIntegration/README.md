MetaTransformScript CLI integration sample.

This demo imports two SQL view definitions into a generated
`MetaTransformScript` workspace, exports them back to SQL, re-imports the emitted
SQL, converts both transform workspaces to `MetaSql`, and diffs the resulting
`MetaSql` workspaces.

The workflow is modeled in:

```text
MetaTransformScriptCliIntegration.MetaMesh
```

Run commands from the mesh folder. `--workspace` is omitted because `meta-mesh`
defaults to the current directory:

```powershell
cd MetaTransformScriptCliIntegration.MetaMesh
meta-mesh show
meta-mesh run --operation cleanup
meta-mesh run --operation round-trip-sql
```

`round-trip-sql` runs:

- `meta-transform-script from sql-file` for `SourceViews\001_customer_order_summary\view.sql`
- `meta-transform-script from sql-file` for `SourceViews\002_invoice_window\view.sql`
- `meta-transform-script to sql-path` to emit both folder and single-file SQL output
- `meta-transform-script from sql-file` for the emitted SQL
- `meta-convert transform-script-to-sql` for the original and round-tripped transform workspaces
- `meta instance diff` to compare the two generated `MetaSql` workspaces

The diff should report no instance differences.
