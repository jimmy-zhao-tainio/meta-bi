using MetaSchema;
using MetaTransform.Binding;
using MetaTransformScript;
using MetaTransformScript.Sql.Parsing;

public sealed class TransformBindingTpcDsValidatedCorpusTests
{
    [Fact]
    public void TpcDsBindingAndValidation_IsValidatedOrExpectedGap()
    {
        var sourceViewsRoot = GetTpcDsSourceViewsRoot();
        var sqlFiles = Directory.GetFiles(sourceViewsRoot, "view.sql", SearchOption.AllDirectories)
            .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.Equal(99, sqlFiles.Length);

        var expectedGapCodes = new HashSet<string>(StringComparer.Ordinal)
        {
            "SelectStarRequiresValidationSchema",
            "GroupedSelectStarNotSupported",
            "FunctionTableReferenceColumnAliasesRequired",
            "GlobalFunctionTableReferenceOutputShapeNotSupported",
            "UnsupportedTableReferenceShape",
            "UnsupportedScalarExpressionShape",
            "UnsupportedBooleanExpressionShape",
            "UnsupportedGroupingExpressionShape",
            "UnsupportedGroupingSpecificationShape",
            "UnsupportedSelectOutputName",
            "UngroupedColumnReference",
            "ColumnReferenceRequiresValidationSchema",
            "ColumnReferenceNotFound",
            "ColumnReferenceAmbiguous",
            "ColumnQualifierNotFound",
            "SelectStarQualifierNotFound",
            "SetOperationColumnCountMismatch",
            "SetOperationColumnNameMismatch",
            "SubqueryOutputColumnCountMismatch"
        };

        var unknownFailures = new List<string>();
        var validatedCount = 0;
        var expectedGapCount = 0;
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.TpcDs.Validate.Tests", Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(tempRoot);

        try
        {
            foreach (var sqlPath in sqlFiles)
            {
                var scenario = Path.GetFileName(Path.GetDirectoryName(sqlPath)) ?? sqlPath;
                MetaTransformScriptModel model;
                try
                {
                    model = new MetaTransformScriptSqlParser().ParseSqlCode(File.ReadAllText(sqlPath));
                }
                catch (Exception ex)
                {
                    unknownFailures.Add($"{scenario}: parser failed with {ex.GetType().Name}: {ex.Message}");
                    continue;
                }

                if (model.TransformScriptList.Count != 1)
                {
                    unknownFailures.Add($"{scenario}: parser produced {model.TransformScriptList.Count} transform scripts.");
                    continue;
                }

                var script = model.TransformScriptList[0];
                if (string.IsNullOrWhiteSpace(script.TargetSqlIdentifier))
                {
                    script.TargetSqlIdentifier = script.Name;
                }

                var bound = new TransformBindingService().BindSingleTransform(model);
                if (bound.HasErrors)
                {
                    var issueCodes = bound.Issues
                        .Select(item => item.Code)
                        .Distinct(StringComparer.Ordinal)
                        .OrderBy(static item => item, StringComparer.Ordinal)
                        .ToArray();

                    var unexpected = issueCodes
                        .Where(code => !expectedGapCodes.Contains(code))
                        .ToArray();

                    if (unexpected.Length == 0)
                    {
                        expectedGapCount++;
                        continue;
                    }

                    unknownFailures.Add(
                        $"{scenario}: unexpected issue codes [{string.Join(", ", unexpected)}], all codes [{string.Join(", ", issueCodes)}]");
                    continue;
                }

                try
                {
                    var schemaModel = BuildSyntheticSchemaModel(bound, script.TargetSqlIdentifier);
                    var scenarioRoot = Path.Combine(tempRoot, scenario);
                    var transformWorkspacePath = Path.Combine(scenarioRoot, "TransformWS");
                    var schemaWorkspacePath = Path.Combine(scenarioRoot, "SchemaWS");
                    var bindingWorkspacePath = Path.Combine(scenarioRoot, "BindingWS");

                    model.SaveToXmlWorkspace(transformWorkspacePath);
                    schemaModel.SaveToXmlWorkspace(schemaWorkspacePath);

                    var result = new TransformBindingWorkspaceService().BindValidatedToWorkspace(
                        transformWorkspacePath,
                        schemaWorkspacePath,
                        bindingWorkspacePath);

                    if (result.TransformBindingCount != 1)
                    {
                        unknownFailures.Add($"{scenario}: expected one binding row, but got {result.TransformBindingCount}.");
                        continue;
                    }

                    validatedCount++;
                }
                catch (TransformBindingValidationException ex)
                {
                    unknownFailures.Add($"{scenario}: validation failed with {ex.Code}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    unknownFailures.Add($"{scenario}: unexpected exception {ex.GetType().Name}: {ex.Message}");
                }
            }
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }

        Assert.True(
            unknownFailures.Count == 0,
            "TPC-DS binding+validation classification has unknown failures:\n" + string.Join("\n", unknownFailures));
        Assert.True(validatedCount > 0, "Expected at least one TPC-DS script to bind+validate.");
        Assert.True(expectedGapCount > 0, "Expected at least one known-gap TPC-DS script.");
    }

    private static MetaSchemaModel BuildSyntheticSchemaModel(TransformBindingResult bound, string targetSqlIdentifier)
    {
        var model = MetaSchemaModel.CreateEmpty();
        var systemByName = new Dictionary<string, MetaSchema.System>(StringComparer.OrdinalIgnoreCase);
        var schemaByKey = new Dictionary<(string SystemName, string SchemaName), Schema>();
        var tableByKey = new Dictionary<(string SystemName, string SchemaName, string TableName), Table>();
        var fieldsByTableId = new Dictionary<string, Dictionary<string, Field>>(StringComparer.Ordinal);
        var sourceIdentifiers = bound.Rowsets
            .Where(item =>
                string.Equals(item.DerivationKind, "Source", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(item.SqlIdentifier))
            .Select(item => item.SqlIdentifier!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (var sourceIdentifier in sourceIdentifiers)
        {
            EnsureTable(model, systemByName, schemaByKey, tableByKey, sourceIdentifier);
        }

        foreach (var sourceRowset in bound.Rowsets.Where(item =>
                     string.Equals(item.DerivationKind, "Source", StringComparison.OrdinalIgnoreCase) &&
                     !string.IsNullOrWhiteSpace(item.SqlIdentifier)))
        {
            var sourceTable = EnsureTable(model, systemByName, schemaByKey, tableByKey, sourceRowset.SqlIdentifier!);
            foreach (var sourceColumn in sourceRowset.Columns.OrderBy(item => item.Ordinal))
            {
                AddFieldIfMissing(
                    model,
                    fieldsByTableId,
                    sourceTable,
                    sourceColumn.Name);
            }
        }

        var targetTable = EnsureTable(model, systemByName, schemaByKey, tableByKey, targetSqlIdentifier);
        var outputColumns = bound.TopLevelRowset?.Columns
            .OrderBy(item => item.Ordinal)
            .ToArray()
            ?? [];

        for (var index = 0; index < outputColumns.Length; index++)
        {
            AddFieldIfMissing(
                model,
                fieldsByTableId,
                targetTable,
                outputColumns[index].Name);
        }

        return model;
    }

    private static void AddFieldIfMissing(
        MetaSchemaModel model,
        Dictionary<string, Dictionary<string, Field>> fieldsByTableId,
        Table table,
        string fieldName)
    {
        if (!fieldsByTableId.TryGetValue(table.Id, out var fieldByName))
        {
            fieldByName = new Dictionary<string, Field>(StringComparer.OrdinalIgnoreCase);
            fieldsByTableId.Add(table.Id, fieldByName);
        }

        if (fieldByName.ContainsKey(fieldName))
        {
            return;
        }

        var ordinal = fieldByName.Count;
        var field = new Field
        {
            Id = $"{table.Id}:field:{NormalizeIdPart(fieldName)}",
            TableId = table.Id,
            Name = fieldName,
            Ordinal = ordinal.ToString(System.Globalization.CultureInfo.InvariantCulture),
            MetaDataTypeId = "sqlserver:type:nvarchar",
            IsNullable = "true"
        };

        fieldByName.Add(fieldName, field);
        model.FieldList.Add(field);
    }

    private static Table EnsureTable(
        MetaSchemaModel model,
        Dictionary<string, MetaSchema.System> systemByName,
        Dictionary<(string SystemName, string SchemaName), Schema> schemaByKey,
        Dictionary<(string SystemName, string SchemaName, string TableName), Table> tableByKey,
        string sqlIdentifier)
    {
        var parts = ParseSqlIdentifierParts(sqlIdentifier);
        var systemName = parts.Length switch
        {
            3 => parts[0],
            _ => "LocalSystem"
        };
        var schemaName = parts.Length switch
        {
            1 => "dbo",
            2 => parts[0],
            3 => parts[1],
            _ => throw new InvalidOperationException(
                $"Identifier '{sqlIdentifier}' must use table, schema.table, or system.schema.table.")
        };
        var tableName = parts.Length switch
        {
            1 => parts[0],
            2 => parts[1],
            3 => parts[2],
            _ => throw new InvalidOperationException(
                $"Identifier '{sqlIdentifier}' must use table, schema.table, or system.schema.table.")
        };

        if (!systemByName.TryGetValue(systemName, out var system))
        {
            system = new MetaSchema.System
            {
                Id = $"system:{NormalizeIdPart(systemName)}",
                Name = systemName
            };

            model.SystemList.Add(system);
            systemByName.Add(systemName, system);
        }

        var schemaKey = (SystemName: systemName, SchemaName: schemaName);
        if (!schemaByKey.TryGetValue(schemaKey, out var schema))
        {
            schema = new Schema
            {
                Id = $"schema:{NormalizeIdPart(systemName)}:{NormalizeIdPart(schemaName)}",
                SystemId = system.Id,
                Name = schemaName
            };

            model.SchemaList.Add(schema);
            schemaByKey.Add(schemaKey, schema);
        }

        var tableKey = (SystemName: systemName, SchemaName: schemaName, TableName: tableName);
        if (tableByKey.TryGetValue(tableKey, out var existingTable))
        {
            return existingTable;
        }

        var table = new Table
        {
            Id = $"table:{NormalizeIdPart(systemName)}:{NormalizeIdPart(schemaName)}:{NormalizeIdPart(tableName)}",
            SchemaId = schema.Id,
            Name = tableName,
            ObjectType = "Table"
        };

        model.TableList.Add(table);
        tableByKey.Add(tableKey, table);
        return table;
    }

    private static string[] ParseSqlIdentifierParts(string sqlIdentifier)
    {
        return sqlIdentifier
            .Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(static value =>
            {
                var trimmed = value.Trim();
                return trimmed.Length >= 2 && trimmed[0] == '[' && trimmed[^1] == ']'
                    ? trimmed[1..^1].Trim()
                    : trimmed;
            })
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .ToArray();
    }

    private static string NormalizeIdPart(string value)
    {
        return value
            .Trim()
            .Replace(' ', '_')
            .Replace('.', '_')
            .Replace('[', '_')
            .Replace(']', '_')
            .Replace(':', '_')
            .Replace('/', '_')
            .Replace('\\', '_');
    }

    private static string GetTpcDsSourceViewsRoot()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !Directory.Exists(Path.Combine(root.FullName, "Samples")))
        {
            root = root.Parent;
        }

        if (root is null)
        {
            throw new DirectoryNotFoundException("Could not locate repository root containing the Samples directory.");
        }

        return Path.Combine(
            root.FullName,
            "Samples",
            "Demos",
            "MetaTransformScriptTpcDsCliIntegration",
            "SourceViews");
    }
}
