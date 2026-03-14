MetaSql Test Surface
====================

Purpose
=======

This document describes the real `MetaSql` test surface as it exists now.

It is split into:

- current implemented behavior to lock down now
- valid next-step scenarios
- future scenarios that need new product behavior

It is not a promise that every scenario below is already implemented.

Current implemented behavior to lock down now
=============================================

These scenarios correspond to behavior that exists in the code today and should be covered directly.

Root, mode, and target context
------------------------------

Current:

- repo root valid
- artifact root valid
- missing `meta-sql.json`
- invalid `rootMode`
- target present in repo config
- target absent in repo config
- target present in artifact config
- target absent in artifact config
- repo mode with inline `connectionString`
- repo mode with `connectionStringEnvVar`
- repo mode with missing `connectionStringEnvVar`
- repo mode with no connection source
- artifact mode with `connectionStringEnvVar`
- artifact mode with missing `connectionStringEnvVar`
- artifact mode with inline `connectionString` only
- artifact mode with both inline `connectionString` and `connectionStringEnvVar`
  Characterization note:
  current behavior prefers the env var.
- artifact mode with no connection source
- resolve refusing artifact mode
- repo mode default migrate layout
- artifact mode default migrate layout

Current behavior note:
- the loader resolves paths and connection sources
- it does not validate repo or artifact layout as a separate concept
- missing desired SQL is caught later by the desired SQL loader
- missing migrate folders are tolerated

Desired SQL loading
-------------------

Current:

- valid `CREATE TABLE`
- valid `ALTER TABLE ... ADD CONSTRAINT`
- valid `CREATE INDEX`
- supported comment/header plus supported SQL
- multiple valid files
- duplicate object definitions
  Characterization note:
  current behavior keeps the last definition seen for the same object key.
- unsupported statement form is ignored unless it causes a missing dependent object later
  Characterization note:
  this is current behavior, not a statement of ideal behavior.
- malformed `CREATE TABLE` rejected when it is encountered as something the loader expects to parse
- malformed `ADD CONSTRAINT` rejected when it is encountered as something the loader expects to parse
- malformed `CREATE INDEX` rejected when it is encountered as something the loader expects to parse
- empty desired SQL directory
- missing desired SQL directory
- one bad file among good files
- schema-qualified naming stability

Traits and policy loading
-------------------------

Current:

- no traits file means safest fallback
- valid traits file
- malformed traits file
- missing trait on object means safest fallback
- persistent trait
- replaceable trait
- additive-only policy
- additive-plus-empty-drop policy
- no destructive unlock by omission

Live DB inspection
------------------

Current:

- live table, column, constraint, and index present
- live table, column, constraint, and index missing
- row count zero
- row count non-zero
- connection failure
- stable naming on repeated reads

Current behavior note:
- permission-failure coverage is only valid if it can be reproduced cleanly in the test environment
- current implementation is SQL Server-specific

Planner behavior
----------------

Current:

- desired present, live missing => add table
- desired present, live present, same shape => no change
- desired absent, live present => manual required unless empty-table replacement is explicitly allowed
- additive missing column => add column
- live extra column => manual required unless empty-table replacement is explicitly allowed
- same column name with differing type or nullability => manual required unless empty-table replacement is explicitly allowed
- replaceable plus `additive-plus-empty-drop` plus empty table => drop and recreate path
- replaceable plus `additive-plus-empty-drop` plus non-empty table => manual required
- missing index => add index
- missing constraint => add constraint
- unresolved manual work blocks dependent index/constraint work
- no rename inference
- no split/merge inference
- no hidden data movement path

Current behavior note:
- extra live indexes and extra live constraints are currently tolerated and should be characterized, not over-explained as deliberate policy unless we decide to formalize that

Plan rendering
--------------

Current:

- `Add table`
- `Add column`
- `Add index`
- `Add constraint`
- `Drop table`
- `Manual required`
- `Blocked`
- no framework-style status/class output
- mixed output remains operator-readable

Blocker identity and active script contract
-------------------------------------------

Current:

- same blocker => stable id
- different object => different id
- different blocker kind => different id
- materially changed blocker => changed id
- valid header plus SQL body
- ASCII
- UTF-8
- missing required header lines rejected
- malformed header values rejected
- empty file rejected
- comment-only file rejected
- placeholder-only file rejected
- NUL and unsupported control characters rejected
- active script becomes stale when blocker is gone
- blocker-id match but kind/object mismatch rejected
- archive scripts ignored by active input

Current behavior note:
- duplicate active scripts for the same blocker are not specially classified today
- baseline and target scripts claiming the same blocker are not specially classified today
- those should be characterized if tested now

