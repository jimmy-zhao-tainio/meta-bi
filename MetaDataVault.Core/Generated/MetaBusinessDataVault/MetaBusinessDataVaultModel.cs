using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Meta.Core.Domain;

namespace MetaBusinessDataVault
{
    public sealed partial class MetaBusinessDataVaultModel
    {
        internal MetaBusinessDataVaultModel(
            IReadOnlyList<BusinessBridge> businessBridgeList,
            IReadOnlyList<BusinessBridgeHub> businessBridgeHubList,
            IReadOnlyList<BusinessBridgeHubKeyPartProjection> businessBridgeHubKeyPartProjectionList,
            IReadOnlyList<BusinessBridgeHubSatelliteAttributeProjection> businessBridgeHubSatelliteAttributeProjectionList,
            IReadOnlyList<BusinessBridgeLink> businessBridgeLinkList,
            IReadOnlyList<BusinessBridgeLinkSatelliteAttributeProjection> businessBridgeLinkSatelliteAttributeProjectionList,
            IReadOnlyList<BusinessHub> businessHubList,
            IReadOnlyList<BusinessHubKeyPart> businessHubKeyPartList,
            IReadOnlyList<BusinessHubKeyPartDataTypeDetail> businessHubKeyPartDataTypeDetailList,
            IReadOnlyList<BusinessHubSatellite> businessHubSatelliteList,
            IReadOnlyList<BusinessHubSatelliteAttribute> businessHubSatelliteAttributeList,
            IReadOnlyList<BusinessHubSatelliteAttributeDataTypeDetail> businessHubSatelliteAttributeDataTypeDetailList,
            IReadOnlyList<BusinessHubSatelliteKeyPart> businessHubSatelliteKeyPartList,
            IReadOnlyList<BusinessHubSatelliteKeyPartDataTypeDetail> businessHubSatelliteKeyPartDataTypeDetailList,
            IReadOnlyList<BusinessLink> businessLinkList,
            IReadOnlyList<BusinessLinkEnd> businessLinkEndList,
            IReadOnlyList<BusinessLinkSatellite> businessLinkSatelliteList,
            IReadOnlyList<BusinessLinkSatelliteAttribute> businessLinkSatelliteAttributeList,
            IReadOnlyList<BusinessLinkSatelliteAttributeDataTypeDetail> businessLinkSatelliteAttributeDataTypeDetailList,
            IReadOnlyList<BusinessLinkSatelliteKeyPart> businessLinkSatelliteKeyPartList,
            IReadOnlyList<BusinessLinkSatelliteKeyPartDataTypeDetail> businessLinkSatelliteKeyPartDataTypeDetailList,
            IReadOnlyList<BusinessPointInTime> businessPointInTimeList,
            IReadOnlyList<BusinessPointInTimeHubSatellite> businessPointInTimeHubSatelliteList,
            IReadOnlyList<BusinessPointInTimeLinkSatellite> businessPointInTimeLinkSatelliteList
        )
        {
            BusinessBridgeList = businessBridgeList;
            BusinessBridgeHubList = businessBridgeHubList;
            BusinessBridgeHubKeyPartProjectionList = businessBridgeHubKeyPartProjectionList;
            BusinessBridgeHubSatelliteAttributeProjectionList = businessBridgeHubSatelliteAttributeProjectionList;
            BusinessBridgeLinkList = businessBridgeLinkList;
            BusinessBridgeLinkSatelliteAttributeProjectionList = businessBridgeLinkSatelliteAttributeProjectionList;
            BusinessHubList = businessHubList;
            BusinessHubKeyPartList = businessHubKeyPartList;
            BusinessHubKeyPartDataTypeDetailList = businessHubKeyPartDataTypeDetailList;
            BusinessHubSatelliteList = businessHubSatelliteList;
            BusinessHubSatelliteAttributeList = businessHubSatelliteAttributeList;
            BusinessHubSatelliteAttributeDataTypeDetailList = businessHubSatelliteAttributeDataTypeDetailList;
            BusinessHubSatelliteKeyPartList = businessHubSatelliteKeyPartList;
            BusinessHubSatelliteKeyPartDataTypeDetailList = businessHubSatelliteKeyPartDataTypeDetailList;
            BusinessLinkList = businessLinkList;
            BusinessLinkEndList = businessLinkEndList;
            BusinessLinkSatelliteList = businessLinkSatelliteList;
            BusinessLinkSatelliteAttributeList = businessLinkSatelliteAttributeList;
            BusinessLinkSatelliteAttributeDataTypeDetailList = businessLinkSatelliteAttributeDataTypeDetailList;
            BusinessLinkSatelliteKeyPartList = businessLinkSatelliteKeyPartList;
            BusinessLinkSatelliteKeyPartDataTypeDetailList = businessLinkSatelliteKeyPartDataTypeDetailList;
            BusinessPointInTimeList = businessPointInTimeList;
            BusinessPointInTimeHubSatelliteList = businessPointInTimeHubSatelliteList;
            BusinessPointInTimeLinkSatelliteList = businessPointInTimeLinkSatelliteList;
        }

