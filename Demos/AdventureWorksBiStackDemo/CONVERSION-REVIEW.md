# Conversion alignment

## Native Raw types

The former built-in conversion workspace contained direct int -> nvarchar and
smallmoney -> decimal mappings described as SQL Server write compatibility.
The DV-to-SQL generator consumed those mappings as physical type policy: integer
keys and payload fields became nvarchar(256). This was reproducible with fresh
generation, not only present in historical schema snapshots.

Those two defaults have been removed through modeled instance authoring.
The Raw model, generated SQL and extracted live schema now retain native int and
smallmoney fields. For example, H_Product.ProductID and
HS_Product_Product.DaysToManufacture are int. Logical Meta-to-SQL mappings and
SQL alias lowering remain available; no mapping cardinality was changed.

## Deliberate load casts

The authored SourceToRaw SQL explicitly converts SQL alias types to their native
destination contracts: Name, AccountNumber and OrderNumber to appropriately sized
nvarchar; Flag and NameStyle to bit. RawToBusiness SQL explicitly converts money
and smallmoney projections to the BDV decimal(19,4) contract.

These are normal TransformScripts. Their source values, hash inputs, predicates
and integer reads are otherwise unchanged. No global int-to-text rule or automatic
conversion-graph approval is involved. A separate apply-conversions mesh step is
unnecessary for these explicitly authored casts.

If a future policy deliberately converts every read of a source type, its
apply-conversions step belongs after import and before Binding, with separate
input/output TransformScript workspaces and the required read-schema evidence.
That operation supports the demo's INSERT SELECT, CTE, nested-query and MERGE
forms; it changes reads, not write destinations. Destination-facet and expression-
typing limits remain documented in MetaConvert/TransformScriptConversion/README.md.

## Catalog and ownership

The mesh selects its data-type workspace explicitly for Binding, Pipeline and
Orchestration using --data-type-workspace. Their documented default remains
BuiltIn. The seven retired --data-type-conversion-workspace arguments are gone.
Binding validates the resulting SQL Server types and facets; it neither selects
nor applies conversions. Execution performs the validated work.

Physical schema generation is still a separate catalog consumer:
MetaConvert/DataVaultToSql/ToMetaSql.cs supplies BuiltIn catalogs, and its
LoweredTypes relation selects the SqlServer system and direct implementation by
fixed IDs. This alignment corrects the inappropriate default data without
redesigning that independent path. Selecting another catalog for Binding or
apply-conversions does not alter those generation choices.

## Source and verification

AdventureWorks2022 is the Microsoft OLTP sample, restored using the existing
prepare-adventureworks-db.ps1 helper. SOURCE-SETUP.md links the official download.
The local backup cache under Runs/_downloads is excluded from Git.

All 120 regenerated scripts pass schema-aware Binding against the live extracted
contracts and selected catalog: 53 SourceToRaw, 47 RawToBusiness and 20
BusinessToWarehouse. Old sanctioned-conversion assessments have been replaced
by validation of the actual expressions. A persisted-workspace integration
regression also verifies native integer keys, integer payload and smallmoney
through production DV SQL generation and reload.

The live orchestration completed all three pipelines and all 120 tasks. Counts,
fact grains, monetary reconciliations, native product types and exact product
values/names passed. The verify-stack operation now reports DQ findings rather
than treating their presence as a failure: 45 of 167 generated checks return findings
(14 JoinMultiplicityExplosion, 14 OutputDuplicateRisk, 12 JoinOrphan and five
OuterJoinNullExpansion). These findings are wanted demo review evidence. All
checks remain present, and SQL execution errors still fail verification.

For example, the Fact_SalesLine multiplicity query groups sales-order-line links
by SalesOrderHashKey and flags counts greater than one. It reports normal
multiple-lines-per-order reuse, while the fact has 121,317 rows and 121,317 distinct
SalesOrderID/SalesOrderDetailID grains. Such observations are retained for review;
they do not automatically establish a faulty transform. The complete modeled
SQL verification operation passes without changing DQ discovery, generated
checks or promotion policy.

The subsequent [DQ investigation](DQ-REVIEW.md) traces the actual discovery,
promotion and generation path. All 45 alerts are explained by lookup reuse,
reverse-direction coverage checks or nullable source relationships. Fifteen
forward-reference probes found no missing lookups or duplicate lookup keys.
The findings remain visible and wanted. The review records the current demo
interpretation, the evidence behind the sample result and the updated verification
report. DQ implementation work is deferred.

That review subsequently exposed and reproduced a separate blocking defect:
XML loading dropped whitespace-only SQL literals and changed customer and
salesperson names. The user authorized a foundation fix. With corrected XML
value preservation and SQL-import newline preservation, all 20 warehouse scripts
were rebound and executed through Pipeline. All eight independently reviewed
outputs now match the source-derived business values; the 45 DQ alerts remain
visible. The fix and exact live evidence are recorded in DQ-REVIEW.md.

Focused verification passed 30 DV converter tests, 10 conversion-catalog tests
and 18 existing catalog service cases. Release builds and the rebuilt Debug CLIs
on the normal PATH passed with no warnings. Optional Analysis Services deployment
and processing were not repeated in this alignment.

## Mesh recovery limitation

MetaMesh currently validates every referenced workspace as an existing input.
It has no modeled input/output distinction. If a failed generation step leaves
an output such as Runs/rdv/Binding absent, rerunning its operation is rejected
before the producing command executes. The alignment encountered and recorded
this failure. Real import and Binding commands recovered the validated output;
no dummy workspace or product bypass was introduced.

The checked-in generated workspaces support the ordinary demo sequence. Bootstrap
from absent outputs and recovery after failed generation remain a separate
MetaMesh ownership/model issue. The sibling meta repository was not changed.

Pipeline also reloads the complete TransformScript and Binding workspaces per
task in MetaPipelineModeledExecutionResolver through
MetaPipelineExecutionWorkspaceResolver.ResolveByIds. On this 120-task demo that
causes substantial worker startup time before SQL tasks begin. No cache or
execution-policy change is included in this alignment.
