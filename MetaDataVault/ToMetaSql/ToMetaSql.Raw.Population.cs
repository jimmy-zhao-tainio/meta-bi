using System.Globalization;
using MetaDataVaultImplementation;
using MetaRawDataVault;
using MetaSql;

namespace MetaDataVault.ToMetaSql;

public static partial class Converter
{
    private static void PopulateRawMetaSqlModel(
        MetaRawDataVaultModel model,
        ConversionContext context,
        IReadOnlyDictionary<string, List<SourceFieldDataTypeDetail>> sourceFieldDetailsByFieldId,
        IReadOnlyDictionary<string, List<RawHubKeyPart>> rawHubKeyPartsByHubId,
        IReadOnlyDictionary<string, List<RawHubSatellite>> rawHubSatellitesByHubId,
        IReadOnlyDictionary<string, List<RawHubSatelliteAttribute>> rawHubSatelliteAttributesBySatelliteId,
        IReadOnlyDictionary<string, List<RawLinkHub>> rawLinkHubsByLinkId,
        IReadOnlyDictionary<string, List<RawLinkSatellite>> rawLinkSatellitesByLinkId,
        IReadOnlyDictionary<string, List<RawLinkSatelliteAttribute>> rawLinkSatelliteAttributesBySatelliteId)
    {
        var rawHubImplementation = RequireSingleImplementation(context.ImplementationModel.RawHubImplementationList, nameof(context.ImplementationModel.RawHubImplementationList));
        var rawHubSatelliteImplementation = RequireSingleImplementation(context.ImplementationModel.RawHubSatelliteImplementationList, nameof(context.ImplementationModel.RawHubSatelliteImplementationList));
        var rawLinkImplementation = RequireSingleImplementation(context.ImplementationModel.RawLinkImplementationList, nameof(context.ImplementationModel.RawLinkImplementationList));
        var rawLinkSatelliteImplementation = RequireSingleImplementation(context.ImplementationModel.RawLinkSatelliteImplementationList, nameof(context.ImplementationModel.RawLinkSatelliteImplementationList));

        var hubTablesByHub = new Dictionary<RawHub, Table>(ReferenceEqualityComparer.Instance);
        var hubHashKeyColumnsByHub = new Dictionary<RawHub, TableColumn>(ReferenceEqualityComparer.Instance);
        var linkTablesByLink = new Dictionary<RawLink, Table>(ReferenceEqualityComparer.Instance);
        var linkHashKeyColumnsByLink = new Dictionary<RawLink, TableColumn>(ReferenceEqualityComparer.Instance);

        foreach (var hub in model.RawHubList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                rawHubImplementation.SchemaName,
                ApplyPattern(rawHubImplementation.TableNamePattern, ("Name", hub.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var protectedColumnNames = BuildProtectedColumnNames(
                rawHubImplementation.HashKeyColumnName,
                rawHubImplementation.LoadTimestampColumnName,
                rawHubImplementation.RecordSourceColumnName,
                rawHubImplementation.AuditIdColumnName);
            var hashKeyColumn = AddImplementationColumn(
                context,
                table,
                rawHubImplementation.HashKeyColumnName,
                rawHubImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawHubImplementation.HashKeyLength));

            foreach (var keyPart in GetGroup(rawHubKeyPartsByHubId, hub.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Id, StringComparer.Ordinal))
            {
                AddSourceFieldColumn(
                    context,
                    table,
                    keyPart.SourceField,
                    reservedColumnNames,
                    sourceFieldDetailsByFieldId,
                    protectedColumnNames);
            }

            AddImplementationColumn(
                context,
                table,
                rawHubImplementation.LoadTimestampColumnName,
                rawHubImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", rawHubImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                rawHubImplementation.RecordSourceColumnName,
                rawHubImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawHubImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                rawHubImplementation.AuditIdColumnName,
                rawHubImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            AddPrimaryKey(
                context,
                table,
                ApplyPattern(rawHubImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)),
                hashKeyColumn);

            hubTablesByHub[hub] = table;
            hubHashKeyColumnsByHub[hub] = hashKeyColumn;
        }

        foreach (var satellite in model.RawHubSatelliteList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                rawHubSatelliteImplementation.SchemaName,
                ApplyPattern(
                    rawHubSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.RawHub.Name),
                    ("Name", satellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var protectedColumnNames = BuildProtectedColumnNames(
                rawHubSatelliteImplementation.ParentHashKeyColumnName,
                rawHubSatelliteImplementation.HashDiffColumnName,
                rawHubSatelliteImplementation.LoadTimestampColumnName,
                rawHubSatelliteImplementation.RecordSourceColumnName,
                rawHubSatelliteImplementation.AuditIdColumnName);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                rawHubSatelliteImplementation.ParentHashKeyColumnName,
                rawHubSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawHubSatelliteImplementation.ParentHashKeyLength));

            foreach (var attribute in GetGroup(rawHubSatelliteAttributesBySatelliteId, satellite.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Id, StringComparer.Ordinal))
            {
                AddSourceFieldColumn(
                    context,
                    table,
                    attribute.SourceField,
                    reservedColumnNames,
                    sourceFieldDetailsByFieldId,
                    protectedColumnNames);
            }

            AddImplementationColumn(
                context,
                table,
                rawHubSatelliteImplementation.HashDiffColumnName,
                rawHubSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawHubSatelliteImplementation.HashDiffLength));

            AddImplementationColumn(
                context,
                table,
                rawHubSatelliteImplementation.LoadTimestampColumnName,
                rawHubSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", rawHubSatelliteImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                rawHubSatelliteImplementation.RecordSourceColumnName,
                rawHubSatelliteImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawHubSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                rawHubSatelliteImplementation.AuditIdColumnName,
                rawHubSatelliteImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            if (hubTablesByHub.TryGetValue(satellite.RawHub, out var parentTable) &&
                hubHashKeyColumnsByHub.TryGetValue(satellite.RawHub, out var parentHashKeyTarget))
            {
                AddForeignKey(
                    context,
                    table,
                    ApplyPattern(
                        rawHubSatelliteImplementation.ParentForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("ParentTableName", parentTable.Name)),
                    parentTable,
                    new[] { (parentHashKeyColumn, parentHashKeyTarget) });
            }
        }

        foreach (var link in model.RawLinkList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                rawLinkImplementation.SchemaName,
                ApplyPattern(rawLinkImplementation.TableNamePattern, ("Name", link.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var hashKeyColumn = AddImplementationColumn(
                context,
                table,
                rawLinkImplementation.HashKeyColumnName,
                rawLinkImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawLinkImplementation.HashKeyLength));

            foreach (var linkHub in GetGroup(rawLinkHubsByLinkId, link.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Id, StringComparer.Ordinal))
            {
                var endHashKeyColumn = AddImplementationColumn(
                    context,
                    table,
                    ApplyPattern(rawLinkImplementation.EndHashKeyColumnPattern, ("RoleName", linkHub.RoleName)),
                    rawHubImplementation.HashKeyDataTypeId,
                    "false",
                    reservedColumnNames,
                    ("Length", rawHubImplementation.HashKeyLength));

                if (hubTablesByHub.TryGetValue(linkHub.RawHub, out var targetHubTable) &&
                    hubHashKeyColumnsByHub.TryGetValue(linkHub.RawHub, out var targetHubHashKey))
                {
                    AddForeignKey(
                        context,
                        table,
                        ApplyPattern(
                            rawLinkImplementation.HubForeignKeyNamePattern,
                            ("TableName", table.Name),
                            ("TargetTableName", targetHubTable.Name),
                            ("SourceColumnName", endHashKeyColumn.Name)),
                        targetHubTable,
                        new[] { (endHashKeyColumn, targetHubHashKey) });
                }
            }

            AddImplementationColumn(
                context,
                table,
                rawLinkImplementation.LoadTimestampColumnName,
                rawLinkImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", rawLinkImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                rawLinkImplementation.RecordSourceColumnName,
                rawLinkImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawLinkImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                rawLinkImplementation.AuditIdColumnName,
                rawLinkImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            AddPrimaryKey(
                context,
                table,
                ApplyPattern(rawLinkImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)),
                hashKeyColumn);

            linkTablesByLink[link] = table;
            linkHashKeyColumnsByLink[link] = hashKeyColumn;
        }

        foreach (var satellite in model.RawLinkSatelliteList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                rawLinkSatelliteImplementation.SchemaName,
                ApplyPattern(
                    rawLinkSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.RawLink.Name),
                    ("Name", satellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var protectedColumnNames = BuildProtectedColumnNames(
                rawLinkSatelliteImplementation.ParentHashKeyColumnName,
                rawLinkSatelliteImplementation.HashDiffColumnName,
                rawLinkSatelliteImplementation.LoadTimestampColumnName,
                rawLinkSatelliteImplementation.RecordSourceColumnName,
                rawLinkSatelliteImplementation.AuditIdColumnName);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                rawLinkSatelliteImplementation.ParentHashKeyColumnName,
                rawLinkSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawLinkSatelliteImplementation.ParentHashKeyLength));

            foreach (var attribute in GetGroup(rawLinkSatelliteAttributesBySatelliteId, satellite.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Id, StringComparer.Ordinal))
            {
                AddSourceFieldColumn(
                    context,
                    table,
                    attribute.SourceField,
                    reservedColumnNames,
                    sourceFieldDetailsByFieldId,
                    protectedColumnNames);
            }

            AddImplementationColumn(
                context,
                table,
                rawLinkSatelliteImplementation.HashDiffColumnName,
                rawLinkSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawLinkSatelliteImplementation.HashDiffLength));

            AddImplementationColumn(
                context,
                table,
                rawLinkSatelliteImplementation.LoadTimestampColumnName,
                rawLinkSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", rawLinkSatelliteImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                rawLinkSatelliteImplementation.RecordSourceColumnName,
                rawLinkSatelliteImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawLinkSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                rawLinkSatelliteImplementation.AuditIdColumnName,
                rawLinkSatelliteImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            if (linkTablesByLink.TryGetValue(satellite.RawLink, out var parentTable) &&
                linkHashKeyColumnsByLink.TryGetValue(satellite.RawLink, out var parentHashKeyTarget))
            {
                AddForeignKey(
                    context,
                    table,
                    ApplyPattern(
                        rawLinkSatelliteImplementation.ParentForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("ParentTableName", parentTable.Name)),
                    parentTable,
                    new[] { (parentHashKeyColumn, parentHashKeyTarget) });
            }
        }
    }

