using Meta.Core.Domain;
using MetaDataTypeConversion.Core;
using MetaSchema;
using MetaTransformBinding;

namespace MetaTransform.Binding;

public sealed class TransformBindingValidationService
{
    private readonly IMetaDataTypeConversionService dataTypeConversionService;
    private readonly Workspace dataTypeConversionWorkspace;

    public TransformBindingValidationService()
        : this(
            new MetaDataTypeConversionService(),
            MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace())
    {
    }

    internal TransformBindingValidationService(
        IMetaDataTypeConversionService dataTypeConversionService,
        Workspace dataTypeConversionWorkspace)
    {
        this.dataTypeConversionService = dataTypeConversionService ?? throw new ArgumentNullException(nameof(dataTypeConversionService));
        this.dataTypeConversionWorkspace = dataTypeConversionWorkspace ?? throw new ArgumentNullException(nameof(dataTypeConversionWorkspace));
    }

    public MetaTransformBindingModel ApplyValidation(
        MetaTransformBindingModel bindingModel,
        MetaSchemaModel sourceSchemaModel,
        MetaSchemaModel targetSchemaModel,
        TransformBindingValidationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(bindingModel);
        ArgumentNullException.ThrowIfNull(sourceSchemaModel);
        ArgumentNullException.ThrowIfNull(targetSchemaModel);

        var resolvedOptions = options ?? TransformBindingValidationOptions.Default;
        var sourceResolver = new MetaSchemaTableResolver(sourceSchemaModel);
        var targetResolver = new MetaSchemaTableResolver(targetSchemaModel);

        var validations = new List<Validation>();
        var sourceRowsetLinks = new List<ValidationSourceRowsetLink>();
        var targetRowsetLinks = new List<ValidationTargetRowsetLink>();
        var sourceColumnLinks = new List<ValidationSourceColumnLink>();
        var targetColumnLinks = new List<ValidationTargetColumnLink>();
        var targetColumnTypeExactRows = new List<ValidationTargetColumnTypeExact>();
        var targetColumnTypeSanctionedConversionRows = new List<ValidationTargetColumnTypeSanctionedConversion>();
        var targetColumnTypeNotClassifiedRows = new List<ValidationTargetColumnTypeNotClassified>();
        var targetIgnoredColumnRows = new List<ValidationTargetIgnoredColumn>();

        foreach (var binding in bindingModel.TransformBindingList)
        {
            ApplyValidation(
                bindingModel,
                binding,
                sourceResolver,
                targetResolver,
                resolvedOptions,
                dataTypeConversionService,
                dataTypeConversionWorkspace,
                validations,
                sourceRowsetLinks,
                targetRowsetLinks,
                sourceColumnLinks,
                targetColumnLinks,
                targetColumnTypeExactRows,
                targetColumnTypeSanctionedConversionRows,
                targetColumnTypeNotClassifiedRows,
                targetIgnoredColumnRows);
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
        bindingModel.ValidationTargetColumnTypeExactList.Clear();
        bindingModel.ValidationTargetColumnTypeExactList.AddRange(targetColumnTypeExactRows);
        bindingModel.ValidationTargetColumnTypeSanctionedConversionList.Clear();
        bindingModel.ValidationTargetColumnTypeSanctionedConversionList.AddRange(targetColumnTypeSanctionedConversionRows);
        bindingModel.ValidationTargetColumnTypeNotClassifiedList.Clear();
        bindingModel.ValidationTargetColumnTypeNotClassifiedList.AddRange(targetColumnTypeNotClassifiedRows);
        bindingModel.ValidationTargetIgnoredColumnList.Clear();
        bindingModel.ValidationTargetIgnoredColumnList.AddRange(targetIgnoredColumnRows);

        return bindingModel;
    }

    private static void ApplyValidation(
        MetaTransformBindingModel model,
        TransformBinding binding,
        MetaSchemaTableResolver sourceResolver,
        MetaSchemaTableResolver targetResolver,
        TransformBindingValidationOptions options,
        IMetaDataTypeConversionService dataTypeConversionService,
        Workspace dataTypeConversionWorkspace,
        List<Validation> validations,
        List<ValidationSourceRowsetLink> sourceRowsetLinks,
        List<ValidationTargetRowsetLink> targetRowsetLinks,
        List<ValidationSourceColumnLink> sourceColumnLinks,
        List<ValidationTargetColumnLink> targetColumnLinks,
        List<ValidationTargetColumnTypeExact> targetColumnTypeExactRows,
        List<ValidationTargetColumnTypeSanctionedConversion> targetColumnTypeSanctionedConversionRows,
        List<ValidationTargetColumnTypeNotClassified> targetColumnTypeNotClassifiedRows,
        List<ValidationTargetIgnoredColumn> targetIgnoredColumnRows)
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
        var sourceColumnTypeCandidatesByName = new Dictionary<string, List<ResolvedSourceColumnType>>(StringComparer.OrdinalIgnoreCase);

        foreach (var sourceRowset in model.RowsetList.Where(item =>
                     string.Equals(item.TransformBindingId, binding.Id, StringComparison.Ordinal) &&
                     string.Equals(item.DerivationKind, "Source", StringComparison.OrdinalIgnoreCase) &&
                     !string.IsNullOrWhiteSpace(item.SqlIdentifier)))
        {
            AddSourceValidation(
                validationId,
                sourceRowset,
                sourceResolver,
                options,
                dataTypeConversionService,
                dataTypeConversionWorkspace,
                rowsetColumnsByRowsetId,
                sourceColumnTypeCandidatesByName,
                sourceRowsetLinks,
                sourceColumnLinks);
        }

        foreach (var target in model.TransformBindingTargetList.Where(item => string.Equals(item.TransformBindingId, binding.Id, StringComparison.Ordinal)))
        {
            AddTargetValidation(
                validationId,
                target,
                targetResolver,
                options.IgnoredTargetColumnNames,
                dataTypeConversionService,
                dataTypeConversionWorkspace,
                finalRowset,
                rowsetColumnsByRowsetId,
                sourceColumnTypeCandidatesByName,
                targetRowsetLinks,
                targetColumnLinks,
                targetColumnTypeExactRows,
                targetColumnTypeSanctionedConversionRows,
                targetColumnTypeNotClassifiedRows,
                targetIgnoredColumnRows);
        }
    }

