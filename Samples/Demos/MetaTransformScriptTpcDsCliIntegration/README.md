# MetaTransformScript TPC-DS CLI Integration

This demo imports a TPC-DS query slice (`q01`-`q99`) into a `MetaTransformScript` workspace using per-file `from sql-file` calls with explicit `--target`, then binds all scripts in one `meta-transform-binding bind` run.

Purpose of this sample is gap-finding, not only green-path demonstration. Parse/export failures are signal and should be used to drive fix slices.

Current status: full `q01`-`q99` import, one-shot workspace bind, and SQL export roundtrip succeed.

See [CURRENT_GAPS.md](./CURRENT_GAPS.md) for the latest known failures from this demo run.

## Schema Snapshot

`SchemaWS` is checked in as a one-off snapshot and is treated as the persisted schema contract for this demo.
It includes:
- TPC-DS source tables used by the corpus.
- `tpcds.v_q01`..`tpcds.v_q99` target tables.

The demo does not re-extract schema on each run.

## Run

```cmd
run.cmd
```

## Cleanup

```cmd
cleanup.cmd
```
