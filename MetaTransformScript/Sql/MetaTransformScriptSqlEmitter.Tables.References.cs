using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private string RenderFromClause(FromClause fromClause)
    {
        var tableReferences = GetOrderedItems(model.FromClauseTableReferencesItemList, fromClause.Id)
            .Select(row => RenderTableReference(row.Value))
            .ToArray();
        return string.Join(", ", tableReferences);
    }

    private string RenderTableReference(TableReference tableReference)
    {
        var aliasBase = FindByBaseId(model.TableReferenceWithAliasList, tableReference.Id);
        var aliasAndColumnsBase = aliasBase is null ? null : FindByBaseId(model.TableReferenceWithAliasAndColumnsList, aliasBase.Id);
        var namedTableReference = FindByBaseId(model.NamedTableReferenceList, aliasBase?.Id ?? tableReference.Id);
        if (namedTableReference is not null)
        {
            var schemaObject = GetOwnerLink(model.NamedTableReferenceSchemaObjectLinkList, namedTableReference.Id, "NamedTableReference.SchemaObject").Value;
            var rendered = RenderSchemaObjectName(schemaObject);

            var aliasLink = aliasBase is null ? null : FindOwnerLink(model.TableReferenceWithAliasAliasLinkList, aliasBase.Id);
            if (aliasLink is not null)
            {
                rendered += " AS " + RenderIdentifier(aliasLink.Value);
            }

            var tableSampleClauseLink = FindOwnerLink(model.NamedTableReferenceTableSampleClauseLinkList, namedTableReference.Id);
            if (tableSampleClauseLink is not null)
            {
                rendered += " " + RenderTableSampleClause(tableSampleClauseLink.Value);
            }

            return rendered;
        }

        var globalFunctionTableReference = aliasBase is null ? null : FindByBaseId(model.GlobalFunctionTableReferenceList, aliasBase.Id);
        if (globalFunctionTableReference is not null)
        {
            var functionName = RenderIdentifier(GetOwnerLink(
                model.GlobalFunctionTableReferenceNameLinkList,
                globalFunctionTableReference.Id,
                "GlobalFunctionTableReference.Name").Value);
            var parameters = GetOrderedItems(model.GlobalFunctionTableReferenceParametersItemList, globalFunctionTableReference.Id)
                .Select(row => RenderScalarExpression(row.Value))
                .ToArray();

            var rendered = $"{functionName}({string.Join(", ", parameters)})";
            var aliasOwner = aliasBase
                ?? throw new InvalidOperationException($"GlobalFunctionTableReference '{globalFunctionTableReference.Id}' did not resolve to TableReferenceWithAlias.");
            var aliasLink = FindOwnerLink(model.TableReferenceWithAliasAliasLinkList, aliasOwner.Id);
            if (aliasLink is not null)
            {
                rendered += " AS " + RenderIdentifier(aliasLink.Value);
            }

            return rendered;
        }

        var schemaObjectFunctionTableReference = aliasAndColumnsBase is null ? null : FindByBaseId(model.SchemaObjectFunctionTableReferenceList, aliasAndColumnsBase.Id);
        if (schemaObjectFunctionTableReference is not null)
        {
            var schemaObject = RenderSchemaObjectName(GetOwnerLink(
                model.SchemaObjectFunctionTableReferenceSchemaObjectLinkList,
                schemaObjectFunctionTableReference.Id,
                "SchemaObjectFunctionTableReference.SchemaObject").Value);
            var parameters = GetOrderedItems(model.SchemaObjectFunctionTableReferenceParametersItemList, schemaObjectFunctionTableReference.Id)
                .Select(row => RenderScalarExpression(row.Value))
                .ToArray();
            return $"{schemaObject}({string.Join(", ", parameters)}){RenderAliasAndColumns(aliasAndColumnsBase!)}";
        }

        var xmlNodesTableReference = aliasAndColumnsBase is null ? null : FindByBaseId(model.XmlNodesTableReferenceList, aliasAndColumnsBase.Id);
        if (xmlNodesTableReference is not null)
        {
            var targetExpression = RenderScalarExpression(GetOwnerLink(
                model.XmlNodesTableReferenceTargetExpressionLinkList,
                xmlNodesTableReference.Id,
                "XmlNodesTableReference.TargetExpression").Value);
            var xQueryString = GetOwnerLink(
                model.XmlNodesTableReferenceXQueryStringLinkList,
                xmlNodesTableReference.Id,
                "XmlNodesTableReference.XQueryString").Value;
            var xQueryLiteral = RenderLiteral(GetById(
                model.LiteralList,
                xQueryString.BaseId,
                "XmlNodesTableReference.XQueryStringBase"));
            return $"{targetExpression}.nodes({xQueryLiteral}){RenderAliasAndColumns(aliasAndColumnsBase!)}";
        }

        var pivotedTableReference = aliasBase is null ? null : FindByBaseId(model.PivotedTableReferenceList, aliasBase.Id);
        if (pivotedTableReference is not null)
        {
            var source = RenderTableReference(GetOwnerLink(
                model.PivotedTableReferenceTableReferenceLinkList,
                pivotedTableReference.Id,
                "PivotedTableReference.TableReference").Value);
            var aggregateFunction = RenderMultiPartIdentifier(GetOwnerLink(
                model.PivotedTableReferenceAggregateFunctionIdentifierLinkList,
                pivotedTableReference.Id,
                "PivotedTableReference.AggregateFunctionIdentifier").Value);
            var valueColumns = GetOrderedItems(model.PivotedTableReferenceValueColumnsItemList, pivotedTableReference.Id)
                .Select(row => RenderColumnReferenceExpression(row.Value))
                .ToArray();
            var pivotColumn = RenderColumnReferenceExpression(GetOwnerLink(
                model.PivotedTableReferencePivotColumnLinkList,
                pivotedTableReference.Id,
                "PivotedTableReference.PivotColumn").Value);
            var inColumns = GetOrderedItems(model.PivotedTableReferenceInColumnsItemList, pivotedTableReference.Id)
                .Select(row => RenderIdentifier(row.Value))
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
            var aliasLink = FindOwnerLink(model.TableReferenceWithAliasAliasLinkList, aliasOwner.Id)
                ?? throw new InvalidOperationException($"PivotedTableReference '{pivotedTableReference.Id}' was missing an alias.");

            return $"{source}{Environment.NewLine}PIVOT{Environment.NewLine}({Environment.NewLine}    {aggregateFunction}({string.Join(", ", valueColumns)}){Environment.NewLine}    FOR {pivotColumn} IN ({string.Join(", ", inColumns)}){Environment.NewLine}) AS {RenderIdentifier(aliasLink.Value)}";
        }

        var unpivotedTableReference = aliasBase is null ? null : FindByBaseId(model.UnpivotedTableReferenceList, aliasBase.Id);
        if (unpivotedTableReference is not null)
        {
            var source = RenderTableReference(GetOwnerLink(
                model.UnpivotedTableReferenceTableReferenceLinkList,
                unpivotedTableReference.Id,
                "UnpivotedTableReference.TableReference").Value);
            var valueColumn = RenderIdentifier(GetOwnerLink(
                model.UnpivotedTableReferenceValueColumnLinkList,
                unpivotedTableReference.Id,
                "UnpivotedTableReference.ValueColumn").Value);
            var pivotColumn = RenderIdentifier(GetOwnerLink(
                model.UnpivotedTableReferencePivotColumnLinkList,
                unpivotedTableReference.Id,
                "UnpivotedTableReference.PivotColumn").Value);
            var inColumns = GetOrderedItems(model.UnpivotedTableReferenceInColumnsItemList, unpivotedTableReference.Id)
                .Select(row => RenderColumnReferenceExpression(row.Value))
                .ToArray();

            if (inColumns.Length == 0)
            {
                throw new InvalidOperationException($"UnpivotedTableReference '{unpivotedTableReference.Id}' had no IN columns.");
            }

            var aliasOwner = aliasBase
                ?? throw new InvalidOperationException($"UnpivotedTableReference '{unpivotedTableReference.Id}' did not resolve to TableReferenceWithAlias.");
            var aliasLink = FindOwnerLink(model.TableReferenceWithAliasAliasLinkList, aliasOwner.Id)
                ?? throw new InvalidOperationException($"UnpivotedTableReference '{unpivotedTableReference.Id}' was missing an alias.");

            return $"{source}{Environment.NewLine}UNPIVOT{Environment.NewLine}({Environment.NewLine}    {valueColumn} FOR {pivotColumn} IN ({string.Join(", ", inColumns)}){Environment.NewLine}) AS {RenderIdentifier(aliasLink.Value)}";
        }

        var fullTextTableReference = aliasBase is null ? null : FindByBaseId(model.FullTextTableReferenceList, aliasBase.Id);
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
                model.FullTextTableReferenceTableNameLinkList,
                fullTextTableReference.Id,
                "FullTextTableReference.TableName").Value);
            var columns = GetOrderedItems(model.FullTextTableReferenceColumnsItemList, fullTextTableReference.Id)
                .Select(row => RenderColumnReferenceExpression(row.Value))
                .ToArray();
            var searchCondition = RenderValueExpression(GetOwnerLink(
                model.FullTextTableReferenceSearchConditionLinkList,
                fullTextTableReference.Id,
                "FullTextTableReference.SearchCondition").Value);

            var rendered = $"{functionName}({tableName}, {RenderFullTextColumns(columns)}, {searchCondition})";
            var aliasOwner = aliasBase
                ?? throw new InvalidOperationException($"FullTextTableReference '{fullTextTableReference.Id}' did not resolve to TableReferenceWithAlias.");
            var aliasLink = FindOwnerLink(model.TableReferenceWithAliasAliasLinkList, aliasOwner.Id);
            if (aliasLink is not null)
            {
                rendered += " AS " + RenderIdentifier(aliasLink.Value);
            }

            return rendered;
        }

        var queryDerivedTable = aliasAndColumnsBase is null ? null : FindByBaseId(model.QueryDerivedTableList, aliasAndColumnsBase.Id);
        if (queryDerivedTable is not null)
        {
            var queryExpression = RenderQueryExpression(GetOwnerLink(
                model.QueryDerivedTableQueryExpressionLinkList,
                queryDerivedTable.Id,
                "QueryDerivedTable.QueryExpression").Value);
            return $"({Environment.NewLine}{queryExpression}{Environment.NewLine}){RenderAliasAndColumns(aliasAndColumnsBase!)}";
        }

        var inlineDerivedTable = aliasAndColumnsBase is null ? null : FindByBaseId(model.InlineDerivedTableList, aliasAndColumnsBase.Id);
        if (inlineDerivedTable is not null)
        {
            var rowValues = GetOrderedItems(model.InlineDerivedTableRowValuesItemList, inlineDerivedTable.Id)
                .Select(row => RenderRowValue(row.Value))
                .ToArray();
            return $"({Environment.NewLine}VALUES{Environment.NewLine}    {string.Join("," + Environment.NewLine + "    ", rowValues)}{Environment.NewLine}){RenderAliasAndColumns(aliasAndColumnsBase!)}";
        }

        var joinParenthesisTableReference = FindByBaseId(model.JoinParenthesisTableReferenceList, tableReference.Id);
        if (joinParenthesisTableReference is not null)
        {
            var join = GetOwnerLink(
                model.JoinParenthesisTableReferenceJoinLinkList,
                joinParenthesisTableReference.Id,
                "JoinParenthesisTableReference.Join").Value;
            return "(" + RenderTableReference(join) + ")";
        }

        var joinTableReference = FindByBaseId(model.JoinTableReferenceList, tableReference.Id)
            ?? throw new InvalidOperationException($"Unsupported MetaTransformScript TableReference id '{tableReference.Id}'.");

        var first = RenderTableReference(GetOwnerLink(model.JoinTableReferenceFirstTableReferenceLinkList, joinTableReference.Id, "JoinTableReference.FirstTableReference").Value);
        var second = RenderTableReference(GetOwnerLink(model.JoinTableReferenceSecondTableReferenceLinkList, joinTableReference.Id, "JoinTableReference.SecondTableReference").Value);

        var qualifiedJoin = FindByBaseId(model.QualifiedJoinList, joinTableReference.Id);
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
            var predicate = RenderBooleanExpression(GetOwnerLink(model.QualifiedJoinSearchConditionLinkList, qualifiedJoin.Id, "QualifiedJoin.SearchCondition").Value);
            return $"{first}{Environment.NewLine}{joinText} {second}{Environment.NewLine}    ON {predicate}";
        }

        var unqualifiedJoin = FindByBaseId(model.UnqualifiedJoinList, joinTableReference.Id);
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
