# Commands

This file is generated from each CLI's own help output.
Update the CLI help text first, then regenerate with:

```powershell
powershell -ExecutionPolicy Bypass -File scripts\update-commands-md.ps1
```

The command sections are intentionally plain console help transcripts so agents and humans see the same interface.

## meta-schema

### `meta-schema --help`

```text
Usage:
  meta-schema <command> [options]

Commands:

  help     Show this help.
  extract  Materialize sanctioned MetaSchema workspaces from external sources.

Next: meta-schema extract --help
```

### `meta-schema extract --help`

```text
Command: extract
Usage:
  meta-schema extract <extractor> [options]

Notes:
  Available extractor: sqlserver.

Next: meta-schema extract sqlserver --help
```

### `meta-schema extract sqlserver --help`

```text
Command: extract sqlserver
Usage:
  meta-schema extract sqlserver --new-workspace <path> --connection-env <name> --system <name>
  (--schema <name> | --all-schemas) (--table <name> | --all-tables)

Options:

  --new-workspace <path>   Required. Directory where the MetaSchema workspace will be created.
  --connection-env <name>  Required. Environment variable containing the SQL Server connection
                           string.
  --system <name>          Required. Source system name recorded in the workspace.
  --schema <name>          Extract one SQL Server schema. Mutually exclusive with --all-schemas.
  --all-schemas            Extract all SQL Server schemas in scope.
  --table <name>           Extract one SQL Server table or view. Mutually exclusive with
                           --all-tables.
  --all-tables             Extract all SQL Server tables and views in scope.

Notes:
  Creates a new workspace with the MetaSchema model and validates it.
  Scope is controlled by schema/table filters or all-schemas/all-tables discovery switches.
  TableRelationship rows are emitted only for enforced and trusted SQL Server foreign keys whose source and target tables are both in scope.
  Field rows carry a scalar DataTypeId plus local FieldDataTypeDetail rows such as Length, Precision, or Scale.
```

## meta-data-type

### `meta-data-type --help`

```text
Usage:
  meta-data-type [--new-workspace <path> | <command> [options]]

Commands:

  help             Show this help.
  --new-workspace  Create a new MetaDataType workspace.

Next: meta-data-type --new-workspace --help
```

### `meta-data-type --new-workspace --help`

```text
Command: --new-workspace
Usage:
  meta-data-type --new-workspace <path>

Options:

  --new-workspace <path>  Required. Directory where the sanctioned workspace will be created.

Notes:
  Creates a new workspace with the MetaDataType model, sanctioned type instances, and validates it.
```

## meta-data-type-conversion

### `meta-data-type-conversion --help`

```text
Usage:
  meta-data-type-conversion [--new-workspace <path> | <command> [options]]

Commands:

  help             Show this help.
  --new-workspace  Create a new MetaDataTypeConversion workspace.
  check            Validate sanctioned type mappings.
  resolve          Resolve one source data type id through the sanctioned mappings.

Next: meta-data-type-conversion --new-workspace --help
```

### `meta-data-type-conversion --new-workspace --help`

```text
Command: --new-workspace
Usage:
  meta-data-type-conversion --new-workspace <path>

Options:

  --new-workspace <path>  Required. Directory where the sanctioned workspace will be created.

Notes:
  Creates a new workspace with the MetaDataTypeConversion model and validates it.
```

### `meta-data-type-conversion check --help`

```text
Command: check
Usage:
  meta-data-type-conversion check --workspace <path>

Options:

  --workspace <path>  Required. MetaDataTypeConversion workspace to validate.

Notes:
  Validates that each source data type maps deterministically per target data type system and that every mapping references a real ConversionImplementation.
```

### `meta-data-type-conversion resolve --help`

```text
Command: resolve
Usage:
  meta-data-type-conversion resolve --workspace <path> --source-data-type <id>
  [--target-data-type-system <name>]

Options:

  --workspace <path>                Required. MetaDataTypeConversion workspace to query.
  --source-data-type <id>           Required. Source data type id to resolve.
  --target-data-type-system <name>  Optional target system when the source type has mappings to
                                    several target systems.

Notes:
  Resolves one source data type id to its target data type id and conversion implementation.
  Use --target-data-type-system when one source type has mappings to several target systems.
```

## meta-sql

### `meta-sql --help`

```text
Usage:
  meta-sql <command> [options]

Commands:

  help         Show this help.
  extract      Materialize sanctioned MetaSql workspaces from external sources.
  deploy-plan  Create a deploy manifest (add/alter/block/replace; destructive actions require exact
               object-scoped approvals).
  deploy       Apply a deploy manifest after source/live fingerprint validation.
  execute      Execute a SQL Server file or query for demo/bootstrap/verification scripts.

Next: meta-sql deploy-plan --help
```

### `meta-sql extract --help`

```text
Command: extract
Usage:
  meta-sql extract <extractor> [options]

Notes:
  Available extractor: sqlserver.

Next: meta-sql extract sqlserver --help
```

### `meta-sql deploy-plan --help`

```text
Command: deploy-plan
Usage:
  meta-sql deploy-plan --source-workspace <path> --connection-env <name> --out <path>
  [--approve-drop-table <schema.table>] [--approve-drop-column <schema.table.column>]
  [--approve-truncate-column <schema.table.column>] [--approval-file <path>]

Options:

  --source-workspace <path>                        Required. Source MetaSql workspace to compare
                                                   with the live database.
  --connection-env <name>                          Required. Environment variable containing the SQL
                                                   Server connection string.
  --out <path>                                     Required. Directory where the deploy manifest
                                                   workspace will be written.
  --approve-drop-table <schema.table>              Approve one exact destructive table drop.
  --approve-drop-column <schema.table.column>      Approve one exact destructive column drop.
  --approve-truncate-column <schema.table.column>  Approve one exact destructive column truncation.
  --approval-file <path>                           Optional JSON file containing destructive
                                                   approvals.

Notes:
  Loads the source MetaSql workspace.
  Extracts the live SQL Server schema to MetaSql.
  Always plans against the full source workspace and full live database. Filtered subset deploy is not supported.
  Creates a deploy manifest with Add/Drop/Truncate/Alter/Replace/Block entries.
  DataDropTable and DataDropColumn require exact object-scoped approvals.
  DataTruncationColumn requires exact object-scoped approval.
  Approvals can be passed as repeated CLI arguments and/or via --approval-file JSON.
  Live-only DropPrimaryKey/DropForeignKey/DropIndex are planned by default.
  Shared table-column differences become AlterTableColumn when executable and feasible.
  Shared primary-key differences become ReplacePrimaryKey when executable; otherwise they are blocked.
  Shared foreign-key differences become ReplaceForeignKey when executable; otherwise they are blocked.
  Shared index differences become ReplaceIndex when executable; otherwise they are blocked.
  Deployable only when there are no block entries.
```

### `meta-sql deploy --help`

```text
Command: deploy
Usage:
  meta-sql deploy --manifest-workspace <path> --source-workspace <path> --connection-env <name>

Options:

  --manifest-workspace <path>  Required. Deploy manifest workspace created by deploy-plan.
  --source-workspace <path>    Required. Source MetaSql workspace used to create the manifest.
  --connection-env <name>      Required. Environment variable containing the SQL Server connection
                               string.

Notes:
  Loads the deploy manifest and source MetaSql workspace.
  Refuses when the manifest contains Block entries.
  Refuses when source/live instance fingerprints no longer match.
  Always validates and applies the full manifest scope. Filtered subset deploy is not supported.
  Creates the database first when the manifest expects a missing database.
  Executes deploy statements without wrapping the full deploy in one SQL transaction.
  If later statements fail after database creation, the database remains and the failure reports that explicitly.
```

### `meta-sql execute --help`

```text
Command: execute
Usage:
  meta-sql execute --connection-env <name> (--file <path> | --query <sql>) [--var <name=value>]
  [--timeout-seconds <seconds>] [--quiet]

Options:

  --connection-env <name>      Required. Environment variable containing the SQL Server connection
                               string.
  --file <path>                Execute SQL from a file. Mutually exclusive with --query.
  --query <sql>                Execute inline SQL. Mutually exclusive with --file.
  --var <name=value>           Replace one SQLCMD-style $(NAME) token before execution. Can be
                               repeated.
  --timeout-seconds <seconds>  Command timeout for each SQL batch. Defaults to 30.
  --quiet                      Suppress result-set and success output.

Notes:
  Executes SQL Server SQL batches for demo/bootstrap/verification scripts.
  Batch separators use GO lines; --var replaces $(NAME) tokens before execution.
  This command is an execution helper. Metadata realization still belongs to deploy-plan/deploy.
```

### `meta-sql extract sqlserver --help`

```text
Command: extract sqlserver
Usage:
  meta-sql extract sqlserver --new-workspace <path> --connection-env <name> [--schema <name>]
  [--table <name>] [--include-tables] [--include-views] [--include-functions]
  [--include-stored-procedures] [--allow-empty]

Options:

  --new-workspace <path>       Required. Directory where the MetaSql workspace will be created.
  --connection-env <name>      Required. Environment variable containing the SQL Server connection
                               string.
  --schema <name>              Optional. Extract only one SQL Server schema.
  --table <name>               Optional. Extract only one table. SQL module extraction is skipped
                               when a table filter is used.
  --include-tables             Extract tables, columns, keys, and indexes. If no include switch is
                               provided, all object kinds are extracted.
  --include-views              Extract view modules. If any include switch is provided, only
                               selected object kinds are extracted.
  --include-functions          Extract function modules. If any include switch is provided, only
                               selected object kinds are extracted.
  --include-stored-procedures  Extract stored procedure modules. If any include switch is provided,
                               only selected object kinds are extracted.
  --allow-empty                Create an empty database/schema workspace when no tables or modules
                               match.

Notes:
  Extracts deployable MetaSql state from SQL Server: tables, columns, primary keys, foreign keys, indexes, views, functions, and stored procedures.
  FunctionKind is derived from SQL Server object type: ScalarFunction, InlineTableValuedFunction, or TableValuedFunction.
  This is deployment-state import. Syntax-modeled CREATE VIEW/FUNCTION import remains owned by MetaTransformScript.
```

## meta-datavault-raw

### `meta-datavault-raw --help`

```text
Usage:
  meta-datavault-raw [--new-workspace <path> | <command> [options]]

Commands:

  help                                 Show this help.
  --new-workspace                      Create an empty MetaRawDataVault workspace.
  add-hub                              Add a raw hub.
  add-hub-key-part                     Add a raw hub key part.
  add-hub-satellite                    Add a raw hub satellite.
  add-hub-satellite-attribute          Add a raw hub satellite attribute.
  add-link                             Add a raw link.
  add-link-hub                         Add a participating hub to a raw link.
  add-link-satellite                   Add a raw link satellite.
  add-link-satellite-attribute         Add a raw link satellite attribute.
  add-source-field                     Add a source field.
  add-source-field-data-type-detail    Add a source field datatype detail.
  add-source-schema                    Add a source schema.
  add-source-system                    Add a source system.
  add-source-table                     Add a source table.
  add-source-table-relationship        Add a source table relationship.
  add-source-table-relationship-field  Add a source table relationship field.

Next: meta-datavault-raw add-hub --help
```

### `meta-datavault-raw --new-workspace --help`

```text
Command: --new-workspace
Usage:
  meta-datavault-raw --new-workspace <path>

Options:

  --new-workspace <path>  Required. Directory where the empty MetaRawDataVault workspace will be
                          created.
```

### `meta-datavault-raw add-hub --help`

```text
Command: add-hub
Usage:
  meta-datavault-raw add-hub [--workspace <path>] --id <id> --name <value> --source-table <id>

Options:

  --workspace <path>   Optional. Workspace path. Default: current working directory.
  --id <id>            Required. RawHub row id.
  --name <value>       Required. Name.
  --source-table <id>  Required. SourceTable id for SourceTableId.

Notes:
  Adds one RawHub row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-raw add-hub-key-part --help`

```text
Command: add-hub-key-part
Usage:
  meta-datavault-raw add-hub-key-part [--workspace <path>] --id <id> --name <value> [--ordinal
  <value>] --hub <id> --source-field <id>

Options:

  --workspace <path>   Optional. Workspace path. Default: current working directory.
  --id <id>            Required. RawHubKeyPart row id.
  --name <value>       Required. Name.
  --ordinal <value>    Optional. Ordinal.
  --hub <id>           Required. RawHub id for RawHubId.
  --source-field <id>  Required. SourceField id for SourceFieldId.

Notes:
  Adds one RawHubKeyPart row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-raw add-hub-satellite --help`

```text
Command: add-hub-satellite
Usage:
  meta-datavault-raw add-hub-satellite [--workspace <path>] --id <id> --name <value>
  --satellite-kind <value> --hub <id> --source-table <id>

Options:

  --workspace <path>        Optional. Workspace path. Default: current working directory.
  --id <id>                 Required. RawHubSatellite row id.
  --name <value>            Required. Name.
  --satellite-kind <value>  Required. SatelliteKind.
  --hub <id>                Required. RawHub id for RawHubId.
  --source-table <id>       Required. SourceTable id for SourceTableId.

Notes:
  Adds one RawHubSatellite row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-raw add-hub-satellite-attribute --help`

```text
Command: add-hub-satellite-attribute
Usage:
  meta-datavault-raw add-hub-satellite-attribute [--workspace <path>] --id <id> --name <value>
  [--ordinal <value>] --hub-satellite <id> --source-field <id>

Options:

  --workspace <path>    Optional. Workspace path. Default: current working directory.
  --id <id>             Required. RawHubSatelliteAttribute row id.
  --name <value>        Required. Name.
  --ordinal <value>     Optional. Ordinal.
  --hub-satellite <id>  Required. RawHubSatellite id for RawHubSatelliteId.
  --source-field <id>   Required. SourceField id for SourceFieldId.

Notes:
  Adds one RawHubSatelliteAttribute row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-raw add-link --help`

```text
Command: add-link
Usage:
  meta-datavault-raw add-link [--workspace <path>] --id <id> --name <value> [--link-kind <value>]
  --source-relationship <id>

Options:

  --workspace <path>          Optional. Workspace path. Default: current working directory.
  --id <id>                   Required. RawLink row id.
  --name <value>              Required. Name.
  --link-kind <value>         Optional. LinkKind.
  --source-relationship <id>  Required. SourceTableRelationship id for SourceTableRelationshipId.

Notes:
  Adds one RawLink row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-raw add-link-hub --help`

```text
Command: add-link-hub
Usage:
  meta-datavault-raw add-link-hub [--workspace <path>] --id <id> [--ordinal <value>] [--role-name
  <value>] --link <id> --hub <id>

Options:

  --workspace <path>   Optional. Workspace path. Default: current working directory.
  --id <id>            Required. RawLinkHub row id.
  --ordinal <value>    Optional. Ordinal.
  --role-name <value>  Optional. RoleName.
  --link <id>          Required. RawLink id for RawLinkId.
  --hub <id>           Required. RawHub id for RawHubId.

Notes:
  Adds one RawLinkHub row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-raw add-link-satellite --help`

```text
Command: add-link-satellite
Usage:
  meta-datavault-raw add-link-satellite [--workspace <path>] --id <id> --name <value>
  --satellite-kind <value> --link <id> --source-table <id>

Options:

  --workspace <path>        Optional. Workspace path. Default: current working directory.
  --id <id>                 Required. RawLinkSatellite row id.
  --name <value>            Required. Name.
  --satellite-kind <value>  Required. SatelliteKind.
  --link <id>               Required. RawLink id for RawLinkId.
  --source-table <id>       Required. SourceTable id for SourceTableId.

Notes:
  Adds one RawLinkSatellite row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-raw add-link-satellite-attribute --help`

```text
Command: add-link-satellite-attribute
Usage:
  meta-datavault-raw add-link-satellite-attribute [--workspace <path>] --id <id> --name <value>
  [--ordinal <value>] --link-satellite <id> --source-field <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. RawLinkSatelliteAttribute row id.
  --name <value>         Required. Name.
  --ordinal <value>      Optional. Ordinal.
  --link-satellite <id>  Required. RawLinkSatellite id for RawLinkSatelliteId.
  --source-field <id>    Required. SourceField id for SourceFieldId.

Notes:
  Adds one RawLinkSatelliteAttribute row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-raw add-source-field --help`

```text
Command: add-source-field
Usage:
  meta-datavault-raw add-source-field [--workspace <path>] --id <id> --name <value> --data-type-id
  <value> [--ordinal <value>] [--is-nullable <value>] --table <id>

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. SourceField row id.
  --name <value>          Required. Name.
  --data-type-id <value>  Required. DataTypeId.
  --ordinal <value>       Optional. Ordinal.
  --is-nullable <value>   Optional. IsNullable.
  --table <id>            Required. SourceTable id for SourceTableId.

Notes:
  Adds one SourceField row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-raw add-source-field-data-type-detail --help`

```text
Command: add-source-field-data-type-detail
Usage:
  meta-datavault-raw add-source-field-data-type-detail [--workspace <path>] --id <id> --name <value>
  --value <value> --field <id>

Options:

  --workspace <path>  Optional. Workspace path. Default: current working directory.
  --id <id>           Required. SourceFieldDataTypeDetail row id.
  --name <value>      Required. Name.
  --value <value>     Required. Value.
  --field <id>        Required. SourceField id for SourceFieldId.

Notes:
  Adds one SourceFieldDataTypeDetail row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-raw add-source-schema --help`

```text
Command: add-source-schema
Usage:
  meta-datavault-raw add-source-schema [--workspace <path>] --id <id> --name <value> --system <id>

Options:

  --workspace <path>  Optional. Workspace path. Default: current working directory.
  --id <id>           Required. SourceSchema row id.
  --name <value>      Required. Name.
  --system <id>       Required. SourceSystem id for SourceSystemId.

Notes:
  Adds one SourceSchema row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-raw add-source-system --help`

```text
Command: add-source-system
Usage:
  meta-datavault-raw add-source-system [--workspace <path>] --id <id> --name <value> [--description
  <value>]

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. SourceSystem row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.

Notes:
  Adds one SourceSystem row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-raw add-source-table --help`

```text
Command: add-source-table
Usage:
  meta-datavault-raw add-source-table [--workspace <path>] --id <id> --name <value> --schema <id>

Options:

  --workspace <path>  Optional. Workspace path. Default: current working directory.
  --id <id>           Required. SourceTable row id.
  --name <value>      Required. Name.
  --schema <id>       Required. SourceSchema id for SourceSchemaId.

Notes:
  Adds one SourceTable row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-raw add-source-table-relationship --help`

```text
Command: add-source-table-relationship
Usage:
  meta-datavault-raw add-source-table-relationship [--workspace <path>] --id <id> --name <value>
  --source-table <id> --target-table <id>

Options:

  --workspace <path>   Optional. Workspace path. Default: current working directory.
  --id <id>            Required. SourceTableRelationship row id.
  --name <value>       Required. Name.
  --source-table <id>  Required. SourceTable id for SourceTableId.
  --target-table <id>  Required. SourceTable id for TargetTableId.

Notes:
  Adds one SourceTableRelationship row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-raw add-source-table-relationship-field --help`

```text
Command: add-source-table-relationship-field
Usage:
  meta-datavault-raw add-source-table-relationship-field [--workspace <path>] --id <id> [--ordinal
  <value>] --relationship <id> --source-field <id> --target-field <id>

Options:

  --workspace <path>   Optional. Workspace path. Default: current working directory.
  --id <id>            Required. SourceTableRelationshipField row id.
  --ordinal <value>    Optional. Ordinal.
  --relationship <id>  Required. SourceTableRelationship id for SourceTableRelationshipId.
  --source-field <id>  Required. SourceField id for SourceFieldId.
  --target-field <id>  Required. SourceField id for TargetFieldId.

Notes:
  Adds one SourceTableRelationshipField row to a MetaRawDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

## meta-datavault-business

### `meta-datavault-business --help`

```text
Usage:
  meta-datavault-business [--new-workspace <path> | <command> [options]]

Commands:

  help                                       Show this help.
  --new-workspace                            Create an empty MetaBusinessDataVault workspace.
  add-bridge                                 Add a business bridge.
  add-bridge-hub                             Add a business hub to an ordered bridge path.
  add-bridge-link                            Add a business link to an ordered bridge path.
  add-hierarchical-link                      Add a hierarchical link.
  add-hierarchical-link-satellite            Add a hierarchical link satellite.
  add-hierarchical-link-satellite-attribute  Add a hierarchical link satellite attribute.
  add-hub                                    Add a business hub.
  add-hub-key-part                           Add a business hub key part.
  add-hub-satellite                          Add a business hub satellite.
  add-hub-satellite-attribute                Add a business hub satellite attribute.
  add-link                                   Add a standard business link.
  add-link-hub                               Add a participating hub to a standard business link.
  add-link-satellite                         Add a business link satellite.
  add-link-satellite-attribute               Add a business link satellite attribute.
  add-point-in-time                          Add a business point-in-time table.
  add-point-in-time-hub-satellite            Add a hub-satellite reference to a point-in-time table.
  add-point-in-time-link-satellite           Add a link-satellite reference to a point-in-time
                                             table.
  add-point-in-time-stamp                    Add a business point-in-time stamp column.
  add-reference                              Add a business reference.
  add-reference-key-part                     Add a business reference key part.
  add-reference-satellite                    Add a business reference satellite.
  add-reference-satellite-attribute          Add a business reference satellite attribute.
  add-same-as-link                           Add a same-as link.
  add-same-as-link-satellite                 Add a same-as link satellite.
  add-same-as-link-satellite-attribute       Add a same-as link satellite attribute.

Next: meta-datavault-business add-hub --help
```

### `meta-datavault-business --new-workspace --help`

```text
Command: --new-workspace
Usage:
  meta-datavault-business --new-workspace <path>

Options:

  --new-workspace <path>  Required. Directory where the empty MetaBusinessDataVault workspace will
                          be created.
```

### `meta-datavault-business add-bridge --help`

```text
Command: add-bridge
Usage:
  meta-datavault-business add-bridge [--workspace <path>] --id <id> --name <value> [--description
  <value>] --hub <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. BusinessBridge row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --hub <id>             Required. BusinessHub id for BusinessHubId.

