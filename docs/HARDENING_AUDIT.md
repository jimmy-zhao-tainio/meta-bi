# meta / meta-bi hardening audit

Date: 2026-05-23

Scope:
- upstream foundation repo: `../meta`
- BI sanctioned model repo: `meta-bi`

This audit treats `meta` as a representation-symmetric executable model foundation, not as descriptive annotations. The accepted model state must remain explicit, deterministic, inspectable, diffable, and generatable. Fuzzy discovery, mining, and agent interpretation can exist upstream as acquisition aids, but they must not become the core accepted model contract.

## 1. Overall architecture reading

The two repositories now implement a credible model-first BI stack.

`meta` owns the generic workspace contract, model and instance integrity, refactoring, diff/merge, import/generate paths, generated C# tooling, SQL generation, and cross-model binding through `MetaWeave`. Its current docs still occasionally call XML canonical. Architecturally, XML should be described as one deterministic workspace surface. The product thesis is representation symmetry: XML, SQL, C#, and future forms should carry the same structure without semantic drift.

`meta-bi` builds sanctioned BI model families over that foundation. The ladder is now visible:

```text
source/database shape
  -> MetaSchema / MetaDataType / MetaDataTypeConversion
  -> MetaTransformScript / MetaTransformBinding
  -> MetaDataQuality
  -> MetaDataVault / MetaDataWarehouse
  -> MetaSql deployment
  -> MetaPipeline operational execution
  -> MetaOrchestration graph/run planning
  -> MetaAnalytics
  -> MetaTabular / MetaMultiDimensional deploy targets
```

The strongest current pattern is that most areas now have:
- a sanctioned `model.xml`
- generated tooling
- a CLI surface
- some sample workspace
- at least one conversion or deployment path

The main hardening risk is no longer vague scope. The risk is target fidelity: modeled rows exist but target generators still omit or default important target settings, and some areas have evidence/candidate models without a clearly separated accepted product model. The biggest product-blocking class is "intent is present or implied, but the generated target either cannot express it, silently defaults it, or has no validation that the target will behave as intended."

## 2. Inventory

### Models

| Model | Location | Purpose | Current maturity | Main hardening theme |
|------|----------|---------|------------------|----------------------|
| Meta workspace/model/instance | `../meta/Meta` | Generic representation-symmetric metadata foundation, validation, refactor, diff/merge, import/generate. | Mature foundation with known C#/SQL symmetry debt. | Remove XML-canonical wording, finish natural C#/SQL truth surfaces, strengthen model/instance diff and generator invariants. |
| Instance diff alignment | `../meta/Meta/Workspaces/InstanceDiff.Alignment` | Explicit alignment for aligned instance diff/merge. | Focused baseline. | Reviewability and model-evolution coverage. |
| MetaWeave | `../meta/MetaWeave` | Cross-model property binding, validation, materialization. | Useful but narrow. | Decide whether it is the sanctioned cross-workspace reference mechanism for full BI ladder. |
| MetaDataType | `MetaDataType/Workspace` | Sanctioned semantic data type systems and types. | Small stable seed. | Expressiveness for precision/scale/length families and target capabilities. |
| MetaDataTypeConversion | `MetaDataTypeConversion/Workspace` | Source/target type conversion policy. | Useful baseline. | Conversion coverage, validation, and target-system selection. |
| MetaSchema | `MetaSchema/Workspaces/MetaSchema` | Imported source/live schema shape. | SQL Server table/view/key baseline. | SQL Server schema coverage and source adapter boundary. |
| MetaSql | `MetaSql/Workspace` | Target-neutral-ish SQL Server deployment shape for generated tables, columns, keys, indexes. | Strong bounded SQL Server table model. | Missing DDL coverage and deterministic migration review. |
| MetaSqlDeployManifest | `MetaSql/DeployManifest/Workspace` | Reviewable deploy manifest between source model and live SQL Server. | Strong add/drop/alter/block baseline. | More operations, clearer review, no silent unsupported changes. |
| MetaTransformScript | `MetaTransform/Script/Workspaces/MetaTransformScript` | Bounded SQL Server statement syntax model for SELECT and DML statements. | Broad and serious, still bounded. | Parser/emitter completeness, explicit unsupported syntax, profile extraction. |
| MetaTransformBinding | `MetaTransform/Binding/Workspaces/MetaTransformBinding` | Binds transform syntax to schema rowsets, targets, output rowsets, type checks. | Good first semantic layer. | Bind all statement effects and emit minimal profiles for pipeline/orchestration. |
| MetaDataQuality | `MetaDataQuality/Workspaces/MetaDataQuality` | DQ candidate/evidence model from transform corpus plus promoted checks. | Evidence-rich, accepted-rule boundary weaker. | Separate mined candidates from explicit accepted DQ rules and generated checks. |
| MetaRawDataVault | `MetaDataVault/Workspaces/MetaRawDataVault` | Raw Data Vault logical model from source schema. | Good sanctioned slice. | Source-to-vault coverage and raw load SQL fidelity. |
| MetaBusinessDataVault | `MetaDataVault/Workspaces/MetaBusinessDataVault` | Business Data Vault logical model. | Broad logical model. | Validation for PIT/bridge/link variants and implementation projection. |
| MetaDataVaultImplementation | `MetaDataVault/Workspaces/MetaDataVaultImplementation` | SQL implementation policy for Data Vault tables. | Strong naming/platform-column baseline. | Target SQL coverage and operational column invariants. |
| MetaDataWarehouse | `MetaDataWarehouse/Workspaces/MetaDataWarehouse` | Logical Kimball-style DW model. | Good early model. | Grain/SCD/bridge validation and target realization fidelity. |
| MetaDataWarehouseImplementation | `MetaDataWarehouse/Workspaces/MetaDataWarehouseImplementation` | SQL implementation policy for DW tables. | Practical baseline. | More physical options without leaking implementation into logical model. |
| MetaAnalytics | `MetaAnalytics/Workspaces/MetaAnalytics` | Conceptual analytics model shared by tabular and multidimensional targets. | Good common surface. | Clean boundary between portable analytics and target-specific semantics. |
| MetaTabular | `MetaTabular/Workspaces/MetaTabular` | SSAS tabular target implementation model. | Deployable first slice. | TOM coverage, target settings, credential/partition/process policy. |
| MetaMultiDimensional | `MetaMultiDimensional/Workspaces/MetaMultiDimensional` | SSAS multidimensional target implementation model. | Deployable first slice. | AMO cube/dimension/processing coverage, large-dimension and storage options. |
| MetaPipeline | `MetaPipeline/Workspaces/MetaPipeline` | Serial execution of bound transform tasks plus operational evidence DB. | Solid local SQL Server runtime slice. | Runtime policy, source adapters, operational evidence validation. |
| MetaOrchestration | `MetaOrchestration/Workspaces/MetaOrchestration` | Inferred task graph, policy rows, run planning, local process execution. | Strong first orchestration slice. | Failure semantics, resource policy, run review, future distributed execution. |
| MetaBusiness | `MetaBusiness` | Future low-level business model. | Placeholder. | Either park explicitly or define first serious bounded scope. |

### CLIs

