using Meta.Core.Services;

namespace MetaSql.Tests;

public sealed class MetaSqlDeployManifestServiceTests
{
    [Fact]
    public void BuildManifest_MapsAddDropAndAlterEntries()
    {
        var sourceWorkspace = CreateWorkspace(
            CreateModel(
                includeSourceOnlyColumn: true,
                includeLiveOnlyColumn: false,
                sourceNameLength: 50),
            "source");
        var liveWorkspace = CreateWorkspace(
            CreateModel(
                includeSourceOnlyColumn: false,
                includeLiveOnlyColumn: true,
                sourceNameLength: 100),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=raw:*",
            destructiveApprovals:
            [
                new MetaSqlDestructiveApproval
                {
                    Kind = MetaSqlDestructiveApprovalKind.DataDropColumn,
                    SchemaName = "dbo",
                    TableName = "Customer",
                    ColumnName = "LegacyCode",
                }
            ]);

        Assert.Equal(1, manifest.AddCount);
        Assert.Equal(1, manifest.DropCount);
        Assert.Equal(1, manifest.AlterCount);
        Assert.Equal(0, manifest.ReplaceCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.True(manifest.IsDeployable);

        var root = Assert.Single(manifest.ManifestModel.DeployManifestList);
        Assert.False(string.IsNullOrWhiteSpace(root.SourceInstanceFingerprint));
        Assert.False(string.IsNullOrWhiteSpace(root.LiveInstanceFingerprint));
        Assert.Equal("Scope=raw:*", root.TargetDescription);
        Assert.Single(manifest.ManifestModel.AddTableColumnList);
        Assert.Single(manifest.ManifestModel.DropTableColumnList);
        Assert.Single(manifest.ManifestModel.AlterTableColumnList);
        Assert.Empty(manifest.ManifestModel.BlockTableColumnDifferenceList);
    }

    [Fact]
    public void BuildManifest_WithoutExactDataDropColumnApproval_BlocksLiveOnlyColumnDrop()
    {
        var sourceWorkspace = CreateWorkspace(
            CreateModel(
                includeSourceOnlyColumn: true,
                includeLiveOnlyColumn: false,
                sourceNameLength: 50),
            "source");
        var liveWorkspace = CreateWorkspace(
            CreateModel(
                includeSourceOnlyColumn: false,
                includeLiveOnlyColumn: true,
                sourceNameLength: 100),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=raw:*");

        Assert.Equal(1, manifest.AddCount);
        Assert.Equal(0, manifest.DropCount);
        Assert.Equal(1, manifest.AlterCount);
        Assert.Equal(0, manifest.ReplaceCount);
        Assert.Equal(1, manifest.BlockCount);
        Assert.False(manifest.IsDeployable);

        Assert.Single(manifest.ManifestModel.AddTableColumnList);
        Assert.Empty(manifest.ManifestModel.DropTableColumnList);
        Assert.Single(manifest.ManifestModel.AlterTableColumnList);
        var block = Assert.Single(manifest.ManifestModel.BlockTableColumnDifferenceList);
        Assert.Contains("missing approval DataDropColumn", block.DifferenceSummary, StringComparison.Ordinal);
    }

    [Fact]
    public void BuildManifest_WithExactTruncationApproval_EmitsExplicitTruncateAction_WhenOnlyBlockerIsTruncationRisk()
    {
        var sourceWorkspace = CreateWorkspace(
            CreateModel(
                includeSourceOnlyColumn: false,
                includeLiveOnlyColumn: false,
                sourceNameLength: 50),
            "source");
        var liveWorkspace = CreateWorkspace(
            CreateModel(
                includeSourceOnlyColumn: false,
                includeLiveOnlyColumn: false,
                sourceNameLength: 100),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);
        var changedColumn = Assert.Single(
            differences,
            row => row.ObjectKind == MetaSqlObjectKind.TableColumn &&
                   row.DifferenceKind == MetaSqlDifferenceKind.Different);

        var blockers = new List<MetaSqlDifferenceBlocker>
        {
            new()
            {
                Difference = changedColumn,
                Code = MetaSqlDifferenceBlockerCode.DataTruncationRequired,
                Reason = $"{changedColumn.DisplayName}: source length 50 is smaller than live data currently stored.",
            }
        };

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=raw:*",
            feasibilityBlockers: blockers,
            destructiveApprovals:
            [
                new MetaSqlDestructiveApproval
                {
                    Kind = MetaSqlDestructiveApprovalKind.DataTruncationColumn,
                    SchemaName = "dbo",
                    TableName = "Customer",
                    ColumnName = "CustomerName",
                }
            ]);

        Assert.Equal(1, manifest.AlterCount);
        Assert.Equal(1, manifest.TruncateCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.True(manifest.IsDeployable);
        Assert.Single(manifest.ManifestModel.AlterTableColumnList);
        Assert.Single(manifest.ManifestModel.TruncateTableColumnDataList);
        Assert.Empty(manifest.ManifestModel.BlockTableColumnDifferenceList);
    }

    [Fact]
    public void BuildManifest_TruncationApproval_IsExactScopeOnly_PerColumn()
    {
        var sourceModel = CreateModel(
            includeSourceOnlyColumn: false,
            includeLiveOnlyColumn: false,
            sourceNameLength: 50);
        var liveModel = CreateModel(
            includeSourceOnlyColumn: false,
            includeLiveOnlyColumn: false,
            sourceNameLength: 100);
        AddSharedLengthColumn(sourceModel, columnName: "Alias", ordinal: "3", length: 50);
        AddSharedLengthColumn(liveModel, columnName: "Alias", ordinal: "3", length: 100);

        var sourceWorkspace = CreateWorkspace(sourceModel, "source");
        var liveWorkspace = CreateWorkspace(liveModel, "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);
        var changedColumns = differences
            .Where(row => row.ObjectKind == MetaSqlObjectKind.TableColumn && row.DifferenceKind == MetaSqlDifferenceKind.Different)
            .OrderBy(row => row.DisplayName, StringComparer.Ordinal)
            .ToList();
        Assert.Equal(2, changedColumns.Count);

        var blockers = changedColumns
            .Select(row => new MetaSqlDifferenceBlocker
            {
                Difference = row,
                Code = MetaSqlDifferenceBlockerCode.DataTruncationRequired,
                Reason = $"{row.DisplayName}: source length is smaller than live data currently stored.",
            })
            .ToList();

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=raw:*",
            feasibilityBlockers: blockers,
            destructiveApprovals:
            [
                new MetaSqlDestructiveApproval
                {
                    Kind = MetaSqlDestructiveApprovalKind.DataTruncationColumn,
                    SchemaName = "dbo",
                    TableName = "Customer",
                    ColumnName = "CustomerName",
                }
            ]);

        Assert.Equal(1, manifest.AlterCount);
        Assert.Equal(1, manifest.TruncateCount);
        Assert.Equal(1, manifest.BlockCount);
        Assert.False(manifest.IsDeployable);
        Assert.Single(manifest.ManifestModel.AlterTableColumnList);
        Assert.Single(manifest.ManifestModel.TruncateTableColumnDataList);
        var block = Assert.Single(manifest.ManifestModel.BlockTableColumnDifferenceList);
        Assert.Contains(
            "Missing approval DataTruncationColumn(dbo.Customer.Alias)",
            block.DifferenceSummary,
            StringComparison.Ordinal);
    }

    [Fact]
    public void BuildManifest_MapsReplaceForeignKeyForExecutableSharedForeignKeyDifference()
    {
        var sourceWorkspace = CreateWorkspace(
            CreateForeignKeyModel(sourceTargetTableName: "ParentAlt", includeSourceForeignKeyMember: true),
            "source");
        var liveWorkspace = CreateWorkspace(
            CreateForeignKeyModel(sourceTargetTableName: "Parent", includeSourceForeignKeyMember: true),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(0, manifest.AddCount);
        Assert.Equal(0, manifest.DropCount);
        Assert.Equal(0, manifest.AlterCount);
        Assert.Equal(1, manifest.ReplaceCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.True(manifest.IsDeployable);
        Assert.Single(manifest.ManifestModel.ReplaceForeignKeyList);
        Assert.Empty(manifest.ManifestModel.BlockForeignKeyDifferenceList);
    }

    [Fact]
    public void BuildManifest_BlocksForeignKeyReplacementWhenSourceMembersAreMissing()
    {
        var sourceWorkspace = CreateWorkspace(
            CreateForeignKeyModel(sourceTargetTableName: "ParentAlt", includeSourceForeignKeyMember: false),
            "source");
        var liveWorkspace = CreateWorkspace(
            CreateForeignKeyModel(sourceTargetTableName: "Parent", includeSourceForeignKeyMember: true),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(0, manifest.ReplaceCount);
        Assert.Equal(1, manifest.BlockCount);
        Assert.False(manifest.IsDeployable);
        Assert.Empty(manifest.ManifestModel.ReplaceForeignKeyList);
        var block = Assert.Single(manifest.ManifestModel.BlockForeignKeyDifferenceList);
        Assert.Contains("no member rows", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildManifest_EmitsAddDropForeignKeyWhenForeignKeyNameDiffers()
    {
        var sourceModel = CreateForeignKeyModel(sourceTargetTableName: "ParentAlt", includeSourceForeignKeyMember: true);
        var sourceForeignKey = Assert.Single(sourceModel.ForeignKeyList);
        sourceForeignKey.Name = "FK_Child_Parent_Source";

        var sourceWorkspace = CreateWorkspace(sourceModel, "source");
        var liveWorkspace = CreateWorkspace(
            CreateForeignKeyModel(sourceTargetTableName: "Parent", includeSourceForeignKeyMember: true),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(1, manifest.AddCount);
        Assert.Equal(1, manifest.DropCount);
        Assert.Equal(0, manifest.ReplaceCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.True(manifest.IsDeployable);
        Assert.Single(manifest.ManifestModel.AddForeignKeyList);
        Assert.Single(manifest.ManifestModel.DropForeignKeyList);
        Assert.Empty(manifest.ManifestModel.ReplaceForeignKeyList);
        Assert.Empty(manifest.ManifestModel.BlockForeignKeyDifferenceList);
    }

    [Fact]
    public void BuildManifest_MapsReplaceIndexForExecutableSharedIndexDifference()
    {
        var sourceWorkspace = CreateWorkspace(
            CreateIndexModel(isClustered: false, sourceDescending: true, includeSourceIndexMember: true),
            "source");
        var liveWorkspace = CreateWorkspace(
            CreateIndexModel(isClustered: false, sourceDescending: false, includeSourceIndexMember: true),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(0, manifest.AddCount);
        Assert.Equal(0, manifest.DropCount);
        Assert.Equal(0, manifest.AlterCount);
        Assert.Equal(1, manifest.ReplaceCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.True(manifest.IsDeployable);
        Assert.Single(manifest.ManifestModel.ReplaceIndexList);
        Assert.Empty(manifest.ManifestModel.BlockIndexDifferenceList);
    }

    [Fact]
    public void BuildManifest_BlocksIndexReplacementWhenSourceMembersAreMissing()
    {
        var sourceWorkspace = CreateWorkspace(
            CreateIndexModel(isClustered: false, sourceDescending: true, includeSourceIndexMember: false),
            "source");
        var liveWorkspace = CreateWorkspace(
            CreateIndexModel(isClustered: false, sourceDescending: false, includeSourceIndexMember: true),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(0, manifest.ReplaceCount);
        Assert.Equal(1, manifest.BlockCount);
        Assert.False(manifest.IsDeployable);
        Assert.Empty(manifest.ManifestModel.ReplaceIndexList);
        var block = Assert.Single(manifest.ManifestModel.BlockIndexDifferenceList);
        Assert.Contains("no member rows", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildManifest_BlocksIndexReplacementWhenClustered()
    {
        var sourceWorkspace = CreateWorkspace(
            CreateIndexModel(isClustered: true, sourceDescending: true, includeSourceIndexMember: true),
            "source");
        var liveWorkspace = CreateWorkspace(
            CreateIndexModel(isClustered: true, sourceDescending: false, includeSourceIndexMember: true),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(0, manifest.ReplaceCount);
        Assert.Equal(1, manifest.BlockCount);
        Assert.False(manifest.IsDeployable);
        Assert.Empty(manifest.ManifestModel.ReplaceIndexList);
        var block = Assert.Single(manifest.ManifestModel.BlockIndexDifferenceList);
        Assert.Contains("clustered index replacement is blocked", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildManifest_EmitsAddDropIndexWhenIndexNameDiffers()
    {
        var sourceModel = CreateIndexModel(isClustered: false, sourceDescending: true, includeSourceIndexMember: true);
        var sourceIndex = Assert.Single(sourceModel.IndexList);
        sourceIndex.Name = "IX_PayloadCase_Payload_Source";

        var sourceWorkspace = CreateWorkspace(sourceModel, "source");
        var liveWorkspace = CreateWorkspace(
            CreateIndexModel(isClustered: false, sourceDescending: false, includeSourceIndexMember: true),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(1, manifest.AddCount);
        Assert.Equal(1, manifest.DropCount);
        Assert.Equal(0, manifest.ReplaceCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.True(manifest.IsDeployable);
        Assert.Single(manifest.ManifestModel.AddIndexList);
        Assert.Single(manifest.ManifestModel.DropIndexList);
        Assert.Empty(manifest.ManifestModel.ReplaceIndexList);
        Assert.Empty(manifest.ManifestModel.BlockIndexDifferenceList);
    }

    [Fact]
    public void BuildManifest_AddsExplicitIndexReplacementForAlteredColumnWithDependentIndex()
    {
        var sourceModel = CreateIndexModel(isClustered: false, sourceDescending: false, includeSourceIndexMember: true);
        var sourcePayloadLength = sourceModel.TableColumnDataTypeDetailList.Single(row =>
            row.TableColumnId == "SalesDb.dbo.PayloadCase.Payload" &&
            string.Equals(row.Name, "Length", StringComparison.Ordinal));
        sourcePayloadLength.Value = "200";

        var sourceWorkspace = CreateWorkspace(sourceModel, "source");
        var liveWorkspace = CreateWorkspace(
            CreateIndexModel(isClustered: false, sourceDescending: false, includeSourceIndexMember: true),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(0, manifest.AddCount);
        Assert.Equal(0, manifest.DropCount);
        Assert.Equal(1, manifest.AlterCount);
        Assert.Equal(1, manifest.ReplaceCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.True(manifest.IsDeployable);
        Assert.Single(manifest.ManifestModel.AlterTableColumnList);
        var replaceIndex = Assert.Single(manifest.ManifestModel.ReplaceIndexList);
        Assert.Equal("SalesDb.dbo.PayloadCase.index.IX_PayloadCase_Payload", replaceIndex.SourceIndexId);
        Assert.Equal("SalesDb.dbo.PayloadCase.index.IX_PayloadCase_Payload", replaceIndex.LiveIndexId);
    }

    [Fact]
    public void BuildManifest_BlocksAlteredColumnWhenDependentIndexIsClustered()
    {
        var sourceModel = CreateIndexModel(isClustered: true, sourceDescending: false, includeSourceIndexMember: true);
        var sourcePayloadLength = sourceModel.TableColumnDataTypeDetailList.Single(row =>
            row.TableColumnId == "SalesDb.dbo.PayloadCase.Payload" &&
            string.Equals(row.Name, "Length", StringComparison.Ordinal));
        sourcePayloadLength.Value = "200";

        var sourceWorkspace = CreateWorkspace(sourceModel, "source");
        var liveWorkspace = CreateWorkspace(
            CreateIndexModel(isClustered: true, sourceDescending: false, includeSourceIndexMember: true),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(1, manifest.AlterCount);
        Assert.Equal(0, manifest.ReplaceCount);
        Assert.Equal(1, manifest.BlockCount);
        Assert.False(manifest.IsDeployable);
        Assert.Single(manifest.ManifestModel.AlterTableColumnList);
        Assert.Empty(manifest.ManifestModel.ReplaceIndexList);
        var block = Assert.Single(manifest.ManifestModel.BlockTableColumnDifferenceList);
        Assert.Contains("clustered index replacement is blocked", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildManifest_MapsReplacePrimaryKeyForExecutableSharedPrimaryKeyDifference()
    {
        var sourceWorkspace = CreateWorkspace(
            CreatePrimaryKeyModel(
                isClustered: false,
                includeSecondPrimaryKeyMember: true,
                includePrimaryKeyMembers: true,
                includeDependentForeignKey: false,
                includeDependentForeignKeyMembers: false,
                dependentForeignKeyTargetsAllMembers: false),
            "source");
        var liveWorkspace = CreateWorkspace(
            CreatePrimaryKeyModel(
                isClustered: false,
                includeSecondPrimaryKeyMember: false,
                includePrimaryKeyMembers: true,
                includeDependentForeignKey: false,
                includeDependentForeignKeyMembers: false,
                dependentForeignKeyTargetsAllMembers: false),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(0, manifest.AddCount);
        Assert.Equal(0, manifest.DropCount);
        Assert.Equal(0, manifest.AlterCount);
        Assert.Equal(1, manifest.ReplaceCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.True(manifest.IsDeployable);
        Assert.Single(manifest.ManifestModel.ReplacePrimaryKeyList);
        Assert.Empty(manifest.ManifestModel.BlockPrimaryKeyDifferenceList);
    }

    [Fact]
    public void BuildManifest_BlocksPrimaryKeyReplacementWhenClustered()
    {
        var sourceWorkspace = CreateWorkspace(
            CreatePrimaryKeyModel(
                isClustered: true,
                includeSecondPrimaryKeyMember: true,
                includePrimaryKeyMembers: true,
                includeDependentForeignKey: false,
                includeDependentForeignKeyMembers: false,
                dependentForeignKeyTargetsAllMembers: false),
            "source");
        var liveWorkspace = CreateWorkspace(
            CreatePrimaryKeyModel(
                isClustered: true,
                includeSecondPrimaryKeyMember: false,
                includePrimaryKeyMembers: true,
                includeDependentForeignKey: false,
                includeDependentForeignKeyMembers: false,
                dependentForeignKeyTargetsAllMembers: false),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(0, manifest.ReplaceCount);
        Assert.Equal(1, manifest.BlockCount);
        Assert.False(manifest.IsDeployable);
        Assert.Empty(manifest.ManifestModel.ReplacePrimaryKeyList);
        var block = Assert.Single(manifest.ManifestModel.BlockPrimaryKeyDifferenceList);
        Assert.Contains("clustered primary key replacement is blocked", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildManifest_EmitsAddDropPrimaryKeyWhenPrimaryKeyNameDiffers()
    {
        var sourceModel = CreatePrimaryKeyModel(
            isClustered: false,
            includeSecondPrimaryKeyMember: true,
            includePrimaryKeyMembers: true,
            includeDependentForeignKey: false,
            includeDependentForeignKeyMembers: false,
            dependentForeignKeyTargetsAllMembers: false);
        var sourcePrimaryKey = sourceModel.PrimaryKeyList.Single(row => row.Id == "SalesDb.dbo.ParentPkCase.pk.PK_ParentPkCase");
        sourcePrimaryKey.Name = "PK_ParentPkCase_Source";

        var sourceWorkspace = CreateWorkspace(sourceModel, "source");
        var liveWorkspace = CreateWorkspace(
            CreatePrimaryKeyModel(
                isClustered: false,
                includeSecondPrimaryKeyMember: false,
                includePrimaryKeyMembers: true,
                includeDependentForeignKey: false,
                includeDependentForeignKeyMembers: false,
                dependentForeignKeyTargetsAllMembers: false),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(1, manifest.AddCount);
        Assert.Equal(1, manifest.DropCount);
        Assert.Equal(0, manifest.ReplaceCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.True(manifest.IsDeployable);
        Assert.Single(manifest.ManifestModel.AddPrimaryKeyList);
        Assert.Single(manifest.ManifestModel.DropPrimaryKeyList);
        Assert.Empty(manifest.ManifestModel.ReplacePrimaryKeyList);
        Assert.Empty(manifest.ManifestModel.BlockPrimaryKeyDifferenceList);
    }

    [Fact]
    public void BuildManifest_BlocksPrimaryKeyReplacementWhenSourceMembersAreMissing()
    {
        var sourceWorkspace = CreateWorkspace(
            CreatePrimaryKeyModel(
                isClustered: false,
                includeSecondPrimaryKeyMember: true,
                includePrimaryKeyMembers: false,
                includeDependentForeignKey: false,
                includeDependentForeignKeyMembers: false,
                dependentForeignKeyTargetsAllMembers: false),
            "source");
        var liveWorkspace = CreateWorkspace(
            CreatePrimaryKeyModel(
                isClustered: false,
                includeSecondPrimaryKeyMember: false,
                includePrimaryKeyMembers: true,
                includeDependentForeignKey: false,
                includeDependentForeignKeyMembers: false,
                dependentForeignKeyTargetsAllMembers: false),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(0, manifest.ReplaceCount);
        Assert.Equal(1, manifest.BlockCount);
        Assert.False(manifest.IsDeployable);
        Assert.Empty(manifest.ManifestModel.ReplacePrimaryKeyList);
        var block = Assert.Single(manifest.ManifestModel.BlockPrimaryKeyDifferenceList);
        Assert.Contains("source primary key has no member rows", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildManifest_BlocksPrimaryKeyReplacementWhenDependentForeignKeyChoreographyIsUnsupported()
    {
        var sourceWorkspace = CreateWorkspace(
            CreatePrimaryKeyModel(
                isClustered: false,
                includeSecondPrimaryKeyMember: true,
                includePrimaryKeyMembers: true,
                includeDependentForeignKey: true,
                includeDependentForeignKeyMembers: true,
                dependentForeignKeyTargetsAllMembers: false),
            "source");
        var liveWorkspace = CreateWorkspace(
            CreatePrimaryKeyModel(
                isClustered: false,
                includeSecondPrimaryKeyMember: false,
                includePrimaryKeyMembers: true,
                includeDependentForeignKey: true,
                includeDependentForeignKeyMembers: true,
                dependentForeignKeyTargetsAllMembers: true),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(0, manifest.ReplaceCount);
        Assert.Equal(1, manifest.BlockCount);
        Assert.False(manifest.IsDeployable);
        Assert.Empty(manifest.ManifestModel.ReplacePrimaryKeyList);
        var block = Assert.Single(manifest.ManifestModel.BlockPrimaryKeyDifferenceList);
        Assert.Contains("unsupported target-column shape", block.DifferenceSummary, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildManifest_AddsExplicitForeignKeyReplacementForPrimaryKeyReplacementDependency()
    {
        var sourceModel = CreatePrimaryKeyModel(
            isClustered: false,
            includeSecondPrimaryKeyMember: false,
            includePrimaryKeyMembers: true,
            includeDependentForeignKey: true,
            includeDependentForeignKeyMembers: true,
            dependentForeignKeyTargetsAllMembers: false);
        var sourcePrimaryKey = sourceModel.PrimaryKeyList.Single(row => row.Id == "SalesDb.dbo.ParentPkCase.pk.PK_ParentPkCase");
        var sourcePrimaryKeyMember = sourceModel.PrimaryKeyColumnList.Single(row =>
            row.PrimaryKeyId == sourcePrimaryKey.Id &&
            string.Equals(row.Ordinal, "1", StringComparison.Ordinal));
        sourcePrimaryKeyMember.IsDescending = "true";

        var sourceWorkspace = CreateWorkspace(sourceModel, "source");
        var liveWorkspace = CreateWorkspace(
            CreatePrimaryKeyModel(
                isClustered: false,
                includeSecondPrimaryKeyMember: false,
                includePrimaryKeyMembers: true,
                includeDependentForeignKey: true,
                includeDependentForeignKeyMembers: true,
                dependentForeignKeyTargetsAllMembers: false),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(0, manifest.AddCount);
        Assert.Equal(0, manifest.DropCount);
        Assert.Equal(0, manifest.AlterCount);
        Assert.Equal(2, manifest.ReplaceCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.True(manifest.IsDeployable);
        Assert.Single(manifest.ManifestModel.ReplacePrimaryKeyList);
        var dependentForeignKeyReplace = Assert.Single(manifest.ManifestModel.ReplaceForeignKeyList);
        Assert.Equal("SalesDb.dbo.ChildPkCase.fk.FK_ChildPkCase_ParentPkCase", dependentForeignKeyReplace.SourceForeignKeyId);
        Assert.Equal("SalesDb.dbo.ChildPkCase.fk.FK_ChildPkCase_ParentPkCase", dependentForeignKeyReplace.LiveForeignKeyId);
    }

    [Fact]
    public void BuildManifest_DeduplicatesDependentForeignKeyReplacement_WhenTwoAlteredColumnsAffectSameForeignKey()
    {
        var sourceModel = CreatePrimaryKeyModel(
            isClustered: false,
            includeSecondPrimaryKeyMember: true,
            includePrimaryKeyMembers: true,
            includeDependentForeignKey: true,
            includeDependentForeignKeyMembers: true,
            dependentForeignKeyTargetsAllMembers: true);
        sourceModel.TableColumnList.Single(row => row.Id == "SalesDb.dbo.ChildPkCase.ParentKeyA").IsNullable = "true";
        sourceModel.TableColumnList.Single(row => row.Id == "SalesDb.dbo.ChildPkCase.ParentKeyB").IsNullable = "true";

        var sourceWorkspace = CreateWorkspace(sourceModel, "source");
        var liveWorkspace = CreateWorkspace(
            CreatePrimaryKeyModel(
                isClustered: false,
                includeSecondPrimaryKeyMember: true,
                includePrimaryKeyMembers: true,
                includeDependentForeignKey: true,
                includeDependentForeignKeyMembers: true,
                dependentForeignKeyTargetsAllMembers: true),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(2, manifest.AlterCount);
        Assert.Equal(1, manifest.ReplaceCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.True(manifest.IsDeployable);
        Assert.Equal(2, manifest.ManifestModel.AlterTableColumnList.Count);
        var replaceForeignKey = Assert.Single(manifest.ManifestModel.ReplaceForeignKeyList);
        Assert.Equal("SalesDb.dbo.ChildPkCase.fk.FK_ChildPkCase_ParentPkCase", replaceForeignKey.SourceForeignKeyId);
        Assert.Equal("SalesDb.dbo.ChildPkCase.fk.FK_ChildPkCase_ParentPkCase", replaceForeignKey.LiveForeignKeyId);
    }

    [Fact]
    public void BuildManifest_DeduplicatesForeignKeyReplacement_WhenAlteredColumnAndSharedForeignKeyDifferenceConverge()
    {
        var sourceModel = CreateForeignKeyModel(sourceTargetTableName: "ParentAlt", includeSourceForeignKeyMember: true);
        sourceModel.TableColumnList.Single(row => row.Id == "SalesDb.dbo.Child.ParentId").IsNullable = "true";

        var sourceWorkspace = CreateWorkspace(sourceModel, "source");
        var liveWorkspace = CreateWorkspace(
            CreateForeignKeyModel(sourceTargetTableName: "Parent", includeSourceForeignKeyMember: true),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(1, manifest.AlterCount);
        Assert.Equal(1, manifest.ReplaceCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.True(manifest.IsDeployable);
        Assert.Single(manifest.ManifestModel.AlterTableColumnList);
        var replaceForeignKey = Assert.Single(manifest.ManifestModel.ReplaceForeignKeyList);
        Assert.Equal("SalesDb.dbo.Child.fk.FK_Child_Parent", replaceForeignKey.SourceForeignKeyId);
        Assert.Equal("SalesDb.dbo.Child.fk.FK_Child_Parent", replaceForeignKey.LiveForeignKeyId);
    }

    [Fact]
    public void BuildManifest_DeduplicatesPrimaryKeyReplacement_WhenAlteredColumnAndSharedPrimaryKeyDifferenceConverge()
    {
        var sourceModel = CreateStringPrimaryKeyModel();
        sourceModel.TableColumnDataTypeDetailList.Single(row =>
            row.TableColumnId == "SalesDb.dbo.StringPkCase.KeyCode" &&
            string.Equals(row.Name, "Length", StringComparison.Ordinal)).Value = "100";
        sourceModel.PrimaryKeyColumnList.Single(row =>
            row.PrimaryKeyId == "SalesDb.dbo.StringPkCase.pk.PK_StringPkCase" &&
            string.Equals(row.Ordinal, "1", StringComparison.Ordinal)).IsDescending = "true";

        var sourceWorkspace = CreateWorkspace(sourceModel, "source");
        var liveWorkspace = CreateWorkspace(CreateStringPrimaryKeyModel(), "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(1, manifest.AlterCount);
        Assert.Equal(1, manifest.ReplaceCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.True(manifest.IsDeployable);
        Assert.Single(manifest.ManifestModel.AlterTableColumnList);
        var replacePrimaryKey = Assert.Single(manifest.ManifestModel.ReplacePrimaryKeyList);
        Assert.Equal("SalesDb.dbo.StringPkCase.pk.PK_StringPkCase", replacePrimaryKey.SourcePrimaryKeyId);
        Assert.Equal("SalesDb.dbo.StringPkCase.pk.PK_StringPkCase", replacePrimaryKey.LivePrimaryKeyId);
    }

    [Fact]
    public void BuildManifest_DeduplicatesMixedConstraintAndIndexDependencyExpansions_PerObject()
    {
        var sourceModel = CreateMixedDependencyModel();
        sourceModel.TableColumnDataTypeDetailList.Single(row =>
            row.TableColumnId == "SalesDb.dbo.ParentMixed.KeyCode" &&
            string.Equals(row.Name, "Length", StringComparison.Ordinal)).Value = "100";

        var sourceWorkspace = CreateWorkspace(sourceModel, "source");
        var liveWorkspace = CreateWorkspace(CreateMixedDependencyModel(), "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            MetaSqlLiveDatabasePresence.Present,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=dbo:*");

        Assert.Equal(1, manifest.AlterCount);
        Assert.Equal(3, manifest.ReplaceCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.True(manifest.IsDeployable);
        Assert.Single(manifest.ManifestModel.AlterTableColumnList);
        Assert.Single(manifest.ManifestModel.ReplacePrimaryKeyList);
        Assert.Single(manifest.ManifestModel.ReplaceForeignKeyList);
        Assert.Single(manifest.ManifestModel.ReplaceIndexList);
    }

    private static MetaSqlModel CreateModel(bool includeSourceOnlyColumn, bool includeLiveOnlyColumn, int sourceNameLength)
    {
        var model = MetaSqlModel.CreateEmpty();

        var database = new Database
        {
            Id = "SalesDb",
            Name = "SalesDb",
        };
        var schema = new Schema
        {
            Id = "SalesDb.dbo",
            Name = "dbo",
            DatabaseId = database.Id,
            Database = database,
        };
        var table = new Table
        {
            Id = "SalesDb.dbo.Customer",
            Name = "Customer",
            SchemaId = schema.Id,
            Schema = schema,
        };

        var customerId = new TableColumn
        {
            Id = "SalesDb.dbo.Customer.CustomerId",
            Name = "CustomerId",
            Ordinal = "1",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            TableId = table.Id,
            Table = table,
        };
        var customerName = new TableColumn
        {
            Id = "SalesDb.dbo.Customer.CustomerName",
            Name = "CustomerName",
            Ordinal = "2",
            MetaDataTypeId = "sqlserver:type:nvarchar",
            IsNullable = "true",
            TableId = table.Id,
            Table = table,
        };
        var customerNameLength = new TableColumnDataTypeDetail
        {
            Id = $"SalesDb.dbo.Customer.CustomerName.detail.Length.{sourceNameLength}",
            Name = "Length",
            Value = sourceNameLength.ToString(),
            TableColumnId = customerName.Id,
            TableColumn = customerName,
        };

        model.DatabaseList.Add(database);
        model.SchemaList.Add(schema);
        model.TableList.Add(table);
        model.TableColumnList.Add(customerId);
        model.TableColumnList.Add(customerName);
        model.TableColumnDataTypeDetailList.Add(customerNameLength);

        if (includeSourceOnlyColumn)
        {
            model.TableColumnList.Add(new TableColumn
            {
                Id = "SalesDb.dbo.Customer.NewCode",
                Name = "NewCode",
                Ordinal = "3",
                MetaDataTypeId = "sqlserver:type:nvarchar",
                IsNullable = "true",
                TableId = table.Id,
                Table = table,
            });
        }

        if (includeLiveOnlyColumn)
        {
            model.TableColumnList.Add(new TableColumn
            {
                Id = "SalesDb.dbo.Customer.LegacyCode",
                Name = "LegacyCode",
                Ordinal = "4",
                MetaDataTypeId = "sqlserver:type:nvarchar",
                IsNullable = "true",
                TableId = table.Id,
                Table = table,
            });
        }

        return model;
    }

    private static MetaSqlModel CreateForeignKeyModel(string sourceTargetTableName, bool includeSourceForeignKeyMember)
    {
        var model = MetaSqlModel.CreateEmpty();

        var database = new Database
        {
            Id = "SalesDb",
            Name = "SalesDb",
        };
        var schema = new Schema
        {
            Id = "SalesDb.dbo",
            Name = "dbo",
            DatabaseId = database.Id,
            Database = database,
        };
        var parent = new Table
        {
            Id = "SalesDb.dbo.Parent",
            Name = "Parent",
            SchemaId = schema.Id,
            Schema = schema,
        };
        var parentAlt = new Table
        {
            Id = "SalesDb.dbo.ParentAlt",
            Name = "ParentAlt",
            SchemaId = schema.Id,
            Schema = schema,
        };
        var child = new Table
        {
            Id = "SalesDb.dbo.Child",
            Name = "Child",
            SchemaId = schema.Id,
            Schema = schema,
        };

        var parentId = new TableColumn
        {
            Id = "SalesDb.dbo.Parent.ParentId",
            Name = "ParentId",
            Ordinal = "1",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            TableId = parent.Id,
            Table = parent,
        };
        var parentAltId = new TableColumn
        {
            Id = "SalesDb.dbo.ParentAlt.ParentId",
            Name = "ParentId",
            Ordinal = "1",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            TableId = parentAlt.Id,
            Table = parentAlt,
        };
        var childId = new TableColumn
        {
            Id = "SalesDb.dbo.Child.ChildId",
            Name = "ChildId",
            Ordinal = "1",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            TableId = child.Id,
            Table = child,
        };
        var childParentId = new TableColumn
        {
            Id = "SalesDb.dbo.Child.ParentId",
            Name = "ParentId",
            Ordinal = "2",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            TableId = child.Id,
            Table = child,
        };

        var childPrimaryKey = new PrimaryKey
        {
            Id = "SalesDb.dbo.Child.pk.PK_Child",
            Name = "PK_Child",
            TableId = child.Id,
            Table = child,
            IsClustered = "true",
        };
        var childPrimaryKeyColumn = new PrimaryKeyColumn
        {
            Id = "SalesDb.dbo.Child.pk.PK_Child.column.1",
            PrimaryKeyId = childPrimaryKey.Id,
            PrimaryKey = childPrimaryKey,
            TableColumnId = childId.Id,
            TableColumn = childId,
            Ordinal = "1",
        };

        var targetTable = string.Equals(sourceTargetTableName, "ParentAlt", StringComparison.Ordinal)
            ? parentAlt
            : parent;
        var targetColumn = string.Equals(sourceTargetTableName, "ParentAlt", StringComparison.Ordinal)
            ? parentAltId
            : parentId;
        var foreignKey = new ForeignKey
        {
            Id = "SalesDb.dbo.Child.fk.FK_Child_Parent",
            Name = "FK_Child_Parent",
            SourceTableId = child.Id,
            SourceTable = child,
            TargetTableId = targetTable.Id,
            TargetTable = targetTable,
        };

        model.DatabaseList.Add(database);
        model.SchemaList.Add(schema);
        model.TableList.Add(parent);
        model.TableList.Add(parentAlt);
        model.TableList.Add(child);
        model.TableColumnList.Add(parentId);
        model.TableColumnList.Add(parentAltId);
        model.TableColumnList.Add(childId);
        model.TableColumnList.Add(childParentId);
        model.PrimaryKeyList.Add(childPrimaryKey);
        model.PrimaryKeyColumnList.Add(childPrimaryKeyColumn);
        model.ForeignKeyList.Add(foreignKey);

        if (includeSourceForeignKeyMember)
        {
            model.ForeignKeyColumnList.Add(new ForeignKeyColumn
            {
                Id = "SalesDb.dbo.Child.fk.FK_Child_Parent.column.1",
                ForeignKeyId = foreignKey.Id,
                ForeignKey = foreignKey,
                SourceColumnId = childParentId.Id,
                SourceColumn = childParentId,
                TargetColumnId = targetColumn.Id,
                TargetColumn = targetColumn,
                Ordinal = "1",
            });
        }

        return model;
    }

    private static void AddSharedLengthColumn(MetaSqlModel model, string columnName, string ordinal, int length)
    {
        var table = model.TableList.Single(row => row.Id == "SalesDb.dbo.Customer");
        var column = new TableColumn
        {
            Id = $"SalesDb.dbo.Customer.{columnName}",
            Name = columnName,
            Ordinal = ordinal,
            MetaDataTypeId = "sqlserver:type:nvarchar",
            IsNullable = "true",
            TableId = table.Id,
            Table = table,
        };
        var detail = new TableColumnDataTypeDetail
        {
            Id = $"SalesDb.dbo.Customer.{columnName}.detail.Length.{length}",
            Name = "Length",
            Value = length.ToString(),
            TableColumnId = column.Id,
            TableColumn = column,
        };

        model.TableColumnList.Add(column);
        model.TableColumnDataTypeDetailList.Add(detail);
    }

    private static MetaSqlModel CreateIndexModel(bool isClustered, bool sourceDescending, bool includeSourceIndexMember)
    {
        var model = MetaSqlModel.CreateEmpty();

        var database = new Database
        {
            Id = "SalesDb",
            Name = "SalesDb",
        };
        var schema = new Schema
        {
            Id = "SalesDb.dbo",
            Name = "dbo",
            DatabaseId = database.Id,
            Database = database,
        };
        var table = new Table
        {
            Id = "SalesDb.dbo.PayloadCase",
            Name = "PayloadCase",
            SchemaId = schema.Id,
            Schema = schema,
        };
        var idColumn = new TableColumn
        {
            Id = "SalesDb.dbo.PayloadCase.Id",
            Name = "Id",
            Ordinal = "1",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            TableId = table.Id,
            Table = table,
        };
        var payloadColumn = new TableColumn
        {
            Id = "SalesDb.dbo.PayloadCase.Payload",
            Name = "Payload",
            Ordinal = "2",
            MetaDataTypeId = "sqlserver:type:nvarchar",
            IsNullable = "false",
            TableId = table.Id,
            Table = table,
        };
        var payloadLength = new TableColumnDataTypeDetail
        {
            Id = "SalesDb.dbo.PayloadCase.Payload.detail.Length",
            Name = "Length",
            Value = "100",
            TableColumnId = payloadColumn.Id,
            TableColumn = payloadColumn,
        };
        var index = new Index
        {
            Id = "SalesDb.dbo.PayloadCase.index.IX_PayloadCase_Payload",
            Name = "IX_PayloadCase_Payload",
            TableId = table.Id,
            Table = table,
            IsUnique = string.Empty,
            IsClustered = isClustered ? "true" : string.Empty,
        };

        model.DatabaseList.Add(database);
        model.SchemaList.Add(schema);
        model.TableList.Add(table);
        model.TableColumnList.Add(idColumn);
        model.TableColumnList.Add(payloadColumn);
        model.TableColumnDataTypeDetailList.Add(payloadLength);
        model.IndexList.Add(index);

        if (includeSourceIndexMember)
        {
            model.IndexColumnList.Add(new IndexColumn
            {
                Id = "SalesDb.dbo.PayloadCase.index.IX_PayloadCase_Payload.column.1",
                IndexId = index.Id,
                Index = index,
                TableColumnId = payloadColumn.Id,
                TableColumn = payloadColumn,
                Ordinal = "1",
                IsIncluded = string.Empty,
                IsDescending = sourceDescending ? "true" : string.Empty,
            });
        }

        return model;
    }

    private static MetaSqlModel CreatePrimaryKeyModel(
        bool isClustered,
        bool includeSecondPrimaryKeyMember,
        bool includePrimaryKeyMembers,
        bool includeDependentForeignKey,
        bool includeDependentForeignKeyMembers,
        bool dependentForeignKeyTargetsAllMembers)
    {
        var model = MetaSqlModel.CreateEmpty();

        var database = new Database
        {
            Id = "SalesDb",
            Name = "SalesDb",
        };
        var schema = new Schema
        {
            Id = "SalesDb.dbo",
            Name = "dbo",
            DatabaseId = database.Id,
            Database = database,
        };
        var parent = new Table
        {
            Id = "SalesDb.dbo.ParentPkCase",
            Name = "ParentPkCase",
            SchemaId = schema.Id,
            Schema = schema,
        };
        var child = new Table
        {
            Id = "SalesDb.dbo.ChildPkCase",
            Name = "ChildPkCase",
            SchemaId = schema.Id,
            Schema = schema,
        };
        var parentKeyA = new TableColumn
        {
            Id = "SalesDb.dbo.ParentPkCase.KeyA",
            Name = "KeyA",
            Ordinal = "1",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            TableId = parent.Id,
            Table = parent,
        };
        var parentKeyB = new TableColumn
        {
            Id = "SalesDb.dbo.ParentPkCase.KeyB",
            Name = "KeyB",
            Ordinal = "2",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            TableId = parent.Id,
            Table = parent,
        };
        var childId = new TableColumn
        {
            Id = "SalesDb.dbo.ChildPkCase.ChildId",
            Name = "ChildId",
            Ordinal = "1",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            TableId = child.Id,
            Table = child,
        };
        var childParentKeyA = new TableColumn
        {
            Id = "SalesDb.dbo.ChildPkCase.ParentKeyA",
            Name = "ParentKeyA",
            Ordinal = "2",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            TableId = child.Id,
            Table = child,
        };
        var childParentKeyB = new TableColumn
        {
            Id = "SalesDb.dbo.ChildPkCase.ParentKeyB",
            Name = "ParentKeyB",
            Ordinal = "3",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            TableId = child.Id,
            Table = child,
        };

        var primaryKey = new PrimaryKey
        {
            Id = "SalesDb.dbo.ParentPkCase.pk.PK_ParentPkCase",
            Name = "PK_ParentPkCase",
            TableId = parent.Id,
            Table = parent,
            IsClustered = isClustered ? "true" : "false",
        };
        var childPrimaryKey = new PrimaryKey
        {
            Id = "SalesDb.dbo.ChildPkCase.pk.PK_ChildPkCase",
            Name = "PK_ChildPkCase",
            TableId = child.Id,
            Table = child,
            IsClustered = "false",
        };

        model.DatabaseList.Add(database);
        model.SchemaList.Add(schema);
        model.TableList.Add(parent);
        model.TableList.Add(child);
        model.TableColumnList.Add(parentKeyA);
        model.TableColumnList.Add(parentKeyB);
        model.TableColumnList.Add(childId);
        model.TableColumnList.Add(childParentKeyA);
        model.TableColumnList.Add(childParentKeyB);
        model.PrimaryKeyList.Add(primaryKey);
        model.PrimaryKeyList.Add(childPrimaryKey);

        model.PrimaryKeyColumnList.Add(new PrimaryKeyColumn
        {
            Id = "SalesDb.dbo.ChildPkCase.pk.PK_ChildPkCase.column.1",
            PrimaryKeyId = childPrimaryKey.Id,
            PrimaryKey = childPrimaryKey,
            TableColumnId = childId.Id,
            TableColumn = childId,
            Ordinal = "1",
        });

        if (includePrimaryKeyMembers)
        {
            model.PrimaryKeyColumnList.Add(new PrimaryKeyColumn
            {
                Id = "SalesDb.dbo.ParentPkCase.pk.PK_ParentPkCase.column.1",
                PrimaryKeyId = primaryKey.Id,
                PrimaryKey = primaryKey,
                TableColumnId = parentKeyA.Id,
                TableColumn = parentKeyA,
                Ordinal = "1",
            });

            if (includeSecondPrimaryKeyMember)
            {
                model.PrimaryKeyColumnList.Add(new PrimaryKeyColumn
                {
                    Id = "SalesDb.dbo.ParentPkCase.pk.PK_ParentPkCase.column.2",
                    PrimaryKeyId = primaryKey.Id,
                    PrimaryKey = primaryKey,
                    TableColumnId = parentKeyB.Id,
                    TableColumn = parentKeyB,
                    Ordinal = "2",
                });
            }
        }

        if (includeDependentForeignKey)
        {
            var foreignKey = new ForeignKey
            {
                Id = "SalesDb.dbo.ChildPkCase.fk.FK_ChildPkCase_ParentPkCase",
                Name = "FK_ChildPkCase_ParentPkCase",
                SourceTableId = child.Id,
                SourceTable = child,
                TargetTableId = parent.Id,
                TargetTable = parent,
            };
            model.ForeignKeyList.Add(foreignKey);

            if (includeDependentForeignKeyMembers)
            {
                model.ForeignKeyColumnList.Add(new ForeignKeyColumn
                {
                    Id = "SalesDb.dbo.ChildPkCase.fk.FK_ChildPkCase_ParentPkCase.column.1",
                    ForeignKeyId = foreignKey.Id,
                    ForeignKey = foreignKey,
                    SourceColumnId = childParentKeyA.Id,
                    SourceColumn = childParentKeyA,
                    TargetColumnId = parentKeyA.Id,
                    TargetColumn = parentKeyA,
                    Ordinal = "1",
                });

                if (dependentForeignKeyTargetsAllMembers && includeSecondPrimaryKeyMember)
                {
                    model.ForeignKeyColumnList.Add(new ForeignKeyColumn
                    {
                        Id = "SalesDb.dbo.ChildPkCase.fk.FK_ChildPkCase_ParentPkCase.column.2",
                        ForeignKeyId = foreignKey.Id,
                        ForeignKey = foreignKey,
                        SourceColumnId = childParentKeyB.Id,
                        SourceColumn = childParentKeyB,
                        TargetColumnId = parentKeyB.Id,
                        TargetColumn = parentKeyB,
                        Ordinal = "2",
                    });
                }
            }
        }

        return model;
    }

    private static MetaSqlModel CreateStringPrimaryKeyModel()
    {
        var model = MetaSqlModel.CreateEmpty();

        var database = new Database
        {
            Id = "SalesDb",
            Name = "SalesDb",
        };
        var schema = new Schema
        {
            Id = "SalesDb.dbo",
            Name = "dbo",
            DatabaseId = database.Id,
            Database = database,
        };
        var table = new Table
        {
            Id = "SalesDb.dbo.StringPkCase",
            Name = "StringPkCase",
            SchemaId = schema.Id,
            Schema = schema,
        };
        var keyCode = new TableColumn
        {
            Id = "SalesDb.dbo.StringPkCase.KeyCode",
            Name = "KeyCode",
            Ordinal = "1",
            MetaDataTypeId = "sqlserver:type:varchar",
            IsNullable = "false",
            TableId = table.Id,
            Table = table,
        };
        var keyCodeLength = new TableColumnDataTypeDetail
        {
            Id = "SalesDb.dbo.StringPkCase.KeyCode.detail.Length",
            Name = "Length",
            Value = "50",
            TableColumnId = keyCode.Id,
            TableColumn = keyCode,
        };
        var primaryKey = new PrimaryKey
        {
            Id = "SalesDb.dbo.StringPkCase.pk.PK_StringPkCase",
            Name = "PK_StringPkCase",
            TableId = table.Id,
            Table = table,
            IsClustered = "false",
        };
        var primaryKeyMember = new PrimaryKeyColumn
        {
            Id = "SalesDb.dbo.StringPkCase.pk.PK_StringPkCase.column.1",
            PrimaryKeyId = primaryKey.Id,
            PrimaryKey = primaryKey,
            TableColumnId = keyCode.Id,
            TableColumn = keyCode,
            Ordinal = "1",
            IsDescending = "false",
        };

        model.DatabaseList.Add(database);
        model.SchemaList.Add(schema);
        model.TableList.Add(table);
        model.TableColumnList.Add(keyCode);
        model.TableColumnDataTypeDetailList.Add(keyCodeLength);
        model.PrimaryKeyList.Add(primaryKey);
        model.PrimaryKeyColumnList.Add(primaryKeyMember);

        return model;
    }

    private static MetaSqlModel CreateMixedDependencyModel()
    {
        var model = MetaSqlModel.CreateEmpty();

        var database = new Database
        {
            Id = "SalesDb",
            Name = "SalesDb",
        };
        var schema = new Schema
        {
            Id = "SalesDb.dbo",
            Name = "dbo",
            DatabaseId = database.Id,
            Database = database,
        };
        var parent = new Table
        {
            Id = "SalesDb.dbo.ParentMixed",
            Name = "ParentMixed",
            SchemaId = schema.Id,
            Schema = schema,
        };
        var child = new Table
        {
            Id = "SalesDb.dbo.ChildMixed",
            Name = "ChildMixed",
            SchemaId = schema.Id,
            Schema = schema,
        };

        var parentKeyCode = new TableColumn
        {
            Id = "SalesDb.dbo.ParentMixed.KeyCode",
            Name = "KeyCode",
            Ordinal = "1",
            MetaDataTypeId = "sqlserver:type:varchar",
            IsNullable = "false",
            TableId = parent.Id,
            Table = parent,
        };
        var parentKeyCodeLength = new TableColumnDataTypeDetail
        {
            Id = "SalesDb.dbo.ParentMixed.KeyCode.detail.Length",
            Name = "Length",
            Value = "50",
            TableColumnId = parentKeyCode.Id,
            TableColumn = parentKeyCode,
        };
        var childId = new TableColumn
        {
            Id = "SalesDb.dbo.ChildMixed.ChildId",
            Name = "ChildId",
            Ordinal = "1",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            TableId = child.Id,
            Table = child,
        };
        var childParentKeyCode = new TableColumn
        {
            Id = "SalesDb.dbo.ChildMixed.ParentKeyCode",
            Name = "ParentKeyCode",
            Ordinal = "2",
            MetaDataTypeId = "sqlserver:type:varchar",
            IsNullable = "false",
            TableId = child.Id,
            Table = child,
        };
        var childParentKeyCodeLength = new TableColumnDataTypeDetail
        {
            Id = "SalesDb.dbo.ChildMixed.ParentKeyCode.detail.Length",
            Name = "Length",
            Value = "50",
            TableColumnId = childParentKeyCode.Id,
            TableColumn = childParentKeyCode,
        };

        var parentPrimaryKey = new PrimaryKey
        {
            Id = "SalesDb.dbo.ParentMixed.pk.PK_ParentMixed",
            Name = "PK_ParentMixed",
            TableId = parent.Id,
            Table = parent,
            IsClustered = "false",
        };
        var parentPrimaryKeyColumn = new PrimaryKeyColumn
        {
            Id = "SalesDb.dbo.ParentMixed.pk.PK_ParentMixed.column.1",
            PrimaryKeyId = parentPrimaryKey.Id,
            PrimaryKey = parentPrimaryKey,
            TableColumnId = parentKeyCode.Id,
            TableColumn = parentKeyCode,
            Ordinal = "1",
            IsDescending = "false",
        };
        var childPrimaryKey = new PrimaryKey
        {
            Id = "SalesDb.dbo.ChildMixed.pk.PK_ChildMixed",
            Name = "PK_ChildMixed",
            TableId = child.Id,
            Table = child,
            IsClustered = "false",
        };
        var childPrimaryKeyColumn = new PrimaryKeyColumn
        {
            Id = "SalesDb.dbo.ChildMixed.pk.PK_ChildMixed.column.1",
            PrimaryKeyId = childPrimaryKey.Id,
            PrimaryKey = childPrimaryKey,
            TableColumnId = childId.Id,
            TableColumn = childId,
            Ordinal = "1",
            IsDescending = "false",
        };
        var foreignKey = new ForeignKey
        {
            Id = "SalesDb.dbo.ChildMixed.fk.FK_ChildMixed_ParentMixed",
            Name = "FK_ChildMixed_ParentMixed",
            SourceTableId = child.Id,
            SourceTable = child,
            TargetTableId = parent.Id,
            TargetTable = parent,
        };
        var foreignKeyColumn = new ForeignKeyColumn
        {
            Id = "SalesDb.dbo.ChildMixed.fk.FK_ChildMixed_ParentMixed.column.1",
            ForeignKeyId = foreignKey.Id,
            ForeignKey = foreignKey,
            SourceColumnId = childParentKeyCode.Id,
            SourceColumn = childParentKeyCode,
            TargetColumnId = parentKeyCode.Id,
            TargetColumn = parentKeyCode,
            Ordinal = "1",
        };
        var index = new Index
        {
            Id = "SalesDb.dbo.ParentMixed.index.IX_ParentMixed_KeyCode",
            Name = "IX_ParentMixed_KeyCode",
            TableId = parent.Id,
            Table = parent,
            IsUnique = string.Empty,
            IsClustered = "false",
        };
        var indexColumn = new IndexColumn
        {
            Id = "SalesDb.dbo.ParentMixed.index.IX_ParentMixed_KeyCode.column.1",
            IndexId = index.Id,
            Index = index,
            TableColumnId = parentKeyCode.Id,
            TableColumn = parentKeyCode,
            Ordinal = "1",
            IsIncluded = string.Empty,
            IsDescending = string.Empty,
        };

        model.DatabaseList.Add(database);
        model.SchemaList.Add(schema);
        model.TableList.Add(parent);
        model.TableList.Add(child);
        model.TableColumnList.Add(parentKeyCode);
        model.TableColumnList.Add(childId);
        model.TableColumnList.Add(childParentKeyCode);
        model.TableColumnDataTypeDetailList.Add(parentKeyCodeLength);
        model.TableColumnDataTypeDetailList.Add(childParentKeyCodeLength);
        model.PrimaryKeyList.Add(parentPrimaryKey);
        model.PrimaryKeyList.Add(childPrimaryKey);
        model.PrimaryKeyColumnList.Add(parentPrimaryKeyColumn);
        model.PrimaryKeyColumnList.Add(childPrimaryKeyColumn);
        model.ForeignKeyList.Add(foreignKey);
        model.ForeignKeyColumnList.Add(foreignKeyColumn);
        model.IndexList.Add(index);
        model.IndexColumnList.Add(indexColumn);

        return model;
    }

    private static Meta.Core.Domain.Workspace CreateWorkspace(MetaSqlModel model, string leafName)
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), leafName);
        model.SaveToXmlWorkspace(workspacePath);
        return new WorkspaceService().LoadAsync(workspacePath, searchUpward: false).GetAwaiter().GetResult();
    }
}