        public IReadOnlyList<BusinessBridge> BusinessBridgeList { get; }
        public IReadOnlyList<BusinessBridgeHub> BusinessBridgeHubList { get; }
        public IReadOnlyList<BusinessBridgeHubKeyPartProjection> BusinessBridgeHubKeyPartProjectionList { get; }
        public IReadOnlyList<BusinessBridgeHubSatelliteAttributeProjection> BusinessBridgeHubSatelliteAttributeProjectionList { get; }
        public IReadOnlyList<BusinessBridgeLink> BusinessBridgeLinkList { get; }
        public IReadOnlyList<BusinessBridgeLinkSatelliteAttributeProjection> BusinessBridgeLinkSatelliteAttributeProjectionList { get; }
        public IReadOnlyList<BusinessHub> BusinessHubList { get; }
        public IReadOnlyList<BusinessHubKeyPart> BusinessHubKeyPartList { get; }
        public IReadOnlyList<BusinessHubKeyPartDataTypeDetail> BusinessHubKeyPartDataTypeDetailList { get; }
        public IReadOnlyList<BusinessHubSatellite> BusinessHubSatelliteList { get; }
        public IReadOnlyList<BusinessHubSatelliteAttribute> BusinessHubSatelliteAttributeList { get; }
        public IReadOnlyList<BusinessHubSatelliteAttributeDataTypeDetail> BusinessHubSatelliteAttributeDataTypeDetailList { get; }
        public IReadOnlyList<BusinessHubSatelliteKeyPart> BusinessHubSatelliteKeyPartList { get; }
        public IReadOnlyList<BusinessHubSatelliteKeyPartDataTypeDetail> BusinessHubSatelliteKeyPartDataTypeDetailList { get; }
        public IReadOnlyList<BusinessLink> BusinessLinkList { get; }
        public IReadOnlyList<BusinessLinkEnd> BusinessLinkEndList { get; }
        public IReadOnlyList<BusinessLinkSatellite> BusinessLinkSatelliteList { get; }
        public IReadOnlyList<BusinessLinkSatelliteAttribute> BusinessLinkSatelliteAttributeList { get; }
        public IReadOnlyList<BusinessLinkSatelliteAttributeDataTypeDetail> BusinessLinkSatelliteAttributeDataTypeDetailList { get; }
        public IReadOnlyList<BusinessLinkSatelliteKeyPart> BusinessLinkSatelliteKeyPartList { get; }
        public IReadOnlyList<BusinessLinkSatelliteKeyPartDataTypeDetail> BusinessLinkSatelliteKeyPartDataTypeDetailList { get; }
        public IReadOnlyList<BusinessPointInTime> BusinessPointInTimeList { get; }
        public IReadOnlyList<BusinessPointInTimeHubSatellite> BusinessPointInTimeHubSatelliteList { get; }
        public IReadOnlyList<BusinessPointInTimeLinkSatellite> BusinessPointInTimeLinkSatelliteList { get; }
    }

    internal static class MetaBusinessDataVaultModelFactory
    {
        internal static MetaBusinessDataVaultModel CreateFromWorkspace(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new global::System.ArgumentNullException(nameof(workspace));
            }

