# MetaLoad design questions

## Purpose

This document is a companion to `META-LOAD.md`.

`META-LOAD.md` is the concern inventory.
This document is the design-question frame.

Its job is to force explicit answers about what `MetaLoad` is, what it generates, what it supports, how it is operated, and where its boundaries are.

The point is not to answer every question immediately.
The point is to make progress by answering them deliberately.

## Relationship to the concern inventory

Use the two documents differently:

- `META-LOAD.md`: what concerns can exist in ETL loading systems
- `META-LOAD-DESIGN-QUESTIONS.md`: what `MetaLoad` chooses to support, guarantee, and refuse

The concern inventory is broader than the sanctioned `MetaLoad` scope should be.

## How to use this document

Each question should eventually receive an explicit answer such as:

- supported
- supported with bounds
- generated indirectly through tooling
- delegated to external runtime/platform behavior
- experimental
- intentionally unsupported

Unsupported is an acceptable answer.
Vague support is not.

## 1. Realization model

Assume `MetaLoad` already generates fully working load pipelines.

Answer first:

- What concrete artifact does `MetaLoad` generate?
- What runtime or technology executes that artifact?
- What parts are generated ahead of time and what parts are decided at runtime?
- What target systems does `MetaLoad` load into directly?
- What source systems does `MetaLoad` read from directly?
- Is `MetaLoad` batch-only, micro-batch-capable, streaming-capable, or some bounded subset?
- Where does load state live?
- Where do watermarks, batch identity, replay state, and error state live?
- What is the smallest complete generated load unit?
- What is the smallest end-to-end generated pipeline that counts as real support?
- Does `MetaLoad` generate only data-shaping logic, or also orchestration logic?
- Does `MetaLoad` generate only target-write logic, or also source-read logic?
- Does `MetaLoad` assume an external scheduler/orchestrator?
- Does `MetaLoad` assume an external deployment engine?
- Which runtime concerns are owned directly by `MetaLoad` and which are intentionally delegated?

This section should force a concrete operating picture before detailed capability claims expand.

## 2. Supported load semantics

Once the realization model is concrete, answer what loading semantics `MetaLoad` actually supports.

### 2.1 Source acquisition

- Which source families are first-class: database, file, API, message, object-store?
- Which source technologies are in scope first?
- Does `MetaLoad` support full extract, incremental extract, or both?
- If incremental is supported, which incremental styles are in scope first?
- Does `MetaLoad` require trusted source metadata?
- Does `MetaLoad` support multi-source loads in one generated pipeline?
- Does `MetaLoad` support source partition discovery, late arrival, and missing partition handling?
- What source consistency model is assumed?

### 2.2 Change detection

- Does `MetaLoad` support full comparison, CDC, watermark logic, or some bounded subset?
- What kinds of source change markers are first-class?
- What matching keys are supported for change detection?
- What counts as a meaningful change?
- Are deletes supported, and if so how are they represented?
- Are backdated corrections supported?
- Are out-of-order changes supported?
- What ambiguity causes hard failure vs tolerated approximation?

### 2.3 Target write semantics

- What target write patterns are first-class: insert-only, append-only, upsert, replace, history maintenance?
- Does `MetaLoad` support staging plus publish, or direct target write, or both?
- Are target writes atomic at row, batch, table, or pipeline scope?
- What target-side identity generation patterns are supported?
- Does `MetaLoad` support partition-aware loading?
- Does `MetaLoad` support target-side delete behavior, and what kinds?
- What write conflicts cause failure?

### 2.4 Temporal and stateful loading

- Does `MetaLoad` support purely current-state loading, historical loading, or both?
- Which temporal models are first-class?
- Does `MetaLoad` support late-arriving facts, late-arriving dimensions, or both?
- Does `MetaLoad` support restatement of historical facts?
- What point-in-time correctness guarantees are intended?
- What historical repair flows are supported?

### 2.5 Reference resolution and keying

- Does `MetaLoad` support dimension lookup directly?
- Which lookup modes are in scope: current-row, effective-dated, point-in-time?
- Are inferred members supported?
- Are unknown-member and error-member fallbacks supported?
- Are unresolved foreign keys allowed temporarily?
- Is late key repair supported?
- Are re-key operations on facts supported after correction or inferred-member replacement?
- What lookup ambiguity causes failure?

### 2.6 Business-semantic loading