| CLI / command group | Location | Purpose | Current maturity | Main hardening theme |
|---------------------|----------|---------|------------------|----------------------|
| `meta` | `../meta/Meta/Cli` | Workspace/model/instance operations, refactor, diff/merge, import/generate. | Mature, wide. | Representation-symmetric wording, exit/output consistency, stronger SQL/C# truth surfaces. |
| `meta-weave` | `../meta/MetaWeave/Cli` | Cross-model binding authoring, validation, materialization. | Focused baseline. | Clarify role in full ladder and add review/diff flows. |
| `meta-schema` | `MetaSchema/Cli` | Extract SQL Server schema into MetaSchema. | Useful SQL Server baseline. | Wider SQL Server schema coverage and adapter boundary. |
| `meta-data-type` | `MetaDataType/Cli` | Create sanctioned type workspace. | Minimal. | Inspect/list/check commands and target capability visibility. |
| `meta-data-type-conversion` | `MetaDataTypeConversion/Cli` | Create/check/resolve type conversion policy. | Useful. | Coverage, diagnostics, conversion matrix review. |
| `meta-sql` | `MetaSql/Cli` | SQL Server deploy-plan/deploy/execute. | Strong deploy baseline. | More DDL operations, dry-run/review output, plan inspection. |
| `meta-datavault-raw` | `MetaDataVault/Cli/Raw` | Author raw DV logical model. | Broad generated add surface. | Validation and guided authoring beyond generic add rows. |
| `meta-datavault-business` | `MetaDataVault/Cli/Business` | Author business DV logical model. | Broad generated add surface. | PIT/bridge/link validations and concept-level diagnostics. |
| `meta-transform-script` | `MetaTransform/Script/Cli` | Import/export supported SQL syntax model. | Broad. | Corpus performance, unsupported syntax diagnostics, stored-procedure behavior declarations. |
| `meta-transform-binding` | `MetaTransform/Binding/Cli` | Bind transform scripts to schema and target contracts. | Good. | Bind effect profiles, type conversion review, strict unbound failure. |
| `meta-data-quality` | `MetaDataQuality/Cli` | Mine/inspect/promote DQ candidates and convert to SQL. | Candidate workflow baseline. | Accepted DQ rule authoring and check lifecycle. |
| `meta-pipeline` | `MetaPipeline/Cli` | Author/execute serial transform pipelines and operational DB. | Solid local runtime. | Source adapters, evidence inspection, failure policy hardening. |
| `meta-orchestration` | `MetaOrchestration/Cli` | Infer graph, resolve policy, plan and execute pipelines. | Strong first graph/runtime slice. | Run-plan semantics, failure dependencies, resource policy, better inspect output. |
| `meta-data-warehouse` | `MetaDataWarehouse/Cli` | Author logical DW model. | Good first logical surface. | Validation and guided DW authoring. |
| `meta-analytics` | `MetaAnalytics/Cli` | Author shared analytics model. | Good first conceptual surface. | Concept validation and target conversion previews. |
| `meta-convert` | `MetaConvert/Cli` | Convert between sanctioned model families. | Central and expanding. | Per-converter fidelity manifests and unsupported concept reporting. |
| `meta-tabular` | `MetaTabular/Cli` | Author/deploy/restore/drop tabular target workspaces. | Deployable first slice. | Target setting coverage and process/backup operational validation. |
| `meta-multi-dimensional` | `MetaMultiDimensional/Cli` | Author/deploy/restore/drop multidimensional target workspaces. | Deployable first slice. | AMO target fidelity, large-model settings, process diagnostics. |
| `install-meta-bi` | `MetaInstaller/Installer` | Offline package/install surface. | Practical. | Reproducible release path and PATH validation. |

## 3. Per-model hardening plans

### Model: Meta workspace/model/instance

Role in ladder:
- Foundation for all sanctioned models.
- Owns generic model/entity/property/relationship/instance integrity and representation projection.

Current support:
- Workspace load/save, validation, model mutation, instance mutation, refactor, import SQL, CSV import/export, generate SQL/C#/SSDT, instance diff/merge and aligned diff/merge.
- Generated tooling performance has improved with split load/save and lazy indexes.

Gaps:
- Docs still contain XML-canonical phrasing in places.
- C# representation remains an architectural watch item: natural object references must remain primary, and public relationship transport ID properties must not come back.
- SQL representation is useful but not yet treated as an equal operational authoring surface across the BI ladder.
- Generic DDL generation in upstream docs is called out as string-assembled and needing stronger object-model support.

Ordered hardening work:
1. P0: Lock down generated C# relationship contract with golden generated POCO tests across role relationships, optional relationships, wrong-type references, out-of-model references, load/save, and C#-as-truth reflection.
2. P0: Remove or correct remaining XML-canonical language. XML is deterministic storage, not semantic authority over SQL or C#.
3. P1: Harden `meta generate sql` and `meta import sql` round-trips for model/instance structures that appear in sanctioned BI workspaces.
4. P1: Promote a minimal DDL object model upstream where generic SQL generation still builds raw strings.
5. P2: Add model/instance review manifests that summarize structural changes without forcing users to inspect raw XML.

Suggested tests:
- Generated C# object-reference API tests.
- `XML -> C# -> XML`, `SQL -> workspace -> SQL`, and mixed-surface round-trip fixtures.
- Diff/merge precondition and postcondition tests over role relationships.

Affected generators/targets:
- All generated tooling in both repos.
- Upstream SQL/C#/SSDT generation.
- All downstream `meta-bi` CLIs that rely on generated tooling.

### Model: MetaWeave

Role in ladder:
- Cross-model binding and materialization mechanism.
- Potential bridge for explicit references across separately authored workspaces.

Current support:
- Model aliases, property bindings, validation, suggestion, materialization.

Gaps:
- Role in `meta-bi` is not yet explicit. Cross-workspace references currently appear domain-specific and path/env based.
- Binding output is useful but not yet a full-ladder review artifact.

Ordered hardening work:
1. P1: Decide whether MetaWeave is the sanctioned cross-workspace reference mechanism for model families such as Analytics -> Tabular and Pipeline -> TransformBinding.
2. P1: Add tests showing materialized binding rows remain representation-symmetric and deterministic.
3. P2: Add inspect/diff output explaining binding coverage and unresolved references.
4. P4: Document when to use MetaWeave instead of domain-owned relationships or conversion.

Suggested tests:
- Binding suggestions over ambiguous and role-scoped references.
- Materialization round-trip tests over generated tooling.

Affected generators/targets:
- Future cross-model validation.
- Potential full-ladder workspace packaging.

### Model: MetaDataType

Role in ladder:
- Shared semantic data type vocabulary.
- Input to schema import, transform binding, DW/DV models, analytics measures, and SQL/SSAS type projection.

Current support:
- `DataTypeSystem` and `DataType` with `Name`, `Category`, `IsCanonical`, and description.

Gaps:
- Type facets such as length, precision, scale, collation, Unicode, signedness, date precision, and binary length live in local detail rows elsewhere rather than in a strong reusable capability model.
- No explicit target capability model for "this target supports these canonical/facet combinations."

Ordered hardening work:
1. P0: Define whether facets belong in `MetaDataType`, target conversion detail rows, or schema/local implementation details. Keep semantic type clean, but make the boundary explicit.
2. P1: Add target capability or validation rows for SQL Server, Tabular, and Multidimensional type projection.
3. P2: Add inspect/check output that explains unsupported or lossy type mappings.
4. P4: Provide examples for semantic type -> SQL Server -> Tabular/Multidimensional mappings.

Suggested tests:
- Decimal, money, datetime2, date, time, binary, rowversion, Unicode string, and large string conversion fixtures.
- Unknown target type system failure tests.

Affected generators/targets:
- MetaSchema extraction.
- MetaTransformBinding type validation.
- MetaSql, DataVault, DataWarehouse, Analytics, Tabular, Multidimensional converters.

### Model: MetaDataTypeConversion

Role in ladder:
- Explicit policy for converting source data types to target systems.

Current support:
- `DataTypeMapping` with source/target ids and `ConversionImplementation`.
- CLI `check` and `resolve`.
- Used by binding and pipeline `InsertRows`.

