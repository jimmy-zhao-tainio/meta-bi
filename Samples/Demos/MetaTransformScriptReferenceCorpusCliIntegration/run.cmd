call cleanup.cmd >nul 2>&1

meta-transform-script from sql-path --path SourceViews --new-workspace MetaTransformScriptReferenceCorpusWorkspace

pushd MetaTransformScriptReferenceCorpusWorkspace

meta-transform-script to sql-path --out ..\RoundTrippedViews
meta-transform-script to sql-path --out ..\RoundTrippedViews.sql
meta-transform-script to sql-code --name dbo.v_window_functions

popd

meta-transform-script from sql-path --path RoundTrippedViews --new-workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace

meta instance diff MetaTransformScriptReferenceCorpusWorkspace MetaTransformScriptReferenceCorpusRoundTripWorkspace

pushd MetaTransformScriptReferenceCorpusRoundTripWorkspace

meta-transform-script to sql-code --name dbo.v_xml_namespaces_and_methods

popd
