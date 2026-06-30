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

## Scope Boundaries

Tracked production CLI surfaces:

- `../meta/Meta/Cli`
- `../meta/MetaCli/Cli`
- `../meta/MetaDocs/Cli`
- `../meta/MetaMesh/Cli`
- `../meta/MetaWeave/Cli`
- every live `meta-bi/**/Cli/*.Cli.csproj` outside generated package artifacts

Ignored for this plan:

- installer utilities such as `MetaInstaller` / `Meta/Installer`
- demo-only programs under `Samples/Demos`
- copied source snapshots under `artifacts/public-sector-pilot`

Do not let ignored surfaces create false positives in CLI architecture scans.

## Current Status

- Done: live `meta-bi` production CLIs now use checked-in `.MetaCli` workspaces, `MetaCliRuntime<TModel>`, runtime-owned help/parsing, and executable command bindings.
- Done: `../meta` production CLIs `meta`, `meta-cli`, `meta-docs`, `meta-mesh`, and `meta-weave` use the runtime-owned help/parsing shape.
- Done: the tracked `meta-bi` runtime-shaped CLIs have been service-thinned/audited.
- Remaining tracked exceptions: none. Future work should be new hardening or service extraction, not catch-up to the basic CLI shape.

## Deferred Command-Surface Issues

Captured during the 2026-06-30 MetaMesh/MetaDocs documentation workflow pass:

- `meta-docs merge` had been modeled as `--new-workspace`; this was corrected to `--workspace` because the suite workspace is a declared persistent workspace in the docs mesh.
- `meta-docs author-page`, `import-cli`, and `import-workspace-model` still expose both `--workspace` and `--new-workspace`. That may be acceptable for create-vs-update authoring commands, but the command family needs a deliberate policy so `--new-workspace` is not used as a casual overwrite switch.
- `meta` still has real public command surfaces such as `meta import sql --new-workspace`, `meta import csv --new-workspace`, and `meta workspace merge --new-workspace`. These were not reviewed in the MetaMesh pass.
- Some README examples still mention older command shapes such as `meta-weave init --new-workspace`. Those are documentation drift, not necessarily live CLI drift, but they should be cleaned when the README is regenerated from MetaDocs.
- `MetaDocsImportSession` still supports the historical `MissingFromSource` lifecycle. CLI imports now prune stale generated CLI rows, but model/instance/prose importers still use missing-state behavior. Review before treating generated docs workspaces as fully self-cleaning.
- `meta-mesh add-step` currently models operation steps as `Executable`, `Arguments`, and `WorkingDirectory`. This is intentionally simple and script-replacement oriented, but it is still a free-form command surface. Revisit only if agents need structured editing of executable arguments rather than direct executable step declarations.

## Phase 1: Tighten Runtime-Aligned CLIs

These already use authored `.MetaCli` workspaces and `MetaCliRuntime<TModel>`, but not all are equally service-thin.

1. Done: `meta-data-type`
   - workspace creation is behind `MetaDataTypeWorkspaceService`
   - command handler is invocation reader/presenter only
   - target-validation failures use the shared presenter failure shape
2. Done: `meta-data-type-conversion`
   - service-backed baseline remains clean
   - command handler reads invocation values, calls `IMetaDataTypeConversionService`, and presents structured results
3. Done: `meta-transform-binding`
   - binding behavior remains in core services
   - partial TSV report artifact writing moved out of the CLI handler into `TransformBindingPartialReportService`
4. Done: `meta-data-quality`
   - discovery/inspection/promote behavior remains in core services
   - promotion persistence moved into `MetaDataQualityPromotionService`
   - inspect prose remains in the CLI handler because console presentation belongs in CLI code
5. Done: `meta-pipeline`
   - runtime is in place
   - command bodies were moved out of `Cli/Program.*` into `Cli/CommandHandlers/MetaPipelineCommandHandlers.*`
   - `Program.cs` is now version/runtime/bindings only

## Phase 2: Port Remaining BI CLIs

Port one CLI at a time. Do not do a broad sweep.

Recommended order:

1. Done: `meta-transform-script`
2. Done: `meta-schema`
3. Done: `meta-sql`
4. Done: `meta-convert`
5. Done: `meta-orchestration`
6. Done: `meta-analytics`
7. Done: `meta-data-warehouse`
8. Done: `meta-multi-dimensional`
9. Done: `meta-tabular`
10. Done: `meta-datavault-raw`
11. Done: `meta-datavault-business`

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
