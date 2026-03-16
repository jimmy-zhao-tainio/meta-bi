using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meta.Core.Domain;

namespace SqlModel
{
    public sealed partial class SqlModelModel
    {
        internal SqlModelModel(
            List<Database> databaseList,
            List<ForeignKey> foreignKeyList,
            List<ForeignKeyColumn> foreignKeyColumnList,
            List<Index> indexList,
            List<IndexColumn> indexColumnList,
            List<PrimaryKey> primaryKeyList,
            List<PrimaryKeyColumn> primaryKeyColumnList,
            List<Schema> schemaList,
            List<Table> tableList,
            List<TableColumn> tableColumnList,
            List<TableColumnDataTypeDetail> tableColumnDataTypeDetailList
        )
        {
            DatabaseList = databaseList;
            ForeignKeyList = foreignKeyList;
            ForeignKeyColumnList = foreignKeyColumnList;
            IndexList = indexList;
            IndexColumnList = indexColumnList;
            PrimaryKeyList = primaryKeyList;
            PrimaryKeyColumnList = primaryKeyColumnList;
            SchemaList = schemaList;
            TableList = tableList;
            TableColumnList = tableColumnList;
            TableColumnDataTypeDetailList = tableColumnDataTypeDetailList;
        }

        public static SqlModelModel CreateEmpty()
        {
            return new SqlModelModel(
                new List<Database>(),
                new List<ForeignKey>(),
                new List<ForeignKeyColumn>(),
                new List<Index>(),
                new List<IndexColumn>(),
                new List<PrimaryKey>(),
                new List<PrimaryKeyColumn>(),
                new List<Schema>(),
                new List<Table>(),
                new List<TableColumn>(),
                new List<TableColumnDataTypeDetail>()
            );
        }

        public List<Database> DatabaseList { get; }
        public List<ForeignKey> ForeignKeyList { get; }
        public List<ForeignKeyColumn> ForeignKeyColumnList { get; }
        public List<Index> IndexList { get; }
        public List<IndexColumn> IndexColumnList { get; }
        public List<PrimaryKey> PrimaryKeyList { get; }
        public List<PrimaryKeyColumn> PrimaryKeyColumnList { get; }
        public List<Schema> SchemaList { get; }
        public List<Table> TableList { get; }
        public List<TableColumn> TableColumnList { get; }
        public List<TableColumnDataTypeDetail> TableColumnDataTypeDetailList { get; }

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

