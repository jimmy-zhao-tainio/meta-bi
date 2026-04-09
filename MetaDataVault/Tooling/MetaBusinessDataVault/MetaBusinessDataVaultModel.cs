using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Meta.Core.Serialization;

namespace MetaBusinessDataVault
{
    [XmlRoot("MetaBusinessDataVault")]
    public sealed partial class MetaBusinessDataVaultModel
    {
        public static MetaBusinessDataVaultModel CreateEmpty() => new();

        [XmlArray("BusinessBridgeList")]
        [XmlArrayItem("BusinessBridge")]
        public List<BusinessBridge> BusinessBridgeList { get; set; } = new();
        public bool ShouldSerializeBusinessBridgeList() => BusinessBridgeList.Count > 0;

        [XmlArray("BusinessBridgeHubList")]
        [XmlArrayItem("BusinessBridgeHub")]
        public List<BusinessBridgeHub> BusinessBridgeHubList { get; set; } = new();
        public bool ShouldSerializeBusinessBridgeHubList() => BusinessBridgeHubList.Count > 0;

        [XmlArray("BusinessBridgeLinkList")]
        [XmlArrayItem("BusinessBridgeLink")]
        public List<BusinessBridgeLink> BusinessBridgeLinkList { get; set; } = new();
        public bool ShouldSerializeBusinessBridgeLinkList() => BusinessBridgeLinkList.Count > 0;

        [XmlArray("BusinessHierarchicalLinkList")]
        [XmlArrayItem("BusinessHierarchicalLink")]
        public List<BusinessHierarchicalLink> BusinessHierarchicalLinkList { get; set; } = new();
        public bool ShouldSerializeBusinessHierarchicalLinkList() => BusinessHierarchicalLinkList.Count > 0;

        [XmlArray("BusinessHierarchicalLinkSatelliteList")]
        [XmlArrayItem("BusinessHierarchicalLinkSatellite")]
        public List<BusinessHierarchicalLinkSatellite> BusinessHierarchicalLinkSatelliteList { get; set; } = new();
        public bool ShouldSerializeBusinessHierarchicalLinkSatelliteList() => BusinessHierarchicalLinkSatelliteList.Count > 0;

        [XmlArray("BusinessHierarchicalLinkSatelliteAttributeList")]
        [XmlArrayItem("BusinessHierarchicalLinkSatelliteAttribute")]
        public List<BusinessHierarchicalLinkSatelliteAttribute> BusinessHierarchicalLinkSatelliteAttributeList { get; set; } = new();
        public bool ShouldSerializeBusinessHierarchicalLinkSatelliteAttributeList() => BusinessHierarchicalLinkSatelliteAttributeList.Count > 0;

        [XmlArray("BusinessHierarchicalLinkSatelliteAttributeDataTypeDetailList")]
        [XmlArrayItem("BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail")]
        public List<BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail> BusinessHierarchicalLinkSatelliteAttributeDataTypeDetailList { get; set; } = new();
        public bool ShouldSerializeBusinessHierarchicalLinkSatelliteAttributeDataTypeDetailList() => BusinessHierarchicalLinkSatelliteAttributeDataTypeDetailList.Count > 0;

        [XmlArray("BusinessHubList")]
        [XmlArrayItem("BusinessHub")]
        public List<BusinessHub> BusinessHubList { get; set; } = new();
        public bool ShouldSerializeBusinessHubList() => BusinessHubList.Count > 0;

        [XmlArray("BusinessHubKeyPartList")]
        [XmlArrayItem("BusinessHubKeyPart")]
        public List<BusinessHubKeyPart> BusinessHubKeyPartList { get; set; } = new();
        public bool ShouldSerializeBusinessHubKeyPartList() => BusinessHubKeyPartList.Count > 0;

        [XmlArray("BusinessHubKeyPartDataTypeDetailList")]
        [XmlArrayItem("BusinessHubKeyPartDataTypeDetail")]
        public List<BusinessHubKeyPartDataTypeDetail> BusinessHubKeyPartDataTypeDetailList { get; set; } = new();
        public bool ShouldSerializeBusinessHubKeyPartDataTypeDetailList() => BusinessHubKeyPartDataTypeDetailList.Count > 0;

        [XmlArray("BusinessHubSatelliteList")]
        [XmlArrayItem("BusinessHubSatellite")]
        public List<BusinessHubSatellite> BusinessHubSatelliteList { get; set; } = new();
        public bool ShouldSerializeBusinessHubSatelliteList() => BusinessHubSatelliteList.Count > 0;

        [XmlArray("BusinessHubSatelliteAttributeList")]
        [XmlArrayItem("BusinessHubSatelliteAttribute")]
        public List<BusinessHubSatelliteAttribute> BusinessHubSatelliteAttributeList { get; set; } = new();
        public bool ShouldSerializeBusinessHubSatelliteAttributeList() => BusinessHubSatelliteAttributeList.Count > 0;

        [XmlArray("BusinessHubSatelliteAttributeDataTypeDetailList")]
        [XmlArrayItem("BusinessHubSatelliteAttributeDataTypeDetail")]
        public List<BusinessHubSatelliteAttributeDataTypeDetail> BusinessHubSatelliteAttributeDataTypeDetailList { get; set; } = new();
        public bool ShouldSerializeBusinessHubSatelliteAttributeDataTypeDetailList() => BusinessHubSatelliteAttributeDataTypeDetailList.Count > 0;

        [XmlArray("BusinessLinkList")]
        [XmlArrayItem("BusinessLink")]
        public List<BusinessLink> BusinessLinkList { get; set; } = new();
        public bool ShouldSerializeBusinessLinkList() => BusinessLinkList.Count > 0;

        [XmlArray("BusinessLinkHubList")]
        [XmlArrayItem("BusinessLinkHub")]
        public List<BusinessLinkHub> BusinessLinkHubList { get; set; } = new();
        public bool ShouldSerializeBusinessLinkHubList() => BusinessLinkHubList.Count > 0;

        [XmlArray("BusinessLinkSatelliteList")]
        [XmlArrayItem("BusinessLinkSatellite")]
        public List<BusinessLinkSatellite> BusinessLinkSatelliteList { get; set; } = new();
        public bool ShouldSerializeBusinessLinkSatelliteList() => BusinessLinkSatelliteList.Count > 0;

        [XmlArray("BusinessLinkSatelliteAttributeList")]
        [XmlArrayItem("BusinessLinkSatelliteAttribute")]
        public List<BusinessLinkSatelliteAttribute> BusinessLinkSatelliteAttributeList { get; set; } = new();
        public bool ShouldSerializeBusinessLinkSatelliteAttributeList() => BusinessLinkSatelliteAttributeList.Count > 0;

        [XmlArray("BusinessLinkSatelliteAttributeDataTypeDetailList")]
        [XmlArrayItem("BusinessLinkSatelliteAttributeDataTypeDetail")]
        public List<BusinessLinkSatelliteAttributeDataTypeDetail> BusinessLinkSatelliteAttributeDataTypeDetailList { get; set; } = new();
        public bool ShouldSerializeBusinessLinkSatelliteAttributeDataTypeDetailList() => BusinessLinkSatelliteAttributeDataTypeDetailList.Count > 0;

        [XmlArray("BusinessPointInTimeList")]
        [XmlArrayItem("BusinessPointInTime")]
        public List<BusinessPointInTime> BusinessPointInTimeList { get; set; } = new();
        public bool ShouldSerializeBusinessPointInTimeList() => BusinessPointInTimeList.Count > 0;

