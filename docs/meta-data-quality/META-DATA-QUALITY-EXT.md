# MetaDataQuality Roadmap

This document is roadmap context only.

Do not implement any future phase unless explicitly instructed.

`meta-data-quality` derives reviewable data-quality candidates from a `MetaTransformScript` workspace.

It operates on the typed `MetaTransformScript` model, not on raw SQL text. It uses modeled transform scripts, query specifications, CTEs, derived query scopes, table references, joins, predicates, expressions, projections, and target mappings as evidence.

The analyzer has two main analysis scopes:

- transform-scope analysis: analysis of each transform independently
- corpus-scope analysis: analysis of repeated semantic patterns across the transform workspace

The term corpus means the collection of modeled transform scripts in a `MetaTransformScript` workspace.

## Current implementation status

### Phase 1 / transform-scope analysis

Transform-scope analysis inspects each transform independently.

Implemented capabilities include:

- join pattern discovery
- join occurrence preservation
- composite equality predicate extraction
- missing referenced row candidates from transform-local joins
- unexpected outer-join null candidates
- row multiplication risk candidates
- duplicate output risk candidates
- suppression of row-multiplication / duplicate-output candidates when the transform visibly projects detail-side identifiers

Transform-scope candidates are tied to individual transform occurrences.

### Phase 2A / relationship consensus

Implemented.

Corpus-scope relationship consensus aggregates repeated `JoinPattern` and `JoinPatternOccurrence` evidence across the workspace.

Implemented candidate families:

- `MinorityJoinPattern`
- `IncompleteCompositeJoin`
- `SuspiciousExtraJoinPredicate`

Purpose:

- identify relationship patterns that differ from the dominant pattern for the same canonical base-object pair
- identify incomplete composite joins where an outlier pattern is a strict subset of the dominant composite pattern
- identify suspicious extra join predicates where an outlier pattern is a strict superset of the dominant pattern

Current SQL generation:

- promoted candidates generate `SemanticReviewFinding` output rows in SQL
- no runtime suspect-row query is executed for these families

### Phase 2B / implied relationship-level integrity

Implemented.

Corpus-scope implied integrity promotes repeated relationship behavior into relationship-level integrity candidates.

Implemented candidate families:

- `ImpliedForeignKeyMissingReference`
- `ImpliedUniqueKeyViolation`

Purpose:

- consolidate repeated transform-local join evidence into higher-confidence relationship-level candidates
- infer relationship-level missing-reference checks from high-consensus join patterns
- infer lookup-side uniqueness checks from high-consensus lookup-side usage

Important distinction:

Transform-scope analysis can already produce missing-reference candidates from individual joins.

Phase 2B does not introduce missing-reference logic as a new concept. It changes the scope and evidence level:

- transform scope: this transform uses this join, so this transform can produce this missing-reference candidate
- corpus scope: many transforms repeatedly use this relationship, so the relationship itself is an inferred integrity contract

Current SQL generation:

- supported for promoted `ImpliedForeignKeyMissingReference`
- supported for promoted `ImpliedUniqueKeyViolation`

### Phase 2C / optionality drift

Implemented.

Corpus-scope optionality drift compares how a relationship pattern is treated across the workspace.

Implemented candidate families:

- `InnerJoinAgainstUsuallyOptionalRelationship`
- `LeftJoinAgainstUsuallyMandatoryRelationship`

Purpose:

- identify transforms that use `INNER JOIN` where the corpus usually treats the same oriented relationship as optional
- identify transforms that use `LEFT JOIN` where the corpus usually treats the same oriented relationship as mandatory

Important direction rule:

`A LEFT JOIN B` and `B LEFT JOIN A` are not equivalent optionality evidence. Optionality must be counted relative to the canonical side that is nullable/preserved. Mixed-direction `LEFT JOIN` evidence must not be merged into one generic left-join bucket.

Current SQL generation:

- promoted candidates generate `SemanticReviewFinding` output rows in SQL
- no runtime suspect-row query is executed for these families

Known limitation:

- self-join optionality is suppressed unless the model can distinguish relationship sides beyond base object name

### Phase 2E (first slice) / implied cardinality-risk signals

Implemented.

This first Phase 2E slice projects unsuppressed transform-scope fanout/duplicate-risk signals into corpus-scope relationship candidates when dominant relationship usage repeatedly carries those signals.

