# Factless Fact Table

## Meaning

A factless fact records the occurrence, coverage, eligibility, or absence of an event without additive numeric measures.

## Sanctioned Invariants

- Has a declared grain like any other fact.
- Has dimension references that define the event or coverage context.
- May contain count-like implicit semantics, but no ordinary additive business measures are required.
- Absence/coverage scenarios should declare whether rows mean occurrence, eligibility, or expected coverage.

## Boundary

The model can distinguish occurrence from coverage. Actual missing rows and expected coverage gaps belong to data-quality validation.
