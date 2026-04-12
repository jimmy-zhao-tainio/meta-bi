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

        var validations = new List<Validation>();
        var sourceRowsetLinks = new List<ValidationSourceRowsetLink>();
        var targetRowsetLinks = new List<ValidationTargetRowsetLink>();
        var sourceColumnLinks = new List<ValidationSourceColumnLink>();
        var targetColumnLinks = new List<ValidationTargetColumnLink>();

        foreach (var binding in bindingModel.TransformBindingList)
        {
            ApplyValidation(
                bindingModel,
                binding,
                resolver,
                validations,
                sourceRowsetLinks,
                targetRowsetLinks,
                sourceColumnLinks,
                targetColumnLinks);
        }

        bindingModel.ValidationList.Clear();
        bindingModel.ValidationList.AddRange(validations);
        bindingModel.ValidationSourceRowsetLinkList.Clear();
        bindingModel.ValidationSourceRowsetLinkList.AddRange(sourceRowsetLinks);
        bindingModel.ValidationTargetRowsetLinkList.Clear();
        bindingModel.ValidationTargetRowsetLinkList.AddRange(targetRowsetLinks);
        bindingModel.ValidationSourceColumnLinkList.Clear();
        bindingModel.ValidationSourceColumnLinkList.AddRange(sourceColumnLinks);
        bindingModel.ValidationTargetColumnLinkList.Clear();
        bindingModel.ValidationTargetColumnLinkList.AddRange(targetColumnLinks);

        return bindingModel;
    }

    private static void ApplyValidation(
        MetaTransformBindingModel model,
        TransformBinding binding,
        MetaSchemaTableResolver resolver,
        List<Validation> validations,
        List<ValidationSourceRowsetLink> sourceRowsetLinks,
        List<ValidationTargetRowsetLink> targetRowsetLinks,
        List<ValidationSourceColumnLink> sourceColumnLinks,
        List<ValidationTargetColumnLink> targetColumnLinks)
    {
        var validationId = $"{binding.Id}:validation";
        validations.Add(new Validation
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
            AddSourceValidation(
                validationId,
                sourceRowset,
                resolver,
                rowsetColumnsByRowsetId,
                sourceRowsetLinks,
                sourceColumnLinks);
        }

        foreach (var target in model.TransformBindingTargetList.Where(item => string.Equals(item.TransformBindingId, binding.Id, StringComparison.Ordinal)))
        {
            AddTargetValidation(
                validationId,
                target,
                resolver,
                finalRowset,
                rowsetColumnsByRowsetId,
                targetRowsetLinks,
                targetColumnLinks);
        }
    }

    private static void AddSourceValidation(
        string validationId,
        Rowset sourceRowset,
        MetaSchemaTableResolver resolver,
        IReadOnlyDictionary<string, Column[]> rowsetColumnsByRowsetId,
        List<ValidationSourceRowsetLink> sourceRowsetLinks,
        List<ValidationSourceColumnLink> sourceColumnLinks)
    {
        var sqlIdentifier = sourceRowset.SqlIdentifier;
        var resolution = resolver.ResolveSqlIdentifier(sqlIdentifier);
        if (!resolution.IsResolved)
        {
            ThrowResolutionFailure(isSource: true, sqlIdentifier, resolution);
        }

        var sourceRowsetLinkId = $"{validationId}:source:{sourceRowsetLinks.Count + 1}";
        sourceRowsetLinks.Add(new ValidationSourceRowsetLink
        {
            Id = sourceRowsetLinkId,
            ValidationId = validationId,
            RowsetId = sourceRowset.Id,
            MetaSchemaTableId = resolution.Table!.TableId
        });

        var actualColumns = rowsetColumnsByRowsetId.GetValueOrDefault(sourceRowset.Id) ?? [];
        if (actualColumns.Length == 0)
        {
            return;
        }

        var expectedFieldsByName = resolution.Table.Fields
            .ToDictionary(item => item.FieldName, StringComparer.OrdinalIgnoreCase);

        foreach (var actualColumn in actualColumns)
        {
            if (!expectedFieldsByName.TryGetValue(actualColumn.Name, out var matchedField))
            {
                throw new TransformBindingValidationException(
                    "SourceRowsetColumnMissingInSchema",
                    $"Source rowset '{sqlIdentifier}' uses column '{actualColumn.Name}', but that column was not found in the sanctioned schema table '{resolution.Table.CanonicalSqlIdentifier}'.");
            }

            sourceColumnLinks.Add(new ValidationSourceColumnLink
            {
                Id = $"{sourceRowsetLinkId}:column:{sourceColumnLinks.Count + 1}",
                ValidationSourceRowsetLinkId = sourceRowsetLinkId,
                ColumnId = actualColumn.Id,
                MetaSchemaFieldId = matchedField.FieldId
            });
        }
    }

    private static void AddTargetValidation(
        string validationId,
        TransformBindingTarget target,
        MetaSchemaTableResolver resolver,
        Rowset? finalRowset,
        IReadOnlyDictionary<string, Column[]> rowsetColumnsByRowsetId,
        List<ValidationTargetRowsetLink> targetRowsetLinks,
        List<ValidationTargetColumnLink> targetColumnLinks)
    {
        var targetSqlIdentifier = target.SqlIdentifier;
        var resolution = resolver.ResolveSqlIdentifier(targetSqlIdentifier);
        if (!resolution.IsResolved)
        {
            ThrowResolutionFailure(isSource: false, targetSqlIdentifier, resolution);
        }

        if (finalRowset is null)
        {
            throw new TransformBindingValidationException(
                "FinalOutputRowsetMissing",
                $"Transform binding declares target '{targetSqlIdentifier}', but binding did not produce a final output rowset.");
        }

        var actualColumns = rowsetColumnsByRowsetId.GetValueOrDefault(finalRowset.Id) ?? [];
        var expectedColumns = resolution.Table!.Fields
            .Where(item => !item.IsIdentity)
            .OrderBy(item => item.Ordinal)
            .ToArray();

        if (actualColumns.Length != expectedColumns.Length)
        {
            throw new TransformBindingValidationException(
                "TargetRowsetColumnCountMismatch",
                $"Final output rowset exposes {actualColumns.Length} column(s), but target table '{targetSqlIdentifier}' declares {expectedColumns.Length} non-identity column(s).");
        }

        var targetRowsetLinkId = $"{validationId}:target:{targetRowsetLinks.Count + 1}";
        targetRowsetLinks.Add(new ValidationTargetRowsetLink
        {
            Id = targetRowsetLinkId,
            ValidationId = validationId,
            TransformBindingTargetId = target.Id,
            RowsetId = finalRowset.Id,
            MetaSchemaTableId = resolution.Table.TableId
        });

        for (var ordinal = 0; ordinal < actualColumns.Length; ordinal++)
        {
            if (!string.Equals(actualColumns[ordinal].Name, expectedColumns[ordinal].FieldName, StringComparison.OrdinalIgnoreCase))
            {
                throw new TransformBindingValidationException(
                    "TargetRowsetColumnNameMismatch",
                    $"Final output rowset for target '{targetSqlIdentifier}' column {ordinal + 1} is '{actualColumns[ordinal].Name}', but the sanctioned schema expects '{expectedColumns[ordinal].FieldName}'.");
            }

            targetColumnLinks.Add(new ValidationTargetColumnLink
            {
                Id = $"{targetRowsetLinkId}:column:{targetColumnLinks.Count + 1}",
                ValidationTargetRowsetLinkId = targetRowsetLinkId,
                ColumnId = actualColumns[ordinal].Id,
                MetaSchemaFieldId = expectedColumns[ordinal].FieldId
            });
        }
    }

    private static void ThrowResolutionFailure(
        bool isSource,
        string sqlIdentifier,
        SchemaTableResolutionResult resolution)
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

        throw new TransformBindingValidationException(code, message);
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

    private static int ParseOrdinal(string ordinal)
    {
        return int.TryParse(ordinal, out var value)
            ? value
            : int.MaxValue;
    }
}
