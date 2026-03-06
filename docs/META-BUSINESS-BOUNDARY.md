# MetaBusiness boundary

## Purpose

`MetaBusiness` should be the sanctioned business-semantic anchor for the BI stack.

It should answer:

- what the BI system is about
- which business processes matter
- which parts of the organization care
- what should be measured
- for whom the measures exist

It should not be a technical model, a source-schema model, or a product-artifact model.

## Design inspiration

This boundary is informed by established business-architecture and process-modeling material, without importing any external standard wholesale:

- The Open Group ArchiMate business layer and motivation concepts: organization, roles, business processes, business objects, and business-facing meaning
- APQC Process Classification Framework: hierarchical process decomposition
- OMG BPMN: process-oriented communication
- OMG Business Motivation Model: traceability from motivation and objectives to operational change

Useful references:

- ArchiMate 101: <https://archimate-community.pages.opengroup.org/workgroups/archimate-101/>
- APQC Process Frameworks: <https://www.apqc.org/process-frameworks>
- OMG BPMN FAQ: <https://www.omg.org/bpmn/Documents/FAQ-WG.htm>
- OMG BPM portal / BMM: <https://www.omg.org/bpm/>
- OMG BMM specification page: <https://www.omg.org/spec/BMM/1.0/>

These are inspiration sources, not sanctioned dependencies.

## What MetaBusiness should own

### 1. Business process structure

`MetaBusiness` should own a nested business process map.

That means:

- top-level process areas
- decomposed business processes
- lower-level subprocess or activity structure where needed
- parent/child process hierarchy
- business ownership of those processes

This is not an execution pipeline model. It is the business view of how work is performed.

### 2. Organization and responsibility

`MetaBusiness` should own the business-side structure of who performs work, who consumes results, and who is accountable.

That includes:

- organization units
- roles
- stakeholder or consumer groups
- ownership and accountability relationships

This is where the org chart belongs, along with the analytical audience for the BI system.

### 3. Business concepts and subject meaning

`MetaBusiness` should own the meaning of core business concepts.

That includes:

- business entities or concepts
- subject areas
- business-facing identities where they exist
- conceptual relationships between business concepts

This is the semantic layer above source schemas and above technical warehouse artifacts.

### 4. Business measures and outcomes

`MetaBusiness` should own what the organization wants to measure and why.

That includes:

- measures
- KPIs
- outcomes
- business-facing definitions
- intended consumers of those measures

This is the layer that should later drive parts of warehouse and analysis design.

### 5. Motivation and intent

`MetaBusiness` should own the business-side rationale for why the BI system exists and what it is meant to support.

That may include:

- goals
- objectives
- business priorities
- policy or governance context

This should be strong enough to connect business motivation to downstream operational and analytical design, without turning `MetaBusiness` into a generic enterprise-strategy dumping ground.

## What MetaBusiness should not own

`MetaBusiness` should not own technical realization concerns that belong elsewhere.

It should not own:

- source schemas, source fields, or extracted source structure
- source-system type identities
- type-conversion rules
- transform execution details
- raw-vault technical structures
- warehouse physical design
- semantic-product artifacts
- SSIS, SSDT, SSAS, Power BI, or cloud-product specifics
- runtime execution state, deployment state, or infrastructure

Those belong in other sanctioned models or in later state/operations layers.

## What MetaBusiness must not become

It must not become:

- a glossary-only model
- an org-chart-only model
- a BPMN clone
- a generic enterprise-architecture monolith
- a technical semantic-model surrogate

`MetaBusiness` is the business-intent model for the BI stack. It should carry enough meaning to drive downstream design, but it should stop before technical realization.

## Relationship to other sanctioned models

### MetaBusiness and MetaSchema

`MetaSchema` describes what source systems physically expose.

`MetaBusiness` describes what the business cares about.

The relationship between them should be explicit through weaving/binding, not by collapsing source structure into business meaning.

### MetaBusiness and MetaTransform

`MetaTransform` should describe how data is reshaped.

`MetaBusiness` should describe why that shaping exists and what business concepts or measures it supports.

### MetaBusiness and MetaDataVault

`MetaBusiness` should not be confused with raw vault.

But business semantics from `MetaBusiness` should be able to influence or drive later business-vault design where the stack introduces that distinction.

### MetaBusiness and MetaDataWarehouse

`MetaBusiness` should help drive warehouse-serving structures by providing the business meaning behind dimensions, facts, and analytical slices.

### MetaBusiness and MetaAnalysis

`MetaBusiness` should help drive analysis design by defining:

- what should be measured
- for whom
- in which business context

## Initial modeling stance

The first version of `MetaBusiness` should be conservative and strong:

- process hierarchy
- org hierarchy
- stakeholder / consumer structure
- business concepts
- measures / KPI intent
- ownership / accountability

That is enough to make it real without overcommitting early to every possible business-architecture concept.

## Why this matters

Without `MetaBusiness`, the stack remains mostly technical:

- source-first
- structure-first
- product-first

With `MetaBusiness`, the stack gains a semantic center:

- the business system says what matters
- the technical models say how it is realized
- the toolchain can then project business intent into downstream metadata and artifacts
