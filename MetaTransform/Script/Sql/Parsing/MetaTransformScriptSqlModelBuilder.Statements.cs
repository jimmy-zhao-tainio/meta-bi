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
            CommonTableExpression = commonTableExpression,
            Identifier = expressionName.GetRef<Identifier>(nameof(Identifier))
        });
        model.CommonTableExpressionQueryExpressionLinkList.Add(new CommonTableExpressionQueryExpressionLink
        {
            Id = NextId(nameof(CommonTableExpressionQueryExpressionLink)),
            CommonTableExpression = commonTableExpression,
            QueryExpression = queryExpression.GetRef<QueryExpression>(nameof(QueryExpression))
        });

        for (var ordinal = 0; columns is not null && ordinal < columns.Count; ordinal++)
        {
            model.CommonTableExpressionColumnsItemList.Add(new CommonTableExpressionColumnsItem
            {
                Id = NextId(nameof(CommonTableExpressionColumnsItem)),
                CommonTableExpression = commonTableExpression,
                Identifier = columns[ordinal].GetRef<Identifier>(nameof(Identifier)),
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
        var statementBase = CreateStatementWithCtesAndXmlNamespaces(commonTableExpressions, xmlNamespaces);
        var statementWithCtesId = statementBase.GetId(nameof(StatementWithCtesAndXmlNamespaces));

        var selectStatement = new SelectStatement
        {
            Id = NextId(nameof(SelectStatement)),
            StatementWithCtesAndXmlNamespaces = (StatementWithCtesAndXmlNamespaces)ResolveBuiltNodeReference(nameof(StatementWithCtesAndXmlNamespaces), statementWithCtesId)
        };
        model.SelectStatementList.Add(selectStatement);
        model.SelectStatementQueryExpressionLinkList.Add(new SelectStatementQueryExpressionLink
        {
            Id = NextId(nameof(SelectStatementQueryExpressionLink)),
            SelectStatement = selectStatement,
            QueryExpression = queryExpression.GetRef<QueryExpression>(nameof(QueryExpression))
        });

        return BuiltNode.Create(
            (nameof(TSqlStatement), statementBase.GetId(nameof(TSqlStatement))),
            (nameof(StatementWithCtesAndXmlNamespaces), statementWithCtesId),
            (nameof(SelectStatement), selectStatement.Id));
    }

    public BuiltNode CreateInsertStatement(
        BuiltNode target,
        BuiltNode source,
        IReadOnlyList<BuiltNode>? columns = null,
        IReadOnlyList<BuiltNode>? commonTableExpressions = null)
    {
        var statementBase = CreateStatementWithCtesAndXmlNamespaces(commonTableExpressions, xmlNamespaces: null);
        var statementWithCtesId = statementBase.GetId(nameof(StatementWithCtesAndXmlNamespaces));

        var insertStatement = new InsertStatement
        {
            Id = NextId(nameof(InsertStatement)),
            StatementWithCtesAndXmlNamespaces = (StatementWithCtesAndXmlNamespaces)ResolveBuiltNodeReference(nameof(StatementWithCtesAndXmlNamespaces), statementWithCtesId)
        };
        model.InsertStatementList.Add(insertStatement);
        model.InsertStatementTargetLinkList.Add(new InsertStatementTargetLink
        {
            Id = NextId(nameof(InsertStatementTargetLink)),
            InsertStatement = insertStatement,
            SchemaObjectName = target.GetRef<SchemaObjectName>(nameof(SchemaObjectName))
        });
        model.InsertStatementSourceLinkList.Add(new InsertStatementSourceLink
        {
            Id = NextId(nameof(InsertStatementSourceLink)),
            InsertStatement = insertStatement,
            InsertSource = source.GetRef<InsertSource>(nameof(InsertSource))
        });

        AddIdentifierItems(
            columns,
            (id, identifierId, ordinal) => new InsertStatementColumnsItem
            {
                Id = id,
                InsertStatement = insertStatement,
                Identifier = (Identifier)ResolveBuiltNodeReference(nameof(Identifier), identifierId),
                Ordinal = ordinal
            },
            model.InsertStatementColumnsItemList,
            nameof(InsertStatementColumnsItem));

        return BuiltNode.Create(
            (nameof(TSqlStatement), statementBase.GetId(nameof(TSqlStatement))),
            (nameof(StatementWithCtesAndXmlNamespaces), statementWithCtesId),
            (nameof(InsertStatement), insertStatement.Id));
    }

    public BuiltNode CreateInsertQuerySource(BuiltNode queryExpression)
    {
        var source = new InsertSource
        {
            Id = NextId(nameof(InsertSource))
        };
        model.InsertSourceList.Add(source);

        var querySource = new InsertQuerySource
        {
            Id = NextId(nameof(InsertQuerySource)),
            InsertSource = source
        };
        model.InsertQuerySourceList.Add(querySource);
        model.InsertQuerySourceQueryExpressionLinkList.Add(new InsertQuerySourceQueryExpressionLink
        {
            Id = NextId(nameof(InsertQuerySourceQueryExpressionLink)),
            InsertQuerySource = querySource,
            QueryExpression = queryExpression.GetRef<QueryExpression>(nameof(QueryExpression))
        });

        return BuiltNode.Create(
            (nameof(InsertSource), source.Id),
            (nameof(InsertQuerySource), querySource.Id));
    }

    public BuiltNode CreateInsertValuesSource(IReadOnlyList<BuiltNode> rowValues)
    {
        var source = new InsertSource
        {
            Id = NextId(nameof(InsertSource))
        };
        model.InsertSourceList.Add(source);

        var valuesSource = new InsertValuesSource
        {
            Id = NextId(nameof(InsertValuesSource)),
            InsertSource = source
        };
        model.InsertValuesSourceList.Add(valuesSource);

        for (var ordinal = 0; ordinal < rowValues.Count; ordinal++)
        {
            model.InsertValuesSourceRowValuesItemList.Add(new InsertValuesSourceRowValuesItem
            {
                Id = NextId(nameof(InsertValuesSourceRowValuesItem)),
                InsertValuesSource = valuesSource,
                RowValue = rowValues[ordinal].GetRef<RowValue>(nameof(RowValue)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(InsertSource), source.Id),
            (nameof(InsertValuesSource), valuesSource.Id));
    }

    public BuiltNode CreateUpdateStatement(
        BuiltNode target,
        BuiltNode setClause,
        BuiltNode? targetAlias = null,
        BuiltNode? fromClause = null,
        BuiltNode? whereClause = null,
        IReadOnlyList<BuiltNode>? commonTableExpressions = null)
    {
        var statementBase = CreateStatementWithCtesAndXmlNamespaces(commonTableExpressions, xmlNamespaces: null);
        var statementWithCtesId = statementBase.GetId(nameof(StatementWithCtesAndXmlNamespaces));

        var updateStatement = new UpdateStatement
        {
            Id = NextId(nameof(UpdateStatement)),
            StatementWithCtesAndXmlNamespaces = (StatementWithCtesAndXmlNamespaces)ResolveBuiltNodeReference(nameof(StatementWithCtesAndXmlNamespaces), statementWithCtesId)
        };
        model.UpdateStatementList.Add(updateStatement);
        model.UpdateStatementTargetLinkList.Add(new UpdateStatementTargetLink
        {
            Id = NextId(nameof(UpdateStatementTargetLink)),
            UpdateStatement = updateStatement,
            SchemaObjectName = target.GetRef<SchemaObjectName>(nameof(SchemaObjectName))
        });
        model.UpdateStatementSetClauseLinkList.Add(new UpdateStatementSetClauseLink
        {
            Id = NextId(nameof(UpdateStatementSetClauseLink)),
            UpdateStatement = updateStatement,
            SetClause = setClause.GetRef<SetClause>(nameof(SetClause))
        });

        if (targetAlias is not null)
        {
            model.UpdateStatementTargetAliasLinkList.Add(new UpdateStatementTargetAliasLink
            {
                Id = NextId(nameof(UpdateStatementTargetAliasLink)),
                UpdateStatement = updateStatement,
                Identifier = targetAlias.GetRef<Identifier>(nameof(Identifier))
            });
        }

        if (fromClause is not null)
        {
            model.UpdateStatementFromClauseLinkList.Add(new UpdateStatementFromClauseLink
            {
                Id = NextId(nameof(UpdateStatementFromClauseLink)),
                UpdateStatement = updateStatement,
                FromClause = fromClause.GetRef<FromClause>(nameof(FromClause))
            });
        }

        if (whereClause is not null)
        {
            model.UpdateStatementWhereClauseLinkList.Add(new UpdateStatementWhereClauseLink
            {
                Id = NextId(nameof(UpdateStatementWhereClauseLink)),
                UpdateStatement = updateStatement,
                WhereClause = whereClause.GetRef<WhereClause>(nameof(WhereClause))
            });
        }

        return BuiltNode.Create(
            (nameof(TSqlStatement), statementBase.GetId(nameof(TSqlStatement))),
            (nameof(StatementWithCtesAndXmlNamespaces), statementWithCtesId),
            (nameof(UpdateStatement), updateStatement.Id));
    }

    public BuiltNode CreateDeleteStatement(
        BuiltNode target,
        BuiltNode? fromClause = null,
        BuiltNode? whereClause = null,
        IReadOnlyList<BuiltNode>? commonTableExpressions = null)
    {
        var statementBase = CreateStatementWithCtesAndXmlNamespaces(commonTableExpressions, xmlNamespaces: null);
        var statementWithCtesId = statementBase.GetId(nameof(StatementWithCtesAndXmlNamespaces));

        var deleteStatement = new DeleteStatement
        {
            Id = NextId(nameof(DeleteStatement)),
            StatementWithCtesAndXmlNamespaces = (StatementWithCtesAndXmlNamespaces)ResolveBuiltNodeReference(nameof(StatementWithCtesAndXmlNamespaces), statementWithCtesId)
        };
        model.DeleteStatementList.Add(deleteStatement);
        model.DeleteStatementTargetLinkList.Add(new DeleteStatementTargetLink
        {
            Id = NextId(nameof(DeleteStatementTargetLink)),
            DeleteStatement = deleteStatement,
            SchemaObjectName = target.GetRef<SchemaObjectName>(nameof(SchemaObjectName))
        });

        if (fromClause is not null)
        {
            model.DeleteStatementFromClauseLinkList.Add(new DeleteStatementFromClauseLink
            {
                Id = NextId(nameof(DeleteStatementFromClauseLink)),
                DeleteStatement = deleteStatement,
                FromClause = fromClause.GetRef<FromClause>(nameof(FromClause))
            });
        }

        if (whereClause is not null)
        {
            model.DeleteStatementWhereClauseLinkList.Add(new DeleteStatementWhereClauseLink
            {
                Id = NextId(nameof(DeleteStatementWhereClauseLink)),
                DeleteStatement = deleteStatement,
                WhereClause = whereClause.GetRef<WhereClause>(nameof(WhereClause))
            });
        }

        return BuiltNode.Create(
            (nameof(TSqlStatement), statementBase.GetId(nameof(TSqlStatement))),
            (nameof(StatementWithCtesAndXmlNamespaces), statementWithCtesId),
            (nameof(DeleteStatement), deleteStatement.Id));
    }

    public BuiltNode CreateTruncateStatement(BuiltNode target)
    {
        var sqlStatement = new TSqlStatement
        {
            Id = NextId(nameof(TSqlStatement))
        };
        model.TSqlStatementList.Add(sqlStatement);

        var truncateStatement = new TruncateStatement
        {
            Id = NextId(nameof(TruncateStatement)),
            TSqlStatement = sqlStatement
        };
        model.TruncateStatementList.Add(truncateStatement);
        model.TruncateStatementTargetLinkList.Add(new TruncateStatementTargetLink
        {
            Id = NextId(nameof(TruncateStatementTargetLink)),
            TruncateStatement = truncateStatement,
            SchemaObjectName = target.GetRef<SchemaObjectName>(nameof(SchemaObjectName))
        });

        return BuiltNode.Create(
            (nameof(TSqlStatement), sqlStatement.Id),
            (nameof(TruncateStatement), truncateStatement.Id));
    }

    public BuiltNode CreateMergeStatement(
        BuiltNode target,
        BuiltNode source,
        BuiltNode searchCondition,
        IReadOnlyList<BuiltNode> whenClauses,
        BuiltNode? targetAlias = null,
        IReadOnlyList<BuiltNode>? commonTableExpressions = null,
        BuiltNode? topRowFilter = null,
        IReadOnlyList<BuiltNode>? targetHints = null,
        BuiltNode? outputClause = null,
        BuiltNode? optionClause = null)
    {
        var statementBase = CreateStatementWithCtesAndXmlNamespaces(commonTableExpressions, xmlNamespaces: null);
        var statementWithCtesId = statementBase.GetId(nameof(StatementWithCtesAndXmlNamespaces));

        var mergeStatement = new MergeStatement
        {
            Id = NextId(nameof(MergeStatement)),
            StatementWithCtesAndXmlNamespaces = (StatementWithCtesAndXmlNamespaces)ResolveBuiltNodeReference(nameof(StatementWithCtesAndXmlNamespaces), statementWithCtesId)
        };
        model.MergeStatementList.Add(mergeStatement);
        model.MergeStatementTargetLinkList.Add(new MergeStatementTargetLink
        {
            Id = NextId(nameof(MergeStatementTargetLink)),
            MergeStatement = mergeStatement,
            SchemaObjectName = target.GetRef<SchemaObjectName>(nameof(SchemaObjectName))
        });
        model.MergeStatementSourceLinkList.Add(new MergeStatementSourceLink
        {
            Id = NextId(nameof(MergeStatementSourceLink)),
            MergeStatement = mergeStatement,
            TableReference = source.GetRef<TableReference>(nameof(TableReference))
        });
        model.MergeStatementSearchConditionLinkList.Add(new MergeStatementSearchConditionLink
        {
            Id = NextId(nameof(MergeStatementSearchConditionLink)),
            MergeStatement = mergeStatement,
            BooleanExpression = searchCondition.GetRef<BooleanExpression>(nameof(BooleanExpression))
        });

        if (topRowFilter is not null)
        {
            model.MergeStatementTopRowFilterLinkList.Add(new MergeStatementTopRowFilterLink
            {
                Id = NextId(nameof(MergeStatementTopRowFilterLink)),
                MergeStatement = mergeStatement,
                TopRowFilter = topRowFilter.GetRef<TopRowFilter>(nameof(TopRowFilter))
            });
        }

        if (targetAlias is not null)
        {
            model.MergeStatementTargetAliasLinkList.Add(new MergeStatementTargetAliasLink
            {
                Id = NextId(nameof(MergeStatementTargetAliasLink)),
                MergeStatement = mergeStatement,
                Identifier = targetAlias.GetRef<Identifier>(nameof(Identifier))
            });
        }

        for (var ordinal = 0; targetHints is not null && ordinal < targetHints.Count; ordinal++)
        {
            model.MergeStatementTargetHintsItemList.Add(new MergeStatementTargetHintsItem
            {
                Id = NextId(nameof(MergeStatementTargetHintsItem)),
                MergeStatement = mergeStatement,
                SqlHint = targetHints[ordinal].GetRef<SqlHint>(nameof(SqlHint)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        for (var ordinal = 0; ordinal < whenClauses.Count; ordinal++)
        {
            model.MergeStatementWhenClausesItemList.Add(new MergeStatementWhenClausesItem
            {
                Id = NextId(nameof(MergeStatementWhenClausesItem)),
                MergeStatement = mergeStatement,
                MergeWhenClause = whenClauses[ordinal].GetRef<MergeWhenClause>(nameof(MergeWhenClause)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        if (outputClause is not null)
        {
            model.MergeStatementOutputClauseLinkList.Add(new MergeStatementOutputClauseLink
            {
                Id = NextId(nameof(MergeStatementOutputClauseLink)),
                MergeStatement = mergeStatement,
                OutputClause = outputClause.GetRef<OutputClause>(nameof(OutputClause))
            });
        }

        if (optionClause is not null)
        {
            model.MergeStatementOptionClauseLinkList.Add(new MergeStatementOptionClauseLink
            {
                Id = NextId(nameof(MergeStatementOptionClauseLink)),
                MergeStatement = mergeStatement,
                OptionClause = optionClause.GetRef<OptionClause>(nameof(OptionClause))
            });
        }

        return BuiltNode.Create(
            (nameof(TSqlStatement), statementBase.GetId(nameof(TSqlStatement))),
            (nameof(StatementWithCtesAndXmlNamespaces), statementWithCtesId),
            (nameof(MergeStatement), mergeStatement.Id));
    }

    public BuiltNode CreateOutputClause(
        IReadOnlyList<BuiltNode> selectElements,
        BuiltNode? intoTarget = null,
        IReadOnlyList<BuiltNode>? intoColumns = null)
    {
        var outputClause = new OutputClause
        {
            Id = NextId(nameof(OutputClause))
        };
        model.OutputClauseList.Add(outputClause);

        for (var ordinal = 0; ordinal < selectElements.Count; ordinal++)
        {
            model.OutputClauseSelectElementsItemList.Add(new OutputClauseSelectElementsItem
            {
                Id = NextId(nameof(OutputClauseSelectElementsItem)),
                OutputClause = outputClause,
                SelectElement = selectElements[ordinal].GetRef<SelectElement>(nameof(SelectElement)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        if (intoTarget is not null)
        {
            model.OutputClauseIntoTargetLinkList.Add(new OutputClauseIntoTargetLink
            {
                Id = NextId(nameof(OutputClauseIntoTargetLink)),
                OutputClause = outputClause,
                SchemaObjectName = intoTarget.GetRef<SchemaObjectName>(nameof(SchemaObjectName))
            });
        }

        AddIdentifierItems(
            intoColumns,
            (id, identifierId, ordinal) => new OutputClauseIntoColumnsItem
            {
                Id = id,
                OutputClause = outputClause,
                Identifier = (Identifier)ResolveBuiltNodeReference(nameof(Identifier), identifierId),
                Ordinal = ordinal
            },
            model.OutputClauseIntoColumnsItemList,
            nameof(OutputClauseIntoColumnsItem));

        return BuiltNode.Create((nameof(OutputClause), outputClause.Id));
    }

    public BuiltNode CreateOptionClause(IReadOnlyList<BuiltNode> queryHints)
    {
        var optionClause = new OptionClause
        {
            Id = NextId(nameof(OptionClause))
        };
        model.OptionClauseList.Add(optionClause);

        for (var ordinal = 0; ordinal < queryHints.Count; ordinal++)
        {
            model.OptionClauseQueryHintsItemList.Add(new OptionClauseQueryHintsItem
            {
                Id = NextId(nameof(OptionClauseQueryHintsItem)),
                OptionClause = optionClause,
                SqlHint = queryHints[ordinal].GetRef<SqlHint>(nameof(SqlHint)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create((nameof(OptionClause), optionClause.Id));
    }

    public BuiltNode CreateSqlHint(
        IReadOnlyList<BuiltNode> keywords,
        IReadOnlyList<BuiltNode>? arguments = null,
        string argumentStyle = "None")
    {
        var sqlHint = new SqlHint
        {
            Id = NextId(nameof(SqlHint)),
            ArgumentStyle = argumentStyle
        };
        model.SqlHintList.Add(sqlHint);

        AddIdentifierItems(
            keywords,
            (id, identifierId, ordinal) => new SqlHintKeywordsItem
            {
                Id = id,
                SqlHint = sqlHint,
                Identifier = (Identifier)ResolveBuiltNodeReference(nameof(Identifier), identifierId),
                Ordinal = ordinal
            },
            model.SqlHintKeywordsItemList,
            nameof(SqlHintKeywordsItem));

        for (var ordinal = 0; arguments is not null && ordinal < arguments.Count; ordinal++)
        {
            model.SqlHintArgumentsItemList.Add(new SqlHintArgumentsItem
            {
                Id = NextId(nameof(SqlHintArgumentsItem)),
                SqlHint = sqlHint,
                ScalarExpression = arguments[ordinal].GetRef<ScalarExpression>(nameof(ScalarExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create((nameof(SqlHint), sqlHint.Id));
    }

    public BuiltNode CreateSetClause(IReadOnlyList<BuiltNode> assignments)
    {
        var setClause = new SetClause
        {
            Id = NextId(nameof(SetClause))
        };
        model.SetClauseList.Add(setClause);

        for (var ordinal = 0; ordinal < assignments.Count; ordinal++)
        {
            model.SetClauseAssignmentsItemList.Add(new SetClauseAssignmentsItem
            {
                Id = NextId(nameof(SetClauseAssignmentsItem)),
                SetClause = setClause,
                SetAssignment = assignments[ordinal].GetRef<SetAssignment>(nameof(SetAssignment)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create((nameof(SetClause), setClause.Id));
    }

    public BuiltNode CreateSetAssignment(BuiltNode target, BuiltNode value)
    {
        var assignment = new SetAssignment
        {
            Id = NextId(nameof(SetAssignment))
        };
        model.SetAssignmentList.Add(assignment);
        model.SetAssignmentTargetLinkList.Add(new SetAssignmentTargetLink
        {
            Id = NextId(nameof(SetAssignmentTargetLink)),
            SetAssignment = assignment,
            ColumnReferenceExpression = target.GetRef<ColumnReferenceExpression>(nameof(ColumnReferenceExpression))
        });
        model.SetAssignmentValueLinkList.Add(new SetAssignmentValueLink
        {
            Id = NextId(nameof(SetAssignmentValueLink)),
            SetAssignment = assignment,
            ScalarExpression = value.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });

        return BuiltNode.Create((nameof(SetAssignment), assignment.Id));
    }

    public BuiltNode CreateMergeWhenClause(string matchKind, BuiltNode action, BuiltNode? searchCondition = null)
    {
        var whenClause = new MergeWhenClause
        {
            Id = NextId(nameof(MergeWhenClause)),
            MatchKind = matchKind
        };
        model.MergeWhenClauseList.Add(whenClause);
        model.MergeWhenClauseActionLinkList.Add(new MergeWhenClauseActionLink
        {
            Id = NextId(nameof(MergeWhenClauseActionLink)),
            MergeWhenClause = whenClause,
            MergeAction = action.GetRef<MergeAction>(nameof(MergeAction))
        });

        if (searchCondition is not null)
        {
            model.MergeWhenClauseSearchConditionLinkList.Add(new MergeWhenClauseSearchConditionLink
            {
                Id = NextId(nameof(MergeWhenClauseSearchConditionLink)),
                MergeWhenClause = whenClause,
                BooleanExpression = searchCondition.GetRef<BooleanExpression>(nameof(BooleanExpression))
            });
        }

        return BuiltNode.Create((nameof(MergeWhenClause), whenClause.Id));
    }

    public BuiltNode CreateMergeUpdateAction(BuiltNode setClause)
    {
        var action = new MergeAction
        {
            Id = NextId(nameof(MergeAction))
        };
        model.MergeActionList.Add(action);

        var updateAction = new MergeUpdateAction
        {
            Id = NextId(nameof(MergeUpdateAction)),
            MergeAction = action
        };
        model.MergeUpdateActionList.Add(updateAction);
        model.MergeUpdateActionSetClauseLinkList.Add(new MergeUpdateActionSetClauseLink
        {
            Id = NextId(nameof(MergeUpdateActionSetClauseLink)),
            MergeUpdateAction = updateAction,
            SetClause = setClause.GetRef<SetClause>(nameof(SetClause))
        });

        return BuiltNode.Create(
            (nameof(MergeAction), action.Id),
            (nameof(MergeUpdateAction), updateAction.Id));
    }

    public BuiltNode CreateMergeDeleteAction()
    {
        var action = new MergeAction
        {
            Id = NextId(nameof(MergeAction))
        };
        model.MergeActionList.Add(action);

        var deleteAction = new MergeDeleteAction
        {
            Id = NextId(nameof(MergeDeleteAction)),
            MergeAction = action
        };
        model.MergeDeleteActionList.Add(deleteAction);

        return BuiltNode.Create(
            (nameof(MergeAction), action.Id),
            (nameof(MergeDeleteAction), deleteAction.Id));
    }

    public BuiltNode CreateMergeInsertAction(IReadOnlyList<BuiltNode>? columns, IReadOnlyList<BuiltNode> values)
    {
        var action = new MergeAction
        {
            Id = NextId(nameof(MergeAction))
        };
        model.MergeActionList.Add(action);

        var insertAction = new MergeInsertAction
        {
            Id = NextId(nameof(MergeInsertAction)),
            MergeAction = action
        };
        model.MergeInsertActionList.Add(insertAction);

        AddIdentifierItems(
            columns,
            (id, identifierId, ordinal) => new MergeInsertActionColumnsItem
            {
                Id = id,
                MergeInsertAction = insertAction,
                Identifier = (Identifier)ResolveBuiltNodeReference(nameof(Identifier), identifierId),
                Ordinal = ordinal
            },
            model.MergeInsertActionColumnsItemList,
            nameof(MergeInsertActionColumnsItem));

        for (var ordinal = 0; ordinal < values.Count; ordinal++)
        {
            model.MergeInsertActionValuesItemList.Add(new MergeInsertActionValuesItem
            {
                Id = NextId(nameof(MergeInsertActionValuesItem)),
                MergeInsertAction = insertAction,
                ScalarExpression = values[ordinal].GetRef<ScalarExpression>(nameof(ScalarExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(MergeAction), action.Id),
            (nameof(MergeInsertAction), insertAction.Id));
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
            QueryExpression = queryExpression,
            BinaryQueryExpressionType = binaryQueryExpressionType,
            All = all ? "true" : string.Empty
        };
        model.BinaryQueryExpressionList.Add(binaryQueryExpression);
        model.BinaryQueryExpressionFirstQueryExpressionLinkList.Add(new BinaryQueryExpressionFirstQueryExpressionLink
        {
            Id = NextId(nameof(BinaryQueryExpressionFirstQueryExpressionLink)),
            BinaryQueryExpression = binaryQueryExpression,
            QueryExpression = firstQueryExpression.GetRef<QueryExpression>(nameof(QueryExpression))
        });
        model.BinaryQueryExpressionSecondQueryExpressionLinkList.Add(new BinaryQueryExpressionSecondQueryExpressionLink
        {
            Id = NextId(nameof(BinaryQueryExpressionSecondQueryExpressionLink)),
            BinaryQueryExpression = binaryQueryExpression,
            QueryExpression = secondQueryExpression.GetRef<QueryExpression>(nameof(QueryExpression))
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
            QueryExpression = parent
        };
        model.QueryParenthesisExpressionList.Add(queryParenthesisExpression);
        model.QueryParenthesisExpressionQueryExpressionLinkList.Add(new QueryParenthesisExpressionQueryExpressionLink
        {
            Id = NextId(nameof(QueryParenthesisExpressionQueryExpressionLink)),
            QueryParenthesisExpression = queryParenthesisExpression,
            QueryExpression = queryExpression.GetRef<QueryExpression>(nameof(QueryExpression))
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
            QueryExpression = queryExpression.GetRef<QueryExpression>(nameof(QueryExpression)),
            OrderByClause = orderByClause.GetRef<OrderByClause>(nameof(OrderByClause))
        });

        return queryExpression;
    }

    private BuiltNode CreateStatementWithCtesAndXmlNamespaces(
        IReadOnlyList<BuiltNode>? commonTableExpressions,
        BuiltNode? xmlNamespaces)
    {
        var sqlStatement = new TSqlStatement
        {
            Id = NextId(nameof(TSqlStatement))
        };
        model.TSqlStatementList.Add(sqlStatement);

        var statementWithCtes = new StatementWithCtesAndXmlNamespaces
        {
            Id = NextId(nameof(StatementWithCtesAndXmlNamespaces)),
            TSqlStatement = sqlStatement
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
                StatementWithCtesAndXmlNamespaces = statementWithCtes,
                WithCtesAndXmlNamespaces = withCtesAndXmlNamespaces
            });

            if (xmlNamespaces is not null)
            {
                model.WithCtesAndXmlNamespacesXmlNamespacesLinkList.Add(new WithCtesAndXmlNamespacesXmlNamespacesLink
                {
                    Id = NextId(nameof(WithCtesAndXmlNamespacesXmlNamespacesLink)),
                    WithCtesAndXmlNamespaces = withCtesAndXmlNamespaces,
                    XmlNamespaces = xmlNamespaces.GetRef<XmlNamespaces>(nameof(XmlNamespaces))
                });
            }

            for (var ordinal = 0; commonTableExpressions is not null && ordinal < commonTableExpressions.Count; ordinal++)
            {
                model.WithCtesAndXmlNamespacesCommonTableExpressionsItemList.Add(new WithCtesAndXmlNamespacesCommonTableExpressionsItem
                {
                    Id = NextId(nameof(WithCtesAndXmlNamespacesCommonTableExpressionsItem)),
                    WithCtesAndXmlNamespaces = withCtesAndXmlNamespaces,
                    CommonTableExpression = commonTableExpressions[ordinal].GetRef<CommonTableExpression>(nameof(CommonTableExpression)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        return BuiltNode.Create(
            (nameof(TSqlStatement), sqlStatement.Id),
            (nameof(StatementWithCtesAndXmlNamespaces), statementWithCtes.Id));
    }

    private delegate TItem CreateIdentifierItem<out TItem>(string id, string identifierId, string ordinal);

    private void AddIdentifierItems<TItem>(
        IReadOnlyList<BuiltNode>? identifiers,
        CreateIdentifierItem<TItem> itemFactory,
        ICollection<TItem> destination,
        string entityName)
    {
        for (var ordinal = 0; identifiers is not null && ordinal < identifiers.Count; ordinal++)
        {
            destination.Add(itemFactory(
                NextId(entityName),
                identifiers[ordinal].GetId(nameof(Identifier)),
                ordinal.ToString(CultureInfo.InvariantCulture)));
        }
    }

    public void AddTransformScript(
        string name,
        string targetSqlIdentifier,
        string? sourcePath,
        BuiltNode statement,
        BuiltNode? schemaIdentifier,
        BuiltNode? objectIdentifier,
        IReadOnlyList<BuiltNode>? viewColumns = null,
        string? scriptObjectKind = null,
        IReadOnlyList<(BuiltNode ParameterName, BuiltNode DataTypeReference)>? functionParameters = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Transform script name cannot be empty.");
        }

        if (model.TransformScriptList.Any(existing => string.Equals(existing.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Transform script '{name}' already exists in this workspace.");
        }

        var row = new TransformScript
        {
            Id = NextId(nameof(TransformScript)),
            Name = name,
            SourcePath = sourcePath ?? string.Empty
        };
        model.TransformScriptList.Add(row);

        var normalizedScriptObjectKind = string.IsNullOrWhiteSpace(scriptObjectKind)
            ? "View"
            : scriptObjectKind.Trim();
        var normalizedTargetSqlIdentifier = targetSqlIdentifier?.Trim() ?? string.Empty;
        if (string.Equals(normalizedScriptObjectKind, "InlineTableValuedFunction", StringComparison.OrdinalIgnoreCase))
        {
            model.ScriptObjectTVFList.Add(new ScriptObjectTVF
            {
                Id = NextId(nameof(ScriptObjectTVF)),
                TransformScript = row
            });
        }
        else if (!string.IsNullOrWhiteSpace(normalizedTargetSqlIdentifier))
        {
            model.ScriptObjectViewList.Add(new ScriptObjectView
            {
                Id = NextId(nameof(ScriptObjectView)),
                TransformScript = row,
                TargetSqlIdentifier = normalizedTargetSqlIdentifier
            });
        }

        model.TransformScriptStatementLinkList.Add(new TransformScriptStatementLink
        {
            Id = NextId(nameof(TransformScriptStatementLink)),
            TransformScript = row,
            TSqlStatement = statement.GetRef<TSqlStatement>(nameof(TSqlStatement))
        });

        if (schemaIdentifier is not null)
        {
            model.TransformScriptSchemaIdentifierLinkList.Add(new TransformScriptSchemaIdentifierLink
            {
                Id = NextId(nameof(TransformScriptSchemaIdentifierLink)),
                TransformScript = row,
                Identifier = schemaIdentifier.GetRef<Identifier>(nameof(Identifier))
            });
        }

        if (objectIdentifier is not null)
        {
            model.TransformScriptObjectIdentifierLinkList.Add(new TransformScriptObjectIdentifierLink
            {
                Id = NextId(nameof(TransformScriptObjectIdentifierLink)),
                TransformScript = row,
                Identifier = objectIdentifier.GetRef<Identifier>(nameof(Identifier))
            });
        }

        if (viewColumns is not null)
        {
            for (var ordinal = 0; ordinal < viewColumns.Count; ordinal++)
            {
                model.TransformScriptViewColumnsItemList.Add(new TransformScriptViewColumnsItem
                {
                    Id = NextId(nameof(TransformScriptViewColumnsItem)),
                    TransformScript = row,
                    Identifier = viewColumns[ordinal].GetRef<Identifier>(nameof(Identifier)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        if (functionParameters is null)
        {
            return;
        }

        for (var ordinal = 0; ordinal < functionParameters.Count; ordinal++)
        {
            var parameter = functionParameters[ordinal];
            model.TransformScriptFunctionParametersItemList.Add(new TransformScriptFunctionParametersItem
            {
                Id = NextId(nameof(TransformScriptFunctionParametersItem)),
                TransformScript = row,
                Identifier = parameter.ParameterName.GetRef<Identifier>(nameof(Identifier)),
                DataTypeReference = parameter.DataTypeReference.GetRef<DataTypeReference>(nameof(DataTypeReference)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }
    }

    public void AddScalarFunctionScript(
        string name,
        string? sourcePath,
        BuiltNode returnDataType,
        BuiltNode returnExpression,
        BuiltNode? schemaIdentifier,
        BuiltNode? objectIdentifier,
        IReadOnlyList<(BuiltNode ParameterName, BuiltNode DataTypeReference)>? functionParameters = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Transform script name cannot be empty.");
        }

        if (model.TransformScriptList.Any(existing => string.Equals(existing.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Transform script '{name}' already exists in this workspace.");
        }

        var row = new TransformScript
        {
            Id = NextId(nameof(TransformScript)),
            Name = name,
            SourcePath = sourcePath ?? string.Empty
        };
        model.TransformScriptList.Add(row);
        model.ScriptObjectScalarFunctionList.Add(new ScriptObjectScalarFunction
        {
            Id = NextId(nameof(ScriptObjectScalarFunction)),
            TransformScript = row,
            DataTypeReference = returnDataType.GetRef<DataTypeReference>(nameof(DataTypeReference)),
            ScalarExpression = returnExpression.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });

        if (schemaIdentifier is not null)
        {
            model.TransformScriptSchemaIdentifierLinkList.Add(new TransformScriptSchemaIdentifierLink
            {
                Id = NextId(nameof(TransformScriptSchemaIdentifierLink)),
                TransformScript = row,
                Identifier = schemaIdentifier.GetRef<Identifier>(nameof(Identifier))
            });
        }

        if (objectIdentifier is not null)
        {
            model.TransformScriptObjectIdentifierLinkList.Add(new TransformScriptObjectIdentifierLink
            {
                Id = NextId(nameof(TransformScriptObjectIdentifierLink)),
                TransformScript = row,
                Identifier = objectIdentifier.GetRef<Identifier>(nameof(Identifier))
            });
        }

        if (functionParameters is null)
        {
            return;
        }

        for (var ordinal = 0; ordinal < functionParameters.Count; ordinal++)
        {
            var parameter = functionParameters[ordinal];
            model.TransformScriptFunctionParametersItemList.Add(new TransformScriptFunctionParametersItem
            {
                Id = NextId(nameof(TransformScriptFunctionParametersItem)),
                TransformScript = row,
                Identifier = parameter.ParameterName.GetRef<Identifier>(nameof(Identifier)),
                DataTypeReference = parameter.DataTypeReference.GetRef<DataTypeReference>(nameof(DataTypeReference)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }
    }
}
