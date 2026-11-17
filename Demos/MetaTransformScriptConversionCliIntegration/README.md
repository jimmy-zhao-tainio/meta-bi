# Authored TransformScript conversion

This SQL Server example normalizes source `int` fields to a target `bigint`
contract. A local, editable MetaDataTypeConversion workspace supplies the scalar
expression. Binding and Pipeline receive the resulting script and selected type
catalog; they do not receive the conversion policy.

```powershell
$env:META_CONVERSION_DEMO_ADMIN_SQL = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_CONVERSION_DEMO_SQL = "Server=.;Database=MetaTransformScriptConversionDemo;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_CONVERSION_DEMO_OPERATIONAL_SQL = "Server=.;Database=MetaTransformScriptConversionPipeline;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
cd MetaTransformScriptConversionCliIntegration.MetaMesh
meta-mesh run --operation cleanup
meta-mesh run --operation prepare-workspaces
meta-mesh run --operation convert-and-execute
```

Preparation creates a local catalog from packaged starting data, authors an empty
conversion workspace through `meta` commands, creates disposable SQL source/target
tables, extracts their contracts using that catalog, and initializes an isolated
Pipeline operational database. The mapping's IDs are authored references to the
selected catalog's SQL Server `int` and `bigint` instances.

The conversion operation:

1. Imports `copy_values.sql` and demonstrates that its unconverted numeric types
   fail schema-aware Binding against the wider destination contract (expected CLI
   exit 5, with `TargetColumnTypeConformanceMismatch` in the diagnostic).
2. Runs `meta-transform-script apply-conversions` with the catalog, conversion
   policy and source schema, materializing a separate TransformScript workspace.
   The authored `CAST($(source) AS $(destination))` snippet wraps every source int
   read, including the predicate. Unmapped nvarchar reads remain unchanged.
3. Reloads/exports the result, binds it independently, and executes a SELECT through
   Pipeline's InsertRows path against actual SQL Server.
4. Changes only the authored snippet to `CONVERT($(destination), $(source))`, creates
   another result workspace, binds it and repeats Pipeline execution. A binary file
   comparison requires the two generated SQL exports to differ.
5. Requires the original script's before/after SQL exports to match exactly.

Both executions independently compare source/target rows using binary text values
and NULL-preserving EXCEPT comparisons. Expected numeric values are unchanged; only
their physical type widens:

| SequenceNo | NumericValue | TextValue |
| ---: | ---: | --- |
| 1 | 17 | `ordinary text` |
| 2 | -42 | `Åsa äter räksmörgås.` |
| 3 | SQL NULL | SQL NULL |
| 4 | 0 | Empty string |

The catalog and conversion policy are instance data; edits must maintain their
authored references. No C# conversion branch or implicit compatibility approval is
involved. This example exercises integer widening, not arbitrary result typing;
the supported and unsupported conversion boundaries remain those documented in
`MetaConvert/TransformScriptConversion/README.md`.

Generated workspaces and SQL exports are ignored. Run `cleanup` afterward to remove
them and both disposable databases. Creation steps use output paths; workspace
references identify prepared inputs and do not require generated destinations to
exist before their producing operation starts.
