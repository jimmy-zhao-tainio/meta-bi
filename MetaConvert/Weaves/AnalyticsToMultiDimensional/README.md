# Analytics to MultiDimensional Weave

This is the sanctioned forward `MetaWeave` correspondence from
`MetaAnalytics` to `MetaMultiDimensional`. Its two requirements define the
admitted source domain, and its 23 transformations populate the shared
multidimensional database, cube, dimensions, measure groups, measures,
relationships, perspectives, roles, cultures and translations.

Tables containing measures become measure groups. Other tables become
dimensions. Relationships from a measure-group table to a dimension table
become dimension usages. The six concrete neutral aggregate-function entities
map to `Sum`, `Average`, `Count`, `DistinctCount`, `Min` and `Max`.

The source and target contracts are:

- [`MetaAnalytics`](../../../MetaAnalytics/Workspaces/MetaAnalytics)
- [`MetaMultiDimensional`](../../../MetaMultiDimensional/Workspaces/MetaMultiDimensional)

Tabular-style table and attribute permissions are outside the shared mapping.
The `UnsupportedSecurity` requirement rejects them before target construction;
dimension and cell permissions can then be authored in the resulting
`MetaMultiDimensional` workspace. `MeasureAggregateFunction` requires every
measure to reference exactly one concrete aggregate-function entity.

Execute the weave into a new target workspace:

```text
meta-weave execute \
  --workspace . \
  --source-workspace ../../../MetaAnalytics/Workspaces/<analytics-workspace> \
  --target-workspace ../../../MetaMultiDimensional/Workspaces/MetaMultiDimensional \
  --xml <new-target-workspace>
```

`forward` is the default direction. The output surface may instead be selected
with `--csharp` or `--sql`.

Inspect a modeled population or requirement as WeaveScript:

```text
meta-weave emit-transformation \
  --workspace . \
  --direction forward \
  --name DimensionUsage

meta-weave emit-requirement \
  --workspace . \
  --direction forward \
  --name UnsupportedSecurity
```

The product converter executes this packaged weave. A complete population
witness remains structurally identical to the frozen C# reference converter
retained by the test suite.