Gaps:
- Current mapping keys are source/target type ids but do not deeply model facets or target context.
- No obvious review matrix for all source types against all target systems.
- Conversion implementation is named, but generated target code may not always prove it applied the intended conversion.

Ordered hardening work:
1. P0: Validate duplicate and incomplete mappings per target data type system including details needed for SQL Server bulk insert and target DDL.
2. P1: Add facet-aware conversion cases where length/precision/scale change target behavior.
3. P1: Add conversion coverage from Meta-system semantic types to SQL Server, Tabular, and Multidimensional target data types.
4. P2: Add `inspect-matrix` or equivalent output for operators and agents.

Suggested tests:
- Exact, sanctioned conversion, not-classified, and unsupported conversion cases.
- Target-system disambiguation tests.
- Binding and pipeline tests that verify a selected conversion workspace changes behavior.

Affected generators/targets:
- MetaTransformBinding.
- MetaPipeline `InsertRows`.
- DataVault/DataWarehouse to MetaSql.
- Analytics to Tabular/Multidimensional.

### Model: MetaSchema

Role in ladder:
- Imported source/live schema state for source systems and target validation.

Current support:
- SQL Server extractor for systems, schemas, tables/views, fields, type details, primary/unique keys, and trusted foreign keys.
- Extracts identity metadata and view columns.

Gaps:
- SQL Server schema coverage is table/key centered. Defaults, computed definitions, check constraints, unique constraints as separate objects, indexes, filtered indexes, extended properties, temporal, columnstore, compression, partitioning, collation, and security are not represented here.
- No non-SQL Server source adapter boundary yet, though future adapters should only need "select columns and types" for extract landing.

Ordered hardening work:
1. P0: Add required SQL Server source/target schema features that affect generation and validation: defaults, computed columns, unique constraints, check constraints, column collation, and indexes.
2. P0: Define the light adapter contract for non-SQL sources: column projection plus type schema, not feature-complete external engines.
3. P1: Align MetaSchema and MetaSql coverage so extracted SQL Server schema can be projected to deployment shape without semantic loss for supported features.
4. P2: Add extraction review output showing skipped SQL Server features explicitly.

Suggested tests:
- SQL Server extractor fixtures for computed columns, defaults, unique constraints, check constraints, views, trusted/untrusted FKs, identity, type details.
- "unsupported feature detected" tests that do not silently drop serious target behavior.

Affected generators/targets:
- MetaTransformBinding.
- MetaDataQuality.
- MetaDataVault raw conversion.
- MetaSql live diff/deploy.

### Model: MetaSql

Role in ladder:
- Realization model for SQL Server structures generated by sanctioned models.
- Source input to deploy planning.

Current support:
- Database, schema, table, columns, type details, identity, computed/default expressions, primary keys, foreign keys, rowstore indexes, filtered indexes.
- Deploy manifest supports add/drop/alter/truncate/replace/block rows.

Gaps:
- Missing SQL Server DDL coverage: check constraints, unique constraints as first-class objects, default constraint names, column collations, sparse, rowguidcol, sequences, schemas permissions, compression, partitions, filegroups, columnstore indexes, indexed views, temporal tables, statistics, and triggers.
- Deploy planning blocks some unsupported changes, but model coverage limits what can be planned.
- Some target enum parsing/defaulting patterns elsewhere show the need for "invalid modeled target value fails" discipline here too.

Ordered hardening work:
1. P0: Add first-class check constraints and unique constraints because they affect data integrity and deploy safety.
2. P0: Add validation that computed/default expressions and constraint dependencies are preserved or explicitly blocked during deploy.
3. P1: Add compression, columnstore, partitioning, and filegroup support as target-specific implementation metadata.
4. P1: Add reviewable deploy-plan summaries independent of CLI success prose.
5. P2: Add "unsupported live feature detected" rows so deploy-plan does not appear complete when live has meaningful unmanaged features.

Suggested tests:
- Golden `MetaSql -> deploy SQL` tests for every supported DDL element.
- Live extraction -> diff -> manifest -> deploy -> re-extract no-diff tests.
- Negative deploy tests for unsupported constraints and destructive changes.

Affected generators/targets:
- DataVault to SQL.
- DataWarehouse to SQL.
- MetaSql deployment.
- Future SQL Server target adapter.

### Model: MetaSqlDeployManifest

Role in ladder:
- Reviewable deployment plan between modeled SQL and live SQL Server.

Current support:
- Add/drop/alter/replace/block actions with source/live fingerprints and exact destructive approvals.

Gaps:
- The manifest is action-rich but still not an operator-friendly review document by itself.
- Some blocked cases are broad "unsupported in this slice" rather than modeled target limitations.
- Full-database scope is deliberate today, but this needs clearer production rollout strategy when databases become large.

Ordered hardening work:
1. P0: Ensure every non-additive target change either has a precise modeled action or a precise block reason.
2. P1: Add manifest inspection that groups changes by risk and affected object.
3. P1: Add deterministic generated SQL preview from the manifest.
4. P2: Add deployment dry-run validation that verifies live fingerprints and action ordering without applying.

Suggested tests:
- Manifest action ordering golden tests.
- Exact approval tests for destructive table/column/truncation actions.
- Block reason golden output tests.

Affected generators/targets:
- `meta-sql deploy-plan`.
- `meta-sql deploy`.
- DataVault and DW demos.

### Model: MetaTransformScript

Role in ladder:
- Syntax model for supported SQL Server transform statements.
- Feeds binding, pipeline execution, data-quality analysis, and orchestration access profiles.

Current support:
- Broad SQL Server SELECT/view/inline TVF/scalar UDF surface plus DML: INSERT, UPDATE, DELETE, TRUNCATE, MERGE.
- Supports many real BI constructs: CTEs, joins, CASE, CAST/CONVERT, aggregates, GROUP BY, multipart identifiers, window functions, UNION, nested selects, table hints, single-quoted aliases, table-valued functions, SQL Server `N'...'` national string literals, SQL Server `!=`, and mojibake/Unicode identifier tolerance in legitimate identifier positions.
- Scalar UDF support is intentionally expression-shaped: parameters, scalar return type, and one modeled return expression; procedural scalar bodies remain explicitly unsupported unless added as a generic lowering subset.

Gaps:
- Stored procedures are not represented. The right next step is not full procedural parsing; it is an explicit behavior-declaration escape hatch for procedures users certify.
- Some parser gaps remain, for example unsupported auxiliary batches, broader `CROSS ...` forms, unsupported wrapper options, INSERT DEFAULT VALUES, and unsupported data types or hints.
- The model is huge, which raises generator performance and reviewability pressure.
- Round-trip is semantic, not trivia preserving, which is correct but needs clear test coverage for every claimed surface.

Ordered hardening work:
1. P0: Add a `DeclaredProcedureBehavior` model or equivalent outside the syntax parser: declared reads, writes, statement effect, output rowset, and determinism/sync intent. Procedures without declarations fail.
2. P0: Make unsupported syntax errors precise and stable for production corpus import.
3. P0: Keep DML access-profile extraction complete for binding/pipeline/orchestration.
4. P1: Expand SQL Server target syntax coverage where real production scripts still fail: table-source variants, data types, hint subsets, APPLY/PIVOT edge cases, and safe wrapper options.
5. P1: Add large corpus round-trip and fuzz tests that measure import/export stability.
6. P2: Add parser support reports that distinguish supported, rejected, and unimplemented syntax.

Suggested tests:
- SQL corpus tests for each statement kind and each claimed SQL Server compatibility feature.
- `SQL -> workspace -> SQL -> workspace` equivalence tests using `meta instance diff`.
- Negative tests for invalid syntax and unsupported procedure usage without declaration.

Affected generators/targets:
- MetaTransformBinding.
- MetaDataQuality.
- MetaPipeline.
- MetaOrchestration.

