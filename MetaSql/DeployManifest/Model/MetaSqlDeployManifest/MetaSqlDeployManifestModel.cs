#nullable enable

using System.Collections.Generic;

namespace MetaSqlDeployManifest
{
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
}
