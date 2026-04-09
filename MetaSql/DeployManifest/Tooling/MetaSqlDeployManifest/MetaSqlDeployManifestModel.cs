using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Meta.Core.Serialization;

namespace MetaSqlDeployManifest
{
    [XmlRoot("MetaSqlDeployManifest")]
    public sealed partial class MetaSqlDeployManifestModel
    {
        public static MetaSqlDeployManifestModel CreateEmpty() => new();

        [XmlArray("AddForeignKeyList")]
        [XmlArrayItem("AddForeignKey")]
        public List<AddForeignKey> AddForeignKeyList { get; set; } = new();
        public bool ShouldSerializeAddForeignKeyList() => AddForeignKeyList.Count > 0;

        [XmlArray("AddIndexList")]
        [XmlArrayItem("AddIndex")]
        public List<AddIndex> AddIndexList { get; set; } = new();
        public bool ShouldSerializeAddIndexList() => AddIndexList.Count > 0;

        [XmlArray("AddPrimaryKeyList")]
        [XmlArrayItem("AddPrimaryKey")]
        public List<AddPrimaryKey> AddPrimaryKeyList { get; set; } = new();
        public bool ShouldSerializeAddPrimaryKeyList() => AddPrimaryKeyList.Count > 0;

        [XmlArray("AddSchemaList")]
        [XmlArrayItem("AddSchema")]
        public List<AddSchema> AddSchemaList { get; set; } = new();
        public bool ShouldSerializeAddSchemaList() => AddSchemaList.Count > 0;

        [XmlArray("AddTableList")]
        [XmlArrayItem("AddTable")]
        public List<AddTable> AddTableList { get; set; } = new();
        public bool ShouldSerializeAddTableList() => AddTableList.Count > 0;

        [XmlArray("AddTableColumnList")]
        [XmlArrayItem("AddTableColumn")]
        public List<AddTableColumn> AddTableColumnList { get; set; } = new();
        public bool ShouldSerializeAddTableColumnList() => AddTableColumnList.Count > 0;

        [XmlArray("AlterTableColumnList")]
        [XmlArrayItem("AlterTableColumn")]
        public List<AlterTableColumn> AlterTableColumnList { get; set; } = new();
        public bool ShouldSerializeAlterTableColumnList() => AlterTableColumnList.Count > 0;

        [XmlArray("TruncateTableColumnDataList")]
        [XmlArrayItem("TruncateTableColumnData")]
        public List<TruncateTableColumnData> TruncateTableColumnDataList { get; set; } = new();
        public bool ShouldSerializeTruncateTableColumnDataList() => TruncateTableColumnDataList.Count > 0;

        [XmlArray("BlockForeignKeyDifferenceList")]
        [XmlArrayItem("BlockForeignKeyDifference")]
        public List<BlockForeignKeyDifference> BlockForeignKeyDifferenceList { get; set; } = new();
        public bool ShouldSerializeBlockForeignKeyDifferenceList() => BlockForeignKeyDifferenceList.Count > 0;

        [XmlArray("BlockIndexDifferenceList")]
        [XmlArrayItem("BlockIndexDifference")]
        public List<BlockIndexDifference> BlockIndexDifferenceList { get; set; } = new();
        public bool ShouldSerializeBlockIndexDifferenceList() => BlockIndexDifferenceList.Count > 0;

        [XmlArray("BlockPrimaryKeyDifferenceList")]
        [XmlArrayItem("BlockPrimaryKeyDifference")]
        public List<BlockPrimaryKeyDifference> BlockPrimaryKeyDifferenceList { get; set; } = new();
        public bool ShouldSerializeBlockPrimaryKeyDifferenceList() => BlockPrimaryKeyDifferenceList.Count > 0;

        [XmlArray("BlockTableColumnDifferenceList")]
        [XmlArrayItem("BlockTableColumnDifference")]
        public List<BlockTableColumnDifference> BlockTableColumnDifferenceList { get; set; } = new();
        public bool ShouldSerializeBlockTableColumnDifferenceList() => BlockTableColumnDifferenceList.Count > 0;

