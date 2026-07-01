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
  --timeout-seconds <seconds>  Command timeout for each SQL batch. 0 or omitted means no timeout.
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
  meta-datavault-raw <command> [options]

Commands:
  add-hub                              Add a RawHub row.
  add-hub-key-part                     Add a RawHubKeyPart row.
  add-hub-satellite                    Add a RawHubSatellite row.
  add-hub-satellite-attribute          Add a RawHubSatelliteAttribute row.
  add-link                             Add a RawLink row.
  add-link-hub                         Add a RawLinkHub row.
  add-link-satellite                   Add a RawLinkSatellite row.
  add-link-satellite-attribute         Add a RawLinkSatelliteAttribute row.
  add-source-field                     Add a SourceField row.
  add-source-field-data-type-detail    Add a SourceFieldDataTypeDetail row.
  add-source-schema                    Add a SourceSchema row.
  add-source-system                    Add a SourceSystem row.
  add-source-table                     Add a SourceTable row.
  add-source-table-relationship        Add a SourceTableRelationship row.
  add-source-table-relationship-field  Add a SourceTableRelationshipField row.
  help                                 Show help.
  new-workspace                        Create a MetaRawDataVault workspace.

Next: meta-datavault-raw help <command>
```

### `meta-datavault-raw add-hub --help`

```text
Usage:
  meta-datavault-raw add-hub [--workspace <path>] --id <value> --name <value> --source-table <value>

Add a RawHub row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  RawHub row id.
  --name <value>                Name.
  --source-table <value>        SourceTable id for SourceTableId.
```

### `meta-datavault-raw add-hub-key-part --help`

```text
Usage:
  meta-datavault-raw add-hub-key-part [--workspace <path>] --id <value> --name <value> [--ordinal <value>] --hub <value> --source-field <value>

Add a RawHubKeyPart row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  RawHubKeyPart row id.
  --name <value>                Name.
  --ordinal <value>             Ordinal.
  --hub <value>                 RawHub id for RawHubId.
  --source-field <value>        SourceField id for SourceFieldId.
```

### `meta-datavault-raw add-hub-satellite --help`

```text
Usage:
  meta-datavault-raw add-hub-satellite [--workspace <path>] --id <value> --name <value> --satellite-kind <value> --hub <value> --source-table <value>

Add a RawHubSatellite row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  RawHubSatellite row id.
  --name <value>                Name.
  --satellite-kind <value>      SatelliteKind.
  --hub <value>                 RawHub id for RawHubId.
  --source-table <value>        SourceTable id for SourceTableId.
```

### `meta-datavault-raw add-hub-satellite-attribute --help`

```text
Usage:
  meta-datavault-raw add-hub-satellite-attribute [--workspace <path>] --id <value> --name <value> [--ordinal <value>] --hub-satellite <value> --source-field <value>

Add a RawHubSatelliteAttribute row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  RawHubSatelliteAttribute row id.
  --name <value>                Name.
  --ordinal <value>             Ordinal.
  --hub-satellite <value>       RawHubSatellite id for RawHubSatelliteId.
  --source-field <value>        SourceField id for SourceFieldId.
```

### `meta-datavault-raw add-link --help`

```text
Usage:
  meta-datavault-raw add-link [--workspace <path>] --id <value> --link-kind <value> --name <value> --source-relationship <value>

Add a RawLink row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  RawLink row id.
  --link-kind <value>           LinkKind.
  --name <value>                Name.
  --source-relationship <value>  SourceTableRelationship id for SourceTableRelationshipId.
```

### `meta-datavault-raw add-link-hub --help`

```text
Usage:
  meta-datavault-raw add-link-hub [--workspace <path>] --id <value> [--ordinal <value>] [--role-name <value>] --hub <value> --link <value>

Add a RawLinkHub row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  RawLinkHub row id.
  --ordinal <value>             Ordinal.
  --role-name <value>           RoleName.
  --hub <value>                 RawHub id for RawHubId.
  --link <value>                RawLink id for RawLinkId.
```

### `meta-datavault-raw add-link-satellite --help`

```text
Usage:
  meta-datavault-raw add-link-satellite [--workspace <path>] --id <value> --name <value> --satellite-kind <value> --link <value> --source-table <value>

Add a RawLinkSatellite row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  RawLinkSatellite row id.
  --name <value>                Name.
  --satellite-kind <value>      SatelliteKind.
  --link <value>                RawLink id for RawLinkId.
  --source-table <value>        SourceTable id for SourceTableId.
```

### `meta-datavault-raw add-link-satellite-attribute --help`

```text
Usage:
  meta-datavault-raw add-link-satellite-attribute [--workspace <path>] --id <value> --name <value> [--ordinal <value>] --link-satellite <value> --source-field <value>

Add a RawLinkSatelliteAttribute row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  RawLinkSatelliteAttribute row id.
  --name <value>                Name.
  --ordinal <value>             Ordinal.
  --link-satellite <value>      RawLinkSatellite id for RawLinkSatelliteId.
  --source-field <value>        SourceField id for SourceFieldId.
```

### `meta-datavault-raw add-source-field --help`

```text
Usage:
  meta-datavault-raw add-source-field [--workspace <path>] --id <value> --data-type-id <value> [--is-nullable true|false] --name <value> [--ordinal <value>] --table <value>

Add a SourceField row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  SourceField row id.
  --data-type-id <value>        DataTypeId.
  --is-nullable true|false      IsNullable.
  --name <value>                Name.
  --ordinal <value>             Ordinal.
  --table <value>               SourceTable id for SourceTableId.
```

### `meta-datavault-raw add-source-field-data-type-detail --help`

```text
Usage:
  meta-datavault-raw add-source-field-data-type-detail [--workspace <path>] --id <value> --name <value> --value <value> --field <value>

Add a SourceFieldDataTypeDetail row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  SourceFieldDataTypeDetail row id.
  --name <value>                Name.
  --value <value>               Value.
  --field <value>               SourceField id for SourceFieldId.
```

### `meta-datavault-raw add-source-schema --help`

```text
Usage:
  meta-datavault-raw add-source-schema [--workspace <path>] --id <value> --name <value> --system <value>

Add a SourceSchema row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  SourceSchema row id.
  --name <value>                Name.
  --system <value>              SourceSystem id for SourceSystemId.
```

### `meta-datavault-raw add-source-system --help`

```text
Usage:
  meta-datavault-raw add-source-system [--workspace <path>] --id <value> [--description <value>] --name <value>

Add a SourceSystem row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  SourceSystem row id.
  --description <value>         Description.
  --name <value>                Name.
```

### `meta-datavault-raw add-source-table --help`

```text
Usage:
  meta-datavault-raw add-source-table [--workspace <path>] --id <value> --name <value> --schema <value>

Add a SourceTable row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  SourceTable row id.
  --name <value>                Name.
  --schema <value>              SourceSchema id for SourceSchemaId.
```

### `meta-datavault-raw add-source-table-relationship --help`

```text
Usage:
  meta-datavault-raw add-source-table-relationship [--workspace <path>] --id <value> --name <value> --source-table <value> --target-table <value>

Add a SourceTableRelationship row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  SourceTableRelationship row id.
  --name <value>                Name.
  --source-table <value>        SourceTable id for SourceTableId.
  --target-table <value>        SourceTable id for TargetTableId.
```

### `meta-datavault-raw add-source-table-relationship-field --help`

```text
Usage:
  meta-datavault-raw add-source-table-relationship-field [--workspace <path>] --id <value> [--ordinal <value>] --source-field <value> --relationship <value> --target-field <value>

Add a SourceTableRelationshipField row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  SourceTableRelationshipField row id.
  --ordinal <value>             Ordinal.
  --source-field <value>        SourceField id for SourceFieldId.
  --relationship <value>        SourceTableRelationship id for SourceTableRelationshipId.
  --target-field <value>        SourceField id for TargetFieldId.
```

### `meta-datavault-raw new-workspace --help`

```text
Usage:
  meta-datavault-raw new-workspace <path>

Create a MetaRawDataVault workspace.

Arguments:
  <path>
```

## meta-datavault-business

### `meta-datavault-business --help`

```text
Usage:
  meta-datavault-business <command> [options]

Commands:
  add-bridge                                 Add a BusinessBridge row.
  add-bridge-hub                             Add a BusinessBridgeHub row.
  add-bridge-link                            Add a BusinessBridgeLink row.
  add-hierarchical-link                      Add a BusinessHierarchicalLink row.
  add-hierarchical-link-satellite            Add a BusinessHierarchicalLinkSatellite row.
  add-hierarchical-link-satellite-attribute  Add a BusinessHierarchicalLinkSatelliteAttribute row.
  add-hub                                    Add a BusinessHub row.
  add-hub-key-part                           Add a BusinessHubKeyPart row.
  add-hub-satellite                          Add a BusinessHubSatellite row.
  add-hub-satellite-attribute                Add a BusinessHubSatelliteAttribute row.
  add-link                                   Add a BusinessLink row.
  add-link-hub                               Add a BusinessLinkHub row.
  add-link-satellite                         Add a BusinessLinkSatellite row.
  add-link-satellite-attribute               Add a BusinessLinkSatelliteAttribute row.
  add-point-in-time                          Add a BusinessPointInTime row.
  add-point-in-time-hub-satellite            Add a BusinessPointInTimeHubSatellite row.
  add-point-in-time-link-satellite           Add a BusinessPointInTimeLinkSatellite row.
  add-point-in-time-stamp                    Add a BusinessPointInTimeStamp row.
  add-reference                              Add a BusinessReference row.
  add-reference-key-part                     Add a BusinessReferenceKeyPart row.
  add-reference-satellite                    Add a BusinessReferenceSatellite row.
  add-reference-satellite-attribute          Add a BusinessReferenceSatelliteAttribute row.
  add-same-as-link                           Add a BusinessSameAsLink row.
  add-same-as-link-satellite                 Add a BusinessSameAsLinkSatellite row.
  add-same-as-link-satellite-attribute       Add a BusinessSameAsLinkSatelliteAttribute row.
  help                                       Show help.
  new-workspace                              Create a MetaBusinessDataVault workspace.

Next: meta-datavault-business help <command>
```

### `meta-datavault-business add-bridge --help`

```text
Usage:
  meta-datavault-business add-bridge [--workspace <path>] --id <value> --name <value> [--description <value>] --hub <value>

Add a BusinessBridge row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --description <value>
  --hub <value>                 BusinessHub id.
```

### `meta-datavault-business add-bridge-hub --help`

```text
Usage:
  meta-datavault-business add-bridge-hub [--workspace <path>] --id <value> [--ordinal <value>] [--role-name <value>] --bridge <value> --hub <value>

Add a BusinessBridgeHub row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --ordinal <value>
  --role-name <value>
  --bridge <value>              BusinessBridge id.
  --hub <value>                 BusinessHub id.
```

### `meta-datavault-business add-bridge-link --help`

```text
Usage:
  meta-datavault-business add-bridge-link [--workspace <path>] --id <value> [--ordinal <value>] [--role-name <value>] --bridge <value> --link <value>

Add a BusinessBridgeLink row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --ordinal <value>
  --role-name <value>
  --bridge <value>              BusinessBridge id.
  --link <value>                BusinessLink id.
```

### `meta-datavault-business add-hierarchical-link --help`

```text
Usage:
  meta-datavault-business add-hierarchical-link [--workspace <path>] --id <value> --name <value> [--description <value>] --parent-hub <value> --child-hub <value>

Add a BusinessHierarchicalLink row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --description <value>
  --parent-hub <value>          BusinessHub id.
  --child-hub <value>           BusinessHub id.
```

### `meta-datavault-business add-hierarchical-link-satellite --help`

```text
Usage:
  meta-datavault-business add-hierarchical-link-satellite [--workspace <path>] --id <value> --name <value> [--description <value>] --hierarchical-link <value>

Add a BusinessHierarchicalLinkSatellite row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --description <value>
  --hierarchical-link <value>   BusinessHierarchicalLink id.
```

### `meta-datavault-business add-hierarchical-link-satellite-attribute --help`

```text
Usage:
  meta-datavault-business add-hierarchical-link-satellite-attribute [--workspace <path>] --id <value> --name <value> --data-type-id <value> [--ordinal <value>] --hierarchical-link-satellite <value> [--length <value>] [--precision <value>] [--scale <value>]

Add a BusinessHierarchicalLinkSatelliteAttribute row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --data-type-id <value>
  --ordinal <value>
  --hierarchical-link-satellite <value>  BusinessHierarchicalLinkSatellite id.
  --length <value>              Optional length datatype facet authored as metadata.
  --precision <value>           Optional precision datatype facet authored as metadata.
  --scale <value>               Optional scale datatype facet authored as metadata.
```

### `meta-datavault-business add-hub --help`

```text
Usage:
  meta-datavault-business add-hub [--workspace <path>] --id <value> --name <value> [--description <value>]

Add a BusinessHub row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --description <value>
```

### `meta-datavault-business add-hub-key-part --help`

```text
Usage:
  meta-datavault-business add-hub-key-part [--workspace <path>] --id <value> --name <value> --data-type-id <value> [--ordinal <value>] --hub <value> [--length <value>] [--precision <value>] [--scale <value>]

