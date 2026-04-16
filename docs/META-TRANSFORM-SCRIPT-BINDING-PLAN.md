# MetaTransformScript Binding Plan

First concrete semantic-layer plan after syntax ownership.

`MetaTransformScript` remains the canonical syntax model.
This plan is for the first derived semantic layer over that syntax:

- binding
- name resolution
- rowset-shape derivation

Type inference and full target validation come later. They should build on this layer, not replace it.

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

- broader query-boundary composition, including CTEs and derived-table expansion
- subqueries in `FROM`
- CTEs
- nested scopes
- output-rowset handoff from one query boundary into another

Key idea:

- every query boundary produces an output rowset
- it may consume zero or more input rowsets depending on the construct

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
- what source rowsets and declared targets the transform names in SQL terms

Language-profile evaluation answers:

- which profile is active for this semantic pass
- whether a present syntax feature is admitted by that profile
- whether the feature is unclassified by that profile

Type inference answers:

- what type and nullability a bound expression or output column has
- whether that result is exact, sanctioned, unknown, or unsupported

Validation answers:

- whether bound source rowsets conform to sanctioned source tables
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

This is enough to prove the semantic direction without dragging in full recursive/set-operation CTE handling, windows, or harder expression rules immediately.

## Inputs

Phase 1 binding should take:

- one `MetaTransformScript` transform
- one resolved active language profile

Not yet:

- source schema
- target schema
- type reconciliation rules
- validation policy

## Proposed Binding Artifact

The sanctioned semantic destination should now be an explicit binding model and instance, not just runtime-only records.

Binding should be rowset-centric:

- a transform can admit zero or more source rowsets `S1..Sn`
- query structure derives new rowsets from those inputs
- the transform yields one final output rowset `T`

Minimal sanctioned binding model:

- `TransformBinding`
  - one binding result per transform
  - carries the resolved active language profile
  - points to the final output rowset

- `Rowset`
  - the central semantic object
  - represents a source, intermediate, or final rowset state
  - attached to one `TransformBinding`
  - carries derivation kind and producing syntax id when relevant

- `SourceTarget`
  - links one derived `Rowset` to one of its input rowsets
  - keeps input order explicit
  - this is where a link entity is appropriate because rowset derivation is graph-shaped and sometimes multi-input

- `Column`
  - one ordered column in a `Rowset`
  - carries exposed name and source field/table identity when known

- `TableSource`
  - records one visible table-source exposure
  - binds a `TableReference` syntax node and exposed alias/name to a `Rowset`

- `ColumnReference`
  - records one resolved syntax column reference
  - links the syntax column reference to its resolved `Column`
  - also records the resolved `TableSource`

- binding/profile issues remain runtime outcomes for now
  - they are not persisted as rows in `MetaTransformBinding`

Runtime helpers may still exist during implementation, but they are not the product artifact.
`BindingScope` is a good example of something that should remain implementation scaffolding unless and until we have a clear reason to persist it.

## Current Implemented Surface

The binder is now beyond the original Phase 1 spine.

Implemented now:

- active language profile resolution with explicit failure when unresolved
- source-less transforms
- named table references and aliases
- direct column references
- `SELECT *` and qualified `alias.*`
- basic join rowset composition
- query-derived tables in `FROM`
- non-recursive CTE rowset binding
- recursive CTE rowset-shape stabilization when the shape can be derived from:
  - explicit CTE column aliases
  - anchor-branch output names
- set-operation rowset binding for `UNION`, `UNION ALL`, `INTERSECT`, and `EXCEPT`
- lateral `CROSS APPLY` / `OUTER APPLY` with a query-derived-table right side
- schema-object function table references when the script supplies explicit column aliases
- `PIVOT` / `UNPIVOT` table references when their input rowset shape is syntax-derived
- correlated scalar subqueries
- correlated `EXISTS`
- correlated `IN (subquery)`
- correlated subquery-comparison predicates such as `> ALL (...)`
- scalar-expression traversal across common expression shells such as:
  - binary and unary expressions
  - parenthesized scalar expressions
  - value/special scalar leaves (`CURRENT_TIMESTAMP`, `NEXT VALUE FOR`, current sanctioned globals, and literals)
  - generic function-call parameter lists
  - `LEFT` / `RIGHT`
  - searched and simple `CASE`
  - `COALESCE`
  - `NULLIF`
  - `IIF`
  - `CAST` / `TRY_CAST`
  - `CONVERT` / `TRY_CONVERT`
  - `PARSE` / `TRY_PARSE`
  - `AT TIME ZONE`
