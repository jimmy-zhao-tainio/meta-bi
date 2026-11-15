This standalone package consumer verifies the migrated owner contracts from a
fresh package feed. It is intentionally separate from the single-direct-package
`PackageConsumer.WorkspaceBindings` fixture and from ordinary test profiles.

Run after the product packages have been built and packed:

```powershell
.\scripts\verify-migrated-owner-packages.ps1 `
  -FoundationFeed C:\absolute\foundation-feed `
  -BiFeed C:\absolute\bi-feed `
  -Version 0.1.0-migration.1 `
  -OutputDirectory C:\absolute\new-external-directory
```

The script copies the fixture outside the checkout and uses a fresh private
NuGet cache. A test-only bridge package references 17 owner packages; the
executable references only that bridge, so direct content delivery cannot mask
missing transitive owner targets. The bridge is a test artifact, never a product
dependency. No repository projects are referenced or built.

Both build and publish executions load 21 actual packaged model workspaces,
call the public contract/default helpers, and use generated explicit bindings
for exact generic-to-typed-to-generic comparisons. MetaSql and its deploy manifest
use neutral generic contract loading from their lightweight CSharp package.
An authored Analytics graph persists through XML and C# with canonical object
references and exact Unicode, whitespace, NULL and empty values. Independent
loads and mutation isolation are checked. All evidence paths are explicit and
separate for build and publish. The consumer performs no live SQL/SSAS execution.
