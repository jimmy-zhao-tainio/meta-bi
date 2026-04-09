call cleanup.cmd >nul 2>&1

meta-transform-script from sql-path --path SourceViews --new-workspace MetaTransformBindingCliIntegrationTransformWorkspace

meta-transform-binding --transform-workspace MetaTransformBindingCliIntegrationTransformWorkspace --schema-workspace SchemaWorkspace --name sales.CustomerOrderSummary --language-profile MetaTransformSqlServer_v1 --new-workspace MetaTransformBindingCliIntegrationBindingWorkspace

meta-transform-binding --transform-workspace MetaTransformBindingCliIntegrationTransformWorkspace --schema-workspace SchemaWorkspace --name reporting.InvoiceWindow --language-profile MetaTransformSqlServer_v1 --new-workspace MetaTransformBindingInvoiceWorkspace
