# Link notes from Hultgren pages 95-110

Source material:
- `C:\Users\jimmy\Pictures\dv\bilaga 1.pdf` through `bilaga 9.pdf`
- OCR over rendered page images
- OCR is imperfect, so wording below keeps close only to text that was readable with high confidence

## Readable extracted points

### 1. A Link represents a natural business relationship
Repeated signal across the pages:
- the Link has no descriptive information of its own
- the Link does not have its own business key
- the Link consists of the sequence IDs of the concepts it relates
- the Link also carries the warehouse-managed Link sequence ID, load date/time stamp, and record source

Usable reading:
- the relationship being modeled is the natural business relationship
- the Link stores warehouse-controlled surrogate identifiers for the participating hubs, not the raw business keys themselves

### 2. Core Data Vault links are many-to-many in form
Readable points:
- Data Vault modeling uses many-to-many relationships in the core
- the specific business cardinality is not enforced in the core link structure the way an FK in 3NF would enforce `1:M` or `M:1`
- the designer should instead model the unique specific natural business relationship at the right grain

Usable reading:
- cardinality expectation is real business knowledge
- but it is not the same thing as the physical core link structure

### 3. Link grain matters
Readable points:
- a given Link should be designed around the grain of the specific unique natural business relationship being modeled
- examples distinguish:
  - customer belongs to class
  - customer is assigned to account rep
  - customer sale
  - sale line item
- the pages stress that apparently similar relationships are still different links when their business meaning/grain differs

Usable reading:
- a Link is not just "some hubs connected"
- it is a specific business relationship at a specific grain

### 4. A Link is not a business event or concept by itself
Readable points:
- a sale is not a relationship; it is a business concept with its own business key, context, and relationships
- therefore a sale should be a Hub, with Links drawn from it
- the Link itself only means something insofar as it is attached to hubs

Usable reading:
- do not collapse business objects/events into links
- links remain relationships, not concepts

### 5. Link attributes belong on a satellite, not on the Link itself
Readable points:
- line item quantity appears as a satellite hanging off the sale line item link
- the pages distinguish the relationship keys from contextual/descriptive attributes about that relationship

Usable reading:
- descriptive or contextual attributes of a relationship should be modeled through link satellites
- this supports the current split between `BusinessLink` and `BusinessLinkSatellite`

### 6. Recursive links have meaningful subtypes: SAL and HAL
Readable points:
- two common forms are the Same-As Link (`SAL`) and the Hierarchical Link (`HAL`)
- they are both recursive links and can look structurally identical from a pure table-shape perspective
- their meaning is very different
- SAL expresses that two hub records represent the same core concept
- HAL expresses that two hub records form a hierarchy
- for HAL, parent vs child must be named explicitly on the two FK roles
- naming is important because SAL and HAL otherwise look technically the same

Usable reading:
- there is real evidence for at least two meaningful recursive link variants
- role naming on link ends matters, especially for HAL

### 7. Link attributes identified explicitly in the text
Readable points:
- Link Sequence ID
- Load Date / Time Stamp
- Record Source

Usable reading:
- these are baseline technical columns for links in the Hultgren treatment
- they align with the current `MetaDataVaultImplementation` direction

### 8. Expected cardinality should be tracked in metadata
Readable point from the OCR around page 97:
- the business expectation for cardinality should be tracked in metadata even though the link itself stays many-to-many in form
- this can support feedback/exception reporting between expected and actual relationship behavior

Usable reading:
- there is a future metadata concept here, but it is not the same thing as the core Link table shape

## Reasoning for the current model

### What this supports strongly
1. `BusinessLink` should stay as its own first-class entity.
2. `BusinessLinkSatellite` is the correct home for relationship attributes.
3. Link ends need explicit role naming support.
4. `SAL` and `HAL` are real link families, not just labels on a generic link.
   These pages justify modeling them explicitly.
5. The core link shape should remain warehouse-managed and surrogate-key based.

### What this does not justify yet
1. It does not justify a broad uncontrolled set of link kinds.
   From these pages, the only clearly grounded non-standard kinds are:
   - SAL
   - HAL
2. It does not justify implementing advanced link semantics in SQL until we define their concrete consequences.
   For example:
   - HAL may require clearer parent/child constraints or naming conventions
   - SAL may require matching/identity semantics beyond plain structure
3. It does not provide support for `DrivingKey` in the way we previously had it in the model.

### Immediate modeling implication
The current baseline in `MetaBusinessDataVault` should be:
- keep `BusinessLink`
- keep `BusinessLinkHub`
- keep `BusinessLinkSatellite`
- keep `BusinessLink` for the baseline standard link
- add explicit `BusinessSameAsLink` and `BusinessHierarchicalLink` families
- keep `BusinessLinkHub` only for the baseline standard link

### Likely next step
Promote `SAL` and `HAL` beyond a kind property.
Specifically:
- keep standard `BusinessLink` as its own family
- model `BusinessSameAsLink` explicitly
- model `BusinessHierarchicalLink` explicitly
- keep expected relationship cardinality as a separate future concern
