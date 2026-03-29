call cleanup.cmd >nul 2>&1

pushd ..\BusinessDataVaultCliIntegration
call .\run.cmd
popd

meta-schema extract sqlserver --new-workspace MetaSchemaWorkspace --connection "Server=.;Database=BusinessDataVaultCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false" --system BusinessDataVaultCliIntegrationWorkspace --all-schemas --all-tables

meta-datavault-raw from-metaschema --source-workspace MetaSchemaWorkspace --new-workspace RawDataVaultFromMetaSchemaCliIntegrationWorkspace

pushd RawDataVaultFromMetaSchemaCliIntegrationWorkspace

meta-datavault-raw generate-metasql --implementation-workspace ..\..\..\..\MetaDataVault\Workspaces\MetaDataVaultImplementation --database-name RawDataVaultFromMetaSchemaCliIntegrationWorkspace --out CurrentMetaSqlWorkspace

meta-sql deploy-plan --source-workspace CurrentMetaSqlWorkspace --connection-string "Server=.;Database=RawDataVaultFromMetaSchemaCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false" --out MetaSqlDeployManifest

meta-sql deploy --manifest-workspace MetaSqlDeployManifest --source-workspace CurrentMetaSqlWorkspace --connection-string "Server=.;Database=RawDataVaultFromMetaSchemaCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

meta-sql deploy-plan --source-workspace CurrentMetaSqlWorkspace --connection-string "Server=.;Database=RawDataVaultFromMetaSchemaCliIntegrationWorkspace;Integrated Security=true;TrustServerCertificate=true;Encrypt=false" --out MetaSqlVerifyManifest

popd
