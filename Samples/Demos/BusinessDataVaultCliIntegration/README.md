# Business Data Vault CLI Integration

This demo creates a new Business Data Vault metadata workspace using only CLI commands, generates SQL through `meta-datavault-business generate-sql`, and deploys that SQL to a new SQL Server database.

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

- `MetaBiBusinessDataVaultCliModelDemo`

## What the demo authors

The workspace models a cross-department enterprise Business Data Vault with:

- hubs for customer, CRM customer, product, supplier, order, invoice, shipment, employee, department, and cost center
- standard links across sales, procurement, fulfillment, HR, and finance
- same-as and hierarchical links
- references for order status and currency code
- hub, link, same-as, hierarchical, and reference satellites
- PIT and bridge helper structures

## What gets deployed

`meta-datavault-business generate-sql` loads the authored `MetaBusinessDataVault` workspace through the generated typed tooling in `MetaDataVault.Core`, iterates the resulting object tree, and emits SQL from those typed objects.

It also loads the sanctioned:

- `MetaDataVaultImplementation` workspace
- `MetaDataTypeConversion` workspace

The deployed SQL therefore represents the authored Business Data Vault structures, for example:

- `Customer`
- `CustomerOrder`
- `CustomerProfile`
- `OrderStatus`
- `CustomerSnapshot`
- `CustomerFulfillmentTraversal`

## Output

Generated SQL is written to:

- `GeneratedSql`
