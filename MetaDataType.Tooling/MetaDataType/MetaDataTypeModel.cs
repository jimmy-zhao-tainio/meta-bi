using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meta.Core.Domain;

namespace MetaDataType
{
    public sealed partial class MetaDataTypeModel
    {
        internal MetaDataTypeModel(
            List<DataType> dataTypeList,
            List<DataTypeSystem> dataTypeSystemList
        )
        {
            DataTypeList = dataTypeList;
            DataTypeSystemList = dataTypeSystemList;
        }

        public static MetaDataTypeModel CreateEmpty()
        {
            return new MetaDataTypeModel(
                new List<DataType>(),
                new List<DataTypeSystem>()
            );
        }

        public List<DataType> DataTypeList { get; }
        public List<DataTypeSystem> DataTypeSystemList { get; }

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

            foreach (var row in DataTypeList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "DataType.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Category))
                {
                    record.Values["Category"] = row.Category;
                }
                if (!string.IsNullOrWhiteSpace(row.Description))
                {
                    record.Values["Description"] = row.Description;
                }
                if (!string.IsNullOrWhiteSpace(row.IsCanonical))
                {
                    record.Values["IsCanonical"] = row.IsCanonical;
                }
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.DataTypeSystemId))
                {
                    record.RelationshipIds["DataTypeSystemId"] = row.DataTypeSystemId;
                }
                workspace.Instance.GetOrCreateEntityRecords("DataType").Add(record);
            }

            foreach (var row in DataTypeSystemList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "DataTypeSystem.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Description))
                {
                    record.Values["Description"] = row.Description;
                }
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                workspace.Instance.GetOrCreateEntityRecords("DataTypeSystem").Add(record);
            }

            return workspace;
        }

        public Task SaveToXmlWorkspaceAsync(
            string workspacePath,
            CancellationToken cancellationToken = default)
        {
            var workspace = ToXmlWorkspace(workspacePath);
            return MetaDataTypeTooling.SaveWorkspaceAsync(workspace, cancellationToken);
        }

        private static GenericModel CreateGenericModelDefinition()
        {
            var model = new GenericModel
            {
                Name = "MetaDataType",
            };

            model.Entities.Add(new GenericEntity
            {
                Name = "DataType",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Category",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "Description",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "IsCanonical",
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
                    new GenericRelationship
                    {
                        Entity = "DataTypeSystem",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "DataTypeSystem",
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

            return model;
        }
    }

    internal static class MetaDataTypeModelFactory
    {
        internal static MetaDataTypeModel CreateFromWorkspace(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new global::System.ArgumentNullException(nameof(workspace));
            }

            var dataTypeList = new List<DataType>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("DataType", out var dataTypeListRecords))
            {
                foreach (var record in dataTypeListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    dataTypeList.Add(new DataType
                    {
                        Id = record.Id ?? string.Empty,
                        Category = record.Values.TryGetValue("Category", out var categoryValue) ? categoryValue ?? string.Empty : string.Empty,
                        Description = record.Values.TryGetValue("Description", out var descriptionValue) ? descriptionValue ?? string.Empty : string.Empty,
                        IsCanonical = record.Values.TryGetValue("IsCanonical", out var isCanonicalValue) ? isCanonicalValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        DataTypeSystemId = record.RelationshipIds.TryGetValue("DataTypeSystemId", out var dataTypeSystemRelationshipId) ? dataTypeSystemRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var dataTypeSystemList = new List<DataTypeSystem>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("DataTypeSystem", out var dataTypeSystemListRecords))
            {
                foreach (var record in dataTypeSystemListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    dataTypeSystemList.Add(new DataTypeSystem
                    {
                        Id = record.Id ?? string.Empty,
                        Description = record.Values.TryGetValue("Description", out var descriptionValue) ? descriptionValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var dataTypeListById = new Dictionary<string, DataType>(global::System.StringComparer.Ordinal);
            foreach (var row in dataTypeList)
            {
                dataTypeListById[row.Id] = row;
            }

            var dataTypeSystemListById = new Dictionary<string, DataTypeSystem>(global::System.StringComparer.Ordinal);
            foreach (var row in dataTypeSystemList)
            {
                dataTypeSystemListById[row.Id] = row;
            }

            foreach (var row in dataTypeList)
            {
                row.DataTypeSystem = RequireTarget(
                    dataTypeSystemListById,
                    row.DataTypeSystemId,
                    "DataType",
                    row.Id,
                    "DataTypeSystemId");
            }

            return new MetaDataTypeModel(
                dataTypeList,
                dataTypeSystemList
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
