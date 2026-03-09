# Varigence Data Vault Sections

This note enumerates the Data Vault sections exposed by Varigence BimlFlex documentation and captures the terminology that matters for `meta-bi`.

Primary overview:
- https://docs.varigence.com/bimlflex/delivering-solutions/delivering-data-vault/

## Top-level Data Vault sections

Under `Delivering Solutions -> Data Vault`, Varigence currently exposes:

1. `Overview`
2. `Introduction to Data Vault`
3. `Hubs`
4. `Links`
5. `Satellites`
6. `Point-In-Time`
7. `Bridge`
8. `Reference Data`
9. `Integration Keys`
10. `Data Vault`
11. `Business Data Vault`
12. `Dimensional Model Delivery`
13. `Data Vault Best Practices`
14. `Hashing`
15. `Driving Keys in Data Vault`

Source:
- https://docs.varigence.com/bimlflex/delivering-solutions/delivering-data-vault/

## Object-type vocabulary confirmed by Varigence

From the static values reference, Varigence treats the following as first-class Data Vault object types:

- `Hub` (`HUB`)
- `Link` (`LNK`)
- `Satellite` (`SAT`)
- `Link Satellite` (`LSAT`)
- `Point In Time` (`PIT`)
- `Bridge` (`BRG`)
- `Reference` (`REF`)
- `Reference Satellite` (`RSAT`)
- `Same As Link` (`SAL`)
- `Hierarchy Link` (`HAL`)

Source:
- https://docs.varigence.com/bimlflex/reference-documentation/metadata-static-values

## Terminology we should align to

### Link
Varigence describes Links as the natural business relationship.

Source:
- https://docs.varigence.com/bimlflex/delivering-solutions/delivering-data-vault/data-vault-concept-link?tabs=settings-link-select
- https://docs.varigence.com/bimlflex/delivering-solutions/delivering-data-vault/

Implication for `meta-bi`:
- weaving `BusinessLink` to `BusinessRelationship` is correct
- `BusinessLink` should remain a first-class entity

### Same As Link (SAL)
Varigence treats SAL as a first-class object type, not just a subtype label.

Source:
- https://docs.varigence.com/bimlflex/reference-documentation/metadata-static-values

Implication for `meta-bi`:
- `BusinessSameAsLink` should be explicit
- SQL generation should treat SAL separately from standard links

### Hierarchy Link (HAL)
Varigence treats HAL as a first-class object type for recursive or parent-child relationships within a single hub.

Source:
- https://docs.varigence.com/bimlflex/reference-documentation/metadata-static-values

Implication for `meta-bi`:
- `BusinessHierarchicalLink` should be explicit
- SQL generation should treat HAL separately from standard links

### Link Satellite
Varigence treats Link Satellite as a first-class object type.

Source:
- https://docs.varigence.com/bimlflex/reference-documentation/metadata-static-values

Implication for `meta-bi`:
- relationship attributes belong in link satellite families, not in the link row itself

### Unit of Work
Varigence uses `Unit of Work` in the context of links and integration keys.
Relevant snippets from the documentation/search index include:
- `Add a link source object and map it to the unit of work`
- `It is also used to define a Unit Of Work for Links`
- `split all Links into Two Way Links or combine all Non Nullable references into a single Link or Unit of Work`

Sources:
- https://docs.varigence.com/bimlflex/technologies/technology-ssis/ssis-on-prem-sql-server/
- https://docs.varigence.com/bimlflex/reference-documentation/entities/Column
- https://docs.varigence.com/bimlflex/delivering-solutions/delivering-data-vault/data-vault-concept-link?tabs=settings-link-select

Implication for `meta-bi`:
- we should not invent competing terms if we mean the same thing
- we do not have to implement `Unit of Work` yet, but the term belongs in the review vocabulary
- if we later model source-side link grouping or link derivation, `Unit of Work` is likely the correct term to use

### Driving Key
Varigence documents driving keys as a Data Vault concern, but they are tied to link-satellite historization behavior rather than the core link row itself.

Sources:
- https://docs.varigence.com/bimlflex/delivering-solutions/delivering-data-vault/driving-keys-in-data-vault
- https://docs.varigence.com/bimlflex/reference-documentation/metadata-static-values

Implication for `meta-bi`:
- removing `IsDrivingKey` from the current baseline was correct
- bring it back only when the surrounding historization/effectivity behavior is modeled explicitly

## Current alignment checklist for `meta-bi`

