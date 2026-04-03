call cleanup.cmd >nul 2>&1

meta-transform-script from sql-path --path SourceViews --new-workspace MetaTransformScriptCliIntegrationWorkspace

pushd MetaTransformScriptCliIntegrationWorkspace

meta-transform-script to sql-path --out ..\RoundTrippedViews
meta-transform-script to sql-path --out ..\RoundTrippedViews.sql
meta-transform-script to sql-code --name sales.CustomerOrderSummary

popd

meta-transform-script from sql-path --path RoundTrippedViews --new-workspace MetaTransformScriptRoundTripWorkspace

pushd MetaTransformScriptRoundTripWorkspace

meta-transform-script to sql-code --name reporting.InvoiceWindow

popd

