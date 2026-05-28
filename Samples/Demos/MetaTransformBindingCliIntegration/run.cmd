call cleanup.cmd >nul 2>&1

meta-transform-script from sql-file --path SourceViews\001_customer_order_summary\view.sql --target sales.CustomerOrderSummary --new-workspace TransformWS
meta-transform-script from sql-file --path SourceViews\002_invoice_window\view.sql --target reporting.InvoiceWindow --workspace TransformWS

meta-transform-binding bind --transform-workspace TransformWS --source-schema SchemaWS --target-schema SchemaWS --execute-system MetaTransformBindingCliIntegration --new-workspace BindingWS
