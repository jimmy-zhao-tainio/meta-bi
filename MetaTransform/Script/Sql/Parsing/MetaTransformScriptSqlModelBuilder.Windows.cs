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
                WindowClause = windowClause,
                WindowDefinition = windowDefinitions[ordinal].GetRef<WindowDefinition>(nameof(WindowDefinition)),
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
            WindowDefinition = windowDefinition,
            Identifier = windowName.GetRef<Identifier>(nameof(Identifier))
        });

        if (refWindowName is not null)
        {
            model.WindowDefinitionRefWindowNameLinkList.Add(new WindowDefinitionRefWindowNameLink
            {
                Id = NextId(nameof(WindowDefinitionRefWindowNameLink)),
                WindowDefinition = windowDefinition,
                Identifier = refWindowName.GetRef<Identifier>(nameof(Identifier))
            });
        }

        if (partitions is not null)
        {
            for (var ordinal = 0; ordinal < partitions.Count; ordinal++)
            {
                model.WindowDefinitionPartitionsItemList.Add(new WindowDefinitionPartitionsItem
                {
                    Id = NextId(nameof(WindowDefinitionPartitionsItem)),
                    WindowDefinition = windowDefinition,
                    ScalarExpression = partitions[ordinal].GetRef<ScalarExpression>(nameof(ScalarExpression)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        if (orderByClause is not null)
        {
            model.WindowDefinitionOrderByClauseLinkList.Add(new WindowDefinitionOrderByClauseLink
            {
                Id = NextId(nameof(WindowDefinitionOrderByClauseLink)),
                WindowDefinition = windowDefinition,
                OrderByClause = orderByClause.GetRef<OrderByClause>(nameof(OrderByClause))
            });
        }

        if (windowFrameClause is not null)
        {
            model.WindowDefinitionWindowFrameClauseLinkList.Add(new WindowDefinitionWindowFrameClauseLink
            {
                Id = NextId(nameof(WindowDefinitionWindowFrameClauseLink)),
                WindowDefinition = windowDefinition,
                WindowFrameClause = windowFrameClause.GetRef<WindowFrameClause>(nameof(WindowFrameClause))
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
                WindowDelimiter = windowDelimiter,
                ScalarExpression = offsetValue.GetRef<ScalarExpression>(nameof(ScalarExpression))
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
            WindowFrameClause = windowFrameClause,
            WindowDelimiter = top.GetRef<WindowDelimiter>(nameof(WindowDelimiter))
        });

        if (bottom is not null)
        {
            model.WindowFrameClauseBottomLinkList.Add(new WindowFrameClauseBottomLink
            {
                Id = NextId(nameof(WindowFrameClauseBottomLink)),
                WindowFrameClause = windowFrameClause,
                WindowDelimiter = bottom.GetRef<WindowDelimiter>(nameof(WindowDelimiter))
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
                OverClause = overClause,
                Identifier = windowName.GetRef<Identifier>(nameof(Identifier))
            });
        }

        if (partitions is not null)
        {
            for (var ordinal = 0; ordinal < partitions.Count; ordinal++)
            {
                model.OverClausePartitionsItemList.Add(new OverClausePartitionsItem
                {
                    Id = NextId(nameof(OverClausePartitionsItem)),
                    OverClause = overClause,
                    ScalarExpression = partitions[ordinal].GetRef<ScalarExpression>(nameof(ScalarExpression)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        if (orderByClause is not null)
        {
            model.OverClauseOrderByClauseLinkList.Add(new OverClauseOrderByClauseLink
            {
                Id = NextId(nameof(OverClauseOrderByClauseLink)),
                OverClause = overClause,
                OrderByClause = orderByClause.GetRef<OrderByClause>(nameof(OrderByClause))
            });
        }

        if (windowFrameClause is not null)
        {
            model.OverClauseWindowFrameClauseLinkList.Add(new OverClauseWindowFrameClauseLink
            {
                Id = NextId(nameof(OverClauseWindowFrameClauseLink)),
                OverClause = overClause,
                WindowFrameClause = windowFrameClause.GetRef<WindowFrameClause>(nameof(WindowFrameClause))
            });
        }

        return BuiltNode.Create((nameof(OverClause), overClause.Id));
    }

    public BuiltNode AttachOverClause(BuiltNode functionCall, BuiltNode overClause)
    {
        model.FunctionCallOverClauseLinkList.Add(new FunctionCallOverClauseLink
        {
            Id = NextId(nameof(FunctionCallOverClauseLink)),
            FunctionCall = functionCall.GetRef<FunctionCall>(nameof(FunctionCall)),
            OverClause = overClause.GetRef<OverClause>(nameof(OverClause))
        });

        return functionCall;
    }
}
