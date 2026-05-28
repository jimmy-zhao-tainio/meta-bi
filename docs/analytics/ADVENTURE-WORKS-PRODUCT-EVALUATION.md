# Adventure Works Product Evaluation

## Purpose

This pass evaluates `meta-bi` as if a BI developer is recreating Adventure Works from scratch for a product decision.

The goal is not to copy Microsoft Analysis Services project artifacts and declare success. The goal is to use public Adventure Works source data and public target behavior as a realistic workload, then discover and fix real product snags in `MetaAnalytics`, `MetaTabular`, `MetaMultiDimensional`, conversion, CLI authoring, deploy, processing, and query verification.

## Reference Material

- Microsoft Adventure Works sample database installation guidance: https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure
- Microsoft SQL Server samples release downloads: https://github.com/microsoft/sql-server-samples/releases/tag/adventureworks
- Microsoft Analysis Services samples overview: https://learn.microsoft.com/en-us/analysis-services/azure-analysis-services/analysis-services-samples
- Microsoft Adventure Works multidimensional tutorial: https://learn.microsoft.com/en-us/analysis-services/multidimensional-tutorial/multidimensional-modeling-adventure-works-tutorial

## Evaluation Rules

- Do not import `.dwproj`, `.bim`, XMLA, TMSL, or generated Visual Studio artifacts as product truth.
- Use AdventureWorksDW source data and public tutorial/model behavior to guide authoring.
- Prefer `MetaAnalytics` for portable analytical intent.
- Patch `MetaTabular` or `MetaMultiDimensional` only for target-specific details that cannot honestly live in `MetaAnalytics`.
- Treat source database installation as evaluation setup, not as a `MetaSql` product requirement. `MetaSql` deploys sanctioned `MetaSql` models; random SQL script execution belongs in setup tooling such as `sqlcmd`, not in the product model/deploy surface.
- Record every snag as a concrete product issue before fixing it.
- Fix snags at the owning layer. CLI output stays in CLI code, deploy semantics stay in target deploy code, and target-specific metadata stays in the target model.
- Keep each fix small enough to verify and commit independently.

## First Target

Start with the Adventure Works multidimensional tutorial shape, not the full historical sample project.

Reason: the tutorial intentionally exercises the surfaces most likely to expose product gaps:

- data sources and data source views
- dimensions, attributes, attribute relationships, and hierarchies
- cubes, measure groups, measures, dimension usage, and partitions
- calculations, KPIs, actions, perspectives, translations, and roles
- deploy, process, and browse/query verification

Tabular follows after the multidimensional pass because its shared conceptual shape should benefit from snags found while authoring the common analytics model.

## Working Phases

1. Source setup: restore or install AdventureWorksDW locally, with environment variables rather than hard-coded connection strings.
2. Conceptual authoring: build the first `MetaAnalytics` workspace from business intent and source tables.
3. Multidimensional conversion and target patching: convert to `MetaMultiDimensional`, then add only target-owned details.
4. Deploy and process: create an SSAS multidimensional database from the authored target workspace.
5. Query verification: run deterministic ADOMD/MDX checks for core measures, hierarchy navigation, KPI/action/perspective visibility where possible.
6. Tabular pass: recreate the same source model through `MetaAnalytics -> MetaTabular`, then deploy/process/query.

## Pressure Points To Validate

These are hunches, not product conclusions. Promote one to the snag log only after the evaluation hits it through an actual authoring, conversion, deploy, processing, or query step.

- `meta-sql execute` is probably legacy demo glue, not a `MetaSql` product surface. Demo/bootstrap SQL should move toward setup-specific tooling such as `sqlcmd` or a small setup script, while `MetaSql` stays focused on sanctioned model extraction, deploy planning, and deploy.
- AdventureWorksDW source installation is evaluation setup. SQLCMD directives, bulk-load row terminators, service-account file access, and binary/photo columns are real setup pressure, but they should not distort `MetaSql`, `MetaAnalytics`, `MetaTabular`, or `MetaMultiDimensional` unless the product model itself owns the behavior being tested.
- Source access for SSAS processing is a production pressure point. Target data sources should remain service-account style; demos may grant the local SSAS service account only when the demo owns the source database.
- Adventure Works is useful because it stresses real BI modeling surfaces: role-playing dates, snowflaked product/geography dimensions, many-to-many sales reasons, parent-child employee/account structures, translations, perspectives, KPIs, actions, calculations, and security.
- If the conceptual authoring CLI becomes painful before deployment, that is a product issue in the `MetaAnalytics` authoring surface, not a reason to import Visual Studio artifacts.
- Conversion should make target-owned gaps visible. A missing tabular/multidimensional detail after conversion should become either a deliberate target patch step or a converter fidelity issue, not an invisible default.
- Processing/query verification is likely to expose the most valuable snags: dimension-usage granularity bindings, DSV relations, partition/query sources, measure data types and formats, default measure behavior, and MDX/DAX script placement.
- The tabular and multidimensional passes should share analytical intent, but differences in target realization are expected. Do not force common metadata upward just to hide a target-specific requirement.

## Snag Log

| Id | Phase | Symptom | Owner | Status | Resolution |
| --- | --- | --- | --- | --- | --- |
| AW-0001 | Evaluation setup | Need an explicit Adventure Works evaluation harness that prevents copied SSAS project artifacts from being treated as success. | Docs/process | Fixed | This document defines the evaluation rules and snag discipline. |
| AW-0002 | Source setup | The official AdventureWorksDW install script is SQLCMD-oriented; using or broadening `meta-sql execute` for random setup SQL would put setup convenience in the wrong product layer. | Evaluation harness / MetaSql boundary | Open | Use a setup-specific path for the source database, such as the official installer flow, `sqlcmd`, or a tracked sample setup script. `docs/meta-sql/META-SQL-EXECUTE-RETIREMENT.md` records the focused retirement finding for existing `meta-sql execute` usage. |

## Acceptance Criteria

- A fresh developer can follow the evaluation path without relying on copied SSAS project artifacts.
- Each discovered blocker is recorded in the snag log before the fix.
- Each product fix lands in the layer that owns the behavior.
- The multidimensional pass ends with a deployed, processed, queryable Adventure Works-shaped database or a documented local environment blocker.
- The tabular pass ends with a deployed, processed, queryable Adventure Works-shaped database or a documented local environment blocker.
