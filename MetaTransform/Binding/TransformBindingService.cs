using System.Globalization;
using MetaSchema;
using MetaTransformBinding;
using MetaTransformScript;

namespace MetaTransform.Binding;

public sealed class TransformBindingService
{
    public MetaTransformBindingModel BindSingleTransformModel(
        MetaTransformScriptModel model,
        MetaSchemaModel sourceSchema,
        string? activeLanguageProfileIdOverride = null)
    {
        var bound = BindSingleTransform(model, sourceSchema, activeLanguageProfileIdOverride);
        return CreateBindingModel(bound);
    }

    public MetaTransformBindingModel BindTransformModel(
        MetaTransformScriptModel model,
        TransformScript transformScript,
        MetaSchemaModel sourceSchema,
        string? activeLanguageProfileIdOverride = null)
    {
        var bound = BindTransform(model, transformScript, sourceSchema, activeLanguageProfileIdOverride);
        return CreateBindingModel(bound);
    }

    public BoundTransform BindSingleTransform(
        MetaTransformScriptModel model,
        MetaSchemaModel sourceSchema,
        string? activeLanguageProfileIdOverride = null)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(sourceSchema);

        if (model.TransformScriptList.Count != 1)
        {
            throw new InvalidOperationException(
                $"Expected exactly one TransformScript row but found {model.TransformScriptList.Count}.");
        }

