using System.Globalization;
using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private void BuildMutationEffects(
        TransformScript transformScript,
        BoundStatementKind statementKind,
        RuntimeRowset targetRowset,
        RuntimeRowset? inputRowset,
        IReadOnlyList<RuntimeTableSource> visibleSources)
    {
        var targetSqlIdentifier = targetRowset.SqlIdentifier ?? transformScript.Name;

        switch (statementKind)
        {
            case BoundStatementKind.Insert:
                BuildInsertEffects(transformScript, targetSqlIdentifier, targetRowset, inputRowset, visibleSources);
                return;

            case BoundStatementKind.Update:
                BuildUpdateEffects(transformScript, targetSqlIdentifier, targetRowset, visibleSources);
                return;

            case BoundStatementKind.Merge:
                BuildMergeEffects(transformScript, targetSqlIdentifier, targetRowset, inputRowset, visibleSources);
                return;

            case BoundStatementKind.Delete:
                AddDeleteEffect(transformScript, targetSqlIdentifier, targetRowset);
                return;

            case BoundStatementKind.Truncate:
                AddTruncateEffect(transformScript, targetSqlIdentifier, targetRowset);
                return;
        }
    }

    private void BuildInsertEffects(
        TransformScript transformScript,
        string targetSqlIdentifier,
        RuntimeRowset targetRowset,
        RuntimeRowset? inputRowset,
        IReadOnlyList<RuntimeTableSource> visibleSources)
    {
        var targetColumnNames = navigator.GetInsertTargetColumnNames(transformScript);
        if (inputRowset is not null)
        {
            var queryExpressionId = navigator.TryGetInsertStatementQueryExpressionId(transformScript);
            if (string.IsNullOrWhiteSpace(queryExpressionId))
            {
                issues.Add(new TransformBindingIssue(
                    "InsertQuerySourceMissing",
                    $"INSERT transform script '{transformScript.Name}' binds a query rowset but does not expose its source query expression.",
                    transformScript.Id));
                return;
            }

            var queryWriteValues = CreatePositionalWriteValues(
                transformScript.Id,
                "insert-query",
                targetColumnNames,
                inputRowset.Columns);
            mutationEffects.Add(new RuntimeInsertQueryWriteEffect(
                targetSqlIdentifier,
                targetRowset,
                inputRowset,
                queryWriteValues,
                RequiresRequiredFieldCoverage: true,
                queryExpressionId));
            return;
        }

        var rows = navigator.GetInsertValuesRows(transformScript);
        if (rows.Count == 0)
        {
            issues.Add(new TransformBindingIssue(
                "InsertWriteSourceMissing",
                $"INSERT transform script '{transformScript.Name}' does not expose a query or VALUES source for target validation.",
                transformScript.Id));
            return;
        }

        if (rows.Count != 1)
        {
            issues.Add(new TransformBindingIssue(
                "InsertValuesRowCountUnsupported",
                $"INSERT transform script '{transformScript.Name}' contains {rows.Count} VALUES rows. Strict target validation currently requires exactly one VALUES row.",
                transformScript.Id));
            return;
        }

        var scope = CreateMutationValueScope(transformScript, targetRowset, visibleSources, targetAlias: null);
        var values = CreateExpressionWriteValues(
            transformScript,
            rows[0].Id,
            "insert-values",
            targetColumnNames,
            navigator.GetRowValueColumnValues(rows[0]),
            scope,
            inputRowset: null);
        var valueRowset = CreateMutationWriteRowset(
            transformScript,
            rows[0].Id,
            "InsertValues",
            values,
            []);
        mutationEffects.Add(new RuntimeInsertValuesWriteEffect(
            targetSqlIdentifier,
            targetRowset,
            valueRowset,
            values,
            RequiresRequiredFieldCoverage: true,
            rows[0].Id));
    }

    private void BuildUpdateEffects(
        TransformScript transformScript,
        string targetSqlIdentifier,
        RuntimeRowset targetRowset,
        IReadOnlyList<RuntimeTableSource> visibleSources)
    {
        var setClause = navigator.TryGetUpdateStatementSetClause(transformScript);
        if (setClause is null)
        {
            issues.Add(new TransformBindingIssue(
                "UpdateSetClauseMissing",
                $"UPDATE transform script '{transformScript.Name}' does not expose a SET clause for target validation.",
                transformScript.Id));
            return;
        }

        var scope = CreateMutationValueScope(
            transformScript,
            targetRowset,
            visibleSources,
            navigator.TryGetUpdateStatementTargetAlias(transformScript));
        var values = CreateSetAssignmentWriteValues(
            transformScript,
            setClause.Id,
            "update",
            navigator.GetSetAssignments(setClause),
            scope);
        var valueRowset = CreateMutationWriteRowset(
            transformScript,
            setClause.Id,
            "Update",
            values,
            visibleSources.Select(item => item.Rowset));
        mutationEffects.Add(new RuntimeUpdateWriteEffect(
            targetSqlIdentifier,
            targetRowset,
            valueRowset,
            values,
            RequiresRequiredFieldCoverage: false,
            setClause.Id));
    }

    private void BuildMergeEffects(
        TransformScript transformScript,
        string targetSqlIdentifier,
        RuntimeRowset targetRowset,
        RuntimeRowset? inputRowset,
        IReadOnlyList<RuntimeTableSource> visibleSources)
    {
        var scope = CreateMutationValueScope(
            transformScript,
            targetRowset,
            visibleSources,
            navigator.TryGetMergeStatementTargetAlias(transformScript));
        foreach (var whenClause in navigator.GetMergeWhenClauses(transformScript))
        {
            var whenSearchCondition = navigator.TryGetMergeWhenClauseSearchCondition(whenClause);
            if (whenSearchCondition is not null)
            {
                BindBooleanExpression(whenSearchCondition, scope, inputRowset, groupingContext: null);
            }

            var action = navigator.TryGetMergeWhenClauseAction(whenClause);
            if (action is null)
            {
                issues.Add(new TransformBindingIssue(
                    "MergeWhenClauseActionMissing",
                    $"MERGE transform script '{transformScript.Name}' has WHEN clause '{whenClause.Id}' without an action.",
                    whenClause.Id));
                continue;
            }

            var updateAction = navigator.TryGetMergeUpdateAction(action);
            if (updateAction is not null)
            {
                var setClause = navigator.TryGetMergeUpdateActionSetClause(updateAction);
                if (setClause is null)
                {
                    issues.Add(new TransformBindingIssue(
                        "MergeUpdateSetClauseMissing",
                        $"MERGE transform script '{transformScript.Name}' has update action '{updateAction.Id}' without a SET clause.",
                        updateAction.Id));
                    continue;
                }

                var values = CreateSetAssignmentWriteValues(
                    transformScript,
                    updateAction.Id,
                    "merge-update",
                    navigator.GetSetAssignments(setClause),
                    scope);
                var valueRowset = CreateMutationWriteRowset(
                    transformScript,
                    updateAction.Id,
                    "MergeUpdate",
                    values,
                    visibleSources.Select(item => item.Rowset));
                mutationEffects.Add(new RuntimeMergeUpdateWriteEffect(
                    targetSqlIdentifier,
                    targetRowset,
                    valueRowset,
                    values,
                    RequiresRequiredFieldCoverage: false,
                    updateAction.Id));
                continue;
            }

            var insertAction = navigator.TryGetMergeInsertAction(action);
            if (insertAction is not null)
            {
                var values = CreateExpressionWriteValues(
                    transformScript,
                    insertAction.Id,
                    "merge-insert",
                    navigator.GetMergeInsertTargetColumnNames(insertAction),
                    navigator.GetMergeInsertValues(insertAction),
                    scope,
                    inputRowset: null);
                var valueRowset = CreateMutationWriteRowset(
                    transformScript,
                    insertAction.Id,
                    "MergeInsert",
                    values,
                    visibleSources.Select(item => item.Rowset));
                mutationEffects.Add(new RuntimeMergeInsertWriteEffect(
                    targetSqlIdentifier,
                    targetRowset,
                    valueRowset,
                    values,
                    RequiresRequiredFieldCoverage: true,
                    insertAction.Id));
                continue;
            }

            var deleteAction = navigator.TryGetMergeDeleteAction(action);
            if (deleteAction is not null)
            {
                mutationEffects.Add(new RuntimeMergeDeleteEffect(
                    targetSqlIdentifier,
                    targetRowset,
                    deleteAction.Id));
                continue;
            }

            issues.Add(new TransformBindingIssue(
                "MergeWhenClauseActionUnsupported",
                $"MERGE transform script '{transformScript.Name}' has WHEN clause '{whenClause.Id}' with an unsupported action.",
                whenClause.Id));
        }
    }

    private void AddDeleteEffect(
        TransformScript transformScript,
        string targetSqlIdentifier,
        RuntimeRowset targetRowset)
    {
        var deleteStatementId = navigator.TryGetDeleteStatementId(transformScript);
        if (string.IsNullOrWhiteSpace(deleteStatementId))
        {
            issues.Add(new TransformBindingIssue(
                "DeleteStatementMissing",
                $"DELETE transform script '{transformScript.Name}' does not expose a delete statement.",
                transformScript.Id));
            return;
        }

        mutationEffects.Add(new RuntimeDeleteEffect(
            targetSqlIdentifier,
            targetRowset,
            deleteStatementId));
    }

    private void AddTruncateEffect(
        TransformScript transformScript,
        string targetSqlIdentifier,
        RuntimeRowset targetRowset)
    {
        var truncateStatementId = navigator.TryGetTruncateStatementId(transformScript);
        if (string.IsNullOrWhiteSpace(truncateStatementId))
        {
            issues.Add(new TransformBindingIssue(
                "TruncateStatementMissing",
                $"TRUNCATE transform script '{transformScript.Name}' does not expose a truncate statement.",
                transformScript.Id));
            return;
        }

        mutationEffects.Add(new RuntimeTruncateEffect(
            targetSqlIdentifier,
            targetRowset,
            truncateStatementId));
    }

    private IReadOnlyList<RuntimeWriteValue> CreatePositionalWriteValues(
        string scopeId,
        string operationName,
        IReadOnlyList<string> targetColumnNames,
        IReadOnlyList<RuntimeColumn> valueColumns)
    {
        if (targetColumnNames.Count > 0 && targetColumnNames.Count != valueColumns.Count)
        {
            issues.Add(new TransformBindingIssue(
                "MutationTargetValueCountMismatch",
                $"{operationName} writes {valueColumns.Count} value column(s) but declares {targetColumnNames.Count} target column(s).",
                scopeId));
            return [];
        }

        return valueColumns
            .Select((valueColumn, ordinal) => new RuntimeWriteValue(
                valueColumn,
                targetColumnNames.Count == 0 ? string.Empty : targetColumnNames[ordinal],
                MetaTransformScriptScalarExpressionId: null))
            .ToArray();
    }

    private IReadOnlyList<RuntimeWriteValue> CreateExpressionWriteValues(
        TransformScript transformScript,
        string ownerId,
        string operationName,
        IReadOnlyList<string> targetColumnNames,
        IReadOnlyList<ScalarExpression> values,
        BindingScope scope,
        RuntimeRowset? inputRowset)
    {
        if (targetColumnNames.Count > 0 && targetColumnNames.Count != values.Count)
        {
            issues.Add(new TransformBindingIssue(
                "MutationTargetValueCountMismatch",
                $"{operationName} writes {values.Count} value expression(s) but declares {targetColumnNames.Count} target column(s).",
                ownerId));
            return [];
        }

        var result = new List<RuntimeWriteValue>(values.Count);
        foreach (var item in values.Select((value, ordinal) => (Value: value, Ordinal: ordinal)))
        {
            var targetFieldName = targetColumnNames.Count == 0
                ? string.Empty
                : targetColumnNames[item.Ordinal];
            var dataType = BindAndResolveWriteValueDataType(
                transformScript,
                item.Value,
                scope,
                inputRowset,
                ownerId);
            var valueColumn = new RuntimeColumn(
                $"{ownerId}:write-column:{item.Ordinal + 1}",
                string.IsNullOrWhiteSpace(targetFieldName) ? $"Value{item.Ordinal + 1}" : targetFieldName,
                item.Ordinal,
                dataType);
            result.Add(new RuntimeWriteValue(
                valueColumn,
                targetFieldName,
                item.Value.Id));
        }

        return result;
    }

    private IReadOnlyList<RuntimeWriteValue> CreateSetAssignmentWriteValues(
        TransformScript transformScript,
        string ownerId,
        string operationName,
        IReadOnlyList<SetAssignment> assignments,
        BindingScope scope)
    {
        var result = new List<RuntimeWriteValue>(assignments.Count);
        foreach (var item in assignments.Select((assignment, ordinal) => (Assignment: assignment, Ordinal: ordinal)))
        {
            var target = navigator.TryGetSetAssignmentTarget(item.Assignment);
            var targetFieldName = target is null
                ? null
                : navigator.GetColumnReferenceParts(target).LastOrDefault();
            var value = navigator.TryGetSetAssignmentValue(item.Assignment);
            if (string.IsNullOrWhiteSpace(targetFieldName) || value is null)
            {
                issues.Add(new TransformBindingIssue(
                    "MutationSetAssignmentIncomplete",
                    $"{operationName} assignment '{item.Assignment.Id}' must expose one target column and one value expression for strict target validation.",
                    item.Assignment.Id));
                continue;
            }

            var dataType = BindAndResolveWriteValueDataType(
                transformScript,
                value,
                scope,
                inputRowset: null,
                item.Assignment.Id);
            var valueColumn = new RuntimeColumn(
                $"{ownerId}:write-column:{item.Ordinal + 1}",
                targetFieldName,
                item.Ordinal,
                dataType);
            result.Add(new RuntimeWriteValue(
                valueColumn,
                targetFieldName,
                value.Id));
        }

        return result;
    }

    private RuntimeRowset CreateMutationWriteRowset(
        TransformScript transformScript,
        string ownerId,
        string writeRole,
        IReadOnlyList<RuntimeWriteValue> values,
        IEnumerable<RuntimeRowset> inputRowsets)
    {
        var inputs = inputRowsets
            .GroupBy(item => item.Id, StringComparer.Ordinal)
            .Select((group, ordinal) => new RuntimeRowsetInput(ordinal, "Input", group.First()))
            .ToArray();
        var rowset = new RuntimeRowset(
            $"{ownerId}:mutation-write-rowset",
            $"{writeRole}:{transformScript.Name}",
            "Projection",
            writeRole,
            ownerId,
            null,
            values.Select(item => item.ValueColumn).ToArray(),
            inputs);
        TrackRowset(rowset);
        return rowset;
    }

    private BindingScope CreateMutationValueScope(
        TransformScript transformScript,
        RuntimeRowset targetRowset,
        IReadOnlyList<RuntimeTableSource> visibleSources,
        string? targetAlias)
    {
        var targetExposedName = targetAlias?.Trim();
        if (string.IsNullOrWhiteSpace(targetExposedName))
        {
            targetExposedName = targetRowset.SqlIdentifier?
                .Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault();
        }

        targetExposedName ??= transformScript.Name;
        var targetSource = new RuntimeTableSource(
            $"{transformScript.Id}:mutation-target-table-source",
            targetExposedName,
            targetRowset.SqlIdentifier ?? string.Empty,
            targetRowset);
        var localSources = new[] { targetSource }
            .Concat(visibleSources)
            .ToArray();
        return new BindingScope(localSources, localSources.Length);
    }

    private RuntimeColumnDataType? BindAndResolveWriteValueDataType(
        TransformScript transformScript,
        ScalarExpression value,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        string ownerId)
    {
        var directColumnReference = navigator.TryGetDirectColumnReference(value);
        if (directColumnReference is not null)
        {
            var boundColumnReference = BindColumnReference(directColumnReference, scope, groupingContext: null, withinAggregate: false);
            if (boundColumnReference is not null)
            {
                boundColumnReferences.Add(boundColumnReference);
                return boundColumnReference.ResolvedColumn.DataType;
            }

            return null;
        }

        BindScalarExpression(value, scope, inputRowset, groupingContext: null, withinAggregate: false);
        var literal = navigator.TryGetLiteral(value);
        if (literal is not null)
        {
            return CreateLiteralDataType(literal);
        }

        issues.Add(new TransformBindingIssue(
            "MutationWriteValueTypeNotResolved",
            $"Transform script '{transformScript.Name}' uses expression '{value.Id}' in a mutation write, but strict target validation cannot yet establish its data type.",
            ownerId));
        return null;
    }

    private RuntimeColumnDataType? CreateLiteralDataType(Literal literal)
    {
        var value = literal.Value ?? string.Empty;
        var literalType = literal.LiteralType?.Trim() ?? string.Empty;
        return literalType.ToUpperInvariant() switch
        {
            "STRING" => new RuntimeColumnDataType(
                navigator.IsNationalStringLiteral(literal) ? "sqlserver:type:nvarchar" : "sqlserver:type:varchar",
                IsNullable: false,
                Length: value.Length,
                Precision: null,
                Scale: null,
                DisplayName: "string literal"),
            "INTEGER" => CreateIntegerLiteralDataType(value),
            "NUMERIC" => CreateNumericLiteralDataType(value),
            "REAL" => new RuntimeColumnDataType(
                "sqlserver:type:float",
                IsNullable: false,
                Length: null,
                Precision: null,
                Scale: null,
                DisplayName: "real literal"),
            "BINARY" => new RuntimeColumnDataType(
                "sqlserver:type:varbinary",
                IsNullable: false,
                Length: Math.Max(0, (value.Length - 2) / 2),
                Precision: null,
                Scale: null,
                DisplayName: "binary literal"),
            _ => null
        };
    }

    private static RuntimeColumnDataType? CreateIntegerLiteralDataType(string value)
    {
        if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
        {
            return null;
        }

        return new RuntimeColumnDataType(
            number is >= int.MinValue and <= int.MaxValue ? "sqlserver:type:int" : "sqlserver:type:bigint",
            IsNullable: false,
            Length: null,
            Precision: null,
            Scale: null,
            DisplayName: "integer literal");
    }

    private static RuntimeColumnDataType? CreateNumericLiteralDataType(string value)
    {
        var normalized = value.Trim();
        if (normalized.StartsWith('+') || normalized.StartsWith('-'))
        {
            normalized = normalized[1..];
        }

        var parts = normalized.Split('.', StringSplitOptions.None);
        if (parts.Length != 2 || parts.Any(static item => item.Length == 0 || item.Any(static character => !char.IsDigit(character))))
        {
            return null;
        }

        var precision = parts[0].Length + parts[1].Length;
        return new RuntimeColumnDataType(
            "sqlserver:type:decimal",
            IsNullable: false,
            Length: null,
            Precision: precision,
            Scale: parts[1].Length,
            DisplayName: "numeric literal");
    }
}
