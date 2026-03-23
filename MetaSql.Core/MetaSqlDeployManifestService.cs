using Meta.Adapters;
using Meta.Core.Domain;
using MetaSqlDeployManifest;

namespace MetaSql;

public sealed record MetaSqlDeployManifestBuildResult
{
    public required MetaSqlDeployManifestModel ManifestModel { get; init; }
    public required int AddCount { get; init; }
    public required int DropCount { get; init; }
    public required int IgnoredLiveOnlyDataDropCount { get; init; }
    public required int AlterCount { get; init; }
    public required int ReplaceCount { get; init; }
    public required int BlockCount { get; init; }
    public bool IsDeployable => BlockCount == 0;
}

public sealed class MetaSqlDeployManifestService
{
    private static readonly HashSet<string> ExecutableColumnAspects = new(StringComparer.Ordinal)
    {
        "MetaDataTypeId",
        "MetaDataTypeDetail",
        "IsNullable",
    };

    private static readonly HashSet<string> SupportedSqlServerColumnChangePrefixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "sqlserver:type:",
    };

    private static readonly HashSet<string> LengthBasedSqlServerTypeNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "varchar",
        "char",
        "nvarchar",
        "nchar",
        "varbinary",
        "binary",
    };

    public MetaSqlDeployManifestBuildResult BuildManifest(
        Workspace sourceWorkspace,
        Workspace liveWorkspace,
        IReadOnlyList<MetaSqlDifference> differences,
        string manifestName,
        string? targetDescription,
        bool withDataDrop = true,
        IReadOnlyList<MetaSqlDifferenceBlocker>? feasibilityBlockers = null)
    {
        ArgumentNullException.ThrowIfNull(sourceWorkspace);
        ArgumentNullException.ThrowIfNull(liveWorkspace);
        ArgumentNullException.ThrowIfNull(differences);
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestName);

        MetaSqlDiffService.EnsureMetaSqlWorkspace(sourceWorkspace, nameof(sourceWorkspace));
        MetaSqlDiffService.EnsureMetaSqlWorkspace(liveWorkspace, nameof(liveWorkspace));

        var model = MetaSqlDeployManifestModel.CreateEmpty();
        var root = new DeployManifest
        {
            Id = "DeployManifest:1",
            Name = manifestName,
            SourceInstanceFingerprint = MetaSqlInstanceFingerprint.Compute(sourceWorkspace),
            LiveInstanceFingerprint = MetaSqlInstanceFingerprint.Compute(liveWorkspace),
            CreatedUtc = DateTime.UtcNow.ToString("O"),
            TargetDescription = targetDescription ?? string.Empty,
        };
        model.DeployManifestList.Add(root);

        var sourceColumnsById = GetRecordIndex(sourceWorkspace, "TableColumn");
        var liveColumnsById = GetRecordIndex(liveWorkspace, "TableColumn");
        var sourceTablesById = GetRecordIndex(sourceWorkspace, "Table");
        var liveTablesById = GetRecordIndex(liveWorkspace, "Table");
        var sourcePrimaryKeysById = GetRecordIndex(sourceWorkspace, "PrimaryKey");
        var livePrimaryKeysById = GetRecordIndex(liveWorkspace, "PrimaryKey");
        var sourcePrimaryKeyColumnsByPrimaryKeyId = GetGroupedRecords(sourceWorkspace, "PrimaryKeyColumn", "PrimaryKeyId");
        var livePrimaryKeyColumnsByPrimaryKeyId = GetGroupedRecords(liveWorkspace, "PrimaryKeyColumn", "PrimaryKeyId");
        var sourceIndexesById = GetRecordIndex(sourceWorkspace, "Index");
        var liveIndexesById = GetRecordIndex(liveWorkspace, "Index");
        var sourceForeignKeysById = GetRecordIndex(sourceWorkspace, "ForeignKey");
        var liveForeignKeysById = GetRecordIndex(liveWorkspace, "ForeignKey");
        var sourceForeignKeysByTargetTableId = GetGroupedRecords(sourceWorkspace, "ForeignKey", "TargetTableId");
        var liveForeignKeysByTargetTableId = GetGroupedRecords(liveWorkspace, "ForeignKey", "TargetTableId");
        var sourceForeignKeyColumnsByForeignKeyId = GetGroupedRecords(sourceWorkspace, "ForeignKeyColumn", "ForeignKeyId");
        var liveForeignKeyColumnsByForeignKeyId = GetGroupedRecords(liveWorkspace, "ForeignKeyColumn", "ForeignKeyId");
        var sourceIndexColumnsByIndexId = GetGroupedRecords(sourceWorkspace, "IndexColumn", "IndexId");
        var liveIndexColumnsByIndexId = GetGroupedRecords(liveWorkspace, "IndexColumn", "IndexId");
        var sourceColumnDetailsByColumnId = GetGroupedRecords(sourceWorkspace, "TableColumnDataTypeDetail", "TableColumnId");
        var liveColumnDetailsByColumnId = GetGroupedRecords(liveWorkspace, "TableColumnDataTypeDetail", "TableColumnId");
        var sourcePrimaryKeyColumnsByColumnId = GetGroupedRecords(sourceWorkspace, "PrimaryKeyColumn", "TableColumnId");
        var livePrimaryKeyColumnsByColumnId = GetGroupedRecords(liveWorkspace, "PrimaryKeyColumn", "TableColumnId");
        var sourceForeignKeySourceColumnsByColumnId = GetGroupedRecords(sourceWorkspace, "ForeignKeyColumn", "SourceColumnId");
        var liveForeignKeySourceColumnsByColumnId = GetGroupedRecords(liveWorkspace, "ForeignKeyColumn", "SourceColumnId");
        var sourceForeignKeyTargetColumnsByColumnId = GetGroupedRecords(sourceWorkspace, "ForeignKeyColumn", "TargetColumnId");
        var liveForeignKeyTargetColumnsByColumnId = GetGroupedRecords(liveWorkspace, "ForeignKeyColumn", "TargetColumnId");
        var sourceIndexColumnsByColumnId = GetGroupedRecords(sourceWorkspace, "IndexColumn", "TableColumnId");
        var liveIndexColumnsByColumnId = GetGroupedRecords(liveWorkspace, "IndexColumn", "TableColumnId");
        var blockerByColumnPairKey = BuildColumnBlockerLookup(feasibilityBlockers);
        var plannedAddedTableIds = differences
            .Where(row => row.ObjectKind == MetaSqlObjectKind.Table && row.DifferenceKind == MetaSqlDifferenceKind.MissingInLive)
            .Select(row => row.SourceId)
            .Where(row => !string.IsNullOrWhiteSpace(row))
            .Select(row => row!)
            .ToHashSet(StringComparer.Ordinal);
        var plannedAddedColumnIds = differences
            .Where(row => row.ObjectKind == MetaSqlObjectKind.TableColumn && row.DifferenceKind == MetaSqlDifferenceKind.MissingInLive)
            .Select(row => row.SourceId)
            .Where(row => !string.IsNullOrWhiteSpace(row))
            .Select(row => row!)
            .ToHashSet(StringComparer.Ordinal);
        var plannedDroppedForeignKeyIds = differences
            .Where(row => row.ObjectKind == MetaSqlObjectKind.ForeignKey && row.DifferenceKind == MetaSqlDifferenceKind.ExtraInLive)
            .Select(row => row.LiveId)
            .Where(row => !string.IsNullOrWhiteSpace(row))
            .Select(row => row!)
            .ToHashSet(StringComparer.Ordinal);
        var plannedAddedForeignKeyIds = differences
            .Where(row => row.ObjectKind == MetaSqlObjectKind.ForeignKey && row.DifferenceKind == MetaSqlDifferenceKind.MissingInLive)
            .Select(row => row.SourceId)
            .Where(row => !string.IsNullOrWhiteSpace(row))
            .Select(row => row!)
            .ToHashSet(StringComparer.Ordinal);

        var addCount = 0;
        var dropCount = 0;
        var ignoredLiveOnlyDataDropCount = 0;
        var alterCount = 0;
        var replaceCount = 0;
        var blockCount = 0;

        foreach (var difference in differences
                     .OrderBy(row => row.ObjectKind)
                     .ThenBy(row => row.DifferenceKind)
                     .ThenBy(row => row.ScopeDisplayName, StringComparer.Ordinal)
                     .ThenBy(row => row.DisplayName, StringComparer.Ordinal))
        {
            switch (difference.DifferenceKind)
            {
                case MetaSqlDifferenceKind.MissingInLive:
                    addCount += AddEntry(model, root, difference);
                    break;
                case MetaSqlDifferenceKind.ExtraInLive:
                    if (!withDataDrop && IsLiveOnlyDataDropDifference(difference))
                    {
                        ignoredLiveOnlyDataDropCount++;
                        break;
                    }

                    dropCount += DropEntry(model, root, difference);
                    break;
                case MetaSqlDifferenceKind.Different:
                {
                    if (difference.ObjectKind == MetaSqlObjectKind.PrimaryKey)
                    {
                        var assessment = AssessPrimaryKeyDifference(
                            difference,
                            sourcePrimaryKeysById,
                            livePrimaryKeysById,
                            sourcePrimaryKeyColumnsByPrimaryKeyId,
                            livePrimaryKeyColumnsByPrimaryKeyId,
                            sourceForeignKeysById,
                            liveForeignKeysById,
                            sourceForeignKeysByTargetTableId,
                            liveForeignKeysByTargetTableId,
                            sourceForeignKeyColumnsByForeignKeyId,
                            liveForeignKeyColumnsByForeignKeyId,
                            sourceTablesById,
                            liveTablesById,
                            sourceColumnsById,
                            liveColumnsById,
                            plannedAddedTableIds,
                            plannedAddedColumnIds,
                            plannedDroppedForeignKeyIds,
                            plannedAddedForeignKeyIds);

                        if (assessment.Executable)
                        {
                            replaceCount += ReplaceEntry(model, root, difference);
                        }
                        else
                        {
                            blockCount += BlockEntry(model, root, difference, assessment.Reason);
                        }

                        break;
                    }

                    if (difference.ObjectKind == MetaSqlObjectKind.ForeignKey)
                    {
                        var assessment = AssessForeignKeyDifference(
                            difference,
                            sourceForeignKeysById,
                            liveForeignKeysById,
                            sourceForeignKeyColumnsByForeignKeyId,
                            liveForeignKeyColumnsByForeignKeyId,
                            sourceTablesById,
                            liveTablesById,
                            sourceColumnsById,
                            liveColumnsById,
                            plannedAddedTableIds,
                            plannedAddedColumnIds);

                        if (assessment.Executable)
                        {
                            replaceCount += ReplaceEntry(model, root, difference);
                        }
                        else
                        {
                            blockCount += BlockEntry(model, root, difference, assessment.Reason);
                        }

                        break;
                    }

                    if (difference.ObjectKind == MetaSqlObjectKind.Index)
                    {
                        var assessment = AssessIndexDifference(
                            difference,
                            sourceIndexesById,
                            liveIndexesById,
                            sourceIndexColumnsByIndexId,
                            liveIndexColumnsByIndexId,
                            sourceTablesById,
                            liveTablesById,
                            sourceColumnsById,
                            liveColumnsById,
                            plannedAddedTableIds,
                            plannedAddedColumnIds);

                        if (assessment.Executable)
                        {
                            replaceCount += ReplaceEntry(model, root, difference);
                        }
                        else
                        {
                            blockCount += BlockEntry(model, root, difference, assessment.Reason);
                        }

                        break;
                    }

                    if (difference.ObjectKind != MetaSqlObjectKind.TableColumn)
                    {
                        blockCount += BlockEntry(model, root, difference);
                        break;
                    }

                    var columnAssessment = AssessTableColumnDifference(
                        difference,
                        sourceColumnsById,
                        liveColumnsById,
                        sourceColumnDetailsByColumnId,
                        liveColumnDetailsByColumnId,
                        blockerByColumnPairKey);

                    if (columnAssessment.Executable)
                    {
                        alterCount += AlterEntry(model, root, difference);
                    }
                    else
                    {
                        blockCount += BlockEntry(model, root, difference, columnAssessment.Reason);
                    }

                    break;
                }
                default:
                    throw new InvalidOperationException($"Unsupported difference kind '{difference.DifferenceKind}'.");
            }
        }

        addCount += AddExplicitDependentEntriesForAddedTables(model, root, sourceWorkspace);
        var dependentConstraintActionExpansion = AddExplicitDependentConstraintEntriesForAlteredColumns(
            model,
            root,
            sourceColumnsById,
            liveColumnsById,
            sourcePrimaryKeysById,
            livePrimaryKeysById,
            sourcePrimaryKeyColumnsByPrimaryKeyId,
            livePrimaryKeyColumnsByPrimaryKeyId,
            sourcePrimaryKeyColumnsByColumnId,
            livePrimaryKeyColumnsByColumnId,
            sourceForeignKeysById,
            liveForeignKeysById,
            sourceForeignKeyColumnsByForeignKeyId,
            liveForeignKeyColumnsByForeignKeyId,
            sourceForeignKeySourceColumnsByColumnId,
            liveForeignKeySourceColumnsByColumnId,
            sourceForeignKeyTargetColumnsByColumnId,
            liveForeignKeyTargetColumnsByColumnId,
            sourceTablesById,
            liveTablesById,
            plannedAddedTableIds,
            plannedAddedColumnIds,
            plannedDroppedForeignKeyIds,
            plannedAddedForeignKeyIds);
        replaceCount += dependentConstraintActionExpansion.AddedReplaceCount;
        blockCount += dependentConstraintActionExpansion.AddedBlockCount;
        var dependentForeignKeyActionExpansion = AddExplicitDependentForeignKeyEntriesForPrimaryKeyReplacements(
            model,
            root,
            sourcePrimaryKeysById,
            livePrimaryKeysById,
            sourcePrimaryKeyColumnsByPrimaryKeyId,
            livePrimaryKeyColumnsByPrimaryKeyId,
            sourceForeignKeysById,
            liveForeignKeysById,
            sourceForeignKeyColumnsByForeignKeyId,
            liveForeignKeyColumnsByForeignKeyId);
        replaceCount += dependentForeignKeyActionExpansion.AddedReplaceForeignKeyCount;
        blockCount += dependentForeignKeyActionExpansion.AddedBlockCount;
        var dependentIndexActionExpansion = AddExplicitDependentIndexEntriesForAlteredColumns(
            model,
            root,
            sourceColumnsById,
            liveColumnsById,
            sourceIndexesById,
            liveIndexesById,
            sourceIndexColumnsByIndexId,
            liveIndexColumnsByIndexId,
            sourceIndexColumnsByColumnId,
            liveIndexColumnsByColumnId,
            sourceTablesById,
            liveTablesById,
            plannedAddedTableIds,
            plannedAddedColumnIds);
        replaceCount += dependentIndexActionExpansion.AddedReplaceIndexCount;
        blockCount += dependentIndexActionExpansion.AddedBlockCount;

        return new MetaSqlDeployManifestBuildResult
        {
            ManifestModel = model,
            AddCount = addCount,
            DropCount = dropCount,
            IgnoredLiveOnlyDataDropCount = ignoredLiveOnlyDataDropCount,
            AlterCount = alterCount,
            ReplaceCount = replaceCount,
            BlockCount = blockCount,
        };
    }

    private static bool IsLiveOnlyDataDropDifference(MetaSqlDifference difference)
    {
        return difference.DifferenceKind == MetaSqlDifferenceKind.ExtraInLive &&
               (difference.ObjectKind == MetaSqlObjectKind.Table ||
                difference.ObjectKind == MetaSqlObjectKind.TableColumn);
    }

    private static int AddEntry(MetaSqlDeployManifestModel model, DeployManifest root, MetaSqlDifference difference)
    {
        switch (difference.ObjectKind)
        {
            case MetaSqlObjectKind.Table:
                model.AddTableList.Add(new AddTable
                {
                    Id = $"AddTable:{RequireValue(difference.SourceId, "SourceId", difference)}",
                    SourceTableId = RequireValue(difference.SourceId, "SourceId", difference),
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.TableColumn:
                model.AddTableColumnList.Add(new AddTableColumn
                {
                    Id = $"AddTableColumn:{RequireValue(difference.SourceId, "SourceId", difference)}",
                    SourceTableColumnId = RequireValue(difference.SourceId, "SourceId", difference),
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.PrimaryKey:
                model.AddPrimaryKeyList.Add(new AddPrimaryKey
                {
                    Id = $"AddPrimaryKey:{RequireValue(difference.SourceId, "SourceId", difference)}",
                    SourcePrimaryKeyId = RequireValue(difference.SourceId, "SourceId", difference),
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.ForeignKey:
                model.AddForeignKeyList.Add(new AddForeignKey
                {
                    Id = $"AddForeignKey:{RequireValue(difference.SourceId, "SourceId", difference)}",
                    SourceForeignKeyId = RequireValue(difference.SourceId, "SourceId", difference),
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.Index:
                model.AddIndexList.Add(new AddIndex
                {
                    Id = $"AddIndex:{RequireValue(difference.SourceId, "SourceId", difference)}",
                    SourceIndexId = RequireValue(difference.SourceId, "SourceId", difference),
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            default:
                throw new InvalidOperationException($"Unsupported add object kind '{difference.ObjectKind}'.");
        }
    }

    private static int DropEntry(MetaSqlDeployManifestModel model, DeployManifest root, MetaSqlDifference difference)
    {
        switch (difference.ObjectKind)
        {
            case MetaSqlObjectKind.Table:
                model.DropTableList.Add(new DropTable
                {
                    Id = $"DropTable:{RequireValue(difference.LiveId, "LiveId", difference)}",
                    LiveTableId = RequireValue(difference.LiveId, "LiveId", difference),
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.TableColumn:
                model.DropTableColumnList.Add(new DropTableColumn
                {
                    Id = $"DropTableColumn:{RequireValue(difference.LiveId, "LiveId", difference)}",
                    LiveTableColumnId = RequireValue(difference.LiveId, "LiveId", difference),
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.PrimaryKey:
                model.DropPrimaryKeyList.Add(new DropPrimaryKey
                {
                    Id = $"DropPrimaryKey:{RequireValue(difference.LiveId, "LiveId", difference)}",
                    LivePrimaryKeyId = RequireValue(difference.LiveId, "LiveId", difference),
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.ForeignKey:
                model.DropForeignKeyList.Add(new DropForeignKey
                {
                    Id = $"DropForeignKey:{RequireValue(difference.LiveId, "LiveId", difference)}",
                    LiveForeignKeyId = RequireValue(difference.LiveId, "LiveId", difference),
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.Index:
                model.DropIndexList.Add(new DropIndex
                {
                    Id = $"DropIndex:{RequireValue(difference.LiveId, "LiveId", difference)}",
                    LiveIndexId = RequireValue(difference.LiveId, "LiveId", difference),
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            default:
                throw new InvalidOperationException($"Unsupported drop object kind '{difference.ObjectKind}'.");
        }
    }

    private static int AlterEntry(MetaSqlDeployManifestModel model, DeployManifest root, MetaSqlDifference difference)
    {
        if (difference.ObjectKind != MetaSqlObjectKind.TableColumn)
        {
            throw new InvalidOperationException(
                $"Unsupported alter object kind '{difference.ObjectKind}'.");
        }

        var sourceId = RequireValue(difference.SourceId, "SourceId", difference);
        var liveId = RequireValue(difference.LiveId, "LiveId", difference);
        model.AlterTableColumnList.Add(new AlterTableColumn
        {
            Id = $"AlterTableColumn:{sourceId}:{liveId}",
            SourceTableColumnId = sourceId,
            LiveTableColumnId = liveId,
            DeployManifestId = root.Id,
            DeployManifest = root,
        });
        return 1;
    }

    private static int ReplaceEntry(MetaSqlDeployManifestModel model, DeployManifest root, MetaSqlDifference difference)
    {
        var sourceId = RequireValue(difference.SourceId, "SourceId", difference);
        var liveId = RequireValue(difference.LiveId, "LiveId", difference);
        switch (difference.ObjectKind)
        {
            case MetaSqlObjectKind.PrimaryKey:
                model.ReplacePrimaryKeyList.Add(new ReplacePrimaryKey
                {
                    Id = $"ReplacePrimaryKey:{sourceId}:{liveId}",
                    SourcePrimaryKeyId = sourceId,
                    LivePrimaryKeyId = liveId,
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.ForeignKey:
                model.ReplaceForeignKeyList.Add(new ReplaceForeignKey
                {
                    Id = $"ReplaceForeignKey:{sourceId}:{liveId}",
                    SourceForeignKeyId = sourceId,
                    LiveForeignKeyId = liveId,
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.Index:
                model.ReplaceIndexList.Add(new ReplaceIndex
                {
                    Id = $"ReplaceIndex:{sourceId}:{liveId}",
                    SourceIndexId = sourceId,
                    LiveIndexId = liveId,
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            default:
                throw new InvalidOperationException(
                    $"Unsupported replace object kind '{difference.ObjectKind}'.");
        }
    }

    private static int AddExplicitDependentEntriesForAddedTables(
        MetaSqlDeployManifestModel model,
        DeployManifest root,
        Workspace sourceWorkspace)
    {
        var addedTableIds = model.AddTableList
            .Select(row => row.SourceTableId)
            .ToHashSet(StringComparer.Ordinal);
        if (addedTableIds.Count == 0)
        {
            return 0;
        }

        var additionalAddCount = 0;
        var primaryKeyIds = model.AddPrimaryKeyList
            .Select(row => row.SourcePrimaryKeyId)
            .ToHashSet(StringComparer.Ordinal);
        foreach (var sourcePrimaryKey in sourceWorkspace.Instance.GetOrCreateEntityRecords("PrimaryKey")
                     .Where(row => addedTableIds.Contains(row.RelationshipIds["TableId"]))
                     .OrderBy(row => row.Id, StringComparer.Ordinal))
        {
            if (!primaryKeyIds.Add(sourcePrimaryKey.Id))
            {
                continue;
            }

            model.AddPrimaryKeyList.Add(new AddPrimaryKey
            {
                Id = $"AddPrimaryKey:{sourcePrimaryKey.Id}",
                SourcePrimaryKeyId = sourcePrimaryKey.Id,
                DeployManifestId = root.Id,
                DeployManifest = root,
            });
            additionalAddCount++;
        }

        var foreignKeyIds = model.AddForeignKeyList
            .Select(row => row.SourceForeignKeyId)
            .ToHashSet(StringComparer.Ordinal);
        foreach (var sourceForeignKey in sourceWorkspace.Instance.GetOrCreateEntityRecords("ForeignKey")
                     .Where(row => addedTableIds.Contains(row.RelationshipIds["SourceTableId"]))
                     .OrderBy(row => row.Id, StringComparer.Ordinal))
        {
            if (!foreignKeyIds.Add(sourceForeignKey.Id))
            {
                continue;
            }

            model.AddForeignKeyList.Add(new AddForeignKey
            {
                Id = $"AddForeignKey:{sourceForeignKey.Id}",
                SourceForeignKeyId = sourceForeignKey.Id,
                DeployManifestId = root.Id,
                DeployManifest = root,
            });
            additionalAddCount++;
        }

        var indexIds = model.AddIndexList
            .Select(row => row.SourceIndexId)
            .ToHashSet(StringComparer.Ordinal);
        foreach (var sourceIndex in sourceWorkspace.Instance.GetOrCreateEntityRecords("Index")
                     .Where(row => addedTableIds.Contains(row.RelationshipIds["TableId"]))
                     .OrderBy(row => row.Id, StringComparer.Ordinal))
        {
            if (!indexIds.Add(sourceIndex.Id))
            {
                continue;
            }

            model.AddIndexList.Add(new AddIndex
            {
                Id = $"AddIndex:{sourceIndex.Id}",
                SourceIndexId = sourceIndex.Id,
                DeployManifestId = root.Id,
                DeployManifest = root,
            });
            additionalAddCount++;
        }

        return additionalAddCount;
    }

    private static (int AddedReplaceCount, int AddedBlockCount) AddExplicitDependentConstraintEntriesForAlteredColumns(
        MetaSqlDeployManifestModel model,
        DeployManifest root,
        IReadOnlyDictionary<string, GenericRecord> sourceColumnsById,
        IReadOnlyDictionary<string, GenericRecord> liveColumnsById,
        IReadOnlyDictionary<string, GenericRecord> sourcePrimaryKeysById,
        IReadOnlyDictionary<string, GenericRecord> livePrimaryKeysById,
        IReadOnlyDictionary<string, List<GenericRecord>> sourcePrimaryKeyColumnsByPrimaryKeyId,
        IReadOnlyDictionary<string, List<GenericRecord>> livePrimaryKeyColumnsByPrimaryKeyId,
        IReadOnlyDictionary<string, List<GenericRecord>> sourcePrimaryKeyColumnsByColumnId,
        IReadOnlyDictionary<string, List<GenericRecord>> livePrimaryKeyColumnsByColumnId,
        IReadOnlyDictionary<string, GenericRecord> sourceForeignKeysById,
        IReadOnlyDictionary<string, GenericRecord> liveForeignKeysById,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceForeignKeyColumnsByForeignKeyId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveForeignKeyColumnsByForeignKeyId,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceForeignKeySourceColumnsByColumnId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveForeignKeySourceColumnsByColumnId,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceForeignKeyTargetColumnsByColumnId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveForeignKeyTargetColumnsByColumnId,
        IReadOnlyDictionary<string, GenericRecord> sourceTablesById,
        IReadOnlyDictionary<string, GenericRecord> liveTablesById,
        IReadOnlySet<string> plannedAddedTableIds,
        IReadOnlySet<string> plannedAddedColumnIds,
        IReadOnlySet<string> plannedDroppedForeignKeyIds,
        IReadOnlySet<string> plannedAddedForeignKeyIds)
    {
        var replacePrimaryKeyByPair = model.ReplacePrimaryKeyList
            .ToDictionary(
                row => BuildReplacePrimaryKeyPairKey(row.SourcePrimaryKeyId, row.LivePrimaryKeyId),
                row => row,
                StringComparer.Ordinal);
        var replaceForeignKeyByPair = model.ReplaceForeignKeyList
            .ToDictionary(
                row => BuildReplaceForeignKeyPairKey(row.SourceForeignKeyId, row.LiveForeignKeyId),
                row => row,
                StringComparer.Ordinal);

        var addPrimaryKeyIds = model.AddPrimaryKeyList
            .Select(row => row.SourcePrimaryKeyId)
            .ToHashSet(StringComparer.Ordinal);
        var dropPrimaryKeyIds = model.DropPrimaryKeyList
            .Select(row => row.LivePrimaryKeyId)
            .ToHashSet(StringComparer.Ordinal);
        var replacePrimaryKeySourceIds = model.ReplacePrimaryKeyList
            .Select(row => row.SourcePrimaryKeyId)
            .ToHashSet(StringComparer.Ordinal);
        var replacePrimaryKeyLiveIds = model.ReplacePrimaryKeyList
            .Select(row => row.LivePrimaryKeyId)
            .ToHashSet(StringComparer.Ordinal);

        var addForeignKeyIds = model.AddForeignKeyList
            .Select(row => row.SourceForeignKeyId)
            .ToHashSet(StringComparer.Ordinal);
        var dropForeignKeyIds = model.DropForeignKeyList
            .Select(row => row.LiveForeignKeyId)
            .ToHashSet(StringComparer.Ordinal);
        var replaceForeignKeySourceIds = model.ReplaceForeignKeyList
            .Select(row => row.SourceForeignKeyId)
            .ToHashSet(StringComparer.Ordinal);
        var replaceForeignKeyLiveIds = model.ReplaceForeignKeyList
            .Select(row => row.LiveForeignKeyId)
            .ToHashSet(StringComparer.Ordinal);
        var sourceForeignKeysByTargetTableId = GetGroupedRecordsByTargetTableId(sourceForeignKeysById.Values);
        var liveForeignKeysByTargetTableId = GetGroupedRecordsByTargetTableId(liveForeignKeysById.Values);

        var addedReplaceCount = 0;
        var addedBlockCount = 0;

        foreach (var alterTableColumn in model.AlterTableColumnList
                     .OrderBy(row => row.SourceTableColumnId, StringComparer.Ordinal)
                     .ThenBy(row => row.LiveTableColumnId, StringComparer.Ordinal))
        {
            if (!sourceColumnsById.TryGetValue(alterTableColumn.SourceTableColumnId, out var sourceColumn) ||
                !liveColumnsById.TryGetValue(alterTableColumn.LiveTableColumnId, out var liveColumn))
            {
                addedBlockCount += AddColumnDependencyBlock(
                    model,
                    root,
                    alterTableColumn,
                    "AlterTableColumn references missing source/live table-column row.");
                continue;
            }

            var sourceDependentPrimaryKeyIds = sourcePrimaryKeyColumnsByColumnId.TryGetValue(sourceColumn.Id, out var sourcePrimaryKeyMembers)
                ? sourcePrimaryKeyMembers.Select(row => row.RelationshipIds["PrimaryKeyId"]).Distinct(StringComparer.Ordinal).ToList()
                : [];
            var liveDependentPrimaryKeyIds = livePrimaryKeyColumnsByColumnId.TryGetValue(liveColumn.Id, out var livePrimaryKeyMembers)
                ? livePrimaryKeyMembers.Select(row => row.RelationshipIds["PrimaryKeyId"]).Distinct(StringComparer.Ordinal).ToList()
                : [];

            if (sourceDependentPrimaryKeyIds.Count > 0 || liveDependentPrimaryKeyIds.Count > 0)
            {
                var sourcePrimaryKeysByMatchKey = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
                var livePrimaryKeysByMatchKey = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
                var ambiguousPrimaryKeyIdentity = false;

                foreach (var sourcePrimaryKeyId in sourceDependentPrimaryKeyIds.OrderBy(row => row, StringComparer.Ordinal))
                {
                    if (!sourcePrimaryKeysById.TryGetValue(sourcePrimaryKeyId, out var sourcePrimaryKey))
                    {
                        addedBlockCount += AddColumnDependencyBlock(
                            model,
                            root,
                            alterTableColumn,
                            $"AlterTableColumn references missing dependent source primary key '{sourcePrimaryKeyId}'.");
                        ambiguousPrimaryKeyIdentity = true;
                        break;
                    }

                    var matchKey = BuildPrimaryKeyMatchKey(sourcePrimaryKey);
                    if (!sourcePrimaryKeysByMatchKey.TryAdd(matchKey, sourcePrimaryKey))
                    {
                        addedBlockCount += AddColumnDependencyBlock(
                            model,
                            root,
                            alterTableColumn,
                            $"AlterTableColumn has ambiguous dependent source primary key identity by (TableId, Name): '{matchKey}'.");
                        ambiguousPrimaryKeyIdentity = true;
                        break;
                    }
                }

                if (!ambiguousPrimaryKeyIdentity)
                {
                    foreach (var livePrimaryKeyId in liveDependentPrimaryKeyIds.OrderBy(row => row, StringComparer.Ordinal))
                    {
                        if (!livePrimaryKeysById.TryGetValue(livePrimaryKeyId, out var livePrimaryKey))
                        {
                            addedBlockCount += AddColumnDependencyBlock(
                                model,
                                root,
                                alterTableColumn,
                                $"AlterTableColumn references missing dependent live primary key '{livePrimaryKeyId}'.");
                            ambiguousPrimaryKeyIdentity = true;
                            break;
                        }

                        var matchKey = BuildPrimaryKeyMatchKey(livePrimaryKey);
                        if (!livePrimaryKeysByMatchKey.TryAdd(matchKey, livePrimaryKey))
                        {
                            addedBlockCount += AddColumnDependencyBlock(
                                model,
                                root,
                                alterTableColumn,
                                $"AlterTableColumn has ambiguous dependent live primary key identity by (TableId, Name): '{matchKey}'.");
                            ambiguousPrimaryKeyIdentity = true;
                            break;
                        }
                    }
                }

                if (!ambiguousPrimaryKeyIdentity)
                {
                    foreach (var matchKey in sourcePrimaryKeysByMatchKey.Keys
                                 .Concat(livePrimaryKeysByMatchKey.Keys)
                                 .Distinct(StringComparer.Ordinal)
                                 .OrderBy(row => row, StringComparer.Ordinal))
                    {
                        sourcePrimaryKeysByMatchKey.TryGetValue(matchKey, out var sourcePrimaryKey);
                        livePrimaryKeysByMatchKey.TryGetValue(matchKey, out var livePrimaryKey);

                        var sourceCovered = sourcePrimaryKey is not null &&
                                            (addPrimaryKeyIds.Contains(sourcePrimaryKey.Id) ||
                                             replacePrimaryKeySourceIds.Contains(sourcePrimaryKey.Id));
                        var liveCovered = livePrimaryKey is not null &&
                                          (dropPrimaryKeyIds.Contains(livePrimaryKey.Id) ||
                                           replacePrimaryKeyLiveIds.Contains(livePrimaryKey.Id));

                        if (sourcePrimaryKey is not null && livePrimaryKey is not null)
                        {
                            if (sourceCovered && liveCovered)
                            {
                                continue;
                            }

                            if (!sourceCovered && !liveCovered)
                            {
                                var primaryKeyDifference = new MetaSqlDifference
                                {
                                    ObjectKind = MetaSqlObjectKind.PrimaryKey,
                                    DifferenceKind = MetaSqlDifferenceKind.Different,
                                    ScopeDisplayName = string.Empty,
                                    DisplayName = GetValue(sourcePrimaryKey, "Name"),
                                    SourceId = sourcePrimaryKey.Id,
                                    LiveId = livePrimaryKey.Id,
                                };
                                var primaryKeyAssessment = AssessPrimaryKeyDifference(
                                    primaryKeyDifference,
                                    sourcePrimaryKeysById,
                                    livePrimaryKeysById,
                                    sourcePrimaryKeyColumnsByPrimaryKeyId,
                                    livePrimaryKeyColumnsByPrimaryKeyId,
                                    sourceForeignKeysById,
                                    liveForeignKeysById,
                                    sourceForeignKeysByTargetTableId,
                                    liveForeignKeysByTargetTableId,
                                    sourceForeignKeyColumnsByForeignKeyId,
                                    liveForeignKeyColumnsByForeignKeyId,
                                    sourceTablesById,
                                    liveTablesById,
                                    sourceColumnsById,
                                    liveColumnsById,
                                    plannedAddedTableIds,
                                    plannedAddedColumnIds,
                                    plannedDroppedForeignKeyIds,
                                    plannedAddedForeignKeyIds);
                                if (!primaryKeyAssessment.Executable)
                                {
                                    addedBlockCount += AddColumnDependencyBlock(
                                        model,
                                        root,
                                        alterTableColumn,
                                        $"AlterTableColumn requires executable replacement for dependent primary key '{GetValue(sourcePrimaryKey, "Name")}': {primaryKeyAssessment.Reason}");
                                    break;
                                }

                                var pairKey = BuildReplacePrimaryKeyPairKey(sourcePrimaryKey.Id, livePrimaryKey.Id);
                                if (!replacePrimaryKeyByPair.ContainsKey(pairKey))
                                {
                                    var replacePrimaryKey = new ReplacePrimaryKey
                                    {
                                        Id = $"ReplacePrimaryKey:{sourcePrimaryKey.Id}:{livePrimaryKey.Id}",
                                        SourcePrimaryKeyId = sourcePrimaryKey.Id,
                                        LivePrimaryKeyId = livePrimaryKey.Id,
                                        DeployManifestId = root.Id,
                                        DeployManifest = root,
                                    };
                                    model.ReplacePrimaryKeyList.Add(replacePrimaryKey);
                                    replacePrimaryKeyByPair[pairKey] = replacePrimaryKey;
                                    replacePrimaryKeySourceIds.Add(sourcePrimaryKey.Id);
                                    replacePrimaryKeyLiveIds.Add(livePrimaryKey.Id);
                                    addedReplaceCount++;
                                }

                                continue;
                            }

                            addedBlockCount += AddColumnDependencyBlock(
                                model,
                                root,
                                alterTableColumn,
                                $"AlterTableColumn has partial dependent primary key coverage for '{matchKey}'. Dependent primary key choreography must be explicit.");
                            break;
                        }

                        if (sourcePrimaryKey is null && livePrimaryKey is not null && !liveCovered)
                        {
                            addedBlockCount += AddColumnDependencyBlock(
                                model,
                                root,
                                alterTableColumn,
                                $"AlterTableColumn requires explicit drop action for dependent live primary key '{livePrimaryKey.Id}'.");
                            break;
                        }

                        if (sourcePrimaryKey is not null && livePrimaryKey is null && !sourceCovered)
                        {
                            addedBlockCount += AddColumnDependencyBlock(
                                model,
                                root,
                                alterTableColumn,
                                $"AlterTableColumn requires explicit add action for dependent source primary key '{sourcePrimaryKey.Id}'.");
                            break;
                        }
                    }
                }
            }

            var sourceDependentForeignKeyIds = sourceForeignKeySourceColumnsByColumnId.TryGetValue(sourceColumn.Id, out var sourceForeignKeySourceMembers)
                ? sourceForeignKeySourceMembers.Select(row => row.RelationshipIds["ForeignKeyId"]).Distinct(StringComparer.Ordinal).ToList()
                : [];
            if (sourceForeignKeyTargetColumnsByColumnId.TryGetValue(sourceColumn.Id, out var sourceForeignKeyTargetMembers))
            {
                sourceDependentForeignKeyIds.AddRange(sourceForeignKeyTargetMembers
                    .Select(row => row.RelationshipIds["ForeignKeyId"])
                    .Distinct(StringComparer.Ordinal));
                sourceDependentForeignKeyIds = sourceDependentForeignKeyIds
                    .Distinct(StringComparer.Ordinal)
                    .ToList();
            }

            var liveDependentForeignKeyIds = liveForeignKeySourceColumnsByColumnId.TryGetValue(liveColumn.Id, out var liveForeignKeySourceMembers)
                ? liveForeignKeySourceMembers.Select(row => row.RelationshipIds["ForeignKeyId"]).Distinct(StringComparer.Ordinal).ToList()
                : [];
            if (liveForeignKeyTargetColumnsByColumnId.TryGetValue(liveColumn.Id, out var liveForeignKeyTargetMembers))
            {
                liveDependentForeignKeyIds.AddRange(liveForeignKeyTargetMembers
                    .Select(row => row.RelationshipIds["ForeignKeyId"])
                    .Distinct(StringComparer.Ordinal));
                liveDependentForeignKeyIds = liveDependentForeignKeyIds
                    .Distinct(StringComparer.Ordinal)
                    .ToList();
            }

            if (sourceDependentForeignKeyIds.Count == 0 && liveDependentForeignKeyIds.Count == 0)
            {
                continue;
            }

            var sourceForeignKeysByMatchKey = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
            var liveForeignKeysByMatchKey = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
            var ambiguousForeignKeyIdentity = false;

            foreach (var sourceForeignKeyId in sourceDependentForeignKeyIds.OrderBy(row => row, StringComparer.Ordinal))
            {
                if (!sourceForeignKeysById.TryGetValue(sourceForeignKeyId, out var sourceForeignKey))
                {
                    addedBlockCount += AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn references missing dependent source foreign key '{sourceForeignKeyId}'.");
                    ambiguousForeignKeyIdentity = true;
                    break;
                }

                var matchKey = BuildForeignKeyMatchKey(sourceForeignKey);
                if (!sourceForeignKeysByMatchKey.TryAdd(matchKey, sourceForeignKey))
                {
                    addedBlockCount += AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn has ambiguous dependent source foreign key identity by (SourceTableId, Name): '{matchKey}'.");
                    ambiguousForeignKeyIdentity = true;
                    break;
                }
            }

            if (ambiguousForeignKeyIdentity)
            {
                continue;
            }

            foreach (var liveForeignKeyId in liveDependentForeignKeyIds.OrderBy(row => row, StringComparer.Ordinal))
            {
                if (!liveForeignKeysById.TryGetValue(liveForeignKeyId, out var liveForeignKey))
                {
                    addedBlockCount += AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn references missing dependent live foreign key '{liveForeignKeyId}'.");
                    ambiguousForeignKeyIdentity = true;
                    break;
                }

                var matchKey = BuildForeignKeyMatchKey(liveForeignKey);
                if (!liveForeignKeysByMatchKey.TryAdd(matchKey, liveForeignKey))
                {
                    addedBlockCount += AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn has ambiguous dependent live foreign key identity by (SourceTableId, Name): '{matchKey}'.");
                    ambiguousForeignKeyIdentity = true;
                    break;
                }
            }

            if (ambiguousForeignKeyIdentity)
            {
                continue;
            }

            foreach (var matchKey in sourceForeignKeysByMatchKey.Keys
                         .Concat(liveForeignKeysByMatchKey.Keys)
                         .Distinct(StringComparer.Ordinal)
                         .OrderBy(row => row, StringComparer.Ordinal))
            {
                sourceForeignKeysByMatchKey.TryGetValue(matchKey, out var sourceForeignKey);
                liveForeignKeysByMatchKey.TryGetValue(matchKey, out var liveForeignKey);

                var sourceCovered = sourceForeignKey is not null &&
                                    (addForeignKeyIds.Contains(sourceForeignKey.Id) ||
                                     replaceForeignKeySourceIds.Contains(sourceForeignKey.Id));
                var liveCovered = liveForeignKey is not null &&
                                  (dropForeignKeyIds.Contains(liveForeignKey.Id) ||
                                   replaceForeignKeyLiveIds.Contains(liveForeignKey.Id));

                if (sourceForeignKey is not null && liveForeignKey is not null)
                {
                    if (sourceCovered && liveCovered)
                    {
                        continue;
                    }

                    if (!sourceCovered && !liveCovered)
                    {
                        var foreignKeyDifference = new MetaSqlDifference
                        {
                            ObjectKind = MetaSqlObjectKind.ForeignKey,
                            DifferenceKind = MetaSqlDifferenceKind.Different,
                            ScopeDisplayName = string.Empty,
                            DisplayName = GetValue(sourceForeignKey, "Name"),
                            SourceId = sourceForeignKey.Id,
                            LiveId = liveForeignKey.Id,
                        };
                        var foreignKeyAssessment = AssessForeignKeyDifference(
                            foreignKeyDifference,
                            sourceForeignKeysById,
                            liveForeignKeysById,
                            sourceForeignKeyColumnsByForeignKeyId,
                            liveForeignKeyColumnsByForeignKeyId,
                            sourceTablesById,
                            liveTablesById,
                            sourceColumnsById,
                            liveColumnsById,
                            plannedAddedTableIds,
                            plannedAddedColumnIds);
                        if (!foreignKeyAssessment.Executable)
                        {
                            addedBlockCount += AddColumnDependencyBlock(
                                model,
                                root,
                                alterTableColumn,
                                $"AlterTableColumn requires executable replacement for dependent foreign key '{GetValue(sourceForeignKey, "Name")}': {foreignKeyAssessment.Reason}");
                            break;
                        }

                        var pairKey = BuildReplaceForeignKeyPairKey(sourceForeignKey.Id, liveForeignKey.Id);
                        if (!replaceForeignKeyByPair.ContainsKey(pairKey))
                        {
                            var replaceForeignKey = new ReplaceForeignKey
                            {
                                Id = $"ReplaceForeignKey:{sourceForeignKey.Id}:{liveForeignKey.Id}",
                                SourceForeignKeyId = sourceForeignKey.Id,
                                LiveForeignKeyId = liveForeignKey.Id,
                                DeployManifestId = root.Id,
                                DeployManifest = root,
                            };
                            model.ReplaceForeignKeyList.Add(replaceForeignKey);
                            replaceForeignKeyByPair[pairKey] = replaceForeignKey;
                            replaceForeignKeySourceIds.Add(sourceForeignKey.Id);
                            replaceForeignKeyLiveIds.Add(liveForeignKey.Id);
                            addedReplaceCount++;
                        }

                        continue;
                    }

                    addedBlockCount += AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn has partial dependent foreign key coverage for '{matchKey}'. Dependent foreign key choreography must be explicit.");
                    break;
                }

                if (sourceForeignKey is null && liveForeignKey is not null && !liveCovered)
                {
                    addedBlockCount += AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn requires explicit drop action for dependent live foreign key '{liveForeignKey.Id}'.");
                    break;
                }

                if (sourceForeignKey is not null && liveForeignKey is null && !sourceCovered)
                {
                    addedBlockCount += AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn requires explicit add action for dependent source foreign key '{sourceForeignKey.Id}'.");
                    break;
                }
            }
        }

        return (addedReplaceCount, addedBlockCount);
    }

    private static IReadOnlyDictionary<string, List<GenericRecord>> GetGroupedRecordsByTargetTableId(IEnumerable<GenericRecord> foreignKeys)
    {
        return foreignKeys
            .GroupBy(row => row.RelationshipIds["TargetTableId"], StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(row => row.Id, StringComparer.Ordinal).ToList(),
                StringComparer.Ordinal);
    }

    private static string BuildPrimaryKeyMatchKey(GenericRecord primaryKey)
    {
        var tableId = primaryKey.RelationshipIds["TableId"];
        var name = GetValue(primaryKey, "Name");
        return tableId + "|" + name;
    }

    private static string BuildReplacePrimaryKeyPairKey(string sourcePrimaryKeyId, string livePrimaryKeyId)
    {
        return sourcePrimaryKeyId + "|" + livePrimaryKeyId;
    }

    private static (int AddedReplaceForeignKeyCount, int AddedBlockCount) AddExplicitDependentForeignKeyEntriesForPrimaryKeyReplacements(
        MetaSqlDeployManifestModel model,
        DeployManifest root,
        IReadOnlyDictionary<string, GenericRecord> sourcePrimaryKeysById,
        IReadOnlyDictionary<string, GenericRecord> livePrimaryKeysById,
        IReadOnlyDictionary<string, List<GenericRecord>> sourcePrimaryKeyColumnsByPrimaryKeyId,
        IReadOnlyDictionary<string, List<GenericRecord>> livePrimaryKeyColumnsByPrimaryKeyId,
        IReadOnlyDictionary<string, GenericRecord> sourceForeignKeysById,
        IReadOnlyDictionary<string, GenericRecord> liveForeignKeysById,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceForeignKeyColumnsByForeignKeyId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveForeignKeyColumnsByForeignKeyId)
    {
        var replaceForeignKeyByPair = model.ReplaceForeignKeyList
            .ToDictionary(
                row => BuildReplaceForeignKeyPairKey(row.SourceForeignKeyId, row.LiveForeignKeyId),
                row => row,
                StringComparer.Ordinal);
        var addForeignKeyIds = model.AddForeignKeyList
            .Select(row => row.SourceForeignKeyId)
            .ToHashSet(StringComparer.Ordinal);
        var dropForeignKeyIds = model.DropForeignKeyList
            .Select(row => row.LiveForeignKeyId)
            .ToHashSet(StringComparer.Ordinal);

        var addedReplaceForeignKeyCount = 0;
        var addedBlockCount = 0;

        foreach (var replacePrimaryKey in model.ReplacePrimaryKeyList
                     .OrderBy(row => row.SourcePrimaryKeyId, StringComparer.Ordinal)
                     .ThenBy(row => row.LivePrimaryKeyId, StringComparer.Ordinal))
        {
            if (!sourcePrimaryKeysById.TryGetValue(replacePrimaryKey.SourcePrimaryKeyId, out var sourcePrimaryKey) ||
                !livePrimaryKeysById.TryGetValue(replacePrimaryKey.LivePrimaryKeyId, out var livePrimaryKey))
            {
                addedBlockCount += AddPrimaryKeyDependencyBlock(
                    model,
                    root,
                    replacePrimaryKey,
                    "ReplacePrimaryKey references missing source/live primary key row.");
                continue;
            }

            var sourceMembers = GetOrderedPrimaryKeyMembers(sourcePrimaryKeyColumnsByPrimaryKeyId, sourcePrimaryKey.Id);
            var liveMembers = GetOrderedPrimaryKeyMembers(livePrimaryKeyColumnsByPrimaryKeyId, livePrimaryKey.Id);
            if (sourceMembers.Count == 0 || liveMembers.Count == 0)
            {
                addedBlockCount += AddPrimaryKeyDependencyBlock(
                    model,
                    root,
                    replacePrimaryKey,
                    "ReplacePrimaryKey requires source/live primary key member rows.");
                continue;
            }

            var sourcePrimaryKeyColumnIds = sourceMembers
                .Select(row => row.RelationshipIds["TableColumnId"])
                .ToList();
            var livePrimaryKeyColumnIds = liveMembers
                .Select(row => row.RelationshipIds["TableColumnId"])
                .ToList();

            var sourceDependentForeignKeys = GetDependentForeignKeysBySourceScope(
                sourceForeignKeysById.Values,
                sourceForeignKeyColumnsByForeignKeyId,
                sourcePrimaryKey.RelationshipIds["TableId"],
                sourcePrimaryKeyColumnIds);
            var liveDependentForeignKeys = GetDependentForeignKeysByLiveScope(
                liveForeignKeysById.Values,
                liveForeignKeyColumnsByForeignKeyId,
                livePrimaryKey.RelationshipIds["TableId"],
                livePrimaryKeyColumnIds);

            var sourceByMatchKey = BuildForeignKeyMatchIndex(sourceDependentForeignKeys);
            var liveByMatchKey = BuildForeignKeyMatchIndex(liveDependentForeignKeys);
            if (sourceByMatchKey is null || liveByMatchKey is null)
            {
                addedBlockCount += AddPrimaryKeyDependencyBlock(
                    model,
                    root,
                    replacePrimaryKey,
                    "ReplacePrimaryKey has ambiguous dependent foreign key identity by (SourceTableId, Name).");
                continue;
            }

            var matchKeys = sourceByMatchKey.Keys
                .Concat(liveByMatchKey.Keys)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(row => row, StringComparer.Ordinal)
                .ToList();

            var blocked = false;
            foreach (var matchKey in matchKeys)
            {
                sourceByMatchKey.TryGetValue(matchKey, out var sourceForeignKey);
                liveByMatchKey.TryGetValue(matchKey, out var liveForeignKey);

                var sourceCovered = sourceForeignKey is not null &&
                                    (addForeignKeyIds.Contains(sourceForeignKey.Id) ||
                                     model.ReplaceForeignKeyList.Any(row => string.Equals(row.SourceForeignKeyId, sourceForeignKey.Id, StringComparison.Ordinal)));
                var liveCovered = liveForeignKey is not null &&
                                  (dropForeignKeyIds.Contains(liveForeignKey.Id) ||
                                   model.ReplaceForeignKeyList.Any(row => string.Equals(row.LiveForeignKeyId, liveForeignKey.Id, StringComparison.Ordinal)));

                if (sourceForeignKey is not null && liveForeignKey is not null)
                {
                    if (sourceCovered && liveCovered)
                    {
                        continue;
                    }

                    if (!sourceCovered && !liveCovered)
                    {
                        var pairKey = BuildReplaceForeignKeyPairKey(sourceForeignKey.Id, liveForeignKey.Id);
                        if (!replaceForeignKeyByPair.ContainsKey(pairKey))
                        {
                            var replaceForeignKey = new ReplaceForeignKey
                            {
                                Id = $"ReplaceForeignKey:{sourceForeignKey.Id}:{liveForeignKey.Id}",
                                SourceForeignKeyId = sourceForeignKey.Id,
                                LiveForeignKeyId = liveForeignKey.Id,
                                DeployManifestId = root.Id,
                                DeployManifest = root,
                            };
                            model.ReplaceForeignKeyList.Add(replaceForeignKey);
                            replaceForeignKeyByPair[pairKey] = replaceForeignKey;
                            addedReplaceForeignKeyCount++;
                        }

                        continue;
                    }

                    blocked = true;
                    addedBlockCount += AddPrimaryKeyDependencyBlock(
                        model,
                        root,
                        replacePrimaryKey,
                        $"ReplacePrimaryKey has partial dependent foreign key coverage for '{matchKey}'. Dependent FK choreography must be explicit.");
                    break;
                }

                if (sourceForeignKey is null && liveForeignKey is not null && !liveCovered)
                {
                    blocked = true;
                    addedBlockCount += AddPrimaryKeyDependencyBlock(
                        model,
                        root,
                        replacePrimaryKey,
                        $"ReplacePrimaryKey requires explicit drop action for dependent live foreign key '{liveForeignKey.Id}'.");
                    break;
                }

                if (sourceForeignKey is not null && liveForeignKey is null && !sourceCovered)
                {
                    blocked = true;
                    addedBlockCount += AddPrimaryKeyDependencyBlock(
                        model,
                        root,
                        replacePrimaryKey,
                        $"ReplacePrimaryKey requires explicit add action for dependent source foreign key '{sourceForeignKey.Id}'.");
                    break;
                }
            }

            if (blocked)
            {
                continue;
            }
        }

        return (addedReplaceForeignKeyCount, addedBlockCount);
    }

    private static List<GenericRecord> GetDependentForeignKeysBySourceScope(
        IEnumerable<GenericRecord> sourceForeignKeys,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceForeignKeyColumnsByForeignKeyId,
        string targetTableId,
        IReadOnlyList<string> targetPrimaryKeyColumnIds)
    {
        return sourceForeignKeys
            .Where(row => string.Equals(row.RelationshipIds["TargetTableId"], targetTableId, StringComparison.Ordinal))
            .Where(row =>
            {
                var members = GetOrderedForeignKeyMembers(sourceForeignKeyColumnsByForeignKeyId, row.Id);
                if (members.Count == 0)
                {
                    return false;
                }

                var targetColumnIds = members
                    .Select(member => member.RelationshipIds["TargetColumnId"])
                    .ToList();
                return targetColumnIds.SequenceEqual(targetPrimaryKeyColumnIds);
            })
            .OrderBy(row => row.Id, StringComparer.Ordinal)
            .ToList();
    }

    private static List<GenericRecord> GetDependentForeignKeysByLiveScope(
        IEnumerable<GenericRecord> liveForeignKeys,
        IReadOnlyDictionary<string, List<GenericRecord>> liveForeignKeyColumnsByForeignKeyId,
        string targetTableId,
        IReadOnlyList<string> targetPrimaryKeyColumnIds)
    {
        return liveForeignKeys
            .Where(row => string.Equals(row.RelationshipIds["TargetTableId"], targetTableId, StringComparison.Ordinal))
            .Where(row =>
            {
                var members = GetOrderedForeignKeyMembers(liveForeignKeyColumnsByForeignKeyId, row.Id);
                if (members.Count == 0)
                {
                    return false;
                }

                var targetColumnIds = members
                    .Select(member => member.RelationshipIds["TargetColumnId"])
                    .ToList();
                return targetColumnIds.SequenceEqual(targetPrimaryKeyColumnIds);
            })
            .OrderBy(row => row.Id, StringComparer.Ordinal)
            .ToList();
    }

    private static Dictionary<string, GenericRecord>? BuildForeignKeyMatchIndex(
        IEnumerable<GenericRecord> foreignKeys)
    {
        var index = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
        foreach (var foreignKey in foreignKeys)
        {
            var matchKey = BuildForeignKeyMatchKey(foreignKey);
            if (!index.TryAdd(matchKey, foreignKey))
            {
                return null;
            }
        }

        return index;
    }

    private static string BuildForeignKeyMatchKey(GenericRecord foreignKey)
    {
        var sourceTableId = foreignKey.RelationshipIds["SourceTableId"];
        var name = GetValue(foreignKey, "Name");
        return sourceTableId + "|" + name;
    }

    private static string BuildReplaceForeignKeyPairKey(string sourceForeignKeyId, string liveForeignKeyId)
    {
        return sourceForeignKeyId + "|" + liveForeignKeyId;
    }

    private static (int AddedReplaceIndexCount, int AddedBlockCount) AddExplicitDependentIndexEntriesForAlteredColumns(
        MetaSqlDeployManifestModel model,
        DeployManifest root,
        IReadOnlyDictionary<string, GenericRecord> sourceColumnsById,
        IReadOnlyDictionary<string, GenericRecord> liveColumnsById,
        IReadOnlyDictionary<string, GenericRecord> sourceIndexesById,
        IReadOnlyDictionary<string, GenericRecord> liveIndexesById,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceIndexColumnsByIndexId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveIndexColumnsByIndexId,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceIndexColumnsByColumnId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveIndexColumnsByColumnId,
        IReadOnlyDictionary<string, GenericRecord> sourceTablesById,
        IReadOnlyDictionary<string, GenericRecord> liveTablesById,
        IReadOnlySet<string> plannedAddedTableIds,
        IReadOnlySet<string> plannedAddedColumnIds)
    {
        var replaceIndexByPair = model.ReplaceIndexList
            .ToDictionary(
                row => BuildReplaceIndexPairKey(row.SourceIndexId, row.LiveIndexId),
                row => row,
                StringComparer.Ordinal);
        var addIndexIds = model.AddIndexList
            .Select(row => row.SourceIndexId)
            .ToHashSet(StringComparer.Ordinal);
        var dropIndexIds = model.DropIndexList
            .Select(row => row.LiveIndexId)
            .ToHashSet(StringComparer.Ordinal);
        var replaceSourceIndexIds = model.ReplaceIndexList
            .Select(row => row.SourceIndexId)
            .ToHashSet(StringComparer.Ordinal);
        var replaceLiveIndexIds = model.ReplaceIndexList
            .Select(row => row.LiveIndexId)
            .ToHashSet(StringComparer.Ordinal);

        var addedReplaceIndexCount = 0;
        var addedBlockCount = 0;

        foreach (var alterTableColumn in model.AlterTableColumnList
                     .OrderBy(row => row.SourceTableColumnId, StringComparer.Ordinal)
                     .ThenBy(row => row.LiveTableColumnId, StringComparer.Ordinal))
        {
            if (!sourceColumnsById.TryGetValue(alterTableColumn.SourceTableColumnId, out var sourceColumn) ||
                !liveColumnsById.TryGetValue(alterTableColumn.LiveTableColumnId, out var liveColumn))
            {
                addedBlockCount += AddColumnDependencyBlock(
                    model,
                    root,
                    alterTableColumn,
                    "AlterTableColumn references missing source/live table-column row.");
                continue;
            }

            var sourceDependentIndexIds = sourceIndexColumnsByColumnId.TryGetValue(sourceColumn.Id, out var sourceIndexMembers)
                ? sourceIndexMembers.Select(row => row.RelationshipIds["IndexId"]).Distinct(StringComparer.Ordinal).ToList()
                : [];
            var liveDependentIndexIds = liveIndexColumnsByColumnId.TryGetValue(liveColumn.Id, out var liveIndexMembers)
                ? liveIndexMembers.Select(row => row.RelationshipIds["IndexId"]).Distinct(StringComparer.Ordinal).ToList()
                : [];
            if (sourceDependentIndexIds.Count == 0 && liveDependentIndexIds.Count == 0)
            {
                continue;
            }

            var sourceByMatchKey = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
            var liveByMatchKey = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
            var ambiguous = false;

            foreach (var sourceIndexId in sourceDependentIndexIds.OrderBy(row => row, StringComparer.Ordinal))
            {
                if (!sourceIndexesById.TryGetValue(sourceIndexId, out var sourceIndex))
                {
                    addedBlockCount += AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn references missing dependent source index '{sourceIndexId}'.");
                    ambiguous = true;
                    break;
                }

                var key = BuildIndexMatchKey(sourceIndex);
                if (!sourceByMatchKey.TryAdd(key, sourceIndex))
                {
                    addedBlockCount += AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn has ambiguous dependent source index identity by (TableId, Name): '{key}'.");
                    ambiguous = true;
                    break;
                }
            }

            if (ambiguous)
            {
                continue;
            }

            foreach (var liveIndexId in liveDependentIndexIds.OrderBy(row => row, StringComparer.Ordinal))
            {
                if (!liveIndexesById.TryGetValue(liveIndexId, out var liveIndex))
                {
                    addedBlockCount += AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn references missing dependent live index '{liveIndexId}'.");
                    ambiguous = true;
                    break;
                }

                var key = BuildIndexMatchKey(liveIndex);
                if (!liveByMatchKey.TryAdd(key, liveIndex))
                {
                    addedBlockCount += AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn has ambiguous dependent live index identity by (TableId, Name): '{key}'.");
                    ambiguous = true;
                    break;
                }
            }

            if (ambiguous)
            {
                continue;
            }

            foreach (var matchKey in sourceByMatchKey.Keys
                         .Concat(liveByMatchKey.Keys)
                         .Distinct(StringComparer.Ordinal)
                         .OrderBy(row => row, StringComparer.Ordinal))
            {
                sourceByMatchKey.TryGetValue(matchKey, out var sourceIndex);
                liveByMatchKey.TryGetValue(matchKey, out var liveIndex);

                var sourceCovered = sourceIndex is not null &&
                                    (addIndexIds.Contains(sourceIndex.Id) ||
                                     replaceSourceIndexIds.Contains(sourceIndex.Id));
                var liveCovered = liveIndex is not null &&
                                  (dropIndexIds.Contains(liveIndex.Id) ||
                                   replaceLiveIndexIds.Contains(liveIndex.Id));

                if (sourceIndex is not null && liveIndex is not null)
                {
                    if (sourceCovered && liveCovered)
                    {
                        continue;
                    }

                    if (!sourceCovered && !liveCovered)
                    {
                        var indexDifference = new MetaSqlDifference
                        {
                            ObjectKind = MetaSqlObjectKind.Index,
                            DifferenceKind = MetaSqlDifferenceKind.Different,
                            ScopeDisplayName = string.Empty,
                            DisplayName = GetValue(sourceIndex, "Name"),
                            SourceId = sourceIndex.Id,
                            LiveId = liveIndex.Id,
                        };
                        var assessment = AssessIndexDifference(
                            indexDifference,
                            sourceIndexesById,
                            liveIndexesById,
                            sourceIndexColumnsByIndexId,
                            liveIndexColumnsByIndexId,
                            sourceTablesById,
                            liveTablesById,
                            sourceColumnsById,
                            liveColumnsById,
                            plannedAddedTableIds,
                            plannedAddedColumnIds);
                        if (!assessment.Executable)
                        {
                            addedBlockCount += AddColumnDependencyBlock(
                                model,
                                root,
                                alterTableColumn,
                                $"AlterTableColumn requires executable replacement for dependent index '{GetValue(sourceIndex, "Name")}': {assessment.Reason}");
                            break;
                        }

                        var pairKey = BuildReplaceIndexPairKey(sourceIndex.Id, liveIndex.Id);
                        if (!replaceIndexByPair.ContainsKey(pairKey))
                        {
                            var replaceIndex = new ReplaceIndex
                            {
                                Id = $"ReplaceIndex:{sourceIndex.Id}:{liveIndex.Id}",
                                SourceIndexId = sourceIndex.Id,
                                LiveIndexId = liveIndex.Id,
                                DeployManifestId = root.Id,
                                DeployManifest = root,
                            };
                            model.ReplaceIndexList.Add(replaceIndex);
                            replaceIndexByPair[pairKey] = replaceIndex;
                            replaceSourceIndexIds.Add(sourceIndex.Id);
                            replaceLiveIndexIds.Add(liveIndex.Id);
                            addedReplaceIndexCount++;
                        }

                        continue;
                    }

                    addedBlockCount += AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn has partial dependent index coverage for '{matchKey}'. Dependent index choreography must be explicit.");
                    break;
                }

                if (sourceIndex is null && liveIndex is not null && !liveCovered)
                {
                    addedBlockCount += AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn requires explicit drop action for dependent live index '{liveIndex.Id}'.");
                    break;
                }

                if (sourceIndex is not null && liveIndex is null && !sourceCovered)
                {
                    addedBlockCount += AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn requires explicit add action for dependent source index '{sourceIndex.Id}'.");
                    break;
                }
            }
        }

        return (addedReplaceIndexCount, addedBlockCount);
    }

    private static string BuildIndexMatchKey(GenericRecord index)
    {
        var tableId = index.RelationshipIds["TableId"];
        var name = GetValue(index, "Name");
        return tableId + "|" + name;
    }

    private static string BuildReplaceIndexPairKey(string sourceIndexId, string liveIndexId)
    {
        return sourceIndexId + "|" + liveIndexId;
    }

    private static int AddColumnDependencyBlock(
        MetaSqlDeployManifestModel model,
        DeployManifest root,
        AlterTableColumn alterTableColumn,
        string reason)
    {
        var existing = model.BlockTableColumnDifferenceList.Any(row =>
            string.Equals(row.SourceTableColumnId, alterTableColumn.SourceTableColumnId, StringComparison.Ordinal) &&
            string.Equals(row.LiveTableColumnId, alterTableColumn.LiveTableColumnId, StringComparison.Ordinal) &&
            string.Equals(row.DifferenceSummary, reason, StringComparison.Ordinal));
        if (existing)
        {
            return 0;
        }

        model.BlockTableColumnDifferenceList.Add(new BlockTableColumnDifference
        {
            Id = $"BlockTableColumnDifference:{alterTableColumn.SourceTableColumnId}:{alterTableColumn.LiveTableColumnId}:DependentIndex:{model.BlockTableColumnDifferenceList.Count + 1}",
            SourceTableColumnId = alterTableColumn.SourceTableColumnId,
            LiveTableColumnId = alterTableColumn.LiveTableColumnId,
            DifferenceSummary = reason,
            DeployManifestId = root.Id,
            DeployManifest = root,
        });
        return 1;
    }

    private static int AddPrimaryKeyDependencyBlock(
        MetaSqlDeployManifestModel model,
        DeployManifest root,
        ReplacePrimaryKey replacePrimaryKey,
        string reason)
    {
        var existing = model.BlockPrimaryKeyDifferenceList.Any(row =>
            string.Equals(row.SourcePrimaryKeyId, replacePrimaryKey.SourcePrimaryKeyId, StringComparison.Ordinal) &&
            string.Equals(row.LivePrimaryKeyId, replacePrimaryKey.LivePrimaryKeyId, StringComparison.Ordinal) &&
            string.Equals(row.DifferenceSummary, reason, StringComparison.Ordinal));
        if (existing)
        {
            return 0;
        }

        model.BlockPrimaryKeyDifferenceList.Add(new BlockPrimaryKeyDifference
        {
            Id = $"BlockPrimaryKeyDifference:{replacePrimaryKey.SourcePrimaryKeyId}:{replacePrimaryKey.LivePrimaryKeyId}:DependentForeignKey:{model.BlockPrimaryKeyDifferenceList.Count + 1}",
            SourcePrimaryKeyId = replacePrimaryKey.SourcePrimaryKeyId,
            LivePrimaryKeyId = replacePrimaryKey.LivePrimaryKeyId,
            DifferenceSummary = reason,
            DeployManifestId = root.Id,
            DeployManifest = root,
        });
        return 1;
    }

    private static int BlockEntry(
        MetaSqlDeployManifestModel model,
        DeployManifest root,
        MetaSqlDifference difference,
        string? summaryOverride = null)
    {
        var summary = summaryOverride ?? BuildSummary(difference);
        switch (difference.ObjectKind)
        {
            case MetaSqlObjectKind.Table:
                model.BlockTableDifferenceList.Add(new BlockTableDifference
                {
                    Id = $"BlockTableDifference:{RequireValue(difference.SourceId, "SourceId", difference)}:{RequireValue(difference.LiveId, "LiveId", difference)}",
                    SourceTableId = RequireValue(difference.SourceId, "SourceId", difference),
                    LiveTableId = RequireValue(difference.LiveId, "LiveId", difference),
                    DifferenceSummary = summary,
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.TableColumn:
                model.BlockTableColumnDifferenceList.Add(new BlockTableColumnDifference
                {
                    Id = $"BlockTableColumnDifference:{RequireValue(difference.SourceId, "SourceId", difference)}:{RequireValue(difference.LiveId, "LiveId", difference)}",
                    SourceTableColumnId = RequireValue(difference.SourceId, "SourceId", difference),
                    LiveTableColumnId = RequireValue(difference.LiveId, "LiveId", difference),
                    DifferenceSummary = summary,
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.PrimaryKey:
                model.BlockPrimaryKeyDifferenceList.Add(new BlockPrimaryKeyDifference
                {
                    Id = $"BlockPrimaryKeyDifference:{RequireValue(difference.SourceId, "SourceId", difference)}:{RequireValue(difference.LiveId, "LiveId", difference)}",
                    SourcePrimaryKeyId = RequireValue(difference.SourceId, "SourceId", difference),
                    LivePrimaryKeyId = RequireValue(difference.LiveId, "LiveId", difference),
                    DifferenceSummary = summary,
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.ForeignKey:
                model.BlockForeignKeyDifferenceList.Add(new BlockForeignKeyDifference
                {
                    Id = $"BlockForeignKeyDifference:{RequireValue(difference.SourceId, "SourceId", difference)}:{RequireValue(difference.LiveId, "LiveId", difference)}",
                    SourceForeignKeyId = RequireValue(difference.SourceId, "SourceId", difference),
                    LiveForeignKeyId = RequireValue(difference.LiveId, "LiveId", difference),
                    DifferenceSummary = summary,
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.Index:
                model.BlockIndexDifferenceList.Add(new BlockIndexDifference
                {
                    Id = $"BlockIndexDifference:{RequireValue(difference.SourceId, "SourceId", difference)}:{RequireValue(difference.LiveId, "LiveId", difference)}",
                    SourceIndexId = RequireValue(difference.SourceId, "SourceId", difference),
                    LiveIndexId = RequireValue(difference.LiveId, "LiveId", difference),
                    DifferenceSummary = summary,
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            default:
                throw new InvalidOperationException($"Unsupported block object kind '{difference.ObjectKind}'.");
        }
    }

    private static (bool Executable, string Reason) AssessPrimaryKeyDifference(
        MetaSqlDifference difference,
        IReadOnlyDictionary<string, GenericRecord> sourcePrimaryKeysById,
        IReadOnlyDictionary<string, GenericRecord> livePrimaryKeysById,
        IReadOnlyDictionary<string, List<GenericRecord>> sourcePrimaryKeyColumnsByPrimaryKeyId,
        IReadOnlyDictionary<string, List<GenericRecord>> livePrimaryKeyColumnsByPrimaryKeyId,
        IReadOnlyDictionary<string, GenericRecord> sourceForeignKeysById,
        IReadOnlyDictionary<string, GenericRecord> liveForeignKeysById,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceForeignKeysByTargetTableId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveForeignKeysByTargetTableId,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceForeignKeyColumnsByForeignKeyId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveForeignKeyColumnsByForeignKeyId,
        IReadOnlyDictionary<string, GenericRecord> sourceTablesById,
        IReadOnlyDictionary<string, GenericRecord> liveTablesById,
        IReadOnlyDictionary<string, GenericRecord> sourceColumnsById,
        IReadOnlyDictionary<string, GenericRecord> liveColumnsById,
        IReadOnlySet<string> plannedAddedTableIds,
        IReadOnlySet<string> plannedAddedColumnIds,
        IReadOnlySet<string> plannedDroppedForeignKeyIds,
        IReadOnlySet<string> plannedAddedForeignKeyIds)
    {
        var sourceId = difference.SourceId ?? string.Empty;
        var liveId = difference.LiveId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sourceId) || string.IsNullOrWhiteSpace(liveId))
        {
            return (false, $"{difference.DisplayName}: missing SourceId/LiveId for changed primary key.");
        }

        if (!sourcePrimaryKeysById.TryGetValue(sourceId, out var sourcePrimaryKey) ||
            !livePrimaryKeysById.TryGetValue(liveId, out var livePrimaryKey))
        {
            return (false, $"{difference.DisplayName}: changed primary key row is missing in source or live workspace.");
        }

        var sourceTableId = sourcePrimaryKey.RelationshipIds["TableId"];
        var liveTableId = livePrimaryKey.RelationshipIds["TableId"];
        if (!string.Equals(sourceTableId, liveTableId, StringComparison.Ordinal))
        {
            return (false, $"{difference.DisplayName}: ReplacePrimaryKey requires the same table scope in source and live.");
        }

        var sourceName = GetValue(sourcePrimaryKey, "Name");
        var liveName = GetValue(livePrimaryKey, "Name");
        if (!string.Equals(sourceName, liveName, StringComparison.Ordinal))
        {
            return (false, $"{difference.DisplayName}: ReplacePrimaryKey requires identical primary key names.");
        }

        if (!sourceTablesById.ContainsKey(sourceTableId))
        {
            return (false, $"{difference.DisplayName}: source primary key references missing table '{sourceTableId}'.");
        }

        if (!(liveTablesById.ContainsKey(sourceTableId) || plannedAddedTableIds.Contains(sourceTableId)))
        {
            return (false, $"{difference.DisplayName}: primary key table '{sourceTableId}' is not present in live and not planned as AddTable.");
        }

        var sourceIsClusteredRaw = GetValue(sourcePrimaryKey, "IsClustered");
        var liveIsClusteredRaw = GetValue(livePrimaryKey, "IsClustered");
        if (!TryParseOptionalBoolean(sourceIsClusteredRaw, out var sourceIsClustered))
        {
            return (false, $"{difference.DisplayName}: source primary key has malformed IsClustered value '{sourceIsClusteredRaw}'.");
        }

        if (!TryParseOptionalBoolean(liveIsClusteredRaw, out var liveIsClustered))
        {
            return (false, $"{difference.DisplayName}: live primary key has malformed IsClustered value '{liveIsClusteredRaw}'.");
        }

        if (sourceIsClustered || liveIsClustered)
        {
            return (false, $"{difference.DisplayName}: clustered primary key replacement is blocked in this slice.");
        }

        var sourceMembers = GetOrderedPrimaryKeyMembers(sourcePrimaryKeyColumnsByPrimaryKeyId, sourceId);
        var liveMembers = GetOrderedPrimaryKeyMembers(livePrimaryKeyColumnsByPrimaryKeyId, liveId);
        if (sourceMembers.Count == 0)
        {
            return (false, $"{difference.DisplayName}: source primary key has no member rows.");
        }

        if (liveMembers.Count == 0)
        {
            return (false, $"{difference.DisplayName}: live primary key has no member rows.");
        }

        var sourcePkColumnIds = new List<string>(sourceMembers.Count);
        foreach (var sourceMember in sourceMembers)
        {
            var sourceColumnId = sourceMember.RelationshipIds["TableColumnId"];
            sourcePkColumnIds.Add(sourceColumnId);
            if (!sourceColumnsById.TryGetValue(sourceColumnId, out var sourceColumn))
            {
                return (false, $"{difference.DisplayName}: source primary key references missing table column '{sourceColumnId}'.");
            }

            if (!string.Equals(sourceColumn.RelationshipIds["TableId"], sourceTableId, StringComparison.Ordinal))
            {
                return (false, $"{difference.DisplayName}: source primary key member column '{sourceColumnId}' is outside the source table scope.");
            }

            if (!(liveColumnsById.ContainsKey(sourceColumnId) || plannedAddedColumnIds.Contains(sourceColumnId)))
            {
                return (false, $"{difference.DisplayName}: source primary key member column '{sourceColumnId}' is not present in live and not planned as AddTableColumn.");
            }

            var nullableRaw = GetValue(sourceColumn, "IsNullable");
            if (!TryParseOptionalBoolean(nullableRaw, out var isNullable))
            {
                return (false, $"{difference.DisplayName}: source primary key member column '{sourceColumnId}' has malformed IsNullable value '{nullableRaw}'.");
            }

            if (isNullable)
            {
                return (false, $"{difference.DisplayName}: source primary key member column '{sourceColumnId}' is nullable.");
            }

            var isDescendingRaw = GetValue(sourceMember, "IsDescending");
            if (!TryParseOptionalBoolean(isDescendingRaw, out _))
            {
                return (false, $"{difference.DisplayName}: source primary key member '{sourceMember.Id}' has malformed IsDescending value '{isDescendingRaw}'.");
            }
        }

        var livePkColumnIds = new List<string>(liveMembers.Count);
        foreach (var liveMember in liveMembers)
        {
            var liveColumnId = liveMember.RelationshipIds["TableColumnId"];
            livePkColumnIds.Add(liveColumnId);
            if (!liveColumnsById.TryGetValue(liveColumnId, out var liveColumn))
            {
                return (false, $"{difference.DisplayName}: live primary key references missing table column '{liveColumnId}'.");
            }

            if (!string.Equals(liveColumn.RelationshipIds["TableId"], liveTableId, StringComparison.Ordinal))
            {
                return (false, $"{difference.DisplayName}: live primary key member column '{liveColumnId}' is outside the live table scope.");
            }

            var isDescendingRaw = GetValue(liveMember, "IsDescending");
            if (!TryParseOptionalBoolean(isDescendingRaw, out _))
            {
                return (false, $"{difference.DisplayName}: live primary key member '{liveMember.Id}' has malformed IsDescending value '{isDescendingRaw}'.");
            }
        }

        var sourceForeignKeyMatchKeys = sourceForeignKeysById.Values
            .Select(BuildForeignKeyMatchKey)
            .ToHashSet(StringComparer.Ordinal);
        var liveForeignKeyMatchKeys = liveForeignKeysById.Values
            .Select(BuildForeignKeyMatchKey)
            .ToHashSet(StringComparer.Ordinal);

        var sourceDependentForeignKeys = GetOrderedTargetTableForeignKeys(sourceForeignKeysByTargetTableId, sourceTableId);
        foreach (var sourceForeignKey in sourceDependentForeignKeys)
        {
            var sourceForeignKeyId = sourceForeignKey.Id;
            var sourceForeignKeyMembers = GetOrderedForeignKeyMembers(sourceForeignKeyColumnsByForeignKeyId, sourceForeignKeyId);
            if (sourceForeignKeyMembers.Count == 0)
            {
                return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' has no member rows.");
            }

            var sourceTargetColumnIds = sourceForeignKeyMembers
                .Select(row => row.RelationshipIds["TargetColumnId"])
                .ToList();
            var referencesSourcePrimaryKey = sourceTargetColumnIds.SequenceEqual(sourcePkColumnIds);
            var intersectsSourcePrimaryKey = sourceTargetColumnIds.Any(row => sourcePkColumnIds.Contains(row, StringComparer.Ordinal));
            if (!referencesSourcePrimaryKey && intersectsSourcePrimaryKey)
            {
                return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' has unsupported target-column shape for ReplacePrimaryKey choreography.");
            }

            if (!referencesSourcePrimaryKey)
            {
                continue;
            }

            var sourceForeignKeySourceTableId = sourceForeignKey.RelationshipIds["SourceTableId"];
            if (!(liveTablesById.ContainsKey(sourceForeignKeySourceTableId) || plannedAddedTableIds.Contains(sourceForeignKeySourceTableId)))
            {
                return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' source table '{sourceForeignKeySourceTableId}' is not present in live and not planned as AddTable.");
            }

            foreach (var sourceForeignKeyMember in sourceForeignKeyMembers)
            {
                var sourceColumnId = sourceForeignKeyMember.RelationshipIds["SourceColumnId"];
                var targetColumnId = sourceForeignKeyMember.RelationshipIds["TargetColumnId"];
                if (!sourceColumnsById.TryGetValue(sourceColumnId, out var sourceColumn))
                {
                    return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' references missing source column '{sourceColumnId}'.");
                }

                if (!sourceColumnsById.TryGetValue(targetColumnId, out var targetColumn))
                {
                    return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' references missing target column '{targetColumnId}'.");
                }

                if (!(liveColumnsById.ContainsKey(sourceColumnId) || plannedAddedColumnIds.Contains(sourceColumnId)))
                {
                    return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' source column '{sourceColumnId}' is not present in live and not planned as AddTableColumn.");
                }

                if (!(liveColumnsById.ContainsKey(targetColumnId) || plannedAddedColumnIds.Contains(targetColumnId)))
                {
                    return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' target column '{targetColumnId}' is not present in live and not planned as AddTableColumn.");
                }

                if (!string.Equals(targetColumn.RelationshipIds["TableId"], sourceTableId, StringComparison.Ordinal))
                {
                    return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' target column '{targetColumnId}' is outside the source primary key table scope.");
                }
            }

            var sourceForeignKeyMatchKey = BuildForeignKeyMatchKey(sourceForeignKey);
            if (!(liveForeignKeyMatchKeys.Contains(sourceForeignKeyMatchKey) || plannedAddedForeignKeyIds.Contains(sourceForeignKeyId)))
            {
                return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' is not present in live and not planned as AddForeignKey.");
            }
        }

        var liveDependentForeignKeys = GetOrderedTargetTableForeignKeys(liveForeignKeysByTargetTableId, liveTableId);
        foreach (var liveForeignKey in liveDependentForeignKeys)
        {
            var liveForeignKeyId = liveForeignKey.Id;
            var liveForeignKeyMembers = GetOrderedForeignKeyMembers(liveForeignKeyColumnsByForeignKeyId, liveForeignKeyId);
            if (liveForeignKeyMembers.Count == 0)
            {
                return (false, $"{difference.DisplayName}: live dependent foreign key '{liveForeignKeyId}' has no member rows.");
            }

            var liveTargetColumnIds = liveForeignKeyMembers
                .Select(row => row.RelationshipIds["TargetColumnId"])
                .ToList();
            var referencesLivePrimaryKey = liveTargetColumnIds.SequenceEqual(livePkColumnIds);
            var intersectsLivePrimaryKey = liveTargetColumnIds.Any(row => livePkColumnIds.Contains(row, StringComparer.Ordinal));
            if (!referencesLivePrimaryKey && intersectsLivePrimaryKey)
            {
                return (false, $"{difference.DisplayName}: live dependent foreign key '{liveForeignKeyId}' has unsupported target-column shape for ReplacePrimaryKey choreography.");
            }

            if (!referencesLivePrimaryKey)
            {
                continue;
            }

            var liveForeignKeyMatchKey = BuildForeignKeyMatchKey(liveForeignKey);
            if (!(sourceForeignKeyMatchKeys.Contains(liveForeignKeyMatchKey) || plannedDroppedForeignKeyIds.Contains(liveForeignKeyId)))
            {
                return (false, $"{difference.DisplayName}: live dependent foreign key '{liveForeignKeyId}' has no source equivalent and is not planned as DropForeignKey.");
            }
        }

        return (true, string.Empty);
    }

    private static List<GenericRecord> GetOrderedPrimaryKeyMembers(
        IReadOnlyDictionary<string, List<GenericRecord>> membersByPrimaryKeyId,
        string primaryKeyId)
    {
        if (!membersByPrimaryKeyId.TryGetValue(primaryKeyId, out var members))
        {
            return [];
        }

        return members
            .OrderBy(row => ParseOrdinal(GetValue(row, "Ordinal")))
            .ThenBy(row => row.Id, StringComparer.Ordinal)
            .ToList();
    }

    private static List<GenericRecord> GetOrderedTargetTableForeignKeys(
        IReadOnlyDictionary<string, List<GenericRecord>> foreignKeysByTargetTableId,
        string targetTableId)
    {
        if (!foreignKeysByTargetTableId.TryGetValue(targetTableId, out var foreignKeys))
        {
            return [];
        }

        return foreignKeys
            .OrderBy(row => row.Id, StringComparer.Ordinal)
            .ToList();
    }

    private static (bool Executable, string Reason) AssessForeignKeyDifference(
        MetaSqlDifference difference,
        IReadOnlyDictionary<string, GenericRecord> sourceForeignKeysById,
        IReadOnlyDictionary<string, GenericRecord> liveForeignKeysById,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceForeignKeyColumnsByForeignKeyId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveForeignKeyColumnsByForeignKeyId,
        IReadOnlyDictionary<string, GenericRecord> sourceTablesById,
        IReadOnlyDictionary<string, GenericRecord> liveTablesById,
        IReadOnlyDictionary<string, GenericRecord> sourceColumnsById,
        IReadOnlyDictionary<string, GenericRecord> liveColumnsById,
        IReadOnlySet<string> plannedAddedTableIds,
        IReadOnlySet<string> plannedAddedColumnIds)
    {
        var sourceId = difference.SourceId ?? string.Empty;
        var liveId = difference.LiveId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sourceId) || string.IsNullOrWhiteSpace(liveId))
        {
            return (false, $"{difference.DisplayName}: missing SourceId/LiveId for changed foreign key.");
        }

        if (!sourceForeignKeysById.TryGetValue(sourceId, out var sourceForeignKey) ||
            !liveForeignKeysById.TryGetValue(liveId, out var liveForeignKey))
        {
            return (false, $"{difference.DisplayName}: changed foreign key row is missing in source or live workspace.");
        }

        var sourceSourceTableId = sourceForeignKey.RelationshipIds["SourceTableId"];
        var liveSourceTableId = liveForeignKey.RelationshipIds["SourceTableId"];
        if (!string.Equals(sourceSourceTableId, liveSourceTableId, StringComparison.Ordinal))
        {
            return (false, $"{difference.DisplayName}: ReplaceForeignKey requires the same source table scope in source and live.");
        }

        var sourceName = GetValue(sourceForeignKey, "Name");
        var liveName = GetValue(liveForeignKey, "Name");
        if (!string.Equals(sourceName, liveName, StringComparison.Ordinal))
        {
            return (false, $"{difference.DisplayName}: ReplaceForeignKey requires identical foreign key names.");
        }

        var sourceMembers = GetOrderedForeignKeyMembers(sourceForeignKeyColumnsByForeignKeyId, sourceId);
        var liveMembers = GetOrderedForeignKeyMembers(liveForeignKeyColumnsByForeignKeyId, liveId);
        if (sourceMembers.Count == 0)
        {
            return (false, $"{difference.DisplayName}: source foreign key has no member rows.");
        }

        if (liveMembers.Count == 0)
        {
            return (false, $"{difference.DisplayName}: live foreign key has no member rows.");
        }

        if (sourceMembers.Count != liveMembers.Count)
        {
            return (false, $"{difference.DisplayName}: ReplaceForeignKey requires matching member counts in this slice.");
        }

        var sourceTargetTableId = sourceForeignKey.RelationshipIds["TargetTableId"];
        var requiredSourceTableIds = new[] { sourceSourceTableId, sourceTargetTableId };
        foreach (var requiredTableId in requiredSourceTableIds)
        {
            if (sourceTablesById.ContainsKey(requiredTableId))
            {
                continue;
            }

            return (false, $"{difference.DisplayName}: source foreign key references missing table '{requiredTableId}'.");
        }

        foreach (var requiredTableId in requiredSourceTableIds)
        {
            if (liveTablesById.ContainsKey(requiredTableId) || plannedAddedTableIds.Contains(requiredTableId))
            {
                continue;
            }

            return (false, $"{difference.DisplayName}: referenced table '{requiredTableId}' is not present in live and not planned as AddTable.");
        }

        foreach (var sourceMember in sourceMembers)
        {
            var sourceColumnId = sourceMember.RelationshipIds["SourceColumnId"];
            var targetColumnId = sourceMember.RelationshipIds["TargetColumnId"];
            if (!sourceColumnsById.TryGetValue(sourceColumnId, out var sourceColumn))
            {
                return (false, $"{difference.DisplayName}: source foreign key references missing source column '{sourceColumnId}'.");
            }

            if (!sourceColumnsById.TryGetValue(targetColumnId, out var targetColumn))
            {
                return (false, $"{difference.DisplayName}: source foreign key references missing target column '{targetColumnId}'.");
            }

            if (!string.Equals(sourceColumn.RelationshipIds["TableId"], sourceSourceTableId, StringComparison.Ordinal))
            {
                return (false, $"{difference.DisplayName}: source member column '{sourceColumnId}' is outside the source table scope.");
            }

            if (!string.Equals(targetColumn.RelationshipIds["TableId"], sourceTargetTableId, StringComparison.Ordinal))
            {
                return (false, $"{difference.DisplayName}: target member column '{targetColumnId}' is outside the target table scope.");
            }

            if (!(liveColumnsById.ContainsKey(sourceColumnId) || plannedAddedColumnIds.Contains(sourceColumnId)))
            {
                return (false, $"{difference.DisplayName}: source member column '{sourceColumnId}' is not present in live and not planned as AddTableColumn.");
            }

            if (!(liveColumnsById.ContainsKey(targetColumnId) || plannedAddedColumnIds.Contains(targetColumnId)))
            {
                return (false, $"{difference.DisplayName}: target member column '{targetColumnId}' is not present in live and not planned as AddTableColumn.");
            }
        }

        foreach (var liveMember in liveMembers)
        {
            var liveSourceColumnId = liveMember.RelationshipIds["SourceColumnId"];
            if (!liveColumnsById.TryGetValue(liveSourceColumnId, out var liveSourceColumn))
            {
                return (false, $"{difference.DisplayName}: live foreign key source column '{liveSourceColumnId}' is missing.");
            }

            if (!string.Equals(liveSourceColumn.RelationshipIds["TableId"], liveSourceTableId, StringComparison.Ordinal))
            {
                return (false, $"{difference.DisplayName}: live source member column '{liveSourceColumnId}' is outside the live source table scope.");
            }
        }

        return (true, string.Empty);
    }

    private static List<GenericRecord> GetOrderedForeignKeyMembers(
        IReadOnlyDictionary<string, List<GenericRecord>> membersByForeignKeyId,
        string foreignKeyId)
    {
        if (!membersByForeignKeyId.TryGetValue(foreignKeyId, out var members))
        {
            return [];
        }

        return members
            .OrderBy(row => ParseOrdinal(GetValue(row, "Ordinal")))
            .ThenBy(row => row.Id, StringComparer.Ordinal)
            .ToList();
    }

    private static (bool Executable, string Reason) AssessIndexDifference(
        MetaSqlDifference difference,
        IReadOnlyDictionary<string, GenericRecord> sourceIndexesById,
        IReadOnlyDictionary<string, GenericRecord> liveIndexesById,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceIndexColumnsByIndexId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveIndexColumnsByIndexId,
        IReadOnlyDictionary<string, GenericRecord> sourceTablesById,
        IReadOnlyDictionary<string, GenericRecord> liveTablesById,
        IReadOnlyDictionary<string, GenericRecord> sourceColumnsById,
        IReadOnlyDictionary<string, GenericRecord> liveColumnsById,
        IReadOnlySet<string> plannedAddedTableIds,
        IReadOnlySet<string> plannedAddedColumnIds)
    {
        var sourceId = difference.SourceId ?? string.Empty;
        var liveId = difference.LiveId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sourceId) || string.IsNullOrWhiteSpace(liveId))
        {
            return (false, $"{difference.DisplayName}: missing SourceId/LiveId for changed index.");
        }

        if (!sourceIndexesById.TryGetValue(sourceId, out var sourceIndex) ||
            !liveIndexesById.TryGetValue(liveId, out var liveIndex))
        {
            return (false, $"{difference.DisplayName}: changed index row is missing in source or live workspace.");
        }

        var sourceTableId = sourceIndex.RelationshipIds["TableId"];
        var liveTableId = liveIndex.RelationshipIds["TableId"];
        if (!string.Equals(sourceTableId, liveTableId, StringComparison.Ordinal))
        {
            return (false, $"{difference.DisplayName}: ReplaceIndex requires the same table scope in source and live.");
        }

        var sourceName = GetValue(sourceIndex, "Name");
        var liveName = GetValue(liveIndex, "Name");
        if (!string.Equals(sourceName, liveName, StringComparison.Ordinal))
        {
            return (false, $"{difference.DisplayName}: ReplaceIndex requires identical index names.");
        }

        var sourceIsClusteredRaw = GetValue(sourceIndex, "IsClustered");
        var liveIsClusteredRaw = GetValue(liveIndex, "IsClustered");
        if (!TryParseOptionalBoolean(sourceIsClusteredRaw, out var sourceIsClustered))
        {
            return (false, $"{difference.DisplayName}: source index has malformed IsClustered value '{sourceIsClusteredRaw}'.");
        }

        if (!TryParseOptionalBoolean(liveIsClusteredRaw, out var liveIsClustered))
        {
            return (false, $"{difference.DisplayName}: live index has malformed IsClustered value '{liveIsClusteredRaw}'.");
        }

        if (sourceIsClustered || liveIsClustered)
        {
            return (false, $"{difference.DisplayName}: clustered index replacement is blocked in this slice.");
        }

        var sourceIsUniqueRaw = GetValue(sourceIndex, "IsUnique");
        var liveIsUniqueRaw = GetValue(liveIndex, "IsUnique");
        if (!TryParseOptionalBoolean(sourceIsUniqueRaw, out _))
        {
            return (false, $"{difference.DisplayName}: source index has malformed IsUnique value '{sourceIsUniqueRaw}'.");
        }

        if (!TryParseOptionalBoolean(liveIsUniqueRaw, out _))
        {
            return (false, $"{difference.DisplayName}: live index has malformed IsUnique value '{liveIsUniqueRaw}'.");
        }

        if (!sourceTablesById.ContainsKey(sourceTableId))
        {
            return (false, $"{difference.DisplayName}: source index references missing table '{sourceTableId}'.");
        }

        if (!(liveTablesById.ContainsKey(sourceTableId) || plannedAddedTableIds.Contains(sourceTableId)))
        {
            return (false, $"{difference.DisplayName}: table '{sourceTableId}' is not present in live and not planned as AddTable.");
        }

        var sourceMembers = GetOrderedIndexMembers(sourceIndexColumnsByIndexId, sourceId);
        var liveMembers = GetOrderedIndexMembers(liveIndexColumnsByIndexId, liveId);
        if (sourceMembers.Count == 0)
        {
            return (false, $"{difference.DisplayName}: source index has no member rows.");
        }

        if (liveMembers.Count == 0)
        {
            return (false, $"{difference.DisplayName}: live index has no member rows.");
        }

        var sourceHasKeyMembers = false;
        foreach (var sourceMember in sourceMembers)
        {
            var sourceColumnId = sourceMember.RelationshipIds["TableColumnId"];
            if (!sourceColumnsById.TryGetValue(sourceColumnId, out var sourceColumn))
            {
                return (false, $"{difference.DisplayName}: source index references missing table column '{sourceColumnId}'.");
            }

            if (!string.Equals(sourceColumn.RelationshipIds["TableId"], sourceTableId, StringComparison.Ordinal))
            {
                return (false, $"{difference.DisplayName}: source index member column '{sourceColumnId}' is outside the source table scope.");
            }

            if (!(liveColumnsById.ContainsKey(sourceColumnId) || plannedAddedColumnIds.Contains(sourceColumnId)))
            {
                return (false, $"{difference.DisplayName}: source index member column '{sourceColumnId}' is not present in live and not planned as AddTableColumn.");
            }

            var sourceMemberIsIncludedRaw = GetValue(sourceMember, "IsIncluded");
            if (!TryParseOptionalBoolean(sourceMemberIsIncludedRaw, out var sourceMemberIsIncluded))
            {
                return (false, $"{difference.DisplayName}: source index member '{sourceMember.Id}' has malformed IsIncluded value '{sourceMemberIsIncludedRaw}'.");
            }

            var sourceMemberIsDescendingRaw = GetValue(sourceMember, "IsDescending");
            if (!TryParseOptionalBoolean(sourceMemberIsDescendingRaw, out var sourceMemberIsDescending))
            {
                return (false, $"{difference.DisplayName}: source index member '{sourceMember.Id}' has malformed IsDescending value '{sourceMemberIsDescendingRaw}'.");
            }

            if (sourceMemberIsIncluded && sourceMemberIsDescending)
            {
                return (false, $"{difference.DisplayName}: source index member '{sourceMember.Id}' cannot be both included and descending.");
            }

            if (!sourceMemberIsIncluded)
            {
                sourceHasKeyMembers = true;
            }
        }

        if (!sourceHasKeyMembers)
        {
            return (false, $"{difference.DisplayName}: source index has no key members.");
        }

        foreach (var liveMember in liveMembers)
        {
            var liveColumnId = liveMember.RelationshipIds["TableColumnId"];
            if (!liveColumnsById.TryGetValue(liveColumnId, out var liveColumn))
            {
                return (false, $"{difference.DisplayName}: live index references missing table column '{liveColumnId}'.");
            }

            if (!string.Equals(liveColumn.RelationshipIds["TableId"], liveTableId, StringComparison.Ordinal))
            {
                return (false, $"{difference.DisplayName}: live index member column '{liveColumnId}' is outside the live table scope.");
            }

            var liveMemberIsIncludedRaw = GetValue(liveMember, "IsIncluded");
            if (!TryParseOptionalBoolean(liveMemberIsIncludedRaw, out var liveMemberIsIncluded))
            {
                return (false, $"{difference.DisplayName}: live index member '{liveMember.Id}' has malformed IsIncluded value '{liveMemberIsIncludedRaw}'.");
            }

            var liveMemberIsDescendingRaw = GetValue(liveMember, "IsDescending");
            if (!TryParseOptionalBoolean(liveMemberIsDescendingRaw, out var liveMemberIsDescending))
            {
                return (false, $"{difference.DisplayName}: live index member '{liveMember.Id}' has malformed IsDescending value '{liveMemberIsDescendingRaw}'.");
            }

            if (liveMemberIsIncluded && liveMemberIsDescending)
            {
                return (false, $"{difference.DisplayName}: live index member '{liveMember.Id}' cannot be both included and descending.");
            }
        }

        return (true, string.Empty);
    }

    private static List<GenericRecord> GetOrderedIndexMembers(
        IReadOnlyDictionary<string, List<GenericRecord>> membersByIndexId,
        string indexId)
    {
        if (!membersByIndexId.TryGetValue(indexId, out var members))
        {
            return [];
        }

        return members
            .OrderBy(row => ParseOrdinal(GetValue(row, "Ordinal")))
            .ThenBy(row => row.Id, StringComparer.Ordinal)
            .ToList();
    }

    private static (bool Executable, string Reason) AssessTableColumnDifference(
        MetaSqlDifference difference,
        IReadOnlyDictionary<string, GenericRecord> sourceColumnsById,
        IReadOnlyDictionary<string, GenericRecord> liveColumnsById,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceColumnDetailsByColumnId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveColumnDetailsByColumnId,
        IReadOnlyDictionary<string, string> blockerByColumnPairKey)
    {
        var sourceId = difference.SourceId ?? string.Empty;
        var liveId = difference.LiveId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sourceId) || string.IsNullOrWhiteSpace(liveId))
        {
            return (false, $"{difference.DisplayName}: missing SourceId/LiveId for changed column.");
        }

        if (!sourceColumnsById.TryGetValue(sourceId, out var sourceColumn) ||
            !liveColumnsById.TryGetValue(liveId, out var liveColumn))
        {
            return (false, $"{difference.DisplayName}: changed column row is missing in source or live workspace.");
        }

        var blockerKey = BuildColumnBlockerKey(sourceId, liveId);
        if (blockerByColumnPairKey.TryGetValue(blockerKey, out var blockerReason))
        {
            return (false, blockerReason);
        }

        var changedAspects = GetChangedColumnAspects(
            sourceColumn,
            liveColumn,
            sourceColumnDetailsByColumnId,
            liveColumnDetailsByColumnId);
        if (changedAspects.Count == 0)
        {
            return (false, $"{difference.DisplayName}: no executable changed aspects were detected.");
        }

        var unsupportedAspects = changedAspects
            .Where(row => !ExecutableColumnAspects.Contains(row))
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();
        if (unsupportedAspects.Count > 0)
        {
            return (false, $"{difference.DisplayName}: unsupported column aspect change(s): {string.Join(", ", unsupportedAspects)}.");
        }

        var sourceTypeId = GetValue(sourceColumn, "MetaDataTypeId");
        var liveTypeId = GetValue(liveColumn, "MetaDataTypeId");
        if (!IsSupportedSqlServerType(sourceTypeId) || !IsSupportedSqlServerType(liveTypeId))
        {
            return (false, $"{difference.DisplayName}: AlterTableColumn supports only sqlserver:type:* MetaDataTypeId values.");
        }

        var typeShapeChanged = changedAspects.Contains("MetaDataTypeId", StringComparer.Ordinal) ||
                               changedAspects.Contains("MetaDataTypeDetail", StringComparer.Ordinal);
        if (typeShapeChanged)
        {
            var sourceTypeName = GetSqlServerTypeName(sourceTypeId);
            var liveTypeName = GetSqlServerTypeName(liveTypeId);
            if (!string.Equals(sourceTypeName, liveTypeName, StringComparison.OrdinalIgnoreCase))
            {
                return (false, $"{difference.DisplayName}: type family transitions are blocked in this slice ({liveTypeName} -> {sourceTypeName}).");
            }

            if (!LengthBasedSqlServerTypeNames.Contains(sourceTypeName))
            {
                return (false, $"{difference.DisplayName}: only length-based sqlserver types are executable for type-shape changes in this slice.");
            }

            var sourceDetailMap = GetDetailMap(sourceColumnDetailsByColumnId, sourceColumn.Id);
            var liveDetailMap = GetDetailMap(liveColumnDetailsByColumnId, liveColumn.Id);
            if (!HasOnlyLengthDetailChange(sourceDetailMap, liveDetailMap))
            {
                return (false, $"{difference.DisplayName}: only Length detail changes are executable for type-shape changes in this slice.");
            }
        }

        return (true, string.Empty);
    }

    private static List<string> GetChangedColumnAspects(
        GenericRecord sourceColumn,
        GenericRecord liveColumn,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceColumnDetailsByColumnId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveColumnDetailsByColumnId)
    {
        var changedAspects = new List<string>();
        AddIfDifferent(changedAspects, "Name", GetValue(sourceColumn, "Name"), GetValue(liveColumn, "Name"));
        AddIfDifferent(changedAspects, "Ordinal", GetValue(sourceColumn, "Ordinal"), GetValue(liveColumn, "Ordinal"));
        AddIfDifferent(changedAspects, "MetaDataTypeId", GetValue(sourceColumn, "MetaDataTypeId"), GetValue(liveColumn, "MetaDataTypeId"));
        AddIfDifferent(changedAspects, "IsNullable", GetValue(sourceColumn, "IsNullable"), GetValue(liveColumn, "IsNullable"));
        AddIfDifferent(changedAspects, "IsIdentity", GetValue(sourceColumn, "IsIdentity"), GetValue(liveColumn, "IsIdentity"));
        AddIfDifferent(changedAspects, "IdentitySeed", GetValue(sourceColumn, "IdentitySeed"), GetValue(liveColumn, "IdentitySeed"));
        AddIfDifferent(changedAspects, "IdentityIncrement", GetValue(sourceColumn, "IdentityIncrement"), GetValue(liveColumn, "IdentityIncrement"));
        AddIfDifferent(changedAspects, "ExpressionSql", GetValue(sourceColumn, "ExpressionSql"), GetValue(liveColumn, "ExpressionSql"));

        var sourceDetails = GetDetailPairs(sourceColumnDetailsByColumnId, sourceColumn.Id);
        var liveDetails = GetDetailPairs(liveColumnDetailsByColumnId, liveColumn.Id);
        if (!sourceDetails.SequenceEqual(liveDetails))
        {
            changedAspects.Add("MetaDataTypeDetail");
        }

        return changedAspects;
    }

    private static List<string> GetDetailPairs(
        IReadOnlyDictionary<string, List<GenericRecord>> detailsByColumnId,
        string columnId)
    {
        if (!detailsByColumnId.TryGetValue(columnId, out var rows))
        {
            return [];
        }

        return rows
            .Select(row => $"{GetValue(row, "Name")}={GetValue(row, "Value")}")
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();
    }

    private static Dictionary<string, string> GetDetailMap(
        IReadOnlyDictionary<string, List<GenericRecord>> detailsByColumnId,
        string columnId)
    {
        if (!detailsByColumnId.TryGetValue(columnId, out var rows))
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        return rows.ToDictionary(
            row => GetValue(row, "Name"),
            row => GetValue(row, "Value"),
            StringComparer.OrdinalIgnoreCase);
    }

    private static bool HasOnlyLengthDetailChange(
        IReadOnlyDictionary<string, string> sourceDetailMap,
        IReadOnlyDictionary<string, string> liveDetailMap)
    {
        var detailNames = sourceDetailMap.Keys
            .Concat(liveDetailMap.Keys)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(row => row, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var detailName in detailNames)
        {
            var sourceValue = sourceDetailMap.TryGetValue(detailName, out var sourceFoundValue) ? sourceFoundValue : string.Empty;
            var liveValue = liveDetailMap.TryGetValue(detailName, out var liveFoundValue) ? liveFoundValue : string.Empty;
            if (string.Equals(sourceValue, liveValue, StringComparison.Ordinal))
            {
                continue;
            }

            if (!string.Equals(detailName, "Length", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    private static Dictionary<string, string> BuildColumnBlockerLookup(
        IReadOnlyList<MetaSqlDifferenceBlocker>? blockers)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        if (blockers is null)
        {
            return result;
        }

        foreach (var blocker in blockers)
        {
            if (blocker.Difference.ObjectKind != MetaSqlObjectKind.TableColumn)
            {
                continue;
            }

            var sourceId = blocker.Difference.SourceId ?? string.Empty;
            var liveId = blocker.Difference.LiveId ?? string.Empty;
            if (string.IsNullOrWhiteSpace(sourceId) || string.IsNullOrWhiteSpace(liveId))
            {
                continue;
            }

            result[BuildColumnBlockerKey(sourceId, liveId)] = blocker.Reason;
        }

        return result;
    }

    private static string BuildColumnBlockerKey(string sourceId, string liveId)
    {
        return sourceId + "|" + liveId;
    }

    private static bool IsSupportedSqlServerType(string metaDataTypeId)
    {
        if (string.IsNullOrWhiteSpace(metaDataTypeId))
        {
            return false;
        }

        foreach (var prefix in SupportedSqlServerColumnChangePrefixes)
        {
            if (metaDataTypeId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string GetSqlServerTypeName(string metaDataTypeId)
    {
        const string prefix = "sqlserver:type:";
        if (string.IsNullOrWhiteSpace(metaDataTypeId) ||
            !metaDataTypeId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        return metaDataTypeId[prefix.Length..];
    }

    private static bool TryParseOptionalBoolean(string value, out bool parsedValue)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            parsedValue = false;
            return true;
        }

        if (string.Equals(value.Trim(), "true", StringComparison.OrdinalIgnoreCase))
        {
            parsedValue = true;
            return true;
        }

        if (string.Equals(value.Trim(), "false", StringComparison.OrdinalIgnoreCase))
        {
            parsedValue = false;
            return true;
        }

        parsedValue = false;
        return false;
    }

    private static int ParseOrdinal(string value)
    {
        return int.TryParse(value, out var ordinal) ? ordinal : int.MaxValue;
    }

    private static void AddIfDifferent(List<string> changedAspects, string aspectName, string left, string right)
    {
        if (!string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.Ordinal))
        {
            changedAspects.Add(aspectName);
        }
    }

    private static string BuildSummary(MetaSqlDifference difference)
    {
        var scope = string.IsNullOrWhiteSpace(difference.ScopeDisplayName)
            ? string.Empty
            : difference.ScopeDisplayName + ".";
        return $"{difference.ObjectKind}:{scope}{difference.DisplayName}";
    }

    private static string GetValue(GenericRecord record, string propertyName)
    {
        return record.Values.TryGetValue(propertyName, out var value) ? value : string.Empty;
    }

    private static Dictionary<string, GenericRecord> GetRecordIndex(Workspace workspace, string entityName)
    {
        return workspace.Instance.GetOrCreateEntityRecords(entityName)
            .ToDictionary(row => row.Id, StringComparer.Ordinal);
    }

    private static Dictionary<string, List<GenericRecord>> GetGroupedRecords(Workspace workspace, string entityName, string relationshipName)
    {
        return workspace.Instance.GetOrCreateEntityRecords(entityName)
            .GroupBy(row => row.RelationshipIds[relationshipName], StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(row => row.Id, StringComparer.Ordinal).ToList(),
                StringComparer.Ordinal);
    }

    private static string RequireValue(string? value, string fieldName, MetaSqlDifference difference)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Difference '{difference.ObjectKind}:{difference.DifferenceKind}:{difference.DisplayName}' does not provide required field '{fieldName}'.");
        }

        return value;
    }
}
