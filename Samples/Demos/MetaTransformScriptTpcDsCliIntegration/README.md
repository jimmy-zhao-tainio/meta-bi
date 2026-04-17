# MetaTransformScript TPC-DS CLI Integration

This demo imports a TPC-DS query slice (`q01`-`q99`) into a `MetaTransformScript` workspace using per-file `from sql-file` calls with explicit `--target`.

Purpose of this sample is gap-finding, not only green-path demonstration. Parse/export failures are signal and should be used to drive fix slices.

See [CURRENT_GAPS.md](./CURRENT_GAPS.md) for the latest known failures from this demo run.

## Run

```cmd
run.cmd
```

## Cleanup

```cmd
cleanup.cmd
```
