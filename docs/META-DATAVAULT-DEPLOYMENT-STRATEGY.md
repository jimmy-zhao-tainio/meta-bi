# MetaDataVault deployment strategy

## Purpose

This document defines the intended deployment strategy for Data Vault in this platform.

It combines:

- general lessons from modern schema-deployment practice
- Data Vault-specific operational behavior
- the strengths of this platform being fully metadata-model driven

The goal is not to bolt EF-style migrations onto Data Vault, and not to copy existing Data Vault automation tools literally.

The goal is:

- metadata-aware deployment planning
- explicit handling of schema changes with data
- deployment behavior driven by sanctioned model semantics
- a clean decomposition into a future `meta-sql` deployer/toolchain

## Design stance

The platform should not treat SQL deployment as a generic diff problem first.

It should treat SQL deployment as:

1. sanctioned metadata intent
2. sanctioned physicalization intent
3. deployable physical state transition
4. runtime-safe execution against a database with existing data

That distinction matters because this platform already knows more than a normal SQL diff tool:

- whether an object is Raw or Business
- whether an object is a Hub, Link, Satellite, PIT, or Bridge
- which columns are semantic and which are implementation-owned technical columns
- which structures are persistent history and which are derived helpers

This should be used aggressively.

## Core principle

Deployment behavior must be driven by object semantics, not inferred from raw SQL text alone.

That means:

- `RawHub` is not just a table
- `BusinessPointInTime` is not just a table
- `RawHubSatellite` is not just a set of columns

These objects carry different deploy semantics and should be handled differently.

## What we learn from general schema deployment

Modern deployment practice contributes the following rules:

- additive change is the cheap case
- rename, split, merge, and shape-change are not cheap cases
- destructive change must never be implicit
- schema changes with data require explicit migration intent
- production deploys need reviewable artifacts
- deploys need version awareness and drift awareness
- idempotent execution is useful but not sufficient
- fix-forward is usually better than fantasy rollback

These rules should be adopted, but expressed in metadata terms.

## What we learn from Data Vault operations

Data Vault contributes the following rules:

- Raw Vault is persistent, auditable, and history-preserving
- Raw Vault should strongly prefer additive evolution
- Business Vault is derived and operationally more rebuildable
- PIT and Bridge are the most rebuildable structures of all
- load and deploy order matters by dependency
- compatibility across layers matters more than clever diffing

These rules should also be adopted, but without baking product-specific workflow assumptions into the sanctioned models.

## The platform advantage

A normal migration system sees:

- old SQL model
- new SQL model
- maybe a diff

This platform can see:

- sanctioned semantic model
- sanctioned implementation model
- generated physical model
- object family
- object dependency graph
- source provenance
- deployment intent
- migration intent

That means deployment can be planned by object meaning rather than by fragile SQL guesswork.

## Deployment semantics classes

The platform should classify Data Vault objects into deployment-semantics classes.

This classification may begin as an engine invariant, but it should be stated explicitly and eventually become sanctioned metadata.

### 1. Persistent additive

Objects in this class are expected to preserve history and should evolve cautiously.

Typical examples:

- `RawHub`
- `RawLink`
- `RawHubSatellite`
- `RawLinkSatellite`
- most persistent Business Vault tables when they hold historized derived state

Expected behavior:

- create is allowed
- additive columns are allowed
- additive indexes and constraints are allowed
- destructive changes are blocked by default
- incompatible type changes require explicit migration intent

### 2. Persistent with explicit migration

Objects in this class still preserve data, but some changes are only acceptable if accompanied by an explicit migration plan.

Typical examples:

- any vault table with data-bearing column changes
- any table where column splits/merges/backfills are required
- any table where a rename must preserve data

Expected behavior:

- deploy is blocked unless an explicit migration exists
- migration must define data motion and sequencing
- generator/deployer must not guess

### 3. Derived rebuildable

Objects in this class are derived artifacts and are operationally acceptable to rebuild from upstream vault state.

Typical examples:

- `BusinessPointInTime`
- `BusinessBridge`
- other future helper/query structures
- some fully derived Business Vault tables

Expected behavior:

- create is allowed
- replace/rebuild is allowed
- drop-and-recreate or create-swap-drop is acceptable
- data preservation inside the object is not the primary concern

## Default deployment semantics by DV object family

This is the recommended default mapping.

| Object family | Default deploy semantics | Notes |
|---|---|---|
| Raw Hub | persistent additive | history-preserving |
| Raw Link | persistent additive | history-preserving |
| Raw Hub Satellite | persistent additive | history-preserving |
| Raw Link Satellite | persistent additive | history-preserving |
| Business Hub | persistent additive | may later vary by modeling stance |
| Business Link | persistent additive | may later vary by modeling stance |
| Business Hub Satellite | persistent additive | derived but often retained |
| Business Link Satellite | persistent additive | derived but often retained |
| Business Reference | persistent additive | depends on platform policy |
| Business PIT | derived rebuildable | query helper |
| Business Bridge | derived rebuildable | query helper |

