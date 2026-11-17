# MetaMultiDimensional Deploy CLI Integration

This metadata-deployment demo authors a small `MetaAnalytics` workspace, converts it to `MetaMultiDimensional`, patches multidimensional-only details, and deploys a live Analysis Services multidimensional database. The separate hierarchy-query demo also processes data and executes MDX.

The demo deploys with `--no-process` because the source queries point at illustrative warehouse tables. Normal `meta-multi-dimensional deploy` processes by default and fails if processing fails.

The workflow is modeled in:

```text
MetaMultiDimensionalDeployCliIntegration.MetaMesh
```

Run commands from the mesh folder. `--workspace` is omitted because `meta-mesh`
defaults to the current directory:

```powershell
cd MetaMultiDimensionalDeployCliIntegration.MetaMesh
meta-mesh show
meta-mesh run --operation cleanup
meta-mesh run --operation deploy-multi-dimensional-model
```

Authored deployment target:

- server: `localhost\MULTI`
- database: `MetaBiMultiDimensionalDeployDemo`

To change the target, use `meta-mesh update-step` on operation
`deploy-multi-dimensional-model`, step `deploy-multi-dimensional`, supplying literal
`--server` and `--database-name` arguments. Mesh `{env:NAME}` tokens supply a variable
name for CLI options such as `--connection-env`; they do not interpolate its value.

Requires these CLIs on `PATH`:

- `meta-analytics`
- `meta-convert`
- `meta-multi-dimensional`

Generated folders:

- `AnalyticsWorkspace`
- `MultiDimensionalWorkspace`
