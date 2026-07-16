using MetaDataTypeConversion;
using MetaDataTypeConversion.Core;
using MetaSchema;
using MetaTransformBinding;

namespace MetaTransform.Binding;

public sealed class TransformBindingValidationService
{
    private readonly IMetaDataTypeConversionService dataTypeConversionService;
    private readonly MetaDataTypeConversionModel dataTypeConversionWorkspace;

    public TransformBindingValidationService()
        : this(
            new MetaDataTypeConversionService(),
            MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace())
    {
    }

    internal TransformBindingValidationService(
        IMetaDataTypeConversionService dataTypeConversionService,
        MetaDataTypeConversionModel dataTypeConversionWorkspace)
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
        return ApplyValidation(
            bindingModel,
            sourceSchemaModel,
            targetSchemaModel,
            boundResults: [],
            options);
    }

    internal MetaTransformBindingModel ApplyValidation(
        MetaTransformBindingModel bindingModel,
        MetaSchemaModel sourceSchemaModel,
        MetaSchemaModel targetSchemaModel,
        IReadOnlyList<TransformBindingResult> boundResults,
        TransformBindingValidationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(bindingModel);
        ArgumentNullException.ThrowIfNull(sourceSchemaModel);
        ArgumentNullException.ThrowIfNull(targetSchemaModel);
        ArgumentNullException.ThrowIfNull(boundResults);

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
        var targetIgnoredColumnRows = new List<ValidationTargetIgnoredColumn>();
        var writes = new List<Write>();
        var writeValues = new List<WriteValue>();
        var writeValueScalarExpressions = new List<WriteValueScalarExpression>();
        var insertQueryWrites = new List<InsertQueryWrite>();
        var insertValuesWrites = new List<InsertValuesWrite>();
        var updateWrites = new List<UpdateWrite>();
        var mergeInsertWrites = new List<MergeInsertWrite>();
        var mergeUpdateWrites = new List<MergeUpdateWrite>();
        var deletes = new List<Delete>();
        var mergeDeletes = new List<MergeDelete>();
        var truncates = new List<Truncate>();
        var targetColumnReferences = new List<TargetColumnReference>();
        var mutationEffectsByBindingId = boundResults
            .GroupBy(item => $"{item.TransformScriptId}:binding", StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.SelectMany(item => item.MutationEffects).ToArray(),
                StringComparer.Ordinal);
        var rowsetsById = bindingModel.RowsetList
            .ToDictionary(item => item.Id, StringComparer.Ordinal);
        var columnsById = bindingModel.ColumnList
            .ToDictionary(item => item.Id, StringComparer.Ordinal);
        var runtimeColumnReferencesByBindingId = boundResults
            .GroupBy(item => $"{item.TransformScriptId}:binding", StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.SelectMany(item => item.ColumnReferences).ToArray(),
                StringComparer.Ordinal);

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
                mutationEffectsByBindingId.GetValueOrDefault(binding.Id) ?? [],
                rowsetsById,
                columnsById,
                runtimeColumnReferencesByBindingId.GetValueOrDefault(binding.Id) ?? [],
                targetIgnoredColumnRows,
                writes,
                writeValues,
                writeValueScalarExpressions,
                insertQueryWrites,
                insertValuesWrites,
                updateWrites,
                mergeInsertWrites,
                mergeUpdateWrites,
                deletes,
                mergeDeletes,
                truncates,
                targetColumnReferences);
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
        bindingModel.ValidationTargetIgnoredColumnList.Clear();
        bindingModel.ValidationTargetIgnoredColumnList.AddRange(targetIgnoredColumnRows);
        bindingModel.WriteList.Clear();
        bindingModel.WriteList.AddRange(writes);
        bindingModel.WriteValueList.Clear();
        bindingModel.WriteValueList.AddRange(writeValues);
        bindingModel.WriteValueScalarExpressionList.Clear();
        bindingModel.WriteValueScalarExpressionList.AddRange(writeValueScalarExpressions);
        bindingModel.InsertQueryWriteList.Clear();
        bindingModel.InsertQueryWriteList.AddRange(insertQueryWrites);
        bindingModel.InsertValuesWriteList.Clear();
        bindingModel.InsertValuesWriteList.AddRange(insertValuesWrites);
        bindingModel.UpdateWriteList.Clear();
        bindingModel.UpdateWriteList.AddRange(updateWrites);
        bindingModel.MergeInsertWriteList.Clear();
        bindingModel.MergeInsertWriteList.AddRange(mergeInsertWrites);
        bindingModel.MergeUpdateWriteList.Clear();
        bindingModel.MergeUpdateWriteList.AddRange(mergeUpdateWrites);
        bindingModel.DeleteList.Clear();
        bindingModel.DeleteList.AddRange(deletes);
        bindingModel.MergeDeleteList.Clear();
        bindingModel.MergeDeleteList.AddRange(mergeDeletes);
        bindingModel.TruncateList.Clear();
        bindingModel.TruncateList.AddRange(truncates);
        bindingModel.TargetColumnReferenceList.Clear();
        bindingModel.TargetColumnReferenceList.AddRange(targetColumnReferences);

        return bindingModel;
    }

    private static void ApplyValidation(
        MetaTransformBindingModel model,
        TransformBinding binding,
        MetaSchemaTableResolver sourceResolver,
        MetaSchemaTableResolver targetResolver,
        TransformBindingValidationOptions options,
        IMetaDataTypeConversionService dataTypeConversionService,
        MetaDataTypeConversionModel dataTypeConversionWorkspace,
        List<Validation> validations,
        List<ValidationSourceRowsetLink> sourceRowsetLinks,
        List<ValidationTargetRowsetLink> targetRowsetLinks,
        List<ValidationSourceColumnLink> sourceColumnLinks,
        List<ValidationTargetColumnLink> targetColumnLinks,
        List<ValidationTargetColumnTypeExact> targetColumnTypeExactRows,
        List<ValidationTargetColumnTypeSanctionedConversion> targetColumnTypeSanctionedConversionRows,
        IReadOnlyList<RuntimeMutationEffect> mutationEffects,
        IReadOnlyDictionary<string, Rowset> rowsetsById,
        IReadOnlyDictionary<string, Column> columnsById,
        IReadOnlyList<RuntimeColumnReference> runtimeColumnReferences,
        List<ValidationTargetIgnoredColumn> targetIgnoredColumnRows,
        List<Write> writes,
        List<WriteValue> writeValues,
        List<WriteValueScalarExpression> writeValueScalarExpressions,
        List<InsertQueryWrite> insertQueryWrites,
        List<InsertValuesWrite> insertValuesWrites,
        List<UpdateWrite> updateWrites,
        List<MergeInsertWrite> mergeInsertWrites,
        List<MergeUpdateWrite> mergeUpdateWrites,
        List<Delete> deletes,
        List<MergeDelete> mergeDeletes,
        List<Truncate> truncates,
        List<TargetColumnReference> targetColumnReferences)
    {
        var validationId = $"{binding.Id}:validation";
        var validation = new Validation
        {
            Id = validationId,
            TransformBinding = binding
        };
        validations.Add(validation);

        var finalRowset = ResolveFinalRowset(model, binding.Id);
        var rowsetColumnsByRowsetId = model.ColumnList
            .GroupBy(item => item.Rowset.Id, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(item => ParseOrdinal(item.Ordinal))
                    .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                    .ToArray(),
                StringComparer.Ordinal);
        var sourceColumnTypeCandidatesByName = new Dictionary<string, List<ResolvedSourceColumnType>>(StringComparer.OrdinalIgnoreCase);

        foreach (var sourceRowset in model.RowsetList.Where(item =>
                     string.Equals(item.TransformBinding.Id, binding.Id, StringComparison.Ordinal) &&
                     string.Equals(item.DerivationKind, "Source", StringComparison.OrdinalIgnoreCase) &&
                     !string.IsNullOrWhiteSpace(item.SqlIdentifier)))
        {
            AddSourceValidation(
                validation,
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

        foreach (var target in model.TransformBindingTargetList.Where(item => string.Equals(item.TransformBinding.Id, binding.Id, StringComparison.Ordinal)))
        {
            AddTargetValidation(
                validation,
                target,
                targetResolver,
                options.IgnoredTargetColumnNames,
                options.IgnoredTargetColumnNamesIfPresent,
                dataTypeConversionService,
                dataTypeConversionWorkspace,
                finalRowset,
                rowsetColumnsByRowsetId,
                sourceColumnTypeCandidatesByName,
                targetRowsetLinks,
                targetColumnLinks,
                targetColumnTypeExactRows,
                targetColumnTypeSanctionedConversionRows,
                mutationEffects,
                rowsetsById,
                columnsById,
                runtimeColumnReferences,
                targetIgnoredColumnRows,
                writes,
                writeValues,
                writeValueScalarExpressions,
                insertQueryWrites,
                insertValuesWrites,
                updateWrites,
                mergeInsertWrites,
                mergeUpdateWrites,
                deletes,
                mergeDeletes,
                truncates,
                targetColumnReferences);
        }
    }

    private static void AddSourceValidation(
        Validation validation,
        Rowset sourceRowset,
        MetaSchemaTableResolver sourceResolver,
        TransformBindingValidationOptions options,
        IMetaDataTypeConversionService dataTypeConversionService,
        MetaDataTypeConversionModel dataTypeConversionWorkspace,
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

        var sourceRowsetLinkId = $"{validation.Id}:source:{sourceRowsetLinks.Count + 1}";
        var sourceRowsetLink = new ValidationSourceRowsetLink
        {
            Id = sourceRowsetLinkId,
            Validation = validation,
            Rowset = sourceRowset,
            MetaSchemaTableId = resolution.Table!.TableId
        };
        sourceRowsetLinks.Add(sourceRowsetLink);

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

            var sourceMetaDataTypeId = EnsureMetaDataTypeKnown(
                dataTypeConversionService,
                dataTypeConversionWorkspace,
                matchedField.MetaDataTypeId,
                "SourceSchemaFieldMetaDataTypeMissing",
                "SourceSchemaFieldMetaDataTypeNotSanctioned",
                $"Source schema field '{resolution.Table.CanonicalSqlIdentifier}.{matchedField.FieldName}'");

            sourceColumnLinks.Add(new ValidationSourceColumnLink
            {
                Id = $"{sourceRowsetLinkId}:column:{sourceColumnLinks.Count + 1}",
                ValidationSourceRowsetLink = sourceRowsetLink,
                Column = actualColumn,
                MetaSchemaFieldId = matchedField.FieldId
            });

            if (!sourceColumnTypeCandidatesByName.TryGetValue(actualColumn.Name, out var candidates))
            {
                candidates = [];
                sourceColumnTypeCandidatesByName.Add(actualColumn.Name, candidates);
            }

            candidates.Add(new ResolvedSourceColumnType(
                actualColumn.Name,
                sourceMetaDataTypeId,
                matchedField.IsNullable,
                matchedField.Length,
                matchedField.Precision,
                matchedField.Scale,
                $"{resolution.Table.CanonicalSqlIdentifier}.{matchedField.FieldName}"));
        }
    }

    private static void AddTargetValidation(
        Validation validation,
        TransformBindingTarget target,
        MetaSchemaTableResolver resolver,
        IReadOnlySet<string> ignoredTargetColumnNames,
        IReadOnlySet<string> ignoredTargetColumnNamesIfPresent,
        IMetaDataTypeConversionService dataTypeConversionService,
        MetaDataTypeConversionModel dataTypeConversionWorkspace,
        Rowset? finalRowset,
        IReadOnlyDictionary<string, Column[]> rowsetColumnsByRowsetId,
        IReadOnlyDictionary<string, List<ResolvedSourceColumnType>> sourceColumnTypeCandidatesByName,
        List<ValidationTargetRowsetLink> targetRowsetLinks,
        List<ValidationTargetColumnLink> targetColumnLinks,
        List<ValidationTargetColumnTypeExact> targetColumnTypeExactRows,
        List<ValidationTargetColumnTypeSanctionedConversion> targetColumnTypeSanctionedConversionRows,
        IReadOnlyList<RuntimeMutationEffect> mutationEffects,
        IReadOnlyDictionary<string, Rowset> rowsetsById,
        IReadOnlyDictionary<string, Column> columnsById,
        IReadOnlyList<RuntimeColumnReference> runtimeColumnReferences,
        List<ValidationTargetIgnoredColumn> targetIgnoredColumnRows,
        List<Write> writes,
        List<WriteValue> writeValues,
        List<WriteValueScalarExpression> writeValueScalarExpressions,
        List<InsertQueryWrite> insertQueryWrites,
        List<InsertValuesWrite> insertValuesWrites,
        List<UpdateWrite> updateWrites,
        List<MergeInsertWrite> mergeInsertWrites,
        List<MergeUpdateWrite> mergeUpdateWrites,
        List<Delete> deletes,
        List<MergeDelete> mergeDeletes,
        List<Truncate> truncates,
        List<TargetColumnReference> targetColumnReferences)
    {
        var targetSqlIdentifier = target.SqlIdentifier;
        var resolution = resolver.ResolveSqlIdentifier(targetSqlIdentifier);
        if (!resolution.IsResolved)
        {
            ThrowResolutionFailure(isSource: false, targetSqlIdentifier, resolution);
        }

        EnsureWritableTargetContract(targetSqlIdentifier, resolution.Table!);

        var matchingMutationEffects = mutationEffects
            .Where(item => string.Equals(item.TargetSqlIdentifier, targetSqlIdentifier, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        AddTargetColumnReferences(
            target,
            resolution.Table!,
            matchingMutationEffects,
            runtimeColumnReferences,
            rowsetsById,
            columnsById,
            targetColumnReferences);

        if (matchingMutationEffects.Length > 0)
        {
            foreach (var mutationEffect in matchingMutationEffects)
            {
                switch (mutationEffect)
                {
                    case RuntimeWriteEffect writeEffect:
                    {
                        var writeValidation = AddStrictTargetValidation(
                            validation,
                            target,
                            resolution.Table!,
                            ignoredTargetColumnNames,
                            ignoredTargetColumnNamesIfPresent,
                            dataTypeConversionService,
                            dataTypeConversionWorkspace,
                            writeEffect,
                            rowsetColumnsByRowsetId,
                            targetRowsetLinks,
                            targetColumnLinks,
                            targetColumnTypeExactRows,
                            targetColumnTypeSanctionedConversionRows,
                            rowsetsById,
                            targetIgnoredColumnRows);
                        AddWriteFacts(
                            writeEffect,
                            writeValidation,
                            writes,
                            writeValues,
                            writeValueScalarExpressions,
                            insertQueryWrites,
                            insertValuesWrites,
                            updateWrites,
                            mergeInsertWrites,
                            mergeUpdateWrites);
                        break;
                    }

                    case RuntimeDeleteEffect deleteEffect:
                    {
                        var deleteTargetRowsetLink = AddMutationTargetValidation(
                            validation,
                            target,
                            resolution.Table!,
                            deleteEffect,
                            rowsetsById,
                            targetRowsetLinks);
                        deletes.Add(new Delete
                        {
                            Id = $"{deleteTargetRowsetLink.Id}:delete",
                            ValidationTargetRowsetLink = deleteTargetRowsetLink,
                            MetaTransformScriptDeleteStatementId = deleteEffect.MetaTransformScriptDeleteStatementId
                        });
                        break;
                    }

                    case RuntimeMergeDeleteEffect mergeDeleteEffect:
                    {
                        var mergeDeleteTargetRowsetLink = AddMutationTargetValidation(
                            validation,
                            target,
                            resolution.Table!,
                            mergeDeleteEffect,
                            rowsetsById,
                            targetRowsetLinks);
                        mergeDeletes.Add(new MergeDelete
                        {
                            Id = $"{mergeDeleteTargetRowsetLink.Id}:merge-delete",
                            ValidationTargetRowsetLink = mergeDeleteTargetRowsetLink,
                            MetaTransformScriptMergeDeleteActionId = mergeDeleteEffect.MetaTransformScriptMergeDeleteActionId
                        });
                        break;
                    }

                    case RuntimeTruncateEffect truncateEffect:
                    {
                        var truncateTargetRowsetLink = AddMutationTargetValidation(
                            validation,
                            target,
                            resolution.Table!,
                            truncateEffect,
                            rowsetsById,
                            targetRowsetLinks);
                        truncates.Add(new Truncate
                        {
                            Id = $"{truncateTargetRowsetLink.Id}:truncate",
                            ValidationTargetRowsetLink = truncateTargetRowsetLink,
                            MetaTransformScriptTruncateStatementId = truncateEffect.MetaTransformScriptTruncateStatementId
                        });
                        break;
                    }

                    default:
                        throw new TransformBindingValidationException(
                            "MutationEffectUnsupported",
                            $"Transform binding target '{targetSqlIdentifier}' received an unsupported mutation effect '{mutationEffect.GetType().Name}'.");
                }
            }

            return;
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

        foreach (var ignoredColumnName in ignoredTargetColumnNamesIfPresent)
        {
            if (expectedColumnsByName.TryGetValue(ignoredColumnName, out var ignoredField) &&
                ignoredTargetFields.All(item => !string.Equals(item.FieldId, ignoredField.FieldId, StringComparison.Ordinal)))
            {
                ignoredTargetFields.Add(ignoredField);
            }
        }

        var ignoredTargetFieldIds = ignoredTargetFields
            .Select(item => item.FieldId)
            .ToHashSet(StringComparer.Ordinal);
        var writeCandidateColumns = allNonIdentityExpectedColumns
            .Where(item => !ignoredTargetFieldIds.Contains(item.FieldId))
            .ToArray();
        var actualWriteColumns = actualColumns
            .Where(item => !IsAnonymousSyntheticOutputColumn(item, expectedColumnsByName))
            .Where(item => !IsDuplicateOutputColumnName(item, actualColumns))
            .ToArray();

        var targetRowsetLinkId = $"{validation.Id}:target:{targetRowsetLinks.Count + 1}";
        var targetRowsetLink = new ValidationTargetRowsetLink
        {
            Id = targetRowsetLinkId,
            Validation = validation,
            TransformBindingTarget = target,
            Rowset = finalRowset,
            MetaSchemaTableId = resolution.Table.TableId
        };
        targetRowsetLinks.Add(targetRowsetLink);

        foreach (var ignoredField in ignoredTargetFields.OrderBy(item => item.Ordinal))
        {
            targetIgnoredColumnRows.Add(new ValidationTargetIgnoredColumn
            {
                Id = $"{targetRowsetLinkId}:ignored:{targetIgnoredColumnRows.Count + 1}",
                ValidationTargetRowsetLink = targetRowsetLink,
                MetaSchemaFieldId = ignoredField.FieldId
            });
        }

        if (IsMutationTargetRowset(finalRowset))
        {
            throw new TransformBindingValidationException(
                "StrictMutationTargetEvidenceMissing",
                $"Transform binding target '{targetSqlIdentifier}' is a mutation target. Strict target validation requires a supported mutation effect derived from the transform syntax.");
        }

        if (actualWriteColumns.Length > writeCandidateColumns.Length)
        {
            throw new TransformBindingValidationException(
                "TargetRowsetColumnCountMismatch",
                $"Final output rowset exposes {actualWriteColumns.Length} write-contracted column(s), but target table '{targetSqlIdentifier}' declares {writeCandidateColumns.Length} non-identity column(s).");
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

            var targetMetaDataTypeId = EnsureMetaDataTypeKnown(
                dataTypeConversionService,
                dataTypeConversionWorkspace,
                matchedTargetField.MetaDataTypeId,
                "TargetSchemaFieldMetaDataTypeMissing",
                "TargetSchemaFieldMetaDataTypeNotSanctioned",
                $"Target schema field '{resolution.Table.CanonicalSqlIdentifier}.{matchedTargetField.FieldName}'");

            var outputColumnName = actualWriteColumns[ordinal].Name;
            if (sourceColumnTypeCandidatesByName.TryGetValue(outputColumnName, out var sourceCandidates) &&
                sourceCandidates.Count == 1)
            {
                var sourceCandidate = sourceCandidates[0];
                var compatibility = ValidateTargetColumnTypeConformance(
                    dataTypeConversionService,
                    dataTypeConversionWorkspace,
                    sourceCandidate.SourceMetaDataTypeId,
                    sourceCandidate.IsNullable,
                    sourceCandidate.Length,
                    sourceCandidate.Precision,
                    sourceCandidate.Scale,
                    sourceCandidate.SourceDisplayName,
                    targetMetaDataTypeId,
                    matchedTargetField,
                    outputColumnName,
                    targetSqlIdentifier,
                    resolution.Table.CanonicalSqlIdentifier);

                var targetColumnLink = new ValidationTargetColumnLink
                {
                    Id = $"{targetRowsetLinkId}:column:{targetColumnLinks.Count + 1}",
                    ValidationTargetRowsetLink = targetRowsetLink,
                    Column = actualWriteColumns[ordinal],
                    MetaSchemaFieldId = matchedTargetField.FieldId
                };
                targetColumnLinks.Add(targetColumnLink);
                AppendTargetColumnTypeAssessment(
                    compatibility.IsExact,
                    targetColumnLink,
                    sourceCandidate.SourceMetaDataTypeId,
                    targetMetaDataTypeId,
                    targetColumnTypeExactRows,
                    targetColumnTypeSanctionedConversionRows);
            }
            else
            {
                targetColumnLinks.Add(new ValidationTargetColumnLink
                {
                    Id = $"{targetRowsetLinkId}:column:{targetColumnLinks.Count + 1}",
                    ValidationTargetRowsetLink = targetRowsetLink,
                    Column = actualWriteColumns[ordinal],
                    MetaSchemaFieldId = matchedTargetField.FieldId
                });
            }
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

    private static TargetWriteValidation AddStrictTargetValidation(
        Validation validation,
        TransformBindingTarget target,
        ResolvedSchemaTable targetTable,
        IReadOnlySet<string> ignoredTargetColumnNames,
        IReadOnlySet<string> ignoredTargetColumnNamesIfPresent,
        IMetaDataTypeConversionService dataTypeConversionService,
        MetaDataTypeConversionModel dataTypeConversionWorkspace,
        RuntimeWriteEffect writeEffect,
        IReadOnlyDictionary<string, Column[]> rowsetColumnsByRowsetId,
        List<ValidationTargetRowsetLink> targetRowsetLinks,
        List<ValidationTargetColumnLink> targetColumnLinks,
        List<ValidationTargetColumnTypeExact> targetColumnTypeExactRows,
        List<ValidationTargetColumnTypeSanctionedConversion> targetColumnTypeSanctionedConversionRows,
        IReadOnlyDictionary<string, Rowset> rowsetsById,
        List<ValidationTargetIgnoredColumn> targetIgnoredColumnRows)
    {
        var writeRowsetColumns = rowsetColumnsByRowsetId.GetValueOrDefault(writeEffect.ValueRowset.Id);
        if (writeRowsetColumns is null)
        {
            throw new TransformBindingValidationException(
                "TargetWriteRowsetMissing",
                $"Strict target validation for '{target.SqlIdentifier}' refers to missing write rowset '{writeEffect.ValueRowset.Id}'.");
        }

        var actualColumnsById = writeRowsetColumns.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var allNonIdentityTargetFields = targetTable.Fields
            .Where(item => !item.IsIdentity)
            .OrderBy(item => item.Ordinal)
            .ToArray();
        var targetFieldsByName = allNonIdentityTargetFields
            .ToDictionary(item => item.FieldName, StringComparer.OrdinalIgnoreCase);
        var ignoredTargetFields = ResolveIgnoredTargetFields(
            targetTable,
            targetFieldsByName,
            ignoredTargetColumnNames,
            ignoredTargetColumnNamesIfPresent);
        var ignoredTargetFieldIds = ignoredTargetFields
            .Select(item => item.FieldId)
            .ToHashSet(StringComparer.Ordinal);
        var writeCandidateFields = allNonIdentityTargetFields
            .Where(item => !ignoredTargetFieldIds.Contains(item.FieldId))
            .ToArray();

        if (!rowsetsById.TryGetValue(writeEffect.ValueRowset.Id, out _))
        {
            throw new TransformBindingValidationException(
                "TargetWriteRowsetMissing",
                $"Strict target validation for '{target.SqlIdentifier}' refers to missing write rowset '{writeEffect.ValueRowset.Id}'.");
        }

        if (!rowsetsById.TryGetValue(writeEffect.TargetRowset.Id, out var targetRowset))
        {
            throw new TransformBindingValidationException(
                "TargetRowsetMissing",
                $"Strict target validation for '{target.SqlIdentifier}' refers to missing target rowset '{writeEffect.TargetRowset.Id}'.");
        }

        var targetRowsetLinkId = $"{validation.Id}:target:{targetRowsetLinks.Count + 1}";
        var targetRowsetLink = new ValidationTargetRowsetLink
        {
            Id = targetRowsetLinkId,
            Validation = validation,
            TransformBindingTarget = target,
            Rowset = targetRowset,
            MetaSchemaTableId = targetTable.TableId
        };
        targetRowsetLinks.Add(targetRowsetLink);

        foreach (var ignoredTargetField in ignoredTargetFields.OrderBy(item => item.Ordinal))
        {
            targetIgnoredColumnRows.Add(new ValidationTargetIgnoredColumn
            {
                Id = $"{targetRowsetLinkId}:ignored:{targetIgnoredColumnRows.Count + 1}",
                ValidationTargetRowsetLink = targetRowsetLink,
                MetaSchemaFieldId = ignoredTargetField.FieldId
            });
        }

        var matchedTargetFieldIds = new HashSet<string>(StringComparer.Ordinal);
        var writeColumnLinks = new List<ValidationTargetColumnLink>();
        foreach (var item in writeEffect.Values.Select((value, ordinal) => (Value: value, Ordinal: ordinal)))
        {
            if (!actualColumnsById.TryGetValue(item.Value.ValueColumn.Id, out var actualColumn))
            {
                throw new TransformBindingValidationException(
                    "TargetWriteColumnMissing",
                    $"Strict target validation for '{target.SqlIdentifier}' refers to missing write column '{item.Value.ValueColumn.Id}'.");
            }

            var targetField = ResolveTargetField(
                item.Value.TargetFieldName,
                item.Ordinal,
                writeCandidateFields,
                targetFieldsByName,
                target.SqlIdentifier);
            if (!matchedTargetFieldIds.Add(targetField.FieldId))
            {
                throw new TransformBindingValidationException(
                    "TargetOutputColumnDuplicateMapping",
                $"Write for target '{target.SqlIdentifier}' maps more than once to field '{targetField.FieldName}'.");
            }

            var targetMetaDataTypeId = EnsureMetaDataTypeKnown(
                dataTypeConversionService,
                dataTypeConversionWorkspace,
                targetField.MetaDataTypeId,
                "TargetSchemaFieldMetaDataTypeMissing",
                "TargetSchemaFieldMetaDataTypeNotSanctioned",
                $"Target schema field '{targetTable.CanonicalSqlIdentifier}.{targetField.FieldName}'");
            var valueDataType = item.Value.ValueColumn.DataType;
            if (valueDataType is null)
            {
                throw new TransformBindingValidationException(
                    "TargetWriteValueTypeNotResolved",
                    $"Write value '{actualColumn.Name}' for target field '{targetTable.CanonicalSqlIdentifier}.{targetField.FieldName}' has no proven data type.");
            }

            var sourceMetaDataTypeId = EnsureMetaDataTypeKnown(
                dataTypeConversionService,
                dataTypeConversionWorkspace,
                valueDataType.MetaDataTypeId,
                "TargetWriteValueMetaDataTypeMissing",
                "TargetWriteValueMetaDataTypeNotSanctioned",
                $"Write value '{valueDataType.DisplayName}'");
            var compatibility = ValidateTargetColumnTypeConformance(
                dataTypeConversionService,
                dataTypeConversionWorkspace,
                sourceMetaDataTypeId,
                valueDataType.IsNullable,
                valueDataType.Length,
                valueDataType.Precision,
                valueDataType.Scale,
                valueDataType.DisplayName,
                targetMetaDataTypeId,
                targetField,
                actualColumn.Name,
                target.SqlIdentifier,
                targetTable.CanonicalSqlIdentifier);

            var targetColumnLink = new ValidationTargetColumnLink
            {
                Id = $"{targetRowsetLinkId}:column:{targetColumnLinks.Count + 1}",
                ValidationTargetRowsetLink = targetRowsetLink,
                Column = actualColumn,
                MetaSchemaFieldId = targetField.FieldId
            };
            targetColumnLinks.Add(targetColumnLink);
            writeColumnLinks.Add(targetColumnLink);
            AppendTargetColumnTypeAssessment(
                compatibility.IsExact,
                targetColumnLink,
                sourceMetaDataTypeId,
                targetMetaDataTypeId,
                targetColumnTypeExactRows,
                targetColumnTypeSanctionedConversionRows);
        }

        if (writeEffect.RequiresRequiredFieldCoverage)
        {
            var missingRequiredFields = writeCandidateFields
                .Where(IsRequiredWriteColumn)
                .Where(item => !matchedTargetFieldIds.Contains(item.FieldId))
                .ToArray();
            if (missingRequiredFields.Length > 0)
            {
                throw new TransformBindingValidationException(
                    "TargetRequiredColumnMissing",
                    $"Write for target '{target.SqlIdentifier}' is missing required writable target column(s): {string.Join(", ", missingRequiredFields.Select(item => item.FieldName))}.");
            }
        }

        return new TargetWriteValidation(targetRowsetLink, writeColumnLinks);
    }

    private static ValidationTargetRowsetLink AddMutationTargetValidation(
        Validation validation,
        TransformBindingTarget target,
        ResolvedSchemaTable targetTable,
        RuntimeMutationEffect mutationEffect,
        IReadOnlyDictionary<string, Rowset> rowsetsById,
        List<ValidationTargetRowsetLink> targetRowsetLinks)
    {
        if (!rowsetsById.TryGetValue(mutationEffect.TargetRowset.Id, out var targetRowset))
        {
            throw new TransformBindingValidationException(
                "TargetRowsetMissing",
                $"Mutation target '{target.SqlIdentifier}' refers to missing target rowset '{mutationEffect.TargetRowset.Id}'.");
        }

        var targetRowsetLink = new ValidationTargetRowsetLink
        {
            Id = $"{validation.Id}:target:{targetRowsetLinks.Count + 1}",
            Validation = validation,
            TransformBindingTarget = target,
            Rowset = targetRowset,
            MetaSchemaTableId = targetTable.TableId
        };
        targetRowsetLinks.Add(targetRowsetLink);
        return targetRowsetLink;
    }

    private static void AddWriteFacts(
        RuntimeWriteEffect writeEffect,
        TargetWriteValidation writeValidation,
        List<Write> writes,
        List<WriteValue> writeValues,
        List<WriteValueScalarExpression> writeValueScalarExpressions,
        List<InsertQueryWrite> insertQueryWrites,
        List<InsertValuesWrite> insertValuesWrites,
        List<UpdateWrite> updateWrites,
        List<MergeInsertWrite> mergeInsertWrites,
        List<MergeUpdateWrite> mergeUpdateWrites)
    {
        if (writeEffect.Values.Count != writeValidation.ColumnLinks.Count)
        {
            throw new TransformBindingValidationException(
                "WriteValueValidationMismatch",
                $"Write for target rowset '{writeEffect.TargetRowset.Id}' has {writeEffect.Values.Count} value(s), but validation produced {writeValidation.ColumnLinks.Count} target column link(s).");
        }

        var write = new Write
        {
            Id = $"{writeValidation.TargetRowsetLink.Id}:write",
            ValidationTargetRowsetLink = writeValidation.TargetRowsetLink
        };
        writes.Add(write);

        for (var ordinal = 0; ordinal < writeEffect.Values.Count; ordinal++)
        {
            var writeValue = new WriteValue
            {
                Id = $"{write.Id}:value:{ordinal + 1}",
                Write = write,
                ValidationTargetColumnLink = writeValidation.ColumnLinks[ordinal]
            };
            writeValues.Add(writeValue);

            var scalarExpressionId = writeEffect.Values[ordinal].MetaTransformScriptScalarExpressionId;
            if (!string.IsNullOrWhiteSpace(scalarExpressionId))
            {
                writeValueScalarExpressions.Add(new WriteValueScalarExpression
                {
                    Id = $"{writeValue.Id}:scalar-expression",
                    WriteValue = writeValue,
                    MetaTransformScriptScalarExpressionId = scalarExpressionId
                });
            }
        }

        switch (writeEffect)
        {
            case RuntimeInsertQueryWriteEffect insertQueryWriteEffect:
                insertQueryWrites.Add(new InsertQueryWrite
                {
                    Id = $"{write.Id}:insert-query",
                    Write = write,
                    MetaTransformScriptQueryExpressionId = insertQueryWriteEffect.MetaTransformScriptQueryExpressionId
                });
                return;

            case RuntimeInsertValuesWriteEffect insertValuesWriteEffect:
                insertValuesWrites.Add(new InsertValuesWrite
                {
                    Id = $"{write.Id}:insert-values",
                    Write = write,
                    MetaTransformScriptRowValueId = insertValuesWriteEffect.MetaTransformScriptRowValueId
                });
                return;

            case RuntimeUpdateWriteEffect updateWriteEffect:
                updateWrites.Add(new UpdateWrite
                {
                    Id = $"{write.Id}:update",
                    Write = write,
                    MetaTransformScriptSetClauseId = updateWriteEffect.MetaTransformScriptSetClauseId
                });
                return;

            case RuntimeMergeInsertWriteEffect mergeInsertWriteEffect:
                mergeInsertWrites.Add(new MergeInsertWrite
                {
                    Id = $"{write.Id}:merge-insert",
                    Write = write,
                    MetaTransformScriptMergeInsertActionId = mergeInsertWriteEffect.MetaTransformScriptMergeInsertActionId
                });
                return;

            case RuntimeMergeUpdateWriteEffect mergeUpdateWriteEffect:
                mergeUpdateWrites.Add(new MergeUpdateWrite
                {
                    Id = $"{write.Id}:merge-update",
                    Write = write,
                    MetaTransformScriptMergeUpdateActionId = mergeUpdateWriteEffect.MetaTransformScriptMergeUpdateActionId
                });
                return;

            default:
                throw new TransformBindingValidationException(
                    "WriteEffectUnsupported",
                    $"Write for target rowset '{writeEffect.TargetRowset.Id}' has unsupported effect '{writeEffect.GetType().Name}'.");
        }
    }

    private static void AddTargetColumnReferences(
        TransformBindingTarget target,
        ResolvedSchemaTable targetTable,
        IReadOnlyList<RuntimeMutationEffect> mutationEffects,
        IReadOnlyList<RuntimeColumnReference> runtimeColumnReferences,
        IReadOnlyDictionary<string, Rowset> rowsetsById,
        IReadOnlyDictionary<string, Column> columnsById,
        List<TargetColumnReference> targetColumnReferences)
    {
        if (mutationEffects.Count == 0)
        {
            return;
        }

        var targetRowsetIds = mutationEffects
            .Select(item => item.TargetRowset.Id)
            .ToHashSet(StringComparer.Ordinal);
        var targetFieldsByName = targetTable.Fields
            .ToDictionary(item => item.FieldName, StringComparer.OrdinalIgnoreCase);

        foreach (var runtimeReference in runtimeColumnReferences
                     .Where(item => targetRowsetIds.Contains(item.ResolvedTableSource.Rowset.Id))
                     .GroupBy(item => item.SyntaxColumnReferenceId, StringComparer.Ordinal)
                     .Select(group => group.First()))
        {
            if (!rowsetsById.TryGetValue(runtimeReference.ResolvedTableSource.Rowset.Id, out var rowset))
            {
                throw new TransformBindingValidationException(
                    "TargetReadRowsetMissing",
                    $"Target column reference '{runtimeReference.SyntaxColumnReferenceId}' resolves to missing rowset '{runtimeReference.ResolvedTableSource.Rowset.Id}'.");
            }

            if (!string.Equals(rowset.TransformBinding.Id, target.TransformBinding.Id, StringComparison.Ordinal))
            {
                throw new TransformBindingValidationException(
                    "TargetReadBindingMismatch",
                    $"Target column reference '{runtimeReference.SyntaxColumnReferenceId}' resolves outside transform binding '{target.TransformBinding.Id}'.");
            }

            if (!columnsById.TryGetValue(runtimeReference.ResolvedColumn.Id, out var column) ||
                !string.Equals(column.Rowset.Id, rowset.Id, StringComparison.Ordinal))
            {
                throw new TransformBindingValidationException(
                    "TargetReadColumnMissing",
                    $"Target column reference '{runtimeReference.SyntaxColumnReferenceId}' resolves to missing column '{runtimeReference.ResolvedColumn.Id}'.");
            }

            if (!targetFieldsByName.TryGetValue(column.Name, out var targetField))
            {
                throw new TransformBindingValidationException(
                    "TargetReadColumnNotInSchema",
                    $"Target column reference '{runtimeReference.SyntaxColumnReferenceId}' resolves to '{column.Name}', which is not a field on target table '{targetTable.CanonicalSqlIdentifier}'.");
            }

            targetColumnReferences.Add(new TargetColumnReference
            {
                Id = $"{target.Id}:column-reference:{runtimeReference.SyntaxColumnReferenceId}",
                TransformBindingTarget = target,
                Column = column,
                MetaSchemaFieldId = targetField.FieldId,
                MetaTransformScriptColumnReferenceId = runtimeReference.SyntaxColumnReferenceId
            });
        }
    }

    private static List<ResolvedSchemaField> ResolveIgnoredTargetFields(
        ResolvedSchemaTable targetTable,
        IReadOnlyDictionary<string, ResolvedSchemaField> targetFieldsByName,
        IReadOnlySet<string> ignoredTargetColumnNames,
        IReadOnlySet<string> ignoredTargetColumnNamesIfPresent)
    {
        var ignoredFields = new List<ResolvedSchemaField>();
        foreach (var ignoredColumnName in ignoredTargetColumnNames)
        {
            if (!targetFieldsByName.TryGetValue(ignoredColumnName, out var ignoredField))
            {
                throw new TransformBindingValidationException(
                    "TargetIgnoredColumnNotFound",
                    $"Ignored target column '{ignoredColumnName}' was not found as a non-identity field on target table '{targetTable.CanonicalSqlIdentifier}'.");
            }

            ignoredFields.Add(ignoredField);
        }

        foreach (var ignoredColumnName in ignoredTargetColumnNamesIfPresent)
        {
            if (targetFieldsByName.TryGetValue(ignoredColumnName, out var ignoredField) &&
                ignoredFields.All(item => !string.Equals(item.FieldId, ignoredField.FieldId, StringComparison.Ordinal)))
            {
                ignoredFields.Add(ignoredField);
            }
        }

        return ignoredFields;
    }

    private static ResolvedSchemaField ResolveTargetField(
        string targetFieldName,
        int ordinal,
        IReadOnlyList<ResolvedSchemaField> writeCandidateFields,
        IReadOnlyDictionary<string, ResolvedSchemaField> targetFieldsByName,
        string targetSqlIdentifier)
    {
        if (!string.IsNullOrWhiteSpace(targetFieldName))
        {
            if (targetFieldsByName.TryGetValue(targetFieldName, out var namedField))
            {
                return namedField;
            }

            throw new TransformBindingValidationException(
                "TargetOutputColumnNotInSchema",
                $"Write for target '{targetSqlIdentifier}' includes column '{targetFieldName}', but no writable non-identity target field with that name exists.");
        }

        if (ordinal < writeCandidateFields.Count)
        {
            return writeCandidateFields[ordinal];
        }

        throw new TransformBindingValidationException(
            "TargetRowsetColumnCountMismatch",
            $"Write for target '{targetSqlIdentifier}' exposes more values than the target has writable non-identity fields.");
    }

    private static void EnsureWritableTargetContract(string? targetSqlIdentifier, ResolvedSchemaTable targetTable)
    {
        var objectType = targetTable.ObjectType?.Trim();
        if (string.IsNullOrWhiteSpace(objectType) ||
            string.Equals(objectType, "Table", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        throw new TransformBindingValidationException(
            "TargetSchemaObjectNotWritable",
            $"Declared target identifier '{targetSqlIdentifier}' resolves to {objectType} '{targetTable.CanonicalSqlIdentifier}', but transform binding targets must be writable table contracts.");
    }

    private static void ThrowResolutionFailure(
        bool isSource,
        string? sqlIdentifier,
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
        string? sqlIdentifier)
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
            .SingleOrDefault(item => string.Equals(item.TransformBinding.Id, bindingId, StringComparison.Ordinal));

        if (finalLink is null)
        {
            return null;
        }

        return model.RowsetList.SingleOrDefault(item => string.Equals(item.Id, finalLink.Rowset.Id, StringComparison.Ordinal));
    }

    private static int ParseOrdinal(string? ordinal)
    {
        return int.TryParse(ordinal, out var value)
            ? value
            : int.MaxValue;
    }

    private static string EnsureMetaDataTypeKnown(
        IMetaDataTypeConversionService dataTypeConversionService,
        MetaDataTypeConversionModel dataTypeConversionWorkspace,
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
            var normalized = metaDataTypeId.Trim();
            _ = dataTypeConversionService.ResolveCompatibility(dataTypeConversionWorkspace, normalized, normalized);
            return normalized;
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

    private static bool IsMutationTargetRowset(Rowset rowset)
    {
        return string.Equals(rowset.DerivationKind, "Target", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsRequiredWriteColumn(ResolvedSchemaField field)
    {
        return field.IsNullable != true;
    }

    private static void AppendTargetColumnTypeAssessment(
        bool isExact,
        ValidationTargetColumnLink validationTargetColumnLink,
        string sourceMetaDataTypeId,
        string targetMetaDataTypeId,
        List<ValidationTargetColumnTypeExact> targetColumnTypeExactRows,
        List<ValidationTargetColumnTypeSanctionedConversion> targetColumnTypeSanctionedConversionRows)
    {
        if (isExact)
        {
            targetColumnTypeExactRows.Add(new ValidationTargetColumnTypeExact
            {
                Id = $"{validationTargetColumnLink.Id}:type-exact",
                ValidationTargetColumnLink = validationTargetColumnLink,
                SourceMetaDataTypeId = sourceMetaDataTypeId,
                TargetMetaDataTypeId = targetMetaDataTypeId
            });
            return;
        }

        targetColumnTypeSanctionedConversionRows.Add(new ValidationTargetColumnTypeSanctionedConversion
        {
            Id = $"{validationTargetColumnLink.Id}:type-sanctioned-conversion",
            ValidationTargetColumnLink = validationTargetColumnLink,
            SourceMetaDataTypeId = sourceMetaDataTypeId,
            TargetMetaDataTypeId = targetMetaDataTypeId
        });
    }

    private static DataTypeCompatibilityResolution ValidateTargetColumnTypeConformance(
        IMetaDataTypeConversionService dataTypeConversionService,
        MetaDataTypeConversionModel dataTypeConversionWorkspace,
        string sourceMetaDataTypeId,
        bool? sourceIsNullable,
        int? sourceLength,
        int? sourcePrecision,
        int? sourceScale,
        string sourceDisplayName,
        string targetMetaDataTypeId,
        ResolvedSchemaField targetField,
        string outputColumnName,
        string targetSqlIdentifier,
        string targetTableSqlIdentifier)
    {
        DataTypeCompatibilityResolution compatibility;
        try
        {
            compatibility = dataTypeConversionService.ResolveCompatibility(
                dataTypeConversionWorkspace,
                sourceMetaDataTypeId,
                targetMetaDataTypeId);
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            throw new TransformBindingValidationException(
                "TargetColumnTypeConformanceMismatch",
                $"Write column '{outputColumnName}' for target '{targetSqlIdentifier}' resolves from '{sourceDisplayName}' with MetaDataTypeId '{sourceMetaDataTypeId}', but target field '{targetTableSqlIdentifier}.{targetField.FieldName}' declares MetaDataTypeId '{targetMetaDataTypeId}' without a sanctioned conversion path. {ex.Message}");
        }

        if (sourceIsNullable == true && targetField.IsNullable == false)
        {
            throw new TransformBindingValidationException(
                "TargetColumnNullabilityConformanceMismatch",
                $"Write column '{outputColumnName}' for target '{targetSqlIdentifier}' resolves from '{sourceDisplayName}' as nullable, but target field '{targetTableSqlIdentifier}.{targetField.FieldName}' is non-nullable.");
        }

        ValidateTypeDetailConformance(
            "Length",
            "TargetColumnLengthConformanceMismatch",
            sourceLength,
            targetField.Length,
            outputColumnName,
            targetSqlIdentifier,
            sourceDisplayName,
            $"{targetTableSqlIdentifier}.{targetField.FieldName}");
        ValidateTypeDetailConformance(
            "Precision",
            "TargetColumnPrecisionConformanceMismatch",
            sourcePrecision,
            targetField.Precision,
            outputColumnName,
            targetSqlIdentifier,
            sourceDisplayName,
            $"{targetTableSqlIdentifier}.{targetField.FieldName}");
        ValidateTypeDetailConformance(
            "Scale",
            "TargetColumnScaleConformanceMismatch",
            sourceScale,
            targetField.Scale,
            outputColumnName,
            targetSqlIdentifier,
            sourceDisplayName,
            $"{targetTableSqlIdentifier}.{targetField.FieldName}");
        return compatibility;
    }

    private sealed record ResolvedSourceColumnType(
        string ColumnName,
        string SourceMetaDataTypeId,
        bool? IsNullable,
        int? Length,
        int? Precision,
        int? Scale,
        string SourceDisplayName);

    private sealed record TargetWriteValidation(
        ValidationTargetRowsetLink TargetRowsetLink,
        IReadOnlyList<ValidationTargetColumnLink> ColumnLinks);
}
