MetaTransformBinding CLI integration sample.

Source tables in `SchemaWorkspace`:
- `sales.Customer`
- `sales.Order`
- `sales.Invoice`

Intended target tables in `SchemaWorkspace`:
- `sales.CustomerOrderSummary`
- `reporting.InvoiceWindow`

Current binding uses the source side of that schema workspace.
Later validation should compare each transform's final output rowset against the matching target table in the same schema workspace.
