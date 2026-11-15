using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private string RenderFromClause(FromClause fromClause)
    {
        var tableReferences = GetOrderedItems(model.FromClauseTableReferencesItemList, static row => row.FromClause?.Id, nameof(FromClauseTableReferencesItem.FromClause), static row => row.Ordinal, fromClause.Id)
            .Select(row => RenderTableReference(row.TableReference))
            .ToArray();
        return string.Join(", ", tableReferences);
    }

    private string RenderTableReference(TableReference tableReference)
    {
        var aliasBase = FindByBaseId(model.TableReferenceWithAliasList, static row => row.TableReference?.Id, nameof(TableReferenceWithAlias.TableReference), tableReference.Id);
        var aliasAndColumnsBase = aliasBase is null ? null : FindByBaseId(model.TableReferenceWithAliasAndColumnsList, static row => row.TableReferenceWithAlias?.Id, nameof(TableReferenceWithAliasAndColumns.TableReferenceWithAlias), aliasBase.Id);
        var namedTableReference = FindByBaseId(model.NamedTableReferenceList, static row => row.TableReferenceWithAlias?.Id, nameof(NamedTableReference.TableReferenceWithAlias), aliasBase?.Id ?? tableReference.Id);
        if (namedTableReference is not null)
        {
            var schemaObject = GetOwnerLink(model.NamedTableReferenceSchemaObjectLinkList, static row => row.NamedTableReference?.Id, nameof(NamedTableReferenceSchemaObjectLink.NamedTableReference), namedTableReference.Id, "NamedTableReference.SchemaObject").SchemaObjectName;
            var rendered = RenderSchemaObjectName(schemaObject);

            var aliasLink = aliasBase is null ? null : FindOwnerLink(model.TableReferenceWithAliasAliasLinkList, static row => row.TableReferenceWithAlias?.Id, nameof(TableReferenceWithAliasAliasLink.TableReferenceWithAlias), aliasBase.Id);
            if (aliasLink is not null)
            {
                rendered += " AS " + RenderIdentifier(aliasLink.Identifier);
            }

            if (aliasBase is not null)
            {
                var tableHints = GetOrderedItems(model.TableReferenceWithAliasTableHintsItemList, static row => row.TableReferenceWithAlias?.Id, nameof(TableReferenceWithAliasTableHintsItem.TableReferenceWithAlias), static row => row.Ordinal, aliasBase.Id)
                    .Select(row => RenderSqlHint(row.SqlHint))
                    .ToArray();
                if (tableHints.Length > 0)
                {
                    rendered += " WITH (" + string.Join(", ", tableHints) + ")";
                }
            }

            var tableSampleClauseLink = FindOwnerLink(model.NamedTableReferenceTableSampleClauseLinkList, static row => row.NamedTableReference?.Id, nameof(NamedTableReferenceTableSampleClauseLink.NamedTableReference), namedTableReference.Id);
            if (tableSampleClauseLink is not null)
            {
                rendered += " " + RenderTableSampleClause(tableSampleClauseLink.TableSampleClause);
            }

            return rendered;
        }

        var globalFunctionTableReference = aliasBase is null ? null : FindByBaseId(model.GlobalFunctionTableReferenceList, static row => row.TableReferenceWithAlias?.Id, nameof(GlobalFunctionTableReference.TableReferenceWithAlias), aliasBase.Id);
        if (globalFunctionTableReference is not null)
        {
            var functionName = RenderIdentifier(GetOwnerLink(
                model.GlobalFunctionTableReferenceNameLinkList, static row => row.GlobalFunctionTableReference?.Id, nameof(GlobalFunctionTableReferenceNameLink.GlobalFunctionTableReference),
                globalFunctionTableReference.Id,
                "GlobalFunctionTableReference.Name").Identifier);
            var parameters = GetOrderedItems(model.GlobalFunctionTableReferenceParametersItemList, static row => row.GlobalFunctionTableReference?.Id, nameof(GlobalFunctionTableReferenceParametersItem.GlobalFunctionTableReference), static row => row.Ordinal, globalFunctionTableReference.Id)
                .Select(row => RenderScalarExpression(row.ScalarExpression))
                .ToArray();

            var rendered = $"{functionName}({string.Join(", ", parameters)})";
            var aliasOwner = aliasBase
                ?? throw new InvalidOperationException($"GlobalFunctionTableReference '{globalFunctionTableReference.Id}' did not resolve to TableReferenceWithAlias.");
            var aliasLink = FindOwnerLink(model.TableReferenceWithAliasAliasLinkList, static row => row.TableReferenceWithAlias?.Id, nameof(TableReferenceWithAliasAliasLink.TableReferenceWithAlias), aliasOwner.Id);
            if (aliasLink is not null)
            {
                rendered += " AS " + RenderIdentifier(aliasLink.Identifier);
            }

            return rendered;
        }

        var schemaObjectFunctionTableReference = aliasAndColumnsBase is null ? null : FindByBaseId(model.SchemaObjectFunctionTableReferenceList, static row => row.TableReferenceWithAliasAndColumns?.Id, nameof(SchemaObjectFunctionTableReference.TableReferenceWithAliasAndColumns), aliasAndColumnsBase.Id);
        if (schemaObjectFunctionTableReference is not null)
        {
            var schemaObject = RenderSchemaObjectName(GetOwnerLink(
                model.SchemaObjectFunctionTableReferenceSchemaObjectLinkList, static row => row.SchemaObjectFunctionTableReference?.Id, nameof(SchemaObjectFunctionTableReferenceSchemaObjectLink.SchemaObjectFunctionTableReference),
                schemaObjectFunctionTableReference.Id,
                "SchemaObjectFunctionTableReference.SchemaObject").SchemaObjectName);
            var parameters = GetOrderedItems(model.SchemaObjectFunctionTableReferenceParametersItemList, static row => row.SchemaObjectFunctionTableReference?.Id, nameof(SchemaObjectFunctionTableReferenceParametersItem.SchemaObjectFunctionTableReference), static row => row.Ordinal, schemaObjectFunctionTableReference.Id)
                .Select(row => RenderScalarExpression(row.ScalarExpression))
                .ToArray();
            return $"{schemaObject}({string.Join(", ", parameters)}){RenderAliasAndColumns(aliasAndColumnsBase!, requireAlias: false)}";
        }

        var xmlNodesTableReference = aliasAndColumnsBase is null ? null : FindByBaseId(model.XmlNodesTableReferenceList, static row => row.TableReferenceWithAliasAndColumns?.Id, nameof(XmlNodesTableReference.TableReferenceWithAliasAndColumns), aliasAndColumnsBase.Id);
        if (xmlNodesTableReference is not null)
        {
            var targetExpression = RenderScalarExpression(GetOwnerLink(
                model.XmlNodesTableReferenceTargetExpressionLinkList, static row => row.XmlNodesTableReference?.Id, nameof(XmlNodesTableReferenceTargetExpressionLink.XmlNodesTableReference),
                xmlNodesTableReference.Id,
                "XmlNodesTableReference.TargetExpression").ScalarExpression);
            var xQueryString = GetOwnerLink(
                model.XmlNodesTableReferenceXQueryStringLinkList, static row => row.XmlNodesTableReference?.Id, nameof(XmlNodesTableReferenceXQueryStringLink.XmlNodesTableReference),
                xmlNodesTableReference.Id,
                "XmlNodesTableReference.XQueryString").StringLiteral;
            var xQueryLiteral = RenderLiteral(GetById(
                model.LiteralList, static row => row.Id,
                xQueryString.Literal.Id,
                "XmlNodesTableReference.XQueryStringBase"));
            return $"{targetExpression}.nodes({xQueryLiteral}){RenderAliasAndColumns(aliasAndColumnsBase!)}";
        }

        var pivotedTableReference = aliasBase is null ? null : FindByBaseId(model.PivotedTableReferenceList, static row => row.TableReferenceWithAlias?.Id, nameof(PivotedTableReference.TableReferenceWithAlias), aliasBase.Id);
        if (pivotedTableReference is not null)
        {
            var source = RenderTableReference(GetOwnerLink(
                model.PivotedTableReferenceTableReferenceLinkList, static row => row.PivotedTableReference?.Id, nameof(PivotedTableReferenceTableReferenceLink.PivotedTableReference),
                pivotedTableReference.Id,
                "PivotedTableReference.TableReference").TableReference);
            var aggregateFunction = RenderMultiPartIdentifier(GetOwnerLink(
                model.PivotedTableReferenceAggregateFunctionIdentifierLinkList, static row => row.PivotedTableReference?.Id, nameof(PivotedTableReferenceAggregateFunctionIdentifierLink.PivotedTableReference),
                pivotedTableReference.Id,
                "PivotedTableReference.AggregateFunctionIdentifier").MultiPartIdentifier);
            var valueColumns = GetOrderedItems(model.PivotedTableReferenceValueColumnsItemList, static row => row.PivotedTableReference?.Id, nameof(PivotedTableReferenceValueColumnsItem.PivotedTableReference), static row => row.Ordinal, pivotedTableReference.Id)
                .Select(row => RenderColumnReferenceExpression(row.ColumnReferenceExpression))
                .ToArray();
            var pivotColumn = RenderColumnReferenceExpression(GetOwnerLink(
                model.PivotedTableReferencePivotColumnLinkList, static row => row.PivotedTableReference?.Id, nameof(PivotedTableReferencePivotColumnLink.PivotedTableReference),
                pivotedTableReference.Id,
                "PivotedTableReference.PivotColumn").ColumnReferenceExpression);
            var inColumns = GetOrderedItems(model.PivotedTableReferenceInColumnsItemList, static row => row.PivotedTableReference?.Id, nameof(PivotedTableReferenceInColumnsItem.PivotedTableReference), static row => row.Ordinal, pivotedTableReference.Id)
                .Select(row => RenderIdentifier(row.Identifier))
                .ToArray();

            if (valueColumns.Length == 0)
            {
                throw new InvalidOperationException($"PivotedTableReference '{pivotedTableReference.Id}' had no value columns.");
            }

            if (inColumns.Length == 0)
            {
                throw new InvalidOperationException($"PivotedTableReference '{pivotedTableReference.Id}' had no IN columns.");
            }

            var aliasOwner = aliasBase
                ?? throw new InvalidOperationException($"PivotedTableReference '{pivotedTableReference.Id}' did not resolve to TableReferenceWithAlias.");
            var aliasLink = FindOwnerLink(model.TableReferenceWithAliasAliasLinkList, static row => row.TableReferenceWithAlias?.Id, nameof(TableReferenceWithAliasAliasLink.TableReferenceWithAlias), aliasOwner.Id)
                ?? throw new InvalidOperationException($"PivotedTableReference '{pivotedTableReference.Id}' was missing an alias.");

            return $"{source}{Environment.NewLine}PIVOT{Environment.NewLine}({Environment.NewLine}    {aggregateFunction}({string.Join(", ", valueColumns)}){Environment.NewLine}    FOR {pivotColumn} IN ({string.Join(", ", inColumns)}){Environment.NewLine}) AS {RenderIdentifier(aliasLink.Identifier)}";
        }

        var unpivotedTableReference = aliasBase is null ? null : FindByBaseId(model.UnpivotedTableReferenceList, static row => row.TableReferenceWithAlias?.Id, nameof(UnpivotedTableReference.TableReferenceWithAlias), aliasBase.Id);
        if (unpivotedTableReference is not null)
        {
            var source = RenderTableReference(GetOwnerLink(
                model.UnpivotedTableReferenceTableReferenceLinkList, static row => row.UnpivotedTableReference?.Id, nameof(UnpivotedTableReferenceTableReferenceLink.UnpivotedTableReference),
                unpivotedTableReference.Id,
                "UnpivotedTableReference.TableReference").TableReference);
            var valueColumn = RenderIdentifier(GetOwnerLink(
                model.UnpivotedTableReferenceValueColumnLinkList, static row => row.UnpivotedTableReference?.Id, nameof(UnpivotedTableReferenceValueColumnLink.UnpivotedTableReference),
                unpivotedTableReference.Id,
                "UnpivotedTableReference.ValueColumn").Identifier);
            var pivotColumn = RenderIdentifier(GetOwnerLink(
                model.UnpivotedTableReferencePivotColumnLinkList, static row => row.UnpivotedTableReference?.Id, nameof(UnpivotedTableReferencePivotColumnLink.UnpivotedTableReference),
                unpivotedTableReference.Id,
                "UnpivotedTableReference.PivotColumn").Identifier);
            var inColumns = GetOrderedItems(model.UnpivotedTableReferenceInColumnsItemList, static row => row.UnpivotedTableReference?.Id, nameof(UnpivotedTableReferenceInColumnsItem.UnpivotedTableReference), static row => row.Ordinal, unpivotedTableReference.Id)
                .Select(row => RenderColumnReferenceExpression(row.ColumnReferenceExpression))
                .ToArray();

            if (inColumns.Length == 0)
            {
                throw new InvalidOperationException($"UnpivotedTableReference '{unpivotedTableReference.Id}' had no IN columns.");
            }

            var aliasOwner = aliasBase
                ?? throw new InvalidOperationException($"UnpivotedTableReference '{unpivotedTableReference.Id}' did not resolve to TableReferenceWithAlias.");
            var aliasLink = FindOwnerLink(model.TableReferenceWithAliasAliasLinkList, static row => row.TableReferenceWithAlias?.Id, nameof(TableReferenceWithAliasAliasLink.TableReferenceWithAlias), aliasOwner.Id)
                ?? throw new InvalidOperationException($"UnpivotedTableReference '{unpivotedTableReference.Id}' was missing an alias.");

            return $"{source}{Environment.NewLine}UNPIVOT{Environment.NewLine}({Environment.NewLine}    {valueColumn} FOR {pivotColumn} IN ({string.Join(", ", inColumns)}){Environment.NewLine}) AS {RenderIdentifier(aliasLink.Identifier)}";
        }

        var fullTextTableReference = aliasBase is null ? null : FindByBaseId(model.FullTextTableReferenceList, static row => row.TableReferenceWithAlias?.Id, nameof(FullTextTableReference.TableReferenceWithAlias), aliasBase.Id);
        if (fullTextTableReference is not null)
        {
            var functionName = fullTextTableReference.FullTextFunctionType switch
            {
                "Contains" => "CONTAINSTABLE",
                "FreeText" => "FREETEXTTABLE",
                _ => throw new InvalidOperationException(
                    $"Unsupported MetaTransformScript FullTextTableReference.FullTextFunctionType '{fullTextTableReference.FullTextFunctionType}'.")
            };
            var tableName = RenderSchemaObjectName(GetOwnerLink(
                model.FullTextTableReferenceTableNameLinkList, static row => row.FullTextTableReference?.Id, nameof(FullTextTableReferenceTableNameLink.FullTextTableReference),
                fullTextTableReference.Id,
                "FullTextTableReference.TableName").SchemaObjectName);
            var columns = GetOrderedItems(model.FullTextTableReferenceColumnsItemList, static row => row.FullTextTableReference?.Id, nameof(FullTextTableReferenceColumnsItem.FullTextTableReference), static row => row.Ordinal, fullTextTableReference.Id)
                .Select(row => RenderColumnReferenceExpression(row.ColumnReferenceExpression))
                .ToArray();
            var searchCondition = RenderValueExpression(GetOwnerLink(
                model.FullTextTableReferenceSearchConditionLinkList, static row => row.FullTextTableReference?.Id, nameof(FullTextTableReferenceSearchConditionLink.FullTextTableReference),
                fullTextTableReference.Id,
                "FullTextTableReference.SearchCondition").ValueExpression);

            var rendered = $"{functionName}({tableName}, {RenderFullTextColumns(columns)}, {searchCondition})";
            var aliasOwner = aliasBase
                ?? throw new InvalidOperationException($"FullTextTableReference '{fullTextTableReference.Id}' did not resolve to TableReferenceWithAlias.");
            var aliasLink = FindOwnerLink(model.TableReferenceWithAliasAliasLinkList, static row => row.TableReferenceWithAlias?.Id, nameof(TableReferenceWithAliasAliasLink.TableReferenceWithAlias), aliasOwner.Id);
            if (aliasLink is not null)
            {
                rendered += " AS " + RenderIdentifier(aliasLink.Identifier);
            }

            return rendered;
        }

        var queryDerivedTable = aliasAndColumnsBase is null ? null : FindByBaseId(model.QueryDerivedTableList, static row => row.TableReferenceWithAliasAndColumns?.Id, nameof(QueryDerivedTable.TableReferenceWithAliasAndColumns), aliasAndColumnsBase.Id);
        if (queryDerivedTable is not null)
        {
            var queryExpression = RenderQueryExpression(GetOwnerLink(
                model.QueryDerivedTableQueryExpressionLinkList, static row => row.QueryDerivedTable?.Id, nameof(QueryDerivedTableQueryExpressionLink.QueryDerivedTable),
                queryDerivedTable.Id,
                "QueryDerivedTable.QueryExpression").QueryExpression);
            return $"({Environment.NewLine}{queryExpression}{Environment.NewLine}){RenderAliasAndColumns(aliasAndColumnsBase!)}";
        }

        var inlineDerivedTable = aliasAndColumnsBase is null ? null : FindByBaseId(model.InlineDerivedTableList, static row => row.TableReferenceWithAliasAndColumns?.Id, nameof(InlineDerivedTable.TableReferenceWithAliasAndColumns), aliasAndColumnsBase.Id);
        if (inlineDerivedTable is not null)
        {
            var rowValues = GetOrderedItems(model.InlineDerivedTableRowValuesItemList, static row => row.InlineDerivedTable?.Id, nameof(InlineDerivedTableRowValuesItem.InlineDerivedTable), static row => row.Ordinal, inlineDerivedTable.Id)
                .Select(row => RenderRowValue(row.RowValue))
                .ToArray();
            return $"({Environment.NewLine}VALUES{Environment.NewLine}    {string.Join("," + Environment.NewLine + "    ", rowValues)}{Environment.NewLine}){RenderAliasAndColumns(aliasAndColumnsBase!)}";
        }

        var joinParenthesisTableReference = FindByBaseId(model.JoinParenthesisTableReferenceList, static row => row.TableReference?.Id, nameof(JoinParenthesisTableReference.TableReference), tableReference.Id);
        if (joinParenthesisTableReference is not null)
        {
            var join = GetOwnerLink(
                model.JoinParenthesisTableReferenceJoinLinkList, static row => row.JoinParenthesisTableReference?.Id, nameof(JoinParenthesisTableReferenceJoinLink.JoinParenthesisTableReference),
                joinParenthesisTableReference.Id,
                "JoinParenthesisTableReference.Join").TableReference;
            return "(" + RenderTableReference(join) + ")";
        }

        var joinTableReference = FindByBaseId(model.JoinTableReferenceList, static row => row.TableReference?.Id, nameof(JoinTableReference.TableReference), tableReference.Id)
            ?? throw new InvalidOperationException($"Unsupported MetaTransformScript TableReference id '{tableReference.Id}'.");

        var first = RenderTableReference(GetOwnerLink(model.JoinTableReferenceFirstTableReferenceLinkList, static row => row.JoinTableReference?.Id, nameof(JoinTableReferenceFirstTableReferenceLink.JoinTableReference), joinTableReference.Id, "JoinTableReference.FirstTableReference").TableReference);
        var second = RenderTableReference(GetOwnerLink(model.JoinTableReferenceSecondTableReferenceLinkList, static row => row.JoinTableReference?.Id, nameof(JoinTableReferenceSecondTableReferenceLink.JoinTableReference), joinTableReference.Id, "JoinTableReference.SecondTableReference").TableReference);

        var qualifiedJoin = FindByBaseId(model.QualifiedJoinList, static row => row.JoinTableReference?.Id, nameof(QualifiedJoin.JoinTableReference), joinTableReference.Id);
        if (qualifiedJoin is not null)
        {
            var joinText = qualifiedJoin.QualifiedJoinType switch
            {
                "Inner" => "INNER JOIN",
                "LeftOuter" => "LEFT OUTER JOIN",
                "RightOuter" => "RIGHT OUTER JOIN",
                "FullOuter" => "FULL OUTER JOIN",
                _ => throw new InvalidOperationException($"Unsupported MetaTransformScript QualifiedJoinType '{qualifiedJoin.QualifiedJoinType}'.")
            };
            var predicate = RenderBooleanExpression(GetOwnerLink(model.QualifiedJoinSearchConditionLinkList, static row => row.QualifiedJoin?.Id, nameof(QualifiedJoinSearchConditionLink.QualifiedJoin), qualifiedJoin.Id, "QualifiedJoin.SearchCondition").BooleanExpression);
            return $"{first}{Environment.NewLine}{joinText} {second}{Environment.NewLine}    ON {predicate}";
        }

        var unqualifiedJoin = FindByBaseId(model.UnqualifiedJoinList, static row => row.JoinTableReference?.Id, nameof(UnqualifiedJoin.JoinTableReference), joinTableReference.Id);
        if (unqualifiedJoin is not null)
        {
            var joinText = unqualifiedJoin.UnqualifiedJoinType switch
            {
                "CrossJoin" => "CROSS JOIN",
                "CrossApply" => "CROSS APPLY",
                "OuterApply" => "OUTER APPLY",
                _ => throw new InvalidOperationException($"Unsupported MetaTransformScript UnqualifiedJoinType '{unqualifiedJoin.UnqualifiedJoinType}'.")
            };
            return $"{first}{Environment.NewLine}{joinText} {second}";
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript JoinTableReference id '{joinTableReference.Id}'.");
    }
}
