MetaTransformBinding CLI integration sample.

Source tables in `SchemaWS`:
- `sales.Customer`
- `sales.Order`
- `sales.Invoice`

Intended target tables in `SchemaWS`:
- `sales.CustomerOrderSummary`
- `reporting.InvoiceWindow`

Current binding infers source tables from the SQL and resolves them against the same schema workspace.
Current binding also requires explicit `--target` values and resolves those target SQL identifiers against the same schema workspace.
Later validation should compare each transform's final output rowset against its resolved target table in that same schema workspace.
