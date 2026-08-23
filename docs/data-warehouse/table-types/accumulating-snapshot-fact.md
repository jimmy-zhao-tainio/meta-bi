# Accumulating Snapshot Fact Table

## Meaning

An accumulating snapshot fact tracks a process with a known lifecycle, updating milestone dates and measures as the process advances.

## Sanctioned Invariants

- Grain is one row per process instance.
- Milestone roles are explicitly modeled.
- Date dimension references are role-playing references for each milestone.
- Measures represent lifecycle durations, counts, or status at the process-instance grain.
- Update behavior is part of the sanctioned load contract because rows evolve.

## Boundary

This is not just a fact table with many nullable dates. The model should know the milestone roles. Data-quality checks can prove legal milestone order and lifecycle completeness.
