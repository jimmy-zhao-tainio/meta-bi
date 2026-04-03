using System.Collections;
using System.Globalization;
using System.Text;
using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed class MetaTransformScriptSqlEmitter
{
    private readonly MetaTransformScriptModel model;

    public MetaTransformScriptSqlEmitter(MetaTransformScriptModel model)
    {
        this.model = model;
    }

    public string Render(SelectStatement root)
    {
        var builder = new StringBuilder();

        var statementBase = GetById(model.StatementWithCtesAndXmlNamespacesList, root.BaseId, "SelectStatement.Base");
        var withCtesLink = FindOwnerLink(model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList, statementBase.Id);
        if (withCtesLink is not null)
        {
            builder.Append(RenderWithClause(withCtesLink.Value));
            builder.AppendLine();
        }

        var queryExpressionLink = GetOwnerLink(model.SelectStatementQueryExpressionLinkList, root.Id, "SelectStatement.QueryExpression");
        builder.Append(RenderQueryExpression(queryExpressionLink.Value));
        return builder.ToString();
    }

    private string RenderWithClause(WithCtesAndXmlNamespaces withCtes)
    {
        var parts = new List<string>();

        var xmlNamespacesLink = FindOwnerLink(model.WithCtesAndXmlNamespacesXmlNamespacesLinkList, withCtes.Id);
        if (xmlNamespacesLink is not null)
        {
            var xmlParts = GetOrderedItems(model.XmlNamespacesXmlNamespacesElementsItemList, xmlNamespacesLink.Value.Id)
                .Select(row => RenderXmlNamespacesElement(row.Value))
                .ToArray();
            parts.Add("XMLNAMESPACES (" + string.Join(", ", xmlParts) + ")");
        }

        var ctes = GetOrderedItems(model.WithCtesAndXmlNamespacesCommonTableExpressionsItemList, withCtes.Id)
            .Select(row => RenderCommonTableExpression(row.Value))
            .ToArray();
        parts.AddRange(ctes);

        return "WITH " + string.Join(", ", parts);
    }

    private string RenderCommonTableExpression(CommonTableExpression cte)
    {
        var name = RenderIdentifier(GetOwnerLink(model.CommonTableExpressionExpressionNameLinkList, cte.Id, "CommonTableExpression.ExpressionName").Value);
        var queryExpression = GetOwnerLink(model.CommonTableExpressionQueryExpressionLinkList, cte.Id, "CommonTableExpression.QueryExpression").Value;
        return $"{name} AS ({RenderQueryExpression(queryExpression)})";
    }

    private string RenderQueryExpression(QueryExpression queryExpression)
    {
        string renderedCore;

        var querySpecification = FindByBaseId(model.QuerySpecificationList, queryExpression.Id);
        if (querySpecification is not null)
        {
            renderedCore = RenderQuerySpecification(querySpecification);
        }
        else
        {
            var binaryQueryExpression = FindByBaseId(model.BinaryQueryExpressionList, queryExpression.Id);
            if (binaryQueryExpression is not null)
            {
                var first = RenderQueryExpression(GetOwnerLink(model.BinaryQueryExpressionFirstQueryExpressionLinkList, binaryQueryExpression.Id, "BinaryQueryExpression.FirstQueryExpression").Value);
                var second = RenderQueryExpression(GetOwnerLink(model.BinaryQueryExpressionSecondQueryExpressionLinkList, binaryQueryExpression.Id, "BinaryQueryExpression.SecondQueryExpression").Value);
                var operatorText = binaryQueryExpression.BinaryQueryExpressionType switch
                {
                    "Union" when IsTrue(binaryQueryExpression.All) => "UNION ALL",
                    "Union" => "UNION",
                    "Except" => "EXCEPT",
                    "Intersect" => "INTERSECT",
                    _ => throw new InvalidOperationException($"Unsupported MetaTransformScript BinaryQueryExpressionType '{binaryQueryExpression.BinaryQueryExpressionType}'.")
                };

                renderedCore = $"{first}{Environment.NewLine}{operatorText}{Environment.NewLine}{second}";
            }
            else
            {
                var queryParenthesisExpression = FindByBaseId(model.QueryParenthesisExpressionList, queryExpression.Id);
                if (queryParenthesisExpression is not null)
                {
                    var child = GetOwnerLink(
                        model.QueryParenthesisExpressionQueryExpressionLinkList,
                        queryParenthesisExpression.Id,
                        "QueryParenthesisExpression.QueryExpression").Value;
                    renderedCore = "(" + RenderQueryExpression(child) + ")";
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported MetaTransformScript QueryExpression id '{queryExpression.Id}'.");
                }
            }
        }

        var orderByClauseLink = FindOwnerLink(model.QueryExpressionOrderByClauseLinkList, queryExpression.Id);
        if (orderByClauseLink is not null)
        {
            renderedCore += Environment.NewLine + RenderOrderByClause(orderByClauseLink.Value);
        }

        var offsetClauseLink = FindOwnerLink(model.QueryExpressionOffsetClauseLinkList, queryExpression.Id);
        if (offsetClauseLink is not null)
        {
            renderedCore += Environment.NewLine + RenderOffsetClause(offsetClauseLink.Value);
        }

        return renderedCore;
    }

    private string RenderQuerySpecification(QuerySpecification querySpecification)
    {
        var builder = new StringBuilder();
        builder.Append("SELECT");

        if (!string.IsNullOrWhiteSpace(querySpecification.UniqueRowFilter) &&
            !string.Equals(querySpecification.UniqueRowFilter, "NotSpecified", StringComparison.Ordinal))
        {
            builder.Append(' ');
            builder.Append(querySpecification.UniqueRowFilter.ToUpperInvariant());
        }

        var topRowFilterLink = FindOwnerLink(model.QuerySpecificationTopRowFilterLinkList, querySpecification.Id);
        if (topRowFilterLink is not null)
        {
            builder.Append(' ');
            builder.Append(RenderTopRowFilter(topRowFilterLink.Value));
        }

        var selectElements = GetOrderedItems(model.QuerySpecificationSelectElementsItemList, querySpecification.Id)
            .Select(row => RenderSelectElement(row.Value))
            .ToArray();

        builder.AppendLine();
        builder.Append("    ");
        builder.Append(string.Join("," + Environment.NewLine + "    ", selectElements));

        var fromClauseLink = FindOwnerLink(model.QuerySpecificationFromClauseLinkList, querySpecification.Id);
        if (fromClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append("FROM ");
            builder.Append(RenderFromClause(fromClauseLink.Value));
        }

        var whereClauseLink = FindOwnerLink(model.QuerySpecificationWhereClauseLinkList, querySpecification.Id);
        if (whereClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append("WHERE ");
            builder.Append(RenderBooleanExpression(GetOwnerLink(model.WhereClauseSearchConditionLinkList, whereClauseLink.Value.Id, "WhereClause.SearchCondition").Value));
        }

        var groupByClauseLink = FindOwnerLink(model.QuerySpecificationGroupByClauseLinkList, querySpecification.Id);
        if (groupByClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append("GROUP BY ");
            builder.Append(RenderGroupByClause(groupByClauseLink.Value));
        }

        var havingClauseLink = FindOwnerLink(model.QuerySpecificationHavingClauseLinkList, querySpecification.Id);
        if (havingClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append("HAVING ");
            builder.Append(RenderBooleanExpression(GetOwnerLink(model.HavingClauseSearchConditionLinkList, havingClauseLink.Value.Id, "HavingClause.SearchCondition").Value));
        }

        var windowClauseLink = FindOwnerLink(model.QuerySpecificationWindowClauseLinkList, querySpecification.Id);
        if (windowClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append(RenderWindowClause(windowClauseLink.Value));
        }

        return builder.ToString();
    }

    private string RenderGroupByClause(GroupByClause groupByClause)
    {
        if (!string.IsNullOrWhiteSpace(groupByClause.GroupByOption) &&
            !string.Equals(groupByClause.GroupByOption, "None", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Unsupported MetaTransformScript GroupByOption '{groupByClause.GroupByOption}'.");
        }

        if (IsTrue(groupByClause.All))
        {
            throw new InvalidOperationException("Phase-1 emitter does not support GROUP BY ALL.");
        }

        var groupingSpecifications = GetOrderedItems(model.GroupByClauseGroupingSpecificationsItemList, groupByClause.Id)
            .Select(row => RenderGroupingSpecification(row.Value))
            .ToArray();
        return string.Join(", ", groupingSpecifications);
    }

    private string RenderGroupingSpecification(GroupingSpecification groupingSpecification)
    {
        var expressionGroupingSpecification = FindByBaseId(model.ExpressionGroupingSpecificationList, groupingSpecification.Id);
        if (expressionGroupingSpecification is not null)
        {
            if (IsTrue(expressionGroupingSpecification.DistributedAggregation))
            {
                throw new InvalidOperationException("Phase-1 emitter does not support distributed aggregation grouping specifications.");
            }

            return RenderScalarExpression(GetOwnerLink(
                model.ExpressionGroupingSpecificationExpressionLinkList,
                expressionGroupingSpecification.Id,
                "ExpressionGroupingSpecification.Expression").Value);
        }

        var groupingSetsGroupingSpecification = FindByBaseId(model.GroupingSetsGroupingSpecificationList, groupingSpecification.Id);
        if (groupingSetsGroupingSpecification is not null)
        {
            var sets = GetOrderedItems(model.GroupingSetsGroupingSpecificationSetsItemList, groupingSetsGroupingSpecification.Id)
                .Select(row => RenderGroupingSpecification(row.Value))
                .ToArray();
            return "GROUPING SETS (" + string.Join(", ", sets) + ")";
        }

        var rollupGroupingSpecification = FindByBaseId(model.RollupGroupingSpecificationList, groupingSpecification.Id);
        if (rollupGroupingSpecification is not null)
        {
            var arguments = GetOrderedItems(model.RollupGroupingSpecificationArgumentsItemList, rollupGroupingSpecification.Id)
                .Select(row => RenderGroupingSpecification(row.Value))
                .ToArray();
            return "ROLLUP (" + string.Join(", ", arguments) + ")";
        }

        var cubeGroupingSpecification = FindByBaseId(model.CubeGroupingSpecificationList, groupingSpecification.Id);
        if (cubeGroupingSpecification is not null)
        {
            var arguments = GetOrderedItems(model.CubeGroupingSpecificationArgumentsItemList, cubeGroupingSpecification.Id)
                .Select(row => RenderGroupingSpecification(row.Value))
                .ToArray();
            return "CUBE (" + string.Join(", ", arguments) + ")";
        }

        var compositeGroupingSpecification = FindByBaseId(model.CompositeGroupingSpecificationList, groupingSpecification.Id);
        if (compositeGroupingSpecification is not null)
        {
            var items = GetOrderedItems(model.CompositeGroupingSpecificationItemsItemList, compositeGroupingSpecification.Id)
                .Select(row => RenderGroupingSpecification(row.Value))
                .ToArray();
            return "(" + string.Join(", ", items) + ")";
        }

        if (FindByBaseId(model.GrandTotalGroupingSpecificationList, groupingSpecification.Id) is not null)
        {
            return "()";
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript GroupingSpecification id '{groupingSpecification.Id}'.");
    }

    private string RenderWindowClause(WindowClause windowClause)
    {
        var definitions = GetOrderedItems(model.WindowClauseWindowDefinitionItemList, windowClause.Id)
            .Select(row => RenderWindowDefinition(row.Value))
            .ToArray();
        return "WINDOW" + Environment.NewLine + "    " + string.Join("," + Environment.NewLine + "    ", definitions);
    }

    private string RenderWindowDefinition(WindowDefinition windowDefinition)
    {
        var name = RenderIdentifier(GetOwnerLink(model.WindowDefinitionWindowNameLinkList, windowDefinition.Id, "WindowDefinition.WindowName").Value);
        var parts = new List<string>();

        var refWindowNameLink = FindOwnerLink(model.WindowDefinitionRefWindowNameLinkList, windowDefinition.Id);
        if (refWindowNameLink is not null)
        {
            parts.Add(RenderIdentifier(refWindowNameLink.Value));
        }

        var partitions = GetOrderedItems(model.WindowDefinitionPartitionsItemList, windowDefinition.Id)
            .Select(row => RenderScalarExpression(row.Value))
            .ToArray();
        if (partitions.Length > 0)
        {
            parts.Add("PARTITION BY " + string.Join(", ", partitions));
        }

        var orderByClauseLink = FindOwnerLink(model.WindowDefinitionOrderByClauseLinkList, windowDefinition.Id);
        if (orderByClauseLink is not null)
        {
            parts.Add(RenderOrderByClause(orderByClauseLink.Value));
        }

        var windowFrameClauseLink = FindOwnerLink(model.WindowDefinitionWindowFrameClauseLinkList, windowDefinition.Id);
        if (windowFrameClauseLink is not null)
        {
            parts.Add(RenderWindowFrameClause(windowFrameClauseLink.Value));
        }

        if (parts.Count == 0)
        {
            throw new InvalidOperationException($"WindowDefinition '{windowDefinition.Id}' had no renderable content.");
        }

        return $"{name} AS ({string.Join(" ", parts)})";
    }

    private string RenderOverClause(OverClause overClause)
    {
        var parts = new List<string>();

        var windowNameLink = FindOwnerLink(model.OverClauseWindowNameLinkList, overClause.Id);
        if (windowNameLink is not null)
        {
            parts.Add(RenderIdentifier(windowNameLink.Value));
        }

        var partitions = GetOrderedItems(model.OverClausePartitionsItemList, overClause.Id)
            .Select(row => RenderScalarExpression(row.Value))
            .ToArray();
        if (partitions.Length > 0)
        {
            parts.Add("PARTITION BY " + string.Join(", ", partitions));
        }

        var orderByClauseLink = FindOwnerLink(model.OverClauseOrderByClauseLinkList, overClause.Id);
        if (orderByClauseLink is not null)
        {
            parts.Add(RenderOrderByClause(orderByClauseLink.Value));
        }

        var windowFrameClauseLink = FindOwnerLink(model.OverClauseWindowFrameClauseLinkList, overClause.Id);
        if (windowFrameClauseLink is not null)
        {
            parts.Add(RenderWindowFrameClause(windowFrameClauseLink.Value));
        }

        if (parts.Count == 1 && windowNameLink is not null && partitions.Length == 0 && orderByClauseLink is null && windowFrameClauseLink is null)
        {
            return "OVER " + parts[0];
        }

        if (parts.Count == 0)
        {
            throw new InvalidOperationException($"OverClause '{overClause.Id}' had no renderable content.");
        }

        return "OVER (" + string.Join(" ", parts) + ")";
    }

    private string RenderOrderByClause(OrderByClause orderByClause)
    {
        var elements = GetOrderedItems(model.OrderByClauseOrderByElementsItemList, orderByClause.Id)
            .Select(row => RenderExpressionWithSortOrder(row.Value))
            .ToArray();
        return "ORDER BY " + string.Join(", ", elements);
    }

    private string RenderExpressionWithSortOrder(ExpressionWithSortOrder expressionWithSortOrder)
    {
        var rendered = RenderScalarExpression(GetOwnerLink(
            model.ExpressionWithSortOrderExpressionLinkList,
            expressionWithSortOrder.Id,
            "ExpressionWithSortOrder.Expression").Value);

        return expressionWithSortOrder.SortOrder switch
        {
            "Descending" => rendered + " DESC",
            "Ascending" => rendered + " ASC",
            "NotSpecified" or "" => rendered,
            _ => throw new InvalidOperationException($"Unsupported MetaTransformScript SortOrder '{expressionWithSortOrder.SortOrder}'.")
        };
    }

    private string RenderTopRowFilter(TopRowFilter topRowFilter)
    {
        if (IsTrue(topRowFilter.WithApproximate))
        {
            throw new InvalidOperationException("Phase-1 emitter does not support approximate TOP row filters.");
        }

        var expression = RenderScalarExpression(GetOwnerLink(
            model.TopRowFilterExpressionLinkList,
            topRowFilter.Id,
            "TopRowFilter.Expression").Value);

        var builder = new StringBuilder();
        builder.Append("TOP ");
        builder.Append(expression);

        if (IsTrue(topRowFilter.Percent))
        {
            builder.Append(" PERCENT");
        }

        if (IsTrue(topRowFilter.WithTies))
        {
            builder.Append(" WITH TIES");
        }

        return builder.ToString();
    }

    private string RenderOffsetClause(OffsetClause offsetClause)
    {
        if (IsTrue(offsetClause.WithApproximate))
        {
            throw new InvalidOperationException("Phase-1 emitter does not support approximate OFFSET/FETCH clauses.");
        }

        var offsetExpression = RenderScalarExpression(GetOwnerLink(
            model.OffsetClauseOffsetExpressionLinkList,
            offsetClause.Id,
            "OffsetClause.OffsetExpression").Value);
        var rendered = "OFFSET " + offsetExpression + " ROWS";

        var fetchExpressionLink = FindOwnerLink(model.OffsetClauseFetchExpressionLinkList, offsetClause.Id);
        if (fetchExpressionLink is not null)
        {
            rendered += " FETCH NEXT " + RenderScalarExpression(fetchExpressionLink.Value) + " ROWS ONLY";
        }

        return rendered;
    }

    private string RenderWindowFrameClause(WindowFrameClause windowFrameClause)
    {
        var frameType = windowFrameClause.WindowFrameType switch
        {
            "Rows" => "ROWS",
            "Range" => "RANGE",
            _ => throw new InvalidOperationException($"Unsupported MetaTransformScript WindowFrameType '{windowFrameClause.WindowFrameType}'.")
        };

        var top = RenderWindowDelimiter(GetOwnerLink(model.WindowFrameClauseTopLinkList, windowFrameClause.Id, "WindowFrameClause.Top").Value);
        var bottomLink = FindOwnerLink(model.WindowFrameClauseBottomLinkList, windowFrameClause.Id);
        return bottomLink is null
            ? $"{frameType} {top}"
            : $"{frameType} BETWEEN {top} AND {RenderWindowDelimiter(bottomLink.Value)}";
    }

    private string RenderWindowDelimiter(WindowDelimiter windowDelimiter)
    {
        return windowDelimiter.WindowDelimiterType switch
        {
            "CurrentRow" => "CURRENT ROW",
            "UnboundedPreceding" => "UNBOUNDED PRECEDING",
            "UnboundedFollowing" => "UNBOUNDED FOLLOWING",
            _ => throw new InvalidOperationException($"Unsupported MetaTransformScript WindowDelimiterType '{windowDelimiter.WindowDelimiterType}'.")
        };
    }

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

    private string RenderSelectElement(SelectElement selectElement)
    {
        var selectStarExpression = FindByBaseId(model.SelectStarExpressionList, selectElement.Id);
        if (selectStarExpression is not null)
        {
            var qualifierLink = FindOwnerLink(model.SelectStarExpressionQualifierLinkList, selectStarExpression.Id);
            return qualifierLink is null
                ? "*"
                : RenderMultiPartIdentifier(qualifierLink.Value) + ".*";
        }

        var selectScalarExpression = FindByBaseId(model.SelectScalarExpressionList, selectElement.Id)
            ?? throw new InvalidOperationException($"Unsupported MetaTransformScript SelectElement id '{selectElement.Id}'.");

        var expression = RenderScalarExpression(GetOwnerLink(
            model.SelectScalarExpressionExpressionLinkList,
            selectScalarExpression.Id,
            "SelectScalarExpression.Expression").Value);

        var columnNameLink = FindOwnerLink(model.SelectScalarExpressionColumnNameLinkList, selectScalarExpression.Id);
        if (columnNameLink is not null)
        {
            return $"{expression} AS {RenderIdentifierOrValueExpression(columnNameLink.Value)}";
        }

        return expression;
    }

    private string RenderRowValue(RowValue rowValue)
    {
        var values = GetOrderedItems(model.RowValueColumnValuesItemList, rowValue.Id)
            .Select(row => RenderScalarExpression(row.Value))
            .ToArray();
        return "(" + string.Join(", ", values) + ")";
    }

    private string RenderTableSampleClause(TableSampleClause tableSampleClause)
    {
        var sampleNumber = RenderScalarExpression(GetOwnerLink(
            model.TableSampleClauseSampleNumberLinkList,
            tableSampleClause.Id,
            "TableSampleClause.SampleNumber").Value);
        var option = tableSampleClause.TableSampleClauseOption switch
        {
            "Percent" => "PERCENT",
            "Rows" => "ROWS",
            _ => throw new InvalidOperationException(
                $"Unsupported MetaTransformScript TableSampleClause.TableSampleClauseOption '{tableSampleClause.TableSampleClauseOption}'.")
        };

        var builder = new StringBuilder();
        builder.Append("TABLESAMPLE ");
        if (IsTrue(GetString(tableSampleClause, "System")))
        {
            builder.Append("SYSTEM ");
        }

        builder.Append('(');
        builder.Append(sampleNumber);
        builder.Append(' ');
        builder.Append(option);
        builder.Append(')');

        var repeatSeedLink = FindOwnerLink(model.TableSampleClauseRepeatSeedLinkList, tableSampleClause.Id);
        if (repeatSeedLink is not null)
        {
            builder.Append(" REPEATABLE (");
            builder.Append(RenderScalarExpression(repeatSeedLink.Value));
            builder.Append(')');
        }

        return builder.ToString();
    }

    private string RenderScalarExpression(ScalarExpression scalarExpression)
    {
        var binaryExpression = FindByBaseId(model.BinaryExpressionList, scalarExpression.Id);
        if (binaryExpression is not null)
        {
            var left = RenderScalarExpression(GetOwnerLink(model.BinaryExpressionFirstExpressionLinkList, binaryExpression.Id, "BinaryExpression.FirstExpression").Value);
            var right = RenderScalarExpression(GetOwnerLink(model.BinaryExpressionSecondExpressionLinkList, binaryExpression.Id, "BinaryExpression.SecondExpression").Value);
            var op = binaryExpression.BinaryExpressionType switch
            {
                "Add" => "+",
                _ => throw new InvalidOperationException($"Unsupported MetaTransformScript BinaryExpressionType '{binaryExpression.BinaryExpressionType}'.")
            };
            return $"{left} {op} {right}";
        }

        var unaryExpression = FindByBaseId(model.UnaryExpressionList, scalarExpression.Id);
        if (unaryExpression is not null)
        {
            var expression = RenderScalarExpression(GetOwnerLink(
                model.UnaryExpressionExpressionLinkList,
                unaryExpression.Id,
                "UnaryExpression.Expression").Value);
            var op = unaryExpression.UnaryExpressionType switch
            {
                "Negative" => "-",
                "Positive" => "+",
                _ => throw new InvalidOperationException(
                    $"Unsupported MetaTransformScript UnaryExpressionType '{unaryExpression.UnaryExpressionType}'.")
            };
            return op + expression;
        }

        var primaryExpression = FindByBaseId(model.PrimaryExpressionList, scalarExpression.Id)
            ?? throw new InvalidOperationException($"Unsupported MetaTransformScript ScalarExpression id '{scalarExpression.Id}'.");
        return RenderPrimaryExpression(primaryExpression);
    }

    private string RenderPrimaryExpression(PrimaryExpression primaryExpression)
    {
        string rendered;

        if (FindByBaseId(model.ColumnReferenceExpressionList, primaryExpression.Id) is { } columnReference)
        {
            rendered = RenderColumnReferenceExpression(columnReference);
        }
        else if (FindByBaseId(model.ParenthesisExpressionList, primaryExpression.Id) is { } parenthesisExpression)
        {
            rendered = "(" + RenderScalarExpression(GetOwnerLink(
                model.ParenthesisExpressionExpressionLinkList,
                parenthesisExpression.Id,
                "ParenthesisExpression.Expression").Value) + ")";
        }
        else if (FindByBaseId(model.CaseExpressionList, primaryExpression.Id) is { } caseExpression)
        {
            if (FindByBaseId(model.SimpleCaseExpressionList, caseExpression.Id) is { } simpleCaseExpression)
            {
                rendered = RenderSimpleCaseExpression(simpleCaseExpression);
            }
            else if (FindByBaseId(model.SearchedCaseExpressionList, caseExpression.Id) is { } searchedCaseExpression)
            {
                rendered = RenderSearchedCaseExpression(searchedCaseExpression);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported MetaTransformScript CaseExpression id '{caseExpression.Id}'.");
            }
        }
        else if (FindByBaseId(model.CoalesceExpressionList, primaryExpression.Id) is { } coalesceExpression)
        {
            rendered = RenderCoalesceExpression(coalesceExpression);
        }
        else if (FindByBaseId(model.NullIfExpressionList, primaryExpression.Id) is { } nullIfExpression)
        {
            rendered = RenderNullIfExpression(nullIfExpression);
        }
        else if (FindByBaseId(model.IIfCallList, primaryExpression.Id) is { } iIfCall)
        {
            rendered = RenderIIfCall(iIfCall);
        }
        else if (FindByBaseId(model.CastCallList, primaryExpression.Id) is { } castCall)
        {
            rendered = RenderCastCall(castCall);
        }
        else if (FindByBaseId(model.TryCastCallList, primaryExpression.Id) is { } tryCastCall)
        {
            rendered = RenderTryCastCall(tryCastCall);
        }
        else if (FindByBaseId(model.ConvertCallList, primaryExpression.Id) is { } convertCall)
        {
            rendered = RenderConvertCall(convertCall);
        }
        else if (FindByBaseId(model.TryConvertCallList, primaryExpression.Id) is { } tryConvertCall)
        {
            rendered = RenderTryConvertCall(tryConvertCall);
        }
        else if (FindByBaseId(model.LeftFunctionCallList, primaryExpression.Id) is { } leftFunctionCall)
        {
            rendered = RenderLeftFunctionCall(leftFunctionCall);
        }
        else if (FindByBaseId(model.RightFunctionCallList, primaryExpression.Id) is { } rightFunctionCall)
        {
            rendered = RenderRightFunctionCall(rightFunctionCall);
        }
        else if (FindByBaseId(model.ParseCallList, primaryExpression.Id) is { } parseCall)
        {
            rendered = RenderParseCall(parseCall);
        }
        else if (FindByBaseId(model.TryParseCallList, primaryExpression.Id) is { } tryParseCall)
        {
            rendered = RenderTryParseCall(tryParseCall);
        }
        else if (FindByBaseId(model.AtTimeZoneCallList, primaryExpression.Id) is { } atTimeZoneCall)
        {
            rendered = RenderAtTimeZoneCall(atTimeZoneCall);
        }
        else if (FindByBaseId(model.NextValueForExpressionList, primaryExpression.Id) is { } nextValueForExpression)
        {
            rendered = RenderNextValueForExpression(nextValueForExpression);
        }
        else if (FindByBaseId(model.ParameterlessCallList, primaryExpression.Id) is { } parameterlessCall)
        {
            rendered = RenderParameterlessCall(parameterlessCall);
        }
        else if (FindByBaseId(model.FunctionCallList, primaryExpression.Id) is { } functionCall)
        {
            rendered = RenderFunctionCall(functionCall);
        }
        else if (FindByBaseId(model.ScalarSubqueryList, primaryExpression.Id) is { } scalarSubquery)
        {
            rendered = RenderScalarSubquery(scalarSubquery);
        }
        else if (FindByBaseId(model.ValueExpressionList, primaryExpression.Id) is { } valueExpression)
        {
            rendered = RenderValueExpression(valueExpression);
        }
        else
        {
            throw new InvalidOperationException($"Unsupported MetaTransformScript PrimaryExpression id '{primaryExpression.Id}'.");
        }

        var collationLink = FindOwnerLink(model.PrimaryExpressionCollationLinkList, primaryExpression.Id);
        return collationLink is null
            ? rendered
            : rendered + " COLLATE " + RenderIdentifier(collationLink.Value);
    }

    private string RenderScalarSubquery(ScalarSubquery scalarSubquery)
    {
        var queryExpression = GetOwnerLink(
            model.ScalarSubqueryQueryExpressionLinkList,
            scalarSubquery.Id,
            "ScalarSubquery.QueryExpression").Value;
        return "(" + RenderQueryExpression(queryExpression) + ")";
    }

    private string RenderColumnReferenceExpression(ColumnReferenceExpression columnReference)
    {
        var multiPartIdentifierLink = FindOwnerLink(model.ColumnReferenceExpressionMultiPartIdentifierLinkList, columnReference.Id);
        if (multiPartIdentifierLink is not null)
        {
            return RenderMultiPartIdentifier(multiPartIdentifierLink.Value);
        }

        if (string.Equals(columnReference.ColumnType, "Wildcard", StringComparison.Ordinal))
        {
            return "*";
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript ColumnReferenceExpression id '{columnReference.Id}'.");
    }

    private string RenderValueExpression(ValueExpression valueExpression)
    {
        var literal = FindByBaseId(model.LiteralList, valueExpression.Id);
        if (literal is not null)
        {
            return RenderLiteral(literal);
        }

        var globalVariableExpression = FindByBaseId(model.GlobalVariableExpressionList, valueExpression.Id);
        if (globalVariableExpression is not null)
        {
            return globalVariableExpression.Name;
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript ValueExpression id '{valueExpression.Id}'.");
    }

    private string RenderSimpleCaseExpression(SimpleCaseExpression simpleCaseExpression)
    {
        var caseExpression = GetById(model.CaseExpressionList, simpleCaseExpression.BaseId, "SimpleCaseExpression.Base");
        var inputExpression = RenderScalarExpression(GetOwnerLink(
            model.SimpleCaseExpressionInputExpressionLinkList,
            simpleCaseExpression.Id,
            "SimpleCaseExpression.InputExpression").Value);
        var whenClauses = GetOrderedItems(model.SimpleCaseExpressionWhenClausesItemList, simpleCaseExpression.Id)
            .Select(row => RenderSimpleWhenClause(row.Value))
            .ToArray();
        return RenderCaseExpression("CASE " + inputExpression, whenClauses, caseExpression);
    }

    private string RenderSearchedCaseExpression(SearchedCaseExpression searchedCaseExpression)
    {
        var caseExpression = GetById(model.CaseExpressionList, searchedCaseExpression.BaseId, "SearchedCaseExpression.Base");
        var whenClauses = GetOrderedItems(model.SearchedCaseExpressionWhenClausesItemList, searchedCaseExpression.Id)
            .Select(row => RenderSearchedWhenClause(row.Value))
            .ToArray();
        return RenderCaseExpression("CASE", whenClauses, caseExpression);
    }

    private string RenderCaseExpression(string header, IReadOnlyList<string> whenClauses, CaseExpression caseExpression)
    {
        if (whenClauses.Count == 0)
        {
            throw new InvalidOperationException($"CaseExpression '{caseExpression.Id}' had no WHEN clauses.");
        }

        var builder = new StringBuilder();
        builder.AppendLine(header);

        foreach (var whenClause in whenClauses)
        {
            builder.Append("    ");
            builder.AppendLine(whenClause);
        }

        var elseExpressionLink = FindOwnerLink(model.CaseExpressionElseExpressionLinkList, caseExpression.Id);
        if (elseExpressionLink is not null)
        {
            builder.Append("    ELSE ");
            builder.AppendLine(RenderScalarExpression(elseExpressionLink.Value));
        }

        builder.Append("END");
        return builder.ToString();
    }

    private string RenderSimpleWhenClause(SimpleWhenClause simpleWhenClause)
    {
        var whenClause = GetById(model.WhenClauseList, simpleWhenClause.BaseId, "SimpleWhenClause.Base");
        var whenExpression = RenderScalarExpression(GetOwnerLink(
            model.SimpleWhenClauseWhenExpressionLinkList,
            simpleWhenClause.Id,
            "SimpleWhenClause.WhenExpression").Value);
        var thenExpression = RenderScalarExpression(GetOwnerLink(
            model.WhenClauseThenExpressionLinkList,
            whenClause.Id,
            "WhenClause.ThenExpression").Value);
        return $"WHEN {whenExpression} THEN {thenExpression}";
    }

    private string RenderSearchedWhenClause(SearchedWhenClause searchedWhenClause)
    {
        var whenClause = GetById(model.WhenClauseList, searchedWhenClause.BaseId, "SearchedWhenClause.Base");
        var whenExpression = RenderBooleanExpression(GetOwnerLink(
            model.SearchedWhenClauseWhenExpressionLinkList,
            searchedWhenClause.Id,
            "SearchedWhenClause.WhenExpression").Value);
        var thenExpression = RenderScalarExpression(GetOwnerLink(
            model.WhenClauseThenExpressionLinkList,
            whenClause.Id,
            "WhenClause.ThenExpression").Value);
        return $"WHEN {whenExpression} THEN {thenExpression}";
    }

    private string RenderCoalesceExpression(CoalesceExpression coalesceExpression)
    {
        var expressions = GetOrderedItems(model.CoalesceExpressionExpressionsItemList, coalesceExpression.Id)
            .Select(row => RenderScalarExpression(row.Value))
            .ToArray();
        return "COALESCE(" + string.Join(", ", expressions) + ")";
    }

    private string RenderNullIfExpression(NullIfExpression nullIfExpression)
    {
        var first = RenderScalarExpression(GetOwnerLink(
            model.NullIfExpressionFirstExpressionLinkList,
            nullIfExpression.Id,
            "NullIfExpression.FirstExpression").Value);
        var second = RenderScalarExpression(GetOwnerLink(
            model.NullIfExpressionSecondExpressionLinkList,
            nullIfExpression.Id,
            "NullIfExpression.SecondExpression").Value);
        return $"NULLIF({first}, {second})";
    }

    private string RenderIIfCall(IIfCall iIfCall)
    {
        var predicate = RenderBooleanExpression(GetOwnerLink(
            model.IIfCallPredicateLinkList,
            iIfCall.Id,
            "IIfCall.Predicate").Value);
        var thenExpression = RenderScalarExpression(GetOwnerLink(
            model.IIfCallThenExpressionLinkList,
            iIfCall.Id,
            "IIfCall.ThenExpression").Value);
        var elseExpression = RenderScalarExpression(GetOwnerLink(
            model.IIfCallElseExpressionLinkList,
            iIfCall.Id,
            "IIfCall.ElseExpression").Value);
        return $"IIF({predicate}, {thenExpression}, {elseExpression})";
    }

    private string RenderCastCall(CastCall castCall)
    {
        var parameter = RenderScalarExpression(GetOwnerLink(
            model.CastCallParameterLinkList,
            castCall.Id,
            "CastCall.Parameter").Value);
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.CastCallDataTypeLinkList,
            castCall.Id,
            "CastCall.DataType").Value);
        return $"CAST({parameter} AS {dataType})";
    }

    private string RenderTryCastCall(TryCastCall tryCastCall)
    {
        var parameter = RenderScalarExpression(GetOwnerLink(
            model.TryCastCallParameterLinkList,
            tryCastCall.Id,
            "TryCastCall.Parameter").Value);
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.TryCastCallDataTypeLinkList,
            tryCastCall.Id,
            "TryCastCall.DataType").Value);
        return $"TRY_CAST({parameter} AS {dataType})";
    }

    private string RenderConvertCall(ConvertCall convertCall)
    {
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.ConvertCallDataTypeLinkList,
            convertCall.Id,
            "ConvertCall.DataType").Value);
        var parameter = RenderScalarExpression(GetOwnerLink(
            model.ConvertCallParameterLinkList,
            convertCall.Id,
            "ConvertCall.Parameter").Value);
        var styleLink = FindOwnerLink(model.ConvertCallStyleLinkList, convertCall.Id);
        return styleLink is null
            ? $"CONVERT({dataType}, {parameter})"
            : $"CONVERT({dataType}, {parameter}, {RenderScalarExpression(styleLink.Value)})";
    }

    private string RenderTryConvertCall(TryConvertCall tryConvertCall)
    {
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.TryConvertCallDataTypeLinkList,
            tryConvertCall.Id,
            "TryConvertCall.DataType").Value);
        var parameter = RenderScalarExpression(GetOwnerLink(
            model.TryConvertCallParameterLinkList,
            tryConvertCall.Id,
            "TryConvertCall.Parameter").Value);
        var styleLink = FindOwnerLink(model.TryConvertCallStyleLinkList, tryConvertCall.Id);
        return styleLink is null
            ? $"TRY_CONVERT({dataType}, {parameter})"
            : $"TRY_CONVERT({dataType}, {parameter}, {RenderScalarExpression(styleLink.Value)})";
    }

    private string RenderParseCall(ParseCall parseCall)
    {
        var stringValue = RenderScalarExpression(GetOwnerLink(
            model.ParseCallStringValueLinkList,
            parseCall.Id,
            "ParseCall.StringValue").Value);
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.ParseCallDataTypeLinkList,
            parseCall.Id,
            "ParseCall.DataType").Value);
        var cultureLink = FindOwnerLink(model.ParseCallCultureLinkList, parseCall.Id);
        return cultureLink is null
            ? $"PARSE({stringValue} AS {dataType})"
            : $"PARSE({stringValue} AS {dataType} USING {RenderScalarExpression(cultureLink.Value)})";
    }

    private string RenderTryParseCall(TryParseCall tryParseCall)
    {
        var stringValue = RenderScalarExpression(GetOwnerLink(
            model.TryParseCallStringValueLinkList,
            tryParseCall.Id,
            "TryParseCall.StringValue").Value);
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.TryParseCallDataTypeLinkList,
            tryParseCall.Id,
            "TryParseCall.DataType").Value);
        var cultureLink = FindOwnerLink(model.TryParseCallCultureLinkList, tryParseCall.Id);
        return cultureLink is null
            ? $"TRY_PARSE({stringValue} AS {dataType})"
            : $"TRY_PARSE({stringValue} AS {dataType} USING {RenderScalarExpression(cultureLink.Value)})";
    }

    private string RenderLeftFunctionCall(LeftFunctionCall leftFunctionCall)
    {
        var parameters = GetOrderedItems(model.LeftFunctionCallParametersItemList, leftFunctionCall.Id)
            .Select(row => RenderScalarExpression(row.Value))
            .ToArray();
        return "LEFT(" + string.Join(", ", parameters) + ")";
    }

    private string RenderRightFunctionCall(RightFunctionCall rightFunctionCall)
    {
        var parameters = GetOrderedItems(model.RightFunctionCallParametersItemList, rightFunctionCall.Id)
            .Select(row => RenderScalarExpression(row.Value))
            .ToArray();
        return "RIGHT(" + string.Join(", ", parameters) + ")";
    }

    private string RenderAtTimeZoneCall(AtTimeZoneCall atTimeZoneCall)
    {
        var dateValue = RenderScalarExpression(GetOwnerLink(
            model.AtTimeZoneCallDateValueLinkList,
            atTimeZoneCall.Id,
            "AtTimeZoneCall.DateValue").Value);
        var timeZone = RenderScalarExpression(GetOwnerLink(
            model.AtTimeZoneCallTimeZoneLinkList,
            atTimeZoneCall.Id,
            "AtTimeZoneCall.TimeZone").Value);
        return $"{dateValue} AT TIME ZONE {timeZone}";
    }

    private string RenderNextValueForExpression(NextValueForExpression nextValueForExpression)
    {
        var sequenceName = RenderSchemaObjectName(GetOwnerLink(
            model.NextValueForExpressionSequenceNameLinkList,
            nextValueForExpression.Id,
            "NextValueForExpression.SequenceName").Value);
        return $"NEXT VALUE FOR {sequenceName}";
    }

    private static string RenderParameterlessCall(ParameterlessCall parameterlessCall)
    {
        return parameterlessCall.ParameterlessCallType switch
        {
            "CurrentTimestamp" => "CURRENT_TIMESTAMP",
            _ => throw new InvalidOperationException(
                $"Unsupported MetaTransformScript ParameterlessCallType '{parameterlessCall.ParameterlessCallType}'.")
        };
    }

    private string RenderDataTypeReference(DataTypeReference dataTypeReference)
    {
        var renderedName = FindOwnerLink(model.DataTypeReferenceNameLinkList, dataTypeReference.Id) is { } nameLink
            ? RenderSchemaObjectName(nameLink.Value)
            : FindByBaseId(model.ParameterizedDataTypeReferenceList, dataTypeReference.Id) is { } parameterizedForName
                && FindByBaseId(model.SqlDataTypeReferenceList, parameterizedForName.Id) is { } sqlDataTypeReference
                    ? RenderSqlDataTypeOption(sqlDataTypeReference.SqlDataTypeOption)
                    : throw new InvalidOperationException($"Unsupported MetaTransformScript DataTypeReference id '{dataTypeReference.Id}'.");

        var parameterizedDataTypeReference = FindByBaseId(model.ParameterizedDataTypeReferenceList, dataTypeReference.Id);
        if (parameterizedDataTypeReference is null)
        {
            return renderedName;
        }

        var parameters = GetOrderedItems(model.ParameterizedDataTypeReferenceParametersItemList, parameterizedDataTypeReference.Id)
            .Select(row => RenderLiteral(row.Value))
            .ToArray();
        return parameters.Length == 0
            ? renderedName
            : renderedName + "(" + string.Join(", ", parameters) + ")";
    }

    private static string RenderSqlDataTypeOption(string sqlDataTypeOption)
    {
        return sqlDataTypeOption switch
        {
            "DateTime2" => "datetime2",
            "VarChar" => "varchar",
            _ => sqlDataTypeOption.ToLowerInvariant()
        };
    }

    private string RenderLiteral(Literal literal)
    {
        var stringLiteral = FindByBaseId(model.StringLiteralList, literal.Id);
        if (stringLiteral is not null)
        {
            var prefix = IsTrue(stringLiteral.IsNational) ? "N" : string.Empty;
            return prefix + "'" + (literal.Value ?? string.Empty).Replace("'", "''", StringComparison.Ordinal) + "'";
        }

        if (FindByBaseId(model.IntegerLiteralList, literal.Id) is not null)
        {
            return literal.Value;
        }

        if (FindByBaseId(model.NumericLiteralList, literal.Id) is not null)
        {
            return literal.Value;
        }

        if (FindByBaseId(model.RealLiteralList, literal.Id) is not null)
        {
            return literal.Value;
        }

        if (FindByBaseId(model.BinaryLiteralList, literal.Id) is not null)
        {
            return literal.Value;
        }

        if (FindByBaseId(model.NullLiteralList, literal.Id) is not null)
        {
            return "NULL";
        }

        if (FindByBaseId(model.MaxLiteralList, literal.Id) is not null)
        {
            return "max";
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript Literal type '{literal.LiteralType}'.");
    }

    private static string RenderComparisonOperator(string? comparisonType)
    {
        return comparisonType switch
        {
            "Equals" => "=",
            "GreaterThan" => ">",
            _ => throw new InvalidOperationException($"Unsupported MetaTransformScript ComparisonType '{comparisonType}'.")
        };
    }

    private string RenderFunctionCall(FunctionCall functionCall)
    {
        if (IsTrue(functionCall.WithArrayWrapper))
        {
            throw new InvalidOperationException("Phase-1 emitter does not support FunctionCall.WithArrayWrapper=true.");
        }

        if (!string.IsNullOrWhiteSpace(functionCall.UniqueRowFilter) &&
            !string.Equals(functionCall.UniqueRowFilter, "NotSpecified", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Phase-1 emitter does not support FunctionCall.UniqueRowFilter='{functionCall.UniqueRowFilter}'.");
        }

        var functionName = RenderIdentifier(GetOwnerLink(model.FunctionCallFunctionNameLinkList, functionCall.Id, "FunctionCall.FunctionName").Value);
        var args = GetOrderedItems(model.FunctionCallParametersItemList, functionCall.Id)
            .Select(row => RenderScalarExpression(row.Value))
            .ToArray();

        var callTargetLink = FindOwnerLink(model.FunctionCallCallTargetLinkList, functionCall.Id);
        var overClauseLink = FindOwnerLink(model.FunctionCallOverClauseLinkList, functionCall.Id);
        if (callTargetLink is not null)
        {
            var renderedCall = $"{RenderCallTarget(callTargetLink.Value)}.{functionName}({string.Join(", ", args)})";
            return overClauseLink is null ? renderedCall : renderedCall + " " + RenderOverClause(overClauseLink.Value);
        }

        var rendered = $"{functionName}({string.Join(", ", args)})";
        return overClauseLink is null ? rendered : rendered + " " + RenderOverClause(overClauseLink.Value);
    }

    private string RenderCallTarget(CallTarget callTarget)
    {
        var multiPartCallTarget = FindByBaseId(model.MultiPartIdentifierCallTargetList, callTarget.Id)
            ?? throw new InvalidOperationException($"Unsupported MetaTransformScript CallTarget id '{callTarget.Id}'.");
        return RenderMultiPartIdentifier(GetOwnerLink(
            model.MultiPartIdentifierCallTargetMultiPartIdentifierLinkList,
            multiPartCallTarget.Id,
            "MultiPartIdentifierCallTarget.MultiPartIdentifier").Value);
    }

    private string RenderBooleanExpression(BooleanExpression booleanExpression)
    {
        var booleanBinary = FindByBaseId(model.BooleanBinaryExpressionList, booleanExpression.Id);
        if (booleanBinary is not null)
        {
            var left = RenderBooleanExpression(GetOwnerLink(model.BooleanBinaryExpressionFirstExpressionLinkList, booleanBinary.Id, "BooleanBinaryExpression.FirstExpression").Value);
            var right = RenderBooleanExpression(GetOwnerLink(model.BooleanBinaryExpressionSecondExpressionLinkList, booleanBinary.Id, "BooleanBinaryExpression.SecondExpression").Value);
            var op = booleanBinary.BinaryExpressionType switch
            {
                "And" => "AND",
                "Or" => "OR",
                _ => throw new InvalidOperationException($"Unsupported MetaTransformScript BooleanBinaryExpressionType '{booleanBinary.BinaryExpressionType}'.")
            };
            return $"{left} {op} {right}";
        }

        var booleanComparison = FindByBaseId(model.BooleanComparisonExpressionList, booleanExpression.Id);
        if (booleanComparison is not null)
        {
            var left = RenderScalarExpression(GetOwnerLink(model.BooleanComparisonExpressionFirstExpressionLinkList, booleanComparison.Id, "BooleanComparisonExpression.FirstExpression").Value);
            var right = RenderScalarExpression(GetOwnerLink(model.BooleanComparisonExpressionSecondExpressionLinkList, booleanComparison.Id, "BooleanComparisonExpression.SecondExpression").Value);
            var op = RenderComparisonOperator(booleanComparison.ComparisonType);
            return $"{left} {op} {right}";
        }

        var subqueryComparisonPredicate = FindByBaseId(model.SubqueryComparisonPredicateList, booleanExpression.Id);
        if (subqueryComparisonPredicate is not null)
        {
            var expression = RenderScalarExpression(GetOwnerLink(
                model.SubqueryComparisonPredicateExpressionLinkList,
                subqueryComparisonPredicate.Id,
                "SubqueryComparisonPredicate.Expression").Value);
            var subquery = RenderScalarSubquery(GetOwnerLink(
                model.SubqueryComparisonPredicateSubqueryLinkList,
                subqueryComparisonPredicate.Id,
                "SubqueryComparisonPredicate.Subquery").Value);
            var comparison = RenderComparisonOperator(subqueryComparisonPredicate.ComparisonType);
            var quantifier = subqueryComparisonPredicate.SubqueryComparisonPredicateType switch
            {
                "All" => "ALL",
                "Any" => "ANY",
                _ => throw new InvalidOperationException(
                    $"Unsupported MetaTransformScript SubqueryComparisonPredicateType '{subqueryComparisonPredicate.SubqueryComparisonPredicateType}'.")
            };
            return $"{expression} {comparison} {quantifier} {subquery}";
        }

        var booleanNot = FindByBaseId(model.BooleanNotExpressionList, booleanExpression.Id);
        if (booleanNot is not null)
        {
            var child = GetOwnerLink(model.BooleanNotExpressionExpressionLinkList, booleanNot.Id, "BooleanNotExpression.Expression").Value;
            return FindByBaseId(model.BooleanParenthesisExpressionList, child.Id) is not null
                ? $"NOT {RenderBooleanExpression(child)}"
                : $"NOT ({RenderBooleanExpression(child)})";
        }

        var booleanParenthesis = FindByBaseId(model.BooleanParenthesisExpressionList, booleanExpression.Id);
        if (booleanParenthesis is not null)
        {
            return $"({RenderBooleanExpression(GetOwnerLink(model.BooleanParenthesisExpressionExpressionLinkList, booleanParenthesis.Id, "BooleanParenthesisExpression.Expression").Value)})";
        }

        var booleanIsNull = FindByBaseId(model.BooleanIsNullExpressionList, booleanExpression.Id);
        if (booleanIsNull is not null)
        {
            var expression = RenderScalarExpression(GetOwnerLink(model.BooleanIsNullExpressionExpressionLinkList, booleanIsNull.Id, "BooleanIsNullExpression.Expression").Value);
            return $"{expression} IS {(IsTrue(booleanIsNull.IsNot) ? "NOT " : string.Empty)}NULL";
        }

        var booleanTernary = FindByBaseId(model.BooleanTernaryExpressionList, booleanExpression.Id);
        if (booleanTernary is not null)
        {
            if (!string.Equals(booleanTernary.TernaryExpressionType, "Between", StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Unsupported MetaTransformScript TernaryExpressionType '{booleanTernary.TernaryExpressionType}'.");
            }

            var first = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionFirstExpressionLinkList, booleanTernary.Id, "BooleanTernaryExpression.FirstExpression").Value);
            var second = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionSecondExpressionLinkList, booleanTernary.Id, "BooleanTernaryExpression.SecondExpression").Value);
            var third = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionThirdExpressionLinkList, booleanTernary.Id, "BooleanTernaryExpression.ThirdExpression").Value);
            return $"{first} BETWEEN {second} AND {third}";
        }

        var inPredicate = FindByBaseId(model.InPredicateList, booleanExpression.Id);
        if (inPredicate is not null)
        {
            var expression = RenderScalarExpression(GetOwnerLink(model.InPredicateExpressionLinkList, inPredicate.Id, "InPredicate.Expression").Value);
            var notText = IsTrue(inPredicate.NotDefined) ? " NOT" : string.Empty;
            var subqueryLink = FindOwnerLink(model.InPredicateSubqueryLinkList, inPredicate.Id);
            if (subqueryLink is not null)
            {
                return $"{expression}{notText} IN {RenderScalarSubquery(subqueryLink.Value)}";
            }

            var values = GetOrderedItems(model.InPredicateValuesItemList, inPredicate.Id)
                .Select(row => RenderScalarExpression(row.Value))
                .ToArray();
            return $"{expression}{notText} IN ({string.Join(", ", values)})";
        }

        var distinctPredicate = FindByBaseId(model.DistinctPredicateList, booleanExpression.Id);
        if (distinctPredicate is not null)
        {
            var first = RenderScalarExpression(GetOwnerLink(
                model.DistinctPredicateFirstExpressionLinkList,
                distinctPredicate.Id,
                "DistinctPredicate.FirstExpression").Value);
            var second = RenderScalarExpression(GetOwnerLink(
                model.DistinctPredicateSecondExpressionLinkList,
                distinctPredicate.Id,
                "DistinctPredicate.SecondExpression").Value);
            var notText = IsTrue(distinctPredicate.IsNot) ? " NOT" : string.Empty;
            return $"{first} IS{notText} DISTINCT FROM {second}";
        }

        var likePredicate = FindByBaseId(model.LikePredicateList, booleanExpression.Id);
        if (likePredicate is not null)
        {
            if (IsTrue(likePredicate.OdbcEscape))
            {
                throw new InvalidOperationException("Phase-1 emitter does not support OdbcEscape LIKE predicates.");
            }

            var first = RenderScalarExpression(GetOwnerLink(model.LikePredicateFirstExpressionLinkList, likePredicate.Id, "LikePredicate.FirstExpression").Value);
            var second = RenderScalarExpression(GetOwnerLink(model.LikePredicateSecondExpressionLinkList, likePredicate.Id, "LikePredicate.SecondExpression").Value);
            var notText = IsTrue(likePredicate.NotDefined) ? " NOT" : string.Empty;
            return $"{first}{notText} LIKE {second}";
        }

        var fullTextPredicate = FindByBaseId(model.FullTextPredicateList, booleanExpression.Id);
        if (fullTextPredicate is not null)
        {
            var functionName = fullTextPredicate.FullTextFunctionType switch
            {
                "Contains" => "CONTAINS",
                "FreeText" => "FREETEXT",
                _ => throw new InvalidOperationException(
                    $"Unsupported MetaTransformScript FullTextPredicate.FullTextFunctionType '{fullTextPredicate.FullTextFunctionType}'.")
            };
            var columns = GetOrderedItems(model.FullTextPredicateColumnsItemList, fullTextPredicate.Id)
                .Select(row => RenderColumnReferenceExpression(row.Value))
                .ToArray();
            var value = RenderValueExpression(GetOwnerLink(
                model.FullTextPredicateValueLinkList,
                fullTextPredicate.Id,
                "FullTextPredicate.Value").Value);
            return $"{functionName}({RenderFullTextColumns(columns)}, {value})";
        }

        var existsPredicate = FindByBaseId(model.ExistsPredicateList, booleanExpression.Id);
        if (existsPredicate is not null)
        {
            var subquery = RenderScalarSubquery(GetOwnerLink(
                model.ExistsPredicateSubqueryLinkList,
                existsPredicate.Id,
                "ExistsPredicate.Subquery").Value);
            return "EXISTS " + subquery;
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript BooleanExpression id '{booleanExpression.Id}'.");
    }

    private string RenderSchemaObjectName(SchemaObjectName schemaObjectName)
    {
        var parts = new List<string>();
        var schemaLink = FindOwnerLink(model.SchemaObjectNameSchemaIdentifierLinkList, schemaObjectName.Id);
        if (schemaLink is not null)
        {
            parts.Add(RenderIdentifier(schemaLink.Value));
        }

        parts.Add(RenderIdentifier(GetOwnerLink(model.SchemaObjectNameBaseIdentifierLinkList, schemaObjectName.Id, "SchemaObjectName.BaseIdentifier").Value));
        return string.Join(".", parts);
    }

    private string RenderIdentifierOrValueExpression(IdentifierOrValueExpression value)
    {
        var identifierLink = FindOwnerLink(model.IdentifierOrValueExpressionIdentifierLinkList, value.Id);
        if (identifierLink is not null)
        {
            return RenderIdentifier(identifierLink.Value);
        }

        if (!string.IsNullOrWhiteSpace(value.Value))
        {
            return value.Value;
        }

        throw new InvalidOperationException($"IdentifierOrValueExpression '{value.Id}' was empty.");
    }

    private string RenderMultiPartIdentifier(MultiPartIdentifier multiPartIdentifier)
    {
        var parts = GetOrderedItems(model.MultiPartIdentifierIdentifiersItemList, multiPartIdentifier.Id)
            .Select(row => RenderIdentifier(row.Value))
            .ToArray();
        return string.Join(".", parts);
    }

    private static string RenderFullTextColumns(IReadOnlyList<string> columns) =>
        columns.Count switch
        {
            0 => throw new InvalidOperationException("Full-text syntax requires at least one column."),
            1 => columns[0],
            _ => "(" + string.Join(", ", columns) + ")"
        };

    private string RenderXmlNamespacesElement(XmlNamespacesElement element)
    {
        var literal = GetOwnerLink(model.XmlNamespacesElementStringLinkList, element.Id, "XmlNamespacesElement.String").Value;
        var literalBase = GetById(model.LiteralList, literal.BaseId, "XmlNamespacesElement.StringLiteralBase");
        var renderedLiteral = RenderLiteral(literalBase);

        var aliasElement = FindByBaseId(model.XmlNamespacesAliasElementList, element.Id);
        if (aliasElement is not null)
        {
            var alias = RenderIdentifier(GetOwnerLink(model.XmlNamespacesAliasElementIdentifierLinkList, aliasElement.Id, "XmlNamespacesAliasElement.Identifier").Value);
            return $"{renderedLiteral} AS {alias}";
        }

        return renderedLiteral;
    }

    private string RenderAliasAndColumns(TableReferenceWithAliasAndColumns aliasAndColumns)
    {
        var aliasBase = GetById(model.TableReferenceWithAliasList, aliasAndColumns.BaseId, "TableReferenceWithAliasAndColumns.Base");
        var aliasLink = FindOwnerLink(model.TableReferenceWithAliasAliasLinkList, aliasBase.Id)
            ?? throw new InvalidOperationException(
                $"TableReferenceWithAliasAndColumns '{aliasAndColumns.Id}' was missing an alias.");

        var columns = GetOrderedItems(model.TableReferenceWithAliasAndColumnsColumnsItemList, aliasAndColumns.Id)
            .Select(row => RenderIdentifier(row.Value))
            .ToArray();

        return columns.Length == 0
            ? " AS " + RenderIdentifier(aliasLink.Value)
            : " AS " + RenderIdentifier(aliasLink.Value) + "(" + string.Join(", ", columns) + ")";
    }

    private static T GetById<T>(IEnumerable<T> rows, string id, string label) where T : class
    {
        var match = rows.FirstOrDefault(row => string.Equals(GetString(row, "Id"), id, StringComparison.Ordinal));
        return match ?? throw new InvalidOperationException($"Required MetaTransformScript lookup '{label}' with id '{id}' was not found.");
    }

    private static T GetByBaseId<T>(IEnumerable<T> rows, string baseId, string label) where T : class
    {
        var match = FindByBaseId(rows, baseId);
        return match ?? throw new InvalidOperationException($"Required MetaTransformScript base lookup '{label}' with base id '{baseId}' was not found.");
    }

    private static T? FindByBaseId<T>(IEnumerable<T> rows, string baseId) where T : class =>
        rows.FirstOrDefault(row => string.Equals(GetString(row, "BaseId"), baseId, StringComparison.Ordinal));

    private static TLink GetOwnerLink<TLink>(IEnumerable<TLink> links, string ownerId, string label) where TLink : class
    {
        var match = FindOwnerLink(links, ownerId);
        return match ?? throw new InvalidOperationException($"Required MetaTransformScript link '{label}' with owner id '{ownerId}' was not found.");
    }

    private static TLink? FindOwnerLink<TLink>(IEnumerable<TLink> links, string ownerId) where TLink : class =>
        links.FirstOrDefault(row => string.Equals(GetString(row, "OwnerId"), ownerId, StringComparison.Ordinal));

    private static IEnumerable<TItem> GetOrderedItems<TItem>(IEnumerable<TItem> items, string ownerId) where TItem : class =>
        items.Where(row => string.Equals(GetString(row, "OwnerId"), ownerId, StringComparison.Ordinal))
            .OrderBy(row => ParseOrdinal(GetString(row, "Ordinal")));

    private static string GetString(object target, string propertyName) =>
        (string?)target.GetType().GetProperty(propertyName)?.GetValue(target) ?? string.Empty;

    private static int ParseOrdinal(string ordinal) =>
        int.TryParse(ordinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : 0;

    private static bool IsTrue(string value) =>
        string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

    private static string RenderIdentifier(Identifier identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier.Value))
        {
            return "[]";
        }

        if (string.Equals(identifier.QuoteType, "SquareBracket", StringComparison.Ordinal))
        {
            return "[" + identifier.Value.Replace("]", "]]", StringComparison.Ordinal) + "]";
        }

        if (!string.IsNullOrWhiteSpace(identifier.QuoteType) &&
            !string.Equals(identifier.QuoteType, "NotQuoted", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Unsupported MetaTransformScript Identifier.QuoteType '{identifier.QuoteType}'.");
        }

        if (IsPlainIdentifier(identifier.Value))
        {
            return identifier.Value;
        }

        return "[" + identifier.Value.Replace("]", "]]", StringComparison.Ordinal) + "]";
    }

    private static bool IsPlainIdentifier(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (!(char.IsLetter(value[0]) || value[0] == '_' || value[0] == '@' || value[0] == '#'))
        {
            return false;
        }

        for (var i = 1; i < value.Length; i++)
        {
            var ch = value[i];
            if (!(char.IsLetterOrDigit(ch) || ch == '_' || ch == '@' || ch == '#' || ch == '$'))
            {
                return false;
            }
        }

        return true;
    }
}
