using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Meta.Core.Serialization;

namespace MetaRawDataVault
{
    [XmlRoot("MetaRawDataVault")]
    public sealed partial class MetaRawDataVaultModel
    {
        public static MetaRawDataVaultModel CreateEmpty() => new();

        [XmlArray("RawHubList")]
        [XmlArrayItem("RawHub")]
        public List<RawHub> RawHubList { get; set; } = new();
        public bool ShouldSerializeRawHubList() => RawHubList.Count > 0;

        [XmlArray("RawHubKeyPartList")]
        [XmlArrayItem("RawHubKeyPart")]
        public List<RawHubKeyPart> RawHubKeyPartList { get; set; } = new();
        public bool ShouldSerializeRawHubKeyPartList() => RawHubKeyPartList.Count > 0;

        [XmlArray("RawHubSatelliteList")]
        [XmlArrayItem("RawHubSatellite")]
        public List<RawHubSatellite> RawHubSatelliteList { get; set; } = new();
        public bool ShouldSerializeRawHubSatelliteList() => RawHubSatelliteList.Count > 0;

        [XmlArray("RawHubSatelliteAttributeList")]
        [XmlArrayItem("RawHubSatelliteAttribute")]
        public List<RawHubSatelliteAttribute> RawHubSatelliteAttributeList { get; set; } = new();
        public bool ShouldSerializeRawHubSatelliteAttributeList() => RawHubSatelliteAttributeList.Count > 0;

        [XmlArray("RawLinkList")]
        [XmlArrayItem("RawLink")]
        public List<RawLink> RawLinkList { get; set; } = new();
        public bool ShouldSerializeRawLinkList() => RawLinkList.Count > 0;

        [XmlArray("RawLinkHubList")]
        [XmlArrayItem("RawLinkHub")]
        public List<RawLinkHub> RawLinkHubList { get; set; } = new();
        public bool ShouldSerializeRawLinkHubList() => RawLinkHubList.Count > 0;

        [XmlArray("RawLinkSatelliteList")]
        [XmlArrayItem("RawLinkSatellite")]
        public List<RawLinkSatellite> RawLinkSatelliteList { get; set; } = new();
        public bool ShouldSerializeRawLinkSatelliteList() => RawLinkSatelliteList.Count > 0;

        [XmlArray("RawLinkSatelliteAttributeList")]
        [XmlArrayItem("RawLinkSatelliteAttribute")]
        public List<RawLinkSatelliteAttribute> RawLinkSatelliteAttributeList { get; set; } = new();
        public bool ShouldSerializeRawLinkSatelliteAttributeList() => RawLinkSatelliteAttributeList.Count > 0;

        [XmlArray("SourceFieldList")]
        [XmlArrayItem("SourceField")]
        public List<SourceField> SourceFieldList { get; set; } = new();
        public bool ShouldSerializeSourceFieldList() => SourceFieldList.Count > 0;

        [XmlArray("SourceFieldDataTypeDetailList")]
        [XmlArrayItem("SourceFieldDataTypeDetail")]
        public List<SourceFieldDataTypeDetail> SourceFieldDataTypeDetailList { get; set; } = new();
        public bool ShouldSerializeSourceFieldDataTypeDetailList() => SourceFieldDataTypeDetailList.Count > 0;

        [XmlArray("SourceSchemaList")]
        [XmlArrayItem("SourceSchema")]
        public List<SourceSchema> SourceSchemaList { get; set; } = new();
        public bool ShouldSerializeSourceSchemaList() => SourceSchemaList.Count > 0;

        [XmlArray("SourceSystemList")]
        [XmlArrayItem("SourceSystem")]
        public List<SourceSystem> SourceSystemList { get; set; } = new();
        public bool ShouldSerializeSourceSystemList() => SourceSystemList.Count > 0;

        [XmlArray("SourceTableList")]
        [XmlArrayItem("SourceTable")]
        public List<SourceTable> SourceTableList { get; set; } = new();
        public bool ShouldSerializeSourceTableList() => SourceTableList.Count > 0;

