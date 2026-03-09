using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Meta.Core.Domain;

namespace MetaDataVaultImplementation
{
    public sealed partial class MetaDataVaultImplementationModel
    {
        internal MetaDataVaultImplementationModel(
            IReadOnlyList<BusinessBridgeImplementation> businessBridgeImplementationList,
            IReadOnlyList<BusinessHierarchicalLinkImplementation> businessHierarchicalLinkImplementationList,
            IReadOnlyList<BusinessHierarchicalLinkSatelliteImplementation> businessHierarchicalLinkSatelliteImplementationList,
            IReadOnlyList<BusinessHubImplementation> businessHubImplementationList,
            IReadOnlyList<BusinessHubSatelliteImplementation> businessHubSatelliteImplementationList,
            IReadOnlyList<BusinessLinkImplementation> businessLinkImplementationList,
            IReadOnlyList<BusinessLinkSatelliteImplementation> businessLinkSatelliteImplementationList,
            IReadOnlyList<BusinessPointInTimeImplementation> businessPointInTimeImplementationList,
            IReadOnlyList<BusinessSameAsLinkImplementation> businessSameAsLinkImplementationList,
            IReadOnlyList<BusinessSameAsLinkSatelliteImplementation> businessSameAsLinkSatelliteImplementationList,
            IReadOnlyList<RawHubImplementation> rawHubImplementationList,
            IReadOnlyList<RawHubSatelliteImplementation> rawHubSatelliteImplementationList,
            IReadOnlyList<RawLinkImplementation> rawLinkImplementationList,
            IReadOnlyList<RawLinkSatelliteImplementation> rawLinkSatelliteImplementationList
        )
        {
            BusinessBridgeImplementationList = businessBridgeImplementationList;
            BusinessHierarchicalLinkImplementationList = businessHierarchicalLinkImplementationList;
            BusinessHierarchicalLinkSatelliteImplementationList = businessHierarchicalLinkSatelliteImplementationList;
            BusinessHubImplementationList = businessHubImplementationList;
            BusinessHubSatelliteImplementationList = businessHubSatelliteImplementationList;
            BusinessLinkImplementationList = businessLinkImplementationList;
            BusinessLinkSatelliteImplementationList = businessLinkSatelliteImplementationList;
            BusinessPointInTimeImplementationList = businessPointInTimeImplementationList;
            BusinessSameAsLinkImplementationList = businessSameAsLinkImplementationList;
            BusinessSameAsLinkSatelliteImplementationList = businessSameAsLinkSatelliteImplementationList;
            RawHubImplementationList = rawHubImplementationList;
            RawHubSatelliteImplementationList = rawHubSatelliteImplementationList;
            RawLinkImplementationList = rawLinkImplementationList;
            RawLinkSatelliteImplementationList = rawLinkSatelliteImplementationList;
        }

        public IReadOnlyList<BusinessBridgeImplementation> BusinessBridgeImplementationList { get; }
        public IReadOnlyList<BusinessHierarchicalLinkImplementation> BusinessHierarchicalLinkImplementationList { get; }
        public IReadOnlyList<BusinessHierarchicalLinkSatelliteImplementation> BusinessHierarchicalLinkSatelliteImplementationList { get; }
        public IReadOnlyList<BusinessHubImplementation> BusinessHubImplementationList { get; }
        public IReadOnlyList<BusinessHubSatelliteImplementation> BusinessHubSatelliteImplementationList { get; }
        public IReadOnlyList<BusinessLinkImplementation> BusinessLinkImplementationList { get; }
        public IReadOnlyList<BusinessLinkSatelliteImplementation> BusinessLinkSatelliteImplementationList { get; }
        public IReadOnlyList<BusinessPointInTimeImplementation> BusinessPointInTimeImplementationList { get; }
        public IReadOnlyList<BusinessSameAsLinkImplementation> BusinessSameAsLinkImplementationList { get; }
        public IReadOnlyList<BusinessSameAsLinkSatelliteImplementation> BusinessSameAsLinkSatelliteImplementationList { get; }
        public IReadOnlyList<RawHubImplementation> RawHubImplementationList { get; }
        public IReadOnlyList<RawHubSatelliteImplementation> RawHubSatelliteImplementationList { get; }
        public IReadOnlyList<RawLinkImplementation> RawLinkImplementationList { get; }
        public IReadOnlyList<RawLinkSatelliteImplementation> RawLinkSatelliteImplementationList { get; }
    }