Add a BusinessHubKeyPart row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --data-type-id <value>
  --ordinal <value>
  --hub <value>                 BusinessHub id.
  --length <value>              Optional length datatype facet authored as metadata.
  --precision <value>           Optional precision datatype facet authored as metadata.
  --scale <value>               Optional scale datatype facet authored as metadata.
```

### `meta-datavault-business add-hub-satellite --help`

```text
Usage:
  meta-datavault-business add-hub-satellite [--workspace <path>] --id <value> --name <value> [--description <value>] --hub <value>

Add a BusinessHubSatellite row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --description <value>
  --hub <value>                 BusinessHub id.
```

### `meta-datavault-business add-hub-satellite-attribute --help`

```text
Usage:
  meta-datavault-business add-hub-satellite-attribute [--workspace <path>] --id <value> --name <value> --data-type-id <value> [--ordinal <value>] --hub-satellite <value> [--length <value>] [--precision <value>] [--scale <value>]

Add a BusinessHubSatelliteAttribute row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --data-type-id <value>
  --ordinal <value>
  --hub-satellite <value>       BusinessHubSatellite id.
  --length <value>              Optional length datatype facet authored as metadata.
  --precision <value>           Optional precision datatype facet authored as metadata.
  --scale <value>               Optional scale datatype facet authored as metadata.
```

### `meta-datavault-business add-link --help`

```text
Usage:
  meta-datavault-business add-link [--workspace <path>] --id <value> --name <value> [--description <value>]

Add a BusinessLink row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --description <value>
```

### `meta-datavault-business add-link-hub --help`

```text
Usage:
  meta-datavault-business add-link-hub [--workspace <path>] --id <value> [--ordinal <value>] [--role-name <value>] --link <value> --hub <value>

Add a BusinessLinkHub row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --ordinal <value>
  --role-name <value>
  --link <value>                BusinessLink id.
  --hub <value>                 BusinessHub id.
```

### `meta-datavault-business add-link-satellite --help`

```text
Usage:
  meta-datavault-business add-link-satellite [--workspace <path>] --id <value> --name <value> [--description <value>] --link <value>

Add a BusinessLinkSatellite row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --description <value>
  --link <value>                BusinessLink id.
```

### `meta-datavault-business add-link-satellite-attribute --help`

```text
Usage:
  meta-datavault-business add-link-satellite-attribute [--workspace <path>] --id <value> --name <value> --data-type-id <value> [--ordinal <value>] --link-satellite <value> [--length <value>] [--precision <value>] [--scale <value>]

Add a BusinessLinkSatelliteAttribute row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --data-type-id <value>
  --ordinal <value>
  --link-satellite <value>      BusinessLinkSatellite id.
  --length <value>              Optional length datatype facet authored as metadata.
  --precision <value>           Optional precision datatype facet authored as metadata.
  --scale <value>               Optional scale datatype facet authored as metadata.
```

### `meta-datavault-business add-point-in-time --help`

```text
Usage:
  meta-datavault-business add-point-in-time [--workspace <path>] --id <value> --name <value> [--description <value>] --hub <value>

Add a BusinessPointInTime row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --description <value>
  --hub <value>                 BusinessHub id.
```

### `meta-datavault-business add-point-in-time-hub-satellite --help`

```text
Usage:
  meta-datavault-business add-point-in-time-hub-satellite [--workspace <path>] --id <value> [--ordinal <value>] --point-in-time <value> --hub-satellite <value>

Add a BusinessPointInTimeHubSatellite row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --ordinal <value>
  --point-in-time <value>       BusinessPointInTime id.
  --hub-satellite <value>       BusinessHubSatellite id.
```

### `meta-datavault-business add-point-in-time-link-satellite --help`

```text
Usage:
  meta-datavault-business add-point-in-time-link-satellite [--workspace <path>] --id <value> [--ordinal <value>] --point-in-time <value> --link-satellite <value>

Add a BusinessPointInTimeLinkSatellite row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --ordinal <value>
  --point-in-time <value>       BusinessPointInTime id.
  --link-satellite <value>      BusinessLinkSatellite id.
```

### `meta-datavault-business add-point-in-time-stamp --help`

```text
Usage:
  meta-datavault-business add-point-in-time-stamp [--workspace <path>] --id <value> --name <value> --data-type-id <value> [--ordinal <value>] --point-in-time <value> [--length <value>] [--precision <value>] [--scale <value>]

Add a BusinessPointInTimeStamp row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --data-type-id <value>
  --ordinal <value>
  --point-in-time <value>       BusinessPointInTime id.
  --length <value>              Optional length datatype facet authored as metadata.
  --precision <value>           Optional precision datatype facet authored as metadata.
  --scale <value>               Optional scale datatype facet authored as metadata.
```

### `meta-datavault-business add-reference --help`

```text
Usage:
  meta-datavault-business add-reference [--workspace <path>] --id <value> --name <value> [--description <value>]

Add a BusinessReference row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --description <value>
```

### `meta-datavault-business add-reference-key-part --help`

```text
Usage:
  meta-datavault-business add-reference-key-part [--workspace <path>] --id <value> --name <value> --data-type-id <value> [--ordinal <value>] --reference <value> [--length <value>] [--precision <value>] [--scale <value>]

Add a BusinessReferenceKeyPart row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --data-type-id <value>
  --ordinal <value>
  --reference <value>           BusinessReference id.
  --length <value>              Optional length datatype facet authored as metadata.
  --precision <value>           Optional precision datatype facet authored as metadata.
  --scale <value>               Optional scale datatype facet authored as metadata.
```

### `meta-datavault-business add-reference-satellite --help`

```text
Usage:
  meta-datavault-business add-reference-satellite [--workspace <path>] --id <value> --name <value> [--description <value>] --reference <value>

Add a BusinessReferenceSatellite row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --description <value>
  --reference <value>           BusinessReference id.
```

### `meta-datavault-business add-reference-satellite-attribute --help`

```text
Usage:
  meta-datavault-business add-reference-satellite-attribute [--workspace <path>] --id <value> --name <value> --data-type-id <value> [--ordinal <value>] --reference-satellite <value> [--length <value>] [--precision <value>] [--scale <value>]

Add a BusinessReferenceSatelliteAttribute row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --data-type-id <value>
  --ordinal <value>
  --reference-satellite <value>  BusinessReferenceSatellite id.
  --length <value>              Optional length datatype facet authored as metadata.
  --precision <value>           Optional precision datatype facet authored as metadata.
  --scale <value>               Optional scale datatype facet authored as metadata.
```

### `meta-datavault-business add-same-as-link --help`

```text
Usage:
  meta-datavault-business add-same-as-link [--workspace <path>] --id <value> --name <value> [--description <value>] --primary-hub <value> --equivalent-hub <value>

Add a BusinessSameAsLink row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --description <value>
  --primary-hub <value>         BusinessHub id.
  --equivalent-hub <value>      BusinessHub id.
```

### `meta-datavault-business add-same-as-link-satellite --help`

```text
Usage:
  meta-datavault-business add-same-as-link-satellite [--workspace <path>] --id <value> --name <value> [--description <value>] --same-as-link <value>

Add a BusinessSameAsLinkSatellite row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --description <value>
  --same-as-link <value>        BusinessSameAsLink id.
```

### `meta-datavault-business add-same-as-link-satellite-attribute --help`

```text
Usage:
  meta-datavault-business add-same-as-link-satellite-attribute [--workspace <path>] --id <value> --name <value> --data-type-id <value> [--ordinal <value>] --same-as-link-satellite <value> [--length <value>] [--precision <value>] [--scale <value>]

Add a BusinessSameAsLinkSatelliteAttribute row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>
  --name <value>
  --data-type-id <value>
  --ordinal <value>
  --same-as-link-satellite <value>  BusinessSameAsLinkSatellite id.
  --length <value>              Optional length datatype facet authored as metadata.
  --precision <value>           Optional precision datatype facet authored as metadata.
  --scale <value>               Optional scale datatype facet authored as metadata.
```

### `meta-datavault-business new-workspace --help`

```text
Usage:
  meta-datavault-business new-workspace <Path>

Create a MetaBusinessDataVault workspace.

Arguments:
  <Path>
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
  meta-pipeline new-workspace <path>
Usage:
  meta-pipeline <command> [options]

Commands:

  execute              Execute a modeled pipeline's serial task chain.
  execute-worker       Execute a modeled pipeline under an orchestration worker protocol.
  execute-step         Execute one modeled pipeline step.
  execute-sqlserver    Execute the direct SQL Server runtime slice.
  create-pipeline-db   Create or update the MetaPipeline operational DB.
  prune-pipeline-db    Prune old MetaPipeline operational diagnostic logs.
  add-pipeline         Add one Pipeline instance to a MetaPipeline workspace.
  add-step             Add one transform-backed step to a pipeline.
  add-executable-step  Add one executable process step to a pipeline.
  inspect              Show a compact MetaPipeline workspace summary.
  help                 Show this help.

Notes:
  --new-workspace creates an empty sanctioned MetaPipeline workspace.

Next: meta-pipeline add-pipeline --help
```

### `meta-pipeline execute --help`

```text
Command: execute
Usage:
  meta-pipeline execute --workspace <path> --pipeline <name> [--transform-workspace <path>]
  [--binding-workspace <path>] [--data-type-conversion-workspace <path>]
  [--pipeline-db-connection-env <name>]

Options:

  --workspace <path>                       Required. MetaPipeline workspace that contains the
                                           modeled serial task chain.
  --pipeline <name>                        Required. Pipeline name to execute.
  --transform-workspace <path>             Required when the pipeline contains transform tasks.
                                           MetaTransformScript workspace used by transform tasks.
  --binding-workspace <path>               Required when the pipeline contains transform tasks.
                                           MetaTransformBinding workspace used by transform tasks.
  --data-type-conversion-workspace <path>  Optional conversion policy workspace; omitted uses the
                                           built-in defaults.
  --pipeline-db-connection-env <name>      Optional shell-visible environment variable for an
                                           initialized MetaPipeline operational DB.

Notes:
  Executes the serial PipelineTask chain declared in a MetaPipeline workspace.
  Every transform task requires a binding workspace.
  Executable tasks do not require transform or binding workspaces.
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

### `meta-pipeline execute-worker --help`

```text
Command: execute-worker
Usage:
  meta-pipeline execute-worker --workspace <path> --pipeline <name> --control-pipe <name>
  [--transform-workspace <path>] [--binding-workspace <path>]
  [--control-pipe-connect-timeout-seconds <n>] [--data-type-conversion-workspace <path>]
  [--pipeline-db-connection-env <name>]

Options:

  --workspace <path>                          Required. MetaPipeline workspace that contains the
                                              modeled serial task chain.
  --pipeline <name>                           Required. Pipeline name to execute as a worker.
  --transform-workspace <path>                Required when the pipeline contains transform tasks.
                                              MetaTransformScript workspace used by transform tasks.
  --binding-workspace <path>                  Required when the pipeline contains transform tasks.
                                              MetaTransformBinding workspace used by transform
                                              tasks.
  --control-pipe <name>                       Required. Named pipe used for orchestration worker
                                              control messages.
  --control-pipe-connect-timeout-seconds <n>  Optional timeout while connecting to the orchestration
                                              control pipe. 0 or omitted means no timeout.
  --data-type-conversion-workspace <path>     Optional conversion policy workspace; omitted uses the
                                              built-in defaults.
  --pipeline-db-connection-env <name>         Optional shell-visible environment variable for an
                                              initialized MetaPipeline operational DB.

Notes:
  This command is an orchestration worker boundary, not an interactive user surface.
  The process loads the whole modeled pipeline once and preserves that pipeline context.
  It uses the named pipe control channel for typed WorkerOnline/WorkerReady/PipelineStarted/TaskReady events and StartPipeline, GrantTask, StopPipeline, or FailPipeline commands.
  The worker waits for StartPipeline before it emits PipelineStarted or any TaskReady task boundary.
  If StartPipeline carries a task id, the worker resumes at that task boundary and does not replay earlier tasks in the same pipeline.
  stdout and stderr are diagnostics only; they are not the worker control plane.
  After TaskFailed it waits at the failed task boundary for retry, stop, or fail commands instead of advancing automatically.
  MetaOrchestration owns cross-pipeline task synchronization; MetaPipeline owns in-process pipeline execution and operational DB evidence.

Examples:

  meta-pipeline execute-worker --workspace .\PipelineWS --pipeline CustomerLoad --transform-workspace .\TransformWS --binding-workspace .\BindingWS --control-pipe meta-worker-123
```

### `meta-pipeline execute-step --help`

