# Temporary Roadmap

This is a working roadmap for the next concrete stretch of `meta-bi`.

It is intentionally narrow:

- get the business side structurally right
- get Business Vault anchored to business meaning
- only then make Data Vault materialization real
- only after that consider product artifact generation

It is not a long-term platform plan.

## Stage 1. Stabilize MetaBusiness

Goal:
- make `MetaBusiness` specific enough to drive downstream structure without dragging in analytics too early

Current floor:
- `BusinessObject`
- `BusinessKey`
- `BusinessKeyPart`
- `BusinessRelationship`
- `BusinessRelationshipParticipant`
- `BusinessProcess`
- `OrganizationUnit`
- role/process/org alignment

Exit criteria:
- the model feels business-specific and actionable
- business keys identify business objects, not processes
- business relationships can anchor future BDV links
- no obvious analytical-detail drift remains in the model

## Stage 2. Stabilize MetaBusinessDataVault

Goal:
- make sure `MetaBusinessDataVault` can faithfully represent a real BDV structure family

Current floor:
- hubs
- links
- satellites
- PIT
- bridge
- explicit helper entities

Exit criteria:
- table families and structural relations are explicit
- no hidden generator knowledge is required to understand what a BDV row means
- implementation naming stays out of the BDV model itself
- the repo has an explicit answer for where business-derived column typing lives

## Stage 3. Define the Business <-> BDV consistency seam

Goal:
- define exactly what must line up between `MetaBusiness` and `MetaBusinessDataVault`

Current intended anchors:
- `BusinessHub` -> `BusinessObject`
- `BusinessHubKeyPart` -> `BusinessKeyPart`
- `BusinessLink` -> `BusinessRelationship`
- `BusinessLinkHub` -> `BusinessRelationshipParticipant`

Exit criteria:
- the anchor set is agreed
- each anchor has a clear semantic reason
- we know which BDV structures inherit meaning from parents and do not need direct first-pass anchoring

## Stage 4. Decide where parent-scoped consistency lives

Problem:
- current `MetaWeave` is flat property binding
- BDV/business consistency needs parent-scoped checks on child rows

Decision options:
1. extend `MetaWeave`
   - add scoped/grouped binding semantics
2. keep `MetaWeave` flat
   - let `meta-datavault` own deeper business/BDV consistency checks

Exit criteria:
- one path is chosen explicitly
- we stop pretending the current flat weave is enough if it is not

## Stage 5. Make meta-datavault materialization real

Goal:
- make `meta-datavault` materially useful beyond empty workspaces and placeholder validation

Minimum useful target:
- materialize `MetaRawDataVault` from sanctioned inputs without heuristics
- later materialize `MetaBusinessDataVault` from sanctioned inputs and explicit business anchoring

Inputs should be explicit and sanctioned.

Exit criteria:
- no hidden business-key inference
- no hidden business-side meaning inference
- failures are precise and early
- resulting workspaces validate cleanly

## Stage 6. Reassess artifact generation boundary

Only after stages 1-5.

Question:
- does `meta-datavault` stop at sanctioned workspaces?
- or should it also participate in artifact generation?

Current recommended stance:
- `meta-datavault` owns DV semantics and materialization
- generic `meta` owns generic representation emission
- future `MetaSSDT` owns SQL Server product artifact specifics

Exit criteria:
- artifact responsibility is explicit
- no domain/tool/product responsibility drift

## Stage 7. Only then begin MetaSSDT

Goal:
- model SQL Server product artifacts on top of already-stable business and vault semantics

Reason:
- otherwise product artifacts become the dumping ground for unresolved model semantics

Exit criteria:
- there is already a stable sanctioned workspace that deserves SQL Server realization
- `MetaSSDT` can stay product-specific instead of compensating for missing upstream meaning

## Things explicitly deferred

These are real, but not the next move:

- `MetaAnalysis`
- measures and KPI detail
- `MetaTransform`
- SSDT/sqlproj specifics
- deployment and runtime state
- monitoring/ops overlays

## Current advice

The next real implementation work should stay inside stages 1-4.

If we skip that and jump to artifact generation, we will hardcode missing semantics into tooling.

## Additional deferred notes

- Reference data is now implemented in the explicit BDV baseline, but its longer-term conceptual framing should stay under review so it remains a lookup-oriented structure rather than drifting into a separate domain.
- Hash algorithm and hash-input semantics are intentionally deferred until load-generation work begins. When that happens, `MetaDataVaultImplementation` should own explicit settings for algorithm, representation, separator, null replacement, and compatibility behavior.