Implemented candidate families:

- `ImpliedJoinFanoutRisk`
- `ImpliedOutputDuplicateRisk`

Current SQL generation:

- promoted candidates generate `SemanticReviewFinding` output rows in SQL
- no runtime suspect-row query is executed for these families

### Phase 2D (first slice) / column equivalence graph

Implemented.

This first Phase 2D slice builds corpus column-equivalence edges from repeated join equality evidence and emits outlier candidates when a column is usually equated with one counterpart but minority usage points to another.

Implemented candidate families:

- `MinorityColumnEquivalence`

Current SQL generation:

- promoted candidates generate `SemanticReviewFinding` output rows in SQL
- no runtime suspect-row query is executed for these families

### Phase 2F (first slice) / filter consensus

Implemented.

This first Phase 2F slice captures normalized WHERE-filter observations and emits review candidates when dominant relationship usage consistently applies a filter that outlier usage omits.

Implemented candidate families:

- `MissingCommonFilter`

Current SQL generation:

- promoted candidates generate `SemanticReviewFinding` output rows in SQL
- no runtime suspect-row query is executed for these families

## Remaining planned implementation phases

The following phases are not implemented or are only partially implemented.

Design must be completed before implementation.

Recommended next technical phase:

- Phase 2G / target-column expression outliers, now that relationship consensus (2A), implied integrity (2B), optionality drift (2C), column equivalence (2D first slice), fanout/duplicate-risk signals (2E first slice), and filter consensus (2F first slice) are in place.
- Keep 2D/2E/2F as expandable foundations; their current slices are intentionally conservative.

### Phase 2D / column equivalence graph

Goal:

Build a corpus-wide graph of columns that are repeatedly equated through joins.

Examples:

- `Order.CustomerId`
- `Invoice.CustomerId`
- `Payment.CustomerId`
- `Customer.CustomerId`

may form an observed equivalence cluster if repeatedly joined to each other.

Candidate examples:

- a column is usually equivalent to one cluster but one transform equates it to a different column
- one relationship uses a key that conflicts with the dominant column equivalence graph
- a business-key column is joined to a descriptive/name/code column in one outlier transform

Current status:

- first slice implemented via `MinorityColumnEquivalence`
- remaining work is broader equivalence clustering and additional candidate families

Required design for next slice:

- equivalence-node identity
- equivalence-edge identity
- confidence thresholds
- direction-independent equality handling
- relationship to existing `CorpusRelationshipPattern`
- candidate families
- evidence model
- CLI summary format

Notes:

This is likely a useful foundation for later phases such as target-column mapping drift, CASE consistency, and type/cast checks.

### Phase 2E / join cardinality and fanout evidence

Goal:

Infer whether relationship usage suggests one-to-one, many-to-one, one-to-many, or potentially multiplying behavior.

Examples:

- a table repeatedly appears on the lookup/reference side of joins
- a relationship repeatedly creates row-multiplication candidates
- a relationship is used as if unique but may have multiple matches
- a join graph contains repeated fanout risk across many transforms

Candidate examples:

- relationship frequently multiplies rows
- lookup side appears non-unique
- transform uses a fanout relationship without projecting detail-side identifiers
- relationship appears one-to-many in practice but is used as many-to-one by transforms

Current status:

- partially implemented through `ImpliedJoinFanoutRisk` and `ImpliedOutputDuplicateRisk`
- remaining work includes richer cardinality directionality, one-to-one/many-to-one classification, and runtime-checkable realization for selected families

Required design before implementation:

- cardinality evidence model
- lookup/detail side inference
- integration with existing implied unique-key candidates
- runtime-checkable vs review-only distinction
- thresholds
- candidate families
- SQL realization rules

### Phase 2F / filter consensus and current-row conventions

Goal:

Detect common filters for a table/source and flag transforms that omit or differ from the common filter.

Examples:

- most transforms reading `Customer` include `IsDeleted = 0`, one omits it
- most transforms use `ValidTo IS NULL`, one does not
- most transforms filter `Status IN ('Active')`, one uses a different status condition
- most transforms reading a history table use a current-row convention

Candidate examples:

- `MissingCommonFilter`
- `FilterValueOutlier`
- `MissingCurrentRowConvention`
- `SoftDeleteFilterOmitted`

Current status:

- first slice implemented via `MissingCommonFilter`
- remaining work includes value outliers and broader current-row convention inference

