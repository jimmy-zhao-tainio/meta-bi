# Business <-> BDV Weave Decision

## Decision for now

For the Business -> Business Data Vault path, current `MetaWeave` remains a flat property-binding tool.

Deeper parent-scoped consistency will be owned by `meta-datavault`, not by `MetaWeave`, until a second domain proves that scoped bindings are a foundational cross-domain requirement rather than a single-domain need.

## Why this is the current choice

`MetaWeave` today is clear and useful for direct property equivalence:

- explicit model references
- explicit property bindings
- deterministic check
- deterministic materialization for direct cases

That is enough for first-pass anchors such as:

- `BusinessHub.Name` -> `BusinessObject.Name`
- `BusinessLink.Name` -> `BusinessRelationship.Name`

It is not enough for parent-scoped child consistency such as:

- `BusinessHubKeyPart` under the correct `BusinessHub`
- `BusinessLinkEnd` under the correct `BusinessLink`

Trying to force those semantics into the current flat weave would muddy the core concept.

## What meta-datavault must therefore own

Until scoped weave semantics exist, `meta-datavault` must own the deeper Business/BDV consistency rules, including:

- hub to business-object alignment
- hub-key-part to business-key-part alignment in the correct parent context
- link to business-relationship alignment
- link-end to business-relationship-participant alignment in the correct parent context

Those checks must be explicit and deterministic.

## What would change this decision

If another sanctioned model family needs the same parent-scoped binding behavior, then the capability should move down into the foundation and `MetaWeave` should grow beyond flat property bindings.

At that point the project should revisit whether the foundational concept is still just weave, or whether it has become something broader and more fabric-like.
