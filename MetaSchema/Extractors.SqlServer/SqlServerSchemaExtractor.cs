using System.Data;
using Microsoft.Data.SqlClient;
using Meta.Core.Domain;
using MetaSchema.Core;

namespace MetaSchema.Extractors.SqlServer;

public sealed class SqlServerSchemaExtractor
{
    public Workspace ExtractMetaSchemaWorkspace(SqlServerExtractRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.NewWorkspacePath))
        {
            throw new InvalidOperationException("extract sqlserver requires --new-workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(request.ConnectionString))
        {
            throw new InvalidOperationException("extract sqlserver requires --connection <connectionString>.");
        }

        if (string.IsNullOrWhiteSpace(request.SystemName))
        {
            throw new InvalidOperationException("extract sqlserver requires --system <name>.");
        }

        if (string.IsNullOrWhiteSpace(request.SchemaName) && !request.AllSchemas)
        {
            throw new InvalidOperationException("extract sqlserver requires --schema <name> or --all-schemas.");
        }

        if (!string.IsNullOrWhiteSpace(request.SchemaName) && request.AllSchemas)
        {
            throw new InvalidOperationException("extract sqlserver does not allow --schema with --all-schemas.");
        }

        if (string.IsNullOrWhiteSpace(request.TableName) && !request.AllTables)
        {
            throw new InvalidOperationException("extract sqlserver requires --table <name> or --all-tables.");
        }

        if (!string.IsNullOrWhiteSpace(request.TableName) && request.AllTables)
        {
            throw new InvalidOperationException("extract sqlserver does not allow --table with --all-tables.");
        }

        var workspace = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace(request.NewWorkspacePath);

        using var connection = new SqlConnection(request.ConnectionString);
        connection.Open();

        var databaseName = string.IsNullOrWhiteSpace(connection.Database)
            ? "(default)"
            : connection.Database;
        var dataSource = connection.DataSource ?? string.Empty;
        var systemName = request.SystemName.Trim();
        var schemaFilter = request.AllSchemas ? null : request.SchemaName.Trim();
        var tableFilter = request.AllTables ? null : request.TableName.Trim();
        var tableRows = LoadTables(connection, schemaFilter, tableFilter)
            .OrderBy(row => row.SchemaName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(row => row.SchemaName, StringComparer.Ordinal)
            .ThenBy(row => row.TableName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(row => row.TableName, StringComparer.Ordinal)
            .ToList();
        if (tableRows.Count == 0)
        {
            var filterDescription = (schemaFilter, tableFilter) switch
            {
                (null, null) => "any schema/table filter",
                (not null, null) => $"schema '{schemaFilter}'",
                (null, not null) => $"table '{tableFilter}'",
                (not null, not null) => $"table '{schemaFilter}.{tableFilter}'",
            };
            throw new InvalidOperationException(
                $"No SQL Server tables matched {filterDescription} in database '{connection.Database}'.");
        }

        var systemId = BuildSystemId(systemName);

        AddRecord(
            workspace,
            "System",
            systemId,
            values =>
            {
                values["Name"] = systemName;
                if (!string.IsNullOrWhiteSpace(dataSource))
                {
                    values["Description"] = databaseName + " @ " + dataSource;
                }
            });

        var schemaNames = tableRows.Select(row => row.SchemaName).Distinct(StringComparer.Ordinal).OrderBy(name => name, StringComparer.Ordinal);
        foreach (var schemaName in schemaNames)
        {
            var schemaId = BuildSchemaId(databaseName, schemaName);
            AddRecord(
                workspace,
                "Schema",
                schemaId,
                values => values["Name"] = schemaName,
                relationships => relationships["SystemId"] = systemId);
        }

        var tableIdsByKey = tableRows.ToDictionary(
            row => BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => BuildTableId(databaseName, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);
        var columnsByTableKey = tableRows.ToDictionary(
            row => BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadColumns(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);
        var tableKeysByTableKey = tableRows.ToDictionary(
            row => BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadTableKeys(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);
        var tableKeyFieldsByTableKey = tableRows.ToDictionary(
            row => BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadTableKeyFields(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);
        var foreignKeysByTableKey = tableRows.ToDictionary(
            row => BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadForeignKeys(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);
        var foreignKeyColumnsByTableKey = tableRows.ToDictionary(
            row => BuildScopedObjectKey(row.SchemaName, row.TableName),
            row => LoadForeignKeyColumns(connection, row.SchemaName, row.TableName),
            StringComparer.OrdinalIgnoreCase);

        foreach (var tableRow in tableRows)
        {
            var schemaId = BuildSchemaId(databaseName, tableRow.SchemaName);
            var scopedTableKey = BuildScopedObjectKey(tableRow.SchemaName, tableRow.TableName);
            var columnRows = columnsByTableKey[scopedTableKey];
            var tableKeys = tableKeysByTableKey[scopedTableKey];
            var tableKeyFields = tableKeyFieldsByTableKey[scopedTableKey];
            var foreignKeys = foreignKeysByTableKey[scopedTableKey];
            var foreignKeyColumns = foreignKeyColumnsByTableKey[scopedTableKey];

            var tableId = BuildTableId(databaseName, tableRow.SchemaName, tableRow.TableName);
            AddRecord(
                workspace,
                "Table",
                tableId,
                values =>
                {
                    values["Name"] = tableRow.TableName;
                    if (!string.IsNullOrWhiteSpace(tableRow.ObjectType))
                    {
                        values["ObjectType"] = tableRow.ObjectType;
                    }
                },
                relationships => relationships["SchemaId"] = schemaId);

            var sourceFieldIdByColumnName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var columnRow in columnRows
                         .OrderBy(row => row.TableName, StringComparer.OrdinalIgnoreCase)
                         .ThenBy(row => row.TableName, StringComparer.Ordinal)
                         .ThenBy(row => row.OrdinalPosition))
            {
                var fieldId = BuildFieldId(databaseName, columnRow.SchemaName, columnRow.TableName, columnRow.ColumnName);
                sourceFieldIdByColumnName[columnRow.ColumnName] = fieldId;
                AddRecord(
                    workspace,
                    "Field",
                    fieldId,
                    values =>
                    {
                        values["Name"] = columnRow.ColumnName;
                        values["MetaDataTypeId"] = BuildDataTypeId(columnRow.DataTypeName);
                        values["Ordinal"] = columnRow.OrdinalPosition.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        values["IsNullable"] = columnRow.IsNullable ? "true" : "false";
                        if (columnRow.IsIdentity)
                        {
                            values["IsIdentity"] = "true";
                        }
                        if (!string.IsNullOrWhiteSpace(columnRow.IdentitySeed))
                        {
                            values["IdentitySeed"] = columnRow.IdentitySeed;
                        }
                        if (!string.IsNullOrWhiteSpace(columnRow.IdentityIncrement))
                        {
                            values["IdentityIncrement"] = columnRow.IdentityIncrement;
                        }
                    },
                    relationships => relationships["TableId"] = tableId);

                AddFieldDataTypeDetails(workspace, fieldId, columnRow);
            }

            var fieldIdsByColumnName = sourceFieldIdByColumnName;
            var keyFieldsByName = tableKeyFields
                .GroupBy(row => row.KeyName, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group
                    .OrderBy(item => item.Ordinal)
                    .ToList(), StringComparer.Ordinal);

            foreach (var tableKey in tableKeys
                         .OrderBy(row => row.KeyType, StringComparer.Ordinal)
                         .ThenBy(row => row.Name, StringComparer.Ordinal))
            {
                var tableKeyId = BuildTableKeyId(databaseName, tableRow.SchemaName, tableRow.TableName, tableKey.Name);
                AddRecord(
                    workspace,
                    "TableKey",
                    tableKeyId,
                    values =>
                    {
                        values["Name"] = tableKey.Name;
                        values["KeyType"] = tableKey.KeyType;
                    },
                    relationships => relationships["TableId"] = tableId);

                if (!keyFieldsByName.TryGetValue(tableKey.Name, out var keyFields))
                {
                    continue;
                }

                foreach (var keyField in keyFields)
                {
                    if (!fieldIdsByColumnName.TryGetValue(keyField.ColumnName, out var sourceFieldId))
                    {
                        throw new InvalidOperationException(
                            $"SQL Server key '{tableRow.SchemaName}.{tableRow.TableName}.{tableKey.Name}' referenced column '{keyField.ColumnName}' that was not extracted.");
                    }

                    var tableKeyFieldId = BuildTableKeyFieldId(
                        databaseName,
                        tableRow.SchemaName,
                        tableRow.TableName,
                        tableKey.Name,
                        keyField.Ordinal);
                    AddRecord(
                        workspace,
                        "TableKeyField",
                        tableKeyFieldId,
                        values =>
                        {
                            values["Ordinal"] = keyField.Ordinal.ToString(System.Globalization.CultureInfo.InvariantCulture);
                            values["FieldName"] = keyField.ColumnName;
                        },
                        relationships =>
                        {
                            relationships["TableKeyId"] = tableKeyId;
                            relationships["FieldId"] = sourceFieldId;
                        });
                }
            }

            var foreignKeyColumnsByName = foreignKeyColumns
                .GroupBy(row => row.ForeignKeyName, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group
                    .OrderBy(item => item.Ordinal)
                    .ToList(), StringComparer.Ordinal);

            foreach (var foreignKey in foreignKeys
                         .OrderBy(row => row.Name, StringComparer.Ordinal))
            {
                var targetTableKey = BuildScopedObjectKey(foreignKey.TargetSchemaName, foreignKey.TargetTableName);
                if (!tableIdsByKey.TryGetValue(targetTableKey, out var targetTableId))
                {
                    continue;
                }

                var relationshipId = BuildRelationshipId(databaseName, tableRow.SchemaName, tableRow.TableName, foreignKey.Name);
                AddRecord(
                    workspace,
                    "TableRelationship",
                    relationshipId,
                    values =>
                    {
                        values["Name"] = foreignKey.Name;
                    },
                    relationships =>
                    {
                        relationships["SourceTableId"] = tableId;
                        relationships["TargetTableId"] = targetTableId;
                    });

                if (!foreignKeyColumnsByName.TryGetValue(foreignKey.Name, out var fkColumns))
                {
                    continue;
                }

                var targetFieldIdByColumnName = columnsByTableKey[targetTableKey]
                    .ToDictionary(
                        row => row.ColumnName,
                        row => BuildFieldId(databaseName, row.SchemaName, row.TableName, row.ColumnName),
                        StringComparer.OrdinalIgnoreCase);

                foreach (var fkColumn in fkColumns)
                {
                    if (!fieldIdsByColumnName.TryGetValue(fkColumn.SourceColumnName, out var sourceFieldId))
                    {
                        throw new InvalidOperationException(
                            $"SQL Server foreign key '{tableRow.SchemaName}.{tableRow.TableName}.{foreignKey.Name}' referenced source column '{fkColumn.SourceColumnName}' that was not extracted.");
                    }

                    if (!targetFieldIdByColumnName.TryGetValue(fkColumn.TargetColumnName, out var targetFieldId))
                    {
                        throw new InvalidOperationException(
                            $"SQL Server foreign key '{tableRow.SchemaName}.{tableRow.TableName}.{foreignKey.Name}' referenced target column '{fkColumn.TargetColumnName}' that was not extracted.");
                    }

                    var relationshipFieldId = BuildRelationshipFieldId(
                        databaseName,
                        tableRow.SchemaName,
                        tableRow.TableName,
                        foreignKey.Name,
                        fkColumn.Ordinal);
                    AddRecord(
                        workspace,
                        "TableRelationshipField",
                        relationshipFieldId,
                        values =>
                        {
                            values["Ordinal"] = fkColumn.Ordinal.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        },
                        relationships =>
                        {
                            relationships["TableRelationshipId"] = relationshipId;
                            relationships["SourceFieldId"] = sourceFieldId;
                            relationships["TargetFieldId"] = targetFieldId;
                        });
                }
            }
        }

        workspace.IsDirty = true;
        return workspace;
    }

    private static List<TableRow> LoadTables(SqlConnection connection, string? schemaName, string? tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                t.TABLE_SCHEMA,
                t.TABLE_NAME,
                t.TABLE_TYPE
            from INFORMATION_SCHEMA.TABLES t
            join sys.objects o on o.object_id = object_id(quotename(t.TABLE_SCHEMA) + '.' + quotename(t.TABLE_NAME))
            where t.TABLE_TYPE in ('BASE TABLE', 'VIEW')
              and t.TABLE_SCHEMA not in ('sys', 'INFORMATION_SCHEMA')
              and o.is_ms_shipped = 0
              and t.TABLE_NAME <> 'sysdiagrams'
              and (@schemaName is null or t.TABLE_SCHEMA = @schemaName)
              and (@tableName is null or t.TABLE_NAME = @tableName)
            order by t.TABLE_SCHEMA, t.TABLE_NAME
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = string.IsNullOrWhiteSpace(schemaName) ? DBNull.Value : schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = string.IsNullOrWhiteSpace(tableName) ? DBNull.Value : tableName });

        var rows = new List<TableRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new TableRow(
                SchemaName: reader.GetString(0),
                TableName: reader.GetString(1),
                ObjectType: NormalizeTableType(reader.GetString(2))));
        }

        return rows;
    }

    private static List<ColumnRow> LoadColumns(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                s.name as SchemaName,
                t.name as TableName,
                c.name as ColumnName,
                c.column_id as OrdinalPosition,
                c.is_nullable,
                ty.name as DataTypeName,
                case
                    when c.max_length = -1 then -1
                    when ty.name in ('nchar', 'nvarchar') then c.max_length / 2
                    else c.max_length
                end as LengthValue,
                case
                    when ty.name in ('decimal', 'numeric') then convert(int, c.precision)
                    when ty.name in ('time', 'datetime2', 'datetimeoffset') then convert(int, c.scale)
                    else null
                end as PrecisionValue,
                case
                    when ty.name in ('decimal', 'numeric') then convert(int, c.scale)
                    else null
                end as ScaleValue,
                c.is_identity,
                convert(nvarchar(50), ic.seed_value) as IdentitySeed,
                convert(nvarchar(50), ic.increment_value) as IdentityIncrement
            from sys.tables t
            join sys.schemas s on s.schema_id = t.schema_id
            join sys.columns c on c.object_id = t.object_id
            join sys.types ty on ty.user_type_id = c.user_type_id
            left join sys.identity_columns ic on ic.object_id = c.object_id and ic.column_id = c.column_id
            where s.name = @schemaName
              and t.name = @tableName
            order by t.name, c.column_id
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<ColumnRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new ColumnRow(
                SchemaName: reader.GetString(0),
                TableName: reader.GetString(1),
                ColumnName: reader.GetString(2),
                OrdinalPosition: ReadInt32(reader, 3),
                IsNullable: reader.GetBoolean(4),
                DataTypeName: reader.GetString(5),
                Length: ReadNullableInt(reader, 6),
                Precision: ReadNullableInt(reader, 7),
                Scale: ReadNullableInt(reader, 8),
                IsIdentity: reader.GetBoolean(9),
                IdentitySeed: ReadNullableString(reader, 10),
                IdentityIncrement: ReadNullableString(reader, 11)));
        }

        return rows;
    }

    private static List<TableKeyRow> LoadTableKeys(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                kc.name as KeyName,
                kc.type as KeyConstraintType
            from sys.key_constraints kc
            join sys.tables srcTable on srcTable.object_id = kc.parent_object_id
            join sys.schemas srcSchema on srcSchema.schema_id = srcTable.schema_id
            where srcSchema.name = @schemaName
              and srcTable.name = @tableName
              and kc.type in ('PK', 'UQ')
            order by
                case kc.type when 'PK' then 0 else 1 end,
                kc.name
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<TableKeyRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new TableKeyRow(
                Name: reader.GetString(0),
                KeyType: NormalizeKeyType(reader.GetString(1))));
        }

        return rows;
    }

    private static List<TableKeyFieldRow> LoadTableKeyFields(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                kc.name as KeyName,
                ic.key_ordinal as Ordinal,
                srcColumn.name as ColumnName
            from sys.key_constraints kc
            join sys.tables srcTable on srcTable.object_id = kc.parent_object_id
            join sys.schemas srcSchema on srcSchema.schema_id = srcTable.schema_id
            join sys.index_columns ic on ic.object_id = kc.parent_object_id and ic.index_id = kc.unique_index_id
            join sys.columns srcColumn on srcColumn.object_id = ic.object_id and srcColumn.column_id = ic.column_id
            where srcSchema.name = @schemaName
              and srcTable.name = @tableName
              and kc.type in ('PK', 'UQ')
              and ic.key_ordinal > 0
            order by kc.name, ic.key_ordinal
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<TableKeyFieldRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new TableKeyFieldRow(
                KeyName: reader.GetString(0),
                Ordinal: ReadInt32(reader, 1),
                ColumnName: reader.GetString(2)));
        }

        return rows;
    }

    private static List<ForeignKeyRow> LoadForeignKeys(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                fk.name as ForeignKeyName,
                refSchema.name as TargetSchemaName,
                refTable.name as TargetTableName
            from sys.foreign_keys fk
            join sys.tables srcTable on srcTable.object_id = fk.parent_object_id
            join sys.schemas srcSchema on srcSchema.schema_id = srcTable.schema_id
            join sys.tables refTable on refTable.object_id = fk.referenced_object_id
            join sys.schemas refSchema on refSchema.schema_id = refTable.schema_id
            where srcSchema.name = @schemaName
              and srcTable.name = @tableName
              and fk.is_disabled = 0
              and fk.is_not_trusted = 0
            order by fk.name
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<ForeignKeyRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new ForeignKeyRow(
                Name: reader.GetString(0),
                TargetSchemaName: reader.GetString(1),
                TargetTableName: reader.GetString(2)));
        }

        return rows;
    }

    private static List<ForeignKeyColumnRow> LoadForeignKeyColumns(SqlConnection connection, string schemaName, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            select
                fk.name as ForeignKeyName,
                fkc.constraint_column_id as Ordinal,
                srcColumn.name as SourceColumnName,
                refColumn.name as TargetColumnName
            from sys.foreign_keys fk
            join sys.tables srcTable on srcTable.object_id = fk.parent_object_id
            join sys.schemas srcSchema on srcSchema.schema_id = srcTable.schema_id
            join sys.foreign_key_columns fkc on fkc.constraint_object_id = fk.object_id
            join sys.columns srcColumn on srcColumn.object_id = fkc.parent_object_id and srcColumn.column_id = fkc.parent_column_id
            join sys.columns refColumn on refColumn.object_id = fkc.referenced_object_id and refColumn.column_id = fkc.referenced_column_id
            where srcSchema.name = @schemaName
              and srcTable.name = @tableName
            order by fk.name, fkc.constraint_column_id
            """;
        command.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 128) { Value = schemaName });
        command.Parameters.Add(new SqlParameter("@tableName", SqlDbType.NVarChar, 128) { Value = tableName });

        var rows = new List<ForeignKeyColumnRow>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new ForeignKeyColumnRow(
                ForeignKeyName: reader.GetString(0),
                Ordinal: ReadInt32(reader, 1),
                SourceColumnName: reader.GetString(2),
                TargetColumnName: reader.GetString(3)));
        }

        return rows;
    }

    private static int? ReadNullableInt(SqlDataReader reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal))
        {
            return null;
        }

        var value = reader.GetValue(ordinal);
        return value switch
        {
            byte byteValue => byteValue,
            short shortValue => shortValue,
            int intValue => intValue,
            long longValue => checked((int)longValue),
            decimal decimalValue => decimal.ToInt32(decimalValue),
            _ => Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture),
        };
    }

    private static int ReadInt32(SqlDataReader reader, int ordinal)
    {
        var value = ReadNullableInt(reader, ordinal);
        if (!value.HasValue)
        {
            throw new InvalidOperationException($"Expected non-null integer at ordinal {ordinal}.");
        }

        return value.Value;
    }

    private static string? ReadNullableString(SqlDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal)
            ? null
            : reader.GetString(ordinal);
    }

    private static string NormalizeTableType(string tableType)
    {
        return tableType switch
        {
            "BASE TABLE" => "Table",
            "VIEW" => "View",
            _ => tableType,
        };
    }

    private static string NormalizeKeyType(string keyConstraintType)
    {
        return keyConstraintType switch
        {
            "PK" => "primary",
            "UQ" => "unique",
            _ => keyConstraintType,
        };
    }

    private static void AddRecord(
        Workspace workspace,
        string entityName,
        string id,
        Action<Dictionary<string, string>>? populateValues = null,
        Action<Dictionary<string, string>>? populateRelationships = null)
    {
        var record = new GenericRecord
        {
            Id = id,
        };
        populateValues?.Invoke(record.Values);
        populateRelationships?.Invoke(record.RelationshipIds);
        workspace.Instance.GetOrCreateEntityRecords(entityName).Add(record);
    }

    private static string BuildSystemId(string databaseName)
    {
        return "sqlserver:system:" + databaseName;
    }

    private static string BuildSchemaId(string databaseName, string schemaName)
    {
        return "sqlserver:" + databaseName + ":schema:" + schemaName;
    }

    private static string BuildTableId(string databaseName, string schemaName, string tableName)
    {
        return "sqlserver:" + databaseName + ":schema:" + schemaName + ":table:" + tableName;
    }

    private static string BuildDataTypeId(string dataTypeName)
    {
        return "sqlserver:type:" + dataTypeName;
    }

    private static string BuildScopedObjectKey(string schemaName, string objectName)
    {
        return schemaName + "." + objectName;
    }

    private static string BuildTableKeyId(string databaseName, string schemaName, string tableName, string keyName)
    {
        return "sqlserver:" + databaseName + ":schema:" + schemaName + ":table:" + tableName + ":key:" + keyName;
    }

    private static string BuildTableKeyFieldId(string databaseName, string schemaName, string tableName, string keyName, int ordinal)
    {
        return BuildTableKeyId(databaseName, schemaName, tableName, keyName) +
               ":field:" +
               ordinal.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string BuildFieldId(string databaseName, string schemaName, string tableName, string columnName)
    {
        return "sqlserver:" + databaseName + ":schema:" + schemaName + ":table:" + tableName + ":field:" + columnName;
    }

    private static void AddFieldDataTypeDetails(Workspace workspace, string fieldId, ColumnRow columnRow)
    {
        switch (columnRow.DataTypeName.ToLowerInvariant())
        {
            case "char":
            case "varchar":
            case "nchar":
            case "nvarchar":
            case "binary":
            case "varbinary":
                AddFieldDataTypeDetail(workspace, fieldId, "Length", columnRow.Length);
                break;

            case "decimal":
            case "numeric":
                AddFieldDataTypeDetail(workspace, fieldId, "Precision", columnRow.Precision);
                AddFieldDataTypeDetail(workspace, fieldId, "Scale", columnRow.Scale);
                break;

            case "time":
            case "datetime2":
            case "datetimeoffset":
                AddFieldDataTypeDetail(workspace, fieldId, "Precision", columnRow.Precision);
                break;
        }
    }

    private static void AddFieldDataTypeDetail(Workspace workspace, string fieldId, string detailName, int? detailValue)
    {
        if (!detailValue.HasValue)
        {
            return;
        }

        AddRecord(
            workspace,
            "FieldDataTypeDetail",
            fieldId + ":detail:" + detailName,
            values =>
            {
                values["Name"] = detailName;
                values["Value"] = detailValue.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            },
            relationships => relationships["FieldId"] = fieldId);
    }

    private static string BuildRelationshipId(string databaseName, string schemaName, string tableName, string relationshipName)
    {
        return "sqlserver:" + databaseName + ":schema:" + schemaName + ":table:" + tableName + ":relationship:" + relationshipName;
    }

    private static string BuildRelationshipFieldId(string databaseName, string schemaName, string tableName, string relationshipName, int ordinal)
    {
        return BuildRelationshipId(databaseName, schemaName, tableName, relationshipName) +
               ":field:" +
               ordinal.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    private readonly record struct TableRow(
        string SchemaName,
        string TableName,
        string ObjectType);

    private readonly record struct ColumnRow(
        string SchemaName,
        string TableName,
        string ColumnName,
        int OrdinalPosition,
        bool IsNullable,
        string DataTypeName,
        int? Length,
        int? Precision,
        int? Scale,
        bool IsIdentity,
        string? IdentitySeed,
        string? IdentityIncrement);

    private readonly record struct ForeignKeyRow(
        string Name,
        string TargetSchemaName,
        string TargetTableName);

    private readonly record struct TableKeyRow(
        string Name,
        string KeyType);

    private readonly record struct TableKeyFieldRow(
        string KeyName,
        int Ordinal,
        string ColumnName);

    private readonly record struct ForeignKeyColumnRow(
        string ForeignKeyName,
        int Ordinal,
        string SourceColumnName,
        string TargetColumnName);
}

public sealed class SqlServerExtractRequest
{
    public string NewWorkspacePath { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public string SchemaName { get; set; } = string.Empty;
    public bool AllSchemas { get; set; }
    public string TableName { get; set; } = string.Empty;
    public bool AllTables { get; set; }
}
