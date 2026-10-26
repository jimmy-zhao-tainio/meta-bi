# Custom conversion binding probe

This diagnostic reproduces a missing integration contract. It is not a working
conversion workflow and does not change production behavior.

A user supplies a new type, `weird:type:ReversedDigits`. A precompiled TSV adapter
reads its `WackyValue` column as `wx321`. Its source MetaSchema declares that
custom type; the destination field is SQL Server `int`. A private copy of the
conversion workspace selects a new implementation and this authored snippet:

```sql
CAST(REVERSE(SUBSTRING($(value), 3, 8)) AS $(type))
```

The expected value is 123. Changing only the snippet instance to multiply the
result by two changes the expected value to 246. No adapter contains a reverse,
substring or multiplication implementation for this conversion.

## Run

Requires the installed `meta` CLI, .NET 8-compatible test execution and local
SQL Server `.` with Windows integrated authentication and permission to create
and drop a disposable database. Inspect `meta create --help`, `meta insert
--help` and `meta instance update --help` for the authoring commands used here.

Build the test host once:

```powershell
dotnet build MetaSchemaAdapter/Tests/MetaSchema.Adapter.Tests.csproj -c Release -m:1 -nr:false
./Demos/CustomDataTypeConversionProbe/Run.ps1
```

The script creates separate conversion/source/target workspaces under
`artifacts/custom-conversion-probe/<guid>` through modeled CLI commands. It runs
the compiled test DLL twice, editing only snippet instance data between runs,
and verifies every DLL in the test output directory has the same SHA-256 hash.
It leaves the workspaces and both evidence reports available for inspection.
The test drops its uniquely named disposable database after each run.

The diagnostic is explicitly skipped during ordinary test runs unless
`META_BI_CUSTOM_CONVERSION_PROBE_ROOT` names these fixture workspaces. Its passing
assertions mean the integration gap was reproduced, not that conversion worked.

## What the probe distinguishes

1. The custom mapping and snippet load through ordinary typed workspace APIs.
2. `SELECT src.WackyValue AS Value` binds against a target named `Value` with
   zero errors, but records no type assessment. The SELECT target-validation
   path looks up source type candidates by output name, so the alias loses the
   evidence. The resulting pipeline column has neither source nor target type.
   The probe retains this as a separate finding rather than fixing or hiding it.
3. A projection with matching source and destination names does record a
   sanctioned conversion to `sqlserver:type:int`.
4. A helper-only SQL control instantiates the selected snippet and executes it
   on SQL Server, returning 123 or 246. This establishes that the authored rule
   is executable, not that adapter execution uses it.
5. The existing TSV adapter projects the original `wx321` value. The ordinary
   adapter execution service connects it to a transport-only fixture target
   adapter wrapping `SqlServerBulkInsertTargetWriteOperation`.
6. Bulk copy ignores the snippet and attempts CLR conversion to `System.Int32`.
   Execution fails at `TargetWrite`, and no row reaches the target table.

The conversion service returns the selected mapping path during validation,
but the persisted binding assessment keeps only source/target type IDs. The
pipeline row shape and adapter request carry no selected conversion
implementation. Even when the custom workspace is explicitly supplied to the
bulk-copy writer, an already-SQL-Server destination type bypasses resolution;
its C# coercion chooses the runtime behavior.

For a user with fixed binaries, the missing capability is an end-to-end handoff
of the resolved conversion and an execution realization that honors its
authored semantics. Binding currently proves compatibility without establishing
that executable contract. Editing another workspace cannot repair that missing
product behavior. SQL snippet execution also needs an appropriate SQL execution
location; the TSV adapter does not claim to interpret SQL functions.

## Observed results, 2026-10-26

| Check | Original snippet | Snippet with `* 2` |
| --- | --- | --- |
| Binding errors, matching names | 0 | 0 |
| Sanctioned conversion assessments | 1 | 1 |
| SQL control result | 123 | 246 |
| Adapter execution | TargetWrite failure | TargetWrite failure |
| Inserted rows | 0 | 0 |
| Alias binding errors / type assessments | 0 / 0 | 0 / 0 |

Both executions reported:

```text
Column 'WackyValue' value of CLR type 'System.String' could not be coerced to
'System.Int32' for SQL Server bulk copy. The input string 'wx321' was not in a
correct format.
```

All DLL hashes stayed unchanged between the two runs. The Release test-host
build passed with zero warnings/errors; both explicit diagnostic runs passed
their reproduction assertions. The ordinary adapter suite passed its five
existing tests and explicitly skipped the manual probe. No disposable probe
databases remained. These checks demonstrate the defects; no production fix is
included.
