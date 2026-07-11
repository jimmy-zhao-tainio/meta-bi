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

The generated run should include a MetaMesh workspace with named, inspectable operations for each phase. The mesh is the replayable workflow surface for the recording.

## Files

- `BUSINESS-REQUIREMENTS.md`: business-user analytics request for the agent.
- `agent-meta.md`: generic agent guide for `meta-bi` CLI workflow and command discipline.
- `supervisor-meta.md`: supervisor guide for accepting or rejecting worker gates and preventing model-layer shortcuts.
- `AGENT-TASK.md`: the prompt/task brief for the agent run.
- `SOURCE-SETUP.md`: AdventureWorks OLTP source setup notes and official links.
- `VIDEO-RUNBOOK.md`: recording sequence and cut points.
- `SNAG-LOG.md`: product snags found while preparing or running the demo.
- `AdventureWorksBiStackVideo.MetaMesh`: source readiness, optional preflight extraction, and source-workspace cleanup operations.
- `prepare-adventureworks-db.cmd`: downloads/restores `AdventureWorks2022` and verifies source data.
- `make-demo-video.ps1`: renders the prepared recording assets; it is a media-production utility rather than a BI workflow.

## Quick start

Restore AdventureWorks first. Then from this folder:

```powershell
.\prepare-adventureworks-db.cmd

$env:AW_SQL_SERVER = "localhost"
$env:AW_SOURCE_DATABASE = "AdventureWorks2022"
$env:AW_RDV_DATABASE = "AdventureWorksRawVault"
$env:AW_BDV_DATABASE = "AdventureWorksBusinessVault"
$env:AW_DW_DATABASE = "AdventureWorksMetaDemo"
$env:AW_TABULAR_SERVER = ".\TABULAR"
$env:AW_TABULAR_DATABASE = "AdventureWorksMetaDemoTabular"
$env:AW_RUN_ROOT = "Runs"
$env:AW_SOURCE_SQL = "Server=localhost;Database=AdventureWorks2022;Trusted_Connection=True;TrustServerCertificate=True;"
$env:AW_RDV_SQL = "Server=localhost;Database=AdventureWorksRawVault;Trusted_Connection=True;TrustServerCertificate=True;"
$env:AW_BDV_SQL = "Server=localhost;Database=AdventureWorksBusinessVault;Trusted_Connection=True;TrustServerCertificate=True;"
$env:AW_DW_SQL = "Server=localhost;Database=AdventureWorksMetaDemo;Trusted_Connection=True;TrustServerCertificate=True;"

cd AdventureWorksBiStackVideo.MetaMesh
meta-mesh run --operation validate-source
cd ..
```

Then start a fresh agent context and give it:

- `BUSINESS-REQUIREMENTS.md`
- `agent-meta.md`
- `AGENT-TASK.md`
- the connection environment variable names and values established for the recording

The supervising agent or human should separately read `supervisor-meta.md` before approving gates.

Use the separate SQL targets `AW_RDV_SQL`, `AW_BDV_SQL`, and `AW_DW_SQL` for RDV, BDV, and DW/mart deploy-plan/deploy/extract work respectively. Do not collapse the modeled layers into one implicit target connection.

The agent should perform the source schema extraction during the recorded run. The first operation step that creates a BI domain workspace should be this kind of command:

```cmd
meta-schema extract sqlserver --new-workspace source\AdventureWorks2022\Schema --connection-env AW_SOURCE_SQL --system AdventureWorks2022 --all-schemas --all-tables
```

The recorded run should be planned and executed in phases, not one-shotted. The expected full stack path is `SourceDBs -> RDV -> BDV -> DW/Mart -> Tabular`; a direct source-to-mart shortcut is only a partial run, not the accepted full ETL demo. Generated folders should be layer/database scoped and role-named, such as `source\AdventureWorks2022\Schema`, `rdv\<database>\RawVault`, `bdv\<database>\BusinessVault`, `dw\<database>\Transforms`, and `ops\Orchestration`. The orchestration proof should be transform-backed table loads first; Tabular deploy/process is the downstream analytics proof, not a replacement for the ETL DAG.

For the final recording, keep only a clean run. Failed attempts are useful diagnostic scratch space: halt at the blocker, fix the product/model/environment issue, then rerun from a fresh timestamped folder so the accepted evidence is a clean replay rather than patched-over history.

The scaffold mesh's `extract-source-schema` operation is only a reference/preflight operation. It is not meant to replace the agent-authored extraction operation in the main recording.