The important part is not whether every line is perfect on day one.

The important part is that the platform stops pretending all tables have the same operational meaning.

## Change classes

The deploy planner should classify changes into explicit categories.

### A. Safe additive

Examples:

- create table
- add nullable column
- add non-null column with safe default and explicit strategy
- add index
- add FK after data is already compatible

Default behavior:

- allowed automatically for compatible object classes

### B. Compatible physical refinement

Examples:

- add non-destructive supporting constraints
- rename FK or index object
- secondary-object rename or rehash of long identifier

Default behavior:

- allowed automatically when data safety is unaffected

### C. Explicit migration required

Examples:

- rename column with existing data
- split one column into two
- merge two columns into one
- change data type with possible truncation/rounding
- change nullability from nullable to non-nullable with existing rows
- move payload between vault structures

Default behavior:

- blocked unless explicit migration metadata is provided

### D. Rebuildable replacement

Examples:

- PIT reshaping
- Bridge reshaping
- fully derived helper-table replacement

Default behavior:

- allowed for derived-rebuildable object classes

### E. Destructive blocked

Examples:

- drop persistent table with data
- drop persistent column with data
- incompatible PK/UQ change on persistent history structures
- destructive refactor with no migration path

Default behavior:

- blocked

## Raw Vault strategy

Raw Vault deployment should be conservative.

### Rules

- prefer additive evolution
- never silently rewrite history
- never silently drop historical payload
- require explicit migration for shape changes with data
- treat business-key reinterpretation as a major event, not a casual diff

### Practical policy

- new hub/link/satellite: create
- new descriptive columns on satellites: additive deploy
- new constraints/indexes: allowed with caution
- rename of semantic columns: explicit migration required
- removal of payload or key parts: blocked unless an approved migration path exists

### Why

Raw Vault is the persistent audit-preserving core.

If the platform treats Raw Vault like a replaceable reporting layer, it breaks the core value of the vault.

## Business Vault strategy

Business Vault deployment should be more flexible, but not careless.

### Rules

- separate persistent derived state from rebuildable helper state
- allow rebuild behavior where semantics permit it
- keep dependency order explicit

### Practical policy

- persistent Business Vault tables: use the same additive-plus-migration rules as Raw Vault
- PIT/Bridge/helper structures: allow replace/rebuild
- materialized derived tables may opt into rebuildable deployment only when explicitly classified that way

### Why

Business Vault is still important, but some of its tables are not the system-of-record for history.

The platform should exploit that rather than treating PIT/Bridge as fragile irreplaceable assets.

## Schema changes with data

This is the real problem class.

The platform must model it explicitly.

### Rename

A rename must not be treated as drop+add.

The migration model should express:

- source object
- target object
- whether this is semantic rename, physical rename, or both

For SQL Server deploy, this likely means:

- physical rename operation where safe
- or create new shape + copy + swap behavior where required

### Column split

Example:

- one descriptive column becomes two columns

The migration model should express:

- source column
- target columns
- transform logic
- backfill order
- cutover rule

### Column merge

Example:

- two legacy descriptive columns become one standardized column

The migration model should express:

- source columns
- target column
- coalescing/merge logic
- post-migration validation

### Type change

Examples:

- `nvarchar(200)` to `nvarchar(100)`
- `decimal(18,4)` to `decimal(18,2)`
- nullable to non-nullable

The migration model should express:

- compatibility classification
- whether backfill/cleanup is required
- whether this is blocked, allowed, or rebuildable

### Table refactor

Examples:

- moving payload from one satellite to another
- replacing one derived structure with another

The migration model should express:

- whether the object is persistent or rebuildable
- whether data motion is required
- whether old and new can coexist during cutover

## Recommended rollout pattern

The deployer should prefer expand-contract style rollout for persistent objects.

### Expand

- create new objects/columns
- keep old ones in place
- preserve backward compatibility

### Migrate/backfill

- populate new structures
- validate row counts and key coverage

### Cutover

- switch producers/consumers
- add constraints only when data is ready

### Contract

- remove old structures only after explicit approval and sufficient stabilization

For rebuildable objects, a simpler replace/rebuild flow is acceptable.

## What should be metadata-driven

The following should become explicit sanctioned metadata concepts, not remain buried in deployer code.

- deployment semantics class
- object rebuildability
- allowed change policy
- migration requirement policy
- dependency order
- deployment batch/group
- compatibility window expectations
- validation requirements
- data-motion steps
- post-deploy actions

## Sketch of a DV migration model

This does not need to be implemented immediately as a final sanctioned model, but the platform should head in this direction.

### Candidate model name

- `MetaDataVaultDeployment`
- or a more generic future `MetaSqlDeployment`

### Core entities

#### DeploymentUnit

Purpose:

- one deployable object or tightly coupled object family

Examples:

- one Raw Hub
- one Raw Link
- one Raw Hub Satellite
- one Business PIT

Properties:

- `Name`
- `DeployKind`
- `ExecutionOrder`
- `FailurePolicy`