    private static T RequireSingleImplementation<T>(IReadOnlyList<T> rows, string logicalName)
        where T : class
    {
        if (rows.Count != 1)
        {
            throw new InvalidOperationException($"{logicalName} must contain exactly one row for this projection path.");
        }

        return rows[0];
    }

    private static Table AddTable(ConversionContext context, string schemaName, string name)
    {
        if (!context.SchemasByName.TryGetValue(schemaName, out var schema))
        {
            throw new InvalidOperationException($"Projected schema '{schemaName}' is not present in conversion context.");
        }

        var id = $"{schema.Id}.{name}";
        EnsureUniqueId(context.MetaSql.TableList.Select(row => row.Id), id, "table");

        var table = new Table
        {
            Id = id,
            Name = name,
            SchemaId = schema.Id,
            Schema = schema,
        };

        context.MetaSql.TableList.Add(table);
        return table;
    }

    private static TableColumn AddSourceFieldColumn(
        ConversionContext context,
        Table table,
        SourceField sourceField,
        HashSet<string> reservedColumnNames,
        IReadOnlyDictionary<string, List<SourceFieldDataTypeDetail>> sourceFieldDetailsByFieldId,
        IReadOnlySet<string>? protectedColumnNames = null)
    {
        var column = AddColumn(
            context,
            table,
            sourceField.Name,
            sourceField.DataTypeId,
            sourceField.IsNullable,
            reservedColumnNames,
            protectedColumnNames);

        foreach (var detail in GetGroup(sourceFieldDetailsByFieldId, sourceField.Id).OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            AddDetail(context, column, detail.Name, detail.Value);
        }

        return column;
    }

