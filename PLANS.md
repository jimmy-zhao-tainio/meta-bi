# PLANS.md

Use this file for long-running work that can drift if the goal is not restated often.

## Plan Template

### Goal

What are we trying to achieve in one sentence?

### Non-goals

What are we explicitly not doing in this phase?

### Current understanding

What do we believe is true right now?

### Constraints

- Canonical artifact format
- Version bounds
- Ownership boundaries
- Acceptance contract

### Risks / likely drift modes

- Wrong abstraction level
- Corpus facts replacing declared structure
- Generic node/edge detours
- Heuristic semantics presented as truth
- Silent scope expansion

### Planned steps

1. Smallest meaningful step
2. Next validating step
3. Stop and review

### Acceptance test

What must be true for this phase to count as done?

### Open questions

Unknowns that should be resolved explicitly, not guessed.

### Decision log

- Date:
- Decision:
- Why:

---

## Active Plan: Bounded SQL CREATE VIEW syntax model and round-trip pipeline

### Goal

Produce a version-specific, view-body-bounded SQL syntax model in the canonical meta/XML world that is faithful enough to support semantic round-trip work.

### Non-goals

- Generic SQL modeling beyond bounded SQL supported inside a view
- Semantic lineage modeling
- Transform semantics beyond syntax preservation
- Whitespace/comment/token-trivia preservation
- Final end-to-end emitter design for all cases

### Current understanding

- The current `MetaTransform` area is speculative and mostly experimental.
- The goal of that work is to reach a bounded SQL-in-view model, but progress is still limited.
- There is exploratory generator and workspace output under `MetaTransform`, but it should not be treated as a stable sanctioned model yet.
- The old failure mode was trying to extract a large AST-shaped model before proving parse, emit, and round-trip on any bounded executable slice.
- The next meaningful phase is to prove that the current direction can materialize real view-body SQL faithfully enough to justify continuing.
- A bounded phase-1 view-body slice now exists and has passed a concrete round-trip proof on a small curated corpus:
  - model workspace generation
  - typed tooling generation
  - SQL -> model instance workspaces
  - model instance workspaces -> SQL
  - SQL -> model instance workspaces again with exact canonical workspace match on the selected slice
- The bounded phase-1 slice now also covers:
  - `PIVOT`
  - `UNPIVOT`
  - derived-table carriers required by those surfaces
  - simple and searched `CASE`
  - `COALESCE`, `NULLIF`, `IIF`, and ordinary scalar function-call expressions such as `CHOOSE`
  - `CAST`, `TRY_CAST`, `CONVERT`, and `TRY_CONVERT`
  - primary-expression collation and parameterized SQL data types
  - `GROUP BY`
  - `HAVING`
  - chained `UNION ALL` / `UNION` / `EXCEPT` / `INTERSECT`
  - aggregate calls including `COUNT(*)`
  - window functions with `OVER`
  - partition/order/frame window semantics
  - named `WINDOW` clause definitions
  - scalar subqueries
  - `EXISTS`
  - `ANY` / `ALL` / `IN (subquery)` predicate forms
  - query-parenthesized set expressions
  - `SELECT *` and qualified star projection
  - `CROSS APPLY` / `OUTER APPLY`
  - inline `VALUES` sources
  - join-parenthesized table references
  - `GROUPING SETS`, `ROLLUP`, and `CUBE`
  - `TOP`
  - query-level `ORDER BY`
  - `OFFSET ... FETCH`
- The bounded phase-1 slice now also covers:
  - `TABLESAMPLE`
  - `IS DISTINCT FROM`
  - built-in/global table functions such as `GENERATE_SERIES` and `STRING_SPLIT`
  - full-text predicate and table-reference surfaces such as `CONTAINS` and `CONTAINSTABLE`
  - unary sign expressions
  - numeric / real / binary / null / `max` literal families needed by the selected corpus
  - `PARSE` / `TRY_PARSE`
  - `AT TIME ZONE`
  - sequence/global surfaces such as `NEXT VALUE FOR` and `@@SPID`
- The current typed tooling and round-trip path now depend on the local `meta` repo build for consistent C# generation during this experiment; the `meta.exe` on `PATH` has behaved inconsistently for this workstream.

