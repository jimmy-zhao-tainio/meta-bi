# Business Data Vault SQL Demo

This demo shows a materialized `MetaBusinessDataVault` workspace with instance data, a sanctioned `MetaDataVaultImplementation` workspace, a sanctioned `MetaDataTypeConversion` workspace, and the generated SQL scripts emitted by `meta-datavault generate-sql`.

## Contents

- `BusinessDataVault`
  - materialized `MetaBusinessDataVault` workspace with model and instance data
- `Implementation`
  - sanctioned `MetaDataVaultImplementation` workspace
- `DataTypeConversion`
  - sanctioned `MetaDataTypeConversion` workspace
- `GeneratedSql`
  - generated SQL scripts checked in for inspection

## Run

Open `cmd.exe`, then run:

```cmd
cd /d C:\Users\jimmy\Desktop\meta-bi\Samples\Demos\BusinessDataVaultSql
run.cmd
```

## Command

```cmd
meta-datavault generate-sql --workspace BusinessDataVault --implementation-workspace Implementation --data-type-conversion-workspace DataTypeConversion --out GeneratedSql
```

## Expected output

```text
OK: business datavault sql generated
Workspace: C:\Users\jimmy\Desktop\meta-bi\Samples\Demos\BusinessDataVaultSql\BusinessDataVault
Implementation Workspace: C:\Users\jimmy\Desktop\meta-bi\Samples\Demos\BusinessDataVaultSql\Implementation
DataTypeConversion Workspace: C:\Users\jimmy\Desktop\meta-bi\Samples\Demos\BusinessDataVaultSql\DataTypeConversion
Path: C:\Users\jimmy\Desktop\meta-bi\Samples\Demos\BusinessDataVaultSql\GeneratedSql
Files: 11
Tables: 11
BusinessHubs: 3
BusinessLinks: 2
BusinessHubSatellites: 1
BusinessLinkSatellites: 1
BusinessReferences: 1
BusinessReferenceSatellites: 1
BusinessPointInTimes: 1
BusinessBridges: 1
```

## Generated files

```text
BH_Customer.sql
BH_Invoice.sql
BH_Order.sql
BHS_Customer_Profile.sql
BL_CustomerInvoice.sql
BL_CustomerOrder.sql
BLS_CustomerOrder_Status.sql
BR_CustomerOrderTraversal.sql
PIT_CustomerSnapshot.sql
REF_Status.sql
RSAT_Status_Current.sql
```

## Example SQL

`GeneratedSql/BH_Customer.sql`

```sql
CREATE TABLE [dbo].[BH_Customer] (
    [HashKey] binary(16) NOT NULL,
    [Identifier] nvarchar(50) NOT NULL,
    [LoadTimestamp] datetime2(7) NOT NULL,
    [RecordSource] nvarchar(256) NOT NULL,
    [AuditId] int NOT NULL,
    CONSTRAINT [PK_BH_Customer] PRIMARY KEY ([HashKey]),
    CONSTRAINT [UQ_BH_Customer] UNIQUE ([Identifier])
);
```

`GeneratedSql/BR_CustomerOrderTraversal.sql`

```sql
CREATE TABLE [dbo].[BR_CustomerOrderTraversal] (
    [RootHashKey] binary(16) NOT NULL,
    [RelatedHashKey] binary(16) NOT NULL,
    [Depth] int NOT NULL,
    [Path] nvarchar(4000) NOT NULL,
    [EffectiveFrom] datetime2(7) NOT NULL,
    [EffectiveTo] datetime2(7) NOT NULL,
    CONSTRAINT [PK_BR_CustomerOrderTraversal] PRIMARY KEY ([RootHashKey], [RelatedHashKey], [EffectiveFrom]),
    CONSTRAINT [FK_BR_CustomerOrderTraversal_BH_Customer_RootHashKey] FOREIGN KEY ([RootHashKey]) REFERENCES [BH_Customer] ([HashKey]),
    CONSTRAINT [FK_BR_CustomerOrderTraversal_BH_Order_RelatedHashKey] FOREIGN KEY ([RelatedHashKey]) REFERENCES [BH_Order] ([HashKey])
);
```