Notes:
  Adds one BusinessBridge row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-bridge-hub --help`

```text
Command: add-bridge-hub
Usage:
  meta-datavault-business add-bridge-hub [--workspace <path>] --id <id> [--ordinal <value>]
  [--role-name <value>] --bridge <id> --hub <id>

Options:

  --workspace <path>   Optional. Workspace path. Default: current working directory.
  --id <id>            Required. BusinessBridgeHub row id.
  --ordinal <value>    Optional. Ordinal.
  --role-name <value>  Optional. RoleName.
  --bridge <id>        Required. BusinessBridge id for BusinessBridgeId.
  --hub <id>           Required. BusinessHub id for BusinessHubId.

Notes:
  Adds one BusinessBridgeHub row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-bridge-link --help`

```text
Command: add-bridge-link
Usage:
  meta-datavault-business add-bridge-link [--workspace <path>] --id <id> [--ordinal <value>]
  [--role-name <value>] --bridge <id> --link <id>

Options:

  --workspace <path>   Optional. Workspace path. Default: current working directory.
  --id <id>            Required. BusinessBridgeLink row id.
  --ordinal <value>    Optional. Ordinal.
  --role-name <value>  Optional. RoleName.
  --bridge <id>        Required. BusinessBridge id for BusinessBridgeId.
  --link <id>          Required. BusinessLink id for BusinessLinkId.

Notes:
  Adds one BusinessBridgeLink row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-hierarchical-link --help`

```text
Command: add-hierarchical-link
Usage:
  meta-datavault-business add-hierarchical-link [--workspace <path>] --id <id> --name <value>
  [--description <value>] --parent-hub <id> --child-hub <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. BusinessHierarchicalLink row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --parent-hub <id>      Required. BusinessHub id for ParentHubId.
  --child-hub <id>       Required. BusinessHub id for ChildHubId.

Notes:
  Adds one BusinessHierarchicalLink row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-hierarchical-link-satellite --help`

```text
Command: add-hierarchical-link-satellite
Usage:
  meta-datavault-business add-hierarchical-link-satellite [--workspace <path>] --id <id> --name
  <value> [--description <value>] --hierarchical-link <id>

Options:

  --workspace <path>        Optional. Workspace path. Default: current working directory.
  --id <id>                 Required. BusinessHierarchicalLinkSatellite row id.
  --name <value>            Required. Name.
  --description <value>     Optional. Description.
  --hierarchical-link <id>  Required. BusinessHierarchicalLink id for BusinessHierarchicalLinkId.

Notes:
  Adds one BusinessHierarchicalLinkSatellite row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-hierarchical-link-satellite-attribute --help`

```text
Command: add-hierarchical-link-satellite-attribute
Usage:
  meta-datavault-business add-hierarchical-link-satellite-attribute [--workspace <path>] --id <id>
  --name <value> --data-type-id <value> [--ordinal <value>] --hierarchical-link-satellite <id>
  [--length <value>] [--precision <value>] [--scale <value>]

Options:

  --workspace <path>                  Optional. Workspace path. Default: current working directory.
  --id <id>                           Required. BusinessHierarchicalLinkSatelliteAttribute row id.
  --name <value>                      Required. Name.
  --data-type-id <value>              Required. DataTypeId.
  --ordinal <value>                   Optional. Ordinal.
  --hierarchical-link-satellite <id>  Required. BusinessHierarchicalLinkSatellite id for
                                      BusinessHierarchicalLinkSatelliteId.
  --length <value>                    Optional. Length datatype facet authored as metadata.
  --precision <value>                 Optional. Precision datatype facet authored as metadata.
  --scale <value>                     Optional. Scale datatype facet authored as metadata.

Notes:
  Adds one BusinessHierarchicalLinkSatelliteAttribute row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
  Optional datatype facets are authored as internal metadata rows.
```

### `meta-datavault-business add-hub --help`

```text
Command: add-hub
Usage:
  meta-datavault-business add-hub [--workspace <path>] --id <id> --name <value> [--description
  <value>]

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. BusinessHub row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.

Notes:
  Adds one BusinessHub row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-hub-key-part --help`

```text
Command: add-hub-key-part
Usage:
  meta-datavault-business add-hub-key-part [--workspace <path>] --id <id> --name <value>
  --data-type-id <value> [--ordinal <value>] --hub <id> [--length <value>] [--precision <value>]
  [--scale <value>]

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. BusinessHubKeyPart row id.
  --name <value>          Required. Name.
  --data-type-id <value>  Required. DataTypeId.
  --ordinal <value>       Optional. Ordinal.
  --hub <id>              Required. BusinessHub id for BusinessHubId.
  --length <value>        Optional. Length datatype facet authored as metadata.
  --precision <value>     Optional. Precision datatype facet authored as metadata.
  --scale <value>         Optional. Scale datatype facet authored as metadata.

Notes:
  Adds one BusinessHubKeyPart row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
  Optional datatype facets are authored as internal metadata rows.
```

### `meta-datavault-business add-hub-satellite --help`

```text
Command: add-hub-satellite
Usage:
  meta-datavault-business add-hub-satellite [--workspace <path>] --id <id> --name <value>
  [--description <value>] --hub <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. BusinessHubSatellite row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --hub <id>             Required. BusinessHub id for BusinessHubId.

Notes:
  Adds one BusinessHubSatellite row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-hub-satellite-attribute --help`

```text
Command: add-hub-satellite-attribute
Usage:
  meta-datavault-business add-hub-satellite-attribute [--workspace <path>] --id <id> --name <value>
  --data-type-id <value> [--ordinal <value>] --hub-satellite <id> [--length <value>] [--precision
  <value>] [--scale <value>]

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. BusinessHubSatelliteAttribute row id.
  --name <value>          Required. Name.
  --data-type-id <value>  Required. DataTypeId.
  --ordinal <value>       Optional. Ordinal.
  --hub-satellite <id>    Required. BusinessHubSatellite id for BusinessHubSatelliteId.
  --length <value>        Optional. Length datatype facet authored as metadata.
  --precision <value>     Optional. Precision datatype facet authored as metadata.
  --scale <value>         Optional. Scale datatype facet authored as metadata.

Notes:
  Adds one BusinessHubSatelliteAttribute row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
  Optional datatype facets are authored as internal metadata rows.
```

### `meta-datavault-business add-link --help`

```text
Command: add-link
Usage:
  meta-datavault-business add-link [--workspace <path>] --id <id> --name <value> [--description
  <value>]

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. BusinessLink row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.

Notes:
  Adds one BusinessLink row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-link-hub --help`

```text
Command: add-link-hub
Usage:
  meta-datavault-business add-link-hub [--workspace <path>] --id <id> [--ordinal <value>]
  [--role-name <value>] --link <id> --hub <id>

Options:

  --workspace <path>   Optional. Workspace path. Default: current working directory.
  --id <id>            Required. BusinessLinkHub row id.
  --ordinal <value>    Optional. Ordinal.
  --role-name <value>  Optional. RoleName.
  --link <id>          Required. BusinessLink id for BusinessLinkId.
  --hub <id>           Required. BusinessHub id for BusinessHubId.

Notes:
  Adds one BusinessLinkHub row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-link-satellite --help`

```text
Command: add-link-satellite
Usage:
  meta-datavault-business add-link-satellite [--workspace <path>] --id <id> --name <value>
  [--description <value>] --link <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. BusinessLinkSatellite row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --link <id>            Required. BusinessLink id for BusinessLinkId.

Notes:
  Adds one BusinessLinkSatellite row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-link-satellite-attribute --help`

```text
Command: add-link-satellite-attribute
Usage:
  meta-datavault-business add-link-satellite-attribute [--workspace <path>] --id <id> --name <value>
  --data-type-id <value> [--ordinal <value>] --link-satellite <id> [--length <value>] [--precision
  <value>] [--scale <value>]

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. BusinessLinkSatelliteAttribute row id.
  --name <value>          Required. Name.
  --data-type-id <value>  Required. DataTypeId.
  --ordinal <value>       Optional. Ordinal.
  --link-satellite <id>   Required. BusinessLinkSatellite id for BusinessLinkSatelliteId.
  --length <value>        Optional. Length datatype facet authored as metadata.
  --precision <value>     Optional. Precision datatype facet authored as metadata.
  --scale <value>         Optional. Scale datatype facet authored as metadata.

Notes:
  Adds one BusinessLinkSatelliteAttribute row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
  Optional datatype facets are authored as internal metadata rows.
```

### `meta-datavault-business add-point-in-time --help`

```text
Command: add-point-in-time
Usage:
  meta-datavault-business add-point-in-time [--workspace <path>] --id <id> --name <value>
  [--description <value>] --hub <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. BusinessPointInTime row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --hub <id>             Required. BusinessHub id for BusinessHubId.

Notes:
  Adds one BusinessPointInTime row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-point-in-time-hub-satellite --help`

```text
Command: add-point-in-time-hub-satellite
Usage:
  meta-datavault-business add-point-in-time-hub-satellite [--workspace <path>] --id <id> [--ordinal
  <value>] --point-in-time <id> --hub-satellite <id>

Options:

  --workspace <path>    Optional. Workspace path. Default: current working directory.
  --id <id>             Required. BusinessPointInTimeHubSatellite row id.
  --ordinal <value>     Optional. Ordinal.
  --point-in-time <id>  Required. BusinessPointInTime id for BusinessPointInTimeId.
  --hub-satellite <id>  Required. BusinessHubSatellite id for BusinessHubSatelliteId.

Notes:
  Adds one BusinessPointInTimeHubSatellite row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-point-in-time-link-satellite --help`

```text
Command: add-point-in-time-link-satellite
Usage:
  meta-datavault-business add-point-in-time-link-satellite [--workspace <path>] --id <id> [--ordinal
  <value>] --point-in-time <id> --link-satellite <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. BusinessPointInTimeLinkSatellite row id.
  --ordinal <value>      Optional. Ordinal.
  --point-in-time <id>   Required. BusinessPointInTime id for BusinessPointInTimeId.
  --link-satellite <id>  Required. BusinessLinkSatellite id for BusinessLinkSatelliteId.

Notes:
  Adds one BusinessPointInTimeLinkSatellite row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-point-in-time-stamp --help`

```text
Command: add-point-in-time-stamp
Usage:
  meta-datavault-business add-point-in-time-stamp [--workspace <path>] --id <id> --name <value>
  --data-type-id <value> [--ordinal <value>] --point-in-time <id> [--length <value>] [--precision
  <value>] [--scale <value>]

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. BusinessPointInTimeStamp row id.
  --name <value>          Required. Name.
  --data-type-id <value>  Required. DataTypeId.
  --ordinal <value>       Optional. Ordinal.
  --point-in-time <id>    Required. BusinessPointInTime id for BusinessPointInTimeId.
  --length <value>        Optional. Length datatype facet authored as metadata.
  --precision <value>     Optional. Precision datatype facet authored as metadata.
  --scale <value>         Optional. Scale datatype facet authored as metadata.

Notes:
  Adds one BusinessPointInTimeStamp row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
  Optional datatype facets are authored as internal metadata rows.
```

### `meta-datavault-business add-reference --help`

```text
Command: add-reference
Usage:
  meta-datavault-business add-reference [--workspace <path>] --id <id> --name <value> [--description
  <value>]

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. BusinessReference row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.

Notes:
  Adds one BusinessReference row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-reference-key-part --help`

```text
Command: add-reference-key-part
Usage:
  meta-datavault-business add-reference-key-part [--workspace <path>] --id <id> --name <value>
  --data-type-id <value> [--ordinal <value>] --reference <id> [--length <value>] [--precision
  <value>] [--scale <value>]

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. BusinessReferenceKeyPart row id.
  --name <value>          Required. Name.
  --data-type-id <value>  Required. DataTypeId.
  --ordinal <value>       Optional. Ordinal.
  --reference <id>        Required. BusinessReference id for BusinessReferenceId.
  --length <value>        Optional. Length datatype facet authored as metadata.
  --precision <value>     Optional. Precision datatype facet authored as metadata.
  --scale <value>         Optional. Scale datatype facet authored as metadata.

Notes:
  Adds one BusinessReferenceKeyPart row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
  Optional datatype facets are authored as internal metadata rows.
```

### `meta-datavault-business add-reference-satellite --help`

```text
Command: add-reference-satellite
Usage:
  meta-datavault-business add-reference-satellite [--workspace <path>] --id <id> --name <value>
  [--description <value>] --reference <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. BusinessReferenceSatellite row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --reference <id>       Required. BusinessReference id for BusinessReferenceId.

Notes:
  Adds one BusinessReferenceSatellite row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-reference-satellite-attribute --help`

```text
Command: add-reference-satellite-attribute
Usage:
  meta-datavault-business add-reference-satellite-attribute [--workspace <path>] --id <id> --name
  <value> --data-type-id <value> [--ordinal <value>] --reference-satellite <id> [--length <value>]
  [--precision <value>] [--scale <value>]

Options:

  --workspace <path>          Optional. Workspace path. Default: current working directory.
  --id <id>                   Required. BusinessReferenceSatelliteAttribute row id.
  --name <value>              Required. Name.
  --data-type-id <value>      Required. DataTypeId.
  --ordinal <value>           Optional. Ordinal.
  --reference-satellite <id>  Required. BusinessReferenceSatellite id for
                              BusinessReferenceSatelliteId.
  --length <value>            Optional. Length datatype facet authored as metadata.
  --precision <value>         Optional. Precision datatype facet authored as metadata.
  --scale <value>             Optional. Scale datatype facet authored as metadata.

Notes:
  Adds one BusinessReferenceSatelliteAttribute row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
  Optional datatype facets are authored as internal metadata rows.
```

### `meta-datavault-business add-same-as-link --help`

```text
Command: add-same-as-link
Usage:
  meta-datavault-business add-same-as-link [--workspace <path>] --id <id> --name <value>
  [--description <value>] --primary-hub <id> --equivalent-hub <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. BusinessSameAsLink row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --primary-hub <id>     Required. BusinessHub id for PrimaryHubId.
  --equivalent-hub <id>  Required. BusinessHub id for EquivalentHubId.

Notes:
  Adds one BusinessSameAsLink row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-same-as-link-satellite --help`

```text
Command: add-same-as-link-satellite
Usage:
  meta-datavault-business add-same-as-link-satellite [--workspace <path>] --id <id> --name <value>
  [--description <value>] --same-as-link <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. BusinessSameAsLinkSatellite row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --same-as-link <id>    Required. BusinessSameAsLink id for BusinessSameAsLinkId.

Notes:
  Adds one BusinessSameAsLinkSatellite row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-datavault-business add-same-as-link-satellite-attribute --help`

```text
Command: add-same-as-link-satellite-attribute
Usage:
  meta-datavault-business add-same-as-link-satellite-attribute [--workspace <path>] --id <id> --name
  <value> --data-type-id <value> [--ordinal <value>] --same-as-link-satellite <id> [--length
  <value>] [--precision <value>] [--scale <value>]

Options:

  --workspace <path>             Optional. Workspace path. Default: current working directory.
  --id <id>                      Required. BusinessSameAsLinkSatelliteAttribute row id.
  --name <value>                 Required. Name.
  --data-type-id <value>         Required. DataTypeId.
  --ordinal <value>              Optional. Ordinal.
  --same-as-link-satellite <id>  Required. BusinessSameAsLinkSatellite id for
                                 BusinessSameAsLinkSatelliteId.
  --length <value>               Optional. Length datatype facet authored as metadata.
  --precision <value>            Optional. Precision datatype facet authored as metadata.
  --scale <value>                Optional. Scale datatype facet authored as metadata.

Notes:
  Adds one BusinessSameAsLinkSatelliteAttribute row to a MetaBusinessDataVault workspace.
  Defaults to the current working directory when --workspace is omitted.
  Optional datatype facets are authored as internal metadata rows.
```

## meta-transform-script

### `meta-transform-script --help`

```text
Usage:
  meta-transform-script <command> [options]

Commands:

  help                Show this help.
  from                Import SQL file/code into a new or existing workspace.
  to                  Emit SQL files or SQL code from a MetaTransformScript workspace.
  stored-procedure    View, add, and remove stored procedure contracts.
  target-identifiers  Update TransformScript target identifiers.

Next: meta-transform-script from --help
```

### `meta-transform-script from --help`

```text
Command: from
Usage:
  meta-transform-script from <source> [options]

Notes:
  Sources: sql-file, sql-files, sql-code.
  --target <sql-identifier> is optional for CREATE VIEW imports, required for bare SELECT imports, and not allowed for TVF or mutation imports.
  Specify exactly one of --new-workspace <path> or --workspace <path>.

Examples:

  meta-transform-script from sql-file --path .\SourceViews\001_customer_order_summary\view.sql --target sales.CustomerOrderSummary --new-workspace .\TransformWorkspace
  meta-transform-script from sql-file --path .\SourceViews\002_invoice_window\view.sql --target reporting.InvoiceWindow --workspace .\TransformWorkspace
  meta-transform-script from sql-files --manifest .\import-manifest.tsv --new-workspace .\TransformWorkspace --report .\import-report.tsv --verbose
  meta-transform-script from sql-code --code "select 1 as A" --name dbo.v_inline --target dbo.TargetTable --new-workspace .\TransformWorkspace

Next: meta-transform-script from sql-file --help
Next: meta-transform-script from sql-files --help
Next: meta-transform-script from sql-code --help
```

### `meta-transform-script to --help`

```text
Command: to
Usage:
  meta-transform-script to <target> [options]

Notes:
  Targets: sql-path, sql-code.

Next: meta-transform-script to sql-path --help
Next: meta-transform-script to sql-code --help
```

### `meta-transform-script stored-procedure --help`

```text
Command: stored-procedure
Usage:
  meta-transform-script stored-procedure <operation> [options]

Notes:
  Operations: view-contract, add-contract, remove-contract.
  Stored procedure bodies stay as SQL blobs; effects are declared as MetaTransformScript metadata.

Examples:

  meta-transform-script stored-procedure view-contract --workspace .\TransformWS
  meta-transform-script stored-procedure add-contract --workspace .\TransformWS --name dq.RunReview --operation 10:read:src.Customer=CustomerInput --operation 20:reset:dq.CustomerReview --operation 30:append:dq.CustomerReview --operation 40:call:audit.MarkStarted
  meta-transform-script stored-procedure remove-contract --workspace .\TransformWS --name dq.RunReview

Next: meta-transform-script stored-procedure view-contract --help
Next: meta-transform-script stored-procedure add-contract --help
Next: meta-transform-script stored-procedure remove-contract --help
```

### `meta-transform-script target-identifiers --help`

```text
Command: target-identifiers
Usage:
  meta-transform-script target-identifiers <operation> [options]

Notes:
  Operations: from-pattern.
  Target identifiers are written to ScriptObjectView.TargetSqlIdentifier in the MetaTransformScript workspace.

Examples:

  meta-transform-script target-identifiers from-pattern --workspace .\TransformWS --source-pattern "{schema}.{object}_TargetView" --target-pattern "Warehouse.{schema}.{object}"

Next: meta-transform-script target-identifiers from-pattern --help
```

### `meta-transform-script from sql-file --help`

```text
Command: from sql-file
Usage:
  meta-transform-script from sql-file --path <file.sql> [--target <sql-identifier>] (--new-workspace
  <path> | --workspace <path>)

Options:

  --path <file.sql>          Required. SQL file to import.
  --target <sql-identifier>  Optional for CREATE VIEW imports; not allowed for inline CREATE
                             FUNCTION or mutation statement imports.
  --new-workspace <path>     Create a new MetaTransformScript workspace. Mutually exclusive with
                             --workspace.
  --workspace <path>         Add one script to an existing workspace. Mutually exclusive with
                             --new-workspace.

Notes:
  Imports one .sql file at a time.
  Folder-level import is intentionally not supported.
  Bare mutation statement file names are used as transform script names.
```

### `meta-transform-script from sql-files --help`

```text
Command: from sql-files
Usage:
  meta-transform-script from sql-files --manifest <manifest.tsv> (--new-workspace <path> |
  --workspace <path>) [--report <report.tsv>] [--verbose]

Options:

  --manifest <manifest.tsv>  Required. TSV manifest with a Path column and optional Target column.
  --new-workspace <path>     Create a new MetaTransformScript workspace. Mutually exclusive with
                             --workspace.
  --workspace <path>         Add successful imports to an existing workspace. Mutually exclusive
                             with --new-workspace.
  --report <report.tsv>      Write per-file Success/Failure rows with structured failure kind,
                             summary, line, and column columns.
  --verbose                  Print one progress line per imported file.

Notes:
  Manifest paths are resolved relative to the manifest file.
  Each row is one import attempt. CREATE VIEW rows may leave Target blank; bare SELECT rows must supply Target; inline TVF and scalar UDF rows must leave Target blank.
  The command continues after per-file failures, saves successful imports once, and exits nonzero when any file failed.
```

### `meta-transform-script from sql-code --help`

```text
Command: from sql-code
Usage:
  meta-transform-script from sql-code --code <sql> [--target <sql-identifier>] (--new-workspace
  <path> | --workspace <path>) [--name <name>]

Options:

  --code <sql>               Required. SQL text to import.
  --target <sql-identifier>  Optional for CREATE VIEW imports, required for bare SELECT imports; not
                             allowed for inline CREATE FUNCTION or mutation statement imports.
  --new-workspace <path>     Create a new MetaTransformScript workspace. Mutually exclusive with
                             --workspace.
  --workspace <path>         Add one script to an existing workspace. Mutually exclusive with
                             --new-workspace.
  --name <name>              Required when the code is a bare SELECT or mutation statement without a
                             CREATE wrapper.

Notes:
  Imports SQL text into a new workspace, or appends one script to an existing workspace.
```

### `meta-transform-script to sql-path --help`

```text
Command: to sql-path
Usage:
  meta-transform-script to sql-path [--workspace <path>] --out <path>

Options:

  --workspace <path>  MetaTransformScript workspace to export. Defaults to the current directory.
  --out <path>        Required. Output .sql file or target folder.

Notes:
  Emits CREATE VIEW/CREATE FUNCTION wrappers where modeled; mutation statements emit as statements.
  If --out ends with .sql, all scripts are emitted into one file.
  Otherwise --out is treated as a target folder and must be empty or missing.
```

### `meta-transform-script to sql-code --help`

```text
Command: to sql-code
Usage:
  meta-transform-script to sql-code [--workspace <path>] [--name <name>]

