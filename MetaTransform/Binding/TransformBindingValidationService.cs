using MetaSchema;
using MetaTransformBinding;

namespace MetaTransform.Binding;

public sealed class TransformBindingValidationService
{
    public MetaTransformBindingModel ApplyValidation(
        MetaTransformBindingModel bindingModel,
        MetaSchemaModel schemaModel)
    {
        ArgumentNullException.ThrowIfNull(bindingModel);
        ArgumentNullException.ThrowIfNull(schemaModel);

        var resolver = new MetaSchemaTableResolver(schemaModel);

        bindingModel.TransformBindingValidationList.Clear();
        bindingModel.RowsetSourceValidationList.Clear();
        bindingModel.RowsetTargetValidationList.Clear();
        bindingModel.TransformBindingValidationIssueList.Clear();

        foreach (var binding in bindingModel.TransformBindingList)
        {
            ApplyValidation(bindingModel, binding, resolver);
        }

        return bindingModel;
    }

    private static void ApplyValidation(
        MetaTransformBindingModel model,
        TransformBinding binding,
        MetaSchemaTableResolver resolver)
    {
        var validationId = $"{binding.Id}:validation";
        model.TransformBindingValidationList.Add(new TransformBindingValidation
        {
            Id = validationId,
            TransformBindingId = binding.Id
        });

        var finalRowset = ResolveFinalRowset(model, binding.Id);
        var rowsetColumnsByRowsetId = model.ColumnList
            .GroupBy(item => item.RowsetId, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(item => ParseOrdinal(item.Ordinal))
                    .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                    .ToArray(),
                StringComparer.Ordinal);

        foreach (var sourceRowset in model.RowsetList.Where(item =>
                     string.Equals(item.TransformBindingId, binding.Id, StringComparison.Ordinal) &&
                     string.Equals(item.DerivationKind, "Source", StringComparison.OrdinalIgnoreCase) &&
                     !string.IsNullOrWhiteSpace(item.SqlIdentifier)))
        {
            AddSourceValidation(model, validationId, sourceRowset, resolver, rowsetColumnsByRowsetId);
        }

        foreach (var target in model.TransformBindingTargetList.Where(item => string.Equals(item.TransformBindingId, binding.Id, StringComparison.Ordinal)))
        {
            AddTargetValidation(model, validationId, target.SqlIdentifier, resolver, finalRowset, rowsetColumnsByRowsetId);
        }
    }

    private static void AddSourceValidation(
        MetaTransformBindingModel model,
        string validationId,
        Rowset sourceRowset,
        MetaSchemaTableResolver resolver,
        IReadOnlyDictionary<string, Column[]> rowsetColumnsByRowsetId)
    {
        var sqlIdentifier = sourceRowset.SqlIdentifier;
        var resolution = resolver.ResolveSqlIdentifier(sqlIdentifier);
        var sourceValidationId = $"{validationId}:source:{model.RowsetSourceValidationList.Count + 1}";
        var conformanceKind = "NotEvaluated";

        if (resolution.IsResolved)
        {
            var actualColumns = rowsetColumnsByRowsetId.GetValueOrDefault(sourceRowset.Id) ?? [];
            if (actualColumns.Length > 0)
            {
                var expectedFieldsByName = resolution.Table!.Fields
                    .ToDictionary(item => item.FieldName, StringComparer.OrdinalIgnoreCase);

                var missingColumns = actualColumns
                    .Where(item => !expectedFieldsByName.ContainsKey(item.Name))
                    .ToArray();

                if (missingColumns.Length == 0)
                {
                    conformanceKind = "ColumnSubsetConforms";
                }
                else
                {
                    conformanceKind = "Mismatch";
                    foreach (var missingColumn in missingColumns)
                    {
                        AddValidationIssue(
                            model,
                            validationId,
                            "SourceRowsetColumnMissingInSchema",
                            $"Source rowset '{sqlIdentifier}' uses column '{missingColumn.Name}', but that column was not found in the sanctioned schema table '{resolution.Table.CanonicalSqlIdentifier}'.",
                            "Error",
                            sourceRowset.SyntaxId);
                    }
                }
            }
        }
        else
        {
            AddResolutionIssue(model, validationId, true, sqlIdentifier, resolution, sourceRowset.SyntaxId);
        }

        model.RowsetSourceValidationList.Add(new RowsetSourceValidation
        {
            Id = sourceValidationId,
            TransformBindingValidationId = validationId,
            RowsetId = sourceRowset.Id,
            ResolutionKind = MapResolutionKind(resolution.FailureKind),
            ConformanceKind = conformanceKind,
            TableId = resolution.Table?.TableId ?? string.Empty
        });
    }

