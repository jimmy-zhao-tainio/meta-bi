# MetaSql Deployment Capability Plan

## Purpose

This document describes what MetaSql currently supports as a schema deployment engine, what it blocks, and what is next.

Operator framing:

> I have a source MetaSql workspace and a live SQL Server database. I want to know whether MetaSql can safely plan and deploy the drift between them.

The current CLI surface is:

- `deploy-plan`: extract live schema when the target database exists, otherwise treat live as empty, compute differences, run feasibility checks, write deploy manifest.
- `deploy`: load manifest, validate source/live fingerprints and block-free status, create the target database only when the manifest explicitly expects missing live, then execute manifest actions.

## Current Command Surface

```bash
meta-sql deploy-plan --source-workspace <path> --connection-string <value> --out <manifest-path> [--schema <name>] [--table <name>] [--approve-drop-table <schema.table>] [--approve-drop-column <schema.table.column>] [--approve-truncate-column <schema.table.column>] [--approval-file <path>]
meta-sql deploy --manifest-workspace <path> --source-workspace <path> --connection-string <value> [--schema <name>] [--table <name>]
```

`deploy-plan` currently accepts `--source-workspace`, `--connection-string`, `--out`, optional `--schema`, optional `--table`, and object-scoped destructive approvals via repeated CLI args and/or `--approval-file`.

`deploy` currently requires `--manifest-workspace`, `--source-workspace`, and `--connection-string`, with optional `--schema` and `--table`.

If the target database is missing during `deploy-plan`, live is modeled as an empty `MetaSql` workspace and the manifest records that missing-database expectation explicitly.

Default policy: live-only data-bearing drift (`Table`, `TableColumn`) is blocked unless exact approvals are supplied.
Live-only `PrimaryKey`, `ForeignKey`, and `Index` drift is planned as drop actions by default.

## Current Planning/Deploy Contract

Difference families:

- `Table`
- `TableColumn`
- `PrimaryKey`
- `ForeignKey`
- `Index`

Difference kinds:

- `MissingInLive`
- `ExtraInLive`
- `Different`

`deploy-plan` maps differences into manifest actions and block entries.

`deploy` validates:

- manifest model contract and supported entity kinds
- no block entries
- source instance fingerprint match
- live instance fingerprint match
- expected live database presence (`Missing` vs `Present`)

Then it builds SQL statements from manifest actions and executes in one transaction.

Statement ordering is explicit and deterministic:

1. Drop foreign keys, indexes, primary keys, columns, tables.
2. Alter columns.
3. Add tables, columns, primary keys, foreign keys, indexes.

Shared-object identity matching in `deploy-plan` is based on explicit MetaSql surface keys, not ID intersection:

- Table: `(Schema.Name, Table.Name)`
- TableColumn: `(Table scope, Column.Name)`
- PrimaryKey: `(Table scope, PrimaryKey.Name)`
- ForeignKey: `(SourceTable scope, ForeignKey.Name)`
- Index: `(Table scope, Index.Name)`

If identity is ambiguous in a workspace (duplicate key in the same scope), planning fails fast instead of guessing.

## Operator Model

MetaSql is currently usable for:

- source-only objects (additions)
- live-only objects (drops)
- a narrow shared-column change slice (`AlterTableColumn`)
- changed shared primary keys via `ReplacePrimaryKey` (nonclustered subset)
- changed shared foreign keys via `ReplaceForeignKey`
- changed shared nonclustered rowstore indexes via `ReplaceIndex`

MetaSql is not yet a general-purpose executor for clustered primary-key replacement, clustered/advanced index replacement, or broad shared-column rewrites. Those are blocked.

## Capability Matrix

