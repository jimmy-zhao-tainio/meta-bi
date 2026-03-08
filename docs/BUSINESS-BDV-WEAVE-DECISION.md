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

## What fabric handles

`MetaFabric` is now the sanctioned place for child bindings that only become deterministic inside a resolved parent binding context.

The first worked BI example is:

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
- source path: `BusinessLinkId`
- target path: `BusinessRelationshipId`

The second worked BI example is:

- parent weave:
  - `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-HubObject-Commerce-RepeatedKeyPart`
  - `BusinessHub.Name` -> `BusinessObject.Name`
- child weave:
  - `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce`
  - `BusinessHubKeyPart.Name` -> `BusinessKeyPart.Name`
- scoped fabric:
  - `Fabrics/Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce`

Without fabric, the child weave is ambiguous because `Identifier` appears in more than one business-key-part row.

With fabric, the child binding becomes deterministic by scoping it under:

- parent binding: `BusinessHub.Name` -> `BusinessObject.Name`
- source path: `BusinessHubId`
- target path: `BusinessKeyId.BusinessObjectId`

## Practical implication

The project no longer needs to hide scoped Business/BDV consistency inside `meta-datavault` for either:

- direct shared-parent cases
- multi-hop path-to-parent cases

The split is now:

- `MetaWeave` for flat direct anchors
- `MetaFabric` for scoped anchors over weave bindings
- `meta-datavault` only for DV-domain logic that is still outside those two foundational capabilities

## Next architectural question

If more complex cases in other sanctioned model families become common, then the next foundational step is not another CLI-side special case.

It is a richer fabric model for:

- multi-parent scoped bindings
- conditional scope across several parent bindings
- richer structural predicates beyond path traversal
## Current meta-datavault consequence

`meta-datavault` now consumes this split through a sanctioned preflight command:

- `meta-datavault check-business-materialization`

That command does not materialize BDV rows yet. It verifies that the current Business/BDV/Implementation/Weave/Fabric input set is coherent enough for future materialization.
