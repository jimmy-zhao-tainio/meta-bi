using MetaConvert.DataVaultToSql;
using Meta.Integration;
using Meta.Operations.Domain;
using Meta.Surfaces.Xml;
using MetaRawDataVault;
using MetaSql.Extractors.SqlServer;
using Meta.Surfaces;

namespace MetaSql.Tests;

public sealed class MetaSqlAlignmentTests
{
    [Fact]
    public async Task RawDataVaultProjection_AndSqlServerProjection_AreEqualForSimpleHub()
    {
        var repoRoot = FindRepositoryRoot();
        var tempRoot = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        var rawWorkspacePath = Path.Combine(tempRoot, "RawDataVault");
        var sourceMetaSqlPath = Path.Combine(tempRoot, "SourceMetaSql");
        var liveMetaSqlPath = Path.Combine(tempRoot, "LiveMetaSql");

        try
        {
            await CreateSimpleRawHubWorkspaceAsync(rawWorkspacePath);

            var sourceWorkspace = await Converter.ConvertAsync(
                rawWorkspacePath,
                Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "MetaDataVaultImplementation"),
                databaseName: "RawVault");
            await WorkspaceSurface.CreateAsync(sourceWorkspace, sourceMetaSqlPath, "xml");

            var liveModel = SqlServerMetaSqlProjector.Project(
                databaseName: "RawVault",
                tableRows:
                [
                    new SqlServerMetaSqlProjector.TableRow("dbo", "H_Customer")
                ],
                columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
                {
                    ["dbo.H_Customer"] =
                    [
                        new("dbo", "H_Customer", "HashKey", 1, false, "binary", 32, null, null),
                        new("dbo", "H_Customer", "CustomerId", 2, true, "nvarchar", 50, null, null),
                        new("dbo", "H_Customer", "LoadTimestamp", 3, false, "datetime2", null, 7, null, DefaultExpressionSql: "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))"),
                        new("dbo", "H_Customer", "RecordSource", 4, false, "nvarchar", 256, null, null),
                        new("dbo", "H_Customer", "AuditId", 5, false, "bigint", null, null, null, DefaultExpressionSql: "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))"),
                    ],
                },
                primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase)
                {
                    ["dbo.H_Customer"] = [new("PK_H_Customer", false)],
                },
                primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
                {
                    ["dbo.H_Customer"] = [new("PK_H_Customer", 1, "HashKey", false)],
                },
                foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase),
                foreignKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
                indexesByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase),
                indexColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase));
            var liveBeforeXml = TypedWorkspaceModelMapper.ToInMemoryWorkspace(liveModel);

            var sourceAfterXml = (await XmlWorkspaceReader.OpenAsync(sourceMetaSqlPath)).State;
            MetaSqlTestSupport.SaveXml(liveModel, liveMetaSqlPath);
            var liveWorkspace = (await XmlWorkspaceReader.OpenAsync(liveMetaSqlPath)).State;

            Assert.Null(InMemoryWorkspaceComparer.FindDifference(sourceWorkspace, sourceAfterXml));
            Assert.Null(InMemoryWorkspaceComparer.FindDifference(liveBeforeXml, liveWorkspace));

            var diffService = new MetaSqlDiffService();
            var result = diffService.BuildEqualDiffWorkspace(
                sourceWorkspace,
                liveWorkspace);

            Assert.False(
                result.HasDifferences,
                string.Join(
                    Environment.NewLine,
                    "Source -> live before XML:",
                    FormatPropertyDifferences(sourceWorkspace, liveBeforeXml),
                    "Source -> live after XML:",
                    FormatPropertyDifferences(sourceWorkspace, liveWorkspace),
                    "Source before -> source after XML:",
                    FormatPropertyDifferences(sourceWorkspace, sourceAfterXml),
                    "Live before -> live after XML:",
                    FormatPropertyDifferences(liveBeforeXml, liveWorkspace)));
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    private static string FormatPropertyDifferences(InMemoryWorkspace source, InMemoryWorkspace projected)
    {
        var differences = new List<string>();
        var entityNames = source.Model.Entities
            .Select(entity => entity.Name)
            .Union(projected.Model.Entities.Select(entity => entity.Name), StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase);

        foreach (var entityName in entityNames)
        {
            var sourceRecords = GetRecords(source, entityName).ToDictionary(record => record.Id, StringComparer.OrdinalIgnoreCase);
            var projectedRecords = GetRecords(projected, entityName).ToDictionary(record => record.Id, StringComparer.OrdinalIgnoreCase);
            foreach (var recordId in sourceRecords.Keys
                         .Union(projectedRecords.Keys, StringComparer.OrdinalIgnoreCase)
                         .OrderBy(id => id, StringComparer.OrdinalIgnoreCase))
            {
                if (!sourceRecords.TryGetValue(recordId, out var sourceRecord) ||
                    !projectedRecords.TryGetValue(recordId, out var projectedRecord))
                {
                    differences.Add($"{entityName}/{recordId}: record {(sourceRecord is null ? "missing from source" : "missing from projected")}");
                    continue;
                }

                foreach (var propertyName in sourceRecord.Values.Keys
                             .Union(projectedRecord.Values.Keys, StringComparer.OrdinalIgnoreCase)
                             .OrderBy(name => name, StringComparer.OrdinalIgnoreCase))
                {
                    var sourcePresent = sourceRecord.Values.TryGetValue(propertyName, out var sourceValue);
                    var projectedPresent = projectedRecord.Values.TryGetValue(propertyName, out var projectedValue);
                    if (sourcePresent == projectedPresent && string.Equals(sourceValue, projectedValue, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    differences.Add(
                        $"{entityName}/{recordId}.{propertyName}: source={DescribeValue(sourcePresent, sourceValue)}; projected={DescribeValue(projectedPresent, projectedValue)}");
                }
            }
        }

        return differences.Count == 0
            ? "  none"
            : string.Join(Environment.NewLine, differences.Select(difference => "  " + difference));
    }

    private static IReadOnlyList<GenericRecord> GetRecords(InMemoryWorkspace workspace, string entityName)
    {
        return workspace.Instance.RecordsByEntity.TryGetValue(entityName, out var records)
            ? records
            : [];
    }

    private static string DescribeValue(bool present, string? value)
    {
        if (!present)
        {
            return "missing";
        }

        if (value is null)
        {
            return "null";
        }

        if (value.Length == 0)
        {
            return "empty";
        }

        if (string.Equals(value, "false", StringComparison.Ordinal))
        {
            return "\"false\"";
        }

        if (string.Equals(value, "true", StringComparison.Ordinal))
        {
            return "\"true\"";
        }

        return string.IsNullOrWhiteSpace(value)
            ? $"whitespace({value.Length})"
            : $"\"{value}\"";
    }

    private static async Task CreateSimpleRawHubWorkspaceAsync(string workspacePath)
    {
        var model = MetaRawDataVaultModel.CreateEmpty();

        var customerIdField = new Field
        {
            Id = "Field:Customer:CustomerId",
            Name = "CustomerId",
            DataTypeId = "sqlserver:type:nvarchar",
        };
        var customerIdLength = new FieldDataTypeDetail
        {
            Id = "FieldDetail:Customer:CustomerId:Length",
            Name = "Length",
            Value = "50",
            Field = customerIdField,
        };
        var rawHub = new RawHub
        {
            Id = "RawHub:Customer",
            Name = "Customer",
        };
        var rawHubKeyPart = new RawHubKeyPart
        {
            Id = "RawHubKeyPart:Customer:CustomerId",
            Name = "CustomerId",
            RawHub = rawHub,
            Field = customerIdField,
        };

        model.FieldList.Add(customerIdField);
        model.FieldDataTypeDetailList.Add(customerIdLength);
        model.RawHubList.Add(rawHub);
        model.RawHubKeyPartList.Add(rawHubKeyPart);

        await MetaSqlTestSupport.SaveXmlAsync(model, workspacePath);
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "MetaDataVault.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Could not find the repository root.");
    }

    private static void DeleteIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