Aligned:
- `BusinessLink` as the natural business relationship
- `AuditId` is now treated as part of the house baseline implementation concern for generated DV tables
- `BusinessSameAsLink` as explicit SAL
- `BusinessHierarchicalLink` as explicit HAL
- separate SQL handling for standard/SAL/HAL link families
- relationship attributes in satellite families, not core link rows
- `BusinessReference` and `BusinessReferenceSatellite` as explicit baseline Data Vault families

Needs future review:
- `Unit of Work`
- `Driving Key` and effectivity semantics
- best-practice sections around hashing and integration keys
- exact BDV workflow guidance from `Business Data Vault` and `Dimensional Model Delivery`

## Recommended review order

1. `Links`
2. `Satellites`
3. `Point-In-Time`
4. `Bridge`
5. `Integration Keys`
6. `Driving Keys in Data Vault`
7. `Business Data Vault`
8. `Dimensional Model Delivery`

That order follows the same pressure points we are already hitting in the sanctioned models.


## Focus review: Satellites

Primary source:
- https://docs.varigence.com/bimlflex/delivering-solutions/delivering-data-vault/data-vault-concept-satellite

### What Varigence states

1. Satellites are the `Context` entities that carry descriptive information for business concepts and business relationships.
2. A Satellite is attached to exactly one parent, either a `Hub` or a `Link`.
3. Multiple Satellites may hang off the same parent, split by source, subject area, or rate of change.
4. Satellite split/merge is allowed as long as history and audit trail are not lost.
5. A Satellite may carry a sub-sequence / ordering identifier as part of uniqueness.
6. Link Satellites are the standard place for relationship effectiveness and relationship attributes.
7. Multi-active Satellites are exceptional and should be used carefully; when possible, the default patterns are preferred.
8. Business Satellites may contain system-generated or aggregated attributes.
9. Link effectivity satellites are specifically tied to driving-key semantics.
10. Satellite system columns are a first-class concern in implementation settings, including:
   - Load Date Time Stamp
   - Record Source
   - Audit Id

### Immediate alignment check against `meta-bi`

Aligned:
- `BusinessHubSatellite` and `BusinessLinkSatellite` are explicit first-class entities.
- Satellites are attached to a single parent only.
- Relationship attributes are modeled in link-satellite families, not in the link row.
- `SatelliteKind` exists and currently supports `standard` and `multi-active`.
- `multi-active` is treated cautiously and requires explicit key-part rows.
- Satellite technical system columns are handled through `MetaDataVaultImplementation` rather than hidden generator defaults.

Partially aligned:
- We model business-derived attributes and key parts explicitly, which is good.
- We model arbitrary PIT stamps, but SQL support for those stamps is still a baseline area.
- We removed `IsDrivingKey` from the baseline, which is correct for now, but that means effectivity/driving-key satellite behavior is intentionally deferred.

Still weak / needs review:
- We do not yet model explicit satellite split/merge intent.
- We do not yet distinguish a dedicated `Business Satellite` concept from ordinary business-vault satellites; we only have satellite families plus typed attributes.
- We have not yet decided whether row-order/sub-sequence should be modeled explicitly for multi-active satellites.

### Likely modeling implications

1. Keep hub and link satellites as explicit first-class entities.
2. Keep the rule that a satellite has exactly one parent.
3. Keep `SatelliteKind`, but only where the SQL/materialization behavior really changes.
4. Add explicit support for row-order or sub-sequence if and when multi-active semantics need it.
5. Treat driving-key/effectivity behavior as a later, explicit extension rather than a hidden boolean.
6. Keep `AuditId` in the house baseline implementation workspace.

### Recommended next satellite-specific checks

1. Review the Varigence `Driving Keys in Data Vault` page specifically through the lens of link satellites and effectivity.
2. Review whether our `multi-active` baseline needs an explicit row-order / set-order concept.
3. Review whether `AuditId` should remain universal across all DV table families.


## Focus review: Integration Keys

Primary source:
- https://docs.varigence.com/bimlflex/concepts/integration-keys

### What Varigence states

1. Integration Keys are the source-side concept used to define business entity identity for Data Vault acceleration.
2. BimlFlex uses Integration Keys to derive Hubs, Links, and Satellites from source metadata and relationships.
3. An Integration Key is modeled as an additional Column in the source object.
4. An Integration Key can be composite, but BimlFlex normalizes it into a single string representation.
5. Integration Key definition can include Record Source to avoid cross-system key collisions.
6. Relationship acceleration in Data Vault is based on references between Integration Keys.
7. By default, the Integration Key is hashed into the surrogate key used in Hubs, and participates in Link and Satellite keys.
8. Varigence is explicit that defining correct Integration Keys requires business analysis, source-system knowledge, and profiling.

### Implications for `meta-bi`

