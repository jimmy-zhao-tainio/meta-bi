# Data Warehouse to SQL Weave

This workspace is the sanctioned forward `MetaWeave` correspondence from a
logical `MetaDataWarehouse` and its implementation policy to `MetaSql`.
WeaveScript models the conversion as eleven target populations. The emitted
form remains recognizable T-SQL, while the workspace stores the typed semantic
model that MetaWeave executes.

The direction has four named source workspaces:

- `warehouse` supplies the logical dimensions, facts, bridges, attributes,
  measures, grain and relationships.
- `implementation` supplies SQL schemas, naming patterns, surrogate-key and
  snapshot conventions, platform columns and indexes.
- `dataTypes` supplies the sanctioned type systems and types.
- `typeConversions` supplies the direct Meta-to-SQL Server type mappings.

The `databaseName` parameter names the resulting `MetaSql` database. Direction
requirements check that it is present, that each required implementation
contract has one row, and that every type used by the conversion has exactly
one SQL Server realization.

`TableColumn` collects the column candidates for each projected table and uses
`ROW_NUMBER() OVER (PARTITION BY ... ORDER BY ...)` to reproduce the converter's
column insertion order. `TRY_CONVERT(int, sourceOrdinal)` gives numeric
ordinals their normal order and places missing or malformed ordinals after
them, with source identity as the deterministic tie-breaker.

Execute the weave into a new target workspace:

```text
meta-weave execute \
  --workspace . \
  --source-workspace warehouse=../../../MetaDataWarehouse/Workspaces/SampleDataWarehouseCommerce \
  --source-workspace implementation=../../../MetaDataWarehouse/Workspaces/MetaDataWarehouseImplementation \
  --source-workspace dataTypes=../../../MetaDataType/Workspace \
  --source-workspace typeConversions=../../../MetaDataTypeConversion/Workspace \
  --parameter databaseName=CommerceDw \
  --target-workspace ../../../MetaSql/Workspace \
  --xml <new-target-workspace>
```

`forward` is the default direction. The output surface may instead be selected
with `--csharp` or `--sql`.

Inspect any modeled target population as WeaveScript:

```text
meta-weave emit-transformation \
  --workspace . \
  --direction forward \
  --name TableColumn
```

Use `emit-requirement` in the same way for `DatabaseName`,
`ImplementationCardinality`, or `SqlServerTypeLowering`. Replacement text is
accepted through standard input by `update-transformation` and
`update-requirement`.

The checked-in sample is covered by an equivalence test against the established
C# converter. The two paths produce the same complete `MetaSql` workspace,
including database, schemas, tables, ordered columns, type details, keys,
relationships and indexes.
