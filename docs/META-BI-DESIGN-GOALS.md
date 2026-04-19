# MetaBi design goals

## Purpose

`meta-bi` is not primarily a place to store metadata for its own sake.

Its main purpose is to act as a generative backend for bounded BI system families.

That means the repo should preserve authored truth strongly enough to generate sanctioned BI artifacts faithfully, rather than trying to become a generic metadata warehouse for every BI concern.

## Core goal

The central design goal is:

- choose a bounded backend family
- model only the sanctioned truth needed to generate it faithfully
- support that bounded slice deeply instead of supporting everything shallowly

Current examples of that stance:

- Data Vault is supported because it is a strong target technology with a bounded modeling surface we can take seriously
- bounded SQL `VIEW` body support is pursued because it is a strong transform realization surface that can be modeled and round-tripped faithfully

The point is not to support those technologies because they are fashionable.
The point is to support target families where `meta-bi` can realistically aim for full and structurally honest support.

## Modeling stance

`meta-bi` should model what must be authored and preserved in order to generate the chosen backend family correctly.

It should not absorb every concern that exists somewhere in BI.

In particular:

- invariant authored truth belongs in sanctioned models
- declared correctness policy belongs in sanctioned models when it changes generated meaning
- target-specific lowering and execution mechanics should stay in tooling or downstream layers unless they are themselves authored truth

This keeps the abstraction level honest.

## Scope discipline

The repo should reduce concern surface by limiting what it claims to support.

That means:

- bounded support is better than broad partial support
- explicit refusal is better than vague "maybe" support
- unsupported surface should fail clearly rather than being silently approximated
- new capability should arrive as a closed slice with explicit acceptance criteria

`meta-bi` should not promise universal BI generation.
It should choose deliberate backend families and go deep on them.

## Support decision rule

When deciding whether to support `X`, `Y`, or both:

- support a backend family directly only if we are willing to close over its real concern surface
- avoid supporting both when "both" mainly creates a large interaction-concern cross-product
- prefer a higher-level sanctioned model plus explicit lowering only when there is real invariant truth to preserve above the target families
- do not hide major backend differences inside a vague generic model

The test is not "can we represent it somehow?"
The test is "can we support it faithfully, explicitly, and at the right layer?"

## What success looks like

A supported `meta-bi` slice should have a clear acceptance contract:

- authored model -> generated artifact -> structurally faithful result

Examples:

- sanctioned model -> generated Data Vault artifact -> structurally faithful vault result
- sanctioned transform syntax model -> generated SQL -> semantically equivalent supported SQL result

If `meta-bi` cannot yet meet that bar for a target family, it should treat that family as unsupported or experimental rather than partially sanctioned.

## What MetaBi should not become

It should not become:

- a generic metadata dumping ground
- a vague universal BI abstraction
- a place where execution-specific concerns leak upward into every model
- a system that claims support for broad backend families without closure

The repo should stay disciplined: narrow claims, explicit boundaries, faithful generation.