Options:

  --workspace <path>  MetaTransformScript workspace to export. Defaults to the current directory.
  --name <name>       Required when the workspace contains multiple scripts.

Notes:
  Emits one modeled statement without CREATE VIEW/inline TVF wrapping; scalar function scripts emit CREATE FUNCTION wrappers.
```

### `meta-transform-script stored-procedure view-contract --help`

```text
Command: stored-procedure view-contract
Usage:
  meta-transform-script stored-procedure view-contract [--workspace <path>] [--name
  <transform-script-name>]

Options:

  --workspace <path>              MetaTransformScript workspace to inspect. Defaults to the current
                                  directory.
  --name <transform-script-name>  Inspect one stored procedure transform script by name.

Notes:
  Reports whether each stored procedure has exactly one StoredProcedureContract row.
  A present contract is authoritative: omitted operation/result rows mean none are declared.
```

### `meta-transform-script stored-procedure add-contract --help`

```text
Command: stored-procedure add-contract
Usage:
  meta-transform-script stored-procedure add-contract [--workspace <path>] --name
  <transform-script-name> [--operation <ordinal>:<kind>:<sql-id>[=<role>]] [--result-rowset <name>]
  [--result-column <rowset>=<column>]

Options:

  --workspace <path>                              MetaTransformScript workspace to update. Defaults
                                                  to the current directory.
  --name <transform-script-name>                  Required. Stored procedure transform script name.
  --operation <ordinal>:<kind>:<sql-id>[=<role>]  Declare an ordered operation. Kinds: read, append,
                                                  replace, reset, mutation, call. May be repeated.
  --result-rowset <name>                          Declare the optional result rowset.
  --result-column <rowset>=<column>               Declare a result column for a named result rowset.
                                                  May be repeated.
  --notes <text>                                  Optional human note stored on the contract.

Notes:
  This command replaces the entire contract for one stored procedure.
  Omitting --operation or --result-* declares that part empty.
  Operations are globally ordered inside the procedure. Use separate reset and append operations when order matters.
```

### `meta-transform-script stored-procedure remove-contract --help`

```text
Command: stored-procedure remove-contract
Usage:
  meta-transform-script stored-procedure remove-contract [--workspace <path>] --name
  <transform-script-name>

Options:

  --workspace <path>              MetaTransformScript workspace to update. Defaults to the current
                                  directory.
  --name <transform-script-name>  Required. Stored procedure transform script name.

Notes:
  Removes the contract row plus operation, result rowset, and result column declaration rows for the stored procedure.
```

### `meta-transform-script target-identifiers from-pattern --help`

```text
Command: target-identifiers from-pattern
Usage:
  meta-transform-script target-identifiers from-pattern [--workspace <path>] --source-pattern
  <pattern> --target-pattern <pattern> [--only-missing] [--dry-run] [--allow-empty] [--verbose]

Options:

  --workspace <path>          MetaTransformScript workspace to update. Defaults to the current
                              directory.
  --source-pattern <pattern>  Required. Pattern matched against TransformScript.Name. Tokens use
                              {name}, for example {schema}.{object}_TargetView.
  --target-pattern <pattern>  Required. Pattern rendered into TargetSqlIdentifier from captured
                              source tokens, for example Warehouse.{schema}.{object}.
  --only-missing              Skip view scripts that already have a target identifier.
  --dry-run                   Show what would change without saving the workspace.
  --allow-empty               Exit successfully even when the pattern updates no target identifiers.
  --verbose                   Print each target identifier update.

Notes:
  The source pattern is matched against the modeled transform script name.
  The target pattern is persisted as MetaTransformScript metadata, not as a side manifest.
  Target identifiers can only be set on view scripts.
```

## meta-transform-binding

### `meta-transform-binding --help`

```text
Usage:
  meta-transform-binding <command> [options]

Commands:

  help  Show this help.
  bind  Bind all transform scripts and validate against source/target schema contracts into a new
        workspace.

Next: meta-transform-binding bind --help
```

### `meta-transform-binding bind --help`

```text
Command: bind
Usage:
  meta-transform-binding bind --transform-workspace <path> --source-schema <path> [--source-schema
  <path> ...] --target-schema <path> --execute-system <name> --new-workspace <path>
  [--execute-system-default-schema-name <schema>] [--ignore-target-columns <col[,col...]>]
  [--ignore-target-columns-if-present <col[,col...]>] [--data-type-conversion-workspace <path>]
  [--allow-partial] [--partial-report <path>]

Options:

  --transform-workspace <path>                       Required. MetaTransformScript workspace to
                                                     bind.
  --source-schema <path>                             Required. Repeatable source MetaSchema
                                                     workspace.
  --target-schema <path>                             Required. Target MetaSchema workspace.
  --execute-system <name>                            Required. Execution context for one/two-part
                                                     source identifiers.
  --new-workspace <path>                             Required. Directory where the binding workspace
                                                     will be created.
  --execute-system-default-schema-name <schema>      Required when any one-part source identifier
                                                     exists.
  --ignore-target-columns <col[,col...]>             Optional comma-separated target columns to
                                                     exclude from target conformance checks.
  --ignore-target-columns-if-present <col[,col...]>  Optional comma-separated target columns to
                                                     exclude only on target tables where they exist.
  --data-type-conversion-workspace <path>            Optional sanctioned conversion policy
                                                     workspace. Omitted uses built-in defaults.
  --allow-partial                                    Optional. Save only objects that bind and
                                                     validate successfully; skipped objects are
                                                     failures.
  --partial-report <path>                            Optional TSV report for objects skipped due to
                                                     binding or validation failure. Requires
                                                     --allow-partial.

Notes:
  bind is atomic: it binds and validates in one run.
  If binding or validation fails, no binding workspace is created.
  --allow-partial is an explicit corpus/discovery mode: objects with binding or validation failures are skipped and successful bindings are saved.
  bind processes all transform scripts in the transform workspace.
  Target SQL identifier is read from ScriptObjectView.TargetSqlIdentifier.
  Source schema workspaces are repeatable; target schema workspace is single.
  Every schema workspace must contain exactly one system.
  --execute-system-default-schema-name is required when any one-part source identifier exists.
  --ignore-target-columns excludes named non-identity target columns from target conformance checks.
  Ignored names must exist on each target table or bind fails explicitly.
  --ignore-target-columns-if-present excludes named non-identity target columns only on target tables where they exist.

Examples:

  meta-transform-binding bind --transform-workspace .\TransformWS --source-schema .\SourceSchemaWS --target-schema .\TargetSchemaWS --execute-system SalesDb --new-workspace .\BindingWS
  meta-transform-binding bind --transform-workspace .\TransformWS --source-schema .\SalesSchemaWS --source-schema .\ReferenceSchemaWS --target-schema .\WarehouseSchemaWS --execute-system WarehouseDb --execute-system-default-schema-name dbo --new-workspace .\BindingWS --ignore-target-columns LoadUtc,RunId --ignore-target-columns-if-present UpdateAudit_ID
  meta-transform-binding bind --transform-workspace .\TransformWS --source-schema .\SourceSchemaWS --target-schema .\TargetSchemaWS --execute-system SalesDb --execute-system-default-schema-name dbo --new-workspace .\BindingWS --allow-partial --partial-report .\binding-partial.tsv
```

## meta-data-quality

### `meta-data-quality --help`

```text
Usage:
  meta-data-quality <command> [options]

Commands:

  help                      Show this help.
  from-transform-workspace  Create generated DQ views from a full MetaTransformScript workspace.
  inspect                   Review the generated DQ pack and optional adjustments.
  promote                   Promote generated DQ candidates for SQL output.

Next: meta-data-quality from-transform-workspace --help
```

### `meta-data-quality from-transform-workspace --help`

```text
Command: from-transform-workspace
Usage:
  meta-data-quality from-transform-workspace --transform-workspace <path> --new-workspace <path>
  [--binding-workspace <path>]

Options:

  --transform-workspace <path>  Required. MetaTransformScript workspace to analyze.
  --new-workspace <path>        Required. Directory where the generated MetaDataQuality workspace
                                will be created.
  --binding-workspace <path>    Optional. MetaTransformBinding workspace used to scan only validated
                                scripts.

Notes:
  Scans all TransformScript instances in one workspace.
  When --binding-workspace is supplied, only TransformScript rows with Validation-backed TransformBinding rows are scanned.
  Creates one MetaDataQuality workspace with generated DQ views.
```

### `meta-data-quality inspect --help`

```text
Command: inspect
Usage:
  meta-data-quality inspect --workspace <path> [--show-cases] [--top-cases <n>]
  [--show-candidate-ids]

Options:

  --workspace <path>    Required. MetaDataQuality workspace to inspect.
  --show-cases          Optional. Show candidate adjustment cases.
  --top-cases <n>       Optional. Show up to n candidate cases. Implies --show-cases. Default: 20.
  --show-candidate-ids  Optional. Include candidate ids in adjustment output. Implies --show-cases.

Notes:
  Default output guides the full-pack-first path.
  Use --show-cases when you want to make small adjustments before SQL generation.
  Use --show-candidate-ids when promoting individual generated candidates.
```

### `meta-data-quality promote --help`

```text
Command: promote
Usage:
  meta-data-quality promote --workspace <path> (--all | --candidate-id <id> [--candidate-id <id>
  ...])

Options:

  --workspace <path>   Required. MetaDataQuality workspace to update.
  --all                Promote every generated data-quality candidate.
  --candidate-id <id>  Promote one generated candidate. May be provided more than once.

Notes:
  Promotes generated DQ candidates for data-quality-to-sql output.
```

## meta-pipeline

### `meta-pipeline --help`

```text
Usage:
  meta-pipeline --new-workspace <path>
Usage:
  meta-pipeline <command> [options]

Commands:

  execute             Execute a modeled pipeline's serial task chain.
  execute-step        Execute one modeled transform-backed pipeline step.
  execute-sqlserver   Execute the direct SQL Server runtime slice.
  create-pipeline-db  Create or update the MetaPipeline operational DB.
  prune-pipeline-db   Prune old MetaPipeline operational diagnostic logs.
  add-pipeline        Add one Pipeline instance to a MetaPipeline workspace.
  add-step            Add one transform-backed step to a pipeline.
  inspect             Show a compact MetaPipeline workspace summary.
  help                Show this help.

Notes:
  --new-workspace creates an empty sanctioned MetaPipeline workspace.

Next: meta-pipeline add-pipeline --help
```

### `meta-pipeline execute --help`

```text
Command: execute
Usage:
  meta-pipeline execute --workspace <path> --pipeline <name> --transform-workspace <path>
  --binding-workspace <path> [--data-type-conversion-workspace <path>] [--pipeline-db-connection-env
  <name>]

Options:

  --workspace <path>                       Required. MetaPipeline workspace that contains the
                                           modeled serial task chain.
  --pipeline <name>                        Required. Pipeline name to execute.
  --transform-workspace <path>             Required. MetaTransformScript workspace used by transform
                                           tasks.
  --binding-workspace <path>               Required. MetaTransformBinding workspace used by
                                           transform tasks.
  --data-type-conversion-workspace <path>  Optional conversion policy workspace; omitted uses the
                                           built-in defaults.
  --pipeline-db-connection-env <name>      Optional shell-visible environment variable for an
                                           initialized MetaPipeline operational DB.

Notes:
  Executes the serial PipelineTask chain declared in a MetaPipeline workspace.
  Every transform task requires a binding workspace.
  SELECT-kind scripts must feed exactly one InsertRows target write.
  Non-SELECT scripts execute directly and must not feed a TargetWrite task.
  Connection references in the model name shell-visible environment variables.
  The command resolves those variable names to connection strings at runtime.
  SELECT-kind InsertRows tasks use their modeled target data type system; omitted defaults to SqlServer.
  --pipeline-db-connection-env records diagnostic logs, audit logs, task runs, metrics, fingerprints, audit ids, and failures in an initialized operational DB.
  In an attached console, execution shows compact live progress with step count, elapsed time, rows, batches, and B/KB/MB/GB rate.
  The command validates the modeled pipeline before execution.

Examples:

  meta-pipeline execute --workspace .\PipelineWS --pipeline CustomerLoad --transform-workspace .\TransformWS --binding-workspace .\BindingWS
```

### `meta-pipeline execute-step --help`

```text
Command: execute-step
Usage:
  meta-pipeline execute-step --workspace <path> --pipeline <name> --step-name <name-or-id>
  --transform-workspace <path> --binding-workspace <path> [--data-type-conversion-workspace <path>]
  [--pipeline-db-connection-env <name>]

Options:

  --workspace <path>                       Required. MetaPipeline workspace that contains the
                                           modeled step.
  --pipeline <name>                        Required. Pipeline name containing the step.
  --step-name <name-or-id>                 Required. Pipeline task name or id to execute.
  --transform-workspace <path>             Required. MetaTransformScript workspace used by the step.
  --binding-workspace <path>               Required. MetaTransformBinding workspace used by the
                                           step.
  --data-type-conversion-workspace <path>  Optional conversion policy workspace; omitted uses the
                                           built-in defaults.
  --pipeline-db-connection-env <name>      Optional shell-visible environment variable for an
                                           initialized MetaPipeline operational DB.

Notes:
  Executes exactly one transform-backed PipelineTask declared in a MetaPipeline workspace.
  The command does not traverse predecessor or successor tasks.
  SELECT-kind steps execute their paired InsertRows target write when modeled.
  Non-SELECT steps execute directly through the modeled execution connection.
  Connection references in the model name shell-visible environment variables.
  This command is the task-grain runtime surface used by MetaOrchestration scheduling.

Examples:

  meta-pipeline execute-step --workspace .\PipelineWS --pipeline CustomerLoad --step-name load-customers --transform-workspace .\TransformWS --binding-workspace .\BindingWS
```

### `meta-pipeline execute-sqlserver --help`

```text
Command: execute-sqlserver
Usage:
  meta-pipeline execute-sqlserver --transform-workspace <path> --binding-workspace <path> --script
  <name-or-id> [--binding <id>] --execution-connection-env <name> [--target-connection-env <name>]
  [--target <sql-identifier>] [--batch-size <n>] [--timeout-seconds <n>] [--target-data-type-system
  <name>] [--data-type-conversion-workspace <path>] [--pipeline-db-connection-env <name>]

Options:

  --transform-workspace <path>             Required. MetaTransformScript workspace containing the
                                           script.
  --binding-workspace <path>               Required. MetaTransformBinding workspace containing
                                           script binding rows.
  --script <name-or-id>                    Required. TransformScript.Name or TransformScript.Id to
                                           execute.
  --binding <id>                           Optional binding id when the selected script has multiple
                                           bindings.
  --execution-connection-env <name>        Required. Shell-visible environment variable for the
                                           execution SQL Server connection.
  --target-connection-env <name>           Required for SELECT-kind scripts. Shell-visible
                                           environment variable for the target SQL Server
                                           connection.
  --target <sql-identifier>                Target table identifier when a SELECT binding has
                                           multiple targets.
  --batch-size <n>                         Bounded in-memory row buffer size. Default: 1000.
  --timeout-seconds <n>                    SQL command and bulk-copy timeout seconds. Omitted means
                                           no command timeout.
  --target-data-type-system <name>         Runtime target type family for InsertRows. Default:
                                           SqlServer.
  --data-type-conversion-workspace <path>  Optional conversion policy workspace; omitted uses the
                                           built-in defaults.
  --pipeline-db-connection-env <name>      Optional shell-visible environment variable for an
                                           initialized MetaPipeline operational DB.

Notes:
  Executes one transform script against SQL Server.
  --script resolves exact TransformScript.Name first, then exact TransformScript.Id.
  If exactly one binding references the selected script, --binding can be omitted.
  Use --binding only when the selected script has multiple bindings.
  SELECT-kind scripts additionally require --target-connection-env.
  Non-SELECT scripts execute directly through the execution connection and do not use a target.
  If a SELECT binding contains multiple targets, --target is required.
  Connection env options name shell-visible environment variables.
  The command resolves those variable names to connection strings at runtime.
  Stage 1 execution supports parameterless transform scripts and one selected target per run.
  --data-type-conversion-workspace selects the conversion policy workspace; omitted uses the built-in defaults.
  --pipeline-db-connection-env records diagnostic logs, audit logs, task runs, metrics, fingerprints, audit ids, and failures in an initialized operational DB.
  In an attached console, execution shows compact live progress with step count, elapsed time, rows, batches, and B/KB/MB/GB rate.

Examples:

  meta-pipeline execute-sqlserver --transform-workspace .\TransformWS --binding-workspace .\BindingWS --script dbo.v_customer_load --execution-connection-env EXECUTION_DB --target-connection-env TARGET_DB
```

### `meta-pipeline create-pipeline-db --help`

```text
Command: create-pipeline-db
Usage:
  meta-pipeline create-pipeline-db --pipeline-db-connection-env <name> [--pipeline-db-name <name>]

Options:

  --pipeline-db-connection-env <name>  Required. Shell-visible environment variable with a SQL
                                       Server connection string that can create the database.
  --pipeline-db-name <name>            Operational database name. Default: MetaPipeline.

Notes:
  Creates the SQL Server MetaPipeline operational database if needed and creates or updates its operational schema.
  --pipeline-db-name defaults to MetaPipeline.
  The operational DB stores diagnostic logs, audit logs, metrics, task runs, workspace fingerprints, audit ids, and failures only.
  It does not store model truth, scheduling state, watermarks, checkpoints, or orchestration semantics.

Examples:

  meta-pipeline create-pipeline-db --pipeline-db-connection-env META_PIPELINE_SQLSERVER --pipeline-db-name MetaPipeline
```

### `meta-pipeline prune-pipeline-db --help`

```text
Command: prune-pipeline-db
Usage:
  meta-pipeline prune-pipeline-db --pipeline-db-connection-env <name> --retention-days <days>
  [--dry-run]

Options:

  --pipeline-db-connection-env <name>  Required. Shell-visible environment variable for the
                                       initialized MetaPipeline operational DB.
  --retention-days <days>              Required. Delete eligible diagnostic rows older than this
                                       retention window.
  --dry-run                            Report eligible rows without deleting them.

Notes:
  Deletes only RunDiagnosticsLog rows for completed runs older than the retention window.
  PipelineRun, TaskRun, RunMetric, RunLog, RunFingerprint, RunFailure, and audit ids are preserved for audit lineage.
  Running runs are not touched because only completed runs with CompletedAtUtc older than the cutoff are eligible.
  This is explicit maintenance; meta-pipeline does not install SQL Agent jobs.

Examples:

  meta-pipeline prune-pipeline-db --pipeline-db-connection-env META_PIPELINE_DB --retention-days 30 --dry-run
```

### `meta-pipeline add-pipeline --help`

```text
Command: add-pipeline
Usage:
  meta-pipeline add-pipeline --workspace <path> --name <name> [--description <text>]

Options:

  --workspace <path>    Required. Existing MetaPipeline workspace to update.
  --name <name>         Required. Pipeline name.
  --description <text>  Optional pipeline description.

Notes:
  Adds one Pipeline instance to an existing MetaPipeline workspace.
```

### `meta-pipeline add-step --help`

```text
Command: add-step
Usage:
  meta-pipeline add-step --workspace <path> --pipeline <name> --script <name-or-id>
  --transform-workspace <path> --binding-workspace <path> --execution-connection-env <name>
  [--step-name <name>] [--binding <id>] [--target-connection-env <name>] [--target <sql-identifier>]
  [--target-write <insert-rows>] [--batch-size <n>] [--timeout-seconds <n>]
  [--target-data-type-system <name>]

Options:

  --workspace <path>                 Required. Existing MetaPipeline workspace to update.
  --pipeline <name>                  Required. Pipeline that receives the new serial step.
  --script <name-or-id>              Required. TransformScript.Name or TransformScript.Id to model.
  --transform-workspace <path>       Required. MetaTransformScript workspace used for script
                                     selection.
  --binding-workspace <path>         Required. MetaTransformBinding workspace used for binding
                                     selection.
  --execution-connection-env <name>  Required. Shell-visible environment variable named by the
                                     modeled execution connection.
  --step-name <name>                 Optional step name; omitted derives a deterministic name from
                                     the script name.
  --binding <id>                     Optional binding id when the selected script has multiple
                                     bindings.
  --target-connection-env <name>     Required for SELECT-kind scripts. Shell-visible environment
                                     variable named by the modeled target connection.
  --target <sql-identifier>          Target table identifier when a SELECT binding has multiple
                                     targets.
  --target-write <insert-rows>       SELECT-kind target write model. The only supported value is
                                     insert-rows.
  --batch-size <n>                   Bounded in-memory row buffer size for InsertRows. Default:
                                     1000.
  --timeout-seconds <n>              SQL command and bulk-copy timeout seconds for the transform
                                     execution.
  --target-data-type-system <name>   InsertRows target type family. Default: SqlServer.

Notes:
  Appends transform-backed task instances to the pipeline's serial task chain.
  --script resolves exact TransformScript.Name first, then exact TransformScript.Id.
  If exactly one binding references the selected script, --binding can be omitted.
  Use --binding only when the selected script has multiple bindings.
  SELECT-kind scripts require target options; add-step records a row stream and InsertRows target write.
  Non-SELECT scripts record only a TransformExecution task and execution connection.
  If a SELECT binding contains multiple targets, --target is required.
  Connection env options name shell-visible environment variables; connection strings are not stored.
  Use meta-pipeline execute to execute the modeled transform task.

Examples:

  meta-pipeline add-step --workspace .\PipelineWS --pipeline CustomerLoad --step-name load-customers --script dbo.v_customer_load --transform-workspace .\TransformWS --binding-workspace .\BindingWS --execution-connection-env EXECUTION_DB --target-connection-env TARGET_DB --target dbo.TargetCustomer --target-write insert-rows --batch-size 1000
```

### `meta-pipeline inspect --help`

```text
Command: inspect
Usage:
  meta-pipeline inspect --workspace <path>

Options:

  --workspace <path>  Required. MetaPipeline workspace to inspect.

Notes:
  Loads a MetaPipeline workspace and prints pipeline/task instance counts.
```

## meta-orchestration

### `meta-orchestration --help`

```text
Usage:
  meta-orchestration --pipeline-workspace <path> --transform-workspace <path> --binding-workspace
  <path> --new-workspace <path> [--description <text>]
