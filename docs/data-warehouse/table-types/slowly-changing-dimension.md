# Slowly Changing Dimension Behavior

## Meaning

Slowly changing dimension behavior describes how a dimension preserves or overwrites changing descriptive attributes.

## Sanctioned Invariants

- The dimension declares its change behavior explicitly.
- Type 1 overwrite attributes are distinguished from type 2 historized attributes.
- Type 2 dimensions require a stable business key.
- Effective dating, current-row markers, hash columns, and related technical primitives are implementation-policy columns, not logical user-authored attributes.
- Mixed type 1/type 2 behavior is modeled per attribute group or attribute, not hidden in load SQL.
- The current row rule is explicit when consumers need current-only behavior.

## Boundary

SCD behavior is part of the warehouse design contract. Detecting unexpected source changes, duplicate current rows, and invalid effective-date ranges belongs to binding and data-quality validation.