- aggregate argument binding when those aggregates are represented through scalar call structure
- basic expression `GROUP BY` rowset binding
- `GROUP BY ALL` rowset binding
- advanced grouping-specification traversal across:
  - `GROUPING SETS`
  - `ROLLUP`
  - `CUBE`
  - composite grouping specifications
  - grand total grouping `()`
- basic `HAVING` traversal over grouped queries
- explicit grouped-query validation for ungrouped direct column references outside aggregate context
- window and ordered-set expression traversal across:
  - `OVER` partitions
  - `OVER` orderings
  - `ROWS` / `RANGE` frame offset expressions
  - query-level named `WINDOW` definitions
  - `WITHIN GROUP` orderings on generic function calls
- query-level modifier traversal across:
  - `DISTINCT`
  - `TOP`
  - query-level `ORDER BY`
  - `OFFSET` / `FETCH`
  - explicit `TOP ... WITH TIES` guard requiring query-level `ORDER BY`
- explicit source declaration capture from named SQL identifiers
- explicit target declaration capture from supplied SQL identifiers
- sanctioned global TVF output-shape inference for:
  - `OPENJSON` (default `key` / `value` / `type`)
  - `GENERATE_SERIES` (`value`)
  - `STRING_SPLIT` (`value`)

The persisted binding artifact is also live now:

- `TransformBinding`
- `Rowset`
- `SourceTarget`
- `Column`
- `TableSource`
- `ColumnReference`
- `TransformBindingTarget`
- validation link entities (`Validation*`) produced by Validate

## Honest Remaining Gaps

Important things that are still not truly implemented:

- profile feature classification beyond active-profile resolution
- grouping / aggregate output rules
- full window semantics and explicit named-window validation
- broader TVF support beyond script-supplied explicit alias shape
  - current sanctioned global TVF coverage is intentionally narrow and name-based
- `PIVOT` / `UNPIVOT` directly over base source rowsets where full source shape is not derivable from syntax alone
- deeper recursive semantics beyond rowset-shape stabilization
- broad nested-subquery support outside the currently implemented predicate/scalar shapes
- type inference
- validate currently appends explicit validation result rows inside `MetaTransformBinding` rather than emitting a separate workspace/artifact family
- source identifier resolution to `MetaSchema.TableId` is implemented in the current Validate slice
- source column-subset conformance is implemented in the current Validate slice
- target identifier resolution to `MetaSchema.TableId` is implemented in the current Validate slice
- target structural count/name validation is implemented in the current Validate slice
  - SQL identity columns are skipped on the target side
  - no data type compatibility validation yet
  - no nullability validation yet
  - no length / precision / scale validation yet
  - no sanctioned conversion classification yet
  - no modeled way to identify platform/system-generated target columns beyond SQL identity
  - no explicit target write-contract semantics for nullable, defaulted, computed, or platform columns
  - no source-to-target compatibility outcomes beyond structural rowset checks

So the binder can now explain a lot of rowset and visibility structure, but it is still intentionally shallow in scalar semantics.

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

Originally out for this first step:

- recursive CTE shape
- `APPLY` visibility rules
- subquery binding
- function/table-valued-function rowset shape derivation
- windows
- set-operation reconciliation
- type inference
- full target validation

Several of the items above are now implemented and should no longer be treated as active Phase 1 gaps.
The remaining unsupported shapes should still fail explicitly rather than guessed.
If a present construct is disallowed by the active language profile, or not classified by that profile, Phase 1 should emit that explicit profile outcome instead of collapsing it into generic unsupported binding.

