using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Meta.Core.Domain;

namespace MetaDataTypeConversion
{
    public sealed partial class MetaDataTypeConversionModel
    {
        internal MetaDataTypeConversionModel(
            IReadOnlyList<ConversionImplementation> conversionImplementationList,
            IReadOnlyList<DataTypeMapping> dataTypeMappingList
        )
        {
            ConversionImplementationList = conversionImplementationList;
            DataTypeMappingList = dataTypeMappingList;
        }

        public IReadOnlyList<ConversionImplementation> ConversionImplementationList { get; }
        public IReadOnlyList<DataTypeMapping> DataTypeMappingList { get; }
    }

    internal static class MetaDataTypeConversionModelFactory
    {
        internal static MetaDataTypeConversionModel CreateFromWorkspace(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new global::System.ArgumentNullException(nameof(workspace));
            }

            var conversionImplementationList = new List<ConversionImplementation>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("ConversionImplementation", out var conversionImplementationListRecords))
            {
                foreach (var record in conversionImplementationListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    conversionImplementationList.Add(new ConversionImplementation
                    {
                        Id = record.Id ?? string.Empty,
                        Description = record.Values.TryGetValue("Description", out var descriptionValue) ? descriptionValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var dataTypeMappingList = new List<DataTypeMapping>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("DataTypeMapping", out var dataTypeMappingListRecords))
            {
                foreach (var record in dataTypeMappingListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    dataTypeMappingList.Add(new DataTypeMapping
                    {
                        Id = record.Id ?? string.Empty,
                        Notes = record.Values.TryGetValue("Notes", out var notesValue) ? notesValue ?? string.Empty : string.Empty,
                        SourceDataTypeId = record.Values.TryGetValue("SourceDataTypeId", out var sourceDataTypeIdValue) ? sourceDataTypeIdValue ?? string.Empty : string.Empty,
                        TargetDataTypeId = record.Values.TryGetValue("TargetDataTypeId", out var targetDataTypeIdValue) ? targetDataTypeIdValue ?? string.Empty : string.Empty,
                        ConversionImplementationId = record.RelationshipIds.TryGetValue("ConversionImplementationId", out var conversionImplementationRelationshipId) ? conversionImplementationRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var conversionImplementationListById = new Dictionary<string, ConversionImplementation>(global::System.StringComparer.Ordinal);
            foreach (var row in conversionImplementationList)
            {
                conversionImplementationListById[row.Id] = row;
            }

            var dataTypeMappingListById = new Dictionary<string, DataTypeMapping>(global::System.StringComparer.Ordinal);
            foreach (var row in dataTypeMappingList)
            {
                dataTypeMappingListById[row.Id] = row;
            }

            foreach (var row in dataTypeMappingList)
            {
                row.ConversionImplementation = RequireTarget(
                    conversionImplementationListById,
                    row.ConversionImplementationId,
                    "DataTypeMapping",
                    row.Id,
                    "ConversionImplementationId");
            }

            return new MetaDataTypeConversionModel(
                new ReadOnlyCollection<ConversionImplementation>(conversionImplementationList),
                new ReadOnlyCollection<DataTypeMapping>(dataTypeMappingList)
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
