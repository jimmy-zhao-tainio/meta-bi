# Isomorphism: C# Integrity Is Non-Negotiable

## Why this exists

This repository treats XML, SQL, and C# as isomorphic surfaces over one modeled truth.
That only works if each surface is represented in its natural form.

- SQL integrity is naturally modeled with keys and relational links.
- C# integrity is naturally modeled with object references.

If C# is reduced to ID tables that require a giant post-load repair pass, isomorphism is broken in practice.

## Non-negotiable rule

For sanctioned models, in-memory C# integrity must be reference-native.

- Object references are first-class truth in memory.
- ID fields are serialization keys and transport shape, not the primary in-memory contract.
- Load/save pipelines must not rely on broad, implicit "repair everything" passes as the normal integrity mechanism.

## Current gap

This is being corrected in generated tooling.
The broad generated `Bind` pass has been split into explicit load/save phases:

- `HydrateReferences()` after XML load
- `PrepareForXmlSerialization()` before XML save

Generated relationship IDs are still public XML transport properties for serializer compatibility, but generated navigation setters now synchronize those IDs from assigned object references.
Treat the remaining public ID surface as transitional transport compatibility, not as the target in-memory representation.

Future tooling work must move toward this shape:

- POCO references are the primary in-memory integrity surface.
- Relationship ID fields exist to serialize/deserialize XML, not to drive normal C# graph correctness.
- Load may perform a bounded second pass to hydrate references from serialized IDs, but after load the object graph itself is the working truth.
- Save should project references to transport IDs deliberately; it must not use a whole-model rebinding sweep as its normal integrity mechanism.

## Design implications

- Mutation APIs must keep references consistent at write time.
- Validation should be explicit and focused, not a hidden full-graph rebinding step.
- If compatibility layers are temporarily needed, they must be documented as transitional debt, not normalized as the target architecture.

## Review gate

Reject changes that:

- treat C# as a row/link bag first and object graph second,
- introduce or expand "eventual integrity via bind/fixup" as the default model behavior,
- weaken the natural C# representation to mimic relational transport shape.
