using MetaSqlDeployManifest;

namespace MetaSql;

/// <summary>
/// Centralized creation of blocking manifest rows.
/// </summary>
internal sealed class ManifestBlockFactory
{
    public int BlockEntry(
        MetaSqlDeployManifestModel model,
        DeployManifest root,
        MetaSqlDifference difference,
        string? reason = null)
    {
        var summary = BuildSummary(difference);
        var sourceId = ResolveBlockSourceId(difference);
        var liveId = ResolveBlockLiveId(difference);
        var blockerReason = string.IsNullOrWhiteSpace(reason) ? summary : reason;
        switch (difference.ObjectKind)
        {
            case MetaSqlObjectKind.Table:
                model.BlockTableDifferenceList.Add(new BlockTableDifference
                {
                    Id = $"BlockTableDifference:{sourceId}:{liveId}",
                    SourceTableId = sourceId,
                    LiveTableId = liveId,
                    DifferenceSummary = blockerReason,
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.TableColumn:
                model.BlockTableColumnDifferenceList.Add(new BlockTableColumnDifference
                {
                    Id = $"BlockTableColumnDifference:{sourceId}:{liveId}",
                    SourceTableColumnId = sourceId,
                    LiveTableColumnId = liveId,
                    DifferenceSummary = blockerReason,
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.PrimaryKey:
                model.BlockPrimaryKeyDifferenceList.Add(new BlockPrimaryKeyDifference
                {
                    Id = $"BlockPrimaryKeyDifference:{sourceId}:{liveId}",
                    SourcePrimaryKeyId = sourceId,
                    LivePrimaryKeyId = liveId,
                    DifferenceSummary = blockerReason,
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.ForeignKey:
                model.BlockForeignKeyDifferenceList.Add(new BlockForeignKeyDifference
                {
                    Id = $"BlockForeignKeyDifference:{sourceId}:{liveId}",
                    SourceForeignKeyId = sourceId,
                    LiveForeignKeyId = liveId,
                    DifferenceSummary = blockerReason,
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            case MetaSqlObjectKind.Index:
                model.BlockIndexDifferenceList.Add(new BlockIndexDifference
                {
                    Id = $"BlockIndexDifference:{sourceId}:{liveId}",
                    SourceIndexId = sourceId,
                    LiveIndexId = liveId,
                    DifferenceSummary = blockerReason,
                    DeployManifestId = root.Id,
                    DeployManifest = root,
                });
                return 1;
            default:
                throw new InvalidOperationException(
                    $"Unsupported difference object kind '{difference.ObjectKind}'.");
        }
    }

    public int AddColumnDependencyBlock(
        MetaSqlDeployManifestModel model,
        DeployManifest root,
        MetaSqlDeployManifest.AlterTableColumn alterTableColumn,
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
            Id = $"BlockTableColumnDifference:{alterTableColumn.SourceTableColumnId}:{alterTableColumn.LiveTableColumnId}:Dependent:{model.BlockTableColumnDifferenceList.Count + 1}",
            SourceTableColumnId = alterTableColumn.SourceTableColumnId,
            LiveTableColumnId = alterTableColumn.LiveTableColumnId,
            DifferenceSummary = reason,
            DeployManifestId = root.Id,
            DeployManifest = root,
        });
        return 1;
    }

    public int AddPrimaryKeyDependencyBlock(
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
            Id = $"BlockPrimaryKeyDifference:{replacePrimaryKey.SourcePrimaryKeyId}:{replacePrimaryKey.LivePrimaryKeyId}:Dependent:{model.BlockPrimaryKeyDifferenceList.Count + 1}",
            SourcePrimaryKeyId = replacePrimaryKey.SourcePrimaryKeyId,
            LivePrimaryKeyId = replacePrimaryKey.LivePrimaryKeyId,
            DifferenceSummary = reason,
            DeployManifestId = root.Id,
            DeployManifest = root,
        });
        return 1;
    }

    private static string BuildSummary(MetaSqlDifference difference)
    {
        var scope = string.IsNullOrWhiteSpace(difference.ScopeDisplayName)
            ? string.Empty
            : difference.ScopeDisplayName + ".";
        return $"{difference.ObjectKind}:{scope}{difference.DisplayName}";
    }

    private static string ResolveBlockSourceId(MetaSqlDifference difference)
    {
        if (!string.IsNullOrWhiteSpace(difference.SourceId))
        {
            return difference.SourceId!;
        }

        if (!string.IsNullOrWhiteSpace(difference.LiveId))
        {
            return difference.LiveId!;
        }

        throw new InvalidOperationException(
            $"Difference '{difference.ObjectKind}:{difference.DifferenceKind}:{difference.DisplayName}' does not provide SourceId or LiveId.");
    }

    private static string ResolveBlockLiveId(MetaSqlDifference difference)
    {
        if (!string.IsNullOrWhiteSpace(difference.LiveId))
        {
            return difference.LiveId!;
        }

        if (!string.IsNullOrWhiteSpace(difference.SourceId))
        {
            return difference.SourceId!;
        }

        throw new InvalidOperationException(
            $"Difference '{difference.ObjectKind}:{difference.DifferenceKind}:{difference.DisplayName}' does not provide SourceId or LiveId.");
    }
}