#### DeploymentUnitObject

Purpose:

- binds the deployment unit to one sanctioned DV object

Relationships:

- `DeploymentUnit -> RawHub | RawLink | RawHubSatellite | ...`

#### SchemaChange

Purpose:

- describes an intended state change between deployed and target states

Properties:

- `ChangeKind`
- `RiskLevel`
- `RequiresMigration`
- `IsDestructive`
- `IsRebuildable`

#### MigrationPlan

Purpose:

- groups explicit steps required for a stateful change

Properties:

- `Name`
- `AppliesToChangeKind`
- `ApprovalRequired`

#### MigrationStep

Purpose:

- one ordered step in a migration

Properties:

- `Ordinal`
- `StepKind`
- `SqlText` or later a structured SQL-operation model
- `RollbackKind`
- `IsOnlineCompatible`

Step kinds could include:

- create
- alter
- backfill
- validate
- cutover
- drop
- rebuild

#### ValidationRule

Purpose:

- declarative checks before or after migration

Examples:

- row-count check
- null check
- uniqueness check
- orphan check

#### DeploymentRecord

Purpose:

- runtime/deployment history entry

Properties:

- `DeploymentId`
- `Environment`
- `AppliedAt`
- `ArtifactVersion`
- `Result`
- `DriftStatus`

This runtime state may eventually live in a runtime/deployment model rather than the design-time model.

## When the migration model is required

The platform should not require explicit migration metadata for every change.

It should require it only when the deploy planner classifies a change as:

- stateful and non-additive
- persistent and incompatible
- destructive but approved

That keeps the platform pragmatic.

## Recommended decomposition into a `meta-sql` deployer

The SQL deployer should be decomposed into explicit stages.

### 1. Model loader

Loads:

- sanctioned DV workspace
- sanctioned implementation workspace
- sanctioned data type conversion workspace
- optional deployment/migration workspace

### 2. Physicalizer

Produces the intended physical SQL model:

- tables
- columns
- constraints
- indexes
- dependency graph

This is not deployment yet.

It is the intended physical state.

### 3. Database introspector

Reads current target state from SQL Server:

- current objects
- columns
- constraints
- indexes
- optional deployment history table

### 4. Drift detector

Determines:

- does target match previously deployed intent
- are there unmanaged changes
- is deployment safe to continue

### 5. Change classifier

Compares target state and intended state, then classifies each change as:

- additive
- compatible refinement
- explicit migration required
- rebuildable
- blocked destructive

This is the most important stage.

### 6. Plan builder

Builds an ordered deployment plan:

- preconditions
- DDL steps
- migration steps
- validation steps
- post-deploy steps

### 7. Artifact writer

Emits:

- reviewable SQL scripts
- deployment plan report
- warnings and risk summary

This should be the main production artifact, not a hidden internal detail.

### 8. Executor

Executes the approved plan:

- version-aware
- idempotent where possible
- ordered by dependency
- transactional only where safe and practical

### 9. Recorder

Records:

- deployed artifact/version
- applied plan
- timestamps
- warnings
- validation result

## Why a generic `meta-sql` deployer still makes sense

A future `meta-sql` deployer should be generic at the pipeline level, but not blind to semantics.

The right split is:

- generic deploy pipeline stages
- specialized change classification plugins per sanctioned model family

That means:

- the pipeline engine can be generic
- the Data Vault classification rules can remain Data Vault-aware
- later SSDT or warehouse models can reuse the same deploy pipeline with different classifiers

This is much better than one giant Data Vault-only deployer and much better than one semantics-blind SQL diff tool.

## Proposed immediate next steps

### 1. Write down deploy classes as engine invariants

Before new sanctioned models exist, make these explicit in code/docs:

- Raw Hub/Link/Satellite = persistent additive
- PIT/Bridge = derived rebuildable

### 2. Add a deploy planner before an executor

Do not jump straight to mutation.

First implement:

- introspection
- change classification
- reviewable deployment plan output

### 3. Support additive deployment first

The first safe slice should support:

- create database
- create missing objects
- add safe additive columns/constraints
- reject unsupported stateful changes with explicit reasons

### 4. Add migration-plan support next

After additive deploy is solid:

- introduce explicit migration metadata for non-additive persistent changes

### 5. Add rebuild support for PIT/Bridge

This is the easiest meaningful non-additive deploy behavior because it fits Data Vault semantics well.

## Non-goals

This strategy should not aim to:

- infer semantic renames from SQL names alone
- allow implicit destructive changes
- hide deployment drift
- mutate persistent vault history casually
- pretend all DV object families are operationally equivalent

## Summary

The right deployment strategy for this platform is:

- metadata-aware, not SQL-text-first
- stateful, not naive-diff-only
- conservative for Raw Vault
- selectively rebuildable for Business Vault helper structures
- explicit about migration when data is at risk
- decomposed into a reusable `meta-sql` deploy pipeline with DV-aware change classification

That is the path that uses the platform's real strength:

the sanctioned models already know what the database objects mean.
