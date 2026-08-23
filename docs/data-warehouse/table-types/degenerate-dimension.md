# Degenerate Dimension

## Meaning

A degenerate dimension is a dimensional identifier stored in a fact table without a separate dimension table, such as an invoice number or transaction number.

## Sanctioned Invariants

- Belongs to a fact table.
- Has a declared business role.
- Is not a hidden surrogate key or technical lineage column.
- Can participate in filtering, grouping, or drill-through semantics.
- Does not become a substitute for a real descriptive dimension when attributes exist.

## Boundary

A degenerate dimension is not a standalone warehouse table type. The warehouse model should still name it because it changes fact semantics and query behavior.
