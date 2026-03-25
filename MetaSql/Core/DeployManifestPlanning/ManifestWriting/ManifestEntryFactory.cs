using MetaSqlDeployManifest;

namespace MetaSql;

/// <summary>
/// Centralized creation of executable manifest rows.
/// </summary>
internal sealed class ManifestEntryFactory
{
    public int AddEntry(MetaSqlDeployManifestModel model, DeployManifest root, MetaSqlDifference difference)
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
                return 0;
        }
    }

    public int AddSchemaEntry(MetaSqlDeployManifestModel model, DeployManifest root, string sourceSchemaId)
    {
        if (string.IsNullOrWhiteSpace(sourceSchemaId))
        {
            throw new InvalidOperationException("AddSchema requires SourceSchemaId.");
        }

        model.AddSchemaList.Add(new AddSchema
        {
            Id = $"AddSchema:{sourceSchemaId}",
            SourceSchemaId = sourceSchemaId,
            DeployManifestId = root.Id,
            DeployManifest = root,
        });
        return 1;
    }

    public int DropEntry(MetaSqlDeployManifestModel model, DeployManifest root, MetaSqlDifference difference)
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
                return 0;
        }
    }

    public int TruncateEntry(MetaSqlDeployManifestModel model, DeployManifest root, MetaSqlDifference difference)
    {
        var sourceId = RequireValue(difference.SourceId, "SourceId", difference);
        var liveId = RequireValue(difference.LiveId, "LiveId", difference);
        model.TruncateTableColumnDataList.Add(new TruncateTableColumnData
        {
            Id = $"TruncateTableColumnData:{sourceId}:{liveId}",
            SourceTableColumnId = sourceId,
            LiveTableColumnId = liveId,
            DeployManifestId = root.Id,
            DeployManifest = root,
        });
        return 1;
    }

    public int AlterEntry(MetaSqlDeployManifestModel model, DeployManifest root, MetaSqlDifference difference)
    {
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

    public int ReplaceEntry(MetaSqlDeployManifestModel model, DeployManifest root, MetaSqlDifference difference)
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
                return 0;
        }
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