Basic non-correlated query-derived tables in `FROM` are now the first implemented slice of Phase 2.
Basic non-recursive `WITH` CTEs that resolve to a single bindable query boundary are also now implemented.
Basic `CROSS APPLY` / `OUTER APPLY` with a query-derived-table right side are now implemented as lateral query-boundary binding.
Schema-object function table references are now bindable when the script explicitly supplies table-reference column aliases.
Function/table-valued-function rowset derivation without an explicit sanctioned shape still produces explicit issues.
Set-operation rowset binding is now implemented for query-boundary shape reconciliation across `UNION`, `UNION ALL`, `INTERSECT`, and `EXCEPT`.
Recursive CTE rowset-shape binding is now implemented when the recursive shape can be stabilized from explicit CTE column aliases or anchor-branch output names.
Deeper recursive semantics still remain outside this phase, but recursive self-reference no longer blocks binding outright.
Correlated scalar subqueries, `EXISTS`, `IN (subquery)`, and subquery-comparison predicates are now implemented for query-boundary binding and outer-scope name visibility.
Expression binding still remains intentionally shallow in semantics even though traversal is now broad: direct column references, current scalar shells, supported window/ordered-set clauses, and the current sanctioned subquery/predicate shapes are walked, while type and full function semantics are still deferred.
Modeled predicate families such as `BETWEEN`, `IN (...)`, `LIKE` (including `ESCAPE`), `IS NULL`, `IS DISTINCT FROM`, and full-text predicates (`CONTAINS` / `FREETEXT`) are now traversed for name resolution.

## Data Flow

Binding should look like:

`MetaTransformScript + ActiveLanguageProfile`
`-> source bound rowsets`
`-> derived bound rowsets`
`-> final output rowset`

Validate should look like:

`TransformBinding + MetaSchema`
`-> source validation outcomes`
`-> target validation outcomes`

Concrete flow:

1. Read the transform syntax.
2. Resolve the active language profile using the shared resolver contract.
3. Classify any encountered profile-sensitive features needed for this semantic pass.
4. Produce one source `Rowset` per named SQL source reference.
5. Build the query-block runtime scope from visible table sources and aliases.
6. Resolve column references against that scope.
7. Expand `*` and qualified `alias.*` when the visible rowset shape is derivable from syntax alone.
8. Produce the derived output `Rowset` for the `SELECT`.
9. Persist the result as a `TransformBinding` instance with one distinguished final output rowset plus unresolved source/target declarations.

## Next Recommended Slices

The next clean slices should stay honest about the current boundary:

1. profile feature classification beyond profile resolution
2. type inference handoff
3. broader table-source coverage (`OPENJSON`, `OPENROWSET`, `OPENQUERY`, `CHANGETABLE`, and TVF shapes without script-supplied alias shape)
4. expand Validate beyond the current structural slice over `TransformBinding + MetaSchema`

This keeps rowset and name-resolution truth ahead of type and validation work.

## Implementation Order

1. Define the semantic model/workspace for the binding layer.
2. Define the shared active-language-profile resolver contract.
3. Add a binder service that consumes `MetaTransformScript` and active language profile.
4. Bind simple named table references to source rowsets named in SQL terms.
5. Build per-query scope from `FROM` sources and aliases.
6. Bind simple column references.
7. Derive select-output columns, including `*` and qualified `alias.*` when the visible rowset shape is derivable from syntax.
8. Emit explicit binding issues for unresolved or ambiguous names.
9. Emit explicit profile outcomes for disallowed or unclassified features encountered during supported semantic passes.

## Acceptance Criteria

Done enough for the current implemented stage means:

- semantic analysis resolves one active language profile or fails explicitly
- a simple single-query view with named table sources binds without guessing
- a source-less transform can bind to a final output rowset
- every simple named table reference produces one explicit source rowset and source declaration
- every simple column reference either:
  - resolves to exactly one bound column, or
  - produces one explicit binding issue
- `SELECT *` and `SELECT alias.*` either:
  - expand to ordered output columns from a syntax-derived bound rowset, or
  - produce one explicit binding issue when schema truth would be required
- the binder derives one output rowset for the top-level query block
- query-derived tables produce distinct derived rowsets and table-source exposures
- CTEs produce distinct rowsets and visible names
- supported set operations produce explicit result rowsets with explicit mismatch issues when shapes do not align
- supported correlated subqueries bind against outer visible scope without smearing that scope into syntax
- binding results are persisted in a separate semantic artifact, not injected into `MetaTransformScript`
- the persisted artifact is rowset-centric and can represent zero or more source rowsets flowing into one final output rowset
- the persisted artifact carries source and target SQL identifiers without pretending schema validation already happened
- validation appends explicit source/target validation rows and fails hard on mismatch without mutating binding facts
- unsupported scalar-expression shapes still fail explicitly rather than silently disappearing

Not done yet:

- real profile feature classification outcomes beyond missing-profile failure
- broad scalar-expression coverage
- type inference
- deeper validate semantics beyond the current structural slice

Representative proof cases:

