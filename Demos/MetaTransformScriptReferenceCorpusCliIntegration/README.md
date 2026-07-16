MetaTransformScript reference corpus CLI integration sample.

This demo imports a broader SQL view corpus into `MetaTransformScript`, emits the
views back to SQL, imports the emitted SQL, converts both transform workspaces to
`MetaSql`, and diffs the resulting `MetaSql` workspaces.

The workflow is modeled in:

```text
MetaTransformScriptReferenceCorpusCliIntegration.MetaMesh
```

Run commands from the mesh folder. `--workspace` is omitted because `meta-mesh`
defaults to the current directory:

```powershell
cd MetaTransformScriptReferenceCorpusCliIntegration.MetaMesh
meta-mesh show
meta-mesh run --operation cleanup
meta-mesh run --operation round-trip-reference-corpus
```

`round-trip-reference-corpus` intentionally records one transform-workspace diff:
re-importing emitted modules produces a fresh syntax representation with emitted-file
provenance. The operation records those expected differences, then requires the
converted `MetaSql` workspaces to diff cleanly.
