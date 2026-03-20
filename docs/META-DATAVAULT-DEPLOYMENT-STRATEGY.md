# MetaDataVault Deployment Strategy

## Purpose

This document defines the deployment architecture for `MetaDataVault` under real operational conditions.

The design assumption is not a clean lab environment.

It is a live SQL Server estate where any of the following may have happened:

- emergency production fixes
- manual hot patches
- partial restores
- rollback to backup
- corruption recovery
- ad hoc operational repair
- deployment drift
- missing or untrusted deployment history

Under those conditions, deployment correctness cannot depend on trusted lineage.

The deployer must therefore be:

- baseline-free
- proof-based
- conservative
- non-interactive in CI/CD

## Core stance

The deployer is not a historian.

It is a conservative transformer from:

- current live physical state

to:

- current metadata-derived desired physical state

It may automate only the subset of transitions that are provably safe from:

- present-state inspection
- desired-state inspection
- generic deploy traits

It must not guess semantic continuity for ambiguous persistent changes.

## Operating without trusted lineage

### Why trusted lineage cannot be required

In real production estates, the following can invalidate lineage:

- deployment history tables are lost, stale, or bypassed
- generated manifests no longer correspond to the live database
- the database was restored to a point that no longer matches recorded model state
- emergency repair changed the physical shape outside the normal pipeline
- the runtime estate contains partial or inconsistent changes

Once that happens, previous manifests and deployment history are no longer safe foundations for correctness.

They may still be informative, but they are not authoritative.

### What remains authoritative

For deployment purposes, only two states are authoritative:

1. the current live database as it physically exists now
2. the current desired physical state derived from current sanctioned metadata

That is the only sound baseline for automated action.

### Why stable metadata IDs still matter

Stable IDs in sanctioned metadata are still valuable.

They support:

- design-time continuity
- model refactoring discipline
- deterministic generation
- cross-model references
- explicit authored migration/reconciliation metadata

But they do not solve physical identity in a live database under amnesia, because those IDs are not stamped into the generated SQL artifacts or persisted into the deployed schema in a trustworthy universal way.

So:

- metadata IDs are useful for authoring continuity
- metadata IDs are not sufficient proof of live database continuity

## Core architectural split

The deployment architecture should be split into three layers.

## A. DV-aware physicalization and classification

This layer may know Data Vault semantics.

Its job is to convert sanctioned Data Vault metadata into desired physical artifacts and generic deploy traits.

It should produce:

- desired physical tables
- desired columns
- desired constraints
- desired indexes
- dependency graph
- deploy unit graph
- generic deploy traits

Examples of generic deploy traits:

- `persistent`
- `rebuildable`
- `additive-safe`
- `migration-required-for-non-additive`
- `deployment-group`
- `validation-policy`

This layer is where Data Vault semantics belong.

Examples:

- `RawHub` is persistent
- `BusinessPointInTime` may be rebuildable
- `BusinessBridge` may be rebuildable
- `RawHubSatellite` is conservative under amnesia

This layer should not execute deployment logic.

Its job is to describe the target and classify it.

## B. Generic `meta-sql` convergence engine

This layer must know nothing about Data Vault families.

It consumes:

- live database introspection
- current desired physical artifacts
- generic deploy traits
- optional prior manifests/history as hints only

It performs:

- present-state comparison
- safe convergence classification
- drift reporting
- plan generation
- script emission
- optional execution
- recording

This engine should not care whether a table came from:

- Raw Data Vault
- Business Data Vault
- a future warehouse model
- another sanctioned SQL-producing model

It should operate on:

- current physical state
- desired physical state
- deploy traits

## C. Human reconciliation path

Some transitions cannot be safely automated from present-state proof alone.

Those cases must not be guessed.

Instead, the engine should emit a precise reconciliation artifact.

Typical ambiguous cases:

- possible rename vs drop/add
- possible split/merge
- possible payload move
- possible semantic reinterpretation
- destructive change on persistent structures

Human reconciliation may take forms such as:

- provide migration SQL
- adopt current live shape into metadata
- approve controlled replacement
- explicitly classify an object as rebuildable

There must be no runtime interactive prompts inside CI/CD.

Reconciliation must happen before or outside the non-interactive deployment run.

## Why pure schema convergence and semantic data recovery are different

Schema convergence asks:

- can the live database be moved safely toward the target shape?

Semantic data recovery asks:

- what should happen to existing data when continuity is ambiguous?

These are different problems.

