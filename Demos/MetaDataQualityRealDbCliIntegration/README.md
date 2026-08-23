# MetaDataQuality Real SQL Server CLI Integration (Phase 2A/2B/2C)

This demo proves corpus-scope `MetaDataQuality` end-to-end against a real SQL Server database.

Prerequisites:

- `meta-transform-script`, `meta-data-quality`, and `meta-convert` available in `PATH`
- `meta-sql` available in `PATH`
- `meta-mesh` available in `PATH`
- SQL Server reachable at `.`

Set:

```powershell
$env:META_DQ_REAL_DEMO_MASTER_SQL = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_DQ_REAL_DEMO_SOURCE_SQL = "Server=.;Database=MetaDataQualityRealDbDemo;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_DQ_REAL_DEMO_OPERATIONAL_SQL = "Server=.;Database=MetaDQ;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
```

Run from the mesh folder:

```powershell
cd Demos\MetaDataQualityRealDbCliIntegration\MetaDataQualityRealDbCliIntegration.MetaMesh
meta-mesh validate --operation cleanup
meta-mesh validate --operation build-real-db-data-quality
meta-mesh run --operation cleanup
meta-mesh run --operation build-real-db-data-quality
```

What this demo proves:

- source SQL view definitions in `SourceViews` are parsed into a `MetaTransformScript` workspace
- `meta-data-quality` discovers both transform-scope and corpus-scope candidates
- Phase 2A candidates appear (review-only semantic outliers):
  - `MinorityJoinPattern`
  - `IncompleteCompositeJoin`
  - `SuspiciousExtraJoinPredicate`
- Phase 2B candidates appear and are runtime-check SQL when promoted:
  - `ImpliedForeignKeyMissingReference`
  - `ImpliedUniqueKeyViolation`
- Phase 2C candidates appear (review-only optionality drift):
  - `InnerJoinAgainstUsuallyOptionalRelationship`
  - `LeftJoinAgainstUsuallyMandatoryRelationship` may appear depending on corpus shape
- mixed promoted runtime-check and review-only semantic families convert in one run
- review-only semantic promotions generate informational SQL findings in `dq.v_DataQualityReview` (not runtime suspect-row checks)
- generated SQL runs against a real SQL Server database and returns real findings from seeded bad rows

Database names:

- `DEMO_DB=MetaDataQualityRealDbDemo`
- `METADQ_DB=MetaDQ`

Intentional bad data in this demo:

- order rows with missing customer references (for implied FK findings)
- duplicate customer composite keys (for implied unique-key findings)
- order rows with campaign references that are missing from `Campaign` (for optionality semantics in corpus analysis)

Important semantics:

- Phase 2A and 2C families are semantic review findings in SQL output.
- Phase 2B implied families are runtime-check SQL findings.
- unsupported promoted families (if introduced later without SQL mode support) fail fast by design.
- semantic review findings are informational and do not execute row-level source-data suspect-row checks.
- runtime checks execute against rows in `MetaDataQualityRealDbDemo`.

Dashboard column semantics:

- `RowsReturned` / `ResultRowCount` / `FindingGroupCount`:
  number of result rows (finding groups) returned by the generated DQ view.
- `TotalSuspectCount` / `SuspectRowCount`:
  sum of `SuspectCount` across returned result rows.
  For unique-key checks, one finding group can represent multiple physical duplicate rows.
- `ReferencingObject`, `ReferencedObject`, `CheckedObject`, `SuspectSide`:
  explicit direction and side context for runtime implied checks.

How to read the review dashboard:

- `OutputMode` tells you whether a row is a runtime data check (`RuntimeCheck`) or a corpus semantic review (`SemanticReviewFinding`).
- `FindingTitle` and `FindingCategory` summarize what kind of issue was promoted.
- `RelationshipLabel` gives plain relationship wording:
  - implied FK: `A references B`
  - implied unique: `B expected unique for A relationship`
- `FindingGroupCount` is the number of finding groups returned by the view.
- `SuspectRowCount` is the number of physical rows represented by those groups.
  For duplicate-key checks, one group can represent many duplicate rows.
- `EvidenceSummary` explains corpus-strength context for the promoted candidate.
- `RecommendedAction` is reviewer-facing next step text.
- `ReviewQuery` / `DetailQuery` opens the detail finding rows.
- `TransformViewQuery` / `SupportingTransformQuery` opens transform views that supplied evidence.

MetaDQ run semantics:

- `MetaDQ.dbo.Run` and `MetaDQ.dbo.Findings` include both runtime and semantic findings.
- semantic findings keep runtime count columns `NULL` by design.
- runtime totals aggregate `NULL` as `0` explicitly (no null-elimination warning in normal flow).
