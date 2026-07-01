# MetaTabular Deploy CLI Integration

This is the single tabular CLI integration demo. It authors a small `MetaAnalytics` workspace, converts it to `MetaTabular`, patches tabular-only details, and deploys a live Analysis Services tabular database.

The demo deploys with `--no-process` because the source queries point at illustrative warehouse tables. Normal `meta-tabular deploy` processes by default and fails if processing fails.

Run:

```cmd
run.cmd
```

Defaults:

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