Usage:
  meta-orchestration <command> [options]

Commands:

  inspect                  Inspect a MetaOrchestration workspace.
  list-issues              List analyzer issues recorded in an orchestration workspace.
  explain-issue            Explain one analyzer issue and its participating pipelines.
  add-dependency           Record an explicit success or failure dependency between tasks.
  add-order                Record an explicit success dependency/order resolution in an
                           orchestration workspace.
  allow-concurrent-append  Allow concurrent execution for multiple Append effects on one object.
  set-lock-policy          Record scoped lock compatibility for one object/effect interaction.
  refresh-run-plan         Refresh lock-aware topological run-plan rows in an orchestration
                           workspace.
  inspect-run-plan         Inspect planned task order.
  execute                  Execute the current run plan by supervising meta-pipeline execute-step
                           workers.
  help                     Show this help.

Notes:
  --new-workspace creates a MetaOrchestration workspace by inferring from bound MetaPipeline transform steps.
  Binding must already exist; orchestration does not parse or bind SQL itself.
  The workspace separates dependency DAG status from determinism and synchronization status.
  Data dependencies are inferred from published producers to dependency consumers.
  Same-object writer interactions become determinism or synchronization issues instead of artificial dependency edges.

Next: meta-orchestration refresh-run-plan --help
```

### `meta-orchestration inspect --help`

```text
Command: inspect
Usage:
  meta-orchestration inspect --workspace <path>

Options:

  --workspace <path>  Required. MetaOrchestration workspace to inspect.

Notes:
  Shows DAG, determinism, synchronization, dependency, effect, and issue summaries.
```

### `meta-orchestration list-issues --help`

```text
Command: list-issues
Usage:
  meta-orchestration list-issues --workspace <path>

Options:

  --workspace <path>  Required. MetaOrchestration workspace to inspect.

Notes:
  Lists dependency, determinism, synchronization, and policy issues without changing analysis evidence.
```

### `meta-orchestration explain-issue --help`

```text
Command: explain-issue
Usage:
  meta-orchestration explain-issue --workspace <path> --issue <id-or-unique-code>

Options:

  --workspace <path>           Required. MetaOrchestration workspace to inspect.
  --issue <id-or-unique-code>  Required. Issue id or unique issue code.

Notes:
  Shows issue domain, severity, blocking flags, object, message, and participating pipelines.
```

### `meta-orchestration add-dependency --help`

```text
Command: add-dependency
Usage:
  meta-orchestration add-dependency --workspace <path> --from-task <task> --to-task <task>
  --condition success|failure [--object <sql-identifier>] [--reason <text>]

Options:

  --workspace <path>           Required. MetaOrchestration workspace to update.
  --from-task <task>           Required. Predecessor task selector.
  --to-task <task>             Required. Successor task selector.
  --condition success|failure  Required. Whether the successor follows predecessor success or
                               failure.
  --object <sql-identifier>    Optional object selector for object-scoped dependency resolution.
  --reason <text>              Optional reason recorded with the policy row.

Notes:
  Adds an explicit conditional DAG edge between planned tasks.
  Success edges run the successor only when the predecessor succeeds.
  Failure edges run the successor only when the predecessor fails.
  Task selectors may be task id, task name, MetaPipeline task id, or Pipeline.Task.
```

### `meta-orchestration add-order --help`

```text
Command: add-order
Usage:
  meta-orchestration add-order --workspace <path> --from-task <task> --to-task <task> [--condition
  success|failure] [--object <sql-identifier>] [--reason <text>]

Options:

  --workspace <path>           Required. MetaOrchestration workspace to update.
  --from-task <task>           Required. Predecessor task selector.
  --to-task <task>             Required. Successor task selector.
  --condition success|failure  Optional dependency condition. Default: success.
  --object <sql-identifier>    Optional object selector for object-scoped dependency resolution.
  --reason <text>              Optional reason recorded with the policy row.

Notes:
  Adds an explicit task dependency resolution for a determinism issue.
  The default condition is success. Use add-dependency when authoring failure branches.
  Failure dependencies are graph edges, not post-run action hooks.
  Task selectors may be task id, task name, MetaPipeline task id, or Pipeline.Task.
```

### `meta-orchestration allow-concurrent-append --help`

```text
Command: allow-concurrent-append
Usage:
  meta-orchestration allow-concurrent-append --workspace <path> --object <sql-identifier> [--reason
  <text>]

Options:

  --workspace <path>         Required. MetaOrchestration workspace to update.
  --object <sql-identifier>  Required. Data object whose append writers can overlap.
  --reason <text>            Optional reason recorded with the policy row.

Notes:
  Adds a scoped Append/Append lock compatibility policy for concurrent append writers.
```

### `meta-orchestration set-lock-policy --help`

```text
Command: set-lock-policy
Usage:
  meta-orchestration set-lock-policy --workspace <path> --object <sql-identifier> --left-effect
  <effect> --right-effect <effect> --behavior <serialize|allow> [--reason <text>]

Options:

  --workspace <path>            Required. MetaOrchestration workspace to update.
  --object <sql-identifier>     Required. Data object whose effect interaction is being resolved.
  --left-effect <effect>        Required. Left write effect, such as Append, Replace, Mutation,
                                KeyedUpsert, or ConditionalKeyedUpsert.
  --right-effect <effect>       Required. Right write effect, such as Append, Replace, Mutation,
                                KeyedUpsert, or ConditionalKeyedUpsert.
  --behavior <serialize|allow>  Required. Lock behavior for the object/effect pair.
  --reason <text>               Optional reason recorded with the policy row.

Notes:
  Adds or updates scoped lock compatibility for an object/effect interaction.
  allow is currently accepted only for Append/Append; use serialize for other pairs.
```

### `meta-orchestration refresh-run-plan --help`

```text
Command: refresh-run-plan
Usage:
  meta-orchestration refresh-run-plan --workspace <path>

Options:

  --workspace <path>  Required. MetaOrchestration workspace to update.

Notes:
  Writes the run plan, planned tasks, and task locks into the existing orchestration workspace.
  The DAG must be complete and run-planning policy must resolve blocking determinism/synchronization issues.
  Execute refreshes the run plan automatically; this command is for preflight and inspection workflows.
```

### `meta-orchestration inspect-run-plan --help`

```text
Command: inspect-run-plan
Usage:
  meta-orchestration inspect-run-plan --workspace <path>

Options:

  --workspace <path>  Required. MetaOrchestration workspace to inspect.

Notes:
  Shows the run-plan shape as a compact tree.
  Use issue/policy inspection commands when you need the reasoning behind the plan.
```

### `meta-orchestration execute --help`

```text
Command: execute
Usage:
  meta-orchestration execute --workspace <path> --pipeline-workspace <path> --transform-workspace
  <path> --binding-workspace <path> [--data-type-conversion-workspace <path>]
  [--pipeline-db-connection-env <name>] [--max-degree-of-parallelism <n>]

Options:

  --workspace <path>                       Required. MetaOrchestration workspace containing the
                                           analysis and run-plan rows.
  --pipeline-workspace <path>              Required. MetaPipeline workspace used by child
                                           execute-step workers.
  --transform-workspace <path>             Required. MetaTransformScript workspace used by child
                                           execute-step workers.
  --binding-workspace <path>               Required. MetaTransformBinding workspace used by child
                                           execute-step workers.
  --data-type-conversion-workspace <path>  Optional conversion policy workspace passed to child
                                           workers.
  --pipeline-db-connection-env <name>      Optional operational DB connection env passed to child
                                           workers.
  --max-degree-of-parallelism <n>          Maximum concurrent child meta-pipeline workers. Default:
                                           1.

Notes:
  Refreshes run-plan rows from current workspace state, then executes the run plan.
  Each planned task is run by launching meta-pipeline execute-step.
  Failed tasks block OnSuccess dependents, enable OnFailure branches, and leave unrelated paths running.
  Task dependencies and locks define runtime eligibility; --max-degree-of-parallelism throttles child processes.
  MetaPipeline remains the owner of transform execution and operational DB evidence.
```

## meta-data-warehouse

### `meta-data-warehouse --help`

```text
Usage:
  meta-data-warehouse [--new-workspace <path> | <command> [options]]

Commands:

  help                                 Show this help.
  --new-workspace                      Create an empty MetaDataWarehouse workspace.
  add-accumulating-snapshot-fact       Mark a fact as an accumulating snapshot.
  add-accumulating-snapshot-milestone  Add a lifecycle milestone to an accumulating snapshot.
  add-aggregate-fact                   Declare an aggregate fact derived from a source fact.
  add-bridge                           Add a dimensional bridge table.
  add-bridge-participant               Add a dimension participant to a bridge.
  add-bridge-weight                    Add a bridge weighting measure with a Meta data type.
  add-conformed-dimension              Mark a dimension as conformed.
  add-degenerate-dimension             Add a degenerate dimension value to a fact.
  add-dimension                        Add a dimension.
  add-dimension-attribute              Add a dimension attribute with a Meta data type.
  add-dimension-business-key           Add a dimension business key.
  add-dimension-business-key-part      Add an ordered attribute to a dimension business key.
  add-dimension-hierarchy              Add a dimension hierarchy.
  add-dimension-hierarchy-level        Add a hierarchy level.
  add-fact                             Add a fact table concept.
  add-fact-bridge                      Connect a fact to a bridge table.
  add-fact-dimension                   Add a dimensional role to a fact.
  add-fact-grain                       Declare a fact grain.
  add-fact-measure                     Add a typed fact measure.
  add-factless-fact                    Mark a fact as factless.
  add-junk-dimension                   Declare a junk dimension.
  add-junk-dimension-component         Add an attribute component to a junk dimension.
  add-mini-dimension                   Declare a mini-dimension relationship.
  add-outrigger-dimension              Declare an outrigger dimension relationship.
  add-periodic-snapshot-fact           Mark a fact as a periodic snapshot.
  add-slowly-changing-dimension        Declare SCD behavior for a dimension.
  add-transaction-fact                 Mark a fact as transaction-grain.
  add-type1-dimension-attribute        Declare a Type 1 attribute in an SCD dimension.
  add-type2-dimension-attribute        Declare a Type 2 attribute in an SCD dimension.
  add-warehouse                        Add a dimensional warehouse.

Notes:
  MetaDataWarehouse owns logical dimensional warehouse concepts; SQL Server realization belongs to conversion and deploy tooling.

Next: meta-data-warehouse add-dimension --help
```

### `meta-data-warehouse --new-workspace --help`

```text
Command: --new-workspace
Usage:
  meta-data-warehouse --new-workspace <path>

Options:

  --new-workspace <path>  Required. Directory where the empty MetaDataWarehouse workspace will be
                          created.
```

### `meta-data-warehouse add-accumulating-snapshot-fact --help`

```text
Command: add-accumulating-snapshot-fact
Usage:
  meta-data-warehouse add-accumulating-snapshot-fact [--workspace <path>] --id <id> [--description
  <value>] --fact <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. AccumulatingSnapshotFact row id.
  --description <value>  Optional. Description.
  --fact <id>            Required. Fact id for FactId.

Notes:
  Adds one AccumulatingSnapshotFact row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-accumulating-snapshot-milestone --help`

```text
Command: add-accumulating-snapshot-milestone
Usage:
  meta-data-warehouse add-accumulating-snapshot-milestone [--workspace <path>] --id <id> --name
  <value> [--ordinal <value>] --date-role-name <value> [--description <value>]
  --accumulating-snapshot <id>

Options:

  --workspace <path>            Optional. Workspace path. Default: current working directory.
  --id <id>                     Required. AccumulatingSnapshotMilestone row id.
  --name <value>                Required. Name.
  --ordinal <value>             Optional. Ordinal.
  --date-role-name <value>      Required. DateRoleName.
  --description <value>         Optional. Description.
  --accumulating-snapshot <id>  Required. AccumulatingSnapshotFact id for
                                AccumulatingSnapshotFactId.

Notes:
  Adds one AccumulatingSnapshotMilestone row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-aggregate-fact --help`

```text
Command: add-aggregate-fact
Usage:
  meta-data-warehouse add-aggregate-fact [--workspace <path>] --id <id> [--description <value>]
  --aggregated-fact <id> --source-fact <id>

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. AggregateFact row id.
  --description <value>   Optional. Description.
  --aggregated-fact <id>  Required. Fact id for AggregatedFactId.
  --source-fact <id>      Required. Fact id for SourceFactId.

Notes:
  Adds one AggregateFact row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-bridge --help`

```text
Command: add-bridge
Usage:
  meta-data-warehouse add-bridge [--workspace <path>] --id <id> --name <value> [--description
  <value>] --warehouse <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. BridgeTable row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --warehouse <id>       Required. Warehouse id for WarehouseId.

Notes:
  Adds one BridgeTable row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-bridge-participant --help`

```text
Command: add-bridge-participant
Usage:
  meta-data-warehouse add-bridge-participant [--workspace <path>] --id <id> --role-name <value>
  [--ordinal <value>] [--is-required <value>] --bridge <id> --dimension <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. BridgeParticipant row id.
  --role-name <value>    Required. RoleName.
  --ordinal <value>      Optional. Ordinal.
  --is-required <value>  Optional. IsRequired.
  --bridge <id>          Required. BridgeTable id for BridgeTableId.
  --dimension <id>       Required. Dimension id for DimensionId.

Notes:
  Adds one BridgeParticipant row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-bridge-weight --help`

```text
Command: add-bridge-weight
Usage:
  meta-data-warehouse add-bridge-weight [--workspace <path>] --id <id> --name <value> --data-type-id
  <value> [--description <value>] --bridge <id>

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. BridgeWeight row id.
  --name <value>          Required. Name.
  --data-type-id <value>  Required. DataTypeId.
  --description <value>   Optional. Description.
  --bridge <id>           Required. BridgeTable id for BridgeTableId.

Notes:
  Adds one BridgeWeight row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-conformed-dimension --help`

```text
Command: add-conformed-dimension
Usage:
  meta-data-warehouse add-conformed-dimension [--workspace <path>] --id <id> --conformance-name
  <value> [--description <value>] --dimension <id>

Options:

  --workspace <path>          Optional. Workspace path. Default: current working directory.
  --id <id>                   Required. ConformedDimension row id.
  --conformance-name <value>  Required. ConformanceName.
  --description <value>       Optional. Description.
  --dimension <id>            Required. Dimension id for DimensionId.

Notes:
  Adds one ConformedDimension row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-degenerate-dimension --help`

```text
Command: add-degenerate-dimension
Usage:
  meta-data-warehouse add-degenerate-dimension [--workspace <path>] --id <id> --name <value>
  --data-type-id <value> [--ordinal <value>] [--description <value>] --fact <id>

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. DegenerateDimension row id.
  --name <value>          Required. Name.
  --data-type-id <value>  Required. DataTypeId.
  --ordinal <value>       Optional. Ordinal.
  --description <value>   Optional. Description.
  --fact <id>             Required. Fact id for FactId.

Notes:
  Adds one DegenerateDimension row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-dimension --help`

```text
Command: add-dimension
Usage:
  meta-data-warehouse add-dimension [--workspace <path>] --id <id> --name <value> [--description
  <value>] --warehouse <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. Dimension row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --warehouse <id>       Required. Warehouse id for WarehouseId.

Notes:
  Adds one Dimension row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-dimension-attribute --help`

```text
Command: add-dimension-attribute
Usage:
  meta-data-warehouse add-dimension-attribute [--workspace <path>] --id <id> --name <value>
  --data-type-id <value> [--ordinal <value>] [--is-nullable <value>] [--description <value>]
  --dimension <id>

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. DimensionAttribute row id.
  --name <value>          Required. Name.
  --data-type-id <value>  Required. DataTypeId.
  --ordinal <value>       Optional. Ordinal.
  --is-nullable <value>   Optional. IsNullable.
  --description <value>   Optional. Description.
  --dimension <id>        Required. Dimension id for DimensionId.

Notes:
  Adds one DimensionAttribute row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-dimension-business-key --help`

```text
Command: add-dimension-business-key
Usage:
  meta-data-warehouse add-dimension-business-key [--workspace <path>] --id <id> --name <value>
  [--description <value>] --dimension <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. DimensionBusinessKey row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --dimension <id>       Required. Dimension id for DimensionId.

Notes:
  Adds one DimensionBusinessKey row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-dimension-business-key-part --help`

```text
Command: add-dimension-business-key-part
Usage:
  meta-data-warehouse add-dimension-business-key-part [--workspace <path>] --id <id> [--ordinal
  <value>] --business-key <id> --attribute <id>

Options:

  --workspace <path>   Optional. Workspace path. Default: current working directory.
  --id <id>            Required. DimensionBusinessKeyPart row id.
  --ordinal <value>    Optional. Ordinal.
  --business-key <id>  Required. DimensionBusinessKey id for DimensionBusinessKeyId.
  --attribute <id>     Required. DimensionAttribute id for DimensionAttributeId.

Notes:
  Adds one DimensionBusinessKeyPart row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-dimension-hierarchy --help`

```text
Command: add-dimension-hierarchy
Usage:
  meta-data-warehouse add-dimension-hierarchy [--workspace <path>] --id <id> --name <value>
  [--description <value>] --dimension <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. DimensionHierarchy row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --dimension <id>       Required. Dimension id for DimensionId.

Notes:
  Adds one DimensionHierarchy row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-dimension-hierarchy-level --help`

```text
Command: add-dimension-hierarchy-level
Usage:
  meta-data-warehouse add-dimension-hierarchy-level [--workspace <path>] --id <id> --name <value>
  [--ordinal <value>] --hierarchy <id> --attribute <id>

Options:

  --workspace <path>  Optional. Workspace path. Default: current working directory.
  --id <id>           Required. DimensionHierarchyLevel row id.
  --name <value>      Required. Name.
  --ordinal <value>   Optional. Ordinal.
  --hierarchy <id>    Required. DimensionHierarchy id for DimensionHierarchyId.
  --attribute <id>    Required. DimensionAttribute id for DimensionAttributeId.

Notes:
  Adds one DimensionHierarchyLevel row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-fact --help`

```text
Command: add-fact
Usage:
  meta-data-warehouse add-fact [--workspace <path>] --id <id> --name <value> [--description <value>]
  --warehouse <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. Fact row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --warehouse <id>       Required. Warehouse id for WarehouseId.

Notes:
  Adds one Fact row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-fact-bridge --help`

```text
Command: add-fact-bridge
Usage:
  meta-data-warehouse add-fact-bridge [--workspace <path>] --id <id> --role-name <value> [--ordinal
  <value>] [--description <value>] --fact <id> --bridge <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. FactBridge row id.
  --role-name <value>    Required. RoleName.
  --ordinal <value>      Optional. Ordinal.
  --description <value>  Optional. Description.
  --fact <id>            Required. Fact id for FactId.
  --bridge <id>          Required. BridgeTable id for BridgeTableId.

Notes:
  Adds one FactBridge row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-fact-dimension --help`

```text
Command: add-fact-dimension
Usage:
  meta-data-warehouse add-fact-dimension [--workspace <path>] --id <id> --role-name <value>
  [--ordinal <value>] [--is-required <value>] [--description <value>] --fact <id> --dimension <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. FactDimension row id.
  --role-name <value>    Required. RoleName.
  --ordinal <value>      Optional. Ordinal.
  --is-required <value>  Optional. IsRequired.
  --description <value>  Optional. Description.
  --fact <id>            Required. Fact id for FactId.
  --dimension <id>       Required. Dimension id for DimensionId.

Notes:
  Adds one FactDimension row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-fact-grain --help`

```text
Command: add-fact-grain
Usage:
  meta-data-warehouse add-fact-grain [--workspace <path>] --id <id> --name <value> --description
  <value> --fact <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. FactGrain row id.
  --name <value>         Required. Name.
  --description <value>  Required. Description.
  --fact <id>            Required. Fact id for FactId.

Notes:
  Adds one FactGrain row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-fact-measure --help`

```text
Command: add-fact-measure
Usage:
  meta-data-warehouse add-fact-measure [--workspace <path>] --id <id> --name <value> --data-type-id
  <value> [--ordinal <value>] [--is-nullable <value>] [--description <value>] --fact <id>

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. FactMeasure row id.
  --name <value>          Required. Name.
  --data-type-id <value>  Required. DataTypeId.
  --ordinal <value>       Optional. Ordinal.
  --is-nullable <value>   Optional. IsNullable.
  --description <value>   Optional. Description.
  --fact <id>             Required. Fact id for FactId.

Notes:
  Adds one FactMeasure row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-factless-fact --help`

```text
Command: add-factless-fact
Usage:
  meta-data-warehouse add-factless-fact [--workspace <path>] --id <id> [--description <value>]
  --fact <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. FactlessFact row id.
  --description <value>  Optional. Description.
  --fact <id>            Required. Fact id for FactId.

Notes:
  Adds one FactlessFact row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-junk-dimension --help`

```text
Command: add-junk-dimension
Usage:
  meta-data-warehouse add-junk-dimension [--workspace <path>] --id <id> [--description <value>]
  --dimension <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. JunkDimension row id.
  --description <value>  Optional. Description.
  --dimension <id>       Required. Dimension id for DimensionId.

Notes:
  Adds one JunkDimension row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-junk-dimension-component --help`

```text
Command: add-junk-dimension-component
Usage:
  meta-data-warehouse add-junk-dimension-component [--workspace <path>] --id <id> [--ordinal
  <value>] [--description <value>] --junk-dimension <id> --attribute <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. JunkDimensionComponent row id.
  --ordinal <value>      Optional. Ordinal.
  --description <value>  Optional. Description.
  --junk-dimension <id>  Required. JunkDimension id for JunkDimensionId.
  --attribute <id>       Required. DimensionAttribute id for DimensionAttributeId.

Notes:
  Adds one JunkDimensionComponent row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-mini-dimension --help`

```text
Command: add-mini-dimension
Usage:
  meta-data-warehouse add-mini-dimension [--workspace <path>] --id <id> [--role-name <value>]
  [--description <value>] --source-dimension <id> --profile-dimension <id>

Options:

  --workspace <path>        Optional. Workspace path. Default: current working directory.
  --id <id>                 Required. MiniDimension row id.
  --role-name <value>       Optional. RoleName.
  --description <value>     Optional. Description.
  --source-dimension <id>   Required. Dimension id for SourceDimensionId.
  --profile-dimension <id>  Required. Dimension id for ProfileDimensionId.