### Constraints

- Canonical destination is XML workspace/model/instances.
- Target one ScriptDOM version only: the one already referenced in this repo.
- The schema follows declared ScriptDOM structure, not merely observed corpus alternatives.
- No generic `ParsedNode` / `ParsedEdge` destination model.
- The root is the view body, not `CREATE VIEW` DDL.

### Risks / likely drift modes

- Reintroducing concrete-union relationship explosion
- Letting observed corpus reachability define the schema
- Pulling token/source-position trivia back into the round-trip contract
- Mixing syntax modeling with lineage/semantic modeling too early
- Producing intermediate artifacts that are informative but not usable
- Treating experimental `MetaTransform` artifacts as if the repo has already converged on a final model

### Planned steps

1. Keep the current bounded phase-1 slice stable:
   - query expressions
   - boolean/search conditions
   - scalar/value expressions
   - derived tables
   - `PIVOT` / `UNPIVOT`
   - broad value-expression families now proven in corpus
   - grouping/having
   - chained set operations
   - window functions
   - explicit ordered collections
2. Extend the next bounded slice only through still-important non-external view-body surfaces:
   - re-check the remaining corpus for any still-unproven non-external view-body surfaces
   - keep `CREATE VIEW` wrapper-only syntax out of the model boundary
   - keep external-source wrappers out until we deliberately widen scope
3. Prove the same round-trip contract on a small corpus for that next slice before widening further.
4. Stop and review before broadening into external-source, ODBC-specific, or other highly specialized surfaces.

### Acceptance test

- The current bounded syntax `model.xml` preserves declared ScriptDOM property structure without concrete-role explosion.
- The current phase demonstrates enough faithful structure on a representative bounded SQL view-body slice to justify further investment.
- The current phase now proves the same for pivot/unpivot semantics without introducing token-trivia dependence or generic node/edge detours.
- The current phase now also proves the same for broader value-expression families without collapsing declared base types or reintroducing heuristic emission.
- The next phase must prove the same for subqueries/correlation without collapsing nested query boundaries into scalar shortcuts.
- The current phase now also proves the same for:
  - grouping extensions
  - top/order/paging semantics
  - star/apply/inline-values/source-parenthesis semantics
- The current phase now also proves the same for:
  - table sampling
  - distinct predicates
  - built-in/global table-function references
  - full-text predicate/table-reference surfaces
  - special literal and system-call surfaces
  - time-zone and sequence/global expression surfaces
- If that proof is weak, the plan should stop and revisit the modeling direction instead of pretending the model is already mature.

### Open questions

- Which remaining non-external corpus files still add important syntax truth instead of just surface noise?
- Should the next bounded slice prioritize remaining table-source variants, special predicates, or system/global-expression surfaces?
- At what point does phase 1 become broad enough that further widening should wait for a calmer second-pass syntax/semantics split?

### Decision log