        [XmlArray("BlockTableDifferenceList")]
        [XmlArrayItem("BlockTableDifference")]
        public List<BlockTableDifference> BlockTableDifferenceList { get; set; } = new();
        public bool ShouldSerializeBlockTableDifferenceList() => BlockTableDifferenceList.Count > 0;

        [XmlArray("DeployManifestList")]
        [XmlArrayItem("DeployManifest")]
        public List<DeployManifest> DeployManifestList { get; set; } = new();
        public bool ShouldSerializeDeployManifestList() => DeployManifestList.Count > 0;

        [XmlArray("DropForeignKeyList")]
        [XmlArrayItem("DropForeignKey")]
        public List<DropForeignKey> DropForeignKeyList { get; set; } = new();
        public bool ShouldSerializeDropForeignKeyList() => DropForeignKeyList.Count > 0;

        [XmlArray("ReplaceForeignKeyList")]
        [XmlArrayItem("ReplaceForeignKey")]
        public List<ReplaceForeignKey> ReplaceForeignKeyList { get; set; } = new();
        public bool ShouldSerializeReplaceForeignKeyList() => ReplaceForeignKeyList.Count > 0;

        [XmlArray("ReplacePrimaryKeyList")]
        [XmlArrayItem("ReplacePrimaryKey")]
        public List<ReplacePrimaryKey> ReplacePrimaryKeyList { get; set; } = new();
        public bool ShouldSerializeReplacePrimaryKeyList() => ReplacePrimaryKeyList.Count > 0;

        [XmlArray("ReplaceIndexList")]
        [XmlArrayItem("ReplaceIndex")]
        public List<ReplaceIndex> ReplaceIndexList { get; set; } = new();
        public bool ShouldSerializeReplaceIndexList() => ReplaceIndexList.Count > 0;

        [XmlArray("DropIndexList")]
        [XmlArrayItem("DropIndex")]
        public List<DropIndex> DropIndexList { get; set; } = new();
        public bool ShouldSerializeDropIndexList() => DropIndexList.Count > 0;

        [XmlArray("DropPrimaryKeyList")]
        [XmlArrayItem("DropPrimaryKey")]
        public List<DropPrimaryKey> DropPrimaryKeyList { get; set; } = new();
        public bool ShouldSerializeDropPrimaryKeyList() => DropPrimaryKeyList.Count > 0;

        [XmlArray("DropTableList")]
        [XmlArrayItem("DropTable")]
        public List<DropTable> DropTableList { get; set; } = new();
        public bool ShouldSerializeDropTableList() => DropTableList.Count > 0;

        [XmlArray("DropTableColumnList")]
        [XmlArrayItem("DropTableColumn")]
        public List<DropTableColumn> DropTableColumnList { get; set; } = new();
        public bool ShouldSerializeDropTableColumnList() => DropTableColumnList.Count > 0;

        public static MetaSqlDeployManifestModel LoadFromXmlWorkspace(
            string workspacePath,
            bool searchUpward = true)
        {
            var model = TypedWorkspaceXmlSerializer.Load<MetaSqlDeployManifestModel>(workspacePath, searchUpward);
            MetaSqlDeployManifestModelFactory.Bind(model);
            return model;
        }

        public static Task<MetaSqlDeployManifestModel> LoadFromXmlWorkspaceAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(LoadFromXmlWorkspace(workspacePath, searchUpward));
        }

        public void SaveToXmlWorkspace(string workspacePath)
        {
            MetaSqlDeployManifestModelFactory.Bind(this);
            TypedWorkspaceXmlSerializer.Save(this, workspacePath, ResolveBundledModelXmlPath());
        }

        public Task SaveToXmlWorkspaceAsync(
            string workspacePath,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SaveToXmlWorkspace(workspacePath);
            return Task.CompletedTask;
        }

