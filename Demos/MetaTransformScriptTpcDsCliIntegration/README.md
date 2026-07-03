# MetaTransformScript TPC-DS CLI Integration

This demo imports a TPC-DS query slice (`q01`-`q99`) into a `MetaTransformScript` workspace using per-file `from sql-file` calls with explicit `--target`, then binds all scripts in one `meta-transform-binding bind` run.

Purpose of this sample is gap-finding, not only green-path demonstration. Parse/export failures are signal and should be used to drive fix slices.

Current status: full `q01`-`q99` import, one-shot workspace bind, SQL export
roundtrip, re-import of emitted SQL, and MetaSql projection diff succeed
offline.

See [CURRENT_GAPS.md](./CURRENT_GAPS.md) for the latest known failures from this demo run.

## Schema Snapshot

`SchemaWS` is checked in as a one-off schema snapshot for this corpus.
It includes:
- TPC-DS source tables used by the corpus.
- `tpcds.v_q01`..`tpcds.v_q99` target table contracts used by binding.

The target rows come from the TPC-DS view metadata workflow: deploy the query
files as views, extract those views with `meta-schema`, then change the extracted
view rows into table rows so transform binding validates writable targets. This
demo does not deploy or extract SQL Server objects during the run.

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

`build-tpc-ds-snapshot` imports, binds, exports, re-imports the emitted SQL,
converts both transform workspaces to MetaSql, and diffs the MetaSql
workspaces without a database connection.
