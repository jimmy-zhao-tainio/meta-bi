# MetaBusinessDataVault boundary

## Purpose

`MetaBusinessDataVault` should be the sanctioned Business Data Vault model in the BI stack.

It exists after raw integration and before downstream warehouse and analysis design.

At this layer, the stack should be able to represent:

- business-driven vault structures
- business-rule-driven relationshiping and mastering
- helper structures for performant temporal and relationship traversal

It should not be simplified down to only PIT and Bridge. Business Vault carries a broader family of structures.

## Design inspiration and sources

This boundary is informed by Data Vault material used as source material, not as a dependency to import blindly.

Verified references used in this draft:

- Microsoft Fabric Data Vault article (business-vault and same-as discussion): <https://techcommunity.microsoft.com/blog/analyticsonazure/implementing-data-vault-2-0-on-fabric-data-warehouse/4227078>
- Redgate, The Business Data Vault: <https://www.red-gate.com/blog/data-vault-series-the-business-data-vault/>
- Redgate, Data Vault 2.0 Modeling Basics: <https://www.red-gate.com/blog/data-vault-series-data-vault-2-0-modeling-basics>
- Varigence reference values for Data Vault structures: <https://docs.varigence.com/bimlflex/reference-documentation/metadata-static-values.html>
- dv2.org 2023 notes on effectivity / multi-active / record tracking satellites: <https://www.dv2.org/2023/>

## What Business Data Vault needs to represent

### 1. Business-driven core structures

The Business Vault is not limited to helper tables. It can carry business-driven versions of core Data Vault structures.

That includes:

- business-driven hubs
- business-driven links
- business-driven satellites

These are structures produced or reshaped by business logic rather than only mirrored from raw source integration.

### 2. Link variants used in Business Vault

The checked sources support link-level variants beyond the plain raw-link pattern, including same-as and hierarchy-oriented structures, and they show that Business Vault can contain business-logic-driven relationship structures.

The model therefore needs to represent link-level variants such as:

- standard business links
- same-as links
- hierarchical links
- non-historized links

`Exploration link` is still a plausible extension term in practice, but it is weaker in the verified citation set and should be treated as tentative until a better primary source is pinned down.

These variants still share a link-like shape, so the model keeps a common `BusinessLink` structure with an explicit `LinkKind`.

### 3. Satellite variants used in Business Vault

Business Vault also needs to represent satellite variants driven by business rules or helper semantics.

That includes at least:

- standard business satellites
- derived / calculated satellites
- status satellites
- effectivity satellites
- multi-active satellites

These still behave like satellites, but the subtype matters. The model therefore keeps explicit hub- and link-satellite structures with a `SatelliteKind`.

### 4. Point-in-time structures

PIT tables are part of the Business Vault helper surface and need first-class representation.

The model must be able to represent:

- a PIT structure
- which business hub it serves
- which hub satellites it includes
- which link satellites it includes when relevant

### 5. Bridge structures

Bridge tables are also part of the Business Vault helper surface and need first-class representation.

The model must be able to represent:

- a bridge structure
- the hubs it spans
- the links it traverses
- ordinal or path-position semantics where needed

## Modeling stance

The sanctioned model should represent the full Business Vault table family, but it does not need to explode every subtype into a separate top-level entity when the table shape is still structurally the same.

So the current modeling split is:

- explicit entities where the structure really differs
- explicit kind properties where the subtype is a variant of the same structure

That leads to:

- `BusinessHub`
- `BusinessHubKeyPart`
- `BusinessLink` + `LinkKind`
- `BusinessLinkEnd`
- `BusinessHubSatellite` + `SatelliteKind`
- `BusinessHubSatelliteAttribute`
- `BusinessLinkSatellite` + `SatelliteKind`
- `BusinessLinkSatelliteAttribute`
- `BusinessPointInTime`
- `BusinessPointInTimeHubSatellite`
- `BusinessPointInTimeLinkSatellite`
- `BusinessBridge`
- `BusinessBridgeHub`
- `BusinessBridgeLink`

## What MetaBusinessDataVault should not own

It should not own:

- source-system extraction detail
- raw-vault lineage semantics that belong in `MetaRawDataVault`
- downstream dimensional or semantic-model artifacts
- measure definitions
- runtime execution details

Those belong in raw, transform, warehouse, analysis, or operations layers.

## Relationship to other sanctioned models

### MetaRawDataVault -> MetaBusinessDataVault

Raw Vault provides the integrated historical substrate.

Business Vault introduces business logic, mastering, helper structures, and business-driven reshaping on top of that substrate.

### MetaBusiness -> MetaBusinessDataVault

`MetaBusiness` should anchor why Business Vault structures exist.

Business Vault should not be built only because raw source structure exists. It should be built because business processes, org scope, and business intent justify it.

### MetaBusinessDataVault -> downstream analytical models

Business Vault should later provide the stronger basis for:

- suggested analytical groupings
- warehouse-serving structures
- analysis/semantic model design

## Current sanctioned direction

- keep `meta-datavault` as the tool family
- keep `MetaRawDataVault` as the current raw sanctioned model
- add `MetaBusinessDataVault` as a separate sanctioned model beside it

That keeps the tool family unified while making the model intent explicit.
