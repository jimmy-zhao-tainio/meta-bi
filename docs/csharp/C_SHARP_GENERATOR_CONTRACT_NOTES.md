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

## Typed Workspace Access

Meta owns source interpretation. Generic consumers use the resulting generic
model, records and operations. Domain code that needs typed objects uses explicit,
compiler-checked member access; it must not discover model structure through CLR
reflection.

Generate typed workspace projections with the foundation CLI:

```powershell
meta export csharp-bindings --workspace <model-workspace> --out <new-directory>
```

Keep the resulting `.bindings.cs` and `.access.cs` files in the consuming
project's `Bindings` directory, outside the authored workspace's source set.
They contain direct member access, not instance data or a replacement model.
Regenerate and compile them when the modeled contract changes. A stale binding
rejects a different model contract.

`ITypedWorkspaceBinding<TModel>.Read` projects selected workspace data into a
fresh graph with canonical object references. `Write` requires the actual generic
model contract. Existing workspaces retain their selected contract when saved;
creation paths load the packaged authored model rather than reconstructing its
schema from a CLR class. Typed algorithms continue to use natural object
references.

The BCL-only access file supplies collection and relationship visitors with
typed selectors and setters. Parser and composition algorithms use these
selectors to traverse incomplete graphs without CLR discovery. Typed CLIs pass
an explicit binding to their runtime; typed creation paths also supply an actual
model-contract factory.

The catalog and conversion workspace providers load the supplied workspace, or
their packaged workspace when no path is supplied. Each load has independent
mutable state. A compiled seed factory remains a convenience for constructing
starting data; it does not replace a selected workspace. BI consumers use the
explicit binding overloads. Generic converter stages pass the selected generic
workspace directly to Weave and retain its generic output; they do not project
it into typed objects merely to reconstruct the same generic data.

Packages that load authored model files own their delivery to build and publish
outputs, including when the package is a transitive dependency. Verify that
boundary with `scripts/verify-workspace-binding-packages.ps1`; its standalone
consumer references only the Binding package and exercises persisted XML and C#
workspaces from both output layouts.

## Review Boundary

Roslyn is an implementation authority used by the C# reader where C# syntax
must be understood. It is not a customer-facing feature of the Meta product.
The important contract is that the C# surface preserves the same modeled
structure as the XML and SQL surfaces.

When reviewing a C# workspace change, verify the semantic round trip through
the current C# reader and writer, then run the affected product integration
test. Use the current workspace creation and surface commands rather than
superseded C# output instructions.
