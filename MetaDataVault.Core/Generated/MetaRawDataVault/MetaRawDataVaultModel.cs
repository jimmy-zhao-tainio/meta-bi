using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Meta.Core.Domain;

namespace MetaRawDataVault
{
    public sealed partial class MetaRawDataVaultModel
    {
        internal MetaRawDataVaultModel(
            IReadOnlyList<RawHub> rawHubList,
            IReadOnlyList<RawHubKeyPart> rawHubKeyPartList,
            IReadOnlyList<RawHubSatellite> rawHubSatelliteList,
            IReadOnlyList<RawHubSatelliteAttribute> rawHubSatelliteAttributeList,
            IReadOnlyList<RawLink> rawLinkList,
            IReadOnlyList<RawLinkHub> rawLinkHubList,
            IReadOnlyList<RawLinkSatellite> rawLinkSatelliteList,
            IReadOnlyList<RawLinkSatelliteAttribute> rawLinkSatelliteAttributeList,
            IReadOnlyList<SourceField> sourceFieldList,
            IReadOnlyList<SourceFieldDataTypeDetail> sourceFieldDataTypeDetailList,
            IReadOnlyList<SourceSchema> sourceSchemaList,
            IReadOnlyList<SourceSystem> sourceSystemList,
            IReadOnlyList<SourceTable> sourceTableList,
            IReadOnlyList<SourceTableRelationship> sourceTableRelationshipList,
            IReadOnlyList<SourceTableRelationshipField> sourceTableRelationshipFieldList
        )
        {
            RawHubList = rawHubList;
            RawHubKeyPartList = rawHubKeyPartList;
            RawHubSatelliteList = rawHubSatelliteList;
            RawHubSatelliteAttributeList = rawHubSatelliteAttributeList;
            RawLinkList = rawLinkList;
            RawLinkHubList = rawLinkHubList;
            RawLinkSatelliteList = rawLinkSatelliteList;
            RawLinkSatelliteAttributeList = rawLinkSatelliteAttributeList;
            SourceFieldList = sourceFieldList;
            SourceFieldDataTypeDetailList = sourceFieldDataTypeDetailList;
            SourceSchemaList = sourceSchemaList;
            SourceSystemList = sourceSystemList;
            SourceTableList = sourceTableList;
            SourceTableRelationshipList = sourceTableRelationshipList;
            SourceTableRelationshipFieldList = sourceTableRelationshipFieldList;
        }

        public IReadOnlyList<RawHub> RawHubList { get; }
        public IReadOnlyList<RawHubKeyPart> RawHubKeyPartList { get; }
        public IReadOnlyList<RawHubSatellite> RawHubSatelliteList { get; }
        public IReadOnlyList<RawHubSatelliteAttribute> RawHubSatelliteAttributeList { get; }
        public IReadOnlyList<RawLink> RawLinkList { get; }
        public IReadOnlyList<RawLinkHub> RawLinkHubList { get; }
        public IReadOnlyList<RawLinkSatellite> RawLinkSatelliteList { get; }
        public IReadOnlyList<RawLinkSatelliteAttribute> RawLinkSatelliteAttributeList { get; }
        public IReadOnlyList<SourceField> SourceFieldList { get; }
        public IReadOnlyList<SourceFieldDataTypeDetail> SourceFieldDataTypeDetailList { get; }
        public IReadOnlyList<SourceSchema> SourceSchemaList { get; }
        public IReadOnlyList<SourceSystem> SourceSystemList { get; }
        public IReadOnlyList<SourceTable> SourceTableList { get; }
        public IReadOnlyList<SourceTableRelationship> SourceTableRelationshipList { get; }
        public IReadOnlyList<SourceTableRelationshipField> SourceTableRelationshipFieldList { get; }
    }

    internal static class MetaRawDataVaultModelFactory
    {
        internal static MetaRawDataVaultModel CreateFromWorkspace(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new global::System.ArgumentNullException(nameof(workspace));
            }

