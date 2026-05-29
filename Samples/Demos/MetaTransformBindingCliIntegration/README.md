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

The sample binds with:
- `--source-schema SchemaWS`
- `--target-schema SchemaWS`
- `--execute-system MetaTransformBindingCliIntegration`

`meta-transform-binding bind` resolves source identifiers against source schema workspaces and target identifiers against the target schema workspace. It hard-fails on:
- missing source/target tables
- ambiguous one/two/three-part identifiers
- source column subset mismatches
- final output/target structural mismatches

Optional:
- `--ignore-target-columns <col[,col...]>` excludes named non-identity target columns from target conformance checks.
- ignored names must exist on each target table or validation fails with `TargetIgnoredColumnNotFound`.
- `--ignore-target-columns-if-present <col[,col...]>` excludes named non-identity target columns only on target tables where they exist.
- if any one-part source identifier is present, `--execute-system-default-schema-name <schema>` is required.
- bind is atomic: if binding or validation fails, no binding workspace is created.
- `--allow-partial` is an explicit corpus/discovery mode that saves only successfully bound/validated objects.
- `--partial-report <path>` writes skipped object diagnostics as TSV and requires `--allow-partial`.
