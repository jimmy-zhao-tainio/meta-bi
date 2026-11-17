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

## SQL Server execution

A disposable SQL Server investigation of the 99 view definitions compiled 60
and rejected 39. Examples include `SUBSTR`, an `ORDER BY` that is invalid in a
view, and expressions without required view-column names. These are corpus
limitations, not evidence that MetaTransformScript executes those constructs.

The complete corpus has not been repaired or executed. Use the dedicated
conversion, Pipeline and AdventureWorks demos for actual execution coverage.
