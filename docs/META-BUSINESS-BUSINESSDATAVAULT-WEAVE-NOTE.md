# MetaBusiness <-> MetaBusinessDataVault weave note

## Purpose

This note captures the minimal business-side contract that must exist before `MetaBusinessDataVault` can be considered anchored to `MetaBusiness`.

It is not itself the sanctioned weave workspace. It records the intended contract around the sanctioned weave instance.

## Minimal direct anchors

The following anchors are the minimum useful set:

- `BusinessHub` -> `BusinessObject`
- `BusinessHubKeyPart` -> `BusinessKeyPart`
- `BusinessLink` -> `BusinessRelationship`
- `BusinessLinkEnd` -> `BusinessRelationshipParticipant`

This is the core seam between business meaning and Business Data Vault structure.

## Why these anchors matter

### BusinessHub -> BusinessObject

A business hub should exist because there is a business object worth identifying and integrating.

Without this anchor, a hub is only a technical container with a name.

### BusinessHubKeyPart -> BusinessKeyPart

A business hub key should not be invented in Data Vault space.

The ordered hub key parts should line up with the ordered business-key parts defined in `MetaBusiness`.

### BusinessLink -> BusinessRelationship

A business link should exist because there is a business relationship worth recording and integrating.

Without this anchor, a link is just a technical association.

### BusinessLinkEnd -> BusinessRelationshipParticipant

A link end should line up with a relationship participant.

That is what tells us which business object participates in the relationship, in what ordinal position, and optionally under what role.

## What does not need direct first-pass anchoring

These structures inherit their first-pass meaning from their parent structures:

- `BusinessHubSatellite`
- `BusinessLinkSatellite`
- `BusinessPointInTime`
- `BusinessBridge`

They still need coherent design, but they do not need an immediate direct business-side anchor before the four core anchors above are stable.

## Current MetaWeave limitation

Current `MetaWeave` is property-binding based.

That is strong enough for direct property equivalence checks, but it is too thin for full parent-scoped consistency on child rows by itself.

Examples of what current `MetaWeave` can express reasonably:

- `BusinessHub.Name` -> `BusinessObject.Name`
- `BusinessLink.Name` -> `BusinessRelationship.Name`

Examples of what current `MetaWeave` does not express well on its own:

- a `BusinessHubKeyPart` must match a `BusinessKeyPart` within the correct parent key/hub context
- a `BusinessLinkEnd` must match a `BusinessRelationshipParticipant` within the correct parent relationship/link context

The second case is now handled by `MetaFabric`, not by ad hoc CLI logic in `meta-datavault`.

## Current sanctioned weave instances

The current sanctioned weave instances are:

- `Weaves/Weave-MetaBusiness-MetaBusinessDataVault`
- `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-HubObject-Commerce`
- `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-LinkRelationship-Commerce`
- `Weaves/Weave-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce`

The first-pass workspace `Weaves/Weave-MetaBusiness-MetaBusinessDataVault` carries only the flat anchors that `MetaWeave` can express honestly:

- `BusinessHub.Name` -> `BusinessObject.Name`
- `BusinessLink.Name` -> `BusinessRelationship.Name`

The commerce sample workspaces show the split more clearly:

- flat hub/object weave
- flat link/relationship weave
- flat child weave for link ends / relationship participants
- scoped fabric over the link-participant seam

## Current sanctioned fabric instance

The current sanctioned fabric instances are:

- `Fabrics/Fabric-Suggest-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce`
- `Fabrics/Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce`

This proves that the link-participant seam is now handled cleanly by foundation tooling:

- `meta-weave check` on the child weave fails because `RoleName` is ambiguous on its own
- `meta-fabric suggest` proposes the required parent scope
- `meta-fabric check` validates the scoped result successfully

## Remaining limitation

The hub-key-part seam still remains outside the current fabric capability.

Reason:

- `BusinessHubKeyPart` scopes through `BusinessHub`
- `BusinessKeyPart` scopes through `BusinessKey`

That is not a simple shared-parent seam, so current `MetaFabric` does not yet express it.

See also:

- `docs/BUSINESS-BDV-WEAVE-DECISION.md`
## Practical conclusion

For now, the right top-down move is:

1. make sure `MetaBusiness` can represent business objects, business keys, business relationships, and relationship participants
2. treat the four direct anchors above as the intended Business/BDV consistency seam
3. use `MetaWeave` for flat anchors
4. use `MetaFabric` for shared-parent child-row consistency
5. do not pretend the current foundation already covers the hub-key-part multi-hop seam
