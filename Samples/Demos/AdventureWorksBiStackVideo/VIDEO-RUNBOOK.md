# Video runbook

## Recording story

1. Show `BUSINESS-REQUIREMENTS.md`.
2. Show the source setup is an ordinary restored AdventureWorks OLTP SQL database, not copied SSAS project metadata.
3. Before recording, run `prepare-adventureworks-db.cmd` if the database is not already restored.
4. In the recording, run `00-env.cmd` and optionally `01-check-source.cmd` to show the connection settings and source readiness.
5. Show `agent-meta.md` and `AGENT-TASK.md`.
6. Start a fresh agent run and give it the business brief plus the connection environment variables.
7. Record what the agent does next: schema extraction, model/workspace authoring, transform creation, binding, DQ, SQL deployment, analytics/tabular generation, pipeline setup, and orchestration setup.
8. Run or let the agent run the generated command scripts in order.
9. Inspect the generated workspaces at a high level.
10. Run orchestration execution and show the compact progress output.
11. If orchestration processes the tabular database successfully, connect from Excel and display a measure requested by the brief.
12. Close with the generated `summary.txt` and `SNAG-LOG.md`.

## What the video is testing

- Can an agent start from only AdventureWorks, connection settings, and a business requirements note?
- Does it extract source schemas rather than hallucinating structure?
- Does it create modeled BI assets instead of copying Microsoft tutorial artifacts?
- Does it use visible `meta` / `meta-bi` CLI commands?
- Does it produce a runnable pipeline/orchestration path?
- Does the final analytics target answer at least one business question in Excel?

## Good cuts

- Requirements brief to generated command scripts.
- Live SQL source to generated source schema.
- Source schema to transform workspace.
- Transform workspace to binding evidence.
- Binding evidence to DQ checks.
- Pipeline/orchestration setup to live progress.
- Tabular processing to Excel validation if available.

## Avoid

- Do not show generated Visual Studio SSAS project files as source truth.
- Do not spend recording time on the `.bak` download.
- Do not pre-extract the schema for the main recording. The recorded agent should do that work from the restored SQL database.
- Do not let the generated stack start from anything except the restored SQL database and the agent-created `MetaSchema` workspace.
- Do not hide the command scripts. The video should show that the system is explainable and repeatable.
- Do not fix product gaps silently. Record snags first.

## Success criteria for the video

- The viewer sees normal business prose become explicit metadata.
- The viewer sees actual `meta` / `meta-bi` commands, not hand-waved automation.
- The viewer sees DQ, pipeline, and orchestration as part of the same modeled flow.
- Any local blocker is named as an environment or product snag, not blurred into the demo.
