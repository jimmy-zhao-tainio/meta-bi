# AdventureWorks BI stack video scaffold

This folder is a recording scaffold for the AdventureWorks end-to-end story.

The intended video is not "copy an existing Microsoft SSAS project." The intended video is:

1. Start with a restored AdventureWorks OLTP SQL Server database as the source.
2. Give an agent the business-requirements brief and the source connection settings.
3. Record what the agent does next using `meta` / `meta-bi` CLI commands.
4. Watch whether it extracts source schema, models the BI stack, writes transforms, creates deployment assets, builds DQ checks, configures pipelines/orchestration, and processes the analytics target.
5. If the tabular target is processed successfully, connect to it from Excel and show a measure requested by the brief.

This scaffold intentionally keeps generated workspaces out of source control. A recording run should write generated output under `Runs`.

The generated run should include visible stage `.cmd` files and, when practical, one top-level `run.cmd` that calls them in order for replay or live execution.

## Files

- `BUSINESS-REQUIREMENTS.md`: business-user analytics request for the agent.
- `agent-meta.md`: generic agent guide for `meta-bi` CLI workflow and command discipline.
- `AGENT-TASK.md`: the prompt/task brief for the agent run.
- `SOURCE-SETUP.md`: AdventureWorks OLTP source setup notes and official links.
- `VIDEO-RUNBOOK.md`: recording sequence and cut points.
- `SNAG-LOG.md`: product snags found while preparing or running the demo.
- `00-env.cmd`: local default environment variables.
- `prepare-adventureworks-db.cmd`: downloads/restores `AdventureWorks2022` and verifies source data.
- `01-check-source.cmd`: checks that the restored AdventureWorks source database is reachable.
- `02-show-agent-task.cmd`: prints the agent task and business brief for recording.
- `03-extract-source-schema.cmd`: optional reference/preflight command for the first schema-extraction action the agent should perform during the recorded run.

## Quick start

Restore AdventureWorks first. Then from this folder:

```cmd
prepare-adventureworks-db.cmd
00-env.cmd
01-check-source.cmd
02-show-agent-task.cmd
```

Then start a fresh agent context and give it:

- `BUSINESS-REQUIREMENTS.md`
- `agent-meta.md`
- `AGENT-TASK.md`
- the connection environment variable names and values from `00-env.cmd`

The agent should perform the source schema extraction during the recorded run. The first generated product command should be this kind of command:

```cmd
meta-schema extract sqlserver --new-workspace SourceSchemaWS --connection-env AW_SOURCE_SQL --system AdventureWorks --all-schemas --all-tables
```

`03-extract-source-schema.cmd` exists only as a reference/preflight command. It is not meant to replace the agent-generated extraction step in the main recording.
