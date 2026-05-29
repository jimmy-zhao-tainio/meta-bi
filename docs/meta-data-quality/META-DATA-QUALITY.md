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
7. Execute the MetaDQ run procedure, then drill into checks that returned rows.

Commands:

```cmd
meta-data-quality from-transform-workspace --transform-workspace TransformWS --new-workspace DataQualityWS
meta-data-quality from-transform-workspace --transform-workspace TransformWS --binding-workspace BindingWS --new-workspace DataQualityWS
meta-data-quality inspect --workspace DataQualityWS
meta-data-quality promote --workspace DataQualityWS --all
meta-convert data-quality-to-sql --workspace DataQualityWS --out DataQualityViews.sql
meta-sql execute --connection-env META_DQ_SOURCE_SQL --file DataQualityViews.sql
meta-sql execute --connection-env META_DQ_OPERATIONAL_SQL --query "DECLARE @RunId bigint; EXEC [dbo].[Run] @SourceDatabaseName = N'MetaDataQualityCliIntegration', @RunId = @RunId OUTPUT;"
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

- one generated DQ view per promoted candidate
- one dashboard view: `dq.v_DataQualityReview`
- one MetaDQ operational pack:
- tables `dbo.RunLog`, `dbo.FindingLog`
- procedures `dbo.Run`, `dbo.Findings`

Generated DQ views return investigation rows with columns such as:

- `DQView`
- `Issue`
- `Relationship`
- `TransformViews`
- `SuspectSide`
- `KeyValues`
- `SuspectCount`

The dashboard summarizes which generated views returned rows and includes prepared query text. `dbo.Run` executes that dashboard against a source database and persists run evidence; `dbo.Findings` returns findings for a run:

- `ReviewQuery`: query the generated DQ view that returned rows
- `TransformViewQuery`: query the original transform view context

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

See:

```cmd
Samples\Demos\MetaDataQualityCliIntegration\run.cmd
```

The demo creates a local SQL Server database, deploys the original transform views, discovers DQ candidates, promotes the generated first-run pack, generates SQL views plus `dq.v_DataQualityReview`, deploys them, and queries returned rows.
