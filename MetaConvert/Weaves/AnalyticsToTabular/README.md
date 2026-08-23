# Analytics to Tabular Weave

This is the sanctioned forward `MetaWeave` correspondence from
`MetaAnalytics` to `MetaTabular`. Its requirement defines the admitted
source domain, and its 24 transformations carry that domain into the
corresponding tabular populations. The modeled queries in this workspace are
the authored conversion truth; emitted SQL provides their editable inspection
surface.

The source and target contracts are:

- [`MetaAnalytics`](../../../MetaAnalytics/Workspaces/MetaAnalytics)
- [`MetaTabular`](../../../MetaTabular/Workspaces/MetaTabular)

A source-backed base measure relates to an `AggregateFunction` with one
concrete function entity: `SumAggregateFunction`,
`AverageAggregateFunction`, `CountAggregateFunction`,
`DistinctCountAggregateFunction`, `MinimumAggregateFunction`, or
`MaximumAggregateFunction`. These are the aggregate forms projected by the
sanctioned weave through
[`AnalyticsToTabularConverter`](../../AnalyticsToTabular/AnalyticsToTabularConverter.cs).

The `TabularMeasure` transformation combines six typed query branches with
`UNION ALL`. Each branch projects its function into a deterministic DAX
base-measure expression over the prepared source attribute. Target-specific
calculated columns, row filters, KPIs, calculation groups and items,
partitions, their perspective memberships, and KPI translations are authored
in the resulting `MetaTabular` workspace.

Execute the weave into a new target workspace:

```text
meta-weave show
meta-weave execute \
  --workspace . \
  --source-workspace ../../../MetaAnalytics/Workspaces/SampleAnalyticsCommerce \
  --target-workspace ../../../MetaTabular/Workspaces/MetaTabular \
  --xml <new-target-workspace>
```

`forward` is the default direction. Use `--direction <name>` for another
direction. `--target-workspace` supplies the target model contract; its
instances are not copied. The required `--xml <path>`, `--csharp <path>`, or
`--sql <path>` option selects where the new result is created. A rejected
source creates no output workspace.

Inspect a requirement and its violation query as readable WeaveScript:

```text
meta-weave emit-requirement \
  --workspace . \
  --direction forward \
  --name MeasureAggregateFunction
```

Inspect any transformation as readable WeaveScript:

```text
meta-weave emit-transformation \
  --workspace . \
  --direction forward \
  --name TabularMeasure
```

Replace a transformation by sending its complete script through standard
input. For example, in PowerShell:

```powershell
@'
SELECT
    m.Id AS Id,
    m.Name AS Name,
    '1500' AS CompatibilityLevel,
    m.DefaultCulture AS DefaultCulture,
    m.Description AS Description
FROM AnalyticsModel AS m;
'@ | meta-weave update-transformation `
  --workspace . `
  --direction forward `
  --name TabularModel
```

Both inspection and replacement require an explicit direction. The emitted
text is an editable projection of the semantic model; the workspace stores
the replacement query graph, not a `.sql` file.

The product converter executes this packaged weave. Its sample and complete
population witnesses remain structurally identical to the frozen C# reference
converter retained by the test suite.
