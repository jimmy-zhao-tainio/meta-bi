# MetaAnalytics CLI Integration

This demo authors a small sanctioned `MetaAnalytics` workspace through CLI commands. It exercises the conceptual analytics slice: data source, tables, attributes, hierarchy, relationship, source-backed base measures, perspective, security intent, and translations.

## Commands

The workflow is modeled in:

```text
MetaAnalyticsCliIntegration.MetaMesh
```

Run commands from the mesh folder. `--workspace` is omitted because `meta-mesh`
defaults to the current directory:

```powershell
cd MetaAnalyticsCliIntegration.MetaMesh
meta-mesh show
meta-mesh run --operation cleanup
meta-mesh run --operation author-analytics-model
```

## Output

- `MetaAnalyticsCliIntegrationWorkspace`