| Change category | Planning support | Deploy support | Current status | Exact condition / notes |
|---|---|---|---|---|
| Add table | Yes | Yes | Supported | Planned as `AddTable`; deploy creates table. Dependent PK/FK/Index adds for newly added tables are materialized during planning into explicit `Add*` manifest rows. |
| Add column to existing table | Yes | Yes | Supported | Planned as `AddTableColumn`; deploy adds unless whole table is already being created. |
| Add primary key | Yes | Yes | Supported | Planned as `AddPrimaryKey`; deploy adds from source members. |
| Add foreign key | Yes | Yes | Supported | Planned as `AddForeignKey`; deploy adds from source members. |
| Add index | Yes | Yes | Supported | Planned as `AddIndex`; deploy adds from source members. |
| Drop table | Yes | Yes | Supported with exact approval | Planned as `DropTable` only when planner has exact `DataDropTable(schema.table)` approval. Otherwise blocked with precise missing approval. |
| Drop column | Yes | Yes | Supported with exact approval | Planned as `DropTableColumn` only when planner has exact `DataDropColumn(schema.table.column)` approval. Otherwise blocked with precise missing approval. |
| Drop primary key | Yes | Yes | Supported by default | Planned as `DropPrimaryKey`; deploy drops unless whole table is being dropped. |
| Drop foreign key | Yes | Yes | Supported by default | Planned as `DropForeignKey`; deploy drops before table/column drops. |
| Drop index | Yes | Yes | Supported by default | Planned as `DropIndex`; deploy drops before PK/column/table drops. |
| Alter shared column nullability | Yes | Yes | Narrow support | Only via `AlterTableColumn`, only when changed aspects are in supported subset and all checks pass. `NULL -> NOT NULL` blocks if live contains `NULL`. |
| Alter shared column type shape | Yes | Yes | Very narrow support | Only for `sqlserver:type:*`, same SQL type family, length-based families only, and only `Length` detail changes. Type-family transitions block. Length narrowing that would truncate live data requires exact `DataTruncationColumn(schema.table.column)` approval and emits explicit `TruncateTableColumnData` plus `AlterTableColumn` actions. |
| Alter shared column identity/computed/ordinal semantics | Planned as block | No | Blocked | `Name`, `Ordinal`, `IsIdentity`, `IdentitySeed`, `IdentityIncrement`, `ExpressionSql` changes block. Apply re-validates subset before SQL generation. |
| Alter shared column participating in nonclustered primary key | Yes | Yes | Narrow support | Planner keeps `AlterTableColumn` and emits explicit dependent `ReplacePrimaryKey` actions when executable; otherwise it emits a block. |
| Alter shared column participating in foreign key | Yes | Yes | Narrow support | Planner keeps `AlterTableColumn` and emits explicit dependent `ReplaceForeignKey` actions when executable; otherwise it emits a block. |
| Alter shared column participating in clustered/unsupported primary key | Planned as block | No | Blocked | Dependent primary-key replacement must be executable in the current nonclustered `ReplacePrimaryKey` subset; clustered/unsupported cases block. |
| Alter shared column participating in nonclustered rowstore index | Yes | Yes | Narrow support | Planner keeps `AlterTableColumn` and emits explicit dependent index actions. If dependent index choreography is executable, it emits `ReplaceIndex`; otherwise it emits a block. |
| Alter shared column participating in clustered/unsupported index | Planned as block | No | Blocked | Dependent index replacement must be executable in the current nonclustered rowstore `ReplaceIndex` subset; clustered/unsupported index dependencies block. |
| Alter shared column with live dependency risk | Planned as block | No | Blocked | Feasibility blocks partitioned-table type-shape changes and live `DEFAULT`/`CHECK`/`UNIQUE` dependencies, plus nullability/truncation blockers. |
| Change shared primary key definition | Yes | Yes | Narrow support | Planned as `ReplacePrimaryKey` when executable (shared PK identity in table scope, nonclustered, valid member rows, dependency choreography supported); otherwise blocked. Planner emits explicit dependent FK actions for PK replacement (for example `ReplaceForeignKey`), and deploy executes only those manifest actions. |
| Change shared clustered primary key definition | Planned as block | No | Blocked | Clustered primary-key replacement is explicitly blocked in this slice. |
| Change shared foreign key definition | Yes | Yes | Narrow support | Planned as `ReplaceForeignKey` when executable (same source table scope, same name, coherent member scope/dependencies); otherwise blocked. Deploy executes as drop live FK + add source FK. |
| Change shared index definition | Yes | Yes | Narrow support | Planned as `ReplaceIndex` when executable (same table scope, same name, nonclustered, coherent member scope/dependencies); otherwise blocked. Deploy executes as drop live index + add source index. |
| Change shared clustered index definition | Planned as block | No | Blocked | Clustered index replacement is explicitly blocked in this slice. |
| Change shared advanced index families (columnstore/XML/spatial/full-text) | Planned as block | No | Blocked | Outside current v1 `ReplaceIndex` executability policy. |
| Rename objects | No practical workflow | No | Not supported | Resolve-era rename concepts are not part of current `deploy-plan`/`deploy` contract. |