    private static void AddSourceValidation(
        string validationId,
        Rowset sourceRowset,
        MetaSchemaTableResolver sourceResolver,
        TransformBindingValidationOptions options,
        IMetaDataTypeConversionService dataTypeConversionService,
        Workspace dataTypeConversionWorkspace,
        IReadOnlyDictionary<string, Column[]> rowsetColumnsByRowsetId,
        Dictionary<string, List<ResolvedSourceColumnType>> sourceColumnTypeCandidatesByName,
        List<ValidationSourceRowsetLink> sourceRowsetLinks,
        List<ValidationSourceColumnLink> sourceColumnLinks)
    {
        var sqlIdentifier = sourceRowset.SqlIdentifier;
        var resolution = ResolveSourceSchemaIdentifier(sourceResolver, options, sqlIdentifier);
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

            var sourceMetaDataTypeResolution = ResolveMetaDataTypeResolution(
                dataTypeConversionService,
                dataTypeConversionWorkspace,
                matchedField.MetaDataTypeId,
                "SourceSchemaFieldMetaDataTypeMissing",
                "SourceSchemaFieldMetaDataTypeNotSanctioned",
                $"Source schema field '{resolution.Table.CanonicalSqlIdentifier}.{matchedField.FieldName}'");

            sourceColumnLinks.Add(new ValidationSourceColumnLink
            {
                Id = $"{sourceRowsetLinkId}:column:{sourceColumnLinks.Count + 1}",
                ValidationSourceRowsetLinkId = sourceRowsetLinkId,
                ColumnId = actualColumn.Id,
                MetaSchemaFieldId = matchedField.FieldId
            });

            if (!sourceColumnTypeCandidatesByName.TryGetValue(actualColumn.Name, out var candidates))
            {
                candidates = [];
                sourceColumnTypeCandidatesByName.Add(actualColumn.Name, candidates);
            }

            candidates.Add(new ResolvedSourceColumnType(
                actualColumn.Name,
                sourceMetaDataTypeResolution.SourceDataTypeId,
                sourceMetaDataTypeResolution.TargetDataTypeId,
                matchedField.IsNullable,
                matchedField.Length,
                matchedField.Precision,
                matchedField.Scale,
                $"{resolution.Table.CanonicalSqlIdentifier}.{matchedField.FieldName}"));
        }
    }

    private static void AddTargetValidation(
        string validationId,
        TransformBindingTarget target,
        MetaSchemaTableResolver resolver,
        IReadOnlySet<string> ignoredTargetColumnNames,
        IMetaDataTypeConversionService dataTypeConversionService,
        Workspace dataTypeConversionWorkspace,
        Rowset? finalRowset,
        IReadOnlyDictionary<string, Column[]> rowsetColumnsByRowsetId,
        IReadOnlyDictionary<string, List<ResolvedSourceColumnType>> sourceColumnTypeCandidatesByName,
        List<ValidationTargetRowsetLink> targetRowsetLinks,
        List<ValidationTargetColumnLink> targetColumnLinks,
        List<ValidationTargetColumnTypeExact> targetColumnTypeExactRows,
        List<ValidationTargetColumnTypeSanctionedConversion> targetColumnTypeSanctionedConversionRows,
        List<ValidationTargetColumnTypeNotClassified> targetColumnTypeNotClassifiedRows,
        List<ValidationTargetIgnoredColumn> targetIgnoredColumnRows)
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
        var allNonIdentityExpectedColumns = resolution.Table!.Fields
            .Where(item => !item.IsIdentity)
            .OrderBy(item => item.Ordinal)
            .ToArray();
        var expectedColumnsByName = allNonIdentityExpectedColumns
            .ToDictionary(item => item.FieldName, StringComparer.OrdinalIgnoreCase);
        var ignoredTargetFields = new List<ResolvedSchemaField>();

        foreach (var ignoredColumnName in ignoredTargetColumnNames)
        {
            if (!expectedColumnsByName.TryGetValue(ignoredColumnName, out var ignoredField))
            {
                throw new TransformBindingValidationException(
                    "TargetIgnoredColumnNotFound",
                    $"Ignored target column '{ignoredColumnName}' was not found as a non-identity field on target table '{resolution.Table.CanonicalSqlIdentifier}'.");
            }

            ignoredTargetFields.Add(ignoredField);
        }

        var writeCandidateColumns = allNonIdentityExpectedColumns
            .Where(item => !ignoredTargetColumnNames.Contains(item.FieldName))
            .ToArray();
        var actualWriteColumns = actualColumns
            .Where(item => !IsAnonymousSyntheticOutputColumn(item, expectedColumnsByName))
            .Where(item => !IsDuplicateOutputColumnName(item, actualColumns))
            .ToArray();

        if (actualWriteColumns.Length > writeCandidateColumns.Length)
        {
            throw new TransformBindingValidationException(
                "TargetRowsetColumnCountMismatch",
                $"Final output rowset exposes {actualWriteColumns.Length} write-contracted column(s), but target table '{targetSqlIdentifier}' declares {writeCandidateColumns.Length} non-identity column(s).");
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

        foreach (var ignoredField in ignoredTargetFields.OrderBy(item => item.Ordinal))
        {
            targetIgnoredColumnRows.Add(new ValidationTargetIgnoredColumn
            {
                Id = $"{targetRowsetLinkId}:ignored:{targetIgnoredColumnRows.Count + 1}",
                ValidationTargetRowsetLinkId = targetRowsetLinkId,
                MetaSchemaFieldId = ignoredField.FieldId
            });
        }

        var writeCandidateColumnsByName = writeCandidateColumns
            .ToDictionary(item => item.FieldName, StringComparer.OrdinalIgnoreCase);
        var matchedWriteCandidateFieldIds = new HashSet<string>(StringComparer.Ordinal);

        for (var ordinal = 0; ordinal < actualWriteColumns.Length; ordinal++)
        {
            if (!writeCandidateColumnsByName.TryGetValue(actualWriteColumns[ordinal].Name, out var matchedTargetField))
            {
                throw new TransformBindingValidationException(
                    "TargetOutputColumnNotInSchema",
                    $"Final output rowset for target '{targetSqlIdentifier}' includes column '{actualWriteColumns[ordinal].Name}', but no writable non-identity target field with that name exists.");
            }

            if (!matchedWriteCandidateFieldIds.Add(matchedTargetField.FieldId))
            {
                throw new TransformBindingValidationException(
                    "TargetOutputColumnDuplicateMapping",
                    $"Final output rowset for target '{targetSqlIdentifier}' maps more than once to target field '{matchedTargetField.FieldName}'.");
            }

            var targetMetaDataTypeResolution = ResolveMetaDataTypeResolution(
                dataTypeConversionService,
                dataTypeConversionWorkspace,
                matchedTargetField.MetaDataTypeId,
                "TargetSchemaFieldMetaDataTypeMissing",
                "TargetSchemaFieldMetaDataTypeNotSanctioned",
                $"Target schema field '{resolution.Table.CanonicalSqlIdentifier}.{matchedTargetField.FieldName}'");

            var outputColumnName = actualWriteColumns[ordinal].Name;
            var targetColumnTypeAssessment = TargetColumnTypeAssessment.NotClassified;
            var sourceMetaDataTypeIdForCompatibility = string.Empty;
            if (sourceColumnTypeCandidatesByName.TryGetValue(outputColumnName, out var sourceCandidates) &&
                sourceCandidates.Count == 1)
            {
                var sourceCandidate = sourceCandidates[0];
                sourceMetaDataTypeIdForCompatibility = sourceCandidate.SourceMetaDataTypeId;
                if (!IsCanonicalTypeConformant(
                        sourceCandidate.CanonicalMetaDataTypeId,
                        targetMetaDataTypeResolution.TargetDataTypeId))
                {
                    throw new TransformBindingValidationException(
                        "TargetColumnTypeConformanceMismatch",
                        $"Final output column '{outputColumnName}' for target '{targetSqlIdentifier}' resolves from source '{sourceCandidate.SourceDisplayName}' with canonical type '{sourceCandidate.CanonicalMetaDataTypeId}', but target field '{resolution.Table.CanonicalSqlIdentifier}.{matchedTargetField.FieldName}' resolves to canonical type '{targetMetaDataTypeResolution.TargetDataTypeId}'.");
                }

                if (sourceCandidate.IsNullable == true && matchedTargetField.IsNullable == false)
                {
                    throw new TransformBindingValidationException(
                        "TargetColumnNullabilityConformanceMismatch",
                        $"Final output column '{outputColumnName}' for target '{targetSqlIdentifier}' resolves from source '{sourceCandidate.SourceDisplayName}' as nullable, but target field '{resolution.Table.CanonicalSqlIdentifier}.{matchedTargetField.FieldName}' is non-nullable.");
                }

                ValidateTypeDetailConformance(
                    detailName: "Length",
                    mismatchCode: "TargetColumnLengthConformanceMismatch",
                    sourceDetail: sourceCandidate.Length,
                    targetDetail: matchedTargetField.Length,
                    outputColumnName,
                    targetSqlIdentifier,
                    sourceCandidate.SourceDisplayName,
                    targetFieldDisplayName: $"{resolution.Table.CanonicalSqlIdentifier}.{matchedTargetField.FieldName}");

                ValidateTypeDetailConformance(
                    detailName: "Precision",
                    mismatchCode: "TargetColumnPrecisionConformanceMismatch",
                    sourceDetail: sourceCandidate.Precision,
                    targetDetail: matchedTargetField.Precision,
                    outputColumnName,
                    targetSqlIdentifier,
                    sourceCandidate.SourceDisplayName,
                    targetFieldDisplayName: $"{resolution.Table.CanonicalSqlIdentifier}.{matchedTargetField.FieldName}");

                ValidateTypeDetailConformance(
                    detailName: "Scale",
                    mismatchCode: "TargetColumnScaleConformanceMismatch",
                    sourceDetail: sourceCandidate.Scale,
                    targetDetail: matchedTargetField.Scale,
                    outputColumnName,
                    targetSqlIdentifier,
                    sourceCandidate.SourceDisplayName,
                    targetFieldDisplayName: $"{resolution.Table.CanonicalSqlIdentifier}.{matchedTargetField.FieldName}");

                targetColumnTypeAssessment = ClassifyDataTypeConformance(
                    sourceCandidate.SourceMetaDataTypeId,
                    targetMetaDataTypeResolution.SourceDataTypeId);
            }

            targetColumnLinks.Add(new ValidationTargetColumnLink
            {
                Id = $"{targetRowsetLinkId}:column:{targetColumnLinks.Count + 1}",
                ValidationTargetRowsetLinkId = targetRowsetLinkId,
                ColumnId = actualWriteColumns[ordinal].Id,
                MetaSchemaFieldId = matchedTargetField.FieldId
            });

            var targetColumnLink = targetColumnLinks[^1];
            AppendTargetColumnTypeAssessment(
                targetColumnTypeAssessment,
                targetColumnLink.Id,
                sourceMetaDataTypeIdForCompatibility,
                targetMetaDataTypeResolution.SourceDataTypeId,
                targetColumnTypeExactRows,
                targetColumnTypeSanctionedConversionRows,
                targetColumnTypeNotClassifiedRows);
        }

        var missingRequiredColumns = writeCandidateColumns
            .Where(IsRequiredWriteColumn)
            .Where(item => !matchedWriteCandidateFieldIds.Contains(item.FieldId))
            .OrderBy(item => item.Ordinal)
            .ToArray();
        if (missingRequiredColumns.Length > 0)
        {
            var missingNames = string.Join(", ", missingRequiredColumns.Select(item => item.FieldName));
            throw new TransformBindingValidationException(
                "TargetRequiredColumnMissing",
                $"Final output rowset for target '{targetSqlIdentifier}' is missing required writable target column(s): {missingNames}.");
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

    private static SchemaTableResolutionResult ResolveSourceSchemaIdentifier(
        MetaSchemaTableResolver sourceResolver,
        TransformBindingValidationOptions options,
        string sqlIdentifier)
    {
        var executeSystemName = options.ExecuteSystemName?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(executeSystemName))
        {
            return sourceResolver.ResolveSqlIdentifier(sqlIdentifier);
        }

        var expanded = SourceSqlIdentifierExpansion.Expand(
            sqlIdentifier,
            executeSystemName,
            options.ExecuteSystemDefaultSchemaName);

        if (!expanded.IsSuccess)
        {
            throw new TransformBindingValidationException(
                expanded.FailureKind switch
                {
                    SourceSqlIdentifierExpansionFailureKind.MissingIdentifier => "SourceSchemaIdentifierMissing",
                    SourceSqlIdentifierExpansionFailureKind.MissingExecuteSystem => "SourceSchemaExecuteSystemMissing",
                    SourceSqlIdentifierExpansionFailureKind.MissingDefaultSchemaName => "SourceSchemaExecuteSystemDefaultSchemaNameMissing",
                    SourceSqlIdentifierExpansionFailureKind.UnsupportedIdentifierShape => "SourceSchemaIdentifierShapeUnsupported",
                    _ => "SourceSchemaResolutionFailed"
                },
                expanded.FailureKind switch
                {
                    SourceSqlIdentifierExpansionFailureKind.MissingIdentifier =>
                        $"Declared source identifier '{sqlIdentifier}' is blank and cannot be resolved against the sanctioned source schema workspace(s).",
                    SourceSqlIdentifierExpansionFailureKind.MissingExecuteSystem =>
                        $"Declared source identifier '{sqlIdentifier}' requires --execute-system for source-schema resolution.",
                    SourceSqlIdentifierExpansionFailureKind.MissingDefaultSchemaName =>
                        $"Declared source identifier '{sqlIdentifier}' is one-part and requires --execute-system-default-schema-name for source-schema resolution.",
                    SourceSqlIdentifierExpansionFailureKind.UnsupportedIdentifierShape =>
                        $"Declared source identifier '{sqlIdentifier}' uses an unsupported identifier shape for source-schema resolution.",
                    _ =>
                        $"Declared source identifier '{sqlIdentifier}' could not be expanded for source-schema resolution."
                });
        }

        return sourceResolver.ResolveIdentifierParts(expanded.ExpandedIdentifierParts);
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

    private static DataTypeMappingResolution ResolveMetaDataTypeResolution(
        IMetaDataTypeConversionService dataTypeConversionService,
        Workspace dataTypeConversionWorkspace,
        string metaDataTypeId,
        string missingCode,
        string notSanctionedCode,
        string fieldDisplayName)
    {
        if (string.IsNullOrWhiteSpace(metaDataTypeId))
        {
            throw new TransformBindingValidationException(
                missingCode,
                $"{fieldDisplayName} is missing required MetaDataTypeId.");
        }

        try
        {
            return dataTypeConversionService.Resolve(dataTypeConversionWorkspace, metaDataTypeId.Trim());
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            throw new TransformBindingValidationException(
                notSanctionedCode,
                $"{fieldDisplayName} uses MetaDataTypeId '{metaDataTypeId}', which is not sanctioned by MetaDataTypeConversion. {ex.Message}");
        }
    }

    private static void ValidateTypeDetailConformance(
        string detailName,
        string mismatchCode,
        int? sourceDetail,
        int? targetDetail,
        string outputColumnName,
        string targetSqlIdentifier,
        string sourceDisplayName,
        string targetFieldDisplayName)
    {
        if (!sourceDetail.HasValue || !targetDetail.HasValue)
        {
            return;
        }

        if (sourceDetail.Value <= targetDetail.Value)
        {
            return;
        }

        throw new TransformBindingValidationException(
            mismatchCode,
            $"Final output column '{outputColumnName}' for target '{targetSqlIdentifier}' resolves from source '{sourceDisplayName}' with {detailName} '{sourceDetail.Value}', but target field '{targetFieldDisplayName}' declares {detailName} '{targetDetail.Value}'.");
    }

    private static bool IsAnonymousSyntheticOutputColumn(
        Column outputColumn,
        IReadOnlyDictionary<string, ResolvedSchemaField> expectedColumnsByName)
    {
        if (expectedColumnsByName.ContainsKey(outputColumn.Name))
        {
            return false;
        }

        return IsSyntheticExpressionOutputName(outputColumn.Name);
    }

    private static bool IsSyntheticExpressionOutputName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            !name.StartsWith("Expr", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var suffix = name.Substring(4);
        return suffix.Length > 0 && suffix.All(char.IsDigit);
    }

    private static bool IsDuplicateOutputColumnName(Column outputColumn, IReadOnlyList<Column> allOutputColumns)
    {
        var firstMatch = allOutputColumns
            .FirstOrDefault(item => string.Equals(item.Name, outputColumn.Name, StringComparison.OrdinalIgnoreCase));
        if (firstMatch is null)
        {
            return false;
        }

        return !string.Equals(firstMatch.Id, outputColumn.Id, StringComparison.Ordinal);
    }

    private static bool IsRequiredWriteColumn(ResolvedSchemaField field)
    {
        return field.IsNullable != true;
    }

    private static bool IsCanonicalTypeConformant(
        string sourceCanonicalMetaDataTypeId,
        string targetCanonicalMetaDataTypeId)
    {
        if (string.Equals(sourceCanonicalMetaDataTypeId, targetCanonicalMetaDataTypeId, StringComparison.Ordinal))
        {
            return true;
        }

        var targetIsUnicodeStringFamily =
            string.Equals(targetCanonicalMetaDataTypeId, "meta:type:String", StringComparison.Ordinal) ||
            string.Equals(targetCanonicalMetaDataTypeId, "meta:type:StringFixedLength", StringComparison.Ordinal);
        if (!targetIsUnicodeStringFamily)
        {
            return false;
        }

        // SQL Server workflows frequently widen to Unicode text in view/result metadata.
        // Treat selected source canonical families as sanctioned conversions to Unicode targets.
        return sourceCanonicalMetaDataTypeId switch
        {
            "meta:type:AnsiString" => true,
            "meta:type:AnsiStringFixedLength" => true,
            "meta:type:Int32" => true,
            "meta:type:Decimal" => true,
            _ => false
        };
    }

    private static TargetColumnTypeAssessment ClassifyDataTypeConformance(
        string sourceMetaDataTypeId,
        string targetMetaDataTypeId)
    {
        return string.Equals(sourceMetaDataTypeId, targetMetaDataTypeId, StringComparison.OrdinalIgnoreCase)
            ? TargetColumnTypeAssessment.Exact
            : TargetColumnTypeAssessment.SanctionedConversion;
    }

    private static void AppendTargetColumnTypeAssessment(
        TargetColumnTypeAssessment assessment,
        string validationTargetColumnLinkId,
        string sourceMetaDataTypeId,
        string targetMetaDataTypeId,
        List<ValidationTargetColumnTypeExact> targetColumnTypeExactRows,
        List<ValidationTargetColumnTypeSanctionedConversion> targetColumnTypeSanctionedConversionRows,
        List<ValidationTargetColumnTypeNotClassified> targetColumnTypeNotClassifiedRows)
    {
        switch (assessment)
        {
            case TargetColumnTypeAssessment.Exact:
                targetColumnTypeExactRows.Add(new ValidationTargetColumnTypeExact
                {
                    Id = $"{validationTargetColumnLinkId}:type-exact",
                    ValidationTargetColumnLinkId = validationTargetColumnLinkId,
                    SourceMetaDataTypeId = sourceMetaDataTypeId,
                    TargetMetaDataTypeId = targetMetaDataTypeId
                });
                return;

            case TargetColumnTypeAssessment.SanctionedConversion:
                targetColumnTypeSanctionedConversionRows.Add(new ValidationTargetColumnTypeSanctionedConversion
                {
                    Id = $"{validationTargetColumnLinkId}:type-sanctioned-conversion",
                    ValidationTargetColumnLinkId = validationTargetColumnLinkId,
                    SourceMetaDataTypeId = sourceMetaDataTypeId,
                    TargetMetaDataTypeId = targetMetaDataTypeId
                });
                return;

            case TargetColumnTypeAssessment.NotClassified:
                targetColumnTypeNotClassifiedRows.Add(new ValidationTargetColumnTypeNotClassified
                {
                    Id = $"{validationTargetColumnLinkId}:type-not-classified",
                    ValidationTargetColumnLinkId = validationTargetColumnLinkId,
                    TargetMetaDataTypeId = targetMetaDataTypeId
                });
                return;

            default:
                throw new TransformBindingValidationException(
                    "TargetColumnTypeAssessmentUnknown",
                    $"Unknown target-column type assessment '{assessment}'.");
        }
    }

    private enum TargetColumnTypeAssessment
    {
        Exact,
        SanctionedConversion,
        NotClassified
    }

    private sealed record ResolvedSourceColumnType(
        string ColumnName,
        string SourceMetaDataTypeId,
        string CanonicalMetaDataTypeId,
        bool? IsNullable,
        int? Length,
        int? Precision,
        int? Scale,
        string SourceDisplayName);
}
