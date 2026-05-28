The structural extraction is complete. Both ManifestPlanningEngine and MetaSqlDeployExecutionEngine are now coordinator-only. Do not do more broad refactoring.

Next pass goal:
semantic consolidation and small shared-infrastructure cleanup.

Priority order:
1. Remove duplication of column type-shape / alter-truncate executable-slice rules between planning assessment and SQL rendering.
2. Centralize repeated scoped identity match-key construction.
3. Centralize synthetic MetaSqlDifference creation used by expansion services.

Hard rules:
- planner decides
- manifest records
- deploy renders/executes only manifest
- no hidden deploy-time inference
- do not widen supported behavior during this pass
- preserve behavior and tests

Detailed requirements:

1. Column alter/truncate semantic convergence
- Planning side must remain the single owner of executable-slice semantics for column changes.
- Rendering side must stop re-deciding the supported alter/truncate slice.
- Introduce a shared result/contract produced by planning semantics that renderers can consume.
- Renderers may perform shallow invariant/assertion checks, but must not contain parallel business-rule classification.
- Preserve current supported slice and current blocking behavior.

2. Scoped identity helper centralization
- Repeated key construction like TableId|Name and SourceTableId|Name should move into a small shared helper/value-object layer.
- Use explicit names reflecting real MetaSql scoped identity.
- Replace duplicated string-building logic in policy/expansion classes.

3. Synthetic difference factory
- Expansion services should stop constructing synthetic MetaSqlDifference instances inline.
- Introduce a dedicated factory/helper for synthetic replacement/expansion differences.
- Keep semantics identical.

Deliverables:
- smaller, dumber renderers with less duplicated rule logic
- centralized scoped identity helper(s)
- centralized synthetic difference factory
- report what semantic duplication was removed and what remains intentionally duplicated, if any