Required design for next slice:

- filter predicate normalization
- table/source association
- literal normalization
- conjunction/disjunction handling
- filter equivalence rules
- thresholds
- candidate families
- evidence model

### Phase 2G / target-column expression outliers

Goal:

Compare how the same target column is populated across transforms.

Examples:

- most transforms populate `BKCustomerKod` from `Customer.Id`, one uses `Customer.Number`
- same target column has different cast lengths
- same target column has different null handling
- same target column has different `CASE` mapping

Candidate examples:

- `TargetExpressionOutlier`
- `TargetColumnLineageOutlier`
- `TargetColumnCastOutlier`
- `TargetColumnNullHandlingOutlier`

Required design before implementation:

- target identity
- target column identity
- expression fingerprinting
- source lineage extraction
- cast normalization
- null-handling normalization
- `CASE` expression normalization
- thresholds
- evidence model
- CLI summary format

### Phase 2H / transform-family outliers

Goal:

Detect structurally similar transforms and flag transforms that differ from their family.

Examples:

- same source relationship graph, but one transform has a different join predicate
- same target shape, but one transform omits a common join
- same transform pattern, but one transform has different optionality
- same family, but one transform has a different filter

Required design before implementation:

- transform fingerprint shape
- family grouping rules
- minimum family size thresholds
- outlier scoring
- evidence model
- CLI summary format

Notes:

This should probably be implemented after column equivalence, filter consensus, and target expression fingerprinting have enough stable primitives.

### Phase 2I / CASE and domain mapping consistency

Goal:

Compare `CASE` expressions and literal/domain mappings across transforms.

Examples:

- most transforms map `'K'` to `'Kvinna'`, one maps `'K'` differently
- one `CASE` expression lacks an `ELSE` branch
- the same business code is mapped inconsistently
- the same source status value maps to different target status values

Candidate examples:

- `CaseMappingConflict`
- `CaseMissingElse`
- `DomainLiteralMappingOutlier`
- `ConflictingCodeMapping`

Required design before implementation:

- `CASE` expression fingerprinting
- branch predicate normalization
- branch output normalization
- source domain identity
- target domain identity
- literal comparison rules
- evidence model

### Phase 2J / aggregate and grain risk

Goal:

Detect aggregate logic that may be affected by fanout or inconsistent grain.

Examples:

- `SUM` after a potentially multiplying join
- same metric calculated at different grains
- `COUNT(*)` versus `COUNT(column)` semantic mismatch
- target fact table populated at inconsistent grain
- grouped result projects columns that suggest a different grain

Candidate examples:

- `AggregateAfterFanout`
- `MetricDefinitionConflict`
- `CountSemanticMismatch`
- `TargetGrainConflict`
- `GroupByGrainRisk`

Required design before implementation:

- aggregate expression recognition
- group-by identity
- metric expression fingerprinting
- grain inference
- target-grain comparison
- relationship to join fanout evidence
- SQL realization rules

### Phase 2K / semantic differential SQL checks

Goal:

For selected review-only semantic candidates, generate SQL that compares dominant corpus logic with outlier logic and returns rows where they differ.

Possible first targets:

- `InnerJoinAgainstUsuallyOptionalRelationship`
- `IncompleteCompositeJoin`
- `SuspiciousExtraJoinPredicate`

Examples:

- for `IncompleteCompositeJoin`, compare matches from the outlier incomplete join against matches from the dominant composite join
- for `SuspiciousExtraJoinPredicate`, find rows where the dominant join matches but the outlier extra-predicate join does not
- for `InnerJoinAgainstUsuallyOptionalRelationship`, find rows that the inner join would drop but the dominant left-join pattern would preserve

Important:

These are runtime evidence checks for semantic outliers. They are not the same as ordinary relationship-level DQ checks.

Generated SQL must compare dominant and outlier logic explicitly. It must not silently treat the outlier as definitely wrong.

Required design before implementation:

- dominant pattern SQL rendering
- outlier pattern SQL rendering
- row identity for comparison
- difference result shape
- candidate-family-specific semantics
- review dashboard wording
- fail-fast behavior for unsupported differential checks

## Full capability backlog

This backlog preserves the larger set of possible capabilities. Items may be implemented in a different order if dependencies require it.

### 1. Relationship consensus checking

