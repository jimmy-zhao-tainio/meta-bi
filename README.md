# meta-bi

`meta-bi` is the BI stack that sits on top of the generic `meta` foundation.

This repository currently contains BI-oriented sanctioned models, CLIs, and docs:

- `MetaSchema.*`
- `MetaType.*`
- `MetaTypeConversion.*`
- `MetaDataVault.*`

It also contains BI architecture notes in `docs/`.

## Current dependency boundary

This is an extracted BI repository, not yet a fully decoupled build.

The projects here still reference foundation projects from `isomorphic-metadata`, especially:

- `Meta.Core`
- `Meta.Adapters`

So this repository is the first source split, not the final dependency split.

## Intent

The long-term repo boundary is:

- `isomorphic-metadata`: generic foundation (`Meta.Core`, `meta`, `MetaWeave`, `meta-weave`, generic metamodels)
- `meta-bi`: sanctioned BI models and BI-specific CLIs/tooling

## Included solution files

- `MetaSchema.sln`
- `MetaType.sln`
- `MetaTypeConversion.sln`
- `MetaDataVault.sln`
