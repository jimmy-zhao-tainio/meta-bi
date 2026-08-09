using Meta.Core.Domain;
using Meta.Core.Serialization;
using Dw = MetaDataWarehouse;
using Dwi = MetaDataWarehouseImplementation;
using MetaDataType;
using MetaDataType.Instance;
using MetaDataTypeConversion;
using MetaDataTypeConversion.Instance;
using SqlIndex = MetaSql.Index;
using MetaSql;

namespace MetaConvert.DataWarehouseToSql;

public static class DataWarehouseToSqlConverter
{
    public static async Task<InMemoryWorkspace> ConvertAsync(
        string dataWarehouseWorkspacePath,
        string pathToNewMetaSqlWorkspace,
        string implementationWorkspacePath,
        string databaseName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataWarehouseWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(pathToNewMetaSqlWorkspace);
        ArgumentException.ThrowIfNullOrWhiteSpace(implementationWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        var model = await Meta.Core.Serialization.TypedWorkspaceXmlSerializer.LoadAsync<Dw.MetaDataWarehouseModel>(
            dataWarehouseWorkspacePath,
            searchUpward: false,
            cancellationToken).ConfigureAwait(false);
        var implementation = await Meta.Core.Serialization.TypedWorkspaceXmlSerializer.LoadAsync<Dwi.MetaDataWarehouseImplementationModel>(
            implementationWorkspacePath,
            searchUpward: false,
            cancellationToken).ConfigureAwait(false);

        var metaSql = ConvertToMetaSql(model, implementation, databaseName);
        Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(metaSql, pathToNewMetaSqlWorkspace);
        var outputWorkspace = await XmlWorkspaceReader
            .OpenAsync(pathToNewMetaSqlWorkspace, cancellationToken)
            .ConfigureAwait(false);
        return outputWorkspace.State;
    }

    public static MetaSqlModel ConvertToMetaSql(
        Dw.MetaDataWarehouseModel model,
        Dwi.MetaDataWarehouseImplementationModel implementation,
        string databaseName)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(implementation);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        var context = ConversionContext.Create(databaseName, implementation);
        PopulateDimensions(model, context);
        PopulateBridges(model, context);
        PopulateFacts(model, context);

        return context.MetaSql;
    }

