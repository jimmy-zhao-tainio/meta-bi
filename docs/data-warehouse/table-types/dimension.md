# Base Dimension Table

## Meaning

A dimension table describes the descriptive context by which facts are filtered, grouped, labeled, or explained.

## Sanctioned Invariants

- Has a stable dimension identity in the warehouse model.
- Has one declared surrogate key unless explicitly modeled as a special case.
- Has one or more declared natural/business keys where the source business identity matters.
- Contains attributes, not additive numeric events.
- Declares its historization behavior, such as type 1 overwrite, type 2 rows, or current-only semantics.
- Can be referenced by facts, bridges, outriggers, or role-playing views.

## Boundary

The dimension model can declare identity and historization intent. Actual row conformance, duplicate business keys, and unexpected attribute drift belong to `MetaDataQuality`.