### Model: MetaTransformBinding

Role in ladder:
- Binds transform syntax to schema and target contracts.
- Produces the minimum structured truth for pipeline and orchestration to reason without parsing SQL.

Current support:
- Transform binding, targets, validations, rowsets, source/target rowset links, column links, table sources, type compatibility outcomes, ignored target columns.
- Current docs indicate SELECT and DML binding are in place for mutation target/source discovery.

Gaps:
- Needs strong statement-effect profiles for procedure declarations and all DML.
- Some rowset inference paths are schema-deferred or function-name based.
- Binding must keep target state reads separate from source/lookup reads for orchestration.

Ordered hardening work:
1. P0: Formalize binding output profile consumed by Pipeline and Orchestration: source reads, lookup reads, target reads, target writes, write effect, output rowset, target rowset, and type conformance.
2. P0: Add binding support for declared stored-procedure behavior.
3. P1: Expand function/table-source binding where production SQL relies on supported SQL Server built-ins.
4. P1: Add conversion-workspace coverage tests for target type systems.
5. P2: Improve binding diagnostics so unbound columns, unsupported rowsets, and target mismatches are distinct.

Suggested tests:
- One fixture per statement kind: SELECT, INSERT, UPDATE, DELETE, TRUNCATE, MERGE.
- Procedure declaration binding fixture.
- Source/lookup versus target-state-read orchestration profile tests.

Affected generators/targets:
- MetaPipeline.
- MetaOrchestration.
- MetaDataQuality.

### Model: MetaDataQuality

Role in ladder:
- Data-quality candidate/evidence analysis over transform corpus, plus promoted SQL checks.

Current support:
- Rich candidate/evidence rows around join patterns, column equivalence, filter observations, orphan/fanout/duplicate risks, implied FK/unique/fanout risks.
- `data-quality-to-sql` can emit operational check artifacts for supported promoted candidates.

Gaps:
- The model is evidence-heavy. Accepted DQ checks need a clearer first-class explicit model boundary.
- Candidate confidence is useful, but accepted product truth cannot be "candidate confidence." It must become a declared DQ rule/check.
- SQL output currently supports only selected families and fails unsupported promoted families clearly.

Ordered hardening work:
1. P0: Split or add explicit accepted DQ rule/check entities separate from mined candidates and evidence.
2. P0: Ensure promoted candidates record exactly what accepted rule will be generated, including SQL target, severity, expected outcome, and ownership.
3. P1: Expand `data-quality-to-sql` coverage across promoted check families.
4. P1: Add operational result model for DQ runs that can feed pipeline/orchestration evidence without becoming product truth.
5. P2: Add inspect output showing candidate -> accepted rule -> generated SQL trace.

Suggested tests:
- Candidate mining fixtures.
- Promotion-to-rule fixtures.
- SQL generation golden tests for each supported DQ rule family.
- Unsupported promoted family fails clearly.

Affected generators/targets:
- MetaDataQuality SQL operational pack.
- MetaPipeline operational evidence.
- MetaOrchestration dependency/resource checks if DQ tasks become pipelines.

### Model: MetaRawDataVault

Role in ladder:
- Source-system aligned Raw Data Vault logical model.

Current support:
- Source systems/schemas/tables/fields/relationships, raw hubs, hub keys, hub satellites, raw links, link hubs, link satellites.

Gaps:
- Source model is close to MetaSchema but duplicated in DV-specific terms.
- Validation must prevent impossible Raw DV shapes such as duplicate key ordinals, missing hub participants, or ambiguous link roles.
- Raw load patterns and hash policy need reviewable model coverage.

Ordered hardening work:
1. P0: Add concept-level validation for hub keys, link participants, role uniqueness, satellite attributes, and ordinals.
2. P1: Align source import from MetaSchema with explicit mapping and no silent loss.
3. P1: Add hash-key/hash-diff policy coverage and SQL generation tests.
4. P2: Add inspect output for raw vault shape and source coverage.

Suggested tests:
- Schema-to-raw-DV conversion fixtures.
- Negative validation fixtures for duplicate roles/ordinals and missing participants.
- Raw DV to MetaSql golden tests.

Affected generators/targets:
- `schema-to-raw-datavault`.
- `raw-datavault-to-sql`.
- MetaSql deploy.

### Model: MetaBusinessDataVault

Role in ladder:
- Business Data Vault logical model.

Current support:
- Business hubs/links/same-as/hierarchical/reference, satellites and attributes, PIT, bridge paths.

Gaps:
- PIT and bridge semantics need stronger validation.
- Business link path ordering is represented but needs full invariants: alternating link/hub, anchor hub, role uniqueness, terminal hub.
- Some implementation details such as data type details are present in logical rows; keep them as Meta-system semantic types, not SQL types.

Ordered hardening work:
1. P0: Harden PIT and bridge validation. Bridge paths must be structurally valid and deterministic.
2. P0: Validate same-as and hierarchical links for role ambiguity and impossible hub pairs.
3. P1: Add SQL generation golden tests for every Business DV family.
4. P2: Add CLI inspection that renders hub/link/satellite/PIT/bridge structures in domain terms.

Suggested tests:
- Business bridge path positive and negative tests.
- PIT satellite membership tests.
- Business-DV-to-MetaSql golden tests.

Affected generators/targets:
- `business-datavault-to-sql`.
- MetaSql deploy.
- Future orchestration/pipeline demos.

### Model: MetaDataVaultImplementation

Role in ladder:
- Physical SQL implementation policy for Raw and Business Data Vault models.

Current support:
- Table/schema/name patterns, hash keys, hash diffs, load timestamps, record source, audit id defaults, PIT/bridge implementation settings.

Gaps:
- Strong platform columns are modeled, but update audit/load variation policy is incomplete.
- Indexing, compression, partitioning, filegroup, and columnstore choices are not visible.
- Defaults use SQL Server session context, which is good, but validation should prove all generated tables receiving pipeline writes have compatible defaults.

Ordered hardening work:
1. P0: Validate implementation rows are complete for every logical entity family being converted.
2. P1: Add physical options: indexes, compression, partitioning, and target filegroup policy where needed.
3. P1: Add platform-column policy variants for insert/update audit ids and timestamps.
4. P2: Add generated implementation manifest explaining physical naming and platform columns.

Suggested tests:
- DV implementation default workspace validation.
- Platform columns appear in generated MetaSql with expected defaults.
- Missing implementation policy fails before SQL generation.

Affected generators/targets:
- DataVault to MetaSql.
- MetaPipeline session context.
- SQL Server deploy.

### Model: MetaDataWarehouse

Role in ladder:
- Logical Kimball-style dimensional warehouse model.

Current support:
- Warehouses, dimensions, conformed dimensions, attributes, business keys, SCD attribute membership, hierarchies, junk/mini/outrigger dimensions, facts, grains, fact dimensions, measures, degenerate dimensions, transaction/periodic/accumulating/factless/aggregate facts, bridges and weights.

Gaps:
- Grain is declared but not strongly validated against fact dimensions/measures/degenerate dimensions.
- SCD is represented logically, but generated SQL/load patterns are not yet complete.
- Fact/dimension relationship optionality and role naming need strict validation.
- Measures have semantic data types, but analytics aggregation belongs later.

Ordered hardening work:
1. P0: Validate grain and fact relationships: every fact must have a grain, fact-dimension role uniqueness, required relationship consistency, bridge participation, and no impossible aggregate fact references.
2. P0: Harden SCD validation and implementation projection for Type 1/Type 2 attribute sets.
3. P1: Expand DW-to-MetaSql generation for all table types, platform columns, keys, and indexes.
4. P1: Add conversion from DW to Analytics where useful: facts/dimensions -> tables/attributes/base measures.
5. P2: Add domain-level inspect output for dimensional designs.