        [XmlArray("BusinessPointInTimeHubSatelliteList")]
        [XmlArrayItem("BusinessPointInTimeHubSatellite")]
        public List<BusinessPointInTimeHubSatellite> BusinessPointInTimeHubSatelliteList { get; set; } = new();
        public bool ShouldSerializeBusinessPointInTimeHubSatelliteList() => BusinessPointInTimeHubSatelliteList.Count > 0;

        [XmlArray("BusinessPointInTimeLinkSatelliteList")]
        [XmlArrayItem("BusinessPointInTimeLinkSatellite")]
        public List<BusinessPointInTimeLinkSatellite> BusinessPointInTimeLinkSatelliteList { get; set; } = new();
        public bool ShouldSerializeBusinessPointInTimeLinkSatelliteList() => BusinessPointInTimeLinkSatelliteList.Count > 0;

        [XmlArray("BusinessPointInTimeStampList")]
        [XmlArrayItem("BusinessPointInTimeStamp")]
        public List<BusinessPointInTimeStamp> BusinessPointInTimeStampList { get; set; } = new();
        public bool ShouldSerializeBusinessPointInTimeStampList() => BusinessPointInTimeStampList.Count > 0;

        [XmlArray("BusinessPointInTimeStampDataTypeDetailList")]
        [XmlArrayItem("BusinessPointInTimeStampDataTypeDetail")]
        public List<BusinessPointInTimeStampDataTypeDetail> BusinessPointInTimeStampDataTypeDetailList { get; set; } = new();
        public bool ShouldSerializeBusinessPointInTimeStampDataTypeDetailList() => BusinessPointInTimeStampDataTypeDetailList.Count > 0;

        [XmlArray("BusinessReferenceList")]
        [XmlArrayItem("BusinessReference")]
        public List<BusinessReference> BusinessReferenceList { get; set; } = new();
        public bool ShouldSerializeBusinessReferenceList() => BusinessReferenceList.Count > 0;

        [XmlArray("BusinessReferenceKeyPartList")]
        [XmlArrayItem("BusinessReferenceKeyPart")]
        public List<BusinessReferenceKeyPart> BusinessReferenceKeyPartList { get; set; } = new();
        public bool ShouldSerializeBusinessReferenceKeyPartList() => BusinessReferenceKeyPartList.Count > 0;

        [XmlArray("BusinessReferenceKeyPartDataTypeDetailList")]
        [XmlArrayItem("BusinessReferenceKeyPartDataTypeDetail")]
        public List<BusinessReferenceKeyPartDataTypeDetail> BusinessReferenceKeyPartDataTypeDetailList { get; set; } = new();
        public bool ShouldSerializeBusinessReferenceKeyPartDataTypeDetailList() => BusinessReferenceKeyPartDataTypeDetailList.Count > 0;

        [XmlArray("BusinessReferenceSatelliteList")]
        [XmlArrayItem("BusinessReferenceSatellite")]
        public List<BusinessReferenceSatellite> BusinessReferenceSatelliteList { get; set; } = new();
        public bool ShouldSerializeBusinessReferenceSatelliteList() => BusinessReferenceSatelliteList.Count > 0;

        [XmlArray("BusinessReferenceSatelliteAttributeList")]
        [XmlArrayItem("BusinessReferenceSatelliteAttribute")]
        public List<BusinessReferenceSatelliteAttribute> BusinessReferenceSatelliteAttributeList { get; set; } = new();
        public bool ShouldSerializeBusinessReferenceSatelliteAttributeList() => BusinessReferenceSatelliteAttributeList.Count > 0;

        [XmlArray("BusinessReferenceSatelliteAttributeDataTypeDetailList")]
        [XmlArrayItem("BusinessReferenceSatelliteAttributeDataTypeDetail")]
        public List<BusinessReferenceSatelliteAttributeDataTypeDetail> BusinessReferenceSatelliteAttributeDataTypeDetailList { get; set; } = new();
        public bool ShouldSerializeBusinessReferenceSatelliteAttributeDataTypeDetailList() => BusinessReferenceSatelliteAttributeDataTypeDetailList.Count > 0;

        [XmlArray("BusinessSameAsLinkList")]
        [XmlArrayItem("BusinessSameAsLink")]
        public List<BusinessSameAsLink> BusinessSameAsLinkList { get; set; } = new();
        public bool ShouldSerializeBusinessSameAsLinkList() => BusinessSameAsLinkList.Count > 0;

        [XmlArray("BusinessSameAsLinkSatelliteList")]
        [XmlArrayItem("BusinessSameAsLinkSatellite")]
        public List<BusinessSameAsLinkSatellite> BusinessSameAsLinkSatelliteList { get; set; } = new();
        public bool ShouldSerializeBusinessSameAsLinkSatelliteList() => BusinessSameAsLinkSatelliteList.Count > 0;

        [XmlArray("BusinessSameAsLinkSatelliteAttributeList")]
        [XmlArrayItem("BusinessSameAsLinkSatelliteAttribute")]
        public List<BusinessSameAsLinkSatelliteAttribute> BusinessSameAsLinkSatelliteAttributeList { get; set; } = new();
        public bool ShouldSerializeBusinessSameAsLinkSatelliteAttributeList() => BusinessSameAsLinkSatelliteAttributeList.Count > 0;

        [XmlArray("BusinessSameAsLinkSatelliteAttributeDataTypeDetailList")]
        [XmlArrayItem("BusinessSameAsLinkSatelliteAttributeDataTypeDetail")]
        public List<BusinessSameAsLinkSatelliteAttributeDataTypeDetail> BusinessSameAsLinkSatelliteAttributeDataTypeDetailList { get; set; } = new();
        public bool ShouldSerializeBusinessSameAsLinkSatelliteAttributeDataTypeDetailList() => BusinessSameAsLinkSatelliteAttributeDataTypeDetailList.Count > 0;

        public static MetaBusinessDataVaultModel LoadFromXmlWorkspace(
            string workspacePath,
            bool searchUpward = true)
        {
            var model = TypedWorkspaceXmlSerializer.Load<MetaBusinessDataVaultModel>(workspacePath, searchUpward);
            MetaBusinessDataVaultModelFactory.Bind(model);
            return model;
        }

        public static Task<MetaBusinessDataVaultModel> LoadFromXmlWorkspaceAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(LoadFromXmlWorkspace(workspacePath, searchUpward));
        }

