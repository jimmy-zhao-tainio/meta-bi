# Mini-Dimension Table

## Meaning

A mini-dimension splits rapidly changing or high-volume descriptive attributes away from a larger dimension, often to avoid excessive type 2 churn.

## Sanctioned Invariants

- Is attached to a parent business subject or dimension context.
- Contains a coherent subset of attributes, not arbitrary leftovers.
- Has its own surrogate key.
- Facts may reference the mini-dimension directly when the profile at event time matters.

## Boundary

Choosing a mini-dimension is a modeling decision about attribute volatility and query behavior. Runtime transforms can populate it, but the warehouse model should declare why this is a separate dimension.