```text
Command: execute-step
Usage:
  meta-pipeline execute-step --workspace <path> --pipeline <name> --step-name <name-or-id>
  [--transform-workspace <path>] [--binding-workspace <path>] [--data-type-conversion-workspace
  <path>] [--pipeline-db-connection-env <name>]

Options:

  --workspace <path>                       Required. MetaPipeline workspace that contains the
                                           modeled step.
  --pipeline <name>                        Required. Pipeline name containing the step.
  --step-name <name-or-id>                 Required. Pipeline task name or id to execute.
  --transform-workspace <path>             Required when the step is a transform task.
                                           MetaTransformScript workspace used by the step.
  --binding-workspace <path>               Required when the step is a transform task.
                                           MetaTransformBinding workspace used by the step.
  --data-type-conversion-workspace <path>  Optional conversion policy workspace; omitted uses the
                                           built-in defaults.
  --pipeline-db-connection-env <name>      Optional shell-visible environment variable for an
                                           initialized MetaPipeline operational DB.

Notes:
  Executes exactly one PipelineTask declared in a MetaPipeline workspace.
  Executable steps can be selected without transform or binding workspaces.
  The command does not traverse predecessor or successor tasks.
  SELECT-kind steps execute their paired InsertRows target write when modeled.
  Non-SELECT steps execute directly through the modeled execution connection.
  Connection references in the model name shell-visible environment variables.
  This command is a diagnostic/debugging surface. MetaOrchestration uses execute-worker so pipeline context is not erased between tasks.

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
  --timeout-seconds <n>                    SQL command and bulk-copy timeout seconds. 0 or omitted
                                           means no command timeout.
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
                                     execution. 0 or omitted means no timeout.
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

### `meta-pipeline add-executable-step --help`

```text
Command: add-executable-step
Usage:
  meta-pipeline add-executable-step --workspace <path> --pipeline <name> --executable <path>
  [--step-name <name>] [--arguments <text>] [--working-directory <path>] [--success-exit-code <n>]
  [--timeout-seconds <n>]

Options:

  --workspace <path>          Required. Existing MetaPipeline workspace to update.
  --pipeline <name>           Required. Pipeline that receives the new serial step.
  --executable <path>         Required. Executable path or executable name resolvable by the
                              operating system.
  --step-name <name>          Optional step name; omitted derives a deterministic name from the
                              executable file name.
  --arguments <text>          Optional raw command-line arguments passed to the executable.
  --working-directory <path>  Optional process working directory.
  --success-exit-code <n>     Expected process exit code. Default: 0.
  --timeout-seconds <n>       Process timeout seconds. 0 or omitted means no timeout.

Notes:
  Appends one executable-backed task instance to the pipeline's serial task chain.
  The executable path, arguments, working directory, expected success exit code, and optional timeout are modeled in the workspace.
  Runtime success is determined by the real process exit code.
  Connection strings are not involved in executable tasks.
  Use meta-pipeline execute to execute the modeled executable task.

Examples:

  meta-pipeline add-executable-step --workspace .\PipelineWS --pipeline CustomerLoad --step-name prepare-files --executable dotnet --arguments "--info"
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
  meta-orchestration <command> [options]

Commands:
  add-dependency           Record an explicit success or failure dependency between tasks.
  add-order                Record an explicit success dependency/order resolution in an
                           orchestration workspace.
  allow-concurrent-append  Allow concurrent execution for multiple Append effects on one object.
  execute                  Execute the current run plan by coordinating meta-pipeline worker
                           processes.
  explain-issue            Explain one analyzer issue and its participating pipelines.
  help                     Show help.
  infer                    Infer a MetaOrchestration workspace from a modeled MetaPipeline workspace.
  inspect                  Inspect a MetaOrchestration workspace.
  inspect-run-plan         Inspect the planned task dependency graph.
  list-issues              List analyzer issues recorded in an orchestration workspace.
  refresh-run-plan         Refresh lock-aware run-plan rows in an orchestration workspace.
  set-lock-policy          Record scoped lock compatibility for one object/effect interaction.

Next: meta-orchestration help <command>
```

### `meta-orchestration infer --help`

```text
Usage:
  meta-orchestration infer --pipeline-workspace <path> --new-workspace <path> [--description <value>]

Infer a MetaOrchestration workspace from a modeled MetaPipeline workspace.

Options:
  --pipeline-workspace <path>   MetaPipeline workspace to analyze.
  --new-workspace <path>        Directory where the MetaOrchestration workspace will be created.
  --description <value>         Description recorded on the orchestration plan.
```

### `meta-orchestration inspect --help`

```text
Usage:
  meta-orchestration inspect [--workspace <path>]

Options:
  --workspace <path>  MetaOrchestration workspace. Defaults to the current directory.
```

### `meta-orchestration list-issues --help`

```text
Usage:
  meta-orchestration list-issues [--workspace <path>]

Options:
  --workspace <path>  MetaOrchestration workspace. Defaults to the current directory.
```

### `meta-orchestration explain-issue --help`

```text
Usage:
  meta-orchestration explain-issue [--workspace <path>] --issue <value>

Options:
  --workspace <path>   MetaOrchestration workspace. Defaults to the current directory.
  --issue <value>      Issue id or unique issue code.
```

### `meta-orchestration add-dependency --help`

```text
Usage:
  meta-orchestration add-dependency [--workspace <path>] --from-task <value> --to-task <value> --condition success|failure [--object <value>] [--reason <value>]

Options:
  --workspace <path>            MetaOrchestration workspace. Defaults to the current directory.
  --from-task <value>           Predecessor task selector.
  --to-task <value>             Successor task selector.
  --condition success|failure   Whether the successor follows predecessor success or failure.
  --object <value>              Object selector for object-scoped dependency resolution.
  --reason <value>              Reason recorded with the policy row.
```

### `meta-orchestration add-order --help`

```text
Usage:
  meta-orchestration add-order [--workspace <path>] --from-task <value> --to-task <value> [--condition success|failure] [--object <value>] [--reason <value>]

Options:
  --workspace <path>            MetaOrchestration workspace. Defaults to the current directory.
  --from-task <value>           Predecessor task selector.
  --to-task <value>             Successor task selector.
  --condition success|failure   Dependency condition. Default: success.
  --object <value>              Object selector for object-scoped dependency resolution.
  --reason <value>              Reason recorded with the policy row.
```

### `meta-orchestration allow-concurrent-append --help`

```text
Usage:
  meta-orchestration allow-concurrent-append [--workspace <path>] --object <value> [--reason <value>]

Options:
  --workspace <path>   MetaOrchestration workspace. Defaults to the current directory.
  --object <value>     Data object whose append writers can overlap.
  --reason <value>     Reason recorded with the policy row.
```

### `meta-orchestration set-lock-policy --help`

```text
Usage:
  meta-orchestration set-lock-policy [--workspace <path>] --object <value> --left-effect <value> --right-effect <value> --behavior serialize|allow [--reason <value>]

Options:
  --workspace <path>          MetaOrchestration workspace. Defaults to the current directory.
  --object <value>            Data object whose effect interaction is being resolved.
  --left-effect <value>       Left write effect, such as Append, Replace, Mutation, KeyedUpsert, or ConditionalKeyedUpsert.
  --right-effect <value>      Right write effect, such as Append, Replace, Mutation, KeyedUpsert, or ConditionalKeyedUpsert.
  --behavior serialize|allow  Lock behavior for the object/effect pair.
  --reason <value>            Reason recorded with the policy row.
```

### `meta-orchestration refresh-run-plan --help`

```text
Usage:
  meta-orchestration refresh-run-plan [--workspace <path>]

Options:
  --workspace <path>  MetaOrchestration workspace. Defaults to the current directory.
```

### `meta-orchestration inspect-run-plan --help`

```text
Usage:
  meta-orchestration inspect-run-plan [--workspace <path>]

Options:
  --workspace <path>  MetaOrchestration workspace. Defaults to the current directory.
```

### `meta-orchestration execute --help`

```text
Usage:
  meta-orchestration execute [--workspace <path>] --pipeline-workspace <path> [--data-type-conversion-workspace <path>] [--pipeline-db-connection-env <value>] [--max-degree-of-parallelism <n>] [--run-artifacts-root <path>] [--worker-event-timeout-seconds <n>] [--worker-activation-timeout-seconds <n>] [--worker-control-pipe-connect-timeout-seconds <n>]

Options:
  --workspace <path>            MetaOrchestration workspace. Defaults to the current directory.
  --pipeline-workspace <path>   MetaPipeline workspace used by child pipeline workers.
  --data-type-conversion-workspace <path>  Conversion policy workspace passed to child workers.
  --pipeline-db-connection-env <value>  Operational DB connection environment variable passed to child workers.
  --max-degree-of-parallelism <n>  Maximum concurrently granted pipeline tasks. Default: 1.
  --run-artifacts-root <path>   Operational root for run journals, worker logs, and workspace execution leases.
  --worker-event-timeout-seconds <n>  Timeout for silent worker protocol periods. 0 or omitted means no timeout.
  --worker-activation-timeout-seconds <n>  Startup/activation timeout. Omitted follows --worker-event-timeout-seconds; 0 disables activation timeout.
  --worker-control-pipe-connect-timeout-seconds <n>  Timeout while waiting for child workers to connect to the named pipe. 0 or omitted means no timeout.
```

## meta-data-warehouse

### `meta-data-warehouse --help`

```text
Usage:
  meta-data-warehouse <command> [options]

Commands:
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
  help                                 Show help.
  new-workspace                        Create a MetaDataWarehouse workspace.

Next: meta-data-warehouse help <command>
```

### `meta-data-warehouse new-workspace --help`

```text
Usage:
  meta-data-warehouse new-workspace <path>

Create a MetaDataWarehouse workspace.

Arguments:
  <path>
```

### `meta-data-warehouse add-accumulating-snapshot-fact --help`

```text
Usage:
  meta-data-warehouse add-accumulating-snapshot-fact [--workspace <path>] --id <value> [--description <value>] --fact <value>

Mark a fact as an accumulating snapshot.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  AccumulatingSnapshotFact row id.
  --description <value>         Description.
  --fact <value>                Fact id for FactId.
```

### `meta-data-warehouse add-accumulating-snapshot-milestone --help`

```text
Usage:
  meta-data-warehouse add-accumulating-snapshot-milestone [--workspace <path>] --id <value> --name <value> [--ordinal <value>] --date-role-name <value> [--description <value>] --accumulating-snapshot <value>

Add a lifecycle milestone to an accumulating snapshot.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  AccumulatingSnapshotMilestone row id.
  --name <value>                Name.
  --ordinal <value>             Ordinal.
  --date-role-name <value>      DateRoleName.
  --description <value>         Description.
  --accumulating-snapshot <value>  AccumulatingSnapshotFact id for AccumulatingSnapshotFactId.
```

### `meta-data-warehouse add-aggregate-fact --help`

```text
Usage:
  meta-data-warehouse add-aggregate-fact [--workspace <path>] --id <value> [--description <value>] --aggregated-fact <value> --source-fact <value>

Declare an aggregate fact derived from a source fact.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  AggregateFact row id.
  --description <value>         Description.
  --aggregated-fact <value>     Fact id for AggregatedFactId.
  --source-fact <value>         Fact id for SourceFactId.
```

### `meta-data-warehouse add-bridge --help`

```text
Usage:
  meta-data-warehouse add-bridge [--workspace <path>] --id <value> --name <value> [--description <value>] --warehouse <value>

Add a dimensional bridge table.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  BridgeTable row id.
  --name <value>                Name.
  --description <value>         Description.
  --warehouse <value>           Warehouse id for WarehouseId.
```

### `meta-data-warehouse add-bridge-participant --help`

```text
Usage:
  meta-data-warehouse add-bridge-participant [--workspace <path>] --id <value> --role-name <value> [--ordinal <value>] [--is-required true|false] --bridge <value> --dimension <value>

Add a dimension participant to a bridge.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  BridgeParticipant row id.
  --role-name <value>           RoleName.
  --ordinal <value>             Ordinal.
  --is-required true|false      IsRequired.
  --bridge <value>              BridgeTable id for BridgeTableId.
  --dimension <value>           Dimension id for DimensionId.
```

### `meta-data-warehouse add-bridge-weight --help`

```text
Usage:
  meta-data-warehouse add-bridge-weight [--workspace <path>] --id <value> --name <value> --data-type-id <value> [--description <value>] --bridge <value>

Add a bridge weighting measure with a Meta data type.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  BridgeWeight row id.
  --name <value>                Name.
  --data-type-id <value>        DataTypeId.
  --description <value>         Description.
  --bridge <value>              BridgeTable id for BridgeTableId.
```

### `meta-data-warehouse add-conformed-dimension --help`

```text
Usage:
  meta-data-warehouse add-conformed-dimension [--workspace <path>] --id <value> --conformance-name <value> [--description <value>] --dimension <value>

Mark a dimension as conformed.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  ConformedDimension row id.
  --conformance-name <value>    ConformanceName.
  --description <value>         Description.
  --dimension <value>           Dimension id for DimensionId.
```

### `meta-data-warehouse add-degenerate-dimension --help`

```text
Usage:
  meta-data-warehouse add-degenerate-dimension [--workspace <path>] --id <value> --name <value> --data-type-id <value> [--ordinal <value>] [--description <value>] --fact <value>

Add a degenerate dimension value to a fact.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  DegenerateDimension row id.
  --name <value>                Name.
  --data-type-id <value>        DataTypeId.
  --ordinal <value>             Ordinal.
  --description <value>         Description.
  --fact <value>                Fact id for FactId.
```

### `meta-data-warehouse add-dimension --help`

```text
Usage:
  meta-data-warehouse add-dimension [--workspace <path>] --id <value> --name <value> [--description <value>] --warehouse <value>

Add a dimension.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  Dimension row id.
  --name <value>                Name.
  --description <value>         Description.
  --warehouse <value>           Warehouse id for WarehouseId.
```

### `meta-data-warehouse add-dimension-attribute --help`

```text
Usage:
  meta-data-warehouse add-dimension-attribute [--workspace <path>] --id <value> --name <value> --data-type-id <value> [--ordinal <value>] [--is-nullable true|false] [--description <value>] --dimension <value>

