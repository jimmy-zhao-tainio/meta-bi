call cleanup.cmd >nul 2>&1

meta-transform-script from sql-code --code "SELECT c.CustomerId, c.RegionId, o.OrderId FROM sales.Customer c INNER JOIN sales.[Order] o ON c.CustomerId = o.CustomerId AND c.RegionId = o.RegionId" --name dq.CustomerOrderComposite01 --target dq.CustomerOrderComposite01 --new-workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, c.RegionId, o.OrderId FROM sales.Customer c INNER JOIN sales.[Order] o ON c.CustomerId = o.CustomerId AND c.RegionId = o.RegionId" --name dq.CustomerOrderComposite02 --target dq.CustomerOrderComposite02 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, c.RegionId, o.OrderId FROM sales.Customer c INNER JOIN sales.[Order] o ON c.CustomerId = o.CustomerId AND c.RegionId = o.RegionId" --name dq.CustomerOrderComposite03 --target dq.CustomerOrderComposite03 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, c.RegionId, o.OrderId FROM sales.Customer c INNER JOIN sales.[Order] o ON c.CustomerId = o.CustomerId AND c.RegionId = o.RegionId" --name dq.CustomerOrderComposite04 --target dq.CustomerOrderComposite04 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, c.RegionId, o.OrderId FROM sales.Customer c INNER JOIN sales.[Order] o ON c.CustomerId = o.CustomerId AND c.RegionId = o.RegionId" --name dq.CustomerOrderComposite05 --target dq.CustomerOrderComposite05 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, c.RegionId, o.OrderId FROM sales.Customer c INNER JOIN sales.[Order] o ON c.CustomerId = o.CustomerId AND c.RegionId = o.RegionId" --name dq.CustomerOrderComposite06 --target dq.CustomerOrderComposite06 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, c.RegionId, o.OrderId FROM sales.Customer c INNER JOIN sales.[Order] o ON c.CustomerId = o.CustomerId AND c.RegionId = o.RegionId" --name dq.CustomerOrderComposite07 --target dq.CustomerOrderComposite07 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, c.RegionId, o.OrderId FROM sales.Customer c INNER JOIN sales.[Order] o ON c.CustomerId = o.CustomerId AND c.RegionId = o.RegionId" --name dq.CustomerOrderComposite08 --target dq.CustomerOrderComposite08 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-code --code "SELECT c.CustomerId, c.RegionId, o.OrderId FROM sales.Customer c INNER JOIN sales.[Order] o ON c.CustomerId = o.CustomerId" --name dq.CustomerOrderSubset01 --target dq.CustomerOrderSubset01 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, c.RegionId, o.OrderId FROM sales.Customer c INNER JOIN sales.[Order] o ON c.CustomerId = o.CustomerId" --name dq.CustomerOrderSubset02 --target dq.CustomerOrderSubset02 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-code --code "SELECT c.CustomerId, i.InvoiceId FROM sales.Customer c INNER JOIN sales.Invoice i ON c.CustomerId = i.CustomerId" --name dq.CustomerInvoiceDominant01 --target dq.CustomerInvoiceDominant01 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, i.InvoiceId FROM sales.Customer c INNER JOIN sales.Invoice i ON c.CustomerId = i.CustomerId" --name dq.CustomerInvoiceDominant02 --target dq.CustomerInvoiceDominant02 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, i.InvoiceId FROM sales.Customer c INNER JOIN sales.Invoice i ON c.CustomerId = i.CustomerId" --name dq.CustomerInvoiceDominant03 --target dq.CustomerInvoiceDominant03 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, i.InvoiceId FROM sales.Customer c INNER JOIN sales.Invoice i ON c.CustomerId = i.CustomerId" --name dq.CustomerInvoiceDominant04 --target dq.CustomerInvoiceDominant04 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, i.InvoiceId FROM sales.Customer c INNER JOIN sales.Invoice i ON c.CustomerId = i.CustomerId" --name dq.CustomerInvoiceDominant05 --target dq.CustomerInvoiceDominant05 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, i.InvoiceId FROM sales.Customer c INNER JOIN sales.Invoice i ON c.CustomerId = i.CustomerId" --name dq.CustomerInvoiceDominant06 --target dq.CustomerInvoiceDominant06 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, i.InvoiceId FROM sales.Customer c INNER JOIN sales.Invoice i ON c.CustomerId = i.CustomerId" --name dq.CustomerInvoiceDominant07 --target dq.CustomerInvoiceDominant07 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%
meta-transform-script from sql-code --code "SELECT c.CustomerId, i.InvoiceId FROM sales.Customer c INNER JOIN sales.Invoice i ON c.CustomerId = i.CustomerId" --name dq.CustomerInvoiceDominant08 --target dq.CustomerInvoiceDominant08 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-code --code "SELECT c.CustomerId, c.RegionId, i.InvoiceId FROM sales.Customer c INNER JOIN sales.Invoice i ON c.CustomerId = i.CustomerId AND c.RegionId = i.RegionId" --name dq.CustomerInvoiceOutlierExtra01 --target dq.CustomerInvoiceOutlierExtra01 --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-data-quality from-transform-workspace --transform-workspace TransformWS --new-workspace DataQualityWS
@if errorlevel 1 exit /b %errorlevel%
meta-data-quality inspect --workspace DataQualityWS
@if errorlevel 1 exit /b %errorlevel%

meta-convert data-quality-to-sql --workspace DataQualityWS --out BeforePromote.sql > before-promote.output 2>&1
@if not errorlevel 1 (
  echo ERROR: expected conversion to fail before promotion.
  type before-promote.output
  exit /b 1
)
type before-promote.output

meta-data-quality promote --workspace DataQualityWS --all
@if errorlevel 1 exit /b %errorlevel%

meta-convert data-quality-to-sql --workspace DataQualityWS --out AllPromoted.sql
@if errorlevel 1 exit /b %errorlevel%

meta-data-quality from-transform-workspace --transform-workspace TransformWS --new-workspace DataQualityWS_Supported
@if errorlevel 1 exit /b %errorlevel%

powershell -NoProfile -ExecutionPolicy Bypass -File collect-implied-candidate-ids.ps1 -WorkspacePath DataQualityWS_Supported -OutFile implied-candidate-ids.txt
@if errorlevel 1 exit /b %errorlevel%

for /f %%I in (implied-candidate-ids.txt) do (
  meta-data-quality promote --workspace DataQualityWS_Supported --candidate-id %%I
  @if errorlevel 1 exit /b %errorlevel%
)

meta-convert data-quality-to-sql --workspace DataQualityWS_Supported --out DataQualityViews.sql
@if errorlevel 1 exit /b %errorlevel%

meta-data-quality inspect --workspace DataQualityWS_Supported
@if errorlevel 1 exit /b %errorlevel%
