using Meta.Core.Domain;
using MetaSqlDeployManifest;

namespace MetaSql;

public sealed record MetaSqlDeployManifestBuildResult
{
    public required MetaSqlDeployManifestModel ManifestModel { get; init; }
    public required int AddCount { get; init; }
    public required int DropCount { get; init; }
    public required int BlockCount { get; init; }
    public bool IsDeployable => BlockCount == 0;
}

public sealed class MetaSqlDeployManifestService
{
    public MetaSqlDeployManifestBuildResult BuildManifest(
        Workspace sourceWorkspace,
        Workspace liveWorkspace,
        IReadOnlyList<MetaSqlDifference> differences,
        string manifestName,
        string? targetDescription)
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
            ManifestVersion = "1.0",
            SourceInstanceFingerprint = MetaSqlInstanceFingerprint.Compute(sourceWorkspace),
            LiveInstanceFingerprint = MetaSqlInstanceFingerprint.Compute(liveWorkspace),
            CreatedUtc = DateTime.UtcNow.ToString("O"),
            TargetDescription = targetDescription ?? string.Empty,
        };
        model.DeployManifestList.Add(root);

        var addCount = 0;
        var dropCount = 0;
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
                    dropCount += DropEntry(model, root, difference);
                    break;
                case MetaSqlDifferenceKind.Different:
                    blockCount += BlockEntry(model, root, difference);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported difference kind '{difference.DifferenceKind}'.");
            }
        }

        return new MetaSqlDeployManifestBuildResult
        {
            ManifestModel = model,
            AddCount = addCount,
            DropCount = dropCount,
            BlockCount = blockCount,
        };
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

    private static int BlockEntry(MetaSqlDeployManifestModel model, DeployManifest root, MetaSqlDifference difference)
    {
        var summary = BuildSummary(difference);
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

    private static string BuildSummary(MetaSqlDifference difference)
    {
        var scope = string.IsNullOrWhiteSpace(difference.ScopeDisplayName)
            ? string.Empty
            : difference.ScopeDisplayName + ".";
        return $"{difference.ObjectKind}:{scope}{difference.DisplayName}";
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
