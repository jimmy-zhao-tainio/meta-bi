# Product Model Review Queue

## At A Glance

1. Done: SQL Server `decimal`/`numeric` compatibility.
2. Done: MetaSchema types and Raw Data Vault ownership, roles, and false-ordering cleanup.
3. Done: Business Data Vault common satellite/attribute structure; roles, traversal, key precedence, payload-nullability policy, and false-ordering cleanup.
4. Done: satellite row identity and physical key contract belong in the implementation model.
5. Done: Data Vault hash-key storage width.
6. Done: Business datatype lowering consumes the sanctioned DataTypeConversion workspace.
7. Complete for supported mutation syntax: persisted, validated facts for every supported mutation form, including `MERGE` actions and conditions.
8. Done: all reported mutation-binding defects are fixed or disproven by coverage. The supported surface includes CTEs, predicates, wildcards, source ambiguity, `CAST`/`CONVERT`, and conservative homogeneous `CASE` contracts. SQL Server precedence inference is separate future expression-semantics work, not open repair debt.
9. Done: TPC-DS is the MetaTransformScript SQL round-trip and strict-binding corpus. Do not mix it into Data Vault, schema-extraction, or AdventureWorks decisions.
10. Done: AdventureWorks full-stack completed a deliberate clean replay across the modeled BI stack.
11. Complete: `MERGE` match semantics use explicit clause entities and an explicit predecessor chain.

Business satellite payload attributes are nullable by design; do not add nullability flags, subtype entities, binding proof, or compatibility shims.

## Purpose

The AdventureWorks full-stack work exposed several real product questions, but implementation continued into sanctioned models before those questions were reviewed. Those changes have been rolled back. This file preserves the problems, the attempted solutions, and the reason each attempt was made so the decisions can be taken one at a time.

No item below is approved by being listed here. Each item starts in `Pending discussion`.

## Review Rule

- Discuss the product concept before changing a sanctioned model.
- Agree on the model shape before changing generated tooling, CLIs, converters, fixtures, or demos.
- Keep bug fixes that do not require a model change separate from model proposals.
- Regenerate dependent artifacts only after the model decision is accepted.

## 1. SQL Server `decimal` and `numeric` Compatibility

Status: Approved and implemented on 2026-07-14

Observed problem:

Strict transform binding compared a modeled `decimal` expression with a target field extracted as `numeric`. SQL Server treats those names as synonyms, but the modeled conversion graph did not provide a direct path between the two SQL Server type identifiers.

Attempted change:

- Added direct `decimal -> numeric` and `numeric -> decimal` rows to the MetaDataTypeConversion instance.
- Added assertions for both mappings.
- Saved the workspace, which also reordered existing rows and made `model.xml` look substantially changed even though its entity schema was unchanged.

Why it was attempted:

To prevent strict binding from reporting an incompatibility for two SQL Server names with the same underlying semantics.

Decision:

SQL Server synonym compatibility belongs in the sanctioned conversion workspace, following the existing direct compatibility mappings for SQL Server aliases and normalized write types. Added direct `decimal -> numeric` and `numeric -> decimal` mappings using the existing `Direct` conversion implementation. The MetaDataTypeConversion model remains unchanged. `ConversionImplementation` was not introduced by this work; it already exists in `HEAD`. Its earlier apparent model change was serialization ordering noise.

## 2. MetaSchema Types and Raw Data Vault Ownership

Status: Complete on 2026-07-18

Observed problem:

The old MetaSchema represented tables/views, key categories, and field nullability as free-text discriminators or boolean-like text. MetaRawDataVault then copied the complete source system/schema/table/field/relationship graph and referred back to it. Raw link endpoint meaning was stored in `RawLinkHub.RoleName`.

Attempted change:

- Tried to make the copied source-field nullability and link role text required.
- Propagated those values through schema-to-vault and vault-to-SQL conversion.
- Regenerated tooling and affected sample/integration workspaces before the model ownership problem was reviewed.

Why it was attempted:

To make the generated physical contract deterministic, but the attempt preserved the wrong ownership boundary and still encoded modeled concepts as text.

Approved target design:

- MetaSchema should model a common `SchemaObject` specialized by `Table` or `View`.
- MetaSchema `Field` should retain its cohesive observed scalar facts, including nullability and identity evidence. The MetaSchema extractor and consumers own their concrete interpretation; do not create a marker entity for each scalar field fact.
- A common `TableKey` should be specialized by exactly one `PrimaryKey` or `UniqueKey`.
- MetaRawDataVault should model only the Raw Data Vault. The copied `SourceSystem`, `SourceSchema`, `SourceTable`, `SourceField`, and source relationship graph should be removed.
- MetaRawDataVault should own independent `Field` and `FieldDataTypeDetail` entities. Hub key parts and satellite attributes should relate to those fields.
- `RawLinkRole` should be a first-class endpoint relationship with a required `Name`; `RawLinkHub` and `RoleName` should be removed.
- Hub key-part order should be modeled by `PreviousKeyPart`, not by an ordinal scalar.
- `Kind`, `Is*`, `ObjectType`, `KeyType`, and equivalent free-text discriminators are not part of the approved target MetaSchema or MetaRawDataVault models.

Resolved design decision:

The target MetaRawDataVault does not model source or physical nullability on `Field`. Schema-to-Raw conversion must not copy MetaSchema nullability into the Raw model. Raw-to-SQL projection emits Raw fields as nullable, including hub business-key parts; technical columns retain their implementation-defined contracts.

