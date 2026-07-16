# Product Model Review Queue

## At A Glance

1. Done: SQL Server `decimal`/`numeric` compatibility.
2. Approved model, audit still open: MetaSchema and Raw Data Vault ownership. The model direction is accepted, but the implementation audit findings must be handled one at a time.
3. Pending product-model decision: Business Data Vault nullability and roles.
4. Pending product-model decision: satellite row identity and load metadata.
5. Done: Data Vault hash-key storage width.
6. Pending product-model decision: business datatype lowering.
7. Complete for supported mutation syntax: persisted, validated facts for every supported mutation form, including `MERGE` actions and conditions.
8. Partly done: mutation CTE binding and mutation-predicate traversal now preserve source and target reads; wildcard and one-part ambiguity reports resolved by coverage. Broader expression output typing remains a separate binder capability.
9. Separate scope: TPC-DS fixture/corpus strategy. Do not mix it into Data Vault, binding, or AdventureWorks decisions.
10. Blocked demo work: AdventureWorks full-stack should not continue until the relevant product decisions are settled and reimplemented deliberately.
11. Complete: `MERGE` match semantics use explicit clause entities and an explicit predecessor chain.

Safest next work: pick one item 2 audit finding or one remaining item 8 proof target. Do not change sanctioned product models without an explicit decision.

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

Status: Model approved on 2026-07-15; implementation audit open

Observed problem:

The old MetaSchema represented tables/views, key categories, and field nullability as free-text discriminators or boolean-like text. MetaRawDataVault then copied the complete source system/schema/table/field/relationship graph and referred back to it. Raw link endpoint meaning was stored in `RawLinkHub.RoleName`.

Attempted change:

- Tried to make the copied source-field nullability and link role text required.
- Propagated those values through schema-to-vault and vault-to-SQL conversion.
- Regenerated tooling and affected sample/integration workspaces before the model ownership problem was reviewed.

Why it was attempted:

To make the generated physical contract deterministic, but the attempt preserved the wrong ownership boundary and still encoded modeled concepts as text.

Approved decisions:

- MetaSchema now models a common `SchemaObject` specialized by `Table` or `View`.
- MetaSchema owns observed source nullability through the presence marker `FieldIsNullable`. A row means the related `Field` is nullable; absence means it is not nullable. There is no complementary `NonNullableField` entity and no scalar boolean-like property.
- A common `TableKey` is specialized by exactly one `PrimaryKey` or `UniqueKey`.
- MetaRawDataVault models only the Raw Data Vault. The copied `SourceSystem`, `SourceSchema`, `SourceTable`, `SourceField`, and source relationship graph is removed.
- MetaRawDataVault owns independent `Field` and `FieldDataTypeDetail` entities. Hub key parts and satellite attributes relate to those fields.
- `RawLinkRole` is a first-class endpoint relationship with a required `Name`; `RawLinkHub` and `RoleName` are removed.
- Hub key-part order is modeled by `PreviousKeyPart`, not by an ordinal scalar.
- `Kind`, `Is*`, `ObjectType`, `KeyType`, and equivalent free-text discriminators are not part of the approved MetaSchema or MetaRawDataVault models.

Resolved decision:

MetaRawDataVault does not model source or physical nullability on `Field`. Schema-to-Raw conversion does not copy MetaSchema nullability into the Raw model. Raw-to-SQL projection derives the physical contract from Raw usage: hub business-key parts are non-nullable, while hub- and link-satellite payload attributes are nullable.

The Raw authoring surface now has one `add-field` command. It creates the neutral field and optional datatype detail; it does not ask the caller to author a nullability classification that the Raw model does not own.

Cross-model blast radius:

- `MetaSchema` owns observed source nullability through `FieldIsNullable`. Extraction emits the marker only for nullable source columns, and binding interprets marker presence as nullable.
- `MetaSql.TableColumn` owns physical SQL nullability but still stores it in the scalar `IsNullable` property. That is a separate product-model review, not a reason to keep nullability on a Raw field.
- `MetaTransformScript.StoredProcedureResultColumnItem` also stores optional nullability in scalar `IsNullable`; its observed/unknown result-contract semantics require a separate review.
- `MetaDataWarehouse.DimensionAttribute`, `MetaDataWarehouse.FactMeasure`, `MetaDataWarehouseImplementation.PlatformColumnImplementation`, `MetaAnalytics.Attribute`, and `MetaTabular.TabularColumn` carry the same scalar smell. Review each domain before changing it; do not mechanically propagate the MetaSchema shape.
- `MetaTransformBinding` does not own a competing nullability model in this slice. It consumes `MetaSchema` source and target contracts, so its adaptation to the approved MetaSchema structure is expected.

Implementation note:

Nine persisted MetaSchema workspaces and five persisted MetaRawDataVault workspaces were migrated through sanctioned `meta` commands, including each canonical product workspace. Generated tooling, the Raw CLI and service, Schema-to-Raw conversion, Raw-to-SQL projection, binding, fixtures, and tests are aligned with the approved models. The generated tooling copies carry the same model structure; their serialized `model.xml` omits only the canonical workspace's XML declaration.

Implementation audit findings:

