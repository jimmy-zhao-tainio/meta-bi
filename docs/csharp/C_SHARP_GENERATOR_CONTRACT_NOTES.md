# C# Workspace Surface Notes

This note records the current C# workspace contract. C# is a workspace
surface, not a separate product workflow built around a special output mode.

## Workspace Shape

A C# workspace is a directory containing the C# sources that describe one
model and its instance graph, together with its `workspace.meta` descriptor.
The descriptor identifies the C# surface. The sources are ordinary C# and may
be produced by a tool or maintained by an application; the workspace contract
does not depend on how they were authored.

## In-Memory Integrity

Object references are the natural C# representation of relationships. Identity
values are persistence and transport details used to reconstruct those
references. Consumers should work with the references exposed by the typed
model rather than treating relationship identity fields as the primary object
model.

The C# surface reads the sources into the shared semantic workspace state and
writes them back from that state. A write reads the published sources again and
rejects a semantic difference before completing.

## Review Boundary

Roslyn is an implementation authority used by the C# reader where C# syntax
must be understood. It is not a customer-facing feature of the Meta product.
The important contract is that the C# surface preserves the same modeled
structure as the XML and SQL surfaces.

When reviewing a C# workspace change, verify the semantic round trip through
the current C# reader and writer, then run the affected product integration
test. Use the current workspace creation and surface commands rather than
superseded C# output instructions.
