using System.Globalization;
using MetaDataVaultImplementation;
using MetaRawDataVault;
using SqlModel;

namespace MetaDataVault.ToMetaSql;

public static partial class Converter
{
    private static void PopulateRawSqlModel(
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
                $"RawHub:{hub.Id}",
                ApplyPattern(rawHubImplementation.TableNamePattern, ("Name", hub.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var hashKeyColumn = AddColumn(
                context,
                table,
                $"{table.Id}:Column:HashKey",
                rawHubImplementation.HashKeyColumnName,
                rawHubImplementation.HashKeyDataTypeId,
                isNullable: "false",
                reservedColumnNames);
            AddDetail(context, hashKeyColumn, "Length", rawHubImplementation.HashKeyLength);

            foreach (var keyPart in GetGroup(rawHubKeyPartsByHubId, hub.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Id, StringComparer.Ordinal))
            {
                AddSourceFieldColumn(
                    context,
                    table,
                    $"{table.Id}:Column:KeyPart:{keyPart.Id}",
                    keyPart.SourceField,
                    reservedColumnNames,
                    sourceFieldDetailsByFieldId);
            }

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:LoadTimestamp",
                rawHubImplementation.LoadTimestampColumnName,
                rawHubImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", rawHubImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RecordSource",
                rawHubImplementation.RecordSourceColumnName,
                rawHubImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawHubImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
                rawHubImplementation.AuditIdColumnName,
                rawHubImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            AddPrimaryKey(
                context,
                table,
                $"{table.Id}:PrimaryKey",
                $"PK_{table.Name}",
                hashKeyColumn);

            hubTablesByHub[hub] = table;
            hubHashKeyColumnsByHub[hub] = hashKeyColumn;
        }

        foreach (var satellite in model.RawHubSatelliteList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                $"RawHubSatellite:{satellite.Id}",
                ApplyPattern(
                    rawHubSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.RawHub.Name),
                    ("Name", satellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:ParentHashKey",
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
                    $"{table.Id}:Column:Attribute:{attribute.Id}",
                    attribute.SourceField,
                    reservedColumnNames,
                    sourceFieldDetailsByFieldId);
            }

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:HashDiff",
                rawHubSatelliteImplementation.HashDiffColumnName,
                rawHubSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawHubSatelliteImplementation.HashDiffLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:LoadTimestamp",
                rawHubSatelliteImplementation.LoadTimestampColumnName,
                rawHubSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", rawHubSatelliteImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RecordSource",
                rawHubSatelliteImplementation.RecordSourceColumnName,
                rawHubSatelliteImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawHubSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
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
                    $"{table.Id}:ForeignKey:ParentHub",
                    $"FK_{table.Name}_{parentTable.Name}",
                    parentTable,
                    new[] { (parentHashKeyColumn, parentHashKeyTarget) });
            }
        }

        foreach (var link in model.RawLinkList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                $"RawLink:{link.Id}",
                ApplyPattern(rawLinkImplementation.TableNamePattern, ("Name", link.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var hashKeyColumn = AddColumn(
                context,
                table,
                $"{table.Id}:Column:HashKey",
                rawLinkImplementation.HashKeyColumnName,
                rawLinkImplementation.HashKeyDataTypeId,
                isNullable: "false",
                reservedColumnNames);
            AddDetail(context, hashKeyColumn, "Length", rawLinkImplementation.HashKeyLength);

            foreach (var linkHub in GetGroup(rawLinkHubsByLinkId, link.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Id, StringComparer.Ordinal))
            {
                var endHashKeyColumn = AddImplementationColumn(
                    context,
                    table,
                    $"{table.Id}:Column:EndHashKey:{linkHub.Id}",
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
                        $"{table.Id}:ForeignKey:Hub:{linkHub.Id}",
                        $"FK_{table.Name}_{targetHubTable.Name}_{endHashKeyColumn.Name}",
                        targetHubTable,
                        new[] { (endHashKeyColumn, targetHubHashKey) });
                }
            }

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:LoadTimestamp",
                rawLinkImplementation.LoadTimestampColumnName,
                rawLinkImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", rawLinkImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RecordSource",
                rawLinkImplementation.RecordSourceColumnName,
                rawLinkImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawLinkImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
                rawLinkImplementation.AuditIdColumnName,
                rawLinkImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            AddPrimaryKey(
                context,
                table,
                $"{table.Id}:PrimaryKey",
                $"PK_{table.Name}",
                hashKeyColumn);

            linkTablesByLink[link] = table;
            linkHashKeyColumnsByLink[link] = hashKeyColumn;
        }

        foreach (var satellite in model.RawLinkSatelliteList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                $"RawLinkSatellite:{satellite.Id}",
                ApplyPattern(
                    rawLinkSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.RawLink.Name),
                    ("Name", satellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:ParentHashKey",
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
                    $"{table.Id}:Column:Attribute:{attribute.Id}",
                    attribute.SourceField,
                    reservedColumnNames,
                    sourceFieldDetailsByFieldId);
            }

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:HashDiff",
                rawLinkSatelliteImplementation.HashDiffColumnName,
                rawLinkSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawLinkSatelliteImplementation.HashDiffLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:LoadTimestamp",
                rawLinkSatelliteImplementation.LoadTimestampColumnName,
                rawLinkSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", rawLinkSatelliteImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RecordSource",
                rawLinkSatelliteImplementation.RecordSourceColumnName,
                rawLinkSatelliteImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", rawLinkSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
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
                    $"{table.Id}:ForeignKey:ParentLink",
                    $"FK_{table.Name}_{parentTable.Name}",
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

    private static Table AddTable(ConversionContext context, string id, string name)
    {
        var table = new Table
        {
            Id = id,
            Name = name,
            SchemaId = context.DefaultSchema.Id,
            Schema = context.DefaultSchema,
        };

        context.SqlModel.TableList.Add(table);
        return table;
    }

    private static TableColumn AddSourceFieldColumn(
        ConversionContext context,
        Table table,
        string id,
        SourceField sourceField,
        HashSet<string> reservedColumnNames,
        IReadOnlyDictionary<string, List<SourceFieldDataTypeDetail>> sourceFieldDetailsByFieldId)
    {
        var column = AddColumn(
            context,
            table,
            id,
            sourceField.Name,
            sourceField.DataTypeId,
            sourceField.IsNullable,
            reservedColumnNames);

        foreach (var detail in GetGroup(sourceFieldDetailsByFieldId, sourceField.Id).OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            AddDetail(context, column, detail.Name, detail.Value);
        }

        return column;
    }

    private static TableColumn AddImplementationColumn(
        ConversionContext context,
        Table table,
        string id,
        string name,
        string metaDataTypeId,
        string isNullable,
        HashSet<string> reservedColumnNames,
        params (string Name, string Value)[] details)
    {
        var column = AddColumn(
            context,
            table,
            id,
            name,
            metaDataTypeId,
            isNullable,
            reservedColumnNames);

        foreach (var detail in details)
        {
            AddDetail(context, column, detail.Name, detail.Value);
        }

        return column;
    }

    private static TableColumn AddColumn(
        ConversionContext context,
        Table table,
        string id,
        string requestedName,
        string metaDataTypeId,
        string isNullable,
        HashSet<string> reservedColumnNames)
    {
        var actualName = ReserveColumnName(reservedColumnNames, requestedName);
        var ordinal = (context.SqlModel.TableColumnList.Count(row => ReferenceEquals(row.Table, table)) + 1).ToString(CultureInfo.InvariantCulture);

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

        context.SqlModel.TableColumnList.Add(column);
        return column;
    }

    private static void AddDetail(ConversionContext context, TableColumn tableColumn, string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        context.SqlModel.TableColumnDataTypeDetailList.Add(new TableColumnDataTypeDetail
        {
            Id = $"{tableColumn.Id}:Detail:{name}",
            Name = name,
            Value = value,
            TableColumnId = tableColumn.Id,
            TableColumn = tableColumn,
        });
    }

    private static void AddPrimaryKey(
        ConversionContext context,
        Table table,
        string id,
        string name,
        TableColumn tableColumn)
    {
        var primaryKey = new PrimaryKey
        {
            Id = id,
            Name = name,
            TableId = table.Id,
            Table = table,
        };
        context.SqlModel.PrimaryKeyList.Add(primaryKey);
        context.SqlModel.PrimaryKeyColumnList.Add(new PrimaryKeyColumn
        {
            Id = $"{id}:Column:{tableColumn.Id}",
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
        string id,
        string name,
        Table targetTable,
        IEnumerable<(TableColumn SourceColumn, TableColumn TargetColumn)> columnPairs)
    {
        var foreignKey = new ForeignKey
        {
            Id = id,
            Name = name,
            SourceTableId = sourceTable.Id,
            SourceTable = sourceTable,
            TargetTableId = targetTable.Id,
            TargetTable = targetTable,
        };
        context.SqlModel.ForeignKeyList.Add(foreignKey);

        var ordinal = 1;
        foreach (var (sourceColumn, targetColumn) in columnPairs)
        {
            context.SqlModel.ForeignKeyColumnList.Add(new ForeignKeyColumn
            {
                Id = $"{id}:Column:{ordinal}",
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

    private static string ReserveColumnName(HashSet<string> reservedColumnNames, string requestedName)
    {
        var actualName = requestedName;
        while (!reservedColumnNames.Add(actualName))
        {
            actualName += "_";
        }

        return actualName;
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
