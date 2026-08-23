# Transaction Fact Table

## Meaning

A transaction fact records one row per business event or transaction at the lowest useful event grain.

## Sanctioned Invariants

- Grain is event-level and singular.
- Event date/time dimensions are explicit.
- Measures are captured at event time.
- Transaction identifiers are either degenerate dimensions or modeled source identifiers, not hidden metadata.
- Rows are normally append-oriented unless correction behavior is explicitly modeled.

## Boundary

Late arriving corrections and restatements are load behavior. The fact table still declares one transaction grain.