The target Raw authoring surface has one `add-field` command. It creates the neutral field and optional datatype detail; it does not ask the caller to author a nullability classification that the Raw model does not own.

Target cross-model boundary:

- `MetaSchema` should own observed source nullability on `Field`. Extraction and binding should use the sanctioned MetaSchema field-property interpretation.
- `MetaSql.TableColumn` owns physical SQL nullability but still stores it in the scalar `IsNullable` property. That is a separate product-model review, not a reason to keep nullability on a Raw field.
- `MetaTransformScript.StoredProcedureResultColumnItem` also stores optional nullability in scalar `IsNullable`; its observed/unknown result-contract semantics require a separate review.
- `MetaDataWarehouse.DimensionAttribute`, `MetaDataWarehouse.FactMeasure`, `MetaDataWarehouseImplementation.PlatformColumnImplementation`, `MetaAnalytics.Attribute`, and `MetaTabular.TabularColumn` carry the same scalar smell. Review each domain before changing it; do not mechanically propagate the MetaSchema shape.
- `MetaTransformBinding` should not own a competing nullability model in this slice. It consumes `MetaSchema` source and target contracts, so its adaptation to the approved MetaSchema structure is expected.

Current implementation status:

The approved MetaSchema portion is now implemented. `SchemaObject` owns the shared name/schema identity and is specialized by exactly one `Table` or `View`; `Table.ObjectType` is gone. `Key` is specialized by `PrimaryKey` or `UniqueKey`; `TableKey.KeyType` and `TableKeyField.FieldName` are gone. `Field` still owns its existing source-column scalar facts.

The SQL Server extractor preserves its established `:table:` IDs for the new common object, table/view specialization, fields, keys, and key fields. It emits table-only key and identity evidence, while view outputs carry only name, type, and source position. Binding resolves a `SchemaObject` identity and treats view fields as nullable read-only rowsets. The Schema-to-Raw converter preserves `--include-views`: a requested view is copied as a read-only source rowset but cannot produce a Raw hub because views have no source key.

The canonical MetaSchema workspace and the two tracked persisted demo Schema workspaces were migrated through `meta model`, `meta insert`, and `meta bulk-insert`, not XML edits. Generated MetaSchema tooling, direct fixtures, extraction, binding, conversion, and both integration demos are aligned. The completed MetaRawDataVault ownership migration is recorded in 2.5 below.

Resolved foundation follow-up on 2026-07-18: `meta bulk-insert --stdin` bypassed the shared standard-input normalization, so a UTF-8 BOM made the first header `\uFEFFId` rather than `Id`. It now uses `MetaCliStandardInput`, matching MetaDocs and MetaMesh. An external-process regression test feeds a BOM-prefixed TSV header over UTF-8 redirected stdin and verifies the inserted row. `Meta.Core.Tests` passed 227/227.

### 2.1 MetaSchema Object Identity

Status: Complete on 2026-07-17

Replace the `Table.ObjectType` discriminator with a common `SchemaObject` specialized by `Table` or `View`. Keep an unchanged source sync identity-stable: existing table-derived IDs must not churn merely because the model introduces a common object row.

Implemented through the canonical MetaSchema workspace, regenerated tooling, SQL Server extraction, direct binding consumers, and the two tracked persisted demo Schema workspaces. Existing `:table:` source IDs were deliberately retained as the shared object/specialization IDs, so a source sync does not churn dependent field or relationship identity.

Acceptance: a repeated extraction of the same source produces no identity churn; tables and views are structurally distinct; no `ObjectType` property remains.

### 2.2 MetaSchema Key Specialization

Status: Complete on 2026-07-17; depended on 2.1

Replace `TableKey.KeyType` with exactly one concrete `PrimaryKey` or `UniqueKey` specialization of a common key. Preserve SQL Server key-column sequence as observed source fact; it is not generic UI ordering.

Implemented through the canonical MetaSchema workspace, regenerated tooling, SQL Server extraction, and Schema-to-Raw conversion. SQL Server constraint sequence remains observed source data on `KeyField.Ordinal`; it is not generic UI ordering.

Acceptance: no `KeyType` text remains; extracted primary and unique keys round-trip through the schema workspace with their observed key-column order intact.

### 2.3 MetaSchema Field Property Policy

Status: Complete as approved policy on 2026-07-17; no separate property-model migration

`Field` remains the cohesive representation of an observed source column. Its intrinsic scalar facts remain entity properties: name, datatype reference, source ordinal, nullability, identity evidence, seed, and increment. The MetaSchema extractor, binder, and SQL projection own their concrete types and permitted values. Do not introduce `FieldIsNullable` or equivalent marker entities merely to avoid scalar properties.

`FieldDataTypeDetail` remains a separate future review only if a concrete datatype-facet problem proves its current name/value representation inadequate. Do not combine that question with the approved Field property policy.

Acceptance: no Field model or converter migration is scheduled by this subtask.

### 2.4 Identity Evidence for Tables and Views

Status: Implemented on 2026-07-17; depended on 2.1

Views are consumed as read-only rowsets, not as writable tables. Their output columns carry names and types, are assumed nullable, and do not carry identity, seed, increment, key, default, or other base-table column metadata. Binding must not treat a view output as an identity target.

Base-table fields retain the cohesive Field property policy from 2.3. This resolves the view-evidence question without replacing that policy or creating a view-column property model.