Notes:
  Adds one MiniDimension row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-outrigger-dimension --help`

```text
Command: add-outrigger-dimension
Usage:
  meta-data-warehouse add-outrigger-dimension [--workspace <path>] --id <id> --role-name <value>
  [--ordinal <value>] [--is-required <value>] [--description <value>] --parent-dimension <id>
  --child-dimension <id>

Options:

  --workspace <path>       Optional. Workspace path. Default: current working directory.
  --id <id>                Required. OutriggerDimension row id.
  --role-name <value>      Required. RoleName.
  --ordinal <value>        Optional. Ordinal.
  --is-required <value>    Optional. IsRequired.
  --description <value>    Optional. Description.
  --parent-dimension <id>  Required. Dimension id for ParentDimensionId.
  --child-dimension <id>   Required. Dimension id for ChildDimensionId.

Notes:
  Adds one OutriggerDimension row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-periodic-snapshot-fact --help`

```text
Command: add-periodic-snapshot-fact
Usage:
  meta-data-warehouse add-periodic-snapshot-fact [--workspace <path>] --id <id> --period-name
  <value> [--description <value>] --fact <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. PeriodicSnapshotFact row id.
  --period-name <value>  Required. PeriodName.
  --description <value>  Optional. Description.
  --fact <id>            Required. Fact id for FactId.

Notes:
  Adds one PeriodicSnapshotFact row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-slowly-changing-dimension --help`

```text
Command: add-slowly-changing-dimension
Usage:
  meta-data-warehouse add-slowly-changing-dimension [--workspace <path>] --id <id> [--name <value>]
  [--description <value>] --dimension <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. SlowlyChangingDimension row id.
  --name <value>         Optional. Name.
  --description <value>  Optional. Description.
  --dimension <id>       Required. Dimension id for DimensionId.

Notes:
  Adds one SlowlyChangingDimension row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-transaction-fact --help`

```text
Command: add-transaction-fact
Usage:
  meta-data-warehouse add-transaction-fact [--workspace <path>] --id <id> [--description <value>]
  --fact <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. TransactionFact row id.
  --description <value>  Optional. Description.
  --fact <id>            Required. Fact id for FactId.

Notes:
  Adds one TransactionFact row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-type1-dimension-attribute --help`

```text
Command: add-type1-dimension-attribute
Usage:
  meta-data-warehouse add-type1-dimension-attribute [--workspace <path>] --id <id> [--description
  <value>] --slowly-changing-dimension <id> --attribute <id>

Options:

  --workspace <path>                Optional. Workspace path. Default: current working directory.
  --id <id>                         Required. Type1DimensionAttribute row id.
  --description <value>             Optional. Description.
  --slowly-changing-dimension <id>  Required. SlowlyChangingDimension id for
                                    SlowlyChangingDimensionId.
  --attribute <id>                  Required. DimensionAttribute id for DimensionAttributeId.

Notes:
  Adds one Type1DimensionAttribute row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-type2-dimension-attribute --help`

```text
Command: add-type2-dimension-attribute
Usage:
  meta-data-warehouse add-type2-dimension-attribute [--workspace <path>] --id <id> [--description
  <value>] --slowly-changing-dimension <id> --attribute <id>

Options:

  --workspace <path>                Optional. Workspace path. Default: current working directory.
  --id <id>                         Required. Type2DimensionAttribute row id.
  --description <value>             Optional. Description.
  --slowly-changing-dimension <id>  Required. SlowlyChangingDimension id for
                                    SlowlyChangingDimensionId.
  --attribute <id>                  Required. DimensionAttribute id for DimensionAttributeId.

Notes:
  Adds one Type2DimensionAttribute row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-data-warehouse add-warehouse --help`

```text
Command: add-warehouse
Usage:
  meta-data-warehouse add-warehouse [--workspace <path>] --id <id> --name <value> [--description
  <value>]

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. Warehouse row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.

Notes:
  Adds one Warehouse row to a MetaDataWarehouse workspace.
  Defaults to the current working directory when --workspace is omitted.
```

## meta-analytics

### `meta-analytics --help`

```text
Usage:
  meta-analytics [--new-workspace <path> | <command> [options]]

Commands:

  help                         Show this help.
  --new-workspace              Create an empty MetaAnalytics workspace.
  add-aggregation-behavior     Declare a base measure aggregate function.
  add-attribute                Add a typed table attribute or calculated attribute.
  add-attribute-permission     Add object-level security for an attribute.
  add-attribute-relationship   Declare an attribute relationship inside a table.
  add-attribute-translation    Translate attribute metadata.
  add-culture                  Add a model culture.
  add-data-source              Add an analytics source declaration.
  add-hierarchy                Add a hierarchy.
  add-hierarchy-level          Add an ordered hierarchy level.
  add-hierarchy-translation    Translate hierarchy metadata.
  add-measure                  Add a source-backed base measure.
  add-measure-translation      Translate measure metadata.
  add-model                    Add an analytics model.
  add-perspective              Add a perspective.
  add-perspective-attribute    Expose an attribute in a perspective.
  add-perspective-hierarchy    Expose a hierarchy in a perspective.
  add-perspective-measure      Expose a measure in a perspective.
  add-perspective-table        Expose a table in a perspective.
  add-perspective-translation  Translate perspective metadata.
  add-relationship             Add a relationship between analytics tables.
  add-role-filter              Add row-level security over a table.
  add-role-member              Add a member to a security role.
  add-security-role            Add a security role.
  add-sort-by-attribute        Declare one attribute as the sort key for another.
  add-table                    Add an analytics table.
  add-table-permission         Add object-level security for a table.
  add-table-translation        Translate table metadata.

Notes:
  MetaAnalytics owns common analytics concepts; target-specific scripts and deployment belong in MetaTabular or MetaMultiDimensional.

Next: meta-analytics add-model --help
```

### `meta-analytics --new-workspace --help`

```text
Command: --new-workspace
Usage:
  meta-analytics --new-workspace <path>

Options:

  --new-workspace <path>  Required. Directory where the empty MetaAnalytics workspace will be
                          created.
```

### `meta-analytics add-aggregation-behavior --help`

```text
Command: add-aggregation-behavior
Usage:
  meta-analytics add-aggregation-behavior [--workspace <path>] --id <id> --function <value>
  [--description <value>] --measure <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. AggregationBehavior row id.
  --function <value>     Required. Function.
  --description <value>  Optional. Description.
  --measure <id>         Required. Measure id for MeasureId.

Notes:
  Adds one AggregationBehavior row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-attribute --help`

```text
Command: add-attribute
Usage:
  meta-analytics add-attribute [--workspace <path>] --id <id> --name <value> --data-type-id <value>
  [--ordinal <value>] [--kind <value>] [--source-name <value>] [--expression-language <value>]
  [--expression <value>] [--is-key <value>] [--is-nullable <value>] [--is-hidden <value>]
  [--format-string <value>] [--summarize-by <value>] [--data-category <value>] [--description
  <value>] --table <id>

Options:

  --workspace <path>             Optional. Workspace path. Default: current working directory.
  --id <id>                      Required. Attribute row id.
  --name <value>                 Required. Name.
  --data-type-id <value>         Required. DataTypeId.
  --ordinal <value>              Optional. Ordinal.
  --kind <value>                 Optional. Kind.
  --source-name <value>          Optional. SourceName.
  --expression-language <value>  Optional. ExpressionLanguage.
  --expression <value>           Optional. Expression.
  --is-key <value>               Optional. IsKey.
  --is-nullable <value>          Optional. IsNullable.
  --is-hidden <value>            Optional. IsHidden.
  --format-string <value>        Optional. FormatString.
  --summarize-by <value>         Optional. SummarizeBy.
  --data-category <value>        Optional. DataCategory.
  --description <value>          Optional. Description.
  --table <id>                   Required. Table id for TableId.

Notes:
  Adds one Attribute row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-attribute-permission --help`

```text
Command: add-attribute-permission
Usage:
  meta-analytics add-attribute-permission [--workspace <path>] --id <id> --metadata-permission
  <value> [--description <value>] --role <id> --attribute <id>

Options:

  --workspace <path>             Optional. Workspace path. Default: current working directory.
  --id <id>                      Required. AttributePermission row id.
  --metadata-permission <value>  Required. MetadataPermission.
  --description <value>          Optional. Description.
  --role <id>                    Required. SecurityRole id for SecurityRoleId.
  --attribute <id>               Required. Attribute id for AttributeId.

Notes:
  Adds one AttributePermission row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-attribute-relationship --help`

```text
Command: add-attribute-relationship
Usage:
  meta-analytics add-attribute-relationship [--workspace <path>] --id <id> [--relationship-type
  <value>] [--description <value>] --child-attribute <id> --parent-attribute <id>

Options:

  --workspace <path>           Optional. Workspace path. Default: current working directory.
  --id <id>                    Required. AttributeRelationship row id.
  --relationship-type <value>  Optional. RelationshipType.
  --description <value>        Optional. Description.
  --child-attribute <id>       Required. Attribute id for ChildAttributeId.
  --parent-attribute <id>      Required. Attribute id for ParentAttributeId.

Notes:
  Adds one AttributeRelationship row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-attribute-translation --help`

```text
Command: add-attribute-translation
Usage:
  meta-analytics add-attribute-translation [--workspace <path>] --id <id> [--caption <value>]
  [--description <value>] --culture <id> --attribute <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. AttributeTranslation row id.
  --caption <value>      Optional. Caption.
  --description <value>  Optional. Description.
  --culture <id>         Required. Culture id for CultureId.
  --attribute <id>       Required. Attribute id for AttributeId.

Notes:
  Adds one AttributeTranslation row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-culture --help`

```text
Command: add-culture
Usage:
  meta-analytics add-culture [--workspace <path>] --id <id> --name <value> [--description <value>]
  --model <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. Culture row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --model <id>           Required. AnalyticsModel id for AnalyticsModelId.

Notes:
  Adds one Culture row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-data-source --help`

```text
Command: add-data-source
Usage:
  meta-analytics add-data-source [--workspace <path>] --id <id> --name <value> [--provider <value>]
  [--connection-reference <value>] [--source-kind <value>] [--description <value>] --model <id>

Options:

  --workspace <path>              Optional. Workspace path. Default: current working directory.
  --id <id>                       Required. DataSource row id.
  --name <value>                  Required. Name.
  --provider <value>              Optional. Provider.
  --connection-reference <value>  Optional. ConnectionReference.
  --source-kind <value>           Optional. SourceKind.
  --description <value>           Optional. Description.
  --model <id>                    Required. AnalyticsModel id for AnalyticsModelId.

Notes:
  Adds one DataSource row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-hierarchy --help`

```text
Command: add-hierarchy
Usage:
  meta-analytics add-hierarchy [--workspace <path>] --id <id> --name <value> [--kind <value>]
  [--is-hidden <value>] [--display-folder <value>] [--description <value>] --table <id>

Options:

  --workspace <path>        Optional. Workspace path. Default: current working directory.
  --id <id>                 Required. Hierarchy row id.
  --name <value>            Required. Name.
  --kind <value>            Optional. Kind.
  --is-hidden <value>       Optional. IsHidden.
  --display-folder <value>  Optional. DisplayFolder.
  --description <value>     Optional. Description.
  --table <id>              Required. Table id for TableId.

Notes:
  Adds one Hierarchy row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-hierarchy-level --help`

```text
Command: add-hierarchy-level
Usage:
  meta-analytics add-hierarchy-level [--workspace <path>] --id <id> --name <value> [--ordinal
  <value>] --hierarchy <id> --attribute <id>

Options:

  --workspace <path>  Optional. Workspace path. Default: current working directory.
  --id <id>           Required. HierarchyLevel row id.
  --name <value>      Required. Name.
  --ordinal <value>   Optional. Ordinal.
  --hierarchy <id>    Required. Hierarchy id for HierarchyId.
  --attribute <id>    Required. Attribute id for AttributeId.

Notes:
  Adds one HierarchyLevel row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-hierarchy-translation --help`

```text
Command: add-hierarchy-translation
Usage:
  meta-analytics add-hierarchy-translation [--workspace <path>] --id <id> [--caption <value>]
  [--description <value>] --culture <id> --hierarchy <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. HierarchyTranslation row id.
  --caption <value>      Optional. Caption.
  --description <value>  Optional. Description.
  --culture <id>         Required. Culture id for CultureId.
  --hierarchy <id>       Required. Hierarchy id for HierarchyId.

Notes:
  Adds one HierarchyTranslation row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-measure --help`

```text
Command: add-measure
Usage:
  meta-analytics add-measure [--workspace <path>] --id <id> --name <value> [--data-type-id <value>]
  [--format-string <value>] [--display-folder <value>] [--is-hidden <value>] [--description <value>]
  --table <id> --source-attribute <id>

Options:

  --workspace <path>        Optional. Workspace path. Default: current working directory.
  --id <id>                 Required. Measure row id.
  --name <value>            Required. Name.
  --data-type-id <value>    Optional. DataTypeId.
  --format-string <value>   Optional. FormatString.
  --display-folder <value>  Optional. DisplayFolder.
  --is-hidden <value>       Optional. IsHidden.
  --description <value>     Optional. Description.
  --table <id>              Required. Table id for TableId.
  --source-attribute <id>   Required. Attribute id for SourceAttributeId.

Notes:
  Adds one Measure row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-measure-translation --help`

```text
Command: add-measure-translation
Usage:
  meta-analytics add-measure-translation [--workspace <path>] --id <id> [--caption <value>]
  [--description <value>] --culture <id> --measure <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. MeasureTranslation row id.
  --caption <value>      Optional. Caption.
  --description <value>  Optional. Description.
  --culture <id>         Required. Culture id for CultureId.
  --measure <id>         Required. Measure id for MeasureId.

Notes:
  Adds one MeasureTranslation row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-model --help`

```text
Command: add-model
Usage:
  meta-analytics add-model [--workspace <path>] --id <id> --name <value> [--default-culture <value>]
  [--description <value>]

Options:

  --workspace <path>         Optional. Workspace path. Default: current working directory.
  --id <id>                  Required. AnalyticsModel row id.
  --name <value>             Required. Name.
  --default-culture <value>  Optional. DefaultCulture.
  --description <value>      Optional. Description.

Notes:
  Adds one AnalyticsModel row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-perspective --help`

```text
Command: add-perspective
Usage:
  meta-analytics add-perspective [--workspace <path>] --id <id> --name <value> [--description
  <value>] --model <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. Perspective row id.
  --name <value>         Required. Name.
  --description <value>  Optional. Description.
  --model <id>           Required. AnalyticsModel id for AnalyticsModelId.

Notes:
  Adds one Perspective row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-perspective-attribute --help`

```text
Command: add-perspective-attribute
Usage:
  meta-analytics add-perspective-attribute [--workspace <path>] --id <id> --perspective <id>
  --attribute <id>

Options:

  --workspace <path>  Optional. Workspace path. Default: current working directory.
  --id <id>           Required. PerspectiveAttribute row id.
  --perspective <id>  Required. Perspective id for PerspectiveId.
  --attribute <id>    Required. Attribute id for AttributeId.

Notes:
  Adds one PerspectiveAttribute row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-perspective-hierarchy --help`

```text
Command: add-perspective-hierarchy
Usage:
  meta-analytics add-perspective-hierarchy [--workspace <path>] --id <id> --perspective <id>
  --hierarchy <id>

Options:

  --workspace <path>  Optional. Workspace path. Default: current working directory.
  --id <id>           Required. PerspectiveHierarchy row id.
  --perspective <id>  Required. Perspective id for PerspectiveId.
  --hierarchy <id>    Required. Hierarchy id for HierarchyId.

Notes:
  Adds one PerspectiveHierarchy row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-perspective-measure --help`

```text
Command: add-perspective-measure
Usage:
  meta-analytics add-perspective-measure [--workspace <path>] --id <id> --perspective <id> --measure
  <id>

Options:

  --workspace <path>  Optional. Workspace path. Default: current working directory.
  --id <id>           Required. PerspectiveMeasure row id.
  --perspective <id>  Required. Perspective id for PerspectiveId.
  --measure <id>      Required. Measure id for MeasureId.

Notes:
  Adds one PerspectiveMeasure row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-perspective-table --help`

```text
Command: add-perspective-table
Usage:
  meta-analytics add-perspective-table [--workspace <path>] --id <id> --perspective <id> --table
  <id>

Options:

  --workspace <path>  Optional. Workspace path. Default: current working directory.
  --id <id>           Required. PerspectiveTable row id.
  --perspective <id>  Required. Perspective id for PerspectiveId.
  --table <id>        Required. Table id for TableId.

Notes:
  Adds one PerspectiveTable row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-perspective-translation --help`

```text
Command: add-perspective-translation
Usage:
  meta-analytics add-perspective-translation [--workspace <path>] --id <id> [--caption <value>]
  [--description <value>] --culture <id> --perspective <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. PerspectiveTranslation row id.
  --caption <value>      Optional. Caption.
  --description <value>  Optional. Description.
  --culture <id>         Required. Culture id for CultureId.
  --perspective <id>     Required. Perspective id for PerspectiveId.

Notes:
  Adds one PerspectiveTranslation row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-relationship --help`

```text
Command: add-relationship
Usage:
  meta-analytics add-relationship [--workspace <path>] --id <id> --name <value> [--role-name
  <value>] --relationship-kind <value> --cardinality <value> [--cross-filter-direction <value>]
  [--is-active <value>] [--is-required <value>] [--description <value>] --from-table <id>
  --from-attribute <id> --to-table <id> --to-attribute <id> [--granularity-attribute <id>]
  [--intermediate-table <id>]

Options:

  --workspace <path>                Optional. Workspace path. Default: current working directory.
  --id <id>                         Required. Relationship row id.
  --name <value>                    Required. Name.
  --role-name <value>               Optional. RoleName.
  --relationship-kind <value>       Required. RelationshipKind.
  --cardinality <value>             Required. Cardinality.
  --cross-filter-direction <value>  Optional. CrossFilterDirection.
  --is-active <value>               Optional. IsActive.
  --is-required <value>             Optional. IsRequired.
  --description <value>             Optional. Description.
  --from-table <id>                 Required. Table id for FromTableId.
  --from-attribute <id>             Required. Attribute id for FromAttributeId.
  --to-table <id>                   Required. Table id for ToTableId.
  --to-attribute <id>               Required. Attribute id for ToAttributeId.
  --granularity-attribute <id>      Optional. Attribute id for GranularityAttributeId.
  --intermediate-table <id>         Optional. Table id for IntermediateTableId.

Notes:
  Adds one Relationship row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-role-filter --help`

```text
Command: add-role-filter
Usage:
  meta-analytics add-role-filter [--workspace <path>] --id <id> --expression-language <value>
  --expression <value> [--description <value>] --role <id> --table <id>

Options:

  --workspace <path>             Optional. Workspace path. Default: current working directory.
  --id <id>                      Required. RoleFilter row id.
  --expression-language <value>  Required. ExpressionLanguage.
  --expression <value>           Required. Expression.
  --description <value>          Optional. Description.
  --role <id>                    Required. SecurityRole id for SecurityRoleId.
  --table <id>                   Required. Table id for TableId.

Notes:
  Adds one RoleFilter row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-role-member --help`

```text
Command: add-role-member
Usage:
  meta-analytics add-role-member [--workspace <path>] --id <id> --member-name <value> [--member-kind
  <value>] --role <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. RoleMember row id.
  --member-name <value>  Required. MemberName.
  --member-kind <value>  Optional. MemberKind.
  --role <id>            Required. SecurityRole id for SecurityRoleId.

Notes:
  Adds one RoleMember row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-security-role --help`

```text
Command: add-security-role
Usage:
  meta-analytics add-security-role [--workspace <path>] --id <id> --name <value> --permission
  <value> [--description <value>] --model <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. SecurityRole row id.
  --name <value>         Required. Name.
  --permission <value>   Required. Permission.
  --description <value>  Optional. Description.
  --model <id>           Required. AnalyticsModel id for AnalyticsModelId.

Notes:
  Adds one SecurityRole row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-sort-by-attribute --help`

```text
Command: add-sort-by-attribute
Usage:
  meta-analytics add-sort-by-attribute [--workspace <path>] --id <id> [--description <value>]
  --source-attribute <id> --sort-attribute <id>

Options:

  --workspace <path>       Optional. Workspace path. Default: current working directory.
  --id <id>                Required. SortByAttribute row id.
  --description <value>    Optional. Description.
  --source-attribute <id>  Required. Attribute id for SourceAttributeId.
  --sort-attribute <id>    Required. Attribute id for SortAttributeId.

Notes:
  Adds one SortByAttribute row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-table --help`

```text
Command: add-table
Usage:
  meta-analytics add-table [--workspace <path>] --id <id> --name <value> --kind <value>
  [--data-category <value>] [--is-hidden <value>] [--display-folder <value>] [--description <value>]
  --model <id>

Options:

  --workspace <path>        Optional. Workspace path. Default: current working directory.
  --id <id>                 Required. Table row id.
  --name <value>            Required. Name.
  --kind <value>            Required. Kind.
  --data-category <value>   Optional. DataCategory.
  --is-hidden <value>       Optional. IsHidden.
  --display-folder <value>  Optional. DisplayFolder.
  --description <value>     Optional. Description.
  --model <id>              Required. AnalyticsModel id for AnalyticsModelId.

Notes:
  Adds one Table row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-table-permission --help`

```text
Command: add-table-permission
Usage:
  meta-analytics add-table-permission [--workspace <path>] --id <id> --metadata-permission <value>
  [--description <value>] --role <id> --table <id>

Options:

  --workspace <path>             Optional. Workspace path. Default: current working directory.
  --id <id>                      Required. TablePermission row id.
  --metadata-permission <value>  Required. MetadataPermission.
  --description <value>          Optional. Description.
  --role <id>                    Required. SecurityRole id for SecurityRoleId.
  --table <id>                   Required. Table id for TableId.

Notes:
  Adds one TablePermission row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-analytics add-table-translation --help`

```text
Command: add-table-translation
Usage:
  meta-analytics add-table-translation [--workspace <path>] --id <id> [--caption <value>]
  [--description <value>] --culture <id> --table <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. TableTranslation row id.
  --caption <value>      Optional. Caption.
  --description <value>  Optional. Description.
  --culture <id>         Required. Culture id for CultureId.
  --table <id>           Required. Table id for TableId.

