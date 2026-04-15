call cleanup.cmd >nul 2>&1

powershell -NoProfile -Command "$sql = (Get-ChildItem -Path 'SourceViews\\*.sql' | Sort-Object Name | ForEach-Object { Get-Content -Raw $_.FullName }) -join [Environment]::NewLine; meta-transform-script from sql-code --code $sql --new-workspace MetaTransformScriptReferenceCorpusWorkspace"

pushd MetaTransformScriptReferenceCorpusWorkspace

meta-transform-script to sql-path --out ..\RoundTrippedViews
meta-transform-script to sql-path --out ..\RoundTrippedViews.sql
meta-transform-script to sql-code --name dbo.v_window_functions

popd

powershell -NoProfile -Command "$sql = (Get-ChildItem -Path 'RoundTrippedViews\\*.sql' | Sort-Object Name | ForEach-Object { Get-Content -Raw $_.FullName }) -join [Environment]::NewLine; meta-transform-script from sql-code --code $sql --new-workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace"

meta instance diff MetaTransformScriptReferenceCorpusWorkspace MetaTransformScriptReferenceCorpusRoundTripWorkspace

pushd MetaTransformScriptReferenceCorpusRoundTripWorkspace

meta-transform-script to sql-code --name dbo.v_xml_namespaces_and_methods

popd
