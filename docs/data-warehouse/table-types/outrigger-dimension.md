# Outrigger Dimension Table

## Meaning

An outrigger is a dimension referenced by another dimension, usually for a stable descriptive hierarchy or snowflaked context.

## Sanctioned Invariants

- Is referenced from a dimension, not directly as the primary grain of a fact.
- Has a clear business reason for being separated from the parent dimension.
- Does not hide fact relationships that should be modeled as fact-to-dimension relationships.

## Boundary

Outriggers should be used deliberately. The model should make snowflaking explicit rather than letting it appear as incidental SQL joins.
