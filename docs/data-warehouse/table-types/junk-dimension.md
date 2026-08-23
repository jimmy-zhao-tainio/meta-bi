# Junk Dimension Table

## Meaning

A junk dimension groups low-cardinality flags, indicators, and small descriptors that do not deserve separate dimensions.

## Sanctioned Invariants

- Has a declared set of member attributes.
- Attributes are low-cardinality descriptive values.
- The table has one surrogate key for the combined profile.
- It should not become a dumping ground for unrelated high-cardinality business entities.

## Boundary

The model can declare that a set of flags forms a junk dimension. Data-quality checks should prove expected value domains and profile uniqueness.
