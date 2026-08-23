# Isomorphism: C# Integrity Is Non-Negotiable

## Why this exists

This repository treats XML, SQL, and C# as workspace surfaces over one modeled truth.
That only works if each surface is represented in its natural form.

- SQL integrity is naturally modeled with keys and relational links.
- C# integrity is naturally modeled with object references.

If C# is reduced to ID tables that require a giant post-load repair pass, isomorphism is broken in practice.

## Non-negotiable rule

For sanctioned models, in-memory C# integrity must be reference-native.

- Object references are first-class truth in memory.
- ID fields are serialization keys and transport shape, not the primary in-memory contract.
- Load/save pipelines must not rely on broad, implicit "repair everything" passes as the normal integrity mechanism.

## Surface contract

The C# workspace reader reconstructs the typed object graph from its sources,
and the writer projects that graph back to C# source. Relationship identities
may be present as transport data, but they are not the primary in-memory
contract.

The surface must preserve this shape:

- POCO references are the primary in-memory integrity surface.
- Relationship ID fields exist to serialize/deserialize XML, not to drive normal C# graph correctness.
- Loading may reconstruct references from serialized identities, but after load
  the object graph is the working truth.
- Saving projects references to transport identities deliberately and verifies
  the published C# sources before completion.

## Design implications

- Mutation APIs must keep references consistent at write time.
- Validation should be explicit and focused, not a hidden full-graph rebinding step.
- If compatibility layers are temporarily needed, they must be documented as transitional debt, not normalized as the target architecture.

## Review gate

Reject changes that:

- treat C# as a row/link bag first and object graph second,
- introduce or expand "eventual integrity via bind/fixup" as the default model behavior,
- weaken the natural C# representation to mimic relational transport shape.
