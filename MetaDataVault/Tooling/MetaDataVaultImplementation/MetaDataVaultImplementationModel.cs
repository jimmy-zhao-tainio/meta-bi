using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Meta.Core.Serialization;

namespace MetaDataVaultImplementation
{
    [XmlRoot("MetaDataVaultImplementation")]
    public sealed partial class MetaDataVaultImplementationModel
    {
        public static MetaDataVaultImplementationModel CreateEmpty() => new();

        [XmlArray("BusinessBridgeImplementationList")]
        [XmlArrayItem("BusinessBridgeImplementation")]
        public List<BusinessBridgeImplementation> BusinessBridgeImplementationList { get; set; } = new();
        public bool ShouldSerializeBusinessBridgeImplementationList() => BusinessBridgeImplementationList.Count > 0;

        [XmlArray("BusinessHierarchicalLinkImplementationList")]
        [XmlArrayItem("BusinessHierarchicalLinkImplementation")]
        public List<BusinessHierarchicalLinkImplementation> BusinessHierarchicalLinkImplementationList { get; set; } = new();
        public bool ShouldSerializeBusinessHierarchicalLinkImplementationList() => BusinessHierarchicalLinkImplementationList.Count > 0;

        [XmlArray("BusinessHierarchicalLinkSatelliteImplementationList")]
        [XmlArrayItem("BusinessHierarchicalLinkSatelliteImplementation")]
        public List<BusinessHierarchicalLinkSatelliteImplementation> BusinessHierarchicalLinkSatelliteImplementationList { get; set; } = new();
        public bool ShouldSerializeBusinessHierarchicalLinkSatelliteImplementationList() => BusinessHierarchicalLinkSatelliteImplementationList.Count > 0;

        [XmlArray("BusinessHubImplementationList")]
        [XmlArrayItem("BusinessHubImplementation")]
        public List<BusinessHubImplementation> BusinessHubImplementationList { get; set; } = new();
        public bool ShouldSerializeBusinessHubImplementationList() => BusinessHubImplementationList.Count > 0;

        [XmlArray("BusinessHubSatelliteImplementationList")]
        [XmlArrayItem("BusinessHubSatelliteImplementation")]
        public List<BusinessHubSatelliteImplementation> BusinessHubSatelliteImplementationList { get; set; } = new();
        public bool ShouldSerializeBusinessHubSatelliteImplementationList() => BusinessHubSatelliteImplementationList.Count > 0;

        [XmlArray("BusinessLinkImplementationList")]
        [XmlArrayItem("BusinessLinkImplementation")]
        public List<BusinessLinkImplementation> BusinessLinkImplementationList { get; set; } = new();
        public bool ShouldSerializeBusinessLinkImplementationList() => BusinessLinkImplementationList.Count > 0;

        [XmlArray("BusinessLinkSatelliteImplementationList")]
        [XmlArrayItem("BusinessLinkSatelliteImplementation")]
        public List<BusinessLinkSatelliteImplementation> BusinessLinkSatelliteImplementationList { get; set; } = new();
        public bool ShouldSerializeBusinessLinkSatelliteImplementationList() => BusinessLinkSatelliteImplementationList.Count > 0;

        [XmlArray("BusinessPointInTimeImplementationList")]
        [XmlArrayItem("BusinessPointInTimeImplementation")]
        public List<BusinessPointInTimeImplementation> BusinessPointInTimeImplementationList { get; set; } = new();
        public bool ShouldSerializeBusinessPointInTimeImplementationList() => BusinessPointInTimeImplementationList.Count > 0;

        [XmlArray("BusinessReferenceImplementationList")]
        [XmlArrayItem("BusinessReferenceImplementation")]
        public List<BusinessReferenceImplementation> BusinessReferenceImplementationList { get; set; } = new();
        public bool ShouldSerializeBusinessReferenceImplementationList() => BusinessReferenceImplementationList.Count > 0;

        [XmlArray("BusinessReferenceSatelliteImplementationList")]
        [XmlArrayItem("BusinessReferenceSatelliteImplementation")]
        public List<BusinessReferenceSatelliteImplementation> BusinessReferenceSatelliteImplementationList { get; set; } = new();
        public bool ShouldSerializeBusinessReferenceSatelliteImplementationList() => BusinessReferenceSatelliteImplementationList.Count > 0;

        [XmlArray("BusinessSameAsLinkImplementationList")]
        [XmlArrayItem("BusinessSameAsLinkImplementation")]
        public List<BusinessSameAsLinkImplementation> BusinessSameAsLinkImplementationList { get; set; } = new();
        public bool ShouldSerializeBusinessSameAsLinkImplementationList() => BusinessSameAsLinkImplementationList.Count > 0;

        [XmlArray("BusinessSameAsLinkSatelliteImplementationList")]
        [XmlArrayItem("BusinessSameAsLinkSatelliteImplementation")]
        public List<BusinessSameAsLinkSatelliteImplementation> BusinessSameAsLinkSatelliteImplementationList { get; set; } = new();
        public bool ShouldSerializeBusinessSameAsLinkSatelliteImplementationList() => BusinessSameAsLinkSatelliteImplementationList.Count > 0;

        [XmlArray("RawHubImplementationList")]
        [XmlArrayItem("RawHubImplementation")]
        public List<RawHubImplementation> RawHubImplementationList { get; set; } = new();
        public bool ShouldSerializeRawHubImplementationList() => RawHubImplementationList.Count > 0;

