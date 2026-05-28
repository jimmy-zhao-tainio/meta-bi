using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private string RenderInsertStatementBody(InsertStatement insertStatement)
    {
        var target = RenderSchemaObjectName(GetOwnerLink(
            model.InsertStatementTargetLinkList,
            insertStatement.Id,
            "InsertStatement.Target").SchemaObjectName);
        var columns = GetOrderedItems(model.InsertStatementColumnsItemList, insertStatement.Id)
            .Select(row => RenderIdentifier(row.Identifier))
            .ToArray();
        var columnList = columns.Length == 0
            ? string.Empty
            : " (" + string.Join(", ", columns) + ")";
        var source = GetOwnerLink(
            model.InsertStatementSourceLinkList,
            insertStatement.Id,
            "InsertStatement.Source").InsertSource;

        return $"INSERT INTO {target}{columnList}{Environment.NewLine}{RenderInsertSource(source)}";
    }

    private string RenderInsertSource(InsertSource source)
    {
        var querySource = FindByBaseId(model.InsertQuerySourceList, source.Id);
        if (querySource is not null)
        {
            var queryExpression = GetOwnerLink(
                model.InsertQuerySourceQueryExpressionLinkList,
                querySource.Id,
                "InsertQuerySource.QueryExpression").QueryExpression;
            return RenderQueryExpression(queryExpression);
        }

        var valuesSource = FindByBaseId(model.InsertValuesSourceList, source.Id);
        if (valuesSource is not null)
        {
            var rows = GetOrderedItems(model.InsertValuesSourceRowValuesItemList, valuesSource.Id)
                .Select(row => RenderRowValue(row.RowValue))
                .ToArray();
            return "VALUES" + Environment.NewLine + "    " + string.Join("," + Environment.NewLine + "    ", rows);
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript InsertSource id '{source.Id}'.");
    }

    private string RenderUpdateStatementBody(UpdateStatement updateStatement)
    {
        var target = RenderSchemaObjectName(GetOwnerLink(
            model.UpdateStatementTargetLinkList,
            updateStatement.Id,
            "UpdateStatement.Target").SchemaObjectName);
        var aliasLink = FindOwnerLink(model.UpdateStatementTargetAliasLinkList, updateStatement.Id);
        if (aliasLink is not null)
        {
            target += " AS " + RenderIdentifier(aliasLink.Identifier);
        }

        var setClause = GetOwnerLink(
            model.UpdateStatementSetClauseLinkList,
            updateStatement.Id,
            "UpdateStatement.SetClause").SetClause;

        var rendered = $"UPDATE {target}{Environment.NewLine}{RenderSetClause(setClause)}";

        var fromClauseLink = FindOwnerLink(model.UpdateStatementFromClauseLinkList, updateStatement.Id);
        if (fromClauseLink is not null)
        {
            rendered += Environment.NewLine + "FROM " + RenderFromClause(fromClauseLink.FromClause);
        }

        var whereClauseLink = FindOwnerLink(model.UpdateStatementWhereClauseLinkList, updateStatement.Id);
        if (whereClauseLink is not null)
        {
            rendered += Environment.NewLine + "WHERE " + RenderBooleanExpression(GetOwnerLink(
                model.WhereClauseSearchConditionLinkList,
                whereClauseLink.WhereClause.Id,
                "UpdateStatement.WhereClause.SearchCondition").BooleanExpression);
        }

        return rendered;
    }

    private string RenderDeleteStatementBody(DeleteStatement deleteStatement)
    {
        var target = RenderSchemaObjectName(GetOwnerLink(
            model.DeleteStatementTargetLinkList,
            deleteStatement.Id,
            "DeleteStatement.Target").SchemaObjectName);

        var fromClauseLink = FindOwnerLink(model.DeleteStatementFromClauseLinkList, deleteStatement.Id);
        var rendered = fromClauseLink is null
            ? "DELETE FROM " + target
            : "DELETE " + target + Environment.NewLine + "FROM " + RenderFromClause(fromClauseLink.FromClause);

        var whereClauseLink = FindOwnerLink(model.DeleteStatementWhereClauseLinkList, deleteStatement.Id);
        if (whereClauseLink is not null)
        {
            rendered += Environment.NewLine + "WHERE " + RenderBooleanExpression(GetOwnerLink(
                model.WhereClauseSearchConditionLinkList,
                whereClauseLink.WhereClause.Id,
                "DeleteStatement.WhereClause.SearchCondition").BooleanExpression);
        }

        return rendered;
    }

    private string RenderTruncateStatementBody(TruncateStatement truncateStatement)
    {
        var target = RenderSchemaObjectName(GetOwnerLink(
            model.TruncateStatementTargetLinkList,
            truncateStatement.Id,
            "TruncateStatement.Target").SchemaObjectName);
        return "TRUNCATE TABLE " + target;
    }

    private string RenderMergeStatementBody(MergeStatement mergeStatement)
    {
        var target = RenderSchemaObjectName(GetOwnerLink(
            model.MergeStatementTargetLinkList,
            mergeStatement.Id,
            "MergeStatement.Target").SchemaObjectName);
        var targetHints = GetOrderedItems(model.MergeStatementTargetHintsItemList, mergeStatement.Id)
            .Select(row => RenderSqlHint(row.SqlHint))
            .ToArray();
        if (targetHints.Length > 0)
        {
            target += " WITH (" + string.Join(", ", targetHints) + ")";
        }

        var aliasLink = FindOwnerLink(model.MergeStatementTargetAliasLinkList, mergeStatement.Id);
        if (aliasLink is not null)
        {
            target += " AS " + RenderIdentifier(aliasLink.Identifier);
        }

        var source = RenderTableReference(GetOwnerLink(
            model.MergeStatementSourceLinkList,
            mergeStatement.Id,
            "MergeStatement.Source").TableReference);
        var searchCondition = RenderBooleanExpression(GetOwnerLink(
            model.MergeStatementSearchConditionLinkList,
            mergeStatement.Id,
            "MergeStatement.SearchCondition").BooleanExpression);
        var whenClauses = GetOrderedItems(model.MergeStatementWhenClausesItemList, mergeStatement.Id)
            .Select(row => RenderMergeWhenClause(row.MergeWhenClause))
            .ToArray();

        var topRowFilterLink = FindOwnerLink(model.MergeStatementTopRowFilterLinkList, mergeStatement.Id);
        var mergeHeader = topRowFilterLink is null
            ? "MERGE"
            : "MERGE " + RenderTopRowFilter(topRowFilterLink.TopRowFilter);

        var rendered = $"{mergeHeader} INTO {target}{Environment.NewLine}USING {source}{Environment.NewLine}ON {searchCondition}{Environment.NewLine}{string.Join(Environment.NewLine, whenClauses)}";

        var outputClauseLink = FindOwnerLink(model.MergeStatementOutputClauseLinkList, mergeStatement.Id);
        if (outputClauseLink is not null)
        {
            rendered += Environment.NewLine + RenderOutputClause(outputClauseLink.OutputClause);
        }

        var optionClauseLink = FindOwnerLink(model.MergeStatementOptionClauseLinkList, mergeStatement.Id);
        if (optionClauseLink is not null)
        {
            rendered += Environment.NewLine + RenderOptionClause(optionClauseLink.OptionClause);
        }

        return rendered + ";";
    }

    private string RenderMergeWhenClause(MergeWhenClause whenClause)
    {
        var header = whenClause.MatchKind switch
        {
            "Matched" => "WHEN MATCHED",
            "NotMatchedByTarget" => "WHEN NOT MATCHED BY TARGET",
            "NotMatchedBySource" => "WHEN NOT MATCHED BY SOURCE",
            _ => throw new InvalidOperationException($"Unsupported MetaTransformScript MergeWhenClause.MatchKind '{whenClause.MatchKind}'.")
        };

        var searchConditionLink = FindOwnerLink(model.MergeWhenClauseSearchConditionLinkList, whenClause.Id);
        if (searchConditionLink is not null)
        {
            header += " AND " + RenderBooleanExpression(searchConditionLink.BooleanExpression);
        }

        var action = GetOwnerLink(
            model.MergeWhenClauseActionLinkList,
            whenClause.Id,
            "MergeWhenClause.Action").MergeAction;
        return header + " THEN " + RenderMergeAction(action);
    }

    private string RenderMergeAction(MergeAction action)
    {
        var updateAction = FindByBaseId(model.MergeUpdateActionList, action.Id);
        if (updateAction is not null)
        {
            var setClause = GetOwnerLink(
                model.MergeUpdateActionSetClauseLinkList,
                updateAction.Id,
                "MergeUpdateAction.SetClause").SetClause;
            return "UPDATE " + RenderSetClause(setClause);
        }

        if (FindByBaseId(model.MergeDeleteActionList, action.Id) is not null)
        {
            return "DELETE";
        }

        var insertAction = FindByBaseId(model.MergeInsertActionList, action.Id);
        if (insertAction is not null)
        {
            var columns = GetOrderedItems(model.MergeInsertActionColumnsItemList, insertAction.Id)
                .Select(row => RenderIdentifier(row.Identifier))
                .ToArray();
            var columnList = columns.Length == 0
                ? string.Empty
                : " (" + string.Join(", ", columns) + ")";
            var values = GetOrderedItems(model.MergeInsertActionValuesItemList, insertAction.Id)
                .Select(row => RenderScalarExpression(row.ScalarExpression))
                .ToArray();
            return $"INSERT{columnList} VALUES ({string.Join(", ", values)})";
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript MergeAction id '{action.Id}'.");
    }

    private string RenderOutputClause(OutputClause outputClause)
    {
        var selectElements = GetOrderedItems(model.OutputClauseSelectElementsItemList, outputClause.Id)
            .Select(row => RenderSelectElement(row.SelectElement))
            .ToArray();
        var rendered = "OUTPUT" + Environment.NewLine + "    " + string.Join("," + Environment.NewLine + "    ", selectElements);

        var intoTargetLink = FindOwnerLink(model.OutputClauseIntoTargetLinkList, outputClause.Id);
        if (intoTargetLink is not null)
        {
            var columns = GetOrderedItems(model.OutputClauseIntoColumnsItemList, outputClause.Id)
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
        var hints = GetOrderedItems(model.OptionClauseQueryHintsItemList, optionClause.Id)
            .Select(row => RenderSqlHint(row.SqlHint))
            .ToArray();
        return "OPTION (" + string.Join(", ", hints) + ")";
    }

    private string RenderSqlHint(SqlHint sqlHint)
    {
        var keywords = GetOrderedItems(model.SqlHintKeywordsItemList, sqlHint.Id)
            .Select(row => RenderIdentifier(row.Identifier))
            .ToArray();
        var rendered = string.Join(" ", keywords);
        var arguments = GetOrderedItems(model.SqlHintArgumentsItemList, sqlHint.Id)
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
        var assignments = GetOrderedItems(model.SetClauseAssignmentsItemList, setClause.Id)
            .Select(row => RenderSetAssignment(row.SetAssignment))
            .ToArray();
        return "SET " + string.Join("," + Environment.NewLine + "    ", assignments);
    }

    private string RenderSetAssignment(SetAssignment assignment)
    {
        var target = RenderColumnReferenceExpression(GetOwnerLink(
            model.SetAssignmentTargetLinkList,
            assignment.Id,
            "SetAssignment.Target").ColumnReferenceExpression);
        var value = RenderScalarExpression(GetOwnerLink(
            model.SetAssignmentValueLinkList,
            assignment.Id,
            "SetAssignment.Value").ScalarExpression);
        return target + " = " + value;
    }
}