- Date: 2026-04-02
- Decision: Treat the bounded `CREATE VIEW` model as a version-specific syntax model first, not a semantic or lineage model.
- Why: Round-trip fidelity depends on faithful syntax structure before derived meaning layers are added.
- Date: 2026-04-02
- Decision: Encode optional ScriptDOM fragment edges as explicit typed link entities rather than direct meta relationships.
- Why: Canonical meta relationships are required by default; optional AST structure must therefore be represented by optional link rows.
- Date: 2026-04-02
- Decision: Keep advancing the bounded proof by widening the existing phase-1 path instead of carrying a parallel phase-2 namespace/tooling branch.
- Why: The separate branch introduced bootstrap overhead before it added product truth. The repo needs working round-trip slices more than parallel scaffolding.
- Date: 2026-04-02
- Decision: Add grouping/having and chained set operations to the working phase-1 slice.
- Why: They are common real view-body constructs, they fit the existing model shape cleanly, and the round-trip proof now passes on representative corpus files.
- Date: 2026-04-02
- Decision: Add window functions and named window clauses to the working phase-1 slice.
- Why: Window semantics are a basic requirement for real SQL authoring, and the bounded phase-1 round-trip proof now passes on representative analytic corpus files.
- Date: 2026-04-03
- Decision: Add `PIVOT`, `UNPIVOT`, and the required derived-table carriers to the working phase-1 slice.
- Why: They are common real view-body constructs, and the bounded round-trip proof now passes on representative pivot/unpivot corpus files without flattening declared identifier quote intent.
- Date: 2026-04-03
- Decision: Add broader value-expression families to the working phase-1 slice.
- Why: `CASE`, cast/convert families, special primary expressions, and collation are common view-body constructs, and the bounded round-trip proof now passes on representative value-expression corpus files while preserving declared inheritance and identifier quote intent.
- Date: 2026-04-03
- Decision: Add scalar subqueries, subquery predicates, and query-parenthesis structures to the working phase-1 slice.
- Why: Nested query/value boundaries are central to real SQL authoring, and the bounded round-trip proof now passes on representative correlation and subquery corpus files without collapsing query structure into scalar shortcuts.
- Date: 2026-04-03
- Decision: Add star projection, apply sources, inline values, and join-parenthesized table references to the working phase-1 slice.
- Why: They are ordinary source-shaping structures in real view bodies, and the bounded round-trip proof now passes on representative corpus files while keeping declared structure and ordered collections explicit.
- Date: 2026-04-03
- Decision: Add grouping extensions plus top/order/paging semantics to the working phase-1 slice.
- Why: `GROUPING SETS` / `ROLLUP` / `CUBE` and query-level `TOP` / `ORDER BY` / `OFFSET-FETCH` materially change view-body syntax and semantics, and the bounded round-trip proof now passes on representative corpus files without reverting to generic node/edge abstractions.
- Date: 2026-04-03
- Decision: Add the remaining ordinary non-external view-body surfaces from the current curated corpus to the working phase-1 slice.
- Why: `TABLESAMPLE`, `IS DISTINCT FROM`, built-in/global table functions, full-text syntax, special literal/system-call families, `AT TIME ZONE`, and sequence/global expressions all materially affect authored view-body syntax, and the bounded round-trip proof now passes on the widened 31-file corpus without changing the model boundary.

---

## Next Run Input: SQL Round-Trip Recovery Plan

Use this section as the explicit input for the next run unless a newer dated plan replaces it.

### Boundary

- We are taking another shot at the SQL line, but not the old way.
- The old failure mode was: extract a whole AST-shaped model first, then think about parse, emit, and round-trip later.
- New rule: no slice counts unless it can do `SQL -> model instances -> semantically equivalent SQL` for that same slice.
- The model root is the view body, not `CREATE VIEW`.

### Scope boundary

In scope for the finished bounded model:

- CTEs
- joins, predicates, grouping, having
- set operations
- core scalar/value expressions
- window functions
- `PIVOT` / `UNPIVOT`
- XML namespaces and XML method-style view constructs

Out for now:

- `OPENROWSET`
- `OPENJSON`
- other external-source surfaces

### Execution method

Build the model slice-by-slice, not entity-by-entity.

Each slice must include:

1. curated `model.xml`
2. typed tooling
3. parser/lowerer into instances

---

## Active Plan: MetaTransformScript sanctioned SQL script subset

### Goal

Fork the current bounded SQL view-body proof into a sanctioned `MetaTransformScript` model family with a usable service and CLI surface.

### Non-goals

- Full SQL closure beyond the currently proven bounded subset
- Modeling `CREATE VIEW` wrapper syntax as product truth
- Semantic lineage or transform semantics above syntax

### Current understanding

- `MetaTransformScript` is now the sanctioned copy of the currently supported SQL view-body subset.
- The core syntax model is copied from the current bounded proof and renamed to `MetaTransformScript`.
- `TransformScript` is the collection/container row used to hold one or more supported SQL scripts in a workspace.
- `CREATE VIEW` wrappers are treated as import/export envelopes only.
- Wrapper schema/object names and explicit view column lists are now captured in the sanctioned model.
- `SET` statements and `GO`-separated batches are tolerated on import as long as supported `CREATE VIEW` statements can be found.
- View options and `WITH CHECK OPTION` are still rejected on import.

### Constraints

- Canonical destination is XML workspace/model/instances.
- `MetaTransformScript` is no longer a ScriptDOM artifact catalog; it is the sanctioned supported subset.
- Import/export code must use the sanctioned model and generated tooling, not generic node/edge graphs.
- CLI owns console output; the SQL service owns structured import/export behavior.

