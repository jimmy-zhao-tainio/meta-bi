# AdventureWorks demo supervisor plan

This is the acceptance checklist for a clean recorded run. It is intentionally stricter than a diagnostic exploration: if a gate fails, stop, fix the root cause outside the accepted run, delete or archive the failed attempt, and rerun from a fresh folder.

## Accepted stack path

```text
SourceDBs -> RDV -> BDV -> DW/Mart -> Tabular
```

RDV and BDV are required. Tabular is the analytics target for the accepted run unless the local Analysis Services Tabular server is genuinely unavailable, in which case the run is partial and the environment blocker must be named.

## Supervisor gates

1. Plan gate
   - `PLAN.md` exists before artifact-producing product commands.
   - The plan names source, RDV, BDV, DW/mart, Tabular, pipeline, orchestration, and proof stages.
   - Folder names are layer/database/role scoped, not root-level `*WS` folders.

2. Source gate
   - The source schema is extracted from the live SQL database with `meta-schema extract sqlserver`.
   - The source `System.Name` matches the SQL identifiers that transforms will bind against.

3. RDV gate
   - RDV target structures exist as persisted raw-vault evidence, not only views or notes.
   - RDV load transforms are imported as `MetaTransformScript`.
   - RDV binding evidence exists or the exact product blocker is recorded.

4. BDV gate
   - BDV target structures exist as persisted business-vault evidence.
   - BDV load transforms are imported as `MetaTransformScript`.
   - BDV transforms read from RDV, not directly from the source unless explicitly justified by the model.

5. DW/mart gate
   - Mart target tables exist and are populated from BDV-backed transforms.
   - Mart proof queries run against persisted target tables, not only source views.

6. DQ gate
   - DQ candidates are generated automatically from modeled transform structure and binding evidence where available.
   - Generated checks include the model-supported families implied by the transforms, such as anti-join missing-reference/orphan checks, uniqueness/duplicate checks, fanout risks, and relationship optionality mismatches.
   - Promoted candidates are converted to executable DQ SQL and the review view is deployed.

7. Pipeline gate
   - `ops\Pipeline` contains one transform-backed pipeline per table-producing RDV, BDV, and DW/mart transform unless a specific modeled reason makes a group atomic.
   - `TransformExecutionTask.xml` exists and carries per-task transform and binding workspace paths.
   - Executable tasks, if present, are auxiliary and do not replace the ETL load DAG.

8. Orchestration gate
   - `ops\Orchestration` is inferred from the pipeline/binding evidence.
   - The inspected run plan task count matches the table-load transform count.
   - Dependency order comes from modeled read/write evidence, not manifest order or manual dependency rows.
   - `meta-orchestration execute` completes the table-load run.

9. Tabular gate
   - `analytics\Analytics` and `analytics\Tabular` are created from the mart.
   - `meta-tabular deploy` and `meta-tabular process` complete against `%AW_TABULAR_SERVER%` and `%AW_TABULAR_DATABASE%`.
   - The final proof shows a Tabular row count or business measure through Excel or a DAX-capable probe.

10. Clean-run gate
    - `logs` contain the successful command output.
    - `summary.txt` answers the recording questions in `AGENT-TASK.md`.
    - `SNAG-LOG.md` contains no unresolved accepted-run blocker.
