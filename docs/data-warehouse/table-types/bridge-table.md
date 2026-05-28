# Bridge Table

## Meaning

A bridge table resolves a many-to-many relationship, often between facts and dimensions or between hierarchical dimension members.

## Sanctioned Invariants

- Declares both participant sides and their roles.
- Declares whether weighting/allocation factors exist.
- Declares whether the bridge is point-in-time/effective-dated.
- Does not replace a fact table when measurable events are present.

## Boundary

Bridge correctness often depends on coverage and allocation checks. The warehouse model declares the relationship shape; `MetaDataQuality` should prove row-level coverage, duplicate detection, and allocation sums.
