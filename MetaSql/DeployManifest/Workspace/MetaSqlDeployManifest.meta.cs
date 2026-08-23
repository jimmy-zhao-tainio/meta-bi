#nullable enable
using System;
using System.Collections.Generic;

namespace MetaSqlDeployManifest;
public sealed partial class AddForeignKey
{
    public string Id { get; set; } = null !;
    public string SourceForeignKeyId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class AddFunction
{
    public string Id { get; set; } = null !;
    public string SourceFunctionId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class AddIndex
{
    public string Id { get; set; } = null !;
    public string SourceIndexId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class AddPrimaryKey
{
    public string Id { get; set; } = null !;
    public string SourcePrimaryKeyId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class AddSchema
{
    public string Id { get; set; } = null !;
    public string SourceSchemaId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class AddStoredProcedure
{
    public string Id { get; set; } = null !;
    public string SourceStoredProcedureId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class AddTable
{
    public string Id { get; set; } = null !;
    public string SourceTableId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class AddTableColumn
{
    public string Id { get; set; } = null !;
    public string SourceTableColumnId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class AddView
{
    public string Id { get; set; } = null !;
    public string SourceViewId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class AlterTableColumn
{
    public string Id { get; set; } = null !;
    public string LiveTableColumnId { get; set; } = null !;
    public string SourceTableColumnId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class BlockForeignKeyDifference
{
    public string Id { get; set; } = null !;
    public string DifferenceSummary { get; set; } = null !;
    public string LiveForeignKeyId { get; set; } = null !;
    public string SourceForeignKeyId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class BlockFunctionDifference
{
    public string Id { get; set; } = null !;
    public string DifferenceSummary { get; set; } = null !;
    public string LiveFunctionId { get; set; } = null !;
    public string SourceFunctionId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class BlockIndexDifference
{
    public string Id { get; set; } = null !;
    public string DifferenceSummary { get; set; } = null !;
    public string LiveIndexId { get; set; } = null !;
    public string SourceIndexId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class BlockPrimaryKeyDifference
{
    public string Id { get; set; } = null !;
    public string DifferenceSummary { get; set; } = null !;
    public string LivePrimaryKeyId { get; set; } = null !;
    public string SourcePrimaryKeyId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class BlockStoredProcedureDifference
{
    public string Id { get; set; } = null !;
    public string DifferenceSummary { get; set; } = null !;
    public string LiveStoredProcedureId { get; set; } = null !;
    public string SourceStoredProcedureId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class BlockTableColumnDifference
{
    public string Id { get; set; } = null !;
    public string DifferenceSummary { get; set; } = null !;
    public string LiveTableColumnId { get; set; } = null !;
    public string SourceTableColumnId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class BlockTableDifference
{
    public string Id { get; set; } = null !;
    public string DifferenceSummary { get; set; } = null !;
    public string LiveTableId { get; set; } = null !;
    public string SourceTableId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class BlockViewDifference
{
    public string Id { get; set; } = null !;
    public string DifferenceSummary { get; set; } = null !;
    public string LiveViewId { get; set; } = null !;
    public string SourceViewId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class DeployManifest
{
    public string Id { get; set; } = null !;
    public string CreatedUtc { get; set; } = null !;
    public string ExpectedLiveDatabasePresence { get; set; } = null !;
    public string LiveInstanceFingerprint { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string SourceInstanceFingerprint { get; set; } = null !;
    public string? TargetDescription { get; set; }
}

public sealed partial class DropForeignKey
{
    public string Id { get; set; } = null !;
    public string LiveForeignKeyId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class DropFunction
{
    public string Id { get; set; } = null !;
    public string LiveFunctionId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class DropIndex
{
    public string Id { get; set; } = null !;
    public string LiveIndexId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class DropPrimaryKey
{
    public string Id { get; set; } = null !;
    public string LivePrimaryKeyId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class DropStoredProcedure
{
    public string Id { get; set; } = null !;
    public string LiveStoredProcedureId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class DropTable
{
    public string Id { get; set; } = null !;
    public string LiveTableId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class DropTableColumn
{
    public string Id { get; set; } = null !;
    public string LiveTableColumnId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class DropView
{
    public string Id { get; set; } = null !;
    public string LiveViewId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class ReplaceForeignKey
{
    public string Id { get; set; } = null !;
    public string LiveForeignKeyId { get; set; } = null !;
    public string SourceForeignKeyId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class ReplaceFunction
{
    public string Id { get; set; } = null !;
    public string LiveFunctionId { get; set; } = null !;
    public string SourceFunctionId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class ReplaceIndex
{
    public string Id { get; set; } = null !;
    public string LiveIndexId { get; set; } = null !;
    public string SourceIndexId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class ReplacePrimaryKey
{
    public string Id { get; set; } = null !;
    public string LivePrimaryKeyId { get; set; } = null !;
    public string SourcePrimaryKeyId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class ReplaceStoredProcedure
{
    public string Id { get; set; } = null !;
    public string LiveStoredProcedureId { get; set; } = null !;
    public string SourceStoredProcedureId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class ReplaceView
{
    public string Id { get; set; } = null !;
    public string LiveViewId { get; set; } = null !;
    public string SourceViewId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class TruncateTableColumnData
{
    public string Id { get; set; } = null !;
    public string LiveTableColumnId { get; set; } = null !;
    public string SourceTableColumnId { get; set; } = null !;
    public DeployManifest DeployManifest { get; set; } = null !;
}

public sealed partial class MetaSqlDeployManifestModel
{
    public static MetaSqlDeployManifestModel CreateEmpty() => new();
    public List<AddForeignKey> AddForeignKeyList { get; set; } = new();
    public List<AddFunction> AddFunctionList { get; set; } = new();
    public List<AddIndex> AddIndexList { get; set; } = new();
    public List<AddPrimaryKey> AddPrimaryKeyList { get; set; } = new();
    public List<AddSchema> AddSchemaList { get; set; } = new();
    public List<AddStoredProcedure> AddStoredProcedureList { get; set; } = new();
    public List<AddTable> AddTableList { get; set; } = new();
    public List<AddTableColumn> AddTableColumnList { get; set; } = new();
    public List<AddView> AddViewList { get; set; } = new();
    public List<AlterTableColumn> AlterTableColumnList { get; set; } = new();
    public List<BlockForeignKeyDifference> BlockForeignKeyDifferenceList { get; set; } = new();
    public List<BlockFunctionDifference> BlockFunctionDifferenceList { get; set; } = new();
    public List<BlockIndexDifference> BlockIndexDifferenceList { get; set; } = new();
    public List<BlockPrimaryKeyDifference> BlockPrimaryKeyDifferenceList { get; set; } = new();
    public List<BlockStoredProcedureDifference> BlockStoredProcedureDifferenceList { get; set; } = new();
    public List<BlockTableColumnDifference> BlockTableColumnDifferenceList { get; set; } = new();
    public List<BlockTableDifference> BlockTableDifferenceList { get; set; } = new();
    public List<BlockViewDifference> BlockViewDifferenceList { get; set; } = new();
    public List<DeployManifest> DeployManifestList { get; set; } = new();
    public List<DropForeignKey> DropForeignKeyList { get; set; } = new();
    public List<DropFunction> DropFunctionList { get; set; } = new();
    public List<DropIndex> DropIndexList { get; set; } = new();
    public List<DropPrimaryKey> DropPrimaryKeyList { get; set; } = new();
    public List<DropStoredProcedure> DropStoredProcedureList { get; set; } = new();
    public List<DropTable> DropTableList { get; set; } = new();
    public List<DropTableColumn> DropTableColumnList { get; set; } = new();
    public List<DropView> DropViewList { get; set; } = new();
    public List<ReplaceForeignKey> ReplaceForeignKeyList { get; set; } = new();
    public List<ReplaceFunction> ReplaceFunctionList { get; set; } = new();
    public List<ReplaceIndex> ReplaceIndexList { get; set; } = new();
    public List<ReplacePrimaryKey> ReplacePrimaryKeyList { get; set; } = new();
    public List<ReplaceStoredProcedure> ReplaceStoredProcedureList { get; set; } = new();
    public List<ReplaceView> ReplaceViewList { get; set; } = new();
    public List<TruncateTableColumnData> TruncateTableColumnDataList { get; set; } = new();
}

public static partial class MetaSqlDeployManifestInstance
{
    private static readonly MetaSqlDeployManifestModel _builtIn = CreateBuiltIn();
    public static MetaSqlDeployManifestModel BuiltIn => _builtIn;

    public static MetaSqlDeployManifestModel CreateBuiltIn()
    {
        var model = MetaSqlDeployManifestModel.CreateEmpty();
        return model;
    }
}