            foreach (var row in DatabaseList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "Database.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Collation))
                {
                    record.Values["Collation"] = row.Collation;
                }
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.Platform))
                {
                    record.Values["Platform"] = row.Platform;
                }
                workspace.Instance.GetOrCreateEntityRecords("Database").Add(record);
            }

            foreach (var row in ForeignKeyList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "ForeignKey.xml",
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
                workspace.Instance.GetOrCreateEntityRecords("ForeignKey").Add(record);
            }

            foreach (var row in ForeignKeyColumnList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "ForeignKeyColumn.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Ordinal))
                {
                    record.Values["Ordinal"] = row.Ordinal;
                }
                if (!string.IsNullOrWhiteSpace(row.ForeignKeyId))
                {
                    record.RelationshipIds["ForeignKeyId"] = row.ForeignKeyId;
                }
                if (!string.IsNullOrWhiteSpace(row.SourceColumnId))
                {
                    record.RelationshipIds["SourceColumnId"] = row.SourceColumnId;
                }
                if (!string.IsNullOrWhiteSpace(row.TargetColumnId))
                {
                    record.RelationshipIds["TargetColumnId"] = row.TargetColumnId;
                }
                workspace.Instance.GetOrCreateEntityRecords("ForeignKeyColumn").Add(record);
            }

            foreach (var row in IndexList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "Index.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.FilterSql))
                {
                    record.Values["FilterSql"] = row.FilterSql;
                }
                if (!string.IsNullOrWhiteSpace(row.IsClustered))
                {
                    record.Values["IsClustered"] = row.IsClustered;
                }
                if (!string.IsNullOrWhiteSpace(row.IsUnique))
                {
                    record.Values["IsUnique"] = row.IsUnique;
                }
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.TableId))
                {
                    record.RelationshipIds["TableId"] = row.TableId;
                }
                workspace.Instance.GetOrCreateEntityRecords("Index").Add(record);
            }

            foreach (var row in IndexColumnList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "IndexColumn.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.IsDescending))
                {
                    record.Values["IsDescending"] = row.IsDescending;
                }
                if (!string.IsNullOrWhiteSpace(row.IsIncluded))
                {
                    record.Values["IsIncluded"] = row.IsIncluded;
                }
                if (!string.IsNullOrWhiteSpace(row.Ordinal))
                {
                    record.Values["Ordinal"] = row.Ordinal;
                }
                if (!string.IsNullOrWhiteSpace(row.IndexId))
                {
                    record.RelationshipIds["IndexId"] = row.IndexId;
                }
                if (!string.IsNullOrWhiteSpace(row.TableColumnId))
                {
                    record.RelationshipIds["TableColumnId"] = row.TableColumnId;
                }
                workspace.Instance.GetOrCreateEntityRecords("IndexColumn").Add(record);
            }

            foreach (var row in PrimaryKeyList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "PrimaryKey.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.IsClustered))
                {
                    record.Values["IsClustered"] = row.IsClustered;
                }
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.TableId))
                {
                    record.RelationshipIds["TableId"] = row.TableId;
                }
                workspace.Instance.GetOrCreateEntityRecords("PrimaryKey").Add(record);
            }

            foreach (var row in PrimaryKeyColumnList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "PrimaryKeyColumn.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.IsDescending))
                {
                    record.Values["IsDescending"] = row.IsDescending;
                }
                if (!string.IsNullOrWhiteSpace(row.Ordinal))
                {
                    record.Values["Ordinal"] = row.Ordinal;
                }
                if (!string.IsNullOrWhiteSpace(row.PrimaryKeyId))
                {
                    record.RelationshipIds["PrimaryKeyId"] = row.PrimaryKeyId;
                }
                if (!string.IsNullOrWhiteSpace(row.TableColumnId))
                {
                    record.RelationshipIds["TableColumnId"] = row.TableColumnId;
                }
                workspace.Instance.GetOrCreateEntityRecords("PrimaryKeyColumn").Add(record);
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
                if (!string.IsNullOrWhiteSpace(row.DatabaseId))
                {
                    record.RelationshipIds["DatabaseId"] = row.DatabaseId;
                }
                workspace.Instance.GetOrCreateEntityRecords("Schema").Add(record);
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
                if (!string.IsNullOrWhiteSpace(row.SchemaId))
                {
                    record.RelationshipIds["SchemaId"] = row.SchemaId;
                }
                workspace.Instance.GetOrCreateEntityRecords("Table").Add(record);
            }

            foreach (var row in TableColumnList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "TableColumn.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.ExpressionSql))
                {
                    record.Values["ExpressionSql"] = row.ExpressionSql;
                }
                if (!string.IsNullOrWhiteSpace(row.IdentityIncrement))
                {
                    record.Values["IdentityIncrement"] = row.IdentityIncrement;
                }
                if (!string.IsNullOrWhiteSpace(row.IdentitySeed))
                {
                    record.Values["IdentitySeed"] = row.IdentitySeed;
                }
                if (!string.IsNullOrWhiteSpace(row.IsIdentity))
                {
                    record.Values["IsIdentity"] = row.IsIdentity;
                }
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
                workspace.Instance.GetOrCreateEntityRecords("TableColumn").Add(record);
            }

            foreach (var row in TableColumnDataTypeDetailList.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
            {
                var record = new GenericRecord
                {
                    Id = row.Id ?? string.Empty,
                    SourceShardFileName = "TableColumnDataTypeDetail.xml",
                };
                if (!string.IsNullOrWhiteSpace(row.Name))
                {
                    record.Values["Name"] = row.Name;
                }
                if (!string.IsNullOrWhiteSpace(row.Value))
                {
                    record.Values["Value"] = row.Value;
                }
                if (!string.IsNullOrWhiteSpace(row.TableColumnId))
                {
                    record.RelationshipIds["TableColumnId"] = row.TableColumnId;
                }
                workspace.Instance.GetOrCreateEntityRecords("TableColumnDataTypeDetail").Add(record);
            }

            return workspace;
        }

        public Task SaveToXmlWorkspaceAsync(
            string workspacePath,
            CancellationToken cancellationToken = default)
        {
            var workspace = ToXmlWorkspace(workspacePath);
            return SqlModelTooling.SaveWorkspaceAsync(workspace, cancellationToken);
        }

        private static GenericModel CreateGenericModelDefinition()
        {
            var model = new GenericModel
            {
                Name = "SqlModel",
            };

            model.Entities.Add(new GenericEntity
            {
                Name = "Database",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "Collation",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "Name",
                        DataType = "string",
                        IsNullable = false,
                    },
                    new GenericProperty
                    {
                        Name = "Platform",
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
                Name = "ForeignKey",
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
                Name = "ForeignKeyColumn",
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
                        Entity = "ForeignKey",
                        Role = "",
                    },
                    new GenericRelationship
                    {
                        Entity = "TableColumn",
                        Role = "SourceColumn",
                    },
                    new GenericRelationship
                    {
                        Entity = "TableColumn",
                        Role = "TargetColumn",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "Index",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "FilterSql",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "IsClustered",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "IsUnique",
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
                        Entity = "Table",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "IndexColumn",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "IsDescending",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "IsIncluded",
                        DataType = "string",
                        IsNullable = true,
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
                        Entity = "Index",
                        Role = "",
                    },
                    new GenericRelationship
                    {
                        Entity = "TableColumn",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "PrimaryKey",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "IsClustered",
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
                        Entity = "Table",
                        Role = "",
                    },
                },
            });

            model.Entities.Add(new GenericEntity
            {
                Name = "PrimaryKeyColumn",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "IsDescending",
                        DataType = "string",
                        IsNullable = true,
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
                        Entity = "PrimaryKey",
                        Role = "",
                    },
                    new GenericRelationship
                    {
                        Entity = "TableColumn",
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
                        Entity = "Database",
                        Role = "",
                    },
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
                Name = "TableColumn",
                Properties =
                {
                    new GenericProperty
                    {
                        Name = "ExpressionSql",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "IdentityIncrement",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "IdentitySeed",
                        DataType = "string",
                        IsNullable = true,
                    },
                    new GenericProperty
                    {
                        Name = "IsIdentity",
                        DataType = "string",
                        IsNullable = true,
                    },
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
                Name = "TableColumnDataTypeDetail",
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
                        Entity = "TableColumn",
                        Role = "",
                    },
                },
            });

            return model;
        }
    }

    internal static class SqlModelModelFactory
    {
        internal static SqlModelModel CreateFromWorkspace(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new global::System.ArgumentNullException(nameof(workspace));
            }

            var databaseList = new List<Database>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("Database", out var databaseListRecords))
            {
                foreach (var record in databaseListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    databaseList.Add(new Database
                    {
                        Id = record.Id ?? string.Empty,
                        Collation = record.Values.TryGetValue("Collation", out var collationValue) ? collationValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Platform = record.Values.TryGetValue("Platform", out var platformValue) ? platformValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var foreignKeyList = new List<ForeignKey>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("ForeignKey", out var foreignKeyListRecords))
            {
                foreach (var record in foreignKeyListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    foreignKeyList.Add(new ForeignKey
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SourceTableId = record.RelationshipIds.TryGetValue("SourceTableId", out var sourceTableRelationshipId) ? sourceTableRelationshipId ?? string.Empty : string.Empty,
                        TargetTableId = record.RelationshipIds.TryGetValue("TargetTableId", out var targetTableRelationshipId) ? targetTableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var foreignKeyColumnList = new List<ForeignKeyColumn>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("ForeignKeyColumn", out var foreignKeyColumnListRecords))
            {
                foreach (var record in foreignKeyColumnListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    foreignKeyColumnList.Add(new ForeignKeyColumn
                    {
                        Id = record.Id ?? string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        ForeignKeyId = record.RelationshipIds.TryGetValue("ForeignKeyId", out var foreignKeyRelationshipId) ? foreignKeyRelationshipId ?? string.Empty : string.Empty,
                        SourceColumnId = record.RelationshipIds.TryGetValue("SourceColumnId", out var sourceColumnRelationshipId) ? sourceColumnRelationshipId ?? string.Empty : string.Empty,
                        TargetColumnId = record.RelationshipIds.TryGetValue("TargetColumnId", out var targetColumnRelationshipId) ? targetColumnRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var indexList = new List<Index>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("Index", out var indexListRecords))
            {
                foreach (var record in indexListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    indexList.Add(new Index
                    {
                        Id = record.Id ?? string.Empty,
                        FilterSql = record.Values.TryGetValue("FilterSql", out var filterSqlValue) ? filterSqlValue ?? string.Empty : string.Empty,
                        IsClustered = record.Values.TryGetValue("IsClustered", out var isClusteredValue) ? isClusteredValue ?? string.Empty : string.Empty,
                        IsUnique = record.Values.TryGetValue("IsUnique", out var isUniqueValue) ? isUniqueValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        TableId = record.RelationshipIds.TryGetValue("TableId", out var tableRelationshipId) ? tableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var indexColumnList = new List<IndexColumn>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("IndexColumn", out var indexColumnListRecords))
            {
                foreach (var record in indexColumnListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    indexColumnList.Add(new IndexColumn
                    {
                        Id = record.Id ?? string.Empty,
                        IsDescending = record.Values.TryGetValue("IsDescending", out var isDescendingValue) ? isDescendingValue ?? string.Empty : string.Empty,
                        IsIncluded = record.Values.TryGetValue("IsIncluded", out var isIncludedValue) ? isIncludedValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        IndexId = record.RelationshipIds.TryGetValue("IndexId", out var indexRelationshipId) ? indexRelationshipId ?? string.Empty : string.Empty,
                        TableColumnId = record.RelationshipIds.TryGetValue("TableColumnId", out var tableColumnRelationshipId) ? tableColumnRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var primaryKeyList = new List<PrimaryKey>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("PrimaryKey", out var primaryKeyListRecords))
            {
                foreach (var record in primaryKeyListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    primaryKeyList.Add(new PrimaryKey
                    {
                        Id = record.Id ?? string.Empty,
                        IsClustered = record.Values.TryGetValue("IsClustered", out var isClusteredValue) ? isClusteredValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        TableId = record.RelationshipIds.TryGetValue("TableId", out var tableRelationshipId) ? tableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var primaryKeyColumnList = new List<PrimaryKeyColumn>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("PrimaryKeyColumn", out var primaryKeyColumnListRecords))
            {
                foreach (var record in primaryKeyColumnListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    primaryKeyColumnList.Add(new PrimaryKeyColumn
                    {
                        Id = record.Id ?? string.Empty,
                        IsDescending = record.Values.TryGetValue("IsDescending", out var isDescendingValue) ? isDescendingValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        PrimaryKeyId = record.RelationshipIds.TryGetValue("PrimaryKeyId", out var primaryKeyRelationshipId) ? primaryKeyRelationshipId ?? string.Empty : string.Empty,
                        TableColumnId = record.RelationshipIds.TryGetValue("TableColumnId", out var tableColumnRelationshipId) ? tableColumnRelationshipId ?? string.Empty : string.Empty,
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
                        DatabaseId = record.RelationshipIds.TryGetValue("DatabaseId", out var databaseRelationshipId) ? databaseRelationshipId ?? string.Empty : string.Empty,
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
                        SchemaId = record.RelationshipIds.TryGetValue("SchemaId", out var schemaRelationshipId) ? schemaRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var tableColumnList = new List<TableColumn>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("TableColumn", out var tableColumnListRecords))
            {
                foreach (var record in tableColumnListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    tableColumnList.Add(new TableColumn
                    {
                        Id = record.Id ?? string.Empty,
                        ExpressionSql = record.Values.TryGetValue("ExpressionSql", out var expressionSqlValue) ? expressionSqlValue ?? string.Empty : string.Empty,
                        IdentityIncrement = record.Values.TryGetValue("IdentityIncrement", out var identityIncrementValue) ? identityIncrementValue ?? string.Empty : string.Empty,
                        IdentitySeed = record.Values.TryGetValue("IdentitySeed", out var identitySeedValue) ? identitySeedValue ?? string.Empty : string.Empty,
                        IsIdentity = record.Values.TryGetValue("IsIdentity", out var isIdentityValue) ? isIdentityValue ?? string.Empty : string.Empty,
                        IsNullable = record.Values.TryGetValue("IsNullable", out var isNullableValue) ? isNullableValue ?? string.Empty : string.Empty,
                        MetaDataTypeId = record.Values.TryGetValue("MetaDataTypeId", out var metaDataTypeIdValue) ? metaDataTypeIdValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        TableId = record.RelationshipIds.TryGetValue("TableId", out var tableRelationshipId) ? tableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var tableColumnDataTypeDetailList = new List<TableColumnDataTypeDetail>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("TableColumnDataTypeDetail", out var tableColumnDataTypeDetailListRecords))
            {
                foreach (var record in tableColumnDataTypeDetailListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    tableColumnDataTypeDetailList.Add(new TableColumnDataTypeDetail
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Value = record.Values.TryGetValue("Value", out var valueValue) ? valueValue ?? string.Empty : string.Empty,
                        TableColumnId = record.RelationshipIds.TryGetValue("TableColumnId", out var tableColumnRelationshipId) ? tableColumnRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var databaseListById = new Dictionary<string, Database>(global::System.StringComparer.Ordinal);
            foreach (var row in databaseList)
            {
                databaseListById[row.Id] = row;
            }

            var foreignKeyListById = new Dictionary<string, ForeignKey>(global::System.StringComparer.Ordinal);
            foreach (var row in foreignKeyList)
            {
                foreignKeyListById[row.Id] = row;
            }

            var foreignKeyColumnListById = new Dictionary<string, ForeignKeyColumn>(global::System.StringComparer.Ordinal);
            foreach (var row in foreignKeyColumnList)
            {
                foreignKeyColumnListById[row.Id] = row;
            }

            var indexListById = new Dictionary<string, Index>(global::System.StringComparer.Ordinal);
            foreach (var row in indexList)
            {
                indexListById[row.Id] = row;
            }

            var indexColumnListById = new Dictionary<string, IndexColumn>(global::System.StringComparer.Ordinal);
            foreach (var row in indexColumnList)
            {
                indexColumnListById[row.Id] = row;
            }

            var primaryKeyListById = new Dictionary<string, PrimaryKey>(global::System.StringComparer.Ordinal);
            foreach (var row in primaryKeyList)
            {
                primaryKeyListById[row.Id] = row;
            }

            var primaryKeyColumnListById = new Dictionary<string, PrimaryKeyColumn>(global::System.StringComparer.Ordinal);
            foreach (var row in primaryKeyColumnList)
            {
                primaryKeyColumnListById[row.Id] = row;
            }

            var schemaListById = new Dictionary<string, Schema>(global::System.StringComparer.Ordinal);
            foreach (var row in schemaList)
            {
                schemaListById[row.Id] = row;
            }

            var tableListById = new Dictionary<string, Table>(global::System.StringComparer.Ordinal);
            foreach (var row in tableList)
            {
                tableListById[row.Id] = row;
            }

            var tableColumnListById = new Dictionary<string, TableColumn>(global::System.StringComparer.Ordinal);
            foreach (var row in tableColumnList)
            {
                tableColumnListById[row.Id] = row;
            }

            var tableColumnDataTypeDetailListById = new Dictionary<string, TableColumnDataTypeDetail>(global::System.StringComparer.Ordinal);
            foreach (var row in tableColumnDataTypeDetailList)
            {
                tableColumnDataTypeDetailListById[row.Id] = row;
            }

            foreach (var row in foreignKeyList)
            {
                row.SourceTable = RequireTarget(
                    tableListById,
                    row.SourceTableId,
                    "ForeignKey",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in foreignKeyList)
            {
                row.TargetTable = RequireTarget(
                    tableListById,
                    row.TargetTableId,
                    "ForeignKey",
                    row.Id,
                    "TargetTableId");
            }

            foreach (var row in foreignKeyColumnList)
            {
                row.ForeignKey = RequireTarget(
                    foreignKeyListById,
                    row.ForeignKeyId,
                    "ForeignKeyColumn",
                    row.Id,
                    "ForeignKeyId");
            }

            foreach (var row in foreignKeyColumnList)
            {
                row.SourceColumn = RequireTarget(
                    tableColumnListById,
                    row.SourceColumnId,
                    "ForeignKeyColumn",
                    row.Id,
                    "SourceColumnId");
            }

            foreach (var row in foreignKeyColumnList)
            {
                row.TargetColumn = RequireTarget(
                    tableColumnListById,
                    row.TargetColumnId,
                    "ForeignKeyColumn",
                    row.Id,
                    "TargetColumnId");
            }

            foreach (var row in indexList)
            {
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "Index",
                    row.Id,
                    "TableId");
            }

            foreach (var row in indexColumnList)
            {
                row.Index = RequireTarget(
                    indexListById,
                    row.IndexId,
                    "IndexColumn",
                    row.Id,
                    "IndexId");
            }

            foreach (var row in indexColumnList)
            {
                row.TableColumn = RequireTarget(
                    tableColumnListById,
                    row.TableColumnId,
                    "IndexColumn",
                    row.Id,
                    "TableColumnId");
            }

            foreach (var row in primaryKeyList)
            {
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "PrimaryKey",
                    row.Id,
                    "TableId");
            }

            foreach (var row in primaryKeyColumnList)
            {
                row.PrimaryKey = RequireTarget(
                    primaryKeyListById,
                    row.PrimaryKeyId,
                    "PrimaryKeyColumn",
                    row.Id,
                    "PrimaryKeyId");
            }

            foreach (var row in primaryKeyColumnList)
            {
                row.TableColumn = RequireTarget(
                    tableColumnListById,
                    row.TableColumnId,
                    "PrimaryKeyColumn",
                    row.Id,
                    "TableColumnId");
            }

            foreach (var row in schemaList)
            {
                row.Database = RequireTarget(
                    databaseListById,
                    row.DatabaseId,
                    "Schema",
                    row.Id,
                    "DatabaseId");
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

            foreach (var row in tableColumnList)
            {
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "TableColumn",
                    row.Id,
                    "TableId");
            }

            foreach (var row in tableColumnDataTypeDetailList)
            {
                row.TableColumn = RequireTarget(
                    tableColumnListById,
                    row.TableColumnId,
                    "TableColumnDataTypeDetail",
                    row.Id,
                    "TableColumnId");
            }

            return new SqlModelModel(
                databaseList,
                foreignKeyList,
                foreignKeyColumnList,
                indexList,
                indexColumnList,
                primaryKeyList,
                primaryKeyColumnList,
                schemaList,
                tableList,
                tableColumnList,
                tableColumnDataTypeDetailList
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
