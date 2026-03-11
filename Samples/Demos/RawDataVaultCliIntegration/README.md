# Raw Data Vault CLI Integration

This demo creates a new Raw Data Vault metadata workspace using only CLI commands, generates DV-shaped raw SQL through `meta-datavault-raw generate-sql`, and deploys that SQL to a new SQL Server database.

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

- `RawDataVaultSample`

## What the demo authors

The workspace models a cross-system Raw Data Vault baseline with:

- source systems for CRM, ERP, FIN, and HR
- source schemas, tables, fields, datatype details, and source relationships
- raw hubs for customer, product, supplier, order, invoice, shipment, employee, and department
- raw standard links for hub-to-hub foreign key relationships
- raw hub satellites and raw link satellites sourced from the same operational tables

## What gets deployed

`meta-datavault-raw generate-sql` loads the authored `MetaRawDataVault` workspace through the generated typed tooling in `MetaDataVault.Core`, iterates the resulting object tree, and emits one SQL file per typed Raw Data Vault object.

It also loads the sanctioned:

- `MetaDataVaultImplementation` workspace
- `MetaDataTypeConversion` workspace

The deployed SQL therefore represents the authored Raw Data Vault structures with physical naming from `MetaDataVaultImplementation`, for example:

- `H_Customer`
- `L_OrderCustomer`
- `HS_Customer_CustomerProfile`
- `LS_OrderCustomer_OrderCustomerStatus`

## Output

Generated SQL is written to:

- `GeneratedSql`
