# MetaTabular Deploy CLI Integration

This is the single tabular CLI integration demo. It authors a small `MetaAnalytics` workspace, converts it to `MetaTabular`, patches tabular-only details, and deploys a live Analysis Services tabular database.

The demo deploys with `--no-process` because the source queries point at illustrative warehouse tables. Normal `meta-tabular deploy` processes by default and fails if processing fails.

The workflow is modeled in:

```text
MetaTabularDeployCliIntegration.MetaMesh
```

Run commands from the mesh folder. `--workspace` is omitted because `meta-mesh`
defaults to the current directory:

```powershell
$env:META_TABULAR_DEMO_SERVER = "localhost\TABULAR"
$env:META_TABULAR_DEMO_DATABASE = "MetaBiTabularDeployDemo"

cd MetaTabularDeployCliIntegration.MetaMesh
meta-mesh show
meta-mesh run --operation cleanup
meta-mesh run --operation deploy-tabular-model
```

Default environment values:

- server: `localhost\TABULAR`
- database: `MetaBiTabularDeployDemo`

Override with:

- `META_TABULAR_DEMO_SERVER`
- `META_TABULAR_DEMO_DATABASE`

Requires these CLIs on `PATH`:

- `meta-analytics`
- `meta-convert`
- `meta-tabular`

Generated folders:

- `AnalyticsWorkspace`
- `TabularWorkspace`
