# Periodic Snapshot Fact Table

## Meaning

A periodic snapshot fact records measurements for a regular interval, such as day, week, month, or accounting period.

## Sanctioned Invariants

- Grain includes the snapshot period.
- One row represents the declared dimensional context for one period.
- Measures describe state or activity over that period.
- Missing activity can still produce rows when the design requires dense snapshots.

## Boundary

The model declares the period and grain. Data-quality checks should prove period completeness and uniqueness for the declared grain.