Suggested tests:
- Logical validation negative tests.
- DW-to-MetaSql golden tests per table type.
- Round-trip sample workspace tests.
- DW-to-Analytics conversion tests when that path lands.

Affected generators/targets:
- `data-warehouse-to-sql`.
- MetaSql deploy.
- MetaAnalytics conversion.
- Pipeline demos.

### Model: MetaDataWarehouseImplementation

Role in ladder:
- SQL implementation policy for logical warehouse tables.

Current support:
- Table naming, surrogate keys, business key columns, SCD primitive columns, period columns, bridge/fact naming, platform columns, indexes.

Gaps:
- Physical options are still limited: compression, partitioning, columnstore, clustered/nonclustered strategies, filegroups, identity/sequence policy.
- Platform columns are good but need validation against pipeline session context and target nullability.
- Index implementation may be too generic for fact vs dimension scale differences.

Ordered hardening work:
1. P0: Validate implementation rows against logical warehouse structures before conversion.
2. P1: Add columnstore/compression/partition policy for fact and aggregate tables.
3. P1: Add surrogate key generation policy: identity, sequence, external key, or no generated key.
4. P2: Add generated physical design report.

Suggested tests:
- Implementation policy validation tests.
- DW-to-MetaSql physical options golden tests.
- Platform default/session-context integration tests.

Affected generators/targets:
- DataWarehouse to MetaSql.
- MetaSql deploy.
- MetaPipeline audit/session context.

### Model: MetaAnalytics

Role in ladder:
- Common conceptual analytics model before target-specific tabular or multidimensional implementation.

Current support:
- Models, data sources, tables, attributes, sort by, attribute relationships, hierarchies/levels, relationships, source-backed measures, aggregation behavior, perspectives, roles, role filters, object permissions, cultures, translations.
- Deliberately excludes calculated measures, KPIs, and target-language scripts.

Gaps:
- Some properties lean target-shaped, for example `ExpressionLanguage` can become tabular-specific if not guarded.
- Measure aggregation behavior is useful for base measurements, but needs a sanctioned function enum/policy instead of arbitrary strings.
- Need conversion previews that show what target-specific work remains after conversion.

Ordered hardening work:
1. P0: Validate conceptual boundaries: no DAX-only or MDX-only semantics in portable rows unless explicitly marked as target-specific and rejected by the other converter.
2. P0: Make aggregation functions sanctioned values and reject unsupported target projections.
3. P1: Add Analytics-to-DW and DW-to-Analytics bridge where base dimensional models should become analytical surfaces.
4. P1: Add conversion manifests explaining rows converted, rows rejected, and target patch points.
5. P2: Add security validation so role filters and object permissions have target-compatible intent.

Suggested tests:
- Conceptual validation tests.
- Analytics-to-Tabular and Analytics-to-Multidimensional conversion golden tests.
- Unsupported expression-language failure tests.

Affected generators/targets:
- MetaTabular.
- MetaMultiDimensional.
- Future docs and partner demos.

### Model: MetaTabular

Role in ladder:
- Target implementation model for SSAS tabular.

Current support:
- Model, data source, tables, columns, sort-by, hierarchies, relationships, measures, KPIs, calculation groups/items, partitions, perspectives, roles/members/filters, table/column permissions, cultures, translations.
- Deploy creates a tabular database, processes by default, supports restore/drop.

Gaps:
- Production target knobs are partial: compatibility level exists, storage mode exists, collation exists, but credential/impersonation policy is hard-coded to service account in deploy.
- Partition processing policy, incremental refresh, detail rows expression kind, M vs SQL source, DirectQuery/import/composite settings, privacy, data source credentials, and refresh transactions are not fully modeled.
- Several map functions silently default invalid enum strings instead of failing validation.

Ordered hardening work:
1. P0: Add validation for all enum-like properties. Invalid `StorageMode`, `DefaultDataView`, `Mode`, `SummarizeBy`, `Permission`, or relationship cardinality must fail clearly, not default.
2. P0: Model data-source credential and impersonation policy explicitly.
3. P0: Model partition source kind and processing policy: query/M/table, mode, refresh type, transaction behavior, and `--no-process` interaction.
4. P1: Add target coverage for incremental refresh, DirectQuery/composite models, object-level security details, culture/translation edge cases, and calculation group precedence validation.
5. P2: Add deploy preview/review rows before live SSAS mutation.

Suggested tests:
- TOM deploy shape tests for every modeled property.
- Invalid enum value fails before deploy.
- Live processing smoke with explicit `CompatibilityLevel`, collation, role filters, OLS, calculation groups, and partitions.
- Backup/restore promotion tests where possible.

Affected generators/targets:
- `analytics-to-tabular`.
- `meta-tabular deploy/restore/drop`.
- SSAS Tabular databases.

### Model: MetaMultiDimensional

Role in ladder:
- Target implementation model for SSAS multidimensional.

Current support:
- Database, data source, cubes, dimensions, dimension attributes, attribute relationships, hierarchies, cube dimensions, measure groups, measures, dimension usage, KPIs, MDX calculations, named sets, actions, partitions, perspectives, roles, dimension/cell permissions, cultures, translations.
- Deploy creates an SSAS multidimensional database, data source view, dimensions, cubes, measure groups, partitions, scripts/actions/security, and processes by default.

Gaps:
- The deploy service hard-codes important target settings:
  - dimensions use `DimensionStorageMode.Molap`
  - cubes use `StorageMode.Molap`
  - measure groups default to `StorageMode.Molap` and `ProcessingMode.Regular`
  - data sources impersonate service account
- The model only exposes `StorageMode`/`ProcessingMode` on `Partition`, not on dimensions, cubes, or measure groups. This blocks settings such as large/64-bit dimension behavior and other production tuning.
- Source binding is currently deterministic but incomplete: docs say fact-side dimension usage key binding still needs explicit modeling.
- Aggregation design, proactive caching, unknown member, attribute key/name/value column richness, member properties, dimension processing group, error configuration, translations breadth, and partition slice/source policy need coverage.
- Enum parsing defaults invalid values to MOLAP/Regular/Allowed in several places. That should be validation failure.

Ordered hardening work:
1. P0: Add modeled storage/processing settings for dimensions, cubes, and measure groups where AMO exposes production-relevant knobs. Include the specific large-dimension/64-bit setting once confirmed against AMO terminology.
2. P0: Stop silent defaulting of invalid target enum values. Validate and fail before deploy.
3. P0: Model fact-side dimension usage key binding explicitly instead of deriving it from dimension granularity attribute.
4. P0: Model data-source credential/impersonation policy.
5. P1: Add aggregation design and partition processing policy.
6. P1: Add unknown member, attribute key/name/value column options, member properties, and dimension processing group settings.
7. P2: Add SSAS deployment preview and process diagnostics with language/collation/locale notes.

Suggested tests:
- AMO deploy shape tests for dimension/cube/measure-group/partition settings.
- Invalid enum value tests.
- Dimension usage binding tests.
- Live processing smoke for a dimension with production storage/processing settings.
- Locale/collation/language smoke notes for LCID 1033 and Swedish environments.

Affected generators/targets:
- `analytics-to-multi-dimensional`.
- `meta-multi-dimensional deploy/restore/drop`.
- SSAS Multidimensional databases.

### Model: MetaPipeline

Role in ladder:
- Serial execution of bound transform scripts.
- Supplies runtime session context and operational evidence.

Current support:
- Pipelines, connection references, serial tasks, dependencies, row streams, transform execution tasks, target writes, `InsertRows`, timeouts, explicit execution/target connections, operational DB, audit/session context, diagnostic pruning.

