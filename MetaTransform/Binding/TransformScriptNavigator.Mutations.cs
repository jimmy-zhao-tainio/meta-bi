using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformScriptNavigator
{
    public IReadOnlyList<string> GetInsertTargetColumnNames(TransformScript script)
    {
        var insertStatement = TryGetInsertStatement(script);
        if (insertStatement is null)
        {
            return [];
        }

        return model.InsertStatementColumnsItemList
            .Where(item => string.Equals(item.InsertStatement.Id, insertStatement.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => identifierById.GetValueOrDefault(item.Identifier.Id)?.Value)
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Cast<string>()
            .ToArray();
    }

    public IReadOnlyList<RowValue> GetInsertValuesRows(TransformScript script)
    {
        var insertStatement = TryGetInsertStatement(script);
        if (insertStatement is null)
        {
            return [];
        }

        var source = model.InsertStatementSourceLinkList
            .SingleOrDefault(item => string.Equals(item.InsertStatement.Id, insertStatement.Id, StringComparison.Ordinal))
            ?.InsertSource;
        if (source is null)
        {
            return [];
        }

        var valuesSource = model.InsertValuesSourceList
            .SingleOrDefault(item => string.Equals(item.InsertSource.Id, source.Id, StringComparison.Ordinal));
        if (valuesSource is null)
        {
            return [];
        }

        return model.InsertValuesSourceRowValuesItemList
            .Where(item => string.Equals(item.InsertValuesSource.Id, valuesSource.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => rowValueById.GetValueOrDefault(item.RowValue.Id))
            .Where(static item => item is not null)
            .Cast<RowValue>()
            .ToArray();
    }

    public string? TryGetUpdateStatementTargetAlias(TransformScript script)
    {
        var updateStatement = TryGetUpdateStatement(script);
        if (updateStatement is null)
        {
            return null;
        }

        var alias = model.UpdateStatementTargetAliasLinkList
            .SingleOrDefault(item => string.Equals(item.UpdateStatement.Id, updateStatement.Id, StringComparison.Ordinal))
            ?.Identifier;
        return alias is null
            ? null
            : identifierById.GetValueOrDefault(alias.Id)?.Value;
    }

    public SetClause? TryGetUpdateStatementSetClause(TransformScript script)
    {
        var updateStatement = TryGetUpdateStatement(script);
        if (updateStatement is null)
        {
            return null;
        }

        return model.UpdateStatementSetClauseLinkList
            .SingleOrDefault(item => string.Equals(item.UpdateStatement.Id, updateStatement.Id, StringComparison.Ordinal))
            ?.SetClause;
    }

    public IReadOnlyList<SetAssignment> GetSetAssignments(SetClause setClause)
    {
        return model.SetClauseAssignmentsItemList
            .Where(item => string.Equals(item.SetClause.Id, setClause.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => item.SetAssignment)
            .ToArray();
    }

    public ColumnReferenceExpression? TryGetSetAssignmentTarget(SetAssignment setAssignment)
    {
        return model.SetAssignmentTargetLinkList
            .SingleOrDefault(item => string.Equals(item.SetAssignment.Id, setAssignment.Id, StringComparison.Ordinal))
            ?.ColumnReferenceExpression;
    }

    public ScalarExpression? TryGetSetAssignmentValue(SetAssignment setAssignment)
    {
        return model.SetAssignmentValueLinkList
            .SingleOrDefault(item => string.Equals(item.SetAssignment.Id, setAssignment.Id, StringComparison.Ordinal))
            ?.ScalarExpression;
    }

    public IReadOnlyList<MergeWhenClause> GetMergeWhenClauses(TransformScript script)
    {
        var mergeStatement = TryGetMergeStatement(script);
        if (mergeStatement is null)
        {
            return [];
        }

        return GetOrderedMergeWhenClauseItems(mergeStatement)
            .Select(item => item.MergeWhenClause)
            .ToArray();
    }

    private IReadOnlyList<MergeStatementWhenClausesItem> GetOrderedMergeWhenClauseItems(MergeStatement mergeStatement)
    {
        var items = model.MergeStatementWhenClausesItemList
            .Where(item => string.Equals(item.MergeStatement.Id, mergeStatement.Id, StringComparison.Ordinal))
            .ToArray();
        if (items.Length == 0)
        {
            return [];
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
                    throw new InvalidOperationException($"Merge statement '{mergeStatement.Id}' has more than one first WHEN clause.");
                }

                head = item;
                continue;
            }

            var previous = item.PreviousMergeWhenClause;
            if (!itemIds.Contains(previous.Id))
            {
                throw new InvalidOperationException($"Merge statement '{mergeStatement.Id}' links WHEN clause '{item.Id}' to a clause from another statement.");
            }

            if (!successors.TryAdd(previous.Id, item))
            {
                throw new InvalidOperationException($"Merge statement '{mergeStatement.Id}' branches after WHEN clause '{previous.Id}'.");
            }
        }

        if (head is null)
        {
            throw new InvalidOperationException($"Merge statement '{mergeStatement.Id}' has no first WHEN clause.");
        }

        var ordered = new List<MergeStatementWhenClausesItem>(items.Length);
        var visited = new HashSet<string>(StringComparer.Ordinal);
        for (var current = head; current is not null; successors.TryGetValue(current.Id, out current!))
        {
            if (!visited.Add(current.Id))
            {
                throw new InvalidOperationException($"Merge statement '{mergeStatement.Id}' has a cycle in its WHEN clauses.");
            }

            ordered.Add(current);
        }

        if (ordered.Count != items.Length)
        {
            throw new InvalidOperationException($"Merge statement '{mergeStatement.Id}' has unreachable WHEN clauses.");
        }

        return ordered;
    }

    public string? TryGetMergeStatementTargetAlias(TransformScript script)
    {
        var mergeStatement = TryGetMergeStatement(script);
        if (mergeStatement is null)
        {
            return null;
        }

        var alias = model.MergeStatementTargetAliasLinkList
            .SingleOrDefault(item => string.Equals(item.MergeStatement.Id, mergeStatement.Id, StringComparison.Ordinal))
            ?.Identifier;
        return alias is null
            ? null
            : identifierById.GetValueOrDefault(alias.Id)?.Value;
    }

    public MergeAction? TryGetMergeWhenClauseAction(MergeWhenClause whenClause)
    {
        return model.MergeWhenClauseActionLinkList
            .SingleOrDefault(item => string.Equals(item.MergeWhenClause.Id, whenClause.Id, StringComparison.Ordinal))
            ?.MergeAction;
    }

    public BooleanExpression? TryGetMergeWhenClauseSearchCondition(MergeWhenClause whenClause)
    {
        return model.MergeWhenClauseSearchConditionLinkList
            .SingleOrDefault(item => string.Equals(item.MergeWhenClause.Id, whenClause.Id, StringComparison.Ordinal))
            ?.BooleanExpression;
    }

    public MergeUpdateAction? TryGetMergeUpdateAction(MergeAction action)
    {
        return model.MergeUpdateActionList
            .SingleOrDefault(item => string.Equals(item.MergeAction.Id, action.Id, StringComparison.Ordinal));
    }

    public MergeInsertAction? TryGetMergeInsertAction(MergeAction action)
    {
        return model.MergeInsertActionList
            .SingleOrDefault(item => string.Equals(item.MergeAction.Id, action.Id, StringComparison.Ordinal));
    }

    public MergeDeleteAction? TryGetMergeDeleteAction(MergeAction action)
    {
        return model.MergeDeleteActionList
            .SingleOrDefault(item => string.Equals(item.MergeAction.Id, action.Id, StringComparison.Ordinal));
    }

    public SetClause? TryGetMergeUpdateActionSetClause(MergeUpdateAction action)
    {
        return model.MergeUpdateActionSetClauseLinkList
            .SingleOrDefault(item => string.Equals(item.MergeUpdateAction.Id, action.Id, StringComparison.Ordinal))
            ?.SetClause;
    }

    public IReadOnlyList<string> GetMergeInsertTargetColumnNames(MergeInsertAction action)
    {
        return model.MergeInsertActionColumnsItemList
            .Where(item => string.Equals(item.MergeInsertAction.Id, action.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => identifierById.GetValueOrDefault(item.Identifier.Id)?.Value)
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Cast<string>()
            .ToArray();
    }

    public IReadOnlyList<ScalarExpression> GetMergeInsertValues(MergeInsertAction action)
    {
        return model.MergeInsertActionValuesItemList
            .Where(item => string.Equals(item.MergeInsertAction.Id, action.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => scalarExpressionById.GetValueOrDefault(item.ScalarExpression.Id))
            .Where(static item => item is not null)
            .Cast<ScalarExpression>()
            .ToArray();
    }

    public Literal? TryGetLiteral(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        var valueExpression = model.ValueExpressionList
            .SingleOrDefault(item => string.Equals(item.PrimaryExpression.Id, primaryExpression.Id, StringComparison.Ordinal));
        return valueExpression is null
            ? null
            : model.LiteralList.SingleOrDefault(item => string.Equals(item.ValueExpression.Id, valueExpression.Id, StringComparison.Ordinal));
    }

    public bool IsNationalStringLiteral(Literal literal)
    {
        return model.StringLiteralList.Any(item =>
            string.Equals(item.Literal.Id, literal.Id, StringComparison.Ordinal) &&
            string.Equals(item.IsNational, "true", StringComparison.OrdinalIgnoreCase));
    }

    public string? TryGetDeleteStatementId(TransformScript script) =>
        TryGetDeleteStatement(script)?.Id;

    public string? TryGetTruncateStatementId(TransformScript script)
    {
        return scriptStatementLinkByOwnerId.TryGetValue(script.Id, out var link) &&
               truncateStatementBySqlStatementId.TryGetValue(link.TSqlStatement.Id, out var truncateStatement)
            ? truncateStatement.Id
            : null;
    }

    private InsertStatement? TryGetInsertStatement(TransformScript script)
    {
        var statementWithCtes = TryGetStatementWithCtes(script);
        return statementWithCtes is null
            ? null
            : insertStatementByStatementWithCtesId.GetValueOrDefault(statementWithCtes.Id);
    }

    private UpdateStatement? TryGetUpdateStatement(TransformScript script)
    {
        var statementWithCtes = TryGetStatementWithCtes(script);
        return statementWithCtes is null
            ? null
            : updateStatementByStatementWithCtesId.GetValueOrDefault(statementWithCtes.Id);
    }

    private DeleteStatement? TryGetDeleteStatement(TransformScript script)
    {
        var statementWithCtes = TryGetStatementWithCtes(script);
        return statementWithCtes is null
            ? null
            : deleteStatementByStatementWithCtesId.GetValueOrDefault(statementWithCtes.Id);
    }

    private MergeStatement? TryGetMergeStatement(TransformScript script)
    {
        var statementWithCtes = TryGetStatementWithCtes(script);
        return statementWithCtes is null
            ? null
            : mergeStatementByStatementWithCtesId.GetValueOrDefault(statementWithCtes.Id);
    }
}