    internal static class MetaDataVaultImplementationModelFactory
    {
        internal static MetaDataVaultImplementationModel CreateFromWorkspace(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new global::System.ArgumentNullException(nameof(workspace));
            }

            var businessBridgeImplementationList = new List<BusinessBridgeImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessBridgeImplementation", out var businessBridgeImplementationListRecords))
            {
                foreach (var record in businessBridgeImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessBridgeImplementationList.Add(new BusinessBridgeImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        DepthColumnName = record.Values.TryGetValue("DepthColumnName", out var depthColumnNameValue) ? depthColumnNameValue ?? string.Empty : string.Empty,
                        DepthDataTypeId = record.Values.TryGetValue("DepthDataTypeId", out var depthDataTypeIdValue) ? depthDataTypeIdValue ?? string.Empty : string.Empty,
                        EffectiveFromColumnName = record.Values.TryGetValue("EffectiveFromColumnName", out var effectiveFromColumnNameValue) ? effectiveFromColumnNameValue ?? string.Empty : string.Empty,
                        EffectiveFromDataTypeId = record.Values.TryGetValue("EffectiveFromDataTypeId", out var effectiveFromDataTypeIdValue) ? effectiveFromDataTypeIdValue ?? string.Empty : string.Empty,
                        EffectiveFromPrecision = record.Values.TryGetValue("EffectiveFromPrecision", out var effectiveFromPrecisionValue) ? effectiveFromPrecisionValue ?? string.Empty : string.Empty,
                        EffectiveToColumnName = record.Values.TryGetValue("EffectiveToColumnName", out var effectiveToColumnNameValue) ? effectiveToColumnNameValue ?? string.Empty : string.Empty,
                        EffectiveToDataTypeId = record.Values.TryGetValue("EffectiveToDataTypeId", out var effectiveToDataTypeIdValue) ? effectiveToDataTypeIdValue ?? string.Empty : string.Empty,
                        EffectiveToPrecision = record.Values.TryGetValue("EffectiveToPrecision", out var effectiveToPrecisionValue) ? effectiveToPrecisionValue ?? string.Empty : string.Empty,
                        PathColumnName = record.Values.TryGetValue("PathColumnName", out var pathColumnNameValue) ? pathColumnNameValue ?? string.Empty : string.Empty,
                        PathDataTypeId = record.Values.TryGetValue("PathDataTypeId", out var pathDataTypeIdValue) ? pathDataTypeIdValue ?? string.Empty : string.Empty,
                        PathLength = record.Values.TryGetValue("PathLength", out var pathLengthValue) ? pathLengthValue ?? string.Empty : string.Empty,
                        RelatedHashKeyColumnName = record.Values.TryGetValue("RelatedHashKeyColumnName", out var relatedHashKeyColumnNameValue) ? relatedHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        RelatedHashKeyDataTypeId = record.Values.TryGetValue("RelatedHashKeyDataTypeId", out var relatedHashKeyDataTypeIdValue) ? relatedHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        RelatedHashKeyLength = record.Values.TryGetValue("RelatedHashKeyLength", out var relatedHashKeyLengthValue) ? relatedHashKeyLengthValue ?? string.Empty : string.Empty,
                        RootHashKeyColumnName = record.Values.TryGetValue("RootHashKeyColumnName", out var rootHashKeyColumnNameValue) ? rootHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        RootHashKeyDataTypeId = record.Values.TryGetValue("RootHashKeyDataTypeId", out var rootHashKeyDataTypeIdValue) ? rootHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        RootHashKeyLength = record.Values.TryGetValue("RootHashKeyLength", out var rootHashKeyLengthValue) ? rootHashKeyLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHierarchicalLinkImplementationList = new List<BusinessHierarchicalLinkImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHierarchicalLinkImplementation", out var businessHierarchicalLinkImplementationListRecords))
            {
                foreach (var record in businessHierarchicalLinkImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHierarchicalLinkImplementationList.Add(new BusinessHierarchicalLinkImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        ChildHashKeyColumnName = record.Values.TryGetValue("ChildHashKeyColumnName", out var childHashKeyColumnNameValue) ? childHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyColumnName = record.Values.TryGetValue("HashKeyColumnName", out var hashKeyColumnNameValue) ? hashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyDataTypeId = record.Values.TryGetValue("HashKeyDataTypeId", out var hashKeyDataTypeIdValue) ? hashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyLength = record.Values.TryGetValue("HashKeyLength", out var hashKeyLengthValue) ? hashKeyLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHierarchicalLinkSatelliteImplementationList = new List<BusinessHierarchicalLinkSatelliteImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHierarchicalLinkSatelliteImplementation", out var businessHierarchicalLinkSatelliteImplementationListRecords))
            {
                foreach (var record in businessHierarchicalLinkSatelliteImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHierarchicalLinkSatelliteImplementationList.Add(new BusinessHierarchicalLinkSatelliteImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffColumnName = record.Values.TryGetValue("HashDiffColumnName", out var hashDiffColumnNameValue) ? hashDiffColumnNameValue ?? string.Empty : string.Empty,
                        HashDiffDataTypeId = record.Values.TryGetValue("HashDiffDataTypeId", out var hashDiffDataTypeIdValue) ? hashDiffDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffLength = record.Values.TryGetValue("HashDiffLength", out var hashDiffLengthValue) ? hashDiffLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHubImplementationList = new List<BusinessHubImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHubImplementation", out var businessHubImplementationListRecords))
            {
                foreach (var record in businessHubImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHubImplementationList.Add(new BusinessHubImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyColumnName = record.Values.TryGetValue("HashKeyColumnName", out var hashKeyColumnNameValue) ? hashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyDataTypeId = record.Values.TryGetValue("HashKeyDataTypeId", out var hashKeyDataTypeIdValue) ? hashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyLength = record.Values.TryGetValue("HashKeyLength", out var hashKeyLengthValue) ? hashKeyLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessHubSatelliteImplementationList = new List<BusinessHubSatelliteImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessHubSatelliteImplementation", out var businessHubSatelliteImplementationListRecords))
            {
                foreach (var record in businessHubSatelliteImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessHubSatelliteImplementationList.Add(new BusinessHubSatelliteImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffColumnName = record.Values.TryGetValue("HashDiffColumnName", out var hashDiffColumnNameValue) ? hashDiffColumnNameValue ?? string.Empty : string.Empty,
                        HashDiffDataTypeId = record.Values.TryGetValue("HashDiffDataTypeId", out var hashDiffDataTypeIdValue) ? hashDiffDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffLength = record.Values.TryGetValue("HashDiffLength", out var hashDiffLengthValue) ? hashDiffLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessLinkImplementationList = new List<BusinessLinkImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessLinkImplementation", out var businessLinkImplementationListRecords))
            {
                foreach (var record in businessLinkImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessLinkImplementationList.Add(new BusinessLinkImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        EndHashKeyColumnPattern = record.Values.TryGetValue("EndHashKeyColumnPattern", out var endHashKeyColumnPatternValue) ? endHashKeyColumnPatternValue ?? string.Empty : string.Empty,
                        HashKeyColumnName = record.Values.TryGetValue("HashKeyColumnName", out var hashKeyColumnNameValue) ? hashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyDataTypeId = record.Values.TryGetValue("HashKeyDataTypeId", out var hashKeyDataTypeIdValue) ? hashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyLength = record.Values.TryGetValue("HashKeyLength", out var hashKeyLengthValue) ? hashKeyLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessLinkSatelliteImplementationList = new List<BusinessLinkSatelliteImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessLinkSatelliteImplementation", out var businessLinkSatelliteImplementationListRecords))
            {
                foreach (var record in businessLinkSatelliteImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessLinkSatelliteImplementationList.Add(new BusinessLinkSatelliteImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffColumnName = record.Values.TryGetValue("HashDiffColumnName", out var hashDiffColumnNameValue) ? hashDiffColumnNameValue ?? string.Empty : string.Empty,
                        HashDiffDataTypeId = record.Values.TryGetValue("HashDiffDataTypeId", out var hashDiffDataTypeIdValue) ? hashDiffDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffLength = record.Values.TryGetValue("HashDiffLength", out var hashDiffLengthValue) ? hashDiffLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessPointInTimeImplementationList = new List<BusinessPointInTimeImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessPointInTimeImplementation", out var businessPointInTimeImplementationListRecords))
            {
                foreach (var record in businessPointInTimeImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessPointInTimeImplementationList.Add(new BusinessPointInTimeImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        SatelliteReferenceColumnNamePattern = record.Values.TryGetValue("SatelliteReferenceColumnNamePattern", out var satelliteReferenceColumnNamePatternValue) ? satelliteReferenceColumnNamePatternValue ?? string.Empty : string.Empty,
                        SatelliteReferenceDataTypeId = record.Values.TryGetValue("SatelliteReferenceDataTypeId", out var satelliteReferenceDataTypeIdValue) ? satelliteReferenceDataTypeIdValue ?? string.Empty : string.Empty,
                        SatelliteReferencePrecision = record.Values.TryGetValue("SatelliteReferencePrecision", out var satelliteReferencePrecisionValue) ? satelliteReferencePrecisionValue ?? string.Empty : string.Empty,
                        SnapshotTimestampColumnName = record.Values.TryGetValue("SnapshotTimestampColumnName", out var snapshotTimestampColumnNameValue) ? snapshotTimestampColumnNameValue ?? string.Empty : string.Empty,
                        SnapshotTimestampDataTypeId = record.Values.TryGetValue("SnapshotTimestampDataTypeId", out var snapshotTimestampDataTypeIdValue) ? snapshotTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        SnapshotTimestampPrecision = record.Values.TryGetValue("SnapshotTimestampPrecision", out var snapshotTimestampPrecisionValue) ? snapshotTimestampPrecisionValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessSameAsLinkImplementationList = new List<BusinessSameAsLinkImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessSameAsLinkImplementation", out var businessSameAsLinkImplementationListRecords))
            {
                foreach (var record in businessSameAsLinkImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessSameAsLinkImplementationList.Add(new BusinessSameAsLinkImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        EquivalentHashKeyColumnName = record.Values.TryGetValue("EquivalentHashKeyColumnName", out var equivalentHashKeyColumnNameValue) ? equivalentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyColumnName = record.Values.TryGetValue("HashKeyColumnName", out var hashKeyColumnNameValue) ? hashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyDataTypeId = record.Values.TryGetValue("HashKeyDataTypeId", out var hashKeyDataTypeIdValue) ? hashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyLength = record.Values.TryGetValue("HashKeyLength", out var hashKeyLengthValue) ? hashKeyLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        PrimaryHashKeyColumnName = record.Values.TryGetValue("PrimaryHashKeyColumnName", out var primaryHashKeyColumnNameValue) ? primaryHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessSameAsLinkSatelliteImplementationList = new List<BusinessSameAsLinkSatelliteImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("BusinessSameAsLinkSatelliteImplementation", out var businessSameAsLinkSatelliteImplementationListRecords))
            {
                foreach (var record in businessSameAsLinkSatelliteImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    businessSameAsLinkSatelliteImplementationList.Add(new BusinessSameAsLinkSatelliteImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffColumnName = record.Values.TryGetValue("HashDiffColumnName", out var hashDiffColumnNameValue) ? hashDiffColumnNameValue ?? string.Empty : string.Empty,
                        HashDiffDataTypeId = record.Values.TryGetValue("HashDiffDataTypeId", out var hashDiffDataTypeIdValue) ? hashDiffDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffLength = record.Values.TryGetValue("HashDiffLength", out var hashDiffLengthValue) ? hashDiffLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawHubImplementationList = new List<RawHubImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawHubImplementation", out var rawHubImplementationListRecords))
            {
                foreach (var record in rawHubImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawHubImplementationList.Add(new RawHubImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyColumnName = record.Values.TryGetValue("HashKeyColumnName", out var hashKeyColumnNameValue) ? hashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyDataTypeId = record.Values.TryGetValue("HashKeyDataTypeId", out var hashKeyDataTypeIdValue) ? hashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyLength = record.Values.TryGetValue("HashKeyLength", out var hashKeyLengthValue) ? hashKeyLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawHubSatelliteImplementationList = new List<RawHubSatelliteImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawHubSatelliteImplementation", out var rawHubSatelliteImplementationListRecords))
            {
                foreach (var record in rawHubSatelliteImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawHubSatelliteImplementationList.Add(new RawHubSatelliteImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffColumnName = record.Values.TryGetValue("HashDiffColumnName", out var hashDiffColumnNameValue) ? hashDiffColumnNameValue ?? string.Empty : string.Empty,
                        HashDiffDataTypeId = record.Values.TryGetValue("HashDiffDataTypeId", out var hashDiffDataTypeIdValue) ? hashDiffDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffLength = record.Values.TryGetValue("HashDiffLength", out var hashDiffLengthValue) ? hashDiffLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawLinkImplementationList = new List<RawLinkImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawLinkImplementation", out var rawLinkImplementationListRecords))
            {
                foreach (var record in rawLinkImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawLinkImplementationList.Add(new RawLinkImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        EndHashKeyColumnPattern = record.Values.TryGetValue("EndHashKeyColumnPattern", out var endHashKeyColumnPatternValue) ? endHashKeyColumnPatternValue ?? string.Empty : string.Empty,
                        HashKeyColumnName = record.Values.TryGetValue("HashKeyColumnName", out var hashKeyColumnNameValue) ? hashKeyColumnNameValue ?? string.Empty : string.Empty,
                        HashKeyDataTypeId = record.Values.TryGetValue("HashKeyDataTypeId", out var hashKeyDataTypeIdValue) ? hashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        HashKeyLength = record.Values.TryGetValue("HashKeyLength", out var hashKeyLengthValue) ? hashKeyLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawLinkSatelliteImplementationList = new List<RawLinkSatelliteImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawLinkSatelliteImplementation", out var rawLinkSatelliteImplementationListRecords))
            {
                foreach (var record in rawLinkSatelliteImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawLinkSatelliteImplementationList.Add(new RawLinkSatelliteImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        AuditIdColumnName = record.Values.TryGetValue("AuditIdColumnName", out var auditIdColumnNameValue) ? auditIdColumnNameValue ?? string.Empty : string.Empty,
                        AuditIdDataTypeId = record.Values.TryGetValue("AuditIdDataTypeId", out var auditIdDataTypeIdValue) ? auditIdDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffColumnName = record.Values.TryGetValue("HashDiffColumnName", out var hashDiffColumnNameValue) ? hashDiffColumnNameValue ?? string.Empty : string.Empty,
                        HashDiffDataTypeId = record.Values.TryGetValue("HashDiffDataTypeId", out var hashDiffDataTypeIdValue) ? hashDiffDataTypeIdValue ?? string.Empty : string.Empty,
                        HashDiffLength = record.Values.TryGetValue("HashDiffLength", out var hashDiffLengthValue) ? hashDiffLengthValue ?? string.Empty : string.Empty,
                        LoadTimestampColumnName = record.Values.TryGetValue("LoadTimestampColumnName", out var loadTimestampColumnNameValue) ? loadTimestampColumnNameValue ?? string.Empty : string.Empty,
                        LoadTimestampDataTypeId = record.Values.TryGetValue("LoadTimestampDataTypeId", out var loadTimestampDataTypeIdValue) ? loadTimestampDataTypeIdValue ?? string.Empty : string.Empty,
                        LoadTimestampPrecision = record.Values.TryGetValue("LoadTimestampPrecision", out var loadTimestampPrecisionValue) ? loadTimestampPrecisionValue ?? string.Empty : string.Empty,
                        ParentHashKeyColumnName = record.Values.TryGetValue("ParentHashKeyColumnName", out var parentHashKeyColumnNameValue) ? parentHashKeyColumnNameValue ?? string.Empty : string.Empty,
                        ParentHashKeyDataTypeId = record.Values.TryGetValue("ParentHashKeyDataTypeId", out var parentHashKeyDataTypeIdValue) ? parentHashKeyDataTypeIdValue ?? string.Empty : string.Empty,
                        ParentHashKeyLength = record.Values.TryGetValue("ParentHashKeyLength", out var parentHashKeyLengthValue) ? parentHashKeyLengthValue ?? string.Empty : string.Empty,
                        RecordSourceColumnName = record.Values.TryGetValue("RecordSourceColumnName", out var recordSourceColumnNameValue) ? recordSourceColumnNameValue ?? string.Empty : string.Empty,
                        RecordSourceDataTypeId = record.Values.TryGetValue("RecordSourceDataTypeId", out var recordSourceDataTypeIdValue) ? recordSourceDataTypeIdValue ?? string.Empty : string.Empty,
                        RecordSourceLength = record.Values.TryGetValue("RecordSourceLength", out var recordSourceLengthValue) ? recordSourceLengthValue ?? string.Empty : string.Empty,
                        TableNamePattern = record.Values.TryGetValue("TableNamePattern", out var tableNamePatternValue) ? tableNamePatternValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var businessBridgeImplementationListById = new Dictionary<string, BusinessBridgeImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessBridgeImplementationList)
            {
                businessBridgeImplementationListById[row.Id] = row;
            }

            var businessHierarchicalLinkImplementationListById = new Dictionary<string, BusinessHierarchicalLinkImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHierarchicalLinkImplementationList)
            {
                businessHierarchicalLinkImplementationListById[row.Id] = row;
            }

            var businessHierarchicalLinkSatelliteImplementationListById = new Dictionary<string, BusinessHierarchicalLinkSatelliteImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHierarchicalLinkSatelliteImplementationList)
            {
                businessHierarchicalLinkSatelliteImplementationListById[row.Id] = row;
            }

            var businessHubImplementationListById = new Dictionary<string, BusinessHubImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHubImplementationList)
            {
                businessHubImplementationListById[row.Id] = row;
            }

            var businessHubSatelliteImplementationListById = new Dictionary<string, BusinessHubSatelliteImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessHubSatelliteImplementationList)
            {
                businessHubSatelliteImplementationListById[row.Id] = row;
            }

            var businessLinkImplementationListById = new Dictionary<string, BusinessLinkImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessLinkImplementationList)
            {
                businessLinkImplementationListById[row.Id] = row;
            }

            var businessLinkSatelliteImplementationListById = new Dictionary<string, BusinessLinkSatelliteImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessLinkSatelliteImplementationList)
            {
                businessLinkSatelliteImplementationListById[row.Id] = row;
            }

            var businessPointInTimeImplementationListById = new Dictionary<string, BusinessPointInTimeImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessPointInTimeImplementationList)
            {
                businessPointInTimeImplementationListById[row.Id] = row;
            }

            var businessSameAsLinkImplementationListById = new Dictionary<string, BusinessSameAsLinkImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessSameAsLinkImplementationList)
            {
                businessSameAsLinkImplementationListById[row.Id] = row;
            }

            var businessSameAsLinkSatelliteImplementationListById = new Dictionary<string, BusinessSameAsLinkSatelliteImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in businessSameAsLinkSatelliteImplementationList)
            {
                businessSameAsLinkSatelliteImplementationListById[row.Id] = row;
            }

            var rawHubImplementationListById = new Dictionary<string, RawHubImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in rawHubImplementationList)
            {
                rawHubImplementationListById[row.Id] = row;
            }

            var rawHubSatelliteImplementationListById = new Dictionary<string, RawHubSatelliteImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in rawHubSatelliteImplementationList)
            {
                rawHubSatelliteImplementationListById[row.Id] = row;
            }

            var rawLinkImplementationListById = new Dictionary<string, RawLinkImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in rawLinkImplementationList)
            {
                rawLinkImplementationListById[row.Id] = row;
            }

            var rawLinkSatelliteImplementationListById = new Dictionary<string, RawLinkSatelliteImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in rawLinkSatelliteImplementationList)
            {
                rawLinkSatelliteImplementationListById[row.Id] = row;
            }

            return new MetaDataVaultImplementationModel(
                new ReadOnlyCollection<BusinessBridgeImplementation>(businessBridgeImplementationList),
                new ReadOnlyCollection<BusinessHierarchicalLinkImplementation>(businessHierarchicalLinkImplementationList),
                new ReadOnlyCollection<BusinessHierarchicalLinkSatelliteImplementation>(businessHierarchicalLinkSatelliteImplementationList),
                new ReadOnlyCollection<BusinessHubImplementation>(businessHubImplementationList),
                new ReadOnlyCollection<BusinessHubSatelliteImplementation>(businessHubSatelliteImplementationList),
                new ReadOnlyCollection<BusinessLinkImplementation>(businessLinkImplementationList),
                new ReadOnlyCollection<BusinessLinkSatelliteImplementation>(businessLinkSatelliteImplementationList),
                new ReadOnlyCollection<BusinessPointInTimeImplementation>(businessPointInTimeImplementationList),
                new ReadOnlyCollection<BusinessSameAsLinkImplementation>(businessSameAsLinkImplementationList),
                new ReadOnlyCollection<BusinessSameAsLinkSatelliteImplementation>(businessSameAsLinkSatelliteImplementationList),
                new ReadOnlyCollection<RawHubImplementation>(rawHubImplementationList),
                new ReadOnlyCollection<RawHubSatelliteImplementation>(rawHubSatelliteImplementationList),
                new ReadOnlyCollection<RawLinkImplementation>(rawLinkImplementationList),
                new ReadOnlyCollection<RawLinkSatelliteImplementation>(rawLinkSatelliteImplementationList)
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
