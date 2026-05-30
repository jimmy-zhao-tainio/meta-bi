# MetaTransformScript Stored Procedure Contracts

Stored procedures are imported into MetaTransformScript as deployable SQL modules, but their operational behavior is not inferred from the procedure body. The procedure body remains the deployable SQL artifact. The stored procedure contract is the authored metadata that binding and orchestration are allowed to trust.

## Contract Scope

A `StoredProcedureContract` row means the stored procedure contract is complete and authoritative. There is no partial contract state. If a stored procedure has no modeled operations or result rowsets, the contract still has one `StoredProcedureContract` row and simply has no declaration rows for those sections.

The contract contains only the parts currently consumed by binding or orchestration:

- `StoredProcedureContractOperation`: an ordered operation inside the procedure. Each row has a global `Ordinal`, an `OperationKind`, a `SqlIdentifier`, and optional `AccessRole` and `Notes`.
- `StoredProcedureResultRowsetItem` and `StoredProcedureResultColumnItem`: the optional single result rowset exposed by the procedure. Binding uses it as the declared output rowset.

Do not add extra contract fields for information that binding and orchestration do not consume. Procedure parameters, transaction behavior, dynamic SQL details, external side effects, and scheduling hints should be modeled only when a concrete binding or orchestration rule needs them.

## Operation Kinds

`StoredProcedureContractOperation` is intentionally aligned with orchestration. Order matters across all operations, so a reset before an append is different from an append before a reset.

- `Read`: the procedure reads the rowset.
- `Append`: the procedure writes rows without declaring a prior reset.
- `Replace`: the procedure replaces the rowset contents.
- `Reset`: the procedure clears or truncates the rowset.
- `Mutation`: the procedure may both read and write the rowset, or the write shape is not more specific yet.
- `Call`: the procedure calls another stored procedure.

Use the narrowest operation kind that binding and orchestration can rely on. For example, model a truncate followed by insert as two operations, `Reset` then `Append`, with increasing ordinals.

## CLI Workflow

Inspect stored procedures first:

```powershell
meta-transform-script stored-procedure view-contract --workspace .\TransformWS
```

Add or replace one complete contract:

```powershell
meta-transform-script stored-procedure add-contract --workspace .\TransformWS --name dq.RunReview --operation 10:read:src.Customer=CustomerInput --operation 20:reset:dq.CustomerReview --operation 30:append:dq.CustomerReview --operation 40:call:audit.MarkStarted
```

`add-contract` replaces the entire operational contract for the named stored procedure. Omitted options are declarations of absence. For example, no `--operation` means the stored procedure has no modeled internal operations. No `--result-rowset` means it exposes no modeled result rowset.

Remove a contract when the previous declaration is wrong and should not be trusted:

```powershell
meta-transform-script stored-procedure remove-contract --workspace .\TransformWS --name dq.RunReview
```

## Declaration Rules

- Use schema-qualified SQL identifiers in `--operation`.
- Repeat `--operation` and `--result-column` as needed.
- Declare at most one `--result-rowset`; multiple result rowsets are intentionally unsupported.
- Use `--operation <ordinal>:<kind>:<sql-id>[=<role>]`.
- Use stable ordinals with gaps, for example `10`, `20`, `30`, so later review edits can insert operations without renumbering everything.
- Use `--result-column <rowset>=<column>` to declare result columns. The named rowset is created if it was not already supplied with `--result-rowset`.
- Use `--notes` for review context, resolved uncertainty, or source evidence.

## Empty Contracts

A stored procedure that has no modeled operations or result rowsets is still declared with `add-contract`:

```powershell
meta-transform-script stored-procedure add-contract --workspace .\TransformWS --name audit.MarkHeartbeat --notes "No modeled rowset effects."
```

This is intentionally all-or-nothing. There is no `--complete` flag because the command always writes a complete contract.

## Binding Behavior

Binding fails stored procedures unless exactly one `StoredProcedureContract` row exists.

- Zero contract rows: `StoredProcedureContractMissing`.
- More than one contract row: `StoredProcedureContractInvalid`.
- Blank or unsupported operation declarations are binding errors on the declaration rows.

Once the contract exists, binding consumes the declaration rows exactly as authored. It does not parse the stored procedure body to discover missing reads, writes, calls, or result sets.

## Orchestration Behavior

Orchestration consumes `StoredProcedureContractOperation` rows in ordinal order. This lets one stored procedure represent the internal steps of a pipeline when needed.

Operations become ordered object accesses:

- `Read` becomes a read dependency.
- `Append` and `Replace` become write effects.
- `Reset` becomes a reset write effect.
- `Mutation` becomes a read/write effect.
- `Call` is a declared procedure call and does not create a rowset object access.

Task object effects are classified from the ordered accesses. A reset followed by an append is treated as a replacement-style load; an append followed by a reset is treated as a final reset.

## Agent Guidance

Agents should treat a stored procedure contract as a reviewed declaration, not a guess. Read the procedure text and any available source schema before writing the contract. When unsure, leave the contract missing or use `--notes` to record the evidence that made the declaration safe. Do not create extra declaration rows for objects that are only mentioned in comments, dynamic strings, temp-table implementation details, or unmodeled external systems.