        [XmlArray("SourceTableRelationshipList")]
        [XmlArrayItem("SourceTableRelationship")]
        public List<SourceTableRelationship> SourceTableRelationshipList { get; set; } = new();
        public bool ShouldSerializeSourceTableRelationshipList() => SourceTableRelationshipList.Count > 0;

        [XmlArray("SourceTableRelationshipFieldList")]
        [XmlArrayItem("SourceTableRelationshipField")]
        public List<SourceTableRelationshipField> SourceTableRelationshipFieldList { get; set; } = new();
        public bool ShouldSerializeSourceTableRelationshipFieldList() => SourceTableRelationshipFieldList.Count > 0;

        public static MetaRawDataVaultModel LoadFromXmlWorkspace(
            string workspacePath,
            bool searchUpward = true)
        {
            var model = TypedWorkspaceXmlSerializer.Load<MetaRawDataVaultModel>(workspacePath, searchUpward);
            MetaRawDataVaultModelFactory.Bind(model);
            return model;
        }

        public static Task<MetaRawDataVaultModel> LoadFromXmlWorkspaceAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(LoadFromXmlWorkspace(workspacePath, searchUpward));
        }

        public void SaveToXmlWorkspace(string workspacePath)
        {
            MetaRawDataVaultModelFactory.Bind(this);
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
            var assemblyDirectory = Path.GetDirectoryName(typeof(MetaRawDataVaultModel).Assembly.Location);
            if (string.IsNullOrWhiteSpace(assemblyDirectory))
            {
                return null;
            }

            var namespacedPath = Path.Combine(assemblyDirectory, "MetaRawDataVault", "model.xml");
            if (File.Exists(namespacedPath))
            {
                return namespacedPath;
            }

            var directPath = Path.Combine(assemblyDirectory, "model.xml");
            return File.Exists(directPath) ? directPath : null;
        }
    }

    internal static class MetaRawDataVaultModelFactory
    {
        internal static void Bind(MetaRawDataVaultModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            model.RawHubList ??= new List<RawHub>();
            model.RawHubKeyPartList ??= new List<RawHubKeyPart>();
            model.RawHubSatelliteList ??= new List<RawHubSatellite>();
            model.RawHubSatelliteAttributeList ??= new List<RawHubSatelliteAttribute>();
            model.RawLinkList ??= new List<RawLink>();
            model.RawLinkHubList ??= new List<RawLinkHub>();
            model.RawLinkSatelliteList ??= new List<RawLinkSatellite>();
            model.RawLinkSatelliteAttributeList ??= new List<RawLinkSatelliteAttribute>();
            model.SourceFieldList ??= new List<SourceField>();
            model.SourceFieldDataTypeDetailList ??= new List<SourceFieldDataTypeDetail>();
            model.SourceSchemaList ??= new List<SourceSchema>();
            model.SourceSystemList ??= new List<SourceSystem>();
            model.SourceTableList ??= new List<SourceTable>();
            model.SourceTableRelationshipList ??= new List<SourceTableRelationship>();
            model.SourceTableRelationshipFieldList ??= new List<SourceTableRelationshipField>();

            NormalizeRawHubList(model);
            NormalizeRawHubKeyPartList(model);
            NormalizeRawHubSatelliteList(model);
            NormalizeRawHubSatelliteAttributeList(model);
            NormalizeRawLinkList(model);
            NormalizeRawLinkHubList(model);
            NormalizeRawLinkSatelliteList(model);
            NormalizeRawLinkSatelliteAttributeList(model);
            NormalizeSourceFieldList(model);
            NormalizeSourceFieldDataTypeDetailList(model);
            NormalizeSourceSchemaList(model);
            NormalizeSourceSystemList(model);
            NormalizeSourceTableList(model);
            NormalizeSourceTableRelationshipList(model);
            NormalizeSourceTableRelationshipFieldList(model);

            var rawHubListById = BuildById(model.RawHubList, row => row.Id, "RawHub");
            var rawHubKeyPartListById = BuildById(model.RawHubKeyPartList, row => row.Id, "RawHubKeyPart");
            var rawHubSatelliteListById = BuildById(model.RawHubSatelliteList, row => row.Id, "RawHubSatellite");
            var rawHubSatelliteAttributeListById = BuildById(model.RawHubSatelliteAttributeList, row => row.Id, "RawHubSatelliteAttribute");
            var rawLinkListById = BuildById(model.RawLinkList, row => row.Id, "RawLink");
            var rawLinkHubListById = BuildById(model.RawLinkHubList, row => row.Id, "RawLinkHub");
            var rawLinkSatelliteListById = BuildById(model.RawLinkSatelliteList, row => row.Id, "RawLinkSatellite");
            var rawLinkSatelliteAttributeListById = BuildById(model.RawLinkSatelliteAttributeList, row => row.Id, "RawLinkSatelliteAttribute");
            var sourceFieldListById = BuildById(model.SourceFieldList, row => row.Id, "SourceField");
            var sourceFieldDataTypeDetailListById = BuildById(model.SourceFieldDataTypeDetailList, row => row.Id, "SourceFieldDataTypeDetail");
            var sourceSchemaListById = BuildById(model.SourceSchemaList, row => row.Id, "SourceSchema");
            var sourceSystemListById = BuildById(model.SourceSystemList, row => row.Id, "SourceSystem");
            var sourceTableListById = BuildById(model.SourceTableList, row => row.Id, "SourceTable");
            var sourceTableRelationshipListById = BuildById(model.SourceTableRelationshipList, row => row.Id, "SourceTableRelationship");
            var sourceTableRelationshipFieldListById = BuildById(model.SourceTableRelationshipFieldList, row => row.Id, "SourceTableRelationshipField");

            foreach (var row in model.RawHubList)
            {
                row.SourceTableId = ResolveRelationshipId(
                    row.SourceTableId,
                    row.SourceTable?.Id,
                    "RawHub",
                    row.Id,
                    "SourceTableId");
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "RawHub",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in model.RawHubKeyPartList)
            {
                row.RawHubId = ResolveRelationshipId(
                    row.RawHubId,
                    row.RawHub?.Id,
                    "RawHubKeyPart",
                    row.Id,
                    "RawHubId");
                row.RawHub = RequireTarget(
                    rawHubListById,
                    row.RawHubId,
                    "RawHubKeyPart",
                    row.Id,
                    "RawHubId");
            }

            foreach (var row in model.RawHubKeyPartList)
            {
                row.SourceFieldId = ResolveRelationshipId(
                    row.SourceFieldId,
                    row.SourceField?.Id,
                    "RawHubKeyPart",
                    row.Id,
                    "SourceFieldId");
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "RawHubKeyPart",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in model.RawHubSatelliteList)
            {
                row.RawHubId = ResolveRelationshipId(
                    row.RawHubId,
                    row.RawHub?.Id,
                    "RawHubSatellite",
                    row.Id,
                    "RawHubId");
                row.RawHub = RequireTarget(
                    rawHubListById,
                    row.RawHubId,
                    "RawHubSatellite",
                    row.Id,
                    "RawHubId");
            }

            foreach (var row in model.RawHubSatelliteList)
            {
                row.SourceTableId = ResolveRelationshipId(
                    row.SourceTableId,
                    row.SourceTable?.Id,
                    "RawHubSatellite",
                    row.Id,
                    "SourceTableId");
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "RawHubSatellite",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in model.RawHubSatelliteAttributeList)
            {
                row.RawHubSatelliteId = ResolveRelationshipId(
                    row.RawHubSatelliteId,
                    row.RawHubSatellite?.Id,
                    "RawHubSatelliteAttribute",
                    row.Id,
                    "RawHubSatelliteId");
                row.RawHubSatellite = RequireTarget(
                    rawHubSatelliteListById,
                    row.RawHubSatelliteId,
                    "RawHubSatelliteAttribute",
                    row.Id,
                    "RawHubSatelliteId");
            }

            foreach (var row in model.RawHubSatelliteAttributeList)
            {
                row.SourceFieldId = ResolveRelationshipId(
                    row.SourceFieldId,
                    row.SourceField?.Id,
                    "RawHubSatelliteAttribute",
                    row.Id,
                    "SourceFieldId");
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "RawHubSatelliteAttribute",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in model.RawLinkList)
            {
                row.SourceTableRelationshipId = ResolveRelationshipId(
                    row.SourceTableRelationshipId,
                    row.SourceTableRelationship?.Id,
                    "RawLink",
                    row.Id,
                    "SourceTableRelationshipId");
                row.SourceTableRelationship = RequireTarget(
                    sourceTableRelationshipListById,
                    row.SourceTableRelationshipId,
                    "RawLink",
                    row.Id,
                    "SourceTableRelationshipId");
            }

            foreach (var row in model.RawLinkHubList)
            {
                row.RawHubId = ResolveRelationshipId(
                    row.RawHubId,
                    row.RawHub?.Id,
                    "RawLinkHub",
                    row.Id,
                    "RawHubId");
                row.RawHub = RequireTarget(
                    rawHubListById,
                    row.RawHubId,
                    "RawLinkHub",
                    row.Id,
                    "RawHubId");
            }

            foreach (var row in model.RawLinkHubList)
            {
                row.RawLinkId = ResolveRelationshipId(
                    row.RawLinkId,
                    row.RawLink?.Id,
                    "RawLinkHub",
                    row.Id,
                    "RawLinkId");
                row.RawLink = RequireTarget(
                    rawLinkListById,
                    row.RawLinkId,
                    "RawLinkHub",
                    row.Id,
                    "RawLinkId");
            }

            foreach (var row in model.RawLinkSatelliteList)
            {
                row.RawLinkId = ResolveRelationshipId(
                    row.RawLinkId,
                    row.RawLink?.Id,
                    "RawLinkSatellite",
                    row.Id,
                    "RawLinkId");
                row.RawLink = RequireTarget(
                    rawLinkListById,
                    row.RawLinkId,
                    "RawLinkSatellite",
                    row.Id,
                    "RawLinkId");
            }

            foreach (var row in model.RawLinkSatelliteList)
            {
                row.SourceTableId = ResolveRelationshipId(
                    row.SourceTableId,
                    row.SourceTable?.Id,
                    "RawLinkSatellite",
                    row.Id,
                    "SourceTableId");
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "RawLinkSatellite",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in model.RawLinkSatelliteAttributeList)
            {
                row.RawLinkSatelliteId = ResolveRelationshipId(
                    row.RawLinkSatelliteId,
                    row.RawLinkSatellite?.Id,
                    "RawLinkSatelliteAttribute",
                    row.Id,
                    "RawLinkSatelliteId");
                row.RawLinkSatellite = RequireTarget(
                    rawLinkSatelliteListById,
                    row.RawLinkSatelliteId,
                    "RawLinkSatelliteAttribute",
                    row.Id,
                    "RawLinkSatelliteId");
            }

            foreach (var row in model.RawLinkSatelliteAttributeList)
            {
                row.SourceFieldId = ResolveRelationshipId(
                    row.SourceFieldId,
                    row.SourceField?.Id,
                    "RawLinkSatelliteAttribute",
                    row.Id,
                    "SourceFieldId");
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "RawLinkSatelliteAttribute",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in model.SourceFieldList)
            {
                row.SourceTableId = ResolveRelationshipId(
                    row.SourceTableId,
                    row.SourceTable?.Id,
                    "SourceField",
                    row.Id,
                    "SourceTableId");
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "SourceField",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in model.SourceFieldDataTypeDetailList)
            {
                row.SourceFieldId = ResolveRelationshipId(
                    row.SourceFieldId,
                    row.SourceField?.Id,
                    "SourceFieldDataTypeDetail",
                    row.Id,
                    "SourceFieldId");
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "SourceFieldDataTypeDetail",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in model.SourceSchemaList)
            {
                row.SourceSystemId = ResolveRelationshipId(
                    row.SourceSystemId,
                    row.SourceSystem?.Id,
                    "SourceSchema",
                    row.Id,
                    "SourceSystemId");
                row.SourceSystem = RequireTarget(
                    sourceSystemListById,
                    row.SourceSystemId,
                    "SourceSchema",
                    row.Id,
                    "SourceSystemId");
            }

            foreach (var row in model.SourceTableList)
            {
                row.SourceSchemaId = ResolveRelationshipId(
                    row.SourceSchemaId,
                    row.SourceSchema?.Id,
                    "SourceTable",
                    row.Id,
                    "SourceSchemaId");
                row.SourceSchema = RequireTarget(
                    sourceSchemaListById,
                    row.SourceSchemaId,
                    "SourceTable",
                    row.Id,
                    "SourceSchemaId");
            }

            foreach (var row in model.SourceTableRelationshipList)
            {
                row.SourceTableId = ResolveRelationshipId(
                    row.SourceTableId,
                    row.SourceTable?.Id,
                    "SourceTableRelationship",
                    row.Id,
                    "SourceTableId");
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "SourceTableRelationship",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in model.SourceTableRelationshipList)
            {
                row.TargetTableId = ResolveRelationshipId(
                    row.TargetTableId,
                    row.TargetTable?.Id,
                    "SourceTableRelationship",
                    row.Id,
                    "TargetTableId");
                row.TargetTable = RequireTarget(
                    sourceTableListById,
                    row.TargetTableId,
                    "SourceTableRelationship",
                    row.Id,
                    "TargetTableId");
            }

            foreach (var row in model.SourceTableRelationshipFieldList)
            {
                row.SourceFieldId = ResolveRelationshipId(
                    row.SourceFieldId,
                    row.SourceField?.Id,
                    "SourceTableRelationshipField",
                    row.Id,
                    "SourceFieldId");
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "SourceTableRelationshipField",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in model.SourceTableRelationshipFieldList)
            {
                row.SourceTableRelationshipId = ResolveRelationshipId(
                    row.SourceTableRelationshipId,
                    row.SourceTableRelationship?.Id,
                    "SourceTableRelationshipField",
                    row.Id,
                    "SourceTableRelationshipId");
                row.SourceTableRelationship = RequireTarget(
                    sourceTableRelationshipListById,
                    row.SourceTableRelationshipId,
                    "SourceTableRelationshipField",
                    row.Id,
                    "SourceTableRelationshipId");
            }

            foreach (var row in model.SourceTableRelationshipFieldList)
            {
                row.TargetFieldId = ResolveRelationshipId(
                    row.TargetFieldId,
                    row.TargetField?.Id,
                    "SourceTableRelationshipField",
                    row.Id,
                    "TargetFieldId");
                row.TargetField = RequireTarget(
                    sourceFieldListById,
                    row.TargetFieldId,
                    "SourceTableRelationshipField",
                    row.Id,
                    "TargetFieldId");
            }

        }

        private static void NormalizeRawHubList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.RawHubList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RawHub' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'RawHub' row '{row.Id}' is missing required property 'Name'.");
                row.SourceTableId ??= string.Empty;
            }
        }

        private static void NormalizeRawHubKeyPartList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.RawHubKeyPartList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RawHubKeyPart' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'RawHubKeyPart' row '{row.Id}' is missing required property 'Name'.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'RawHubKeyPart' row '{row.Id}' is missing required property 'Ordinal'.");
                row.RawHubId ??= string.Empty;
                row.SourceFieldId ??= string.Empty;
            }
        }

        private static void NormalizeRawHubSatelliteList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.RawHubSatelliteList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RawHubSatellite' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'RawHubSatellite' row '{row.Id}' is missing required property 'Name'.");
                row.SatelliteKind = RequireText(row.SatelliteKind, $"Entity 'RawHubSatellite' row '{row.Id}' is missing required property 'SatelliteKind'.");
                row.RawHubId ??= string.Empty;
                row.SourceTableId ??= string.Empty;
            }
        }

        private static void NormalizeRawHubSatelliteAttributeList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.RawHubSatelliteAttributeList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RawHubSatelliteAttribute' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'RawHubSatelliteAttribute' row '{row.Id}' is missing required property 'Name'.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'RawHubSatelliteAttribute' row '{row.Id}' is missing required property 'Ordinal'.");
                row.RawHubSatelliteId ??= string.Empty;
                row.SourceFieldId ??= string.Empty;
            }
        }

        private static void NormalizeRawLinkList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.RawLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RawLink' contains a row with empty Id.");
                row.LinkKind = RequireText(row.LinkKind, $"Entity 'RawLink' row '{row.Id}' is missing required property 'LinkKind'.");
                row.Name = RequireText(row.Name, $"Entity 'RawLink' row '{row.Id}' is missing required property 'Name'.");
                row.SourceTableRelationshipId ??= string.Empty;
            }
        }

        private static void NormalizeRawLinkHubList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.RawLinkHubList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RawLinkHub' contains a row with empty Id.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'RawLinkHub' row '{row.Id}' is missing required property 'Ordinal'.");
                row.RoleName ??= string.Empty;
                row.RawHubId ??= string.Empty;
                row.RawLinkId ??= string.Empty;
            }
        }

        private static void NormalizeRawLinkSatelliteList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.RawLinkSatelliteList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RawLinkSatellite' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'RawLinkSatellite' row '{row.Id}' is missing required property 'Name'.");
                row.SatelliteKind = RequireText(row.SatelliteKind, $"Entity 'RawLinkSatellite' row '{row.Id}' is missing required property 'SatelliteKind'.");
                row.RawLinkId ??= string.Empty;
                row.SourceTableId ??= string.Empty;
            }
        }

        private static void NormalizeRawLinkSatelliteAttributeList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.RawLinkSatelliteAttributeList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'RawLinkSatelliteAttribute' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'RawLinkSatelliteAttribute' row '{row.Id}' is missing required property 'Name'.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'RawLinkSatelliteAttribute' row '{row.Id}' is missing required property 'Ordinal'.");
                row.RawLinkSatelliteId ??= string.Empty;
                row.SourceFieldId ??= string.Empty;
            }
        }

        private static void NormalizeSourceFieldList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.SourceFieldList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SourceField' contains a row with empty Id.");
                row.DataTypeId = RequireText(row.DataTypeId, $"Entity 'SourceField' row '{row.Id}' is missing required property 'DataTypeId'.");
                row.IsNullable ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'SourceField' row '{row.Id}' is missing required property 'Name'.");
                row.Ordinal ??= string.Empty;
                row.SourceTableId ??= string.Empty;
            }
        }

        private static void NormalizeSourceFieldDataTypeDetailList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.SourceFieldDataTypeDetailList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SourceFieldDataTypeDetail' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'SourceFieldDataTypeDetail' row '{row.Id}' is missing required property 'Name'.");
                row.Value = RequireText(row.Value, $"Entity 'SourceFieldDataTypeDetail' row '{row.Id}' is missing required property 'Value'.");
                row.SourceFieldId ??= string.Empty;
            }
        }

        private static void NormalizeSourceSchemaList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.SourceSchemaList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SourceSchema' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'SourceSchema' row '{row.Id}' is missing required property 'Name'.");
                row.SourceSystemId ??= string.Empty;
            }
        }

        private static void NormalizeSourceSystemList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.SourceSystemList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SourceSystem' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'SourceSystem' row '{row.Id}' is missing required property 'Name'.");
            }
        }

        private static void NormalizeSourceTableList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.SourceTableList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SourceTable' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'SourceTable' row '{row.Id}' is missing required property 'Name'.");
                row.SourceSchemaId ??= string.Empty;
            }
        }

        private static void NormalizeSourceTableRelationshipList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.SourceTableRelationshipList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SourceTableRelationship' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'SourceTableRelationship' row '{row.Id}' is missing required property 'Name'.");
                row.SourceTableId ??= string.Empty;
                row.TargetTableId ??= string.Empty;
            }
        }

        private static void NormalizeSourceTableRelationshipFieldList(MetaRawDataVaultModel model)
        {
            foreach (var row in model.SourceTableRelationshipFieldList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'SourceTableRelationshipField' contains a row with empty Id.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'SourceTableRelationshipField' row '{row.Id}' is missing required property 'Ordinal'.");
                row.SourceFieldId ??= string.Empty;
                row.SourceTableRelationshipId ??= string.Empty;
                row.TargetFieldId ??= string.Empty;
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
