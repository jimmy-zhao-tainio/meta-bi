# Analytics to Tabular Weave

This is the sanctioned forward `MetaWeave` correspondence from
`MetaAnalytics` to `MetaTabular`. Its four requirements define the admitted
source domain, and its 25 transformations carry that domain into the
corresponding tabular populations. The modeled queries in this workspace are
the authored conversion truth; emitted SQL is only an inspection surface.

The source and target contracts are:

- [`MetaAnalytics`](../../../MetaAnalytics/Workspaces/MetaAnalytics)
- [`MetaTabular`](../../../MetaTabular/Workspaces/MetaTabular)

The direction is intentionally partial. A source-backed measure must have
exactly one `AggregationBehavior` using `SUM`, `COUNT`, `DISTINCTCOUNT`,
`DISTINCT_COUNT`, `MIN`, `MAX`, `AVERAGE`, or `AVG`. Attribute expressions,
when present, and role-filter expressions must be DAX. These are the same
admissibility rules as the established
[`AnalyticsToTabularConverter`](../../AnalyticsToTabular/AnalyticsToTabularConverter.cs).

`AggregationBehavior` is consumed to construct a DAX base-measure expression.
`AttributeRelationship` and source members without a tabular counterpart are
deliberate losses. Target-only KPIs, calculation groups and items, partitions,
their perspective memberships, and KPI translations remain empty.

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
  --name MeasureAggregationCardinality
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

For the checked-in `SampleAnalyticsCommerce` source, the executed workspace is
byte-for-byte identical across all emitted instance files to the established
converter output.