Notes:
  Adds one TableTranslation row to a MetaAnalytics workspace.
  Defaults to the current working directory when --workspace is omitted.
```

## meta-convert

### `meta-convert --help`

```text
Usage:
  meta-convert <command> [options]

Commands:

  help                            Show this help.
  schema-to-raw-datavault         Convert MetaSchema workspace to MetaRawDataVault workspace.
  raw-datavault-to-sql            Convert MetaRawDataVault workspace to MetaSql workspace.
  business-datavault-to-sql       Convert MetaBusinessDataVault workspace to MetaSql workspace.
  data-quality-to-sql             Convert promoted MetaDataQuality candidates to SQL DQ views.
  data-warehouse-to-sql           Convert MetaDataWarehouse workspace to MetaSql workspace.
  transform-script-to-sql         Convert MetaTransformScript SQL modules to MetaSql workspace.
  sql-to-transform-script         Convert MetaSql SQL modules to MetaTransformScript workspace.
  analytics-to-tabular            Convert MetaAnalytics workspace to MetaTabular workspace.
  analytics-to-multi-dimensional  Convert MetaAnalytics workspace to MetaMultiDimensional workspace.

Next: meta-convert schema-to-raw-datavault --help
```

### `meta-convert schema-to-raw-datavault --help`

```text
Command: schema-to-raw-datavault
Usage:
  meta-convert schema-to-raw-datavault --source-workspace <path> --new-workspace <path>
  [--ignore-field-name <name>]... [--ignore-field-suffix <suffix>]... [--include-views] [--verbose]

Options:

  --source-workspace <path>       Required. MetaSchema workspace to convert.
  --new-workspace <path>          Required. Directory where the MetaRawDataVault workspace will be
                                  created.
  --ignore-field-name <name>      Optional source field name to ignore. May be repeated.
  --ignore-field-suffix <suffix>  Optional source field suffix to ignore. May be repeated.
  --include-views                 Optional. Include source views.
  --verbose                       Optional. Print conversion summary.

Notes:
  Loads MetaSchema from --source-workspace and saves MetaRawDataVault at --new-workspace.
  Uses typed MetaSchema and MetaRawDataVault instance/tooling libraries.
  Does not use generic workspace model loading.
```

### `meta-convert raw-datavault-to-sql --help`

```text
Command: raw-datavault-to-sql
Usage:
  meta-convert raw-datavault-to-sql [--workspace <path>] --implementation-workspace <path>
  --database-name <name> --out <path>

Options:

  --workspace <path>                 Optional. Source workspace path. Default: current working
                                     directory.
  --implementation-workspace <path>  Required. Implementation policy workspace.
  --database-name <name>             Required. Target MetaSql database name.
  --out <path>                       Required. Output MetaSql workspace path.

Notes:
  Converts the current sanctioned MetaRawDataVault workspace to a current MetaSql workspace.
  Target schema comes from the sanctioned MetaDataVaultImplementation workspace.
  Does not query any live database.
  Saves the generated current MetaSql workspace at --out.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-convert business-datavault-to-sql --help`

```text
Command: business-datavault-to-sql
Usage:
  meta-convert business-datavault-to-sql [--workspace <path>] --implementation-workspace <path>
  --database-name <name> --out <path>

Options:

  --workspace <path>                 Optional. Source workspace path. Default: current working
                                     directory.
  --implementation-workspace <path>  Required. Implementation policy workspace.
  --database-name <name>             Required. Target MetaSql database name.
  --out <path>                       Required. Output MetaSql workspace path.

Notes:
  Converts the current sanctioned MetaBusinessDataVault workspace to a current MetaSql workspace.
  Applies sanctioned business-type lowering during conversion.
  Target schema comes from the sanctioned MetaDataVaultImplementation workspace.
  Does not query any live database.
  Saves the generated current MetaSql workspace at --out.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-convert data-quality-to-sql --help`

```text
Command: data-quality-to-sql
Usage:
  meta-convert data-quality-to-sql [--workspace <path>] --out <path>

Options:

  --workspace <path>  Optional. MetaDataQuality workspace to convert. Default: current working
                      directory.
  --out <path>        Required. Output workspace or script folder path.

Notes:
  Reads promoted candidates from a MetaDataQuality workspace.
  Generates SQL view scripts plus a MetaDQ operational pack (run/finding tables and execution procedure).
  The operational procedure reads dq.v_DataQualityReview from a source database and persists each run in MetaDQ.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-convert data-warehouse-to-sql --help`

```text
Command: data-warehouse-to-sql
Usage:
  meta-convert data-warehouse-to-sql [--workspace <path>] --implementation-workspace <path>
  --database-name <name> --out <path>

Options:

  --workspace <path>                 Optional. Source workspace path. Default: current working
                                     directory.
  --implementation-workspace <path>  Required. Implementation policy workspace.
  --database-name <name>             Required. Target MetaSql database name.
  --out <path>                       Required. Output MetaSql workspace path.

Notes:
  Converts the current sanctioned MetaDataWarehouse workspace to a current MetaSql workspace.
  Target table/column/key policy comes from the sanctioned MetaDataWarehouseImplementation workspace.
  Does not query any live database.
  Saves the generated current MetaSql workspace at --out.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-convert transform-script-to-sql --help`

```text
Command: transform-script-to-sql
Usage:
  meta-convert transform-script-to-sql [--workspace <path>] --database-name <name> --out <path>

Options:

  --workspace <path>      Optional. Source MetaTransformScript workspace path. Default: current
                          working directory.
  --database-name <name>  Required. Target MetaSql database name.
  --out <path>            Required. Output MetaSql workspace path.

Notes:
  Converts MetaTransformScript view, function, and stored procedure modules to a current MetaSql workspace.
  SQL module declarations must already be schema-qualified in the source workspace.
  Saves the generated current MetaSql workspace at --out.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-convert sql-to-transform-script --help`

```text
Command: sql-to-transform-script
Usage:
  meta-convert sql-to-transform-script [--workspace <path>] --out <path> [--include-views]
  [--include-functions] [--include-stored-procedures] [--allow-empty]

Options:

  --workspace <path>           Optional. Source MetaSql workspace path. Default: current working
                               directory.
  --out <path>                 Required. Output MetaTransformScript workspace path.
  --include-views              Convert view modules. If no include switch is provided, all module
                               kinds are selected.
  --include-functions          Convert function modules. If no include switch is provided, all
                               module kinds are selected.
  --include-stored-procedures  Convert stored procedure modules. If no include switch is provided,
                               all module kinds are selected.
  --allow-empty                Create an empty MetaTransformScript workspace when selected module
                               kinds have no convertible modules.

Notes:
  Reads view, function, and stored procedure module definitions from a MetaSql workspace.
  Imports each module through the MetaTransformScript SQL importer.
  If any include switch is provided, only selected module kinds are converted.
  Saves the generated current MetaTransformScript workspace at --out.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-convert analytics-to-tabular --help`

```text
Command: analytics-to-tabular
Usage:
  meta-convert analytics-to-tabular [--workspace <path>] --out <path>

Options:

  --workspace <path>  Optional. MetaAnalytics workspace to convert. Default: current working
                      directory.
  --out <path>        Required. Output workspace or script folder path.

Notes:
  Converts common MetaAnalytics intent to a MetaTabular workspace.
  DAX expressions are copied when present; non-DAX expressions fail clearly.
  Target-specific calculation groups, partitions, and deployment details are patched in MetaTabular after conversion.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-convert analytics-to-multi-dimensional --help`

```text
Command: analytics-to-multi-dimensional
Usage:
  meta-convert analytics-to-multi-dimensional [--workspace <path>] --out <path>

Options:

  --workspace <path>  Optional. MetaAnalytics workspace to convert. Default: current working
                      directory.
  --out <path>        Required. Output workspace or script folder path.

Notes:
  Converts common MetaAnalytics intent to a MetaMultiDimensional workspace.
  Tabular-style row/object security and measure-expression rows fail clearly until multidimensional calculated-measure projection is modeled.
  Target-specific measure groups, cell security, named sets, actions, partitions, and deployment details are patched in MetaMultiDimensional after conversion.
  Defaults to the current working directory when --workspace is omitted.
```

## meta-tabular

### `meta-tabular --help`

```text
Usage:
  meta-tabular [--new-workspace <path> | <command> [options]]

Commands:

  help                                       Show this help.
  --new-workspace                            Create an empty MetaTabular workspace.
  deploy                                     Create modeled objects on an Analysis Services tabular
                                             instance.
  restore                                    Promote a processed tabular database through backup and
                                             restore.
  drop                                       Drop a tabular database from an Analysis Services
                                             tabular instance.
  add-tabular-calculation-group              Add a TabularCalculationGroup row.
  add-tabular-calculation-item               Add a TabularCalculationItem row.
  add-tabular-column                         Add a TabularColumn row.
  add-tabular-column-permission              Add a TabularColumnPermission row.
  add-tabular-column-translation             Add a TabularColumnTranslation row.
  add-tabular-culture                        Add a TabularCulture row.
  add-tabular-data-source                    Add a TabularDataSource row.
  add-tabular-hierarchy                      Add a TabularHierarchy row.
  add-tabular-hierarchy-level                Add a TabularHierarchyLevel row.
  add-tabular-hierarchy-translation          Add a TabularHierarchyTranslation row.
  add-tabular-kpi                            Add a TabularKpi row.
  add-tabular-kpi-translation                Add a TabularKpiTranslation row.
  add-tabular-measure                        Add a TabularMeasure row.
  add-tabular-measure-translation            Add a TabularMeasureTranslation row.
  add-tabular-model                          Add a TabularModel row.
  add-tabular-partition                      Add a TabularPartition row.
  add-tabular-perspective                    Add a TabularPerspective row.
  add-tabular-perspective-calculation-group  Add a TabularPerspectiveCalculationGroup row.
  add-tabular-perspective-column             Add a TabularPerspectiveColumn row.
  add-tabular-perspective-hierarchy          Add a TabularPerspectiveHierarchy row.
  add-tabular-perspective-kpi                Add a TabularPerspectiveKpi row.
  add-tabular-perspective-measure            Add a TabularPerspectiveMeasure row.
  add-tabular-perspective-table              Add a TabularPerspectiveTable row.
  add-tabular-perspective-translation        Add a TabularPerspectiveTranslation row.
  add-tabular-relationship                   Add a TabularRelationship row.
  add-tabular-role-filter                    Add a TabularRoleFilter row.
  add-tabular-role-member                    Add a TabularRoleMember row.
  add-tabular-security-role                  Add a TabularSecurityRole row.
  add-tabular-sort-by-column                 Add a TabularSortByColumn row.
  add-tabular-table                          Add a TabularTable row.
  add-tabular-table-permission               Add a TabularTablePermission row.
  add-tabular-table-translation              Add a TabularTableTranslation row.

Next: meta-tabular add-tabular-model --help
```

### `meta-tabular --new-workspace --help`

```text
Command: --new-workspace
Usage:
  meta-tabular --new-workspace <path>

Options:

  --new-workspace <path>  Required. Directory where the empty MetaTabular workspace will be created.
```

### `meta-tabular deploy --help`

```text
Command: deploy
Usage:
  meta-tabular deploy [--workspace <path>] --server <server> [--database-name <name>]
  [--drop-existing] [--no-process]

Options:

  --workspace <path>      MetaTabular workspace to deploy. Defaults to the current directory.
  --server <server>       Required. Analysis Services tabular server.
  --database-name <name>  Optional target database name. Defaults to the modeled database name.
  --drop-existing         Drop an existing target database before create/deploy.
  --no-process            Deploy metadata only and skip full processing.

Notes:
  Creates tabular database objects on an Analysis Services tabular instance.
  By default, the command runs full processing after deploy and fails if processing fails.
  Without --drop-existing, the command fails if the database already exists.
  With --drop-existing, the command uses the safe drop, create, full-process sequence.
  With --no-process, the command deploys metadata only.
  This deploys modeled data sources, tables, columns, partitions, measures, relationships, calculation groups, and role filters.
```

### `meta-tabular restore --help`

```text
Command: restore
Usage:
  meta-tabular restore --source-server <server> --source-database-name <name> --target-server
  <server> --target-database-name <name> --backup-file <path> [--drop-existing]
  [--overwrite-backup-file]

Options:

  --source-server <server>       Required. Source Analysis Services server containing the processed
                                 database.
  --source-database-name <name>  Required. Source processed database name.
  --target-server <server>       Required. Target Analysis Services server.
  --target-database-name <name>  Required. Target database name to restore.
  --backup-file <path>           Required. Backup file path accessible to the Analysis Services
                                 service accounts.
  --drop-existing                Drop an existing target database before restore.
  --overwrite-backup-file        Overwrite an existing backup file.

Notes:
  Backs up a processed source tabular database and restores it as the target database.
  Use this for pre-prod-to-prod promotion after pre-prod deploy and processing succeeds.
  If the target database exists, --drop-existing is required before restore.
  Restore does not process. Partial or object-level processing belongs in a separate command.
  The backup file path must be accessible to the Analysis Services service accounts on both source and target servers.
```

### `meta-tabular drop --help`

```text
Command: drop
Usage:
  meta-tabular drop --server <server> --database-name <name>

Options:

  --server <server>       Required. Analysis Services tabular server.
  --database-name <name>  Required. Database name to drop.

Notes:
  Drops a tabular database from an Analysis Services tabular instance.
  This command has no confirmation prompt; use it only with an explicit database name.
  The command fails if the database does not exist.
```

### `meta-tabular add-tabular-calculation-group --help`

```text
Command: add-tabular-calculation-group
Usage:
  meta-tabular add-tabular-calculation-group [--workspace <path>] --id <id> [--description <value>]
  --name <value> --precedence <value> --tabular-model <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. TabularCalculationGroup row id.
  --description <value>  Optional. Description.
  --name <value>         Required. Name.
  --precedence <value>   Required. Precedence.
  --tabular-model <id>   Required. TabularModel id for TabularModelId.

Notes:
  Adds one TabularCalculationGroup row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-calculation-item --help`

```text
Command: add-tabular-calculation-item
Usage:
  meta-tabular add-tabular-calculation-item [--workspace <path>] --id <id> [--description <value>]
  --expression <value> [--format-string-expression <value>] --name <value> [--ordinal <value>]
  --tabular-calculation-group <id>

Options:

  --workspace <path>                  Optional. Workspace path. Default: current working directory.
  --id <id>                           Required. TabularCalculationItem row id.
  --description <value>               Optional. Description.
  --expression <value>                Required. Expression.
  --format-string-expression <value>  Optional. FormatStringExpression.
  --name <value>                      Required. Name.
  --ordinal <value>                   Optional. Ordinal.
  --tabular-calculation-group <id>    Required. TabularCalculationGroup id for
                                      TabularCalculationGroupId.

Notes:
  Adds one TabularCalculationItem row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-column --help`

```text
Command: add-tabular-column
Usage:
  meta-tabular add-tabular-column [--workspace <path>] --id <id> [--data-category <value>]
  --data-type-id <value> [--description <value>] [--expression <value>] [--format-string <value>]
  [--is-hidden <value>] [--is-key <value>] [--is-nullable <value>] --name <value> [--ordinal
  <value>] [--source-name <value>] [--summarize-by <value>] --tabular-table <id>

Options:

  --workspace <path>       Optional. Workspace path. Default: current working directory.
  --id <id>                Required. TabularColumn row id.
  --data-category <value>  Optional. DataCategory.
  --data-type-id <value>   Required. DataTypeId.
  --description <value>    Optional. Description.
  --expression <value>     Optional. Expression.
  --format-string <value>  Optional. FormatString.
  --is-hidden <value>      Optional. IsHidden.
  --is-key <value>         Optional. IsKey.
  --is-nullable <value>    Optional. IsNullable.
  --name <value>           Required. Name.
  --ordinal <value>        Optional. Ordinal.
  --source-name <value>    Optional. SourceName.
  --summarize-by <value>   Optional. SummarizeBy.
  --tabular-table <id>     Required. TabularTable id for TabularTableId.

Notes:
  Adds one TabularColumn row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-column-permission --help`

```text
Command: add-tabular-column-permission
Usage:
  meta-tabular add-tabular-column-permission [--workspace <path>] --id <id> --metadata-permission
  <value> --tabular-column <id> --tabular-security-role <id>

Options:

  --workspace <path>             Optional. Workspace path. Default: current working directory.
  --id <id>                      Required. TabularColumnPermission row id.
  --metadata-permission <value>  Required. MetadataPermission.
  --tabular-column <id>          Required. TabularColumn id for TabularColumnId.
  --tabular-security-role <id>   Required. TabularSecurityRole id for TabularSecurityRoleId.

Notes:
  Adds one TabularColumnPermission row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-column-translation --help`

```text
Command: add-tabular-column-translation
Usage:
  meta-tabular add-tabular-column-translation [--workspace <path>] --id <id> [--caption <value>]
  [--description <value>] --tabular-column <id> --tabular-culture <id>

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. TabularColumnTranslation row id.
  --caption <value>       Optional. Caption.
  --description <value>   Optional. Description.
  --tabular-column <id>   Required. TabularColumn id for TabularColumnId.
  --tabular-culture <id>  Required. TabularCulture id for TabularCultureId.

Notes:
  Adds one TabularColumnTranslation row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-culture --help`

```text
Command: add-tabular-culture
Usage:
  meta-tabular add-tabular-culture [--workspace <path>] --id <id> --name <value> --tabular-model
  <id>

Options:

  --workspace <path>    Optional. Workspace path. Default: current working directory.
  --id <id>             Required. TabularCulture row id.
  --name <value>        Required. Name.
  --tabular-model <id>  Required. TabularModel id for TabularModelId.

Notes:
  Adds one TabularCulture row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-data-source --help`

```text
Command: add-tabular-data-source
Usage:
  meta-tabular add-tabular-data-source [--workspace <path>] --id <id> [--connection-reference
  <value>] [--description <value>] --name <value> [--provider <value>] --tabular-model <id>

Options:

  --workspace <path>              Optional. Workspace path. Default: current working directory.
  --id <id>                       Required. TabularDataSource row id.
  --connection-reference <value>  Optional. ConnectionReference.
  --description <value>           Optional. Description.
  --name <value>                  Required. Name.
  --provider <value>              Optional. Provider.
  --tabular-model <id>            Required. TabularModel id for TabularModelId.

Notes:
  Adds one TabularDataSource row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-hierarchy --help`

```text
Command: add-tabular-hierarchy
Usage:
  meta-tabular add-tabular-hierarchy [--workspace <path>] --id <id> [--description <value>]
  [--display-folder <value>] [--is-hidden <value>] --name <value> --tabular-table <id>

Options:

  --workspace <path>        Optional. Workspace path. Default: current working directory.
  --id <id>                 Required. TabularHierarchy row id.
  --description <value>     Optional. Description.
  --display-folder <value>  Optional. DisplayFolder.
  --is-hidden <value>       Optional. IsHidden.
  --name <value>            Required. Name.
  --tabular-table <id>      Required. TabularTable id for TabularTableId.

Notes:
  Adds one TabularHierarchy row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-hierarchy-level --help`

```text
Command: add-tabular-hierarchy-level
Usage:
  meta-tabular add-tabular-hierarchy-level [--workspace <path>] --id <id> --name <value> [--ordinal
  <value>] --tabular-column <id> --tabular-hierarchy <id>

Options:

  --workspace <path>        Optional. Workspace path. Default: current working directory.
  --id <id>                 Required. TabularHierarchyLevel row id.
  --name <value>            Required. Name.
  --ordinal <value>         Optional. Ordinal.
  --tabular-column <id>     Required. TabularColumn id for TabularColumnId.
  --tabular-hierarchy <id>  Required. TabularHierarchy id for TabularHierarchyId.

Notes:
  Adds one TabularHierarchyLevel row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-hierarchy-translation --help`

```text
Command: add-tabular-hierarchy-translation
Usage:
  meta-tabular add-tabular-hierarchy-translation [--workspace <path>] --id <id> [--caption <value>]
  [--description <value>] --tabular-culture <id> --tabular-hierarchy <id>

Options:

  --workspace <path>        Optional. Workspace path. Default: current working directory.
  --id <id>                 Required. TabularHierarchyTranslation row id.
  --caption <value>         Optional. Caption.
  --description <value>     Optional. Description.
  --tabular-culture <id>    Required. TabularCulture id for TabularCultureId.
  --tabular-hierarchy <id>  Required. TabularHierarchy id for TabularHierarchyId.

Notes:
  Adds one TabularHierarchyTranslation row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-kpi --help`

```text
Command: add-tabular-kpi
Usage:
  meta-tabular add-tabular-kpi [--workspace <path>] --id <id> [--description <value>]
  [--status-expression <value>] [--status-graphic <value>] [--target-expression <value>]
  [--trend-expression <value>] [--trend-graphic <value>] --base-measure <id> [--target-measure <id>]

Options:

  --workspace <path>           Optional. Workspace path. Default: current working directory.
  --id <id>                    Required. TabularKpi row id.
  --description <value>        Optional. Description.
  --status-expression <value>  Optional. StatusExpression.
  --status-graphic <value>     Optional. StatusGraphic.
  --target-expression <value>  Optional. TargetExpression.
  --trend-expression <value>   Optional. TrendExpression.
  --trend-graphic <value>      Optional. TrendGraphic.
  --base-measure <id>          Required. TabularMeasure id for BaseMeasureId.
  --target-measure <id>        Optional. TabularMeasure id for TargetMeasureId.

Notes:
  Adds one TabularKpi row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-kpi-translation --help`

```text
Command: add-tabular-kpi-translation
Usage:
  meta-tabular add-tabular-kpi-translation [--workspace <path>] --id <id> [--description <value>]
  --tabular-culture <id> --tabular-kpi <id>

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. TabularKpiTranslation row id.
  --description <value>   Optional. Description.
  --tabular-culture <id>  Required. TabularCulture id for TabularCultureId.
  --tabular-kpi <id>      Required. TabularKpi id for TabularKpiId.

