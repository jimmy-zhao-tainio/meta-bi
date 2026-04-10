# MetaTransformScript Next Steps

Captured note for the next implementation phase after the canonical round-trip syntax model.

You already have the hard substrate: a canonical, semantically round-trippable syntax model. To make it operational, you now need to add meaning to it in three passes:

- first schema binding
- then type inference
- then target validation

The key design rule is: do not stuff these into the syntax model itself. The syntax model should stay the truth about script structure. Binding, typing, and validation should be derived layers over that structure.

The clean mental model is this.

A MetaTransformScript instance says what the script is.  
Binding says what each name refers to.  
Type inference says what each expression produces.  
Validation says whether the produced output fits the target contract.

That separation will save you a lot of pain.

The first serious problem to solve is binding. Without binding, type inference is mostly fake.

Binding means you can take a reference like `i.Amount` and say exactly which source schema column that is. It also means you can expand `*`, resolve aliases, know what a CTE exposes, know what a derived table exposes, know which columns are in scope inside a query block, and know what the output columns of a subquery or `VALUES` table are.

So the real binding layer needs to understand scopes and rowsets.

At minimum, it should derive:

- a bound source for every table reference
- a bound output rowset for every query-producing node
- a bound target for every column reference
- a resolved output column list for every `SELECT`
- a scope chain for nested queries, CTEs, and subqueries

That is the foundation.

Once that exists, type inference becomes mechanical in a good way. Each bound source column has a declared type from the source schema. Literals have intrinsic types. Operators and functions have rules. `CASE`, `COALESCE`, `SUM`, `COUNT`, `UNION`, window functions, casts, and comparisons all reconcile types according to explicit rules. Then every select item gets an inferred result type, and every intermediate rowset gets a derived schema.

This is also where nullability should be inferred, not just base type. For transform validation, nullability drift matters almost as much as type family.

Then target validation is just the final compare:

- projected output column count
- projected output names
- projected output types and nullability
- against the known target schema

That is where you can say things like:

- this transform projects 4 columns but target expects 5
- this expression infers `decimal(38,6)` but target expects `decimal(18,2)`
- this output is nullable but target is not
- this `SELECT *` expands to a different shape than the target contract
- this `UNION` branch reconciliation widened unexpectedly
- this target column requires an explicit sanctioned conversion

So the stack now wants a derived semantic model. Keep it very small at first.

You do not need fifty entities. You need a few sharp ones.

Something like:

- `BindingScope`
- `Rowset`
- `Column`
- `ColumnReference`
- `InferredType`
- `ValidationIssue`

That is enough to start.

A `Rowset` is the important one. Every table reference, derived table, CTE, subquery, and query block should resolve to a rowset with named columns and types. Once you have that, a lot of the rest becomes straightforward.

The correct order of implementation is also important.

Do not start with functions and weird edge cases.  
Do not start with target validation first.  
Start with rowset binding for the easy spine.

Suggested phase order:

- Phase 1: bind simple named table references and plain column references
- Phase 2: bind joins, aliases, and select-item output schemas
- Phase 3: bind CTEs and derived tables
- Phase 4: infer scalar types for literals, references, arithmetic, comparison, and aggregates
- Phase 5: infer query output schemas and compare to target schema
- Phase 6: add harder things like windows, set ops, `CASE`, `COALESCE`, casts/converts, and `*` in more complex contexts

That gets you to a useful validator quickly instead of disappearing into completeness work.

There is also one important decision to make now: what is the source of schema truth?

Given the existing ecosystem, the obvious answer is that source and target schemas should come from sanctioned models, not ad hoc in-script declarations. In other words, binding should work against a known schema workspace, and target validation should compare against a known target schema workspace. That keeps the whole thing in the meta world and avoids reintroducing blob semantics through the back door.

So the operational contract becomes:

`TransformScript + ActiveLanguageProfile + SourceSchema + TargetSchema`  
`-> binding`  
`-> inferred output schema`  
`-> validation result`

The active language profile should resolve by one invariant only:

1. semantic call input override
2. else `TransformScript.LanguageProfileId`
3. else explicit failure

That is a strong backend story.

The most important acceptance test for this phase is not â€œwe inferred some types.â€ It is:

for a supported transform with known source and target schemas, the system can either prove the transform is valid, or produce explicit, local validation issues without guessing.

That is the real threshold.

So yes, these are the last big pieces. After this, MetaTransformScript stops being just a canonical round-trip model and becomes an executable understanding substrate.

The next concrete move is to define the binding model first, not the type rules. Binding is the skeleton everything else hangs on.

## Test-bed note

- The current reference corpus includes subqueries and correlated subqueries.
- It does not yet contain a true nested subquery-inside-subquery example.
- Add that explicitly before later parser, binding, or validation phases rely on nested-scope behavior being covered by the corpus.
