# Adventure Works sales analytics request

Hi,

We need a first useful sales analytics model for Adventure Works. The current reporting is too spreadsheet-heavy, and different teams keep answering the same questions in slightly different ways.

This does not need to be the final enterprise model. It should be good enough that sales, finance, and operations can review the numbers together and decide what to improve next.

## Questions we need answered

Sales leadership wants to understand:

- How are sales trending by month, quarter, and year?
- How much comes from online orders compared with store or reseller-related sales?
- Which product categories and subcategories are growing or shrinking?
- Which countries, regions, and sales territories are carrying the business?
- Which customers, stores, and salespeople are responsible for high-value sales?
- How do actual sales compare with salesperson quotas over time?
- Are there obvious data-quality problems we should know about before trusting the results?

Finance needs stable measures:

- sales amount
- tax amount
- freight amount
- discount amount
- order quantity
- order count
- average order value
- gross margin, if the available data supports it clearly

Operations cares about dates:

- order date
- due date
- ship date
- month and fiscal period
- shipments where the dates look late or inconsistent

## Business-friendly dimensions

Please make the model easy to browse by:

- date, including calendar and fiscal views if possible
- product, category, subcategory, model, and product
- geography, country, state/province, city, and sales territory
- customer
- store or reseller
- salesperson or employee where sales and quota data support it
- currency if it is needed to keep sales amounts clear

## First analytical areas

The first useful version should support:

- online sales
- store/reseller sales
- salesperson quota comparison
- shared dimensions for date, product, geography, customer, store/reseller, salesperson, and sales territory where the data allows it

## Data concerns

Please flag issues that would make the report embarrassing or misleading:

- sales rows without a valid product
- sales rows without a valid customer, store, or salesperson where one is expected
- quota rows without a valid salesperson or period
- negative order quantity
- negative sales amount unless there is a clear business reason
- ship date earlier than order date
- duplicate-looking sales lines

## What good looks like

For the first review, I want to open the analytics model and see sales amount over time, sales by product category, sales by geography, and quota comparison for salespeople.

If some requested area is not supported by the source data, please call that out plainly rather than forcing it.