    private static void AddTargetValidation(
        MetaTransformBindingModel model,
        string validationId,
        string targetSqlIdentifier,
        MetaSchemaTableResolver resolver,
        Rowset? finalRowset,
        IReadOnlyDictionary<string, Column[]> rowsetColumnsByRowsetId)
    {
        var resolution = resolver.ResolveSqlIdentifier(targetSqlIdentifier);
        var targetValidationId = $"{validationId}:target:{model.RowsetTargetValidationList.Count + 1}";
        var conformanceKind = "NotEvaluated";
        Rowset resolvedFinalRowset;

        if (!resolution.IsResolved)
        {
            AddResolutionIssue(model, validationId, false, targetSqlIdentifier, resolution, finalRowset?.SyntaxId);
            return;
        }

        if (finalRowset is null)
        {
            AddValidationIssue(
                model,
                validationId,
                "FinalOutputRowsetMissing",
                $"Transform binding declares target '{targetSqlIdentifier}', but binding did not produce a final output rowset.",
                "Error",
                null);
            return;
        }
        resolvedFinalRowset = finalRowset;
        var actualColumns = rowsetColumnsByRowsetId.GetValueOrDefault(resolvedFinalRowset.Id) ?? [];
        var expectedColumns = resolution.Table!.Fields
            .Where(item => !item.IsIdentity)
            .OrderBy(item => item.Ordinal)
            .ToArray();

        var hasMismatch = false;

        if (actualColumns.Length != expectedColumns.Length)
        {
            hasMismatch = true;
            AddValidationIssue(
                model,
                validationId,
                "TargetRowsetColumnCountMismatch",
                $"Final output rowset exposes {actualColumns.Length} column(s), but target table '{targetSqlIdentifier}' declares {expectedColumns.Length} non-identity column(s).",
                "Error",
                resolvedFinalRowset.SyntaxId);
        }

        var compareCount = Math.Min(actualColumns.Length, expectedColumns.Length);
        for (var ordinal = 0; ordinal < compareCount; ordinal++)
        {
            if (string.Equals(actualColumns[ordinal].Name, expectedColumns[ordinal].FieldName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            hasMismatch = true;
            AddValidationIssue(
                model,
                validationId,
                "TargetRowsetColumnNameMismatch",
                $"Final output rowset for target '{targetSqlIdentifier}' column {ordinal + 1} is '{actualColumns[ordinal].Name}', but the sanctioned schema expects '{expectedColumns[ordinal].FieldName}'.",
                "Error",
                resolvedFinalRowset.SyntaxId);
        }

        conformanceKind = hasMismatch ? "Mismatch" : "Conforms";

        model.RowsetTargetValidationList.Add(new RowsetTargetValidation
        {
            Id = targetValidationId,
            TransformBindingValidationId = validationId,
            RowsetId = resolvedFinalRowset.Id,
            SqlIdentifier = targetSqlIdentifier,
            ResolutionKind = MapResolutionKind(resolution.FailureKind),
            ConformanceKind = conformanceKind,
            TableId = resolution.Table?.TableId ?? string.Empty
        });
    }

    private static Rowset? ResolveFinalRowset(MetaTransformBindingModel model, string bindingId)
    {
        var finalLink = model.OutputRowsetList
            .SingleOrDefault(item => string.Equals(item.TransformBindingId, bindingId, StringComparison.Ordinal));

        if (finalLink is null)
        {
            return null;
        }

        return model.RowsetList.SingleOrDefault(item => string.Equals(item.Id, finalLink.RowsetId, StringComparison.Ordinal));
    }

    private static void AddResolutionIssue(
        MetaTransformBindingModel model,
        string validationId,
        bool isSource,
        string sqlIdentifier,
        SchemaTableResolutionResult resolution,
        string? syntaxId)
    {
        var code = resolution.FailureKind switch
        {
            SchemaTableResolutionFailureKind.MissingIdentifier => isSource ? "SourceSchemaIdentifierMissing" : "TargetSchemaIdentifierMissing",
            SchemaTableResolutionFailureKind.UnsupportedIdentifierShape => isSource ? "SourceSchemaIdentifierShapeUnsupported" : "TargetSchemaIdentifierShapeUnsupported",
            SchemaTableResolutionFailureKind.NotFound => isSource ? "SourceSchemaTableNotFound" : "TargetSchemaTableNotFound",
            SchemaTableResolutionFailureKind.Ambiguous => isSource ? "SourceSchemaTableAmbiguous" : "TargetSchemaTableAmbiguous",
            _ => isSource ? "SourceSchemaResolutionFailed" : "TargetSchemaResolutionFailed"
        };

        var objectKind = isSource ? "source" : "target";
        var message = resolution.FailureKind switch
        {
            SchemaTableResolutionFailureKind.MissingIdentifier =>
                $"Declared {objectKind} identifier '{sqlIdentifier}' is blank and cannot be resolved against the sanctioned schema workspace.",
            SchemaTableResolutionFailureKind.UnsupportedIdentifierShape =>
                $"Declared {objectKind} identifier '{sqlIdentifier}' uses an unsupported identifier shape for schema resolution.",
            SchemaTableResolutionFailureKind.NotFound =>
                $"Declared {objectKind} identifier '{sqlIdentifier}' was not found in the sanctioned schema workspace.",
            SchemaTableResolutionFailureKind.Ambiguous =>
                $"Declared {objectKind} identifier '{sqlIdentifier}' matches more than one table in the sanctioned schema workspace.",
            _ =>
                $"Declared {objectKind} identifier '{sqlIdentifier}' could not be resolved against the sanctioned schema workspace."
        };

        AddValidationIssue(model, validationId, code, message, "Error", syntaxId);
    }

    private static void AddValidationIssue(
        MetaTransformBindingModel model,
        string validationId,
        string code,
        string message,
        string severity,
        string? syntaxId)
    {
        model.TransformBindingValidationIssueList.Add(new TransformBindingValidationIssue
        {
            Id = $"{validationId}:issue:{model.TransformBindingValidationIssueList.Count + 1}",
            TransformBindingValidationId = validationId,
            Code = code,
            Message = message,
            Severity = severity,
            SyntaxId = syntaxId ?? string.Empty
        });
    }

    private static string MapResolutionKind(SchemaTableResolutionFailureKind failureKind)
    {
        return failureKind switch
        {
            SchemaTableResolutionFailureKind.None => "Resolved",
            SchemaTableResolutionFailureKind.MissingIdentifier => "MissingIdentifier",
            SchemaTableResolutionFailureKind.UnsupportedIdentifierShape => "UnsupportedIdentifierShape",
            SchemaTableResolutionFailureKind.NotFound => "NotFound",
            SchemaTableResolutionFailureKind.Ambiguous => "Ambiguous",
            _ => failureKind.ToString()
        };
    }

    private static int ParseOrdinal(string ordinal)
    {
        return int.TryParse(ordinal, out var value)
            ? value
            : int.MaxValue;
    }
}
