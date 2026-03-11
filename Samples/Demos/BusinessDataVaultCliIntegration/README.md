# Business Data Vault CLI Integration

This demo creates a new Business Data Vault metadata workspace using only CLI commands, generates DV-shaped SQL through `meta-datavault-business generate-sql`, and deploys that SQL to a new SQL Server database.

## Commands

Run from this directory:

```cmd
run.cmd
```

Remove the generated workspace, SQL scripts, and demo database:

```cmd
cleanup.cmd
```

## Target database

- `BusinessDataVaultSample`

## What the demo authors

The workspace models a cross-department enterprise Business Data Vault with:

- hubs for customer, CRM customer, product, supplier, order, invoice, shipment, employee, department, and cost center
- standard links across sales, procurement, fulfillment, HR, finance, billing, and cost allocation
- same-as and hierarchical links
- references for order status and currency code
- multiple hub satellites per business concept, richer link satellites, same-as, hierarchical, and reference satellites
- PIT and bridge helper structures

## What gets deployed

`meta-datavault-business generate-sql` loads the authored `MetaBusinessDataVault` workspace through the generated typed tooling in `MetaDataVault.Core`, iterates the resulting object tree, and emits one SQL file per typed Business Data Vault object.

It also loads the sanctioned:

- `MetaDataVaultImplementation` workspace
- `MetaDataTypeConversion` workspace

The deployed SQL therefore represents the authored Business Data Vault structures with physical naming from `MetaDataVaultImplementation`, for example:

- `BH_Customer`
- `BL_CustomerOrder`
- `BHS_Customer_CustomerProfile`
- `REF_OrderStatus`
- `PIT_CustomerSnapshot`
- `BR_CustomerFulfillmentTraversal`

## Exact SQL mapping

The SQL generator loads:

- `MetaBusinessDataVaultModel.LoadFromXmlWorkspace(...)`
- `MetaDataVaultImplementationModel.LoadFromXmlWorkspace(...)`
- `MetaDataTypeConversionModel.LoadFromXmlWorkspace(...)`

Then `BusinessDataVaultSqlGenerator` iterates these typed lists in stable name order and emits one `.sql` file per row:

- `BusinessHub` -> one table per hub
  - physical table name from `BusinessHubImplementation.TableNamePattern`
  - hash key column from `BusinessHubImplementation`
  - one column per `BusinessHubKeyPart`
  - optional `LoadTimestamp` and `RecordSource`
  - required `AuditId`
  - PK on hub hash key
  - UQ on ordered hub key parts

- `BusinessLink` -> one table per standard link
  - physical table name from `BusinessLinkImplementation.TableNamePattern`
  - hash key column from `BusinessLinkImplementation`
  - one foreign-key hash column per `BusinessLinkHub`, named from `EndHashKeyColumnPattern`
  - optional `LoadTimestamp` and `RecordSource`
  - required `AuditId`
  - PK on link hash key
  - UQ on the ordered participating hub hash key columns
  - FK from each participating hub hash key column to the corresponding hub table

- `BusinessSameAsLink` -> one table per same-as link
  - physical table name from `BusinessSameAsLinkImplementation.TableNamePattern`
  - hash key column
  - primary hub hash key column
  - equivalent hub hash key column
  - optional `LoadTimestamp` and `RecordSource`
  - required `AuditId`
  - PK on link hash key
  - UQ on primary + equivalent hash key columns
  - FKs to the referenced hub table

- `BusinessHierarchicalLink` -> one table per hierarchical link
  - physical table name from `BusinessHierarchicalLinkImplementation.TableNamePattern`
  - hash key column
  - parent hub hash key column
  - child hub hash key column
  - optional `LoadTimestamp` and `RecordSource`
  - required `AuditId`
  - PK on link hash key
  - UQ on parent + child hash key columns
  - FKs to the referenced hub table

- `BusinessReference` -> one table per reference
  - physical table name from `BusinessReferenceImplementation.TableNamePattern`
  - hash key column
  - one column per `BusinessReferenceKeyPart`
  - optional `LoadTimestamp` and `RecordSource`
  - required `AuditId`
  - PK on reference hash key
  - UQ on ordered reference key parts

