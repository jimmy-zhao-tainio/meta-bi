# Base Fact Table

## Meaning

A fact table records measurements or events at one declared grain.

## Sanctioned Invariants

- Has exactly one declared grain.
- Has explicit dimension references for the grain participants.
- Measures are declared with Meta-system semantic data types.
- Degenerate dimensions are modeled intentionally when transaction identifiers live in the fact.
- Does not mix transaction, periodic snapshot, and accumulating snapshot grain in one sanctioned fact table.

## Boundary

The model can declare the grain and expected columns. It cannot stop a user-authored transform from returning nonsense values. Binding and data-quality layers must validate that actual data respects the declared grain. Additivity and aggregation functions belong to the analytics/semantic layer.
