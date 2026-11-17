# MetaTransformScript CLI Integration with a Corpus Derived from TPC-DS

This demo imports a query slice derived from TPC-DS (`q01`-`q99`) into a
`MetaTransformScript` workspace using one `from sql-files` call and an explicit
path/target manifest. It exercises syntax round-tripping and keeps a separate
schema-aware Binding probe against the historical schema snapshot.

Purpose of this sample is gap-finding, not only green-path demonstration. Parse/export failures are signal and should be used to drive fix slices.

The MetaTransformScript corpus is derived from TPC-DS and is not comparable to
published TPC-DS results: the corpus is modified for parser and binding coverage
and does not comply with the TPC-DS Specification. See [SOURCE.md](SOURCE.md)
and the repository [third-party notices](../../THIRD-PARTY-NOTICES.md).

Import, semantic SQL module export, manifest-based re-import and byte-for-byte
canonical SQL comparison are the syntax checks. They do not establish that these
view definitions execute on SQL Server. Binding currently fails; see the gaps
below. This sample is not an execution baseline.

See [CURRENT_GAPS.md](./CURRENT_GAPS.md) for the latest known failures from this demo run.

## Schema Snapshot

`SchemaWS` is checked in as a one-off schema snapshot for this corpus.
It includes:
- source tables derived from TPC-DS and used by the corpus.
- `tpcds.v_q01`..`tpcds.v_q99` target table contracts used by binding.

The historical target rows are modeled as tables for write-contract validation.
Their provenance does not establish validity: current strict Binding finds type
mismatches, and the query files include definitions that SQL Server rejects.
The normal round-trip operation does not deploy or extract SQL Server objects.

## Run

The workflow is modeled in:

```text
MetaTransformScriptTpcDsCliIntegration.MetaMesh
```

Run commands from the mesh folder. `--workspace` is omitted because `meta-mesh`
defaults to the current directory:

```powershell
cd MetaTransformScriptTpcDsCliIntegration.MetaMesh
meta-mesh show
meta-mesh run --operation cleanup
meta-mesh run --operation build-tpc-ds-snapshot
```

`build-tpc-ds-snapshot` imports, exports and re-imports the emitted SQL, then
requires the re-exported canonical SQL to match byte for byte, without a database
connection. The separate transform-model diff records representation and source
file provenance changes.

After building the snapshot, run the separate diagnostic probe:

```powershell
meta-mesh run --operation bind-snapshot
```

Its Binding rejection is a failure, not an expected-success exit code. Repairing
the full corpus and its target contracts into an executable SQL Server workload
is outside this syntax example.

`compare-sql-projections` retains the additional full-graph MetaSql conversion
and comparison. It requires both transform workspaces from the syntax run. This
large comparison was interrupted after its first conversion used over fifteen
CPU minutes without completing; no successful result is claimed. It remains a
separate performance investigation, rather than a gate in the ordinary syntax tour.
