using MetaDataVault.ToMetaSql;
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
                Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation"),
                databaseName: "RawVault",
                defaultSchemaName: "raw");

            var liveWorkspace = SqlServerMetaSqlProjector.Project(
                newWorkspacePath: liveMetaSqlPath,
                databaseName: "RawVault",
                tableRows:
                [
                    new SqlServerMetaSqlProjector.TableRow("raw", "H_Customer")
                ],
                columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
                {
                    ["raw.H_Customer"] =
                    [
                        new("raw", "H_Customer", "HashKey", 1, false, "binary", 16, null, null),
                        new("raw", "H_Customer", "CustomerId", 2, false, "nvarchar", 50, null, null),
                        new("raw", "H_Customer", "LoadTimestamp", 3, false, "datetime2", null, 7, null),
                        new("raw", "H_Customer", "RecordSource", 4, false, "nvarchar", 256, null, null),
                        new("raw", "H_Customer", "AuditId", 5, false, "int", null, null, null),
                    ],
                },
                primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase)
                {
                    ["raw.H_Customer"] = [new("PK_H_Customer", false)],
                },
                primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
                {
                    ["raw.H_Customer"] = [new("PK_H_Customer", 1, "HashKey", false)],
                },
                foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase),
                foreignKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
                indexesByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase),
                indexColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase));

            var diffService = new MetaSqlDiffService();
            var result = diffService.BuildEqualDiffWorkspace(
                sourceWorkspace,
                liveWorkspace,
                liveMetaSqlPath);

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

        var sourceSystem = new SourceSystem
        {
            Id = "SourceSystem:CRM",
            Name = "CRM",
        };
        var sourceSchema = new SourceSchema
        {
            Id = "SourceSchema:CRM:dbo",
            Name = "dbo",
            SourceSystemId = sourceSystem.Id,
            SourceSystem = sourceSystem,
        };
        var sourceTable = new SourceTable
        {
            Id = "SourceTable:Customer",
            Name = "Customer",
            SourceSchemaId = sourceSchema.Id,
            SourceSchema = sourceSchema,
        };
        var customerIdField = new SourceField
        {
            Id = "SourceField:Customer:CustomerId",
            Name = "CustomerId",
            Ordinal = "1",
            DataTypeId = "sqlserver:type:nvarchar",
            IsNullable = "false",
            SourceTableId = sourceTable.Id,
            SourceTable = sourceTable,
        };
        var customerIdLength = new SourceFieldDataTypeDetail
        {
            Id = "SourceFieldDetail:Customer:CustomerId:Length",
            Name = "Length",
            Value = "50",
            SourceFieldId = customerIdField.Id,
            SourceField = customerIdField,
        };
        var rawHub = new RawHub
        {
            Id = "RawHub:Customer",
            Name = "Customer",
            SourceTableId = sourceTable.Id,
            SourceTable = sourceTable,
        };
        var rawHubKeyPart = new RawHubKeyPart
        {
            Id = "RawHubKeyPart:Customer:CustomerId",
            Name = "CustomerId",
            Ordinal = "1",
            RawHubId = rawHub.Id,
            RawHub = rawHub,
            SourceFieldId = customerIdField.Id,
            SourceField = customerIdField,
        };

        model.SourceSystemList.Add(sourceSystem);
        model.SourceSchemaList.Add(sourceSchema);
        model.SourceTableList.Add(sourceTable);
        model.SourceFieldList.Add(customerIdField);
        model.SourceFieldDataTypeDetailList.Add(customerIdLength);
        model.RawHubList.Add(rawHub);
        model.RawHubKeyPartList.Add(rawHubKeyPart);

        await model.SaveToXmlWorkspaceAsync(workspacePath);
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
