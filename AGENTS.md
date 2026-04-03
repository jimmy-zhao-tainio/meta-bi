# AGENTS.md

## Purpose

`meta-bi` is the BI-side sanctioned modeling repo on top of the generic `meta` foundation.
This is not a generic app repo. Most work here is metadata architecture, model design, conversion glue, and compiler-style tooling around sanctioned models such as `MetaSchema`, `MetaDataType`, `MetaDataVault`, `MetaSql`, and `MetaTransform`.

## Working Contract

- The canonical metadata world is XML workspace/model/instance form:
  - `workspace.xml`
  - `model.xml`
  - `instances/*.xml`
- XML / SQL / C# are isomorphic surfaces over the same modeled truth.
- Do not introduce ad-hoc JSON as a destination artifact.
- Do not use blob semantics as product truth.
- Do not use heuristic lineage or inferred semantics as product truth.
- Prefer explicitly modeled understanding over clever post-hoc interpretation.

## SQL Model Direction

- Current high-stakes work is a bounded SQL `CREATE VIEW` syntax model.
- The acceptance goal is:
  - `SQL -> model -> semantically equivalent SQL`
- This is a syntax-modeling problem first.
- Semantic, lineage, transform, or optimization layers are derived later.
- They must not replace syntax understanding.
- The current `MetaTransform` area is still speculative and experimental.
- Do not treat files under `MetaTransform` as settled sanctioned product truth without checking the current task and latest review context.

## Codex Rules

- Plan first for difficult or architectural tasks.
- Keep the abstraction level honest. Do not land "interesting artifacts" at the wrong layer.
- Do not invent generic `ParsedNode` / `ParsedEdge` destination models.
- Do not mirror external libraries blindly just because reflection exposes them.
- Preserve declared structure once; model inheritance separately.
- Model polymorphic properties against their declared base type, not exploded concrete alternatives.
- Ordered collections must remain explicit.
- If reflection or repo structure does not justify a modeling choice, surface the uncertainty instead of inventing.

## Ownership Boundaries

- Put behavior under the domain that owns the outcome.
- Refuse to continue if ownership boundaries are mixed or unclear.
- CLI output belongs in CLI code, not service code.
- Services should return structured results, not preformatted console prose.
- Shared presenter conventions from `Meta.Core.Presentation.ConsolePresenter` are the UX baseline.

## How To Work

- Prefer small staged deliverables.
- Every substantial task should end with explicit acceptance criteria.
- Verify with the actual sanctioned artifact whenever possible:
  - generated `model.xml`
  - generated workspace
  - generated tooling
  - representative run output
- Do not wander into broad side reports, catalog dumps, or "interesting artifact" detours unless the task explicitly asks for them.
- If docs, code, and generated artifacts disagree, stop and reconcile before building more on top.

## Definition Of Done

- The artifact lands on the right abstraction level.
- The result is structurally faithful, not just plausibly shaped.
- The change is explained briefly in repo terms.
- Verification is stated explicitly:
  - what was checked
  - what passed
  - what remains unverified

## Repo Notes

- Current repo-level structure is centered around sanctioned model families plus conversion and tooling projects.
- `MetaTransform` currently contains exploratory work aimed at reaching a bounded SQL-in-`VIEW` model.
- Progress there is still limited. Treat it as an experiment unless the task explicitly says otherwise.
- If a task touches `MetaTransform`, avoid assuming older docs, generated workspaces, or exploratory files are final.