The extractor omits nullability, identity, seed, increment, and keys for views. Binding supplies assumed-nullable runtime contracts for those outputs and rejects a view as a writable target contract.

### 2.5 Raw Data Vault Ownership and Native Fields

Status: Complete on 2026-07-18; depended on 2.1, 2.2, and 2.4

Remove the copied source graph from MetaRawDataVault. Model only Raw Data Vault structures, with independent `Field` and `FieldDataTypeDetail` entities referenced by hub key parts and satellite attributes. Raw does not own observed source nullability.

This slice includes the model-tooling migration, Raw CLI/service, Schema-to-Raw conversion, Raw fixtures, and canonical Raw workspace migration. It does not change Raw link endpoint semantics.

Implementation: the canonical model and tracked Raw fixture were migrated through `meta model` and `meta delete`; generated tooling, the Raw CLI workspace, Schema-to-Raw conversion, Raw-to-SQL projection, and the tracked Raw integration mesh now author independent `Field` and `FieldDataTypeDetail` rows only. The integration mesh retains its declared order, removes its 28 copied-source graph steps, and reauthors fields through the Raw surface.

Acceptance: no `SourceSystem`, `SourceSchema`, `SourceTable`, `SourceField`, source relationship, or copied source nullability remains in MetaRawDataVault; a schema-to-Raw conversion produces a self-contained Raw workspace.

### 2.6 Raw Link Endpoint Semantics and Sequence

Status: Complete on 2026-07-18; depended on 2.5

`RawLinkRole` is the named endpoint relationship from one `RawLink` to one `RawHub`. `Name` is required and must be unique within its Raw link. `RawLinkHub`, optional `RoleName`, and endpoint `Ordinal` are removed.

Endpoint sequence does not contribute Raw link identity or hash semantics. Raw-to-SQL creates a physical link table and its endpoint foreign-key columns, but it does not generate a hash expression or define which input values compose a link hash. That belongs to an explicit transformation pattern when the product supports one.

For example, an `Assignment` link can have `Employee`, `Department`, and `AssignedProject` roles. The SQL projection orders its endpoint columns deterministically by role name as `AssignedProjectHashKey`, `DepartmentHashKey`, and `EmployeeHashKey`. That stable physical order is a projection rule, not an authored link sequence.

### 2.7 Raw Link Role Migration

Status: Complete on 2026-07-18; depended on 2.6

Implemented the accepted role model through the canonical Raw workspace, generated tooling, the Raw MetaCli workspace and authoring surface, Schema-to-Raw conversion, Raw-to-SQL projection, the tracked Raw fixture, and the Raw integration mesh. The public command is now `add-link-role --id <id> --link <id> --hub <id> --name <value>`.

Raw-to-SQL rejects duplicate role names within one link. Focused tests prove three-role physical projection order and duplicate-role diagnostics. The external Raw mesh passed cleanup 5/5 and build/deploy 137/137, then produced a verification manifest with no remaining changes.

### 2.8 Cross-Model Integration Closure

Status: Complete on 2026-07-18; depended on 2.1 through 2.7

Regenerated `MetaSchema` and `MetaRawDataVault` tooling through the sanctioned `scripts/regenerate-tooling.ps1` path. Both tooling projects built successfully and regeneration produced no source diff. The canonical MetaSchema, MetaRawDataVault, MetaDataVaultImplementation, and Raw CLI workspaces passed supplemental generic workspace integrity loading.

Focused consumer proof passed: `MetaSchema.Tests` 9/9, `MetaDataVault.Tests` 46/46, and the strict cross-model `MetaTransformScript.Tests` 382/382. The full `RawDataVaultFromMetaSchemaCliIntegration` MetaMesh demo rebuilt its 150-step Business source, extracted 58 tables into MetaSchema, converted to Raw, projected to MetaSql, deployed 80 tables with 54 primary keys and 82 foreign keys, and finished with a no-change verification manifest. Cleanup then removed the generated workspaces and databases.

The scoped audit found no compatibility shim for the removed Raw endpoint entity or command in MetaSchema, Raw, Schema-to-Raw, Raw-to-SQL, or Raw demos. It also exposed and then removed a missed target-design inconsistency: `RawHubKeyPart`, `RawHubSatelliteAttribute`, and `RawLinkSatelliteAttribute` had scalar `Ordinal` values even though their only consumer was deterministic SQL column layout. The properties and their persisted values were removed through `meta model drop-property` from the canonical Raw and tracked Raw demo workspaces; the matching Raw CLI option aggregates were removed through generic `meta delete` from its authored MetaCli workspace. Raw authoring and Schema-to-Raw conversion no longer manufacture an order. Raw-to-SQL projects each collection by `Name`, then `Id`.

Regenerated Raw tooling no longer exposes the three properties. The Data Vault test project now builds and runs its exact local Raw, Business, and Convert CLI executables instead of invoking PATH, closing a real external-test false-positive/false-negative hazard. Verification passed: generic `meta check` for canonical Raw, tracked Raw demo, and Raw CLI workspaces; `MetaDataVault.Tests` 46/46; Raw CLI help for all three commands; no retired `--ordinal` use in Raw demos or the Raw CLI workspace; and the local `RawDataVaultCliIntegration` mesh validated 137 steps, ran 137/137 to a zero-change verification manifest after deploying 27 tables, 14 primary keys, and 25 foreign keys, then cleaned up 5/5. The remaining `add-link-hub` and `RoleName` usages belong to the Business Data Vault surface in item 3.

## 3. Business Data Vault Satellite Structure and Link Roles