Compare join patterns used for the same canonical base-object pair.

Status:

- implemented in Phase 2A

Candidate examples:

- `MinorityJoinPattern`
- `IncompleteCompositeJoin`
- `SuspiciousExtraJoinPredicate`

### 2. Implied foreign-key discovery

Infer relationship-level missing-reference checks from repeated join behavior.

Status:

- implemented in Phase 2B

Candidate examples:

- `ImpliedForeignKeyMissingReference`

### 3. Join cardinality inference

Infer many-to-one, one-to-many, one-to-one, and fanout behavior from repeated join usage and runtime check evidence.

Status:

- partially implemented (review-only `ImpliedJoinFanoutRisk`, `ImpliedOutputDuplicateRisk`)

### 4. Optionality drift

Compare `INNER JOIN` and oriented `LEFT JOIN` treatment across the corpus.

Status:

- implemented in Phase 2C

Candidate examples:

- `InnerJoinAgainstUsuallyOptionalRelationship`
- `LeftJoinAgainstUsuallyMandatoryRelationship`

### 5. Composite key completeness

Detect joins that use only part of a dominant composite key.

Status:

- implemented in Phase 2A

Candidate examples:

- `IncompleteCompositeJoin`

### 6. Over-specified join candidates

Detect joins that add extra predicates compared with the dominant relationship pattern.

Status:

- implemented in Phase 2A

Candidate examples:

- `SuspiciousExtraJoinPredicate`

### 7. Column equivalence graph

Build equivalence clusters from repeated equality relationships.

Status:

- first slice implemented in Phase 2D (`MinorityColumnEquivalence`)
- broader equivalence graph capabilities are still planned

### 8. Filter consensus and accidental filtering

Detect common filters and filter outliers for source objects.

Status:

- first slice implemented in Phase 2F (`MissingCommonFilter`)
- broader filter-value and convention outlier coverage is still planned

### 9. Soft-delete and current-row convention discovery

Infer common lifecycle filters such as `IsDeleted = 0`, `DeletedDate IS NULL`, `ValidTo IS NULL`, or `IsCurrent = 1`.

Status:

- not implemented

### 10. Target contract consistency across transforms

Compare transforms that populate the same target object.

Status:

- not implemented

Examples:

- same target column populated from different source columns
- target column omitted in one transform
- target grain differs across transforms

### 11. Source-to-target mapping drift

Compare source lineage for target columns across transforms.

Status:

- not implemented

Examples:

- target column usually sourced from one column but one transform uses another
- same business key target populated from different source keys

### 12. Reused transform motifs / transform-family outliers

Cluster structurally similar transforms and detect outliers inside a family.

Status:

- not implemented

### 13. Join graph cycle and fanout risk

Analyze join graphs for repeated fanout or cyclic relationship risk.

Status:

- not implemented

### 14. Inferred unique-key candidates

Infer lookup-side uniqueness checks from repeated relationship usage.

Status:

- implemented in Phase 2B

Candidate examples:

- `ImpliedUniqueKeyViolation`

### 15. Grain inference and grain conflicts

Infer row grain from keys, projections, grouping, and target usage.

Status:

- not implemented

### 16. Aggregation correctness checks

Detect aggregate calculations that may be affected by fanout, grain mismatch, or inconsistent metric definitions.

Status:

- not implemented

### 17. Anti-join and exclusion pattern checks

Compare `LEFT JOIN ... IS NULL`, `NOT EXISTS`, `NOT IN`, `EXCEPT`, and similar exclusion patterns.

Status:

- not implemented

Examples:

- nullable `NOT IN` risk
- inconsistent exclusion semantics for the same relationship

### 18. Null-handling convention drift

Compare `COALESCE`, `ISNULL`, `NULLIF`, default values, and null-to-empty-string conventions.

Status:

- not implemented

### 19. Type / cast / length risk checks

Detect risky casts and inconsistent type conversion behavior.

Status:

- not implemented

Examples:

- truncating cast
- inconsistent `varchar` length for the same target column
- numeric precision loss
- date conversion through string

### 20. Date-window and incremental-load convention checks

Detect common date-window and incremental-load filtering conventions.

Status:

- not implemented

Examples:

- missing incremental boundary
- different incremental column
- hard-coded date window
- open-ended history filter

### 21. Literal and magic-value outliers

Detect rare or inconsistent literal values in filters, mappings, or projections.

