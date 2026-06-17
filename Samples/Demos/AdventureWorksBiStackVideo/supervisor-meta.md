# Supervisor guide: accepting modeled BI demo runs

This guide is for the human/Codex supervisor, not the worker agent.

`agent-meta.md` tells the worker how to move. This file records what the supervisor must notice, challenge, and accept or reject during a recorded run.

## Core stance

Do not let operational success hide a modeling shortcut.

A run can deploy tables, load rows, derive DQ, execute orchestration, and process Tabular while still skipping the product model that should own a layer. Downstream evidence is valuable, but it cannot retroactively prove the right upstream abstraction.

For the accepted demo, each durable physical or execution artifact must be traceable to the product model that owns it.

## Owning-product-model invariant

CLI usage alone is not enough. A worker can use `meta-sql` or shell execution and still bypass the model layer that should own the semantics.

Accepted ownership:

- Source database contract: `MetaSchema`
- RDV physical tables: `MetaRawDataVault` lowered to `MetaSql`
- BDV physical tables: `MetaBusinessDataVault` lowered to `MetaSql`
- DW/mart physical tables: `MetaDataWarehouse` lowered to `MetaSql`
- Table-load transform SQL: `MetaTransformScript`
- Transform source/target proof: `MetaTransformBinding`
- DQ checks: `MetaDataQuality` lowered to DQ SQL
- Table-load work units: `MetaPipeline`
- Safe run order: `MetaOrchestration`
- Portable analytics intent: `MetaAnalytics`
- Tabular target: `MetaTabular`

`MetaSql` is a realization surface. It is not a substitute owner for DW, vault, transform, DQ, pipeline, orchestration, or analytics semantics.

## No side-door contract creation

Reject a gate when the worker creates a physical contract by side effect and then extracts it back as if that were the model.

Examples to reject:

- DW/mart tables created with `SELECT TOP (0) ... INTO awbi.Fact...` and then extracted with `meta-schema`.
- Hand-authored DDL deployed with `meta-sql execute` while claiming a model generated it.
- A `MetaSchema` extraction used as the only durable DW/mart contract.
- One executable wrapper that loads a whole layer while pretending to be the transform-backed pipeline DAG.

Acceptable role for extraction:

- Source extraction is product truth for external source contracts.
- Target extraction is verification and binding input after an owning model has produced/deployed the target contract.

## DW/mart gate

The DW/mart gate is not clean unless all of these are true:

- `dw/<database>/Warehouse` exists as a `MetaDataWarehouse` workspace.
- Dimensions, facts, business keys, relationships, and measures needed for the demo slice are present in that workspace.
- The physical mart contract is generated from `MetaDataWarehouse -> MetaSql`.
- `dw/<database>/Sql` and `dw/<database>/DeployManifest` exist, or the exact missing conversion/deploy blocker is recorded.
- Mart load transforms are imported as `MetaTransformScript`.
- Mart binding validates against the BDV source schema and the modeled/deployed mart target schema.
- Mart proof queries run against persisted target tables.

If the run has only `dw/<database>/Transforms`, `Schema`, `Binding`, and `DataQuality`, it may be a useful operational mart proof, but it is not a modeled DW proof.

## Evidence chain to inspect

At each phase, ask "what produced this artifact?"

Good chain:

```text
business requirements
  -> source MetaSchema
  -> RDV model
  -> RDV MetaSql/deploy
  -> RDV load transforms/binding
  -> BDV model
  -> BDV MetaSql/deploy
  -> BDV load transforms/binding
  -> DW model
  -> DW MetaSql/deploy
  -> DW load transforms/binding
  -> DQ model/SQL
  -> transform-backed pipeline tasks
  -> inferred orchestration run plan
  -> analytics/tabular model
  -> Tabular process/proof
```

Suspicious chain:

```text
transform SQL
  -> ad hoc physical table creation
  -> target MetaSchema extraction
  -> binding/DQ/pipeline/orchestration
```

The suspicious chain can pass many downstream checks. It still skips the DW model.

## Supervisor phase behavior

Require the worker to stop at gates. Read enough artifacts before approving the next phase.

Minimum gate checks:

- Plan: named layer folders include `Warehouse` for DW/mart and not only `Transforms`.
- RDV: model workspace exists before SQL/deploy/load evidence.
- BDV: model workspace exists before SQL/deploy/load evidence.
- DW/mart: `Warehouse` model exists before target schema extraction and binding.
- DQ: generated from modeled transforms plus binding evidence, not hand-written review SQL.
- Pipeline: transform execution tasks exist for table loads; executable tasks are auxiliary only.
- Orchestration: dependency/run plan comes from modeled access profiles, not manifest order or hand-added edges.
- Tabular: consumes persisted mart, not vault internals or source tables.

If a gate fails, do not keep polishing the same accepted run. Use the failed folder as diagnostic evidence, fix the instruction/product/environment problem, then restart from a fresh folder.

## What to write in the transcript

Record:

- supervisor prompt for each gate
- worker response for each gate
- exact acceptance or rejection reason
- any product/model/environment blocker
- whether a rerun is required

When rejecting a gate, name the abstraction violation plainly. Example:

```text
Rejected DW/mart gate: physical awbi tables were created directly from transform SELECT shape and then extracted as MetaSchema. No MetaDataWarehouse workspace produced the mart contract.
```

## Demo acceptance rule

The final demo claim is a modeled BI stack:

```text
SourceDBs -> RDV -> BDV -> DW/Mart -> Tabular
```

The run is accepted only when every arrow is backed by the owning model and its downstream realization evidence. A working SQL database and a working Tabular model are not enough if the chain skipped a required model layer.