Status: Complete on 2026-07-18

Observed problem:

Five repeated Business Data Vault satellite-attribute families had no structural nullability contract, while physical SQL generation must decide whether each emitted column is nullable. `BusinessLinkHub.RoleName` was optional even though endpoint roles are used in physical names. The model also uses scalar `Ordinal` values for link endpoints, bridge paths, key parts, attributes, point-in-time stamps, and point-in-time satellite references.

Attempted change:

- Added `IsNullable` to hub, link, reference, same-as-link, and hierarchical-link satellite attributes.
- Made `BusinessLinkHub.RoleName` required.
- Added matching CLI options and handler plumbing.
- Propagated the values through SQL conversion.
- Regenerated tooling, CLI workspaces, samples, and integration fixtures.

Why it was attempted:

The AdventureWorks binding gate exposed a nullable source attribute being projected into a target whose requiredness could not be represented honestly by the Business Data Vault model.

Accepted target design:

- `BusinessLinkRole` replaces `BusinessLinkHub`. It relates one Business link to one Business hub and has required `Name`; `RoleName` and `Ordinal` are removed. Names must be unique within one link.
- `BusinessBridgeTraversal` replaces the interleaved `BusinessBridgeLink` and `BusinessBridgeHub` lists. It relates one bridge to `SourceRole` and `TargetRole` relationships to `BusinessLinkRole`, plus optional `PreviousTraversal`. The chain expresses direction, preserves sequence, and disambiguates a link that reaches the same hub type through different roles.
- `BusinessSatellite` is the common satellite identity, specialized by the five current parent kinds. `BusinessSatelliteAttribute` is the common attribute identity and relates to that base satellite; one shared detail entity relates to the common attribute. The repeated parent-specific attribute and detail entities are removed.
- Business satellite payload attributes are nullable by design. MetaBusinessDataVault does not copy source nullability or assert a non-null business contract, so it has no nullability property, subtype, or marker entity. SQL projection emits nullable payload columns. Transform authoring and data-quality rules own source-specific requiredness handling.
- Composite Business hub and reference keys retain user-authored sequence through a predecessor relationship on each key part. Satellite attributes, point-in-time stamps, and point-in-time satellite references have no demonstrated domain sequence; their `Ordinal` properties should be removed and physical projection should use deterministic `Name`, then `Id` ordering. The equivalent Raw collections receive the same correction.
- Technical Data Vault columns remain implementation-defined. This decision concerns modeled Business satellite attributes only.

Role direction approved on 2026-07-15: role meaning must be represented by entities and relationships, not a free-text `RoleName` property. The accepted target makes that rule concrete for Business links and bridge traversal.

### 3.1 Completed: Business Link Roles and Bridge Traversal

- `BusinessLinkRole` now replaces `BusinessLinkHub` in the canonical Business workspace, all tracked Business samples, the Business CLI workspace, generated tooling, conversion, tests, and the Business integration mesh. A role has one link, one hub, and a required `Name`; names are unique within a link without regard to case.
- `BusinessBridgeTraversal` now replaces the interleaved bridge-link and bridge-hub rows. Each traversal has one bridge, distinct source and target roles on the same link, and an optional predecessor. Shared domain rules require one non-branching, acyclic, connected chain starting at the bridge anchor and continuing hub-to-hub.
- The authoring service rejects invalid role names and invalid traversal chains before saving. The converter invokes the same rules before projecting SQL, so malformed externally supplied models fail at the consumption boundary rather than being interpreted heuristically.
- Verification: the Business Data Vault test suite passed 47/47; the tracked integration mesh validated 148 steps, built/deployed 148/148 to a zero-change final manifest, and its modeled cleanup passed 9/9.

The satellite/attribute/common-structure redesign and the remaining false Business ordinals were completed later under this item. They were deliberately kept separate from this roles/traversal sub-slice.

### 3.2 Completed: Composite Business Key Precedence

- `BusinessHubKeyPart` and `BusinessReferenceKeyPart` now use optional `PreviousKeyPart` relationships instead of scalar `Ordinal` properties. This preserves an author-defined component sequence where it is genuinely part of a composite business-key contract.
- The chain is validated within one hub or reference: one start, no cross-parent predecessor, branch, cycle, or disconnected key part. The Business CLI exposes `--previous-key-part` for subsequent components and rejects a second unlinked head before saving.
- SQL conversion consumes the validated chain, so malformed externally supplied key-part graphs fail at the converter boundary instead of falling back to physical collection order.
- Verification: the full Data Vault suite passed 50/50; all canonical/tracked Business workspaces passed generic integrity checks; and the 148-step Business integration operation rebuilt and deployed the full stack from scratch before ending with a zero-change verification manifest.

Hygiene finding: the typed `meta-cli` save that authored the new Business CLI options rewrote its existing instance shards into canonical layout. The semantic change is limited to replacing the two retired `--ordinal` option aggregates with `--previous-key-part`; the broad deterministic formatting/order churn is a separate typed-workspace persistence concern and must be addressed through sanctioned save behavior, never by editing XML directly.

### 3.3 Completed: Projection-Only Business Ordering

