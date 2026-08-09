using Tom = Microsoft.AnalysisServices.Tabular;
using AnalysisServices = Microsoft.AnalysisServices;
using System.Globalization;

namespace MetaTabular.Core.Deploy;

public sealed class MetaTabularDeployService
{
    public Task<MetaTabularDeployResult> DeployAsync(MetaTabularDeployRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Server);

        var workspacePath = Path.GetFullPath(request.WorkspacePath);
        var model = Meta.Core.Serialization.TypedWorkspaceModelMapper.Load<MetaTabularModel>(workspacePath, searchUpward: false);
        var root = RequireSingleModel(model);
        var databaseName = string.IsNullOrWhiteSpace(request.DatabaseName)
            ? root.Name
            : request.DatabaseName.Trim();
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("Tabular deploy requires a database name. Set TabularModel.Name or pass --database-name.");
        }

        using var server = new Tom.Server();
        server.Connect($"Data Source={request.Server}");

        var existing = FindDatabase(server, databaseName);
        if (existing != null)
        {
            if (!request.DropExisting)
            {
                var processingNote = request.Process
                    ? " Processing an existing tabular database requires --drop-existing so deploy uses the safe drop, create, full-process sequence."
                    : string.Empty;
                throw new InvalidOperationException($"Tabular database '{databaseName}' already exists. Pass --drop-existing to drop it before deploying.{processingNote}");
            }

            existing.Drop();
            server.Refresh();
        }

        var database = BuildDatabase(model, root, databaseName);
        server.Databases.Add(database);
        database.Update(AnalysisServices.UpdateOptions.ExpandFull);
        if (request.Process)
        {
            ProcessDatabase(database, databaseName);
        }

        return Task.FromResult(new MetaTabularDeployResult
        {
            WorkspacePath = workspacePath,
            Server = request.Server,
            DatabaseName = databaseName,
            DropExisting = request.DropExisting,
            Processed = request.Process,
            TableCount = model.TabularTableList.Count + model.TabularCalculationGroupList.Count,
            ColumnCount = model.TabularColumnList.Count,
            MeasureCount = model.TabularMeasureList.Count,
            RelationshipCount = model.TabularRelationshipList.Count,
        });
    }

    private static Tom.Database BuildDatabase(MetaTabularModel source, TabularModel root, string databaseName)
    {
        var cultureName = string.IsNullOrWhiteSpace(root.DefaultCulture) ? "en-US" : root.DefaultCulture;
        var culture = CultureInfo.GetCultureInfo(cultureName);
        var database = new Tom.Database
        {
            ID = databaseName,
            Name = databaseName,
            Description = root.Description,
            Language = culture.LCID,
            CompatibilityLevel = ParseInt(root.CompatibilityLevel, 1500, "TabularModel.CompatibilityLevel"),
            StorageEngineUsed = AnalysisServices.StorageEngineUsed.TabularMetadata,
            Model = new Tom.Model
            {
                Name = root.Name,
                Description = root.Description,
                Culture = culture.Name,
                DiscourageImplicitMeasures = source.TabularCalculationGroupList.Any(row => ReferenceEquals(row.TabularModel, root)),
            },
        };

        if (!string.IsNullOrWhiteSpace(root.Collation))
        {
            database.Collation = root.Collation;
            database.Model.Collation = root.Collation;
        }

        if (!string.IsNullOrWhiteSpace(root.DefaultDataView))
        {
            database.Model.DefaultDataView = ParseEnum<Tom.DataViewType>(root.DefaultDataView, "TabularModel.DefaultDataView");
        }

        var dataSources = AddDataSources(database.Model, source, root);
        var tables = AddTables(database.Model, source, root);
        var columns = AddColumns(source, tables);
        AddSortByColumns(source, columns);
        var hierarchies = AddHierarchies(source, tables, columns);
        AddPartitions(source, tables, dataSources);
        var measures = AddMeasures(source, tables);
        var kpis = AddKpis(source, measures);
        AddRelationships(database.Model, source, tables, columns);
        var calculationGroups = AddCalculationGroups(database.Model, source, root);
        var perspectives = AddPerspectives(database.Model, source, root, tables, columns, hierarchies, measures, kpis, calculationGroups);
        var cultures = AddCultures(database.Model, source, root);
        AddTranslations(source, cultures, tables, columns, hierarchies, measures, kpis, perspectives);
        AddRoles(database.Model, source, root, tables, columns);
        return database;
    }

    private static void ProcessDatabase(Tom.Database database, string databaseName)
    {
        try
        {
            database.Model.RequestRefresh(Tom.RefreshType.Full);
            database.Model.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Deployed tabular database '{databaseName}', but processing failed: {ex.Message}", ex);
        }
    }

    private static Dictionary<TabularDataSource, Tom.DataSource> AddDataSources(
        Tom.Model target,
        MetaTabularModel source,
        TabularModel root)
    {
        var result = new Dictionary<TabularDataSource, Tom.DataSource>();
        foreach (var row in source.TabularDataSourceList.Where(row => ReferenceEquals(row.TabularModel, root)))
        {
            var dataSource = new Tom.ProviderDataSource
            {
                Name = row.Name,
                Description = row.Description,
                ConnectionString = ResolveConnectionString(row.ConnectionReference),
                ImpersonationMode = Tom.ImpersonationMode.ImpersonateServiceAccount,
            };
            dataSource.Provider = MapProvider(row.Provider);

            target.DataSources.Add(dataSource);
            result[row] = dataSource;
        }

        return result;
    }

    private static Dictionary<TabularTable, Tom.Table> AddTables(
        Tom.Model target,
        MetaTabularModel source,
        TabularModel root)
    {
        var result = new Dictionary<TabularTable, Tom.Table>();
        foreach (var row in source.TabularTableList.Where(row => ReferenceEquals(row.TabularModel, root)))
        {
            var table = new Tom.Table
            {
                Name = row.Name,
                Description = row.Description,
                DataCategory = row.DataCategory,
                IsHidden = ParseBool(row.IsHidden),
            };
            target.Tables.Add(table);
            result[row] = table;
        }

        return result;
    }

    private static Dictionary<TabularColumn, Tom.Column> AddColumns(
        MetaTabularModel source,
        IReadOnlyDictionary<TabularTable, Tom.Table> tables)
    {
        var result = new Dictionary<TabularColumn, Tom.Column>();
        foreach (var row in source.TabularColumnList
                     .Where(row => tables.ContainsKey(row.TabularTable))
                     .OrderBy(row => ParseInt(row.Ordinal, int.MaxValue, "TabularColumn.Ordinal")))
        {
            Tom.Column column = string.IsNullOrWhiteSpace(row.Expression)
                ? new Tom.DataColumn
                {
                    SourceColumn = string.IsNullOrWhiteSpace(row.SourceName) ? row.Name : row.SourceName,
                    DataType = MapTabularDataType(row.DataTypeId),
                }
                : new Tom.CalculatedColumn
                {
                    Expression = row.Expression,
                    DataType = MapTabularDataType(row.DataTypeId),
                };

            column.Name = row.Name;
            column.Description = row.Description;
            column.DisplayOrdinal = ParseInt(row.Ordinal, 0, "TabularColumn.Ordinal");
            column.DataCategory = row.DataCategory;
            column.FormatString = row.FormatString;
            column.IsHidden = ParseBool(row.IsHidden);
            column.IsKey = ParseBool(row.IsKey);
            column.IsNullable = ParseNullableBool(row.IsNullable) ?? true;
            column.SummarizeBy = MapSummarizeBy(row.SummarizeBy);

            tables[row.TabularTable].Columns.Add(column);
            result[row] = column;
        }

        return result;
    }

    private static void AddSortByColumns(
        MetaTabularModel source,
        IReadOnlyDictionary<TabularColumn, Tom.Column> columns)
    {
        var assigned = new HashSet<TabularColumn>();
        foreach (var row in source.TabularSortByColumnList)
        {
            if (!columns.TryGetValue(row.SourceColumn, out var targetSourceColumn))
            {
                throw new InvalidOperationException($"TabularSortByColumn '{row.Id}' references source column '{row.SourceColumn.Id}' that was not emitted to the target model.");
            }

            if (!columns.TryGetValue(row.SortColumn, out var targetSortColumn))
            {
                throw new InvalidOperationException($"TabularSortByColumn '{row.Id}' references sort column '{row.SortColumn.Id}' that was not emitted to the target model.");
            }

            if (!ReferenceEquals(row.SourceColumn.TabularTable, row.SortColumn.TabularTable))
            {
                throw new InvalidOperationException($"TabularSortByColumn '{row.Id}' must reference columns in the same TabularTable.");
            }

            if (!assigned.Add(row.SourceColumn))
            {
                throw new InvalidOperationException($"TabularColumn '{row.SourceColumn.Id}' has more than one TabularSortByColumn row.");
            }

            targetSourceColumn.SortByColumn = targetSortColumn;
        }
    }

    private static Dictionary<TabularHierarchy, Tom.Hierarchy> AddHierarchies(
        MetaTabularModel source,
        IReadOnlyDictionary<TabularTable, Tom.Table> tables,
        IReadOnlyDictionary<TabularColumn, Tom.Column> columns)
    {
        var result = new Dictionary<TabularHierarchy, Tom.Hierarchy>();
        foreach (var row in source.TabularHierarchyList)
        {
            if (!tables.TryGetValue(row.TabularTable, out var targetTable))
            {
                throw new InvalidOperationException($"TabularHierarchy '{row.Id}' references table '{row.TabularTable.Id}' that was not emitted to the target model.");
            }

            var hierarchy = new Tom.Hierarchy
            {
                Name = row.Name,
                Description = row.Description,
                DisplayFolder = row.DisplayFolder,
                IsHidden = ParseBool(row.IsHidden),
            };

            var levels = source.TabularHierarchyLevelList
                .Where(level => ReferenceEquals(level.TabularHierarchy, row))
                .OrderBy(level => ParseInt(level.Ordinal, int.MaxValue, "TabularHierarchyLevel.Ordinal"))
                .ToArray();
            if (levels.Length == 0)
            {
                throw new InvalidOperationException($"TabularHierarchy '{row.Id}' requires at least one TabularHierarchyLevel row.");
            }

            var ordinals = new HashSet<int>();
            for (var levelIndex = 0; levelIndex < levels.Length; levelIndex++)
            {
                var level = levels[levelIndex];
                if (!columns.TryGetValue(level.TabularColumn, out var targetColumn))
                {
                    throw new InvalidOperationException($"TabularHierarchyLevel '{level.Id}' references column '{level.TabularColumn.Id}' that was not emitted to the target model.");
                }

                if (!ReferenceEquals(level.TabularColumn.TabularTable, row.TabularTable))
                {
                    throw new InvalidOperationException($"TabularHierarchyLevel '{level.Id}' must reference a column in the hierarchy table.");
                }

                var modeledOrdinal = ParseInt(level.Ordinal, 0, "TabularHierarchyLevel.Ordinal");
                if (!ordinals.Add(modeledOrdinal))
                {
                    throw new InvalidOperationException($"TabularHierarchy '{row.Id}' has more than one level with ordinal '{modeledOrdinal}'.");
                }

                hierarchy.Levels.Add(new Tom.Level
                {
                    Name = level.Name,
                    Ordinal = levelIndex,
                    Column = targetColumn,
                });
            }

            targetTable.Hierarchies.Add(hierarchy);
            result[row] = hierarchy;
        }

        return result;
    }

    private static void AddPartitions(
        MetaTabularModel source,
        IReadOnlyDictionary<TabularTable, Tom.Table> tables,
        IReadOnlyDictionary<TabularDataSource, Tom.DataSource> dataSources)
    {
        var partitionsByTable = source.TabularPartitionList
            .Where(row => tables.ContainsKey(row.TabularTable))
            .GroupBy(row => row.TabularTable)
            .ToDictionary(group => group.Key, group => group.OrderBy(row => ParseInt(row.Ordinal, int.MaxValue, "TabularPartition.Ordinal")).ToArray());

        foreach (var tablePair in tables)
        {
            var rows = partitionsByTable.GetValueOrDefault(tablePair.Key);
            if (rows == null || rows.Length == 0)
            {
                tablePair.Value.Partitions.Add(new Tom.Partition
                {
                    Name = $"{tablePair.Value.Name} Partition",
                    Mode = Tom.ModeType.Import,
                });
                continue;
            }

            foreach (var row in rows)
            {
                if (!string.IsNullOrWhiteSpace(row.Expression) && row.TabularDataSource == null)
                {
                    throw new InvalidOperationException($"TabularPartition '{row.Id}' defines Expression but no TabularDataSource.");
                }

                if (string.IsNullOrWhiteSpace(row.Expression) && row.TabularDataSource != null)
                {
                    throw new InvalidOperationException($"TabularPartition '{row.Id}' defines TabularDataSource but no Expression to realize as a query partition source.");
                }

                Tom.DataSource? dataSource = null;
                if (row.TabularDataSource != null && !dataSources.TryGetValue(row.TabularDataSource, out dataSource))
                {
                    throw new InvalidOperationException($"TabularPartition '{row.Id}' references data source '{row.TabularDataSource.Id}' that was not emitted to the target model.");
                }

                var partition = new Tom.Partition
                {
                    Name = row.Name,
                    Description = row.Description,
                    Mode = MapMode(row.Mode),
                };
                if (!string.IsNullOrWhiteSpace(row.Expression))
                {
                    partition.Source = new Tom.QueryPartitionSource
                    {
                        DataSource = dataSource,
                        Query = row.Expression,
                    };
                }

                tablePair.Value.Partitions.Add(partition);
            }
        }
    }

    private static Dictionary<TabularMeasure, Tom.Measure> AddMeasures(
        MetaTabularModel source,
        IReadOnlyDictionary<TabularTable, Tom.Table> tables)
    {
        var result = new Dictionary<TabularMeasure, Tom.Measure>();
        foreach (var row in source.TabularMeasureList.Where(row => tables.ContainsKey(row.TabularTable)))
        {
            var measure = new Tom.Measure
            {
                Name = row.Name,
                Description = row.Description,
                Expression = string.IsNullOrWhiteSpace(row.Expression) ? "BLANK()" : row.Expression,
                FormatString = row.FormatString,
                DisplayFolder = row.DisplayFolder,
                IsHidden = ParseBool(row.IsHidden),
            };
            tables[row.TabularTable].Measures.Add(measure);
            result[row] = measure;
        }

        return result;
    }

    private static Dictionary<TabularKpi, Tom.KPI> AddKpis(
        MetaTabularModel source,
        IReadOnlyDictionary<TabularMeasure, Tom.Measure> measures)
    {
        var result = new Dictionary<TabularKpi, Tom.KPI>();
        var baseMeasures = new HashSet<TabularMeasure>();
        foreach (var row in source.TabularKpiList)
        {
            if (!measures.TryGetValue(row.BaseMeasure, out var baseMeasure))
            {
                throw new InvalidOperationException($"TabularKpi '{row.Id}' references base measure '{row.BaseMeasure.Id}' that was not emitted to the target model.");
            }

            if (!baseMeasures.Add(row.BaseMeasure))
            {
                throw new InvalidOperationException($"TabularMeasure '{row.BaseMeasure.Id}' has more than one TabularKpi row.");
            }

            var targetExpression = row.TargetExpression;
            if (row.TargetMeasure != null)
            {
                if (!measures.TryGetValue(row.TargetMeasure, out var targetMeasure))
                {
                    throw new InvalidOperationException($"TabularKpi '{row.Id}' references target measure '{row.TargetMeasure.Id}' that was not emitted to the target model.");
                }

                var targetMeasureExpression = ToDaxMeasureReference(targetMeasure.Name);
                if (!string.IsNullOrWhiteSpace(row.TargetExpression) &&
                    !string.Equals(row.TargetExpression.Trim(), targetMeasureExpression, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException($"TabularKpi '{row.Id}' defines both TargetExpression and TargetMeasure. Use one target expression source.");
                }

                targetExpression = targetMeasureExpression;
            }

            var kpi = new Tom.KPI
            {
                Description = row.Description,
                TargetExpression = targetExpression,
                StatusExpression = row.StatusExpression,
                StatusGraphic = row.StatusGraphic,
                TrendExpression = row.TrendExpression,
                TrendGraphic = row.TrendGraphic,
            };
            baseMeasure.KPI = kpi;
            result[row] = kpi;
        }

        return result;
    }

    private static void AddRelationships(
        Tom.Model target,
        MetaTabularModel source,
        IReadOnlyDictionary<TabularTable, Tom.Table> tables,
        IReadOnlyDictionary<TabularColumn, Tom.Column> columns)
    {
        foreach (var row in source.TabularRelationshipList)
        {
            RequireTable(tables, row.FromTable, $"TabularRelationship '{row.Id}' from table");
            RequireTable(tables, row.ToTable, $"TabularRelationship '{row.Id}' to table");
            var fromColumn = RequireColumn(columns, row.FromColumn, $"TabularRelationship '{row.Id}' from column");
            var toColumn = RequireColumn(columns, row.ToColumn, $"TabularRelationship '{row.Id}' to column");

            if (!ReferenceEquals(row.FromColumn.TabularTable, row.FromTable))
            {
                throw new InvalidOperationException($"TabularRelationship '{row.Id}' FromColumn must belong to FromTable.");
            }

            if (!ReferenceEquals(row.ToColumn.TabularTable, row.ToTable))
            {
                throw new InvalidOperationException($"TabularRelationship '{row.Id}' ToColumn must belong to ToTable.");
            }

            target.Relationships.Add(new Tom.SingleColumnRelationship
            {
                Name = row.Name,
                FromColumn = fromColumn,
                ToColumn = toColumn,
                FromCardinality = MapFromCardinality(row.Cardinality),
                ToCardinality = MapToCardinality(row.Cardinality),
                CrossFilteringBehavior = MapCrossFiltering(row.CrossFilterDirection),
                IsActive = ParseNullableBool(row.IsActive) ?? true,
                RelyOnReferentialIntegrity = ParseBool(row.IsRequired),
            });
        }
    }

    private static Dictionary<TabularCalculationGroup, Tom.Table> AddCalculationGroups(Tom.Model target, MetaTabularModel source, TabularModel root)
    {
        var result = new Dictionary<TabularCalculationGroup, Tom.Table>();
        foreach (var row in source.TabularCalculationGroupList.Where(row => ReferenceEquals(row.TabularModel, root)))
        {
            var table = new Tom.Table
            {
                Name = row.Name,
                Description = row.Description,
                IsHidden = true,
                CalculationGroup = new Tom.CalculationGroup
                {
                    Precedence = ParseInt(row.Precedence, 0, "TabularCalculationGroup.Precedence"),
                    Description = row.Description,
                },
            };
            table.Partitions.Add(new Tom.Partition
            {
                Name = $"{row.Name} Partition",
                Mode = Tom.ModeType.Import,
                Source = new Tom.CalculationGroupSource(),
            });
            table.Columns.Add(new Tom.DataColumn
            {
                Name = "Name",
                DataType = Tom.DataType.String,
                SourceColumn = "Name",
                IsHidden = true,
            });

            foreach (var item in source.TabularCalculationItemList
                         .Where(item => ReferenceEquals(item.TabularCalculationGroup, row))
                         .OrderBy(item => ParseInt(item.Ordinal, int.MaxValue, "TabularCalculationItem.Ordinal")))
            {
                var calculationItem = new Tom.CalculationItem
                {
                    Name = item.Name,
                    Description = item.Description,
                    Expression = item.Expression,
                    Ordinal = ParseInt(item.Ordinal, 0, "TabularCalculationItem.Ordinal"),
                };
                if (!string.IsNullOrWhiteSpace(item.FormatStringExpression))
                {
                    calculationItem.FormatStringDefinition = new Tom.FormatStringDefinition
                    {
                        Expression = item.FormatStringExpression,
                    };
                }

                table.CalculationGroup.CalculationItems.Add(calculationItem);
            }

            target.Tables.Add(table);
            result[row] = table;
        }

        return result;
    }

    private static Dictionary<TabularPerspective, Tom.Perspective> AddPerspectives(
        Tom.Model target,
        MetaTabularModel source,
        TabularModel root,
        IReadOnlyDictionary<TabularTable, Tom.Table> tables,
        IReadOnlyDictionary<TabularColumn, Tom.Column> columns,
        IReadOnlyDictionary<TabularHierarchy, Tom.Hierarchy> hierarchies,
        IReadOnlyDictionary<TabularMeasure, Tom.Measure> measures,
        IReadOnlyDictionary<TabularKpi, Tom.KPI> kpis,
        IReadOnlyDictionary<TabularCalculationGroup, Tom.Table> calculationGroups)
    {
        var perspectives = new Dictionary<TabularPerspective, Tom.Perspective>();
        var perspectiveTables = new Dictionary<(TabularPerspective Perspective, Tom.Table Table), Tom.PerspectiveTable>();

        foreach (var row in source.TabularPerspectiveList.Where(row => ReferenceEquals(row.TabularModel, root)))
        {
            var perspective = new Tom.Perspective
            {
                Name = row.Name,
                Description = row.Description,
            };
            target.Perspectives.Add(perspective);
            perspectives[row] = perspective;
        }

        foreach (var row in source.TabularPerspectiveTableList)
        {
            var perspective = RequirePerspective(perspectives, row.TabularPerspective, $"TabularPerspectiveTable '{row.Id}'");
            var table = RequireTable(tables, row.TabularTable, $"TabularPerspectiveTable '{row.Id}'");
            RequireSameModel(row.TabularPerspective, row.TabularTable.TabularModel, $"TabularPerspectiveTable '{row.Id}'");
            var perspectiveTable = EnsurePerspectiveTable(row.TabularPerspective, perspective, table, perspectiveTables);
            if (perspectiveTable.IncludeAll)
            {
                throw new InvalidOperationException($"TabularPerspective '{row.TabularPerspective.Id}' contains duplicate full-table membership for table '{row.TabularTable.Id}'.");
            }

            perspectiveTable.IncludeAll = true;
        }

        foreach (var row in source.TabularPerspectiveColumnList)
        {
            var perspective = RequirePerspective(perspectives, row.TabularPerspective, $"TabularPerspectiveColumn '{row.Id}'");
            var column = RequireColumn(columns, row.TabularColumn, $"TabularPerspectiveColumn '{row.Id}'");
            RequireSameModel(row.TabularPerspective, row.TabularColumn.TabularTable.TabularModel, $"TabularPerspectiveColumn '{row.Id}'");
            var perspectiveTable = EnsurePerspectiveTable(row.TabularPerspective, perspective, column.Table, perspectiveTables);
            if (perspectiveTable.IncludeAll)
            {
                continue;
            }

            if (perspectiveTable.PerspectiveColumns.Any(item => ReferenceEquals(item.Column, column)))
            {
                throw new InvalidOperationException($"TabularPerspective '{row.TabularPerspective.Id}' contains duplicate column membership for column '{row.TabularColumn.Id}'.");
            }

            perspectiveTable.PerspectiveColumns.Add(new Tom.PerspectiveColumn
            {
                Name = column.Name,
                Column = column,
            });
        }

        foreach (var row in source.TabularPerspectiveHierarchyList)
        {
            var perspective = RequirePerspective(perspectives, row.TabularPerspective, $"TabularPerspectiveHierarchy '{row.Id}'");
            var hierarchy = RequireHierarchy(hierarchies, row.TabularHierarchy, $"TabularPerspectiveHierarchy '{row.Id}'");
            RequireSameModel(row.TabularPerspective, row.TabularHierarchy.TabularTable.TabularModel, $"TabularPerspectiveHierarchy '{row.Id}'");
            var perspectiveTable = EnsurePerspectiveTable(row.TabularPerspective, perspective, hierarchy.Table, perspectiveTables);
            if (perspectiveTable.IncludeAll)
            {
                continue;
            }

            if (perspectiveTable.PerspectiveHierarchies.Any(item => ReferenceEquals(item.Hierarchy, hierarchy)))
            {
                throw new InvalidOperationException($"TabularPerspective '{row.TabularPerspective.Id}' contains duplicate hierarchy membership for hierarchy '{row.TabularHierarchy.Id}'.");
            }

            perspectiveTable.PerspectiveHierarchies.Add(new Tom.PerspectiveHierarchy
            {
                Name = hierarchy.Name,
                Hierarchy = hierarchy,
            });
        }

        foreach (var row in source.TabularPerspectiveMeasureList)
        {
            var perspective = RequirePerspective(perspectives, row.TabularPerspective, $"TabularPerspectiveMeasure '{row.Id}'");
            var measure = RequireMeasure(measures, row.TabularMeasure, $"TabularPerspectiveMeasure '{row.Id}'");
            RequireSameModel(row.TabularPerspective, row.TabularMeasure.TabularTable.TabularModel, $"TabularPerspectiveMeasure '{row.Id}'");
            var perspectiveTable = EnsurePerspectiveTable(row.TabularPerspective, perspective, measure.Table, perspectiveTables);
            AddPerspectiveMeasureMembership(row.TabularPerspective, perspectiveTable, measure, row.TabularMeasure.Id);
        }

        foreach (var row in source.TabularPerspectiveCalculationGroupList)
        {
            var perspective = RequirePerspective(perspectives, row.TabularPerspective, $"TabularPerspectiveCalculationGroup '{row.Id}'");
            if (!calculationGroups.TryGetValue(row.TabularCalculationGroup, out var table))
            {
                throw new InvalidOperationException($"TabularPerspectiveCalculationGroup '{row.Id}' references calculation group '{row.TabularCalculationGroup.Id}' that was not emitted to the target model.");
            }

            RequireSameModel(row.TabularPerspective, row.TabularCalculationGroup.TabularModel, $"TabularPerspectiveCalculationGroup '{row.Id}'");
            var perspectiveTable = EnsurePerspectiveTable(row.TabularPerspective, perspective, table, perspectiveTables);
            if (perspectiveTable.IncludeAll)
            {
                throw new InvalidOperationException($"TabularPerspective '{row.TabularPerspective.Id}' contains duplicate calculation-group membership for calculation group '{row.TabularCalculationGroup.Id}'.");
            }

            perspectiveTable.IncludeAll = true;
        }

        foreach (var row in source.TabularPerspectiveKpiList)
        {
            var perspective = RequirePerspective(perspectives, row.TabularPerspective, $"TabularPerspectiveKpi '{row.Id}'");
            if (!kpis.ContainsKey(row.TabularKpi))
            {
                throw new InvalidOperationException($"TabularPerspectiveKpi '{row.Id}' references KPI '{row.TabularKpi.Id}' that was not emitted to the target model.");
            }

            var measure = RequireMeasure(measures, row.TabularKpi.BaseMeasure, $"TabularPerspectiveKpi '{row.Id}'");
            RequireSameModel(row.TabularPerspective, row.TabularKpi.BaseMeasure.TabularTable.TabularModel, $"TabularPerspectiveKpi '{row.Id}'");
            var perspectiveTable = EnsurePerspectiveTable(row.TabularPerspective, perspective, measure.Table, perspectiveTables);
            AddPerspectiveMeasureMembership(row.TabularPerspective, perspectiveTable, measure, row.TabularKpi.BaseMeasure.Id);
        }

        return perspectives;
    }

    private static void AddPerspectiveMeasureMembership(
        TabularPerspective sourcePerspective,
        Tom.PerspectiveTable perspectiveTable,
        Tom.Measure measure,
        string sourceMeasureId)
    {
        if (perspectiveTable.IncludeAll)
        {
            return;
        }

        if (perspectiveTable.PerspectiveMeasures.Any(item => ReferenceEquals(item.Measure, measure)))
        {
            throw new InvalidOperationException($"TabularPerspective '{sourcePerspective.Id}' contains duplicate measure membership for measure '{sourceMeasureId}'.");
        }

        perspectiveTable.PerspectiveMeasures.Add(new Tom.PerspectiveMeasure
        {
            Name = measure.Name,
            Measure = measure,
        });
    }

    private static Tom.PerspectiveTable EnsurePerspectiveTable(
        TabularPerspective sourcePerspective,
        Tom.Perspective targetPerspective,
        Tom.Table targetTable,
        IDictionary<(TabularPerspective Perspective, Tom.Table Table), Tom.PerspectiveTable> perspectiveTables)
    {
        var key = (sourcePerspective, targetTable);
        if (perspectiveTables.TryGetValue(key, out var existing))
        {
            return existing;
        }

        var perspectiveTable = new Tom.PerspectiveTable
        {
            Name = targetTable.Name,
            Table = targetTable,
            IncludeAll = false,
        };
        targetPerspective.PerspectiveTables.Add(perspectiveTable);
        perspectiveTables[key] = perspectiveTable;
        return perspectiveTable;
    }

    private static Tom.Perspective RequirePerspective(
        IReadOnlyDictionary<TabularPerspective, Tom.Perspective> perspectives,
        TabularPerspective sourcePerspective,
        string context)
    {
        return perspectives.TryGetValue(sourcePerspective, out var perspective)
            ? perspective
            : throw new InvalidOperationException($"{context} references perspective '{sourcePerspective.Id}' that was not emitted to the target model.");
    }

    private static Tom.Table RequireTable(
        IReadOnlyDictionary<TabularTable, Tom.Table> tables,
        TabularTable sourceTable,
        string context)
    {
        return tables.TryGetValue(sourceTable, out var table)
            ? table
            : throw new InvalidOperationException($"{context} references table '{sourceTable.Id}' that was not emitted to the target model.");
    }

    private static Tom.Column RequireColumn(
        IReadOnlyDictionary<TabularColumn, Tom.Column> columns,
        TabularColumn sourceColumn,
        string context)
    {
        return columns.TryGetValue(sourceColumn, out var column)
            ? column
            : throw new InvalidOperationException($"{context} references column '{sourceColumn.Id}' that was not emitted to the target model.");
    }

    private static Tom.Hierarchy RequireHierarchy(
        IReadOnlyDictionary<TabularHierarchy, Tom.Hierarchy> hierarchies,
        TabularHierarchy sourceHierarchy,
        string context)
    {
        return hierarchies.TryGetValue(sourceHierarchy, out var hierarchy)
            ? hierarchy
            : throw new InvalidOperationException($"{context} references hierarchy '{sourceHierarchy.Id}' that was not emitted to the target model.");
    }

    private static Tom.Measure RequireMeasure(
        IReadOnlyDictionary<TabularMeasure, Tom.Measure> measures,
        TabularMeasure sourceMeasure,
        string context)
    {
        return measures.TryGetValue(sourceMeasure, out var measure)
            ? measure
            : throw new InvalidOperationException($"{context} references measure '{sourceMeasure.Id}' that was not emitted to the target model.");
    }

    private static Tom.KPI RequireKpi(
        IReadOnlyDictionary<TabularKpi, Tom.KPI> kpis,
        TabularKpi sourceKpi,
        string context)
    {
        return kpis.TryGetValue(sourceKpi, out var kpi)
            ? kpi
            : throw new InvalidOperationException($"{context} references KPI '{sourceKpi.Id}' that was not emitted to the target model.");
    }

    private static void RequireSameModel(TabularPerspective perspective, TabularModel owner, string context)
    {
        if (!ReferenceEquals(perspective.TabularModel, owner))
        {
            throw new InvalidOperationException($"{context} references an item outside perspective model '{perspective.TabularModel.Id}'.");
        }
    }

    private static Dictionary<TabularCulture, Tom.Culture> AddCultures(
        Tom.Model target,
        MetaTabularModel source,
        TabularModel root)
    {
        var result = new Dictionary<TabularCulture, Tom.Culture>();
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in source.TabularCultureList.Where(row => ReferenceEquals(row.TabularModel, root)))
        {
            if (!names.Add(row.Name))
            {
                throw new InvalidOperationException($"TabularModel '{root.Id}' has more than one TabularCulture row named '{row.Name}'.");
            }

            var cultureInfo = CultureInfo.GetCultureInfo(row.Name);
            var culture = new Tom.Culture
            {
                Name = cultureInfo.Name,
            };
            target.Cultures.Add(culture);
            result[row] = culture;
        }

        return result;
    }

    private static void AddTranslations(
        MetaTabularModel source,
        IReadOnlyDictionary<TabularCulture, Tom.Culture> cultures,
        IReadOnlyDictionary<TabularTable, Tom.Table> tables,
        IReadOnlyDictionary<TabularColumn, Tom.Column> columns,
        IReadOnlyDictionary<TabularHierarchy, Tom.Hierarchy> hierarchies,
        IReadOnlyDictionary<TabularMeasure, Tom.Measure> measures,
        IReadOnlyDictionary<TabularKpi, Tom.KPI> kpis,
        IReadOnlyDictionary<TabularPerspective, Tom.Perspective> perspectives)
    {
        var assigned = new HashSet<(Tom.Culture Culture, Tom.MetadataObject Object, Tom.TranslatedProperty Property)>();

        foreach (var row in source.TabularTableTranslationList)
        {
            var culture = RequireCulture(cultures, row.TabularCulture, $"TabularTableTranslation '{row.Id}'");
            var table = RequireTable(tables, row.TabularTable, $"TabularTableTranslation '{row.Id}'");
            RequireSameModel(row.TabularCulture, row.TabularTable.TabularModel, $"TabularTableTranslation '{row.Id}'");
            AddObjectTranslation(culture, table, Tom.TranslatedProperty.Caption, row.Caption, assigned, $"TabularTableTranslation '{row.Id}'");
            AddObjectTranslation(culture, table, Tom.TranslatedProperty.Description, row.Description, assigned, $"TabularTableTranslation '{row.Id}'");
        }

        foreach (var row in source.TabularColumnTranslationList)
        {
            var culture = RequireCulture(cultures, row.TabularCulture, $"TabularColumnTranslation '{row.Id}'");
            var column = RequireColumn(columns, row.TabularColumn, $"TabularColumnTranslation '{row.Id}'");
            RequireSameModel(row.TabularCulture, row.TabularColumn.TabularTable.TabularModel, $"TabularColumnTranslation '{row.Id}'");
            AddObjectTranslation(culture, column, Tom.TranslatedProperty.Caption, row.Caption, assigned, $"TabularColumnTranslation '{row.Id}'");
            AddObjectTranslation(culture, column, Tom.TranslatedProperty.Description, row.Description, assigned, $"TabularColumnTranslation '{row.Id}'");
        }

        foreach (var row in source.TabularHierarchyTranslationList)
        {
            var culture = RequireCulture(cultures, row.TabularCulture, $"TabularHierarchyTranslation '{row.Id}'");
            var hierarchy = RequireHierarchy(hierarchies, row.TabularHierarchy, $"TabularHierarchyTranslation '{row.Id}'");
            RequireSameModel(row.TabularCulture, row.TabularHierarchy.TabularTable.TabularModel, $"TabularHierarchyTranslation '{row.Id}'");
            AddObjectTranslation(culture, hierarchy, Tom.TranslatedProperty.Caption, row.Caption, assigned, $"TabularHierarchyTranslation '{row.Id}'");
            AddObjectTranslation(culture, hierarchy, Tom.TranslatedProperty.Description, row.Description, assigned, $"TabularHierarchyTranslation '{row.Id}'");
        }

        foreach (var row in source.TabularMeasureTranslationList)
        {
            var culture = RequireCulture(cultures, row.TabularCulture, $"TabularMeasureTranslation '{row.Id}'");
            var measure = RequireMeasure(measures, row.TabularMeasure, $"TabularMeasureTranslation '{row.Id}'");
            RequireSameModel(row.TabularCulture, row.TabularMeasure.TabularTable.TabularModel, $"TabularMeasureTranslation '{row.Id}'");
            AddObjectTranslation(culture, measure, Tom.TranslatedProperty.Caption, row.Caption, assigned, $"TabularMeasureTranslation '{row.Id}'");
            AddObjectTranslation(culture, measure, Tom.TranslatedProperty.Description, row.Description, assigned, $"TabularMeasureTranslation '{row.Id}'");
        }

        foreach (var row in source.TabularPerspectiveTranslationList)
        {
            var culture = RequireCulture(cultures, row.TabularCulture, $"TabularPerspectiveTranslation '{row.Id}'");
            var perspective = RequirePerspective(perspectives, row.TabularPerspective, $"TabularPerspectiveTranslation '{row.Id}'");
            RequireSameModel(row.TabularCulture, row.TabularPerspective.TabularModel, $"TabularPerspectiveTranslation '{row.Id}'");
            AddObjectTranslation(culture, perspective, Tom.TranslatedProperty.Caption, row.Caption, assigned, $"TabularPerspectiveTranslation '{row.Id}'");
            AddObjectTranslation(culture, perspective, Tom.TranslatedProperty.Description, row.Description, assigned, $"TabularPerspectiveTranslation '{row.Id}'");
        }

        foreach (var row in source.TabularKpiTranslationList)
        {
            var culture = RequireCulture(cultures, row.TabularCulture, $"TabularKpiTranslation '{row.Id}'");
            var kpi = RequireKpi(kpis, row.TabularKpi, $"TabularKpiTranslation '{row.Id}'");
            RequireSameModel(row.TabularCulture, row.TabularKpi.BaseMeasure.TabularTable.TabularModel, $"TabularKpiTranslation '{row.Id}'");
            AddObjectTranslation(culture, kpi, Tom.TranslatedProperty.Description, row.Description, assigned, $"TabularKpiTranslation '{row.Id}'");
        }
    }

    private static Tom.Culture RequireCulture(
        IReadOnlyDictionary<TabularCulture, Tom.Culture> cultures,
        TabularCulture sourceCulture,
        string context)
    {
        return cultures.TryGetValue(sourceCulture, out var culture)
            ? culture
            : throw new InvalidOperationException($"{context} references culture '{sourceCulture.Id}' that was not emitted to the target model.");
    }

    private static void AddObjectTranslation(
        Tom.Culture culture,
        Tom.MetadataObject targetObject,
        Tom.TranslatedProperty property,
        string? value,
        ISet<(Tom.Culture Culture, Tom.MetadataObject Object, Tom.TranslatedProperty Property)> assigned,
        string context)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (!assigned.Add((culture, targetObject, property)))
        {
            throw new InvalidOperationException($"{context} duplicates a {property} translation for target object '{targetObject.ObjectType}'.");
        }

        culture.ObjectTranslations.SetTranslation(targetObject, property, value);
    }

    private static void RequireSameModel(TabularCulture culture, TabularModel owner, string context)
    {
        if (!ReferenceEquals(culture.TabularModel, owner))
        {
            throw new InvalidOperationException($"{context} references an item outside culture model '{culture.TabularModel.Id}'.");
        }
    }

    private static void AddRoles(
        Tom.Model target,
        MetaTabularModel source,
        TabularModel root,
        IReadOnlyDictionary<TabularTable, Tom.Table> tables,
        IReadOnlyDictionary<TabularColumn, Tom.Column> columns)
    {
        foreach (var row in source.TabularSecurityRoleList.Where(row => ReferenceEquals(row.TabularModel, root)))
        {
            var role = new Tom.ModelRole
            {
                Name = row.Name,
                Description = row.Description,
                ModelPermission = MapPermission(row.Permission),
            };

            foreach (var member in source.TabularRoleMemberList.Where(member => ReferenceEquals(member.TabularSecurityRole, row)))
            {
                var roleMember = new Tom.WindowsModelRoleMember
                {
                    MemberName = member.MemberName,
                };
                if (!string.IsNullOrWhiteSpace(member.MemberId))
                {
                    roleMember.MemberID = member.MemberId;
                }

                role.Members.Add(roleMember);
            }

            var tablePermissions = new Dictionary<TabularTable, Tom.TablePermission>();
            var filteredTables = new HashSet<TabularTable>();
            var metadataPermissionTables = new HashSet<TabularTable>();
            var metadataPermissionColumns = new HashSet<TabularColumn>();
            foreach (var filter in source.TabularRoleFilterList.Where(filter => ReferenceEquals(filter.TabularSecurityRole, row)))
            {
                var table = RequireTable(tables, filter.TabularTable, $"TabularRoleFilter '{filter.Id}'");
                RequireSameModel(row, filter.TabularTable.TabularModel, $"TabularRoleFilter '{filter.Id}'");
                if (!filteredTables.Add(filter.TabularTable))
                {
                    throw new InvalidOperationException($"TabularSecurityRole '{row.Id}' has more than one TabularRoleFilter for table '{filter.TabularTable.Id}'.");
                }

                var tablePermission = EnsureTablePermission(role, filter.TabularTable, table, tablePermissions);
                tablePermission.FilterExpression = filter.Expression;
            }

            foreach (var permission in source.TabularTablePermissionList.Where(permission => ReferenceEquals(permission.TabularSecurityRole, row)))
            {
                var table = RequireTable(tables, permission.TabularTable, $"TabularTablePermission '{permission.Id}'");
                RequireSameModel(row, permission.TabularTable.TabularModel, $"TabularTablePermission '{permission.Id}'");
                if (!metadataPermissionTables.Add(permission.TabularTable))
                {
                    throw new InvalidOperationException($"TabularSecurityRole '{row.Id}' has more than one TabularTablePermission for table '{permission.TabularTable.Id}'.");
                }

                var tablePermission = EnsureTablePermission(role, permission.TabularTable, table, tablePermissions);
                tablePermission.MetadataPermission = MapMetadataPermission(permission.MetadataPermission);
            }

            foreach (var permission in source.TabularColumnPermissionList.Where(permission => ReferenceEquals(permission.TabularSecurityRole, row)))
            {
                var column = RequireColumn(columns, permission.TabularColumn, $"TabularColumnPermission '{permission.Id}'");
                RequireSameModel(row, permission.TabularColumn.TabularTable.TabularModel, $"TabularColumnPermission '{permission.Id}'");
                if (!metadataPermissionColumns.Add(permission.TabularColumn))
                {
                    throw new InvalidOperationException($"TabularSecurityRole '{row.Id}' has more than one TabularColumnPermission for column '{permission.TabularColumn.Id}'.");
                }

                var tablePermission = EnsureTablePermission(role, permission.TabularColumn.TabularTable, column.Table, tablePermissions);
                tablePermission.ColumnPermissions.Add(new Tom.ColumnPermission
                {
                    Name = column.Name,
                    Column = column,
                    MetadataPermission = MapMetadataPermission(permission.MetadataPermission),
                });
            }

            target.Roles.Add(role);
        }
    }

    private static Tom.TablePermission EnsureTablePermission(
        Tom.ModelRole role,
        TabularTable sourceTable,
        Tom.Table targetTable,
        IDictionary<TabularTable, Tom.TablePermission> tablePermissions)
    {
        if (tablePermissions.TryGetValue(sourceTable, out var existing))
        {
            return existing;
        }

        var tablePermission = new Tom.TablePermission
        {
            Name = targetTable.Name,
            Table = targetTable,
        };
        role.TablePermissions.Add(tablePermission);
        tablePermissions[sourceTable] = tablePermission;
        return tablePermission;
    }

    private static void RequireSameModel(TabularSecurityRole role, TabularModel owner, string context)
    {
        if (!ReferenceEquals(role.TabularModel, owner))
        {
            throw new InvalidOperationException($"{context} references an item outside role model '{role.TabularModel.Id}'.");
        }
    }

    private static TabularModel RequireSingleModel(MetaTabularModel model)
    {
        return model.TabularModelList.Count switch
        {
            1 => model.TabularModelList[0],
            0 => throw new InvalidOperationException("MetaTabular deploy requires exactly one TabularModel row. Found none."),
            _ => throw new InvalidOperationException($"MetaTabular deploy requires exactly one TabularModel row. Found {model.TabularModelList.Count}."),
        };
    }

    private static Tom.Database? FindDatabase(Tom.Server server, string databaseName)
    {
        return server.Databases
            .OfType<Tom.Database>()
            .FirstOrDefault(database =>
                string.Equals(database.ID, databaseName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(database.Name, databaseName, StringComparison.OrdinalIgnoreCase));
    }

    private static string ResolveConnectionString(string? connectionReference)
    {
        if (!string.IsNullOrWhiteSpace(connectionReference))
        {
            var value = Environment.GetEnvironmentVariable(connectionReference);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return "Data Source=localhost;Initial Catalog=master;Integrated Security=SSPI";
    }

    private static string MapProvider(string? provider)
    {
        var trimmed = provider?.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return "System.Data.SqlClient";
        }

        return trimmed.ToUpperInvariant() switch
        {
            "SQLSERVER" or "SQLCLIENT" or "SYSTEM.DATA.SQLCLIENT" => "System.Data.SqlClient",
            "OLEDB" or "MSOLEDBSQL" => "MSOLEDBSQL",
            _ => trimmed,
        };
    }

    private static Tom.DataType MapTabularDataType(string? dataTypeId)
    {
        var value = StripMetaTypePrefix(dataTypeId).ToUpperInvariant();
        return value switch
        {
            "BOOLEAN" or "BOOL" => Tom.DataType.Boolean,
            "DATETIME" or "DATE" or "TIME" => Tom.DataType.DateTime,
            "DECIMAL" or "MONEY" or "NUMERIC" => Tom.DataType.Decimal,
            "DOUBLE" or "FLOAT" or "REAL" => Tom.DataType.Double,
            "INT16" or "INT32" or "INT64" or "INTEGER" or "LONG" => Tom.DataType.Int64,
            "BINARY" => Tom.DataType.Binary,
            _ => Tom.DataType.String,
        };
    }

    private static Tom.AggregateFunction MapSummarizeBy(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Tom.AggregateFunction.Default
            : ParseEnum<Tom.AggregateFunction>(value, "TabularColumn.SummarizeBy");
    }

    private static Tom.ModeType MapMode(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Tom.ModeType.Import
            : ParseEnum<Tom.ModeType>(value, "TabularPartition.Mode");
    }

    private static Tom.RelationshipEndCardinality MapFromCardinality(string value)
    {
        return ParseRelationshipCardinality(value) switch
        {
            "OneToMany" => Tom.RelationshipEndCardinality.One,
            _ => Tom.RelationshipEndCardinality.Many,
        };
    }

    private static Tom.RelationshipEndCardinality MapToCardinality(string value)
    {
        return ParseRelationshipCardinality(value) switch
        {
            "OneToOne" or "ManyToOne" => Tom.RelationshipEndCardinality.One,
            _ => Tom.RelationshipEndCardinality.Many,
        };
    }

    private static Tom.CrossFilteringBehavior MapCrossFiltering(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Tom.CrossFilteringBehavior.OneDirection;
        }

        return value.Trim().ToUpperInvariant() switch
        {
            "SINGLE" or "ONEDIRECTION" => Tom.CrossFilteringBehavior.OneDirection,
            "BOTH" or "BOTHDIRECTIONS" => Tom.CrossFilteringBehavior.BothDirections,
            _ => throw new InvalidOperationException($"TabularRelationship.CrossFilterDirection value '{value}' is not supported. Use Single, OneDirection, Both, or BothDirections."),
        };
    }

    private static Tom.ModelPermission MapPermission(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Tom.ModelPermission.Read
            : ParseEnum<Tom.ModelPermission>(value, "TabularSecurityRole.Permission");
    }

    private static Tom.MetadataPermission MapMetadataPermission(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Tom.MetadataPermission.Default
            : ParseEnum<Tom.MetadataPermission>(value, "MetadataPermission");
    }

    private static string ParseRelationshipCardinality(string value)
    {
        return value.Trim().ToUpperInvariant() switch
        {
            "ONETOONE" => "OneToOne",
            "ONETOMANY" => "OneToMany",
            "MANYTOONE" => "ManyToOne",
            "MANYTOMANY" => "ManyToMany",
            _ => throw new InvalidOperationException($"TabularRelationship.Cardinality value '{value}' is not supported. Use OneToOne, OneToMany, ManyToOne, or ManyToMany."),
        };
    }

    private static TEnum ParseEnum<TEnum>(string value, string context)
        where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        throw new InvalidOperationException($"{context} value '{value}' is not a valid {typeof(TEnum).Name}.");
    }

    private static bool ParseBool(string? value)
    {
        return ParseNullableBool(value) == true;
    }

    private static bool? ParseNullableBool(string? value)
    {
        return bool.TryParse(value, out var parsed) ? parsed : null;
    }

    private static int ParseInt(string? value, int defaultValue, string context)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        if (int.TryParse(value, out var parsed))
        {
            return parsed;
        }

        throw new InvalidOperationException($"{context} value '{value}' is not a valid integer.");
    }

    private static string StripMetaTypePrefix(string? value)
    {
        const string prefix = "meta:type:";
        return !string.IsNullOrWhiteSpace(value) && value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? value[prefix.Length..]
            : value?.Trim() ?? string.Empty;
    }

    private static string ToDaxMeasureReference(string measureName)
    {
        return $"[{measureName.Replace("]", "]]")}]";
    }
}
