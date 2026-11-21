# MetaDataQuality

`MetaDataQuality` scans a sanctioned `MetaTransformScript` workspace and proposes SQL data-quality views from the joins it finds.

It is not a hidden business-rules engine. It turns modeled transform syntax into an explicit review pack: discovered candidates, promoted candidates, generated SQL views, and an operational execution pack in `MetaDQ`.

## Workflow

1. Build or import a `MetaTransformScript` workspace.
2. Create a `MetaDataQuality` workspace from the whole transform workspace, or from the subset represented by a BindingWS.
3. Inspect what was discovered.
4. Promote the generated candidates that should become SQL.
5. Convert the promoted candidates to SQL views and MetaDQ operational SQL.
6. Deploy the SQL into a database that contains the underlying source tables and, where useful, the original transform views.
7. Execute `dq.Run` in the database where the checks are deployed. Browse the checks and their saved results, then run a transform's checks when needed.

Commands:

```cmd
meta-data-quality from-transform-workspace --transform-workspace TransformWS --output-xml DataQualityWS
meta-data-quality from-transform-workspace --transform-workspace TransformWS --binding-workspace BindingWS --output-xml DataQualityWS
meta-data-quality inspect --workspace DataQualityWS
meta-data-quality promote --workspace DataQualityWS --all
meta-convert data-quality-to-sql --workspace DataQualityWS --out DataQualityViews.sql
meta-sql execute --connection-env META_DQ_SOURCE_SQL --file DataQualityViews.sql
meta-sql execute --connection-env META_DQ_SOURCE_SQL --query "EXEC dq.Run;"
```

## How Discovery Works

Discovery traverses the typed semantic `MetaTransformScript` model. It does not parse ad-hoc SQL text during DQ discovery, and it does not treat inferred lineage as product truth.

When `--binding-workspace` is supplied, discovery first reads validation-backed `MetaTransformBinding.TransformBinding` rows and scans only the matching `TransformScript` ids. After partial binding, broken transform objects can remain in the raw transform workspace without producing DQ SQL that fails deployment.

For each SELECT-kind `TransformScript`, discovery starts at the modeled `TSqlStatement`, resolves the `SelectStatement` branch, and walks the explicit instance graph:

- `TransformScriptStatementLink`
- `TSqlStatement`
- `StatementWithCtesAndXmlNamespaces`
- `SelectStatementQueryExpressionLink`
- `QueryExpression`
- `QuerySpecification`
- `QuerySpecificationFromClauseLink`
- `FromClauseTableReferencesItem`
- `TableReference`
- `JoinTableReference`
- `QualifiedJoin`
- `QualifiedJoinSearchConditionLink`
- scalar and boolean expression links used by join predicates

It also follows CTE structure where the sanctioned model exposes it:

- `StatementWithCtesAndXmlNamespaces`
- `WithCtesAndXmlNamespaces`
- `CommonTableExpression`
- `CommonTableExpressionQueryExpressionLink`

When a join input points at a CTE or derived query, discovery recursively follows that scope and attempts to resolve the input back to concrete modeled base tables. This is why DQ output can say that a join found several CTE layers up eventually relates `sales.Customer` to `sales.Invoice`.

## What It Detects

Discovery currently detects qualified joins with modeled search conditions. From those joins it extracts:

- join type, for example inner or left outer
- left and right join inputs
- resolved base tables per join side, when available
- equality predicate parts from the modeled boolean expression tree
- composite join keys such as `c.CustomerId = o.CustomerId AND c.RegionId = o.RegionId`
- transform script id/name where the join was found
- query/specification/join ids and CTE/scope path for traceability

Repeated discoveries are compacted into `JoinPattern` rows. This means thousands of scripts can discover the same structural relationship without producing thousands of unrelated candidate definitions. The repeated script locations remain available as `JoinPatternOccurrence` rows.

Current candidate families:

- `JoinOrphan`: generated when a qualified join has equality predicates. SQL output checks whether rows on the referenced/detail side point to missing related rows.
- `OuterJoinNullExpansion`: generated when an outer join is found. SQL output checks whether the preserved side has rows without a match on the optional side.
- `JoinMultiplicityExplosion`: generated when an equality join may multiply rows at the apparent driving-side grain.
- `OutputDuplicateRisk`: generated when a transform shape may duplicate rows at the apparent output grain.

False-positive reduction currently uses projection evidence from the semantic TransformScript model. If the transform projects a non-key detail-side column such as `OrderId` or `InvoiceId`, discovery treats the transform as visibly detail-grain and suppresses row-multiplication and duplicate-output candidates for that join pattern.

## Output Shape

`meta-data-quality from-transform-workspace` writes a sanctioned `MetaDataQuality` workspace with explicit entities:

- `DataQualityCandidate`
- `JoinOrphan`
- `OuterJoinNullExpansion`
- `JoinMultiplicityExplosion`
- `OutputDuplicateRisk`
- `JoinPattern`
- `JoinPatternOccurrence`
- `JoinPatternOccurrenceBaseTable`
- `JoinPatternKeyPart`
- `DataQualityCandidateJoinPatternLink`

`meta-data-quality promote` changes candidate status from `Discovered` to `Promoted`. SQL generation only uses promoted candidates.

