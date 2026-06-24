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

Semantic precision must still be readable to both humans and agents, but prose cannot rescue vague model surfaces.
Do not split or name concepts so finely that the reader can no longer understand the model surface.
Do not rely on surrounding documentation to teach hidden semantics into a weak name or weak shape.
If a technically precise structure obscures the domain meaning, improve the names, types, prose, or model boundary before accepting it.

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
- ordering matters
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
- Order is a relationship between entities.
- Keep ordered collections explicit with linked order relationships, ordered relationships, sequence/member entities, or another structural relationship model.
- Do not encode order by padding text with leading zeros (`001`, `010`, `095`) so lexical sorting pretends to be numeric ordering.
- If order matters, model it as order: a previous/next relationship, ordered relationship, sequence entity, or another explicit structure with order semantics.
- Do not use scalar `Ordinal`, `Position`, or `Order` properties as the normal way to model ordering.
- Use numeric order values only when the number itself is domain truth or an external surface requires numeric position as authored data.
- Avoid vague ordering names such as `Order`. Use names and relationships that carry the semantics, such as `PreviousStep`, `PreviousSegment`, or a dedicated sequence/member entity.
- If a value must be interpreted as an integer, number, date, identifier, or other non-plain-text shape, that meaning belongs in the model surface. It must not live only in reader knowledge or prose.
- Text values should declare domain meaning. They must not carry implementation hacks for sorting, grouping, or display stability.
- Keep polymorphic references typed to declared base type, not exploded concrete alternatives.

Leading zeros are only acceptable when they are part of an external/domain value that is itself text, such as a source code, account number, postal code, or other authored identifier. They are not acceptable for modeled ordering values.

## Isomorphism Discipline

For sanctioned slices, maintain this contract:

- `XML -> model -> semantically equivalent XML`
- `SQL -> model -> semantically equivalent SQL` (bounded supported surface)
- `C# surface -> model -> semantically equivalent C# surface`

Each surface should be able to act as an authoring surface for the same modeled truth within its supported scope.

If two surfaces disagree, reconcile before extending the model.

## Provider Integrity Proof

Do not treat a standalone `check` command as product truth.

The first integrity judge is relationship / referential integrity in the modeled instance graph.
The stronger product judge is isomorphism: the represented structure must survive projection into sanctioned surfaces such as generated C# and SQL without semantic drift.

XML is one deterministic working surface, not the semantic authority.
A workspace can be mechanically jumbled as XML and still be acceptable if it loads, binds, and round-trips through the modeled surfaces correctly.

Diagnostics should be evidence produced by real load, bind, convert, generate, or round-trip operations.
The provider of the modeling software must prove it knows how to model by making those operations preserve the representation-integrity and isomorphism contract.

Do not add public commands whose purpose is to compensate for XML being more permissive than C# references or SQL foreign keys.

## Practical Review Checklist

- [ ] Any "magic property" representing a hidden relationship was challenged.
- [ ] Relationship multiplicity is explicit.
- [ ] Role disambiguation is used only for same-kind multi-relationships.
- [ ] Ordered members are explicit and stable through relationships rather than scalar ordering properties.
- [ ] No ordering value relies on leading-zero text padding.
- [ ] Property names do not rely on prose to reveal hidden type or ordering semantics.
- [ ] Round-trip acceptance criteria are defined for the affected surface.
- [ ] Provider integrity is proven through load, bind, convert, generate, or round-trip behavior rather than a standalone `check` concept.

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
