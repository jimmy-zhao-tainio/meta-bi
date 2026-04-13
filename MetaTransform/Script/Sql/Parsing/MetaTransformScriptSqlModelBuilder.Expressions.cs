using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    public BuiltNode CreateBinaryExpression(BuiltNode firstExpression, BuiltNode secondExpression, string binaryExpressionType)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var binaryExpression = new BinaryExpression
        {
            Id = NextId(nameof(BinaryExpression)),
            ScalarExpressionId = scalar.Id,
            BinaryExpressionType = binaryExpressionType
        };
        model.BinaryExpressionList.Add(binaryExpression);
        model.BinaryExpressionFirstExpressionLinkList.Add(new BinaryExpressionFirstExpressionLink
        {
            Id = NextId(nameof(BinaryExpressionFirstExpressionLink)),
            BinaryExpressionId = binaryExpression.Id,
            ScalarExpressionId = firstExpression.GetId(nameof(ScalarExpression))
        });
        model.BinaryExpressionSecondExpressionLinkList.Add(new BinaryExpressionSecondExpressionLink
        {
            Id = NextId(nameof(BinaryExpressionSecondExpressionLink)),
            BinaryExpressionId = binaryExpression.Id,
            ScalarExpressionId = secondExpression.GetId(nameof(ScalarExpression))
        });

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(BinaryExpression), binaryExpression.Id));
    }

    public BuiltNode CreateColumnReferenceExpression(BuiltNode multiPartIdentifier, string columnType = "Regular")
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var columnReference = new ColumnReferenceExpression
        {
            Id = NextId(nameof(ColumnReferenceExpression)),
            PrimaryExpressionId = primary.Id,
            ColumnType = columnType
        };
        model.ColumnReferenceExpressionList.Add(columnReference);
        model.ColumnReferenceExpressionMultiPartIdentifierLinkList.Add(new ColumnReferenceExpressionMultiPartIdentifierLink
        {
            Id = NextId(nameof(ColumnReferenceExpressionMultiPartIdentifierLink)),
            ColumnReferenceExpressionId = columnReference.Id,
            MultiPartIdentifierId = multiPartIdentifier.GetId(nameof(MultiPartIdentifier))
        });

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(ColumnReferenceExpression), columnReference.Id));
    }

    public BuiltNode CreateWildcardColumnReferenceExpression()
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var columnReference = new ColumnReferenceExpression
        {
            Id = NextId(nameof(ColumnReferenceExpression)),
            PrimaryExpressionId = primary.Id,
            ColumnType = "Wildcard"
        };
        model.ColumnReferenceExpressionList.Add(columnReference);

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(ColumnReferenceExpression), columnReference.Id));
    }

    public BuiltNode CreateParenthesisExpression(BuiltNode expression)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var parenthesisExpression = new ParenthesisExpression
        {
            Id = NextId(nameof(ParenthesisExpression)),
            PrimaryExpressionId = primary.Id
        };
        model.ParenthesisExpressionList.Add(parenthesisExpression);
        model.ParenthesisExpressionExpressionLinkList.Add(new ParenthesisExpressionExpressionLink
        {
            Id = NextId(nameof(ParenthesisExpressionExpressionLink)),
            ParenthesisExpressionId = parenthesisExpression.Id,
            ScalarExpressionId = expression.GetId(nameof(ScalarExpression))
        });

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(ParenthesisExpression), parenthesisExpression.Id));
    }

    public BuiltNode CreateFunctionCall(BuiltNode functionName, IReadOnlyList<BuiltNode> parameters, string? uniqueRowFilter = null)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var functionCall = new FunctionCall
        {
            Id = NextId(nameof(FunctionCall)),
            PrimaryExpressionId = primary.Id,
            UniqueRowFilter = uniqueRowFilter ?? string.Empty
        };
        model.FunctionCallList.Add(functionCall);
        model.FunctionCallFunctionNameLinkList.Add(new FunctionCallFunctionNameLink
        {
            Id = NextId(nameof(FunctionCallFunctionNameLink)),
            FunctionCallId = functionCall.Id,
            IdentifierId = functionName.GetId(nameof(Identifier))
        });

        for (var ordinal = 0; ordinal < parameters.Count; ordinal++)
        {
            model.FunctionCallParametersItemList.Add(new FunctionCallParametersItem
            {
                Id = NextId(nameof(FunctionCallParametersItem)),
                FunctionCallId = functionCall.Id,
                ScalarExpressionId = parameters[ordinal].GetId(nameof(ScalarExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(FunctionCall), functionCall.Id));
    }

    public BuiltNode CreateScalarSubquery(BuiltNode queryExpression)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var scalarSubquery = new ScalarSubquery
        {
            Id = NextId(nameof(ScalarSubquery)),
            PrimaryExpressionId = primary.Id
        };
        model.ScalarSubqueryList.Add(scalarSubquery);
        model.ScalarSubqueryQueryExpressionLinkList.Add(new ScalarSubqueryQueryExpressionLink
        {
            Id = NextId(nameof(ScalarSubqueryQueryExpressionLink)),
            ScalarSubqueryId = scalarSubquery.Id,
            QueryExpressionId = queryExpression.GetId(nameof(QueryExpression))
        });

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(ScalarSubquery), scalarSubquery.Id));
    }

    public BuiltNode CreateSearchedCaseExpression(
        IReadOnlyList<(BuiltNode WhenExpression, BuiltNode ThenExpression)> whenClauses,
        BuiltNode? elseExpression)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpressionId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var caseExpression = new CaseExpression
        {
            Id = NextId(nameof(CaseExpression)),
            PrimaryExpressionId = primary.Id
        };
        model.CaseExpressionList.Add(caseExpression);

        var searchedCaseExpression = new SearchedCaseExpression
        {
            Id = NextId(nameof(SearchedCaseExpression)),
            CaseExpressionId = caseExpression.Id
        };
        model.SearchedCaseExpressionList.Add(searchedCaseExpression);

        for (var ordinal = 0; ordinal < whenClauses.Count; ordinal++)
        {
            var whenClause = new WhenClause
            {
                Id = NextId(nameof(WhenClause))
            };
            model.WhenClauseList.Add(whenClause);

            var searchedWhenClause = new SearchedWhenClause
            {
                Id = NextId(nameof(SearchedWhenClause)),
                WhenClauseId = whenClause.Id
            };
            model.SearchedWhenClauseList.Add(searchedWhenClause);

            model.SearchedCaseExpressionWhenClausesItemList.Add(new SearchedCaseExpressionWhenClausesItem
            {
                Id = NextId(nameof(SearchedCaseExpressionWhenClausesItem)),
                SearchedCaseExpressionId = searchedCaseExpression.Id,
                SearchedWhenClauseId = searchedWhenClause.Id,
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
            model.SearchedWhenClauseWhenExpressionLinkList.Add(new SearchedWhenClauseWhenExpressionLink
            {
                Id = NextId(nameof(SearchedWhenClauseWhenExpressionLink)),
                SearchedWhenClauseId = searchedWhenClause.Id,
                BooleanExpressionId = whenClauses[ordinal].WhenExpression.GetId(nameof(BooleanExpression))
            });
            model.WhenClauseThenExpressionLinkList.Add(new WhenClauseThenExpressionLink
            {
                Id = NextId(nameof(WhenClauseThenExpressionLink)),
                WhenClauseId = whenClause.Id,
                ScalarExpressionId = whenClauses[ordinal].ThenExpression.GetId(nameof(ScalarExpression))
            });
        }

        if (elseExpression is not null)
        {
            model.CaseExpressionElseExpressionLinkList.Add(new CaseExpressionElseExpressionLink
            {
                Id = NextId(nameof(CaseExpressionElseExpressionLink)),
                CaseExpressionId = caseExpression.Id,
                ScalarExpressionId = elseExpression.GetId(nameof(ScalarExpression))
            });
        }

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(CaseExpression), caseExpression.Id),
            (nameof(SearchedCaseExpression), searchedCaseExpression.Id));
    }
}