### Risks / likely drift modes

- Letting the new service silently drop wrapper semantics we do not model
- Reusing exploratory naming or harness logic and treating it as product API
- Forgetting that multi-script workspace support is now part of the contract

### Planned steps

1. Keep the copied bounded syntax model stable under the new `MetaTransformScript` name.
2. Harden the SQL service surface:
   - `from sql-file`
   - `from sql-code`
   - `to sql-path`
   - `to sql-code`
3. Verify one real folder-of-view-files import/export flow and one direct SQL-code flow.
4. Stop and review before widening support or adding workspace-maintenance commands.

### Acceptance test

- A folder containing representative `CREATE VIEW ... GO` scripts can be imported into a new `MetaTransformScript` workspace.
- A single `.sql` file containing multiple `CREATE VIEW` batches can be imported into a new `MetaTransformScript` workspace.
- A direct SQL code snippet can be imported into a new `MetaTransformScript` workspace.
- A `MetaTransformScript` workspace can emit body-only SQL code.
- A `MetaTransformScript` workspace can emit `CREATE VIEW ... GO` scripts back to disk, including explicit view column lists when present.
- Unsupported wrapper options fail loudly instead of being silently dropped.

### Open questions

- Should `OPENJSON` be admitted into the sanctioned subset now or later?
- Should file/folder export preserve original relative `SourcePath` layout or stay name-based for now?
- When should `MetaTransformScript` grow explicit workspace-maintenance commands beyond import/export?

### Decision log

- Date: 2026-04-03
- Decision: Fork the current bounded SQL view-body proof into a new sanctioned `MetaTransformScript` project family.
- Why: The repo now needs a sanctioned supported SQL subset with real import/export tooling, not just more exploratory `MetaTransform` work.
- Date: 2026-04-03
- Decision: Treat `CREATE VIEW` as an envelope on import/export, not as the core modeled syntax truth.
- Why: The current sanctioned model still focuses on the view body, while the practical CLI must still accept and emit ordinary view-script files.
- Date: 2026-04-03
- Decision: Capture wrapper schema/object identifiers and explicit view column lists in `MetaTransformScript`, while still ignoring `SET` statements and `GO` as import/export envelope mechanics.
- Why: Real-world view files commonly include wrapper column lists and wrapper naming that materially affect view meaning, while `SET` and `GO` are operational framing rather than core modeled SQL body truth.
4. emitter back to SQL
5. round-trip test

### Slice 1: executable spine

Scope:

- `SelectStatement`
- `WithCtesAndXmlNamespaces`
- `CommonTableExpression`
- `QueryExpression`
- `QuerySpecification`
- select elements
- `FromClause`
- named table references and aliasing
- joins
- `WhereClause`
- identifiers
- multipart identifiers
- column references
- literals
- core boolean expressions

Acceptance:

- a small corpus of real view bodies with CTEs, joins, aliases, and predicates round-trips through model instances

### Slice 2: relational shaping

Scope:

- set operations
- `GroupBy`
- `Having`
- functions
- `CASE`
- casts/converts

Acceptance:

- those constructs round-trip without schema hacks or concrete-role explosion

### Slice 3: windowing

Scope:

- `OVER`
- partition/order/frame
- named windows if required by declared structure

Acceptance:

- representative ranking/analytic view bodies round-trip

### Slice 4: business-real SQL

Scope:

- `PIVOT`
- `UNPIVOT`
- XML namespaces
- XML method-style constructs used in views

Acceptance:

- representative business-style view bodies round-trip

### Hard execution rules

- Declared property once, declared target type once, inheritance separately
- Ordered collections stay explicit
- No `ParsedNode` / `ParsedEdge` destination layer
- Corpus is verification evidence, not schema truth
- If a slice cannot emit semantically equivalent SQL without token trivia, stop and fix the model before adding scope
- If a construct forces concrete-alternative relationship explosion, the schema is wrong

### Ownership rule for next run

- Treat this as a compiler project, not an inventory project
- Each run must end with one of two outcomes:
  - a finished round-trip slice with explicit acceptance results
  - a clear stop report explaining why the current direction failed
