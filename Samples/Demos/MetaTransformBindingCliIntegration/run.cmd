call cleanup.cmd >nul 2>&1

meta-transform-script from sql-path --path SourceViews --new-workspace TransformWS

meta-transform-binding --transform-workspace TransformWS --name sales.CustomerOrderSummary --target sales.CustomerOrderSummary --new-workspace SummaryBindingWS

meta-transform-binding --transform-workspace TransformWS --name reporting.InvoiceWindow --target reporting.InvoiceWindow --new-workspace InvoiceBindingWS
