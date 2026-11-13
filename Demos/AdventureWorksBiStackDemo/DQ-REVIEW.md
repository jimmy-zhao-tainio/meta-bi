# DQ alignment investigation

The demo's recorded result is **167 promoted checks, 45 with findings**. Those
findings are wanted review evidence and remain visible. They describe situations
to inspect, not 45 established transformation defects. The individual findings
and independent comparisons below explain this sample result.

`Verification/verify-stack.sql` now executes the deployed review once and reports
its check and finding counts. Findings do not automatically fail verification;
missing checks, SQL execution errors and other reconciliation failures still do.
The complete modeled SQL verification operation passes on the loaded stack.
MetaDataQuality implementation and production-scale execution/dashboard work
are deferred.

## Actual path and ownership

The mesh's `create-data-quality` operation runs
`meta-data-quality from-transform-workspace` over BusinessToWarehouse and its
Binding workspace. `MetaDataQualityCandidateDiscoveryService` uses Binding only
to select validated script IDs. It extracts modeled join syntax and materializes
candidates, then adds corpus-level inference. Binding supplies no DQ acceptance
decision, relationship cardinality or target-grain contract.

`MetaDataQualityCandidateDiscoveryService.Materialization.cs` explicitly records
these assumptions: relationship direction needs correction, multiplicity is not
inferable from syntax alone, optionality needs business intent, and candidate
keys need confirmation. The current model has no explicit relationship-direction,
required/optional or output-grain acceptance contract.

The mesh then promotes **all** candidates. `MetaDataQualityPromotionService`
only changes status; it does not resolve those assumptions.
`DataQualityToSqlConverter` executes the sanctioned
`MetaConvert/Weaves/DataQualityToSql` weave to create MetaSql view definitions,
then emits deployment SQL. The demo deploys 167 check views plus the dashboard.
The former verification SQL treated any nonempty investigation view as a
failure. That zero-findings guard has been replaced by reporting the observed
results, consistent with the review contract. These are not 45 Pipeline failures
or Binding type failures.

## SQL Server findings

Run the read-only probes against the loaded demo:

```powershell
sqlcmd -S . -E -C -d AdventureWorksAnalytics -b -W -s '|' `
  -i Demos/AdventureWorksBiStackDemo/Verification/investigate-dq.sql
