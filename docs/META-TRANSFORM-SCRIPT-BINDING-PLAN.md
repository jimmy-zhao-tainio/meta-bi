# MetaTransformScript Binding Plan

First concrete semantic-layer plan after syntax ownership.

`MetaTransformScript` remains the canonical syntax model.
This plan is for the first derived semantic layer over that syntax:

- binding
- name resolution
- rowset-shape derivation

Type inference and target validation come later. They should build on this layer, not replace it.

## First Step

Phase 1 should bind the easy spine only:

- named table references
- table aliases
- plain column references
- simple `SELECT` output columns
- simple `*` and qualified `alias.*`
- simple query-block scope

This is enough to prove the semantic direction without dragging in CTEs, derived tables, windows, or harder expression rules immediately.

## Inputs

Phase 1 binding should take:

- one `MetaTransformScript` transform
- one sanctioned source-schema workspace

Not yet:

- target schema
- type reconciliation rules
- validation policy

## Proposed Semantic Objects

Keep the first cut small.

- `BoundTransform`
  - one bound result per transform script
  - owns the top-level bound query result

- `BoundScope`
  - one scope per query boundary
  - contains visible rowsets and visible output names
  - parent link for nested lookup later

- `BoundRowset`
  - the central object
  - represents the resolved output shape of a table-like source or query block
  - owns ordered bound columns

- `BoundColumn`
  - one named column in a bound rowset
  - includes source identity when known
  - includes ordinal and exposed name

- `BoundTableSource`
  - binds one `TableReference` syntax node to one `BoundRowset`
  - carries alias/exposed name

- `BoundColumnReference`
  - binds one `ColumnReferenceExpression` syntax node to one resolved `BoundColumn`
  - keeps the original qualifier shape and the resolved target

This is intentionally not a giant semantic graph yet.

## Phase 1 Scope Rules

Included:

- `FROM dbo.T`
- `FROM dbo.T AS t`
- `SELECT t.Col`
- `SELECT Col` when unambiguous
- `SELECT *`
- `SELECT t.*`
- basic joins where both sides are already bindable named sources
- output column naming for simple select elements

Explicitly out for this first step:

- CTE binding
- derived tables
- recursive CTE shape
- `APPLY` visibility rules
- subquery binding
- function/table-valued-function rowset shape derivation
- windows
- set-operation reconciliation
- type inference
- target validation

If one of those appears, Phase 1 should fail explicitly as unsupported semantic binding scope, not guess.

## Data Flow

Phase 1 should look like:

`MetaTransformScript + SourceSchema`
`-> bound table sources`
`-> bound query scope`
`-> resolved select output rowset`

Concrete flow:

1. Read the transform syntax.
2. Resolve each simple named table reference against sanctioned source schema truth.
3. Produce one `BoundRowset` per resolved table source.
4. Build the query-block `BoundScope` from visible table sources and aliases.
5. Resolve column references against that scope.
6. Expand `*` and qualified `alias.*` against bound rowsets.
7. Produce the bound output rowset for the `SELECT`.

## Implementation Order

1. Define the semantic model/workspace for the binding layer.
2. Add a binder service that consumes `MetaTransformScript` plus source schema.
3. Bind simple named table references to source tables.
4. Build per-query scope from `FROM` sources and aliases.
5. Bind simple column references.
6. Derive select-output columns, including `*` and qualified `alias.*`.
7. Emit explicit binding issues for unresolved or ambiguous names.

## Acceptance Criteria

Done for Phase 1 means:

- a simple single-query view with named table sources binds without guessing
- every simple named table reference resolves to a sanctioned source table
- every simple column reference either:
  - resolves to exactly one bound column, or
  - produces one explicit binding issue
- `SELECT *` and `SELECT alias.*` expand to ordered output columns from the bound rowset
- the binder derives one output rowset for the top-level query block
- binding results are persisted in a separate semantic artifact, not injected into `MetaTransformScript`

Representative proof cases:

- single-table select with aliases
- join with unqualified but unambiguous columns
- join with ambiguous unqualified column reference producing one issue
- `SELECT *`
- `SELECT t.*`

## Example Outcomes

Example:

```sql
SELECT
    c.CustomerId,
    c.CustomerName
FROM sales.Customer AS c
```

Expected semantic result:

- one `BoundTableSource` for alias `c`
- one `BoundRowset` for `sales.Customer`
- two bound column references
- one derived output rowset with:
  - `CustomerId`
  - `CustomerName`

Example:

```sql
SELECT Id
FROM sales.Customer AS c
JOIN sales.[Order] AS o
    ON o.CustomerId = c.CustomerId
```

Expected semantic result:

- `Id` is ambiguous
- binder returns one explicit binding issue
- no guessed column target

## Non-Goals For Phase 1

Do not do these yet:

- inferred data types
- nullability inference
- target-schema comparison
- conversion sanctioning
- lineage
- orchestration classification
- broad semantic best-effort behavior outside the simple binding spine

## Tracking Checklist

- [ ] semantic binding model defined
- [ ] binder service scaffolded
- [ ] named table references bind to source schema
- [ ] aliases populate query scope
- [ ] simple column references bind
- [ ] `SELECT *` expands from bound rowsets
- [ ] qualified `alias.*` expands from bound rowsets
- [ ] simple select-output rowset is derived
- [ ] unresolved names produce explicit binding issues
- [ ] ambiguous names produce explicit binding issues
- [ ] representative Phase 1 proof cases pass