Add a dimension attribute with a Meta data type.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  DimensionAttribute row id.
  --name <value>                Name.
  --data-type-id <value>        DataTypeId.
  --ordinal <value>             Ordinal.
  --is-nullable true|false      IsNullable.
  --description <value>         Description.
  --dimension <value>           Dimension id for DimensionId.
```

### `meta-data-warehouse add-dimension-business-key --help`

```text
Usage:
  meta-data-warehouse add-dimension-business-key [--workspace <path>] --id <value> --name <value> [--description <value>] --dimension <value>

Add a dimension business key.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  DimensionBusinessKey row id.
  --name <value>                Name.
  --description <value>         Description.
  --dimension <value>           Dimension id for DimensionId.
```

### `meta-data-warehouse add-dimension-business-key-part --help`

```text
Usage:
  meta-data-warehouse add-dimension-business-key-part [--workspace <path>] --id <value> [--ordinal <value>] --business-key <value> --attribute <value>

Add an ordered attribute to a dimension business key.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  DimensionBusinessKeyPart row id.
  --ordinal <value>             Ordinal.
  --business-key <value>        DimensionBusinessKey id for DimensionBusinessKeyId.
  --attribute <value>           DimensionAttribute id for DimensionAttributeId.
```

### `meta-data-warehouse add-dimension-hierarchy --help`

```text
Usage:
  meta-data-warehouse add-dimension-hierarchy [--workspace <path>] --id <value> --name <value> [--description <value>] --dimension <value>

Add a dimension hierarchy.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  DimensionHierarchy row id.
  --name <value>                Name.
  --description <value>         Description.
  --dimension <value>           Dimension id for DimensionId.
```

### `meta-data-warehouse add-dimension-hierarchy-level --help`

```text
Usage:
  meta-data-warehouse add-dimension-hierarchy-level [--workspace <path>] --id <value> --name <value> [--ordinal <value>] --hierarchy <value> --attribute <value>

Add a hierarchy level.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  DimensionHierarchyLevel row id.
  --name <value>                Name.
  --ordinal <value>             Ordinal.
  --hierarchy <value>           DimensionHierarchy id for DimensionHierarchyId.
  --attribute <value>           DimensionAttribute id for DimensionAttributeId.
```

### `meta-data-warehouse add-fact --help`

```text
Usage:
  meta-data-warehouse add-fact [--workspace <path>] --id <value> --name <value> [--description <value>] --warehouse <value>

Add a fact table concept.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  Fact row id.
  --name <value>                Name.
  --description <value>         Description.
  --warehouse <value>           Warehouse id for WarehouseId.
```

### `meta-data-warehouse add-fact-bridge --help`

```text
Usage:
  meta-data-warehouse add-fact-bridge [--workspace <path>] --id <value> --role-name <value> [--ordinal <value>] [--description <value>] --fact <value> --bridge <value>

Connect a fact to a bridge table.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  FactBridge row id.
  --role-name <value>           RoleName.
  --ordinal <value>             Ordinal.
  --description <value>         Description.
  --fact <value>                Fact id for FactId.
  --bridge <value>              BridgeTable id for BridgeTableId.
```

### `meta-data-warehouse add-fact-dimension --help`

```text
Usage:
  meta-data-warehouse add-fact-dimension [--workspace <path>] --id <value> --role-name <value> [--ordinal <value>] [--is-required true|false] [--description <value>] --fact <value> --dimension <value>

Add a dimensional role to a fact.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  FactDimension row id.
  --role-name <value>           RoleName.
  --ordinal <value>             Ordinal.
  --is-required true|false      IsRequired.
  --description <value>         Description.
  --fact <value>                Fact id for FactId.
  --dimension <value>           Dimension id for DimensionId.
```

### `meta-data-warehouse add-fact-grain --help`

```text
Usage:
  meta-data-warehouse add-fact-grain [--workspace <path>] --id <value> --name <value> --description <value> --fact <value>

Declare a fact grain.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  FactGrain row id.
  --name <value>                Name.
  --description <value>         Description.
  --fact <value>                Fact id for FactId.
```

### `meta-data-warehouse add-factless-fact --help`

```text
Usage:
  meta-data-warehouse add-factless-fact [--workspace <path>] --id <value> [--description <value>] --fact <value>

Mark a fact as factless.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  FactlessFact row id.
  --description <value>         Description.
  --fact <value>                Fact id for FactId.
```

### `meta-data-warehouse add-fact-measure --help`

```text
Usage:
  meta-data-warehouse add-fact-measure [--workspace <path>] --id <value> --name <value> --data-type-id <value> [--ordinal <value>] [--is-nullable true|false] [--description <value>] --fact <value>

Add a typed fact measure.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  FactMeasure row id.
  --name <value>                Name.
  --data-type-id <value>        DataTypeId.
  --ordinal <value>             Ordinal.
  --is-nullable true|false      IsNullable.
  --description <value>         Description.
  --fact <value>                Fact id for FactId.
```

### `meta-data-warehouse add-junk-dimension --help`

```text
Usage:
  meta-data-warehouse add-junk-dimension [--workspace <path>] --id <value> [--description <value>] --dimension <value>

Declare a junk dimension.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  JunkDimension row id.
  --description <value>         Description.
  --dimension <value>           Dimension id for DimensionId.
```

### `meta-data-warehouse add-junk-dimension-component --help`

```text
Usage:
  meta-data-warehouse add-junk-dimension-component [--workspace <path>] --id <value> [--ordinal <value>] [--description <value>] --junk-dimension <value> --attribute <value>

Add an attribute component to a junk dimension.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  JunkDimensionComponent row id.
  --ordinal <value>             Ordinal.
  --description <value>         Description.
  --junk-dimension <value>      JunkDimension id for JunkDimensionId.
  --attribute <value>           DimensionAttribute id for DimensionAttributeId.
```

### `meta-data-warehouse add-mini-dimension --help`

```text
Usage:
  meta-data-warehouse add-mini-dimension [--workspace <path>] --id <value> [--role-name <value>] [--description <value>] --source-dimension <value> --profile-dimension <value>

Declare a mini-dimension relationship.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  MiniDimension row id.
  --role-name <value>           RoleName.
  --description <value>         Description.
  --source-dimension <value>    Dimension id for SourceDimensionId.
  --profile-dimension <value>   Dimension id for ProfileDimensionId.
```

### `meta-data-warehouse add-outrigger-dimension --help`

```text
Usage:
  meta-data-warehouse add-outrigger-dimension [--workspace <path>] --id <value> --role-name <value> [--ordinal <value>] [--is-required true|false] [--description <value>] --parent-dimension <value> --child-dimension <value>

Declare an outrigger dimension relationship.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  OutriggerDimension row id.
  --role-name <value>           RoleName.
  --ordinal <value>             Ordinal.
  --is-required true|false      IsRequired.
  --description <value>         Description.
  --parent-dimension <value>    Dimension id for ParentDimensionId.
  --child-dimension <value>     Dimension id for ChildDimensionId.
```

### `meta-data-warehouse add-periodic-snapshot-fact --help`

```text
Usage:
  meta-data-warehouse add-periodic-snapshot-fact [--workspace <path>] --id <value> --period-name <value> [--description <value>] --fact <value>

Mark a fact as a periodic snapshot.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  PeriodicSnapshotFact row id.
  --period-name <value>         PeriodName.
  --description <value>         Description.
  --fact <value>                Fact id for FactId.
```

### `meta-data-warehouse add-slowly-changing-dimension --help`

```text
Usage:
  meta-data-warehouse add-slowly-changing-dimension [--workspace <path>] --id <value> [--name <value>] [--description <value>] --dimension <value>

Declare SCD behavior for a dimension.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  SlowlyChangingDimension row id.
  --name <value>                Name.
  --description <value>         Description.
  --dimension <value>           Dimension id for DimensionId.
```

### `meta-data-warehouse add-transaction-fact --help`

```text
Usage:
  meta-data-warehouse add-transaction-fact [--workspace <path>] --id <value> [--description <value>] --fact <value>

Mark a fact as transaction-grain.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TransactionFact row id.
  --description <value>         Description.
  --fact <value>                Fact id for FactId.
```

### `meta-data-warehouse add-type1-dimension-attribute --help`

```text
Usage:
  meta-data-warehouse add-type1-dimension-attribute [--workspace <path>] --id <value> [--description <value>] --slowly-changing-dimension <value> --attribute <value>

Declare a Type 1 attribute in an SCD dimension.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  Type1DimensionAttribute row id.
  --description <value>         Description.
  --slowly-changing-dimension <value>  SlowlyChangingDimension id for SlowlyChangingDimensionId.
  --attribute <value>           DimensionAttribute id for DimensionAttributeId.
```

### `meta-data-warehouse add-type2-dimension-attribute --help`

```text
Usage:
  meta-data-warehouse add-type2-dimension-attribute [--workspace <path>] --id <value> [--description <value>] --slowly-changing-dimension <value> --attribute <value>

Declare a Type 2 attribute in an SCD dimension.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  Type2DimensionAttribute row id.
  --description <value>         Description.
  --slowly-changing-dimension <value>  SlowlyChangingDimension id for SlowlyChangingDimensionId.
  --attribute <value>           DimensionAttribute id for DimensionAttributeId.
```

### `meta-data-warehouse add-warehouse --help`

```text
Usage:
  meta-data-warehouse add-warehouse [--workspace <path>] --id <value> --name <value> [--description <value>]

Add a dimensional warehouse.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  Warehouse row id.
  --name <value>                Name.
  --description <value>         Description.
```

## meta-analytics

### `meta-analytics help`

```text
Usage:
  meta-analytics <command> [options]

Commands:
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
  help                         Show help.
  new-workspace                Create a MetaAnalytics workspace.

Next: meta-analytics help <command>
```

### `meta-analytics new-workspace --help`

```text
Usage:
  meta-analytics new-workspace <path>

Create a MetaAnalytics workspace.

Arguments:
  <path>                        Directory where the MetaAnalytics workspace will be created.
```

### `meta-analytics add-aggregation-behavior --help`

```text
Usage:
  meta-analytics add-aggregation-behavior --id <value> [--workspace <path>] --function <value> [--description <value>] --measure <value>

Declare a base measure aggregate function.

Options:
  --id <value>                  AggregationBehavior row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --function <value>            Aggregate function.
  --description <value>         Description.
  --measure <value>             Measure id.
```

### `meta-analytics add-attribute --help`

```text
Usage:
  meta-analytics add-attribute --id <value> [--workspace <path>] --name <value> --data-type-id <value> [--ordinal <value>] [--kind <value>] [--source-name <value>] [--expression-language <value>] [--expression <value>] [--is-key true|false] [--is-nullable true|false] [--is-hidden true|false] [--format-string <value>] [--summarize-by <value>] [--data-category <value>] [--description <value>] --table <value>

Add a typed table attribute or calculated attribute.

Options:
  --id <value>                  Attribute row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --name <value>                Attribute name.
  --data-type-id <value>        Data type id.
  --ordinal <value>             Attribute ordinal.
  --kind <value>                Attribute kind.
  --source-name <value>         Source column name.
  --expression-language <value>  Expression language.
  --expression <value>          Expression.
  --is-key true|false           Whether the attribute is a key.
  --is-nullable true|false      Whether the attribute is nullable.
  --is-hidden true|false        Whether the attribute is hidden.
  --format-string <value>       Format string.
  --summarize-by <value>        Summarize by.
  --data-category <value>       Data category.
  --description <value>         Description.
  --table <value>               Table id.
```

### `meta-analytics add-attribute-permission --help`

```text
Usage:
  meta-analytics add-attribute-permission --id <value> [--workspace <path>] --metadata-permission <value> [--description <value>] --role <value> --attribute <value>

Add object-level security for an attribute.

Options:
  --id <value>                  AttributePermission row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --metadata-permission <value>  Metadata permission.
  --description <value>         Description.
  --role <value>                SecurityRole id.
  --attribute <value>           Attribute id.
```

### `meta-analytics add-attribute-relationship --help`

```text
Usage:
  meta-analytics add-attribute-relationship --id <value> [--workspace <path>] [--relationship-type <value>] [--description <value>] --child-attribute <value> --parent-attribute <value>

Declare an attribute relationship inside a table.

Options:
  --id <value>                  AttributeRelationship row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --relationship-type <value>   Relationship type.
  --description <value>         Description.
  --child-attribute <value>     Child attribute id.
  --parent-attribute <value>    Parent attribute id.
```

### `meta-analytics add-attribute-translation --help`

```text
Usage:
  meta-analytics add-attribute-translation --id <value> [--workspace <path>] [--caption <value>] [--description <value>] --culture <value> --attribute <value>

Translate attribute metadata.

Options:
  --id <value>                  AttributeTranslation row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --caption <value>             Caption.
  --description <value>         Description.
  --culture <value>             Culture id.
  --attribute <value>           Attribute id.
```

### `meta-analytics add-culture --help`

```text
Usage:
  meta-analytics add-culture --id <value> [--workspace <path>] --name <value> [--description <value>] --model <value>

Add a model culture.

Options:
  --id <value>                  Culture row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --name <value>                Culture name.
  --description <value>         Description.
  --model <value>               AnalyticsModel id.