        private static string? ResolveBundledModelXmlPath()
        {
            var assemblyDirectory = Path.GetDirectoryName(typeof(MetaSqlDeployManifestModel).Assembly.Location);
            if (string.IsNullOrWhiteSpace(assemblyDirectory))
            {
                return null;
            }

            var namespacedPath = Path.Combine(assemblyDirectory, "MetaSqlDeployManifest", "model.xml");
            if (File.Exists(namespacedPath))
            {
                return namespacedPath;
            }

            var directPath = Path.Combine(assemblyDirectory, "model.xml");
            return File.Exists(directPath) ? directPath : null;
        }
    }

    internal static class MetaSqlDeployManifestModelFactory
    {
        internal static void Bind(MetaSqlDeployManifestModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            model.AddForeignKeyList ??= new List<AddForeignKey>();
            model.AddIndexList ??= new List<AddIndex>();
            model.AddPrimaryKeyList ??= new List<AddPrimaryKey>();
            model.AddSchemaList ??= new List<AddSchema>();
            model.AddTableList ??= new List<AddTable>();
            model.AddTableColumnList ??= new List<AddTableColumn>();
            model.AlterTableColumnList ??= new List<AlterTableColumn>();
            model.TruncateTableColumnDataList ??= new List<TruncateTableColumnData>();
            model.BlockForeignKeyDifferenceList ??= new List<BlockForeignKeyDifference>();
            model.BlockIndexDifferenceList ??= new List<BlockIndexDifference>();
            model.BlockPrimaryKeyDifferenceList ??= new List<BlockPrimaryKeyDifference>();
            model.BlockTableColumnDifferenceList ??= new List<BlockTableColumnDifference>();
            model.BlockTableDifferenceList ??= new List<BlockTableDifference>();
            model.DeployManifestList ??= new List<DeployManifest>();
            model.DropForeignKeyList ??= new List<DropForeignKey>();
            model.ReplaceForeignKeyList ??= new List<ReplaceForeignKey>();
            model.ReplacePrimaryKeyList ??= new List<ReplacePrimaryKey>();
            model.ReplaceIndexList ??= new List<ReplaceIndex>();
            model.DropIndexList ??= new List<DropIndex>();
            model.DropPrimaryKeyList ??= new List<DropPrimaryKey>();
            model.DropTableList ??= new List<DropTable>();
            model.DropTableColumnList ??= new List<DropTableColumn>();

            NormalizeAddForeignKeyList(model);
            NormalizeAddIndexList(model);
            NormalizeAddPrimaryKeyList(model);
            NormalizeAddSchemaList(model);
            NormalizeAddTableList(model);
            NormalizeAddTableColumnList(model);
            NormalizeAlterTableColumnList(model);
            NormalizeTruncateTableColumnDataList(model);
            NormalizeBlockForeignKeyDifferenceList(model);
            NormalizeBlockIndexDifferenceList(model);
            NormalizeBlockPrimaryKeyDifferenceList(model);
            NormalizeBlockTableColumnDifferenceList(model);
            NormalizeBlockTableDifferenceList(model);
            NormalizeDeployManifestList(model);
            NormalizeDropForeignKeyList(model);
            NormalizeReplaceForeignKeyList(model);
            NormalizeReplacePrimaryKeyList(model);
            NormalizeReplaceIndexList(model);
            NormalizeDropIndexList(model);
            NormalizeDropPrimaryKeyList(model);
            NormalizeDropTableList(model);
            NormalizeDropTableColumnList(model);

            var addForeignKeyListById = BuildById(model.AddForeignKeyList, row => row.Id, "AddForeignKey");
            var addIndexListById = BuildById(model.AddIndexList, row => row.Id, "AddIndex");
            var addPrimaryKeyListById = BuildById(model.AddPrimaryKeyList, row => row.Id, "AddPrimaryKey");
            var addTableListById = BuildById(model.AddTableList, row => row.Id, "AddTable");
            var addTableColumnListById = BuildById(model.AddTableColumnList, row => row.Id, "AddTableColumn");
            var alterTableColumnListById = BuildById(model.AlterTableColumnList, row => row.Id, "AlterTableColumn");
            var blockForeignKeyDifferenceListById = BuildById(model.BlockForeignKeyDifferenceList, row => row.Id, "BlockForeignKeyDifference");
            var blockIndexDifferenceListById = BuildById(model.BlockIndexDifferenceList, row => row.Id, "BlockIndexDifference");
            var blockPrimaryKeyDifferenceListById = BuildById(model.BlockPrimaryKeyDifferenceList, row => row.Id, "BlockPrimaryKeyDifference");
            var blockTableColumnDifferenceListById = BuildById(model.BlockTableColumnDifferenceList, row => row.Id, "BlockTableColumnDifference");
            var blockTableDifferenceListById = BuildById(model.BlockTableDifferenceList, row => row.Id, "BlockTableDifference");
            var deployManifestListById = BuildById(model.DeployManifestList, row => row.Id, "DeployManifest");
            var dropForeignKeyListById = BuildById(model.DropForeignKeyList, row => row.Id, "DropForeignKey");
            var dropIndexListById = BuildById(model.DropIndexList, row => row.Id, "DropIndex");
            var dropPrimaryKeyListById = BuildById(model.DropPrimaryKeyList, row => row.Id, "DropPrimaryKey");
            var dropTableListById = BuildById(model.DropTableList, row => row.Id, "DropTable");
            var dropTableColumnListById = BuildById(model.DropTableColumnList, row => row.Id, "DropTableColumn");

            foreach (var row in model.AddForeignKeyList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "AddForeignKey",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "AddForeignKey",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.AddIndexList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "AddIndex",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "AddIndex",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.AddPrimaryKeyList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "AddPrimaryKey",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "AddPrimaryKey",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.AddSchemaList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "AddSchema",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "AddSchema",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.AddTableList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "AddTable",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "AddTable",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.AddTableColumnList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "AddTableColumn",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "AddTableColumn",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.AlterTableColumnList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "AlterTableColumn",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "AlterTableColumn",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.TruncateTableColumnDataList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "TruncateTableColumnData",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "TruncateTableColumnData",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.BlockForeignKeyDifferenceList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "BlockForeignKeyDifference",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "BlockForeignKeyDifference",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.BlockIndexDifferenceList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "BlockIndexDifference",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "BlockIndexDifference",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.BlockPrimaryKeyDifferenceList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "BlockPrimaryKeyDifference",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "BlockPrimaryKeyDifference",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.BlockTableColumnDifferenceList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "BlockTableColumnDifference",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "BlockTableColumnDifference",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.BlockTableDifferenceList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "BlockTableDifference",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "BlockTableDifference",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.DropForeignKeyList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "DropForeignKey",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "DropForeignKey",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.ReplaceForeignKeyList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "ReplaceForeignKey",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "ReplaceForeignKey",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.ReplacePrimaryKeyList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "ReplacePrimaryKey",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "ReplacePrimaryKey",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.ReplaceIndexList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "ReplaceIndex",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "ReplaceIndex",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.DropIndexList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "DropIndex",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "DropIndex",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.DropPrimaryKeyList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "DropPrimaryKey",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "DropPrimaryKey",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.DropTableList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "DropTable",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "DropTable",
                    row.Id,
                    "DeployManifestId");
            }

            foreach (var row in model.DropTableColumnList)
            {
                row.DeployManifestId = ResolveRelationshipId(
                    row.DeployManifestId,
                    row.DeployManifest?.Id,
                    "DropTableColumn",
                    row.Id,
                    "DeployManifestId");
                row.DeployManifest = RequireTarget(
                    deployManifestListById,
                    row.DeployManifestId,
                    "DropTableColumn",
                    row.Id,
                    "DeployManifestId");
            }

        }

        private static void NormalizeAddForeignKeyList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.AddForeignKeyList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'AddForeignKey' contains a row with empty Id.");
                row.SourceForeignKeyId = RequireText(row.SourceForeignKeyId, $"Entity 'AddForeignKey' row '{row.Id}' is missing required property 'SourceForeignKeyId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeAddIndexList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.AddIndexList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'AddIndex' contains a row with empty Id.");
                row.SourceIndexId = RequireText(row.SourceIndexId, $"Entity 'AddIndex' row '{row.Id}' is missing required property 'SourceIndexId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeAddPrimaryKeyList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.AddPrimaryKeyList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'AddPrimaryKey' contains a row with empty Id.");
                row.SourcePrimaryKeyId = RequireText(row.SourcePrimaryKeyId, $"Entity 'AddPrimaryKey' row '{row.Id}' is missing required property 'SourcePrimaryKeyId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeAddTableList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.AddTableList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'AddTable' contains a row with empty Id.");
                row.SourceTableId = RequireText(row.SourceTableId, $"Entity 'AddTable' row '{row.Id}' is missing required property 'SourceTableId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeAddSchemaList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.AddSchemaList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'AddSchema' contains a row with empty Id.");
                row.SourceSchemaId = RequireText(row.SourceSchemaId, $"Entity 'AddSchema' row '{row.Id}' is missing required property 'SourceSchemaId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeAddTableColumnList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.AddTableColumnList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'AddTableColumn' contains a row with empty Id.");
                row.SourceTableColumnId = RequireText(row.SourceTableColumnId, $"Entity 'AddTableColumn' row '{row.Id}' is missing required property 'SourceTableColumnId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeAlterTableColumnList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.AlterTableColumnList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'AlterTableColumn' contains a row with empty Id.");
                row.SourceTableColumnId = RequireText(row.SourceTableColumnId, $"Entity 'AlterTableColumn' row '{row.Id}' is missing required property 'SourceTableColumnId'.");
                row.LiveTableColumnId = RequireText(row.LiveTableColumnId, $"Entity 'AlterTableColumn' row '{row.Id}' is missing required property 'LiveTableColumnId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeTruncateTableColumnDataList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.TruncateTableColumnDataList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TruncateTableColumnData' contains a row with empty Id.");
                row.SourceTableColumnId = RequireText(row.SourceTableColumnId, $"Entity 'TruncateTableColumnData' row '{row.Id}' is missing required property 'SourceTableColumnId'.");
                row.LiveTableColumnId = RequireText(row.LiveTableColumnId, $"Entity 'TruncateTableColumnData' row '{row.Id}' is missing required property 'LiveTableColumnId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeBlockForeignKeyDifferenceList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.BlockForeignKeyDifferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BlockForeignKeyDifference' contains a row with empty Id.");
                row.DifferenceSummary = RequireText(row.DifferenceSummary, $"Entity 'BlockForeignKeyDifference' row '{row.Id}' is missing required property 'DifferenceSummary'.");
                row.LiveForeignKeyId = RequireText(row.LiveForeignKeyId, $"Entity 'BlockForeignKeyDifference' row '{row.Id}' is missing required property 'LiveForeignKeyId'.");
                row.SourceForeignKeyId = RequireText(row.SourceForeignKeyId, $"Entity 'BlockForeignKeyDifference' row '{row.Id}' is missing required property 'SourceForeignKeyId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeBlockIndexDifferenceList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.BlockIndexDifferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BlockIndexDifference' contains a row with empty Id.");
                row.DifferenceSummary = RequireText(row.DifferenceSummary, $"Entity 'BlockIndexDifference' row '{row.Id}' is missing required property 'DifferenceSummary'.");
                row.LiveIndexId = RequireText(row.LiveIndexId, $"Entity 'BlockIndexDifference' row '{row.Id}' is missing required property 'LiveIndexId'.");
                row.SourceIndexId = RequireText(row.SourceIndexId, $"Entity 'BlockIndexDifference' row '{row.Id}' is missing required property 'SourceIndexId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeBlockPrimaryKeyDifferenceList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.BlockPrimaryKeyDifferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BlockPrimaryKeyDifference' contains a row with empty Id.");
                row.DifferenceSummary = RequireText(row.DifferenceSummary, $"Entity 'BlockPrimaryKeyDifference' row '{row.Id}' is missing required property 'DifferenceSummary'.");
                row.LivePrimaryKeyId = RequireText(row.LivePrimaryKeyId, $"Entity 'BlockPrimaryKeyDifference' row '{row.Id}' is missing required property 'LivePrimaryKeyId'.");
                row.SourcePrimaryKeyId = RequireText(row.SourcePrimaryKeyId, $"Entity 'BlockPrimaryKeyDifference' row '{row.Id}' is missing required property 'SourcePrimaryKeyId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeBlockTableColumnDifferenceList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.BlockTableColumnDifferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BlockTableColumnDifference' contains a row with empty Id.");
                row.DifferenceSummary = RequireText(row.DifferenceSummary, $"Entity 'BlockTableColumnDifference' row '{row.Id}' is missing required property 'DifferenceSummary'.");
                row.LiveTableColumnId = RequireText(row.LiveTableColumnId, $"Entity 'BlockTableColumnDifference' row '{row.Id}' is missing required property 'LiveTableColumnId'.");
                row.SourceTableColumnId = RequireText(row.SourceTableColumnId, $"Entity 'BlockTableColumnDifference' row '{row.Id}' is missing required property 'SourceTableColumnId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeBlockTableDifferenceList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.BlockTableDifferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BlockTableDifference' contains a row with empty Id.");
                row.DifferenceSummary = RequireText(row.DifferenceSummary, $"Entity 'BlockTableDifference' row '{row.Id}' is missing required property 'DifferenceSummary'.");
                row.LiveTableId = RequireText(row.LiveTableId, $"Entity 'BlockTableDifference' row '{row.Id}' is missing required property 'LiveTableId'.");
                row.SourceTableId = RequireText(row.SourceTableId, $"Entity 'BlockTableDifference' row '{row.Id}' is missing required property 'SourceTableId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeDeployManifestList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.DeployManifestList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'DeployManifest' contains a row with empty Id.");
                row.CreatedUtc = RequireText(row.CreatedUtc, $"Entity 'DeployManifest' row '{row.Id}' is missing required property 'CreatedUtc'.");
                row.LiveInstanceFingerprint = RequireText(row.LiveInstanceFingerprint, $"Entity 'DeployManifest' row '{row.Id}' is missing required property 'LiveInstanceFingerprint'.");
                row.Name = RequireText(row.Name, $"Entity 'DeployManifest' row '{row.Id}' is missing required property 'Name'.");
                row.SourceInstanceFingerprint = RequireText(row.SourceInstanceFingerprint, $"Entity 'DeployManifest' row '{row.Id}' is missing required property 'SourceInstanceFingerprint'.");
                row.TargetDescription ??= string.Empty;
            }
        }

        private static void NormalizeDropForeignKeyList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.DropForeignKeyList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'DropForeignKey' contains a row with empty Id.");
                row.LiveForeignKeyId = RequireText(row.LiveForeignKeyId, $"Entity 'DropForeignKey' row '{row.Id}' is missing required property 'LiveForeignKeyId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeReplaceForeignKeyList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.ReplaceForeignKeyList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ReplaceForeignKey' contains a row with empty Id.");
                row.SourceForeignKeyId = RequireText(row.SourceForeignKeyId, $"Entity 'ReplaceForeignKey' row '{row.Id}' is missing required property 'SourceForeignKeyId'.");
                row.LiveForeignKeyId = RequireText(row.LiveForeignKeyId, $"Entity 'ReplaceForeignKey' row '{row.Id}' is missing required property 'LiveForeignKeyId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeReplacePrimaryKeyList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.ReplacePrimaryKeyList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ReplacePrimaryKey' contains a row with empty Id.");
                row.SourcePrimaryKeyId = RequireText(row.SourcePrimaryKeyId, $"Entity 'ReplacePrimaryKey' row '{row.Id}' is missing required property 'SourcePrimaryKeyId'.");
                row.LivePrimaryKeyId = RequireText(row.LivePrimaryKeyId, $"Entity 'ReplacePrimaryKey' row '{row.Id}' is missing required property 'LivePrimaryKeyId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeReplaceIndexList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.ReplaceIndexList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ReplaceIndex' contains a row with empty Id.");
                row.SourceIndexId = RequireText(row.SourceIndexId, $"Entity 'ReplaceIndex' row '{row.Id}' is missing required property 'SourceIndexId'.");
                row.LiveIndexId = RequireText(row.LiveIndexId, $"Entity 'ReplaceIndex' row '{row.Id}' is missing required property 'LiveIndexId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeDropIndexList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.DropIndexList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'DropIndex' contains a row with empty Id.");
                row.LiveIndexId = RequireText(row.LiveIndexId, $"Entity 'DropIndex' row '{row.Id}' is missing required property 'LiveIndexId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeDropPrimaryKeyList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.DropPrimaryKeyList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'DropPrimaryKey' contains a row with empty Id.");
                row.LivePrimaryKeyId = RequireText(row.LivePrimaryKeyId, $"Entity 'DropPrimaryKey' row '{row.Id}' is missing required property 'LivePrimaryKeyId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeDropTableList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.DropTableList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'DropTable' contains a row with empty Id.");
                row.LiveTableId = RequireText(row.LiveTableId, $"Entity 'DropTable' row '{row.Id}' is missing required property 'LiveTableId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static void NormalizeDropTableColumnList(MetaSqlDeployManifestModel model)
        {
            foreach (var row in model.DropTableColumnList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'DropTableColumn' contains a row with empty Id.");
                row.LiveTableColumnId = RequireText(row.LiveTableColumnId, $"Entity 'DropTableColumn' row '{row.Id}' is missing required property 'LiveTableColumnId'.");
                row.DeployManifestId ??= string.Empty;
            }
        }

        private static Dictionary<string, T> BuildById<T>(
            IEnumerable<T> rows,
            Func<T, string> getId,
            string entityName)
            where T : class
        {
            var rowsById = new Dictionary<string, T>(StringComparer.Ordinal);
            foreach (var row in rows)
            {
                ArgumentNullException.ThrowIfNull(row);
                var id = RequireIdentity(getId(row), $"Entity '{entityName}' contains a row with empty Id.");
                if (!rowsById.TryAdd(id, row))
                {
                    throw new InvalidOperationException($"Entity '{entityName}' contains duplicate Id '{id}'.");
                }
            }

            return rowsById;
        }

        private static T RequireTarget<T>(
            Dictionary<string, T> rowsById,
            string targetId,
            string sourceEntityName,
            string sourceId,
            string relationshipName)
            where T : class
        {
            var normalizedTargetId = RequireIdentity(targetId, $"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' is empty.");
            if (!rowsById.TryGetValue(normalizedTargetId, out var target))
            {
                throw new InvalidOperationException($"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' points to missing Id '{normalizedTargetId}'.");
            }

            return target;
        }

        private static string ResolveRelationshipId(
            string relationshipId,
            string? navigationId,
            string sourceEntityName,
            string sourceId,
            string relationshipName)
        {
            var normalizedRelationshipId = NormalizeIdentity(relationshipId);
            var normalizedNavigationId = NormalizeIdentity(navigationId);
            if (!string.IsNullOrEmpty(normalizedRelationshipId) &&
                !string.IsNullOrEmpty(normalizedNavigationId) &&
                !string.Equals(normalizedRelationshipId, normalizedNavigationId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' conflicts between '{normalizedRelationshipId}' and '{normalizedNavigationId}'.");
            }

            var resolvedTargetId = string.IsNullOrEmpty(normalizedRelationshipId)
                ? normalizedNavigationId
                : normalizedRelationshipId;
            return RequireIdentity(resolvedTargetId, $"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' is empty.");
        }

        private static string RequireIdentity(string? value, string errorMessage)
        {
            var normalizedValue = NormalizeIdentity(value);
            if (string.IsNullOrEmpty(normalizedValue))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return normalizedValue;
        }

        private static string RequireText(string? value, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return value;
        }

        private static string NormalizeIdentity(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim();
        }
    }
}
