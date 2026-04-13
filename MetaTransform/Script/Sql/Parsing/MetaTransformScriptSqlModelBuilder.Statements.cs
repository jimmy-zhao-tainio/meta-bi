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
            CommonTableExpressionId = commonTableExpression.Id,
            IdentifierId = expressionName.GetId(nameof(Identifier))
        });
        model.CommonTableExpressionQueryExpressionLinkList.Add(new CommonTableExpressionQueryExpressionLink
        {
            Id = NextId(nameof(CommonTableExpressionQueryExpressionLink)),
            CommonTableExpressionId = commonTableExpression.Id,
            QueryExpressionId = queryExpression.GetId(nameof(QueryExpression))
        });

        for (var ordinal = 0; columns is not null && ordinal < columns.Count; ordinal++)
        {
            model.CommonTableExpressionColumnsItemList.Add(new CommonTableExpressionColumnsItem
            {
                Id = NextId(nameof(CommonTableExpressionColumnsItem)),
                CommonTableExpressionId = commonTableExpression.Id,
                IdentifierId = columns[ordinal].GetId(nameof(Identifier)),
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
            TSqlStatementId = sqlStatement.Id
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
                StatementWithCtesAndXmlNamespacesId = statementWithCtes.Id,
                WithCtesAndXmlNamespacesId = withCtesAndXmlNamespaces.Id
            });

            if (xmlNamespaces is not null)
            {
                model.WithCtesAndXmlNamespacesXmlNamespacesLinkList.Add(new WithCtesAndXmlNamespacesXmlNamespacesLink
                {
                    Id = NextId(nameof(WithCtesAndXmlNamespacesXmlNamespacesLink)),
                    WithCtesAndXmlNamespacesId = withCtesAndXmlNamespaces.Id,
                    XmlNamespacesId = xmlNamespaces.GetId(nameof(XmlNamespaces))
                });
            }

            for (var ordinal = 0; commonTableExpressions is not null && ordinal < commonTableExpressions.Count; ordinal++)
            {
                model.WithCtesAndXmlNamespacesCommonTableExpressionsItemList.Add(new WithCtesAndXmlNamespacesCommonTableExpressionsItem
                {
                    Id = NextId(nameof(WithCtesAndXmlNamespacesCommonTableExpressionsItem)),
                    WithCtesAndXmlNamespacesId = withCtesAndXmlNamespaces.Id,
                    CommonTableExpressionId = commonTableExpressions[ordinal].GetId(nameof(CommonTableExpression)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        var selectStatement = new SelectStatement
        {
            Id = NextId(nameof(SelectStatement)),
            StatementWithCtesAndXmlNamespacesId = statementWithCtes.Id
        };
        model.SelectStatementList.Add(selectStatement);
        model.SelectStatementQueryExpressionLinkList.Add(new SelectStatementQueryExpressionLink
        {
            Id = NextId(nameof(SelectStatementQueryExpressionLink)),
            SelectStatementId = selectStatement.Id,
            QueryExpressionId = queryExpression.GetId(nameof(QueryExpression))
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
            QueryExpressionId = queryExpression.Id,
            BinaryQueryExpressionType = binaryQueryExpressionType,
            All = all ? "true" : string.Empty
        };
        model.BinaryQueryExpressionList.Add(binaryQueryExpression);
        model.BinaryQueryExpressionFirstQueryExpressionLinkList.Add(new BinaryQueryExpressionFirstQueryExpressionLink
        {
            Id = NextId(nameof(BinaryQueryExpressionFirstQueryExpressionLink)),
            BinaryQueryExpressionId = binaryQueryExpression.Id,
            QueryExpressionId = firstQueryExpression.GetId(nameof(QueryExpression))
        });
        model.BinaryQueryExpressionSecondQueryExpressionLinkList.Add(new BinaryQueryExpressionSecondQueryExpressionLink
        {
            Id = NextId(nameof(BinaryQueryExpressionSecondQueryExpressionLink)),
            BinaryQueryExpressionId = binaryQueryExpression.Id,
            QueryExpressionId = secondQueryExpression.GetId(nameof(QueryExpression))
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
            QueryExpressionId = parent.Id
        };
        model.QueryParenthesisExpressionList.Add(queryParenthesisExpression);
        model.QueryParenthesisExpressionQueryExpressionLinkList.Add(new QueryParenthesisExpressionQueryExpressionLink
        {
            Id = NextId(nameof(QueryParenthesisExpressionQueryExpressionLink)),
            QueryParenthesisExpressionId = queryParenthesisExpression.Id,
            QueryExpressionId = queryExpression.GetId(nameof(QueryExpression))
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
            QueryExpressionId = queryExpression.GetId(nameof(QueryExpression)),
            OrderByClauseId = orderByClause.GetId(nameof(OrderByClause))
        });

        return queryExpression;
    }

    public void AddTransformScript(
        string name,
        string targetSqlIdentifier,
        string? sourcePath,
        BuiltNode selectStatement,
        BuiltNode? schemaIdentifier,
        BuiltNode? objectIdentifier,
        IReadOnlyList<BuiltNode>? viewColumns = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Transform script name cannot be empty.");
        }

        if (model.TransformScriptList.Any(existing => string.Equals(existing.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Transform script '{name}' already exists in this workspace.");
        }

        if (string.IsNullOrWhiteSpace(targetSqlIdentifier))
        {
            throw new InvalidOperationException("Transform script target SQL identifier cannot be empty.");
        }

        var row = new TransformScript
        {
            Id = NextId(nameof(TransformScript)),
            Name = name,
            TargetSqlIdentifier = targetSqlIdentifier,
            SourcePath = sourcePath ?? string.Empty
        };
        model.TransformScriptList.Add(row);
        model.TransformScriptSelectStatementLinkList.Add(new TransformScriptSelectStatementLink
        {
            Id = NextId(nameof(TransformScriptSelectStatementLink)),
            TransformScriptId = row.Id,
            SelectStatementId = selectStatement.GetId(nameof(SelectStatement))
        });

        if (schemaIdentifier is not null)
        {
            model.TransformScriptSchemaIdentifierLinkList.Add(new TransformScriptSchemaIdentifierLink
            {
                Id = NextId(nameof(TransformScriptSchemaIdentifierLink)),
                TransformScriptId = row.Id,
                IdentifierId = schemaIdentifier.GetId(nameof(Identifier))
            });
        }

        if (objectIdentifier is not null)
        {
            model.TransformScriptObjectIdentifierLinkList.Add(new TransformScriptObjectIdentifierLink
            {
                Id = NextId(nameof(TransformScriptObjectIdentifierLink)),
                TransformScriptId = row.Id,
                IdentifierId = objectIdentifier.GetId(nameof(Identifier))
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
                TransformScriptId = row.Id,
                IdentifierId = viewColumns[ordinal].GetId(nameof(Identifier)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }
    }
}
