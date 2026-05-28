# Conformed Dimension Table

## Meaning

A conformed dimension is shared consistently across fact processes so different facts can be compared through the same business context.

## Sanctioned Invariants

- Has an enterprise-level business meaning, not only a local source-system meaning.
- Has one canonical warehouse identity and one declared attribute vocabulary.
- Can be referenced by multiple facts without changing semantics per fact.
- May have source-specific mappings, but the conformed dimension remains the authored truth.

## Boundary

Conformance is a design contract. If a transform loads incompatible values into the conformed dimension, that is binding or data-quality failure, not a reason to weaken the dimension definition.
