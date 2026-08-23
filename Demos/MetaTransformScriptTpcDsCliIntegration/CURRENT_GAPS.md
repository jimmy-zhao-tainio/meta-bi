# Current Gaps

## TransformScript Import / Export

- Coverage: `q01`-`q99`
- Result: `99/99` scripts imported.
- Result: SQL export, re-import, and MetaSql projection diff succeeded for
  all imported scripts.
- Remaining import/export gaps in this slice: none.

## Binding + Validate Against Checked-In `SchemaWS`

Run basis:
- Coverage: `q01`-`q99`
- Active schema contract: checked-in `SchemaWS` in this demo folder.
- Execution mode: one workspace bind+validate against the checked-in `SchemaWS`
  as both source and target schema.

Result summary:
- Current operation: `meta-mesh run --operation build-tpc-ds-snapshot`
- Current result: imports, binds, exports, re-imports emitted SQL, converts
  both transform workspaces to MetaSql, and diffs the MetaSql workspaces
  offline.
- Remaining gaps: none known for this integration proof.

Gap classes:
- none known for offline import/export/bind.

Notes:
- Transform binding validates writable target contracts, so `tpcds.v_qNN`
  target contracts are modeled as `ObjectType=Table`.
- The transform workspace diff records expected representation and emitted-file
  provenance differences after re-importing semantic module paths through
  `RoundTrippedViews.manifest.tsv`.
- The MetaSql workspace diff is the semantic round-trip proof and currently
  reports no differences.
- The checked-in target rows come from extracted TPC-DS view metadata, with
  view rows changed to table rows so binding validates writable targets. The
  current fixture preserves the projected `varchar` and `decimal` fields needed
  by strict binding.
