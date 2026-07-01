@set "META_DQ_DEMO_MASTER_SQL=Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
@set "META_DQ_DEMO_SOURCE_SQL=Server=.;Database=MetaDataQualityCliIntegration;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"
@set "META_DQ_DEMO_OPERATIONAL_SQL=Server=.;Database=MetaDQ;Integrated Security=true;TrustServerCertificate=true;Encrypt=false"

call cleanup.cmd >nul 2>&1

meta-sql execute --connection-env META_DQ_DEMO_MASTER_SQL --file setup.sql
@if errorlevel 1 exit /b %errorlevel%

meta-sql execute --connection-env META_DQ_DEMO_SOURCE_SQL --file SourceViews\001_customer_order_coverage\view.sql
@if errorlevel 1 exit /b %errorlevel%

meta-sql execute --connection-env META_DQ_DEMO_SOURCE_SQL --file SourceViews\002_customer_invoice_composite\view.sql
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path SourceViews\001_customer_order_coverage\view.sql --target dq.CustomerOrderCoverage --new-workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-transform-script from sql-file --path SourceViews\002_customer_invoice_composite\view.sql --target dq.CustomerInvoiceComposite --workspace TransformWS
@if errorlevel 1 exit /b %errorlevel%

meta-data-quality from-transform-workspace --transform-workspace TransformWS --new-workspace DataQualityWS
@if errorlevel 1 exit /b %errorlevel%

meta-data-quality inspect --workspace DataQualityWS
@if errorlevel 1 exit /b %errorlevel%

meta-data-quality promote --workspace DataQualityWS --all
@if errorlevel 1 exit /b %errorlevel%

meta-convert data-quality-to-sql --workspace DataQualityWS --out DataQualityViews.sql
@if errorlevel 1 exit /b %errorlevel%

meta-sql execute --connection-env META_DQ_DEMO_SOURCE_SQL --file DataQualityViews.sql
@if errorlevel 1 exit /b %errorlevel%
