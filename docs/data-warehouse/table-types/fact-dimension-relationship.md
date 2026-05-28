# Fact-To-Dimension Relationship Roles

## Meaning

A dimensional fact references dimensions through named roles. When the same dimension participates more than once, the role is the fact relationship, not a separate warehouse table type.

Examples include order date, ship date, and invoice date all referencing the same date dimension.

## Sanctioned Invariants

- The dimension identity is shared.
- Each same-dimension relationship from a fact has a distinct role name.
- Role names belong to fact participation, not to duplicated dimensions.
- Analytics layers may expose role-playing dimensions, but the warehouse model preserves the authored fact relationship roles.

## Boundary

`MetaDataWarehouse` should model role names on fact-to-dimension relationships. SSAS/tabular/multidimensional projections may later decide how those roles become analytical dimensions.