## What "Deployable" Means Right Now

A manifest is deployable only when all differences were converted into executable `Add*`, `Drop*`, `AlterTableColumn`, `ReplacePrimaryKey`, `ReplaceForeignKey`, or `ReplaceIndex` entries and there are zero block entries.

`deploy` refuses when:

- block entries exist
- source fingerprint mismatches current source workspace
- live fingerprint mismatches current extracted live database
- the manifest expected a missing target database but the database already exists
- the manifest expected an existing target database but the database is missing

## Practical Operator Interpretation

Already usable territory:

- new tables, columns, PKs, FKs, indexes
- live-only PK/FK/index extras by default
- live-only table/column extras when exact object-scoped approvals are provided
- simple safe shared-column nullability/length changes
- shared-column nullability/length changes with dependent nonclustered primary keys (planned as `AlterTableColumn` + explicit `ReplacePrimaryKey`)
- shared-column nullability/length changes with dependent foreign keys (planned as `AlterTableColumn` + explicit `ReplaceForeignKey`)
- shared-column nullability/length changes with dependent nonclustered rowstore indexes (planned as `AlterTableColumn` + explicit `ReplaceIndex`)
- shared primary-key replacement in the supported executable subset
- shared FK replacement in the supported executable subset
- shared nonclustered rowstore index replacement in the supported executable subset

Expected blocked territory:

- changed clustered PK definitions
- changed clustered/advanced index definitions
- identity/computed/expression changes
- type-family transitions
- dependency-heavy shared-column rewrites

## Object-Scoped Destructive Approvals

Global destructive flags are no longer used for planner authorization.
Planner authorization is object-scoped and exact.

Approval kinds:

- `DataDropTable` targeting `schema.table`
- `DataDropColumn` targeting `schema.table.column`
- `DataTruncationColumn` targeting `schema.table.column`

Planner behavior:

- Without exact approval, planner does not emit executable destructive actions and emits precise block entries with missing approval details.
- With exact approval, planner emits explicit destructive actions.
- Truncation is explicit: planner emits `TruncateTableColumnData` followed by `AlterTableColumn`.

Deploy behavior:

- deploy executes only explicit manifest actions.
- no deploy-time destructive inference.
- no ambient/global destructive authorization.

## Roadmap Implied by the Current Engine

Current solid base:

- manifest-driven deploy
- strict source/live fingerprint validation
- block refusal
- explicit statement ordering
- one transaction
- narrow but real `AlterTableColumn` support
- broad add/drop support

Most important expansion areas next:

- broaden `ReplacePrimaryKey` beyond nonclustered-only support
- broaden `ReplaceIndex` beyond nonclustered rowstore only (with explicit safety policy)
- close remaining blocked shared-column alter families via explicit closure classes (`Automatic` / `Staged` / `ManualRequired`) instead of broadening automatic `AlterTableColumn`

## AlterTableColumn Closure Classification

Policy change: automatic `AlterTableColumn` support is intentionally frozen to the current narrow safe subset.
Remaining blocked alter families are classified for closure as follows:

