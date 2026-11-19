# MetaTransformScript CLI Integration with a Corpus Derived from TPC-DS

This demo imports a query slice derived from TPC-DS (`q01`-`q99`) into a
`MetaTransformScript` workspace using one `from sql-files` call and an explicit
path/target manifest. It exercises syntax round-tripping and keeps a separate
schema-aware Binding against SQL Server-derived expected output contracts.

Purpose of this sample is gap-finding, not only green-path demonstration. Parse/export failures are signal and should be used to drive fix slices.

The MetaTransformScript corpus is derived from TPC-DS and is not comparable to
published TPC-DS results: the corpus is modified for parser and binding coverage
and does not comply with the TPC-DS Specification. See [SOURCE.md](SOURCE.md)
and the repository [third-party notices](../../THIRD-PARTY-NOTICES.md).

Import, SQL module export, manifest-based re-import and byte-for-byte canonical
SQL comparison exercise syntax fidelity. Fixture regeneration compiles all 99
views on SQL Server and extracts their actual output metadata. The strict Binding
tests expose remaining expression-typing and validation gaps.

See [CURRENT_GAPS.md](./CURRENT_GAPS.md) for the latest known failures from this demo run.

## Schema Snapshot

`SchemaWS` contains 24 source table contracts and 99 expected output table
contracts. `SourceSchema.sql` is the reproducible source DDL, authored from the
previous source contract with its length, precision/scale and nullability facets.
`SourceViews` contains the SQL Server-adapted query definitions.

`Rebuild-Schema.ps1` creates the disposable `MetaBiTpcDsFixture` database, creates
the source tables and all 99 views, and invokes `meta-schema extract sqlserver`.
It verifies all 1,040 columns against SQL Server metadata using the selected
MetaDataType catalog. Modeled `meta` commands replace each output's `View`
specialization with `Table`, preserving its fields and type facets. The expected
table fields receive SQL Server's reported nullability through `meta import csv`,
because extraction leaves view nullability unspecified.

The table specialization supplies Binding's writable-target contract. SQL Server
supplies the expected column types independently of Binding. Generation verifies
the complete workspace before publication, preserves the previous workspace in
its reported evidence directory, and drops its disposable database afterward.
An existing database with the generation name causes a failure before replacement.

Regenerate from the mesh directory with current CLIs on `PATH`:

```powershell
$env:META_TPCDS_FIXTURE_ADMIN_SQL = 'Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false'
meta-mesh run --operation regenerate-schema
```

The script also accepts `-DataTypeWorkspace`, `-OutputWorkspace`,
`-EvidenceDirectory` and `-ReplaceExisting` for explicit selections.

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

The operation requires successful Binding and reports any rejection as failure.
See `CURRENT_GAPS.md` for the strict per-query integration command and remaining
results. Fixture compilation verifies output metadata without running the corpus
over a populated benchmark dataset.

`compare-sql-projections` retains the additional full-graph MetaSql conversion
and comparison. It requires both transform workspaces from the syntax run. This
large comparison was interrupted after its first conversion used over fifteen
CPU minutes without completing; no successful result is claimed. It remains a
separate performance investigation, rather than a gate in the ordinary syntax tour.
