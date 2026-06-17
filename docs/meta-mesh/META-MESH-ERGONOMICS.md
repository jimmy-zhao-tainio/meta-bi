# MetaMesh Ergonomics

## Decision

The solid product proof already exists: `meta-bi` can generate a full model-first BI stack
from source to analytics, agent-driven, with automatically derived data quality and
automatically derived orchestration.

MetaMesh is the ergonomics layer for that reality. It is models-of-models work at this
stage: a foundation-level model in `../meta`, not a BI-only service in `meta-bi`.
The current slice is a deterministic workspace map plus a small `meta-mesh` CLI. It
models the multi-workspace shape explicitly enough that humans, agents, and future tools
can ask:

- what sanctioned workspaces exist
- where each workspace is mounted
- which logical handles are stable enough to use in commands and docs
- which workspace links are declared product structure
- which gaps are only suggestions

The BI repo contributes an ergonomics pressure-test workspace at
`Samples/Demos/MetaMeshBiStackDemo/BIStackDemo.MetaMesh`.

## Model Surface

The foundation `MetaMesh` model currently includes:

- `Mesh`: named workspace map, root path, lifecycle state, and description.
- `WorkspaceInstance`: stable handle, model name, role, lifecycle state, and summary.
- `WorkspaceMount`: handle-to-path mapping for physical workspace locations.
- `WorkspaceLink`: declared directed relationship between workspace handles.
- `ModelProvider` and `ModelProviderCapability`: modeled provider/capability rows for future host or adapter registration.
- `MeshSuggestion`: non-authoritative findings from scan/suggest passes.

This is intentionally not a generic parsed-node graph. Handles, mounts, lifecycle, links,
providers, capabilities, and suggestions are concrete workspace-map concepts.

## CLI Surface

`meta-mesh` lives in `../meta/MetaMesh/Cli` and uses the same `CliAppDefinition` and
`ConsolePresenter` conventions as the rest of the suite.

Representative commands:

```cmd
..\meta\MetaMesh\Cli\bin\Debug\net8.0\meta-mesh.exe scan . --new-workspace .\out\Current.MetaMesh --name Current
..\meta\MetaMesh\Cli\bin\Debug\net8.0\meta-mesh.exe suggest .\out\Current.MetaMesh
..\meta\MetaMesh\Cli\bin\Debug\net8.0\meta-mesh.exe show --mesh .\Samples\Demos\MetaMeshBiStackDemo\BIStackDemo.MetaMesh
..\meta\MetaMesh\Cli\bin\Debug\net8.0\meta-mesh.exe check --mesh .\Samples\Demos\MetaMeshBiStackDemo\BIStackDemo.MetaMesh
..\meta\MetaMesh\Cli\bin\Debug\net8.0\meta-mesh.exe impact --mesh .\Samples\Demos\MetaMeshBiStackDemo\BIStackDemo.MetaMesh --workspace transform
```

The command name is `suggest`; this surface deliberately avoids `doctor` language.
Suggestions are review material, not product truth.

## MetaCli Slice

`../meta/MetaCli/Core` adds a small descriptor projection from `CliAppDefinition`.
The `meta-mesh describe` command emits its own descriptor so future hosts can discover
operations without becoming the source of truth.

This is a CLI contract description, not an HTTP API and not a JSON destination artifact.

## Host Boundary

`MetaHost` is deferred. A future host may accelerate discovery, execution, and UI flows,
but it must not own the mesh truth. The durable truth remains the sanctioned model workspace:
`workspace.xml`, `model.xml`, and `instances/*.xml`.

This ergonomics layer intentionally does not introduce:

- an HTTP daemon
- a central database
- blob persistence as product truth
- inferred lineage as product truth

## BI Demo Map

The BI demo workspace declares handles for representative stack layers:

- `source`
- `conversion`
- `vault`
- `warehouse`
- `transform`
- `binding`
- `quality`
- `pipeline`
- `orchestration`
- `sql`
- `analytics`
- `tabular`
- `multidim`

The links are deliberately coarse and declared. For example, `transform` feeds `binding`,
`quality`, `pipeline`, and derived `sql`; `warehouse` feeds `analytics`; `analytics`
derives `tabular` and `multidim`.

## Deferred Work

- richer provider and capability rows for existing CLIs
- stale derived workspace checks using declared producers/consumers
- workspace port/contract concepts after the handle/link model has proven itself
- optional `MetaHost` once the CLI and model semantics are stable
- broader docs generation integration after the descriptor surface has real consumers
