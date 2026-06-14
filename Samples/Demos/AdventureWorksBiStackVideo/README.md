# AdventureWorks BI stack video scaffold

This folder is a recording scaffold for the AdventureWorks end-to-end story.

The intended video is not "copy an existing Microsoft SSAS project." The intended video is:

1. Start with a restored AdventureWorks OLTP SQL Server database as the source.
2. Give an agent the business-requirements brief and the source connection settings.
3. Record what the agent does next using `meta` / `meta-bi` CLI commands.
4. Watch whether it extracts source schema, models the BI stack, writes transforms, creates deployment assets, derives DQ from the modeled transforms, infers the table-load orchestration DAG from transform-backed pipeline/binding evidence where safe, and processes the analytics target.
5. If the tabular target is processed successfully, connect to it from Excel or run a DAX-capable proof and show a measure requested by the brief.

The demo claim is a full modeled BI stack, not a pile of generated SQL files. The differentiators to make visible are strict binding, automatic transform-derived DQ candidates, modeled promotion to executable DQ SQL, one transform-backed pipeline per table-producing transform, orchestration/run planning inferred from modeled access profiles within the current safety rules, and a Tabular proof from the generated mart.

The intended takeaway is that a large middle slice of the BI stack is automatic when the work stays inside the supported modeled surface: ordinary SQL transforms, strict source/target binding, supported DQ candidate families, and modeled pipeline steps.

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
meta-schema extract sqlserver --new-workspace source\AdventureWorks2022\Schema --connection-env AW_SOURCE_SQL --system AdventureWorks2022 --all-schemas --all-tables
```

The recorded run should be planned and executed in phases, not one-shotted. The expected full stack path is `SourceDBs -> RDV -> BDV -> DW/Mart -> Tabular`; a direct source-to-mart shortcut is only a partial run, not the accepted full ETL demo. Generated folders should be layer/database scoped and role-named, such as `source\AdventureWorks2022\Schema`, `rdv\<database>\RawVault`, `bdv\<database>\BusinessVault`, `dw\<database>\Transforms`, and `ops\Orchestration`. The orchestration proof should be transform-backed table loads first; Tabular deploy/process is the downstream analytics proof, not a replacement for the ETL DAG.

For the final recording, keep only a clean run. Failed attempts are useful diagnostic scratch space: halt at the blocker, fix the product/model/environment issue, then rerun from a fresh timestamped folder so the accepted evidence is a clean replay rather than patched-over history.

`03-extract-source-schema.cmd` exists only as a reference/preflight command. It is not meant to replace the agent-generated extraction step in the main recording.
