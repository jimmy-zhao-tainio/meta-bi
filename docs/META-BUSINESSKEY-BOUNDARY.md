# MetaBusinessKey boundary

## Purpose

`MetaBusinessKey` is the sanctioned model for explicit business-key intent.

It exists because business-key selection should not be inferred from `MetaSchema` naming patterns.

## Modeling stance

This model owns business-key definition only.

It does not own source-schema detail. Binding a business key to extracted tables and fields should happen through weave metadata against `MetaSchema`.

## Current minimal structure

- `BusinessKey`
- `BusinessKeyPart`

This is intentionally minimal. The first goal is to replace heuristic key selection with explicit sanctioned input.