The deployer should solve the first automatically only where it is provable.

It should not pretend to solve the second by guessing.

Examples:

- adding a nullable column is usually a convergence problem
- determining whether `CustomerCode` was renamed to `CustomerIdentifier` is a semantic continuity problem
- rebuilding a PIT table is a convergence problem if the object is classified rebuildable
- moving payload from one persistent satellite to another is a semantic data migration problem

## Why the automation boundary is asymmetric

The safe automation boundary is asymmetric.

Some changes are often provable:

- create missing table
- add safe new column
- add supporting index
- rebuild explicitly rebuildable helper object

Some changes are usually not provable under amnesia:

- rename on persistent objects
- split/merge on existing payload
- destructive drop on persistent objects
- semantic reinterpretation of keys or payload

This asymmetry is expected.

The deployer should embrace it, not fight it.

## Convergence versus reconciliation

### Convergence

Convergence is safe automatic movement from present physical state toward target physical state.

Convergence is valid only when:

- the current live state is inspectable
- the target state is known
- the transition is provably safe from present-state proof plus generic deploy traits

Typical convergence operations:

- create missing objects
- add additive-safe columns
- add supporting indexes and constraints
- rebuild explicitly rebuildable objects

### Reconciliation

Reconciliation is human-authored resolution of ambiguity or stateful mismatch.

Reconciliation is required when:

- semantic continuity is uncertain
- data-moving changes are needed
- destructive persistent changes are involved
- the engine cannot prove safe automation

Typical reconciliation outputs:

- migration script
- controlled replacement approval
- metadata update to match current live reality
- explicit deploy-class override

### Why the distinction matters

If convergence and reconciliation are not separated, the deployer tends to become unsafe in one of two ways:

- it blocks too much because every mismatch is treated as fatal
- it guesses too much because every mismatch is treated as automatable

The correct middle path is:

- automate convergence
- surface reconciliation precisely

## Key invariants

- The deployer must function without trusted prior manifest, prior model, or deployment history.
- Prior manifests/history may be used only as optional diagnostics or hints.
- The current live database is the only authoritative current physical state.
- The current metadata-derived target is the authoritative desired state.
- The engine must not guess semantic continuity for ambiguous persistent changes.
- CI/CD deployment must remain non-interactive.
- Partial forward progress is allowed when safe and dependency-valid.
- Reconciliation is outside the runtime deployment loop.
- Data Vault semantics belong in physicalization/classification, not in the generic convergence engine.

## Minimal authored metadata surface

The authored deployment metadata surface should stay small.

Do not build a giant deployment ontology.

The minimum useful surface is:

### 1. Deploy class

Per deploy unit or per object family:

- `persistent`
- `rebuildable`

### 2. Non-additive policy

Minimal flag:

- `migration-required-for-non-additive`

### 3. Deployment group

Minimal ordering control:

- `deployment-group`

This supports layered rollout without inventing a full workflow language.

### 4. Validation policy

Minimal policy reference:

- `validation-policy`

Examples:

- none
- row-count
- dependency-ready
- post-rebuild validation

### 5. Optional reconciliation artifact binding

Minimal way to attach human resolution when needed:

- `reconciliation-artifact`

That artifact might later point to:

- SQL migration script
- controlled replacement approval
- adoption note

This is enough to start.

Anything larger should be justified by proven need.

## Minimal planner and runtime artifact shape

The planner output must not be only pass/fail.

It should classify deploy units into explicit statuses.

### Recommended statuses

- `executable`
- `executable-with-validation`
- `rebuildable`
- `requires-reconciliation`
- `blocked-by-unresolved-dependency`

Optional later statuses:

- `noop`
- `diagnostic-warning`

### Minimal planner artifact

For each deploy unit:

- deploy unit id
- desired artifact name
- current live object match summary
- deploy traits
- status
- change summary
- dependency list
- required validations
- reconciliation reason, if any

### Minimal runtime record

The runtime recorder may store:

- deployment run id
- environment
- deploy unit id
- chosen plan status
- executed artifact path
- execution result
- validation result
- timestamp

This runtime record is useful, but not authoritative for future correctness.

It is telemetry, not baseline truth.

## Planner behavior

The planner should process deploy units independently where possible.

It should not block the whole deployment just because one persistent unit is ambiguous, unless dependency structure requires it.

That means:

- safe units may move forward
- ambiguous units are surfaced for reconciliation
- dependent units are blocked only if their prerequisites are unresolved

This is essential for large estates.