| Blocked alter family | Closure class | Closure path |
|---|---|---|
| `NULL -> NOT NULL` when live data contains `NULL` | `Staged` | Data remediation/backfill first, then rerun `deploy-plan` so normal `AlterTableColumn` becomes executable. |
| Length/precision reduction when live data would truncate | `Staged` | Data remediation (trim/migrate) first, then rerun `deploy-plan`. |
| Type-family transitions (for example `varchar` -> `int`) | `ManualRequired` | Requires explicit data migration/cast strategy outside current automatic alter slice. |
| `IsIdentity`, `IdentitySeed`, `IdentityIncrement` changes | `ManualRequired` | Requires table-rebuild style workflow not represented by current manifest action families. |
| `ExpressionSql` changes / computed-column shape changes | `ManualRequired` | Keep blocked; requires explicit computed-column replacement semantics not in current slice. |
| `Ordinal` reshaping | `ManualRequired` | Column order is not treated as an automatic deploy target in current engine. |
| Partitioned-table type-shape changes | `ManualRequired` | SQL Server restriction; keep blocked unless future explicit staged workflow is introduced. |
| Live dependency blockers on `DEFAULT` / `CHECK` / `UNIQUE` | `Staged` | Close through future explicit dependency action families; until then block with precise reason. |
| Column affected by clustered PK/index or advanced index families | `Staged` | Close via explicit replacement support for clustered/advanced families; do not widen automatic alter behavior. |

Closure class definitions:

- `Automatic`: executable now via existing automatic planner + `AlterTableColumn` subset.
- `Staged`: potentially closable with explicit preconditions and/or additional explicit manifest action families.
- `ManualRequired`: requires manual operator workflow in current architecture.

## Task Status Check

Completed:

- [x] Strict manifest-driven `deploy-plan` -> `deploy` flow
- [x] Source/live instance fingerprint validation
- [x] Block refusal before execution
- [x] Object-scoped destructive approvals (`DataDropTable`, `DataDropColumn`, `DataTruncationColumn`)
- [x] Explicit truncation action (`TruncateTableColumnData`) instead of inferred alter-side truncation flags
- [x] `AlterTableColumn` executable subset
- [x] `ReplacePrimaryKey` action family (plan + deploy), nonclustered subset
- [x] `ReplaceForeignKey` action family (plan + deploy)
- [x] `ReplaceIndex` action family for nonclustered rowstore (plan + deploy)
- [x] Explicit dependent `ReplaceIndex` expansion for executable `AlterTableColumn` when column participates in nonclustered rowstore indexes
- [x] Explicit dependent `ReplacePrimaryKey` / `ReplaceForeignKey` expansion for executable `AlterTableColumn` when column participates in nonclustered PK/FK dependencies
- [x] Explicit dependent FK manifest actions for `ReplacePrimaryKey` (no deploy-time FK inference)
- [x] Shared-object matching by scoped names (not ID-intersection), with ambiguity fail-fast

Remaining:

- [ ] Broaden `ReplacePrimaryKey` beyond nonclustered-only support
- [ ] Clustered/advanced index replacement support
- [ ] Implement staged closure paths for blocked shared-column alter families (without broadening automatic `AlterTableColumn`)

## Principle For Future Work

Broaden by explicit action family, not implicit policy:

- planner emits explicit manifest action
- feasibility/policy defines exact executable subset
- deploy executes only that action family from manifest
- tests prove success, refusal, and rollback behavior

## Bottom Line

MetaSql is currently a real but partial migration engine.

Strong today:

- adding missing objects
- dropping extra objects
- applying a small safe shared-column slice

Not yet broad:

- full replacement of changed constraints/indexes
- broad shared-column rewrite support
- rename workflows

Next operational improvement:

- improve ignored live-only drift reporting details (for example grouped by family/scope in success output).

Next engine improvement:

- expand staged explicit action-family coverage while preserving strict block-on-uncertainty behavior.
