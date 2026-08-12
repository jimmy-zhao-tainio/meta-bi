# AGENTS.md

## Purpose

`meta-bi` is the BI-side sanctioned modeling repo on top of the generic `meta` foundation.
This is not a generic app repo. Most work here is metadata architecture, model design, conversion glue, and compiler-style tooling around sanctioned models such as `MetaSchema`, `MetaDataType`, `MetaDataVault`, `MetaSql`, and `MetaTransform`.

## Session Bootstrap (Required)

Before doing substantial work, read these first:

- `docs/META-MODELING-GUARDRAILS.md`

Use it as a first-class constraint, not an optional note.

## Working Contract

- `meta` is representation-symmetric metadata, not XML-canonical metadata.
- XML workspace/model/instance files are one deterministic working surface:
  - `workspace.meta`
  - `model.xml`
  - `instances/*.xml`
- XML, SQL, C#, and future forms should be able to carry the same structure without semantic drift.
- Do not privilege XML persistence shape over natural SQL or C# representation when deciding model semantics.
- Do not introduce ad-hoc JSON as a destination artifact.
- Do not use blob semantics as product truth.
- Do not use heuristic lineage or inferred semantics as product truth.
- Prefer explicitly modeled understanding over clever post-hoc interpretation.

## MetaTransform Product Contract

- `MetaTransform` is a settled, sanctioned, bounded product area.
- `MetaTransformScript` is the sanctioned SQL Server transform syntax model.
- Its structural contract is:
  - `SQL -> model -> semantically equivalent SQL`
- `MetaTransformBinding` is the sanctioned derived binding and validation model for
  resolving script structure against source and target schema contracts.
- Syntax understanding remains primary. Binding, type evidence, target validation,
  lineage, and operational profiles are derived layers and must not replace it.
- Bounded support and explicit unsupported syntax do not make either model
  experimental. Product model changes still require deliberate review.

## Architecture and Review Rules

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
  - `model.xml`
  - `workspace.meta`
  - typed workspace sources
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
- `MetaTransformScript` and `MetaTransformBinding` are sanctioned product models
  with checked-in workspaces, typed models, CLIs, converters, and tests.
- Use the current model workspaces, source, and support trackers to determine the
  bounded supported surface. Historical planning notes are not product status.