- Removed scalar `Ordinal` from the five Business satellite-attribute entities, `BusinessPointInTimeStamp`, `BusinessPointInTimeHubSatellite`, and `BusinessPointInTimeLinkSatellite`. The canonical Business model, three tracked sample workspaces, and the tracked Business integration workspace were migrated through `meta model drop-property --strict`, which also removed persisted property values.
- Removed the eight matching `--ordinal` option aggregates from the authored Business MetaCli workspace through `meta delete --strict`, in relationship-safe token, option, executable-command-parameter, and parameter order. The generic writer canonicalized touched empty MetaCli rows to self-closing form; that broad textual churn has no command-surface semantic change beyond the eight deleted aggregates.
- Business authoring no longer manufactures sequence numbers. Business SQL conversion orders satellite payload members by case-insensitive `Name`, then `Id`; point-in-time hub and link satellite references are projected as one `Name`, then `Id` sequence rather than two ordinal-driven groups.
- Verification: regenerated `MetaBusinessDataVault` tooling built with 0 warnings/errors; `MetaDataVault.Tests` passed 53/53, including direct name-over-ID projection coverage and help coverage for `add-satellite-attribute`; a direct executable help check confirms it exposes no `--ordinal`. With trusted localhost SQL connections, the 148-step `build-and-deploy-business-data-vault` mesh passed end to end: 58 tables, 26 primary keys, and 61 foreign keys were deployed, and the verification deploy manifest reported no changes.

### 3.4 Completed: Common Satellite and Attribute Structure

- `BusinessSatellite` now holds the common satellite identity, name, and optional description. Each common row has exactly one typed concrete specialization: hub, link, reference, same-as-link, or hierarchical-link satellite. The existing point-in-time hub/link satellite relationships remain concrete and typed.
- `BusinessSatelliteAttribute` and `BusinessSatelliteAttributeDataTypeDetail` replace the five repeated satellite-specific attribute/detail families. Existing satellite, attribute, and detail IDs were preserved while canonical and tracked sample/demo workspaces were migrated through generic Meta tooling.
- The five parent-specific satellite commands create the common base and their specialization atomically. One `add-satellite-attribute --satellite <id>` command authors payload attributes for every satellite type; the retired payload commands and aliases are gone.
- Domain rules run in both authoring and Business-to-SQL conversion: every concrete satellite must reference a modeled common satellite, and every common satellite must have exactly one concrete specialization.
- The Business integration mesh's 44 payload steps now use the common attribute command. The migration used generic Meta instance operations because MetaMesh does not yet expose step edit/remove commands; no XML was edited.

## 4. Satellite Row Identity and Load Metadata

Status: Complete on 2026-07-19

Observed problem:

Generated Raw and Business Satellite tables were heaps. Latest-row lookups over the AdventureWorks data became extremely slow, and the physical model did not declare a row-identity/access contract for satellite history.

Decision:

`LoadTimestamp` and other standard physical satellite columns belong in the Raw/Business Data Vault implementation workspaces, not in the user-authored Raw/Business Data Vault models. Their SQL Server types, facets, constraints, and access structures are implementation concerns.

Every satellite table has the implementation-level identity `(ParentHashKey, LoadTimestamp)`. The parent hash key identifies the parent Hub, Link, Reference, Same-As Link, or Hierarchical Link row; the load timestamp identifies the historical version. The implementation workspace declares the physical primary-key name policy.

Implementation:

- Added required `PrimaryKeyNamePattern` to all seven Raw/Business satellite implementation entities. Default rows use `PK_{TableName}`.
- Preserved Raw satellite timestamp name/type requirements, which were already present.
- Made `LoadTimestampColumnName` and `LoadTimestampDataTypeId` required for the five Business satellite implementation entities. `LoadTimestampPrecision` and the default expression remain optional physical refinements.
- Updated Raw and Business SQL conversion so each satellite emits a composite primary key with `ParentHashKey` first and `LoadTimestamp` second.
- Left the Raw and Business author-intent models unchanged.

Verification:

- `MetaDataVault.Tests` passed 53/53. The conversion coverage proves all seven satellite families emit `PK_{TableName}` over the expected parent hash-key column and `LoadTimestamp` in that order.
- `meta check` passed for `MetaDataVaultImplementation`.
- The Raw mesh validated 137 steps and deployed 27 tables, 27 primary keys, and 25 foreign keys, then reported no verification changes and cleaned up 5/5.
- The Business mesh validated 148 steps and deployed 58 tables, 56 primary keys, and 61 foreign keys, then reported no verification changes and cleaned up 9/9.

## 5. Data Vault Hash Storage Width

Status: Approved and implemented on 2026-07-15 in commit `7dab49d`

Observed problem:

The first demo used MD5 and 16-byte hash storage. That is not an acceptable contemporary default for the intended public demo.

Attempted change:

- Changed Data Vault implementation instance hash-key lengths from 16 to 32 bytes.
- Considered changing demo transform hashing and a MetaSql alignment assertion in the abandoned slice, but those changes were not part of the approved final implementation.

Why it was attempted:

To align the physical storage contract with SHA-256. The hash algorithm remained authored transform logic; the implementation model only described storage width.

Decision:

The sanctioned Data Vault implementation workspace should use 32-byte binary storage for hash keys. The hash algorithm remains authored transform logic; the implementation model owns the physical storage width only. Updated all default Raw and Business Data Vault hash-key, parent-hash-key, root-hash-key, and related-hash-key implementation lengths from `16` to `32` using `meta instance update --strict`. Existing hash-difference lengths were already `32`. Added an implementation-workspace test that loads the authored workspace through generated tooling and verifies the default hash storage widths.

## 6. Business Datatype Lowering into SQL Server

Status: Complete on 2026-07-19; existing implementation and coverage verified

Observed problem:

Business Data Vault attributes carry logical datatype identifiers, while the SQL projection needs a sanctioned SQL Server datatype and facets. Existing paths mixed canonical lowering with compatibility behavior and contained incomplete optional implementation groups.

Attempted change:

- Reworked `SqlServerBusinessTypeLowering` and persistent Business Data Vault SQL projection.
- Propagated attribute nullability.
- Added stricter handling for incomplete optional datatype/detail groups.
- Updated persistent hubs, links, references, and link variants.

Why it was attempted:

To make SQL generation reject unsupported or half-configured type mappings instead of choosing a plausible physical type silently.

Decision required:

The sanctioned `MetaDataTypeConversion` workspace owns logical Business datatype to SQL Server datatype lowering. This is already implemented: `MetaConvert.DataVaultToSql.SqlServerBusinessTypeLowering` builds its mapping from `MetaDataTypeInstance.Default` and `MetaDataTypeConversionInstance.Default`, retaining only direct `Meta` to `SqlServer` mappings. `MetaBusinessDataVault` carries the logical datatype reference; conversion rejects conflicting mappings, unknown/non-Meta logical types, and a missing sanctioned direct SQL Server lowering rather than choosing a plausible physical datatype.

Verification on 2026-07-19: focused converter tests passed 3/3 for successful sanctioned Business lowering, missing direct lowering rejection, and rejection of a SQL Server typed value not sanctioned in `MetaDataType`.

## 7. Strict Mutation Target Binding

Status: Complete for the supported mutation syntax on 2026-07-16

Observed problem:

MetaTransformBinding skipped target-column validation when the final rowset was a mutation target. An `INSERT ... SELECT` could therefore pass strict binding without proving target-column mapping, required-column coverage, datatype compatibility, facets, or nullability.

Decision:

The final mutation target rowset is the table shape, not the values being written. The public bind flow derives every supported mutation effect from syntax, proves it against the target schema, and persists that proof before saving the binding workspace.

The binding model now holds the mutation facts explicitly, without a generic kind discriminator:

- `Write` relates one validated target rowset to a value-producing mutation effect.
- `WriteValue` relates each write value to its validated target column link.
- `WriteValueScalarExpression` records the source scalar expression for values introduced by `VALUES`, `UPDATE`, and `MERGE` actions.
- `InsertQueryWrite`, `InsertValuesWrite`, `UpdateWrite`, `MergeInsertWrite`, and `MergeUpdateWrite` identify the exact syntax construct that produced each write.
- `Delete`, `MergeDelete`, and `Truncate` identify effects that change rows without supplying values.
- Existing `TableSource`, `ColumnReference`, `ValidationSourceRowsetLink`, and `ValidationSourceColumnLink` retain resolved source-object and bound-column evidence.
- `ValidationTargetColumnTypeExact` or `ValidationTargetColumnTypeSanctionedConversion` records the proven compatibility result for every persisted write value.

Runtime type propagation and expression resolution remain transient binding work. The resulting source, target, write, delete, truncate, and compatibility facts are persisted as the binding result; the workspace does not depend on generated rowset names or runtime conventions to explain a mutation.

Implementation:

- `INSERT ... SELECT` validates positional values against an explicit target-column list or the writable target-field order.
- `INSERT ... VALUES` validates one values row, including literal type, length, precision, scale, and nullability checks.
- `UPDATE` validates only assigned target fields.
- `MERGE` validates every supported action independently: `UPDATE`, `INSERT`, and `DELETE`. It also binds the `ON` predicate and every optional `WHEN ... AND ...` predicate before saving the facts.
- `DELETE`, `MERGE ... DELETE`, and `TRUNCATE` validate the writable target contract and persist their exact syntax effect without inventing value-to-target mappings.
- Mutation `WHERE` predicates, `MERGE ... ON` conditions, and every `MERGE WHEN` condition retain resolved source reads as `ColumnReference` facts and resolved target reads as `TargetColumnReference` facts before a workspace is saved.
- A mutation value with no proven type now fails the public bind operation. It is not persisted as an unclassified successful result.
- `ValidationTargetColumnTypeNotClassified` was removed through `meta model drop-entity`, and its generated tooling and MetaPipeline consumer were regenerated/updated.
- The obsolete post-hoc binding-workspace validation service was removed. Strict mutation validation now runs where the transform syntax and both schema contracts are present.
- The old public schema-free `BindToWorkspace` API is gone. Its internal `BindStructureToWorkspace` replacement exists only for test fixtures that need rowset/name-resolution structure; without a target schema contract it deliberately does not emit strict mutation facts.

Bounded syntax:

Strict type proof currently covers direct column references and supported literals. Complex write expressions whose type is not yet established, and multi-row `INSERT ... VALUES`, fail explicitly rather than weakening the result. Those are clear syntax-support extensions, not hidden successful states.

Verification:

- Focused public-flow coverage proves persisted `INSERT ... SELECT`, `INSERT ... VALUES`, `UPDATE`, all three `MERGE` actions, `DELETE`, and `TRUNCATE` facts; the complete `MERGE` proof covers `MATCHED`, `NOT MATCHED BY TARGET`, `NOT MATCHED BY SOURCE`, and each `WHEN` predicate through save/reload. It also covers a qualified target-predicate read, mismatched source/target types, and an unresolved write expression that fails hard.
- `dotnet build MetaTransform\\Binding\\Cli\\MetaTransformBinding.Cli.csproj --nologo -m:1 -nr:false` passed with 0 warnings and 0 errors.
- `dotnet test MetaTransform\\Script\\Tests\\MetaTransformScript.Tests.csproj --nologo -m:1 -nr:false` passed 373/373.
- `dotnet test MetaPipeline\\Tests\\MetaPipeline.Tests.csproj --nologo -m:1 -nr:false` passed 69/69.
- `dotnet test MetaOrchestration\\Tests\\MetaOrchestration.Tests.csproj --nologo -m:1 -nr:false` passed 91/91.
- `dotnet test Tests\\TransformSurfaceContracts\\MetaBi.TransformSurfaceContracts.Tests.csproj --nologo -m:1 -nr:false` passed 14/14.

## 8. Transform Binding Bugs Independent of the Model Proposal

Status: Done on 2026-07-17

Closure:

All reported defects in this queue item are fixed or were disproven by focused public-flow coverage. There is no remaining item 8 repair work. Full SQL Server expression typing, including type precedence and implicit mixed-branch conversion, is deliberately a separate future feature.

Fixed candidate:

- Mutation statement CTE binding was fixed without changing the product model. `TransformScriptNavigator` now exposes CTEs from the active statement-level `StatementWithCtesAndXmlNamespaces`, and mutation binding initializes from that same statement-level CTE list instead of asking for a SELECT statement.
- Regression coverage: `BindInsertStatementWithCte_DerivesMutationSourceFromCte` proves `WITH src AS (...) INSERT ... SELECT ... FROM src` initializes `src` as a CTE rowset and uses it as the mutation source.
- Qualified wildcard projection through CTE and query-derived-table rowsets was verified as already supported by binding. Regression coverage: `BindQualifiedStarFromCommonTableExpression_DerivesCteColumns` and `BindQualifiedStarFromQueryDerivedTable_DerivesDerivedTableColumns`.
- Name-only source ambiguity is already covered by `ValidationService_WithAmbiguousOnePartSourceIdentifier_FailsHard`, which uses duplicate one-part table names in different schemas and fails with `SourceSchemaTableAmbiguous`.

Observed problems:

- Fixed: mutation statements lost statement-level CTE definitions because initialization searched only for a top-level SELECT.

  Example shape:

  ```sql
  WITH src AS
  (
      SELECT CustomerId
      FROM dbo.Customer
  )
  INSERT INTO dbo.CustomerStage (CustomerId)
  SELECT CustomerId
  FROM src;
  ```

  Expected behavior: `src` is initialized as a CTE rowset for the INSERT statement, and the mutation source binds to that rowset. This is now covered by a focused regression test. Follow-up, if needed, should add equivalent UPDATE, DELETE, or MERGE examples only if a concrete failure appears.

- Resolved by coverage: qualified wildcard projection through CTE and query-derived-table paths expands the exposed rowset columns.

  Reproduction shapes to prove or delete:

  ```sql
  WITH src AS
  (
      SELECT CustomerId, CustomerName
      FROM dbo.Customer
  )
  SELECT src.*
  FROM src;
  ```

  ```sql
  SELECT d.*
  FROM
  (
      SELECT CustomerId, CustomerName
      FROM dbo.Customer
  ) AS d;
  ```

  Expected behavior: `src.*` and `d.*` expand to the columns exposed by the CTE or derived-table rowset. This is covered by focused regression tests and did not require a binding fix.

- Resolved by existing coverage: name-only source matching is ambiguous when schemas contain duplicate object names.

  Reproduction shape to prove or delete:

  ```sql
  SELECT CustomerId
  FROM Customer;
  ```

  with a source schema workspace containing both `sales.Customer` and `crm.Customer`.

  Expected behavior: validation fails as ambiguous unless the transform qualifies the table enough to select one source. The alias used inside a query exposes a rowset; it is not treated as evidence that resolves a source-schema table. Existing coverage: `ValidationService_WithAmbiguousOnePartSourceIdentifier_FailsHard`.

- Resolved projection gap: a qualified target reference such as `dbo.Customer.CustomerId` in an `UPDATE` predicate is present in runtime binding, but cannot use the source-table `ColumnReference` entity. `TargetColumnReference` now preserves the exact syntax reference ID, resolved target rowset column, declared binding target, and resolved schema field after validation.

- Fixed candidate: explicit `CAST` and `CONVERT` mutation write expressions were structurally modeled but were not classified by the binding session. The binder now reads their modeled target `DataTypeReference`, including length, precision, and scale parameters, and derives conservative nullability from the source expression. The resulting type participates in the same strict compatibility proof as a direct source column; no binding product-model change was needed.

  Verified examples:

  ```sql
  UPDATE dbo.Customer
  SET Code = CAST(src.Code AS varchar(25))
  FROM dbo.CustomerStage AS src;
  ```

  ```sql
  UPDATE dbo.Customer
  SET Amount = CONVERT(decimal(18, 2), src.Amount)
  FROM dbo.CustomerStage AS src;
  ```

  Focused public-flow coverage proves successful exact binding, length/scale mismatch rejection, and nullable-source rejection for casts. `MetaTransformScript.Tests` passed 382/382 and the binding CLI build passed with 0 warnings and 0 errors.