```

The current result is 45 nonempty checks out of 167:

| Candidate family | Nonempty checks | What the generated queries actually measure |
| --- | ---: | --- |
| JoinMultiplicityExplosion | 14 | Multiple detail rows sharing a join key |
| OutputDuplicateRisk | 14 | The same join-key grouping as the multiplicity query |
| JoinOrphan | 12 | Lookup-side rows unused by the detail side |
| OuterJoinNullExpansion | 5 | Preserved rows lacking an optional relationship |

### Lookup reuse is not fanout at output grain

For example, `JoinMultiplicityExplosion.JoinPattern.23.50` and
`OutputDuplicateRisk.JoinPattern.23.51` join `BL_SalesOrderLineSalesOrder` to
`BH_SalesOrder`, group by the **sales-order foreign key**, and flag counts above
one. They report 21,209 orders with multiple lines. Neither query evaluates
duplicate `(SalesOrderID, SalesOrderDetailID)` output grains.

The other reports count product, customer, salesperson, territory, address,
state/province or subcategory lookup reuse. All corresponding lookup keys are
unique at the scope used by the authored transforms. The investigation's 15
explicit forward-reference probes returned **zero missing lookup rows and zero
duplicate lookup keys**. Satellite lookups use `VersionRank=1`, matching the
authored latest-version semantics.

The actual `Fact_SalesLine` contains **121,317 rows and 121,317 distinct
SalesOrderID/SalesOrderDetailID grains**. Existing verification also establishes
the order and quota fact grains and source measure reconciliations.

### The orphan direction is reversed for these relationships

For example, `JoinOrphan.JoinPattern.17.154` starts from
`BHS_Person_PersonProfile` and finds people without a `BL_SalesPersonPerson` link.
Its 19,955 findings are not salespeople lacking person records. The authored
lookup in the opposite direction has zero missing references.

The other alerts inspect unused people (853), countries (232 or 226), products
(238), customers (701), addresses (495), and state/province rows (107).
Several relationships are reported in more than one fact. These are lookup
coverage observations, not evidence that a detail reference points nowhere.

There is a second execution-semantic limit: these candidate SQL views query
resolved base tables, not the complete transform expression. For example, the
person orphan query reads the entire satellite, while the transform joins a CTE
restricted to the latest version. A clean first run does not establish that
these queries remain meaningful after additional satellite versions. Reversing
all orphan checks mechanically would not solve this scope issue.

### Optional gaps agree with the source

| Nullable source relationship | Source NULL rows | Reported rows |
| --- | ---: | ---: |
| Customer.PersonID | 701 | 701 |
| Customer.StoreID | 18,484 | 18,484 |
| Product.ProductSubcategoryID | 209 | 209 |
| SalesOrderHeader.SalesPersonID | 27,659 | 27,659 in each of two facts |

The generated label says “Unexpected NULLs from outer joins”, but no authored
rule establishes that these gaps are unexpected. The investigation also compares
the exact natural-key sets of source NULL relationships with missing BDV links in
both directions: **zero mismatches**. This proves the observed optional gaps were
preserved at that boundary; it does not establish a general DQ acceptance policy.

## Audit of the earlier zero-findings claim

The user recalled surprise at the original zero-alert result and requested an
investigation of that claim before choosing any acceptance change.

The available file history begins at initial public commit `851bb3a`. That commit
already contains both the README's “167 promoted Data Quality checks with no
findings” claim and the verification script's zero-findings guard. There is no
earlier incremental history for these demo files in this repository, and no
captured original DQ execution result was found in the checked-in demo artifacts.
The local MetaDQ RunLog was also empty when inspected; that does not establish
what happened on the original installation.

The historical check SQL was tested rather than inferred from the claim:

1. Extracted the 167 candidate view definitions and dashboard directly from
   `851bb3a`, without regenerating them. All **168 definitions match** the current
   generated definitions after line-ending normalization. The only differences
   in the complete SQL file are two blank lines in the operational pack.
2. Deployed those exact historical views into the disposable database
   `MetaBiDqHistoricalProbe20261109`. Their original three-part references read
   the current fully loaded demo BDV. They returned **167 checks, 45 nonempty**,
   with precisely the same family counts and finding-group totals as the current
   pack. The disposable database was dropped afterward.
3. Executed the original `851bb3a` verification file through the repository's
   Release `meta-sql` CLI against the live stack. It failed at the DQ guard with
   **exit code 1** and “Data Quality checks returned findings.” The original
   checks preceding that guard passed. The CLI Program and execute handler have
   no changes between `851bb3a` and the current committed main.

Thus this alignment did not introduce the alerts, and the shipped SQL does not
hardcode zero counts, filter out these checks, or swallow this SQL failure in the
tested execution path. The earlier zero-findings claim is not reproducible on
the loaded official sample. This is a historical-source replay against current
data, not reconstruction of the original installation or its unretained output.
It cannot establish whether the old claim came from a different deployment/data
state, an unexecuted verification, a misread failure or deliberate concealment.
It must not be treated as verified evidence.

Historical SQL, deployment output, query results and the failing CLI transcript
are retained outside the checkout in
`C:/Users/jimmy/Documents/CodexCheckpoints/meta-bi-adventureworks-dq-investigation-2026-11-09`.

## Demo interpretation and verification status

The producing demo owns its intended row grain, source-preservation expectations
and optional relationships. MetaDataQuality owns candidate discovery and the
meaning of its generated checks. Binding and Pipeline must not acquire those
decisions.

The demo retains all 167 promoted checks and the 45 nonempty results as wanted
review evidence. Source comparisons and independent grain/value checks explain
the observed situations. This interpretation does not require changing
MetaDataQuality or deciding its future production-scale execution design.

The earlier no-findings expectation is superseded. The verifier materializes
the deployed review result once, confirms that all 167 checks are present and
reports nonempty checks and per-family finding groups. There is no candidate-ID
whitelist, no finding suppression and no hardcoded 45-result threshold. SQL
execution errors still propagate as operation failures.

The probes change no business data, and the only new database used for the
historical replay was removed. The historical investigation and repair sections
below describe their original verification state; the final verification is
recorded at the end of this document.

Verification: all **54 MetaDataQuality tests passed** with
`dotnet test MetaDataQuality/Tests/MetaDataQuality.Tests.csproj -c Release -m:1 -nr:false --nologo --verbosity quiet`.
The separate existing full-unit timing-budget failure is unchanged.

## Individual finding review

`Verification/review-dq-findings.sql` compares every nonempty view's actual
business keys and SuspectCount values with an independent query of the OLTP
source. Hash-key findings are resolved through the corresponding BDV hub; an
unresolvable key remains a mismatch. The review explicitly covers the 45 current
candidate IDs and reports any nonempty candidate outside that set. It is a
diagnostic for this run, not a permanent whitelist.

All **45** returned exactly the source-derived key/count sets: zero missing or
extra groups, and no nonempty candidates outside the review.

For the following paired checks, the IDs are the suffix after `JoinPattern.`.
Each pair consists of a JoinMultiplicityExplosion and OutputDuplicateRisk check.
Both count the same ordinary lookup reuse; neither establishes duplicate output.

| Multiplicity / duplicate IDs | Source situation | Finding groups per check |
| --- | --- | ---: |
| 10.113 / 10.115 | Addresses sharing a state/province | 68 |
| 14.143 / 14.145 | Products sharing a subcategory | 28 |
| 23.50 / 23.51 | Lines sharing an order | 21,209 |
| 26.32 / 26.33 | Lines sharing a product | 266 |
| 29.24 / 29.25 | Orders sharing a customer (line transform) | 7,470 |
| 32.81 / 32.83 | Orders sharing a salesperson (line transform) | 17 |
| 35.71 / 35.73 | Orders sharing a territory (line transform) | 10 |
| 38.10 / 38.11 | Orders sharing a ship address (line transform) | 7,470 |
| 44.41 / 44.42 | Orders sharing a customer (order transform) | 7,470 |
| 47.101 / 47.103 | Orders sharing a salesperson (order transform) | 17 |
| 50.91 / 50.93 | Orders sharing a territory (order transform) | 10 |
| 53.17 / 53.18 | Orders sharing a bill address | 7,470 |
| 56.3 / 56.4 | Orders sharing a ship address (order transform) | 7,470 |
| 65.60 / 65.61 | Quota periods sharing a salesperson | 17 |

The 12 JoinOrphan checks inspect these exact unused-lookup key sets:

| ID suffix | Source situation | Findings |
| --- | --- | ---: |
| 10.112 | State/province with no address, through country link | 107 |
| 9.116 | State/province with no address, through profile | 107 |
| 17.154 | Person who is not a salesperson | 19,955 |
| 2.130 | Person not referenced by a customer | 853 |
| 20.160 | Country not referenced by a sales territory | 232 |
| 26.31 | Product not used on an order line | 238 |
| 29.23 | Customer with no order (line transform) | 701 |
| 44.40 | Customer with no order (order transform) | 701 |
| 38.9 | Address unused for order shipping (line transform) | 495 |
| 56.2 | Address unused for order shipping (order transform) | 495 |
| 53.16 | Address unused for order billing | 495 |
| 5.110 | Country not referenced by a state/province | 226 |

The five OuterJoinNullExpansion checks also match exact source keys:
`1.136` = Customer.PersonID NULL (701), `3.128` = Customer.StoreID NULL
(18,484), `12.150` = Product.ProductSubcategoryID NULL (209), and `31.86` /
`46.106` = SalesOrderHeader.SalesPersonID NULL (27,659 each). These observed
source situations do not establish a missing-reference or duplicate-output defect.

## Separate correctness defect exposed by output review

The independent comparisons in `Verification/review-dq-outputs.sql` found a real
value-preservation failure that these join candidates do not detect:

| Output | Compared rows | Changed rows |
| --- | ---: | ---: |
| Dim_Customer | 19,820 | 18,484: CustomerName loses its separating space |
| Dim_Salesperson | 17 | 17: FullName loses its separating space |
| Dim_Geography | 19,614 | 0 |
| Dim_Product | 504 | 0 |
| Dim_SalesTerritory | 10 | 0 |
| Fact_SalesOrder | 31,465 | 0 |
| Fact_SalesLine | 121,317 | 0 |
| Fact_SalespersonQuota | 163 | 0 |

These comparisons cover the selected business values and relationship keys in
that diagnostic, not all columns in the warehouse. Text is compared as binary
Unicode values, after the deliberate varchar-to-nvarchar AccountNumber conversion.
Fact comparisons include per-row measures and resolved dimension business keys.

For example salesperson 274 should have `Stephen Jiang`, but has `StephenJiang`.
The authored SQL uses `CONCAT(person.FirstName, N' ', person.LastName)`.
The persisted XML still contains `<Value> </Value>`, but exporting that workspace
through the real CLI produces `CONCAT(person.FirstName, N'', person.LastName)`.

A minimal reproduction through modeled CLI import/export confirmed:

```sql
-- Authored input, and export after a C# workspace round trip:
SELECT N' ' AS [Space], N'a b' AS [Words];
-- Export after an XML workspace round trip:
SELECT N'' AS [Space], N'a b' AS [Words];
```

Actual SQL Server DATALENGTH of Space is **2 / 0 / 2 bytes** for authored SQL /
XML reload / C# reload respectively. The C# control isolates the failure from
SQL parsing and the expression emitter. Foundation XML instance loading uses
`XDocument.Load(..., LoadOptions.None)` in `Meta/Surfaces.Xml/XmlWorkspaceReader.cs`
and `InstanceXmlCodec.cs`. That XML option discards whitespace-only text nodes:
parsing `<Value> </Value>` yields length zero with None and length one with
PreserveWhitespace. The transform emitter subsequently renders the empty value
faithfully. The initial public demo XML already contains these two space literals.

This is a foundation XML value-fidelity defect, with a concrete TransformScript
and Pipeline consequence. The correct fix belongs to the XML workspace owner.
The investigation initially left the sibling meta repository unchanged. The
user subsequently authorized fixing this blocker directly; the verified fix
and live repair are recorded below.

Both review scripts and the whitespace reproduction were executed against the
live SQL Server. The reproduction workspaces and transcripts are retained in
the external investigation checkpoint. The acceptance guard remains unchanged.

## Whitespace fix and live repair

The foundation fix was developed on `meta` branch `fix/xml-whitespace-values`
and subsequently fast-forwarded into its default `master` branch:

- Instance codecs and the workspace reader preserve whitespace text nodes.
- XML writing entitizes carriage returns rather than normalizing value content.
  File formatting continues to use LF.
- Twelve persisted-workspace regression cases cover NULL, empty, spaces, tabs,
  CR, LF, CRLF, mixed whitespace, surrounding spaces and Unicode. They exercise
  typed loading, both direct instance-loading entry points and unchanged saves.
  Eight cases failed before the fix; all now pass. All 25 typed XML tests, the
  28-case surrounding integration selection and four XML codec/merge cases pass.

The additional TransformScript regression exposed two earlier losses in SQL
import itself: batch splitting rebuilt lines using platform newlines, and lexer
line tracking consumed CRLF while retaining only CR in literal values. Those
points now retain the exact original characters. Function export uses lexical
token spans to indent syntax without changing quoted contents; regression cases
cover views, inline table functions and scalar functions. The modeled
TransformScript-to-SQL generator likewise preserves literal render-event tokens
when indenting function bodies. Its relation and transformation were updated
through the owning MetaWeave CLI. A new regression asserts exact literal values
from that production generator; existing SQL formatting comparisons also pass.
The persisted SQL literal cases include mixed line endings. No SQL conversion policy,
Binding approval rule or Pipeline coercion was introduced.

BI consumers were rebuilt in Release against local foundation package
`0.1.0-internal.17`. Script CLI, Binding CLI and Pipeline CLI contain the same
corrected XML DLL (SHA256
`F1ED6368C321DC7AFF34BA900C448C9CF90AF1143F0A2338889A301B6E4D8D6B`).
These local packages have not been published; the existing shared installed
tools were not overwritten.

The original persisted demo TransformScript workspace was reused without
editing its values. Export now retains `N' '`. Schema-aware Binding was
regenerated through its CLI and validated all 20 warehouse scripts. The prior
Binding is preserved outside the checkout. Actual Pipeline execution completed
all 20 tasks successfully (run `D19170F1-B411-4499-8488-0CF100C1698B`).

The repeated live source/output comparison reports zero missing, extra or
changed selected rows in all eight outputs above. In particular all 19,820
customer names and all 17 salesperson full names now match exactly; salesperson
274 is `Stephen Jiang` in both source-derived expectation and output. This is a
repair through the ordinary transform execution path, not a direct SQL update
to the damaged values.

The external four-row baseline also passed schema extraction, persisted import,
schema-aware Binding and actual Pipeline bulk writing. Ordinary text, Unicode,
NULL and empty strings matched exactly; its disposable database and workspaces
were removed. DQ remains **167 checks, 45 nonempty**. No findings were hidden and
the existing full-stack zero-findings guard still prevents a green declaration.

Scalar and inline-table functions with mixed CRLF/LF values were also exported
through `to sql-path`, deployed to disposable SQL Server and executed. Their
returned bytes match the expected value exactly; the probe database was removed.

Fix patches, local packages, prior Binding, regression logs and live transcripts
are retained in
`C:/Users/jimmy/Documents/CodexCheckpoints/meta-xml-whitespace-fix-2026-11-09`.
The earlier alignment and investigation checkpoints remain intact. The existing
unit timing-budget failure is separate; its threshold was not changed.

Whitespace regression verification passed **571 TransformScript integration tests**, with no
failures or skips. On user approval, foundation commit `3743f7c` was pushed to
`origin/master` and BI import, formatting and modeled-generator commit `3570521`
was pushed to `origin/main`. Package `0.1.0-internal.17` has not been published to
NuGet.org; the existing consumer CI builds its development feed from foundation
default-branch source. Those fixes were committed separately from the demo
alignment verified below.

## Final SQL alignment verification

The completed alignment was verified on the local SQL Server on 2026-11-13:

- The actual `meta-mesh run --operation verify-stack` operation succeeded using
  the repository's Release `meta-sql` CLI. It reported 31,465 orders, 121,317
  lines, 163 quota rows and monetary totals of 123216786.1159, 109846381.4250 and
  95714000.0000 for total due, line sales and quotas respectively.
- The verifier executed the 167 deployed checks once and reported 45 nonempty
  checks: 14 multiplicity, 14 duplicate-risk, 12 orphan and five outer-join checks.
  No check was changed or suppressed, and the verifier does not require a fixed
  nonempty count.
- `review-dq-outputs.sql` found zero missing, extra or changed selected business
  rows across all eight outputs. Exact customer-name comparison covered 19,820
  rows with zero changes; no salesperson-name differences were returned.
- Fresh schema-aware Binding through the CLI validated all 120 persisted
  scripts with the selected catalog: 53 SourceToRaw, 47 RawToBusiness and 20
  BusinessToWarehouse. The latest recorded Pipeline task evidence has those
  same counts, all succeeded. This verification did not rerun ETL.
- A separate copy of the verification SQL with an invalid DQ column failed
  through `meta-sql` with exit code 1. Reporting findings does not swallow SQL
  execution errors; the failure probe changed no database objects or data.
- Release builds and 100 focused integration cases passed: 30 DV converter,
  10 conversion-catalog, six type-catalog and 54 Data Quality cases. The 18
  affected conversion-service unit cases also passed. The previously recorded
  full-unit timing-budget failure remains separate; its threshold was unchanged
  and the full timing profile was not rerun.

Analysis Services was not rerun. No DQ redesign, execution scheduler or dashboard
scalability work was included. Inputs, prior work and final verification logs
are preserved outside the checkout in
`C:/Users/jimmy/Documents/CodexCheckpoints/meta-bi-demo-finalization-20261113-223041`.