            var rawHubList = new List<RawHub>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawHub", out var rawHubListRecords))
            {
                foreach (var record in rawHubListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawHubList.Add(new RawHub
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SourceTableId = record.RelationshipIds.TryGetValue("SourceTableId", out var sourceTableRelationshipId) ? sourceTableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawHubKeyPartList = new List<RawHubKeyPart>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawHubKeyPart", out var rawHubKeyPartListRecords))
            {
                foreach (var record in rawHubKeyPartListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawHubKeyPartList.Add(new RawHubKeyPart
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        RawHubId = record.RelationshipIds.TryGetValue("RawHubId", out var rawHubRelationshipId) ? rawHubRelationshipId ?? string.Empty : string.Empty,
                        SourceFieldId = record.RelationshipIds.TryGetValue("SourceFieldId", out var sourceFieldRelationshipId) ? sourceFieldRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawHubSatelliteList = new List<RawHubSatellite>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawHubSatellite", out var rawHubSatelliteListRecords))
            {
                foreach (var record in rawHubSatelliteListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawHubSatelliteList.Add(new RawHubSatellite
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SatelliteKind = record.Values.TryGetValue("SatelliteKind", out var satelliteKindValue) ? satelliteKindValue ?? string.Empty : string.Empty,
                        RawHubId = record.RelationshipIds.TryGetValue("RawHubId", out var rawHubRelationshipId) ? rawHubRelationshipId ?? string.Empty : string.Empty,
                        SourceTableId = record.RelationshipIds.TryGetValue("SourceTableId", out var sourceTableRelationshipId) ? sourceTableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawHubSatelliteAttributeList = new List<RawHubSatelliteAttribute>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawHubSatelliteAttribute", out var rawHubSatelliteAttributeListRecords))
            {
                foreach (var record in rawHubSatelliteAttributeListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawHubSatelliteAttributeList.Add(new RawHubSatelliteAttribute
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        RawHubSatelliteId = record.RelationshipIds.TryGetValue("RawHubSatelliteId", out var rawHubSatelliteRelationshipId) ? rawHubSatelliteRelationshipId ?? string.Empty : string.Empty,
                        SourceFieldId = record.RelationshipIds.TryGetValue("SourceFieldId", out var sourceFieldRelationshipId) ? sourceFieldRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawLinkList = new List<RawLink>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawLink", out var rawLinkListRecords))
            {
                foreach (var record in rawLinkListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawLinkList.Add(new RawLink
                    {
                        Id = record.Id ?? string.Empty,
                        LinkKind = record.Values.TryGetValue("LinkKind", out var linkKindValue) ? linkKindValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SourceTableRelationshipId = record.RelationshipIds.TryGetValue("SourceTableRelationshipId", out var sourceTableRelationshipRelationshipId) ? sourceTableRelationshipRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawLinkHubList = new List<RawLinkHub>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawLinkHub", out var rawLinkHubListRecords))
            {
                foreach (var record in rawLinkHubListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawLinkHubList.Add(new RawLinkHub
                    {
                        Id = record.Id ?? string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        RoleName = record.Values.TryGetValue("RoleName", out var roleNameValue) ? roleNameValue ?? string.Empty : string.Empty,
                        RawHubId = record.RelationshipIds.TryGetValue("RawHubId", out var rawHubRelationshipId) ? rawHubRelationshipId ?? string.Empty : string.Empty,
                        RawLinkId = record.RelationshipIds.TryGetValue("RawLinkId", out var rawLinkRelationshipId) ? rawLinkRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawLinkSatelliteList = new List<RawLinkSatellite>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawLinkSatellite", out var rawLinkSatelliteListRecords))
            {
                foreach (var record in rawLinkSatelliteListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawLinkSatelliteList.Add(new RawLinkSatellite
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SatelliteKind = record.Values.TryGetValue("SatelliteKind", out var satelliteKindValue) ? satelliteKindValue ?? string.Empty : string.Empty,
                        RawLinkId = record.RelationshipIds.TryGetValue("RawLinkId", out var rawLinkRelationshipId) ? rawLinkRelationshipId ?? string.Empty : string.Empty,
                        SourceTableId = record.RelationshipIds.TryGetValue("SourceTableId", out var sourceTableRelationshipId) ? sourceTableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawLinkSatelliteAttributeList = new List<RawLinkSatelliteAttribute>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("RawLinkSatelliteAttribute", out var rawLinkSatelliteAttributeListRecords))
            {
                foreach (var record in rawLinkSatelliteAttributeListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    rawLinkSatelliteAttributeList.Add(new RawLinkSatelliteAttribute
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        RawLinkSatelliteId = record.RelationshipIds.TryGetValue("RawLinkSatelliteId", out var rawLinkSatelliteRelationshipId) ? rawLinkSatelliteRelationshipId ?? string.Empty : string.Empty,
                        SourceFieldId = record.RelationshipIds.TryGetValue("SourceFieldId", out var sourceFieldRelationshipId) ? sourceFieldRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var sourceFieldList = new List<SourceField>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("SourceField", out var sourceFieldListRecords))
            {
                foreach (var record in sourceFieldListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sourceFieldList.Add(new SourceField
                    {
                        Id = record.Id ?? string.Empty,
                        DataTypeId = record.Values.TryGetValue("DataTypeId", out var dataTypeIdValue) ? dataTypeIdValue ?? string.Empty : string.Empty,
                        IsNullable = record.Values.TryGetValue("IsNullable", out var isNullableValue) ? isNullableValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        SourceTableId = record.RelationshipIds.TryGetValue("SourceTableId", out var sourceTableRelationshipId) ? sourceTableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var sourceFieldDataTypeDetailList = new List<SourceFieldDataTypeDetail>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("SourceFieldDataTypeDetail", out var sourceFieldDataTypeDetailListRecords))
            {
                foreach (var record in sourceFieldDataTypeDetailListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sourceFieldDataTypeDetailList.Add(new SourceFieldDataTypeDetail
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Value = record.Values.TryGetValue("Value", out var valueValue) ? valueValue ?? string.Empty : string.Empty,
                        SourceFieldId = record.RelationshipIds.TryGetValue("SourceFieldId", out var sourceFieldRelationshipId) ? sourceFieldRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var sourceSchemaList = new List<SourceSchema>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("SourceSchema", out var sourceSchemaListRecords))
            {
                foreach (var record in sourceSchemaListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sourceSchemaList.Add(new SourceSchema
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SourceSystemId = record.RelationshipIds.TryGetValue("SourceSystemId", out var sourceSystemRelationshipId) ? sourceSystemRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var sourceSystemList = new List<SourceSystem>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("SourceSystem", out var sourceSystemListRecords))
            {
                foreach (var record in sourceSystemListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sourceSystemList.Add(new SourceSystem
                    {
                        Id = record.Id ?? string.Empty,
                        Description = record.Values.TryGetValue("Description", out var descriptionValue) ? descriptionValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var sourceTableList = new List<SourceTable>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("SourceTable", out var sourceTableListRecords))
            {
                foreach (var record in sourceTableListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sourceTableList.Add(new SourceTable
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SourceSchemaId = record.RelationshipIds.TryGetValue("SourceSchemaId", out var sourceSchemaRelationshipId) ? sourceSchemaRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var sourceTableRelationshipList = new List<SourceTableRelationship>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("SourceTableRelationship", out var sourceTableRelationshipListRecords))
            {
                foreach (var record in sourceTableRelationshipListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sourceTableRelationshipList.Add(new SourceTableRelationship
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        TargetSchemaName = record.Values.TryGetValue("TargetSchemaName", out var targetSchemaNameValue) ? targetSchemaNameValue ?? string.Empty : string.Empty,
                        TargetTableName = record.Values.TryGetValue("TargetTableName", out var targetTableNameValue) ? targetTableNameValue ?? string.Empty : string.Empty,
                        SourceTableId = record.RelationshipIds.TryGetValue("SourceTableId", out var sourceTableRelationshipId) ? sourceTableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var sourceTableRelationshipFieldList = new List<SourceTableRelationshipField>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("SourceTableRelationshipField", out var sourceTableRelationshipFieldListRecords))
            {
                foreach (var record in sourceTableRelationshipFieldListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sourceTableRelationshipFieldList.Add(new SourceTableRelationshipField
                    {
                        Id = record.Id ?? string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        SourceFieldName = record.Values.TryGetValue("SourceFieldName", out var sourceFieldNameValue) ? sourceFieldNameValue ?? string.Empty : string.Empty,
                        TargetFieldName = record.Values.TryGetValue("TargetFieldName", out var targetFieldNameValue) ? targetFieldNameValue ?? string.Empty : string.Empty,
                        SourceFieldId = record.RelationshipIds.TryGetValue("SourceFieldId", out var sourceFieldRelationshipId) ? sourceFieldRelationshipId ?? string.Empty : string.Empty,
                        SourceTableRelationshipId = record.RelationshipIds.TryGetValue("SourceTableRelationshipId", out var sourceTableRelationshipRelationshipId) ? sourceTableRelationshipRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var rawHubListById = new Dictionary<string, RawHub>(global::System.StringComparer.Ordinal);
            foreach (var row in rawHubList)
            {
                rawHubListById[row.Id] = row;
            }

            var rawHubKeyPartListById = new Dictionary<string, RawHubKeyPart>(global::System.StringComparer.Ordinal);
            foreach (var row in rawHubKeyPartList)
            {
                rawHubKeyPartListById[row.Id] = row;
            }

            var rawHubSatelliteListById = new Dictionary<string, RawHubSatellite>(global::System.StringComparer.Ordinal);
            foreach (var row in rawHubSatelliteList)
            {
                rawHubSatelliteListById[row.Id] = row;
            }

            var rawHubSatelliteAttributeListById = new Dictionary<string, RawHubSatelliteAttribute>(global::System.StringComparer.Ordinal);
            foreach (var row in rawHubSatelliteAttributeList)
            {
                rawHubSatelliteAttributeListById[row.Id] = row;
            }

            var rawLinkListById = new Dictionary<string, RawLink>(global::System.StringComparer.Ordinal);
            foreach (var row in rawLinkList)
            {
                rawLinkListById[row.Id] = row;
            }

            var rawLinkHubListById = new Dictionary<string, RawLinkHub>(global::System.StringComparer.Ordinal);
            foreach (var row in rawLinkHubList)
            {
                rawLinkHubListById[row.Id] = row;
            }

            var rawLinkSatelliteListById = new Dictionary<string, RawLinkSatellite>(global::System.StringComparer.Ordinal);
            foreach (var row in rawLinkSatelliteList)
            {
                rawLinkSatelliteListById[row.Id] = row;
            }

            var rawLinkSatelliteAttributeListById = new Dictionary<string, RawLinkSatelliteAttribute>(global::System.StringComparer.Ordinal);
            foreach (var row in rawLinkSatelliteAttributeList)
            {
                rawLinkSatelliteAttributeListById[row.Id] = row;
            }

            var sourceFieldListById = new Dictionary<string, SourceField>(global::System.StringComparer.Ordinal);
            foreach (var row in sourceFieldList)
            {
                sourceFieldListById[row.Id] = row;
            }

            var sourceFieldDataTypeDetailListById = new Dictionary<string, SourceFieldDataTypeDetail>(global::System.StringComparer.Ordinal);
            foreach (var row in sourceFieldDataTypeDetailList)
            {
                sourceFieldDataTypeDetailListById[row.Id] = row;
            }

            var sourceSchemaListById = new Dictionary<string, SourceSchema>(global::System.StringComparer.Ordinal);
            foreach (var row in sourceSchemaList)
            {
                sourceSchemaListById[row.Id] = row;
            }

            var sourceSystemListById = new Dictionary<string, SourceSystem>(global::System.StringComparer.Ordinal);
            foreach (var row in sourceSystemList)
            {
                sourceSystemListById[row.Id] = row;
            }

            var sourceTableListById = new Dictionary<string, SourceTable>(global::System.StringComparer.Ordinal);
            foreach (var row in sourceTableList)
            {
                sourceTableListById[row.Id] = row;
            }

            var sourceTableRelationshipListById = new Dictionary<string, SourceTableRelationship>(global::System.StringComparer.Ordinal);
            foreach (var row in sourceTableRelationshipList)
            {
                sourceTableRelationshipListById[row.Id] = row;
            }

            var sourceTableRelationshipFieldListById = new Dictionary<string, SourceTableRelationshipField>(global::System.StringComparer.Ordinal);
            foreach (var row in sourceTableRelationshipFieldList)
            {
                sourceTableRelationshipFieldListById[row.Id] = row;
            }

            foreach (var row in rawHubList)
            {
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "RawHub",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in rawHubKeyPartList)
            {
                row.RawHub = RequireTarget(
                    rawHubListById,
                    row.RawHubId,
                    "RawHubKeyPart",
                    row.Id,
                    "RawHubId");
            }

            foreach (var row in rawHubKeyPartList)
            {
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "RawHubKeyPart",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in rawHubSatelliteList)
            {
                row.RawHub = RequireTarget(
                    rawHubListById,
                    row.RawHubId,
                    "RawHubSatellite",
                    row.Id,
                    "RawHubId");
            }

            foreach (var row in rawHubSatelliteList)
            {
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "RawHubSatellite",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in rawHubSatelliteAttributeList)
            {
                row.RawHubSatellite = RequireTarget(
                    rawHubSatelliteListById,
                    row.RawHubSatelliteId,
                    "RawHubSatelliteAttribute",
                    row.Id,
                    "RawHubSatelliteId");
            }

            foreach (var row in rawHubSatelliteAttributeList)
            {
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "RawHubSatelliteAttribute",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in rawLinkList)
            {
                row.SourceTableRelationship = RequireTarget(
                    sourceTableRelationshipListById,
                    row.SourceTableRelationshipId,
                    "RawLink",
                    row.Id,
                    "SourceTableRelationshipId");
            }

            foreach (var row in rawLinkHubList)
            {
                row.RawHub = RequireTarget(
                    rawHubListById,
                    row.RawHubId,
                    "RawLinkHub",
                    row.Id,
                    "RawHubId");
            }

            foreach (var row in rawLinkHubList)
            {
                row.RawLink = RequireTarget(
                    rawLinkListById,
                    row.RawLinkId,
                    "RawLinkHub",
                    row.Id,
                    "RawLinkId");
            }

            foreach (var row in rawLinkSatelliteList)
            {
                row.RawLink = RequireTarget(
                    rawLinkListById,
                    row.RawLinkId,
                    "RawLinkSatellite",
                    row.Id,
                    "RawLinkId");
            }

            foreach (var row in rawLinkSatelliteList)
            {
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "RawLinkSatellite",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in rawLinkSatelliteAttributeList)
            {
                row.RawLinkSatellite = RequireTarget(
                    rawLinkSatelliteListById,
                    row.RawLinkSatelliteId,
                    "RawLinkSatelliteAttribute",
                    row.Id,
                    "RawLinkSatelliteId");
            }

            foreach (var row in rawLinkSatelliteAttributeList)
            {
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "RawLinkSatelliteAttribute",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in sourceFieldList)
            {
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "SourceField",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in sourceFieldDataTypeDetailList)
            {
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "SourceFieldDataTypeDetail",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in sourceSchemaList)
            {
                row.SourceSystem = RequireTarget(
                    sourceSystemListById,
                    row.SourceSystemId,
                    "SourceSchema",
                    row.Id,
                    "SourceSystemId");
            }

            foreach (var row in sourceTableList)
            {
                row.SourceSchema = RequireTarget(
                    sourceSchemaListById,
                    row.SourceSchemaId,
                    "SourceTable",
                    row.Id,
                    "SourceSchemaId");
            }

            foreach (var row in sourceTableRelationshipList)
            {
                row.SourceTable = RequireTarget(
                    sourceTableListById,
                    row.SourceTableId,
                    "SourceTableRelationship",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in sourceTableRelationshipFieldList)
            {
                row.SourceField = RequireTarget(
                    sourceFieldListById,
                    row.SourceFieldId,
                    "SourceTableRelationshipField",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in sourceTableRelationshipFieldList)
            {
                row.SourceTableRelationship = RequireTarget(
                    sourceTableRelationshipListById,
                    row.SourceTableRelationshipId,
                    "SourceTableRelationshipField",
                    row.Id,
                    "SourceTableRelationshipId");
            }

            return new MetaRawDataVaultModel(
                new ReadOnlyCollection<RawHub>(rawHubList),
                new ReadOnlyCollection<RawHubKeyPart>(rawHubKeyPartList),
                new ReadOnlyCollection<RawHubSatellite>(rawHubSatelliteList),
                new ReadOnlyCollection<RawHubSatelliteAttribute>(rawHubSatelliteAttributeList),
                new ReadOnlyCollection<RawLink>(rawLinkList),
                new ReadOnlyCollection<RawLinkHub>(rawLinkHubList),
                new ReadOnlyCollection<RawLinkSatellite>(rawLinkSatelliteList),
                new ReadOnlyCollection<RawLinkSatelliteAttribute>(rawLinkSatelliteAttributeList),
                new ReadOnlyCollection<SourceField>(sourceFieldList),
                new ReadOnlyCollection<SourceFieldDataTypeDetail>(sourceFieldDataTypeDetailList),
                new ReadOnlyCollection<SourceSchema>(sourceSchemaList),
                new ReadOnlyCollection<SourceSystem>(sourceSystemList),
                new ReadOnlyCollection<SourceTable>(sourceTableList),
                new ReadOnlyCollection<SourceTableRelationship>(sourceTableRelationshipList),
                new ReadOnlyCollection<SourceTableRelationshipField>(sourceTableRelationshipFieldList)
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
