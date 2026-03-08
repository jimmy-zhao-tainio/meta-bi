# Business <-> BDV Weave Decision

## Current decision

For the Business -> Business Data Vault path:

- flat direct anchors stay in `MetaWeave`
- parent-scoped child consistency moves into `MetaFabric`

This is no longer hypothetical. The current foundation now supports scoped validation over weave workspaces through `meta-fabric`.

## What flat weave handles well

`MetaWeave` remains the correct place for direct, context-free anchors such as:

- `BusinessHub.Name` -> `BusinessObject.Name`
- `BusinessLink.Name` -> `BusinessRelationship.Name`

These bindings are structurally simple:

- one source property
- one target property
- no parent context required

## What fabric now handles

`MetaFabric` is now the sanctioned place for child bindings that only become deterministic inside a resolved parent binding context.

The current worked BI example is:

- parent weave:
  - `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-LinkRelationship-Commerce`
  - `BusinessLink.Name` -> `BusinessRelationship.Name`
- child weave:
  - `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce`
  - `BusinessLinkEnd.RoleName` -> `BusinessRelationshipParticipant.RoleName`
- scoped fabric:
  - `Fabrics/Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce`

Without fabric, the child weave is ambiguous because `Customer` appears in more than one relationship participant row.

With fabric, the child binding becomes deterministic by scoping it under:

- parent binding: `BusinessLink.Name` -> `BusinessRelationship.Name`
- source parent reference: `BusinessLinkId`
- target parent reference: `BusinessRelationshipId`

## What remains unresolved

The remaining hard case is:

- `BusinessHubKeyPart` -> `BusinessKeyPart`

This is harder than the link-participant seam because the child rows do not scope through the same direct parent binding:

- source child parent: `BusinessHub`
- target child parent: `BusinessKey`

That is a multi-hop or cross-parent seam, not a simple shared-parent seam.

Current `MetaFabric` does not yet model that richer path.

## Practical implication

The project no longer needs to hide scoped Business/BDV consistency inside `meta-datavault` for the direct shared-parent cases.

The split is now:

- `MetaWeave` for flat direct anchors
- `MetaFabric` for shared-parent scoped anchors
- `meta-datavault` only for DV-domain logic that is still outside those two foundational capabilities

## Next architectural question

If the hub-key-part case, or similar cases in other sanctioned model families, becomes common, then the next foundational step is not another CLI-side special case.

It is a richer fabric model for:

- multi-hop parent scoping
- chained scoped bindings
- cross-parent coordination