- single-table select with aliases
- join with unqualified but unambiguous columns
- join with ambiguous unqualified column reference producing one issue
- `SELECT *`
- `SELECT t.*`
- correlated scalar subquery in a select list
- `WHERE EXISTS (...)`
- `WHERE x IN (SELECT ...)`
- `WHERE x > ALL (SELECT ...)`
- common scalar expression shells such as `CASE`, `COALESCE`, `IIF`, casts, and parenthesized arithmetic
- aggregate argument binding such as `MAX(o.Amount)` within a supported query boundary
- basic expression `GROUP BY` with `HAVING`
- `GROUP BY ALL`
- recursive CTE with explicit column list
- recursive CTE with anchor-derived output names
- `CROSS APPLY` with a correlated derived-table right side
- direct `OVER (...)` window functions
- named `WINDOW` definitions
- `WITHIN GROUP` percentile calls with `OVER (...)`

## Example Outcomes

Example:

```sql
SELECT
    c.CustomerId,
    c.CustomerName
FROM sales.Customer AS c
```

Expected semantic result:

- one `TableSource` for alias `c`
- one `Rowset` for `sales.Customer`
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
- source-schema comparison
- target-schema comparison
- conversion sanctioning
- lineage
- orchestration classification
- broad semantic best-effort behavior outside the simple binding spine

## Tracking Checklist

- [x] semantic binding model defined
- [x] active language profile resolution contract enforced
- [x] binder service scaffolded
- [x] named table references bind from SQL identifier shape without requiring schema truth
- [x] aliases populate query scope
- [x] simple column references bind
- [x] `SELECT *` expands from syntax-derived bound rowsets or fails explicitly when schema truth would be required
- [x] qualified `alias.*` expands from syntax-derived bound rowsets or fails explicitly when schema truth would be required
- [x] simple select-output rowset is derived
- [x] query-derived tables bind as explicit rowset boundaries
- [x] non-recursive CTE rowsets bind
- [x] recursive CTE rowset shape can stabilize in sanctioned cases
- [x] set-operation rowset reconciliation is implemented
- [x] lateral `APPLY` works for the currently sanctioned query-boundary shapes
- [x] correlated predicate/scalar subquery binding works for the currently sanctioned shapes
- [x] unresolved names produce explicit binding issues
- [x] ambiguous names produce explicit binding issues
- [x] broader scalar-expression traversal is implemented for the current sanctioned shells
- [x] modeled predicate traversal for `BETWEEN`, `IN (...)`, `LIKE` / `ESCAPE`, `IS NULL`, and `IS DISTINCT FROM` is implemented
- [x] aggregate/function argument binding is implemented for scalar call structures
- [x] basic expression `GROUP BY` rowset binding is implemented
- [x] `GROUP BY ALL` binding is implemented
- [x] basic `HAVING` traversal is implemented
- [x] window and ordered-set traversal is implemented for the current sanctioned expression shapes
- [x] advanced grouping forms are implemented for the current sanctioned traversal shapes
- [x] `DISTINCT` binding is implemented
- [x] `TOP` binding is implemented
- [x] query `ORDER BY` binding is implemented
- [x] `OFFSET` / `FETCH` binding is implemented
- [x] query parenthesis binding is implemented
- [x] join parenthesis binding is implemented
- [ ] built-in and global TVF binding is implemented without script-supplied alias shape
- [ ] `OPENJSON` / `OPENROWSET` / `OPENQUERY` / `CHANGETABLE` binding is implemented
- [x] full-text predicate traversal (`CONTAINS` / `FREETEXT`) is implemented
- [x] full-text table-form binding (`CONTAINSTABLE` / `FREETEXTTABLE`) is implemented
- [x] XML namespace / method binding is implemented
- [x] `PIVOT` / `UNPIVOT` binding is implemented for syntax-derived input rowsets
- [ ] data type validation is implemented
- [ ] nullability validation is implemented
- [ ] length / precision / scale validation is implemented
- [ ] sanctioned conversion classification is implemented
- [ ] platform/system-generated target columns beyond SQL identity can be identified explicitly
- [ ] target write-contract semantics beyond structural rowset checks are implemented
- [x] validation result entities are captured explicitly inside `MetaTransformBinding`
- [x] source and target identifier resolution against `MetaSchema` is implemented in the current Validate slice
- [x] structural source/target validation is implemented in the current Validate slice
- [x] representative current proof cases pass
