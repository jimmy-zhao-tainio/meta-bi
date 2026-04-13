using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    private BuiltNode CreateBooleanExpressionBase()
    {
        var row = new BooleanExpression
        {
            Id = NextId(nameof(BooleanExpression))
        };
        model.BooleanExpressionList.Add(row);
        return BuiltNode.Create((nameof(BooleanExpression), row.Id));
    }

    public BuiltNode CreateBooleanBinaryExpression(BuiltNode firstExpression, BuiltNode secondExpression, string binaryExpressionType)
    {
        var booleanExpression = CreateBooleanExpressionBase();

        var row = new BooleanBinaryExpression
        {
            Id = NextId(nameof(BooleanBinaryExpression)),
            BooleanExpressionId = booleanExpression.GetId(nameof(BooleanExpression)),
            BinaryExpressionType = binaryExpressionType
        };
        model.BooleanBinaryExpressionList.Add(row);
        model.BooleanBinaryExpressionFirstExpressionLinkList.Add(new BooleanBinaryExpressionFirstExpressionLink
        {
            Id = NextId(nameof(BooleanBinaryExpressionFirstExpressionLink)),
            BooleanBinaryExpressionId = row.Id,
            BooleanExpressionId = firstExpression.GetId(nameof(BooleanExpression))
        });
        model.BooleanBinaryExpressionSecondExpressionLinkList.Add(new BooleanBinaryExpressionSecondExpressionLink
        {
            Id = NextId(nameof(BooleanBinaryExpressionSecondExpressionLink)),
            BooleanBinaryExpressionId = row.Id,
            BooleanExpressionId = secondExpression.GetId(nameof(BooleanExpression))
        });

        return BuiltNode.Create(
            (nameof(BooleanExpression), booleanExpression.GetId(nameof(BooleanExpression))),
            (nameof(BooleanBinaryExpression), row.Id));
    }

    public BuiltNode CreateBooleanComparisonExpression(BuiltNode firstExpression, BuiltNode secondExpression, string comparisonType)
    {
        var booleanExpression = CreateBooleanExpressionBase();

        var row = new BooleanComparisonExpression
        {
            Id = NextId(nameof(BooleanComparisonExpression)),
            BooleanExpressionId = booleanExpression.GetId(nameof(BooleanExpression)),
            ComparisonType = comparisonType
        };
        model.BooleanComparisonExpressionList.Add(row);
        model.BooleanComparisonExpressionFirstExpressionLinkList.Add(new BooleanComparisonExpressionFirstExpressionLink
        {
            Id = NextId(nameof(BooleanComparisonExpressionFirstExpressionLink)),
            BooleanComparisonExpressionId = row.Id,
            ScalarExpressionId = firstExpression.GetId(nameof(ScalarExpression))
        });
        model.BooleanComparisonExpressionSecondExpressionLinkList.Add(new BooleanComparisonExpressionSecondExpressionLink
        {
            Id = NextId(nameof(BooleanComparisonExpressionSecondExpressionLink)),
            BooleanComparisonExpressionId = row.Id,
            ScalarExpressionId = secondExpression.GetId(nameof(ScalarExpression))
        });

        return BuiltNode.Create(
            (nameof(BooleanExpression), booleanExpression.GetId(nameof(BooleanExpression))),
            (nameof(BooleanComparisonExpression), row.Id));
    }

    public BuiltNode CreateBooleanTernaryExpression(
        BuiltNode firstExpression,
        BuiltNode secondExpression,
        BuiltNode thirdExpression,
        string ternaryExpressionType)
    {
        var booleanExpression = CreateBooleanExpressionBase();

        var row = new BooleanTernaryExpression
        {
            Id = NextId(nameof(BooleanTernaryExpression)),
            BooleanExpressionId = booleanExpression.GetId(nameof(BooleanExpression)),
            TernaryExpressionType = ternaryExpressionType
        };
        model.BooleanTernaryExpressionList.Add(row);
        model.BooleanTernaryExpressionFirstExpressionLinkList.Add(new BooleanTernaryExpressionFirstExpressionLink
        {
            Id = NextId(nameof(BooleanTernaryExpressionFirstExpressionLink)),
            BooleanTernaryExpressionId = row.Id,
            ScalarExpressionId = firstExpression.GetId(nameof(ScalarExpression))
        });
        model.BooleanTernaryExpressionSecondExpressionLinkList.Add(new BooleanTernaryExpressionSecondExpressionLink
        {
            Id = NextId(nameof(BooleanTernaryExpressionSecondExpressionLink)),
            BooleanTernaryExpressionId = row.Id,
            ScalarExpressionId = secondExpression.GetId(nameof(ScalarExpression))
        });
        model.BooleanTernaryExpressionThirdExpressionLinkList.Add(new BooleanTernaryExpressionThirdExpressionLink
        {
            Id = NextId(nameof(BooleanTernaryExpressionThirdExpressionLink)),
            BooleanTernaryExpressionId = row.Id,
            ScalarExpressionId = thirdExpression.GetId(nameof(ScalarExpression))
        });

        return BuiltNode.Create(
            (nameof(BooleanExpression), booleanExpression.GetId(nameof(BooleanExpression))),
            (nameof(BooleanTernaryExpression), row.Id));
    }

    public BuiltNode CreateBooleanParenthesisExpression(BuiltNode expression)
    {
        var booleanExpression = CreateBooleanExpressionBase();

        var row = new BooleanParenthesisExpression
        {
            Id = NextId(nameof(BooleanParenthesisExpression)),
            BooleanExpressionId = booleanExpression.GetId(nameof(BooleanExpression))
        };
        model.BooleanParenthesisExpressionList.Add(row);
        model.BooleanParenthesisExpressionExpressionLinkList.Add(new BooleanParenthesisExpressionExpressionLink
        {
            Id = NextId(nameof(BooleanParenthesisExpressionExpressionLink)),
            BooleanParenthesisExpressionId = row.Id,
            BooleanExpressionId = expression.GetId(nameof(BooleanExpression))
        });

        return BuiltNode.Create(
            (nameof(BooleanExpression), booleanExpression.GetId(nameof(BooleanExpression))),
            (nameof(BooleanParenthesisExpression), row.Id));
    }

    public BuiltNode CreateBooleanNotExpression(BuiltNode expression)
    {
        var booleanExpression = CreateBooleanExpressionBase();

        var row = new BooleanNotExpression
        {
            Id = NextId(nameof(BooleanNotExpression)),
            BooleanExpressionId = booleanExpression.GetId(nameof(BooleanExpression))
        };
        model.BooleanNotExpressionList.Add(row);
        model.BooleanNotExpressionExpressionLinkList.Add(new BooleanNotExpressionExpressionLink
        {
            Id = NextId(nameof(BooleanNotExpressionExpressionLink)),
            BooleanNotExpressionId = row.Id,
            BooleanExpressionId = expression.GetId(nameof(BooleanExpression))
        });

        return BuiltNode.Create(
            (nameof(BooleanExpression), booleanExpression.GetId(nameof(BooleanExpression))),
            (nameof(BooleanNotExpression), row.Id));
    }

    public BuiltNode CreateBooleanIsNullExpression(BuiltNode expression, bool isNot)
    {
        var booleanExpression = CreateBooleanExpressionBase();

        var row = new BooleanIsNullExpression
        {
            Id = NextId(nameof(BooleanIsNullExpression)),
            BooleanExpressionId = booleanExpression.GetId(nameof(BooleanExpression)),
            IsNot = isNot ? "true" : string.Empty
        };
        model.BooleanIsNullExpressionList.Add(row);
        model.BooleanIsNullExpressionExpressionLinkList.Add(new BooleanIsNullExpressionExpressionLink
        {
            Id = NextId(nameof(BooleanIsNullExpressionExpressionLink)),
            BooleanIsNullExpressionId = row.Id,
            ScalarExpressionId = expression.GetId(nameof(ScalarExpression))
        });

        return BuiltNode.Create(
            (nameof(BooleanExpression), booleanExpression.GetId(nameof(BooleanExpression))),
            (nameof(BooleanIsNullExpression), row.Id));
    }

    public BuiltNode CreateLikePredicate(BuiltNode firstExpression, BuiltNode secondExpression, bool notDefined, BuiltNode? escapeExpression = null)
    {
        var booleanExpression = CreateBooleanExpressionBase();

        var row = new LikePredicate
        {
            Id = NextId(nameof(LikePredicate)),
            BooleanExpressionId = booleanExpression.GetId(nameof(BooleanExpression)),
            NotDefined = notDefined ? "true" : string.Empty
        };
        model.LikePredicateList.Add(row);
        model.LikePredicateFirstExpressionLinkList.Add(new LikePredicateFirstExpressionLink
        {
            Id = NextId(nameof(LikePredicateFirstExpressionLink)),
            LikePredicateId = row.Id,
            ScalarExpressionId = firstExpression.GetId(nameof(ScalarExpression))
        });
        model.LikePredicateSecondExpressionLinkList.Add(new LikePredicateSecondExpressionLink
        {
            Id = NextId(nameof(LikePredicateSecondExpressionLink)),
            LikePredicateId = row.Id,
            ScalarExpressionId = secondExpression.GetId(nameof(ScalarExpression))
        });

        if (escapeExpression is not null)
        {
            model.LikePredicateEscapeExpressionLinkList.Add(new LikePredicateEscapeExpressionLink
            {
                Id = NextId(nameof(LikePredicateEscapeExpressionLink)),
                LikePredicateId = row.Id,
                ScalarExpressionId = escapeExpression.GetId(nameof(ScalarExpression))
            });
        }

        return BuiltNode.Create(
            (nameof(BooleanExpression), booleanExpression.GetId(nameof(BooleanExpression))),
            (nameof(LikePredicate), row.Id));
    }

    public BuiltNode CreateDistinctPredicate(BuiltNode firstExpression, BuiltNode secondExpression, bool isNot)
    {
        var booleanExpression = CreateBooleanExpressionBase();

        var row = new DistinctPredicate
        {
            Id = NextId(nameof(DistinctPredicate)),
            BooleanExpressionId = booleanExpression.GetId(nameof(BooleanExpression)),
            IsNot = isNot ? "true" : string.Empty
        };
        model.DistinctPredicateList.Add(row);
        model.DistinctPredicateFirstExpressionLinkList.Add(new DistinctPredicateFirstExpressionLink
        {
            Id = NextId(nameof(DistinctPredicateFirstExpressionLink)),
            DistinctPredicateId = row.Id,
            ScalarExpressionId = firstExpression.GetId(nameof(ScalarExpression))
        });
        model.DistinctPredicateSecondExpressionLinkList.Add(new DistinctPredicateSecondExpressionLink
        {
            Id = NextId(nameof(DistinctPredicateSecondExpressionLink)),
            DistinctPredicateId = row.Id,
            ScalarExpressionId = secondExpression.GetId(nameof(ScalarExpression))
        });

        return BuiltNode.Create(
            (nameof(BooleanExpression), booleanExpression.GetId(nameof(BooleanExpression))),
            (nameof(DistinctPredicate), row.Id));
    }

    public BuiltNode CreateFullTextPredicate(string fullTextFunctionType, IReadOnlyList<BuiltNode> columns, BuiltNode value)
    {
        var booleanExpression = CreateBooleanExpressionBase();

        var row = new FullTextPredicate
        {
            Id = NextId(nameof(FullTextPredicate)),
            BooleanExpressionId = booleanExpression.GetId(nameof(BooleanExpression)),
            FullTextFunctionType = fullTextFunctionType
        };
        model.FullTextPredicateList.Add(row);

        for (var ordinal = 0; ordinal < columns.Count; ordinal++)
        {
            model.FullTextPredicateColumnsItemList.Add(new FullTextPredicateColumnsItem
            {
                Id = NextId(nameof(FullTextPredicateColumnsItem)),
                FullTextPredicateId = row.Id,
                ColumnReferenceExpressionId = columns[ordinal].GetId(nameof(ColumnReferenceExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        model.FullTextPredicateValueLinkList.Add(new FullTextPredicateValueLink
        {
            Id = NextId(nameof(FullTextPredicateValueLink)),
            FullTextPredicateId = row.Id,
            ValueExpressionId = value.GetId(nameof(ValueExpression))
        });

        return BuiltNode.Create(
            (nameof(BooleanExpression), booleanExpression.GetId(nameof(BooleanExpression))),
            (nameof(FullTextPredicate), row.Id));
    }

    public BuiltNode CreateInPredicate(BuiltNode expression, IReadOnlyList<BuiltNode> values, bool notDefined)
    {
        var booleanExpression = CreateBooleanExpressionBase();

        var row = new InPredicate
        {
            Id = NextId(nameof(InPredicate)),
            BooleanExpressionId = booleanExpression.GetId(nameof(BooleanExpression)),
            NotDefined = notDefined ? "true" : string.Empty
        };
        model.InPredicateList.Add(row);
        model.InPredicateExpressionLinkList.Add(new InPredicateExpressionLink
        {
            Id = NextId(nameof(InPredicateExpressionLink)),
            InPredicateId = row.Id,
            ScalarExpressionId = expression.GetId(nameof(ScalarExpression))
        });

        for (var ordinal = 0; ordinal < values.Count; ordinal++)
        {
            model.InPredicateValuesItemList.Add(new InPredicateValuesItem
            {
                Id = NextId(nameof(InPredicateValuesItem)),
                InPredicateId = row.Id,
                ScalarExpressionId = values[ordinal].GetId(nameof(ScalarExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(BooleanExpression), booleanExpression.GetId(nameof(BooleanExpression))),
            (nameof(InPredicate), row.Id));
    }

    public BuiltNode CreateInPredicateSubquery(BuiltNode expression, BuiltNode subquery, bool notDefined)
    {
        var booleanExpression = CreateBooleanExpressionBase();

        var row = new InPredicate
        {
            Id = NextId(nameof(InPredicate)),
            BooleanExpressionId = booleanExpression.GetId(nameof(BooleanExpression)),
            NotDefined = notDefined ? "true" : string.Empty
        };
        model.InPredicateList.Add(row);
        model.InPredicateExpressionLinkList.Add(new InPredicateExpressionLink
        {
            Id = NextId(nameof(InPredicateExpressionLink)),
            InPredicateId = row.Id,
            ScalarExpressionId = expression.GetId(nameof(ScalarExpression))
        });
        model.InPredicateSubqueryLinkList.Add(new InPredicateSubqueryLink
        {
            Id = NextId(nameof(InPredicateSubqueryLink)),
            InPredicateId = row.Id,
            ScalarSubqueryId = subquery.GetId(nameof(ScalarSubquery))
        });

        return BuiltNode.Create(
            (nameof(BooleanExpression), booleanExpression.GetId(nameof(BooleanExpression))),
            (nameof(InPredicate), row.Id));
    }

    public BuiltNode CreateExistsPredicate(BuiltNode subquery)
    {
        var booleanExpression = CreateBooleanExpressionBase();

        var row = new ExistsPredicate
        {
            Id = NextId(nameof(ExistsPredicate)),
            BooleanExpressionId = booleanExpression.GetId(nameof(BooleanExpression))
        };
        model.ExistsPredicateList.Add(row);
        model.ExistsPredicateSubqueryLinkList.Add(new ExistsPredicateSubqueryLink
        {
            Id = NextId(nameof(ExistsPredicateSubqueryLink)),
            ExistsPredicateId = row.Id,
            ScalarSubqueryId = subquery.GetId(nameof(ScalarSubquery))
        });

        return BuiltNode.Create(
            (nameof(BooleanExpression), booleanExpression.GetId(nameof(BooleanExpression))),
            (nameof(ExistsPredicate), row.Id));
    }

    public BuiltNode CreateSubqueryComparisonPredicate(
        BuiltNode expression,
        BuiltNode subquery,
        string comparisonType,
        string subqueryComparisonPredicateType)
    {
        var booleanExpression = CreateBooleanExpressionBase();

        var row = new SubqueryComparisonPredicate
        {
            Id = NextId(nameof(SubqueryComparisonPredicate)),
            BooleanExpressionId = booleanExpression.GetId(nameof(BooleanExpression)),
            ComparisonType = comparisonType,
            SubqueryComparisonPredicateType = subqueryComparisonPredicateType
        };
        model.SubqueryComparisonPredicateList.Add(row);
        model.SubqueryComparisonPredicateExpressionLinkList.Add(new SubqueryComparisonPredicateExpressionLink
        {
            Id = NextId(nameof(SubqueryComparisonPredicateExpressionLink)),
            SubqueryComparisonPredicateId = row.Id,
            ScalarExpressionId = expression.GetId(nameof(ScalarExpression))
        });
        model.SubqueryComparisonPredicateSubqueryLinkList.Add(new SubqueryComparisonPredicateSubqueryLink
        {
            Id = NextId(nameof(SubqueryComparisonPredicateSubqueryLink)),
            SubqueryComparisonPredicateId = row.Id,
            ScalarSubqueryId = subquery.GetId(nameof(ScalarSubquery))
        });

        return BuiltNode.Create(
            (nameof(BooleanExpression), booleanExpression.GetId(nameof(BooleanExpression))),
            (nameof(SubqueryComparisonPredicate), row.Id));
    }
}