1. The unresolved source-side identity problem in our stack is real.
2. `MetaSchema` on its own is not enough to derive a serious RDV/BDV, because it does not currently model source-side business identity in a Varigence-style way.
3. The current idea of hanging business identity off `MetaBusiness` is valid, but if we want a source-to-DV acceleration story comparable to Varigence we still need an explicit source-side identity/binding concept.
4. This strengthens the earlier conclusion that source derivation and explicit DV modeling are separate fronts.
5. It also suggests that `Unit of Work` and Integration Key semantics may eventually belong in the source/business seam, not inside the DV model itself.

### Current alignment

Aligned:
- we do not pretend `MetaSchema` alone is sufficient for business identity derivation
- we already separate explicit BDV modeling from upstream derivation/materialization

Not aligned yet:
- there is no sanctioned source-side identity construct equivalent in role to Integration Key
- there is no explicit source/business identity binding model yet

## Focus review: Driving Keys in Data Vault

Primary source:
- https://docs.varigence.com/bimlflex/delivering-solutions/delivering-data-vault/driving-keys-in-data-vault

### What Varigence states

1. Driving Key is about enforcing relationship behavior when a Link represents a many-to-one source relationship.
2. The driving key is the consistent part of the relationship key.
3. The actual enforcement happens in the corresponding Link Satellite, not in the core Link table.
4. End-dating/effectivity behavior is part of the mechanism.
5. BimlFlex can infer driving-key scenarios automatically for Links derived out of a Hub / source FK pattern.

### Implications for `meta-bi`

1. Removing `IsDrivingKey` from the baseline was the correct move.
2. If we ever reintroduce driving-key behavior, it should come back together with:
   - link-satellite effectivity semantics
   - end-dating semantics
   - explicit loading behavior
3. Driving Key is not a simple boolean on the link row.
4. It is currently out of scope for the broad baseline, and that is fine.

### Current alignment

Aligned:
- `IsDrivingKey` is gone from the baseline
- we are not pretending to support effectivity/driving-key SQL semantics yet

## Focus review: Business Data Vault

Primary sources used:
- https://docs.varigence.com/bimlflex/technologies/technology-ssis/ssis-on-prem-sql-server/
- https://docs.varigence.com/bimlflex/getting-started/first-project-walkthrough/
- https://docs.varigence.com/bimlflex/delivering-solutions/delivering-data-vault/data-vault-dimensional-model

### What Varigence states

1. BDV is a set of materialized constructs focused on performance improvement and easier querying.
2. In BimlFlex, the two central BDV artifacts emphasized are PIT and Bridge.
3. PIT is used to simplify timeline-sensitive querying and equi-joins across surrounding satellites.
4. Bridge is used to flatten related business entities and reduce join complexity.
5. For bridge usage, BimlFlex starts from a single business concept / central hub and expects an N-1 or many-to-one flow from that central business concept.
6. Dimensional delivery is defined on top of the combined Raw + Business DV model, often through proxy/source objects.

### Implications for `meta-bi`

1. Our current `MetaBusinessDataVault` focus on PIT and Bridge is directionally right.
2. Our explicit `AnchorHub` on `BusinessBridge` aligns with the idea of a central business concept.
3. Our bridge model still needs to be reviewed against the N-1 / many-to-one expectation more explicitly.
4. The fact that BDV is described as materialized helper constructs supports keeping `meta-datavault` responsible for both:
   - native DV workspace management
   - BDV materialization from sanctioned inputs
5. It also supports the earlier choice not to jump straight into product artifact generators as if that were the same concern as BDV semantics.

### Current alignment

Aligned:
- PIT and Bridge are explicit first-class BDV constructs in the sanctioned model
- bridge has explicit `AnchorHub`
- SQL generation for PIT/Bridge exists

Needs more review:
- whether our bridge path semantics fully reflect the N-1 / many-to-one discipline
- whether PIT should eventually expose richer timeline semantics in SQL, not only in metadata

## Suggested next review order after this

1. `Reference Data`
2. `Hashing`
3. `Data Vault Best Practices`

Reason:
- `Reference Data` may expose another under-modeled sanctioned area
- `Hashing` will affect the SQL-generation contract directly
- `Best Practices` will likely expose any remaining baseline gaps in naming, loading, or helper structure behavior


## Current decision

- `Integration Key` is treated as a Varigence/BimlFlex-specific pragmatic feature, not a standard term to copy into `meta-bi`.
- `Driving Key` is deferred until effectivity/end-dating semantics are modeled explicitly.
- `Reference` and `Reference Satellite` are a standard Data Vault area and are now implemented in the explicit `MetaBusinessDataVault` baseline.
