# Video runbook

## Recording story

1. Show `BUSINESS-REQUIREMENTS.md`.
2. Show the source setup is an ordinary restored AdventureWorks OLTP SQL database, not copied SSAS project metadata.
3. Before recording, run `prepare-adventureworks-db.cmd` if the database is not already restored.
4. In the recording, set the documented connection environment variables and run `meta-mesh run --operation validate-source` from `AdventureWorksBiStackVideo.MetaMesh` to show source readiness.
5. Show `agent-meta.md` and `AGENT-TASK.md`.
6. Start a fresh agent run and give it the business brief plus the connection environment variables.
7. Have the agent write `PLAN.md` first. Review the phase plan before product artifact generation.
8. Record the agent running the stack in phases: source extraction, RDV, BDV, DW/mart, binding/DQ, SQL deployment, transform-backed pipeline setup, orchestration inference/run planning, and analytics/tabular generation.
9. Inspect and run the agent-authored MetaMesh operations in the phase order recorded in `PLAN.md`.
10. Inspect the generated workspaces at a high level.
11. Run orchestration execution and show the compact progress output.
12. After the modeled table-load orchestration succeeds, deploy/process the tabular database. Then connect from Excel or run a DAX-capable proof and display a measure requested by the brief.
13. Close with the generated `summary.txt` and `SNAG-LOG.md`.

## What the video is testing

- Can an agent start from only AdventureWorks, connection settings, and a business requirements note?
- Does it extract source schemas rather than hallucinating structure?
- Does it create modeled BI assets instead of copying Microsoft tutorial artifacts?
- Does it preserve a proper ETL/data-layer path with RDV and BDV before DW/mart?
- Does it plan first and run the demo in visible phases instead of one-shotting the stack?
- Does it use visible `meta` / `meta-bi` CLI commands?
- Does it derive DQ from the modeled transform structure instead of relying on hand-authored one-off checks?
- Does it infer the safe table-load orchestration path from modeled transform-backed pipeline and binding evidence?
- Does the final Tabular analytics target answer at least one business question in Excel or a DAX-capable proof?

## Good cuts

- Requirements brief to modeled MetaMesh operations.
- Live SQL source to generated source schema.
- Source schema to RDV.
- RDV to BDV.
- BDV to DW/mart transforms.
- DW/mart transforms to binding evidence.
- Binding evidence to automatic DQ candidates and generated DQ SQL.
- One transform-backed pipeline per table-producing transform to modeled access profiles.
- Modeled pipeline access profiles to inferred orchestration/run-plan rows.
- Orchestration run-plan to live progress.
- Tabular processing to Excel or DAX validation if available.

## Avoid

- Do not show generated Visual Studio SSAS project files as source truth.
- Do not spend recording time on the `.bak` download.
- Do not pre-extract the schema for the main recording. The recorded agent should do that work from the restored SQL database.
- Do not let the generated stack start from anything except the restored SQL database and the agent-created `MetaSchema` workspace.
- Do not accept a source-to-DW/mart shortcut as the full demo. RDV and BDV must exist, or the run is partial with a named blocker.
- Do not let the agent one-shot the whole stack without a written plan and phase gates.
- Do not use flat root-level workspace folders like `SourceSchemaWS`, `TransformWS`, or `BindingWS`; generated folders should be layer/database scoped and role-named.
- Do not accept one executable pipeline or one monolithic command script as the primary orchestration proof. The table-load DAG should come from transform-backed pipeline tasks.
- Do not hide the operation steps. The video should show that the system is explainable and repeatable.
- Do not fix product gaps silently. Halt the diagnostic run, record the snag, fix the root cause, then record from a clean rerun.

## Success criteria for the video

- The viewer sees normal business prose become explicit metadata.
- The viewer sees actual `meta` / `meta-bi` commands, not hand-waved automation.
- The viewer sees automatic DQ derived from transform semantics and binding evidence.
- The viewer sees orchestration inferred from modeled transform-backed pipeline/binding profiles where safe, with policy issues surfaced instead of hidden.
- The viewer understands that ordinary modeled transforms and pipeline tasks make a large middle slice of the BI stack automatic.
- The viewer sees Tabular deployed/processed from the mart and a final row-count or business-measure proof.
- Any local blocker is named as an environment or product snag in the diagnostic attempt, not blurred into the accepted clean demo.