        public void SaveToXmlWorkspace(string workspacePath)
        {
            MetaBusinessDataVaultModelFactory.Bind(this);
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
            var assemblyDirectory = Path.GetDirectoryName(typeof(MetaBusinessDataVaultModel).Assembly.Location);
            if (string.IsNullOrWhiteSpace(assemblyDirectory))
            {
                return null;
            }

            var namespacedPath = Path.Combine(assemblyDirectory, "MetaBusinessDataVault", "model.xml");
            if (File.Exists(namespacedPath))
            {
                return namespacedPath;
            }

            var directPath = Path.Combine(assemblyDirectory, "model.xml");
            return File.Exists(directPath) ? directPath : null;
        }
    }

    internal static class MetaBusinessDataVaultModelFactory
    {
        internal static void Bind(MetaBusinessDataVaultModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            model.BusinessBridgeList ??= new List<BusinessBridge>();
            model.BusinessBridgeHubList ??= new List<BusinessBridgeHub>();
            model.BusinessBridgeLinkList ??= new List<BusinessBridgeLink>();
            model.BusinessHierarchicalLinkList ??= new List<BusinessHierarchicalLink>();
            model.BusinessHierarchicalLinkSatelliteList ??= new List<BusinessHierarchicalLinkSatellite>();
            model.BusinessHierarchicalLinkSatelliteAttributeList ??= new List<BusinessHierarchicalLinkSatelliteAttribute>();
            model.BusinessHierarchicalLinkSatelliteAttributeDataTypeDetailList ??= new List<BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail>();
            model.BusinessHubList ??= new List<BusinessHub>();
            model.BusinessHubKeyPartList ??= new List<BusinessHubKeyPart>();
            model.BusinessHubKeyPartDataTypeDetailList ??= new List<BusinessHubKeyPartDataTypeDetail>();
            model.BusinessHubSatelliteList ??= new List<BusinessHubSatellite>();
            model.BusinessHubSatelliteAttributeList ??= new List<BusinessHubSatelliteAttribute>();
            model.BusinessHubSatelliteAttributeDataTypeDetailList ??= new List<BusinessHubSatelliteAttributeDataTypeDetail>();
            model.BusinessLinkList ??= new List<BusinessLink>();
            model.BusinessLinkHubList ??= new List<BusinessLinkHub>();
            model.BusinessLinkSatelliteList ??= new List<BusinessLinkSatellite>();
            model.BusinessLinkSatelliteAttributeList ??= new List<BusinessLinkSatelliteAttribute>();
            model.BusinessLinkSatelliteAttributeDataTypeDetailList ??= new List<BusinessLinkSatelliteAttributeDataTypeDetail>();
            model.BusinessPointInTimeList ??= new List<BusinessPointInTime>();
            model.BusinessPointInTimeHubSatelliteList ??= new List<BusinessPointInTimeHubSatellite>();
            model.BusinessPointInTimeLinkSatelliteList ??= new List<BusinessPointInTimeLinkSatellite>();
            model.BusinessPointInTimeStampList ??= new List<BusinessPointInTimeStamp>();
            model.BusinessPointInTimeStampDataTypeDetailList ??= new List<BusinessPointInTimeStampDataTypeDetail>();
            model.BusinessReferenceList ??= new List<BusinessReference>();
            model.BusinessReferenceKeyPartList ??= new List<BusinessReferenceKeyPart>();
            model.BusinessReferenceKeyPartDataTypeDetailList ??= new List<BusinessReferenceKeyPartDataTypeDetail>();
            model.BusinessReferenceSatelliteList ??= new List<BusinessReferenceSatellite>();
            model.BusinessReferenceSatelliteAttributeList ??= new List<BusinessReferenceSatelliteAttribute>();
            model.BusinessReferenceSatelliteAttributeDataTypeDetailList ??= new List<BusinessReferenceSatelliteAttributeDataTypeDetail>();
            model.BusinessSameAsLinkList ??= new List<BusinessSameAsLink>();
            model.BusinessSameAsLinkSatelliteList ??= new List<BusinessSameAsLinkSatellite>();
            model.BusinessSameAsLinkSatelliteAttributeList ??= new List<BusinessSameAsLinkSatelliteAttribute>();
            model.BusinessSameAsLinkSatelliteAttributeDataTypeDetailList ??= new List<BusinessSameAsLinkSatelliteAttributeDataTypeDetail>();

            NormalizeBusinessBridgeList(model);
            NormalizeBusinessBridgeHubList(model);
            NormalizeBusinessBridgeLinkList(model);
            NormalizeBusinessHierarchicalLinkList(model);
            NormalizeBusinessHierarchicalLinkSatelliteList(model);
            NormalizeBusinessHierarchicalLinkSatelliteAttributeList(model);
            NormalizeBusinessHierarchicalLinkSatelliteAttributeDataTypeDetailList(model);
            NormalizeBusinessHubList(model);
            NormalizeBusinessHubKeyPartList(model);
            NormalizeBusinessHubKeyPartDataTypeDetailList(model);
            NormalizeBusinessHubSatelliteList(model);
            NormalizeBusinessHubSatelliteAttributeList(model);
            NormalizeBusinessHubSatelliteAttributeDataTypeDetailList(model);
            NormalizeBusinessLinkList(model);
            NormalizeBusinessLinkHubList(model);
            NormalizeBusinessLinkSatelliteList(model);
            NormalizeBusinessLinkSatelliteAttributeList(model);
            NormalizeBusinessLinkSatelliteAttributeDataTypeDetailList(model);
            NormalizeBusinessPointInTimeList(model);
            NormalizeBusinessPointInTimeHubSatelliteList(model);
            NormalizeBusinessPointInTimeLinkSatelliteList(model);
            NormalizeBusinessPointInTimeStampList(model);
            NormalizeBusinessPointInTimeStampDataTypeDetailList(model);
            NormalizeBusinessReferenceList(model);
            NormalizeBusinessReferenceKeyPartList(model);
            NormalizeBusinessReferenceKeyPartDataTypeDetailList(model);
            NormalizeBusinessReferenceSatelliteList(model);
            NormalizeBusinessReferenceSatelliteAttributeList(model);
            NormalizeBusinessReferenceSatelliteAttributeDataTypeDetailList(model);
            NormalizeBusinessSameAsLinkList(model);
            NormalizeBusinessSameAsLinkSatelliteList(model);
            NormalizeBusinessSameAsLinkSatelliteAttributeList(model);
            NormalizeBusinessSameAsLinkSatelliteAttributeDataTypeDetailList(model);

            var businessBridgeListById = BuildById(model.BusinessBridgeList, row => row.Id, "BusinessBridge");
            var businessBridgeHubListById = BuildById(model.BusinessBridgeHubList, row => row.Id, "BusinessBridgeHub");
            var businessBridgeLinkListById = BuildById(model.BusinessBridgeLinkList, row => row.Id, "BusinessBridgeLink");
            var businessHierarchicalLinkListById = BuildById(model.BusinessHierarchicalLinkList, row => row.Id, "BusinessHierarchicalLink");
            var businessHierarchicalLinkSatelliteListById = BuildById(model.BusinessHierarchicalLinkSatelliteList, row => row.Id, "BusinessHierarchicalLinkSatellite");
            var businessHierarchicalLinkSatelliteAttributeListById = BuildById(model.BusinessHierarchicalLinkSatelliteAttributeList, row => row.Id, "BusinessHierarchicalLinkSatelliteAttribute");
            var businessHierarchicalLinkSatelliteAttributeDataTypeDetailListById = BuildById(model.BusinessHierarchicalLinkSatelliteAttributeDataTypeDetailList, row => row.Id, "BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail");
            var businessHubListById = BuildById(model.BusinessHubList, row => row.Id, "BusinessHub");
            var businessHubKeyPartListById = BuildById(model.BusinessHubKeyPartList, row => row.Id, "BusinessHubKeyPart");
            var businessHubKeyPartDataTypeDetailListById = BuildById(model.BusinessHubKeyPartDataTypeDetailList, row => row.Id, "BusinessHubKeyPartDataTypeDetail");
            var businessHubSatelliteListById = BuildById(model.BusinessHubSatelliteList, row => row.Id, "BusinessHubSatellite");
            var businessHubSatelliteAttributeListById = BuildById(model.BusinessHubSatelliteAttributeList, row => row.Id, "BusinessHubSatelliteAttribute");
            var businessHubSatelliteAttributeDataTypeDetailListById = BuildById(model.BusinessHubSatelliteAttributeDataTypeDetailList, row => row.Id, "BusinessHubSatelliteAttributeDataTypeDetail");
            var businessLinkListById = BuildById(model.BusinessLinkList, row => row.Id, "BusinessLink");
            var businessLinkHubListById = BuildById(model.BusinessLinkHubList, row => row.Id, "BusinessLinkHub");
            var businessLinkSatelliteListById = BuildById(model.BusinessLinkSatelliteList, row => row.Id, "BusinessLinkSatellite");
            var businessLinkSatelliteAttributeListById = BuildById(model.BusinessLinkSatelliteAttributeList, row => row.Id, "BusinessLinkSatelliteAttribute");
            var businessLinkSatelliteAttributeDataTypeDetailListById = BuildById(model.BusinessLinkSatelliteAttributeDataTypeDetailList, row => row.Id, "BusinessLinkSatelliteAttributeDataTypeDetail");
            var businessPointInTimeListById = BuildById(model.BusinessPointInTimeList, row => row.Id, "BusinessPointInTime");
            var businessPointInTimeHubSatelliteListById = BuildById(model.BusinessPointInTimeHubSatelliteList, row => row.Id, "BusinessPointInTimeHubSatellite");
            var businessPointInTimeLinkSatelliteListById = BuildById(model.BusinessPointInTimeLinkSatelliteList, row => row.Id, "BusinessPointInTimeLinkSatellite");
            var businessPointInTimeStampListById = BuildById(model.BusinessPointInTimeStampList, row => row.Id, "BusinessPointInTimeStamp");
            var businessPointInTimeStampDataTypeDetailListById = BuildById(model.BusinessPointInTimeStampDataTypeDetailList, row => row.Id, "BusinessPointInTimeStampDataTypeDetail");
            var businessReferenceListById = BuildById(model.BusinessReferenceList, row => row.Id, "BusinessReference");
            var businessReferenceKeyPartListById = BuildById(model.BusinessReferenceKeyPartList, row => row.Id, "BusinessReferenceKeyPart");
            var businessReferenceKeyPartDataTypeDetailListById = BuildById(model.BusinessReferenceKeyPartDataTypeDetailList, row => row.Id, "BusinessReferenceKeyPartDataTypeDetail");
            var businessReferenceSatelliteListById = BuildById(model.BusinessReferenceSatelliteList, row => row.Id, "BusinessReferenceSatellite");
            var businessReferenceSatelliteAttributeListById = BuildById(model.BusinessReferenceSatelliteAttributeList, row => row.Id, "BusinessReferenceSatelliteAttribute");
            var businessReferenceSatelliteAttributeDataTypeDetailListById = BuildById(model.BusinessReferenceSatelliteAttributeDataTypeDetailList, row => row.Id, "BusinessReferenceSatelliteAttributeDataTypeDetail");
            var businessSameAsLinkListById = BuildById(model.BusinessSameAsLinkList, row => row.Id, "BusinessSameAsLink");
            var businessSameAsLinkSatelliteListById = BuildById(model.BusinessSameAsLinkSatelliteList, row => row.Id, "BusinessSameAsLinkSatellite");
            var businessSameAsLinkSatelliteAttributeListById = BuildById(model.BusinessSameAsLinkSatelliteAttributeList, row => row.Id, "BusinessSameAsLinkSatelliteAttribute");
            var businessSameAsLinkSatelliteAttributeDataTypeDetailListById = BuildById(model.BusinessSameAsLinkSatelliteAttributeDataTypeDetailList, row => row.Id, "BusinessSameAsLinkSatelliteAttributeDataTypeDetail");

            foreach (var row in model.BusinessBridgeList)
            {
                row.AnchorHubId = ResolveRelationshipId(
                    row.AnchorHubId,
                    row.AnchorHub?.Id,
                    "BusinessBridge",
                    row.Id,
                    "AnchorHubId");
                row.AnchorHub = RequireTarget(
                    businessHubListById,
                    row.AnchorHubId,
                    "BusinessBridge",
                    row.Id,
                    "AnchorHubId");
            }

            foreach (var row in model.BusinessBridgeHubList)
            {
                row.BusinessBridgeId = ResolveRelationshipId(
                    row.BusinessBridgeId,
                    row.BusinessBridge?.Id,
                    "BusinessBridgeHub",
                    row.Id,
                    "BusinessBridgeId");
                row.BusinessBridge = RequireTarget(
                    businessBridgeListById,
                    row.BusinessBridgeId,
                    "BusinessBridgeHub",
                    row.Id,
                    "BusinessBridgeId");
            }

            foreach (var row in model.BusinessBridgeHubList)
            {
                row.BusinessHubId = ResolveRelationshipId(
                    row.BusinessHubId,
                    row.BusinessHub?.Id,
                    "BusinessBridgeHub",
                    row.Id,
                    "BusinessHubId");
                row.BusinessHub = RequireTarget(
                    businessHubListById,
                    row.BusinessHubId,
                    "BusinessBridgeHub",
                    row.Id,
                    "BusinessHubId");
            }

            foreach (var row in model.BusinessBridgeLinkList)
            {
                row.BusinessBridgeId = ResolveRelationshipId(
                    row.BusinessBridgeId,
                    row.BusinessBridge?.Id,
                    "BusinessBridgeLink",
                    row.Id,
                    "BusinessBridgeId");
                row.BusinessBridge = RequireTarget(
                    businessBridgeListById,
                    row.BusinessBridgeId,
                    "BusinessBridgeLink",
                    row.Id,
                    "BusinessBridgeId");
            }

            foreach (var row in model.BusinessBridgeLinkList)
            {
                row.BusinessLinkId = ResolveRelationshipId(
                    row.BusinessLinkId,
                    row.BusinessLink?.Id,
                    "BusinessBridgeLink",
                    row.Id,
                    "BusinessLinkId");
                row.BusinessLink = RequireTarget(
                    businessLinkListById,
                    row.BusinessLinkId,
                    "BusinessBridgeLink",
                    row.Id,
                    "BusinessLinkId");
            }

            foreach (var row in model.BusinessHierarchicalLinkList)
            {
                row.ChildHubId = ResolveRelationshipId(
                    row.ChildHubId,
                    row.ChildHub?.Id,
                    "BusinessHierarchicalLink",
                    row.Id,
                    "ChildHubId");
                row.ChildHub = RequireTarget(
                    businessHubListById,
                    row.ChildHubId,
                    "BusinessHierarchicalLink",
                    row.Id,
                    "ChildHubId");
            }

            foreach (var row in model.BusinessHierarchicalLinkList)
            {
                row.ParentHubId = ResolveRelationshipId(
                    row.ParentHubId,
                    row.ParentHub?.Id,
                    "BusinessHierarchicalLink",
                    row.Id,
                    "ParentHubId");
                row.ParentHub = RequireTarget(
                    businessHubListById,
                    row.ParentHubId,
                    "BusinessHierarchicalLink",
                    row.Id,
                    "ParentHubId");
            }

            foreach (var row in model.BusinessHierarchicalLinkSatelliteList)
            {
                row.BusinessHierarchicalLinkId = ResolveRelationshipId(
                    row.BusinessHierarchicalLinkId,
                    row.BusinessHierarchicalLink?.Id,
                    "BusinessHierarchicalLinkSatellite",
                    row.Id,
                    "BusinessHierarchicalLinkId");
                row.BusinessHierarchicalLink = RequireTarget(
                    businessHierarchicalLinkListById,
                    row.BusinessHierarchicalLinkId,
                    "BusinessHierarchicalLinkSatellite",
                    row.Id,
                    "BusinessHierarchicalLinkId");
            }

            foreach (var row in model.BusinessHierarchicalLinkSatelliteAttributeList)
            {
                row.BusinessHierarchicalLinkSatelliteId = ResolveRelationshipId(
                    row.BusinessHierarchicalLinkSatelliteId,
                    row.BusinessHierarchicalLinkSatellite?.Id,
                    "BusinessHierarchicalLinkSatelliteAttribute",
                    row.Id,
                    "BusinessHierarchicalLinkSatelliteId");
                row.BusinessHierarchicalLinkSatellite = RequireTarget(
                    businessHierarchicalLinkSatelliteListById,
                    row.BusinessHierarchicalLinkSatelliteId,
                    "BusinessHierarchicalLinkSatelliteAttribute",
                    row.Id,
                    "BusinessHierarchicalLinkSatelliteId");
            }

            foreach (var row in model.BusinessHierarchicalLinkSatelliteAttributeDataTypeDetailList)
            {
                row.BusinessHierarchicalLinkSatelliteAttributeId = ResolveRelationshipId(
                    row.BusinessHierarchicalLinkSatelliteAttributeId,
                    row.BusinessHierarchicalLinkSatelliteAttribute?.Id,
                    "BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail",
                    row.Id,
                    "BusinessHierarchicalLinkSatelliteAttributeId");
                row.BusinessHierarchicalLinkSatelliteAttribute = RequireTarget(
                    businessHierarchicalLinkSatelliteAttributeListById,
                    row.BusinessHierarchicalLinkSatelliteAttributeId,
                    "BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail",
                    row.Id,
                    "BusinessHierarchicalLinkSatelliteAttributeId");
            }

            foreach (var row in model.BusinessHubKeyPartList)
            {
                row.BusinessHubId = ResolveRelationshipId(
                    row.BusinessHubId,
                    row.BusinessHub?.Id,
                    "BusinessHubKeyPart",
                    row.Id,
                    "BusinessHubId");
                row.BusinessHub = RequireTarget(
                    businessHubListById,
                    row.BusinessHubId,
                    "BusinessHubKeyPart",
                    row.Id,
                    "BusinessHubId");
            }

            foreach (var row in model.BusinessHubKeyPartDataTypeDetailList)
            {
                row.BusinessHubKeyPartId = ResolveRelationshipId(
                    row.BusinessHubKeyPartId,
                    row.BusinessHubKeyPart?.Id,
                    "BusinessHubKeyPartDataTypeDetail",
                    row.Id,
                    "BusinessHubKeyPartId");
                row.BusinessHubKeyPart = RequireTarget(
                    businessHubKeyPartListById,
                    row.BusinessHubKeyPartId,
                    "BusinessHubKeyPartDataTypeDetail",
                    row.Id,
                    "BusinessHubKeyPartId");
            }

            foreach (var row in model.BusinessHubSatelliteList)
            {
                row.BusinessHubId = ResolveRelationshipId(
                    row.BusinessHubId,
                    row.BusinessHub?.Id,
                    "BusinessHubSatellite",
                    row.Id,
                    "BusinessHubId");
                row.BusinessHub = RequireTarget(
                    businessHubListById,
                    row.BusinessHubId,
                    "BusinessHubSatellite",
                    row.Id,
                    "BusinessHubId");
            }

            foreach (var row in model.BusinessHubSatelliteAttributeList)
            {
                row.BusinessHubSatelliteId = ResolveRelationshipId(
                    row.BusinessHubSatelliteId,
                    row.BusinessHubSatellite?.Id,
                    "BusinessHubSatelliteAttribute",
                    row.Id,
                    "BusinessHubSatelliteId");
                row.BusinessHubSatellite = RequireTarget(
                    businessHubSatelliteListById,
                    row.BusinessHubSatelliteId,
                    "BusinessHubSatelliteAttribute",
                    row.Id,
                    "BusinessHubSatelliteId");
            }

            foreach (var row in model.BusinessHubSatelliteAttributeDataTypeDetailList)
            {
                row.BusinessHubSatelliteAttributeId = ResolveRelationshipId(
                    row.BusinessHubSatelliteAttributeId,
                    row.BusinessHubSatelliteAttribute?.Id,
                    "BusinessHubSatelliteAttributeDataTypeDetail",
                    row.Id,
                    "BusinessHubSatelliteAttributeId");
                row.BusinessHubSatelliteAttribute = RequireTarget(
                    businessHubSatelliteAttributeListById,
                    row.BusinessHubSatelliteAttributeId,
                    "BusinessHubSatelliteAttributeDataTypeDetail",
                    row.Id,
                    "BusinessHubSatelliteAttributeId");
            }

            foreach (var row in model.BusinessLinkHubList)
            {
                row.BusinessHubId = ResolveRelationshipId(
                    row.BusinessHubId,
                    row.BusinessHub?.Id,
                    "BusinessLinkHub",
                    row.Id,
                    "BusinessHubId");
                row.BusinessHub = RequireTarget(
                    businessHubListById,
                    row.BusinessHubId,
                    "BusinessLinkHub",
                    row.Id,
                    "BusinessHubId");
            }

            foreach (var row in model.BusinessLinkHubList)
            {
                row.BusinessLinkId = ResolveRelationshipId(
                    row.BusinessLinkId,
                    row.BusinessLink?.Id,
                    "BusinessLinkHub",
                    row.Id,
                    "BusinessLinkId");
                row.BusinessLink = RequireTarget(
                    businessLinkListById,
                    row.BusinessLinkId,
                    "BusinessLinkHub",
                    row.Id,
                    "BusinessLinkId");
            }

            foreach (var row in model.BusinessLinkSatelliteList)
            {
                row.BusinessLinkId = ResolveRelationshipId(
                    row.BusinessLinkId,
                    row.BusinessLink?.Id,
                    "BusinessLinkSatellite",
                    row.Id,
                    "BusinessLinkId");
                row.BusinessLink = RequireTarget(
                    businessLinkListById,
                    row.BusinessLinkId,
                    "BusinessLinkSatellite",
                    row.Id,
                    "BusinessLinkId");
            }

            foreach (var row in model.BusinessLinkSatelliteAttributeList)
            {
                row.BusinessLinkSatelliteId = ResolveRelationshipId(
                    row.BusinessLinkSatelliteId,
                    row.BusinessLinkSatellite?.Id,
                    "BusinessLinkSatelliteAttribute",
                    row.Id,
                    "BusinessLinkSatelliteId");
                row.BusinessLinkSatellite = RequireTarget(
                    businessLinkSatelliteListById,
                    row.BusinessLinkSatelliteId,
                    "BusinessLinkSatelliteAttribute",
                    row.Id,
                    "BusinessLinkSatelliteId");
            }

            foreach (var row in model.BusinessLinkSatelliteAttributeDataTypeDetailList)
            {
                row.BusinessLinkSatelliteAttributeId = ResolveRelationshipId(
                    row.BusinessLinkSatelliteAttributeId,
                    row.BusinessLinkSatelliteAttribute?.Id,
                    "BusinessLinkSatelliteAttributeDataTypeDetail",
                    row.Id,
                    "BusinessLinkSatelliteAttributeId");
                row.BusinessLinkSatelliteAttribute = RequireTarget(
                    businessLinkSatelliteAttributeListById,
                    row.BusinessLinkSatelliteAttributeId,
                    "BusinessLinkSatelliteAttributeDataTypeDetail",
                    row.Id,
                    "BusinessLinkSatelliteAttributeId");
            }

            foreach (var row in model.BusinessPointInTimeList)
            {
                row.BusinessHubId = ResolveRelationshipId(
                    row.BusinessHubId,
                    row.BusinessHub?.Id,
                    "BusinessPointInTime",
                    row.Id,
                    "BusinessHubId");
                row.BusinessHub = RequireTarget(
                    businessHubListById,
                    row.BusinessHubId,
                    "BusinessPointInTime",
                    row.Id,
                    "BusinessHubId");
            }

            foreach (var row in model.BusinessPointInTimeHubSatelliteList)
            {
                row.BusinessHubSatelliteId = ResolveRelationshipId(
                    row.BusinessHubSatelliteId,
                    row.BusinessHubSatellite?.Id,
                    "BusinessPointInTimeHubSatellite",
                    row.Id,
                    "BusinessHubSatelliteId");
                row.BusinessHubSatellite = RequireTarget(
                    businessHubSatelliteListById,
                    row.BusinessHubSatelliteId,
                    "BusinessPointInTimeHubSatellite",
                    row.Id,
                    "BusinessHubSatelliteId");
            }

            foreach (var row in model.BusinessPointInTimeHubSatelliteList)
            {
                row.BusinessPointInTimeId = ResolveRelationshipId(
                    row.BusinessPointInTimeId,
                    row.BusinessPointInTime?.Id,
                    "BusinessPointInTimeHubSatellite",
                    row.Id,
                    "BusinessPointInTimeId");
                row.BusinessPointInTime = RequireTarget(
                    businessPointInTimeListById,
                    row.BusinessPointInTimeId,
                    "BusinessPointInTimeHubSatellite",
                    row.Id,
                    "BusinessPointInTimeId");
            }

            foreach (var row in model.BusinessPointInTimeLinkSatelliteList)
            {
                row.BusinessLinkSatelliteId = ResolveRelationshipId(
                    row.BusinessLinkSatelliteId,
                    row.BusinessLinkSatellite?.Id,
                    "BusinessPointInTimeLinkSatellite",
                    row.Id,
                    "BusinessLinkSatelliteId");
                row.BusinessLinkSatellite = RequireTarget(
                    businessLinkSatelliteListById,
                    row.BusinessLinkSatelliteId,
                    "BusinessPointInTimeLinkSatellite",
                    row.Id,
                    "BusinessLinkSatelliteId");
            }

            foreach (var row in model.BusinessPointInTimeLinkSatelliteList)
            {
                row.BusinessPointInTimeId = ResolveRelationshipId(
                    row.BusinessPointInTimeId,
                    row.BusinessPointInTime?.Id,
                    "BusinessPointInTimeLinkSatellite",
                    row.Id,
                    "BusinessPointInTimeId");
                row.BusinessPointInTime = RequireTarget(
                    businessPointInTimeListById,
                    row.BusinessPointInTimeId,
                    "BusinessPointInTimeLinkSatellite",
                    row.Id,
                    "BusinessPointInTimeId");
            }

            foreach (var row in model.BusinessPointInTimeStampList)
            {
                row.BusinessPointInTimeId = ResolveRelationshipId(
                    row.BusinessPointInTimeId,
                    row.BusinessPointInTime?.Id,
                    "BusinessPointInTimeStamp",
                    row.Id,
                    "BusinessPointInTimeId");
                row.BusinessPointInTime = RequireTarget(
                    businessPointInTimeListById,
                    row.BusinessPointInTimeId,
                    "BusinessPointInTimeStamp",
                    row.Id,
                    "BusinessPointInTimeId");
            }

            foreach (var row in model.BusinessPointInTimeStampDataTypeDetailList)
            {
                row.BusinessPointInTimeStampId = ResolveRelationshipId(
                    row.BusinessPointInTimeStampId,
                    row.BusinessPointInTimeStamp?.Id,
                    "BusinessPointInTimeStampDataTypeDetail",
                    row.Id,
                    "BusinessPointInTimeStampId");
                row.BusinessPointInTimeStamp = RequireTarget(
                    businessPointInTimeStampListById,
                    row.BusinessPointInTimeStampId,
                    "BusinessPointInTimeStampDataTypeDetail",
                    row.Id,
                    "BusinessPointInTimeStampId");
            }

            foreach (var row in model.BusinessReferenceKeyPartList)
            {
                row.BusinessReferenceId = ResolveRelationshipId(
                    row.BusinessReferenceId,
                    row.BusinessReference?.Id,
                    "BusinessReferenceKeyPart",
                    row.Id,
                    "BusinessReferenceId");
                row.BusinessReference = RequireTarget(
                    businessReferenceListById,
                    row.BusinessReferenceId,
                    "BusinessReferenceKeyPart",
                    row.Id,
                    "BusinessReferenceId");
            }

            foreach (var row in model.BusinessReferenceKeyPartDataTypeDetailList)
            {
                row.BusinessReferenceKeyPartId = ResolveRelationshipId(
                    row.BusinessReferenceKeyPartId,
                    row.BusinessReferenceKeyPart?.Id,
                    "BusinessReferenceKeyPartDataTypeDetail",
                    row.Id,
                    "BusinessReferenceKeyPartId");
                row.BusinessReferenceKeyPart = RequireTarget(
                    businessReferenceKeyPartListById,
                    row.BusinessReferenceKeyPartId,
                    "BusinessReferenceKeyPartDataTypeDetail",
                    row.Id,
                    "BusinessReferenceKeyPartId");
            }

            foreach (var row in model.BusinessReferenceSatelliteList)
            {
                row.BusinessReferenceId = ResolveRelationshipId(
                    row.BusinessReferenceId,
                    row.BusinessReference?.Id,
                    "BusinessReferenceSatellite",
                    row.Id,
                    "BusinessReferenceId");
                row.BusinessReference = RequireTarget(
                    businessReferenceListById,
                    row.BusinessReferenceId,
                    "BusinessReferenceSatellite",
                    row.Id,
                    "BusinessReferenceId");
            }

            foreach (var row in model.BusinessReferenceSatelliteAttributeList)
            {
                row.BusinessReferenceSatelliteId = ResolveRelationshipId(
                    row.BusinessReferenceSatelliteId,
                    row.BusinessReferenceSatellite?.Id,
                    "BusinessReferenceSatelliteAttribute",
                    row.Id,
                    "BusinessReferenceSatelliteId");
                row.BusinessReferenceSatellite = RequireTarget(
                    businessReferenceSatelliteListById,
                    row.BusinessReferenceSatelliteId,
                    "BusinessReferenceSatelliteAttribute",
                    row.Id,
                    "BusinessReferenceSatelliteId");
            }

            foreach (var row in model.BusinessReferenceSatelliteAttributeDataTypeDetailList)
            {
                row.BusinessReferenceSatelliteAttributeId = ResolveRelationshipId(
                    row.BusinessReferenceSatelliteAttributeId,
                    row.BusinessReferenceSatelliteAttribute?.Id,
                    "BusinessReferenceSatelliteAttributeDataTypeDetail",
                    row.Id,
                    "BusinessReferenceSatelliteAttributeId");
                row.BusinessReferenceSatelliteAttribute = RequireTarget(
                    businessReferenceSatelliteAttributeListById,
                    row.BusinessReferenceSatelliteAttributeId,
                    "BusinessReferenceSatelliteAttributeDataTypeDetail",
                    row.Id,
                    "BusinessReferenceSatelliteAttributeId");
            }

            foreach (var row in model.BusinessSameAsLinkList)
            {
                row.EquivalentHubId = ResolveRelationshipId(
                    row.EquivalentHubId,
                    row.EquivalentHub?.Id,
                    "BusinessSameAsLink",
                    row.Id,
                    "EquivalentHubId");
                row.EquivalentHub = RequireTarget(
                    businessHubListById,
                    row.EquivalentHubId,
                    "BusinessSameAsLink",
                    row.Id,
                    "EquivalentHubId");
            }

            foreach (var row in model.BusinessSameAsLinkList)
            {
                row.PrimaryHubId = ResolveRelationshipId(
                    row.PrimaryHubId,
                    row.PrimaryHub?.Id,
                    "BusinessSameAsLink",
                    row.Id,
                    "PrimaryHubId");
                row.PrimaryHub = RequireTarget(
                    businessHubListById,
                    row.PrimaryHubId,
                    "BusinessSameAsLink",
                    row.Id,
                    "PrimaryHubId");
            }

            foreach (var row in model.BusinessSameAsLinkSatelliteList)
            {
                row.BusinessSameAsLinkId = ResolveRelationshipId(
                    row.BusinessSameAsLinkId,
                    row.BusinessSameAsLink?.Id,
                    "BusinessSameAsLinkSatellite",
                    row.Id,
                    "BusinessSameAsLinkId");
                row.BusinessSameAsLink = RequireTarget(
                    businessSameAsLinkListById,
                    row.BusinessSameAsLinkId,
                    "BusinessSameAsLinkSatellite",
                    row.Id,
                    "BusinessSameAsLinkId");
            }

            foreach (var row in model.BusinessSameAsLinkSatelliteAttributeList)
            {
                row.BusinessSameAsLinkSatelliteId = ResolveRelationshipId(
                    row.BusinessSameAsLinkSatelliteId,
                    row.BusinessSameAsLinkSatellite?.Id,
                    "BusinessSameAsLinkSatelliteAttribute",
                    row.Id,
                    "BusinessSameAsLinkSatelliteId");
                row.BusinessSameAsLinkSatellite = RequireTarget(
                    businessSameAsLinkSatelliteListById,
                    row.BusinessSameAsLinkSatelliteId,
                    "BusinessSameAsLinkSatelliteAttribute",
                    row.Id,
                    "BusinessSameAsLinkSatelliteId");
            }

            foreach (var row in model.BusinessSameAsLinkSatelliteAttributeDataTypeDetailList)
            {
                row.BusinessSameAsLinkSatelliteAttributeId = ResolveRelationshipId(
                    row.BusinessSameAsLinkSatelliteAttributeId,
                    row.BusinessSameAsLinkSatelliteAttribute?.Id,
                    "BusinessSameAsLinkSatelliteAttributeDataTypeDetail",
                    row.Id,
                    "BusinessSameAsLinkSatelliteAttributeId");
                row.BusinessSameAsLinkSatelliteAttribute = RequireTarget(
                    businessSameAsLinkSatelliteAttributeListById,
                    row.BusinessSameAsLinkSatelliteAttributeId,
                    "BusinessSameAsLinkSatelliteAttributeDataTypeDetail",
                    row.Id,
                    "BusinessSameAsLinkSatelliteAttributeId");
            }

        }

        private static void NormalizeBusinessBridgeList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessBridgeList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessBridge' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'BusinessBridge' row '{row.Id}' is missing required property 'Name'.");
                row.AnchorHubId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessBridgeHubList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessBridgeHubList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessBridgeHub' contains a row with empty Id.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'BusinessBridgeHub' row '{row.Id}' is missing required property 'Ordinal'.");
                row.RoleName ??= string.Empty;
                row.BusinessBridgeId ??= string.Empty;
                row.BusinessHubId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessBridgeLinkList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessBridgeLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessBridgeLink' contains a row with empty Id.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'BusinessBridgeLink' row '{row.Id}' is missing required property 'Ordinal'.");
                row.RoleName ??= string.Empty;
                row.BusinessBridgeId ??= string.Empty;
                row.BusinessLinkId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessHierarchicalLinkList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessHierarchicalLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessHierarchicalLink' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'BusinessHierarchicalLink' row '{row.Id}' is missing required property 'Name'.");
                row.ChildHubId ??= string.Empty;
                row.ParentHubId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessHierarchicalLinkSatelliteList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessHierarchicalLinkSatelliteList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessHierarchicalLinkSatellite' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'BusinessHierarchicalLinkSatellite' row '{row.Id}' is missing required property 'Name'.");
                row.BusinessHierarchicalLinkId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessHierarchicalLinkSatelliteAttributeList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessHierarchicalLinkSatelliteAttributeList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessHierarchicalLinkSatelliteAttribute' contains a row with empty Id.");
                row.DataTypeId = RequireText(row.DataTypeId, $"Entity 'BusinessHierarchicalLinkSatelliteAttribute' row '{row.Id}' is missing required property 'DataTypeId'.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessHierarchicalLinkSatelliteAttribute' row '{row.Id}' is missing required property 'Name'.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'BusinessHierarchicalLinkSatelliteAttribute' row '{row.Id}' is missing required property 'Ordinal'.");
                row.BusinessHierarchicalLinkSatelliteId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessHierarchicalLinkSatelliteAttributeDataTypeDetailList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessHierarchicalLinkSatelliteAttributeDataTypeDetailList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail' row '{row.Id}' is missing required property 'Name'.");
                row.Value = RequireText(row.Value, $"Entity 'BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail' row '{row.Id}' is missing required property 'Value'.");
                row.BusinessHierarchicalLinkSatelliteAttributeId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessHubList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessHubList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessHub' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'BusinessHub' row '{row.Id}' is missing required property 'Name'.");
            }
        }

        private static void NormalizeBusinessHubKeyPartList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessHubKeyPartList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessHubKeyPart' contains a row with empty Id.");
                row.DataTypeId = RequireText(row.DataTypeId, $"Entity 'BusinessHubKeyPart' row '{row.Id}' is missing required property 'DataTypeId'.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessHubKeyPart' row '{row.Id}' is missing required property 'Name'.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'BusinessHubKeyPart' row '{row.Id}' is missing required property 'Ordinal'.");
                row.BusinessHubId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessHubKeyPartDataTypeDetailList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessHubKeyPartDataTypeDetailList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessHubKeyPartDataTypeDetail' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessHubKeyPartDataTypeDetail' row '{row.Id}' is missing required property 'Name'.");
                row.Value = RequireText(row.Value, $"Entity 'BusinessHubKeyPartDataTypeDetail' row '{row.Id}' is missing required property 'Value'.");
                row.BusinessHubKeyPartId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessHubSatelliteList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessHubSatelliteList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessHubSatellite' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'BusinessHubSatellite' row '{row.Id}' is missing required property 'Name'.");
                row.BusinessHubId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessHubSatelliteAttributeList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessHubSatelliteAttributeList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessHubSatelliteAttribute' contains a row with empty Id.");
                row.DataTypeId = RequireText(row.DataTypeId, $"Entity 'BusinessHubSatelliteAttribute' row '{row.Id}' is missing required property 'DataTypeId'.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessHubSatelliteAttribute' row '{row.Id}' is missing required property 'Name'.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'BusinessHubSatelliteAttribute' row '{row.Id}' is missing required property 'Ordinal'.");
                row.BusinessHubSatelliteId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessHubSatelliteAttributeDataTypeDetailList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessHubSatelliteAttributeDataTypeDetailList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessHubSatelliteAttributeDataTypeDetail' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessHubSatelliteAttributeDataTypeDetail' row '{row.Id}' is missing required property 'Name'.");
                row.Value = RequireText(row.Value, $"Entity 'BusinessHubSatelliteAttributeDataTypeDetail' row '{row.Id}' is missing required property 'Value'.");
                row.BusinessHubSatelliteAttributeId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessLinkList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessLink' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'BusinessLink' row '{row.Id}' is missing required property 'Name'.");
            }
        }

        private static void NormalizeBusinessLinkHubList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessLinkHubList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessLinkHub' contains a row with empty Id.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'BusinessLinkHub' row '{row.Id}' is missing required property 'Ordinal'.");
                row.RoleName ??= string.Empty;
                row.BusinessHubId ??= string.Empty;
                row.BusinessLinkId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessLinkSatelliteList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessLinkSatelliteList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessLinkSatellite' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'BusinessLinkSatellite' row '{row.Id}' is missing required property 'Name'.");
                row.BusinessLinkId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessLinkSatelliteAttributeList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessLinkSatelliteAttributeList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessLinkSatelliteAttribute' contains a row with empty Id.");
                row.DataTypeId = RequireText(row.DataTypeId, $"Entity 'BusinessLinkSatelliteAttribute' row '{row.Id}' is missing required property 'DataTypeId'.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessLinkSatelliteAttribute' row '{row.Id}' is missing required property 'Name'.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'BusinessLinkSatelliteAttribute' row '{row.Id}' is missing required property 'Ordinal'.");
                row.BusinessLinkSatelliteId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessLinkSatelliteAttributeDataTypeDetailList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessLinkSatelliteAttributeDataTypeDetailList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessLinkSatelliteAttributeDataTypeDetail' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessLinkSatelliteAttributeDataTypeDetail' row '{row.Id}' is missing required property 'Name'.");
                row.Value = RequireText(row.Value, $"Entity 'BusinessLinkSatelliteAttributeDataTypeDetail' row '{row.Id}' is missing required property 'Value'.");
                row.BusinessLinkSatelliteAttributeId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessPointInTimeList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessPointInTimeList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessPointInTime' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'BusinessPointInTime' row '{row.Id}' is missing required property 'Name'.");
                row.BusinessHubId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessPointInTimeHubSatelliteList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessPointInTimeHubSatelliteList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessPointInTimeHubSatellite' contains a row with empty Id.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'BusinessPointInTimeHubSatellite' row '{row.Id}' is missing required property 'Ordinal'.");
                row.BusinessHubSatelliteId ??= string.Empty;
                row.BusinessPointInTimeId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessPointInTimeLinkSatelliteList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessPointInTimeLinkSatelliteList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessPointInTimeLinkSatellite' contains a row with empty Id.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'BusinessPointInTimeLinkSatellite' row '{row.Id}' is missing required property 'Ordinal'.");
                row.BusinessLinkSatelliteId ??= string.Empty;
                row.BusinessPointInTimeId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessPointInTimeStampList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessPointInTimeStampList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessPointInTimeStamp' contains a row with empty Id.");
                row.DataTypeId = RequireText(row.DataTypeId, $"Entity 'BusinessPointInTimeStamp' row '{row.Id}' is missing required property 'DataTypeId'.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessPointInTimeStamp' row '{row.Id}' is missing required property 'Name'.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'BusinessPointInTimeStamp' row '{row.Id}' is missing required property 'Ordinal'.");
                row.BusinessPointInTimeId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessPointInTimeStampDataTypeDetailList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessPointInTimeStampDataTypeDetailList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessPointInTimeStampDataTypeDetail' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessPointInTimeStampDataTypeDetail' row '{row.Id}' is missing required property 'Name'.");
                row.Value = RequireText(row.Value, $"Entity 'BusinessPointInTimeStampDataTypeDetail' row '{row.Id}' is missing required property 'Value'.");
                row.BusinessPointInTimeStampId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessReferenceList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessReferenceList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessReference' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'BusinessReference' row '{row.Id}' is missing required property 'Name'.");
            }
        }

        private static void NormalizeBusinessReferenceKeyPartList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessReferenceKeyPartList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessReferenceKeyPart' contains a row with empty Id.");
                row.DataTypeId = RequireText(row.DataTypeId, $"Entity 'BusinessReferenceKeyPart' row '{row.Id}' is missing required property 'DataTypeId'.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessReferenceKeyPart' row '{row.Id}' is missing required property 'Name'.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'BusinessReferenceKeyPart' row '{row.Id}' is missing required property 'Ordinal'.");
                row.BusinessReferenceId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessReferenceKeyPartDataTypeDetailList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessReferenceKeyPartDataTypeDetailList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessReferenceKeyPartDataTypeDetail' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessReferenceKeyPartDataTypeDetail' row '{row.Id}' is missing required property 'Name'.");
                row.Value = RequireText(row.Value, $"Entity 'BusinessReferenceKeyPartDataTypeDetail' row '{row.Id}' is missing required property 'Value'.");
                row.BusinessReferenceKeyPartId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessReferenceSatelliteList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessReferenceSatelliteList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessReferenceSatellite' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'BusinessReferenceSatellite' row '{row.Id}' is missing required property 'Name'.");
                row.BusinessReferenceId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessReferenceSatelliteAttributeList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessReferenceSatelliteAttributeList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessReferenceSatelliteAttribute' contains a row with empty Id.");
                row.DataTypeId = RequireText(row.DataTypeId, $"Entity 'BusinessReferenceSatelliteAttribute' row '{row.Id}' is missing required property 'DataTypeId'.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessReferenceSatelliteAttribute' row '{row.Id}' is missing required property 'Name'.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'BusinessReferenceSatelliteAttribute' row '{row.Id}' is missing required property 'Ordinal'.");
                row.BusinessReferenceSatelliteId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessReferenceSatelliteAttributeDataTypeDetailList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessReferenceSatelliteAttributeDataTypeDetailList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessReferenceSatelliteAttributeDataTypeDetail' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessReferenceSatelliteAttributeDataTypeDetail' row '{row.Id}' is missing required property 'Name'.");
                row.Value = RequireText(row.Value, $"Entity 'BusinessReferenceSatelliteAttributeDataTypeDetail' row '{row.Id}' is missing required property 'Value'.");
                row.BusinessReferenceSatelliteAttributeId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessSameAsLinkList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessSameAsLinkList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessSameAsLink' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'BusinessSameAsLink' row '{row.Id}' is missing required property 'Name'.");
                row.EquivalentHubId ??= string.Empty;
                row.PrimaryHubId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessSameAsLinkSatelliteList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessSameAsLinkSatelliteList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessSameAsLinkSatellite' contains a row with empty Id.");
                row.Description ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'BusinessSameAsLinkSatellite' row '{row.Id}' is missing required property 'Name'.");
                row.BusinessSameAsLinkId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessSameAsLinkSatelliteAttributeList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessSameAsLinkSatelliteAttributeList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessSameAsLinkSatelliteAttribute' contains a row with empty Id.");
                row.DataTypeId = RequireText(row.DataTypeId, $"Entity 'BusinessSameAsLinkSatelliteAttribute' row '{row.Id}' is missing required property 'DataTypeId'.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessSameAsLinkSatelliteAttribute' row '{row.Id}' is missing required property 'Name'.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'BusinessSameAsLinkSatelliteAttribute' row '{row.Id}' is missing required property 'Ordinal'.");
                row.BusinessSameAsLinkSatelliteId ??= string.Empty;
            }
        }

        private static void NormalizeBusinessSameAsLinkSatelliteAttributeDataTypeDetailList(MetaBusinessDataVaultModel model)
        {
            foreach (var row in model.BusinessSameAsLinkSatelliteAttributeDataTypeDetailList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'BusinessSameAsLinkSatelliteAttributeDataTypeDetail' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'BusinessSameAsLinkSatelliteAttributeDataTypeDetail' row '{row.Id}' is missing required property 'Name'.");
                row.Value = RequireText(row.Value, $"Entity 'BusinessSameAsLinkSatelliteAttributeDataTypeDetail' row '{row.Id}' is missing required property 'Value'.");
                row.BusinessSameAsLinkSatelliteAttributeId ??= string.Empty;
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
