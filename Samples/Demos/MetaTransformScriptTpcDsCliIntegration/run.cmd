call cleanup.cmd >nul 2>&1

meta-transform-script from sql-file --path SourceViews\001_q01\view.sql --target tpcds.v_q01 --new-workspace TransformWS
meta-transform-script from sql-file --path SourceViews\002_q02\view.sql --target tpcds.v_q02 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\003_q03\view.sql --target tpcds.v_q03 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\004_q04\view.sql --target tpcds.v_q04 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\005_q05\view.sql --target tpcds.v_q05 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\006_q06\view.sql --target tpcds.v_q06 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\007_q07\view.sql --target tpcds.v_q07 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\008_q08\view.sql --target tpcds.v_q08 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\009_q09\view.sql --target tpcds.v_q09 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\010_q10\view.sql --target tpcds.v_q10 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\011_q11\view.sql --target tpcds.v_q11 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\012_q12\view.sql --target tpcds.v_q12 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\013_q13\view.sql --target tpcds.v_q13 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\014_q14\view.sql --target tpcds.v_q14 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\015_q15\view.sql --target tpcds.v_q15 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\016_q16\view.sql --target tpcds.v_q16 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\017_q17\view.sql --target tpcds.v_q17 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\018_q18\view.sql --target tpcds.v_q18 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\019_q19\view.sql --target tpcds.v_q19 --workspace TransformWS
meta-transform-script from sql-file --path SourceViews\020_q20\view.sql --target tpcds.v_q20 --workspace TransformWS

pushd TransformWS

meta-transform-script to sql-path --out ..\RoundTrippedViews
meta-transform-script to sql-path --out ..\RoundTrippedViews.sql
meta-transform-script to sql-code --name tpcds.v_q01

popd
