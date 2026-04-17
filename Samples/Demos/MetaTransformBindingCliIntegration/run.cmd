call cleanup.cmd >nul 2>&1

meta-transform-script from sql-file --path SourceViews\001_customer_order_summary\view.sql --target sales.CustomerOrderSummary --new-workspace TransformWS
meta-transform-script from sql-file --path SourceViews\002_invoice_window\view.sql --target reporting.InvoiceWindow --workspace TransformWS

meta-transform-binding bind --transform-workspace TransformWS --name sales.CustomerOrderSummary --new-workspace SummaryBindingWS
meta-transform-binding validate --binding-workspace SummaryBindingWS --schema-workspace SchemaWS --new-workspace SummaryValidatedWS

meta-transform-binding bind --transform-workspace TransformWS --name reporting.InvoiceWindow --new-workspace InvoiceBindingWS
meta-transform-binding validate --binding-workspace InvoiceBindingWS --schema-workspace SchemaWS --new-workspace InvoiceValidatedWS