`meta-convert data-quality-to-sql` writes:

- one detail view and one result-summary view per promoted candidate
- `dq.v_ReviewChecks`, the list of checks in this deployment
- `dq.v_DataQualityReview`, the dashboard of saved results
- `dq.Run`, the local procedure for listing checks and running a selected scope
- `dq.Explain`, the interpreted result and evidence for one check
- `dq.MarkNoIssue`, records a review decision on a finding from a transform run
- one MetaDQ operational pack:
- tables `dbo.RunLog`, `dbo.RunCheck`, `dbo.FindingLog`, `dbo.FindingReview`
- procedures `dbo.Run`, `dbo.Findings`

The sanctioned `MetaDataQuality` → `MetaSql` weave generates the detail and result
summary for each check. `meta-convert` packages those views with the check list,
the dashboard and the MetaDQ operational pack. Deploy the combined file in order.
For split output, deploy `MetaDQ.Operational.sql` first, then the check files and
finally `v_DataQualityReview.sql`. Regenerate and redeploy the review pack before
running an installation that still has the original dashboard.

Generated DQ views return investigation rows with columns such as:

- `DQView`
- `Issue`
- `Relationship`
- `TransformViews`
- `SuspectSide`
- `KeyValues`
- `SuspectCount`

Run checks in the database where they are deployed, outside a transaction and
after the data is ready:

```sql
EXEC dq.Run;
```

With no arguments, `dq.Run` lists the deployed checks and reads saved results
without executing any checks. **Why look** shows what the last run found, or
what the check is looking for if it has yet to run. **Next step** opens the saved
evidence or runs the transform's checks when there is no saved result.

Pass a unique transform name or script ID to `dq.Run @Transform` to execute
its checks. A check can be shared by multiple transforms. Use `@All = 1` for
an explicit full run. Add `@IncludeClean = 1` to see clean checks in an
execution result as well.

If the evidence describes an expected result, copy **Mark no issue** from the explanation.
You can add `@Reason` to record why. The decision saves the reviewer, time and
reason alongside that finding. A later run produces a new result for review.

The dashboard reads the latest completed result for each transform from its
most recent scoped or full run. You can filter by transform or outcome without
executing the checks again.

```sql
SELECT [Transform], [Check], Outcome, Investigate
FROM dq.v_DataQualityReview
WHERE Outcome IN (N'Review affected rows', N'Review transform pattern', N'Check failed')
ORDER BY [Transform], [Check];
```

- **Review affected rows** means the check found data worth investigating.
- **Review transform pattern** means the finding comes from comparing how transforms are written.
- **No rows found** means the data check reported no findings.
- **Check failed** means the check raised a SQL error. Run the **Investigate** command to read it.
- **No issue** means a reviewer accepted this saved finding.
- **Not checked** means there is no saved result in the latest applicable run.

For row-multiplication and duplicate-risk findings with one modeled join pattern
and resolved base-table keys, the procedure checks how many matching rows
each input row receives. When each input matches once, a shared lookup key
explains the repeated join key. If any inputs receive multiple matches, the
procedure reports the number of affected inputs and returns both sides' actual
key values with the match count per input. This measures the modeled base-table
equality. Verifying the transform's full predicates and output grain requires
separate evidence.

For a resolved missing-match check, the evidence grid has one column per join
key on each side. SQL NULL marks the missing side. The query returns native
values, so SQL Server clients display binary keys as hex. The evidence uses the
same resolved base-table equalities as the generated check. A check with
multiple resolved join patterns can return one grid per pattern. Checks without
a resolved key query show the checked relationship and row counts.

**Saved result** shows what the prior run measured, or `-` if it could not
complete the check. **Evidence query** is the exact SQL used for the following
grids and can be run on its own. A runtime grid with no rows means the evidence
query no longer finds the issue in current data. Transform-pattern reviews show
the saved common and differing conditions side by side.

`MetaDQ.dbo.Findings @RunId = …` reads the detailed evidence captured during
the specified run, including the original rationale and counts. Automation can
capture the run ID from `dq.Run` using its optional `@RunId` output parameter.
Deploy and run under an identity with access to the DQ objects and their
underlying data. Dashboard readers need access to the saved results in `MetaDQ`.

## Interpretation

Returned rows mean: investigate this data situation.

They do not automatically mean the transform view is wrong. A transform can be correct while the underlying data still contains missing references, optional-side gaps, or business-valid multiplicity. The generated views are evidence starters, not final policy.

The intended first-pass workflow for large estates is:

- generate the full pack
- run it against real data
- review the dashboard first
- keep promoted views that reveal useful problems
- leave unhelpful candidates unpromoted in later curated runs

## What It Does Not Know Yet

Syntax alone cannot reliably infer:

- business key semantics
- tolerated duplicate policy
- tolerated orphan thresholds
- guaranteed relationship direction in every join
- whether an outer join null is expected by business design
- target-table grain, SCD semantics, or load policy

Those should become explicit metadata or policy when the project needs them. They should not be guessed silently.

## Demo

The [DQ CLI integration demo](../../Demos/MetaDataQualityCliIntegration/README.md)
creates a local SQL Server database, discovers and promotes checks, and deploys
the review pack. Its mesh runs the checks and reads the saved dashboard.
