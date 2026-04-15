call cleanup.cmd >nul 2>&1

powershell -NoProfile -Command "$sql = (Get-ChildItem -Path 'SourceViews\\*.sql' | Sort-Object Name | ForEach-Object { Get-Content -Raw $_.FullName }) -join [Environment]::NewLine; meta-transform-script from sql-code --code $sql --new-workspace TransformWS"

meta-transform-binding bind --transform-workspace TransformWS --name sales.CustomerOrderSummary --new-workspace SummaryBindingWS
meta-transform-binding validate --binding-workspace SummaryBindingWS --schema-workspace SchemaWS --new-workspace SummaryValidatedWS

meta-transform-binding bind --transform-workspace TransformWS --name reporting.InvoiceWindow --new-workspace InvoiceBindingWS
meta-transform-binding validate --binding-workspace InvoiceBindingWS --schema-workspace SchemaWS --new-workspace InvoiceValidatedWS
