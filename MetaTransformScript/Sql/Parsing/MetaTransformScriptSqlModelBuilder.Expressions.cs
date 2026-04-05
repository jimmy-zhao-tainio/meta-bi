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
            BaseId = scalar.Id,
            BinaryExpressionType = binaryExpressionType
        };
        model.BinaryExpressionList.Add(binaryExpression);
        model.BinaryExpressionFirstExpressionLinkList.Add(new BinaryExpressionFirstExpressionLink
        {
            Id = NextId(nameof(BinaryExpressionFirstExpressionLink)),
            OwnerId = binaryExpression.Id,
            ValueId = firstExpression.GetId(nameof(ScalarExpression))
        });
        model.BinaryExpressionSecondExpressionLinkList.Add(new BinaryExpressionSecondExpressionLink
        {
            Id = NextId(nameof(BinaryExpressionSecondExpressionLink)),
            OwnerId = binaryExpression.Id,
            ValueId = secondExpression.GetId(nameof(ScalarExpression))
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var columnReference = new ColumnReferenceExpression
        {
            Id = NextId(nameof(ColumnReferenceExpression)),
            BaseId = primary.Id,
            ColumnType = columnType
        };
        model.ColumnReferenceExpressionList.Add(columnReference);
        model.ColumnReferenceExpressionMultiPartIdentifierLinkList.Add(new ColumnReferenceExpressionMultiPartIdentifierLink
        {
            Id = NextId(nameof(ColumnReferenceExpressionMultiPartIdentifierLink)),
            OwnerId = columnReference.Id,
            ValueId = multiPartIdentifier.GetId(nameof(MultiPartIdentifier))
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var columnReference = new ColumnReferenceExpression
        {
            Id = NextId(nameof(ColumnReferenceExpression)),
            BaseId = primary.Id,
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var parenthesisExpression = new ParenthesisExpression
        {
            Id = NextId(nameof(ParenthesisExpression)),
            BaseId = primary.Id
        };
        model.ParenthesisExpressionList.Add(parenthesisExpression);
        model.ParenthesisExpressionExpressionLinkList.Add(new ParenthesisExpressionExpressionLink
        {
            Id = NextId(nameof(ParenthesisExpressionExpressionLink)),
            OwnerId = parenthesisExpression.Id,
            ValueId = expression.GetId(nameof(ScalarExpression))
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var functionCall = new FunctionCall
        {
            Id = NextId(nameof(FunctionCall)),
            BaseId = primary.Id,
            UniqueRowFilter = uniqueRowFilter ?? string.Empty
        };
        model.FunctionCallList.Add(functionCall);
        model.FunctionCallFunctionNameLinkList.Add(new FunctionCallFunctionNameLink
        {
            Id = NextId(nameof(FunctionCallFunctionNameLink)),
            OwnerId = functionCall.Id,
            ValueId = functionName.GetId(nameof(Identifier))
        });

        for (var ordinal = 0; ordinal < parameters.Count; ordinal++)
        {
            model.FunctionCallParametersItemList.Add(new FunctionCallParametersItem
            {
                Id = NextId(nameof(FunctionCallParametersItem)),
                OwnerId = functionCall.Id,
                ValueId = parameters[ordinal].GetId(nameof(ScalarExpression)),
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var scalarSubquery = new ScalarSubquery
        {
            Id = NextId(nameof(ScalarSubquery)),
            BaseId = primary.Id
        };
        model.ScalarSubqueryList.Add(scalarSubquery);
        model.ScalarSubqueryQueryExpressionLinkList.Add(new ScalarSubqueryQueryExpressionLink
        {
            Id = NextId(nameof(ScalarSubqueryQueryExpressionLink)),
            OwnerId = scalarSubquery.Id,
            ValueId = queryExpression.GetId(nameof(QueryExpression))
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var caseExpression = new CaseExpression
        {
            Id = NextId(nameof(CaseExpression)),
            BaseId = primary.Id
        };
        model.CaseExpressionList.Add(caseExpression);

        var searchedCaseExpression = new SearchedCaseExpression
        {
            Id = NextId(nameof(SearchedCaseExpression)),
            BaseId = caseExpression.Id
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
                BaseId = whenClause.Id
            };
            model.SearchedWhenClauseList.Add(searchedWhenClause);

            model.SearchedCaseExpressionWhenClausesItemList.Add(new SearchedCaseExpressionWhenClausesItem
            {
                Id = NextId(nameof(SearchedCaseExpressionWhenClausesItem)),
                OwnerId = searchedCaseExpression.Id,
                ValueId = searchedWhenClause.Id,
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
            model.SearchedWhenClauseWhenExpressionLinkList.Add(new SearchedWhenClauseWhenExpressionLink
            {
                Id = NextId(nameof(SearchedWhenClauseWhenExpressionLink)),
                OwnerId = searchedWhenClause.Id,
                ValueId = whenClauses[ordinal].WhenExpression.GetId(nameof(BooleanExpression))
            });
            model.WhenClauseThenExpressionLinkList.Add(new WhenClauseThenExpressionLink
            {
                Id = NextId(nameof(WhenClauseThenExpressionLink)),
                OwnerId = whenClause.Id,
                ValueId = whenClauses[ordinal].ThenExpression.GetId(nameof(ScalarExpression))
            });
        }

        if (elseExpression is not null)
        {
            model.CaseExpressionElseExpressionLinkList.Add(new CaseExpressionElseExpressionLink
            {
                Id = NextId(nameof(CaseExpressionElseExpressionLink)),
                OwnerId = caseExpression.Id,
                ValueId = elseExpression.GetId(nameof(ScalarExpression))
            });
        }

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(CaseExpression), caseExpression.Id),
            (nameof(SearchedCaseExpression), searchedCaseExpression.Id));
    }
}
