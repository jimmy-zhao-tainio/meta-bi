# MetaMultiDimensional Deploy CLI Integration

This is the single multidimensional CLI integration demo. It authors a small `MetaAnalytics` workspace, converts it to `MetaMultiDimensional`, patches multidimensional-only details, and deploys a live Analysis Services multidimensional database.

The demo deploys with `--no-process` because the source queries point at illustrative warehouse tables. Normal `meta-multi-dimensional deploy` processes by default and fails if processing fails.

Run:

```cmd
run.cmd
```

Defaults:

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
