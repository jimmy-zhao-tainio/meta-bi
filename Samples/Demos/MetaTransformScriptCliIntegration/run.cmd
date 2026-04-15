call cleanup.cmd >nul 2>&1

powershell -NoProfile -Command "$sql = (Get-ChildItem -Path 'SourceViews\\*.sql' | Sort-Object Name | ForEach-Object { Get-Content -Raw $_.FullName }) -join [Environment]::NewLine; meta-transform-script from sql-code --code $sql --new-workspace MetaTransformScriptCliIntegrationWorkspace"

pushd MetaTransformScriptCliIntegrationWorkspace

meta-transform-script to sql-path --out ..\RoundTrippedViews
meta-transform-script to sql-path --out ..\RoundTrippedViews.sql
meta-transform-script to sql-code --name sales.CustomerOrderSummary

popd

powershell -NoProfile -Command "$sql = (Get-ChildItem -Path 'RoundTrippedViews\\*.sql' | Sort-Object Name | ForEach-Object { Get-Content -Raw $_.FullName }) -join [Environment]::NewLine; meta-transform-script from sql-code --code $sql --new-workspace MetaTransformScriptRoundTripWorkspace"

pushd MetaTransformScriptRoundTripWorkspace

meta-transform-script to sql-code --name reporting.InvoiceWindow

popd
