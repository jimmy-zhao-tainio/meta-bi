# MetaTransformScript Binding Plan

First concrete semantic-layer plan after syntax ownership.

`MetaTransformScript` remains the canonical syntax model.
This plan is for the first derived semantic layer over that syntax:

- binding
- name resolution
- rowset-shape derivation

Type inference and target validation come later. They should build on this layer, not replace it.

## Architectural Invariant

Keep this invariant across all phases:

`MetaTransformScript`
`-> binding layer`
`-> inferred type and output-shape layer`
`-> target validation layer`

Never reverse that direction.
Never smear semantic facts back into syntax entities.

## Active Language Profile Invariant

Binding and later semantic phases must evaluate one transform under one resolved active language profile.

Resolution rule:

1. semantic call input override
2. else `TransformScript.LanguageProfileId`
3. else explicit failure

Additional rules:

- no semantic phase may invent a default profile
- binder, inference, and validation must use the same resolver contract
- profile admissibility is a separate semantic contract over syntax, not a property of syntax nodes themselves

## Phase Map

Phase 1 is detailed and executable below.
Phases 2 and onward are intentionally higher level, but their purpose and done-criteria still matter now because Phase 1 should not choose shapes that make later binding, typing, and validation awkward.

### Phase 1: Base Binding Spine

Bind the easy spine only:

- named table references
- table aliases
- plain column references
- simple `SELECT` output columns
- simple `*` and qualified `alias.*`
- simple query-block scope

Done means:

- simple query blocks produce an explicit bound output rowset or explicit binding issues
- no guessing is used for unresolved or ambiguous names

### Phase 2: Query-Boundary Composition

Add:

- derived tables
- subqueries in `FROM`
- CTEs
- nested scopes
- output-rowset handoff from one query boundary into another

Key idea:

- every query boundary consumes an input scope and produces an output rowset

Done means:

- a query can bind not only base tables, but also rowsets produced by other query boundaries
- visibility rules are explicit and deterministic

### Phase 3: Advanced Visibility And Rowset-Producing Constructs

Add:

- `APPLY`
- correlated subqueries where sanctioned
- recursive CTE shape handling
- set-operation rowset-shape reconciliation at the binding level

This phase is still about rowset existence and visibility, not full scalar typing.

Done means:

- the binder can explain scope and rowset visibility across lateral/correlated constructs and multi-branch query forms
- unsupported constructs are rejected explicitly rather than guessed

### Phase 4: Scalar Type Inference Core

Add type and nullability inference for:

- literals
- direct column references
- arithmetic
- comparisons
- boolean predicates
- `CASE`
- `COALESCE`
- `NULLIF`
- `IIF`
- `CAST`
- `CONVERT`
- `TRY_*`

Each expression result should classify as:

- exact
- sanctioned-converted
- unknown
- unsupported

Done means:

- each supported bound output column has an inferred type/nullability result with an explicit reasoning class

### Phase 5: Rowset Schema Inference

Lift scalar inference to query outputs.
Infer each produced rowset schema:

- column names
- ordinals
- types
- length / precision / scale where relevant
- nullability
- origin / reasoning class

Add:

- aggregates
- group-by output rules
- window-function output typing where supported

Done means:

- every supported query boundary can produce a fully inferred output schema, not just names

### Phase 6: Target Validation

Compare inferred output schema to a declared target contract.
Check:

- column count
- names
- ordinals where relevant
- types
- nullability
- length / precision / scale
- sanctioned conversions
- missing mappings
- incompatible assignments
- structural mismatch

Done means:

- given a bound and inferred transform plus a target schema, the system can say:
  - compatible
  - compatible with sanctioned conversions
  - not compatible
- explicit issues identify where and why

### Phase 7: Support Classification

Do not design orchestration policy here.
Do prepare the semantic layer so later orchestration intelligence becomes possible.

Emit stable semantic classifications such as:

- simple projection
- projection plus joins
- grouped aggregate
- recursive
- lateral / correlated
- unsupported conversion reliance
- unknown-type leak

Done means:

- the semantic layer can emit stable structural classifications from semantic facts, not syntax heuristics

## Semantic Boundary Rules

Binding answers:

- what this name refers to
- what rowsets are visible here
- what output rowset this query boundary produces structurally

Language-profile evaluation answers:

- which profile is active for this semantic pass
- whether a present syntax feature is admitted by that profile
- whether the feature is unclassified by that profile

Type inference answers:

- what type and nullability a bound expression or output column has
- whether that result is exact, sanctioned, unknown, or unsupported

Validation answers:

- whether the inferred output fits the target contract
- where and why it does not

Anything that does not fit one of those categories should be treated suspiciously.

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
- one resolved active language profile
- one sanctioned source-schema workspace

Not yet:

- target schema
- type reconciliation rules
- validation policy

## Proposed Binding Artifact

The sanctioned semantic destination should now be an explicit binding model and instance, not just runtime-only records.

Binding should be rowset-centric:

- a transform can admit many source rowsets `S1..Sn`
- query structure derives new rowsets from those inputs
- the transform yields one final output rowset `T`

Minimal sanctioned binding model:

- `TransformBinding`
  - one binding result per transform
  - carries the resolved active language profile
  - points to the final output rowset

- `BoundRowset`
  - the central semantic object
  - represents a source, intermediate, or final rowset state
  - owned by one `TransformBinding`
  - carries derivation kind and producing syntax id when relevant

- `BoundRowsetInput`
  - links one derived `BoundRowset` to one of its input rowsets
  - keeps input order explicit
  - this is where a link entity is appropriate because rowset derivation is graph-shaped and sometimes multi-input

- `BoundColumn`
  - one ordered column in a `BoundRowset`
  - carries exposed name and source field/table identity when known

- `BoundTableSource`
  - records one visible table-source exposure
  - binds a `TableReference` syntax node and exposed alias/name to a `BoundRowset`

- `BoundIssue`
  - explicit binding and profile outcomes

Runtime helpers may still exist during implementation, but they are not the product artifact.
`BoundScope` is a good example of something that should remain implementation scaffolding unless and until we have a clear reason to persist it.

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
If a present construct is disallowed by the active language profile, or not classified by that profile, Phase 1 should emit that explicit profile outcome instead of collapsing it into generic unsupported binding.

## Data Flow

Phase 1 should look like:

`MetaTransformScript + ActiveLanguageProfile + SourceSchema`
`-> source bound rowsets`
`-> derived bound rowsets`
`-> final output rowset`

Concrete flow:

1. Read the transform syntax.
2. Resolve the active language profile using the shared resolver contract.
3. Classify any encountered profile-sensitive features needed for this semantic pass.
4. Resolve each simple named table reference against sanctioned source schema truth.
5. Produce one source `BoundRowset` per resolved table source.
6. Build the query-block runtime scope from visible table sources and aliases.
7. Resolve column references against that scope.
8. Expand `*` and qualified `alias.*` against bound rowsets.
9. Produce the derived output `BoundRowset` for the `SELECT`.
10. Persist the result as a `TransformBinding` instance with one distinguished final output rowset.

## Implementation Order

1. Define the semantic model/workspace for the binding layer.
2. Define the shared active-language-profile resolver contract.
3. Add a binder service that consumes `MetaTransformScript`, active language profile, and source schema.
4. Bind simple named table references to source tables.
5. Build per-query scope from `FROM` sources and aliases.
6. Bind simple column references.
7. Derive select-output columns, including `*` and qualified `alias.*`.
8. Emit explicit binding issues for unresolved or ambiguous names.
9. Emit explicit profile outcomes for disallowed or unclassified features encountered during supported semantic passes.

## Acceptance Criteria

Done for Phase 1 means:

- semantic analysis resolves one active language profile or fails explicitly
- a simple single-query view with named table sources binds without guessing
- every simple named table reference resolves to a sanctioned source table
- every simple column reference either:
  - resolves to exactly one bound column, or
  - produces one explicit binding issue
- `SELECT *` and `SELECT alias.*` expand to ordered output columns from the bound rowset
- the binder derives one output rowset for the top-level query block
- profile-sensitive constructs encountered during supported semantic passes produce distinct outcomes for:
  - disallowed by profile
  - unclassified by profile
  - allowed by profile but not yet implemented
- binding results are persisted in a separate semantic artifact, not injected into `MetaTransformScript`
- the persisted artifact is rowset-centric and can represent many source rowsets flowing into one final output rowset

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

- [x] semantic binding model defined
- [ ] active language profile resolver contract defined
- [ ] binder service scaffolded
- [ ] named table references bind to source schema
- [ ] aliases populate query scope
- [ ] simple column references bind
- [ ] `SELECT *` expands from bound rowsets
- [ ] qualified `alias.*` expands from bound rowsets
- [ ] simple select-output rowset is derived
- [ ] unresolved names produce explicit binding issues
- [ ] ambiguous names produce explicit binding issues
- [ ] disallowed profile features produce explicit semantic outcomes
- [ ] unclassified profile features produce explicit semantic outcomes
- [ ] representative Phase 1 proof cases pass
