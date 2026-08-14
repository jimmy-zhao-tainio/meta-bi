# MetaAnalytics

`MetaAnalytics` is the sanctioned conceptual analytics model. It describes common analytical intent, not Visual Studio project plumbing and not a generated SSAS/TMSL/XMLA payload.

The intended workflow is:

```text
MetaAnalytics
  |-> MetaTabular
  `-> MetaMultiDimensional
```

Users do the bulk of portable analytical authoring in `MetaAnalytics`, convert to one target implementation model, then patch target-specific details there.

## Conceptual Surface

`MetaAnalytics` deliberately keeps the shared names simple:

- `AnalyticsModel`
- `DataSource`
- `Table`
- `Attribute`
- `Hierarchy`
- `HierarchyLevel`
- `Relationship`
- `Measure`
- `Perspective`
- typed perspective membership rows
- `SecurityRole`
- `RoleMember`
- `TablePermission`
- `AttributePermission`
- `Culture`
- typed translation rows

Measures are source-backed base measurements. Each `Measure` selects a prepared fact-table `SourceAttribute` and relates to an `AggregateFunction`. Exactly one concrete entity—`SumAggregateFunction`, `AverageAggregateFunction`, `CountAggregateFunction`, `DistinctCountAggregateFunction`, `MinimumAggregateFunction`, or `MaximumAggregateFunction`—types that function. This structure projects deterministically into DAX or multidimensional aggregation metadata.

Target-specific calculated columns, calculated measures, row filters, KPIs, rolling-period patterns, and language expressions are authored after conversion in `MetaTabular` or `MetaMultiDimensional`.

## Target Surfaces

`MetaTabular` owns tabular implementation reality:

- tabular model/table/column/relationship/measure rows
- DAX-compatible calculated measures and row filters
- tabular KPIs and KPI translations
- calculation groups and calculation items
- table partitions
- tabular perspectives, roles, row/object-level security, cultures, and translations

`MetaMultiDimensional` owns multidimensional implementation reality:

- databases, cubes, dimensions, cube dimensions, measure groups, and dimension usage
- measures, KPIs, MDX calculations, named sets, cube actions, and partitions
- multidimensional perspectives, roles, dimension/cell security, cultures, and translations

## Conversion Boundary

The sanctioned [`AnalyticsToTabular` MetaWeave workspace](../../MetaConvert/Weaves/AnalyticsToTabular) maps common analytics rows to `MetaTabular`. Source-backed base measures become deterministic tabular measures using the declared aggregate function and source attribute. The existing `meta-convert analytics-to-tabular` implementation remains an executable compatibility oracle while the modeled weave is adopted.

`meta-convert analytics-to-multi-dimensional` maps common analytics rows to `MetaMultiDimensional`. Source-backed base measures become measure group measures using the declared aggregate function and source attribute, and tabular-only security remains rejected clearly.

This is intentional. `MetaAnalytics` is a useful common authoring surface, not a lowest-common-denominator prison and not a fake universal SSAS model.

## Deploy Status

Deployment belongs to the target models, not to `MetaAnalytics`.

The current target realization slice is intentionally bounded but no longer empty:

- `meta-tabular deploy` creates an Analysis Services tabular database from one `TabularModel` and realizes modeled data sources, tables, columns, partitions, measures, relationships, calculation groups/items, and role filters.
- `meta-multi-dimensional deploy` creates an Analysis Services multidimensional database from one `MultiDimensionalDatabase` and realizes modeled data sources, data source views, dimensions/attributes, cubes, measure groups, measures, partitions, MDX scripts/named sets, actions, roles, and cell permissions.
- `--drop-existing` means explicit drop before create. Do not rely on XMLA overwrite/replace semantics because SSAS replacement behavior has known rough edges in real deployments.
- Deployment processes by default and fails the command if processing fails. For existing databases, pass `--drop-existing` so the operational sequence is drop, create, then full process; this avoids SSAS stale-cache behavior around processing recreated artifacts. Use `--no-process` only for metadata-only deployment, demos with placeholder sources, or deliberate staged operations.
- `meta-tabular restore` and `meta-multi-dimensional restore` are production-promotion commands: back up a processed source/pre-prod database and restore it as the target/prod database. Existing targets require explicit `--drop-existing`; restore does not rely on overwrite/replace behavior.
- `meta-tabular drop` and `meta-multi-dimensional drop` directly delete the named target database. They have no confirmation prompt and fail if the database does not exist.
- Partial processing is intentionally not part of deploy or production restore/promotion. It should be a separate target-owned command when object-level processing intent is modeled.
- Restore does not process. The backup file path must be accessible to the Analysis Services service accounts on both source and target servers.

The production realization pass must revisit SSAS settings that are easy to miss and painful to discover late:

- tabular `CompatibilityLevel`
- 64-bit / large-model / memory-sensitive deployment behavior
- dimensional table and partition size strategy
- partition processing policy and transaction behavior
- data-source impersonation, credentials, and service-account requirements
- collation, language/culture defaults, storage mode, and processing mode
- multidimensional aggregation design and large dimension processing settings

These are target implementation concerns. They should become explicit `MetaTabular` / `MetaMultiDimensional` metadata or deploy options as the target realization slice grows.

SSMS multidimensional browse/process sessions can inherit Windows client locale `4096` from custom or neutral regional formats. Local smoke testing showed cubes that were processed and queryable could still show generic SSMS errors until the browse/process language was set to a concrete supported culture such as English (United States), LCID `1033`. Enterprise guidance should confirm whether production environments standardize client locale, database/cube language, collation, or connection-string locale identifiers such as `Locale Identifier=1033` / `1053`.

Current multidimensional deploy still uses deterministic source-column projection where the target model does not yet carry a separate fact-side dimension usage key. That is enough for unprocessed object creation, but processing-real deployments need this binding modeled explicitly rather than inferred from the dimension granularity attribute.

## Research Anchors

The split follows Microsoft Analysis Services documentation:

- Tabular models center on tables, columns, relationships, measures, perspectives, roles, partitions, cultures/translations, DAX, object-level security, and calculation groups.
- Multidimensional models center on databases, cubes, dimensions, attributes, hierarchies, measure groups, dimension usage, partitions, KPIs, calculations, named sets, actions, perspectives, translations, and cell/dimension security.

## Current Workspace

- Conceptual workspace: `MetaAnalytics/Workspaces/MetaAnalytics`
- Tabular workspace: `MetaTabular/Workspaces/MetaTabular`
- Multidimensional workspace: `MetaMultiDimensional/Workspaces/MetaMultiDimensional`
- Conceptual CLI: `meta-analytics`
- Conversion CLI: `meta-convert analytics-to-tabular` and `meta-convert analytics-to-multi-dimensional`
- Target CLIs: `meta-tabular` and `meta-multi-dimensional`
