using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meta.Core.Domain;

namespace MetaSchema
{
    public sealed partial class MetaSchemaModel
    {
        internal MetaSchemaModel(
            List<Field> fieldList,
            List<FieldDataTypeDetail> fieldDataTypeDetailList,
            List<Schema> schemaList,
            List<System> systemList,
            List<Table> tableList,
            List<TableKey> tableKeyList,
            List<TableKeyField> tableKeyFieldList,
            List<TableRelationship> tableRelationshipList,
            List<TableRelationshipField> tableRelationshipFieldList
        )
        {
            FieldList = fieldList;
            FieldDataTypeDetailList = fieldDataTypeDetailList;
            SchemaList = schemaList;
            SystemList = systemList;
            TableList = tableList;
            TableKeyList = tableKeyList;
            TableKeyFieldList = tableKeyFieldList;
            TableRelationshipList = tableRelationshipList;
            TableRelationshipFieldList = tableRelationshipFieldList;
        }

        public static MetaSchemaModel CreateEmpty()
        {
            return new MetaSchemaModel(
                new List<Field>(),
                new List<FieldDataTypeDetail>(),
                new List<Schema>(),
                new List<System>(),
                new List<Table>(),
                new List<TableKey>(),
                new List<TableKeyField>(),
                new List<TableRelationship>(),
                new List<TableRelationshipField>()
            );
        }

        public List<Field> FieldList { get; }
        public List<FieldDataTypeDetail> FieldDataTypeDetailList { get; }
        public List<Schema> SchemaList { get; }
        public List<System> SystemList { get; }
        public List<Table> TableList { get; }
        public List<TableKey> TableKeyList { get; }
        public List<TableKeyField> TableKeyFieldList { get; }
        public List<TableRelationship> TableRelationshipList { get; }
        public List<TableRelationshipField> TableRelationshipFieldList { get; }

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

