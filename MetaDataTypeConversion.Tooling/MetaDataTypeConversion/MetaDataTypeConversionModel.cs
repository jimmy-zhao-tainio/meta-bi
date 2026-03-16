using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meta.Core.Domain;

namespace MetaDataTypeConversion
{
    public sealed partial class MetaDataTypeConversionModel
    {
        internal MetaDataTypeConversionModel(
            List<ConversionImplementation> conversionImplementationList,
            List<DataTypeMapping> dataTypeMappingList
        )
        {
            ConversionImplementationList = conversionImplementationList;
            DataTypeMappingList = dataTypeMappingList;
        }

        public static MetaDataTypeConversionModel CreateEmpty()
        {
            return new MetaDataTypeConversionModel(
                new List<ConversionImplementation>(),
                new List<DataTypeMapping>()
            );
        }

        public List<ConversionImplementation> ConversionImplementationList { get; }
        public List<DataTypeMapping> DataTypeMappingList { get; }

        public Workspace ToXmlWorkspace(string workspacePath)
        {
            if (string.IsNullOrWhiteSpace(workspacePath))
            {
                throw new global::System.ArgumentException("Workspace path is required.", nameof(workspacePath));
            }

            var rootPath = global::System.IO.Path.GetFullPath(workspacePath);
            var metadataRootPath = global::System.IO.Path.Combine(rootPath, "metadata");
            var model = CreateGenericModelDefinition();
            var workspace = new Workspace
            {
                WorkspaceRootPath = rootPath,
                MetadataRootPath = metadataRootPath,
                WorkspaceConfig = global::Meta.Core.WorkspaceConfig.Generated.MetaWorkspace.CreateDefault(),
                Model = model,
                Instance = new GenericInstance
                {
                    ModelName = model.Name,
                },
                IsDirty = true,
            };

            foreach (var row in ConversionImplementationList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "ConversionImplementation.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Description))
                {
                    record.Values["Description"] = row.Description;
                }
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                workspace.Instance.GetOrCreateEntityRecords("ConversionImplementation").Add(record);
            }

            foreach (var row in DataTypeMappingList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "DataTypeMapping.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Notes))
                {
                    record.Values["Notes"] = row.Notes;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceDataTypeId))
                {
                    record.Values["SourceDataTypeId"] = row.SourceDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.TargetDataTypeId))
                {
                    record.Values["TargetDataTypeId"] = row.TargetDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.ConversionImplementationId))
                {
                    record.RelationshipIds["ConversionImplementationId"] = row.ConversionImplementationId;
                }
                workspace.Instance.GetOrCreateEntityRecords("DataTypeMapping").Add(record);
            }

            return workspace;
        }

        public Task SaveToXmlWorkspaceAsync(
            string workspacePath,
            CancellationToken cancellationToken = default)
        {
            var workspace = ToXmlWorkspace(workspacePath);
            return MetaDataTypeConversionTooling.SaveWorkspaceAsync(workspace, cancellationToken);
        }

        private static GenericModel CreateGenericModelDefinition()
        {
            var model = new GenericModel
            {
                Name = "MetaDataTypeConversion",
            };

            model.Entities.Add(new GenericEntity
            {
                Name = "ConversionImplementation",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Description",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "DataTypeMapping",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Notes",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "SourceDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "TargetDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "ConversionImplementation",
                        Role = "",
                    },
                },
            });

            return model;
        }
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
                conversionImplementationList,
                dataTypeMappingList
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
