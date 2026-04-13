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
Later Validate should compare each transform's source and final output rowsets against `SchemaWS`.
