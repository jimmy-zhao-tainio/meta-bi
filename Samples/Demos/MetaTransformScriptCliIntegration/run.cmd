call cleanup.cmd >nul 2>&1

meta-transform-script from sql-file --path SourceViews\001_customer_order_summary\view.sql --target sales.CustomerOrderSummary --new-workspace MetaTransformScriptCliIntegrationWorkspace
meta-transform-script from sql-file --path SourceViews\002_invoice_window\view.sql --target reporting.InvoiceWindow --workspace MetaTransformScriptCliIntegrationWorkspace

pushd MetaTransformScriptCliIntegrationWorkspace

meta-transform-script to sql-path --out ..\RoundTrippedViews
meta-transform-script to sql-path --out ..\RoundTrippedViews.sql
meta-transform-script to sql-code --name sales.CustomerOrderSummary

popd

meta-transform-script from sql-file --path RoundTrippedViews\view.sql --target sales.CustomerOrderSummary --new-workspace MetaTransformScriptRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_2.sql --target reporting.InvoiceWindow --workspace MetaTransformScriptRoundTripWorkspace

pushd MetaTransformScriptRoundTripWorkspace

meta-transform-script to sql-code --name reporting.InvoiceWindow

popd