- `BusinessHubSatellite` -> one table per hub satellite
  - physical table name from `BusinessHubSatelliteImplementation.TableNamePattern`
  - parent hub hash key column
  - zero or more `BusinessHubSatelliteKeyPart` columns for `multi-active`
  - one column per `BusinessHubSatelliteAttribute`
  - optional `HashDiff`, `LoadTimestamp`, and `RecordSource`
  - required `AuditId`
  - PK on parent hash key plus any satellite key parts plus `LoadTimestamp` when present
  - FK to the parent hub table

- `BusinessLinkSatellite` -> one table per standard link satellite
  - physical table name from `BusinessLinkSatelliteImplementation.TableNamePattern`
  - parent link hash key column
  - zero or more `BusinessLinkSatelliteKeyPart` columns for `multi-active`
  - one column per `BusinessLinkSatelliteAttribute`
  - optional `HashDiff`, `LoadTimestamp`, and `RecordSource`
  - required `AuditId`
  - PK on parent hash key plus any satellite key parts plus `LoadTimestamp` when present
  - FK to the parent link table

- `BusinessSameAsLinkSatellite` -> one table per same-as link satellite
  - physical table name from `BusinessSameAsLinkSatelliteImplementation.TableNamePattern`
  - same shape as link satellites, but parent FK goes to the same-as link table

- `BusinessHierarchicalLinkSatellite` -> one table per hierarchical link satellite
  - physical table name from `BusinessHierarchicalLinkSatelliteImplementation.TableNamePattern`
  - same shape as link satellites, but parent FK goes to the hierarchical link table

- `BusinessReferenceSatellite` -> one table per reference satellite
  - physical table name from `BusinessReferenceSatelliteImplementation.TableNamePattern`
  - parent reference hash key column
  - zero or more reference-satellite key parts for `multi-active`
  - one column per `BusinessReferenceSatelliteAttribute`
  - optional `HashDiff`, `LoadTimestamp`, and `RecordSource`
  - required `AuditId`
  - PK on parent reference hash key plus any satellite key parts plus `LoadTimestamp` when present
  - FK to the parent reference table

- `BusinessPointInTime` -> one table per PIT
  - physical table name from `BusinessPointInTimeImplementation.TableNamePattern`
  - parent hub hash key column
  - snapshot timestamp column
  - one satellite reference column per `BusinessPointInTimeHubSatellite`
  - one satellite reference column per `BusinessPointInTimeLinkSatellite`
  - one plain extra column per `BusinessPointInTimeStamp`
  - required `AuditId`
  - PK on parent hub hash key + snapshot timestamp
  - FK to the parent hub table

- `BusinessBridge` -> one table per bridge
  - physical table name from `BusinessBridgeImplementation.TableNamePattern`
  - root hash key column from the anchor hub
  - related hash key column from the resolved terminal hub in the ordered bridge path
  - one projected column per:
    - `BusinessBridgeHubKeyPartProjection`
    - `BusinessBridgeHubSatelliteAttributeProjection`
    - `BusinessBridgeLinkSatelliteAttributeProjection`
  - optional `Depth`, `Path`, `EffectiveFrom`, `EffectiveTo`
  - required `AuditId`
  - PK on root + related, and also `EffectiveFrom` if configured
  - FK to the anchor hub and to the resolved related hub

## Type resolution

For any business-derived column, SQL type resolution is:

1. take the row `DataTypeId`
2. collect local `...DataTypeDetail` rows such as `Length`, `Precision`, `Scale`
3. resolve through `MetaDataTypeConversion` into a SQL Server type id when needed
4. render the final SQL type, for example:
   - `meta:type:String` + `Length=50` -> `nvarchar(50)`
   - `meta:type:Decimal` + `Precision=18`, `Scale=2` -> `decimal(18, 2)`

## Output

Generated SQL is written to:

- `GeneratedSql`

The file names follow the same implementation naming patterns as the emitted table names.


