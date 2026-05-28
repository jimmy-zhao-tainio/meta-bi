# Aggregate Fact Table

## Meaning

An aggregate fact stores summarized measurements derived from a lower-grain fact or process.

## Sanctioned Invariants

- Declares its summarized grain.
- Declares the source lower-grain fact or process when it is sanctioned.
- Measures must be valid at the aggregate grain.
- Measures are declared with Meta-system semantic data types at the aggregate grain.
- It should not masquerade as the base transaction fact.

## Boundary

Aggregate refresh strategy belongs to transforms and pipeline execution. The warehouse model owns the summarized grain and typed measures; additivity and aggregation behavior belong to the analytics/semantic layer.
