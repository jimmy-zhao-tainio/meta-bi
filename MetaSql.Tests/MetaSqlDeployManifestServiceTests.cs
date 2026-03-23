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
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=raw:*");

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
    public void BuildManifest_WithoutDataDrop_IgnoresLiveOnlyDataDropDifferences()
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
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=raw:*",
            withDataDrop: false);

        Assert.Equal(1, manifest.AddCount);
        Assert.Equal(0, manifest.DropCount);
        Assert.Equal(1, manifest.AlterCount);
        Assert.Equal(0, manifest.ReplaceCount);
        Assert.Equal(0, manifest.BlockCount);
        Assert.Equal(1, manifest.IgnoredLiveOnlyDataDropCount);
        Assert.True(manifest.IsDeployable);

        Assert.Single(manifest.ManifestModel.AddTableColumnList);
        Assert.Empty(manifest.ManifestModel.DropTableColumnList);
        Assert.Single(manifest.ManifestModel.AlterTableColumnList);
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

    private static MetaSqlModel CreateModel(bool includeSourceOnlyColumn, bool includeLiveOnlyColumn, int sourceNameLength)
    {
        var model = MetaSqlModel.CreateEmpty();

        var database = new Database
        {
            Id = "SalesDb",
            Name = "SalesDb",
            Platform = "sqlserver",
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
            Platform = "sqlserver",
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

    private static MetaSqlModel CreateIndexModel(bool isClustered, bool sourceDescending, bool includeSourceIndexMember)
    {
        var model = MetaSqlModel.CreateEmpty();

        var database = new Database
        {
            Id = "SalesDb",
            Name = "SalesDb",
            Platform = "sqlserver",
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
            Platform = "sqlserver",
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

    private static Meta.Core.Domain.Workspace CreateWorkspace(MetaSqlModel model, string leafName)
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), leafName);
        model.SaveToXmlWorkspace(workspacePath);
        return new WorkspaceService().LoadAsync(workspacePath, searchUpward: false).GetAwaiter().GetResult();
    }
}