        return BindTransform(model, model.TransformScriptList[0], sourceSchema, activeLanguageProfileIdOverride);
    }

    public BoundTransform BindTransform(
        MetaTransformScriptModel model,
        TransformScript transformScript,
        MetaSchemaModel sourceSchema,
        string? activeLanguageProfileIdOverride = null)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(transformScript);
        ArgumentNullException.ThrowIfNull(sourceSchema);

        var navigator = new TransformScriptNavigator(model);
        var schemaIndex = new SourceSchemaIndex(sourceSchema);
        var issues = new List<TransformBindingIssue>();
        var boundTableSources = new List<RuntimeBoundTableSource>();
        var boundColumnReferences = new List<BoundColumnReference>();

        var activeLanguageProfileId = ResolveActiveLanguageProfile(transformScript, activeLanguageProfileIdOverride);
        if (string.IsNullOrWhiteSpace(activeLanguageProfileId))
        {
            issues.Add(new TransformBindingIssue(
                "ActiveLanguageProfileMissing",
                $"TransformScript '{transformScript.Name}' does not resolve an active language profile.",
                transformScript.Id));

            return CreateResult(null, null);
        }

        var selectStatement = navigator.TryGetSelectStatement(transformScript);
        if (selectStatement is null)
        {
            issues.Add(new TransformBindingIssue(
                "TransformScriptSelectStatementMissing",
                $"TransformScript '{transformScript.Name}' is missing its SelectStatement link.",
                transformScript.Id));

            return CreateResult(null, null);
        }

        var querySpecification = navigator.TryGetQuerySpecification(selectStatement);
        if (querySpecification is null)
        {
            issues.Add(new TransformBindingIssue(
                "UnsupportedQueryExpressionShape",
                $"TransformScript '{transformScript.Name}' does not currently bind non-QuerySpecification roots.",
                selectStatement.Id));

            return CreateResult(null, null);
        }

        var fromClause = navigator.TryGetFromClause(querySpecification);
        if (fromClause is null)
        {
            issues.Add(new TransformBindingIssue(
                "MissingFromClause",
                $"QuerySpecification '{querySpecification.Id}' is missing a FROM clause for Phase 1 binding.",
                querySpecification.Id));

            return CreateResult(null, null);
        }

        foreach (var tableReference in navigator.GetTableReferences(fromClause))
        {
            boundTableSources.AddRange(BindTableReference(tableReference));
        }

        var topLevelScope = new BoundScope(boundTableSources);
        var topLevelRowset = BindSelectElements(querySpecification, topLevelScope);
        return CreateResult(topLevelScope, topLevelRowset);

        BoundTransform CreateResult(BoundScope? topLevelScope, RuntimeBoundRowset? topLevelRowset)
        {
            return new BoundTransform(
                transformScript.Id,
                transformScript.Name,
                activeLanguageProfileId,
                topLevelScope,
                topLevelRowset,
                boundTableSources,
                boundColumnReferences,
                issues);
        }

        IEnumerable<RuntimeBoundTableSource> BindTableReference(TableReference tableReference)
        {
            var namedTableReference = navigator.TryGetNamedTableReference(tableReference);
            if (namedTableReference is not null)
            {
                var boundTableSource = BindNamedTableReference(tableReference, namedTableReference);
                return boundTableSource is null ? [] : [boundTableSource];
            }

            var joinChildren = navigator.TryGetJoinChildren(tableReference);
            if (joinChildren is not null)
            {
                var visibleSources = new List<RuntimeBoundTableSource>();
                if (joinChildren.Value.First is not null)
                {
                    visibleSources.AddRange(BindTableReference(joinChildren.Value.First));
                }

                if (joinChildren.Value.Second is not null)
                {
                    visibleSources.AddRange(BindTableReference(joinChildren.Value.Second));
                }

                return visibleSources;
            }

            issues.Add(new TransformBindingIssue(
                "UnsupportedTableReferenceShape",
                $"TableReference '{tableReference.Id}' is not yet supported by Phase 1 binding.",
                tableReference.Id));
            return [];
        }

        RuntimeBoundTableSource? BindNamedTableReference(TableReference tableReference, NamedTableReference namedTableReference)
        {
            var identifierParts = navigator.GetNamedTableReferenceParts(namedTableReference);
            if (identifierParts.Count is < 1 or > 2)
            {
                issues.Add(new TransformBindingIssue(
                    "UnsupportedSchemaObjectNameShape",
                    $"Named table reference '{namedTableReference.Id}' uses {identifierParts.Count} identifier parts; Phase 1 supports one-part or two-part names only.",
                    namedTableReference.Id));
                return null;
            }

            var resolvedTable = schemaIndex.ResolveTable(identifierParts, namedTableReference.Id, issues);
            if (resolvedTable is null)
            {
                return null;
            }

            var exposedName = navigator.TryGetTableAlias(tableReference) ?? resolvedTable.TableName;
            var columns = resolvedTable.Fields
                .Select((field, ordinal) => new RuntimeBoundColumn(
                    $"{tableReference.Id}:source-column:{field.FieldId}",
                    field.FieldName,
                    ordinal,
                    field.FieldId,
                    resolvedTable.TableId))
                .ToArray();

            return new RuntimeBoundTableSource(
                tableReference.Id,
                exposedName,
                resolvedTable.TableId,
                resolvedTable.SchemaName,
                resolvedTable.TableName,
                new RuntimeBoundRowset(
                    $"{tableReference.Id}:rowset",
                    $"{resolvedTable.SchemaName}.{resolvedTable.TableName}",
                    columns));
        }

        RuntimeBoundRowset BindSelectElements(QuerySpecification querySpecification, BoundScope scope)
        {
            var outputColumns = new List<RuntimeBoundColumn>();

            foreach (var selectElement in navigator.GetSelectElements(querySpecification))
            {
                var selectScalarExpression = navigator.TryGetSelectScalarExpression(selectElement);
                if (selectScalarExpression is not null)
                {
                    BindSelectScalarExpression(selectElement, selectScalarExpression, scope, outputColumns);
                    continue;
                }

                var selectStarExpression = navigator.TryGetSelectStarExpression(selectElement);
                if (selectStarExpression is not null)
                {
                    BindSelectStarExpression(selectElement, selectStarExpression, scope, outputColumns);
                    continue;
                }

                issues.Add(new TransformBindingIssue(
                    "UnsupportedSelectElementShape",
                    $"SelectElement '{selectElement.Id}' is not yet supported by Phase 1 binding.",
                    selectElement.Id));
            }

            return new RuntimeBoundRowset(
                $"{querySpecification.Id}:output-rowset",
                transformScript.Name,
                outputColumns);
        }

        void BindSelectScalarExpression(
            SelectElement selectElement,
            SelectScalarExpression selectScalarExpression,
            BoundScope scope,
            List<RuntimeBoundColumn> outputColumns)
        {
            var scalarExpression = navigator.TryGetSelectScalarExpressionBody(selectScalarExpression);
            if (scalarExpression is null)
            {
                issues.Add(new TransformBindingIssue(
                    "SelectScalarExpressionBodyMissing",
                    $"SelectScalarExpression '{selectScalarExpression.Id}' is missing its expression body.",
                    selectScalarExpression.Id));
                return;
            }

            BoundColumnReference? boundColumnReference = null;
            var directColumnReference = navigator.TryGetDirectColumnReference(scalarExpression);
            if (directColumnReference is not null)
            {
                boundColumnReference = BindColumnReference(directColumnReference, scope);
                if (boundColumnReference is not null)
                {
                    boundColumnReferences.Add(boundColumnReference);
                }
            }

            var outputName = navigator.TryGetSelectScalarExpressionAlias(selectScalarExpression);
            if (string.IsNullOrWhiteSpace(outputName) && boundColumnReference is not null)
            {
                outputName = boundColumnReference.ResolvedColumn.Name;
            }

            if (string.IsNullOrWhiteSpace(outputName) && directColumnReference is not null)
            {
                outputName = navigator.GetColumnReferenceParts(directColumnReference).LastOrDefault();
            }

            if (string.IsNullOrWhiteSpace(outputName))
            {
                issues.Add(new TransformBindingIssue(
                    "UnsupportedSelectOutputName",
                    $"SelectElement '{selectElement.Id}' does not currently expose a supported output name in Phase 1 binding.",
                    selectElement.Id));
                return;
            }

            outputColumns.Add(new RuntimeBoundColumn(
                $"{selectElement.Id}:output",
                outputName,
                outputColumns.Count,
                boundColumnReference?.ResolvedColumn.SourceFieldId,
                boundColumnReference?.ResolvedColumn.SourceTableId));
        }

        void BindSelectStarExpression(
            SelectElement selectElement,
            SelectStarExpression selectStarExpression,
            BoundScope scope,
            List<RuntimeBoundColumn> outputColumns)
        {
            var qualifierParts = navigator.GetSelectStarQualifierParts(selectStarExpression);
            IEnumerable<RuntimeBoundTableSource> sourcesToExpand;

            if (qualifierParts.Count == 0)
            {
                sourcesToExpand = scope.VisibleTableSources;
            }
            else if (qualifierParts.Count == 1)
            {
                var matchedSources = scope.VisibleTableSources
                    .Where(item => string.Equals(item.ExposedName, qualifierParts[0], StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (matchedSources.Length == 0)
                {
                    issues.Add(new TransformBindingIssue(
                        "SelectStarQualifierNotFound",
                        $"Select star qualifier '{qualifierParts[0]}' does not match any visible table source.",
                        selectStarExpression.Id));
                    return;
                }

                if (matchedSources.Length > 1)
                {
                    issues.Add(new TransformBindingIssue(
                        "SelectStarQualifierAmbiguous",
                        $"Select star qualifier '{qualifierParts[0]}' matches more than one visible table source.",
                        selectStarExpression.Id));
                    return;
                }

                sourcesToExpand = matchedSources;
            }
            else
            {
                issues.Add(new TransformBindingIssue(
                    "UnsupportedSelectStarQualifierShape",
                    $"Select star qualifier on '{selectStarExpression.Id}' uses {qualifierParts.Count} identifier parts; Phase 1 supports single-part qualifiers only.",
                    selectStarExpression.Id));
                return;
            }

            foreach (var source in sourcesToExpand)
            {
                foreach (var column in source.Rowset.Columns)
                {
                    outputColumns.Add(new RuntimeBoundColumn(
                        $"{selectElement.Id}:output:{outputColumns.Count}",
                        column.Name,
                        outputColumns.Count,
                        column.SourceFieldId,
                        column.SourceTableId));
                }
            }
        }

        BoundColumnReference? BindColumnReference(ColumnReferenceExpression columnReferenceExpression, BoundScope scope)
        {
            var parts = navigator.GetColumnReferenceParts(columnReferenceExpression);
            if (parts.Count == 0)
            {
                issues.Add(new TransformBindingIssue(
                    "ColumnReferenceMissingIdentifier",
                    $"ColumnReferenceExpression '{columnReferenceExpression.Id}' is missing its multipart identifier.",
                    columnReferenceExpression.Id));
                return null;
            }

            if (parts.Count == 1)
            {
                var matches = scope.VisibleTableSources
                    .SelectMany(source => source.Rowset.Columns.Select(column => (Source: source, Column: column)))
                    .Where(item => string.Equals(item.Column.Name, parts[0], StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (matches.Length == 0)
                {
                    issues.Add(new TransformBindingIssue(
                        "ColumnReferenceNotFound",
                        $"Column '{parts[0]}' is not visible in the current query scope.",
                        columnReferenceExpression.Id));
                    return null;
                }

                if (matches.Length > 1)
                {
                    issues.Add(new TransformBindingIssue(
                        "ColumnReferenceAmbiguous",
                        $"Column '{parts[0]}' resolves ambiguously across visible table sources.",
                        columnReferenceExpression.Id));
                    return null;
                }

                return new BoundColumnReference(columnReferenceExpression.Id, parts, matches[0].Column, matches[0].Source);
            }

            if (parts.Count == 2)
            {
                var matchedSources = scope.VisibleTableSources
                    .Where(item => string.Equals(item.ExposedName, parts[0], StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (matchedSources.Length == 0)
                {
                    issues.Add(new TransformBindingIssue(
                        "ColumnQualifierNotFound",
                        $"Column qualifier '{parts[0]}' is not visible in the current query scope.",
                        columnReferenceExpression.Id));
                    return null;
                }

                if (matchedSources.Length > 1)
                {
                    issues.Add(new TransformBindingIssue(
                        "ColumnQualifierAmbiguous",
                        $"Column qualifier '{parts[0]}' matches more than one visible table source.",
                        columnReferenceExpression.Id));
                    return null;
                }

                var matchedColumns = matchedSources[0].Rowset.Columns
                    .Where(item => string.Equals(item.Name, parts[1], StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (matchedColumns.Length == 0)
                {
                    issues.Add(new TransformBindingIssue(
                        "QualifiedColumnReferenceNotFound",
                        $"Column '{parts[1]}' is not exposed by table source '{parts[0]}'.",
                        columnReferenceExpression.Id));
                    return null;
                }

                if (matchedColumns.Length > 1)
                {
                    issues.Add(new TransformBindingIssue(
                        "QualifiedColumnReferenceAmbiguous",
                        $"Column '{parts[1]}' resolves ambiguously within table source '{parts[0]}'.",
                        columnReferenceExpression.Id));
                    return null;
                }

                return new BoundColumnReference(columnReferenceExpression.Id, parts, matchedColumns[0], matchedSources[0]);
            }

            issues.Add(new TransformBindingIssue(
                "UnsupportedColumnReferenceShape",
                $"ColumnReferenceExpression '{columnReferenceExpression.Id}' uses {parts.Count} identifier parts; Phase 1 supports one-part or two-part references only.",
                columnReferenceExpression.Id));
            return null;
        }
    }

    private static string ResolveActiveLanguageProfile(TransformScript transformScript, string? activeLanguageProfileIdOverride)
    {
        if (!string.IsNullOrWhiteSpace(activeLanguageProfileIdOverride))
        {
            return activeLanguageProfileIdOverride.Trim();
        }

        return string.IsNullOrWhiteSpace(transformScript.LanguageProfileId)
            ? string.Empty
            : transformScript.LanguageProfileId.Trim();
    }

    private static MetaTransformBindingModel CreateBindingModel(BoundTransform bound)
    {
        var model = MetaTransformBindingModel.CreateEmpty();
        var bindingId = $"{bound.TransformScriptId}:binding";
        var bindingRow = new TransformBinding
        {
            Id = bindingId,
            TransformScriptId = bound.TransformScriptId,
            TransformScriptName = bound.TransformScriptName,
            ActiveLanguageProfileId = bound.ActiveLanguageProfileId
        };

        model.TransformBindingList.Add(bindingRow);

        foreach (var issue in bound.Issues.Select((item, ordinal) => (Item: item, Ordinal: ordinal)))
        {
            model.BoundIssueList.Add(new BoundIssue
            {
                Id = $"{bindingId}:issue:{issue.Ordinal + 1}",
                OwnerId = bindingId,
                Code = issue.Item.Code,
                Message = issue.Item.Message,
                Severity = "Error",
                SyntaxId = issue.Item.SyntaxId ?? string.Empty
            });
        }

        foreach (var source in bound.BoundTableSources.Select((item, ordinal) => (Item: item, Ordinal: ordinal)))
        {
            AddRowset(
                model,
                bindingId,
                source.Item.Rowset.Id,
                source.Item.Rowset.Name,
                "Source",
                "Source",
                source.Item.SyntaxTableReferenceId,
                source.Item.SourceTableId,
                source.Item.Rowset.Columns);

            model.BoundTableSourceList.Add(new BoundTableSource
            {
                Id = $"{bindingId}:table-source:{source.Ordinal + 1}",
                OwnerId = bindingId,
                ValueId = source.Item.Rowset.Id,
                ExposedName = source.Item.ExposedName,
                SyntaxTableReferenceId = source.Item.SyntaxTableReferenceId
            });
        }

        if (bound.TopLevelRowset is not null)
        {
            AddRowset(
                model,
                bindingId,
                bound.TopLevelRowset.Id,
                bound.TopLevelRowset.Name,
                "Projection",
                "FinalOutput",
                string.Empty,
                string.Empty,
                bound.TopLevelRowset.Columns);

            model.TransformBindingFinalRowsetLinkList.Add(new TransformBindingFinalRowsetLink
            {
                Id = $"{bindingId}:final-rowset",
                OwnerId = bindingId,
                ValueId = bound.TopLevelRowset.Id
            });

            foreach (var source in bound.BoundTableSources.Select((item, ordinal) => (Item: item, Ordinal: ordinal)))
            {
                model.BoundRowsetInputList.Add(new BoundRowsetInput
                {
                    Id = $"{bound.TopLevelRowset.Id}:input:{source.Ordinal + 1}",
                    OwnerId = bound.TopLevelRowset.Id,
                    ValueId = source.Item.Rowset.Id,
                    Ordinal = source.Ordinal.ToString(),
                    InputRole = "VisibleSource"
                });
            }
        }

        return model;
    }

    private static void AddRowset(
        MetaTransformBindingModel model,
        string bindingId,
        string rowsetId,
        string rowsetName,
        string derivationKind,
        string rowsetRole,
        string syntaxId,
        string sourceTableId,
            IReadOnlyList<RuntimeBoundColumn> columns)
    {
        if (model.BoundRowsetList.Any(item => string.Equals(item.Id, rowsetId, StringComparison.Ordinal)))
        {
            return;
        }

        model.BoundRowsetList.Add(new BoundRowset
        {
            Id = rowsetId,
            OwnerId = bindingId,
            Name = rowsetName,
            DerivationKind = derivationKind,
            RowsetRole = rowsetRole,
            SyntaxId = syntaxId,
            SourceTableId = sourceTableId
        });

        foreach (var column in columns)
        {
            model.BoundColumnList.Add(new BoundColumn
            {
                Id = column.Id,
                OwnerId = rowsetId,
                Name = column.Name,
                Ordinal = column.Ordinal.ToString(),
                SourceFieldId = column.SourceFieldId ?? string.Empty,
                SourceTableId = column.SourceTableId ?? string.Empty
            });
        }
    }

    private sealed record SourceFieldDefinition(
        string FieldId,
        string FieldName,
        int Ordinal);

    private sealed record SourceTableDefinition(
        string TableId,
        string SchemaName,
        string TableName,
        IReadOnlyList<SourceFieldDefinition> Fields);

    private sealed class SourceSchemaIndex
    {
        private readonly IReadOnlyList<SourceTableDefinition> tables;

        public SourceSchemaIndex(MetaSchemaModel model)
        {
            var schemaNamesById = model.SchemaList.ToDictionary(item => item.Id, item => item.Name, StringComparer.Ordinal);
            var fieldRowsByTableId = model.FieldList
                .GroupBy(item => item.TableId, StringComparer.Ordinal)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .OrderBy(item => ParseOrdinal(item.Ordinal))
                        .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                        .Select(item => new SourceFieldDefinition(item.Id, item.Name, ParseOrdinal(item.Ordinal)))
                        .ToArray(),
                    StringComparer.Ordinal);

            tables = model.TableList
                .Select(item => new SourceTableDefinition(
                    item.Id,
                    schemaNamesById.GetValueOrDefault(item.SchemaId) ?? string.Empty,
                    item.Name,
                    fieldRowsByTableId.GetValueOrDefault(item.Id) ?? []))
                .ToArray();
        }

        public SourceTableDefinition? ResolveTable(
            IReadOnlyList<string> identifierParts,
            string syntaxId,
            List<TransformBindingIssue> issues)
        {
            if (identifierParts.Count == 1)
            {
                var matches = tables
                    .Where(item => string.Equals(item.TableName, identifierParts[0], StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (matches.Length == 0)
                {
                    issues.Add(new TransformBindingIssue(
                        "SourceTableNotFound",
                        $"Source table '{identifierParts[0]}' was not found in the sanctioned source schema.",
                        syntaxId));
                    return null;
                }

                if (matches.Length > 1)
                {
                    issues.Add(new TransformBindingIssue(
                        "SourceTableAmbiguous",
                        $"Source table '{identifierParts[0]}' resolves ambiguously across schemas.",
                        syntaxId));
                    return null;
                }

                return matches[0];
            }

            var schemaName = identifierParts[0];
            var tableName = identifierParts[1];
            var qualifiedMatches = tables
                .Where(item =>
                    string.Equals(item.SchemaName, schemaName, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(item.TableName, tableName, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (qualifiedMatches.Length == 0)
            {
                issues.Add(new TransformBindingIssue(
                    "SourceTableNotFound",
                    $"Source table '{schemaName}.{tableName}' was not found in the sanctioned source schema.",
                    syntaxId));
                return null;
            }

            if (qualifiedMatches.Length > 1)
            {
                issues.Add(new TransformBindingIssue(
                    "SourceTableAmbiguous",
                    $"Source table '{schemaName}.{tableName}' resolves ambiguously in the sanctioned source schema.",
                    syntaxId));
                return null;
            }

            return qualifiedMatches[0];
        }

        private static int ParseOrdinal(string ordinal)
        {
            return int.TryParse(ordinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                ? value
                : int.MaxValue;
        }
    }
}
