call cleanup.cmd >nul 2>&1

meta-transform-script from sql-file --path SourceViews\001_basic_select\view.sql --target dbo.v_basic --new-workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\002_select_star\view.sql --target dbo.v_select_star --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\003_join_variants\view.sql --target dbo.v_join_variants --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\004_apply_sources\view.sql --target dbo.v_apply_sources --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\005_pivot\view.sql --target dbo.v_pivot --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\006_unpivot\view.sql --target dbo.v_unpivot --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\007_where_predicates\view.sql --target dbo.v_where_predicates --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\008_group_by_having\view.sql --target dbo.v_group_by_having --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\009_grouping_sets\view.sql --target dbo.v_grouping_sets --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\010_rollup_cube\view.sql --target dbo.v_rollup_cube --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\011_subqueries_and_correlation\view.sql --target dbo.v_subqueries_and_correlation --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\012_subquery_predicates\view.sql --target dbo.v_subquery_predicates --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\013_set_operations\view.sql --target dbo.v_set_operations --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\014_value_expressions\view.sql --target dbo.v_value_expressions --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\015_window_functions\view.sql --target dbo.v_window_functions --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\016_named_window\view.sql --target dbo.v_named_window --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\017_cte\view.sql --target dbo.v_cte --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\018_ordering_and_top\view.sql --target dbo.v_ordering_and_top --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\019_offset_fetch\view.sql --target dbo.v_offset_fetch --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\020_xml_namespaces_and_methods\view.sql --target dbo.v_xml_namespaces_and_methods --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\021_inline_values\view.sql --target dbo.v_inline_values --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\023_table_sample\view.sql --target dbo.v_table_sample --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\024_query_parentheses\view.sql --target dbo.v_query_parentheses --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\025_distinct_predicate\view.sql --target dbo.v_distinct_predicate --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\026_builtin_table_functions\view.sql --target dbo.v_builtin_table_functions --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\027_fulltext\view.sql --target dbo.v_fulltext --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\028_fulltext_table\view.sql --target dbo.v_fulltext_table --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\029_literals_and_special_calls\view.sql --target dbo.v_literals_and_special_calls --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\030_time_zone_extract\view.sql --target dbo.v_time_zone_extract --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\031_join_parentheses\view.sql --target dbo.v_join_parentheses --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\036_sequence_and_globals\view.sql --target dbo.v_sequence_and_globals --workspace MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script from sql-file --path SourceViews\040_view_column_list\view.sql --target dbo.v_view_column_list --workspace MetaTransformScriptReferenceCorpusWorkspace

pushd MetaTransformScriptReferenceCorpusWorkspace

meta-transform-script to sql-path --out ..\RoundTrippedViews
meta-transform-script to sql-path --out ..\RoundTrippedViews.sql
meta-transform-script to sql-code --name dbo.v_window_functions

popd

meta-transform-script from sql-file --path RoundTrippedViews\view.sql --target dbo.v_basic --new-workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_2.sql --target dbo.v_select_star --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_3.sql --target dbo.v_join_variants --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_4.sql --target dbo.v_apply_sources --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_5.sql --target dbo.v_pivot --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_6.sql --target dbo.v_unpivot --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_7.sql --target dbo.v_where_predicates --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_8.sql --target dbo.v_group_by_having --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_9.sql --target dbo.v_grouping_sets --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_10.sql --target dbo.v_rollup_cube --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_11.sql --target dbo.v_subqueries_and_correlation --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_12.sql --target dbo.v_subquery_predicates --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_13.sql --target dbo.v_set_operations --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_14.sql --target dbo.v_value_expressions --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_15.sql --target dbo.v_window_functions --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_16.sql --target dbo.v_named_window --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_17.sql --target dbo.v_cte --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_18.sql --target dbo.v_ordering_and_top --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_19.sql --target dbo.v_offset_fetch --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_20.sql --target dbo.v_xml_namespaces_and_methods --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_21.sql --target dbo.v_inline_values --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_22.sql --target dbo.v_table_sample --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_23.sql --target dbo.v_query_parentheses --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_24.sql --target dbo.v_distinct_predicate --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_25.sql --target dbo.v_builtin_table_functions --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_26.sql --target dbo.v_fulltext --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_27.sql --target dbo.v_fulltext_table --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_28.sql --target dbo.v_literals_and_special_calls --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_29.sql --target dbo.v_time_zone_extract --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_30.sql --target dbo.v_join_parentheses --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_31.sql --target dbo.v_sequence_and_globals --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script from sql-file --path RoundTrippedViews\view_32.sql --target dbo.v_view_column_list --workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace

meta instance diff MetaTransformScriptReferenceCorpusWorkspace MetaTransformScriptReferenceCorpusRoundTripWorkspace

pushd MetaTransformScriptReferenceCorpusRoundTripWorkspace

meta-transform-script to sql-code --name dbo.v_xml_namespaces_and_methods

popd
