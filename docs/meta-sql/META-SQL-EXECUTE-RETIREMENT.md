# MetaSql Execute Retirement Note

## Purpose

This note records the grounded finding behind the `meta-sql execute` correction.

`MetaSql` is a sanctioned model deployment surface. It owns extracting live SQL Server schema into `MetaSql`, planning drift against a source `MetaSql` workspace, writing a deploy manifest, and applying that manifest. It should not also be a general SQL script runner for setup, cleanup, seed data, or third-party installers.

## Finding

`meta-sql execute` is implemented as a standalone SQL Server mini-runner in `MetaSql.Cli`.

It:

- reads SQL from `--file` or `--query`
- substitutes `$(NAME)` tokens from `--var`
- splits batches on `GO`
- executes each batch through `Microsoft.Data.SqlClient`
- renders result sets unless `--quiet` is supplied

It does not:

- load a `MetaSql` workspace
- read or write a deploy manifest
- validate source/live fingerprints
- use the `MetaSql` difference planner
- restrict execution to modeled schema changes
- represent SQLCMD as a product model

So the command is operationally useful demo glue, but it is outside the product boundary implied by the `MetaSql` model/deploy architecture.

## Intended MetaSql Surface

The supported product surface should remain:

```cmd
meta-sql deploy-plan --source-workspace <path> --connection-env <name> --out <manifest-path> [approvals]
meta-sql deploy --manifest-workspace <path> --source-workspace <path> --connection-env <name>
```

`deploy-plan` and `deploy` are model-to-database operations. Random SQL execution is setup or verification tooling.

## Current Usage Inventory

The current repository still uses `meta-sql execute` in demos, generated command docs, and one CLI help test. The usage falls into three groups.

| Group | Current uses | Ownership classification |
| --- | --- | --- |
| Setup and cleanup SQL | Demo database create/drop scripts in DataVault, DataWarehouse, Pipeline, Orchestration, DataQuality, client corpus, and the multidimensional hierarchy query demo. | Setup tooling, not `MetaSql` product behavior. Convert to `sqlcmd`, PowerShell SQL client setup scripts, or domain-owned setup commands where they already exist. |
| Verification SQL | A few demo checks use inline SQL to prove expected rows or failure evidence. | Demo verification tooling. Prefer `sqlcmd -b -Q` or a small demo-specific verification script. |
| Generated DataQuality SQL deployment | `meta-convert data-quality-to-sql` emits raw SQL and demos currently apply it with `meta-sql execute`. | Real product boundary question. Either DataQuality owns an install/deploy path for its generated pack, or the converter should eventually emit a modeled `MetaSql` workspace/manifest path. Until that decision, demos can still use setup tooling to apply generated SQL. |

The DataQuality group is the only one that deserves product design attention. The rest are plain setup/cleanup convenience.

## DataQuality Nuance

The DataQuality case is not merely "demo SQL that should use `sqlcmd`."

`meta-convert data-quality-to-sql` currently emits target SQL that includes:

- generated `dq` review views
- `dq.v_DataQualityReview`
- operational tables such as `dbo.RunLog` and `dbo.FindingLog`
- indexes and foreign keys for that operational pack
- `dbo.Run` and `dbo.Findings` stored procedures
- idempotent upgrade checks around schemas, tables, columns, indexes, and modules

That shape used to be broader than the `MetaSql` deploy model, which was intentionally centered on tables, columns, primary keys, foreign keys, indexes, and a strict manifest-driven drift workflow. `MetaSql` now has a first explicit SQL module slice for views and stored procedures, represented as schema-scoped module definitions and deployed through the same manifest discipline. That is not the same thing as accepting arbitrary scripts.

So the right conclusion is not "DataQuality was wrong to need deployment." It is:

- `meta-sql execute` masked a missing product decision.
- DataQuality has legitimate deployable SQL surfaces; tables, indexes, foreign keys, views, and stored procedures are now modelable in `MetaSql`, while idempotent upgrade-script behavior and DataQuality pack ownership still need a product decision.
- Short-term demos can apply the generated pack with setup tooling.
- Long-term product work should choose between a DataQuality-owned installer/deployer and making the converter emit a proper `MetaSql` workspace/manifest path for the deployable pack.

Do not solve the remaining gap by adding a generic SQL blob or `ParsedNode`-style escape hatch to `MetaSql`. The current module support is intentionally constrained to explicit SQL Server objects and module definitions, with source/live/manifest discipline like the rest of the engine.

## Retirement Path

1. Stop treating `execute` as part of the public `MetaSql` command surface.
2. Convert setup, cleanup, and verification demo calls to setup-specific tooling, with explicit `sqlcmd` variables where scripts currently use `--var`.
3. Decide the DataQuality generated-SQL deployment owner:
   - short term: use `sqlcmd` in the demos to apply the generated SQL pack;
   - better product path: add a DataQuality-owned deploy/install command or make the converter emit a proper `MetaSql` workspace/manifest path when the output is intended to be model-deployed.
4. Remove the `execute` CLI route, `Program.Execute.cs`, and the execute help test.
5. Regenerate the MetaDocs reference from the updated MetaCli workspace.
6. Refresh affected demo `run.output` files after the script conversion.

## Non-Goals

- Do not broaden `MetaSql` into a SQLCMD-compatible runner.
- Do not move the same generic SQL runner into another product CLI under a different name.
- Do not model third-party installer scripts as `MetaSql` product truth.
- Do not treat setup SQL as evidence that random SQL execution belongs in sanctioned metadata deployment.

## Acceptance Criteria For Removal

- `meta-sql --help` lists `deploy-plan` and `deploy`, not `execute`.
- The regenerated MetaDocs reference contains no `meta-sql execute` command after the CLI workspace update.
- Live demos no longer call `meta-sql execute` for setup, cleanup, verification, or generated SQL application.
- `rg "meta-sql execute" MetaSql README.md docs Samples` returns only this historical retirement note, active-context handoff notes, or archived run output kept deliberately for history.
- Any remaining DataQuality deployment gap is tracked as a DataQuality or MetaSql-model conversion decision, not hidden behind a generic SQL runner.

## Bottom Line

This is a scoped command-design mistake, not a deep architecture failure.

The command helped demos avoid `sqlcmd`, then quietly made `MetaSql` look broader than its modeled deployment contract. The correct hardening move is to retire it in a focused pass and force each remaining caller back to its owner: setup tooling, demo verification, DataQuality deployment design, or real `MetaSql` model deployment.
