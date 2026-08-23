# MetaMultiDimensional Deploy CLI Integration

This is the single multidimensional CLI integration demo. It authors a small `MetaAnalytics` workspace, converts it to `MetaMultiDimensional`, patches multidimensional-only details, and deploys a live Analysis Services multidimensional database.

The demo deploys with `--no-process` because the source queries point at illustrative warehouse tables. Normal `meta-multi-dimensional deploy` processes by default and fails if processing fails.

The workflow is modeled in:

```text
MetaMultiDimensionalDeployCliIntegration.MetaMesh
```

Run commands from the mesh folder. `--workspace` is omitted because `meta-mesh`
defaults to the current directory:

```powershell
$env:META_MULTI_DIMENSIONAL_DEMO_SERVER = "localhost\MULTI"
$env:META_MULTI_DIMENSIONAL_DEMO_DATABASE = "MetaBiMultiDimensionalDeployDemo"

cd MetaMultiDimensionalDeployCliIntegration.MetaMesh
meta-mesh show
meta-mesh run --operation cleanup
meta-mesh run --operation deploy-multi-dimensional-model
```

Default environment values:

- server: `localhost\MULTI`
- database: `MetaBiMultiDimensionalDeployDemo`

Override with:

- `META_MULTI_DIMENSIONAL_DEMO_SERVER`
- `META_MULTI_DIMENSIONAL_DEMO_DATABASE`

Requires these CLIs on `PATH`:

- `meta-analytics`
- `meta-convert`
- `meta-multi-dimensional`

Generated folders:

- `AnalyticsWorkspace`
- `MultiDimensionalWorkspace`
