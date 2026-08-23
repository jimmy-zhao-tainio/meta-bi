using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    public BuiltNode CreateSimpleCaseExpression(
        BuiltNode inputExpression,
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
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var caseExpression = new CaseExpression
        {
            Id = NextId(nameof(CaseExpression)),
            PrimaryExpression = primary
        };
        model.CaseExpressionList.Add(caseExpression);

        var simpleCaseExpression = new SimpleCaseExpression
        {
            Id = NextId(nameof(SimpleCaseExpression)),
            CaseExpression = caseExpression
        };
        model.SimpleCaseExpressionList.Add(simpleCaseExpression);
        model.SimpleCaseExpressionInputExpressionLinkList.Add(new SimpleCaseExpressionInputExpressionLink
        {
            Id = NextId(nameof(SimpleCaseExpressionInputExpressionLink)),
            SimpleCaseExpression = simpleCaseExpression,
            ScalarExpression = inputExpression.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });

        for (var ordinal = 0; ordinal < whenClauses.Count; ordinal++)
        {
            var whenClause = new WhenClause
            {
                Id = NextId(nameof(WhenClause))
            };
            model.WhenClauseList.Add(whenClause);

            var simpleWhenClause = new SimpleWhenClause
            {
                Id = NextId(nameof(SimpleWhenClause)),
                WhenClause = whenClause
            };
            model.SimpleWhenClauseList.Add(simpleWhenClause);

            model.SimpleCaseExpressionWhenClausesItemList.Add(new SimpleCaseExpressionWhenClausesItem
            {
                Id = NextId(nameof(SimpleCaseExpressionWhenClausesItem)),
                SimpleCaseExpression = simpleCaseExpression,
                SimpleWhenClause = simpleWhenClause,
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
            model.SimpleWhenClauseWhenExpressionLinkList.Add(new SimpleWhenClauseWhenExpressionLink
            {
                Id = NextId(nameof(SimpleWhenClauseWhenExpressionLink)),
                SimpleWhenClause = simpleWhenClause,
                ScalarExpression = whenClauses[ordinal].WhenExpression.GetRef<ScalarExpression>(nameof(ScalarExpression))
            });
            model.WhenClauseThenExpressionLinkList.Add(new WhenClauseThenExpressionLink
            {
                Id = NextId(nameof(WhenClauseThenExpressionLink)),
                WhenClause = whenClause,
                ScalarExpression = whenClauses[ordinal].ThenExpression.GetRef<ScalarExpression>(nameof(ScalarExpression))
            });
        }

        if (elseExpression is not null)
        {
            model.CaseExpressionElseExpressionLinkList.Add(new CaseExpressionElseExpressionLink
            {
                Id = NextId(nameof(CaseExpressionElseExpressionLink)),
                CaseExpression = caseExpression,
                ScalarExpression = elseExpression.GetRef<ScalarExpression>(nameof(ScalarExpression))
            });
        }

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(CaseExpression), caseExpression.Id),
            (nameof(SimpleCaseExpression), simpleCaseExpression.Id));
    }

    public BuiltNode CreateCoalesceExpression(IReadOnlyList<BuiltNode> expressions)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var coalesceExpression = new CoalesceExpression
        {
            Id = NextId(nameof(CoalesceExpression)),
            PrimaryExpression = primary
        };
        model.CoalesceExpressionList.Add(coalesceExpression);

        for (var ordinal = 0; ordinal < expressions.Count; ordinal++)
        {
            model.CoalesceExpressionExpressionsItemList.Add(new CoalesceExpressionExpressionsItem
            {
                Id = NextId(nameof(CoalesceExpressionExpressionsItem)),
                CoalesceExpression = coalesceExpression,
                ScalarExpression = expressions[ordinal].GetRef<ScalarExpression>(nameof(ScalarExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(CoalesceExpression), coalesceExpression.Id));
    }

    public BuiltNode CreateNullIfExpression(BuiltNode firstExpression, BuiltNode secondExpression)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var nullIfExpression = new NullIfExpression
        {
            Id = NextId(nameof(NullIfExpression)),
            PrimaryExpression = primary
        };
        model.NullIfExpressionList.Add(nullIfExpression);
        model.NullIfExpressionFirstExpressionLinkList.Add(new NullIfExpressionFirstExpressionLink
        {
            Id = NextId(nameof(NullIfExpressionFirstExpressionLink)),
            NullIfExpression = nullIfExpression,
            ScalarExpression = firstExpression.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });
        model.NullIfExpressionSecondExpressionLinkList.Add(new NullIfExpressionSecondExpressionLink
        {
            Id = NextId(nameof(NullIfExpressionSecondExpressionLink)),
            NullIfExpression = nullIfExpression,
            ScalarExpression = secondExpression.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(NullIfExpression), nullIfExpression.Id));
    }

    public BuiltNode CreateIIfCall(BuiltNode predicate, BuiltNode thenExpression, BuiltNode elseExpression)
    {
        var scalar = new ScalarExpression
        {
            Id = NextId(nameof(ScalarExpression))
        };
        model.ScalarExpressionList.Add(scalar);

        var primary = new PrimaryExpression
        {
            Id = NextId(nameof(PrimaryExpression)),
            ScalarExpression = scalar
        };
        model.PrimaryExpressionList.Add(primary);

        var iIfCall = new IIfCall
        {
            Id = NextId(nameof(IIfCall)),
            PrimaryExpression = primary
        };
        model.IIfCallList.Add(iIfCall);
        model.IIfCallPredicateLinkList.Add(new IIfCallPredicateLink
        {
            Id = NextId(nameof(IIfCallPredicateLink)),
            IIfCall = iIfCall,
            BooleanExpression = predicate.GetRef<BooleanExpression>(nameof(BooleanExpression))
        });
        model.IIfCallThenExpressionLinkList.Add(new IIfCallThenExpressionLink
        {
            Id = NextId(nameof(IIfCallThenExpressionLink)),
            IIfCall = iIfCall,
            ScalarExpression = thenExpression.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });
        model.IIfCallElseExpressionLinkList.Add(new IIfCallElseExpressionLink
        {
            Id = NextId(nameof(IIfCallElseExpressionLink)),
            IIfCall = iIfCall,
            ScalarExpression = elseExpression.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(IIfCall), iIfCall.Id));
    }
}