Notes:
  Adds one TabularKpiTranslation row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-measure --help`

```text
Command: add-tabular-measure
Usage:
  meta-tabular add-tabular-measure [--workspace <path>] --id <id> [--description <value>]
  [--display-folder <value>] [--expression <value>] [--format-string <value>] [--is-hidden <value>]
  --name <value> --tabular-table <id>

Options:

  --workspace <path>        Optional. Workspace path. Default: current working directory.
  --id <id>                 Required. TabularMeasure row id.
  --description <value>     Optional. Description.
  --display-folder <value>  Optional. DisplayFolder.
  --expression <value>      Optional. Expression.
  --format-string <value>   Optional. FormatString.
  --is-hidden <value>       Optional. IsHidden.
  --name <value>            Required. Name.
  --tabular-table <id>      Required. TabularTable id for TabularTableId.

Notes:
  Adds one TabularMeasure row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-measure-translation --help`

```text
Command: add-tabular-measure-translation
Usage:
  meta-tabular add-tabular-measure-translation [--workspace <path>] --id <id> [--caption <value>]
  [--description <value>] --tabular-culture <id> --tabular-measure <id>

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. TabularMeasureTranslation row id.
  --caption <value>       Optional. Caption.
  --description <value>   Optional. Description.
  --tabular-culture <id>  Required. TabularCulture id for TabularCultureId.
  --tabular-measure <id>  Required. TabularMeasure id for TabularMeasureId.

Notes:
  Adds one TabularMeasureTranslation row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-model --help`

```text
Command: add-tabular-model
Usage:
  meta-tabular add-tabular-model [--workspace <path>] --id <id> [--collation <value>]
  [--compatibility-level <value>] [--default-culture <value>] [--default-data-view <value>]
  [--description <value>] --name <value>

Options:

  --workspace <path>             Optional. Workspace path. Default: current working directory.
  --id <id>                      Required. TabularModel row id.
  --collation <value>            Optional. Collation.
  --compatibility-level <value>  Optional. CompatibilityLevel.
  --default-culture <value>      Optional. DefaultCulture.
  --default-data-view <value>    Optional. DefaultDataView.
  --description <value>          Optional. Description.
  --name <value>                 Required. Name.

Notes:
  Adds one TabularModel row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-partition --help`

```text
Command: add-tabular-partition
Usage:
  meta-tabular add-tabular-partition [--workspace <path>] --id <id> [--description <value>]
  [--expression <value>] [--mode <value>] --name <value> [--ordinal <value>] [--tabular-data-source
  <id>] --tabular-table <id>

Options:

  --workspace <path>          Optional. Workspace path. Default: current working directory.
  --id <id>                   Required. TabularPartition row id.
  --description <value>       Optional. Description.
  --expression <value>        Optional. Expression.
  --mode <value>              Optional. Mode.
  --name <value>              Required. Name.
  --ordinal <value>           Optional. Ordinal.
  --tabular-data-source <id>  Optional. TabularDataSource id for TabularDataSourceId.
  --tabular-table <id>        Required. TabularTable id for TabularTableId.

Notes:
  Adds one TabularPartition row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-perspective --help`

```text
Command: add-tabular-perspective
Usage:
  meta-tabular add-tabular-perspective [--workspace <path>] --id <id> [--description <value>] --name
  <value> --tabular-model <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. TabularPerspective row id.
  --description <value>  Optional. Description.
  --name <value>         Required. Name.
  --tabular-model <id>   Required. TabularModel id for TabularModelId.

Notes:
  Adds one TabularPerspective row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-perspective-calculation-group --help`

```text
Command: add-tabular-perspective-calculation-group
Usage:
  meta-tabular add-tabular-perspective-calculation-group [--workspace <path>] --id <id>
  --tabular-calculation-group <id> --tabular-perspective <id>

Options:

  --workspace <path>                Optional. Workspace path. Default: current working directory.
  --id <id>                         Required. TabularPerspectiveCalculationGroup row id.
  --tabular-calculation-group <id>  Required. TabularCalculationGroup id for
                                    TabularCalculationGroupId.
  --tabular-perspective <id>        Required. TabularPerspective id for TabularPerspectiveId.

Notes:
  Adds one TabularPerspectiveCalculationGroup row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-perspective-column --help`

```text
Command: add-tabular-perspective-column
Usage:
  meta-tabular add-tabular-perspective-column [--workspace <path>] --id <id> --tabular-column <id>
  --tabular-perspective <id>

Options:

  --workspace <path>          Optional. Workspace path. Default: current working directory.
  --id <id>                   Required. TabularPerspectiveColumn row id.
  --tabular-column <id>       Required. TabularColumn id for TabularColumnId.
  --tabular-perspective <id>  Required. TabularPerspective id for TabularPerspectiveId.

Notes:
  Adds one TabularPerspectiveColumn row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-perspective-hierarchy --help`

```text
Command: add-tabular-perspective-hierarchy
Usage:
  meta-tabular add-tabular-perspective-hierarchy [--workspace <path>] --id <id> --tabular-hierarchy
  <id> --tabular-perspective <id>

Options:

  --workspace <path>          Optional. Workspace path. Default: current working directory.
  --id <id>                   Required. TabularPerspectiveHierarchy row id.
  --tabular-hierarchy <id>    Required. TabularHierarchy id for TabularHierarchyId.
  --tabular-perspective <id>  Required. TabularPerspective id for TabularPerspectiveId.

Notes:
  Adds one TabularPerspectiveHierarchy row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-perspective-kpi --help`

```text
Command: add-tabular-perspective-kpi
Usage:
  meta-tabular add-tabular-perspective-kpi [--workspace <path>] --id <id> --tabular-kpi <id>
  --tabular-perspective <id>

Options:

  --workspace <path>          Optional. Workspace path. Default: current working directory.
  --id <id>                   Required. TabularPerspectiveKpi row id.
  --tabular-kpi <id>          Required. TabularKpi id for TabularKpiId.
  --tabular-perspective <id>  Required. TabularPerspective id for TabularPerspectiveId.

Notes:
  Adds one TabularPerspectiveKpi row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-perspective-measure --help`

```text
Command: add-tabular-perspective-measure
Usage:
  meta-tabular add-tabular-perspective-measure [--workspace <path>] --id <id> --tabular-measure <id>
  --tabular-perspective <id>

Options:

  --workspace <path>          Optional. Workspace path. Default: current working directory.
  --id <id>                   Required. TabularPerspectiveMeasure row id.
  --tabular-measure <id>      Required. TabularMeasure id for TabularMeasureId.
  --tabular-perspective <id>  Required. TabularPerspective id for TabularPerspectiveId.

Notes:
  Adds one TabularPerspectiveMeasure row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-perspective-table --help`

```text
Command: add-tabular-perspective-table
Usage:
  meta-tabular add-tabular-perspective-table [--workspace <path>] --id <id> --tabular-perspective
  <id> --tabular-table <id>

Options:

  --workspace <path>          Optional. Workspace path. Default: current working directory.
  --id <id>                   Required. TabularPerspectiveTable row id.
  --tabular-perspective <id>  Required. TabularPerspective id for TabularPerspectiveId.
  --tabular-table <id>        Required. TabularTable id for TabularTableId.

Notes:
  Adds one TabularPerspectiveTable row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-perspective-translation --help`

```text
Command: add-tabular-perspective-translation
Usage:
  meta-tabular add-tabular-perspective-translation [--workspace <path>] --id <id> [--caption
  <value>] [--description <value>] --tabular-culture <id> --tabular-perspective <id>

Options:

  --workspace <path>          Optional. Workspace path. Default: current working directory.
  --id <id>                   Required. TabularPerspectiveTranslation row id.
  --caption <value>           Optional. Caption.
  --description <value>       Optional. Description.
  --tabular-culture <id>      Required. TabularCulture id for TabularCultureId.
  --tabular-perspective <id>  Required. TabularPerspective id for TabularPerspectiveId.

Notes:
  Adds one TabularPerspectiveTranslation row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-relationship --help`

```text
Command: add-tabular-relationship
Usage:
  meta-tabular add-tabular-relationship [--workspace <path>] --id <id> --cardinality <value>
  [--cross-filter-direction <value>] [--is-active <value>] [--is-required <value>] --name <value>
  --from-column <id> --from-table <id> --to-column <id> --to-table <id>

Options:

  --workspace <path>                Optional. Workspace path. Default: current working directory.
  --id <id>                         Required. TabularRelationship row id.
  --cardinality <value>             Required. Cardinality.
  --cross-filter-direction <value>  Optional. CrossFilterDirection.
  --is-active <value>               Optional. IsActive.
  --is-required <value>             Optional. IsRequired.
  --name <value>                    Required. Name.
  --from-column <id>                Required. TabularColumn id for FromColumnId.
  --from-table <id>                 Required. TabularTable id for FromTableId.
  --to-column <id>                  Required. TabularColumn id for ToColumnId.
  --to-table <id>                   Required. TabularTable id for ToTableId.

Notes:
  Adds one TabularRelationship row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-role-filter --help`

```text
Command: add-tabular-role-filter
Usage:
  meta-tabular add-tabular-role-filter [--workspace <path>] --id <id> --expression <value>
  --tabular-security-role <id> --tabular-table <id>

Options:

  --workspace <path>            Optional. Workspace path. Default: current working directory.
  --id <id>                     Required. TabularRoleFilter row id.
  --expression <value>          Required. Expression.
  --tabular-security-role <id>  Required. TabularSecurityRole id for TabularSecurityRoleId.
  --tabular-table <id>          Required. TabularTable id for TabularTableId.

Notes:
  Adds one TabularRoleFilter row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-role-member --help`

```text
Command: add-tabular-role-member
Usage:
  meta-tabular add-tabular-role-member [--workspace <path>] --id <id> [--member-id <value>]
  --member-name <value> --tabular-security-role <id>

Options:

  --workspace <path>            Optional. Workspace path. Default: current working directory.
  --id <id>                     Required. TabularRoleMember row id.
  --member-id <value>           Optional. MemberId.
  --member-name <value>         Required. MemberName.
  --tabular-security-role <id>  Required. TabularSecurityRole id for TabularSecurityRoleId.

Notes:
  Adds one TabularRoleMember row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-security-role --help`

```text
Command: add-tabular-security-role
Usage:
  meta-tabular add-tabular-security-role [--workspace <path>] --id <id> [--description <value>]
  --name <value> --permission <value> --tabular-model <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. TabularSecurityRole row id.
  --description <value>  Optional. Description.
  --name <value>         Required. Name.
  --permission <value>   Required. Permission.
  --tabular-model <id>   Required. TabularModel id for TabularModelId.

Notes:
  Adds one TabularSecurityRole row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-sort-by-column --help`

```text
Command: add-tabular-sort-by-column
Usage:
  meta-tabular add-tabular-sort-by-column [--workspace <path>] --id <id> --sort-column <id>
  --source-column <id>

Options:

  --workspace <path>    Optional. Workspace path. Default: current working directory.
  --id <id>             Required. TabularSortByColumn row id.
  --sort-column <id>    Required. TabularColumn id for SortColumnId.
  --source-column <id>  Required. TabularColumn id for SourceColumnId.

Notes:
  Adds one TabularSortByColumn row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-table --help`

```text
Command: add-tabular-table
Usage:
  meta-tabular add-tabular-table [--workspace <path>] --id <id> [--data-category <value>]
  [--description <value>] [--is-hidden <value>] --name <value> --tabular-model <id>

Options:

  --workspace <path>       Optional. Workspace path. Default: current working directory.
  --id <id>                Required. TabularTable row id.
  --data-category <value>  Optional. DataCategory.
  --description <value>    Optional. Description.
  --is-hidden <value>      Optional. IsHidden.
  --name <value>           Required. Name.
  --tabular-model <id>     Required. TabularModel id for TabularModelId.

Notes:
  Adds one TabularTable row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-table-permission --help`

```text
Command: add-tabular-table-permission
Usage:
  meta-tabular add-tabular-table-permission [--workspace <path>] --id <id> --metadata-permission
  <value> --tabular-security-role <id> --tabular-table <id>

Options:

  --workspace <path>             Optional. Workspace path. Default: current working directory.
  --id <id>                      Required. TabularTablePermission row id.
  --metadata-permission <value>  Required. MetadataPermission.
  --tabular-security-role <id>   Required. TabularSecurityRole id for TabularSecurityRoleId.
  --tabular-table <id>           Required. TabularTable id for TabularTableId.

Notes:
  Adds one TabularTablePermission row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-tabular add-tabular-table-translation --help`

```text
Command: add-tabular-table-translation
Usage:
  meta-tabular add-tabular-table-translation [--workspace <path>] --id <id> [--caption <value>]
  [--description <value>] --tabular-culture <id> --tabular-table <id>

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. TabularTableTranslation row id.
  --caption <value>       Optional. Caption.
  --description <value>   Optional. Description.
  --tabular-culture <id>  Required. TabularCulture id for TabularCultureId.
  --tabular-table <id>    Required. TabularTable id for TabularTableId.

Notes:
  Adds one TabularTableTranslation row to a MetaTabular workspace.
  Defaults to the current working directory when --workspace is omitted.
```

## meta-multi-dimensional

### `meta-multi-dimensional --help`

```text
Usage:
  meta-multi-dimensional [--new-workspace <path> | <command> [options]]

Commands:

  help                               Show this help.
  --new-workspace                    Create an empty MetaMultiDimensional workspace.
  deploy                             Create modeled objects on an Analysis Services multidimensional
                                     instance.
  restore                            Promote a processed multidimensional database through backup
                                     and restore.
  drop                               Drop a multidimensional database from an Analysis Services
                                     multidimensional instance.
  add-action-translation             Add a ActionTranslation row.
  add-attribute-relationship         Add a AttributeRelationship row.
  add-attribute-translation          Add a AttributeTranslation row.
  add-cell-permission                Add a CellPermission row.
  add-cube                           Add a Cube row.
  add-cube-action                    Add a CubeAction row.
  add-cube-dimension                 Add a CubeDimension row.
  add-cube-translation               Add a CubeTranslation row.
  add-culture                        Add a Culture row.
  add-dimension                      Add a Dimension row.
  add-dimension-attribute            Add a DimensionAttribute row.
  add-dimension-hierarchy            Add a DimensionHierarchy row.
  add-dimension-hierarchy-level      Add a DimensionHierarchyLevel row.
  add-dimension-permission           Add a DimensionPermission row.
  add-dimension-translation          Add a DimensionTranslation row.
  add-dimension-usage                Add a DimensionUsage row.
  add-kpi                            Add a Kpi row.
  add-kpi-translation                Add a KpiTranslation row.
  add-mdx-calculation                Add a MdxCalculation row.
  add-measure                        Add a Measure row.
  add-measure-group                  Add a MeasureGroup row.
  add-measure-translation            Add a MeasureTranslation row.
  add-multi-dimensional-data-source  Add a MultiDimensionalDataSource row.
  add-multi-dimensional-database     Add a MultiDimensionalDatabase row.
  add-named-set                      Add a NamedSet row.
  add-named-set-translation          Add a NamedSetTranslation row.
  add-partition                      Add a Partition row.
  add-perspective                    Add a Perspective row.
  add-perspective-action             Add a PerspectiveAction row.
  add-perspective-calculation        Add a PerspectiveCalculation row.
  add-perspective-dimension          Add a PerspectiveDimension row.
  add-perspective-kpi                Add a PerspectiveKpi row.
  add-perspective-measure            Add a PerspectiveMeasure row.
  add-perspective-measure-group      Add a PerspectiveMeasureGroup row.
  add-perspective-named-set          Add a PerspectiveNamedSet row.
  add-perspective-translation        Add a PerspectiveTranslation row.
  add-role-member                    Add a RoleMember row.
  add-security-role                  Add a SecurityRole row.

Next: meta-multi-dimensional add-cube --help
```

### `meta-multi-dimensional --new-workspace --help`

```text
Command: --new-workspace
Usage:
  meta-multi-dimensional --new-workspace <path>

Options:

  --new-workspace <path>  Required. Directory where the empty MetaMultiDimensional workspace will be
                          created.
```

### `meta-multi-dimensional deploy --help`

```text
Command: deploy
Usage:
  meta-multi-dimensional deploy [--workspace <path>] --server <server> [--database-name <name>]
  [--drop-existing] [--no-process]

Options:

  --workspace <path>      MetaMultiDimensional workspace to deploy. Defaults to the current
                          directory.
  --server <server>       Required. Analysis Services multidimensional server.
  --database-name <name>  Optional target database name. Defaults to the modeled database name.
  --drop-existing         Drop an existing target database before create/deploy.
  --no-process            Deploy metadata only and skip full processing.

Notes:
  Creates multidimensional database objects on an Analysis Services multidimensional instance.
  By default, the command runs full processing after deploy and fails if processing fails.
  Without --drop-existing, the command fails if the database already exists.
  With --drop-existing, the command uses the safe drop, create, full-process sequence.
  With --no-process, the command deploys metadata only.
  This deploys modeled data sources, data source views, dimensions, cubes, measure groups, measures, partitions, MDX scripts, actions, and roles.
```

### `meta-multi-dimensional restore --help`

```text
Command: restore
Usage:
  meta-multi-dimensional restore --source-server <server> --source-database-name <name>
  --target-server <server> --target-database-name <name> --backup-file <path> [--drop-existing]
  [--overwrite-backup-file]

Options:

  --source-server <server>       Required. Source Analysis Services server containing the processed
                                 database.
  --source-database-name <name>  Required. Source processed database name.
  --target-server <server>       Required. Target Analysis Services server.
  --target-database-name <name>  Required. Target database name to restore.
  --backup-file <path>           Required. Backup file path accessible to the Analysis Services
                                 service accounts.
  --drop-existing                Drop an existing target database before restore.
  --overwrite-backup-file        Overwrite an existing backup file.

Notes:
  Backs up a processed multidimensional database and restores it as the target database.
  Use this for pre-prod-to-prod promotion after pre-prod deploy and processing succeeds.
  If the target database exists, --drop-existing is required before restore.
  Restore does not process. Partial or object-level processing belongs in a separate command.
  The backup file path must be accessible to the Analysis Services service accounts on both source and target servers.
```

### `meta-multi-dimensional drop --help`

```text
Command: drop
Usage:
  meta-multi-dimensional drop --server <server> --database-name <name>

Options:

  --server <server>       Required. Analysis Services multidimensional server.
  --database-name <name>  Required. Database name to drop.

Notes:
  Drops a multidimensional database from an Analysis Services multidimensional instance.
  This command has no confirmation prompt; use it only with an explicit database name.
  The command fails if the database does not exist.
```

### `meta-multi-dimensional add-action-translation --help`

```text
Command: add-action-translation
Usage:
  meta-multi-dimensional add-action-translation [--workspace <path>] --id <id> [--caption <value>]
  [--description <value>] --cube-action <id> --culture <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. ActionTranslation row id.
  --caption <value>      Optional. Caption.
  --description <value>  Optional. Description.
  --cube-action <id>     Required. CubeAction id for CubeActionId.
  --culture <id>         Required. Culture id for CultureId.

Notes:
  Adds one ActionTranslation row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-attribute-relationship --help`

```text
Command: add-attribute-relationship
Usage:
  meta-multi-dimensional add-attribute-relationship [--workspace <path>] --id <id> [--description
  <value>] [--relationship-type <value>] --child-attribute <id> --parent-attribute <id>

Options:

  --workspace <path>           Optional. Workspace path. Default: current working directory.
  --id <id>                    Required. AttributeRelationship row id.
  --description <value>        Optional. Description.
  --relationship-type <value>  Optional. RelationshipType.
  --child-attribute <id>       Required. DimensionAttribute id for ChildAttributeId.
  --parent-attribute <id>      Required. DimensionAttribute id for ParentAttributeId.

Notes:
  Adds one AttributeRelationship row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-attribute-translation --help`

```text
Command: add-attribute-translation
Usage:
  meta-multi-dimensional add-attribute-translation [--workspace <path>] --id <id> [--caption
  <value>] [--description <value>] --culture <id> --dimension-attribute <id>

Options:

  --workspace <path>          Optional. Workspace path. Default: current working directory.
  --id <id>                   Required. AttributeTranslation row id.
  --caption <value>           Optional. Caption.
  --description <value>       Optional. Description.
  --culture <id>              Required. Culture id for CultureId.
  --dimension-attribute <id>  Required. DimensionAttribute id for DimensionAttributeId.

Notes:
  Adds one AttributeTranslation row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-cell-permission --help`

```text
Command: add-cell-permission
Usage:
  meta-multi-dimensional add-cell-permission [--workspace <path>] --id <id> [--description <value>]
  --expression <value> --cube <id> --security-role <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. CellPermission row id.
  --description <value>  Optional. Description.
  --expression <value>   Required. Expression.
  --cube <id>            Required. Cube id for CubeId.
  --security-role <id>   Required. SecurityRole id for SecurityRoleId.

Notes:
  Adds one CellPermission row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-cube --help`

```text
Command: add-cube
Usage:
  meta-multi-dimensional add-cube [--workspace <path>] --id <id> [--default-measure-name <value>]
  [--description <value>] --name <value> [--processing-mode <value>] [--storage-mode <value>]
  --multi-dimensional-database <id>

Options:

  --workspace <path>                 Optional. Workspace path. Default: current working directory.
  --id <id>                          Required. Cube row id.
  --default-measure-name <value>     Optional. DefaultMeasureName.
  --description <value>              Optional. Description.
  --name <value>                     Required. Name.
  --processing-mode <value>          Optional. ProcessingMode. Default: Regular.
  --storage-mode <value>             Optional. StorageMode. Default: Molap.
  --multi-dimensional-database <id>  Required. MultiDimensionalDatabase id for
                                     MultiDimensionalDatabaseId.

Notes:
  Adds one Cube row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-cube-action --help`

```text
Command: add-cube-action
Usage:
  meta-multi-dimensional add-cube-action [--workspace <path>] --id <id> --action-type <value>
  [--caption <value>] [--description <value>] --expression <value> --name <value> [--target <value>]
  --target-kind <value> --cube <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. CubeAction row id.
  --action-type <value>  Required. ActionType.
  --caption <value>      Optional. Caption.
  --description <value>  Optional. Description.
  --expression <value>   Required. Expression.
  --name <value>         Required. Name.
  --target <value>       Optional. Target.
  --target-kind <value>  Required. TargetKind.
  --cube <id>            Required. Cube id for CubeId.

