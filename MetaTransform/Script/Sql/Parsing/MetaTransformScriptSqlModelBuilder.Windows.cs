using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    public BuiltNode CreateWindowClause(IReadOnlyList<BuiltNode> windowDefinitions)
    {
        var windowClause = new WindowClause
        {
            Id = NextId(nameof(WindowClause))
        };
        model.WindowClauseList.Add(windowClause);

        for (var ordinal = 0; ordinal < windowDefinitions.Count; ordinal++)
        {
            model.WindowClauseWindowDefinitionItemList.Add(new WindowClauseWindowDefinitionItem
            {
                Id = NextId(nameof(WindowClauseWindowDefinitionItem)),
                WindowClauseId = windowClause.Id,
                WindowDefinitionId = windowDefinitions[ordinal].GetId(nameof(WindowDefinition)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create((nameof(WindowClause), windowClause.Id));
    }

    public BuiltNode CreateWindowDefinition(
        BuiltNode windowName,
        BuiltNode? refWindowName = null,
        IReadOnlyList<BuiltNode>? partitions = null,
        BuiltNode? orderByClause = null,
        BuiltNode? windowFrameClause = null)
    {
        var windowDefinition = new WindowDefinition
        {
            Id = NextId(nameof(WindowDefinition))
        };
        model.WindowDefinitionList.Add(windowDefinition);
        model.WindowDefinitionWindowNameLinkList.Add(new WindowDefinitionWindowNameLink
        {
            Id = NextId(nameof(WindowDefinitionWindowNameLink)),
            WindowDefinitionId = windowDefinition.Id,
            IdentifierId = windowName.GetId(nameof(Identifier))
        });

        if (refWindowName is not null)
        {
            model.WindowDefinitionRefWindowNameLinkList.Add(new WindowDefinitionRefWindowNameLink
            {
                Id = NextId(nameof(WindowDefinitionRefWindowNameLink)),
                WindowDefinitionId = windowDefinition.Id,
                IdentifierId = refWindowName.GetId(nameof(Identifier))
            });
        }

        if (partitions is not null)
        {
            for (var ordinal = 0; ordinal < partitions.Count; ordinal++)
            {
                model.WindowDefinitionPartitionsItemList.Add(new WindowDefinitionPartitionsItem
                {
                    Id = NextId(nameof(WindowDefinitionPartitionsItem)),
                    WindowDefinitionId = windowDefinition.Id,
                    ScalarExpressionId = partitions[ordinal].GetId(nameof(ScalarExpression)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        if (orderByClause is not null)
        {
            model.WindowDefinitionOrderByClauseLinkList.Add(new WindowDefinitionOrderByClauseLink
            {
                Id = NextId(nameof(WindowDefinitionOrderByClauseLink)),
                WindowDefinitionId = windowDefinition.Id,
                OrderByClauseId = orderByClause.GetId(nameof(OrderByClause))
            });
        }

        if (windowFrameClause is not null)
        {
            model.WindowDefinitionWindowFrameClauseLinkList.Add(new WindowDefinitionWindowFrameClauseLink
            {
                Id = NextId(nameof(WindowDefinitionWindowFrameClauseLink)),
                WindowDefinitionId = windowDefinition.Id,
                WindowFrameClauseId = windowFrameClause.GetId(nameof(WindowFrameClause))
            });
        }

        return BuiltNode.Create((nameof(WindowDefinition), windowDefinition.Id));
    }

    public BuiltNode CreateWindowDelimiter(string windowDelimiterType, BuiltNode? offsetValue = null)
    {
        var windowDelimiter = new WindowDelimiter
        {
            Id = NextId(nameof(WindowDelimiter)),
            WindowDelimiterType = windowDelimiterType
        };
        model.WindowDelimiterList.Add(windowDelimiter);

        if (offsetValue is not null)
        {
            model.WindowDelimiterOffsetValueLinkList.Add(new WindowDelimiterOffsetValueLink
            {
                Id = NextId(nameof(WindowDelimiterOffsetValueLink)),
                WindowDelimiterId = windowDelimiter.Id,
                ScalarExpressionId = offsetValue.GetId(nameof(ScalarExpression))
            });
        }

        return BuiltNode.Create((nameof(WindowDelimiter), windowDelimiter.Id));
    }

    public BuiltNode CreateWindowFrameClause(string windowFrameType, BuiltNode top, BuiltNode? bottom = null)
    {
        var windowFrameClause = new WindowFrameClause
        {
            Id = NextId(nameof(WindowFrameClause)),
            WindowFrameType = windowFrameType
        };
        model.WindowFrameClauseList.Add(windowFrameClause);
        model.WindowFrameClauseTopLinkList.Add(new WindowFrameClauseTopLink
        {
            Id = NextId(nameof(WindowFrameClauseTopLink)),
            WindowFrameClauseId = windowFrameClause.Id,
            WindowDelimiterId = top.GetId(nameof(WindowDelimiter))
        });

        if (bottom is not null)
        {
            model.WindowFrameClauseBottomLinkList.Add(new WindowFrameClauseBottomLink
            {
                Id = NextId(nameof(WindowFrameClauseBottomLink)),
                WindowFrameClauseId = windowFrameClause.Id,
                WindowDelimiterId = bottom.GetId(nameof(WindowDelimiter))
            });
        }

        return BuiltNode.Create((nameof(WindowFrameClause), windowFrameClause.Id));
    }

    public BuiltNode CreateOverClause(
        BuiltNode? windowName = null,
        IReadOnlyList<BuiltNode>? partitions = null,
        BuiltNode? orderByClause = null,
        BuiltNode? windowFrameClause = null)
    {
        var overClause = new OverClause
        {
            Id = NextId(nameof(OverClause))
        };
        model.OverClauseList.Add(overClause);

        if (windowName is not null)
        {
            model.OverClauseWindowNameLinkList.Add(new OverClauseWindowNameLink
            {
                Id = NextId(nameof(OverClauseWindowNameLink)),
                OverClauseId = overClause.Id,
                IdentifierId = windowName.GetId(nameof(Identifier))
            });
        }

        if (partitions is not null)
        {
            for (var ordinal = 0; ordinal < partitions.Count; ordinal++)
            {
                model.OverClausePartitionsItemList.Add(new OverClausePartitionsItem
                {
                    Id = NextId(nameof(OverClausePartitionsItem)),
                    OverClauseId = overClause.Id,
                    ScalarExpressionId = partitions[ordinal].GetId(nameof(ScalarExpression)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        if (orderByClause is not null)
        {
            model.OverClauseOrderByClauseLinkList.Add(new OverClauseOrderByClauseLink
            {
                Id = NextId(nameof(OverClauseOrderByClauseLink)),
                OverClauseId = overClause.Id,
                OrderByClauseId = orderByClause.GetId(nameof(OrderByClause))
            });
        }

        if (windowFrameClause is not null)
        {
            model.OverClauseWindowFrameClauseLinkList.Add(new OverClauseWindowFrameClauseLink
            {
                Id = NextId(nameof(OverClauseWindowFrameClauseLink)),
                OverClauseId = overClause.Id,
                WindowFrameClauseId = windowFrameClause.GetId(nameof(WindowFrameClause))
            });
        }

        return BuiltNode.Create((nameof(OverClause), overClause.Id));
    }

    public BuiltNode AttachOverClause(BuiltNode functionCall, BuiltNode overClause)
    {
        model.FunctionCallOverClauseLinkList.Add(new FunctionCallOverClauseLink
        {
            Id = NextId(nameof(FunctionCallOverClauseLink)),
            FunctionCallId = functionCall.GetId(nameof(FunctionCall)),
            OverClauseId = overClause.GetId(nameof(OverClause))
        });

        return functionCall;
    }
}
