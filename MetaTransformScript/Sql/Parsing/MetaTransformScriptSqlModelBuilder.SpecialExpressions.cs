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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var caseExpression = new CaseExpression
        {
            Id = NextId(nameof(CaseExpression)),
            BaseId = primary.Id
        };
        model.CaseExpressionList.Add(caseExpression);

        var simpleCaseExpression = new SimpleCaseExpression
        {
            Id = NextId(nameof(SimpleCaseExpression)),
            BaseId = caseExpression.Id
        };
        model.SimpleCaseExpressionList.Add(simpleCaseExpression);
        model.SimpleCaseExpressionInputExpressionLinkList.Add(new SimpleCaseExpressionInputExpressionLink
        {
            Id = NextId(nameof(SimpleCaseExpressionInputExpressionLink)),
            OwnerId = simpleCaseExpression.Id,
            ValueId = inputExpression.GetId(nameof(ScalarExpression))
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
                BaseId = whenClause.Id
            };
            model.SimpleWhenClauseList.Add(simpleWhenClause);

            model.SimpleCaseExpressionWhenClausesItemList.Add(new SimpleCaseExpressionWhenClausesItem
            {
                Id = NextId(nameof(SimpleCaseExpressionWhenClausesItem)),
                OwnerId = simpleCaseExpression.Id,
                ValueId = simpleWhenClause.Id,
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
            model.SimpleWhenClauseWhenExpressionLinkList.Add(new SimpleWhenClauseWhenExpressionLink
            {
                Id = NextId(nameof(SimpleWhenClauseWhenExpressionLink)),
                OwnerId = simpleWhenClause.Id,
                ValueId = whenClauses[ordinal].WhenExpression.GetId(nameof(ScalarExpression))
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var coalesceExpression = new CoalesceExpression
        {
            Id = NextId(nameof(CoalesceExpression)),
            BaseId = primary.Id
        };
        model.CoalesceExpressionList.Add(coalesceExpression);

        for (var ordinal = 0; ordinal < expressions.Count; ordinal++)
        {
            model.CoalesceExpressionExpressionsItemList.Add(new CoalesceExpressionExpressionsItem
            {
                Id = NextId(nameof(CoalesceExpressionExpressionsItem)),
                OwnerId = coalesceExpression.Id,
                ValueId = expressions[ordinal].GetId(nameof(ScalarExpression)),
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var nullIfExpression = new NullIfExpression
        {
            Id = NextId(nameof(NullIfExpression)),
            BaseId = primary.Id
        };
        model.NullIfExpressionList.Add(nullIfExpression);
        model.NullIfExpressionFirstExpressionLinkList.Add(new NullIfExpressionFirstExpressionLink
        {
            Id = NextId(nameof(NullIfExpressionFirstExpressionLink)),
            OwnerId = nullIfExpression.Id,
            ValueId = firstExpression.GetId(nameof(ScalarExpression))
        });
        model.NullIfExpressionSecondExpressionLinkList.Add(new NullIfExpressionSecondExpressionLink
        {
            Id = NextId(nameof(NullIfExpressionSecondExpressionLink)),
            OwnerId = nullIfExpression.Id,
            ValueId = secondExpression.GetId(nameof(ScalarExpression))
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
            BaseId = scalar.Id
        };
        model.PrimaryExpressionList.Add(primary);

        var iIfCall = new IIfCall
        {
            Id = NextId(nameof(IIfCall)),
            BaseId = primary.Id
        };
        model.IIfCallList.Add(iIfCall);
        model.IIfCallPredicateLinkList.Add(new IIfCallPredicateLink
        {
            Id = NextId(nameof(IIfCallPredicateLink)),
            OwnerId = iIfCall.Id,
            ValueId = predicate.GetId(nameof(BooleanExpression))
        });
        model.IIfCallThenExpressionLinkList.Add(new IIfCallThenExpressionLink
        {
            Id = NextId(nameof(IIfCallThenExpressionLink)),
            OwnerId = iIfCall.Id,
            ValueId = thenExpression.GetId(nameof(ScalarExpression))
        });
        model.IIfCallElseExpressionLinkList.Add(new IIfCallElseExpressionLink
        {
            Id = NextId(nameof(IIfCallElseExpressionLink)),
            OwnerId = iIfCall.Id,
            ValueId = elseExpression.GetId(nameof(ScalarExpression))
        });

        return BuiltNode.Create(
            (nameof(ScalarExpression), scalar.Id),
            (nameof(PrimaryExpression), primary.Id),
            (nameof(IIfCall), iIfCall.Id));
    }
}