    private static TableColumn AddImplementationColumn(
        ConversionContext context,
        Table table,
        string name,
        string metaDataTypeId,
        string isNullable,
        HashSet<string> reservedColumnNames,
        params (string Name, string Value)[] details)
    {
        var column = AddColumn(
            context,
            table,
            name,
            ResolveProjectedMetaDataTypeId(context, metaDataTypeId),
            isNullable,
            reservedColumnNames);

        foreach (var detail in details)
        {
            AddDetail(context, column, detail.Name, detail.Value);
        }

        return column;
    }

    private static string ResolveProjectedMetaDataTypeId(ConversionContext context, string metaDataTypeId)
    {
        return context.BusinessTypeLowering is null
            ? metaDataTypeId
            : context.BusinessTypeLowering.LowerRequired(metaDataTypeId);
    }

    private static TableColumn AddColumn(
        ConversionContext context,
        Table table,
        string requestedName,
        string metaDataTypeId,
        string isNullable,
        HashSet<string> reservedColumnNames,
        IReadOnlySet<string>? protectedColumnNames = null)
    {
        var actualName = ReserveColumnName(reservedColumnNames, requestedName, protectedColumnNames);
        var id = $"{table.Id}.{actualName}";
        EnsureUniqueId(context.MetaSql.TableColumnList.Select(row => row.Id), id, "table column");
        var ordinal = (context.MetaSql.TableColumnList.Count(row => ReferenceEquals(row.Table, table)) + 1).ToString(CultureInfo.InvariantCulture);

        var column = new TableColumn
        {
            Id = id,
            Name = actualName,
            Ordinal = ordinal,
            MetaDataTypeId = metaDataTypeId,
            IsNullable = isNullable,
            TableId = table.Id,
            Table = table,
        };

        context.MetaSql.TableColumnList.Add(column);
        return column;
    }

