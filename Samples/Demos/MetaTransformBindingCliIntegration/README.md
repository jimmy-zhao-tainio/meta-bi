MetaTransformBinding CLI integration sample.

Source tables in `SchemaWS`:
- `sales.Customer`
- `sales.Order`
- `sales.Invoice`

Intended target tables in `SchemaWS`:
- `sales.CustomerOrderSummary`
- `reporting.InvoiceWindow`

Current binding infers source rowset identifiers from the SQL and persists them as SQL identifiers only.
Current binding reads each target SQL identifier from `TransformScript.TargetSqlIdentifier` and persists that target in the binding workspace.

`meta-transform-binding validate` then resolves those source/target SQL identifiers against `SchemaWS` and hard-fails on:
- missing source/target tables
- ambiguous one/two/three-part identifiers
- source column subset mismatches
- final output/target structural mismatches
