# Current gaps

All 99 query modules pass canonical SQL round-trip and schema-aware Binding.
Binding matches the output types, lengths, precision and scale that SQL Server
reports. These checks cover syntax fidelity and output contracts.

## Syntax round-trip

`build-tpc-ds-snapshot` imports all 99 modules through `SourceViews.manifest.tsv`,
exports them, re-imports through the emitted-file manifest and requires canonical
SQL re-export to match byte for byte. The transform diff records representation
and file provenance differences.

`compare-sql-projections` separately retains the full-graph MetaSql conversion
and comparison. Its first conversion was interrupted after more than fifteen
CPU minutes without a result. This additional comparison is unverified; its
runtime requires investigation before treating it as a practical routine check.

## Schema-aware Binding

`SchemaWS` was regenerated on 2026-11-19 from 24 SQL Server source tables and all
99 compiled query views. Its 1,040 column contracts were checked against native
SQL Server metadata, including applicable type facets and nullability. Two
complete generations produced byte-identical workspaces. The output views were
adapted into table contracts through modeled CLI operations. For example, q01's
`c_customer_id` is now `char(16)` instead of the former `nvarchar` declaration.

The integration suite reports each of the 99 queries independently through
`TransformBindingTpcDsValidatedCorpusTests.Query_BindsAndValidatesAgainstSavedSchema`.
It imports each query through the production SQL service, loads the saved schema
and selected catalog, and requires successful schema-aware Binding with exact
type assessments. Every Binding error fails its query case. The former aggregate
classification tests and their synthetic all-`nvarchar` schema have been removed.

Run these cases from the repository root:

```powershell
dotnet test MetaTransform/Script/Tests/MetaTransformScript.Tests.csproj -c Release --no-build --no-restore --filter FullyQualifiedName~TransformBindingTpcDsValidatedCorpusTests
```

Before regeneration, the strict run on 2026-11-18
reported one passing query and 98 failures: 75 type-conformance mismatches and
23 cases where Binding returned without complete output type assessments.
After regeneration, seven passed and 92 failed because Binding supplied missing
or incomplete output type assessments. That exposed a SELECT validation defect:
an unresolved expression could receive a target column link without type
validation or rejection.

Binding now rejects unresolved output types and derives SQL Server result
contracts for the corpus's aggregates, numeric arithmetic, supported functions,
CASE/COALESCE, scalar subqueries and set operations. Types resolve through the
selected catalog, including the distinction between `numeric` literals and
`decimal` expressions and their precision/scale rules. The unchanged regenerated
fixture now passes **all 99 strict Binding cases**, with an exact type assessment
for every output column and zero skips. The suite also compares every inferred
output length, precision and scale directly with the saved native contract;
conformance to a wider destination cannot conceal an incorrect inference. No
accepted-error list is used.

Focused integration cases additionally compare expected expression contracts
with live SQL Server metadata and verify catalog IDs that have arbitrary values.
Outer joins and subtotal grouping preserve their possible NULL results instead
of retaining a source field's NOT NULL assertion. See the
[Binding support tracker](../../MetaTransform/Binding/Reference/BINDING_SUPPORT_TRACKER.md)
for supported result typing and remaining boundaries.

## SQL Server metadata

All 99 adapted view definitions compile in the fixture database. `SOURCE.md`
records the dialect and view-wrapper changes. The previous compilation probe
reported 60 successful views and 39 rejections, and also omitted source facets
stored in `FieldDataTypeDetail`. That probe has been superseded by the complete
regeneration from `SourceSchema.sql` with modeled facets preserved.

Generation compiles the views and verifies their metadata against the modeled
fixture in an empty database.