Gaps:
- SQL Server is the only current runtime source/target.
- `InsertRows` needs the planned light adapter/plugin boundary for external data sources: select columns/rows and schema columns/types only.
- Failure policy is currently local and simple. Orchestration now handles viable DAG continuation, but pipeline-local retry/stop behavior needs explicit model rows if expanded.
- Operational DB retention preserves audit logs and prunes diagnostics, but inspection/review surfaces can improve.

Ordered hardening work:
1. P0: Add explicit source adapter interface for "select rows plus schema" without dragging vendor feature models into core pipeline.
2. P0: Validate each transform task has a binding, explicit execution connection, and legal target-write shape.
3. P1: Add pipeline evidence inspection: run, task, metrics, failures, fingerprints, diagnostics.
4. P1: Add modeled retry/backoff only if there is a concrete operational requirement; otherwise keep orchestration-level failure paths.
5. P2: Add dry-run/check that validates workspaces, connections, binding, and pipeline DB before execution.

Suggested tests:
- Adapter contract tests with a fake column/row source.
- Pipeline validation tests for bad connection, missing binding, invalid InsertRows, DML with target write, and type conversion gaps.
- Operational DB schema/evidence tests.

Affected generators/targets:
- MetaPipeline runtime.
- MetaOrchestration execution.
- SQL Server operational evidence DB.

### Model: MetaOrchestration

Role in ladder:
- Builds task dependency graph and run plan from bound pipeline profiles.
- Separates data dependency, write determinism, synchronization, and conditional success/failure branches.

Current support:
- Plans, pipeline references, data objects, task profiles, object access evidence, task object effects, task dependencies, ordering resolutions, lock policies, run plans, planned tasks, planned locks, pipeline dependency projection, issues.
- Local process-based execution over `meta-pipeline execute-worker`, with orchestration grants between worker task-ready events, per-run journals, an exclusive workspace execution lease, and local liveness guards for impossible worker waits.
- Continues viable DAG paths by default; success/failure dependency conditions exist.

Gaps:
- The term "run plan" is now better than batches, but the model still has planned task rows without a future full scheduler model.
- Resource policy is still thin: max degree of parallelism exists, but CPU/memory/disk/network/source-system pools are not modeled.
- Failure continuation exists, but retry, compensation, alerting, and manual gate patterns are not modeled.
- Lock-aware scheduling is deterministic but not yet a distributed runtime lock manager.
- The local lease prevents duplicate execution of one orchestration workspace on one machine, but a future operational DB lease is still needed for multi-machine execution.

Ordered hardening work:
1. P0: Keep run plan graph-based. Do not make topological batches the default semantic contract unless the user explicitly models batch grouping.
2. P0: Validate failure dependency graphs: no impossible unhandled cycles, no missing failure handler task references, clear skipped branch semantics.
3. P1: Add resource policy rows for execution constraints that are not data dependencies: named resource pools, max concurrency per resource, and source-system throttles.
4. P1: Add run-plan diff/review output: dependencies, locks, policies, and affected pipelines.
5. P2: Add execution history import from MetaPipeline operational DB so orchestration can inspect real outcomes without becoming runtime truth.

Suggested tests:
- Graph run-plan tests without batch semantics.
- Success/failure dependency tests.
- Same-object lock policy tests.
- Resource-policy scheduling tests once modeled.

Affected generators/targets:
- MetaPipeline execution.
- Operational DB evidence.
- Future scheduler/runtime.

### Model: MetaBusiness

Role in ladder:
- Intended low-level business model upstream of DW/Analytics.

Current support:
- Placeholder docs and project area, no serious sanctioned model yet.

Gaps:
- Empty or placeholder sanctioned areas create confusion if presented as product coverage.

Ordered hardening work:
1. P0: Mark MetaBusiness explicitly experimental/parked or define a first bounded model slice.
2. P1: If pursued, model business capabilities/processes/terms at a level that can feed DW/Analytics without becoming a consulting framework.
3. P4: Keep docs clear that it is not part of the current hardened ladder until it has model/tooling/tests.

Suggested tests:
- None until scope is defined.

Affected generators/targets:
- Future upstream modeling.

## 4. Per-CLI hardening plans

### CLI: `meta`

Workflow:
- Generic workspace creation, validation, model/instance mutation, refactor, import/export, generate, diff/merge.

Current support:
- Wide command surface and documented exit codes.

Gaps:
- `init` remains upstream while `meta-bi` standardized many model CLIs on `--new-workspace`.
- README still opens with XML-canonical wording.
- CLI output is not fully aligned with newer `Ok` / concise inspect design.

Ordered hardening work:
1. P0: Align docs to representation symmetry, not XML-canonical truth.
2. P1: Add checks that generated C#/SQL surfaces preserve natural representation semantics.
3. P2: Normalize failure output to current presenter style.
4. P3: Consider `--new-workspace` alias without breaking existing `init`.

Suggested tests:
- Help snapshot tests.
- Exit code tests for validation, diff, generation, and merge precondition failures.

### CLI: `meta-weave`

Workflow:
- Author and validate cross-model property bindings.

Current support:
- Add model, suggest, add binding, validate, materialize.

Gaps:
- Full-ladder role is unclear.
- Inspect output could better explain binding coverage and unresolved references.

Ordered hardening work:
1. P1: Define when full-ladder models should use MetaWeave.
2. P2: Add binding coverage inspection and diff-friendly materialization review.
3. P3: Align output style with `meta-bi` CLI design.

Suggested tests:
- Suggest/add/materialize CLI fixtures with ambiguous and role-scoped bindings.

### CLI: `meta-schema`

Workflow:
- Extract SQL Server schema to MetaSchema.

Current support:
- `extract sqlserver` with schema/table filters.

Gaps:
- SQL Server feature coverage is narrow.
- No non-SQL adapter plugin story yet.

Ordered hardening work:
1. P0: Add extractor coverage for constraints/defaults/computed/indexes needed downstream.
2. P1: Add adapter contract for simple source-column/type extraction.
3. P2: Add skipped-feature report.

Suggested tests:
- SQL Server extraction fixtures for every supported schema element.

### CLI: `meta-data-type`

Workflow:
- Create sanctioned data-type workspace.

Current support:
- `--new-workspace`.

Gaps:
- No inspect/list command for users or agents.

Ordered hardening work:
1. P2: Add inspect/list for type systems and canonical types.
2. P3: Add CLI tests proving no stale `init` surface returns.

Suggested tests:
- Workspace creation and inspect output tests.

### CLI: `meta-data-type-conversion`

Workflow:
- Create/check/resolve type conversion policy.

Current support:
- `--new-workspace`, `check`, `resolve`.

Gaps:
- No full matrix/report for target coverage.
- Resolve is one-off, not review-oriented.

Ordered hardening work:
1. P1: Add conversion matrix inspection per target system.
2. P2: Improve diagnostics for unsupported facets or ambiguous target systems.

Suggested tests:
- Matrix golden output.
- Ambiguous/unsupported conversion failures.

### CLI: `meta-sql`

Workflow:
- Plan SQL Server deployment, apply manifest, execute small SQL scripts.

Current support:
- Deploy plan, deploy, exact destructive approvals, manifest fingerprint validation.

Gaps:
- No standalone `inspect-manifest` command.
- Missing DDL support creates block rows instead of full deployment.
- `execute` is helpful but must remain demo/bootstrap, not a backdoor model bypass.

Ordered hardening work:
1. P0: Add support or explicit detection for more SQL Server target features.
2. P1: Add manifest inspect/preview SQL.
3. P2: Add dry-run validation command.
4. P3: Keep CLI output terse by default; put details behind inspect.

