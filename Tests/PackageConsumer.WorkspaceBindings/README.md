# Standalone workspace-binding package consumer

This executable fixture has exactly one direct dependency, the prepacked
`MetaTransform.Binding` NuGet package. It has no project references and belongs
outside the ordinary Integration profile because it requires package feeds.

Run the owning verification script from the repository:

```powershell
.\scripts\verify-workspace-binding-packages.ps1 `
  -FoundationFeed C:\Packages\foundation `
  -BiFeed C:\Packages\bi `
  -Version 0.1.0-example.1 `
  -OutputDirectory C:\Evidence\workspace-bindings-new-run
```

The output directory must be new and outside a Git checkout. The script copies
this fixture there, restores to a sibling private package cache, builds the
consumer, runs it, publishes it, and runs the published DLL. Repository projects
are never built. Build and publish executions get separate evidence directories.
Supplied local feeds and nuget.org are the only configured package sources.

The fixture verifies transitive workspace delivery, fresh independent shipped
catalog instances, selected arbitrary catalog identities, exact nullable text,
Unicode, whitespace and length facets, XML/C# persistence and canonical object
references. It then invokes production schema-aware Binding using that selected
catalog and verifies exact type assessments and the persisted binding graph.

The executable requires one explicit, new evidence-directory argument. It does
not infer workspaces or evidence paths by walking parent directories. It uses
no runtime reflection or reflective projection. It executes no SQL against a
database; emitted SQL and binding metadata are the checked outcomes.
