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

Needs future review:
- `Unit of Work`
- `Reference` / `Reference Satellite`
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