            var businessBridgeList = new List<BusinessBridge>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessBridge", out var businessBridgeListRecords))
            {
                foreach (var record in businessBridgeListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessBridgeList.Add(new BusinessBridge
                    {
                        Id = record.Id ?? string.Empty,
                        Description = record.Values.TryGetValue("Description", out var descriptionValue) ? descriptionValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        AnchorHubId = record.RelationshipIds.TryGetValue("AnchorHubId", out var anchorHubRelationshipId) ? anchorHubRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessBridgeHubList = new List<BusinessBridgeHub>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessBridgeHub", out var businessBridgeHubListRecords))
            {
                foreach (var record in businessBridgeHubListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessBridgeHubList.Add(new BusinessBridgeHub
                    {
                        Id = record.Id ?? string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        RoleName = record.Values.TryGetValue("RoleName", out var roleNameValue) ? roleNameValue ?? string.Empty : string.Empty,
                        BusinessBridgeId = record.RelationshipIds.TryGetValue("BusinessBridgeId", out var businessBridgeRelationshipId) ? businessBridgeRelationshipId ?? string.Empty : string.Empty,
                        BusinessHubId = record.RelationshipIds.TryGetValue("BusinessHubId", out var businessHubRelationshipId) ? businessHubRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessBridgeHubKeyPartProjectionList = new List<BusinessBridgeHubKeyPartProjection>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessBridgeHubKeyPartProjection", out var businessBridgeHubKeyPartProjectionListRecords))
            {
                foreach (var record in businessBridgeHubKeyPartProjectionListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessBridgeHubKeyPartProjectionList.Add(new BusinessBridgeHubKeyPartProjection
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        BusinessBridgeId = record.RelationshipIds.TryGetValue("BusinessBridgeId", out var businessBridgeRelationshipId) ? businessBridgeRelationshipId ?? string.Empty : string.Empty,
                        BusinessHubKeyPartId = record.RelationshipIds.TryGetValue("BusinessHubKeyPartId", out var businessHubKeyPartRelationshipId) ? businessHubKeyPartRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessBridgeHubSatelliteAttributeProjectionList = new List<BusinessBridgeHubSatelliteAttributeProjection>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessBridgeHubSatelliteAttributeProjection", out var businessBridgeHubSatelliteAttributeProjectionListRecords))
            {
                foreach (var record in businessBridgeHubSatelliteAttributeProjectionListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessBridgeHubSatelliteAttributeProjectionList.Add(new BusinessBridgeHubSatelliteAttributeProjection
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        BusinessBridgeId = record.RelationshipIds.TryGetValue("BusinessBridgeId", out var businessBridgeRelationshipId) ? businessBridgeRelationshipId ?? string.Empty : string.Empty,
                        BusinessHubSatelliteAttributeId = record.RelationshipIds.TryGetValue("BusinessHubSatelliteAttributeId", out var businessHubSatelliteAttributeRelationshipId) ? businessHubSatelliteAttributeRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessBridgeLinkList = new List<BusinessBridgeLink>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessBridgeLink", out var businessBridgeLinkListRecords))
            {
                foreach (var record in businessBridgeLinkListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessBridgeLinkList.Add(new BusinessBridgeLink
                    {
                        Id = record.Id ?? string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        RoleName = record.Values.TryGetValue("RoleName", out var roleNameValue) ? roleNameValue ?? string.Empty : string.Empty,
                        BusinessBridgeId = record.RelationshipIds.TryGetValue("BusinessBridgeId", out var businessBridgeRelationshipId) ? businessBridgeRelationshipId ?? string.Empty : string.Empty,
                        BusinessLinkId = record.RelationshipIds.TryGetValue("BusinessLinkId", out var businessLinkRelationshipId) ? businessLinkRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessBridgeLinkSatelliteAttributeProjectionList = new List<BusinessBridgeLinkSatelliteAttributeProjection>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessBridgeLinkSatelliteAttributeProjection", out var businessBridgeLinkSatelliteAttributeProjectionListRecords))
            {
                foreach (var record in businessBridgeLinkSatelliteAttributeProjectionListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessBridgeLinkSatelliteAttributeProjectionList.Add(new BusinessBridgeLinkSatelliteAttributeProjection
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        BusinessBridgeId = record.RelationshipIds.TryGetValue("BusinessBridgeId", out var businessBridgeRelationshipId) ? businessBridgeRelationshipId ?? string.Empty : string.Empty,
                        BusinessLinkSatelliteAttributeId = record.RelationshipIds.TryGetValue("BusinessLinkSatelliteAttributeId", out var businessLinkSatelliteAttributeRelationshipId) ? businessLinkSatelliteAttributeRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHubList = new List<BusinessHub>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHub", out var businessHubListRecords))
            {
                foreach (var record in businessHubListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHubList.Add(new BusinessHub
                    {
                        Id = record.Id ?? string.Empty,
                        Description = record.Values.TryGetValue("Description", out var descriptionValue) ? descriptionValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHubKeyPartList = new List<BusinessHubKeyPart>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHubKeyPart", out var businessHubKeyPartListRecords))
            {
                foreach (var record in businessHubKeyPartListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHubKeyPartList.Add(new BusinessHubKeyPart
                    {
                        Id = record.Id ?? string.Empty,
                        DataTypeId = record.Values.TryGetValue("DataTypeId", out var dataTypeIdValue) ? dataTypeIdValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        BusinessHubId = record.RelationshipIds.TryGetValue("BusinessHubId", out var businessHubRelationshipId) ? businessHubRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHubKeyPartDataTypeDetailList = new List<BusinessHubKeyPartDataTypeDetail>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHubKeyPartDataTypeDetail", out var businessHubKeyPartDataTypeDetailListRecords))
            {
                foreach (var record in businessHubKeyPartDataTypeDetailListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHubKeyPartDataTypeDetailList.Add(new BusinessHubKeyPartDataTypeDetail
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Value = record.Values.TryGetValue("Value", out var valueValue) ? valueValue ?? string.Empty : string.Empty,
                        BusinessHubKeyPartId = record.RelationshipIds.TryGetValue("BusinessHubKeyPartId", out var businessHubKeyPartRelationshipId) ? businessHubKeyPartRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHubSatelliteList = new List<BusinessHubSatellite>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHubSatellite", out var businessHubSatelliteListRecords))
            {
                foreach (var record in businessHubSatelliteListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHubSatelliteList.Add(new BusinessHubSatellite
                    {
                        Id = record.Id ?? string.Empty,
                        Description = record.Values.TryGetValue("Description", out var descriptionValue) ? descriptionValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SatelliteKind = record.Values.TryGetValue("SatelliteKind", out var satelliteKindValue) ? satelliteKindValue ?? string.Empty : string.Empty,
                        BusinessHubId = record.RelationshipIds.TryGetValue("BusinessHubId", out var businessHubRelationshipId) ? businessHubRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHubSatelliteAttributeList = new List<BusinessHubSatelliteAttribute>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHubSatelliteAttribute", out var businessHubSatelliteAttributeListRecords))
            {
                foreach (var record in businessHubSatelliteAttributeListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHubSatelliteAttributeList.Add(new BusinessHubSatelliteAttribute
                    {
                        Id = record.Id ?? string.Empty,
                        DataTypeId = record.Values.TryGetValue("DataTypeId", out var dataTypeIdValue) ? dataTypeIdValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        BusinessHubSatelliteId = record.RelationshipIds.TryGetValue("BusinessHubSatelliteId", out var businessHubSatelliteRelationshipId) ? businessHubSatelliteRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHubSatelliteAttributeDataTypeDetailList = new List<BusinessHubSatelliteAttributeDataTypeDetail>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHubSatelliteAttributeDataTypeDetail", out var businessHubSatelliteAttributeDataTypeDetailListRecords))
            {
                foreach (var record in businessHubSatelliteAttributeDataTypeDetailListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHubSatelliteAttributeDataTypeDetailList.Add(new BusinessHubSatelliteAttributeDataTypeDetail
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Value = record.Values.TryGetValue("Value", out var valueValue) ? valueValue ?? string.Empty : string.Empty,
                        BusinessHubSatelliteAttributeId = record.RelationshipIds.TryGetValue("BusinessHubSatelliteAttributeId", out var businessHubSatelliteAttributeRelationshipId) ? businessHubSatelliteAttributeRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHubSatelliteKeyPartList = new List<BusinessHubSatelliteKeyPart>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHubSatelliteKeyPart", out var businessHubSatelliteKeyPartListRecords))
            {
                foreach (var record in businessHubSatelliteKeyPartListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHubSatelliteKeyPartList.Add(new BusinessHubSatelliteKeyPart
                    {
                        Id = record.Id ?? string.Empty,
                        DataTypeId = record.Values.TryGetValue("DataTypeId", out var dataTypeIdValue) ? dataTypeIdValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        BusinessHubSatelliteId = record.RelationshipIds.TryGetValue("BusinessHubSatelliteId", out var businessHubSatelliteRelationshipId) ? businessHubSatelliteRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHubSatelliteKeyPartDataTypeDetailList = new List<BusinessHubSatelliteKeyPartDataTypeDetail>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHubSatelliteKeyPartDataTypeDetail", out var businessHubSatelliteKeyPartDataTypeDetailListRecords))
            {
                foreach (var record in businessHubSatelliteKeyPartDataTypeDetailListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHubSatelliteKeyPartDataTypeDetailList.Add(new BusinessHubSatelliteKeyPartDataTypeDetail
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Value = record.Values.TryGetValue("Value", out var valueValue) ? valueValue ?? string.Empty : string.Empty,
                        BusinessHubSatelliteKeyPartId = record.RelationshipIds.TryGetValue("BusinessHubSatelliteKeyPartId", out var businessHubSatelliteKeyPartRelationshipId) ? businessHubSatelliteKeyPartRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessLinkList = new List<BusinessLink>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessLink", out var businessLinkListRecords))
            {
                foreach (var record in businessLinkListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessLinkList.Add(new BusinessLink
                    {
                        Id = record.Id ?? string.Empty,
                        Description = record.Values.TryGetValue("Description", out var descriptionValue) ? descriptionValue ?? string.Empty : string.Empty,
                        LinkKind = record.Values.TryGetValue("LinkKind", out var linkKindValue) ? linkKindValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessLinkEndList = new List<BusinessLinkEnd>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessLinkEnd", out var businessLinkEndListRecords))
            {
                foreach (var record in businessLinkEndListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessLinkEndList.Add(new BusinessLinkEnd
                    {
                        Id = record.Id ?? string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        RoleName = record.Values.TryGetValue("RoleName", out var roleNameValue) ? roleNameValue ?? string.Empty : string.Empty,
                        BusinessHubId = record.RelationshipIds.TryGetValue("BusinessHubId", out var businessHubRelationshipId) ? businessHubRelationshipId ?? string.Empty : string.Empty,
                        BusinessLinkId = record.RelationshipIds.TryGetValue("BusinessLinkId", out var businessLinkRelationshipId) ? businessLinkRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessLinkSatelliteList = new List<BusinessLinkSatellite>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessLinkSatellite", out var businessLinkSatelliteListRecords))
            {
                foreach (var record in businessLinkSatelliteListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessLinkSatelliteList.Add(new BusinessLinkSatellite
                    {
                        Id = record.Id ?? string.Empty,
                        Description = record.Values.TryGetValue("Description", out var descriptionValue) ? descriptionValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SatelliteKind = record.Values.TryGetValue("SatelliteKind", out var satelliteKindValue) ? satelliteKindValue ?? string.Empty : string.Empty,
                        BusinessLinkId = record.RelationshipIds.TryGetValue("BusinessLinkId", out var businessLinkRelationshipId) ? businessLinkRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessLinkSatelliteAttributeList = new List<BusinessLinkSatelliteAttribute>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessLinkSatelliteAttribute", out var businessLinkSatelliteAttributeListRecords))
            {
                foreach (var record in businessLinkSatelliteAttributeListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessLinkSatelliteAttributeList.Add(new BusinessLinkSatelliteAttribute
                    {
                        Id = record.Id ?? string.Empty,
                        DataTypeId = record.Values.TryGetValue("DataTypeId", out var dataTypeIdValue) ? dataTypeIdValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        BusinessLinkSatelliteId = record.RelationshipIds.TryGetValue("BusinessLinkSatelliteId", out var businessLinkSatelliteRelationshipId) ? businessLinkSatelliteRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessLinkSatelliteAttributeDataTypeDetailList = new List<BusinessLinkSatelliteAttributeDataTypeDetail>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessLinkSatelliteAttributeDataTypeDetail", out var businessLinkSatelliteAttributeDataTypeDetailListRecords))
            {
                foreach (var record in businessLinkSatelliteAttributeDataTypeDetailListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessLinkSatelliteAttributeDataTypeDetailList.Add(new BusinessLinkSatelliteAttributeDataTypeDetail
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Value = record.Values.TryGetValue("Value", out var valueValue) ? valueValue ?? string.Empty : string.Empty,
                        BusinessLinkSatelliteAttributeId = record.RelationshipIds.TryGetValue("BusinessLinkSatelliteAttributeId", out var businessLinkSatelliteAttributeRelationshipId) ? businessLinkSatelliteAttributeRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessLinkSatelliteKeyPartList = new List<BusinessLinkSatelliteKeyPart>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessLinkSatelliteKeyPart", out var businessLinkSatelliteKeyPartListRecords))
            {
                foreach (var record in businessLinkSatelliteKeyPartListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessLinkSatelliteKeyPartList.Add(new BusinessLinkSatelliteKeyPart
                    {
                        Id = record.Id ?? string.Empty,
                        DataTypeId = record.Values.TryGetValue("DataTypeId", out var dataTypeIdValue) ? dataTypeIdValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        BusinessLinkSatelliteId = record.RelationshipIds.TryGetValue("BusinessLinkSatelliteId", out var businessLinkSatelliteRelationshipId) ? businessLinkSatelliteRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessLinkSatelliteKeyPartDataTypeDetailList = new List<BusinessLinkSatelliteKeyPartDataTypeDetail>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessLinkSatelliteKeyPartDataTypeDetail", out var businessLinkSatelliteKeyPartDataTypeDetailListRecords))
            {
                foreach (var record in businessLinkSatelliteKeyPartDataTypeDetailListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessLinkSatelliteKeyPartDataTypeDetailList.Add(new BusinessLinkSatelliteKeyPartDataTypeDetail
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Value = record.Values.TryGetValue("Value", out var valueValue) ? valueValue ?? string.Empty : string.Empty,
                        BusinessLinkSatelliteKeyPartId = record.RelationshipIds.TryGetValue("BusinessLinkSatelliteKeyPartId", out var businessLinkSatelliteKeyPartRelationshipId) ? businessLinkSatelliteKeyPartRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessPointInTimeList = new List<BusinessPointInTime>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessPointInTime", out var businessPointInTimeListRecords))
            {
                foreach (var record in businessPointInTimeListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessPointInTimeList.Add(new BusinessPointInTime
                    {
                        Id = record.Id ?? string.Empty,
                        Description = record.Values.TryGetValue("Description", out var descriptionValue) ? descriptionValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        BusinessHubId = record.RelationshipIds.TryGetValue("BusinessHubId", out var businessHubRelationshipId) ? businessHubRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessPointInTimeHubSatelliteList = new List<BusinessPointInTimeHubSatellite>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessPointInTimeHubSatellite", out var businessPointInTimeHubSatelliteListRecords))
            {
                foreach (var record in businessPointInTimeHubSatelliteListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessPointInTimeHubSatelliteList.Add(new BusinessPointInTimeHubSatellite
                    {
                        Id = record.Id ?? string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        BusinessHubSatelliteId = record.RelationshipIds.TryGetValue("BusinessHubSatelliteId", out var businessHubSatelliteRelationshipId) ? businessHubSatelliteRelationshipId ?? string.Empty : string.Empty,
                        BusinessPointInTimeId = record.RelationshipIds.TryGetValue("BusinessPointInTimeId", out var businessPointInTimeRelationshipId) ? businessPointInTimeRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessPointInTimeLinkSatelliteList = new List<BusinessPointInTimeLinkSatellite>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessPointInTimeLinkSatellite", out var businessPointInTimeLinkSatelliteListRecords))
            {
                foreach (var record in businessPointInTimeLinkSatelliteListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessPointInTimeLinkSatelliteList.Add(new BusinessPointInTimeLinkSatellite
                    {
                        Id = record.Id ?? string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        BusinessLinkSatelliteId = record.RelationshipIds.TryGetValue("BusinessLinkSatelliteId", out var businessLinkSatelliteRelationshipId) ? businessLinkSatelliteRelationshipId ?? string.Empty : string.Empty,
                        BusinessPointInTimeId = record.RelationshipIds.TryGetValue("BusinessPointInTimeId", out var businessPointInTimeRelationshipId) ? businessPointInTimeRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessBridgeListById = new Dictionary<string, BusinessBridge>(global::System.StringComparer.Ordinal);
            foreach (var row in businessBridgeList)
            {
                businessBridgeListById[row.Id] = row;
            }

            var businessBridgeHubListById = new Dictionary<string, BusinessBridgeHub>(global::System.StringComparer.Ordinal);
            foreach (var row in businessBridgeHubList)
            {
                businessBridgeHubListById[row.Id] = row;
            }

            var businessBridgeHubKeyPartProjectionListById = new Dictionary<string, BusinessBridgeHubKeyPartProjection>(global::System.StringComparer.Ordinal);
            foreach (var row in businessBridgeHubKeyPartProjectionList)
            {
                businessBridgeHubKeyPartProjectionListById[row.Id] = row;
            }

            var businessBridgeHubSatelliteAttributeProjectionListById = new Dictionary<string, BusinessBridgeHubSatelliteAttributeProjection>(global::System.StringComparer.Ordinal);
            foreach (var row in businessBridgeHubSatelliteAttributeProjectionList)
            {
                businessBridgeHubSatelliteAttributeProjectionListById[row.Id] = row;
            }

            var businessBridgeLinkListById = new Dictionary<string, BusinessBridgeLink>(global::System.StringComparer.Ordinal);
            foreach (var row in businessBridgeLinkList)
            {
                businessBridgeLinkListById[row.Id] = row;
            }

            var businessBridgeLinkSatelliteAttributeProjectionListById = new Dictionary<string, BusinessBridgeLinkSatelliteAttributeProjection>(global::System.StringComparer.Ordinal);
            foreach (var row in businessBridgeLinkSatelliteAttributeProjectionList)
            {
                businessBridgeLinkSatelliteAttributeProjectionListById[row.Id] = row;
            }

            var businessHubListById = new Dictionary<string, BusinessHub>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHubList)
            {
                businessHubListById[row.Id] = row;
            }

            var businessHubKeyPartListById = new Dictionary<string, BusinessHubKeyPart>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHubKeyPartList)
            {
                businessHubKeyPartListById[row.Id] = row;
            }

            var businessHubKeyPartDataTypeDetailListById = new Dictionary<string, BusinessHubKeyPartDataTypeDetail>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHubKeyPartDataTypeDetailList)
            {
                businessHubKeyPartDataTypeDetailListById[row.Id] = row;
            }

            var businessHubSatelliteListById = new Dictionary<string, BusinessHubSatellite>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHubSatelliteList)
            {
                businessHubSatelliteListById[row.Id] = row;
            }

            var businessHubSatelliteAttributeListById = new Dictionary<string, BusinessHubSatelliteAttribute>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHubSatelliteAttributeList)
            {
                businessHubSatelliteAttributeListById[row.Id] = row;
            }

            var businessHubSatelliteAttributeDataTypeDetailListById = new Dictionary<string, BusinessHubSatelliteAttributeDataTypeDetail>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHubSatelliteAttributeDataTypeDetailList)
            {
                businessHubSatelliteAttributeDataTypeDetailListById[row.Id] = row;
            }

            var businessHubSatelliteKeyPartListById = new Dictionary<string, BusinessHubSatelliteKeyPart>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHubSatelliteKeyPartList)
            {
                businessHubSatelliteKeyPartListById[row.Id] = row;
            }

            var businessHubSatelliteKeyPartDataTypeDetailListById = new Dictionary<string, BusinessHubSatelliteKeyPartDataTypeDetail>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHubSatelliteKeyPartDataTypeDetailList)
            {
                businessHubSatelliteKeyPartDataTypeDetailListById[row.Id] = row;
            }

            var businessLinkListById = new Dictionary<string, BusinessLink>(global::System.StringComparer.Ordinal);
            foreach (var row in businessLinkList)
            {
                businessLinkListById[row.Id] = row;
            }

            var businessLinkEndListById = new Dictionary<string, BusinessLinkEnd>(global::System.StringComparer.Ordinal);
            foreach (var row in businessLinkEndList)
            {
                businessLinkEndListById[row.Id] = row;
            }

            var businessLinkSatelliteListById = new Dictionary<string, BusinessLinkSatellite>(global::System.StringComparer.Ordinal);
            foreach (var row in businessLinkSatelliteList)
            {
                businessLinkSatelliteListById[row.Id] = row;
            }

            var businessLinkSatelliteAttributeListById = new Dictionary<string, BusinessLinkSatelliteAttribute>(global::System.StringComparer.Ordinal);
            foreach (var row in businessLinkSatelliteAttributeList)
            {
                businessLinkSatelliteAttributeListById[row.Id] = row;
            }

            var businessLinkSatelliteAttributeDataTypeDetailListById = new Dictionary<string, BusinessLinkSatelliteAttributeDataTypeDetail>(global::System.StringComparer.Ordinal);
            foreach (var row in businessLinkSatelliteAttributeDataTypeDetailList)
            {
                businessLinkSatelliteAttributeDataTypeDetailListById[row.Id] = row;
            }

            var businessLinkSatelliteKeyPartListById = new Dictionary<string, BusinessLinkSatelliteKeyPart>(global::System.StringComparer.Ordinal);
            foreach (var row in businessLinkSatelliteKeyPartList)
            {
                businessLinkSatelliteKeyPartListById[row.Id] = row;
            }

            var businessLinkSatelliteKeyPartDataTypeDetailListById = new Dictionary<string, BusinessLinkSatelliteKeyPartDataTypeDetail>(global::System.StringComparer.Ordinal);
            foreach (var row in businessLinkSatelliteKeyPartDataTypeDetailList)
            {
                businessLinkSatelliteKeyPartDataTypeDetailListById[row.Id] = row;
            }

            var businessPointInTimeListById = new Dictionary<string, BusinessPointInTime>(global::System.StringComparer.Ordinal);
            foreach (var row in businessPointInTimeList)
            {
                businessPointInTimeListById[row.Id] = row;
            }

            var businessPointInTimeHubSatelliteListById = new Dictionary<string, BusinessPointInTimeHubSatellite>(global::System.StringComparer.Ordinal);
            foreach (var row in businessPointInTimeHubSatelliteList)
            {
                businessPointInTimeHubSatelliteListById[row.Id] = row;
            }

            var businessPointInTimeLinkSatelliteListById = new Dictionary<string, BusinessPointInTimeLinkSatellite>(global::System.StringComparer.Ordinal);
            foreach (var row in businessPointInTimeLinkSatelliteList)
            {
                businessPointInTimeLinkSatelliteListById[row.Id] = row;
            }

            foreach (var row in businessBridgeList)
            {
                row.AnchorHub = RequireTarget(
                    businessHubListById,
                    row.AnchorHubId,
                    "BusinessBridge",
                    row.Id,
                    "AnchorHubId");
            }

            foreach (var row in businessBridgeHubList)
            {
                row.BusinessBridge = RequireTarget(
                    businessBridgeListById,
                    row.BusinessBridgeId,
                    "BusinessBridgeHub",
                    row.Id,
                    "BusinessBridgeId");
            }

            foreach (var row in businessBridgeHubList)
            {
                row.BusinessHub = RequireTarget(
                    businessHubListById,
                    row.BusinessHubId,
                    "BusinessBridgeHub",
                    row.Id,
                    "BusinessHubId");
            }

            foreach (var row in businessBridgeHubKeyPartProjectionList)
            {
                row.BusinessBridge = RequireTarget(
                    businessBridgeListById,
                    row.BusinessBridgeId,
                    "BusinessBridgeHubKeyPartProjection",
                    row.Id,
                    "BusinessBridgeId");
            }

            foreach (var row in businessBridgeHubKeyPartProjectionList)
            {
                row.BusinessHubKeyPart = RequireTarget(
                    businessHubKeyPartListById,
                    row.BusinessHubKeyPartId,
                    "BusinessBridgeHubKeyPartProjection",
                    row.Id,
                    "BusinessHubKeyPartId");
            }

            foreach (var row in businessBridgeHubSatelliteAttributeProjectionList)
            {
                row.BusinessBridge = RequireTarget(
                    businessBridgeListById,
                    row.BusinessBridgeId,
                    "BusinessBridgeHubSatelliteAttributeProjection",
                    row.Id,
                    "BusinessBridgeId");
            }

            foreach (var row in businessBridgeHubSatelliteAttributeProjectionList)
            {
                row.BusinessHubSatelliteAttribute = RequireTarget(
                    businessHubSatelliteAttributeListById,
                    row.BusinessHubSatelliteAttributeId,
                    "BusinessBridgeHubSatelliteAttributeProjection",
                    row.Id,
                    "BusinessHubSatelliteAttributeId");
            }

            foreach (var row in businessBridgeLinkList)
            {
                row.BusinessBridge = RequireTarget(
                    businessBridgeListById,
                    row.BusinessBridgeId,
                    "BusinessBridgeLink",
                    row.Id,
                    "BusinessBridgeId");
            }

            foreach (var row in businessBridgeLinkList)
            {
                row.BusinessLink = RequireTarget(
                    businessLinkListById,
                    row.BusinessLinkId,
                    "BusinessBridgeLink",
                    row.Id,
                    "BusinessLinkId");
            }

            foreach (var row in businessBridgeLinkSatelliteAttributeProjectionList)
            {
                row.BusinessBridge = RequireTarget(
                    businessBridgeListById,
                    row.BusinessBridgeId,
                    "BusinessBridgeLinkSatelliteAttributeProjection",
                    row.Id,
                    "BusinessBridgeId");
            }

            foreach (var row in businessBridgeLinkSatelliteAttributeProjectionList)
            {
                row.BusinessLinkSatelliteAttribute = RequireTarget(
                    businessLinkSatelliteAttributeListById,
                    row.BusinessLinkSatelliteAttributeId,
                    "BusinessBridgeLinkSatelliteAttributeProjection",
                    row.Id,
                    "BusinessLinkSatelliteAttributeId");
            }

            foreach (var row in businessHubKeyPartList)
            {
                row.BusinessHub = RequireTarget(
                    businessHubListById,
                    row.BusinessHubId,
                    "BusinessHubKeyPart",
                    row.Id,
                    "BusinessHubId");
            }

            foreach (var row in businessHubKeyPartDataTypeDetailList)
            {
                row.BusinessHubKeyPart = RequireTarget(
                    businessHubKeyPartListById,
                    row.BusinessHubKeyPartId,
                    "BusinessHubKeyPartDataTypeDetail",
                    row.Id,
                    "BusinessHubKeyPartId");
            }

            foreach (var row in businessHubSatelliteList)
            {
                row.BusinessHub = RequireTarget(
                    businessHubListById,
                    row.BusinessHubId,
                    "BusinessHubSatellite",
                    row.Id,
                    "BusinessHubId");
            }

            foreach (var row in businessHubSatelliteAttributeList)
            {
                row.BusinessHubSatellite = RequireTarget(
                    businessHubSatelliteListById,
                    row.BusinessHubSatelliteId,
                    "BusinessHubSatelliteAttribute",
                    row.Id,
                    "BusinessHubSatelliteId");
            }

            foreach (var row in businessHubSatelliteAttributeDataTypeDetailList)
            {
                row.BusinessHubSatelliteAttribute = RequireTarget(
                    businessHubSatelliteAttributeListById,
                    row.BusinessHubSatelliteAttributeId,
                    "BusinessHubSatelliteAttributeDataTypeDetail",
                    row.Id,
                    "BusinessHubSatelliteAttributeId");
            }

            foreach (var row in businessHubSatelliteKeyPartList)
            {
                row.BusinessHubSatellite = RequireTarget(
                    businessHubSatelliteListById,
                    row.BusinessHubSatelliteId,
                    "BusinessHubSatelliteKeyPart",
                    row.Id,
                    "BusinessHubSatelliteId");
            }

            foreach (var row in businessHubSatelliteKeyPartDataTypeDetailList)
            {
                row.BusinessHubSatelliteKeyPart = RequireTarget(
                    businessHubSatelliteKeyPartListById,
                    row.BusinessHubSatelliteKeyPartId,
                    "BusinessHubSatelliteKeyPartDataTypeDetail",
                    row.Id,
                    "BusinessHubSatelliteKeyPartId");
            }

            foreach (var row in businessLinkEndList)
            {
                row.BusinessHub = RequireTarget(
                    businessHubListById,
                    row.BusinessHubId,
                    "BusinessLinkEnd",
                    row.Id,
                    "BusinessHubId");
            }

            foreach (var row in businessLinkEndList)
            {
                row.BusinessLink = RequireTarget(
                    businessLinkListById,
                    row.BusinessLinkId,
                    "BusinessLinkEnd",
                    row.Id,
                    "BusinessLinkId");
            }

            foreach (var row in businessLinkSatelliteList)
            {
                row.BusinessLink = RequireTarget(
                    businessLinkListById,
                    row.BusinessLinkId,
                    "BusinessLinkSatellite",
                    row.Id,
                    "BusinessLinkId");
            }

            foreach (var row in businessLinkSatelliteAttributeList)
            {
                row.BusinessLinkSatellite = RequireTarget(
                    businessLinkSatelliteListById,
                    row.BusinessLinkSatelliteId,
                    "BusinessLinkSatelliteAttribute",
                    row.Id,
                    "BusinessLinkSatelliteId");
            }

            foreach (var row in businessLinkSatelliteAttributeDataTypeDetailList)
            {
                row.BusinessLinkSatelliteAttribute = RequireTarget(
                    businessLinkSatelliteAttributeListById,
                    row.BusinessLinkSatelliteAttributeId,
                    "BusinessLinkSatelliteAttributeDataTypeDetail",
                    row.Id,
                    "BusinessLinkSatelliteAttributeId");
            }

            foreach (var row in businessLinkSatelliteKeyPartList)
            {
                row.BusinessLinkSatellite = RequireTarget(
                    businessLinkSatelliteListById,
                    row.BusinessLinkSatelliteId,
                    "BusinessLinkSatelliteKeyPart",
                    row.Id,
                    "BusinessLinkSatelliteId");
            }

            foreach (var row in businessLinkSatelliteKeyPartDataTypeDetailList)
            {
                row.BusinessLinkSatelliteKeyPart = RequireTarget(
                    businessLinkSatelliteKeyPartListById,
                    row.BusinessLinkSatelliteKeyPartId,
                    "BusinessLinkSatelliteKeyPartDataTypeDetail",
                    row.Id,
                    "BusinessLinkSatelliteKeyPartId");
            }

            foreach (var row in businessPointInTimeList)
            {
                row.BusinessHub = RequireTarget(
                    businessHubListById,
                    row.BusinessHubId,
                    "BusinessPointInTime",
                    row.Id,
                    "BusinessHubId");
            }

            foreach (var row in businessPointInTimeHubSatelliteList)
            {
                row.BusinessHubSatellite = RequireTarget(
                    businessHubSatelliteListById,
                    row.BusinessHubSatelliteId,
                    "BusinessPointInTimeHubSatellite",
                    row.Id,
                    "BusinessHubSatelliteId");
            }

            foreach (var row in businessPointInTimeHubSatelliteList)
            {
                row.BusinessPointInTime = RequireTarget(
                    businessPointInTimeListById,
                    row.BusinessPointInTimeId,
                    "BusinessPointInTimeHubSatellite",
                    row.Id,
                    "BusinessPointInTimeId");
            }

            foreach (var row in businessPointInTimeLinkSatelliteList)
            {
                row.BusinessLinkSatellite = RequireTarget(
                    businessLinkSatelliteListById,
                    row.BusinessLinkSatelliteId,
                    "BusinessPointInTimeLinkSatellite",
                    row.Id,
                    "BusinessLinkSatelliteId");
            }

            foreach (var row in businessPointInTimeLinkSatelliteList)
            {
                row.BusinessPointInTime = RequireTarget(
                    businessPointInTimeListById,
                    row.BusinessPointInTimeId,
                    "BusinessPointInTimeLinkSatellite",
                    row.Id,
                    "BusinessPointInTimeId");
            }

            return new MetaBusinessDataVaultModel(
                new ReadOnlyCollection<BusinessBridge>(businessBridgeList),
                new ReadOnlyCollection<BusinessBridgeHub>(businessBridgeHubList),
                new ReadOnlyCollection<BusinessBridgeHubKeyPartProjection>(businessBridgeHubKeyPartProjectionList),
                new ReadOnlyCollection<BusinessBridgeHubSatelliteAttributeProjection>(businessBridgeHubSatelliteAttributeProjectionList),
                new ReadOnlyCollection<BusinessBridgeLink>(businessBridgeLinkList),
                new ReadOnlyCollection<BusinessBridgeLinkSatelliteAttributeProjection>(businessBridgeLinkSatelliteAttributeProjectionList),
                new ReadOnlyCollection<BusinessHub>(businessHubList),
                new ReadOnlyCollection<BusinessHubKeyPart>(businessHubKeyPartList),
                new ReadOnlyCollection<BusinessHubKeyPartDataTypeDetail>(businessHubKeyPartDataTypeDetailList),
                new ReadOnlyCollection<BusinessHubSatellite>(businessHubSatelliteList),
                new ReadOnlyCollection<BusinessHubSatelliteAttribute>(businessHubSatelliteAttributeList),
                new ReadOnlyCollection<BusinessHubSatelliteAttributeDataTypeDetail>(businessHubSatelliteAttributeDataTypeDetailList),
                new ReadOnlyCollection<BusinessHubSatelliteKeyPart>(businessHubSatelliteKeyPartList),
                new ReadOnlyCollection<BusinessHubSatelliteKeyPartDataTypeDetail>(businessHubSatelliteKeyPartDataTypeDetailList),
                new ReadOnlyCollection<BusinessLink>(businessLinkList),
                new ReadOnlyCollection<BusinessLinkEnd>(businessLinkEndList),
                new ReadOnlyCollection<BusinessLinkSatellite>(businessLinkSatelliteList),
                new ReadOnlyCollection<BusinessLinkSatelliteAttribute>(businessLinkSatelliteAttributeList),
                new ReadOnlyCollection<BusinessLinkSatelliteAttributeDataTypeDetail>(businessLinkSatelliteAttributeDataTypeDetailList),
                new ReadOnlyCollection<BusinessLinkSatelliteKeyPart>(businessLinkSatelliteKeyPartList),
                new ReadOnlyCollection<BusinessLinkSatelliteKeyPartDataTypeDetail>(businessLinkSatelliteKeyPartDataTypeDetailList),
                new ReadOnlyCollection<BusinessPointInTime>(businessPointInTimeList),
                new ReadOnlyCollection<BusinessPointInTimeHubSatellite>(businessPointInTimeHubSatelliteList),
                new ReadOnlyCollection<BusinessPointInTimeLinkSatellite>(businessPointInTimeLinkSatelliteList)
            );
        }

        private static T RequireTarget<T>(
            Dictionary<string, T> rowsById,
            string targetId,
            string sourceEntityName,
            string sourceId,
            string relationshipName)
            where T : class
        {
            if (string.IsNullOrEmpty(targetId))
            {
                throw new global::System.InvalidOperationException(
                    $"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' is empty."
                );
            }

            if (!rowsById.TryGetValue(targetId, out var target))
            {
                throw new global::System.InvalidOperationException(
                    $"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' points to missing Id '{targetId}'."
                );
            }

            return target;
        }
    }
}
