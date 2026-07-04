# MetaDataQuality CLI Integration Sample

This demo shows the current end-to-end MetaDataQuality workflow against a real local SQL Server database.

It creates source tables and data, deploys the original transform views, imports the same scripts into `TransformWS`, discovers generated DQ views into `DataQualityWS`, promotes the full first-run pack, generates SQL views, and deploys them.

The workflow is modeled in:

```text
MetaDataQualityCliIntegration.MetaMesh
```

Run commands from the mesh folder. `--workspace` is omitted because `meta-mesh`
defaults to the current directory:

```powershell
$env:META_DQ_DEMO_MASTER_SQL = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
$env:META_DQ_DEMO_SOURCE_SQL = "Server=.;Database=MetaDataQualityCliIntegration;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

cd MetaDataQualityCliIntegration.MetaMesh
meta-mesh show
meta-mesh run --operation cleanup
meta-mesh run --operation build-data-quality
```

Operations:
- `cleanup`: drops the source and MetaDQ operational databases and removes generated outputs.
- `build-data-quality`: creates source SQL objects, imports transforms, discovers DQ candidates, promotes them, generates SQL, and deploys the generated review objects.

Workflow:
- `setup.sql` creates `sales.Customer`, `sales.Order`, and `sales.Invoice` with seeded data.
- `SourceViews/*/view.sql` creates the original transform views in the database.
- `meta-transform-script` imports those views into `TransformWS`.
- `meta-data-quality from-transform-workspace` scans the full transform workspace; `--binding-workspace <path>` can constrain discovery to validation-backed binding rows when a corpus contains broken objects.
- `meta-data-quality inspect` explains the generated DQ pack.
- `meta-data-quality promote --all` promotes all generated candidates for SQL output.
- `meta-convert data-quality-to-sql` writes `DataQualityViews.sql`.
- `meta-sql execute` deploys generated DQ views, `dq.v_DataQualityReview`, and the `MetaDQ` operational pack.

What this demo proves:
- generated DQ views are real SQL, not placeholders
- original transform views are present in the same database and referenced by DQ output
- `dq.v_DataQualityReview` summarizes which generated views returned rows
- `MetaDQ.dbo.Run` persists each review run and `MetaDQ.dbo.Findings` returns actionable rows
- `MetaDQ.dbo.RunLog` and `MetaDQ.dbo.FindingLog` keep run history
- row-multiplication and duplicate-output checks are suppressed when the transform projects detail-side identifiers such as `OrderId` or `InvoiceId`
- missing-reference and outer-join-null checks still return rows from the seeded data
- verification data is left in SQL Server for inspection rather than printed back from the script

Outputs:
- `TransformWS`
- `DataQualityWS`
- `DataQualityViews.sql`
- `MetaDataQualityCliIntegration` local SQL Server database
- `dq.v_DataQualityReview` dashboard view
- `MetaDQ` operational database with plain `dbo` objects

