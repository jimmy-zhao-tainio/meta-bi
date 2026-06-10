# Agent guide: using meta-bi CLIs

This guide is generic. It explains how agents should approach BI work with `meta-bi` command-line tools. Pair it with a task prompt that names the actual source system, business requirements, output folder, and connection variables.

## Core stance

Use modeled metadata as the durable product truth.

Source systems, SQL scripts, business requirements, and operational logs are inputs or evidence. The work should move structure into sanctioned workspaces so it can be inspected, converted, validated, deployed, and operated.

Do not replace source evidence with guesses. Extract it, inspect it, model it, and carry it forward.

When a CLI surface is missing or insufficient, record the gap clearly. Do not invent commands, fake generated outputs, or silently switch to an unrelated artifact type.

## Expected stack shape

A useful `meta-bi` BI stack usually has these layers:

1. `MetaSchema` source contracts.
2. Transform SQL as source files and/or `MetaTransformScript` workspace data.
3. `MetaSchema` target contracts for generated SQL objects or deployed target tables/views.
4. `MetaTransformBinding` validation evidence connecting transforms to source and target contracts.
5. `MetaDataQuality` findings derived from modeled transform structure.
6. `MetaSql` deployable SQL assets and deploy manifests.
7. Optional warehouse, vault, or mart model workspaces when the current CLI surface supports the requested shape.
8. `MetaAnalytics` portable analytical intent.
9. `MetaTabular` or `MetaMultiDimensional` target analytical model.
10. `MetaPipeline` modeled execution steps.
11. `MetaOrchestration` cross-pipeline execution plan.
12. Run artifacts, logs, and a short summary describing what happened.

Not every task needs every layer. Use the smallest honest slice that proves the requested outcome, but keep the shape coherent. A narrow runnable stack is better than a broad stack that relies on invented behavior.

## Typical flow

Start from source evidence:

```cmd
meta-schema extract sqlserver --new-workspace <SourceSchemaWS> --connection-env <SOURCE_CONNECTION_ENV> --system <SourceSystemName> (--schema <name> | --all-schemas) (--table <name> | --all-tables)
```

Then move through the build:

1. Inspect the extracted source schema and business requirements.
2. Choose a bounded analytical slice.
3. Author SQL transforms as files with clear target identifiers.
4. Import transforms into `MetaTransformScript`.
5. Create or extract target schema contracts where needed.
6. Bind transforms with `MetaTransformBinding`.
7. Generate and promote useful DQ candidates with `MetaDataQuality`.
8. Convert DQ or SQL assets through `meta-convert` / `meta-sql` where supported.
9. Deploy SQL assets with `meta-sql deploy-plan` and `meta-sql deploy`.
10. Author or convert analytical models.
11. Add pipeline steps, including executable steps for external CLI actions when appropriate.
12. Create orchestration and execute it.
13. Leave a summary and snag log.

## Command scripts

When asked to leave runnable evidence, generate plain `.cmd` scripts.

Stage scripts are useful. A good run folder often contains scripts such as:

- `01-extract-source-schema.cmd`
- `02-author-transforms.cmd`
- `03-bind.cmd`
- `04-data-quality.cmd`
- `05-deploy-sql.cmd`
- `06-author-analytics.cmd`
- `07-author-pipeline.cmd`
- `08-author-orchestration.cmd`
- `09-execute-orchestration.cmd`

Also create a top-level `run.cmd` when practical. It should call the stage scripts in order and stop on the first failure. This gives humans one command to replay the full build while preserving each visible CLI step.

Scripts should echo each command before running it and should leave normal CLI output visible.

Use environment variables for connection strings and server names. Avoid hard-coding local machine values inside generated metadata or SQL assets.

Generated work should live under a task-specific run/output folder. Do not write generated workspaces or run artifacts into source-controlled product folders unless the task explicitly asks for committed sample assets.

## CLI landmarks

Use local help and `docs/commands.md` when unsure. Prefer current command help over old examples.

These are landmarks, not a substitute for `--help`:

```cmd
meta-schema extract sqlserver --new-workspace <path> --connection-env <name> --system <name> (--schema <name> | --all-schemas) (--table <name> | --all-tables)
meta-transform-script from sql-file --path <file.sql> --target <sql-identifier> (--new-workspace <path> | --workspace <path>)
meta-transform-script from sql-files --manifest <manifest.tsv> (--new-workspace <path> | --workspace <path>)
meta-transform-script to sql-path --workspace <path> --out <path>
meta-transform-binding bind --transform-workspace <path> --source-schema <path> --target-schema <path> --execute-system <name> --new-workspace <path>
meta-data-quality from-transform-workspace --transform-workspace <path> --new-workspace <path>
meta-data-quality from-transform-workspace --transform-workspace <path> --binding-workspace <path> --new-workspace <path>
meta-convert data-quality-to-sql --workspace <path> --out <file.sql>
meta-sql deploy-plan --source-workspace <path> --connection-env <name> --out <path>
meta-sql deploy --manifest-workspace <path> --source-workspace <path> --connection-env <name>
meta-tabular deploy --workspace <path> --server <server> --database-name <name>
meta-tabular process --server <server> --database-name <name>
meta-pipeline add-executable-step --workspace <path> --pipeline <name> --executable <path>
meta-pipeline execute --workspace <path>
meta-orchestration execute --workspace <path> --pipeline-workspace <path>
```

Some commands require additional workspaces or options depending on whether the modeled work is transform-backed, executable-only, tabular, multidimensional, or deployment-related. Use command help to choose the exact invocation.

## Model ownership

Keep ownership boundaries clean:

- `MetaSchema` describes database structure.
- `MetaTransformScript` describes transform SQL structure.
- `MetaTransformBinding` describes source/target binding evidence and validation.
- `MetaDataQuality` describes reviewable quality findings.
- `MetaSql` describes SQL deployable objects.
- `MetaAnalytics` describes portable analytical intent.
- `MetaTabular` and `MetaMultiDimensional` describe target-specific analytical artifacts.
- `MetaPipeline` describes pipeline structure and executable/transform tasks.
- `MetaOrchestration` describes cross-pipeline execution structure.

Do not use one model as a dumping ground for another model's responsibility.

## Evidence and summaries

Leave a concise run summary:

- inputs used
- commands generated or executed
- workspaces created
- deployment targets touched
- checks passed
- gaps or snags
- next manual action, if any

For failures, preserve the real failing command, exit code, and relevant output. Do not overwrite evidence with a polished story.

## What not to do

Do not import external generated project artifacts as product truth unless the task explicitly asks for that surface and the repo supports it.

Do not hand-author source schema metadata when a live source can be extracted.

Do not hide important model/conversion/deployment work inside opaque scripts.

Do not invent successful results for unsupported slices. Record the product gap and continue with the nearest truthful path.
