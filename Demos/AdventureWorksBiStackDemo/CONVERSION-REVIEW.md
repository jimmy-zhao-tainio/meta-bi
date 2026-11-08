# Conversion review, 2026-11-08

The recorded demo is not yet a verified execution baseline for current main.
No generated demo workspaces, load SQL or conversion policies were changed in
this review.

## Statement support

The recorded scripts contain 53 INSERT statements in SourceToRaw, 47 INSERTs in
RawToBusiness, and 8 INSERTs plus 12 MERGEs in BusinessToWarehouse. They include
CTEs and nested queries. All 120 scripts pass apply-conversions source analysis
with an empty rule set, using both referenced database schema contracts.

apply-conversions now uses existing TransformScript/Binding statement scopes
rather than a SELECT-only gate. It rewrites original physical column reads in
supported statements, including predicates and hashes. It does not change write
destinations. An authored rule is therefore a deliberate policy for every read of
that source type, not an inferred fix for one target assignment.

## Contract evidence

Strict Binding against the checked-in schema snapshots produces:

| Boundary | Valid scripts | Scripts with errors |
| --- | ---: | ---: |
| SourceToRaw | 28 / 53 | 25 |
| RawToBusiness | 41 / 47 | 6 |
| BusinessToWarehouse | 20 / 20 | 0 |

The first reported mismatches in SourceToRaw are int -> nvarchar (15 scripts),
Name -> nvarchar (6), NameStyle -> bit (1), Flag -> bit (2), and smallmoney ->
decimal (1). RawToBusiness reports money -> decimal (6). These are first failures
per script, not an exhaustive list of column mismatches.

For example, the Raw model retains sqlserver:type:int for ProductID and
DaysToManufacture, while the extracted Raw physical schema uses nvarchar(256).
This does not establish that Data Vault requires textual integer keys or that a
global int -> nvarchar conversion should be introduced. Reconcile the authored
model, physical generation and recorded deployed schema first. If textual
representation is intentional in a particular load, normal TransformScript can
express it explicitly without changing all reads inside hashes and predicates.

The physical generator is also a separate catalog consumer:
MetaConvert/DataVaultToSql/ToMetaSql.cs currently supplies BuiltIn type and
conversion catalogs. Its LoweredTypes relation selects the SqlServer system and
the built-in direct implementation by fixed IDs. Passing a selected catalog to
apply-conversions does not alter those schema-generation choices. This review
does not change that independent path.

A disposable model probe applying an authored money -> decimal(19,4) snippet
rewrote 119 reads and made RawToBusiness bind 47 / 47. That establishes technical
feasibility, not approval of that global policy; the demo was left unchanged.

## Mesh and execution prerequisites

Seven existing mesh steps still pass the retired
--data-type-conversion-workspace option to Binding, Pipeline or Orchestration.
Those consumers now accept --data-type-workspace, with BuiltIn as default;
conversion selection/application belongs to apply-conversions only. The existing
conversion workspace supplies physical mappings but contains no value snippets.

If a global read-conversion policy is approved, its mesh step belongs after
TransformScript import and before Binding: import into a separate workspace,
apply-conversions with repeatable --source-schema inputs into the downstream
Transforms workspace, then bind and execute that result. Ordinary explicitly
authored CASTs need no conversion step. Do not reuse historical sanctioned-
conversion Binding assessments as current validation.

The development SQL Server lacks AdventureWorks2022 and the four demo-owned
databases. Full demo execution was therefore not performed. Disposable SQL Server
integration tests separately verify converted INSERT SELECT, CTE INSERT, UPDATE,
MERGE and DELETE through persisted workspaces, Binding and actual Pipeline
execution, with exact row values and NULL preservation. Existing destination-
facet and expression-typing limits remain unchanged.
