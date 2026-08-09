using MetaConvert.DataVaultToSql;
using Meta.Core.Serialization;
using MetaRawDataVault;
using MetaSql.Extractors.SqlServer;

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
                sourceMetaSqlPath,
                Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "MetaDataVaultImplementation"),
                databaseName: "RawVault");

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
            Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(liveModel, liveMetaSqlPath);
            var liveWorkspace = (await XmlWorkspaceReader.OpenAsync(liveMetaSqlPath)).State;

            var diffService = new MetaSqlDiffService();
            var result = diffService.BuildEqualDiffWorkspace(
                sourceWorkspace,
                liveWorkspace);

            Assert.False(result.HasDifferences);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
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

        await Meta.Core.Serialization.TypedWorkspaceXmlSerializer.SaveAsync(model, workspacePath);
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