Status:

- not implemented

Examples:

- one transform hard-codes a different status literal
- one transform hard-codes a specific organization code
- one transform uses a rare literal for the same target concept

### 22. CASE expression consistency

Compare `CASE` mappings across transforms.

Status:

- not implemented

### 23. Source coverage checks

Analyze which source tables and columns are used, unused, or only partially used.

Status:

- not implemented

Examples:

- source column exists but is never used
- target column is never populated
- table is only used in filters and never projected
- source table disappeared from transform usage

### 24. Dependency impact ranking

Rank candidates by affected transform count, affected target count, relationship frequency, and centrality.

Status:

- not implemented

Examples:

- high-impact relationship appears in hundreds of transforms
- candidate affects a core fact table
- candidate is isolated to one low-use transform

### 25. Evidence-based candidate suppression

Use corpus evidence to suppress or downgrade candidates more accurately.

Status:

- partially implemented in transform-scope suppression for visible detail-side identifiers
- corpus-level evidence-based suppression not implemented

Examples:

- row multiplication is likely intentional when the transform projects detail-side identifiers
- a relationship is known to be one-to-many and output grain includes the many-side key
- suppress duplicate-output risk when inferred grain proves the output is detail-level

## Candidate classes by realization type

### SQL output modes

`data-quality-to-sql` currently uses three conceptual output modes:

- `RuntimeCheck`: candidate generates executable source-data SQL checks.
- `SemanticReviewFinding`: candidate generates informational SQL review findings (no runtime suspect-row query).
- `Unsupported`: candidate has no SQL realization and must fail fast when promoted.

### Runtime SQL-checkable candidates

Currently SQL-checkable:

- `ImpliedForeignKeyMissingReference`
- `ImpliedUniqueKeyViolation`

Potential future SQL-checkable candidates:

- filter convention checks
- soft-delete/current-row convention checks
- type/cast/length checks
- date-window checks
- literal/domain checks
- semantic differential checks for selected outlier candidates

### Semantic review findings in SQL output

Currently emitted as `SemanticReviewFinding`:

- `MinorityJoinPattern`
- `IncompleteCompositeJoin`
- `SuspiciousExtraJoinPredicate`
- `InnerJoinAgainstUsuallyOptionalRelationship`
- `LeftJoinAgainstUsuallyMandatoryRelationship`
- `ImpliedJoinFanoutRisk`
- `ImpliedOutputDuplicateRisk`

These candidates identify semantic outliers. They are included in generated SQL/dashboard output as informational findings and do not produce runtime suspect-row counts.

### Unsupported candidate families

Any promoted family that has neither `RuntimeCheck` nor `SemanticReviewFinding` realization must fail fast in `data-quality-to-sql`.

### Evidence / infrastructure capabilities

These may support multiple candidate families:

- column equivalence graph
- transform fingerprinting
- expression fingerprinting
- target lineage extraction
- grain inference
- dependency impact ranking
- evidence-based suppression

## Global rules for all future phases

- Preserve observation / inference / candidate separation.
- Do not auto-promote candidates.
- Do not generate SQL unless the candidate family has a clean SQL realization.
- Unsupported promoted candidates must fail fast.
- No concrete table, column, entity, or project-name special-casing.
- No compatibility fallbacks.
- No dual-write behavior.
- No silent skipping.
- Keep model changes minimal and only for implemented features.
- Do not add placeholder entities for future phases.
- Prefer small phases with focused tests and demo proof artifacts.
- Before implementing a future phase, inspect whether the current model contains sufficient evidence.
- If required evidence is missing, stop and report the minimal model change instead of guessing.
- Preserve deterministic ordering and canonicalization.
- Persist enough evidence for candidates to be explainable.
- Treat raw transform counts as provisional evidence strength; add transform-family weighting only when a stable family identity is modeled.
- Keep candidate lifecycle and promotion semantics centralized in `DataQualityCandidate`.
- Do not duplicate promotion/status state in candidate detail entities.

## Required proof bar for future implementation phases

Each implemented phase should include:

- unit tests for normalization, threshold behavior, and candidate generation
- integration tests for workspace persistence and CLI inspect output
- converter tests for SQL-supported and SQL-unsupported candidate families
- at least one demo or sample proof when the feature affects user-visible CLI behavior
- no golden/demo output drift unless explicitly reviewed
