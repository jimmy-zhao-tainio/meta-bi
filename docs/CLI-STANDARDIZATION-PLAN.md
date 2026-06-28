# CLI Standardization Plan

## Target Shape

Every production CLI should converge on the same small architecture:

- `Cli/Program.cs`
  - version check
  - `MetaCliRuntime<TModel>`
  - executable-command bindings
  - `runtime.Run(args)`
- `Cli/<executable>.MetaCli`
  - authored command surface
  - command grammar, parameters, help shape, and parser facts
- generated tooling
  - MetaCli tooling loads the command-surface workspace
  - domain tooling loads and saves the domain workspace model
- CLI command handlers
  - read `MetaCliInvocation`
  - map invocation values into domain request objects
  - call domain services
  - present structured service results
- domain services
  - own real domain behavior
  - own workspace mutation/query logic
  - return structured results and domain errors

## CLI Smells

Stop and fix the boundary when a production CLI has any of these:

- `CliAppDefinition`, `CliCommandDefinition`, or duplicate C# command catalogs
- handwritten generic option parsing
- handwritten generic help/usage rendering
- command handlers receiving `string[] args` as semantic input
- CLI code loading or reloading the primary workspace behind `MetaCliRuntime`
- service methods returning console prose
- CLI handlers doing domain implementation rather than adapter/presenter work
- tests proving obsolete commands or unmodeled options do not exist

Generic parser rejection belongs in MetaCli runtime tests. Product CLI tests should prove current modeled behavior and domain behavior.

## Phase 1: Tighten Runtime-Aligned CLIs

These already use authored `.MetaCli` workspaces and `MetaCliRuntime<TModel>`, but not all are equally service-thin.

1. `meta-data-type`
   - move workspace creation behind a service
   - keep CLI as invocation reader and presenter only
2. `meta-data-type-conversion`
   - keep as the baseline for a mostly clean service-backed CLI
   - split handlers from `Program.cs` if useful
3. `meta-transform-binding`
   - keep binding behavior in core service
   - move adapter/report shaping out of `Program.cs` where it becomes nontrivial
4. `meta-data-quality`
   - move `inspect` summarization out of CLI
   - keep CLI presentation separate from discovery/promote behavior
5. `meta-pipeline`
   - runtime is in place
   - move execution/orchestration behavior out of `Cli/Program.*` into services

## Phase 2: Port Remaining BI CLIs

Port one CLI at a time. Do not do a broad sweep.

Recommended order:

1. `meta-transform-script`
2. `meta-schema`
3. `meta-sql`
4. `meta-convert`
5. `meta-orchestration`
6. `meta-analytics`
7. `meta-data-warehouse`
8. `meta-multidimensional`
9. `meta-tabular`
10. `meta-datavault-raw`
11. `meta-datavault-business`

Start near transform/schema/sql because those are central and will expose the reusable patterns early. Leave authoring-heavy DataVault/Tabular surfaces until the runtime/service pattern is boring.

## Per-CLI Checklist

For each CLI:

1. Create or verify `Cli/<executable>.MetaCli`.
2. Copy the `.MetaCli` workspace to output from the CLI project.
3. Replace command catalogs, parser code, and generic help with `MetaCliRuntime<TModel>`.
4. Bind executable command ids to small command handlers.
5. Move real domain work into core services.
6. Ensure workspace-backed commands default omitted `--workspace` to the current directory through runtime/domain loading.
7. Keep tests focused on current behavior:
   - help comes from MetaCli
   - core happy path
   - current-directory workspace default where applicable
   - real domain validation
8. Run focused tests/builds serially.
9. Commit only the coherent slice.

## Hard Stops

Stop and call it out when:

- a service still depends on generic `Meta.Core.Domain.Workspace` for primary domain behavior
- a CLI needs custom parser behavior that should be modeled or added to MetaCli runtime
- a handler grows beyond adapter/presenter work
- a model or generated tooling boundary blocks a clean typed service
- docs, generated artifacts, and source disagree

