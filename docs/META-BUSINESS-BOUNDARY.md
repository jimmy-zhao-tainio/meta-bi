# MetaBusiness boundary

## Purpose

`MetaBusiness` should be the sanctioned business-structure model for the BI stack.

At this stage it should answer:

- how the business is organized
- which business processes matter
- how those processes decompose
- which roles participate in them
- how organization units align to those processes

It should stay at business-map level. It should not yet drop into analytical design details such as measures or semantic-model structure.

## Design inspiration

This boundary is informed by established business-architecture and process-modeling material, without importing any external standard wholesale:

- The Open Group ArchiMate business layer: organization, roles, and business processes
- APQC Process Classification Framework: hierarchical process decomposition
- OMG BPMN: process-oriented communication

Useful references:

- ArchiMate 101: <https://archimate-community.pages.opengroup.org/workgroups/archimate-101/>
- APQC Process Frameworks: <https://www.apqc.org/process-frameworks>
- OMG BPMN FAQ: <https://www.omg.org/bpmn/Documents/FAQ-WG.htm>

These are inspiration sources, not sanctioned dependencies.

## What MetaBusiness should own

### 1. Business process map

`MetaBusiness` should own a nested business process map.

That means:

- business processes
- parent/child process hierarchy
- role participation in those processes

This is not an execution pipeline model. It is the business view of how work is performed.

### 2. Organization structure

`MetaBusiness` should own the business-side structure of who the organization is.

That includes:

- organization units
- organization hierarchy
- business roles
- which roles belong to which organization units

This is where the org chart belongs.

### 3. Organization-to-process alignment

`MetaBusiness` should explicitly model how organization units relate to business processes.

That includes relationships such as:

- ownership
- accountability
- execution responsibility
- support responsibility
- consumption scope

The first version can keep this simple as an explicit organization-unit to business-process relation with a small relationship-kind field.

This is important because later analytical models will need to infer which parts of the process map matter to which levels of the organization.

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
- measures, KPIs, dimensional structures, or semantic-model details
- SSIS, SSDT, SSAS, Power BI, or cloud-product specifics
- runtime execution state, deployment state, or infrastructure

Those belong in other sanctioned models or later layers.

## What MetaBusiness must not become

It must not become:

- a glossary-only model
- an org-chart-only model
- a BPMN clone
- a generic enterprise-architecture monolith
- a technical semantic-model surrogate

`MetaBusiness` is the business-map model for the BI stack. It should carry enough structure to orient the rest of the platform, but it should stop before technical and analytical realization.

## Relationship to other sanctioned models

### MetaBusiness and MetaSchema

`MetaSchema` describes what source systems physically expose.

`MetaBusiness` describes the business structure that the BI system is meant to support.

The relationship between them should be explicit through weaving/binding, not by collapsing source structure into business meaning.

### MetaBusiness and MetaTransform

`MetaTransform` should describe how data is reshaped.

`MetaBusiness` should describe the business process and organizational context that later motivates that shaping.

### MetaBusiness and downstream analytical models

Later models such as warehouse and analysis should be able to consume `MetaBusiness`, but `MetaBusiness` should not try to pre-own their detailed semantics now.

What it should preserve is the structure that later analytical models can consume:

- process hierarchy
- organization hierarchy
- role participation
- organization-to-process scope

That is enough groundwork for later inference of analytical groupings without pretending to define measures too early.

## Initial modeling stance

The first version of `MetaBusiness` should stay minimal and strong:

- business process hierarchy
- organization hierarchy
- business roles
- process participation by role
- organization-unit to business-process alignment

That is enough to make it real without overcommitting early.

## Why this matters

Without `MetaBusiness`, the stack remains mostly technical.

With `MetaBusiness`, the stack gets a business-side map:

- what the organization does
- who does it
- how work decomposes
- which process areas each part of the organization is tied to

That structure can later guide analytical organization, aggregation scope, and model generation without forcing analytical details into the business model too early.