- SQL Server extraction currently replaces the established `:table:` identity segment with `:object:` for schema objects and every dependent field, key, and relationship Id. An unchanged source sync would therefore create broad identity churn.
- Identity metadata exposed through a SQL Server view was previously retained on its fields; the new extractor emits `IdentityField` only for tables even though write-target rejection is already handled by binding.
- The public `schema-to-raw-datavault --include-views` behavior was removed. The current converter always consumes `TableList` and ignores `ViewList`.
- `ValidationSourceRowsetLink.MetaSchemaTableId` and `ValidationTargetRowsetLink.MetaSchemaTableId` now receive `SchemaObject.Id`. This only appears correct in workspaces that deliberately reuse the same Id for specialization and object rows; combined binding workspaces already give those rows distinct Ids.
- Raw link roles are represented correctly as named relationship entities, but the removal of endpoint ordinals changed Raw-to-SQL physical column order to lexical role-name order. The checked-in Raw demo changes order for three of six links. Decide whether roles are an unordered named set with canonical lexical projection or whether endpoint order is domain truth.
- The sanctioned migration path removed final newlines from 39 of the 50 changed XML files. Fix the shared writer rather than hand-editing generated workspaces.

## 3. Business Data Vault Attribute Nullability and Link Roles

Status: Pending discussion

Observed problem:

Five Business Data Vault satellite-attribute families had no nullability contract, while physical SQL generation must decide whether each emitted column is nullable. `BusinessLinkHub.RoleName` was optional even though endpoint roles are used in physical names.

Attempted change:

- Added `IsNullable` to hub, link, reference, same-as-link, and hierarchical-link satellite attributes.
- Made `BusinessLinkHub.RoleName` required.
- Added matching CLI options and handler plumbing.
- Propagated the values through SQL conversion.
- Regenerated tooling, CLI workspaces, samples, and integration fixtures.

Why it was attempted:

The AdventureWorks binding gate exposed a nullable source attribute being projected into a target whose requiredness could not be represented honestly by the Business Data Vault model.

Decision required:

Decide whether nullability belongs on these logical attributes, on an implementation mapping, or elsewhere. Decide separately whether endpoint roles are universally required or only required when a physical naming pattern consumes them.

Role direction approved on 2026-07-15: role meaning must be represented by entities and relationships, not a free-text `RoleName` property. No MetaBusinessDataVault model or implementation change was made in item 2; the exact Business Data Vault role entities and migration remain part of this separate review item.

## 4. Satellite Row Identity and Load Metadata

Status: Pending discussion

Observed problem:

Generated Raw and Business Satellite tables were heaps. Latest-row lookups over the AdventureWorks data became extremely slow, and the physical model did not declare a row-identity/access contract for satellite history.

Attempted change:

- Added `PrimaryKeyNamePattern` to raw hub/link satellite implementations and all business satellite implementation families.
- Emitted composite satellite primary keys over parent hash key plus load timestamp.
- Made the business satellite load-timestamp column, datatype, and precision fields required.
- Hardened validation for partially configured implementation-column groups.
- Updated implementation instances and converter tests.

Why it was attempted:

To give satellite history a declared physical key and make replay/latest-row access practical rather than relying on heap scans.

Decision required:

Confirm the intended satellite row identity, including whether `(ParentHashKey, LoadTimestamp)` is sufficient, whether the key is a product requirement or a SQL Server implementation policy, and whether business satellite load metadata must always be present.

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

Status: Pending discussion

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

Review this separately from the Data Vault schema changes. Establish which workspace owns logical-to-platform lowering and what constitutes a complete implementation mapping before restoring converter behavior.

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

Status: Pending review as separate bug fixes; mutation statement CTE binding fixed on 2026-07-16; wildcard and one-part source ambiguity reports resolved by coverage on 2026-07-16

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

- Unverified report: expression outputs may lack enough known type information for strict target validation.

  Reproduction shapes to prove or delete independently:

  ```sql
  SELECT CAST(src.Code AS varchar(25)) AS Code
  FROM dbo.Source AS src;
  ```

  ```sql
  SELECT CONVERT(decimal(18, 2), src.Amount) AS Amount
  FROM dbo.Source AS src;
  ```

  ```sql
  SELECT CASE WHEN src.IsActive = 1 THEN src.ActiveCode ELSE src.InactiveCode END AS StatusCode
  FROM dbo.Source AS src;
  ```

  Expected behavior: only expressions whose type is explicit or safely derivable should participate in strict type/length/precision/scale validation. Expressions whose type cannot be known should remain not-classified rather than guessing. This is likely several separate bugs or policy decisions, not one fix.

Attempted change:

These fixes were implemented inside the same large slice as the unapproved MetaTransformBinding model expansion.

Why it was attempted:

The AdventureWorks transforms and the TPC-DS corpus exposed them while strict mutation validation was being hardened.

Decision required:

Reproduce and fix each behavior independently where possible. A parser/binder bug does not automatically justify a product-model change.

## 9. TPC-DS Binding Fixture and Corpus Expansion

Status: Pending discussion

Observed problem:

The checked-in synthetic TPC-DS schema fixture did not accurately represent the SQL Server view contracts used by the corpus, which complicated strict binding evidence.

Attempted change:

- Replaced the fixture with a much larger SQL Server-extracted schema snapshot.
- Expanded corpus validation and related hardening tests.
- Added datatype synonym handling to make more views pass strict validation.

Why it was attempted:

To use the TPC-DS corpus as broad pressure on TransformBinding while the AdventureWorks issue was being investigated.

Decision required:

This was the wrong task scope. Decide separately what the TPC-DS demo is intended to prove, whether its schema should remain synthetic, and which statements belong in a sanctioned SQL Server corpus.

## 10. AdventureWorks Full-Stack Consequences

Status: Blocked by the decisions above

The new `Demos/AdventureWorksFullStack` authoring work is retained because it is the active demo design, not a sanctioned product-model change. It is not currently valid end-to-end proof after this rollback. Its generated contracts and operations assume some combination of:

- 32-byte Data Vault hashes,
- source and business attribute nullability,
- satellite primary keys/load metadata,
- strict mutation target binding,
- and datatype compatibility behavior.

Do not continue the demo factory until the relevant product decisions have been reviewed and reimplemented deliberately.

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