```

### `meta-analytics add-data-source --help`

```text
Usage:
  meta-analytics add-data-source --id <value> [--workspace <path>] --name <value> [--provider <value>] [--connection-reference <value>] [--source-kind <value>] [--description <value>] --model <value>

Add an analytics source declaration.

Options:
  --id <value>                  DataSource row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --name <value>                Data source name.
  --provider <value>            Provider name.
  --connection-reference <value>  Connection reference.
  --source-kind <value>         Source kind.
  --description <value>         Description.
  --model <value>               AnalyticsModel id.
```

### `meta-analytics add-hierarchy --help`

```text
Usage:
  meta-analytics add-hierarchy --id <value> [--workspace <path>] --name <value> [--kind <value>] [--is-hidden true|false] [--display-folder <value>] [--description <value>] --table <value>

Add a hierarchy.

Options:
  --id <value>                  Hierarchy row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --name <value>                Hierarchy name.
  --kind <value>                Hierarchy kind.
  --is-hidden true|false        Whether the hierarchy is hidden.
  --display-folder <value>      Display folder.
  --description <value>         Description.
  --table <value>               Table id.
```

### `meta-analytics add-hierarchy-level --help`

```text
Usage:
  meta-analytics add-hierarchy-level --id <value> [--workspace <path>] --name <value> [--ordinal <value>] --hierarchy <value> --attribute <value>

Add an ordered hierarchy level.

Options:
  --id <value>                  HierarchyLevel row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --name <value>                Level name.
  --ordinal <value>             Hierarchy level ordinal.
  --hierarchy <value>           Hierarchy id.
  --attribute <value>           Attribute id.
```

### `meta-analytics add-hierarchy-translation --help`

```text
Usage:
  meta-analytics add-hierarchy-translation --id <value> [--workspace <path>] [--caption <value>] [--description <value>] --culture <value> --hierarchy <value>

Translate hierarchy metadata.

Options:
  --id <value>                  HierarchyTranslation row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --caption <value>             Caption.
  --description <value>         Description.
  --culture <value>             Culture id.
  --hierarchy <value>           Hierarchy id.
```

### `meta-analytics add-measure --help`

```text
Usage:
  meta-analytics add-measure --id <value> [--workspace <path>] --name <value> [--data-type-id <value>] [--format-string <value>] [--display-folder <value>] [--is-hidden true|false] [--description <value>] --table <value> --source-attribute <value>

Add a source-backed base measure.

Options:
  --id <value>                  Measure row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --name <value>                Measure name.
  --data-type-id <value>        Data type id.
  --format-string <value>       Format string.
  --display-folder <value>      Display folder.
  --is-hidden true|false        Whether the measure is hidden.
  --description <value>         Description.
  --table <value>               Table id.
  --source-attribute <value>    Source attribute id.
```

### `meta-analytics add-measure-translation --help`

```text
Usage:
  meta-analytics add-measure-translation --id <value> [--workspace <path>] [--caption <value>] [--description <value>] --culture <value> --measure <value>

Translate measure metadata.

Options:
  --id <value>                  MeasureTranslation row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --caption <value>             Caption.
  --description <value>         Description.
  --culture <value>             Culture id.
  --measure <value>             Measure id.
```

### `meta-analytics add-model --help`

```text
Usage:
  meta-analytics add-model --id <value> [--workspace <path>] --name <value> [--default-culture <value>] [--description <value>]

Add an analytics model.

Options:
  --id <value>                  AnalyticsModel row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --name <value>                Analytics model name.
  --default-culture <value>     Default culture.
  --description <value>         Description.
```

### `meta-analytics add-perspective --help`

```text
Usage:
  meta-analytics add-perspective --id <value> [--workspace <path>] --name <value> [--description <value>] --model <value>

Add a perspective.

Options:
  --id <value>                  Perspective row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --name <value>                Perspective name.
  --description <value>         Description.
  --model <value>               AnalyticsModel id.
```

### `meta-analytics add-perspective-attribute --help`

```text
Usage:
  meta-analytics add-perspective-attribute --id <value> [--workspace <path>] --perspective <value> --attribute <value>

Expose an attribute in a perspective.

Options:
  --id <value>                  PerspectiveAttribute row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --perspective <value>         Perspective id.
  --attribute <value>           Attribute id.
```

### `meta-analytics add-perspective-hierarchy --help`

```text
Usage:
  meta-analytics add-perspective-hierarchy --id <value> [--workspace <path>] --perspective <value> --hierarchy <value>

Expose a hierarchy in a perspective.

Options:
  --id <value>                  PerspectiveHierarchy row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --perspective <value>         Perspective id.
  --hierarchy <value>           Hierarchy id.
```

### `meta-analytics add-perspective-measure --help`

```text
Usage:
  meta-analytics add-perspective-measure --id <value> [--workspace <path>] --perspective <value> --measure <value>

Expose a measure in a perspective.

Options:
  --id <value>                  PerspectiveMeasure row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --perspective <value>         Perspective id.
  --measure <value>             Measure id.
```

### `meta-analytics add-perspective-table --help`

```text
Usage:
  meta-analytics add-perspective-table --id <value> [--workspace <path>] --perspective <value> --table <value>

Expose a table in a perspective.

Options:
  --id <value>                  PerspectiveTable row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --perspective <value>         Perspective id.
  --table <value>               Table id.
```

### `meta-analytics add-perspective-translation --help`

```text
Usage:
  meta-analytics add-perspective-translation --id <value> [--workspace <path>] [--caption <value>] [--description <value>] --culture <value> --perspective <value>

Translate perspective metadata.

Options:
  --id <value>                  PerspectiveTranslation row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --caption <value>             Caption.
  --description <value>         Description.
  --culture <value>             Culture id.
  --perspective <value>         Perspective id.
```

### `meta-analytics add-relationship --help`

```text
Usage:
  meta-analytics add-relationship --id <value> [--workspace <path>] --name <value> [--role-name <value>] --relationship-kind <value> --cardinality <value> [--cross-filter-direction <value>] [--is-active true|false] [--is-required true|false] [--description <value>] --from-table <value> --from-attribute <value> --to-table <value> --to-attribute <value> [--granularity-attribute <value>] [--intermediate-table <value>]

Add a relationship between analytics tables.

Options:
  --id <value>                  Relationship row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --name <value>                Relationship name.
  --role-name <value>           Role name.
  --relationship-kind <value>   Relationship kind.
  --cardinality <value>         Cardinality.
  --cross-filter-direction <value>  Cross-filter direction.
  --is-active true|false        Whether the relationship is active.
  --is-required true|false      Whether the relationship is required.
  --description <value>         Description.
  --from-table <value>          From table id.
  --from-attribute <value>      From attribute id.
  --to-table <value>            To table id.
  --to-attribute <value>        To attribute id.
  --granularity-attribute <value>  Granularity attribute id.
  --intermediate-table <value>  Intermediate table id.
```

### `meta-analytics add-role-filter --help`

```text
Usage:
  meta-analytics add-role-filter --id <value> [--workspace <path>] --expression-language <value> --expression <value> [--description <value>] --role <value> --table <value>

Add row-level security over a table.

Options:
  --id <value>                  RoleFilter row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --expression-language <value>  Expression language.
  --expression <value>          Expression.
  --description <value>         Description.
  --role <value>                SecurityRole id.
  --table <value>               Table id.
```

### `meta-analytics add-role-member --help`

```text
Usage:
  meta-analytics add-role-member --id <value> [--workspace <path>] --member-name <value> [--member-kind <value>] --role <value>

Add a member to a security role.

Options:
  --id <value>                  RoleMember row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --member-name <value>         Member name.
  --member-kind <value>         Member kind.
  --role <value>                SecurityRole id.
```

### `meta-analytics add-security-role --help`

```text
Usage:
  meta-analytics add-security-role --id <value> [--workspace <path>] --name <value> --permission <value> [--description <value>] --model <value>

Add a security role.

Options:
  --id <value>                  SecurityRole row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --name <value>                Security role name.
  --permission <value>          Permission.
  --description <value>         Description.
  --model <value>               AnalyticsModel id.
```

### `meta-analytics add-sort-by-attribute --help`

```text
Usage:
  meta-analytics add-sort-by-attribute --id <value> [--workspace <path>] [--description <value>] --source-attribute <value> --sort-attribute <value>

Declare one attribute as the sort key for another.

Options:
  --id <value>                  SortByAttribute row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --description <value>         Description.
  --source-attribute <value>    Source attribute id.
  --sort-attribute <value>      Sort attribute id.
```

### `meta-analytics add-table --help`

```text
Usage:
  meta-analytics add-table --id <value> [--workspace <path>] --name <value> --kind <value> [--data-category <value>] [--is-hidden true|false] [--display-folder <value>] [--description <value>] --model <value>

Add an analytics table.

Options:
  --id <value>                  Table row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --name <value>                Table name.
  --kind <value>                Table kind.
  --data-category <value>       Data category.
  --is-hidden true|false        Whether the table is hidden.
  --display-folder <value>      Display folder.
  --description <value>         Description.
  --model <value>               AnalyticsModel id.
```

### `meta-analytics add-table-permission --help`

```text
Usage:
  meta-analytics add-table-permission --id <value> [--workspace <path>] --metadata-permission <value> [--description <value>] --role <value> --table <value>

Add object-level security for a table.

Options:
  --id <value>                  TablePermission row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --metadata-permission <value>  Metadata permission.
  --description <value>         Description.
  --role <value>                SecurityRole id.
  --table <value>               Table id.
```

### `meta-analytics add-table-translation --help`

```text
Usage:
  meta-analytics add-table-translation --id <value> [--workspace <path>] [--caption <value>] [--description <value>] --culture <value> --table <value>

Translate table metadata.

Options:
  --id <value>                  TableTranslation row id.
  --workspace <path>            MetaAnalytics workspace. Defaults to the current directory.
  --caption <value>             Caption.
  --description <value>         Description.
  --culture <value>             Culture id.
  --table <value>               Table id.
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
  meta-tabular <command> [options]

Commands:
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
  deploy                                     Create modeled tabular database objects, including calculation groups, and process unless --no-process is used; processing failures fail the command.
  drop                                       Drop a tabular database from an Analysis Services tabular instance with no confirmation prompt.
  help                                       Show help.
  new-workspace                              Create a MetaTabular workspace.
  process                                    Process an existing tabular database, table, or partition.
  restore                                    Promote a processed tabular database through backup and restore for pre-prod-to-prod promotion; restore does not process.

Next: meta-tabular help <command>
```

### `meta-tabular add-tabular-calculation-group --help`

```text
Usage:
  meta-tabular add-tabular-calculation-group [--workspace <path>] --id <value> [--description <value>] --name <value> --precedence <value> --tabular-model <value>

Add a TabularCalculationGroup row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularCalculationGroup row id.
  --description <value>         Description.
  --name <value>                Name.
  --precedence <value>          Precedence.
  --tabular-model <value>       TabularModel id for TabularModelId.
```

### `meta-tabular add-tabular-calculation-item --help`

```text
Usage:
  meta-tabular add-tabular-calculation-item [--workspace <path>] --id <value> [--description <value>] --expression <value> [--format-string-expression <value>] --name <value> [--ordinal <value>] --tabular-calculation-group <value>

Add a TabularCalculationItem row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularCalculationItem row id.
  --description <value>         Description.
  --expression <value>          Expression.
  --format-string-expression <value>  FormatStringExpression.
  --name <value>                Name.
  --ordinal <value>             Ordinal.
  --tabular-calculation-group <value>  TabularCalculationGroup id for TabularCalculationGroupId.
```

### `meta-tabular add-tabular-column --help`

```text
Usage:
  meta-tabular add-tabular-column [--workspace <path>] --id <value> [--data-category <value>] --data-type-id <value> [--description <value>] [--expression <value>] [--format-string <value>] [--is-hidden true|false] [--is-key true|false] [--is-nullable true|false] --name <value> [--ordinal <value>] [--source-name <value>] [--summarize-by <value>] --tabular-table <value>

Add a TabularColumn row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularColumn row id.
  --data-category <value>       DataCategory.
  --data-type-id <value>        DataTypeId.
  --description <value>         Description.
  --expression <value>          Expression.
  --format-string <value>       FormatString.
  --is-hidden true|false        IsHidden.
  --is-key true|false           IsKey.
  --is-nullable true|false      IsNullable.
  --name <value>                Name.
  --ordinal <value>             Ordinal.
  --source-name <value>         SourceName.
  --summarize-by <value>        SummarizeBy.
  --tabular-table <value>       TabularTable id for TabularTableId.
```

### `meta-tabular add-tabular-column-permission --help`

```text
Usage:
  meta-tabular add-tabular-column-permission [--workspace <path>] --id <value> --metadata-permission <value> --tabular-column <value> --tabular-security-role <value>

Add a TabularColumnPermission row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularColumnPermission row id.
  --metadata-permission <value>  MetadataPermission.
  --tabular-column <value>      TabularColumn id for TabularColumnId.
  --tabular-security-role <value>  TabularSecurityRole id for TabularSecurityRoleId.
```

### `meta-tabular add-tabular-column-translation --help`

```text
Usage:
  meta-tabular add-tabular-column-translation [--workspace <path>] --id <value> [--caption <value>] [--description <value>] --tabular-column <value> --tabular-culture <value>