- Fixed bounded policy: `CASE` mutation writes now resolve only when every `THEN` branch and any explicit `ELSE` resolve to the same stored datatype contract: the same MetaDataType id and the same length, precision, and scale. The branch expressions still bind normally, so direct column references, literals, explicit `CAST`/`CONVERT`, and nested bounded `CASE` expressions can supply the contract.

  Reproduction shape:

  ```sql
  UPDATE dbo.Customer
  SET StatusCode = CASE WHEN src.IsActive = 1 THEN src.ActiveCode ELSE src.InactiveCode END
  FROM dbo.CustomerStage AS src;
  ```

  A missing `ELSE` makes the result nullable. Any nullable or unknown-nullability branch is also conservatively treated as nullable. Mixed datatype or facet contracts remain an explicit strict failure (`MutationWriteValueTypeNotResolved`); the binder does not emulate SQL Server implicit conversion or type-precedence rules. Focused public-flow tests cover searched and simple `CASE` success, missing-`ELSE` rejection for a required target, and mixed `nvarchar`/`varchar` rejection.

Attempted change:

These fixes were implemented inside the same large slice as the unapproved MetaTransformBinding model expansion.

Why it was attempted:

The AdventureWorks transforms and the TPC-DS corpus exposed them while strict mutation validation was being hardened.

Decision:

Each behavior was reproduced or proven by focused public-flow coverage and repaired within the existing syntax and binding boundary. No product-model change was required. Future full SQL Server precedence work is a separately scoped expression-semantics feature, not unfinished queue item 8 work.

## 9. TPC-DS MetaTransformScript Round Trip

Status: Decided and complete on 2026-07-19

Purpose:

The TPC-DS corpus proves the bounded `SQL -> MetaTransformScript -> semantically equivalent SQL` round trip across a substantial curated set of SQL Server views, and that those modeled transforms bind strictly to their modeled schema contract.

Scope:

- Import the selected views into `MetaTransformScript`.
- Bind the transforms against the declared TPC-DS `MetaSchema` workspace.
- Emit SQL from the modeled syntax.
- Re-import the emitted SQL and require an exact final `MetaSql` workspace diff.

Schema contract construction:

1. Deploy the TPC-DS query files as SQL Server views.
2. Extract their contracts with `meta-schema`.
3. Change the extracted `tpcds.v_qNN` schema rows from views to tables, preserving their identifiers and fields, so strict binding can validate them as writable transform targets.
4. Check in that derived `SchemaWS` alongside the corpus.

The mesh consumes the checked-in derived contract offline. It does not deploy the views or perform schema extraction at run time.

Out of scope:

- Proving the SQL Server extraction operation itself.
- Data Vault or AdventureWorks full-stack design.

Verification:

Fresh verification on 2026-07-19: `build-tpc-ds-snapshot` completed 108/108 in 243 seconds, including strict binding of all 99 scripts. The expected syntax-graph/provenance diff contained 5,032 rows on each side; the two projected `MetaSql` workspaces had 101 rows and 399 properties each, with zero differences. Modeled cleanup then completed 9/9. Keep future corpus changes scoped to syntax round-trip fidelity and binding against the declared schema contract; add a separate demo if a different product boundary needs proving.

## 10. AdventureWorks Full-Stack Consequences

Status: Inactive; no longer blocked by items 4 through 9

The `Demos/AdventureWorksFullStack` authoring work is retained as a demo design, not as sanctioned end-to-end proof. Its generated contracts and operations assume:

- 32-byte Data Vault hashes,
- source attribute nullability,
- satellite primary keys/load metadata,
- strict mutation target binding,
- and datatype compatibility behavior.

Those product decisions are now settled. The demo remains inactive until it is replayed from clean workspaces and its current outputs are reviewed as one deliberate full-stack proof.

## 11. MERGE Match Semantics

Status: Complete on 2026-07-16

Resolution:

`MergeWhenClause.MatchKind` and `MergeStatementWhenClausesItem.Ordinal` were removed from the sanctioned `MetaTransformScript` model. The three SQL Server match forms are now explicit entities:

- `MergeMatchedWhenClause`
- `MergeNotMatchedByTargetWhenClause`
- `MergeNotMatchedBySourceWhenClause`

Each concrete entity relates to the common `MergeWhenClause` action and optional search-condition structure. `MergeStatementWhenClausesItem` now uses `PreviousMergeWhenClause`, so source-order is an explicit relationship rather than a scalar implementation value.

The parser creates the concrete form, the emitter and binding navigator require exactly one form, and both validate the predecessor chain for a single head, no cross-statement link, no branch, no cycle, and no unreachable clause. Focused tests cover all three forms, multiple `WHEN MATCHED` clauses, binding persistence, malformed branching, and workspace save/reload.

The companion SQL export repair orders modules by modeled SQL identity (`schema.object`) and emits semantic paths such as `views/dbo/v_example.sql`; it no longer derives deploy order or file names from physical list insertion. The reference corpus and TPC-DS meshes re-import those paths through checked-in manifests and prove the final `MetaSql` workspaces are identical. `DeployOrdinal` is now a stable projection value only. Module references remain resolved by strict binding against the modeled schema contracts; the lexical export order is not used as a dependency rule.

## Rollback Scope

The rollback restores the attempted implementations and generated artifacts under:

- `MetaDataTypeConversion`
- `MetaDataVault`
- the affected Data Vault files under `MetaConvert`
- `MetaTransform`
- affected Business/Raw Data Vault integration workspaces
- the changed TPC-DS schema fixture

The deleted legacy AdventureWorks demo, the new `AdventureWorksFullStack` source files, unrelated documentation work, and unrelated repository changes are not part of this rollback.

The Data Vault hash storage portion was reviewed separately and resolved in item 5.
