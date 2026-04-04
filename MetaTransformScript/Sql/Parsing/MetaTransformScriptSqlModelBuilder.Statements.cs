using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    public BuiltNode CreateCommonTableExpression(
        BuiltNode expressionName,
        BuiltNode queryExpression,
        IReadOnlyList<BuiltNode>? columns = null)
    {
        var commonTableExpression = new CommonTableExpression
        {
            Id = NextId(nameof(CommonTableExpression))
        };
        model.CommonTableExpressionList.Add(commonTableExpression);
        model.CommonTableExpressionExpressionNameLinkList.Add(new CommonTableExpressionExpressionNameLink
        {
            Id = NextId(nameof(CommonTableExpressionExpressionNameLink)),
            OwnerId = commonTableExpression.Id,
            ValueId = expressionName.GetId(nameof(Identifier))
        });
        model.CommonTableExpressionQueryExpressionLinkList.Add(new CommonTableExpressionQueryExpressionLink
        {
            Id = NextId(nameof(CommonTableExpressionQueryExpressionLink)),
            OwnerId = commonTableExpression.Id,
            ValueId = queryExpression.GetId(nameof(QueryExpression))
        });

        for (var ordinal = 0; columns is not null && ordinal < columns.Count; ordinal++)
        {
            model.CommonTableExpressionColumnsItemList.Add(new CommonTableExpressionColumnsItem
            {
                Id = NextId(nameof(CommonTableExpressionColumnsItem)),
                OwnerId = commonTableExpression.Id,
                ValueId = columns[ordinal].GetId(nameof(Identifier)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create((nameof(CommonTableExpression), commonTableExpression.Id));
    }

    public BuiltNode CreateSelectStatement(
        BuiltNode queryExpression,
        IReadOnlyList<BuiltNode>? commonTableExpressions = null,
        BuiltNode? xmlNamespaces = null)
    {
        var sqlStatement = new TSqlStatement
        {
            Id = NextId(nameof(TSqlStatement))
        };
        model.TSqlStatementList.Add(sqlStatement);

        var statementWithCtes = new StatementWithCtesAndXmlNamespaces
        {
            Id = NextId(nameof(StatementWithCtesAndXmlNamespaces)),
            BaseId = sqlStatement.Id
        };
        model.StatementWithCtesAndXmlNamespacesList.Add(statementWithCtes);

        if (xmlNamespaces is not null || (commonTableExpressions is not null && commonTableExpressions.Count > 0))
        {
            var withCtesAndXmlNamespaces = new WithCtesAndXmlNamespaces
            {
                Id = NextId(nameof(WithCtesAndXmlNamespaces))
            };
            model.WithCtesAndXmlNamespacesList.Add(withCtesAndXmlNamespaces);
            model.StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLinkList.Add(new StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink
            {
                Id = NextId(nameof(StatementWithCtesAndXmlNamespacesWithCtesAndXmlNamespacesLink)),
                OwnerId = statementWithCtes.Id,
                ValueId = withCtesAndXmlNamespaces.Id
            });

            if (xmlNamespaces is not null)
            {
                model.WithCtesAndXmlNamespacesXmlNamespacesLinkList.Add(new WithCtesAndXmlNamespacesXmlNamespacesLink
                {
                    Id = NextId(nameof(WithCtesAndXmlNamespacesXmlNamespacesLink)),
                    OwnerId = withCtesAndXmlNamespaces.Id,
                    ValueId = xmlNamespaces.GetId(nameof(XmlNamespaces))
                });
            }

            for (var ordinal = 0; commonTableExpressions is not null && ordinal < commonTableExpressions.Count; ordinal++)
            {
                model.WithCtesAndXmlNamespacesCommonTableExpressionsItemList.Add(new WithCtesAndXmlNamespacesCommonTableExpressionsItem
                {
                    Id = NextId(nameof(WithCtesAndXmlNamespacesCommonTableExpressionsItem)),
                    OwnerId = withCtesAndXmlNamespaces.Id,
                    ValueId = commonTableExpressions[ordinal].GetId(nameof(CommonTableExpression)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        var selectStatement = new SelectStatement
        {
            Id = NextId(nameof(SelectStatement)),
            BaseId = statementWithCtes.Id
        };
        model.SelectStatementList.Add(selectStatement);
        model.SelectStatementQueryExpressionLinkList.Add(new SelectStatementQueryExpressionLink
        {
            Id = NextId(nameof(SelectStatementQueryExpressionLink)),
            OwnerId = selectStatement.Id,
            ValueId = queryExpression.GetId(nameof(QueryExpression))
        });

        return BuiltNode.Create(
            (nameof(TSqlStatement), sqlStatement.Id),
            (nameof(StatementWithCtesAndXmlNamespaces), statementWithCtes.Id),
            (nameof(SelectStatement), selectStatement.Id));
    }

    public BuiltNode CreateBinaryQueryExpression(
        BuiltNode firstQueryExpression,
        BuiltNode secondQueryExpression,
        string binaryQueryExpressionType,
        bool all)
    {
        var queryExpression = new QueryExpression
        {
            Id = NextId(nameof(QueryExpression))
        };
        model.QueryExpressionList.Add(queryExpression);

        var binaryQueryExpression = new BinaryQueryExpression
        {
            Id = NextId(nameof(BinaryQueryExpression)),
            BaseId = queryExpression.Id,
            BinaryQueryExpressionType = binaryQueryExpressionType,
            All = all ? "true" : string.Empty
        };
        model.BinaryQueryExpressionList.Add(binaryQueryExpression);
        model.BinaryQueryExpressionFirstQueryExpressionLinkList.Add(new BinaryQueryExpressionFirstQueryExpressionLink
        {
            Id = NextId(nameof(BinaryQueryExpressionFirstQueryExpressionLink)),
            OwnerId = binaryQueryExpression.Id,
            ValueId = firstQueryExpression.GetId(nameof(QueryExpression))
        });
        model.BinaryQueryExpressionSecondQueryExpressionLinkList.Add(new BinaryQueryExpressionSecondQueryExpressionLink
        {
            Id = NextId(nameof(BinaryQueryExpressionSecondQueryExpressionLink)),
            OwnerId = binaryQueryExpression.Id,
            ValueId = secondQueryExpression.GetId(nameof(QueryExpression))
        });

        return BuiltNode.Create(
            (nameof(QueryExpression), queryExpression.Id),
            (nameof(BinaryQueryExpression), binaryQueryExpression.Id));
    }

    public BuiltNode CreateQueryParenthesisExpression(BuiltNode queryExpression)
    {
        var parent = new QueryExpression
        {
            Id = NextId(nameof(QueryExpression))
        };
        model.QueryExpressionList.Add(parent);

        var queryParenthesisExpression = new QueryParenthesisExpression
        {
            Id = NextId(nameof(QueryParenthesisExpression)),
            BaseId = parent.Id
        };
        model.QueryParenthesisExpressionList.Add(queryParenthesisExpression);
        model.QueryParenthesisExpressionQueryExpressionLinkList.Add(new QueryParenthesisExpressionQueryExpressionLink
        {
            Id = NextId(nameof(QueryParenthesisExpressionQueryExpressionLink)),
            OwnerId = queryParenthesisExpression.Id,
            ValueId = queryExpression.GetId(nameof(QueryExpression))
        });

        return BuiltNode.Create(
            (nameof(QueryExpression), parent.Id),
            (nameof(QueryParenthesisExpression), queryParenthesisExpression.Id));
    }

    public BuiltNode AttachOrderByClause(BuiltNode queryExpression, BuiltNode orderByClause)
    {
        model.QueryExpressionOrderByClauseLinkList.Add(new QueryExpressionOrderByClauseLink
        {
            Id = NextId(nameof(QueryExpressionOrderByClauseLink)),
            OwnerId = queryExpression.GetId(nameof(QueryExpression)),
            ValueId = orderByClause.GetId(nameof(OrderByClause))
        });

        return queryExpression;
    }

    public void AddTransformScript(
        string name,
        string? sourcePath,
        BuiltNode selectStatement,
        BuiltNode? schemaIdentifier,
        BuiltNode? objectIdentifier,
        IReadOnlyList<BuiltNode>? viewColumns = null)
    {
        var row = new TransformScript
        {
            Id = NextId(nameof(TransformScript)),
            Name = name,
            SourcePath = sourcePath ?? string.Empty
        };
        model.TransformScriptList.Add(row);
        model.TransformScriptSelectStatementLinkList.Add(new TransformScriptSelectStatementLink
        {
            Id = NextId(nameof(TransformScriptSelectStatementLink)),
            OwnerId = row.Id,
            ValueId = selectStatement.GetId(nameof(SelectStatement))
        });

        if (schemaIdentifier is not null)
        {
            model.TransformScriptSchemaIdentifierLinkList.Add(new TransformScriptSchemaIdentifierLink
            {
                Id = NextId(nameof(TransformScriptSchemaIdentifierLink)),
                OwnerId = row.Id,
                ValueId = schemaIdentifier.GetId(nameof(Identifier))
            });
        }

        if (objectIdentifier is not null)
        {
            model.TransformScriptObjectIdentifierLinkList.Add(new TransformScriptObjectIdentifierLink
            {
                Id = NextId(nameof(TransformScriptObjectIdentifierLink)),
                OwnerId = row.Id,
                ValueId = objectIdentifier.GetId(nameof(Identifier))
            });
        }

        if (viewColumns is null)
        {
            return;
        }

        for (var ordinal = 0; ordinal < viewColumns.Count; ordinal++)
        {
            model.TransformScriptViewColumnsItemList.Add(new TransformScriptViewColumnsItem
            {
                Id = NextId(nameof(TransformScriptViewColumnsItem)),
                OwnerId = row.Id,
                ValueId = viewColumns[ordinal].GetId(nameof(Identifier)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }
    }
}
