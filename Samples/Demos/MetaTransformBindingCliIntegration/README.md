MetaTransformBinding CLI integration sample.

Source tables in `SchemaWS`:
- `sales.Customer`
- `sales.Order`
- `sales.Invoice`

Intended target tables in `SchemaWS`:
- `sales.CustomerOrderSummary`
- `reporting.InvoiceWindow`

Binding infers source rowset identifiers from SQL and reads each target SQL identifier from `TransformScript.TargetSqlIdentifier`.
`meta-transform-binding bind` processes all transform scripts in `TransformWS` into one binding workspace.

`meta-transform-binding bind` resolves those source/target SQL identifiers against `SchemaWS` and hard-fails on:
- missing source/target tables
- ambiguous one/two/three-part identifiers
- source column subset mismatches
- final output/target structural mismatches

Optional:
- `--ignore-target-columns <col[,col...]>` excludes named non-identity target columns from target conformance checks.
- ignored names must exist on each target table or validation fails with `TargetIgnoredColumnNotFound`.
- bind is atomic: if binding or validation fails, no binding workspace is created.
