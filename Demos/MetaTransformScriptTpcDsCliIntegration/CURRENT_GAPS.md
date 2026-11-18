# Current gaps

The corpus is useful for syntax coverage. Parsing, exporting and comparing
modeled SQL do not establish executable SQL Server semantics or target contracts.

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

`bind-snapshot` currently fails against the checked-in `SchemaWS`. The first
rejection is a direct `char` projection whose target declares `nvarchar`.
The fixture also leaves many source facets unknown. Matching target IDs to
Binding's inferred answers would not independently establish the right contract.
No conversion mappings or relaxed type checks are used to approve this probe.

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

The saved fixture still requires regeneration. The strict run on 2026-11-18
reported one passing query and 98 failures: 75 type-conformance mismatches and
23 cases where Binding returned without complete output type assessments.
The latter cases fail the test's evidence assertion. The production SELECT
validation path currently adds a column link when expression type evidence is
absent, without adding an exact type assessment or rejecting the expression.
These tests expose both problems instead of accepting a global list of diagnostics.

## SQL Server execution

A disposable SQL Server investigation of the 99 view definitions compiled 60
and rejected 39. Examples include `SUBSTR`, an `ORDER BY` that is invalid in a
view, and expressions without required view-column names. These are corpus
limitations, not evidence that MetaTransformScript executes those constructs.

The complete corpus has not been repaired or executed. Use the dedicated
conversion, Pipeline and AdventureWorks demos for actual execution coverage.