    private static void PopulateDimensions(Dw.MetaDataWarehouseModel model, ConversionContext context)
    {
        var impl = context.DimensionTableImplementation;
        var scdImpl = context.SlowlyChangingDimensionTableImplementation;
        var businessKeysByDimension = model.DimensionBusinessKeyList
            .GroupBy(row => row.Dimension.Id, StringComparer.Ordinal)
            .ToDictionary(row => row.Key, row => row.OrderBy(item => item.Id, StringComparer.Ordinal).ToArray(), StringComparer.Ordinal);
        var businessKeyPartsByKey = model.DimensionBusinessKeyPartList
            .GroupBy(row => row.DimensionBusinessKey.Id, StringComparer.Ordinal)
            .ToDictionary(row => row.Key, row => row.OrderByOrdinalThenId(item => item.Ordinal, item => item.Id).ToArray(), StringComparer.Ordinal);
        var attributesByDimension = model.DimensionAttributeList
            .GroupBy(row => row.Dimension.Id, StringComparer.Ordinal)
            .ToDictionary(row => row.Key, row => row.OrderByOrdinalThenId(item => item.Ordinal, item => item.Id).ToArray(), StringComparer.Ordinal);
        var scdByDimension = model.SlowlyChangingDimensionList
            .GroupBy(row => row.Dimension.Id, StringComparer.Ordinal)
            .ToDictionary(row => row.Key, row => row.OrderBy(item => item.Id, StringComparer.Ordinal).First(), StringComparer.Ordinal);

        foreach (var dimension in model.DimensionList.OrderBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = context.AddTable(
                impl.SchemaName,
                ApplyPattern(impl.TableNamePattern, ("Name", dimension.Name)),
                dimension.Id);
            context.DimensionTablesById.Add(dimension.Id, table);

            var surrogateColumn = context.AddColumn(
                table,
                ApplyPattern(impl.SurrogateKeyColumnName, ("Name", dimension.Name)),
                impl.SurrogateKeyDataTypeId,
                ordinal: 10,
                isNullable: "false");
            context.DimensionKeyColumnsById.Add(dimension.Id, surrogateColumn);

            var pk = context.AddPrimaryKey(table, ApplyPattern(impl.PrimaryKeyNamePattern, ("TableName", table.Name)), surrogateColumn);

            var ordinal = 100;
            var businessKeyAttributeIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var key in GetGroup(businessKeysByDimension, dimension.Id))
            {
                foreach (var part in GetGroup(businessKeyPartsByKey, key.Id))
                {
                    var attribute = part.DimensionAttribute;
                    businessKeyAttributeIds.Add(attribute.Id);
                    var column = context.AddColumn(
                        table,
                        ApplyPattern(impl.BusinessKeyColumnPattern, ("Name", dimension.Name), ("BusinessKeyName", key.Name), ("PartName", attribute.Name)),
                        attribute.DataTypeId,
                        ordinal,
                        isNullable: "false");
                    _ = column;
                    ordinal += 10;
                }
            }

            foreach (var attribute in GetGroup(attributesByDimension, dimension.Id)
                         .Where(attribute => !businessKeyAttributeIds.Contains(attribute.Id)))
            {
                var column = context.AddColumn(
                    table,
                    ApplyPattern(impl.AttributeColumnPattern, ("Name", attribute.Name), ("DimensionName", dimension.Name)),
                    attribute.DataTypeId,
                    ordinal,
                    attribute.IsNullable);
                _ = column;
                ordinal += 10;
            }

            if (scdByDimension.ContainsKey(dimension.Id))
            {
                context.AddColumn(table, scdImpl.EffectiveFromColumnName, scdImpl.EffectiveFromDataTypeId, ordinal, "false");
                ordinal += 10;
                context.AddColumn(table, scdImpl.EffectiveToColumnName, scdImpl.EffectiveToDataTypeId, ordinal, "true");
                ordinal += 10;
                context.AddColumn(table, scdImpl.CurrentFlagColumnName, scdImpl.CurrentFlagDataTypeId, ordinal, "false");
                ordinal += 10;
                if (!string.IsNullOrWhiteSpace(scdImpl.HashDiffColumnName) && !string.IsNullOrWhiteSpace(scdImpl.HashDiffDataTypeId))
                {
                    context.AddColumn(table, scdImpl.HashDiffColumnName, scdImpl.HashDiffDataTypeId, ordinal, "false");
                }
            }

            AddPlatformColumns(context, table, appliesTo: PlatformApplicability.Dimension);
            AddConfiguredIndexes(context, table, appliesTo: PlatformApplicability.Dimension, preferredColumn: surrogateColumn);

            _ = pk;
        }
    }

    private static void PopulateFacts(Dw.MetaDataWarehouseModel model, ConversionContext context)
    {
        var impl = context.FactTableImplementation;
        var factDimensionsByFact = model.FactDimensionList
            .GroupBy(row => row.Fact.Id, StringComparer.Ordinal)
            .ToDictionary(row => row.Key, row => row.OrderByOrdinalThenId(item => item.Ordinal, item => item.Id).ToArray(), StringComparer.Ordinal);
        var degenerateDimensionsByFact = model.DegenerateDimensionList
            .GroupBy(row => row.Fact.Id, StringComparer.Ordinal)
            .ToDictionary(row => row.Key, row => row.OrderByOrdinalThenId(item => item.Ordinal, item => item.Id).ToArray(), StringComparer.Ordinal);
        var measuresByFact = model.FactMeasureList
            .GroupBy(row => row.Fact.Id, StringComparer.Ordinal)
            .ToDictionary(row => row.Key, row => row.OrderByOrdinalThenId(item => item.Ordinal, item => item.Id).ToArray(), StringComparer.Ordinal);
        var periodicSnapshotsByFact = model.PeriodicSnapshotFactList
            .GroupBy(row => row.Fact.Id, StringComparer.Ordinal)
            .ToDictionary(row => row.Key, row => row.OrderBy(item => item.Id, StringComparer.Ordinal).First(), StringComparer.Ordinal);
        var accumulatingSnapshotsByFact = model.AccumulatingSnapshotFactList
            .GroupBy(row => row.Fact.Id, StringComparer.Ordinal)
            .ToDictionary(row => row.Key, row => row.OrderBy(item => item.Id, StringComparer.Ordinal).First(), StringComparer.Ordinal);
        var milestonesBySnapshot = model.AccumulatingSnapshotMilestoneList
            .GroupBy(row => row.AccumulatingSnapshotFact.Id, StringComparer.Ordinal)
            .ToDictionary(row => row.Key, row => row.OrderByOrdinalThenId(item => item.Ordinal, item => item.Id).ToArray(), StringComparer.Ordinal);

        foreach (var fact in model.FactList.OrderBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = context.AddTable(
                impl.SchemaName,
                ApplyPattern(impl.TableNamePattern, ("Name", fact.Name)),
                fact.Id);

            var ordinal = 10;
            var keyColumns = new List<TableColumn>();
            foreach (var factDimension in GetGroup(factDimensionsByFact, fact.Id))
            {
                if (!context.DimensionTablesById.TryGetValue(factDimension.Dimension.Id, out var targetTable) ||
                    !context.DimensionKeyColumnsById.TryGetValue(factDimension.Dimension.Id, out var targetColumn))
                {
                    throw new InvalidOperationException(
                        $"FactDimension '{factDimension.Id}' references dimension '{factDimension.Dimension.Id}', but no dimension table was projected.");
                }

                var isRequired = IsRequired(factDimension.IsRequired);
                var column = context.AddColumn(
                    table,
                    ApplyPattern(impl.DimensionKeyColumnPattern, ("RoleName", factDimension.RoleName), ("DimensionName", factDimension.Dimension.Name)),
                    targetColumn.MetaDataTypeId,
                    ordinal,
                    isRequired ? "false" : "true");
                CopyDetails(context.MetaSql.TableColumnDataTypeDetailList.Where(row => ReferenceEquals(row.TableColumn, targetColumn)).ToArray(), context, column);
                if (isRequired)
                {
                    keyColumns.Add(column);
                }

                var foreignKey = context.AddForeignKey(
                    table,
                    targetTable,
                    ApplyPattern(
                        impl.DimensionForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("RoleName", factDimension.RoleName),
                        ("DimensionName", factDimension.Dimension.Name)),
                    column,
                    targetColumn);

                _ = foreignKey;
                ordinal += 10;
            }

            foreach (var degenerate in GetGroup(degenerateDimensionsByFact, fact.Id))
            {
                keyColumns.Add(context.AddColumn(
                    table,
                    ApplyPattern(impl.DegenerateDimensionColumnPattern, ("Name", degenerate.Name), ("FactName", fact.Name)),
                    degenerate.DataTypeId,
                    ordinal,
                    isNullable: "false"));
                ordinal += 10;
            }

            if (periodicSnapshotsByFact.TryGetValue(fact.Id, out var periodicSnapshot))
            {
                var periodicImpl = context.PeriodicSnapshotFactTableImplementation;
                keyColumns.Add(context.AddColumn(table, periodicImpl.PeriodStartColumnName, periodicImpl.PeriodDataTypeId, ordinal, "false"));
                ordinal += 10;
                if (!string.IsNullOrWhiteSpace(periodicImpl.PeriodEndColumnName))
                {
                    context.AddColumn(table, periodicImpl.PeriodEndColumnName, periodicImpl.PeriodDataTypeId, ordinal, "true");
                    ordinal += 10;
                }

                _ = periodicSnapshot;
            }

            if (accumulatingSnapshotsByFact.TryGetValue(fact.Id, out var accumulatingSnapshot))
            {
                var accumulatingImpl = context.AccumulatingSnapshotFactTableImplementation;
                foreach (var milestone in GetGroup(milestonesBySnapshot, accumulatingSnapshot.Id))
                {
                    context.AddColumn(
                        table,
                        ApplyPattern(
                            accumulatingImpl.MilestoneDateColumnPattern,
                            ("DateRoleName", milestone.DateRoleName),
                            ("MilestoneName", milestone.Name)),
                        accumulatingImpl.MilestoneDateDataTypeId,
                        ordinal,
                        isNullable: "true");
                    ordinal += 10;
                }
            }

            foreach (var measure in GetGroup(measuresByFact, fact.Id))
            {
                var column = context.AddColumn(
                    table,
                    ApplyPattern(impl.MeasureColumnPattern, ("Name", measure.Name), ("FactName", fact.Name)),
                    measure.DataTypeId,
                    ordinal,
                    measure.IsNullable);
                _ = column;
                ordinal += 10;
            }

            AddPlatformColumns(context, table, appliesTo: PlatformApplicability.Fact);
            if (!string.IsNullOrWhiteSpace(impl.PrimaryKeyNamePattern) && keyColumns.Count > 0)
            {
                context.AddPrimaryKey(table, ApplyPattern(impl.PrimaryKeyNamePattern, ("TableName", table.Name)), keyColumns.ToArray());
            }

            AddConfiguredIndexes(context, table, appliesTo: PlatformApplicability.Fact, preferredColumn: keyColumns.FirstOrDefault());
        }
    }

    private static void PopulateBridges(Dw.MetaDataWarehouseModel model, ConversionContext context)
    {
        var impl = context.BridgeTableImplementation;
        var participantsByBridge = model.BridgeParticipantList
            .GroupBy(row => row.BridgeTable.Id, StringComparer.Ordinal)
            .ToDictionary(row => row.Key, row => row.OrderByOrdinalThenId(item => item.Ordinal, item => item.Id).ToArray(), StringComparer.Ordinal);
        var weightsByBridge = model.BridgeWeightList
            .GroupBy(row => row.BridgeTable.Id, StringComparer.Ordinal)
            .ToDictionary(row => row.Key, row => row.OrderBy(item => item.Id, StringComparer.Ordinal).ToArray(), StringComparer.Ordinal);

        foreach (var bridge in model.BridgeTableList.OrderBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = context.AddTable(
                impl.SchemaName,
                ApplyPattern(impl.TableNamePattern, ("Name", bridge.Name)),
                bridge.Id);

            var ordinal = 10;
            var keyColumns = new List<TableColumn>();
            foreach (var participant in GetGroup(participantsByBridge, bridge.Id))
            {
                if (!context.DimensionTablesById.TryGetValue(participant.Dimension.Id, out var targetTable) ||
                    !context.DimensionKeyColumnsById.TryGetValue(participant.Dimension.Id, out var targetColumn))
                {
                    throw new InvalidOperationException(
                        $"BridgeParticipant '{participant.Id}' references dimension '{participant.Dimension.Id}', but no dimension table was projected.");
                }

                var isRequired = IsRequired(participant.IsRequired);
                var column = context.AddColumn(
                    table,
                    ApplyPattern(impl.ParticipantKeyColumnPattern, ("RoleName", participant.RoleName), ("DimensionName", participant.Dimension.Name)),
                    targetColumn.MetaDataTypeId,
                    ordinal,
                    isRequired ? "false" : "true");
                CopyDetails(context.MetaSql.TableColumnDataTypeDetailList.Where(row => ReferenceEquals(row.TableColumn, targetColumn)).ToArray(), context, column);
                if (isRequired)
                {
                    keyColumns.Add(column);
                }

                context.AddForeignKey(
                    table,
                    targetTable,
                    ApplyPattern(
                        impl.ParticipantForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("RoleName", participant.RoleName),
                        ("DimensionName", participant.Dimension.Name)),
                    column,
                    targetColumn);
                ordinal += 10;
            }

            foreach (var weight in GetGroup(weightsByBridge, bridge.Id))
            {
                context.AddColumn(
                    table,
                    string.IsNullOrWhiteSpace(impl.WeightColumnName) ? weight.Name : impl.WeightColumnName,
                    string.IsNullOrWhiteSpace(impl.WeightDataTypeId) ? weight.DataTypeId : impl.WeightDataTypeId,
                    ordinal,
                    isNullable: "true");
                ordinal += 10;
            }

            AddPlatformColumns(context, table, appliesTo: PlatformApplicability.Bridge);
            if (keyColumns.Count > 0)
            {
                context.AddPrimaryKey(table, ApplyPattern(impl.PrimaryKeyNamePattern, ("TableName", table.Name)), keyColumns.ToArray());
            }

            context.BridgeTablesById.Add(bridge.Id, table);
            AddConfiguredIndexes(context, table, appliesTo: PlatformApplicability.Bridge, preferredColumn: keyColumns.FirstOrDefault());
        }
    }

    private static void AddPlatformColumns(ConversionContext context, Table table, PlatformApplicability appliesTo)
    {
        var ordinal = 9000;
        foreach (var platformColumn in context.PlatformColumns
                     .Where(row => Applies(row, appliesTo))
                     .OrderByOrdinalThenId(row => row.Ordinal, row => row.Id))
        {
            var column = context.AddColumn(
                table,
                platformColumn.ColumnName,
                platformColumn.DataTypeId,
                ordinal,
                platformColumn.IsNullable,
                platformColumn.DefaultExpressionSql);

            if (!string.IsNullOrWhiteSpace(platformColumn.Length))
            {
                context.AddDataTypeDetail(column, "Length", platformColumn.Length);
            }

            if (!string.IsNullOrWhiteSpace(platformColumn.Precision))
            {
                context.AddDataTypeDetail(column, "Precision", platformColumn.Precision);
            }

            if (!string.IsNullOrWhiteSpace(platformColumn.Scale))
            {
                context.AddDataTypeDetail(column, "Scale", platformColumn.Scale);
            }

            ordinal += 10;
        }
    }

    private static void AddConfiguredIndexes(
        ConversionContext context,
        Table table,
        PlatformApplicability appliesTo,
        TableColumn? preferredColumn)
    {
        if (preferredColumn == null)
        {
            return;
        }

        foreach (var implementation in context.IndexImplementations
                     .Where(row => Applies(row, appliesTo))
                     .OrderByOrdinalThenId(row => row.Ordinal, row => row.Id))
        {
            var name = ApplyPattern(
                implementation.NamePattern,
                ("TableName", table.Name),
                ("ColumnName", preferredColumn.Name));
            var index = new SqlIndex
            {
                Id = $"{table.Id}:index:{name}",
                Table = table,
                Name = name,
                IsUnique = NormalizeOptionalFalse(implementation.IsUnique),
                IsClustered = NormalizeOptionalFalse(implementation.IsClustered),
                FilterSql = implementation.FilterSql,
            };
            context.MetaSql.IndexList.Add(index);
            context.MetaSql.IndexColumnList.Add(new IndexColumn
            {
                Id = $"{index.Id}:column:1",
                Index = index,
                TableColumn = preferredColumn,
                Ordinal = "1",
            });
        }
    }

    private static void CopyDetails(IEnumerable<TableColumnDataTypeDetail> source, ConversionContext context, TableColumn target)
    {
        foreach (var detail in source.OrderBy(row => row.Id, StringComparer.Ordinal))
        {
            context.AddDataTypeDetail(target, detail.Name, detail.Value);
        }
    }

    private static bool Applies(Dwi.PlatformColumnImplementation implementation, PlatformApplicability appliesTo)
    {
        return appliesTo switch
        {
            PlatformApplicability.Dimension => IsTrue(implementation.AppliesToDimensionTables),
            PlatformApplicability.Fact => IsTrue(implementation.AppliesToFactTables),
            PlatformApplicability.Bridge => IsTrue(implementation.AppliesToBridgeTables),
            _ => false,
        };
    }

    private static bool Applies(Dwi.IndexImplementation implementation, PlatformApplicability appliesTo)
    {
        return appliesTo switch
        {
            PlatformApplicability.Dimension => IsTrue(implementation.AppliesToDimensionTables),
            PlatformApplicability.Fact => IsTrue(implementation.AppliesToFactTables),
            PlatformApplicability.Bridge => IsTrue(implementation.AppliesToBridgeTables),
            _ => false,
        };
    }

    private static IReadOnlyList<T> GetGroup<T>(IReadOnlyDictionary<string, T[]> groups, string key)
        where T : class
    {
        return groups.TryGetValue(key, out var rows)
            ? rows
            : Array.Empty<T>();
    }

    private static bool IsTrue(string? value)
    {
        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "1", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsRequired(string? value)
    {
        return string.IsNullOrWhiteSpace(value) || IsTrue(value);
    }

    private static string NormalizeOptionalFalse(string? value)
    {
        return string.Equals(value?.Trim(), "false", StringComparison.OrdinalIgnoreCase)
            ? string.Empty
            : value ?? string.Empty;
    }

    private static string ApplyPattern(string pattern, params (string Name, string Value)[] tokens)
    {
        var result = pattern;
        foreach (var token in tokens)
        {
            result = result.Replace("{" + token.Name + "}", token.Value, StringComparison.Ordinal);
        }

        return result;
    }

    private enum PlatformApplicability
    {
        Dimension,
        Fact,
        Bridge,
    }

    private sealed class ConversionContext
    {
        private readonly Dictionary<string, Schema> schemasByName;
        private readonly HashSet<string> tableIds = new(StringComparer.Ordinal);
        private readonly HashSet<string> columnIds = new(StringComparer.Ordinal);

        private ConversionContext(
            MetaSqlModel metaSql,
            Database database,
            Dictionary<string, Schema> schemasByName,
            Dwi.MetaDataWarehouseImplementationModel implementation)
        {
            MetaSql = metaSql;
            Database = database;
            this.schemasByName = schemasByName;
            DimensionTableImplementation = RequireSingle(implementation.DimensionTableImplementationList, "DimensionTableImplementation");
            SlowlyChangingDimensionTableImplementation = RequireSingle(implementation.SlowlyChangingDimensionTableImplementationList, "SlowlyChangingDimensionTableImplementation");
            FactTableImplementation = RequireSingle(implementation.FactTableImplementationList, "FactTableImplementation");
            PeriodicSnapshotFactTableImplementation = RequireSingle(implementation.PeriodicSnapshotFactTableImplementationList, "PeriodicSnapshotFactTableImplementation");
            AccumulatingSnapshotFactTableImplementation = RequireSingle(implementation.AccumulatingSnapshotFactTableImplementationList, "AccumulatingSnapshotFactTableImplementation");
            BridgeTableImplementation = RequireSingle(implementation.BridgeTableImplementationList, "BridgeTableImplementation");
            PlatformColumns = implementation.PlatformColumnImplementationList.ToArray();
            IndexImplementations = implementation.IndexImplementationList.ToArray();
        }

        public MetaSqlModel MetaSql { get; }
        public Database Database { get; }
        public Dwi.DimensionTableImplementation DimensionTableImplementation { get; }
        public Dwi.SlowlyChangingDimensionTableImplementation SlowlyChangingDimensionTableImplementation { get; }
        public Dwi.FactTableImplementation FactTableImplementation { get; }
        public Dwi.PeriodicSnapshotFactTableImplementation PeriodicSnapshotFactTableImplementation { get; }
        public Dwi.AccumulatingSnapshotFactTableImplementation AccumulatingSnapshotFactTableImplementation { get; }
        public Dwi.BridgeTableImplementation BridgeTableImplementation { get; }
        public IReadOnlyList<Dwi.PlatformColumnImplementation> PlatformColumns { get; }
        public IReadOnlyList<Dwi.IndexImplementation> IndexImplementations { get; }
        public Dictionary<string, Table> DimensionTablesById { get; } = new(StringComparer.Ordinal);
        public Dictionary<string, TableColumn> DimensionKeyColumnsById { get; } = new(StringComparer.Ordinal);
        public Dictionary<string, Table> BridgeTablesById { get; } = new(StringComparer.Ordinal);

        public static ConversionContext Create(string databaseName, Dwi.MetaDataWarehouseImplementationModel implementation)
        {
            var metaSql = MetaSqlModel.CreateEmpty();
            var database = new Database
            {
                Id = databaseName,
                Name = databaseName,
            };
            metaSql.DatabaseList.Add(database);

            var schemaNames = new[]
                {
                    RequireSingle(implementation.DimensionTableImplementationList, "DimensionTableImplementation").SchemaName,
                    RequireSingle(implementation.FactTableImplementationList, "FactTableImplementation").SchemaName,
                    RequireSingle(implementation.BridgeTableImplementationList, "BridgeTableImplementation").SchemaName,
                }
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(row => row, StringComparer.OrdinalIgnoreCase)
                .ThenBy(row => row, StringComparer.Ordinal)
                .ToArray();

            var schemasByName = new Dictionary<string, Schema>(StringComparer.OrdinalIgnoreCase);
            foreach (var schemaName in schemaNames)
            {
                var schema = new Schema
                {
                    Id = $"{database.Id}.{schemaName}",
                    Name = schemaName,
                    Database = database,
                };
                metaSql.SchemaList.Add(schema);
                schemasByName.Add(schemaName, schema);
            }

            return new ConversionContext(metaSql, database, schemasByName, implementation);
        }

        public Table AddTable(string schemaName, string tableName, string sourceId)
        {
            var schema = schemasByName.TryGetValue(schemaName, out var found)
                ? found
                : throw new InvalidOperationException($"Implementation references schema '{schemaName}', but it was not initialized.");
            var tableId = $"{Database.Id}.{schema.Name}.{tableName}";
            if (!tableIds.Add(tableId))
            {
                throw new InvalidOperationException($"Duplicate projected table id '{tableId}' from source '{sourceId}'.");
            }

            var table = new Table
            {
                Id = tableId,
                Schema = schema,
                Name = tableName,
            };
            MetaSql.TableList.Add(table);
            return table;
        }

        public TableColumn AddColumn(
            Table table,
            string columnName,
            string dataTypeId,
            int ordinal,
            string? isNullable,
            string? defaultExpressionSql = null)
        {
            var columnId = $"{table.Id}.{columnName}";
            if (!columnIds.Add(columnId))
            {
                throw new InvalidOperationException($"Duplicate projected column id '{columnId}'.");
            }

            var loweredType = SqlServerTypeLowering.Default.LowerRequired(dataTypeId);
            var column = new TableColumn
            {
                Id = columnId,
                Table = table,
                Name = columnName,
                MetaDataTypeId = loweredType.DataTypeId,
                Ordinal = (MetaSql.TableColumnList.Count(row => ReferenceEquals(row.Table, table)) + 1).ToString(),
                IsNullable = string.IsNullOrWhiteSpace(isNullable) ? "true" : isNullable,
                DefaultExpressionSql = defaultExpressionSql,
            };
            MetaSql.TableColumnList.Add(column);
            foreach (var detail in loweredType.DefaultDetails)
            {
                AddDataTypeDetail(column, detail.Name, detail.Value);
            }

            return column;
        }

        public void AddDataTypeDetail(TableColumn column, string name, string value)
        {
            var id = $"{column.Id}:type-detail:{name}";
            if (MetaSql.TableColumnDataTypeDetailList.Any(row => string.Equals(row.Id, id, StringComparison.Ordinal)))
            {
                return;
            }

            MetaSql.TableColumnDataTypeDetailList.Add(new TableColumnDataTypeDetail
            {
                Id = id,
                TableColumn = column,
                Name = name,
                Value = value,
            });
        }

        public PrimaryKey AddPrimaryKey(Table table, string name, params TableColumn[] columns)
        {
            var primaryKey = new PrimaryKey
            {
                Id = $"{table.Id}:primary-key",
                Table = table,
                Name = name,
                IsClustered = string.Empty,
            };
            MetaSql.PrimaryKeyList.Add(primaryKey);

            for (var i = 0; i < columns.Length; i++)
            {
                MetaSql.PrimaryKeyColumnList.Add(new PrimaryKeyColumn
                {
                    Id = $"{primaryKey.Id}:column:{i + 1}",
                    PrimaryKey = primaryKey,
                    TableColumn = columns[i],
                    Ordinal = (i + 1).ToString(),
                });
            }

            return primaryKey;
        }

        public ForeignKey AddForeignKey(
            Table sourceTable,
            Table targetTable,
            string name,
            TableColumn sourceColumn,
            TableColumn targetColumn)
        {
            var foreignKey = new ForeignKey
            {
                Id = $"{sourceTable.Id}:foreign-key:{name}",
                SourceTable = sourceTable,
                TargetTable = targetTable,
                Name = name,
            };
            MetaSql.ForeignKeyList.Add(foreignKey);
            MetaSql.ForeignKeyColumnList.Add(new ForeignKeyColumn
            {
                Id = $"{foreignKey.Id}:column:1",
                ForeignKey = foreignKey,
                SourceColumn = sourceColumn,
                TargetColumn = targetColumn,
                Ordinal = "1",
            });
            return foreignKey;
        }

        private static T RequireSingle<T>(IReadOnlyCollection<T> rows, string entityName)
        {
            return rows.Count switch
            {
                1 => rows.First(),
                0 => throw new InvalidOperationException($"MetaDataWarehouseImplementation requires one {entityName} row."),
                _ => throw new InvalidOperationException($"MetaDataWarehouseImplementation requires one {entityName} row, but found {rows.Count}."),
            };
        }
    }

    private sealed class SqlServerTypeLowering
    {
        private const string DirectConversionImplementationId = "MetaDataTypeConversion:implementation:direct";
        private const string MetaTypeSystemId = "Meta";
        private const string SqlServerTypeSystemId = "SqlServer";

        public static readonly SqlServerTypeLowering Default = Create(
            MetaDataTypeInstance.Default,
            MetaDataTypeConversionInstance.Default);

        private readonly IReadOnlyDictionary<string, DataType> dataTypesById;
        private readonly IReadOnlyDictionary<string, string> sqlServerTypesByLogicalTypeId;

        private SqlServerTypeLowering(
            IReadOnlyDictionary<string, DataType> dataTypesById,
            IReadOnlyDictionary<string, string> sqlServerTypesByLogicalTypeId)
        {
            this.dataTypesById = dataTypesById;
            this.sqlServerTypesByLogicalTypeId = sqlServerTypesByLogicalTypeId;
        }

        private static SqlServerTypeLowering Create(
            MetaDataTypeModel dataTypeModel,
            MetaDataTypeConversionModel conversionModel)
        {
            var dataTypesById = dataTypeModel.DataTypeList.ToDictionary(row => row.Id, StringComparer.Ordinal);
            var sqlServerTypesByLogicalTypeId = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var mapping in conversionModel.DataTypeMappingList
                         .Where(row => string.Equals(row.ConversionImplementation.Id, DirectConversionImplementationId, StringComparison.Ordinal))
                         .Where(row => IsSqlServerType(dataTypesById, row.TargetDataTypeId)))
            {
                if (sqlServerTypesByLogicalTypeId.TryGetValue(mapping.SourceDataTypeId, out var existingTargetDataTypeId))
                {
                    if (!string.Equals(existingTargetDataTypeId, mapping.TargetDataTypeId, StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            $"Data warehouse logical type '{mapping.SourceDataTypeId}' has conflicting sanctioned SQL Server mappings.");
                    }

                    continue;
                }

                sqlServerTypesByLogicalTypeId.Add(mapping.SourceDataTypeId, mapping.TargetDataTypeId);
            }

            return new SqlServerTypeLowering(dataTypesById, sqlServerTypesByLogicalTypeId);
        }

        public LoweredSqlServerType LowerRequired(string sourceTypeId)
        {
            if (string.IsNullOrWhiteSpace(sourceTypeId))
            {
                throw new InvalidOperationException("Data warehouse column type id is required.");
            }

            if (!dataTypesById.TryGetValue(sourceTypeId, out var sourceType))
            {
                throw new InvalidOperationException(
                    $"Data warehouse column type '{sourceTypeId}' is not sanctioned in MetaDataType.");
            }

            string sqlServerTypeId;
            if (string.Equals(sourceType.DataTypeSystem.Id, SqlServerTypeSystemId, StringComparison.Ordinal))
            {
                sqlServerTypeId = sourceTypeId;
            }
            else if (string.Equals(sourceType.DataTypeSystem.Id, MetaTypeSystemId, StringComparison.Ordinal))
            {
                if (!sqlServerTypesByLogicalTypeId.TryGetValue(sourceTypeId, out sqlServerTypeId!))
                {
                    throw new InvalidOperationException(
                        $"Data warehouse logical type '{sourceTypeId}' has no sanctioned direct SqlServer lowering.");
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"Data warehouse column type '{sourceTypeId}' must belong to DataTypeSystem '{MetaTypeSystemId}' or '{SqlServerTypeSystemId}'.");
            }

            if (!dataTypesById.TryGetValue(sqlServerTypeId, out var sqlServerType) ||
                !string.Equals(sqlServerType.DataTypeSystem.Id, SqlServerTypeSystemId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Data warehouse column type '{sourceTypeId}' lowered to non-SqlServer type '{sqlServerTypeId}'.");
            }

            return new LoweredSqlServerType(sqlServerTypeId, GetDefaultDetails(sqlServerType.Name));
        }

        private static bool IsSqlServerType(IReadOnlyDictionary<string, DataType> dataTypesById, string dataTypeId)
        {
            return dataTypesById.TryGetValue(dataTypeId, out var dataType) &&
                   string.Equals(dataType.DataTypeSystem.Id, SqlServerTypeSystemId, StringComparison.Ordinal);
        }

        private static IReadOnlyList<(string Name, string Value)> GetDefaultDetails(string sqlServerTypeName)
        {
            return sqlServerTypeName.ToLowerInvariant() switch
            {
                "char" or "varchar" or "nchar" or "nvarchar" => [("Length", "256")],
                "binary" or "varbinary" => [("Length", "32")],
                "decimal" or "numeric" => [("Precision", "18"), ("Scale", "4")],
                "time" or "datetime2" or "datetimeoffset" => [("Precision", "7")],
                _ => [],
            };
        }
    }

    private sealed record LoweredSqlServerType(
        string DataTypeId,
        IReadOnlyList<(string Name, string Value)> DefaultDetails);
}

internal static class DataWarehouseToSqlOrderingExtensions
{
    public static IOrderedEnumerable<T> OrderByOrdinalThenId<T>(
        this IEnumerable<T> source,
        Func<T, string?> ordinalSelector,
        Func<T, string> idSelector)
    {
        return source
            .OrderBy(row => ParseOrdinal(ordinalSelector(row)))
            .ThenBy(idSelector, StringComparer.Ordinal);
    }

    private static int ParseOrdinal(string? value)
    {
        return int.TryParse(value, out var result)
            ? result
            : int.MaxValue;
    }
}
