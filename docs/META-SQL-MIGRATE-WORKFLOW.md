# Meta SQL Migrate Workflow

## Purpose

This document explains where migration SQL lives, how `meta-sql` finds blockers, how people resolve them with normal `.sql` files, and how build and release consume those files.

The goal is simple:

- `meta-sql` finds blockers
- `meta-sql` emits SQL script stubs for those blockers
- developers or release owners fill in the SQL
- build validates and packages the scripts
- release deploys from the packaged artifact
- next preflight matches scripts back to the blocker they were written for

This is meant to be much easier to work with than "the publish failed, now go spelunk in generated projects."

## Folder layout

Use this layout:

```text
deploy/
    migrate/
        baseline/
        target/
            qa/
            prod/
            local/
        archive/
```

## What the folders mean

### `baseline/`

This is the normal supported upgrade path for the current codebase.

These scripts are committed as part of the normal release tree and are expected to apply broadly wherever the target environment is on the supported path for this codebase.

Use `baseline/` for scripts such as:

- persistent schema corrections that are part of the normal release path
- explicit migration SQL that many environments on the supported path will need

### `target/<env>/`

This is for scripts needed because a particular target currently has a specific blocker or odd live state.

Examples:

- QA has a table shape left behind by rehearsal
- PROD has a manual hotfix lineage that needs an explicit transition
- local has stale disposable state that needs a one-off cleanup

These scripts are still committed in the same repo and in the current release-candidate tree.

They do **not** imply environment branches.

They mean:

- this script resolves a target-specific live state
- the target state it resolves is `qa`, `prod`, or `local`

A person may rehearse a `target/prod` script locally against a schematic copy of PROD, but the script still belongs in `target/prod` because that is the target state it resolves.

### `archive/`

This is history, not the normal release surface.

Archived scripts are still useful for:

- historical investigation
- stale local BI developers catching up
- understanding old target-specific repairs

But build and release should not treat `archive/` as normal release input.

The normal release surface is:

- `baseline/`
- `target/<env>/`

not:

- `archive/`

## Script format

Scripts are normal `.sql` files in source control.

`meta-sql` should not require a migration DSL.

Each script only needs a tiny header that `meta-sql` can parse and match back to the blocker it was created for.

Example:

```sql
-- meta-sql: 1
-- blocker-id: prod:dbo.S_CustomerProfile:drop-column:LegacyName
-- target: prod
-- summary: Drop legacy column after explicit business sign-off

ALTER TABLE dbo.S_CustomerProfile
    DROP COLUMN LegacyName;
```

The important part is not that `meta-sql` understands arbitrary SQL in the general sense.

The important part is that it can:

- validate the header
- lint the script envelope
- match the script to the blocker it emitted
- re-check live state on the next preflight

## What preflight does

Preflight compares:

- desired physical SQL shape
- tiny traits
- live target database state

It then emits an operator-readable deployment sheet.

For executable work, it emits concrete SQL operations.

For blocked or unsafe work, it emits blocker entries such as:

Manual required:
    dbo.S_CustomerProfile
        Refused DROP COLUMN LegacyName because the table contains rows.

At that point, `meta-sql resolve` can emit a stub `.sql` file into the right folder with the tiny header already filled in.

## What developers or release owners do

They edit the generated stub like a normal SQL script.

They do not need to learn a special migration language.

They only need to:

- keep the required header intact
- write the SQL they want reviewed and run
- commit it in the correct repo tree

## What build does

Build should:

1. fetch the repo tree for the release candidate
2. validate migration script headers
3. lint the script envelope
4. run the meta tools
5. package metadata, generated SQL, and migration scripts into a release artifact

Build should be branch-agnostic.

It only cares about the exact repo tree or commit being built into the artifact.

It should not care whether the repo uses:

- GitHub Flow
- release branches
- long-lived integration branches
- some other branch policy

Branch strategy is orthogonal.

Repo paths should mean the same thing on every branch.

## What release does

Release should fetch only the packaged artifact and deploy from that packaged copy.

Release should not depend on reading git directly.

That means:

- source control is a build concern
- deployment is an artifact concern

The release pipeline should ideally introspect the target DB using pipeline or service credentials, not ad hoc developer elevated access.

## How `meta-sql` matches scripts back to blockers

The guarantee does not come from fully understanding arbitrary SQL.

The guarantee comes from:

- blocker identity in the script header
- next preflight re-checking the resulting live state

That means the workflow is:

1. preflight finds blocker
2. stub script is emitted with blocker header
3. human fills script
4. build validates the header and packages the script
5. release runs the script
6. next preflight checks whether the blocker is gone, still unresolved, or stale

Possible outcomes on the next preflight:

- covered and resolved
- still unresolved
- stale because the target state changed
- stale because the live DB changed differently

That is enough.

`meta-sql` does not need to become a full SQL theorem prover.

## Old script policy

Old scripts lurking around need explicit handling.

The key rule is:

- `deploy/migrate/archive` is history, not normal release input

So:

- build and release should consider `baseline/` plus `target/<env>/`
- `archive/` should be ignored for normal release planning and packaging unless a special historical recovery workflow says otherwise

Archive is not trash.

But archive must not dictate the normal release architecture.

## How scripts move to archive

Move scripts to `archive/` when they are no longer part of the normal supported upgrade path.

Typical reasons:

- the blocker is long gone in normal environments
- the release train has moved past that state
- the script is still historically useful but no longer part of normal release input

## Local rehearsal

Local rehearsal is still useful.

A person may run:

- baseline scripts locally
- `target/qa` scripts against a schematic QA copy
- `target/prod` scripts against a schematic PROD copy

That rehearsal does not change folder semantics.

The script belongs to the target state it resolves, not to the place where it happened to be rehearsed.

## Optional local hard mode

It is acceptable to have a brutal convenience switch for stale disposable local environments, such as a hard truncate-and-drop mode.

But that must be clearly fenced:

- local/dev only
- not part of the normal release contract
- impossible or extremely hard to run in CI or promoted environments

This is convenience tooling, not release architecture.

## Summary

The intended workflow is plain:

- metadata and migration SQL live in the source repo
- `meta-sql` preflight finds blockers
- `meta-sql resolve` emits SQL stubs with tiny headers
- humans fill the SQL
- build validates and packages the scripts
- release consumes only the packaged artifact
- next preflight matches scripts back to blockers by header plus resulting-state re-check
- old scripts move to `archive/` when they are no longer part of the normal release path

This keeps the system boring, reviewable, and operator-friendly.