            foreach (var row in FieldList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "Field.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.IsNullable))
                {
                    record.Values["IsNullable"] = row.IsNullable;
                }
                if (!string.IsNullOrWhiteSpace(row.MetaDataTypeId))
                {
                    record.Values["MetaDataTypeId"] = row.MetaDataTypeId;
                }
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.Ordinal))
                {
                    record.Values["Ordinal"] = row.Ordinal;
                }
                if (!string.IsNullOrWhiteSpace(row.TableId))
                {
                    record.RelationshipIds["TableId"] = row.TableId;
                }
                workspace.Instance.GetOrCreateEntityRecords("Field").Add(record);
            }

            foreach (var row in FieldDataTypeDetailList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "FieldDataTypeDetail.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.Value))
                {
                    record.Values["Value"] = row.Value;
                }
                if (!string.IsNullOrWhiteSpace(row.FieldId))
                {
                    record.RelationshipIds["FieldId"] = row.FieldId;
                }
                workspace.Instance.GetOrCreateEntityRecords("FieldDataTypeDetail").Add(record);
            }

            foreach (var row in SchemaList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "Schema.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.SystemId))
                {
                    record.RelationshipIds["SystemId"] = row.SystemId;
                }
                workspace.Instance.GetOrCreateEntityRecords("Schema").Add(record);
            }

            foreach (var row in SystemList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "System.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Description))
                {
                    record.Values["Description"] = row.Description;
                }
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                workspace.Instance.GetOrCreateEntityRecords("System").Add(record);
            }

            foreach (var row in TableList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "Table.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.ObjectType))
                {
                    record.Values["ObjectType"] = row.ObjectType;
                }
                if (!string.IsNullOrWhiteSpace(row.SchemaId))
                {
                    record.RelationshipIds["SchemaId"] = row.SchemaId;
                }
                workspace.Instance.GetOrCreateEntityRecords("Table").Add(record);
            }

            foreach (var row in TableKeyList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "TableKey.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.KeyType))
                {
                    record.Values["KeyType"] = row.KeyType;
                }
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.TableId))
                {
                    record.RelationshipIds["TableId"] = row.TableId;
                }
                workspace.Instance.GetOrCreateEntityRecords("TableKey").Add(record);
            }

            foreach (var row in TableKeyFieldList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "TableKeyField.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.FieldName))
                {
                    record.Values["FieldName"] = row.FieldName;
                }
                if (!string.IsNullOrWhiteSpace(row.Ordinal))
                {
                    record.Values["Ordinal"] = row.Ordinal;
                }
                if (!string.IsNullOrWhiteSpace(row.FieldId))
                {
                    record.RelationshipIds["FieldId"] = row.FieldId;
                }
                if (!string.IsNullOrWhiteSpace(row.TableKeyId))
                {
                    record.RelationshipIds["TableKeyId"] = row.TableKeyId;
                }
                workspace.Instance.GetOrCreateEntityRecords("TableKeyField").Add(record);
            }

            foreach (var row in TableRelationshipList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "TableRelationship.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceTableId))
                {
                    record.RelationshipIds["SourceTableId"] = row.SourceTableId;
                }
                if (!string.IsNullOrWhiteSpace(row.TargetTableId))
                {
                    record.RelationshipIds["TargetTableId"] = row.TargetTableId;
                }
                workspace.Instance.GetOrCreateEntityRecords("TableRelationship").Add(record);
            }

            foreach (var row in TableRelationshipFieldList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "TableRelationshipField.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Ordinal))
                {
                    record.Values["Ordinal"] = row.Ordinal;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceFieldId))
                {
                    record.RelationshipIds["SourceFieldId"] = row.SourceFieldId;
                }
                if (!string.IsNullOrWhiteSpace(row.TableRelationshipId))
                {
                    record.RelationshipIds["TableRelationshipId"] = row.TableRelationshipId;
                }
                if (!string.IsNullOrWhiteSpace(row.TargetFieldId))
                {
                    record.RelationshipIds["TargetFieldId"] = row.TargetFieldId;
                }
                workspace.Instance.GetOrCreateEntityRecords("TableRelationshipField").Add(record);
            }

            return workspace;
        }

        public Task SaveToXmlWorkspaceAsync(
            string workspacePath,
            CancellationToken cancellationToken = default)
        {
            var workspace = ToXmlWorkspace(workspacePath);
            return MetaSchemaTooling.SaveWorkspaceAsync(workspace, cancellationToken);
        }

        private static GenericModel CreateGenericModelDefinition()
        {
            var model = new GenericModel
            {
                Name = "MetaSchema",
            };

            model.Entities.Add(new GenericEntity
            {
                Name = "Field",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "IsNullable",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "MetaDataTypeId",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "Ordinal",
                        DataType = "string",
                        IsNullable = true,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "Table",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "FieldDataTypeDetail",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "Value",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "Field",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "Schema",
                Properties =
                {
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
                        Entity = "System",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "System",
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
                Name = "Table",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "ObjectType",
                        DataType = "string",
                        IsNullable = true,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "Schema",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "TableKey",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "KeyType",
                        DataType = "string",
                        IsNullable = false,
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
                        Entity = "Table",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "TableKeyField",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "FieldName",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "Ordinal",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "Field",
                        Role = "",
                    },
                    new GenericRelationship
                    {
                        Entity = "TableKey",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "TableRelationship",
                Properties =
                {
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
                        Entity = "Table",
                        Role = "SourceTable",
                    },
                    new GenericRelationship
                    {
                        Entity = "Table",
                        Role = "TargetTable",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "TableRelationshipField",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Ordinal",
                        DataType = "string",
                        IsNullable = false,
                    },
                },
                Relationships =
                {
                    new GenericRelationship
                    {
                        Entity = "Field",
                        Role = "SourceField",
                    },
                    new GenericRelationship
                    {
                        Entity = "TableRelationship",
                        Role = "",
                    },
                    new GenericRelationship
                    {
                        Entity = "Field",
                        Role = "TargetField",
                    },
                },
            });

            return model;
        }
    }

    internal static class MetaSchemaModelFactory
    {
        internal static MetaSchemaModel CreateFromWorkspace(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new global::System.ArgumentNullException(nameof(workspace));
            }

            var fieldList = new List<Field>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("Field", out var fieldListRecords))
            {
                foreach (var record in fieldListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    fieldList.Add(new Field
                    {
                        Id = record.Id ?? string.Empty,
                        IsNullable = record.Values.TryGetValue("IsNullable", out var isNullableValue) ? isNullableValue ?? string.Empty : string.Empty,
                        MetaDataTypeId = record.Values.TryGetValue("MetaDataTypeId", out var metaDataTypeIdValue) ? metaDataTypeIdValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        TableId = record.RelationshipIds.TryGetValue("TableId", out var tableRelationshipId) ? tableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var fieldDataTypeDetailList = new List<FieldDataTypeDetail>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("FieldDataTypeDetail", out var fieldDataTypeDetailListRecords))
            {
                foreach (var record in fieldDataTypeDetailListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    fieldDataTypeDetailList.Add(new FieldDataTypeDetail
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Value = record.Values.TryGetValue("Value", out var valueValue) ? valueValue ?? string.Empty : string.Empty,
                        FieldId = record.RelationshipIds.TryGetValue("FieldId", out var fieldRelationshipId) ? fieldRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var schemaList = new List<Schema>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("Schema", out var schemaListRecords))
            {
                foreach (var record in schemaListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    schemaList.Add(new Schema
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SystemId = record.RelationshipIds.TryGetValue("SystemId", out var systemRelationshipId) ? systemRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var systemList = new List<System>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("System", out var systemListRecords))
            {
                foreach (var record in systemListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    systemList.Add(new System
                    {
                        Id = record.Id ?? string.Empty,
                        Description = record.Values.TryGetValue("Description", out var descriptionValue) ? descriptionValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var tableList = new List<Table>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("Table", out var tableListRecords))
            {
                foreach (var record in tableListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    tableList.Add(new Table
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        ObjectType = record.Values.TryGetValue("ObjectType", out var objectTypeValue) ? objectTypeValue ?? string.Empty : string.Empty,
                        SchemaId = record.RelationshipIds.TryGetValue("SchemaId", out var schemaRelationshipId) ? schemaRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var tableKeyList = new List<TableKey>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("TableKey", out var tableKeyListRecords))
            {
                foreach (var record in tableKeyListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    tableKeyList.Add(new TableKey
                    {
                        Id = record.Id ?? string.Empty,
                        KeyType = record.Values.TryGetValue("KeyType", out var keyTypeValue) ? keyTypeValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        TableId = record.RelationshipIds.TryGetValue("TableId", out var tableRelationshipId) ? tableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var tableKeyFieldList = new List<TableKeyField>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("TableKeyField", out var tableKeyFieldListRecords))
            {
                foreach (var record in tableKeyFieldListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    tableKeyFieldList.Add(new TableKeyField
                    {
                        Id = record.Id ?? string.Empty,
                        FieldName = record.Values.TryGetValue("FieldName", out var fieldNameValue) ? fieldNameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        FieldId = record.RelationshipIds.TryGetValue("FieldId", out var fieldRelationshipId) ? fieldRelationshipId ?? string.Empty : string.Empty,
                        TableKeyId = record.RelationshipIds.TryGetValue("TableKeyId", out var tableKeyRelationshipId) ? tableKeyRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var tableRelationshipList = new List<TableRelationship>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("TableRelationship", out var tableRelationshipListRecords))
            {
                foreach (var record in tableRelationshipListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    tableRelationshipList.Add(new TableRelationship
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SourceTableId = record.RelationshipIds.TryGetValue("SourceTableId", out var sourceTableRelationshipId) ? sourceTableRelationshipId ?? string.Empty : string.Empty,
                        TargetTableId = record.RelationshipIds.TryGetValue("TargetTableId", out var targetTableRelationshipId) ? targetTableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var tableRelationshipFieldList = new List<TableRelationshipField>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("TableRelationshipField", out var tableRelationshipFieldListRecords))
            {
                foreach (var record in tableRelationshipFieldListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    tableRelationshipFieldList.Add(new TableRelationshipField
                    {
                        Id = record.Id ?? string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        SourceFieldId = record.RelationshipIds.TryGetValue("SourceFieldId", out var sourceFieldRelationshipId) ? sourceFieldRelationshipId ?? string.Empty : string.Empty,
                        TableRelationshipId = record.RelationshipIds.TryGetValue("TableRelationshipId", out var tableRelationshipRelationshipId) ? tableRelationshipRelationshipId ?? string.Empty : string.Empty,
                        TargetFieldId = record.RelationshipIds.TryGetValue("TargetFieldId", out var targetFieldRelationshipId) ? targetFieldRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var fieldListById = new Dictionary<string, Field>(global::System.StringComparer.Ordinal);
            foreach (var row in fieldList)
            {
                fieldListById[row.Id] = row;
            }

            var fieldDataTypeDetailListById = new Dictionary<string, FieldDataTypeDetail>(global::System.StringComparer.Ordinal);
            foreach (var row in fieldDataTypeDetailList)
            {
                fieldDataTypeDetailListById[row.Id] = row;
            }

            var schemaListById = new Dictionary<string, Schema>(global::System.StringComparer.Ordinal);
            foreach (var row in schemaList)
            {
                schemaListById[row.Id] = row;
            }

            var systemListById = new Dictionary<string, System>(global::System.StringComparer.Ordinal);
            foreach (var row in systemList)
            {
                systemListById[row.Id] = row;
            }

            var tableListById = new Dictionary<string, Table>(global::System.StringComparer.Ordinal);
            foreach (var row in tableList)
            {
                tableListById[row.Id] = row;
            }

            var tableKeyListById = new Dictionary<string, TableKey>(global::System.StringComparer.Ordinal);
            foreach (var row in tableKeyList)
            {
                tableKeyListById[row.Id] = row;
            }

            var tableKeyFieldListById = new Dictionary<string, TableKeyField>(global::System.StringComparer.Ordinal);
            foreach (var row in tableKeyFieldList)
            {
                tableKeyFieldListById[row.Id] = row;
            }

            var tableRelationshipListById = new Dictionary<string, TableRelationship>(global::System.StringComparer.Ordinal);
            foreach (var row in tableRelationshipList)
            {
                tableRelationshipListById[row.Id] = row;
            }

            var tableRelationshipFieldListById = new Dictionary<string, TableRelationshipField>(global::System.StringComparer.Ordinal);
            foreach (var row in tableRelationshipFieldList)
            {
                tableRelationshipFieldListById[row.Id] = row;
            }

            foreach (var row in fieldList)
            {
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "Field",
                    row.Id,
                    "TableId");
            }

            foreach (var row in fieldDataTypeDetailList)
            {
                row.Field = RequireTarget(
                    fieldListById,
                    row.FieldId,
                    "FieldDataTypeDetail",
                    row.Id,
                    "FieldId");
            }

            foreach (var row in schemaList)
            {
                row.System = RequireTarget(
                    systemListById,
                    row.SystemId,
                    "Schema",
                    row.Id,
                    "SystemId");
            }

            foreach (var row in tableList)
            {
                row.Schema = RequireTarget(
                    schemaListById,
                    row.SchemaId,
                    "Table",
                    row.Id,
                    "SchemaId");
            }

            foreach (var row in tableKeyList)
            {
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "TableKey",
                    row.Id,
                    "TableId");
            }

            foreach (var row in tableKeyFieldList)
            {
                row.Field = RequireTarget(
                    fieldListById,
                    row.FieldId,
                    "TableKeyField",
                    row.Id,
                    "FieldId");
            }

            foreach (var row in tableKeyFieldList)
            {
                row.TableKey = RequireTarget(
                    tableKeyListById,
                    row.TableKeyId,
                    "TableKeyField",
                    row.Id,
                    "TableKeyId");
            }

            foreach (var row in tableRelationshipList)
            {
                row.SourceTable = RequireTarget(
                    tableListById,
                    row.SourceTableId,
                    "TableRelationship",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in tableRelationshipList)
            {
                row.TargetTable = RequireTarget(
                    tableListById,
                    row.TargetTableId,
                    "TableRelationship",
                    row.Id,
                    "TargetTableId");
            }

            foreach (var row in tableRelationshipFieldList)
            {
                row.SourceField = RequireTarget(
                    fieldListById,
                    row.SourceFieldId,
                    "TableRelationshipField",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in tableRelationshipFieldList)
            {
                row.TableRelationship = RequireTarget(
                    tableRelationshipListById,
                    row.TableRelationshipId,
                    "TableRelationshipField",
                    row.Id,
                    "TableRelationshipId");
            }

            foreach (var row in tableRelationshipFieldList)
            {
                row.TargetField = RequireTarget(
                    fieldListById,
                    row.TargetFieldId,
                    "TableRelationshipField",
                    row.Id,
                    "TargetFieldId");
            }

            return new MetaSchemaModel(
                fieldList,
                fieldDataTypeDetailList,
                schemaList,
                systemList,
                tableList,
                tableKeyList,
                tableKeyFieldList,
                tableRelationshipList,
                tableRelationshipFieldList
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