## Default Data Vault behavior

## RDV persistent objects

Raw Data Vault persistent objects should be conservative under amnesia.

Recommended default:

- allow create
- allow safe additive extension
- allow compatible supporting structures
- block ambiguous non-additive transitions unless reconciled

Examples of usually safe convergence:

- create missing hub table
- add nullable descriptive column to persistent satellite when safe
- add supporting non-destructive index

Examples of reconciliation-required cases:

- possible rename of persistent payload column
- possible move of payload between satellites
- key reinterpretation
- destructive removal

## BDV rebuildable helper objects

Rebuildable helper objects may be rebuilt or replaced when explicitly classified rebuildable and when upstream requirements are satisfied.

Typical examples:

- PIT
- Bridge
- future query-helper structures

Recommended default:

- allow drop/recreate or create/swap/replace
- require upstream dependency readiness
- validate after rebuild

## Other BDV objects

Do not assume rebuildable merely because an object is in Business Data Vault.

Rebuildability must be explicitly classified.

Some Business Vault structures are still persistent enough that ambiguous non-additive change should require reconciliation.

## Safe convergence rules

The generic engine should automate only what is provable.

### Usually executable

- missing object create
- additive-safe column add
- supporting index/constraint add
- rebuild of explicitly rebuildable helper

### Usually executable with validation

- operations where structure is safe but runtime verification is prudent
- rebuildable object replacement with dependency checks

### Usually requires reconciliation

- possible rename vs drop/add on persistent object
- split/merge on persistent object
- payload move
- destructive change
- semantic reinterpretation

### Usually blocked by unresolved dependency

- rebuildable helper missing required upstream objects
- dependent object whose parent unit requires reconciliation first

## Recommended decomposition of `meta-sql`

The future `meta-sql` deployer should be decomposed into small explicit stages.

### 1. Desired-state builder

Consumes sanctioned models and outputs:

- desired physical artifacts
- generic deploy traits
- dependency graph

This stage may use model-family-specific physicalizers such as Data Vault physicalizers.

### 2. Live-state introspector

Reads current SQL Server physical state:

- tables
- columns
- constraints
- indexes
- dependencies

### 3. Drift reporter

Reports mismatch between:

- current live state
- desired state

This should include optional prior manifests/history only as hints.

### 4. Convergence classifier

Classifies each deploy unit into:

- executable
- executable-with-validation
- rebuildable
- requires-reconciliation
- blocked-by-unresolved-dependency

### 5. Plan builder

Builds:

- ordered executable subset
- ordered rebuild subset
- blocked/reconciliation subset

### 6. Script emitter

Emits:

- reviewable SQL scripts
- planner report
- reconciliation artifacts

### 7. Optional executor

Applies only the executable/rebuildable validated subset.

No CI/CD prompt loop.

### 8. Recorder

Stores runtime telemetry and artifacts for observability.

Again:

- useful
- not authoritative

## Reconciliation artifact expectations

When a unit cannot be safely converged, the system should emit a precise artifact, not a vague error.

The reconciliation artifact should contain:

- deploy unit id
- live object summary
- target object summary
- reason automation is unsafe
- recommended resolution path

Recommended resolution paths:

- provide migration script
- adopt live shape into metadata
- approve controlled replacement
- reclassify as rebuildable if truly valid

## What the system should not do

- require trusted deployment history for correctness
- assume prior manifest continuity
- use old metadata as mandatory baseline
- infer semantic rename from name similarity alone
- perform destructive persistent changes by default
- pause CI/CD waiting for interactive operator decisions
- model an oversized deployment ontology before there is real need

## Immediate implementation direction

The next practical slice should be:

1. Data Vault-aware physicalization emits deploy units and generic traits.
2. SQL Server introspection captures current live physical state.
3. A planner classifies deploy units into the five core statuses.
4. Script emission covers only executable and rebuildable units.
5. Reconciliation artifacts are emitted for ambiguous persistent units.

That gives useful forward progress without requiring trusted lineage.

## Summary

The correct deployment architecture for this platform is:

- baseline-free
- proof-based
- conservative under amnesia
- Data Vault-aware during physicalization/classification
- Data Vault-agnostic during generic SQL convergence
- explicit about reconciliation rather than guessing continuity

This fits both reality and the platform's strengths:

- metadata models still provide strong design-time continuity
- live-state proof governs deployment safety
- non-interactive CI/CD remains possible
- automation is allowed to move forward where it is provably safe