Resolve workflow
----------------

Current:

- no issues found => summary, no prompts
- unresolved issues => walked in one session
- stale scripts => walked in one session
- mixed matching/stale/unresolved => only stale and unresolved are prompted
- choose baseline placement
- choose target placement
- archive stale script
- stub created with required header
- resolve refused in artifact mode
- prompt language is plain

Current behavior note:
- interaction coverage is partly best treated as characterization unless a dedicated interaction seam is added

Deploy-test and deploy
----------------------

Current:

- deploy-test in repo mode
- deploy-test in artifact mode
- deploy in repo mode
- deploy in artifact mode
- matching scripts reported
- stale scripts reported
- active stale scripts visibly block normal deploy
- malformed active script fails plainly
- placeholder active script fails plainly
- deploy runs matching scripts first, then reinspects, then applies structural SQL
- blockers remaining after scripts => deploy refuses
- unresolved manual issues before execution => deploy refuses
- active stale scripts => deploy refuses
- script execution failure aborts
- structural execution failure aborts
- no auto-drop of non-empty tables

Script executor
---------------

Current:

- single SQL script file
- multiple script files in path order
- plain `GO`-separated batches
- bad SQL fails plainly
- executor does not make policy decisions
- lowercase `go`

Characterization-only current behavior:

- `GO` in comments
- `GO` in strings

These are current executor behaviors worth locking down if tested, but they are not a sign that the executor is meant to become smarter.

Artifact writing
----------------

Current:

- package one selected target
- package multiple selected targets
- unselected target omitted
- baseline active scripts copied
- `target/<env>` active scripts for selected targets copied
- archive omitted
- artifact config written
- env-var connection packaging preserved
- target absent from packaged artifact fails plainly when loaded later

Current behavior note:
- current artifact writing copies active inputs but does not validate them before packaging

Valid next-step scenarios
=========================

These are immediate hardening steps that make sense next, but they are not all current behavior yet.

Next-step:

- split the current test suite by responsibility instead of keeping it concentrated in `PreflightTests.cs`
- add more explicit characterization tests for:
  - duplicate desired object definitions
  - extra live indexes
  - extra live constraints
  - duplicate active scripts for the same blocker
  - same blocker claimed by baseline and target script
  - `GO` in comment and string cases
- add command-level tests for `deploy-test`, `deploy`, and `resolve` with controlled seams instead of relying mostly on smoke runs
- tighten active-script reporting further if duplicate/multi-claim cases need to become explicit operator-facing failures
- add narrow packaging validation in `MetaSqlArtifactWriter`
  - malformed active script blocks packaging
  - placeholder active script blocks packaging
  - stale active script blocks packaging

Future scenarios requiring new product behavior
===============================================

These are valid long-term scenarios, but they should not be treated as current contract.

Future:

- separate repo-layout validation as a first-class behavior
- separate artifact-layout validation as a first-class behavior
- richer duplicate-script policy
- richer target-packaging policy beyond the current selected-target copy model
- dedicated packaging CLI surface
- broader SQL Server permission-failure coverage if a repeatable harness is added
- general SQL parsing
- rename inference
- split/merge inference
- heavyweight migration execution
- hidden data movement strategies
- smart executor behavior such as retry, batch strategy, or migration intelligence

Recommended harness cleanup
===========================

The current suite is still concentrated in `PreflightTests.cs`.

That is acceptable temporarily, but the next cleanup step should be to split tests by responsibility, for example:

- `TargetContextLoaderTests.cs`
- `DesiredSqlLoaderTests.cs`
- `TraitsLoaderTests.cs`
- `PlannerTests.cs`
- `PlanRenderingTests.cs`
- `BlockerIdentityTests.cs`
- `BlockerScriptValidationTests.cs`
- `ResolveWorkflowTests.cs`
- `DeploySessionTests.cs`
- `ArtifactWriterTests.cs`

Summary
=======

Moved out of current:

- artifact packaging validation as if it already blocks malformed, stale, or placeholder scripts
- missing repo/artifact layout as if loader already validates those as separate behaviors
- any implication that the multi-file harness already exists

Real current MetaSql test surface:

- repo and artifact target loading
- narrow desired SQL loading
- safest traits fallback
- conservative planner behavior
- operator-readable rendering
- blocker identity
- active script validation and matching
- resolve repo-only workflow
- deploy-test and deploy behavior
- selected-target artifact writing

Recommended next harness cleanup:

- split `PreflightTests.cs` by responsibility before the suite grows much further
