# Integration demos

These meshes cover different product boundaries. Run each demo's documented
operations in order, using current CLI builds on `PATH`. SQL Server examples
use disposable local databases; Analysis Services examples require the named
local instance. Cleanup operations describe which resources they preserve.

| Demo | Useful coverage |
| --- | --- |
| [AdventureWorksBiStackDemo](AdventureWorksBiStackDemo) | Source discovery, Raw and Business Vault, warehouse, Binding, 120 Pipeline tasks, DQ review and processed Tabular measures. |
| [MetaTransformScriptConversionCliIntegration](MetaTransformScriptConversionCliIntegration) | Editable CAST/CONVERT snippets, separate output workspaces, strict Binding and exact SQL Server values including Unicode, NULL and empty strings. |
| [MetaPipelineSqlServerCliIntegration](MetaPipelineSqlServerCliIntegration) | TRUNCATE, INSERT SELECT, UPDATE, MERGE, DELETE and SELECT/InsertRows with exact target and audit checks. |
| [MetaOrchestrationCliIntegration](MetaOrchestrationCliIntegration) | Dependency planning, nonempty exact-value execution, mutation policies, invalid plans and independent/failure branches. |
| [MetaTransformBindingCliIntegration](MetaTransformBindingCliIntegration) | Focused persisted-schema Binding evidence. |
| [MetaTransformScriptCliIntegration](MetaTransformScriptCliIntegration) | Small semantic SQL import/export round-trip. |
| [MetaTransformScriptReferenceCorpusCliIntegration](MetaTransformScriptReferenceCorpusCliIntegration) | Broader supported syntax round-trip coverage. |
| [MetaTransformScriptTpcDsCliIntegration](MetaTransformScriptTpcDsCliIntegration) | Historical 99-module syntax corpus; separate Binding probe currently fails. Not an executable SQL Server baseline. |
| [RawDataVaultCliIntegration](RawDataVaultCliIntegration) | Authored Raw Vault generation, deployment and empty verification diff. |
| [BusinessDataVaultCliIntegration](BusinessDataVaultCliIntegration) | Authored Business Vault generation, deployment and empty verification diff. |
| [RawDataVaultFromMetaSchemaCliIntegration](RawDataVaultFromMetaSchemaCliIntegration) | Customer/Orders SQL schema extraction followed by Raw Vault generation and deployment. |
| [MetaDataWarehouseCliIntegration](MetaDataWarehouseCliIntegration) | Warehouse model authoring and SQL deployment. |
| [MetaDataQualityCliIntegration](MetaDataQualityCliIntegration) | DQ discovery, promotion and SQL generation. |
| [MetaDataQualityPhase2ABCliIntegration](MetaDataQualityPhase2ABCliIntegration) | Broader transform-derived DQ generation coverage. |
| [MetaDataQualityRealDbCliIntegration](MetaDataQualityRealDbCliIntegration) | DQ checks executed on a local SQL database. |
| [MetaAnalyticsCliIntegration](MetaAnalyticsCliIntegration) | Analytics model authoring after explicit workspace preparation. |
| [MetaTabularDeployCliIntegration](MetaTabularDeployCliIntegration) | Tabular metadata deployment; processing is deliberately skipped. |
| [MetaMultiDimensionalDeployCliIntegration](MetaMultiDimensionalDeployCliIntegration) | Analytics conversion and multidimensional metadata deployment; processing is deliberately skipped. |
| [MetaMultiDimensionalHierarchyQueryCliIntegration](MetaMultiDimensionalHierarchyQueryCliIntegration) | Relational source, multidimensional deployment, processing and actual MDX hierarchy results. |

Workspace references in a mesh identify inputs available at operation start.
Outputs created during the operation use paths under the mesh root, including
when subsequent steps consume them. An authoring workflow may instead prepare
its empty model workspace in a preceding operation. Neither pattern requires
dummy artifacts or a preflight bypass.

`{env:NAME}` passes an environment-variable name to owner options such as
`--connection-env`; it does not substitute a server/database argument's value.