Add a TabularColumnTranslation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularColumnTranslation row id.
  --caption <value>             Caption.
  --description <value>         Description.
  --tabular-column <value>      TabularColumn id for TabularColumnId.
  --tabular-culture <value>     TabularCulture id for TabularCultureId.
```

### `meta-tabular add-tabular-culture --help`

```text
Usage:
  meta-tabular add-tabular-culture [--workspace <path>] --id <value> --name <value> --tabular-model <value>

Add a TabularCulture row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularCulture row id.
  --name <value>                Name.
  --tabular-model <value>       TabularModel id for TabularModelId.
```

### `meta-tabular add-tabular-data-source --help`

```text
Usage:
  meta-tabular add-tabular-data-source [--workspace <path>] --id <value> [--connection-reference <value>] [--description <value>] --name <value> [--provider <value>] --tabular-model <value>

Add a TabularDataSource row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularDataSource row id.
  --connection-reference <value>  ConnectionReference.
  --description <value>         Description.
  --name <value>                Name.
  --provider <value>            Provider.
  --tabular-model <value>       TabularModel id for TabularModelId.
```

### `meta-tabular add-tabular-hierarchy --help`

```text
Usage:
  meta-tabular add-tabular-hierarchy [--workspace <path>] --id <value> [--description <value>] [--display-folder <value>] [--is-hidden true|false] --name <value> --tabular-table <value>

Add a TabularHierarchy row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularHierarchy row id.
  --description <value>         Description.
  --display-folder <value>      DisplayFolder.
  --is-hidden true|false        IsHidden.
  --name <value>                Name.
  --tabular-table <value>       TabularTable id for TabularTableId.
```

### `meta-tabular add-tabular-hierarchy-level --help`

```text
Usage:
  meta-tabular add-tabular-hierarchy-level [--workspace <path>] --id <value> --name <value> [--ordinal <value>] --tabular-column <value> --tabular-hierarchy <value>

Add a TabularHierarchyLevel row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularHierarchyLevel row id.
  --name <value>                Name.
  --ordinal <value>             Ordinal.
  --tabular-column <value>      TabularColumn id for TabularColumnId.
  --tabular-hierarchy <value>   TabularHierarchy id for TabularHierarchyId.
```

### `meta-tabular add-tabular-hierarchy-translation --help`

```text
Usage:
  meta-tabular add-tabular-hierarchy-translation [--workspace <path>] --id <value> [--caption <value>] [--description <value>] --tabular-culture <value> --tabular-hierarchy <value>

Add a TabularHierarchyTranslation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularHierarchyTranslation row id.
  --caption <value>             Caption.
  --description <value>         Description.
  --tabular-culture <value>     TabularCulture id for TabularCultureId.
  --tabular-hierarchy <value>   TabularHierarchy id for TabularHierarchyId.
```

### `meta-tabular add-tabular-kpi --help`

```text
Usage:
  meta-tabular add-tabular-kpi [--workspace <path>] --id <value> [--description <value>] [--status-expression <value>] [--status-graphic <value>] [--target-expression <value>] [--trend-expression <value>] [--trend-graphic <value>] --base-measure <value> [--target-measure <value>]

Add a TabularKpi row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularKpi row id.
  --description <value>         Description.
  --status-expression <value>   StatusExpression.
  --status-graphic <value>      StatusGraphic.
  --target-expression <value>   TargetExpression.
  --trend-expression <value>    TrendExpression.
  --trend-graphic <value>       TrendGraphic.
  --base-measure <value>        TabularMeasure id for BaseMeasureId.
  --target-measure <value>      TabularMeasure id for TargetMeasureId.
```

### `meta-tabular add-tabular-kpi-translation --help`

```text
Usage:
  meta-tabular add-tabular-kpi-translation [--workspace <path>] --id <value> [--description <value>] --tabular-culture <value> --tabular-kpi <value>

Add a TabularKpiTranslation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularKpiTranslation row id.
  --description <value>         Description.
  --tabular-culture <value>     TabularCulture id for TabularCultureId.
  --tabular-kpi <value>         TabularKpi id for TabularKpiId.
```

### `meta-tabular add-tabular-measure --help`

```text
Usage:
  meta-tabular add-tabular-measure [--workspace <path>] --id <value> [--description <value>] [--display-folder <value>] [--expression <value>] [--format-string <value>] [--is-hidden true|false] --name <value> --tabular-table <value>

Add a TabularMeasure row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularMeasure row id.
  --description <value>         Description.
  --display-folder <value>      DisplayFolder.
  --expression <value>          Expression.
  --format-string <value>       FormatString.
  --is-hidden true|false        IsHidden.
  --name <value>                Name.
  --tabular-table <value>       TabularTable id for TabularTableId.
```

### `meta-tabular add-tabular-measure-translation --help`

```text
Usage:
  meta-tabular add-tabular-measure-translation [--workspace <path>] --id <value> [--caption <value>] [--description <value>] --tabular-culture <value> --tabular-measure <value>

Add a TabularMeasureTranslation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularMeasureTranslation row id.
  --caption <value>             Caption.
  --description <value>         Description.
  --tabular-culture <value>     TabularCulture id for TabularCultureId.
  --tabular-measure <value>     TabularMeasure id for TabularMeasureId.
```

### `meta-tabular add-tabular-model --help`

```text
Usage:
  meta-tabular add-tabular-model [--workspace <path>] --id <value> [--collation <value>] [--compatibility-level <value>] [--default-culture <value>] [--default-data-view <value>] [--description <value>] --name <value>

Add a TabularModel row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularModel row id.
  --collation <value>           Collation.
  --compatibility-level <value>  CompatibilityLevel.
  --default-culture <value>     DefaultCulture.
  --default-data-view <value>   DefaultDataView.
  --description <value>         Description.
  --name <value>                Name.
```

### `meta-tabular add-tabular-partition --help`

```text
Usage:
  meta-tabular add-tabular-partition [--workspace <path>] --id <value> [--description <value>] [--expression <value>] [--mode <value>] --name <value> [--ordinal <value>] [--tabular-data-source <value>] --tabular-table <value>

Add a TabularPartition row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularPartition row id.
  --description <value>         Description.
  --expression <value>          Expression.
  --mode <value>                Mode.
  --name <value>                Name.
  --ordinal <value>             Ordinal.
  --tabular-data-source <value>  TabularDataSource id for TabularDataSourceId.
  --tabular-table <value>       TabularTable id for TabularTableId.
```

### `meta-tabular add-tabular-perspective --help`

```text
Usage:
  meta-tabular add-tabular-perspective [--workspace <path>] --id <value> [--description <value>] --name <value> --tabular-model <value>

Add a TabularPerspective row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularPerspective row id.
  --description <value>         Description.
  --name <value>                Name.
  --tabular-model <value>       TabularModel id for TabularModelId.
```

### `meta-tabular add-tabular-perspective-calculation-group --help`

```text
Usage:
  meta-tabular add-tabular-perspective-calculation-group [--workspace <path>] --id <value> --tabular-calculation-group <value> --tabular-perspective <value>

Add a TabularPerspectiveCalculationGroup row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularPerspectiveCalculationGroup row id.
  --tabular-calculation-group <value>  TabularCalculationGroup id for TabularCalculationGroupId.
  --tabular-perspective <value>  TabularPerspective id for TabularPerspectiveId.
```

### `meta-tabular add-tabular-perspective-column --help`

```text
Usage:
  meta-tabular add-tabular-perspective-column [--workspace <path>] --id <value> --tabular-column <value> --tabular-perspective <value>

Add a TabularPerspectiveColumn row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularPerspectiveColumn row id.
  --tabular-column <value>      TabularColumn id for TabularColumnId.
  --tabular-perspective <value>  TabularPerspective id for TabularPerspectiveId.
```

### `meta-tabular add-tabular-perspective-hierarchy --help`

```text
Usage:
  meta-tabular add-tabular-perspective-hierarchy [--workspace <path>] --id <value> --tabular-hierarchy <value> --tabular-perspective <value>

Add a TabularPerspectiveHierarchy row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularPerspectiveHierarchy row id.
  --tabular-hierarchy <value>   TabularHierarchy id for TabularHierarchyId.
  --tabular-perspective <value>  TabularPerspective id for TabularPerspectiveId.
```

### `meta-tabular add-tabular-perspective-kpi --help`

```text
Usage:
  meta-tabular add-tabular-perspective-kpi [--workspace <path>] --id <value> --tabular-kpi <value> --tabular-perspective <value>

Add a TabularPerspectiveKpi row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularPerspectiveKpi row id.
  --tabular-kpi <value>         TabularKpi id for TabularKpiId.
  --tabular-perspective <value>  TabularPerspective id for TabularPerspectiveId.
```

### `meta-tabular add-tabular-perspective-measure --help`

```text
Usage:
  meta-tabular add-tabular-perspective-measure [--workspace <path>] --id <value> --tabular-measure <value> --tabular-perspective <value>

Add a TabularPerspectiveMeasure row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularPerspectiveMeasure row id.
  --tabular-measure <value>     TabularMeasure id for TabularMeasureId.
  --tabular-perspective <value>  TabularPerspective id for TabularPerspectiveId.
```

### `meta-tabular add-tabular-perspective-table --help`

```text
Usage:
  meta-tabular add-tabular-perspective-table [--workspace <path>] --id <value> --tabular-perspective <value> --tabular-table <value>

Add a TabularPerspectiveTable row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularPerspectiveTable row id.
  --tabular-perspective <value>  TabularPerspective id for TabularPerspectiveId.
  --tabular-table <value>       TabularTable id for TabularTableId.
```

### `meta-tabular add-tabular-perspective-translation --help`

```text
Usage:
  meta-tabular add-tabular-perspective-translation [--workspace <path>] --id <value> [--caption <value>] [--description <value>] --tabular-culture <value> --tabular-perspective <value>

Add a TabularPerspectiveTranslation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularPerspectiveTranslation row id.
  --caption <value>             Caption.
  --description <value>         Description.
  --tabular-culture <value>     TabularCulture id for TabularCultureId.
  --tabular-perspective <value>  TabularPerspective id for TabularPerspectiveId.
```

### `meta-tabular add-tabular-relationship --help`

```text
Usage:
  meta-tabular add-tabular-relationship [--workspace <path>] --id <value> --cardinality <value> [--cross-filter-direction <value>] [--is-active true|false] [--is-required true|false] --name <value> --from-column <value> --from-table <value> --to-column <value> --to-table <value>

Add a TabularRelationship row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularRelationship row id.
  --cardinality <value>         Cardinality.
  --cross-filter-direction <value>  CrossFilterDirection.
  --is-active true|false        IsActive.
  --is-required true|false      IsRequired.
  --name <value>                Name.
  --from-column <value>         TabularColumn id for FromColumnId.
  --from-table <value>          TabularTable id for FromTableId.
  --to-column <value>           TabularColumn id for ToColumnId.
  --to-table <value>            TabularTable id for ToTableId.
```

### `meta-tabular add-tabular-role-filter --help`

```text
Usage:
  meta-tabular add-tabular-role-filter [--workspace <path>] --id <value> --expression <value> --tabular-security-role <value> --tabular-table <value>

Add a TabularRoleFilter row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularRoleFilter row id.
  --expression <value>          Expression.
  --tabular-security-role <value>  TabularSecurityRole id for TabularSecurityRoleId.
  --tabular-table <value>       TabularTable id for TabularTableId.
```

### `meta-tabular add-tabular-role-member --help`

```text
Usage:
  meta-tabular add-tabular-role-member [--workspace <path>] --id <value> [--member-id <value>] --member-name <value> --tabular-security-role <value>

Add a TabularRoleMember row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularRoleMember row id.
  --member-id <value>           MemberId.
  --member-name <value>         MemberName.
  --tabular-security-role <value>  TabularSecurityRole id for TabularSecurityRoleId.
```

### `meta-tabular add-tabular-security-role --help`

```text
Usage:
  meta-tabular add-tabular-security-role [--workspace <path>] --id <value> [--description <value>] --name <value> --permission <value> --tabular-model <value>

Add a TabularSecurityRole row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularSecurityRole row id.
  --description <value>         Description.
  --name <value>                Name.
  --permission <value>          Permission.
  --tabular-model <value>       TabularModel id for TabularModelId.
```

### `meta-tabular add-tabular-sort-by-column --help`

```text
Usage:
  meta-tabular add-tabular-sort-by-column [--workspace <path>] --id <value> --sort-column <value> --source-column <value>

Add a TabularSortByColumn row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularSortByColumn row id.
  --sort-column <value>         TabularColumn id for SortColumnId.
  --source-column <value>       TabularColumn id for SourceColumnId.
```

### `meta-tabular add-tabular-table --help`

```text
Usage:
  meta-tabular add-tabular-table [--workspace <path>] --id <value> [--data-category <value>] [--description <value>] [--is-hidden true|false] --name <value> --tabular-model <value>

Add a TabularTable row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularTable row id.
  --data-category <value>       DataCategory.
  --description <value>         Description.
  --is-hidden true|false        IsHidden.
  --name <value>                Name.
  --tabular-model <value>       TabularModel id for TabularModelId.
```

### `meta-tabular add-tabular-table-permission --help`

```text
Usage:
  meta-tabular add-tabular-table-permission [--workspace <path>] --id <value> --metadata-permission <value> --tabular-security-role <value> --tabular-table <value>

