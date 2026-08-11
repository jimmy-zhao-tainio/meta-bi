# MetaTransform boundary

## Status and purpose

`MetaTransform` is the settled, sanctioned, bounded transformation product area
in the BI stack. It currently contains two sanctioned models:

- `MetaTransformScript` represents supported SQL Server transform syntax.
- `MetaTransformBinding` records derived binding and validation evidence for a
  script against explicit source and target contracts.

The models are deliberately bounded. Unsupported SQL or deferred binding
coverage must be reported explicitly; bounded coverage does not imply
experimental product status.

## MetaTransformScript ownership

`MetaTransformScript` owns the authored structure of supported SQL Server
transform statements. Its core acceptance contract is:

```text
SQL -> MetaTransformScript -> semantically equivalent SQL
```

The supported surface includes the sanctioned view, query, expression,
function, and mutation shapes represented by its current model. Stored
procedures use explicit modeled contracts for supported operational
understanding rather than pretending to provide a general procedural SQL
model.

Syntax structure is product truth at this layer. Binding, inferred types,
lineage, target validation, optimization, and operational execution profiles
are derived concerns and must not be substituted for syntax understanding.

The detailed claimed surface lives in
[`VIEW_SURFACE_SUPPORT_TRACKER.md`](../../MetaTransform/Script/Reference/VIEW_SURFACE_SUPPORT_TRACKER.md).

## MetaTransformBinding ownership

`MetaTransformBinding` derives explicit semantic evidence from a
`MetaTransformScript` workspace and sanctioned supporting contracts. It owns:

- name and scope resolution
- derived rowsets and columns
- source and target references
- read and write evidence
- target-shape validation evidence
- exact or sanctioned type-conversion outcomes

Source and target schema truth comes from `MetaSchema`. Type compatibility comes
from `MetaDataType` and `MetaDataTypeConversion`; Binding does not redefine
those vocabularies.

The detailed claimed surface lives in
[`BINDING_SUPPORT_TRACKER.md`](../../MetaTransform/Binding/Reference/BINDING_SUPPORT_TRACKER.md).

## Ownership exclusions

The MetaTransform product area does not own:

- source-schema extraction
- Data Vault or dimensional-model semantics
- physical SQL deployment planning
- pipeline or orchestration runtime history
- external-system connectivity
- inferred optimization behavior as authored truth

Those outcomes belong to their respective sanctioned models and services.

## Review rule

The checked-in model workspaces, typed sources, current services, and tests are
authoritative for implemented structure. Support trackers describe the bounded
coverage. Historical plans and audits can explain how the product arrived here,
but they do not downgrade the sanctioned models to experimental status.
