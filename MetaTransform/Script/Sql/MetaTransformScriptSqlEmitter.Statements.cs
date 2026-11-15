using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private string RenderInsertStatementBody(InsertStatement insertStatement)
    {
        var target = RenderSchemaObjectName(GetOwnerLink(
            model.InsertStatementTargetLinkList, static row => row.InsertStatement?.Id, nameof(InsertStatementTargetLink.InsertStatement),
            insertStatement.Id,
            "InsertStatement.Target").SchemaObjectName);
        var columns = GetOrderedItems(model.InsertStatementColumnsItemList, static row => row.InsertStatement?.Id, nameof(InsertStatementColumnsItem.InsertStatement), static row => row.Ordinal, insertStatement.Id)
            .Select(row => RenderIdentifier(row.Identifier))
            .ToArray();
        var columnList = columns.Length == 0
            ? string.Empty
            : " (" + string.Join(", ", columns) + ")";
        var source = GetOwnerLink(
            model.InsertStatementSourceLinkList, static row => row.InsertStatement?.Id, nameof(InsertStatementSourceLink.InsertStatement),
            insertStatement.Id,
            "InsertStatement.Source").InsertSource;

        return $"INSERT INTO {target}{columnList}{Environment.NewLine}{RenderInsertSource(source)}";
    }

    private string RenderInsertSource(InsertSource source)
    {
        var querySource = FindByBaseId(model.InsertQuerySourceList, static row => row.InsertSource?.Id, nameof(InsertQuerySource.InsertSource), source.Id);
        if (querySource is not null)
        {
            var queryExpression = GetOwnerLink(
                model.InsertQuerySourceQueryExpressionLinkList, static row => row.InsertQuerySource?.Id, nameof(InsertQuerySourceQueryExpressionLink.InsertQuerySource),
                querySource.Id,
                "InsertQuerySource.QueryExpression").QueryExpression;
            return RenderQueryExpression(queryExpression);
        }

        var valuesSource = FindByBaseId(model.InsertValuesSourceList, static row => row.InsertSource?.Id, nameof(InsertValuesSource.InsertSource), source.Id);
        if (valuesSource is not null)
        {
            var rows = GetOrderedItems(model.InsertValuesSourceRowValuesItemList, static row => row.InsertValuesSource?.Id, nameof(InsertValuesSourceRowValuesItem.InsertValuesSource), static row => row.Ordinal, valuesSource.Id)
                .Select(row => RenderRowValue(row.RowValue))
                .ToArray();
            return "VALUES" + Environment.NewLine + "    " + string.Join("," + Environment.NewLine + "    ", rows);
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript InsertSource id '{source.Id}'.");
    }

    private string RenderUpdateStatementBody(UpdateStatement updateStatement)
    {
        var target = RenderSchemaObjectName(GetOwnerLink(
            model.UpdateStatementTargetLinkList, static row => row.UpdateStatement?.Id, nameof(UpdateStatementTargetLink.UpdateStatement),
            updateStatement.Id,
            "UpdateStatement.Target").SchemaObjectName);
        var aliasLink = FindOwnerLink(model.UpdateStatementTargetAliasLinkList, static row => row.UpdateStatement?.Id, nameof(UpdateStatementTargetAliasLink.UpdateStatement), updateStatement.Id);
        if (aliasLink is not null)
        {
            target += " AS " + RenderIdentifier(aliasLink.Identifier);
        }

        var setClause = GetOwnerLink(
            model.UpdateStatementSetClauseLinkList, static row => row.UpdateStatement?.Id, nameof(UpdateStatementSetClauseLink.UpdateStatement),
            updateStatement.Id,
            "UpdateStatement.SetClause").SetClause;

        var rendered = $"UPDATE {target}{Environment.NewLine}{RenderSetClause(setClause)}";

        var fromClauseLink = FindOwnerLink(model.UpdateStatementFromClauseLinkList, static row => row.UpdateStatement?.Id, nameof(UpdateStatementFromClauseLink.UpdateStatement), updateStatement.Id);
        if (fromClauseLink is not null)
        {
            rendered += Environment.NewLine + "FROM " + RenderFromClause(fromClauseLink.FromClause);
        }

        var whereClauseLink = FindOwnerLink(model.UpdateStatementWhereClauseLinkList, static row => row.UpdateStatement?.Id, nameof(UpdateStatementWhereClauseLink.UpdateStatement), updateStatement.Id);
        if (whereClauseLink is not null)
        {
            rendered += Environment.NewLine + "WHERE " + RenderBooleanExpression(GetOwnerLink(
                model.WhereClauseSearchConditionLinkList, static row => row.WhereClause?.Id, nameof(WhereClauseSearchConditionLink.WhereClause),
                whereClauseLink.WhereClause.Id,
                "UpdateStatement.WhereClause.SearchCondition").BooleanExpression);
        }

        return rendered;
    }

    private string RenderDeleteStatementBody(DeleteStatement deleteStatement)
    {
        var target = RenderSchemaObjectName(GetOwnerLink(
            model.DeleteStatementTargetLinkList, static row => row.DeleteStatement?.Id, nameof(DeleteStatementTargetLink.DeleteStatement),
            deleteStatement.Id,
            "DeleteStatement.Target").SchemaObjectName);

        var fromClauseLink = FindOwnerLink(model.DeleteStatementFromClauseLinkList, static row => row.DeleteStatement?.Id, nameof(DeleteStatementFromClauseLink.DeleteStatement), deleteStatement.Id);
        var rendered = fromClauseLink is null
            ? "DELETE FROM " + target
            : "DELETE " + target + Environment.NewLine + "FROM " + RenderFromClause(fromClauseLink.FromClause);

        var whereClauseLink = FindOwnerLink(model.DeleteStatementWhereClauseLinkList, static row => row.DeleteStatement?.Id, nameof(DeleteStatementWhereClauseLink.DeleteStatement), deleteStatement.Id);
        if (whereClauseLink is not null)
        {
            rendered += Environment.NewLine + "WHERE " + RenderBooleanExpression(GetOwnerLink(
                model.WhereClauseSearchConditionLinkList, static row => row.WhereClause?.Id, nameof(WhereClauseSearchConditionLink.WhereClause),
                whereClauseLink.WhereClause.Id,
                "DeleteStatement.WhereClause.SearchCondition").BooleanExpression);
        }

        return rendered;
    }

    private string RenderTruncateStatementBody(TruncateStatement truncateStatement)
    {
        var target = RenderSchemaObjectName(GetOwnerLink(
            model.TruncateStatementTargetLinkList, static row => row.TruncateStatement?.Id, nameof(TruncateStatementTargetLink.TruncateStatement),
            truncateStatement.Id,
            "TruncateStatement.Target").SchemaObjectName);
        return "TRUNCATE TABLE " + target;
    }

    private string RenderMergeStatementBody(MergeStatement mergeStatement)
    {
        var target = RenderSchemaObjectName(GetOwnerLink(
            model.MergeStatementTargetLinkList, static row => row.MergeStatement?.Id, nameof(MergeStatementTargetLink.MergeStatement),
            mergeStatement.Id,
            "MergeStatement.Target").SchemaObjectName);
        var targetHints = GetOrderedItems(model.MergeStatementTargetHintsItemList, static row => row.MergeStatement?.Id, nameof(MergeStatementTargetHintsItem.MergeStatement), static row => row.Ordinal, mergeStatement.Id)
            .Select(row => RenderSqlHint(row.SqlHint))
            .ToArray();
        if (targetHints.Length > 0)
        {
            target += " WITH (" + string.Join(", ", targetHints) + ")";
        }

        var aliasLink = FindOwnerLink(model.MergeStatementTargetAliasLinkList, static row => row.MergeStatement?.Id, nameof(MergeStatementTargetAliasLink.MergeStatement), mergeStatement.Id);
        if (aliasLink is not null)
        {
            target += " AS " + RenderIdentifier(aliasLink.Identifier);
        }

        var source = RenderTableReference(GetOwnerLink(
            model.MergeStatementSourceLinkList, static row => row.MergeStatement?.Id, nameof(MergeStatementSourceLink.MergeStatement),
            mergeStatement.Id,
            "MergeStatement.Source").TableReference);
        var searchCondition = RenderBooleanExpression(GetOwnerLink(
            model.MergeStatementSearchConditionLinkList, static row => row.MergeStatement?.Id, nameof(MergeStatementSearchConditionLink.MergeStatement),
            mergeStatement.Id,
            "MergeStatement.SearchCondition").BooleanExpression);
        var whenClauses = GetOrderedMergeWhenClauseItems(mergeStatement)
            .Select(row => RenderMergeWhenClause(row.MergeWhenClause))
            .ToArray();

        var topRowFilterLink = FindOwnerLink(model.MergeStatementTopRowFilterLinkList, static row => row.MergeStatement?.Id, nameof(MergeStatementTopRowFilterLink.MergeStatement), mergeStatement.Id);
        var mergeHeader = topRowFilterLink is null
            ? "MERGE"
            : "MERGE " + RenderTopRowFilter(topRowFilterLink.TopRowFilter);

        var rendered = $"{mergeHeader} INTO {target}{Environment.NewLine}USING {source}{Environment.NewLine}ON {searchCondition}{Environment.NewLine}{string.Join(Environment.NewLine, whenClauses)}";

        var outputClauseLink = FindOwnerLink(model.MergeStatementOutputClauseLinkList, static row => row.MergeStatement?.Id, nameof(MergeStatementOutputClauseLink.MergeStatement), mergeStatement.Id);
        if (outputClauseLink is not null)
        {
            rendered += Environment.NewLine + RenderOutputClause(outputClauseLink.OutputClause);
        }

        var optionClauseLink = FindOwnerLink(model.MergeStatementOptionClauseLinkList, static row => row.MergeStatement?.Id, nameof(MergeStatementOptionClauseLink.MergeStatement), mergeStatement.Id);
        if (optionClauseLink is not null)
        {
            rendered += Environment.NewLine + RenderOptionClause(optionClauseLink.OptionClause);
        }

        return rendered + ";";
    }

    private string RenderMergeWhenClause(MergeWhenClause whenClause)
    {
        var forms = new[]
        {
            (Present: FindByBaseId(model.MergeMatchedWhenClauseList, static row => row.MergeWhenClause?.Id, nameof(MergeMatchedWhenClause.MergeWhenClause), whenClause.Id) is not null, Header: "WHEN MATCHED"),
            (Present: FindByBaseId(model.MergeNotMatchedByTargetWhenClauseList, static row => row.MergeWhenClause?.Id, nameof(MergeNotMatchedByTargetWhenClause.MergeWhenClause), whenClause.Id) is not null, Header: "WHEN NOT MATCHED BY TARGET"),
            (Present: FindByBaseId(model.MergeNotMatchedBySourceWhenClauseList, static row => row.MergeWhenClause?.Id, nameof(MergeNotMatchedBySourceWhenClause.MergeWhenClause), whenClause.Id) is not null, Header: "WHEN NOT MATCHED BY SOURCE")
        };
        var headers = forms.Where(static form => form.Present).Select(static form => form.Header).ToArray();
        if (headers.Length != 1)
        {
            throw new InvalidOperationException($"MetaTransformScript MergeWhenClause '{whenClause.Id}' must have exactly one modeled match form.");
        }

        var header = headers[0];

        var searchConditionLink = FindOwnerLink(model.MergeWhenClauseSearchConditionLinkList, static row => row.MergeWhenClause?.Id, nameof(MergeWhenClauseSearchConditionLink.MergeWhenClause), whenClause.Id);
        if (searchConditionLink is not null)
        {
            header += " AND " + RenderBooleanExpression(searchConditionLink.BooleanExpression);
        }

        var action = GetOwnerLink(
            model.MergeWhenClauseActionLinkList, static row => row.MergeWhenClause?.Id, nameof(MergeWhenClauseActionLink.MergeWhenClause),
            whenClause.Id,
            "MergeWhenClause.Action").MergeAction;
        return header + " THEN " + RenderMergeAction(action);
    }

    private IReadOnlyList<MergeStatementWhenClausesItem> GetOrderedMergeWhenClauseItems(MergeStatement mergeStatement)
    {
        var items = model.MergeStatementWhenClausesItemList
            .Where(item => string.Equals(item.MergeStatement.Id, mergeStatement.Id, StringComparison.Ordinal))
            .ToArray();
        if (items.Length == 0)
        {
            throw new InvalidOperationException($"MetaTransformScript MergeStatement '{mergeStatement.Id}' has no WHEN clauses.");
        }

        var itemIds = items.Select(item => item.Id).ToHashSet(StringComparer.Ordinal);
        var successors = new Dictionary<string, MergeStatementWhenClausesItem>(StringComparer.Ordinal);
        MergeStatementWhenClausesItem? head = null;
        foreach (var item in items)
        {
            if (item.PreviousMergeWhenClause is null)
            {
                if (head is not null)
                {
                    throw new InvalidOperationException($"MetaTransformScript MergeStatement '{mergeStatement.Id}' has more than one first WHEN clause.");
                }

                head = item;
                continue;
            }

            var previous = item.PreviousMergeWhenClause;
            if (!itemIds.Contains(previous.Id))
            {
                throw new InvalidOperationException($"MetaTransformScript MergeStatement '{mergeStatement.Id}' links WHEN clause '{item.Id}' to a clause from another statement.");
            }

            if (!successors.TryAdd(previous.Id, item))
            {
                throw new InvalidOperationException($"MetaTransformScript MergeStatement '{mergeStatement.Id}' branches after WHEN clause '{previous.Id}'.");
            }
        }

        if (head is null)
        {
            throw new InvalidOperationException($"MetaTransformScript MergeStatement '{mergeStatement.Id}' has no first WHEN clause.");
        }

        var ordered = new List<MergeStatementWhenClausesItem>(items.Length);
        var visited = new HashSet<string>(StringComparer.Ordinal);
        for (var current = head; current is not null; successors.TryGetValue(current.Id, out current!))
        {
            if (!visited.Add(current.Id))
            {
                throw new InvalidOperationException($"MetaTransformScript MergeStatement '{mergeStatement.Id}' has a cycle in its WHEN clauses.");
            }

            ordered.Add(current);
        }

        if (ordered.Count != items.Length)
        {
            throw new InvalidOperationException($"MetaTransformScript MergeStatement '{mergeStatement.Id}' has unreachable WHEN clauses.");
        }

        return ordered;
    }

    private string RenderMergeAction(MergeAction action)
    {
        var updateAction = FindByBaseId(model.MergeUpdateActionList, static row => row.MergeAction?.Id, nameof(MergeUpdateAction.MergeAction), action.Id);
        if (updateAction is not null)
        {
            var setClause = GetOwnerLink(
                model.MergeUpdateActionSetClauseLinkList, static row => row.MergeUpdateAction?.Id, nameof(MergeUpdateActionSetClauseLink.MergeUpdateAction),
                updateAction.Id,
                "MergeUpdateAction.SetClause").SetClause;
            return "UPDATE " + RenderSetClause(setClause);
        }

        if (FindByBaseId(model.MergeDeleteActionList, static row => row.MergeAction?.Id, nameof(MergeDeleteAction.MergeAction), action.Id) is not null)
        {
            return "DELETE";
        }

        var insertAction = FindByBaseId(model.MergeInsertActionList, static row => row.MergeAction?.Id, nameof(MergeInsertAction.MergeAction), action.Id);
        if (insertAction is not null)
        {
            var columns = GetOrderedItems(model.MergeInsertActionColumnsItemList, static row => row.MergeInsertAction?.Id, nameof(MergeInsertActionColumnsItem.MergeInsertAction), static row => row.Ordinal, insertAction.Id)
                .Select(row => RenderIdentifier(row.Identifier))
                .ToArray();
            var columnList = columns.Length == 0
                ? string.Empty
                : " (" + string.Join(", ", columns) + ")";
            var values = GetOrderedItems(model.MergeInsertActionValuesItemList, static row => row.MergeInsertAction?.Id, nameof(MergeInsertActionValuesItem.MergeInsertAction), static row => row.Ordinal, insertAction.Id)
                .Select(row => RenderScalarExpression(row.ScalarExpression))
                .ToArray();
            return $"INSERT{columnList} VALUES ({string.Join(", ", values)})";
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript MergeAction id '{action.Id}'.");
    }

    private string RenderOutputClause(OutputClause outputClause)
    {
        var selectElements = GetOrderedItems(model.OutputClauseSelectElementsItemList, static row => row.OutputClause?.Id, nameof(OutputClauseSelectElementsItem.OutputClause), static row => row.Ordinal, outputClause.Id)
            .Select(row => RenderSelectElement(row.SelectElement))
            .ToArray();
        var rendered = "OUTPUT" + Environment.NewLine + "    " + string.Join("," + Environment.NewLine + "    ", selectElements);

        var intoTargetLink = FindOwnerLink(model.OutputClauseIntoTargetLinkList, static row => row.OutputClause?.Id, nameof(OutputClauseIntoTargetLink.OutputClause), outputClause.Id);
        if (intoTargetLink is not null)
        {
            var columns = GetOrderedItems(model.OutputClauseIntoColumnsItemList, static row => row.OutputClause?.Id, nameof(OutputClauseIntoColumnsItem.OutputClause), static row => row.Ordinal, outputClause.Id)
                .Select(row => RenderIdentifier(row.Identifier))
                .ToArray();
            var columnList = columns.Length == 0
                ? string.Empty
                : " (" + string.Join(", ", columns) + ")";
            rendered += Environment.NewLine + "INTO " + RenderSchemaObjectName(intoTargetLink.SchemaObjectName) + columnList;
        }

        return rendered;
    }

    private string RenderOptionClause(OptionClause optionClause)
    {
        var hints = GetOrderedItems(model.OptionClauseQueryHintsItemList, static row => row.OptionClause?.Id, nameof(OptionClauseQueryHintsItem.OptionClause), static row => row.Ordinal, optionClause.Id)
            .Select(row => RenderSqlHint(row.SqlHint))
            .ToArray();
        return "OPTION (" + string.Join(", ", hints) + ")";
    }

    private string RenderSqlHint(SqlHint sqlHint)
    {
        var keywords = GetOrderedItems(model.SqlHintKeywordsItemList, static row => row.SqlHint?.Id, nameof(SqlHintKeywordsItem.SqlHint), static row => row.Ordinal, sqlHint.Id)
            .Select(row => RenderIdentifier(row.Identifier))
            .ToArray();
        var rendered = string.Join(" ", keywords);
        var arguments = GetOrderedItems(model.SqlHintArgumentsItemList, static row => row.SqlHint?.Id, nameof(SqlHintArgumentsItem.SqlHint), static row => row.Ordinal, sqlHint.Id)
            .Select(row => RenderScalarExpression(row.ScalarExpression))
            .ToArray();

        return sqlHint.ArgumentStyle switch
        {
            "Parenthesized" => rendered + " (" + string.Join(", ", arguments) + ")",
            "Bare" => rendered + " " + string.Join(", ", arguments),
            "None" or "" => rendered,
            _ => throw new InvalidOperationException($"Unsupported MetaTransformScript SqlHint.ArgumentStyle '{sqlHint.ArgumentStyle}'.")
        };
    }

    private string RenderSetClause(SetClause setClause)
    {
        var assignments = GetOrderedItems(model.SetClauseAssignmentsItemList, static row => row.SetClause?.Id, nameof(SetClauseAssignmentsItem.SetClause), static row => row.Ordinal, setClause.Id)
            .Select(row => RenderSetAssignment(row.SetAssignment))
            .ToArray();
        return "SET " + string.Join("," + Environment.NewLine + "    ", assignments);
    }

    private string RenderSetAssignment(SetAssignment assignment)
    {
        var target = RenderColumnReferenceExpression(GetOwnerLink(
            model.SetAssignmentTargetLinkList, static row => row.SetAssignment?.Id, nameof(SetAssignmentTargetLink.SetAssignment),
            assignment.Id,
            "SetAssignment.Target").ColumnReferenceExpression);
        var value = RenderScalarExpression(GetOwnerLink(
            model.SetAssignmentValueLinkList, static row => row.SetAssignment?.Id, nameof(SetAssignmentValueLink.SetAssignment),
            assignment.Id,
            "SetAssignment.Value").ScalarExpression);
        return target + " = " + value;
    }
}