Add a TabularTablePermission row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularTablePermission row id.
  --metadata-permission <value>  MetadataPermission.
  --tabular-security-role <value>  TabularSecurityRole id for TabularSecurityRoleId.
  --tabular-table <value>       TabularTable id for TabularTableId.
```

### `meta-tabular add-tabular-table-translation --help`

```text
Usage:
  meta-tabular add-tabular-table-translation [--workspace <path>] --id <value> [--caption <value>] [--description <value>] --tabular-culture <value> --tabular-table <value>

Add a TabularTableTranslation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  TabularTableTranslation row id.
  --caption <value>             Caption.
  --description <value>         Description.
  --tabular-culture <value>     TabularCulture id for TabularCultureId.
  --tabular-table <value>       TabularTable id for TabularTableId.
```

### `meta-tabular deploy --help`

```text
Usage:
  meta-tabular deploy [--workspace <path>] --server <value> [--database-name <value>] [--drop-existing] [--no-process]

Create modeled tabular database objects, including calculation groups, and process unless --no-process is used; processing failures fail the command.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --server <value>              Analysis Services tabular server.
  --database-name <value>       Target database name. Defaults to the modeled database name.
  --drop-existing               Drop an existing target database before create/deploy.
  --no-process                  Deploy metadata only and skip full processing.
```

### `meta-tabular drop --help`

```text
Usage:
  meta-tabular drop --server <value> --database-name <value>

Drop a tabular database from an Analysis Services tabular instance with no confirmation prompt.

Options:
  --server <value>              Analysis Services tabular server.
  --database-name <value>       Database name to drop.
```

### `meta-tabular new-workspace --help`

```text
Usage:
  meta-tabular new-workspace <path>

Create a MetaTabular workspace.

Arguments:
  <path>
```

### `meta-tabular process --help`

```text
Usage:
  meta-tabular process --server <value> --database-name <value> [--refresh-type <value>] [--table <value>] [--partition <value>]

Process an existing tabular database, table, or partition.

Options:
  --server <value>              Analysis Services tabular server.
  --database-name <value>       Database name to process.
  --refresh-type <value>        Refresh type. Defaults to Full. Common values: Full, DataOnly, Calculate, ClearValues, Automatic, Add, Defragment.
  --table <value>               Table name or id to process instead of the whole database.
  --partition <value>           Partition name or id to process. Requires --table.
```

### `meta-tabular restore --help`

```text
Usage:
  meta-tabular restore --source-server <value> --source-database-name <value> --target-server <value> --target-database-name <value> --backup-file <path> [--drop-existing] [--overwrite-backup-file]

Promote a processed tabular database through backup and restore for pre-prod-to-prod promotion; restore does not process.

Options:
  --source-server <value>       Source Analysis Services server containing the processed database.
  --source-database-name <value>  Source processed database name.
  --target-server <value>       Target Analysis Services server.
  --target-database-name <value>  Target database name to restore.
  --backup-file <path>          Backup file path accessible to the Analysis Services service accounts.
  --drop-existing               Drop an existing target database before restore.
  --overwrite-backup-file       Overwrite an existing backup file.
```

## meta-multi-dimensional

### `meta-multi-dimensional --help`

```text
Usage:
  meta-multi-dimensional <command> [options]

Commands:
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
  deploy                             Create modeled multidimensional database objects, including data source views, and process unless --no-process is used; processing failures fail the command.
  drop                               Drop a multidimensional database from an Analysis Services multidimensional instance with no confirmation prompt.
  help                               Show help.
  new-workspace                      Create a MetaMultiDimensional workspace.
  restore                            Promote a processed multidimensional database through backup and restore for pre-prod-to-prod promotion; restore does not process.

Next: meta-multi-dimensional help <command>
```

### `meta-multi-dimensional new-workspace --help`

```text
Usage:
  meta-multi-dimensional new-workspace <path>

Create a MetaMultiDimensional workspace.

Arguments:
  <path>
```

### `meta-multi-dimensional deploy --help`

```text
Usage:
  meta-multi-dimensional deploy [--workspace <path>] --server <value> [--database-name <value>] [--drop-existing] [--no-process]

Create modeled multidimensional database objects, including data source views, and process unless --no-process is used; processing failures fail the command.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --server <value>              Analysis Services multidimensional server.
  --database-name <value>       Target database name. Defaults to the modeled database name.
  --drop-existing               Drop an existing target database before create/deploy.
  --no-process                  Deploy metadata only and skip full processing.
```

### `meta-multi-dimensional restore --help`

```text
Usage:
  meta-multi-dimensional restore --source-server <value> --source-database-name <value> --target-server <value> --target-database-name <value> --backup-file <path> [--drop-existing] [--overwrite-backup-file]

Promote a processed multidimensional database through backup and restore for pre-prod-to-prod promotion; restore does not process.

Options:
  --source-server <value>       Source Analysis Services server containing the processed database.
  --source-database-name <value>  Source processed database name.
  --target-server <value>       Target Analysis Services server.
  --target-database-name <value>  Target database name to restore.
  --backup-file <path>          Backup file path accessible to the Analysis Services service accounts.
  --drop-existing               Drop an existing target database before restore.
  --overwrite-backup-file       Overwrite an existing backup file.
```

### `meta-multi-dimensional drop --help`

```text
Usage:
  meta-multi-dimensional drop --server <value> --database-name <value>

Drop a multidimensional database from an Analysis Services multidimensional instance with no confirmation prompt.

Options:
  --server <value>              Analysis Services multidimensional server.
  --database-name <value>       Database name to drop.
```

### `meta-multi-dimensional add-action-translation --help`

```text
Usage:
  meta-multi-dimensional add-action-translation [--workspace <path>] --id <value> [--caption <value>] [--description <value>] --cube-action <value> --culture <value>

Add a ActionTranslation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  ActionTranslation row id.
  --caption <value>             Caption.
  --description <value>         Description.
  --cube-action <value>         CubeAction id for CubeActionId.
  --culture <value>             Culture id for CultureId.
```

### `meta-multi-dimensional add-attribute-relationship --help`

```text
Usage:
  meta-multi-dimensional add-attribute-relationship [--workspace <path>] --id <value> [--description <value>] [--relationship-type <value>] --child-attribute <value> --parent-attribute <value>

Add a AttributeRelationship row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  AttributeRelationship row id.
  --description <value>         Description.
  --relationship-type <value>   RelationshipType.
  --child-attribute <value>     DimensionAttribute id for ChildAttributeId.
  --parent-attribute <value>    DimensionAttribute id for ParentAttributeId.
```

### `meta-multi-dimensional add-attribute-translation --help`

```text
Usage:
  meta-multi-dimensional add-attribute-translation [--workspace <path>] --id <value> [--caption <value>] [--description <value>] --culture <value> --dimension-attribute <value>

Add a AttributeTranslation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  AttributeTranslation row id.
  --caption <value>             Caption.
  --description <value>         Description.
  --culture <value>             Culture id for CultureId.
  --dimension-attribute <value>  DimensionAttribute id for DimensionAttributeId.
```

### `meta-multi-dimensional add-cell-permission --help`

```text
Usage:
  meta-multi-dimensional add-cell-permission [--workspace <path>] --id <value> [--description <value>] --expression <value> --cube <value> --security-role <value>

Add a CellPermission row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  CellPermission row id.
  --description <value>         Description.
  --expression <value>          Expression.
  --cube <value>                Cube id for CubeId.
  --security-role <value>       SecurityRole id for SecurityRoleId.
```

### `meta-multi-dimensional add-cube --help`

```text
Usage:
  meta-multi-dimensional add-cube [--workspace <path>] --id <value> [--default-measure-name <value>] [--description <value>] --name <value> [--processing-mode <value>] [--storage-mode <value>] --multi-dimensional-database <value>

Add a Cube row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  Cube row id.
  --default-measure-name <value>  DefaultMeasureName.
  --description <value>         Description.
  --name <value>                Name.
  --processing-mode <value>     ProcessingMode. Default: Regular.
  --storage-mode <value>        StorageMode. Default: Molap.
  --multi-dimensional-database <value>  MultiDimensionalDatabase id for MultiDimensionalDatabaseId.
```

### `meta-multi-dimensional add-cube-action --help`

```text
Usage:
  meta-multi-dimensional add-cube-action [--workspace <path>] --id <value> --action-type <value> [--caption <value>] [--description <value>] --expression <value> --name <value> [--target <value>] --target-kind <value> --cube <value>

Add a CubeAction row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  CubeAction row id.
  --action-type <value>         ActionType.
  --caption <value>             Caption.
  --description <value>         Description.
  --expression <value>          Expression.
  --name <value>                Name.
  --target <value>              Target.
  --target-kind <value>         TargetKind.
  --cube <value>                Cube id for CubeId.
```

### `meta-multi-dimensional add-cube-dimension --help`

```text
Usage:
  meta-multi-dimensional add-cube-dimension [--workspace <path>] --id <value> [--description <value>] --name <value> [--role-name <value>] --cube <value> --dimension <value>

Add a CubeDimension row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  CubeDimension row id.
  --description <value>         Description.
  --name <value>                Name.
  --role-name <value>           RoleName.
  --cube <value>                Cube id for CubeId.
  --dimension <value>           Dimension id for DimensionId.
```

### `meta-multi-dimensional add-cube-translation --help`

```text
Usage:
  meta-multi-dimensional add-cube-translation [--workspace <path>] --id <value> [--caption <value>] [--description <value>] --cube <value> --culture <value>

Add a CubeTranslation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  CubeTranslation row id.
  --caption <value>             Caption.
  --description <value>         Description.
  --cube <value>                Cube id for CubeId.
  --culture <value>             Culture id for CultureId.
```

### `meta-multi-dimensional add-culture --help`

```text
Usage:
  meta-multi-dimensional add-culture [--workspace <path>] --id <value> [--description <value>] [--language-id <value>] --name <value> --multi-dimensional-database <value>

Add a Culture row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  Culture row id.
  --description <value>         Description.
  --language-id <value>         LanguageId.
  --name <value>                Name.
  --multi-dimensional-database <value>  MultiDimensionalDatabase id for MultiDimensionalDatabaseId.
```

### `meta-multi-dimensional add-dimension --help`

```text
Usage:
  meta-multi-dimensional add-dimension [--workspace <path>] --id <value> [--description <value>] [--dimension-type <value>] --name <value> [--processing-group <value>] [--processing-mode <value>] [--source-name <value>] [--storage-mode <value>] --multi-dimensional-database <value>

Add a Dimension row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  Dimension row id.
  --description <value>         Description.
  --dimension-type <value>      DimensionType.
  --name <value>                Name.
  --processing-group <value>    ProcessingGroup. Default: ByAttribute.
  --processing-mode <value>     ProcessingMode. Default: Regular.
  --source-name <value>         SourceName.
  --storage-mode <value>        StorageMode. Default: Molap.
  --multi-dimensional-database <value>  MultiDimensionalDatabase id for MultiDimensionalDatabaseId.
```

### `meta-multi-dimensional add-dimension-attribute --help`

```text
Usage:
  meta-multi-dimensional add-dimension-attribute [--workspace <path>] --id <value> [--attribute-hierarchy-enabled <value>] [--attribute-hierarchy-visible <value>] --data-type-id <value> [--description <value>] [--is-key <value>] --name <value> [--ordinal <value>] [--source-name <value>] [--usage <value>] --dimension <value>

Add a DimensionAttribute row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  DimensionAttribute row id.
  --attribute-hierarchy-enabled <value>  AttributeHierarchyEnabled.
  --attribute-hierarchy-visible <value>  AttributeHierarchyVisible.
  --data-type-id <value>        DataTypeId.
  --description <value>         Description.
  --is-key <value>              IsKey.
  --name <value>                Name.
  --ordinal <value>             Ordinal.
  --source-name <value>         SourceName.
  --usage <value>               Usage.
  --dimension <value>           Dimension id for DimensionId.
```

### `meta-multi-dimensional add-dimension-hierarchy --help`

```text
Usage:
  meta-multi-dimensional add-dimension-hierarchy [--workspace <path>] --id <value> [--description <value>] [--hierarchy-type <value>] --name <value> --dimension <value>

Add a DimensionHierarchy row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  DimensionHierarchy row id.
  --description <value>         Description.
  --hierarchy-type <value>      HierarchyType.
  --name <value>                Name.
  --dimension <value>           Dimension id for DimensionId.
```

### `meta-multi-dimensional add-dimension-hierarchy-level --help`

```text
Usage:
  meta-multi-dimensional add-dimension-hierarchy-level [--workspace <path>] --id <value> --name <value> [--ordinal <value>] --dimension-attribute <value> --dimension-hierarchy <value>

Add a DimensionHierarchyLevel row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  DimensionHierarchyLevel row id.
  --name <value>                Name.
  --ordinal <value>             Ordinal.
  --dimension-attribute <value>  DimensionAttribute id for DimensionAttributeId.
  --dimension-hierarchy <value>  DimensionHierarchy id for DimensionHierarchyId.
```

### `meta-multi-dimensional add-dimension-permission --help`

```text
Usage:
  meta-multi-dimensional add-dimension-permission [--workspace <path>] --id <value> [--allowed-set-expression <value>] [--default-member-expression <value>] [--denied-set-expression <value>] [--description <value>] [--visual-totals <value>] --dimension-attribute <value> --dimension <value> --security-role <value>

