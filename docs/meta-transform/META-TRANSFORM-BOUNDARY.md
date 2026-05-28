# MetaTransform boundary

## Purpose

`MetaTransform` is the sanctioned neutral transform model for the BI stack.

It should describe how tabular data is reshaped from a source surface into a target surface without learning source-domain or target-domain semantics.

## What MetaTransform should own

`MetaTransform` should own:

- the authored transform contract
- the source-side tabular surface
- the target-side tabular surface
- explicit table mappings
- explicit field mappings
- structured row-shape expressions
- structured value-shape expressions
- target feed intent
- type intent via sanctioned `MetaDataTypeId`
- requested coercion via sanctioned `MetaDataTypeConversionId`

## What MetaTransform should not own

`MetaTransform` should not own:

- source-schema extraction concerns
- raw-vault or business-vault semantics
- SQL physical design
- deployment plans
- CLI prose
- runtime execution history
- external-system connectivity

Those belong in other sanctioned models or in glue code.

## Core structure

The first sanctioned shape is intentionally neutral:

- `Transform` is the authored contract root
- `Source` and `Target` are named tabular surfaces under that contract
- `SourceTable` and `SourceField` describe the source surface
- `TargetTable` and `TargetField` describe the target surface
- datatype facets stay on `SourceFieldDataTypeDetail` and `TargetFieldDataTypeDetail`
- `TableMapping` owns row-shape
- `FieldMapping` owns value-shape
- `TableExpressionNode` forms the row-shape tree
- `ValueExpressionNode` forms the value-shape tree

This keeps the model cleaner than free-form SQL while staying separate from source-model and target-model ownership.

## Sanctioned type references

`MetaTransform` must not redefine type vocabularies already sanctioned elsewhere.

- `SourceField.MetaDataTypeId`
- `TargetField.MetaDataTypeId`
- `ValueExpressionNode.ResultMetaDataTypeId`

should point at sanctioned `MetaDataType` ids.

`ValueExpressionNode.MetaDataTypeConversionId` should point at a sanctioned `MetaDataTypeConversion` id when a transform explicitly requests a coercion.

## Initial vocabulary

Until tooling hardens these further, the initial intended vocabularies are:

- `TargetField.FeedKind`: `SourceMapped`, `DerivedMapped`, `Defaulted`, `SystemGenerated`, `Ignored`
- `TableExpressionNode.ExpressionKind`: `SourceTable`, `Join`, `Filter`, `Distinct`, `Union`, `Intersect`, `Except`
- `TableExpressionNode.JoinKind`: `Inner`, `Left`, `Right`, `Full`
- `ValueExpressionNode.ExpressionKind`: `SourceField`, `Literal`, `Function`, `Cast`, `Comparison`, `Boolean`, `Case`

These values are conventions for now, but they should stay small and deliberate.

## Initial invariants

The model is meant to support these invariants:

- each `SourceTable` name is unique within its `Source`
- each `TargetTable` name is unique within its `Target`
- each field name is unique within its owning table
- every `TableMapping` targets exactly one `TargetTable`
- every `FieldMapping` targets exactly one `TargetField`
- every target field with `FeedKind` of `SourceMapped`, `DerivedMapped`, or `Defaulted` should be satisfied exactly once
- `SystemGenerated` and `Ignored` target fields should not require source coverage
- every referenced `SourceField` must be reachable from the `TableMapping` row-shape

## Relationship to other sanctioned models

`MetaTransform` should stay neutral and let other sanctioned models keep their own meaning:

- `MetaSchema` describes extracted source structure
- `MetaRawDataVault` and `MetaBusinessDataVault` describe logical vault targets
- `MetaSql` describes physical SQL realization

Glue code and weave can bind those worlds to a `MetaTransform` workspace without forcing `MetaTransform` to understand them directly.
