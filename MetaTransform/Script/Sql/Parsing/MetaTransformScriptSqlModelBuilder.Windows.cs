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
                OwnerId = windowClause.Id,
                ValueId = windowDefinitions[ordinal].GetId(nameof(WindowDefinition)),
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
            OwnerId = windowDefinition.Id,
            ValueId = windowName.GetId(nameof(Identifier))
        });

        if (refWindowName is not null)
        {
            model.WindowDefinitionRefWindowNameLinkList.Add(new WindowDefinitionRefWindowNameLink
            {
                Id = NextId(nameof(WindowDefinitionRefWindowNameLink)),
                OwnerId = windowDefinition.Id,
                ValueId = refWindowName.GetId(nameof(Identifier))
            });
        }

        if (partitions is not null)
        {
            for (var ordinal = 0; ordinal < partitions.Count; ordinal++)
            {
                model.WindowDefinitionPartitionsItemList.Add(new WindowDefinitionPartitionsItem
                {
                    Id = NextId(nameof(WindowDefinitionPartitionsItem)),
                    OwnerId = windowDefinition.Id,
                    ValueId = partitions[ordinal].GetId(nameof(ScalarExpression)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        if (orderByClause is not null)
        {
            model.WindowDefinitionOrderByClauseLinkList.Add(new WindowDefinitionOrderByClauseLink
            {
                Id = NextId(nameof(WindowDefinitionOrderByClauseLink)),
                OwnerId = windowDefinition.Id,
                ValueId = orderByClause.GetId(nameof(OrderByClause))
            });
        }

        if (windowFrameClause is not null)
        {
            model.WindowDefinitionWindowFrameClauseLinkList.Add(new WindowDefinitionWindowFrameClauseLink
            {
                Id = NextId(nameof(WindowDefinitionWindowFrameClauseLink)),
                OwnerId = windowDefinition.Id,
                ValueId = windowFrameClause.GetId(nameof(WindowFrameClause))
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
                OwnerId = windowDelimiter.Id,
                ValueId = offsetValue.GetId(nameof(ScalarExpression))
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
            OwnerId = windowFrameClause.Id,
            ValueId = top.GetId(nameof(WindowDelimiter))
        });

        if (bottom is not null)
        {
            model.WindowFrameClauseBottomLinkList.Add(new WindowFrameClauseBottomLink
            {
                Id = NextId(nameof(WindowFrameClauseBottomLink)),
                OwnerId = windowFrameClause.Id,
                ValueId = bottom.GetId(nameof(WindowDelimiter))
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
                OwnerId = overClause.Id,
                ValueId = windowName.GetId(nameof(Identifier))
            });
        }

        if (partitions is not null)
        {
            for (var ordinal = 0; ordinal < partitions.Count; ordinal++)
            {
                model.OverClausePartitionsItemList.Add(new OverClausePartitionsItem
                {
                    Id = NextId(nameof(OverClausePartitionsItem)),
                    OwnerId = overClause.Id,
                    ValueId = partitions[ordinal].GetId(nameof(ScalarExpression)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        if (orderByClause is not null)
        {
            model.OverClauseOrderByClauseLinkList.Add(new OverClauseOrderByClauseLink
            {
                Id = NextId(nameof(OverClauseOrderByClauseLink)),
                OwnerId = overClause.Id,
                ValueId = orderByClause.GetId(nameof(OrderByClause))
            });
        }

        if (windowFrameClause is not null)
        {
            model.OverClauseWindowFrameClauseLinkList.Add(new OverClauseWindowFrameClauseLink
            {
                Id = NextId(nameof(OverClauseWindowFrameClauseLink)),
                OwnerId = overClause.Id,
                ValueId = windowFrameClause.GetId(nameof(WindowFrameClause))
            });
        }

        return BuiltNode.Create((nameof(OverClause), overClause.Id));
    }

    public BuiltNode AttachOverClause(BuiltNode functionCall, BuiltNode overClause)
    {
        model.FunctionCallOverClauseLinkList.Add(new FunctionCallOverClauseLink
        {
            Id = NextId(nameof(FunctionCallOverClauseLink)),
            OwnerId = functionCall.GetId(nameof(FunctionCall)),
            ValueId = overClause.GetId(nameof(OverClause))
        });

        return functionCall;
    }
}