Add a DimensionPermission row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  DimensionPermission row id.
  --allowed-set-expression <value>  AllowedSetExpression.
  --default-member-expression <value>  DefaultMemberExpression.
  --denied-set-expression <value>  DeniedSetExpression.
  --description <value>         Description.
  --visual-totals <value>       VisualTotals.
  --dimension-attribute <value>  DimensionAttribute id for DimensionAttributeId.
  --dimension <value>           Dimension id for DimensionId.
  --security-role <value>       SecurityRole id for SecurityRoleId.
```

### `meta-multi-dimensional add-dimension-translation --help`

```text
Usage:
  meta-multi-dimensional add-dimension-translation [--workspace <path>] --id <value> [--caption <value>] [--description <value>] --culture <value> --dimension <value>

Add a DimensionTranslation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  DimensionTranslation row id.
  --caption <value>             Caption.
  --description <value>         Description.
  --culture <value>             Culture id for CultureId.
  --dimension <value>           Dimension id for DimensionId.
```

### `meta-multi-dimensional add-dimension-usage --help`

```text
Usage:
  meta-multi-dimensional add-dimension-usage [--workspace <path>] --id <value> [--description <value>] [--is-required <value>] [--role-name <value>] --usage-kind <value> --cube-dimension <value> [--granularity-attribute <value>] [--intermediate-measure-group <value>] --measure-group <value>

Add a DimensionUsage row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  DimensionUsage row id.
  --description <value>         Description.
  --is-required <value>         IsRequired.
  --role-name <value>           RoleName.
  --usage-kind <value>          UsageKind.
  --cube-dimension <value>      CubeDimension id for CubeDimensionId.
  --granularity-attribute <value>  DimensionAttribute id for GranularityAttributeId.
  --intermediate-measure-group <value>  MeasureGroup id for IntermediateMeasureGroupId.
  --measure-group <value>       MeasureGroup id for MeasureGroupId.
```

### `meta-multi-dimensional add-kpi --help`

```text
Usage:
  meta-multi-dimensional add-kpi [--workspace <path>] --id <value> [--description <value>] [--goal-expression <value>] --name <value> [--status-expression <value>] [--status-graphic <value>] [--trend-expression <value>] [--trend-graphic <value>] [--value-expression <value>] [--associated-measure <value>] --cube <value>

Add a Kpi row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  Kpi row id.
  --description <value>         Description.
  --goal-expression <value>     GoalExpression.
  --name <value>                Name.
  --status-expression <value>   StatusExpression.
  --status-graphic <value>      StatusGraphic.
  --trend-expression <value>    TrendExpression.
  --trend-graphic <value>       TrendGraphic.
  --value-expression <value>    ValueExpression.
  --associated-measure <value>  Measure id for AssociatedMeasureId.
  --cube <value>                Cube id for CubeId.
```

### `meta-multi-dimensional add-kpi-translation --help`

```text
Usage:
  meta-multi-dimensional add-kpi-translation [--workspace <path>] --id <value> [--caption <value>] [--description <value>] --culture <value> --kpi <value>

Add a KpiTranslation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  KpiTranslation row id.
  --caption <value>             Caption.
  --description <value>         Description.
  --culture <value>             Culture id for CultureId.
  --kpi <value>                 Kpi id for KpiId.
```

### `meta-multi-dimensional add-mdx-calculation --help`

```text
Usage:
  meta-multi-dimensional add-mdx-calculation [--workspace <path>] --id <value> --calculation-kind <value> [--description <value>] [--display-folder <value>] --expression <value> --name <value> [--solve-order <value>] --cube <value>

Add a MdxCalculation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  MdxCalculation row id.
  --calculation-kind <value>    CalculationKind.
  --description <value>         Description.
  --display-folder <value>      DisplayFolder.
  --expression <value>          Expression.
  --name <value>                Name.
  --solve-order <value>         SolveOrder.
  --cube <value>                Cube id for CubeId.
```

### `meta-multi-dimensional add-measure --help`

```text
Usage:
  meta-multi-dimensional add-measure [--workspace <path>] --id <value> [--aggregate-function <value>] [--data-type-id <value>] [--description <value>] [--display-folder <value>] [--format-string <value>] --name <value> [--source-name <value>] --measure-group <value>

Add a Measure row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  Measure row id.
  --aggregate-function <value>  AggregateFunction.
  --data-type-id <value>        DataTypeId.
  --description <value>         Description.
  --display-folder <value>      DisplayFolder.
  --format-string <value>       FormatString.
  --name <value>                Name.
  --source-name <value>         SourceName.
  --measure-group <value>       MeasureGroup id for MeasureGroupId.
```

### `meta-multi-dimensional add-measure-group --help`

```text
Usage:
  meta-multi-dimensional add-measure-group [--workspace <path>] --id <value> [--description <value>] --name <value> [--processing-mode <value>] [--source-name <value>] [--storage-mode <value>] --cube <value>

Add a MeasureGroup row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  MeasureGroup row id.
  --description <value>         Description.
  --name <value>                Name.
  --processing-mode <value>     ProcessingMode. Default: Regular.
  --source-name <value>         SourceName.
  --storage-mode <value>        StorageMode. Default: Molap.
  --cube <value>                Cube id for CubeId.
```

### `meta-multi-dimensional add-measure-translation --help`

```text
Usage:
  meta-multi-dimensional add-measure-translation [--workspace <path>] --id <value> [--caption <value>] [--description <value>] --culture <value> --measure <value>

Add a MeasureTranslation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  MeasureTranslation row id.
  --caption <value>             Caption.
  --description <value>         Description.
  --culture <value>             Culture id for CultureId.
  --measure <value>             Measure id for MeasureId.
```

### `meta-multi-dimensional add-multi-dimensional-database --help`

```text
Usage:
  meta-multi-dimensional add-multi-dimensional-database [--workspace <path>] --id <value> [--collation <value>] [--default-language <value>] [--description <value>] --name <value>

Add a MultiDimensionalDatabase row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  MultiDimensionalDatabase row id.
  --collation <value>           Collation.
  --default-language <value>    DefaultLanguage.
  --description <value>         Description.
  --name <value>                Name.
```

### `meta-multi-dimensional add-multi-dimensional-data-source --help`

```text
Usage:
  meta-multi-dimensional add-multi-dimensional-data-source [--workspace <path>] --id <value> [--connection-reference <value>] [--description <value>] --name <value> [--provider <value>] [--source-kind <value>] --multi-dimensional-database <value>

Add a MultiDimensionalDataSource row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  MultiDimensionalDataSource row id.
  --connection-reference <value>  ConnectionReference.
  --description <value>         Description.
  --name <value>                Name.
  --provider <value>            Provider.
  --source-kind <value>         SourceKind.
  --multi-dimensional-database <value>  MultiDimensionalDatabase id for MultiDimensionalDatabaseId.
```

### `meta-multi-dimensional add-named-set --help`

```text
Usage:
  meta-multi-dimensional add-named-set [--workspace <path>] --id <value> [--description <value>] [--display-folder <value>] --expression <value> --name <value> --cube <value>

Add a NamedSet row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  NamedSet row id.
  --description <value>         Description.
  --display-folder <value>      DisplayFolder.
  --expression <value>          Expression.
  --name <value>                Name.
  --cube <value>                Cube id for CubeId.
```

### `meta-multi-dimensional add-named-set-translation --help`

```text
Usage:
  meta-multi-dimensional add-named-set-translation [--workspace <path>] --id <value> [--caption <value>] [--description <value>] --culture <value> --named-set <value>

Add a NamedSetTranslation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  NamedSetTranslation row id.
  --caption <value>             Caption.
  --description <value>         Description.
  --culture <value>             Culture id for CultureId.
  --named-set <value>           NamedSet id for NamedSetId.
```

### `meta-multi-dimensional add-partition --help`

```text
Usage:
  meta-multi-dimensional add-partition [--workspace <path>] --id <value> [--description <value>] --name <value> [--ordinal <value>] [--processing-mode <value>] [--slice-expression <value>] [--source-expression <value>] [--storage-mode <value>] --measure-group <value> [--multi-dimensional-data-source <value>]

Add a Partition row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  Partition row id.
  --description <value>         Description.
  --name <value>                Name.
  --ordinal <value>             Ordinal.
  --processing-mode <value>     ProcessingMode. Default: Regular.
  --slice-expression <value>    SliceExpression.
  --source-expression <value>   SourceExpression.
  --storage-mode <value>        StorageMode. Default: Molap.
  --measure-group <value>       MeasureGroup id for MeasureGroupId.
  --multi-dimensional-data-source <value>  MultiDimensionalDataSource id for MultiDimensionalDataSourceId.
```

### `meta-multi-dimensional add-perspective --help`

```text
Usage:
  meta-multi-dimensional add-perspective [--workspace <path>] --id <value> [--default-measure-name <value>] [--description <value>] --name <value> --cube <value>

Add a Perspective row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  Perspective row id.
  --default-measure-name <value>  DefaultMeasureName.
  --description <value>         Description.
  --name <value>                Name.
  --cube <value>                Cube id for CubeId.
```

### `meta-multi-dimensional add-perspective-action --help`

```text
Usage:
  meta-multi-dimensional add-perspective-action [--workspace <path>] --id <value> --cube-action <value> --perspective <value>

Add a PerspectiveAction row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  PerspectiveAction row id.
  --cube-action <value>         CubeAction id for CubeActionId.
  --perspective <value>         Perspective id for PerspectiveId.
```

### `meta-multi-dimensional add-perspective-calculation --help`

```text
Usage:
  meta-multi-dimensional add-perspective-calculation [--workspace <path>] --id <value> --mdx-calculation <value> --perspective <value>

Add a PerspectiveCalculation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  PerspectiveCalculation row id.
  --mdx-calculation <value>     MdxCalculation id for MdxCalculationId.
  --perspective <value>         Perspective id for PerspectiveId.
```

### `meta-multi-dimensional add-perspective-dimension --help`

```text
Usage:
  meta-multi-dimensional add-perspective-dimension [--workspace <path>] --id <value> --cube-dimension <value> --perspective <value>

Add a PerspectiveDimension row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  PerspectiveDimension row id.
  --cube-dimension <value>      CubeDimension id for CubeDimensionId.
  --perspective <value>         Perspective id for PerspectiveId.
```

### `meta-multi-dimensional add-perspective-kpi --help`

```text
Usage:
  meta-multi-dimensional add-perspective-kpi [--workspace <path>] --id <value> --kpi <value> --perspective <value>

Add a PerspectiveKpi row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  PerspectiveKpi row id.
  --kpi <value>                 Kpi id for KpiId.
  --perspective <value>         Perspective id for PerspectiveId.
```

### `meta-multi-dimensional add-perspective-measure --help`

```text
Usage:
  meta-multi-dimensional add-perspective-measure [--workspace <path>] --id <value> --measure <value> --perspective <value>

Add a PerspectiveMeasure row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  PerspectiveMeasure row id.
  --measure <value>             Measure id for MeasureId.
  --perspective <value>         Perspective id for PerspectiveId.
```

### `meta-multi-dimensional add-perspective-measure-group --help`

```text
Usage:
  meta-multi-dimensional add-perspective-measure-group [--workspace <path>] --id <value> --measure-group <value> --perspective <value>

Add a PerspectiveMeasureGroup row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  PerspectiveMeasureGroup row id.
  --measure-group <value>       MeasureGroup id for MeasureGroupId.
  --perspective <value>         Perspective id for PerspectiveId.
```

### `meta-multi-dimensional add-perspective-named-set --help`

```text
Usage:
  meta-multi-dimensional add-perspective-named-set [--workspace <path>] --id <value> --named-set <value> --perspective <value>

Add a PerspectiveNamedSet row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  PerspectiveNamedSet row id.
  --named-set <value>           NamedSet id for NamedSetId.
  --perspective <value>         Perspective id for PerspectiveId.
```

### `meta-multi-dimensional add-perspective-translation --help`

```text
Usage:
  meta-multi-dimensional add-perspective-translation [--workspace <path>] --id <value> [--caption <value>] [--description <value>] --culture <value> --perspective <value>

Add a PerspectiveTranslation row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  PerspectiveTranslation row id.
  --caption <value>             Caption.
  --description <value>         Description.
  --culture <value>             Culture id for CultureId.
  --perspective <value>         Perspective id for PerspectiveId.
```

### `meta-multi-dimensional add-role-member --help`

```text
Usage:
  meta-multi-dimensional add-role-member [--workspace <path>] --id <value> --member-name <value> [--member-sid <value>] --security-role <value>

Add a RoleMember row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  RoleMember row id.
  --member-name <value>         MemberName.
  --member-sid <value>          MemberSid.
  --security-role <value>       SecurityRole id for SecurityRoleId.
```

### `meta-multi-dimensional add-security-role --help`

```text
Usage:
  meta-multi-dimensional add-security-role [--workspace <path>] --id <value> [--description <value>] --name <value> --permission <value> --multi-dimensional-database <value>

Add a SecurityRole row.

Options:
  --workspace <path>            Workspace path. Defaults to the current directory.
  --id <value>                  SecurityRole row id.
  --description <value>         Description.
  --name <value>                Name.
  --permission <value>          Permission.
  --multi-dimensional-database <value>  MultiDimensionalDatabase id for MultiDimensionalDatabaseId.
```