Notes:
  Adds one CubeAction row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-cube-dimension --help`

```text
Command: add-cube-dimension
Usage:
  meta-multi-dimensional add-cube-dimension [--workspace <path>] --id <id> [--description <value>]
  --name <value> [--role-name <value>] --cube <id> --dimension <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. CubeDimension row id.
  --description <value>  Optional. Description.
  --name <value>         Required. Name.
  --role-name <value>    Optional. RoleName.
  --cube <id>            Required. Cube id for CubeId.
  --dimension <id>       Required. Dimension id for DimensionId.

Notes:
  Adds one CubeDimension row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-cube-translation --help`

```text
Command: add-cube-translation
Usage:
  meta-multi-dimensional add-cube-translation [--workspace <path>] --id <id> [--caption <value>]
  [--description <value>] --cube <id> --culture <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. CubeTranslation row id.
  --caption <value>      Optional. Caption.
  --description <value>  Optional. Description.
  --cube <id>            Required. Cube id for CubeId.
  --culture <id>         Required. Culture id for CultureId.

Notes:
  Adds one CubeTranslation row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-culture --help`

```text
Command: add-culture
Usage:
  meta-multi-dimensional add-culture [--workspace <path>] --id <id> [--description <value>]
  [--language-id <value>] --name <value> --multi-dimensional-database <id>

Options:

  --workspace <path>                 Optional. Workspace path. Default: current working directory.
  --id <id>                          Required. Culture row id.
  --description <value>              Optional. Description.
  --language-id <value>              Optional. LanguageId.
  --name <value>                     Required. Name.
  --multi-dimensional-database <id>  Required. MultiDimensionalDatabase id for
                                     MultiDimensionalDatabaseId.

Notes:
  Adds one Culture row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-dimension --help`

```text
Command: add-dimension
Usage:
  meta-multi-dimensional add-dimension [--workspace <path>] --id <id> [--description <value>]
  [--dimension-type <value>] --name <value> [--processing-group <value>] [--processing-mode <value>]
  [--source-name <value>] [--storage-mode <value>] --multi-dimensional-database <id>

Options:

  --workspace <path>                 Optional. Workspace path. Default: current working directory.
  --id <id>                          Required. Dimension row id.
  --description <value>              Optional. Description.
  --dimension-type <value>           Optional. DimensionType.
  --name <value>                     Required. Name.
  --processing-group <value>         Optional. ProcessingGroup. Default: ByAttribute.
  --processing-mode <value>          Optional. ProcessingMode. Default: Regular.
  --source-name <value>              Optional. SourceName.
  --storage-mode <value>             Optional. StorageMode. Default: Molap.
  --multi-dimensional-database <id>  Required. MultiDimensionalDatabase id for
                                     MultiDimensionalDatabaseId.

Notes:
  Adds one Dimension row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-dimension-attribute --help`

```text
Command: add-dimension-attribute
Usage:
  meta-multi-dimensional add-dimension-attribute [--workspace <path>] --id <id>
  [--attribute-hierarchy-enabled <value>] [--attribute-hierarchy-visible <value>] --data-type-id
  <value> [--description <value>] [--is-key <value>] --name <value> [--ordinal <value>]
  [--source-name <value>] [--usage <value>] --dimension <id>

Options:

  --workspace <path>                     Optional. Workspace path. Default: current working
                                         directory.
  --id <id>                              Required. DimensionAttribute row id.
  --attribute-hierarchy-enabled <value>  Optional. AttributeHierarchyEnabled.
  --attribute-hierarchy-visible <value>  Optional. AttributeHierarchyVisible.
  --data-type-id <value>                 Required. DataTypeId.
  --description <value>                  Optional. Description.
  --is-key <value>                       Optional. IsKey.
  --name <value>                         Required. Name.
  --ordinal <value>                      Optional. Ordinal.
  --source-name <value>                  Optional. SourceName.
  --usage <value>                        Optional. Usage.
  --dimension <id>                       Required. Dimension id for DimensionId.

Notes:
  Adds one DimensionAttribute row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-dimension-hierarchy --help`

```text
Command: add-dimension-hierarchy
Usage:
  meta-multi-dimensional add-dimension-hierarchy [--workspace <path>] --id <id> [--description
  <value>] [--hierarchy-type <value>] --name <value> --dimension <id>

Options:

  --workspace <path>        Optional. Workspace path. Default: current working directory.
  --id <id>                 Required. DimensionHierarchy row id.
  --description <value>     Optional. Description.
  --hierarchy-type <value>  Optional. HierarchyType.
  --name <value>            Required. Name.
  --dimension <id>          Required. Dimension id for DimensionId.

Notes:
  Adds one DimensionHierarchy row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-dimension-hierarchy-level --help`

```text
Command: add-dimension-hierarchy-level
Usage:
  meta-multi-dimensional add-dimension-hierarchy-level [--workspace <path>] --id <id> --name <value>
  [--ordinal <value>] --dimension-attribute <id> --dimension-hierarchy <id>

Options:

  --workspace <path>          Optional. Workspace path. Default: current working directory.
  --id <id>                   Required. DimensionHierarchyLevel row id.
  --name <value>              Required. Name.
  --ordinal <value>           Optional. Ordinal.
  --dimension-attribute <id>  Required. DimensionAttribute id for DimensionAttributeId.
  --dimension-hierarchy <id>  Required. DimensionHierarchy id for DimensionHierarchyId.

Notes:
  Adds one DimensionHierarchyLevel row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-dimension-permission --help`

```text
Command: add-dimension-permission
Usage:
  meta-multi-dimensional add-dimension-permission [--workspace <path>] --id <id>
  [--allowed-set-expression <value>] [--default-member-expression <value>] [--denied-set-expression
  <value>] [--description <value>] [--visual-totals <value>] --dimension-attribute <id> --dimension
  <id> --security-role <id>

Options:

  --workspace <path>                   Optional. Workspace path. Default: current working directory.
  --id <id>                            Required. DimensionPermission row id.
  --allowed-set-expression <value>     Optional. AllowedSetExpression.
  --default-member-expression <value>  Optional. DefaultMemberExpression.
  --denied-set-expression <value>      Optional. DeniedSetExpression.
  --description <value>                Optional. Description.
  --visual-totals <value>              Optional. VisualTotals.
  --dimension-attribute <id>           Required. DimensionAttribute id for DimensionAttributeId.
  --dimension <id>                     Required. Dimension id for DimensionId.
  --security-role <id>                 Required. SecurityRole id for SecurityRoleId.

Notes:
  Adds one DimensionPermission row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-dimension-translation --help`

```text
Command: add-dimension-translation
Usage:
  meta-multi-dimensional add-dimension-translation [--workspace <path>] --id <id> [--caption
  <value>] [--description <value>] --culture <id> --dimension <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. DimensionTranslation row id.
  --caption <value>      Optional. Caption.
  --description <value>  Optional. Description.
  --culture <id>         Required. Culture id for CultureId.
  --dimension <id>       Required. Dimension id for DimensionId.

Notes:
  Adds one DimensionTranslation row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-dimension-usage --help`

```text
Command: add-dimension-usage
Usage:
  meta-multi-dimensional add-dimension-usage [--workspace <path>] --id <id> [--description <value>]
  [--is-required <value>] [--role-name <value>] --usage-kind <value> --cube-dimension <id>
  [--granularity-attribute <id>] [--intermediate-measure-group <id>] --measure-group <id>

Options:

  --workspace <path>                 Optional. Workspace path. Default: current working directory.
  --id <id>                          Required. DimensionUsage row id.
  --description <value>              Optional. Description.
  --is-required <value>              Optional. IsRequired.
  --role-name <value>                Optional. RoleName.
  --usage-kind <value>               Required. UsageKind.
  --cube-dimension <id>              Required. CubeDimension id for CubeDimensionId.
  --granularity-attribute <id>       Optional. DimensionAttribute id for GranularityAttributeId.
  --intermediate-measure-group <id>  Optional. MeasureGroup id for IntermediateMeasureGroupId.
  --measure-group <id>               Required. MeasureGroup id for MeasureGroupId.

Notes:
  Adds one DimensionUsage row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-kpi --help`

```text
Command: add-kpi
Usage:
  meta-multi-dimensional add-kpi [--workspace <path>] --id <id> [--description <value>]
  [--goal-expression <value>] --name <value> [--status-expression <value>] [--status-graphic
  <value>] [--trend-expression <value>] [--trend-graphic <value>] [--value-expression <value>]
  [--associated-measure <id>] --cube <id>

Options:

  --workspace <path>           Optional. Workspace path. Default: current working directory.
  --id <id>                    Required. Kpi row id.
  --description <value>        Optional. Description.
  --goal-expression <value>    Optional. GoalExpression.
  --name <value>               Required. Name.
  --status-expression <value>  Optional. StatusExpression.
  --status-graphic <value>     Optional. StatusGraphic.
  --trend-expression <value>   Optional. TrendExpression.
  --trend-graphic <value>      Optional. TrendGraphic.
  --value-expression <value>   Optional. ValueExpression.
  --associated-measure <id>    Optional. Measure id for AssociatedMeasureId.
  --cube <id>                  Required. Cube id for CubeId.

Notes:
  Adds one Kpi row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-kpi-translation --help`

```text
Command: add-kpi-translation
Usage:
  meta-multi-dimensional add-kpi-translation [--workspace <path>] --id <id> [--caption <value>]
  [--description <value>] --culture <id> --kpi <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. KpiTranslation row id.
  --caption <value>      Optional. Caption.
  --description <value>  Optional. Description.
  --culture <id>         Required. Culture id for CultureId.
  --kpi <id>             Required. Kpi id for KpiId.

Notes:
  Adds one KpiTranslation row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-mdx-calculation --help`

```text
Command: add-mdx-calculation
Usage:
  meta-multi-dimensional add-mdx-calculation [--workspace <path>] --id <id> --calculation-kind
  <value> [--description <value>] [--display-folder <value>] --expression <value> --name <value>
  [--solve-order <value>] --cube <id>

Options:

  --workspace <path>          Optional. Workspace path. Default: current working directory.
  --id <id>                   Required. MdxCalculation row id.
  --calculation-kind <value>  Required. CalculationKind.
  --description <value>       Optional. Description.
  --display-folder <value>    Optional. DisplayFolder.
  --expression <value>        Required. Expression.
  --name <value>              Required. Name.
  --solve-order <value>       Optional. SolveOrder.
  --cube <id>                 Required. Cube id for CubeId.

Notes:
  Adds one MdxCalculation row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-measure --help`

```text
Command: add-measure
Usage:
  meta-multi-dimensional add-measure [--workspace <path>] --id <id> [--aggregate-function <value>]
  [--data-type-id <value>] [--description <value>] [--display-folder <value>] [--format-string
  <value>] --name <value> [--source-name <value>] --measure-group <id>

Options:

  --workspace <path>            Optional. Workspace path. Default: current working directory.
  --id <id>                     Required. Measure row id.
  --aggregate-function <value>  Optional. AggregateFunction.
  --data-type-id <value>        Optional. DataTypeId.
  --description <value>         Optional. Description.
  --display-folder <value>      Optional. DisplayFolder.
  --format-string <value>       Optional. FormatString.
  --name <value>                Required. Name.
  --source-name <value>         Optional. SourceName.
  --measure-group <id>          Required. MeasureGroup id for MeasureGroupId.

Notes:
  Adds one Measure row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-measure-group --help`

```text
Command: add-measure-group
Usage:
  meta-multi-dimensional add-measure-group [--workspace <path>] --id <id> [--description <value>]
  --name <value> [--processing-mode <value>] [--source-name <value>] [--storage-mode <value>] --cube
  <id>

Options:

  --workspace <path>         Optional. Workspace path. Default: current working directory.
  --id <id>                  Required. MeasureGroup row id.
  --description <value>      Optional. Description.
  --name <value>             Required. Name.
  --processing-mode <value>  Optional. ProcessingMode. Default: Regular.
  --source-name <value>      Optional. SourceName.
  --storage-mode <value>     Optional. StorageMode. Default: Molap.
  --cube <id>                Required. Cube id for CubeId.

Notes:
  Adds one MeasureGroup row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-measure-translation --help`

```text
Command: add-measure-translation
Usage:
  meta-multi-dimensional add-measure-translation [--workspace <path>] --id <id> [--caption <value>]
  [--description <value>] --culture <id> --measure <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. MeasureTranslation row id.
  --caption <value>      Optional. Caption.
  --description <value>  Optional. Description.
  --culture <id>         Required. Culture id for CultureId.
  --measure <id>         Required. Measure id for MeasureId.

Notes:
  Adds one MeasureTranslation row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-multi-dimensional-data-source --help`

```text
Command: add-multi-dimensional-data-source
Usage:
  meta-multi-dimensional add-multi-dimensional-data-source [--workspace <path>] --id <id>
  [--connection-reference <value>] [--description <value>] --name <value> [--provider <value>]
  [--source-kind <value>] --multi-dimensional-database <id>

Options:

  --workspace <path>                 Optional. Workspace path. Default: current working directory.
  --id <id>                          Required. MultiDimensionalDataSource row id.
  --connection-reference <value>     Optional. ConnectionReference.
  --description <value>              Optional. Description.
  --name <value>                     Required. Name.
  --provider <value>                 Optional. Provider.
  --source-kind <value>              Optional. SourceKind.
  --multi-dimensional-database <id>  Required. MultiDimensionalDatabase id for
                                     MultiDimensionalDatabaseId.

Notes:
  Adds one MultiDimensionalDataSource row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-multi-dimensional-database --help`

```text
Command: add-multi-dimensional-database
Usage:
  meta-multi-dimensional add-multi-dimensional-database [--workspace <path>] --id <id> [--collation
  <value>] [--default-language <value>] [--description <value>] --name <value>

Options:

  --workspace <path>          Optional. Workspace path. Default: current working directory.
  --id <id>                   Required. MultiDimensionalDatabase row id.
  --collation <value>         Optional. Collation.
  --default-language <value>  Optional. DefaultLanguage.
  --description <value>       Optional. Description.
  --name <value>              Required. Name.

Notes:
  Adds one MultiDimensionalDatabase row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-named-set --help`

```text
Command: add-named-set
Usage:
  meta-multi-dimensional add-named-set [--workspace <path>] --id <id> [--description <value>]
  [--display-folder <value>] --expression <value> --name <value> --cube <id>

Options:

  --workspace <path>        Optional. Workspace path. Default: current working directory.
  --id <id>                 Required. NamedSet row id.
  --description <value>     Optional. Description.
  --display-folder <value>  Optional. DisplayFolder.
  --expression <value>      Required. Expression.
  --name <value>            Required. Name.
  --cube <id>               Required. Cube id for CubeId.

Notes:
  Adds one NamedSet row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-named-set-translation --help`

```text
Command: add-named-set-translation
Usage:
  meta-multi-dimensional add-named-set-translation [--workspace <path>] --id <id> [--caption
  <value>] [--description <value>] --culture <id> --named-set <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. NamedSetTranslation row id.
  --caption <value>      Optional. Caption.
  --description <value>  Optional. Description.
  --culture <id>         Required. Culture id for CultureId.
  --named-set <id>       Required. NamedSet id for NamedSetId.

Notes:
  Adds one NamedSetTranslation row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-partition --help`

```text
Command: add-partition
Usage:
  meta-multi-dimensional add-partition [--workspace <path>] --id <id> [--description <value>] --name
  <value> [--ordinal <value>] [--processing-mode <value>] [--slice-expression <value>]
  [--source-expression <value>] [--storage-mode <value>] --measure-group <id>
  [--multi-dimensional-data-source <id>]

Options:

  --workspace <path>                    Optional. Workspace path. Default: current working
                                        directory.
  --id <id>                             Required. Partition row id.
  --description <value>                 Optional. Description.
  --name <value>                        Required. Name.
  --ordinal <value>                     Optional. Ordinal.
  --processing-mode <value>             Optional. ProcessingMode. Default: Regular.
  --slice-expression <value>            Optional. SliceExpression.
  --source-expression <value>           Optional. SourceExpression.
  --storage-mode <value>                Optional. StorageMode. Default: Molap.
  --measure-group <id>                  Required. MeasureGroup id for MeasureGroupId.
  --multi-dimensional-data-source <id>  Optional. MultiDimensionalDataSource id for
                                        MultiDimensionalDataSourceId.

Notes:
  Adds one Partition row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-perspective --help`

```text
Command: add-perspective
Usage:
  meta-multi-dimensional add-perspective [--workspace <path>] --id <id> [--default-measure-name
  <value>] [--description <value>] --name <value> --cube <id>

Options:

  --workspace <path>              Optional. Workspace path. Default: current working directory.
  --id <id>                       Required. Perspective row id.
  --default-measure-name <value>  Optional. DefaultMeasureName.
  --description <value>           Optional. Description.
  --name <value>                  Required. Name.
  --cube <id>                     Required. Cube id for CubeId.

Notes:
  Adds one Perspective row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-perspective-action --help`

```text
Command: add-perspective-action
Usage:
  meta-multi-dimensional add-perspective-action [--workspace <path>] --id <id> --cube-action <id>
  --perspective <id>

Options:

  --workspace <path>  Optional. Workspace path. Default: current working directory.
  --id <id>           Required. PerspectiveAction row id.
  --cube-action <id>  Required. CubeAction id for CubeActionId.
  --perspective <id>  Required. Perspective id for PerspectiveId.

Notes:
  Adds one PerspectiveAction row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-perspective-calculation --help`

```text
Command: add-perspective-calculation
Usage:
  meta-multi-dimensional add-perspective-calculation [--workspace <path>] --id <id>
  --mdx-calculation <id> --perspective <id>

Options:

  --workspace <path>      Optional. Workspace path. Default: current working directory.
  --id <id>               Required. PerspectiveCalculation row id.
  --mdx-calculation <id>  Required. MdxCalculation id for MdxCalculationId.
  --perspective <id>      Required. Perspective id for PerspectiveId.

Notes:
  Adds one PerspectiveCalculation row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-perspective-dimension --help`

```text
Command: add-perspective-dimension
Usage:
  meta-multi-dimensional add-perspective-dimension [--workspace <path>] --id <id> --cube-dimension
  <id> --perspective <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. PerspectiveDimension row id.
  --cube-dimension <id>  Required. CubeDimension id for CubeDimensionId.
  --perspective <id>     Required. Perspective id for PerspectiveId.

Notes:
  Adds one PerspectiveDimension row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-perspective-kpi --help`

```text
Command: add-perspective-kpi
Usage:
  meta-multi-dimensional add-perspective-kpi [--workspace <path>] --id <id> --kpi <id> --perspective
  <id>

Options:

  --workspace <path>  Optional. Workspace path. Default: current working directory.
  --id <id>           Required. PerspectiveKpi row id.
  --kpi <id>          Required. Kpi id for KpiId.
  --perspective <id>  Required. Perspective id for PerspectiveId.

Notes:
  Adds one PerspectiveKpi row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-perspective-measure --help`

```text
Command: add-perspective-measure
Usage:
  meta-multi-dimensional add-perspective-measure [--workspace <path>] --id <id> --measure <id>
  --perspective <id>

Options:

  --workspace <path>  Optional. Workspace path. Default: current working directory.
  --id <id>           Required. PerspectiveMeasure row id.
  --measure <id>      Required. Measure id for MeasureId.
  --perspective <id>  Required. Perspective id for PerspectiveId.

Notes:
  Adds one PerspectiveMeasure row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-perspective-measure-group --help`

```text
Command: add-perspective-measure-group
Usage:
  meta-multi-dimensional add-perspective-measure-group [--workspace <path>] --id <id>
  --measure-group <id> --perspective <id>

Options:

  --workspace <path>    Optional. Workspace path. Default: current working directory.
  --id <id>             Required. PerspectiveMeasureGroup row id.
  --measure-group <id>  Required. MeasureGroup id for MeasureGroupId.
  --perspective <id>    Required. Perspective id for PerspectiveId.

Notes:
  Adds one PerspectiveMeasureGroup row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-perspective-named-set --help`

```text
Command: add-perspective-named-set
Usage:
  meta-multi-dimensional add-perspective-named-set [--workspace <path>] --id <id> --named-set <id>
  --perspective <id>

Options:

  --workspace <path>  Optional. Workspace path. Default: current working directory.
  --id <id>           Required. PerspectiveNamedSet row id.
  --named-set <id>    Required. NamedSet id for NamedSetId.
  --perspective <id>  Required. Perspective id for PerspectiveId.

Notes:
  Adds one PerspectiveNamedSet row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-perspective-translation --help`

```text
Command: add-perspective-translation
Usage:
  meta-multi-dimensional add-perspective-translation [--workspace <path>] --id <id> [--caption
  <value>] [--description <value>] --culture <id> --perspective <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. PerspectiveTranslation row id.
  --caption <value>      Optional. Caption.
  --description <value>  Optional. Description.
  --culture <id>         Required. Culture id for CultureId.
  --perspective <id>     Required. Perspective id for PerspectiveId.

Notes:
  Adds one PerspectiveTranslation row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-role-member --help`

```text
Command: add-role-member
Usage:
  meta-multi-dimensional add-role-member [--workspace <path>] --id <id> --member-name <value>
  [--member-sid <value>] --security-role <id>

Options:

  --workspace <path>     Optional. Workspace path. Default: current working directory.
  --id <id>              Required. RoleMember row id.
  --member-name <value>  Required. MemberName.
  --member-sid <value>   Optional. MemberSid.
  --security-role <id>   Required. SecurityRole id for SecurityRoleId.

Notes:
  Adds one RoleMember row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```

### `meta-multi-dimensional add-security-role --help`

```text
Command: add-security-role
Usage:
  meta-multi-dimensional add-security-role [--workspace <path>] --id <id> [--description <value>]
  --name <value> --permission <value> --multi-dimensional-database <id>

Options:

  --workspace <path>                 Optional. Workspace path. Default: current working directory.
  --id <id>                          Required. SecurityRole row id.
  --description <value>              Optional. Description.
  --name <value>                     Required. Name.
  --permission <value>               Required. Permission.
  --multi-dimensional-database <id>  Required. MultiDimensionalDatabase id for
                                     MultiDimensionalDatabaseId.

Notes:
  Adds one SecurityRole row to a MetaMultiDimensional workspace.
  Defaults to the current working directory when --workspace is omitted.
```
