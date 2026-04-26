# MetaPipeline

## Purpose

`MetaPipeline` is the core data-moving model and CLI for `meta-bi`.

The model is `MetaPipeline`.
The CLI is `meta-pipeline`.

Stage 1 is not trying to define the full orchestration story.
It is trying to close the small kernel that must be true for one sanctioned pipeline unit to run honestly.
Internal preparation or compilation may exist, but visible UX should not be centered on `plan` / `run` verbs or on artifact generation.

## Pipeline unit

The stage 1 pipeline unit is centered on:

- one `TransformScript` instance
- one `TransformBinding` instance
- the `MetaSchema` workspace context that the binding depends on
- the runtime machinery needed to read, buffer, and write that combination honestly

The transform is the core.
The binding is the guarantee.

Every stage 1 run names the transform script explicitly.
If that binding exposes multiple targets, the run also names the target explicitly.
In the modeled CLI path, `meta-pipeline execute` selects the declared pipeline task that already names those details.

This is a narrower center than generic ETL or orchestration language.
Stage 1 is about making this unit real before widening the support claim.

## Stage 1 nucleus

The early nucleus is small:

- pipeline unit shape
- source read realization
- target write realization
- runtime state
- run / replay / failure honesty
- minimal validation
- minimal evidence / logging

This is the irreducible base layer even if later orchestration or richer semantics impose additional constraints downward.

## Stage 1 source grounding

Stage 1 is grounded on database sources already supported by `MetaSchema`.

That grounding is deliberate.
Files, APIs, object stores, and messages are not the stage 1 source family for this document.

## Connection references

Connection strings are not stored in sanctioned metadata or artifacts.

Sanctioned metadata may name connection references explicitly.
At runtime those reference names are resolved through shell-visible environment variables.
Users and devops systems are responsible for populating those variables before invoking `meta-pipeline`.

## Relationship to other docs

- `META-LOAD.md` is the broad concern inventory.
- `META-PIPELINE.md` is the compact grounding note for the first `MetaPipeline` slice.
- `META-PIPELINE-MODEL.md` is the overarching model sketch for task families and boundaries.
- `META-PIPELINE-WORKING-PLAN.md` is the near-term steering note for growing the executable core without absorbing orchestration policy.

If later stages widen into orchestration or richer semantics, they should build on this nucleus rather than replace it.