- What business structures are first-class in generated loading behavior?
- Does `MetaLoad` support conformed dimensions directly?
- Does `MetaLoad` support fact grain enforcement directly?
- Does `MetaLoad` support bridge handling, hierarchy handling, and relationship history directly?
- Which business-semantic concerns belong in `MetaLoad` itself and which belong in upstream sanctioned models?

This section should end in explicit capability families rather than a generic "loads data" claim.

## 3. Correctness and guarantees

For each supported load semantic, answer what `MetaLoad` guarantees.

- What does "supported" mean operationally?
- What determinism is expected across reruns?
- What idempotency is expected across reruns?
- What replay and restart behavior is part of the contract?
- What validation happens before execution?
- What validation happens during execution?
- What validation happens after execution?
- What input ambiguity causes hard refusal?
- What unsupported surface causes hard refusal?
- What approximations, if any, are ever allowed?
- What artifacts must be reproducible from the same authored inputs?
- What evidence is available to prove the generated result matches supported intent?

This section should define the quality bar for support.

## 4. Management and operation

Assume supported generated pipelines already exist.
How are they managed as real systems?

### 4.1 Orchestration and dependency

- Does `MetaLoad` own orchestration directly, generate orchestration artifacts, or rely on an external orchestrator?
- How are dependencies declared?
- How are retries, timeouts, and ordering controlled?
- Does `MetaLoad` support one-off loads only, recurring loads, or both?
- What backfill and catch-up flows are supported operationally?

### 4.2 Logging, observability, and audit

- What run-level logging is required?
- What step-level logging is required?
- What row-count and watermark evidence is required?
- What generated-fingerprint or version evidence is required?
- What audit trail is required for reruns, replays, and manual intervention?

### 4.3 Deployment and environment

- How are generated pipelines deployed?
- What environment differences are first-class?
- What environment differences are intentionally unsupported?
- What runtime prerequisites must exist outside `MetaLoad`?
- How are configuration and secrets handled?

### 4.4 Security and governance

- What credential boundaries are assumed?
- What operator actions require explicit approval?
- What governance expectations are in scope for generated loads?
- What sensitive data handling is required by default?

### 4.5 Performance and scale

- What scale envelope is the first supported target?
- What performance behavior is part of the support claim vs best effort only?
- Which performance strategies are explicit model/runtime choices and which are delegated to the platform?

This section should make `MetaLoad` operable, not just generative in theory.

## 5. Variation and extensibility

Once a bounded core exists, answer where controlled variation is allowed.

- Which behaviors are fixed by the sanctioned model?
- Which behaviors are policy choices with explicit vocabulary?
- Which behaviors can vary by backend family?
- Which behaviors can vary by generated target technology?
- What extension points are allowed?
- What extension points are forbidden?
- How is custom logic governed?
- How much custom logic can exist before the generated system is no longer meaningfully "MetaLoad-supported"?

This section should prevent uncontrolled escape hatches.

## 6. Capability boundaries

Support should be classified explicitly.

For each major capability family, decide whether it is:

- sanctioned and supported
- supported with explicit bounds
- generated indirectly rather than owned directly
- experimental
- out of scope

Also answer:

- What kinds of generated loading systems does `MetaLoad` explicitly aim to produce?
- What kinds of loading systems does it intentionally not aim to produce?
- What target families are good ideas for `MetaLoad` because they are valuable and realistically closeable?
- What target families should stay outside the sanctioned scope even if they are attractive in theory?

This section is where ambition is forced to meet closure.

## 7. Bottom-up closure order

If development starts bottom up, answer in what order closure should happen.

Suggested sequence:

1. choose one concrete realization model
2. choose one smallest real supported load pipeline
3. close one bounded source acquisition slice
4. close one bounded target write slice
5. close deterministic correctness for that slice
6. add stateful and historical behavior only after the base slice is trustworthy
7. add operational management depth
8. add controlled extensibility last

This sequence should be adjusted explicitly rather than drifting implicitly.

## 8. Immediate next questions

If work were to start soon, the first questions to answer should probably be:

- What concrete artifact does `MetaLoad` generate first?
- What runtime executes it first?
- What is the first complete supported load pattern?
- What is explicitly out of scope for that first slice?
- What evidence will prove the first slice is truly supported?

Those answers should narrow the future concern surface before implementation begins.