Suggested tests:
- End-to-end no-diff deploy fixtures.
- Manifest inspect golden tests.

### CLI: `meta-datavault-raw`

Workflow:
- Create and author Raw DV logical workspaces.

Current support:
- Generated `add-*` authoring commands.

Gaps:
- Generic add commands can create conceptually invalid DV unless service validation catches all invariants.

Ordered hardening work:
1. P0: Add domain validation for hubs, links, satellites, roles, ordinals.
2. P2: Add inspect command that renders the raw vault by hubs/links/satellites.

Suggested tests:
- Positive CLI authoring and negative invariant tests.

### CLI: `meta-datavault-business`

Workflow:
- Create and author Business DV logical workspaces.

Current support:
- Broad `add-*` surface including PIT and bridges.

Gaps:
- PIT/bridge path validation needs to be complete.

Ordered hardening work:
1. P0: Harden PIT/bridge/link validation at authoring time.
2. P2: Add inspect command for business vault graph and bridge paths.

Suggested tests:
- PIT and bridge CLI fixtures.

### CLI: `meta-transform-script`

Workflow:
- Import/export supported SQL into/from transform workspace.

Current support:
- File/code import, file/code export, append to workspace, bounded SQL Server syntax surface.

Gaps:
- Process startup/generator JIT still matters on many one-file invocations.
- Folder import was removed until target mapping exists.
- Stored procedures are not accepted without behavior declarations.

Ordered hardening work:
1. P0: Add declared stored-procedure behavior authoring/import path.
2. P1: Continue parser corpus hardening.
3. P1: Investigate session mode only if CLI startup remains user-painful after ReadyToRun.
4. P2: Add compact import summary and detailed import report on demand.

Suggested tests:
- Production corpus import/round-trip.
- Unsupported procedure without declaration fails.
- Declared procedure behavior creates binding profile.

### CLI: `meta-transform-binding`

Workflow:
- Bind transform workspace against schema and target schema.

Current support:
- Bind with source/target schema, execution system, ignored target columns, data type conversion workspace.

Gaps:
- Needs richer inspect output for source/target/effect profiles.
- Must remain strict about unsupported/unbound constructs.

Ordered hardening work:
1. P0: Bind procedure behavior declarations and all DML effects.
2. P1: Add profile inspection for orchestration and pipeline consumers.
3. P2: Improve target mismatch and type conversion diagnostics.

Suggested tests:
- Binding profile golden tests per statement kind.

### CLI: `meta-data-quality`

Workflow:
- Mine candidates from transform workspace, inspect, promote, convert to SQL.

Current support:
- Candidate discovery and selected SQL output modes.

Gaps:
- Promotion should create explicit accepted rule state, not just candidate status.

Ordered hardening work:
1. P0: Add accepted DQ rule/check model and CLI promotion into it.
2. P1: Expand SQL generation coverage for accepted rules.
3. P2: Add inspect chain: evidence -> accepted rule -> generated SQL.

Suggested tests:
- Accepted check lifecycle tests.

### CLI: `meta-pipeline`

Workflow:
- Author and execute serial transform pipelines; bootstrap/prune operational DB.

Current support:
- `--new-workspace`, add pipeline/step, execute, execute-step, execute-sqlserver, operational DB create/prune, inspect.

Gaps:
- No source adapter plugin surface.
- Evidence inspection is limited.
- No dry-run/check for all execution preconditions.

Ordered hardening work:
1. P0: Add light adapter contract for non-SQL sources.
2. P0: Add `check` or dry-run to verify pipeline workspace, transform workspace, binding workspace, conversion workspace, connections, and operational DB before execution.
3. P1: Add operational DB inspect commands.
4. P2: Add clearer failure summaries for child SQL exceptions.

Suggested tests:
- Adapter fake source tests.
- Preflight tests.
- Operational evidence tests.

### CLI: `meta-orchestration`

Workflow:
- Infer orchestration graph from bound pipelines, resolve policy, refresh/inspect/execute run plan.

Current support:
- New workspace inference, inspect, issues, dependency/order policy, lock policy, run-plan refresh/inspect/execute, viable path continuation.

Gaps:
- User-visible run-plan semantics need graph-first language, not batch-first language.
- Resource policy and failure strategy are still minimal.
- Inspect output can get too verbose if not curated.

Ordered hardening work:
1. P0: Remove or hide batch semantics from default run-plan output unless user-authored grouping is modeled.
2. P0: Harden failure dependency validation and error reporting.
3. P1: Add resource policy rows and scheduler tests.
4. P2: Add concise graph diff/review command.

Suggested tests:
- Graph execution, failure path, skipped branch, lock policy, resource policy tests.

### CLI: `meta-data-warehouse`

Workflow:
- Create and author logical DW workspaces.

Current support:
- Generated `add-*` commands for dimensions, facts, SCD, bridges, snapshots.

Gaps:
- Needs more concept validation and guided inspect.

Ordered hardening work:
1. P0: Validate grain, SCD membership, role uniqueness, and fact/dimension consistency.
2. P1: Add `inspect` for dimensional model shape.
3. P1: Add conversion preview to MetaSql.

Suggested tests:
- Logical invalid model tests.
- CLI authoring fixtures.

### CLI: `meta-analytics`

Workflow:
- Create and author conceptual analytics model.

Current support:
- Generated `add-*` commands for common analytics concepts.

Gaps:
- Needs validation that target-specific expression languages or unsupported functions do not sneak into portable state.

Ordered hardening work:
1. P0: Validate conceptual/target boundary.
2. P1: Add conversion preview to Tabular/Multi with rejected/patch-required rows.
3. P2: Add inspect command that shows tables, relationships, measures, perspectives, roles.

Suggested tests:
- Concept validation and target conversion preview tests.

### CLI: `meta-convert`

Workflow:
- Project one sanctioned model family into another.

Current support:
- Schema to raw DV, raw/business DV to SQL, DQ to SQL, DW to SQL, Analytics to Tabular/Multi.

Gaps:
- Converters return counts, but not a reviewable fidelity manifest.
- Unsupported concepts fail in some paths, but not all paths have a formal "converted/rejected/defaulted" report.

Ordered hardening work:
1. P0: Add conversion fidelity manifest option for each converter.
2. P0: Fail when a source concept is silently dropped unless explicitly documented as not applicable.
3. P1: Add round-trip or target-diff tests where target can be re-imported.

Suggested tests:
- Per-converter golden manifests.
- Unsupported concept failure tests.

### CLI: `meta-tabular`

Workflow:
- Author target tabular workspace and deploy/restore/drop SSAS tabular DB.

Current support:
- Target model authoring, deploy with drop/process, restore promotion, drop.

Gaps:
- Target settings and invalid enum validation are incomplete.
- No deploy preview.

Ordered hardening work:
1. P0: Validate target enum values and credential/impersonation policy.
2. P0: Add model coverage for partition processing and data-source credentials.
3. P1: Add deploy preview/manifest.
4. P2: Add inspect command for target model shape.

Suggested tests:
- TOM shape and live smoke tests for production settings.

### CLI: `meta-multi-dimensional`

Workflow:
- Author target multidimensional workspace and deploy/restore/drop SSAS multidimensional DB.

Current support:
- Target authoring, deploy with drop/process, restore promotion, drop.

Gaps:
- Important AMO settings hard-coded or missing from model.
- Invalid enum values default silently in deploy service.
- Processing diagnostics and source binding need hardening.

Ordered hardening work:
1. P0: Add dimension/cube/measure-group storage/processing settings and validate them.
2. P0: Add explicit dimension usage source-key binding.
3. P0: Model credentials/impersonation.
4. P1: Add aggregation design, unknown member, member properties, and processing policy.
5. P2: Add deploy preview and process diagnostics.

