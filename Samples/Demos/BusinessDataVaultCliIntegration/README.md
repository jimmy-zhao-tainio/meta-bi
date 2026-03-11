# Business Data Vault CLI Integration

This demo creates a new Business Data Vault workspace using only CLI commands, generates SQL, and deploys it to a new SQL Server database.

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

- `MetaBiBusinessDataVaultCliDemo`

## What the demo authors

The workspace models a cross-department enterprise Business Data Vault with:

- hubs for customer, CRM customer, product, supplier, order, invoice, shipment, employee, department, and cost center
- standard links across sales, procurement, fulfillment, HR, and finance
- same-as and hierarchical links
- references for order status and currency code
- hub, link, same-as, hierarchical, and reference satellites
- PIT and bridge helper structures

## Output

SQL is written to:

- `GeneratedSql`
