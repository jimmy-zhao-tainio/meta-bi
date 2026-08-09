using System.Globalization;
using System.Data;
using Amo = Microsoft.AnalysisServices;

namespace MetaMultiDimensional.Core.Deploy;

public sealed class MetaMultiDimensionalDeployService
{
    private const int MultidimensionalCompatibilityLevel = 1100;
    private const int LargeStringStoresCompatibilityLevel = 1100;

    private sealed record MeasureGroupTargets(
        Dictionary<MeasureGroup, Amo.MeasureGroup> MeasureGroups,
        Dictionary<Measure, Amo.Measure> Measures);

    private sealed record DimensionTargets(
        Dictionary<Dimension, Amo.Dimension> Dimensions,
        Dictionary<DimensionAttribute, Amo.DimensionAttribute> Attributes);

    private sealed record ScriptTargets(
        Dictionary<NamedSet, Amo.CalculationProperty> NamedSets,
        Dictionary<MdxCalculation, Amo.CalculationProperty> Calculations);

    public Task<MetaMultiDimensionalDeployResult> DeployAsync(MetaMultiDimensionalDeployRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Server);

        var workspacePath = Path.GetFullPath(request.WorkspacePath);
        var model = Meta.Core.Serialization.TypedWorkspaceModelMapper.Load<MetaMultiDimensionalModel>(workspacePath, searchUpward: false);
        var root = RequireSingleDatabase(model);
        var databaseName = string.IsNullOrWhiteSpace(request.DatabaseName)
            ? root.Name
            : request.DatabaseName.Trim();
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("Multidimensional deploy requires a database name. Set MultiDimensionalDatabase.Name or pass --database-name.");
        }

        using var server = new Amo.Server();
        server.Connect($"Data Source={request.Server}");

        var existing = FindDatabase(server, databaseName);
        if (existing != null)
        {
            if (!request.DropExisting)
            {
                var processingNote = request.Process
                    ? " Processing an existing multidimensional database requires --drop-existing so deploy uses the safe drop, create, full-process sequence."
                    : string.Empty;
                throw new InvalidOperationException($"Multidimensional database '{databaseName}' already exists. Pass --drop-existing to drop it before deploying.{processingNote}");
            }

            existing.Drop();
            server.Refresh();
        }

        var database = BuildDatabase(model, root, databaseName);
        server.Databases.Add(database);
        database.Update(Amo.UpdateOptions.ExpandFull);
        if (request.Process)
        {
            ProcessDatabase(database, databaseName);
        }

        return Task.FromResult(new MetaMultiDimensionalDeployResult
        {
            WorkspacePath = workspacePath,
            Server = request.Server,
            DatabaseName = databaseName,
            DropExisting = request.DropExisting,
            Processed = request.Process,
            CubeCount = model.CubeList.Count,
            DimensionCount = model.DimensionList.Count,
            MeasureGroupCount = model.MeasureGroupList.Count,
            MeasureCount = model.MeasureList.Count,
        });
    }

    private static Amo.Database BuildDatabase(MetaMultiDimensionalModel source, MultiDimensionalDatabase root, string databaseName)
    {
        var database = new Amo.Database
        {
            ID = databaseName,
            Name = databaseName,
            Description = root.Description,
            Language = ParseLanguage(root.DefaultLanguage),
            CompatibilityLevel = MultidimensionalCompatibilityLevel,
        };
        if (!string.IsNullOrWhiteSpace(root.Collation))
        {
            database.Collation = root.Collation;
        }

        var dataSources = AddDataSources(database, source, root);
        var dataSourceViews = AddDataSourceViews(database, source, root, dataSources);
        var defaultDataSourceView = ResolveDefaultDataSourceView(source, root, dataSourceViews);
        var dimensionTargets = AddDimensions(database, source, root, defaultDataSourceView);
        var cubes = AddCubes(database, source, root, dimensionTargets.Dimensions, defaultDataSourceView);
        var measureGroupTargets = AddMeasureGroups(source, cubes, dataSources);
        var kpis = AddKpis(source, cubes);
        var scriptTargets = AddMdxScripts(source, cubes);
        var actions = AddActions(source, cubes);
        var perspectives = AddPerspectives(source, cubes, measureGroupTargets, kpis, actions);
        AddTranslations(source, root, cubes, dimensionTargets, measureGroupTargets, kpis, actions, perspectives, scriptTargets);
        AddRoles(database, source, root, cubes, dimensionTargets);
        return database;
    }

    private static void ProcessDatabase(Amo.Database database, string databaseName)
    {
        try
        {
            database.Process(Amo.ProcessType.ProcessFull);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Deployed multidimensional database '{databaseName}', but processing failed: {ex.Message}", ex);
        }
    }

    private static Dictionary<MultiDimensionalDataSource, Amo.DataSource> AddDataSources(
        Amo.Database target,
        MetaMultiDimensionalModel source,
        MultiDimensionalDatabase root)
    {
        var result = new Dictionary<MultiDimensionalDataSource, Amo.DataSource>();
        foreach (var row in source.MultiDimensionalDataSourceList.Where(row => ReferenceEquals(row.MultiDimensionalDatabase, root)))
        {
            var dataSource = new Amo.RelationalDataSource
            {
                ID = StableId(row.Id),
                Name = row.Name,
                Description = row.Description,
                ConnectionString = ResolveConnectionString(row.ConnectionReference),
                ManagedProvider = MapManagedProvider(row.Provider),
                ImpersonationInfo = new Amo.ImpersonationInfo(Amo.ImpersonationMode.ImpersonateServiceAccount),
            };
            target.DataSources.Add(dataSource);
            result[row] = dataSource;
        }

        return result;
    }

    private static Dictionary<MultiDimensionalDataSource, Amo.DataSourceView> AddDataSourceViews(
        Amo.Database target,
        MetaMultiDimensionalModel source,
        MultiDimensionalDatabase root,
        IReadOnlyDictionary<MultiDimensionalDataSource, Amo.DataSource> dataSources)
    {
        var result = new Dictionary<MultiDimensionalDataSource, Amo.DataSourceView>();
        foreach (var row in source.MultiDimensionalDataSourceList.Where(row => ReferenceEquals(row.MultiDimensionalDatabase, root)))
        {
            if (!dataSources.TryGetValue(row, out var dataSource))
            {
                continue;
            }

            var dataSourceView = new Amo.DataSourceView(row.Name, $"{StableId(row.Id)}_dsv")
            {
                DataSourceID = dataSource.ID,
                Schema = BuildDataSourceViewSchema(source, root),
            };
            target.DataSourceViews.Add(dataSourceView);
            result[row] = dataSourceView;
        }

        return result;
    }

    private static DataSet BuildDataSourceViewSchema(MetaMultiDimensionalModel source, MultiDimensionalDatabase root)
    {
        var dataSet = new DataSet("MetaMultiDimensional")
        {
            Locale = GetCulture(root.DefaultLanguage),
        };

        foreach (var dimension in source.DimensionList.Where(row => ReferenceEquals(row.MultiDimensionalDatabase, root)))
        {
            var table = EnsureTable(dataSet, SourceName(dimension.SourceName, dimension.Name, dimension.Id));
            var attributes = source.DimensionAttributeList
                .Where(attribute => ReferenceEquals(attribute.Dimension, dimension))
                .ToArray();
            if (attributes.Length == 0)
            {
                EnsureColumn(table, "Key", typeof(string));
                continue;
            }

            foreach (var attribute in attributes)
            {
                var sourceColumnName = SourceName(attribute.SourceName, attribute.Name, attribute.Id);
                EnsureColumn(
                    table,
                    sourceColumnName,
                    MapClrDataType(attribute.DataTypeId));
                EnsureColumn(table, AttributeNameColumnName(sourceColumnName, attribute.DataTypeId), typeof(string));
            }
        }

        foreach (var measureGroup in source.MeasureGroupList.Where(row => ReferenceEquals(row.Cube.MultiDimensionalDatabase, root)))
        {
            var table = EnsureTable(dataSet, SourceName(measureGroup.SourceName, measureGroup.Name, measureGroup.Id));
            foreach (var measure in source.MeasureList.Where(measure => ReferenceEquals(measure.MeasureGroup, measureGroup)))
            {
                EnsureColumn(
                    table,
                    SourceName(measure.SourceName, measure.Name, measure.Id),
                    MapMeasureClrDataType(measure.DataTypeId));
            }

            foreach (var usage in source.DimensionUsageList.Where(usage => ReferenceEquals(usage.MeasureGroup, measureGroup)))
            {
                if (usage.GranularityAttribute != null)
                {
                    EnsureColumn(
                        table,
                        SourceName(usage.GranularityAttribute.SourceName, usage.GranularityAttribute.Name, usage.GranularityAttribute.Id),
                        MapClrDataType(usage.GranularityAttribute.DataTypeId));
                }
            }
        }

        AddDataSourceViewRelations(dataSet, source, root);
        return dataSet;
    }

    private static void AddDataSourceViewRelations(DataSet dataSet, MetaMultiDimensionalModel source, MultiDimensionalDatabase root)
    {
        foreach (var measureGroup in source.MeasureGroupList.Where(row => ReferenceEquals(row.Cube.MultiDimensionalDatabase, root)))
        {
            var factTableName = SourceName(measureGroup.SourceName, measureGroup.Name, measureGroup.Id);
            var factTable = dataSet.Tables[factTableName];
            if (factTable == null)
            {
                continue;
            }

            foreach (var usage in source.DimensionUsageList.Where(usage => ReferenceEquals(usage.MeasureGroup, measureGroup)))
            {
                var attribute = usage.GranularityAttribute;
                if (attribute == null)
                {
                    continue;
                }

                var dimension = attribute.Dimension;
                var dimensionTableName = SourceName(dimension.SourceName, dimension.Name, dimension.Id);
                var dimensionTable = dataSet.Tables[dimensionTableName];
                if (dimensionTable == null)
                {
                    continue;
                }

                var keyColumnName = SourceName(attribute.SourceName, attribute.Name, attribute.Id);
                var dimensionColumn = dimensionTable.Columns[keyColumnName];
                var factColumn = factTable.Columns[keyColumnName];
                if (dimensionColumn == null || factColumn == null)
                {
                    continue;
                }

                var relationName = StableId($"{measureGroup.Id}_{usage.Id}_{attribute.Id}_relation");
                if (dataSet.Relations.Contains(relationName))
                {
                    continue;
                }

                dataSet.Relations.Add(new DataRelation(
                    relationName,
                    dimensionColumn,
                    factColumn,
                    createConstraints: false));
            }
        }
    }

    private static Amo.DataSourceView? ResolveDefaultDataSourceView(
        MetaMultiDimensionalModel source,
        MultiDimensionalDatabase root,
        IReadOnlyDictionary<MultiDimensionalDataSource, Amo.DataSourceView> dataSourceViews)
    {
        if (dataSourceViews.Count == 1)
        {
            return dataSourceViews.Values.Single();
        }

        var hasSourceBoundObjects =
            source.DimensionList.Any(row => ReferenceEquals(row.MultiDimensionalDatabase, root)) ||
            source.CubeList.Any(row => ReferenceEquals(row.MultiDimensionalDatabase, root)) ||
            source.MeasureGroupList.Any(row => ReferenceEquals(row.Cube.MultiDimensionalDatabase, root));

        if (!hasSourceBoundObjects)
        {
            return null;
        }

        if (dataSourceViews.Count == 0)
        {
            throw new InvalidOperationException("Multidimensional deploy requires one MultiDimensionalDataSource row when dimensions, cubes, or measure groups are present.");
        }

        throw new InvalidOperationException("Multidimensional deploy currently requires exactly one MultiDimensionalDataSource row because per-dimension and per-measure-group source binding is not modeled yet.");
    }

    private static DimensionTargets AddDimensions(
        Amo.Database target,
        MetaMultiDimensionalModel source,
        MultiDimensionalDatabase root,
        Amo.DataSourceView? dataSourceView)
    {
        var dimensions = new Dictionary<Dimension, Amo.Dimension>();
        var dimensionAttributes = new Dictionary<DimensionAttribute, Amo.DimensionAttribute>();
        foreach (var row in source.DimensionList.Where(row => ReferenceEquals(row.MultiDimensionalDatabase, root)))
        {
            var dimension = new Amo.Dimension
            {
                ID = StableId(row.Id),
                Name = row.Name,
                Description = row.Description,
                Type = MapDimensionType(row.DimensionType),
                StorageMode = MapDimensionStorageMode(row.StorageMode),
                ProcessingMode = MapProcessingMode(row.ProcessingMode, "Dimension.ProcessingMode"),
                ProcessingGroup = MapProcessingGroup(row.ProcessingGroup),
                StringStoresCompatibilityLevel = LargeStringStoresCompatibilityLevel,
            };
            if (dataSourceView != null)
            {
                dimension.Source = new Amo.DataSourceViewBinding(dataSourceView.ID);
            }

            var attributes = source.DimensionAttributeList
                .Where(attribute => ReferenceEquals(attribute.Dimension, row))
                .OrderBy(attribute => ParseInt(attribute.Ordinal, int.MaxValue, "DimensionAttribute.Ordinal"))
                .ToArray();
            var targetAttributes = new Dictionary<DimensionAttribute, Amo.DimensionAttribute>();
            if (attributes.Length == 0)
            {
                var keyAttribute = new Amo.DimensionAttribute
                {
                    ID = "Key",
                    Name = "Key",
                    Usage = Amo.AttributeUsage.Key,
                };
                    keyAttribute.KeyColumns.Add(CreateDataItem(SourceName(row.SourceName, row.Name, row.Id), "Key", "meta:type:String"));
                    keyAttribute.NameColumn = CreateDataItem(SourceName(row.SourceName, row.Name, row.Id), "Key", "meta:type:String");
                    dimension.Attributes.Add(keyAttribute);
            }
            else
            {
                var sourceTableName = SourceName(row.SourceName, row.Name, row.Id);
                foreach (var attribute in attributes)
                {
                    var sourceColumnName = SourceName(attribute.SourceName, attribute.Name, attribute.Id);
                    var targetAttribute = new Amo.DimensionAttribute
                    {
                        ID = StableId(attribute.Id),
                        Name = attribute.Name,
                        Description = attribute.Description,
                        Usage = MapAttributeUsage(attribute.Usage, attribute.IsKey),
                        AttributeHierarchyEnabled = ParseNullableBool(attribute.AttributeHierarchyEnabled) ?? true,
                        AttributeHierarchyVisible = ParseNullableBool(attribute.AttributeHierarchyVisible) ?? true,
                    };
                    targetAttribute.KeyColumns.Add(CreateDataItem(sourceTableName, sourceColumnName, attribute.DataTypeId));
                    targetAttribute.NameColumn = new Amo.DataItem(sourceTableName, AttributeNameColumnName(sourceColumnName, attribute.DataTypeId), Amo.DataType.String);
                    dimension.Attributes.Add(targetAttribute);
                    targetAttributes[attribute] = targetAttribute;
                    dimensionAttributes[attribute] = targetAttribute;
                }
            }

            AddAttributeRelationships(source, row, targetAttributes);
            AddDimensionHierarchies(dimension, source, row, targetAttributes);
            target.Dimensions.Add(dimension);
            dimensions[row] = dimension;
        }

        return new DimensionTargets(dimensions, dimensionAttributes);
    }

    private static void AddAttributeRelationships(
        MetaMultiDimensionalModel source,
        Dimension row,
        IReadOnlyDictionary<DimensionAttribute, Amo.DimensionAttribute> targetAttributes)
    {
        foreach (var relationship in source.AttributeRelationshipList
                     .Where(relationship => ReferenceEquals(relationship.ChildAttribute.Dimension, row)))
        {
            if (!ReferenceEquals(relationship.ParentAttribute.Dimension, row))
            {
                throw new InvalidOperationException(
                    $"AttributeRelationship '{relationship.Id}' references attributes from different dimensions.");
            }

            if (ReferenceEquals(relationship.ChildAttribute, relationship.ParentAttribute))
            {
                throw new InvalidOperationException(
                    $"AttributeRelationship '{relationship.Id}' cannot reference the same child and parent attribute.");
            }

            if (!targetAttributes.TryGetValue(relationship.ChildAttribute, out var targetChild))
            {
                throw new InvalidOperationException(
                    $"AttributeRelationship '{relationship.Id}' references child attribute '{relationship.ChildAttribute.Id}' that is not emitted for dimension '{row.Id}'.");
            }

            if (!targetAttributes.TryGetValue(relationship.ParentAttribute, out var targetParent))
            {
                throw new InvalidOperationException(
                    $"AttributeRelationship '{relationship.Id}' references parent attribute '{relationship.ParentAttribute.Id}' that is not emitted for dimension '{row.Id}'.");
            }

            if (targetChild.AttributeRelationships.Contains(targetParent.ID))
            {
                throw new InvalidOperationException(
                    $"DimensionAttribute '{relationship.ChildAttribute.Id}' already has an attribute relationship to '{relationship.ParentAttribute.Id}'.");
            }

            var targetRelationship = targetChild.AttributeRelationships.Add(targetParent.ID);
            targetRelationship.RelationshipType = MapAttributeRelationshipType(relationship.RelationshipType);
        }
    }

    private static void AddDimensionHierarchies(
        Amo.Dimension target,
        MetaMultiDimensionalModel source,
        Dimension row,
        IReadOnlyDictionary<DimensionAttribute, Amo.DimensionAttribute> targetAttributes)
    {
        foreach (var hierarchyRow in source.DimensionHierarchyList.Where(hierarchy => ReferenceEquals(hierarchy.Dimension, row)))
        {
            var levels = source.DimensionHierarchyLevelList
                .Where(level => ReferenceEquals(level.DimensionHierarchy, hierarchyRow))
                .OrderBy(level => ParseInt(level.Ordinal, int.MaxValue, "DimensionHierarchyLevel.Ordinal"))
                .ToArray();
            if (levels.Length == 0)
            {
                throw new InvalidOperationException($"DimensionHierarchy '{hierarchyRow.Id}' must contain at least one level.");
            }

            var seenOrdinals = new HashSet<int>();
            var hierarchy = new Amo.Hierarchy
            {
                ID = StableId(hierarchyRow.Id),
                Name = hierarchyRow.Name,
                Description = hierarchyRow.Description,
                StructureType = MapHierarchyStructureType(hierarchyRow.HierarchyType),
            };

            foreach (var levelRow in levels)
            {
                var ordinal = ParseInt(levelRow.Ordinal, int.MaxValue, "DimensionHierarchyLevel.Ordinal");
                if (!seenOrdinals.Add(ordinal))
                {
                    throw new InvalidOperationException(
                        $"DimensionHierarchy '{hierarchyRow.Id}' contains duplicate level ordinal '{ordinal}'.");
                }

                if (!ReferenceEquals(levelRow.DimensionAttribute.Dimension, row))
                {
                    throw new InvalidOperationException(
                        $"DimensionHierarchyLevel '{levelRow.Id}' references attribute '{levelRow.DimensionAttribute.Id}' outside hierarchy dimension '{row.Id}'.");
                }

                if (!targetAttributes.TryGetValue(levelRow.DimensionAttribute, out var targetAttribute))
                {
                    throw new InvalidOperationException(
                        $"DimensionHierarchyLevel '{levelRow.Id}' references attribute '{levelRow.DimensionAttribute.Id}' that is not emitted for dimension '{row.Id}'.");
                }

                hierarchy.Levels.Add(new Amo.Level
                {
                    ID = StableId(levelRow.Id),
                    Name = levelRow.Name,
                    SourceAttributeID = targetAttribute.ID,
                });
            }

            target.Hierarchies.Add(hierarchy);
        }
    }

    private static Dictionary<Cube, Amo.Cube> AddCubes(
        Amo.Database target,
        MetaMultiDimensionalModel source,
        MultiDimensionalDatabase root,
        IReadOnlyDictionary<Dimension, Amo.Dimension> dimensions,
        Amo.DataSourceView? dataSourceView)
    {
        var result = new Dictionary<Cube, Amo.Cube>();
        foreach (var row in source.CubeList.Where(row => ReferenceEquals(row.MultiDimensionalDatabase, root)))
        {
            var cube = new Amo.Cube
            {
                ID = StableId(row.Id),
                Name = row.Name,
                Description = row.Description,
                StorageMode = MapStorageMode(row.StorageMode, "Cube.StorageMode"),
                ProcessingMode = MapProcessingMode(row.ProcessingMode, "Cube.ProcessingMode"),
            };
            if (dataSourceView != null)
            {
                cube.Source = new Amo.DataSourceViewBinding(dataSourceView.ID);
            }
            if (!string.IsNullOrWhiteSpace(row.DefaultMeasureName))
            {
                cube.DefaultMeasure = row.DefaultMeasureName;
            }

            foreach (var cubeDimension in source.CubeDimensionList.Where(item => ReferenceEquals(item.Cube, row)))
            {
                if (dimensions.TryGetValue(cubeDimension.Dimension, out var dimension))
                {
                    cube.Dimensions.Add(dimension.ID, cubeDimension.Name, StableId(cubeDimension.Id));
                }
            }

            target.Cubes.Add(cube);
            result[row] = cube;
        }

        return result;
    }

    private static MeasureGroupTargets AddMeasureGroups(
        MetaMultiDimensionalModel source,
        IReadOnlyDictionary<Cube, Amo.Cube> cubes,
        IReadOnlyDictionary<MultiDimensionalDataSource, Amo.DataSource> dataSources)
    {
        var measureGroups = new Dictionary<MeasureGroup, Amo.MeasureGroup>();
        var measures = new Dictionary<Measure, Amo.Measure>();
        foreach (var row in source.MeasureGroupList.Where(row => cubes.ContainsKey(row.Cube)))
        {
            var cube = cubes[row.Cube];
            var measureGroup = new Amo.MeasureGroup
            {
                ID = StableId(row.Id),
                Name = row.Name,
                Description = row.Description,
                StorageMode = MapStorageMode(row.StorageMode, "MeasureGroup.StorageMode"),
                ProcessingMode = MapProcessingMode(row.ProcessingMode, "MeasureGroup.ProcessingMode"),
            };

            foreach (var measure in source.MeasureList.Where(measure => ReferenceEquals(measure.MeasureGroup, row)))
            {
                if (string.IsNullOrWhiteSpace(cube.DefaultMeasure))
                {
                    cube.DefaultMeasure = $"[Measures].[{EscapeMdxIdentifier(measure.Name)}]";
                }

                var targetMeasure = new Amo.Measure
                {
                    ID = StableId(measure.Id),
                    Name = measure.Name,
                    Description = measure.Description,
                    AggregateFunction = MapAggregationFunction(measure.AggregateFunction),
                    DataType = MapMeasureDataType(measure.DataTypeId),
                    Source = CreateDataItem(
                        SourceName(row.SourceName, row.Name, row.Id),
                        SourceName(measure.SourceName, measure.Name, measure.Id),
                        MapMeasureSourceDataType(measure.DataTypeId)),
                    FormatString = measure.FormatString,
                    DisplayFolder = measure.DisplayFolder,
                    Visible = true,
                };
                measureGroup.Measures.Add(targetMeasure);
                measures[measure] = targetMeasure;
            }

            foreach (var usage in source.DimensionUsageList.Where(usage => ReferenceEquals(usage.MeasureGroup, row)))
            {
                var measureGroupDimension = new Amo.RegularMeasureGroupDimension(StableId(usage.CubeDimension.Id));
                if (usage.GranularityAttribute != null)
                {
                    var attribute = measureGroupDimension.Attributes.Add(StableId(usage.GranularityAttribute.Id));
                    attribute.Type = Amo.MeasureGroupAttributeType.Granularity;
                    attribute.KeyColumns.Add(CreateDataItem(
                        SourceName(row.SourceName, row.Name, row.Id),
                        SourceName(usage.GranularityAttribute.SourceName, usage.GranularityAttribute.Name, usage.GranularityAttribute.Id),
                        usage.GranularityAttribute.DataTypeId));
                }

                measureGroup.Dimensions.Add(measureGroupDimension);
            }

            var partitions = source.PartitionList
                .Where(partition => ReferenceEquals(partition.MeasureGroup, row))
                .OrderBy(partition => ParseInt(partition.Ordinal, int.MaxValue, "Partition.Ordinal"))
                .ToArray();
            if (partitions.Length == 0)
            {
                measureGroup.Partitions.Add(new Amo.Partition
                {
                    ID = $"{measureGroup.ID}_Partition",
                    Name = $"{measureGroup.Name} Partition",
                    StorageMode = Amo.StorageMode.Molap,
                    ProcessingMode = Amo.ProcessingMode.Regular,
                    StringStoresCompatibilityLevel = LargeStringStoresCompatibilityLevel,
                });
            }
            else
            {
                foreach (var partition in partitions)
                {
                    var targetPartition = new Amo.Partition
                    {
                        ID = StableId(partition.Id),
                        Name = partition.Name,
                        Description = partition.Description,
                        StorageMode = MapStorageMode(partition.StorageMode, "Partition.StorageMode"),
                        ProcessingMode = MapProcessingMode(partition.ProcessingMode, "Partition.ProcessingMode"),
                        StringStoresCompatibilityLevel = LargeStringStoresCompatibilityLevel,
                        Slice = partition.SliceExpression,
                    };
                    if (!string.IsNullOrWhiteSpace(partition.SourceExpression) &&
                        partition.MultiDimensionalDataSource != null &&
                        dataSources.TryGetValue(partition.MultiDimensionalDataSource, out var dataSource))
                    {
                        targetPartition.Source = new Amo.QueryBinding(dataSource.ID, partition.SourceExpression);
                    }

                    measureGroup.Partitions.Add(targetPartition);
                }
            }

            cube.MeasureGroups.Add(measureGroup);
            measureGroups[row] = measureGroup;
        }

        return new MeasureGroupTargets(measureGroups, measures);
    }

    private static Dictionary<Kpi, Amo.Kpi> AddKpis(
        MetaMultiDimensionalModel source,
        IReadOnlyDictionary<Cube, Amo.Cube> cubes)
    {
        var result = new Dictionary<Kpi, Amo.Kpi>();
        foreach (var row in source.KpiList)
        {
            if (!cubes.TryGetValue(row.Cube, out var cube))
            {
                throw new InvalidOperationException($"Kpi '{row.Id}' references cube '{row.Cube.Id}' that was not emitted to the target database.");
            }

            if (row.AssociatedMeasure != null &&
                !ReferenceEquals(row.AssociatedMeasure.MeasureGroup.Cube, row.Cube))
            {
                throw new InvalidOperationException($"Kpi '{row.Id}' associated measure '{row.AssociatedMeasure.Id}' must belong to the KPI cube.");
            }

            var kpi = new Amo.Kpi
            {
                ID = StableId(row.Id),
                Name = row.Name,
                Description = row.Description,
                Value = row.ValueExpression,
                Goal = row.GoalExpression,
                Status = row.StatusExpression,
                Trend = row.TrendExpression,
                StatusGraphic = row.StatusGraphic,
                TrendGraphic = row.TrendGraphic,
            };

            if (row.AssociatedMeasure != null)
            {
                kpi.AssociatedMeasureGroupID = StableId(row.AssociatedMeasure.MeasureGroup.Id);
            }

            cube.Kpis.Add(kpi);
            result[row] = kpi;
        }

        return result;
    }

    private static ScriptTargets AddMdxScripts(MetaMultiDimensionalModel source, IReadOnlyDictionary<Cube, Amo.Cube> cubes)
    {
        var namedSets = new Dictionary<NamedSet, Amo.CalculationProperty>();
        var calculations = new Dictionary<MdxCalculation, Amo.CalculationProperty>();
        foreach (var cubePair in cubes)
        {
            var cubeNamedSets = source.NamedSetList
                .Where(row => ReferenceEquals(row.Cube, cubePair.Key))
                .ToArray();
            var cubeCalculations = source.MdxCalculationList
                .Where(row => ReferenceEquals(row.Cube, cubePair.Key))
                .ToArray();
            var commands = new List<string>
            {
                "CALCULATE;",
            };
            commands.AddRange(cubeNamedSets.Select(row => $"CREATE SET CURRENTCUBE.[{EscapeMdxIdentifier(row.Name)}] AS {row.Expression};"));
            commands.AddRange(cubeCalculations.Select(row => row.Expression.EndsWith(";", StringComparison.Ordinal) ? row.Expression : row.Expression + ";"));

            var script = new Amo.MdxScript
            {
                ID = "MdxScript",
                Name = "MdxScript",
                DefaultScript = true,
            };
            script.Commands.Add(new Amo.Command(string.Join(Environment.NewLine, commands)));

            foreach (var row in cubeNamedSets)
            {
                var property = new Amo.CalculationProperty(row.Name, Amo.CalculationType.Set)
                {
                    Description = row.Description,
                    DisplayFolder = row.DisplayFolder,
                };
                script.CalculationProperties.Add(property);
                namedSets[row] = property;
            }

            foreach (var row in cubeCalculations)
            {
                var property = new Amo.CalculationProperty(row.Name, MapCalculationType(row.CalculationKind))
                {
                    Description = row.Description,
                    DisplayFolder = row.DisplayFolder,
                };
                if (!string.IsNullOrWhiteSpace(row.SolveOrder))
                {
                    property.SolveOrder = ParseInt(row.SolveOrder, 0, "MdxCalculation.SolveOrder");
                }

                script.CalculationProperties.Add(property);
                calculations[row] = property;
            }

            cubePair.Value.MdxScripts.Add(script);
        }

        return new ScriptTargets(namedSets, calculations);
    }

    private static Dictionary<CubeAction, Amo.Action> AddActions(MetaMultiDimensionalModel source, IReadOnlyDictionary<Cube, Amo.Cube> cubes)
    {
        var result = new Dictionary<CubeAction, Amo.Action>();
        foreach (var row in source.CubeActionList)
        {
            if (!cubes.TryGetValue(row.Cube, out var cube))
            {
                throw new InvalidOperationException($"CubeAction '{row.Id}' references cube '{row.Cube.Id}' that was not emitted to the target database.");
            }

            var action = CreateAction(row);
            cube.Actions.Add(action);
            result[row] = action;
        }

        return result;
    }

    private static Dictionary<Perspective, Amo.Perspective> AddPerspectives(
        MetaMultiDimensionalModel source,
        IReadOnlyDictionary<Cube, Amo.Cube> cubes,
        MeasureGroupTargets measureGroupTargets,
        IReadOnlyDictionary<Kpi, Amo.Kpi> kpis,
        IReadOnlyDictionary<CubeAction, Amo.Action> actions)
    {
        var perspectives = new Dictionary<Perspective, Amo.Perspective>();
        var fullMeasureGroups = new HashSet<(Perspective Perspective, MeasureGroup MeasureGroup)>();

        foreach (var row in source.PerspectiveList)
        {
            if (!cubes.TryGetValue(row.Cube, out var cube))
            {
                throw new InvalidOperationException($"Perspective '{row.Id}' references cube '{row.Cube.Id}' that was not emitted to the target database.");
            }

            var perspective = new Amo.Perspective
            {
                ID = StableId(row.Id),
                Name = row.Name,
                Description = row.Description,
            };
            if (!string.IsNullOrWhiteSpace(row.DefaultMeasureName))
            {
                perspective.DefaultMeasure = row.DefaultMeasureName;
            }

            cube.Perspectives.Add(perspective);
            perspectives[row] = perspective;
        }

        foreach (var row in source.PerspectiveDimensionList)
        {
            var perspective = RequirePerspective(perspectives, row.Perspective, $"PerspectiveDimension '{row.Id}'");
            RequireSameCube(row.Perspective, row.CubeDimension.Cube, $"PerspectiveDimension '{row.Id}'");
            var cube = cubes[row.Perspective.Cube];
            var cubeDimensionId = StableId(row.CubeDimension.Id);
            if (!cube.Dimensions.Cast<Amo.CubeDimension>().Any(item => string.Equals(item.ID, cubeDimensionId, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException($"PerspectiveDimension '{row.Id}' references cube dimension '{row.CubeDimension.Id}' that was not emitted to the target cube.");
            }

            if (perspective.Dimensions.Cast<Amo.PerspectiveDimension>().Any(item => string.Equals(item.CubeDimensionID, cubeDimensionId, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException($"Perspective '{row.Perspective.Id}' contains duplicate dimension membership for cube dimension '{row.CubeDimension.Id}'.");
            }

            perspective.Dimensions.Add(cubeDimensionId);
        }

        foreach (var row in source.PerspectiveMeasureGroupList)
        {
            var perspective = RequirePerspective(perspectives, row.Perspective, $"PerspectiveMeasureGroup '{row.Id}'");
            RequireSameCube(row.Perspective, row.MeasureGroup.Cube, $"PerspectiveMeasureGroup '{row.Id}'");
            var targetMeasureGroup = RequireMeasureGroup(measureGroupTargets.MeasureGroups, row.MeasureGroup, $"PerspectiveMeasureGroup '{row.Id}'");
            if (perspective.MeasureGroups.Cast<Amo.PerspectiveMeasureGroup>().Any(item => string.Equals(item.MeasureGroupID, targetMeasureGroup.ID, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException($"Perspective '{row.Perspective.Id}' contains duplicate full measure-group membership for measure group '{row.MeasureGroup.Id}'.");
            }

            perspective.MeasureGroups.Add(targetMeasureGroup.ID);
            fullMeasureGroups.Add((row.Perspective, row.MeasureGroup));
        }

        foreach (var row in source.PerspectiveMeasureList)
        {
            var perspective = RequirePerspective(perspectives, row.Perspective, $"PerspectiveMeasure '{row.Id}'");
            RequireSameCube(row.Perspective, row.Measure.MeasureGroup.Cube, $"PerspectiveMeasure '{row.Id}'");
            var targetMeasure = RequireMeasure(measureGroupTargets.Measures, row.Measure, $"PerspectiveMeasure '{row.Id}'");
            var targetMeasureGroup = RequireMeasureGroup(measureGroupTargets.MeasureGroups, row.Measure.MeasureGroup, $"PerspectiveMeasure '{row.Id}'");
            if (fullMeasureGroups.Contains((row.Perspective, row.Measure.MeasureGroup)))
            {
                continue;
            }

            var perspectiveMeasureGroup = EnsurePerspectiveMeasureGroup(perspective, targetMeasureGroup);
            if (perspectiveMeasureGroup.Measures.Cast<Amo.PerspectiveMeasure>().Any(item => string.Equals(item.MeasureID, targetMeasure.ID, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException($"Perspective '{row.Perspective.Id}' contains duplicate measure membership for measure '{row.Measure.Id}'.");
            }

            perspectiveMeasureGroup.Measures.Add(targetMeasure.ID);
        }

        foreach (var row in source.PerspectiveCalculationList)
        {
            var perspective = RequirePerspective(perspectives, row.Perspective, $"PerspectiveCalculation '{row.Id}'");
            RequireSameCube(row.Perspective, row.MdxCalculation.Cube, $"PerspectiveCalculation '{row.Id}'");
            if (!cubes.ContainsKey(row.MdxCalculation.Cube))
            {
                throw new InvalidOperationException($"PerspectiveCalculation '{row.Id}' references calculation '{row.MdxCalculation.Id}' that was not emitted to the target cube.");
            }

            AddPerspectiveCalculation(
                perspective,
                row.Perspective.Id,
                row.MdxCalculation.Name,
                MapPerspectiveCalculationType(row.MdxCalculation.CalculationKind),
                row.MdxCalculation.Id);
        }

        foreach (var row in source.PerspectiveKpiList)
        {
            var perspective = RequirePerspective(perspectives, row.Perspective, $"PerspectiveKpi '{row.Id}'");
            RequireSameCube(row.Perspective, row.Kpi.Cube, $"PerspectiveKpi '{row.Id}'");
            if (!kpis.TryGetValue(row.Kpi, out var targetKpi))
            {
                throw new InvalidOperationException($"PerspectiveKpi '{row.Id}' references KPI '{row.Kpi.Id}' that was not emitted to the target cube.");
            }

            if (perspective.Kpis.Cast<Amo.PerspectiveKpi>().Any(item => string.Equals(item.KpiID, targetKpi.ID, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException($"Perspective '{row.Perspective.Id}' contains duplicate KPI membership for KPI '{row.Kpi.Id}'.");
            }

            perspective.Kpis.Add(targetKpi.ID);
        }

        foreach (var row in source.PerspectiveNamedSetList)
        {
            var perspective = RequirePerspective(perspectives, row.Perspective, $"PerspectiveNamedSet '{row.Id}'");
            RequireSameCube(row.Perspective, row.NamedSet.Cube, $"PerspectiveNamedSet '{row.Id}'");
            if (!cubes.ContainsKey(row.NamedSet.Cube))
            {
                throw new InvalidOperationException($"PerspectiveNamedSet '{row.Id}' references named set '{row.NamedSet.Id}' that was not emitted to the target cube.");
            }

            AddPerspectiveCalculation(
                perspective,
                row.Perspective.Id,
                row.NamedSet.Name,
                Amo.PerspectiveCalculationType.Set,
                row.NamedSet.Id);
        }

        foreach (var row in source.PerspectiveActionList)
        {
            var perspective = RequirePerspective(perspectives, row.Perspective, $"PerspectiveAction '{row.Id}'");
            RequireSameCube(row.Perspective, row.CubeAction.Cube, $"PerspectiveAction '{row.Id}'");
            if (!actions.TryGetValue(row.CubeAction, out var targetAction))
            {
                throw new InvalidOperationException($"PerspectiveAction '{row.Id}' references action '{row.CubeAction.Id}' that was not emitted to the target cube.");
            }

            if (perspective.Actions.Cast<Amo.PerspectiveAction>().Any(item => string.Equals(item.ActionID, targetAction.ID, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException($"Perspective '{row.Perspective.Id}' contains duplicate action membership for action '{row.CubeAction.Id}'.");
            }

            perspective.Actions.Add(targetAction.ID);
        }

        return perspectives;
    }

    private static Amo.Perspective RequirePerspective(
        IReadOnlyDictionary<Perspective, Amo.Perspective> perspectives,
        Perspective sourcePerspective,
        string context)
    {
        return perspectives.TryGetValue(sourcePerspective, out var perspective)
            ? perspective
            : throw new InvalidOperationException($"{context} references perspective '{sourcePerspective.Id}' that was not emitted to the target cube.");
    }

    private static Amo.MeasureGroup RequireMeasureGroup(
        IReadOnlyDictionary<MeasureGroup, Amo.MeasureGroup> measureGroups,
        MeasureGroup sourceMeasureGroup,
        string context)
    {
        return measureGroups.TryGetValue(sourceMeasureGroup, out var measureGroup)
            ? measureGroup
            : throw new InvalidOperationException($"{context} references measure group '{sourceMeasureGroup.Id}' that was not emitted to the target cube.");
    }

    private static Amo.Measure RequireMeasure(
        IReadOnlyDictionary<Measure, Amo.Measure> measures,
        Measure sourceMeasure,
        string context)
    {
        return measures.TryGetValue(sourceMeasure, out var measure)
            ? measure
            : throw new InvalidOperationException($"{context} references measure '{sourceMeasure.Id}' that was not emitted to the target cube.");
    }

    private static Amo.PerspectiveMeasureGroup EnsurePerspectiveMeasureGroup(
        Amo.Perspective perspective,
        Amo.MeasureGroup measureGroup)
    {
        var existing = perspective.MeasureGroups
            .Cast<Amo.PerspectiveMeasureGroup>()
            .FirstOrDefault(item => string.Equals(item.MeasureGroupID, measureGroup.ID, StringComparison.Ordinal));
        return existing ?? perspective.MeasureGroups.Add(measureGroup.ID);
    }

    private static void AddPerspectiveCalculation(
        Amo.Perspective perspective,
        string sourcePerspectiveId,
        string name,
        Amo.PerspectiveCalculationType type,
        string sourceCalculationId)
    {
        if (perspective.Calculations.Cast<Amo.PerspectiveCalculation>().Any(item => string.Equals(item.Name, name, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException($"Perspective '{sourcePerspectiveId}' contains duplicate calculation membership for calculation '{sourceCalculationId}'.");
        }

        perspective.Calculations.Add(name, type);
    }

    private static void RequireSameCube(Perspective perspective, Cube owner, string context)
    {
        if (!ReferenceEquals(perspective.Cube, owner))
        {
            throw new InvalidOperationException($"{context} references an item outside perspective cube '{perspective.Cube.Id}'.");
        }
    }

    private static void AddTranslations(
        MetaMultiDimensionalModel source,
        MultiDimensionalDatabase root,
        IReadOnlyDictionary<Cube, Amo.Cube> cubes,
        DimensionTargets dimensionTargets,
        MeasureGroupTargets measureGroupTargets,
        IReadOnlyDictionary<Kpi, Amo.Kpi> kpis,
        IReadOnlyDictionary<CubeAction, Amo.Action> actions,
        IReadOnlyDictionary<Perspective, Amo.Perspective> perspectives,
        ScriptTargets scriptTargets)
    {
        var cultures = ResolveCultures(source, root);

        foreach (var row in source.CubeTranslationList)
        {
            var language = RequireCulture(cultures, row.Culture, $"CubeTranslation '{row.Id}'");
            var cube = RequireCube(cubes, row.Cube, $"CubeTranslation '{row.Id}'");
            AddTranslation(cube.Translations, language, row.Caption, row.Description, $"CubeTranslation '{row.Id}'");
        }

        foreach (var row in source.DimensionTranslationList)
        {
            var language = RequireCulture(cultures, row.Culture, $"DimensionTranslation '{row.Id}'");
            var dimension = RequireDimension(dimensionTargets.Dimensions, row.Dimension, $"DimensionTranslation '{row.Id}'");
            AddTranslation(dimension.Translations, language, row.Caption, row.Description, $"DimensionTranslation '{row.Id}'");
        }

        foreach (var row in source.AttributeTranslationList)
        {
            var language = RequireCulture(cultures, row.Culture, $"AttributeTranslation '{row.Id}'");
            var attribute = RequireDimensionAttribute(dimensionTargets.Attributes, row.DimensionAttribute, $"AttributeTranslation '{row.Id}'");
            AddAttributeTranslation(attribute.Translations, language, row.Caption, row.Description, $"AttributeTranslation '{row.Id}'");
        }

        foreach (var row in source.MeasureTranslationList)
        {
            var language = RequireCulture(cultures, row.Culture, $"MeasureTranslation '{row.Id}'");
            var measure = RequireMeasure(measureGroupTargets.Measures, row.Measure, $"MeasureTranslation '{row.Id}'");
            AddTranslation(measure.Translations, language, row.Caption, row.Description, $"MeasureTranslation '{row.Id}'");
        }

        foreach (var row in source.PerspectiveTranslationList)
        {
            var language = RequireCulture(cultures, row.Culture, $"PerspectiveTranslation '{row.Id}'");
            var perspective = RequirePerspective(perspectives, row.Perspective, $"PerspectiveTranslation '{row.Id}'");
            AddTranslation(perspective.Translations, language, row.Caption, row.Description, $"PerspectiveTranslation '{row.Id}'");
        }

        foreach (var row in source.KpiTranslationList)
        {
            var language = RequireCulture(cultures, row.Culture, $"KpiTranslation '{row.Id}'");
            if (!kpis.TryGetValue(row.Kpi, out var kpi))
            {
                throw new InvalidOperationException($"KpiTranslation '{row.Id}' references KPI '{row.Kpi.Id}' that was not emitted to the target cube.");
            }

            AddTranslation(kpi.Translations, language, row.Caption, row.Description, $"KpiTranslation '{row.Id}'");
        }

        foreach (var row in source.ActionTranslationList)
        {
            var language = RequireCulture(cultures, row.Culture, $"ActionTranslation '{row.Id}'");
            if (!actions.TryGetValue(row.CubeAction, out var action))
            {
                throw new InvalidOperationException($"ActionTranslation '{row.Id}' references action '{row.CubeAction.Id}' that was not emitted to the target cube.");
            }

            AddTranslation(action.Translations, language, row.Caption, row.Description, $"ActionTranslation '{row.Id}'");
        }

        foreach (var row in source.NamedSetTranslationList)
        {
            var language = RequireCulture(cultures, row.Culture, $"NamedSetTranslation '{row.Id}'");
            if (!scriptTargets.NamedSets.TryGetValue(row.NamedSet, out var namedSet))
            {
                throw new InvalidOperationException($"NamedSetTranslation '{row.Id}' references named set '{row.NamedSet.Id}' that was not emitted to the target cube.");
            }

            AddTranslation(namedSet.Translations, language, row.Caption, row.Description, $"NamedSetTranslation '{row.Id}'");
        }
    }

    private static Dictionary<Culture, int> ResolveCultures(MetaMultiDimensionalModel source, MultiDimensionalDatabase root)
    {
        var result = new Dictionary<Culture, int>();
        var languages = new HashSet<int>();
        foreach (var row in source.CultureList.Where(row => ReferenceEquals(row.MultiDimensionalDatabase, root)))
        {
            var language = ParseCultureLanguage(row);
            if (!languages.Add(language))
            {
                throw new InvalidOperationException($"MultiDimensionalDatabase '{root.Id}' has more than one Culture row for language '{language}'.");
            }

            result[row] = language;
        }

        return result;
    }

    private static int RequireCulture(IReadOnlyDictionary<Culture, int> cultures, Culture sourceCulture, string context)
    {
        return cultures.TryGetValue(sourceCulture, out var language)
            ? language
            : throw new InvalidOperationException($"{context} references culture '{sourceCulture.Id}' that is not available for the target database.");
    }

    private static Amo.Cube RequireCube(IReadOnlyDictionary<Cube, Amo.Cube> cubes, Cube sourceCube, string context)
    {
        return cubes.TryGetValue(sourceCube, out var cube)
            ? cube
            : throw new InvalidOperationException($"{context} references cube '{sourceCube.Id}' that was not emitted to the target database.");
    }

    private static Amo.Dimension RequireDimension(
        IReadOnlyDictionary<Dimension, Amo.Dimension> dimensions,
        Dimension sourceDimension,
        string context)
    {
        return dimensions.TryGetValue(sourceDimension, out var dimension)
            ? dimension
            : throw new InvalidOperationException($"{context} references dimension '{sourceDimension.Id}' that was not emitted to the target database.");
    }

    private static Amo.DimensionAttribute RequireDimensionAttribute(
        IReadOnlyDictionary<DimensionAttribute, Amo.DimensionAttribute> attributes,
        DimensionAttribute sourceAttribute,
        string context)
    {
        return attributes.TryGetValue(sourceAttribute, out var attribute)
            ? attribute
            : throw new InvalidOperationException($"{context} references attribute '{sourceAttribute.Id}' that was not emitted to the target dimension.");
    }

    private static void AddTranslation(
        Amo.TranslationCollection translations,
        int language,
        string? caption,
        string? description,
        string context)
    {
        if (translations.Cast<Amo.Translation>().Any(item => item.Language == language))
        {
            throw new InvalidOperationException($"{context} duplicates translation language '{language}'.");
        }

        translations.Add(new Amo.Translation(language)
        {
            Caption = caption,
            Description = description,
        });
    }

    private static void AddAttributeTranslation(
        Amo.AttributeTranslationCollection translations,
        int language,
        string? caption,
        string? description,
        string context)
    {
        if (translations.Cast<Amo.AttributeTranslation>().Any(item => item.Language == language))
        {
            throw new InvalidOperationException($"{context} duplicates translation language '{language}'.");
        }

        translations.Add(new Amo.AttributeTranslation(language)
        {
            Caption = caption,
            Description = description,
        });
    }

    private static Amo.Action CreateAction(CubeAction row)
    {
        Amo.Action action;
        if (row.ActionType.Equals("DrillThrough", StringComparison.OrdinalIgnoreCase) ||
            row.ActionType.Equals("Drillthrough", StringComparison.OrdinalIgnoreCase))
        {
            action = new Amo.DrillThroughAction
            {
                Type = Amo.ActionType.DrillThrough,
            };
        }
        else
        {
            action = new Amo.StandardAction
            {
                Expression = row.Expression,
                Type = MapActionType(row.ActionType),
            };
        }

        action.ID = StableId(row.Id);
        action.Name = row.Name;
        action.Caption = row.Caption ?? row.Name;
        action.Description = row.Description;
        action.TargetType = MapActionTargetType(row.TargetKind);
        if (!string.IsNullOrWhiteSpace(row.Target))
        {
            action.Target = row.Target;
        }

        return action;
    }

    private static void AddRoles(
        Amo.Database target,
        MetaMultiDimensionalModel source,
        MultiDimensionalDatabase root,
        IReadOnlyDictionary<Cube, Amo.Cube> cubes,
        DimensionTargets dimensionTargets)
    {
        var roles = new Dictionary<SecurityRole, Amo.Role>();
        foreach (var row in source.SecurityRoleList.Where(row => ReferenceEquals(row.MultiDimensionalDatabase, root)))
        {
            var role = new Amo.Role
            {
                ID = StableId(row.Id),
                Name = row.Name,
                Description = row.Description,
            };
            foreach (var member in source.RoleMemberList.Where(member => ReferenceEquals(member.SecurityRole, row)))
            {
                role.Members.Add(string.IsNullOrWhiteSpace(member.MemberSid)
                    ? new Amo.RoleMember(member.MemberName)
                    : new Amo.RoleMember(member.MemberName, member.MemberSid));
            }

            target.Roles.Add(role);
            roles[row] = role;
            target.DatabasePermissions.Add(new Amo.DatabasePermission(role.ID, $"{row.Name} Database Permission", $"{role.ID}_database_permission")
            {
                Read = MapReadAccess(row.Permission),
                ReadDefinition = Amo.ReadDefinitionAccess.Allowed,
            });
        }

        AddDimensionPermissions(source, roles, dimensionTargets);
        AddCellPermissions(source, roles, cubes);
    }

    private static void AddDimensionPermissions(
        MetaMultiDimensionalModel source,
        IReadOnlyDictionary<SecurityRole, Amo.Role> roles,
        DimensionTargets dimensionTargets)
    {
        var dimensionPermissions = new Dictionary<(SecurityRole Role, Dimension Dimension), Amo.DimensionPermission>();
        foreach (var row in source.DimensionPermissionList)
        {
            var role = RequireRole(roles, row.SecurityRole, $"DimensionPermission '{row.Id}'");
            var dimension = RequireDimension(dimensionTargets.Dimensions, row.Dimension, $"DimensionPermission '{row.Id}'");
            var attribute = RequireDimensionAttribute(dimensionTargets.Attributes, row.DimensionAttribute, $"DimensionPermission '{row.Id}'");
            if (!ReferenceEquals(row.DimensionAttribute.Dimension, row.Dimension))
            {
                throw new InvalidOperationException($"DimensionPermission '{row.Id}' references attribute '{row.DimensionAttribute.Id}' outside dimension '{row.Dimension.Id}'.");
            }

            var permissionKey = (row.SecurityRole, row.Dimension);
            if (!dimensionPermissions.TryGetValue(permissionKey, out var dimensionPermission))
            {
                var permissionId = $"{role.ID}_{dimension.ID}_permission";
                dimensionPermission = new Amo.DimensionPermission(
                    role.ID,
                    $"{row.SecurityRole.Name} {row.Dimension.Name} Permission",
                    permissionId);
                dimension.DimensionPermissions.Add(dimensionPermission);
                dimensionPermissions[permissionKey] = dimensionPermission;
            }

            if (dimensionPermission.AttributePermissions.Cast<Amo.AttributePermission>().Any(item => string.Equals(item.AttributeID, attribute.ID, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException($"DimensionPermission '{row.Id}' duplicates attribute permission for attribute '{row.DimensionAttribute.Id}'.");
            }

            dimensionPermission.AttributePermissions.Add(new Amo.AttributePermission(attribute.ID)
            {
                AllowedSet = row.AllowedSetExpression,
                DeniedSet = row.DeniedSetExpression,
                DefaultMember = row.DefaultMemberExpression,
                VisualTotals = row.VisualTotals,
                Description = row.Description,
            });
        }
    }

    private static void AddCellPermissions(
        MetaMultiDimensionalModel source,
        IReadOnlyDictionary<SecurityRole, Amo.Role> roles,
        IReadOnlyDictionary<Cube, Amo.Cube> cubes)
    {
        var cubePermissions = new Dictionary<(SecurityRole Role, Cube Cube), Amo.CubePermission>();
        foreach (var row in source.CellPermissionList)
        {
            var role = RequireRole(roles, row.SecurityRole, $"CellPermission '{row.Id}'");
            var cube = RequireCube(cubes, row.Cube, $"CellPermission '{row.Id}'");
            var permissionKey = (row.SecurityRole, row.Cube);
            if (!cubePermissions.TryGetValue(permissionKey, out var cubePermission))
            {
                var permissionId = $"{role.ID}_{StableId(row.Cube.Id)}_permission";
                cubePermission = new Amo.CubePermission(role.ID, $"{row.SecurityRole.Name} {row.Cube.Name} Permission", permissionId)
                {
                    Read = MapReadAccess(row.SecurityRole.Permission),
                    ReadDefinition = Amo.ReadDefinitionAccess.Allowed,
                };
                cube.CubePermissions.Add(cubePermission);
                cubePermissions[permissionKey] = cubePermission;
            }

            cubePermission.CellPermissions.Add(new Amo.CellPermission(Amo.CellPermissionAccess.Read, row.Expression));
        }
    }

    private static Amo.Role RequireRole(
        IReadOnlyDictionary<SecurityRole, Amo.Role> roles,
        SecurityRole sourceRole,
        string context)
    {
        return roles.TryGetValue(sourceRole, out var role)
            ? role
            : throw new InvalidOperationException($"{context} references role '{sourceRole.Id}' that was not emitted to the target database.");
    }

    private static MultiDimensionalDatabase RequireSingleDatabase(MetaMultiDimensionalModel model)
    {
        return model.MultiDimensionalDatabaseList.Count switch
        {
            1 => model.MultiDimensionalDatabaseList[0],
            0 => throw new InvalidOperationException("MetaMultiDimensional deploy requires exactly one MultiDimensionalDatabase row. Found none."),
            _ => throw new InvalidOperationException($"MetaMultiDimensional deploy requires exactly one MultiDimensionalDatabase row. Found {model.MultiDimensionalDatabaseList.Count}."),
        };
    }

    private static Amo.Database? FindDatabase(Amo.Server server, string databaseName)
    {
        return server.Databases
            .OfType<Amo.Database>()
            .FirstOrDefault(database =>
                string.Equals(database.ID, databaseName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(database.Name, databaseName, StringComparison.OrdinalIgnoreCase));
    }

    private static int ParseLanguage(string? language)
    {
        return GetCulture(language).LCID;
    }

    private static CultureInfo GetCulture(string? language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? CultureInfo.GetCultureInfo("en-US")
            : CultureInfo.GetCultureInfo(language);
    }

    private static DataTable EnsureTable(DataSet dataSet, string tableName)
    {
        var existing = dataSet.Tables[tableName];
        if (existing != null)
        {
            return existing;
        }

        var table = new DataTable(tableName)
        {
            Locale = dataSet.Locale,
        };
        dataSet.Tables.Add(table);
        return table;
    }

    private static void EnsureColumn(DataTable table, string columnName, Type dataType)
    {
        if (table.Columns.Contains(columnName))
        {
            return;
        }

        table.Columns.Add(new DataColumn(columnName, dataType));
    }

    private static Amo.DataItem CreateDataItem(string tableName, string columnName, string? dataTypeId)
    {
        return new Amo.DataItem(tableName, columnName, MapAmoDataType(dataTypeId));
    }

    private static Amo.DataItem CreateDataItem(string tableName, string columnName, Amo.DataType dataType)
    {
        return new Amo.DataItem(tableName, columnName, dataType);
    }

    private static string SourceName(string? sourceName, string name, string id)
    {
        if (!string.IsNullOrWhiteSpace(sourceName))
        {
            return sourceName.Trim();
        }

        return !string.IsNullOrWhiteSpace(name)
            ? name.Trim()
            : StableId(id);
    }

    private static string AttributeNameColumnName(string sourceColumnName, string? dataTypeId)
    {
        return MapAmoDataType(dataTypeId) == Amo.DataType.String
            ? sourceColumnName
            : $"{sourceColumnName}_Name";
    }

    private static Type MapClrDataType(string? dataTypeId)
    {
        var value = StripMetaTypePrefix(dataTypeId).ToUpperInvariant();
        return value switch
        {
            "BOOLEAN" or "BOOL" => typeof(bool),
            "DATETIME" or "DATE" or "TIME" => typeof(DateTime),
            "DECIMAL" or "MONEY" or "NUMERIC" => typeof(decimal),
            "DOUBLE" or "FLOAT" or "REAL" => typeof(double),
            "INT16" => typeof(short),
            "INT32" or "INTEGER" => typeof(int),
            "INT64" or "LONG" => typeof(long),
            "BINARY" => typeof(byte[]),
            _ => typeof(string),
        };
    }

    private static Type MapMeasureClrDataType(string? dataTypeId)
    {
        var value = StripMetaTypePrefix(dataTypeId).ToUpperInvariant();
        return value switch
        {
            "DECIMAL" or "NUMERIC" => typeof(double),
            _ => MapClrDataType(dataTypeId),
        };
    }

    private static Amo.DataType MapAmoDataType(string? dataTypeId)
    {
        var value = StripMetaTypePrefix(dataTypeId).ToUpperInvariant();
        return value switch
        {
            "BOOLEAN" or "BOOL" => Amo.DataType.Boolean,
            "DATETIME" or "DATE" or "TIME" => Amo.DataType.DateTime,
            "DECIMAL" or "MONEY" or "NUMERIC" => Amo.DataType.Decimal,
            "DOUBLE" or "FLOAT" or "REAL" => Amo.DataType.Double,
            "INT16" => Amo.DataType.Int16,
            "INT32" or "INTEGER" => Amo.DataType.Int32,
            "INT64" or "LONG" => Amo.DataType.Int64,
            "BINARY" => Amo.DataType.Binary,
            _ => Amo.DataType.String,
        };
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

    private static string MapManagedProvider(string? provider)
    {
        return provider?.Trim().ToUpperInvariant() switch
        {
            "SQLSERVER" or "SQLCLIENT" or "SYSTEM.DATA.SQLCLIENT" => "System.Data.SqlClient",
            _ => "System.Data.SqlClient",
        };
    }

    private static Amo.DimensionType MapDimensionType(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Amo.DimensionType.Regular
            : value.Equals("Time", StringComparison.OrdinalIgnoreCase)
                ? Amo.DimensionType.Time
                : ParseEnum<Amo.DimensionType>(value, "Dimension.DimensionType");
    }

    private static Amo.DimensionStorageMode MapDimensionStorageMode(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Amo.DimensionStorageMode.Molap
            : ParseEnum<Amo.DimensionStorageMode>(value, "Dimension.StorageMode");
    }

    private static Amo.ProcessingGroup MapProcessingGroup(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Amo.ProcessingGroup.ByAttribute
            : ParseEnum<Amo.ProcessingGroup>(value, "Dimension.ProcessingGroup");
    }

    private static Amo.AttributeUsage MapAttributeUsage(string? usage, string? isKey)
    {
        if (ParseBool(isKey))
        {
            return Amo.AttributeUsage.Key;
        }

        return string.IsNullOrWhiteSpace(usage)
            ? Amo.AttributeUsage.Regular
            : ParseEnum<Amo.AttributeUsage>(usage, "DimensionAttribute.Usage");
    }

    private static Amo.RelationshipType MapAttributeRelationshipType(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Amo.RelationshipType.Flexible
            : ParseEnum<Amo.RelationshipType>(value, "AttributeRelationship.RelationshipType");
    }

    private static Amo.HierarchyStructureType MapHierarchyStructureType(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Amo.HierarchyStructureType.Unknown
            : ParseEnum<Amo.HierarchyStructureType>(value, "DimensionHierarchy.HierarchyType");
    }

    private static Amo.AggregationFunction MapAggregationFunction(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Amo.AggregationFunction.Sum
            : value.Equals("DistinctCount", StringComparison.OrdinalIgnoreCase) ||
              value.Equals("Distinct_Count", StringComparison.OrdinalIgnoreCase)
                ? Amo.AggregationFunction.DistinctCount
                : ParseEnum<Amo.AggregationFunction>(value, "Measure.AggregateFunction");
    }

    private static Amo.MeasureDataType MapMeasureDataType(string? dataTypeId)
    {
        var value = StripMetaTypePrefix(dataTypeId).ToUpperInvariant();
        return value switch
        {
            "BOOLEAN" or "BOOL" => Amo.MeasureDataType.Boolean,
            "DATETIME" or "DATE" or "TIME" => Amo.MeasureDataType.Date,
            "MONEY" => Amo.MeasureDataType.Currency,
            "DECIMAL" or "NUMERIC" => Amo.MeasureDataType.Double,
            "DOUBLE" or "FLOAT" or "REAL" => Amo.MeasureDataType.Double,
            "INT16" => Amo.MeasureDataType.SmallInt,
            "INT32" or "INTEGER" => Amo.MeasureDataType.Integer,
            "INT64" or "LONG" => Amo.MeasureDataType.BigInt,
            _ => Amo.MeasureDataType.Double,
        };
    }

    private static Amo.DataType MapMeasureSourceDataType(string? dataTypeId)
    {
        var value = StripMetaTypePrefix(dataTypeId).ToUpperInvariant();
        return value switch
        {
            "DECIMAL" or "NUMERIC" => Amo.DataType.Double,
            _ => MapAmoDataType(dataTypeId),
        };
    }

    private static Amo.StorageMode MapStorageMode(string? value, string context)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Amo.StorageMode.Molap
            : ParseEnum<Amo.StorageMode>(value, context);
    }

    private static Amo.ProcessingMode MapProcessingMode(string? value, string context)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Amo.ProcessingMode.Regular
            : ParseEnum<Amo.ProcessingMode>(value, context);
    }

    private static Amo.ActionType MapActionType(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Amo.ActionType.Statement
            : ParseEnum<Amo.ActionType>(value, "CubeAction.ActionType");
    }

    private static Amo.ActionTargetType MapActionTargetType(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Amo.ActionTargetType.Cells
            : ParseEnum<Amo.ActionTargetType>(value, "CubeAction.TargetKind");
    }

    private static Amo.CalculationType MapCalculationType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Amo.CalculationType.Member;
        }

        var normalized = value.Trim()
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .ToUpperInvariant();
        return normalized switch
        {
            "MEMBER" or "CALCULATEDMEMBER" => Amo.CalculationType.Member,
            "SET" or "NAMEDSET" => Amo.CalculationType.Set,
            "CELL" or "CELLS" or "CALCULATEDCELLS" => Amo.CalculationType.Cells,
            _ => ParseEnum<Amo.CalculationType>(value, "MdxCalculation.CalculationKind"),
        };
    }

    private static Amo.PerspectiveCalculationType MapPerspectiveCalculationType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Amo.PerspectiveCalculationType.Member;
        }

        var normalized = value.Trim()
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .ToUpperInvariant();
        return normalized switch
        {
            "MEMBER" or "CALCULATEDMEMBER" => Amo.PerspectiveCalculationType.Member,
            "SET" or "NAMEDSET" => Amo.PerspectiveCalculationType.Set,
            _ => ParseEnum<Amo.PerspectiveCalculationType>(value, "MdxCalculation.CalculationKind"),
        };
    }

    private static int ParseCultureLanguage(Culture row)
    {
        var value = string.IsNullOrWhiteSpace(row.LanguageId)
            ? row.Name
            : row.LanguageId;
        if (int.TryParse(value, out var language))
        {
            try
            {
                return CultureInfo.GetCultureInfo(language).LCID;
            }
            catch (Exception ex) when (ex is CultureNotFoundException or ArgumentOutOfRangeException)
            {
                var propertyName = string.IsNullOrWhiteSpace(row.LanguageId)
                    ? "Culture.Name"
                    : "Culture.LanguageId";
                throw new InvalidOperationException($"{propertyName} value '{value}' is not a valid culture name or LCID.", ex);
            }
        }

        try
        {
            return CultureInfo.GetCultureInfo(value).LCID;
        }
        catch (CultureNotFoundException ex)
        {
            var propertyName = string.IsNullOrWhiteSpace(row.LanguageId)
                ? "Culture.Name"
                : "Culture.LanguageId";
            throw new InvalidOperationException($"{propertyName} value '{value}' is not a valid culture name or LCID.", ex);
        }
    }

    private static Amo.ReadAccess MapReadAccess(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Amo.ReadAccess.Allowed
            : ParseEnum<Amo.ReadAccess>(value, "SecurityRole.Permission");
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

    private static string StableId(string value)
    {
        var chars = value.Select(ch => char.IsLetterOrDigit(ch) || ch == '_' ? ch : '_').ToArray();
        var result = new string(chars);
        return string.IsNullOrWhiteSpace(result) ? "Object" : result;
    }

    private static string EscapeMdxIdentifier(string value)
    {
        return value.Replace("]", "]]", StringComparison.Ordinal);
    }

    private static string StripMetaTypePrefix(string? value)
    {
        const string prefix = "meta:type:";
        return !string.IsNullOrWhiteSpace(value) && value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? value[prefix.Length..]
            : value?.Trim() ?? string.Empty;
    }
}