Suggested tests:
- AMO shape tests and live processing smoke around large dimension settings.

### CLI: `install-meta-bi`

Workflow:
- Install offline `meta-bi` command package.

Current support:
- Installer exists and is packaged.

Gaps:
- Release/install path has been manually sensitive.

Ordered hardening work:
1. P1: Add release smoke script that installs package in a clean temp location and verifies every CLI reports help.
2. P2: Add PATH diagnostics and version output.
3. P3: Add signed/checksummed artifact manifest if productizing externally.

Suggested tests:
- Offline package install smoke.

## 5. Full-ladder hardening order

1. P0: Fix target deploy validation defaults in `MetaTabular` and `MetaMultiDimensional`. Invalid target enum/property values must fail before deploy.
2. P0: Add `MetaMultiDimensional` storage/processing settings for dimensions, cubes, and measure groups, including the large/64-bit dimension setting once verified against AMO.
3. P0: Model SSAS data-source credential/impersonation policy in both target models.
4. P0: Add explicit multidimensional dimension-usage source key binding. Do not infer fact-side keys from dimension attributes.
5. P0: Formalize accepted DQ checks separately from mined `MetaDataQuality` candidates.
6. P0: Add declared stored-procedure behavior rows and binding profiles. Procedures without declaration should fail.
7. P0: Add stronger validation to DataWarehouse and DataVault authoring services: grain, SCD, role uniqueness, bridge/PIT invariants.
8. P0: Add schema/SQL coverage for check constraints, unique constraints, defaults, computed columns, and key SQL Server features needed by generated outputs.
9. P1: Add conversion fidelity manifests for every `meta-convert` path.
10. P1: Add deploy preview/review manifests for `meta-tabular` and `meta-multi-dimensional`.
11. P1: Add the light source adapter contract for pipeline/schema extraction: rows plus column/type schema, not full external feature modeling.
12. P1: Add orchestration resource policies after graph/run-plan terminology is settled.
13. P2: Add inspect commands where the default CLI should stay quiet: pipeline evidence, orchestration graph review, analytics/DW/DV shape, type conversion matrix.
14. P2: Build a golden full-ladder demo:
    - SQL Server source schema
    - MetaSchema
    - TransformScript and Binding
    - DataWarehouse
    - MetaSql deploy
    - Pipeline
    - Orchestration
    - Analytics
    - Tabular and Multidimensional deploy
15. P3: Continue CLI output polish only behind shared presenter APIs to avoid drift.

## 6. Specific target-coverage gaps

### SSAS Multidimensional

- `Dimension.StorageMode` is hard-coded to `DimensionStorageMode.Molap` in deploy. The model does not expose dimension storage settings.
- `Cube.StorageMode` is hard-coded to `StorageMode.Molap`.
- `MeasureGroup.StorageMode` and `MeasureGroup.ProcessingMode` are hard-coded to MOLAP/Regular.
- `Partition.StorageMode` and `Partition.ProcessingMode` are modeled, but invalid values default silently to MOLAP/Regular.
- Data-source impersonation is hard-coded to service account.
- Dimension usage fact-side key binding is inferred, not explicitly modeled.
- Aggregation design is not modeled.
- Proactive caching is not modeled.
- Unknown member and null processing policy are not modeled.
- Attribute key/name/value column richness is limited.
- Member properties are not modeled.
- Dimension processing group and large-dimension/64-bit processing settings need explicit AMO confirmation and modeling.
- Processing error configuration is not modeled.
- Locale/collation/language are partially modeled, but production guidance and tests are thin.

### SSAS Tabular

- `CompatibilityLevel`, `DefaultCulture`, `StorageMode`, `DefaultDataView`, and `Collation` exist on the model, but several target values are parsed leniently or only partially used.
- Data-source impersonation is hard-coded to service account.
- Credentials/secret references are not modeled.
- Partition mode exists, but partition source kind, M expressions, SQL query details, incremental refresh, and processing policy are not sufficiently modeled.
- DirectQuery/composite model settings are not fully represented.
- Object-level security exists through table/column permissions, but target shape tests need to prove deploy fidelity.
- Calculation groups exist, but precedence and interaction validation should be stronger.

### SQL Server / MetaSql

- Check constraints are not first-class model elements.
- Unique constraints are not first-class model elements separate from keys/indexes.
- Default constraint names are rendered by convention rather than fully modeled.
- Computed column expressions exist, but alter/deploy support is limited.
- Column collation is missing.
- Sparse, rowguidcol, sequences, temporal, compression, partitioning, filegroups, columnstore, statistics, triggers, and permissions are not represented.
- Some deploy operations are blocked because clustered primary key/index replacement is outside the current executable subset.

### MetaSchema extraction

- Extracts core table/view columns, type details, identity, keys, and trusted FKs.
- Does not yet carry enough SQL Server feature coverage to serve as a no-loss live schema representation for all generated SQL targets.
- Needs skipped-feature reporting when live SQL contains supported-by-SQL-Server but unsupported-by-meta structures.

### TransformScript / Binding

- Stored procedures need explicit declared behavior, not parser heroics and not blind execution.
- `INSERT DEFAULT VALUES`, broader table-source forms, unsupported wrapper options, and some data types/hints remain unsupported.
- Binding must surface unsupported scalar/rowset shapes as explicit diagnostics.

### Data Quality

- Candidate/evidence rows are rich, but accepted DQ rule/check rows are not yet a clear separate product truth.
- SQL generation supports selected promoted families only.

### Pipeline / Orchestration

- Pipeline source adapters are not yet modeled.
- Orchestration resource pools, source throttles, retry/resume, and distributed execution are not modeled.
- Topological batch grouping should remain optional/user-authored, not the default semantics of a run plan.

## 7. Recommended next implementation batch

Small first batch that would make the stack feel more serious without a rewrite:

1. Harden SSAS target value validation.
   - Add validators for `MetaTabular` and `MetaMultiDimensional` enum-like properties.
   - Replace silent deploy defaults with clear model validation errors.
   - Add tests for invalid storage mode, processing mode, permissions, data view, relationship cardinality, and aggregation function values.

2. Add multidimensional production storage/processing knobs.
   - Add modeled storage/processing settings to dimensions, cubes, and measure groups.
   - Confirm the exact AMO property for the 64-bit/large-dimension setting and model it explicitly.
   - Update deploy service and AMO shape tests.

3. Add SSAS data-source credential/impersonation policy.
   - Model impersonation mode and credential/reference policy in Tabular and Multidimensional data sources.
   - Update deploy services and tests.

4. Add conversion/deploy fidelity manifests.
   - Start with `analytics-to-tabular`, `analytics-to-multi-dimensional`, `meta-tabular deploy`, and `meta-multi-dimensional deploy`.
   - The manifest should say which modeled rows were emitted, rejected, defaulted by explicit policy, or left as target patch work.

5. Split accepted DataQuality checks from candidate evidence.
   - Keep candidate mining as upstream evidence.
   - Promote to explicit accepted DQ rule/check rows before SQL generation.

6. Add declared stored-procedure behavior.
   - Model procedure name, declared reads/writes, output rowset, write effect, synchronization intent, and binding profile.
   - Make pipeline/orchestration accept only declared procedure behavior, never opaque undeclared stored procedure execution.

7. Add a golden full-ladder fixture.
   - One compact Commerce model that runs source schema -> transforms -> binding -> DW -> SQL deploy -> pipeline -> orchestration -> analytics -> tabular/multidimensional deploy.
   - The point is not demo polish; it is compiler fidelity across every layer.

This batch closes product-blocking expressiveness and generation gaps first. It does not replace the model-first architecture, does not make SQL blobs source of truth, and does not introduce fuzzy interpretation into accepted model state.
