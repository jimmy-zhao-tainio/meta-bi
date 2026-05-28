# Meta Modeling Guardrails

## Purpose

This note defines modeling decisions that should stay stable across `meta` and `meta-bi`.
It exists to reduce drift during iterative work and avoid context-loss regressions.

## Core Principle

Model explicit authored truth, not convenient projections.
If meaning needs to survive generation, round-trip, or translation, it must be modeled structurally.

`meta` is representation-symmetric metadata, not XML-canonical metadata.
XML, SQL, C#, and future forms are working surfaces over the same modeled structure, and no surface should gain semantic authority just because it is the current persistence format.
XML workspace files are important deterministic artifacts, but they are not product truth above natural SQL or C# representation.

## Non-Negotiable Isomorphism Rule

For sanctioned slices, C# must be represented naturally as C#:

- SQL integrity is relational (keys and relationships).
- C# integrity is object references.

Do not normalize C# into ID-table semantics as the primary in-memory model.
ID fields are serialization transport keys, not the default integrity mechanism.
If a compatibility fixup/bind layer exists, treat it as transitional debt and call it out explicitly.

## Entity vs Magic Property

Use an entity + relationship when any of these are true:

- identity of the thing matters
- multiple instances can exist
- relationship direction/role matters
- ordering/ordinal matters
- lifecycle/history matters
- additional attributes may grow around the concept

Use a simple property only when all of these are true:

- exactly one scalar value
- no independent lifecycle
- no role ambiguity
- no ordering semantics
- no need for future participation in other relationships

## Smells That Usually Mean Under-Modeling

- properties named like hidden relationships (`FooId`, `PrimaryFooId`, `SecondaryFooId`)
- repeated scalar columns that imply participants (`Role1`, `Role2`)
- encoded lists in text fields
- discriminator strings controlling meaning of unrelated fields
- null-heavy property sets that represent variant subtypes

When these appear, stop and re-evaluate whether an entity/relationship should exist.

## Role Relationship Rule

Role relationships are reserved for one specific case:

- multiple relationships to the same target entity kind need disambiguation

Examples:

- process -> role (owner)
- process -> role (executor)
- process -> role (reviewer)

Do not use role relationships as a generic substitute for missing model structure.

## Structure Integrity Rules

- Preserve declared structure once.
- Model inheritance separately from declaration ownership.
- Keep ordered collections explicit (with ordinal where needed).
- Keep polymorphic references typed to declared base type, not exploded concrete alternatives.

## Isomorphism Discipline

For sanctioned slices, maintain this contract:

- `XML -> model -> semantically equivalent XML`
- `SQL -> model -> semantically equivalent SQL` (bounded supported surface)
- `C# surface -> model -> semantically equivalent C# surface`

Each surface should be able to act as an authoring surface for the same modeled truth within its supported scope.

If two surfaces disagree, reconcile before extending the model.

## Practical Review Checklist

- [ ] Any "magic property" representing a hidden relationship was challenged.
- [ ] Relationship multiplicity is explicit.
- [ ] Role disambiguation is used only for same-kind multi-relationships.
- [ ] Ordered members are explicit and stable.
- [ ] Round-trip acceptance criteria are defined for the affected surface.

## Operational Pitfalls (Cross-Repo)

### Recursive tooling directory growth

Common failure mode:

- regenerate tooling with relative `--out` from a different current directory than the previous run
- the same logical output segment gets appended again (for example model/tooling folder names), creating nested repeats

Operational rules:

- treat generation outputs as fixed canonical paths per model
- resolve workspace and output paths to absolute paths before invoking generation
- run generation from repo root (or a wrapper that pins repo root) instead of ad-hoc subdirectories
- reject/stop when output path shows repeated logical segments before writing anything

### Using artifacts before build completion

Common failure mode:

- thread A starts building CLI/service/tooling
- thread B loads/runs against partially produced artifacts
- failures look nondeterministic but are simply read-before-ready races

Operational rules:

- single-writer principle for each build output directory
- consumer steps must wait for an explicit success gate (build exit success + readiness marker)
- do not share partially updated `bin/obj` outputs across concurrent workflows
- when in doubt, use clean dedicated output roots per thread/run