    private static void AddDetail(ConversionContext context, TableColumn tableColumn, string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        context.MetaSql.TableColumnDataTypeDetailList.Add(new TableColumnDataTypeDetail
        {
            Id = $"{tableColumn.Id}.detail.{name}",
            Name = name,
            Value = value,
            TableColumnId = tableColumn.Id,
            TableColumn = tableColumn,
        });
    }

    private static void AddPrimaryKey(
        ConversionContext context,
        Table table,
        string name,
        TableColumn tableColumn)
    {
        var id = $"{table.Id}.pk.{name}";
        EnsureUniqueId(context.MetaSql.PrimaryKeyList.Select(row => row.Id), id, "primary key");

        var primaryKey = new PrimaryKey
        {
            Id = id,
            Name = name,
            TableId = table.Id,
            Table = table,
        };
        context.MetaSql.PrimaryKeyList.Add(primaryKey);
        context.MetaSql.PrimaryKeyColumnList.Add(new PrimaryKeyColumn
        {
            Id = $"{id}.column.1",
            PrimaryKeyId = primaryKey.Id,
            PrimaryKey = primaryKey,
            TableColumnId = tableColumn.Id,
            TableColumn = tableColumn,
            Ordinal = "1",
        });
    }

    private static void AddForeignKey(
        ConversionContext context,
        Table sourceTable,
        string name,
        Table targetTable,
        IEnumerable<(TableColumn SourceColumn, TableColumn TargetColumn)> columnPairs)
    {
        var id = $"{sourceTable.Id}.fk.{name}";
        EnsureUniqueId(context.MetaSql.ForeignKeyList.Select(row => row.Id), id, "foreign key");

        var foreignKey = new ForeignKey
        {
            Id = id,
            Name = name,
            SourceTableId = sourceTable.Id,
            SourceTable = sourceTable,
            TargetTableId = targetTable.Id,
            TargetTable = targetTable,
        };
        context.MetaSql.ForeignKeyList.Add(foreignKey);

        var ordinal = 1;
        foreach (var (sourceColumn, targetColumn) in columnPairs)
        {
            context.MetaSql.ForeignKeyColumnList.Add(new ForeignKeyColumn
            {
                Id = $"{id}.column.{ordinal}",
                ForeignKeyId = foreignKey.Id,
                ForeignKey = foreignKey,
                SourceColumnId = sourceColumn.Id,
                SourceColumn = sourceColumn,
                TargetColumnId = targetColumn.Id,
                TargetColumn = targetColumn,
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture),
            });
            ordinal++;
        }
    }

    private static HashSet<string> BuildProtectedColumnNames(params string[] names)
    {
        return names
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static string ReserveColumnName(
        HashSet<string> reservedColumnNames,
        string requestedName,
        IReadOnlySet<string>? protectedColumnNames = null)
    {
        var actualName = requestedName;
        while (reservedColumnNames.Contains(actualName) ||
               (protectedColumnNames is not null && protectedColumnNames.Contains(actualName)))
        {
            actualName = "_" + actualName;
        }

        reservedColumnNames.Add(actualName);
        return actualName;
    }

    private static void EnsureUniqueId(IEnumerable<string> existingIds, string id, string logicalName)
    {
        if (existingIds.Contains(id, StringComparer.Ordinal))
        {
            throw new InvalidOperationException($"Projected {logicalName} id '{id}' is duplicated. The physical naming contract is not unique.");
        }
    }

    private static string ApplyPattern(string pattern, params (string Token, string Value)[] replacements)
    {
        var output = pattern;
        foreach (var (token, value) in replacements)
        {
            output = output.Replace("{" + token + "}", value, StringComparison.Ordinal);
        }

        return output;
    }

    private static int ParseOrdinal(string ordinal)
    {
        return int.TryParse(ordinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : int.MaxValue;
    }
}
