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
            BaseId = booleanExpression.GetId(nameof(BooleanExpression)),
            BinaryExpressionType = binaryExpressionType
        };
        model.BooleanBinaryExpressionList.Add(row);
        model.BooleanBinaryExpressionFirstExpressionLinkList.Add(new BooleanBinaryExpressionFirstExpressionLink
        {
            Id = NextId(nameof(BooleanBinaryExpressionFirstExpressionLink)),
            OwnerId = row.Id,
            ValueId = firstExpression.GetId(nameof(BooleanExpression))
        });
        model.BooleanBinaryExpressionSecondExpressionLinkList.Add(new BooleanBinaryExpressionSecondExpressionLink
        {
            Id = NextId(nameof(BooleanBinaryExpressionSecondExpressionLink)),
            OwnerId = row.Id,
            ValueId = secondExpression.GetId(nameof(BooleanExpression))
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
            BaseId = booleanExpression.GetId(nameof(BooleanExpression)),
            ComparisonType = comparisonType
        };
        model.BooleanComparisonExpressionList.Add(row);
        model.BooleanComparisonExpressionFirstExpressionLinkList.Add(new BooleanComparisonExpressionFirstExpressionLink
        {
            Id = NextId(nameof(BooleanComparisonExpressionFirstExpressionLink)),
            OwnerId = row.Id,
            ValueId = firstExpression.GetId(nameof(ScalarExpression))
        });
        model.BooleanComparisonExpressionSecondExpressionLinkList.Add(new BooleanComparisonExpressionSecondExpressionLink
        {
            Id = NextId(nameof(BooleanComparisonExpressionSecondExpressionLink)),
            OwnerId = row.Id,
            ValueId = secondExpression.GetId(nameof(ScalarExpression))
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
            BaseId = booleanExpression.GetId(nameof(BooleanExpression)),
            TernaryExpressionType = ternaryExpressionType
        };
        model.BooleanTernaryExpressionList.Add(row);
        model.BooleanTernaryExpressionFirstExpressionLinkList.Add(new BooleanTernaryExpressionFirstExpressionLink
        {
            Id = NextId(nameof(BooleanTernaryExpressionFirstExpressionLink)),
            OwnerId = row.Id,
            ValueId = firstExpression.GetId(nameof(ScalarExpression))
        });
        model.BooleanTernaryExpressionSecondExpressionLinkList.Add(new BooleanTernaryExpressionSecondExpressionLink
        {
            Id = NextId(nameof(BooleanTernaryExpressionSecondExpressionLink)),
            OwnerId = row.Id,
            ValueId = secondExpression.GetId(nameof(ScalarExpression))
        });
        model.BooleanTernaryExpressionThirdExpressionLinkList.Add(new BooleanTernaryExpressionThirdExpressionLink
        {
            Id = NextId(nameof(BooleanTernaryExpressionThirdExpressionLink)),
            OwnerId = row.Id,
            ValueId = thirdExpression.GetId(nameof(ScalarExpression))
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
            BaseId = booleanExpression.GetId(nameof(BooleanExpression))
        };
        model.BooleanParenthesisExpressionList.Add(row);
        model.BooleanParenthesisExpressionExpressionLinkList.Add(new BooleanParenthesisExpressionExpressionLink
        {
            Id = NextId(nameof(BooleanParenthesisExpressionExpressionLink)),
            OwnerId = row.Id,
            ValueId = expression.GetId(nameof(BooleanExpression))
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
            BaseId = booleanExpression.GetId(nameof(BooleanExpression))
        };
        model.BooleanNotExpressionList.Add(row);
        model.BooleanNotExpressionExpressionLinkList.Add(new BooleanNotExpressionExpressionLink
        {
            Id = NextId(nameof(BooleanNotExpressionExpressionLink)),
            OwnerId = row.Id,
            ValueId = expression.GetId(nameof(BooleanExpression))
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
            BaseId = booleanExpression.GetId(nameof(BooleanExpression)),
            IsNot = isNot ? "true" : string.Empty
        };
        model.BooleanIsNullExpressionList.Add(row);
        model.BooleanIsNullExpressionExpressionLinkList.Add(new BooleanIsNullExpressionExpressionLink
        {
            Id = NextId(nameof(BooleanIsNullExpressionExpressionLink)),
            OwnerId = row.Id,
            ValueId = expression.GetId(nameof(ScalarExpression))
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
            BaseId = booleanExpression.GetId(nameof(BooleanExpression)),
            NotDefined = notDefined ? "true" : string.Empty
        };
        model.LikePredicateList.Add(row);
        model.LikePredicateFirstExpressionLinkList.Add(new LikePredicateFirstExpressionLink
        {
            Id = NextId(nameof(LikePredicateFirstExpressionLink)),
            OwnerId = row.Id,
            ValueId = firstExpression.GetId(nameof(ScalarExpression))
        });
        model.LikePredicateSecondExpressionLinkList.Add(new LikePredicateSecondExpressionLink
        {
            Id = NextId(nameof(LikePredicateSecondExpressionLink)),
            OwnerId = row.Id,
            ValueId = secondExpression.GetId(nameof(ScalarExpression))
        });

        if (escapeExpression is not null)
        {
            model.LikePredicateEscapeExpressionLinkList.Add(new LikePredicateEscapeExpressionLink
            {
                Id = NextId(nameof(LikePredicateEscapeExpressionLink)),
                OwnerId = row.Id,
                ValueId = escapeExpression.GetId(nameof(ScalarExpression))
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
            BaseId = booleanExpression.GetId(nameof(BooleanExpression)),
            IsNot = isNot ? "true" : string.Empty
        };
        model.DistinctPredicateList.Add(row);
        model.DistinctPredicateFirstExpressionLinkList.Add(new DistinctPredicateFirstExpressionLink
        {
            Id = NextId(nameof(DistinctPredicateFirstExpressionLink)),
            OwnerId = row.Id,
            ValueId = firstExpression.GetId(nameof(ScalarExpression))
        });
        model.DistinctPredicateSecondExpressionLinkList.Add(new DistinctPredicateSecondExpressionLink
        {
            Id = NextId(nameof(DistinctPredicateSecondExpressionLink)),
            OwnerId = row.Id,
            ValueId = secondExpression.GetId(nameof(ScalarExpression))
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
            BaseId = booleanExpression.GetId(nameof(BooleanExpression)),
            FullTextFunctionType = fullTextFunctionType
        };
        model.FullTextPredicateList.Add(row);

        for (var ordinal = 0; ordinal < columns.Count; ordinal++)
        {
            model.FullTextPredicateColumnsItemList.Add(new FullTextPredicateColumnsItem
            {
                Id = NextId(nameof(FullTextPredicateColumnsItem)),
                OwnerId = row.Id,
                ValueId = columns[ordinal].GetId(nameof(ColumnReferenceExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        model.FullTextPredicateValueLinkList.Add(new FullTextPredicateValueLink
        {
            Id = NextId(nameof(FullTextPredicateValueLink)),
            OwnerId = row.Id,
            ValueId = value.GetId(nameof(ValueExpression))
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
            BaseId = booleanExpression.GetId(nameof(BooleanExpression)),
            NotDefined = notDefined ? "true" : string.Empty
        };
        model.InPredicateList.Add(row);
        model.InPredicateExpressionLinkList.Add(new InPredicateExpressionLink
        {
            Id = NextId(nameof(InPredicateExpressionLink)),
            OwnerId = row.Id,
            ValueId = expression.GetId(nameof(ScalarExpression))
        });

        for (var ordinal = 0; ordinal < values.Count; ordinal++)
        {
            model.InPredicateValuesItemList.Add(new InPredicateValuesItem
            {
                Id = NextId(nameof(InPredicateValuesItem)),
                OwnerId = row.Id,
                ValueId = values[ordinal].GetId(nameof(ScalarExpression)),
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
            BaseId = booleanExpression.GetId(nameof(BooleanExpression)),
            NotDefined = notDefined ? "true" : string.Empty
        };
        model.InPredicateList.Add(row);
        model.InPredicateExpressionLinkList.Add(new InPredicateExpressionLink
        {
            Id = NextId(nameof(InPredicateExpressionLink)),
            OwnerId = row.Id,
            ValueId = expression.GetId(nameof(ScalarExpression))
        });
        model.InPredicateSubqueryLinkList.Add(new InPredicateSubqueryLink
        {
            Id = NextId(nameof(InPredicateSubqueryLink)),
            OwnerId = row.Id,
            ValueId = subquery.GetId(nameof(ScalarSubquery))
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
            BaseId = booleanExpression.GetId(nameof(BooleanExpression))
        };
        model.ExistsPredicateList.Add(row);
        model.ExistsPredicateSubqueryLinkList.Add(new ExistsPredicateSubqueryLink
        {
            Id = NextId(nameof(ExistsPredicateSubqueryLink)),
            OwnerId = row.Id,
            ValueId = subquery.GetId(nameof(ScalarSubquery))
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
            BaseId = booleanExpression.GetId(nameof(BooleanExpression)),
            ComparisonType = comparisonType,
            SubqueryComparisonPredicateType = subqueryComparisonPredicateType
        };
        model.SubqueryComparisonPredicateList.Add(row);
        model.SubqueryComparisonPredicateExpressionLinkList.Add(new SubqueryComparisonPredicateExpressionLink
        {
            Id = NextId(nameof(SubqueryComparisonPredicateExpressionLink)),
            OwnerId = row.Id,
            ValueId = expression.GetId(nameof(ScalarExpression))
        });
        model.SubqueryComparisonPredicateSubqueryLinkList.Add(new SubqueryComparisonPredicateSubqueryLink
        {
            Id = NextId(nameof(SubqueryComparisonPredicateSubqueryLink)),
            OwnerId = row.Id,
            ValueId = subquery.GetId(nameof(ScalarSubquery))
        });

        return BuiltNode.Create(
            (nameof(BooleanExpression), booleanExpression.GetId(nameof(BooleanExpression))),
            (nameof(SubqueryComparisonPredicate), row.Id));
    }
}