        [XmlArray("RawHubSatelliteImplementationList")]
        [XmlArrayItem("RawHubSatelliteImplementation")]
        public List<RawHubSatelliteImplementation> RawHubSatelliteImplementationList { get; set; } = new();
        public bool ShouldSerializeRawHubSatelliteImplementationList() => RawHubSatelliteImplementationList.Count > 0;

        [XmlArray("RawLinkImplementationList")]
        [XmlArrayItem("RawLinkImplementation")]
        public List<RawLinkImplementation> RawLinkImplementationList { get; set; } = new();
        public bool ShouldSerializeRawLinkImplementationList() => RawLinkImplementationList.Count > 0;

        [XmlArray("RawLinkSatelliteImplementationList")]
        [XmlArrayItem("RawLinkSatelliteImplementation")]
        public List<RawLinkSatelliteImplementation> RawLinkSatelliteImplementationList { get; set; } = new();
        public bool ShouldSerializeRawLinkSatelliteImplementationList() => RawLinkSatelliteImplementationList.Count > 0;

        public static MetaDataVaultImplementationModel LoadFromXmlWorkspace(
            string workspacePath,
            bool searchUpward = true)
        {
            var model = TypedWorkspaceXmlSerializer.Load<MetaDataVaultImplementationModel>(workspacePath, searchUpward);
            MetaDataVaultImplementationModelFactory.Bind(model);
            return model;
        }

        public static Task<MetaDataVaultImplementationModel> LoadFromXmlWorkspaceAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(LoadFromXmlWorkspace(workspacePath, searchUpward));
        }

