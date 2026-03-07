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

That is strong enough for direct property equivalence checks, but it is too thin for full parent-scoped consistency on child rows.

Examples of what current `MetaWeave` can express reasonably:

- `BusinessHub.Name` -> `BusinessObject.Name`
- `BusinessLink.Name` -> `BusinessRelationship.Name`

Examples of what current `MetaWeave` does not express well yet:

- a `BusinessHubKeyPart` must match a `BusinessKeyPart` within the correct parent key/hub context
- a `BusinessLinkEnd` must match a `BusinessRelationshipParticipant` within the correct parent relationship/link context

That means a full sanctioned Business/BDV weave will likely require either:

- richer weave semantics in `MetaWeave`
- or an explicit materialization/check step in `meta-datavault` that understands parent-scoped consistency

## Current sanctioned weave instance

The current sanctioned first-pass weave instance is:

- `MetaWeave.Workspaces/Weave-MetaBusiness-MetaBusinessDataVault`

It carries only the flat anchors that current `MetaWeave` can express honestly:

- `BusinessHub.Name` -> `BusinessObject.Name`
- `BusinessLink.Name` -> `BusinessRelationship.Name`

Deeper parent-scoped consistency remains outside the weave instance for now.

See also:

- `docs/BUSINESS-BDV-WEAVE-DECISION.md`
## Practical conclusion

For now, the right top-down move is:

1. make sure `MetaBusiness` can represent business objects, business keys, business relationships, and relationship participants
2. treat the four direct anchors above as the intended Business/BDV consistency seam
3. do not pretend the current `MetaWeave` model already covers the full child-row consistency story



