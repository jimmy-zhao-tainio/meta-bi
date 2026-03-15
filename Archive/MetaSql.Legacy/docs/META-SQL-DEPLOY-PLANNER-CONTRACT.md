# Meta SQL Deploy Planner Contract

## Purpose

`meta-sql` is generic deploy plumbing.

It takes:

- desired physical SQL shape from an upstream physicalizer
- a tiny set of generic traits
- live database introspection

It emits a conservative deployment sheet in plain database terms.

It is a structural deployer, not a data migration engine.

It does not know Data Vault families directly. If a model family has special meaning, that meaning must be reduced upstream into plain SQL targets plus tiny generic traits before `meta-sql` sees anything.

## What `meta-sql` does

`meta-sql` owns structural SQL convergence only.

It may plan and execute bounded structural SQL such as:

- `CREATE TABLE`
- `ALTER TABLE ... ADD COLUMN`
- `CREATE INDEX`
- `ADD CONSTRAINT`
- `DROP TABLE` only under strict policy

It must work without requiring trusted prior lineage.

That means it must be able to operate from:

- desired physical SQL shape
- tiny generic traits
- current live database state

Prior manifests, prior model snapshots, and deployment history may exist, but they are hints only. They are never correctness prerequisites.

## What `meta-sql` does not do

`meta-sql` must not invent heavyweight data movement.

That includes:

- create-copy-swap backed by `INSERT ... SELECT`
- shadow-table refill
- giant copy-based replacement
- batching, logging, or recovery-model cleverness
- ETL-like migration choreography

The dangerous thing is not `DROP` or `CREATE` by themselves.

The dangerous thing is hidden data motion masquerading as a harmless "swap" or "refresh".

If a target transition would require moving significant existing data, `meta-sql` must stop short of automation and say so plainly.

## Tiny trait contract

The trait contract should stay small.

The useful minimum is:

- state class:
  - `persistent`
  - `replaceable`

- auto policy:
  - `additive-only`
  - `additive-plus-empty-drop`

- optional:
  - validation profile
  - dependency or grouping marker

`replaceable` must be defined carefully.

It means destructive structural replacement may be allowed by policy.

It does **not** mean automatic repopulation, refill, or hidden data-motion tactics.

## Safety rules

- safe additive change on persistent objects may be automated
- replaceable objects may be dropped and recreated only when policy allows and the table is empty or otherwise safely disposable
- ambiguous or non-additive persistent changes must stop
- unresolved dependencies must stop downstream work
- no interactive runtime decisions are allowed in CI/CD

## Automatic DROP boundary

`meta-sql` must refuse automatic `DROP TABLE` on non-empty tables in the normal architecture.

If a table contains rows, destructive or data-bearing transitions require explicit migration SQL outside the generic deployer.

The normal contract is:

- empty replaceable table may be dropped if policy allows
- non-empty table may not be automatically dropped

If an exceptional mode ever exists for disposable local environments, it must be clearly fenced away from normal build and promoted release behavior.

## Visible plan style

The visible plan must be operation-led and human-readable.

Good plan language looks like this:

Add table:
    CREATE TABLE dbo.H_Customer (...)

Add column:
    ALTER TABLE dbo.S_CustomerProfile
        ADD CustomerName nvarchar(200) NULL

Add index:
    CREATE INDEX IX_S_CustomerProfile_LoadDate
        ON dbo.S_CustomerProfile (...)

Drop table:
    DROP TABLE dbo.PIT_CustomerSnapshot

Manual required:
    dbo.S_CustomerProfile
        Refused DROP COLUMN LegacyName because the table contains rows.

Blocked:
    dbo.Bridge_CustomerSnapshot
        Waiting on dbo.S_CustomerProfile

Bad plan language looks like this:

- `apply`
- `rebuild`
- `reconcile`
- `status: executable`
- `class: additive`

The top-level plan should speak in DB terms first.

## What the planner refuses to infer

`meta-sql` must not guess:

- rename versus drop-and-add
- split versus merge
- payload movement
- semantic reinterpretation
- destructive persistent change with unclear continuity

If the live state and target state differ in a way that cannot be safely resolved through bounded structural SQL alone, the planner must emit `Manual required` and stop.

## Data Vault defaults

For Data Vault, the upstream physicalizer may know:

- RDV versus BDV
- Hub versus Link versus Satellite
- PIT and Bridge-like helper semantics

`meta-sql` must not know any of that directly.

The recommended defaults are:

- RDV normally maps to `persistent` + `additive-only`
- PIT/Bridge-like helpers may be marked `replaceable`
- other BDV objects are not automatically replaceable just because they live in BDV

And again:

`replaceable` never implies hidden refill choreography.

## Plain examples

Add table:
    CREATE TABLE dbo.H_Customer (...)

Add column:
    ALTER TABLE dbo.S_CustomerProfile
        ADD CustomerName nvarchar(200) NULL

Add index:
    CREATE INDEX IX_S_CustomerProfile_LoadDate
        ON dbo.S_CustomerProfile (...)

Manual required:
    dbo.PIT_CustomerSnapshot
        Target shape change would require data movement to repopulate existing contents.

    dbo.S_CustomerProfile
        Non-additive change on persistent structure cannot be inferred safely from live shape.

Blocked:
    dbo.Bridge_CustomerSnapshot
        Waiting on dbo.S_CustomerProfile

## Scope boundary

This document defines the architectural contract, not the internal code structure.

The implementation will almost certainly have richer internal representations for comparison, planning, grouping, and script generation.

That is fine.

Those are implementation detail unless they become truly stable external concepts.

The stable contract is only this:

`meta-sql` is a branch-agnostic, generic structural deployer. It reads desired physical SQL, tiny traits, and live DB state. It emits a conservative deployment sheet in DB terms. It refuses hidden data movement, automatic drop of non-empty tables, and guessed non-additive persistent transitions.