        public void SaveToXmlWorkspace(string workspacePath)
        {
            MetaDataVaultImplementationModelFactory.Bind(this);
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
            var assemblyDirectory = Path.GetDirectoryName(typeof(MetaDataVaultImplementationModel).Assembly.Location);
            if (string.IsNullOrWhiteSpace(assemblyDirectory))
            {
                return null;
            }

            var directPath = Path.Combine(assemblyDirectory, "model.xml");
            if (File.Exists(directPath))
            {
                return directPath;
            }

            var namespacedPath = Path.Combine(assemblyDirectory, "MetaDataVaultImplementation", "model.xml");
            return File.Exists(namespacedPath) ? namespacedPath : null;
        }
    }

    internal static class MetaDataVaultImplementationModelFactory
    {
        internal static void Bind(MetaDataVaultImplementationModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            model.BusinessBridgeImplementationList ??= new List<BusinessBridgeImplementation>();
            model.BusinessHierarchicalLinkImplementationList ??= new List<BusinessHierarchicalLinkImplementation>();
            model.BusinessHierarchicalLinkSatelliteImplementationList ??= new List<BusinessHierarchicalLinkSatelliteImplementation>();
            model.BusinessHubImplementationList ??= new List<BusinessHubImplementation>();
            model.BusinessHubSatelliteImplementationList ??= new List<BusinessHubSatelliteImplementation>();
            model.BusinessLinkImplementationList ??= new List<BusinessLinkImplementation>();
            model.BusinessLinkSatelliteImplementationList ??= new List<BusinessLinkSatelliteImplementation>();
            model.BusinessPointInTimeImplementationList ??= new List<BusinessPointInTimeImplementation>();
            model.BusinessReferenceImplementationList ??= new List<BusinessReferenceImplementation>();
            model.BusinessReferenceSatelliteImplementationList ??= new List<BusinessReferenceSatelliteImplementation>();
            model.BusinessSameAsLinkImplementationList ??= new List<BusinessSameAsLinkImplementation>();
            model.BusinessSameAsLinkSatelliteImplementationList ??= new List<BusinessSameAsLinkSatelliteImplementation>();
            model.RawHubImplementationList ??= new List<RawHubImplementation>();
            model.RawHubSatelliteImplementationList ??= new List<RawHubSatelliteImplementation>();
            model.RawLinkImplementationList ??= new List<RawLinkImplementation>();
            model.RawLinkSatelliteImplementationList ??= new List<RawLinkSatelliteImplementation>();

            NormalizeBusinessBridgeImplementationList(model);
            NormalizeBusinessHierarchicalLinkImplementationList(model);
            NormalizeBusinessHierarchicalLinkSatelliteImplementationList(model);
            NormalizeBusinessHubImplementationList(model);
            NormalizeBusinessHubSatelliteImplementationList(model);
            NormalizeBusinessLinkImplementationList(model);
            NormalizeBusinessLinkSatelliteImplementationList(model);
            NormalizeBusinessPointInTimeImplementationList(model);
            NormalizeBusinessReferenceImplementationList(model);
            NormalizeBusinessReferenceSatelliteImplementationList(model);
            NormalizeBusinessSameAsLinkImplementationList(model);
            NormalizeBusinessSameAsLinkSatelliteImplementationList(model);
            NormalizeRawHubImplementationList(model);
            NormalizeRawHubSatelliteImplementationList(model);
            NormalizeRawLinkImplementationList(model);
            NormalizeRawLinkSatelliteImplementationList(model);

            var businessBridgeImplementationListById = BuildById(model.BusinessBridgeImplementationList, row => row.Id, "BusinessBridgeImplementation");
            var businessHierarchicalLinkImplementationListById = BuildById(model.BusinessHierarchicalLinkImplementationList, row => row.Id, "BusinessHierarchicalLinkImplementation");
            var businessHierarchicalLinkSatelliteImplementationListById = BuildById(model.BusinessHierarchicalLinkSatelliteImplementationList, row => row.Id, "BusinessHierarchicalLinkSatelliteImplementation");
            var businessHubImplementationListById = BuildById(model.BusinessHubImplementationList, row => row.Id, "BusinessHubImplementation");
            var businessHubSatelliteImplementationListById = BuildById(model.BusinessHubSatelliteImplementationList, row => row.Id, "BusinessHubSatelliteImplementation");
            var businessLinkImplementationListById = BuildById(model.BusinessLinkImplementationList, row => row.Id, "BusinessLinkImplementation");
            var businessLinkSatelliteImplementationListById = BuildById(model.BusinessLinkSatelliteImplementationList, row => row.Id, "BusinessLinkSatelliteImplementation");
            var businessPointInTimeImplementationListById = BuildById(model.BusinessPointInTimeImplementationList, row => row.Id, "BusinessPointInTimeImplementation");
            var businessReferenceImplementationListById = BuildById(model.BusinessReferenceImplementationList, row => row.Id, "BusinessReferenceImplementation");
            var businessReferenceSatelliteImplementationListById = BuildById(model.BusinessReferenceSatelliteImplementationList, row => row.Id, "BusinessReferenceSatelliteImplementation");
            var businessSameAsLinkImplementationListById = BuildById(model.BusinessSameAsLinkImplementationList, row => row.Id, "BusinessSameAsLinkImplementation");
            var businessSameAsLinkSatelliteImplementationListById = BuildById(model.BusinessSameAsLinkSatelliteImplementationList, row => row.Id, "BusinessSameAsLinkSatelliteImplementation");
            var rawHubImplementationListById = BuildById(model.RawHubImplementationList, row => row.Id, "RawHubImplementation");
            var rawHubSatelliteImplementationListById = BuildById(model.RawHubSatelliteImplementationList, row => row.Id, "RawHubSatelliteImplementation");
            var rawLinkImplementationListById = BuildById(model.RawLinkImplementationList, row => row.Id, "RawLinkImplementation");
            var rawLinkSatelliteImplementationListById = BuildById(model.RawLinkSatelliteImplementationList, row => row.Id, "RawLinkSatelliteImplementation");
        }

        private static void NormalizeBusinessBridgeImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.BusinessBridgeImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessBridgeImplementation' contains a row with empty Id.");
                row.AnchorHubForeignKeyNamePattern = RequireText(row.AnchorHubForeignKeyNamePattern, $"Entity 'BusinessBridgeImplementation' row '{row.Id}' is missing required property 'AnchorHubForeignKeyNamePattern'.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'BusinessBridgeImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'BusinessBridgeImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.DepthColumnName ??= string.Empty;
                row.DepthDataTypeId ??= string.Empty;
                row.EffectiveFromColumnName ??= string.Empty;
                row.EffectiveFromDataTypeId ??= string.Empty;
                row.EffectiveFromPrecision ??= string.Empty;
                row.EffectiveToColumnName ??= string.Empty;
                row.EffectiveToDataTypeId ??= string.Empty;
                row.EffectiveToPrecision ??= string.Empty;
                row.PathColumnName ??= string.Empty;
                row.PathDataTypeId ??= string.Empty;
                row.PathLength ??= string.Empty;
                row.RelatedHashKeyColumnName = RequireText(row.RelatedHashKeyColumnName, $"Entity 'BusinessBridgeImplementation' row '{row.Id}' is missing required property 'RelatedHashKeyColumnName'.");
                row.RelatedHashKeyDataTypeId = RequireText(row.RelatedHashKeyDataTypeId, $"Entity 'BusinessBridgeImplementation' row '{row.Id}' is missing required property 'RelatedHashKeyDataTypeId'.");
                row.RelatedHashKeyLength = RequireText(row.RelatedHashKeyLength, $"Entity 'BusinessBridgeImplementation' row '{row.Id}' is missing required property 'RelatedHashKeyLength'.");
                row.RootHashKeyColumnName = RequireText(row.RootHashKeyColumnName, $"Entity 'BusinessBridgeImplementation' row '{row.Id}' is missing required property 'RootHashKeyColumnName'.");
                row.RootHashKeyDataTypeId = RequireText(row.RootHashKeyDataTypeId, $"Entity 'BusinessBridgeImplementation' row '{row.Id}' is missing required property 'RootHashKeyDataTypeId'.");
                row.RootHashKeyLength = RequireText(row.RootHashKeyLength, $"Entity 'BusinessBridgeImplementation' row '{row.Id}' is missing required property 'RootHashKeyLength'.");
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'BusinessBridgeImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeBusinessHierarchicalLinkImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.BusinessHierarchicalLinkImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessHierarchicalLinkImplementation' contains a row with empty Id.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'BusinessHierarchicalLinkImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'BusinessHierarchicalLinkImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.ChildHashKeyColumnName = RequireText(row.ChildHashKeyColumnName, $"Entity 'BusinessHierarchicalLinkImplementation' row '{row.Id}' is missing required property 'ChildHashKeyColumnName'.");
                row.ChildHubForeignKeyNamePattern = RequireText(row.ChildHubForeignKeyNamePattern, $"Entity 'BusinessHierarchicalLinkImplementation' row '{row.Id}' is missing required property 'ChildHubForeignKeyNamePattern'.");
                row.HashKeyColumnName = RequireText(row.HashKeyColumnName, $"Entity 'BusinessHierarchicalLinkImplementation' row '{row.Id}' is missing required property 'HashKeyColumnName'.");
                row.HashKeyDataTypeId = RequireText(row.HashKeyDataTypeId, $"Entity 'BusinessHierarchicalLinkImplementation' row '{row.Id}' is missing required property 'HashKeyDataTypeId'.");
                row.HashKeyLength = RequireText(row.HashKeyLength, $"Entity 'BusinessHierarchicalLinkImplementation' row '{row.Id}' is missing required property 'HashKeyLength'.");
                row.LoadTimestampColumnName ??= string.Empty;
                row.LoadTimestampDataTypeId ??= string.Empty;
                row.LoadTimestampPrecision ??= string.Empty;
                row.ParentHashKeyColumnName = RequireText(row.ParentHashKeyColumnName, $"Entity 'BusinessHierarchicalLinkImplementation' row '{row.Id}' is missing required property 'ParentHashKeyColumnName'.");
                row.ParentHubForeignKeyNamePattern = RequireText(row.ParentHubForeignKeyNamePattern, $"Entity 'BusinessHierarchicalLinkImplementation' row '{row.Id}' is missing required property 'ParentHubForeignKeyNamePattern'.");
                row.PrimaryKeyNamePattern = RequireText(row.PrimaryKeyNamePattern, $"Entity 'BusinessHierarchicalLinkImplementation' row '{row.Id}' is missing required property 'PrimaryKeyNamePattern'.");
                row.RecordSourceColumnName ??= string.Empty;
                row.RecordSourceDataTypeId ??= string.Empty;
                row.RecordSourceLength ??= string.Empty;
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'BusinessHierarchicalLinkImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeBusinessHierarchicalLinkSatelliteImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.BusinessHierarchicalLinkSatelliteImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessHierarchicalLinkSatelliteImplementation' contains a row with empty Id.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'BusinessHierarchicalLinkSatelliteImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'BusinessHierarchicalLinkSatelliteImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.HashDiffColumnName ??= string.Empty;
                row.HashDiffDataTypeId ??= string.Empty;
                row.HashDiffLength ??= string.Empty;
                row.LoadTimestampColumnName ??= string.Empty;
                row.LoadTimestampDataTypeId ??= string.Empty;
                row.LoadTimestampPrecision ??= string.Empty;
                row.ParentForeignKeyNamePattern = RequireText(row.ParentForeignKeyNamePattern, $"Entity 'BusinessHierarchicalLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentForeignKeyNamePattern'.");
                row.ParentHashKeyColumnName = RequireText(row.ParentHashKeyColumnName, $"Entity 'BusinessHierarchicalLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyColumnName'.");
                row.ParentHashKeyDataTypeId = RequireText(row.ParentHashKeyDataTypeId, $"Entity 'BusinessHierarchicalLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyDataTypeId'.");
                row.ParentHashKeyLength = RequireText(row.ParentHashKeyLength, $"Entity 'BusinessHierarchicalLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyLength'.");
                row.RecordSourceColumnName ??= string.Empty;
                row.RecordSourceDataTypeId ??= string.Empty;
                row.RecordSourceLength ??= string.Empty;
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'BusinessHierarchicalLinkSatelliteImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeBusinessHubImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.BusinessHubImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessHubImplementation' contains a row with empty Id.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'BusinessHubImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'BusinessHubImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.HashKeyColumnName = RequireText(row.HashKeyColumnName, $"Entity 'BusinessHubImplementation' row '{row.Id}' is missing required property 'HashKeyColumnName'.");
                row.HashKeyDataTypeId = RequireText(row.HashKeyDataTypeId, $"Entity 'BusinessHubImplementation' row '{row.Id}' is missing required property 'HashKeyDataTypeId'.");
                row.HashKeyLength = RequireText(row.HashKeyLength, $"Entity 'BusinessHubImplementation' row '{row.Id}' is missing required property 'HashKeyLength'.");
                row.LoadTimestampColumnName ??= string.Empty;
                row.LoadTimestampDataTypeId ??= string.Empty;
                row.LoadTimestampPrecision ??= string.Empty;
                row.PrimaryKeyNamePattern = RequireText(row.PrimaryKeyNamePattern, $"Entity 'BusinessHubImplementation' row '{row.Id}' is missing required property 'PrimaryKeyNamePattern'.");
                row.RecordSourceColumnName ??= string.Empty;
                row.RecordSourceDataTypeId ??= string.Empty;
                row.RecordSourceLength ??= string.Empty;
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'BusinessHubImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeBusinessHubSatelliteImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.BusinessHubSatelliteImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessHubSatelliteImplementation' contains a row with empty Id.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'BusinessHubSatelliteImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'BusinessHubSatelliteImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.HashDiffColumnName ??= string.Empty;
                row.HashDiffDataTypeId ??= string.Empty;
                row.HashDiffLength ??= string.Empty;
                row.LoadTimestampColumnName ??= string.Empty;
                row.LoadTimestampDataTypeId ??= string.Empty;
                row.LoadTimestampPrecision ??= string.Empty;
                row.ParentForeignKeyNamePattern = RequireText(row.ParentForeignKeyNamePattern, $"Entity 'BusinessHubSatelliteImplementation' row '{row.Id}' is missing required property 'ParentForeignKeyNamePattern'.");
                row.ParentHashKeyColumnName = RequireText(row.ParentHashKeyColumnName, $"Entity 'BusinessHubSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyColumnName'.");
                row.ParentHashKeyDataTypeId = RequireText(row.ParentHashKeyDataTypeId, $"Entity 'BusinessHubSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyDataTypeId'.");
                row.ParentHashKeyLength = RequireText(row.ParentHashKeyLength, $"Entity 'BusinessHubSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyLength'.");
                row.RecordSourceColumnName ??= string.Empty;
                row.RecordSourceDataTypeId ??= string.Empty;
                row.RecordSourceLength ??= string.Empty;
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'BusinessHubSatelliteImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeBusinessLinkImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.BusinessLinkImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessLinkImplementation' contains a row with empty Id.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'BusinessLinkImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'BusinessLinkImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.EndHashKeyColumnPattern = RequireText(row.EndHashKeyColumnPattern, $"Entity 'BusinessLinkImplementation' row '{row.Id}' is missing required property 'EndHashKeyColumnPattern'.");
                row.HashKeyColumnName = RequireText(row.HashKeyColumnName, $"Entity 'BusinessLinkImplementation' row '{row.Id}' is missing required property 'HashKeyColumnName'.");
                row.HashKeyDataTypeId = RequireText(row.HashKeyDataTypeId, $"Entity 'BusinessLinkImplementation' row '{row.Id}' is missing required property 'HashKeyDataTypeId'.");
                row.HashKeyLength = RequireText(row.HashKeyLength, $"Entity 'BusinessLinkImplementation' row '{row.Id}' is missing required property 'HashKeyLength'.");
                row.HubForeignKeyNamePattern = RequireText(row.HubForeignKeyNamePattern, $"Entity 'BusinessLinkImplementation' row '{row.Id}' is missing required property 'HubForeignKeyNamePattern'.");
                row.LoadTimestampColumnName ??= string.Empty;
                row.LoadTimestampDataTypeId ??= string.Empty;
                row.LoadTimestampPrecision ??= string.Empty;
                row.PrimaryKeyNamePattern = RequireText(row.PrimaryKeyNamePattern, $"Entity 'BusinessLinkImplementation' row '{row.Id}' is missing required property 'PrimaryKeyNamePattern'.");
                row.RecordSourceColumnName ??= string.Empty;
                row.RecordSourceDataTypeId ??= string.Empty;
                row.RecordSourceLength ??= string.Empty;
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'BusinessLinkImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeBusinessLinkSatelliteImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.BusinessLinkSatelliteImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessLinkSatelliteImplementation' contains a row with empty Id.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'BusinessLinkSatelliteImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'BusinessLinkSatelliteImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.HashDiffColumnName ??= string.Empty;
                row.HashDiffDataTypeId ??= string.Empty;
                row.HashDiffLength ??= string.Empty;
                row.LoadTimestampColumnName ??= string.Empty;
                row.LoadTimestampDataTypeId ??= string.Empty;
                row.LoadTimestampPrecision ??= string.Empty;
                row.ParentForeignKeyNamePattern = RequireText(row.ParentForeignKeyNamePattern, $"Entity 'BusinessLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentForeignKeyNamePattern'.");
                row.ParentHashKeyColumnName = RequireText(row.ParentHashKeyColumnName, $"Entity 'BusinessLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyColumnName'.");
                row.ParentHashKeyDataTypeId = RequireText(row.ParentHashKeyDataTypeId, $"Entity 'BusinessLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyDataTypeId'.");
                row.ParentHashKeyLength = RequireText(row.ParentHashKeyLength, $"Entity 'BusinessLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyLength'.");
                row.RecordSourceColumnName ??= string.Empty;
                row.RecordSourceDataTypeId ??= string.Empty;
                row.RecordSourceLength ??= string.Empty;
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'BusinessLinkSatelliteImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeBusinessPointInTimeImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.BusinessPointInTimeImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessPointInTimeImplementation' contains a row with empty Id.");
                row.AnchorHubForeignKeyNamePattern = RequireText(row.AnchorHubForeignKeyNamePattern, $"Entity 'BusinessPointInTimeImplementation' row '{row.Id}' is missing required property 'AnchorHubForeignKeyNamePattern'.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'BusinessPointInTimeImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'BusinessPointInTimeImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.ParentHashKeyColumnName = RequireText(row.ParentHashKeyColumnName, $"Entity 'BusinessPointInTimeImplementation' row '{row.Id}' is missing required property 'ParentHashKeyColumnName'.");
                row.ParentHashKeyDataTypeId = RequireText(row.ParentHashKeyDataTypeId, $"Entity 'BusinessPointInTimeImplementation' row '{row.Id}' is missing required property 'ParentHashKeyDataTypeId'.");
                row.ParentHashKeyLength = RequireText(row.ParentHashKeyLength, $"Entity 'BusinessPointInTimeImplementation' row '{row.Id}' is missing required property 'ParentHashKeyLength'.");
                row.SatelliteReferenceColumnNamePattern = RequireText(row.SatelliteReferenceColumnNamePattern, $"Entity 'BusinessPointInTimeImplementation' row '{row.Id}' is missing required property 'SatelliteReferenceColumnNamePattern'.");
                row.SatelliteReferenceDataTypeId = RequireText(row.SatelliteReferenceDataTypeId, $"Entity 'BusinessPointInTimeImplementation' row '{row.Id}' is missing required property 'SatelliteReferenceDataTypeId'.");
                row.SatelliteReferencePrecision = RequireText(row.SatelliteReferencePrecision, $"Entity 'BusinessPointInTimeImplementation' row '{row.Id}' is missing required property 'SatelliteReferencePrecision'.");
                row.SnapshotTimestampColumnName = RequireText(row.SnapshotTimestampColumnName, $"Entity 'BusinessPointInTimeImplementation' row '{row.Id}' is missing required property 'SnapshotTimestampColumnName'.");
                row.SnapshotTimestampDataTypeId = RequireText(row.SnapshotTimestampDataTypeId, $"Entity 'BusinessPointInTimeImplementation' row '{row.Id}' is missing required property 'SnapshotTimestampDataTypeId'.");
                row.SnapshotTimestampPrecision = RequireText(row.SnapshotTimestampPrecision, $"Entity 'BusinessPointInTimeImplementation' row '{row.Id}' is missing required property 'SnapshotTimestampPrecision'.");
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'BusinessPointInTimeImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeBusinessReferenceImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.BusinessReferenceImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessReferenceImplementation' contains a row with empty Id.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'BusinessReferenceImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'BusinessReferenceImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.HashKeyColumnName = RequireText(row.HashKeyColumnName, $"Entity 'BusinessReferenceImplementation' row '{row.Id}' is missing required property 'HashKeyColumnName'.");
                row.HashKeyDataTypeId = RequireText(row.HashKeyDataTypeId, $"Entity 'BusinessReferenceImplementation' row '{row.Id}' is missing required property 'HashKeyDataTypeId'.");
                row.HashKeyLength = RequireText(row.HashKeyLength, $"Entity 'BusinessReferenceImplementation' row '{row.Id}' is missing required property 'HashKeyLength'.");
                row.LoadTimestampColumnName ??= string.Empty;
                row.LoadTimestampDataTypeId ??= string.Empty;
                row.LoadTimestampPrecision ??= string.Empty;
                row.PrimaryKeyNamePattern = RequireText(row.PrimaryKeyNamePattern, $"Entity 'BusinessReferenceImplementation' row '{row.Id}' is missing required property 'PrimaryKeyNamePattern'.");
                row.RecordSourceColumnName ??= string.Empty;
                row.RecordSourceDataTypeId ??= string.Empty;
                row.RecordSourceLength ??= string.Empty;
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'BusinessReferenceImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeBusinessReferenceSatelliteImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.BusinessReferenceSatelliteImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessReferenceSatelliteImplementation' contains a row with empty Id.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'BusinessReferenceSatelliteImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'BusinessReferenceSatelliteImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.HashDiffColumnName ??= string.Empty;
                row.HashDiffDataTypeId ??= string.Empty;
                row.HashDiffLength ??= string.Empty;
                row.LoadTimestampColumnName ??= string.Empty;
                row.LoadTimestampDataTypeId ??= string.Empty;
                row.LoadTimestampPrecision ??= string.Empty;
                row.ParentForeignKeyNamePattern = RequireText(row.ParentForeignKeyNamePattern, $"Entity 'BusinessReferenceSatelliteImplementation' row '{row.Id}' is missing required property 'ParentForeignKeyNamePattern'.");
                row.ParentHashKeyColumnName = RequireText(row.ParentHashKeyColumnName, $"Entity 'BusinessReferenceSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyColumnName'.");
                row.ParentHashKeyDataTypeId = RequireText(row.ParentHashKeyDataTypeId, $"Entity 'BusinessReferenceSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyDataTypeId'.");
                row.ParentHashKeyLength = RequireText(row.ParentHashKeyLength, $"Entity 'BusinessReferenceSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyLength'.");
                row.RecordSourceColumnName ??= string.Empty;
                row.RecordSourceDataTypeId ??= string.Empty;
                row.RecordSourceLength ??= string.Empty;
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'BusinessReferenceSatelliteImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeBusinessSameAsLinkImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.BusinessSameAsLinkImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessSameAsLinkImplementation' contains a row with empty Id.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'BusinessSameAsLinkImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'BusinessSameAsLinkImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.EquivalentHashKeyColumnName = RequireText(row.EquivalentHashKeyColumnName, $"Entity 'BusinessSameAsLinkImplementation' row '{row.Id}' is missing required property 'EquivalentHashKeyColumnName'.");
                row.EquivalentHubForeignKeyNamePattern = RequireText(row.EquivalentHubForeignKeyNamePattern, $"Entity 'BusinessSameAsLinkImplementation' row '{row.Id}' is missing required property 'EquivalentHubForeignKeyNamePattern'.");
                row.HashKeyColumnName = RequireText(row.HashKeyColumnName, $"Entity 'BusinessSameAsLinkImplementation' row '{row.Id}' is missing required property 'HashKeyColumnName'.");
                row.HashKeyDataTypeId = RequireText(row.HashKeyDataTypeId, $"Entity 'BusinessSameAsLinkImplementation' row '{row.Id}' is missing required property 'HashKeyDataTypeId'.");
                row.HashKeyLength = RequireText(row.HashKeyLength, $"Entity 'BusinessSameAsLinkImplementation' row '{row.Id}' is missing required property 'HashKeyLength'.");
                row.LoadTimestampColumnName ??= string.Empty;
                row.LoadTimestampDataTypeId ??= string.Empty;
                row.LoadTimestampPrecision ??= string.Empty;
                row.PrimaryHashKeyColumnName = RequireText(row.PrimaryHashKeyColumnName, $"Entity 'BusinessSameAsLinkImplementation' row '{row.Id}' is missing required property 'PrimaryHashKeyColumnName'.");
                row.PrimaryHubForeignKeyNamePattern = RequireText(row.PrimaryHubForeignKeyNamePattern, $"Entity 'BusinessSameAsLinkImplementation' row '{row.Id}' is missing required property 'PrimaryHubForeignKeyNamePattern'.");
                row.PrimaryKeyNamePattern = RequireText(row.PrimaryKeyNamePattern, $"Entity 'BusinessSameAsLinkImplementation' row '{row.Id}' is missing required property 'PrimaryKeyNamePattern'.");
                row.RecordSourceColumnName ??= string.Empty;
                row.RecordSourceDataTypeId ??= string.Empty;
                row.RecordSourceLength ??= string.Empty;
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'BusinessSameAsLinkImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeBusinessSameAsLinkSatelliteImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.BusinessSameAsLinkSatelliteImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessSameAsLinkSatelliteImplementation' contains a row with empty Id.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'BusinessSameAsLinkSatelliteImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'BusinessSameAsLinkSatelliteImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.HashDiffColumnName ??= string.Empty;
                row.HashDiffDataTypeId ??= string.Empty;
                row.HashDiffLength ??= string.Empty;
                row.LoadTimestampColumnName ??= string.Empty;
                row.LoadTimestampDataTypeId ??= string.Empty;
                row.LoadTimestampPrecision ??= string.Empty;
                row.ParentForeignKeyNamePattern = RequireText(row.ParentForeignKeyNamePattern, $"Entity 'BusinessSameAsLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentForeignKeyNamePattern'.");
                row.ParentHashKeyColumnName = RequireText(row.ParentHashKeyColumnName, $"Entity 'BusinessSameAsLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyColumnName'.");
                row.ParentHashKeyDataTypeId = RequireText(row.ParentHashKeyDataTypeId, $"Entity 'BusinessSameAsLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyDataTypeId'.");
                row.ParentHashKeyLength = RequireText(row.ParentHashKeyLength, $"Entity 'BusinessSameAsLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyLength'.");
                row.RecordSourceColumnName ??= string.Empty;
                row.RecordSourceDataTypeId ??= string.Empty;
                row.RecordSourceLength ??= string.Empty;
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'BusinessSameAsLinkSatelliteImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeRawHubImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.RawHubImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RawHubImplementation' contains a row with empty Id.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'RawHubImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'RawHubImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.HashKeyColumnName = RequireText(row.HashKeyColumnName, $"Entity 'RawHubImplementation' row '{row.Id}' is missing required property 'HashKeyColumnName'.");
                row.HashKeyDataTypeId = RequireText(row.HashKeyDataTypeId, $"Entity 'RawHubImplementation' row '{row.Id}' is missing required property 'HashKeyDataTypeId'.");
                row.HashKeyLength = RequireText(row.HashKeyLength, $"Entity 'RawHubImplementation' row '{row.Id}' is missing required property 'HashKeyLength'.");
                row.LoadTimestampColumnName = RequireText(row.LoadTimestampColumnName, $"Entity 'RawHubImplementation' row '{row.Id}' is missing required property 'LoadTimestampColumnName'.");
                row.LoadTimestampDataTypeId = RequireText(row.LoadTimestampDataTypeId, $"Entity 'RawHubImplementation' row '{row.Id}' is missing required property 'LoadTimestampDataTypeId'.");
                row.LoadTimestampPrecision = RequireText(row.LoadTimestampPrecision, $"Entity 'RawHubImplementation' row '{row.Id}' is missing required property 'LoadTimestampPrecision'.");
                row.PrimaryKeyNamePattern = RequireText(row.PrimaryKeyNamePattern, $"Entity 'RawHubImplementation' row '{row.Id}' is missing required property 'PrimaryKeyNamePattern'.");
                row.RecordSourceColumnName = RequireText(row.RecordSourceColumnName, $"Entity 'RawHubImplementation' row '{row.Id}' is missing required property 'RecordSourceColumnName'.");
                row.RecordSourceDataTypeId = RequireText(row.RecordSourceDataTypeId, $"Entity 'RawHubImplementation' row '{row.Id}' is missing required property 'RecordSourceDataTypeId'.");
                row.RecordSourceLength = RequireText(row.RecordSourceLength, $"Entity 'RawHubImplementation' row '{row.Id}' is missing required property 'RecordSourceLength'.");
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'RawHubImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeRawHubSatelliteImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.RawHubSatelliteImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RawHubSatelliteImplementation' contains a row with empty Id.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.HashDiffColumnName = RequireText(row.HashDiffColumnName, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'HashDiffColumnName'.");
                row.HashDiffDataTypeId = RequireText(row.HashDiffDataTypeId, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'HashDiffDataTypeId'.");
                row.HashDiffLength = RequireText(row.HashDiffLength, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'HashDiffLength'.");
                row.LoadTimestampColumnName = RequireText(row.LoadTimestampColumnName, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'LoadTimestampColumnName'.");
                row.LoadTimestampDataTypeId = RequireText(row.LoadTimestampDataTypeId, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'LoadTimestampDataTypeId'.");
                row.LoadTimestampPrecision = RequireText(row.LoadTimestampPrecision, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'LoadTimestampPrecision'.");
                row.ParentForeignKeyNamePattern = RequireText(row.ParentForeignKeyNamePattern, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'ParentForeignKeyNamePattern'.");
                row.ParentHashKeyColumnName = RequireText(row.ParentHashKeyColumnName, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyColumnName'.");
                row.ParentHashKeyDataTypeId = RequireText(row.ParentHashKeyDataTypeId, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyDataTypeId'.");
                row.ParentHashKeyLength = RequireText(row.ParentHashKeyLength, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyLength'.");
                row.RecordSourceColumnName = RequireText(row.RecordSourceColumnName, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'RecordSourceColumnName'.");
                row.RecordSourceDataTypeId = RequireText(row.RecordSourceDataTypeId, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'RecordSourceDataTypeId'.");
                row.RecordSourceLength = RequireText(row.RecordSourceLength, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'RecordSourceLength'.");
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'RawHubSatelliteImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeRawLinkImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.RawLinkImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RawLinkImplementation' contains a row with empty Id.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.EndHashKeyColumnPattern = RequireText(row.EndHashKeyColumnPattern, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'EndHashKeyColumnPattern'.");
                row.HashKeyColumnName = RequireText(row.HashKeyColumnName, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'HashKeyColumnName'.");
                row.HashKeyDataTypeId = RequireText(row.HashKeyDataTypeId, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'HashKeyDataTypeId'.");
                row.HashKeyLength = RequireText(row.HashKeyLength, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'HashKeyLength'.");
                row.HubForeignKeyNamePattern = RequireText(row.HubForeignKeyNamePattern, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'HubForeignKeyNamePattern'.");
                row.LoadTimestampColumnName = RequireText(row.LoadTimestampColumnName, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'LoadTimestampColumnName'.");
                row.LoadTimestampDataTypeId = RequireText(row.LoadTimestampDataTypeId, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'LoadTimestampDataTypeId'.");
                row.LoadTimestampPrecision = RequireText(row.LoadTimestampPrecision, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'LoadTimestampPrecision'.");
                row.PrimaryKeyNamePattern = RequireText(row.PrimaryKeyNamePattern, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'PrimaryKeyNamePattern'.");
                row.RecordSourceColumnName = RequireText(row.RecordSourceColumnName, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'RecordSourceColumnName'.");
                row.RecordSourceDataTypeId = RequireText(row.RecordSourceDataTypeId, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'RecordSourceDataTypeId'.");
                row.RecordSourceLength = RequireText(row.RecordSourceLength, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'RecordSourceLength'.");
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'RawLinkImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
            }
        }

        private static void NormalizeRawLinkSatelliteImplementationList(MetaDataVaultImplementationModel model)
        {
            foreach (var row in model.RawLinkSatelliteImplementationList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RawLinkSatelliteImplementation' contains a row with empty Id.");
                row.AuditIdColumnName = RequireText(row.AuditIdColumnName, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'AuditIdColumnName'.");
                row.AuditIdDataTypeId = RequireText(row.AuditIdDataTypeId, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'AuditIdDataTypeId'.");
                row.HashDiffColumnName = RequireText(row.HashDiffColumnName, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'HashDiffColumnName'.");
                row.HashDiffDataTypeId = RequireText(row.HashDiffDataTypeId, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'HashDiffDataTypeId'.");
                row.HashDiffLength = RequireText(row.HashDiffLength, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'HashDiffLength'.");
                row.LoadTimestampColumnName = RequireText(row.LoadTimestampColumnName, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'LoadTimestampColumnName'.");
                row.LoadTimestampDataTypeId = RequireText(row.LoadTimestampDataTypeId, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'LoadTimestampDataTypeId'.");
                row.LoadTimestampPrecision = RequireText(row.LoadTimestampPrecision, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'LoadTimestampPrecision'.");
                row.ParentForeignKeyNamePattern = RequireText(row.ParentForeignKeyNamePattern, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentForeignKeyNamePattern'.");
                row.ParentHashKeyColumnName = RequireText(row.ParentHashKeyColumnName, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyColumnName'.");
                row.ParentHashKeyDataTypeId = RequireText(row.ParentHashKeyDataTypeId, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyDataTypeId'.");
                row.ParentHashKeyLength = RequireText(row.ParentHashKeyLength, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'ParentHashKeyLength'.");
                row.RecordSourceColumnName = RequireText(row.RecordSourceColumnName, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'RecordSourceColumnName'.");
                row.RecordSourceDataTypeId = RequireText(row.RecordSourceDataTypeId, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'RecordSourceDataTypeId'.");
                row.RecordSourceLength = RequireText(row.RecordSourceLength, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'RecordSourceLength'.");
                row.TableNamePattern = RequireText(row.TableNamePattern, $"Entity 'RawLinkSatelliteImplementation' row '{row.Id}' is missing required property 'TableNamePattern'.");
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
