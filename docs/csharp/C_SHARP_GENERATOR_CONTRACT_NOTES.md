# C# Generator Contract Notes

This note records stable lessons from the generated C# tooling alignment work.
It avoids machine-local paths and keeps the focus on reproducible repository contracts.

## Workspace Arguments

The C# workspace surface is selected by `workspace.meta`; the workspace directory also contains the model and emitted C# sources.
Passing a metadata file path directly is invalid.

Correct shape:

```cmd
meta generate csharp --workspace MetaSchema\Workspaces\MetaSchema --out MetaSchema\Tooling\MetaSchema --tooling
```

## Output Shape

Generated tooling output must land under the model-specific tooling folder, for example:

```text
MetaSchema\Tooling\MetaSchema
```

Avoid flat output directly under `Tooling`; it can leave generated model files and generated project files out of sync.

Use the repository wrapper for regeneration:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\regenerate-tooling.ps1 -Project MetaSchema
```

The wrapper pins canonical output paths and can build the generated tooling against locally packed foundation packages.

## C# Integrity Surface

Generated public C# model APIs use object references as the in-memory integrity surface.
XML identity attributes are serializer transport, not normal authoring API.

Consumer code should assign and traverse relationship references:

```csharp
row.DataTypeSystem = dataTypeSystem;
var systemName = row.DataTypeSystem.Name;
```

Avoid authoring through relationship transport names such as `DataTypeSystemId`, `TableId`, `PrimaryKeyId`, or similar generated XML projection details.

## Verification Pattern

For a bounded generator/tooling change:

1. Regenerate through `scripts\regenerate-tooling.ps1`.
2. Build the affected generated tooling project.
3. Run the smallest relevant test project using the same local package source.
4. Expand to dependent projects only after the direct model contract is green.
