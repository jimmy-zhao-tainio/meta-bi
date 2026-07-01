call cleanup.cmd >nul 2>&1

pushd ..\BusinessDataVaultCliIntegration
call .\run.cmd
popd

@set "META_BI_SOURCE_SQL=Server=.;Database=BusinessDataVaultCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
@set "META_BI_TARGET_SQL=Server=.;Database=RawDataVaultFromMetaSchemaCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

meta-schema extract sqlserver --new-workspace MetaSchemaWorkspace --connection-env META_BI_SOURCE_SQL --system BusinessDataVaultCliIntegrationWorkspace --all-schemas --all-tables

meta-convert schema-to-raw-datavault --source-workspace MetaSchemaWorkspace --new-workspace RawDataVaultFromMetaSchemaCliIntegrationWorkspace

pushd RawDataVaultFromMetaSchemaCliIntegrationWorkspace

meta-convert raw-datavault-to-sql --implementation-workspace ..\..\..\..\MetaDataVault\Workspaces\MetaDataVaultImplementation --database-name RawDataVaultFromMetaSchemaCliIntegrationWorkspace --out CurrentMetaSqlWorkspace

meta-sql deploy-plan --source-workspace CurrentMetaSqlWorkspace --connection-env META_BI_TARGET_SQL --out MetaSqlDeployManifest

meta-sql deploy --manifest-workspace MetaSqlDeployManifest --source-workspace CurrentMetaSqlWorkspace --connection-env META_BI_TARGET_SQL

meta-sql deploy-plan --source-workspace CurrentMetaSqlWorkspace --connection-env META_BI_TARGET_SQL --out MetaSqlVerifyManifest

popd
