using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using MetaSql;
using MetaSqlDeployManifest;
using MetaSql.Extractors.SqlServer;

namespace MetaSql.Tests;

public sealed partial class CliDiffTests
{
    private static Task CreateSourceWorkspaceWithChangedCustomerIdLengthAsync(string sourcePath, string databaseName)
    {
        return CreateSourceWorkspaceWithCustomerIdLengthAsync(sourcePath, databaseName, customerIdLength: 20);
    }

    private static Task CreateSourceWorkspaceWithCustomerIdLengthAsync(
        string sourcePath,
        string databaseName,
        int customerIdLength)
    {
        SqlServerMetaSqlProjector.Project(
            newWorkspacePath: sourcePath,
            databaseName: databaseName,
            tableRows:
            [
                new SqlServerMetaSqlProjector.TableRow("raw", "H_Customer")
            ],
            columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.H_Customer"] =
                [
                    new("raw", "H_Customer", "HashKey", 1, false, "binary", 16, null, null),
                    new("raw", "H_Customer", "CustomerId", 2, false, "nvarchar", customerIdLength, null, null),
                    new("raw", "H_Customer", "LoadTimestamp", 3, false, "datetime2", null, 7, null),
                    new("raw", "H_Customer", "RecordSource", 4, false, "nvarchar", 256, null, null),
                    new("raw", "H_Customer", "AuditId", 5, false, "int", null, null, null),
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.H_Customer"] = [new("PK_H_Customer", true)],
            },
            primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.H_Customer"] = [new("PK_H_Customer", 1, "HashKey", false)],
            },
            foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase),
            foreignKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            indexesByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase),
            indexColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase));
        return Task.CompletedTask;
    }

    private static Task CreateSourceWorkspaceWithChildOnlyNoForeignKeyAsync(string sourcePath, string databaseName)
    {
        SqlServerMetaSqlProjector.Project(
            newWorkspacePath: sourcePath,
            databaseName: databaseName,
            tableRows:
            [
                new SqlServerMetaSqlProjector.TableRow("raw", "Child")
            ],
            columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.Child"] =
                [
                    new("raw", "Child", "ChildId", 1, false, "int", null, null, null),
                    new("raw", "Child", "ParentId", 2, false, "int", null, null, null),
                    new("raw", "Child", "LoadTimestamp", 3, false, "datetime2", null, 7, null),
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.Child"] = [new("PK_Child", true)],
            },
            primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.Child"] = [new("PK_Child", 1, "ChildId", false)],
            },
            foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase),
            foreignKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            indexesByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase),
            indexColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase));
        return Task.CompletedTask;
    }

    private static Task CreateSourceWorkspaceWithSingleForeignKeyTargetAsync(
        string sourcePath,
        string databaseName,
        string targetTableName,
        bool includeForeignKeyMember)
    {
        SqlServerMetaSqlProjector.Project(
            newWorkspacePath: sourcePath,
            databaseName: databaseName,
            tableRows:
            [
                new SqlServerMetaSqlProjector.TableRow("raw", "ParentA"),
                new SqlServerMetaSqlProjector.TableRow("raw", "ParentB"),
                new SqlServerMetaSqlProjector.TableRow("raw", "Child")
            ],
            columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ParentA"] =
                [
                    new("raw", "ParentA", "ParentId", 1, false, "int", null, null, null),
                ],
                ["raw.ParentB"] =
                [
                    new("raw", "ParentB", "ParentId", 1, false, "int", null, null, null),
                ],
                ["raw.Child"] =
                [
                    new("raw", "Child", "ChildId", 1, false, "int", null, null, null),
                    new("raw", "Child", "ParentId", 2, false, "int", null, null, null),
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ParentA"] = [new("PK_ParentA", true)],
                ["raw.ParentB"] = [new("PK_ParentB", true)],
                ["raw.Child"] = [new("PK_Child", true)],
            },
            primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ParentA"] = [new("PK_ParentA", 1, "ParentId", false)],
                ["raw.ParentB"] = [new("PK_ParentB", 1, "ParentId", false)],
                ["raw.Child"] = [new("PK_Child", 1, "ChildId", false)],
            },
            foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.Child"] = [new("FK_Child_Parent", "raw", targetTableName)],
            },
            foreignKeyColumnsByTableKey: includeForeignKeyMember
                ? new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
                {
                    ["raw.Child"] = [new("FK_Child_Parent", 1, "ParentId", "ParentId")],
                }
                : new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            indexesByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase),
            indexColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase));
        return Task.CompletedTask;
    }

    private static Task CreateSourceWorkspaceWithTwoForeignKeysTargetingParentBAsync(string sourcePath, string databaseName)
    {
        SqlServerMetaSqlProjector.Project(
            newWorkspacePath: sourcePath,
            databaseName: databaseName,
            tableRows:
            [
                new SqlServerMetaSqlProjector.TableRow("raw", "ParentA"),
                new SqlServerMetaSqlProjector.TableRow("raw", "ParentB"),
                new SqlServerMetaSqlProjector.TableRow("raw", "ChildA"),
                new SqlServerMetaSqlProjector.TableRow("raw", "ChildB")
            ],
            columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ParentA"] =
                [
                    new("raw", "ParentA", "ParentId", 1, false, "int", null, null, null),
                ],
                ["raw.ParentB"] =
                [
                    new("raw", "ParentB", "ParentId", 1, false, "int", null, null, null),
                ],
                ["raw.ChildA"] =
                [
                    new("raw", "ChildA", "ChildAId", 1, false, "int", null, null, null),
                    new("raw", "ChildA", "ParentId", 2, false, "int", null, null, null),
                ],
                ["raw.ChildB"] =
                [
                    new("raw", "ChildB", "ChildBId", 1, false, "int", null, null, null),
                    new("raw", "ChildB", "ParentId", 2, false, "int", null, null, null),
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ParentA"] = [new("PK_ParentA", true)],
                ["raw.ParentB"] = [new("PK_ParentB", true)],
                ["raw.ChildA"] = [new("PK_ChildA", true)],
                ["raw.ChildB"] = [new("PK_ChildB", true)],
            },
            primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ParentA"] = [new("PK_ParentA", 1, "ParentId", false)],
                ["raw.ParentB"] = [new("PK_ParentB", 1, "ParentId", false)],
                ["raw.ChildA"] = [new("PK_ChildA", 1, "ChildAId", false)],
                ["raw.ChildB"] = [new("PK_ChildB", 1, "ChildBId", false)],
            },
            foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ChildA"] = [new("FK_ChildA_Parent", "raw", "ParentB")],
                ["raw.ChildB"] = [new("FK_ChildB_Parent", "raw", "ParentB")],
            },
            foreignKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.ChildA"] = [new("FK_ChildA_Parent", 1, "ParentId", "ParentId")],
                ["raw.ChildB"] = [new("FK_ChildB_Parent", 1, "ParentId", "ParentId")],
            },
            indexesByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase),
            indexColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase));
        return Task.CompletedTask;
    }

    private static Task CreateSourceWorkspaceWithExtraColumnAsync(string sourcePath, string databaseName)
    {
        SqlServerMetaSqlProjector.Project(
            newWorkspacePath: sourcePath,
            databaseName: databaseName,
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
                    new("raw", "H_Customer", "CustomerName", 6, true, "nvarchar", 200, null, null),
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.H_Customer"] = [new("PK_H_Customer", true)],
            },
            primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw.H_Customer"] = [new("PK_H_Customer", 1, "HashKey", false)],
            },
            foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase),
            foreignKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            indexesByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase),
            indexColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase));
        return Task.CompletedTask;
    }

    private static Task CreateSourceWorkspaceWithExtraColumnInDboAsync(string sourcePath, string databaseName)
    {
        SqlServerMetaSqlProjector.Project(
            newWorkspacePath: sourcePath,
            databaseName: databaseName,
            tableRows:
            [
                new SqlServerMetaSqlProjector.TableRow("dbo", "H_Customer")
            ],
            columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.H_Customer"] =
                [
                    new("dbo", "H_Customer", "HashKey", 1, false, "binary", 16, null, null),
                    new("dbo", "H_Customer", "CustomerId", 2, false, "nvarchar", 50, null, null),
                    new("dbo", "H_Customer", "LoadTimestamp", 3, false, "datetime2", null, 7, null),
                    new("dbo", "H_Customer", "RecordSource", 4, false, "nvarchar", 256, null, null),
                    new("dbo", "H_Customer", "AuditId", 5, false, "int", null, null, null),
                    new("dbo", "H_Customer", "CustomerName", 6, true, "nvarchar", 200, null, null),
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.H_Customer"] = [new("PK_H_Customer", true)],
            },
            primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.H_Customer"] = [new("PK_H_Customer", 1, "HashKey", false)],
            },
            foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase),
            foreignKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            indexesByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase),
            indexColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase));
        return Task.CompletedTask;
    }

    private static async Task CreateSourceWorkspaceFromLiveAndMutateAsync(
        string sourcePath,
        string connectionString,
        string schemaName,
        string? tableName,
        Action<MetaSqlModel> mutate)
    {
        var extractor = new SqlServerMetaSqlExtractor();
        extractor.ExtractMetaSqlWorkspace(new SqlServerExtractRequest
        {
            NewWorkspacePath = sourcePath,
            ConnectionString = connectionString,
            SchemaName = schemaName,
            TableName = tableName,
        });

        await MutateSourceWorkspaceAsync(sourcePath, mutate);
    }

    private static async Task MutateSourceWorkspaceAsync(
        string sourcePath,
        Action<MetaSqlModel> mutate)
    {
        var model = await MetaSqlModel.LoadFromXmlWorkspaceAsync(sourcePath, searchUpward: false);
        mutate(model);
        await model.SaveToXmlWorkspaceAsync(sourcePath);
    }

    private static TableColumn RequireColumn(
        MetaSqlModel model,
        string schemaName,
        string tableName,
        string columnName)
    {
        var schema = model.SchemaList
            .Single(row => string.Equals(row.Name, schemaName, StringComparison.OrdinalIgnoreCase));
        var table = model.TableList
            .Single(row =>
                row.SchemaId == schema.Id &&
                string.Equals(row.Name, tableName, StringComparison.OrdinalIgnoreCase));
        return model.TableColumnList
            .Single(row =>
                row.TableId == table.Id &&
                string.Equals(row.Name, columnName, StringComparison.OrdinalIgnoreCase));
    }

    private static PrimaryKey RequirePrimaryKey(
        MetaSqlModel model,
        string schemaName,
        string tableName,
        string primaryKeyName)
    {
        var schema = model.SchemaList
            .Single(row => string.Equals(row.Name, schemaName, StringComparison.OrdinalIgnoreCase));
        var table = model.TableList
            .Single(row =>
                row.SchemaId == schema.Id &&
                string.Equals(row.Name, tableName, StringComparison.OrdinalIgnoreCase));
        return model.PrimaryKeyList
            .Single(row =>
                row.TableId == table.Id &&
                string.Equals(row.Name, primaryKeyName, StringComparison.OrdinalIgnoreCase));
    }

    private static void SetPrimaryKeyMembers(
        MetaSqlModel model,
        PrimaryKey primaryKey,
        IReadOnlyList<TableColumn> columns)
    {
        model.PrimaryKeyColumnList.RemoveAll(row => row.PrimaryKeyId == primaryKey.Id);
        for (var i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            model.PrimaryKeyColumnList.Add(new PrimaryKeyColumn
            {
                Id = $"{primaryKey.Id}.column.{i + 1}",
                PrimaryKeyId = primaryKey.Id,
                PrimaryKey = primaryKey,
                TableColumnId = column.Id,
                TableColumn = column,
                Ordinal = (i + 1).ToString(),
            });
        }
    }

    private static Index RequireIndex(
        MetaSqlModel model,
        string schemaName,
        string tableName,
        string indexName)
    {
        var schema = model.SchemaList
            .Single(row => string.Equals(row.Name, schemaName, StringComparison.OrdinalIgnoreCase));
        var table = model.TableList
            .Single(row =>
                row.SchemaId == schema.Id &&
                string.Equals(row.Name, tableName, StringComparison.OrdinalIgnoreCase));
        return model.IndexList
            .Single(row =>
                row.TableId == table.Id &&
                string.Equals(row.Name, indexName, StringComparison.OrdinalIgnoreCase));
    }

    private static IndexColumn RequireIndexMember(
        MetaSqlModel model,
        string schemaName,
        string tableName,
        string indexName,
        string columnName)
    {
        var index = RequireIndex(model, schemaName, tableName, indexName);
        var column = RequireColumn(model, schemaName, tableName, columnName);
        return model.IndexColumnList
            .Single(row =>
                row.IndexId == index.Id &&
                row.TableColumnId == column.Id);
    }

    private static void SetOrReplaceColumnDetail(
        MetaSqlModel model,
        TableColumn column,
        string detailName,
        string detailValue)
    {
        var existing = model.TableColumnDataTypeDetailList.FirstOrDefault(row =>
            row.TableColumnId == column.Id &&
            string.Equals(row.Name, detailName, StringComparison.OrdinalIgnoreCase));
        if (existing is null)
        {
            model.TableColumnDataTypeDetailList.Add(new TableColumnDataTypeDetail
            {
                Id = $"{column.Id}.detail.{detailName}",
                Name = detailName,
                Value = detailValue,
                TableColumnId = column.Id,
                TableColumn = column,
            });
            return;
        }

        existing.Value = detailValue;
    }

    private static void CreateVarcharCaseTable(
        string connectionString,
        int length,
        string? seedValue)
    {
        var script = $"""
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.VarcharCase (
                Id int NOT NULL,
                ValueCol varchar({length}) NOT NULL,
                CONSTRAINT PK_VarcharCase PRIMARY KEY (Id)
            );
            """;
        if (!string.IsNullOrEmpty(seedValue))
        {
            var escapedSeedValue = seedValue.Replace("'", "''", StringComparison.Ordinal);
            script += $"""

                INSERT INTO raw.VarcharCase(Id, ValueCol)
                VALUES (1, '{escapedSeedValue}');
                """;
        }

        ExecuteSql(connectionString, script);
    }

    private static void CreateNullableCaseTable(string connectionString, bool includeNullRow)
    {
        var script = """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.NullableCase (
                Id int NOT NULL,
                ValueCol varchar(50) NULL,
                CONSTRAINT PK_NullableCase PRIMARY KEY (Id)
            );
            INSERT INTO raw.NullableCase(Id, ValueCol)
            VALUES (1, 'not-null');
            """;
        if (includeNullRow)
        {
            script += """

                INSERT INTO raw.NullableCase(Id, ValueCol)
                VALUES (2, NULL);
                """;
        }

        ExecuteSql(connectionString, script);
    }

    private static void CreateDecimalCaseTable(
        string connectionString,
        int precision,
        int scale)
    {
        ExecuteSql(connectionString, $"""
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.DecimalCase (
                Id int NOT NULL,
                Amount decimal({precision},{scale}) NOT NULL,
                CONSTRAINT PK_DecimalCase PRIMARY KEY (Id)
            );
            INSERT INTO raw.DecimalCase(Id, Amount)
            VALUES (1, 123.45);
            """);
    }

    private static void CreateDatabase(string masterConnectionString, string databaseName)
    {
        ExecuteSql(masterConnectionString, $"""
            IF DB_ID('{databaseName}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{databaseName}];
            END;
            CREATE DATABASE [{databaseName}];
            """);
    }

    private static void CreateSimpleTable(
        string databaseConnectionString,
        int customerIdLength = 50,
        string? customerIdValue = null)
    {
        var setupSql = $"""
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.H_Customer (
                HashKey binary(16) NOT NULL,
                CustomerId nvarchar({customerIdLength}) NOT NULL,
                LoadTimestamp datetime2(7) NOT NULL,
                RecordSource nvarchar(256) NOT NULL,
                AuditId int NOT NULL,
                CONSTRAINT PK_H_Customer PRIMARY KEY (HashKey)
            );
            """;

        if (!string.IsNullOrEmpty(customerIdValue))
        {
            var escapedValue = customerIdValue.Replace("'", "''", StringComparison.Ordinal);
            setupSql += $"""
                
                INSERT INTO raw.H_Customer(HashKey, CustomerId, LoadTimestamp, RecordSource, AuditId)
                VALUES (CONVERT(binary(16), 0x00000000000000000000000000000001), N'{escapedValue}', SYSUTCDATETIME(), N'SRC', 1);
                """;
        }

        ExecuteSql(databaseConnectionString, setupSql);
    }

    private static void CreatePkIndexOnlyDriftFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.PkIndexCase (
                Id int NOT NULL,
                Payload nvarchar(100) NOT NULL,
                CONSTRAINT PK_PkIndexCase PRIMARY KEY (Id)
            );
            CREATE INDEX IX_PkIndexCase_Payload
                ON raw.PkIndexCase (Payload);
            """);
    }

    private static void CreateTableAndColumnOnlyDataDropFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.ActiveCase (
                Id int NOT NULL,
                KeepCol nvarchar(50) NOT NULL,
                LegacyCol nvarchar(50) NULL,
                CONSTRAINT PK_ActiveCase PRIMARY KEY (Id)
            );
            CREATE TABLE raw.LegacyOnly (
                Id int NOT NULL,
                Payload nvarchar(50) NOT NULL,
                CONSTRAINT PK_LegacyOnly PRIMARY KEY (Id)
            );
            """);
    }

    private static void CreateTableAndColumnOnlyDataDropFixtureWithDefaultConstraint(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.ActiveCase (
                Id int NOT NULL,
                KeepCol nvarchar(50) NOT NULL,
                LegacyCol nvarchar(50) NULL CONSTRAINT DF_ActiveCase_LegacyCol DEFAULT (N'LEG'),
                CONSTRAINT PK_ActiveCase PRIMARY KEY (Id)
            );
            CREATE TABLE raw.LegacyOnly (
                Id int NOT NULL,
                Payload nvarchar(50) NOT NULL,
                CONSTRAINT PK_LegacyOnly PRIMARY KEY (Id)
            );
            """);
    }

    private static void CreateParentChildWithForeignKey(string databaseConnectionString)
    {
        ExecuteSql(databaseConnectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.Parent (
                ParentId int NOT NULL,
                CONSTRAINT PK_Parent PRIMARY KEY (ParentId)
            );
            CREATE TABLE raw.Child (
                ChildId int NOT NULL,
                ParentId int NOT NULL,
                LoadTimestamp datetime2(7) NOT NULL,
                CONSTRAINT PK_Child PRIMARY KEY (ChildId)
            );
            ALTER TABLE raw.Child
                ADD CONSTRAINT FK_Child_Parent
                FOREIGN KEY (ParentId) REFERENCES raw.Parent(ParentId);
            """);
    }

    private static void CreateForeignKeyReplaceFixture(string databaseConnectionString)
    {
        ExecuteSql(databaseConnectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.ParentA (
                ParentId int NOT NULL,
                CONSTRAINT PK_ParentA PRIMARY KEY (ParentId)
            );
            CREATE TABLE raw.ParentB (
                ParentId int NOT NULL,
                CONSTRAINT PK_ParentB PRIMARY KEY (ParentId)
            );
            CREATE TABLE raw.Child (
                ChildId int NOT NULL,
                ParentId int NOT NULL,
                CONSTRAINT PK_Child PRIMARY KEY (ChildId)
            );
            ALTER TABLE raw.Child
                ADD CONSTRAINT FK_Child_Parent
                FOREIGN KEY (ParentId) REFERENCES raw.ParentA(ParentId);
            """);
    }

    private static void CreateForeignKeyReplaceRollbackFixture(string databaseConnectionString)
    {
        ExecuteSql(databaseConnectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.ParentA (
                ParentId int NOT NULL,
                CONSTRAINT PK_ParentA PRIMARY KEY (ParentId)
            );
            CREATE TABLE raw.ParentB (
                ParentId int NOT NULL,
                CONSTRAINT PK_ParentB PRIMARY KEY (ParentId)
            );
            CREATE TABLE raw.ChildA (
                ChildAId int NOT NULL,
                ParentId int NOT NULL,
                CONSTRAINT PK_ChildA PRIMARY KEY (ChildAId)
            );
            CREATE TABLE raw.ChildB (
                ChildBId int NOT NULL,
                ParentId int NOT NULL,
                CONSTRAINT PK_ChildB PRIMARY KEY (ChildBId)
            );
            INSERT INTO raw.ParentA(ParentId) VALUES (1), (2);
            INSERT INTO raw.ParentB(ParentId) VALUES (1);
            INSERT INTO raw.ChildA(ChildAId, ParentId) VALUES (1, 1);
            INSERT INTO raw.ChildB(ChildBId, ParentId) VALUES (1, 2);
            ALTER TABLE raw.ChildA
                ADD CONSTRAINT FK_ChildA_Parent
                FOREIGN KEY (ParentId) REFERENCES raw.ParentA(ParentId);
            ALTER TABLE raw.ChildB
                ADD CONSTRAINT FK_ChildB_Parent
                FOREIGN KEY (ParentId) REFERENCES raw.ParentA(ParentId);
            """);
    }

    private static void CreatePrimaryKeyReplaceFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.PkReplaceCase (
                KeyA int NOT NULL,
                KeyB int NOT NULL,
                Payload int NULL,
                CONSTRAINT PK_PkReplaceCase PRIMARY KEY NONCLUSTERED (KeyA)
            );
            INSERT INTO raw.PkReplaceCase(KeyA, KeyB, Payload)
            VALUES (1, 10, 100), (2, 20, 200), (3, 30, 300);
            """);
    }

    private static void CreateClusteredPrimaryKeyReplaceFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.PkClusteredCase (
                KeyA int NOT NULL,
                KeyB int NOT NULL,
                Payload int NULL,
                CONSTRAINT PK_PkClusteredCase PRIMARY KEY CLUSTERED (KeyA)
            );
            INSERT INTO raw.PkClusteredCase(KeyA, KeyB, Payload)
            VALUES (1, 10, 100), (2, 20, 200), (3, 30, 300);
            """);
    }

    private static void CreatePrimaryKeyReplaceWithDependentForeignKeyFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.ParentPkCase (
                KeyA int NOT NULL,
                KeyB int NOT NULL,
                Payload int NULL,
                CONSTRAINT PK_ParentPkCase PRIMARY KEY NONCLUSTERED (KeyA)
            );
            CREATE TABLE raw.ChildPkCase (
                ChildId int NOT NULL,
                ParentKeyA int NOT NULL,
                ParentKeyB int NOT NULL,
                CONSTRAINT PK_ChildPkCase PRIMARY KEY (ChildId)
            );
            ALTER TABLE raw.ChildPkCase
                ADD CONSTRAINT FK_ChildPkCase_ParentPkCase
                FOREIGN KEY (ParentKeyA) REFERENCES raw.ParentPkCase(KeyA);
            INSERT INTO raw.ParentPkCase(KeyA, KeyB, Payload)
            VALUES (1, 10, 100), (2, 20, 200), (3, 30, 300);
            INSERT INTO raw.ChildPkCase(ChildId, ParentKeyA, ParentKeyB)
            VALUES (1, 1, 10), (2, 2, 20), (3, 3, 30);
            """);
    }

    private static void CreatePrimaryKeyReplaceRollbackFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.PkRollbackA (
                KeyA int NOT NULL,
                KeyB int NOT NULL,
                Payload int NULL,
                CONSTRAINT PK_PkRollbackA PRIMARY KEY NONCLUSTERED (KeyA)
            );
            CREATE TABLE raw.PkRollbackB (
                KeyA int NOT NULL,
                KeyB int NOT NULL,
                Payload int NULL,
                CONSTRAINT PK_PkRollbackB PRIMARY KEY NONCLUSTERED (KeyA)
            );
            INSERT INTO raw.PkRollbackA(KeyA, KeyB, Payload)
            VALUES (1, 10, 100), (2, 20, 200), (3, 30, 300);
            INSERT INTO raw.PkRollbackB(KeyA, KeyB, Payload)
            VALUES (1, 7, 100), (2, 7, 200), (3, 8, 300);
            """);
    }

    private static void CreateIndexReplaceFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.IndexReplaceCase (
                Id int NOT NULL,
                Payload int NOT NULL,
                Note int NULL,
                CONSTRAINT PK_IndexReplaceCase PRIMARY KEY (Id)
            );
            INSERT INTO raw.IndexReplaceCase(Id, Payload, Note)
            VALUES (1, 10, 1), (2, 20, 2), (3, 30, 3);
            CREATE NONCLUSTERED INDEX IX_IndexReplaceCase_Payload
                ON raw.IndexReplaceCase (Payload ASC);
            """);
    }

    private static void CreateClusteredIndexReplaceFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.IndexClusteredCase (
                Id int NOT NULL,
                Payload int NOT NULL
            );
            INSERT INTO raw.IndexClusteredCase(Id, Payload)
            VALUES (1, 10), (2, 20), (3, 30);
            CREATE CLUSTERED INDEX IX_IndexClusteredCase_Payload
                ON raw.IndexClusteredCase (Payload ASC);
            """);
    }

    private static void CreateIndexReplaceRollbackFixture(string connectionString)
    {
        ExecuteSql(connectionString, """
            IF SCHEMA_ID('raw') IS NULL EXEC('CREATE SCHEMA raw');
            CREATE TABLE raw.IndexRollbackCase (
                Id int NOT NULL,
                A int NOT NULL,
                B int NOT NULL,
                CONSTRAINT PK_IndexRollbackCase PRIMARY KEY (Id)
            );
            INSERT INTO raw.IndexRollbackCase(Id, A, B)
            VALUES (1, 1, 1), (2, 2, 1), (3, 3, 2);
            CREATE NONCLUSTERED INDEX IX_IndexRollback_A
                ON raw.IndexRollbackCase (A ASC);
            CREATE NONCLUSTERED INDEX IX_IndexRollback_B
                ON raw.IndexRollbackCase (B ASC);
            """);
    }

    private static void DropDatabase(string masterConnectionString, string databaseName)
    {
        try
        {
            ExecuteSql(masterConnectionString, $"""
                IF DB_ID('{databaseName}') IS NOT NULL
                BEGIN
                    ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{databaseName}];
                END;
                """);
        }
        catch
        {
        }
    }

    private static void ExecuteSql(string connectionString, string sql)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static bool DatabaseExists(string masterConnectionString, string databaseName)
    {
        using var connection = new SqlConnection(masterConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT CASE WHEN DB_ID(@DatabaseName) IS NULL THEN 0 ELSE 1 END;";
        command.Parameters.AddWithValue("@DatabaseName", databaseName);
        return Convert.ToInt32(command.ExecuteScalar()) == 1;
    }

    private static bool SchemaExists(string connectionString, string schemaName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(*)
            FROM sys.schemas
            WHERE name = @SchemaName;
            """;
        command.Parameters.AddWithValue("@SchemaName", schemaName);
        var value = command.ExecuteScalar();
        return Convert.ToInt32(value) > 0;
    }

    private static bool ColumnExists(string connectionString, string schemaName, string tableName, string columnName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(*)
            FROM sys.columns AS c
            INNER JOIN sys.tables AS t ON t.object_id = c.object_id
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            WHERE s.name = @SchemaName
              AND t.name = @TableName
              AND c.name = @ColumnName;
            """;
        command.Parameters.AddWithValue("@SchemaName", schemaName);
        command.Parameters.AddWithValue("@TableName", tableName);
        command.Parameters.AddWithValue("@ColumnName", columnName);
        var value = command.ExecuteScalar();
        return Convert.ToInt32(value) > 0;
    }

    private static short GetColumnMaxLengthBytes(string connectionString, string schemaName, string tableName, string columnName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT c.max_length
            FROM sys.columns AS c
            INNER JOIN sys.tables AS t ON t.object_id = c.object_id
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            WHERE s.name = @SchemaName
              AND t.name = @TableName
              AND c.name = @ColumnName;
            """;
        command.Parameters.AddWithValue("@SchemaName", schemaName);
        command.Parameters.AddWithValue("@TableName", tableName);
        command.Parameters.AddWithValue("@ColumnName", columnName);
        var value = command.ExecuteScalar();
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Column '{schemaName}.{tableName}.{columnName}' was not found.");
        }

        return Convert.ToInt16(value);
    }

    private static int GetValueLength(
        string connectionString,
        string schemaName,
        string tableName,
        string columnName,
        int id)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"""
            SELECT LEN([{columnName}])
            FROM [{schemaName}].[{tableName}]
            WHERE [Id] = @Id;
            """;
        command.Parameters.AddWithValue("@Id", id);
        var value = command.ExecuteScalar();
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Row '{schemaName}.{tableName}.Id={id}' was not found.");
        }

        return Convert.ToInt32(value);
    }

    private static bool GetColumnNullable(string connectionString, string schemaName, string tableName, string columnName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT c.is_nullable
            FROM sys.columns AS c
            INNER JOIN sys.tables AS t ON t.object_id = c.object_id
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            WHERE s.name = @SchemaName
              AND t.name = @TableName
              AND c.name = @ColumnName;
            """;
        command.Parameters.AddWithValue("@SchemaName", schemaName);
        command.Parameters.AddWithValue("@TableName", tableName);
        command.Parameters.AddWithValue("@ColumnName", columnName);
        var value = command.ExecuteScalar();
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Column '{schemaName}.{tableName}.{columnName}' was not found.");
        }

        return Convert.ToInt32(value) == 1;
    }

    private static bool TableExists(string connectionString, string schemaName, string tableName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(*)
            FROM sys.tables AS t
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            WHERE s.name = @SchemaName
              AND t.name = @TableName;
            """;
        command.Parameters.AddWithValue("@SchemaName", schemaName);
        command.Parameters.AddWithValue("@TableName", tableName);
        var value = command.ExecuteScalar();
        return Convert.ToInt32(value) > 0;
    }

    private static bool ForeignKeyExists(string connectionString, string foreignKeyName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(*)
            FROM sys.foreign_keys
            WHERE name = @ForeignKeyName;
            """;
        command.Parameters.AddWithValue("@ForeignKeyName", foreignKeyName);
        var value = command.ExecuteScalar();
        return Convert.ToInt32(value) > 0;
    }

    private static bool IndexExists(string connectionString, string indexName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(1)
            FROM sys.indexes
            WHERE name = @IndexName;
            """;
        command.Parameters.AddWithValue("@IndexName", indexName);
        var value = command.ExecuteScalar();
        return Convert.ToInt32(value) > 0;
    }

    private static string GetForeignKeyTargetTableName(string connectionString, string foreignKeyName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT t.name
            FROM sys.foreign_keys AS fk
            INNER JOIN sys.tables AS t ON t.object_id = fk.referenced_object_id
            WHERE fk.name = @ForeignKeyName;
            """;
        command.Parameters.AddWithValue("@ForeignKeyName", foreignKeyName);
        var value = command.ExecuteScalar();
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Foreign key '{foreignKeyName}' was not found.");
        }

        return Convert.ToString(value)!;
    }

    private static List<string> GetPrimaryKeyKeyColumns(string connectionString, string schemaName, string tableName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT c.name
            FROM sys.key_constraints AS kc
            INNER JOIN sys.tables AS t ON t.object_id = kc.parent_object_id
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            INNER JOIN sys.index_columns AS ic
                ON ic.object_id = kc.parent_object_id
               AND ic.index_id = kc.unique_index_id
            INNER JOIN sys.columns AS c
                ON c.object_id = ic.object_id
               AND c.column_id = ic.column_id
            WHERE kc.type = 'PK'
              AND s.name = @SchemaName
              AND t.name = @TableName
              AND ic.key_ordinal > 0
            ORDER BY ic.key_ordinal;
            """;
        command.Parameters.AddWithValue("@SchemaName", schemaName);
        command.Parameters.AddWithValue("@TableName", tableName);
        using var reader = command.ExecuteReader();
        var result = new List<string>();
        while (reader.Read())
        {
            result.Add(reader.GetString(0));
        }

        if (result.Count == 0)
        {
            throw new InvalidOperationException($"Primary key for '{schemaName}.{tableName}' was not found.");
        }

        return result;
    }

    private static bool PrimaryKeyExists(string connectionString, string schemaName, string tableName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(1)
            FROM sys.key_constraints AS kc
            INNER JOIN sys.tables AS t ON t.object_id = kc.parent_object_id
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            WHERE kc.type = 'PK'
              AND s.name = @SchemaName
              AND t.name = @TableName;
            """;
        command.Parameters.AddWithValue("@SchemaName", schemaName);
        command.Parameters.AddWithValue("@TableName", tableName);
        var value = command.ExecuteScalar();
        return Convert.ToInt32(value) > 0;
    }

    private static bool GetPrimaryKeyIsClustered(string connectionString, string schemaName, string tableName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT i.type_desc
            FROM sys.key_constraints AS kc
            INNER JOIN sys.tables AS t ON t.object_id = kc.parent_object_id
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            INNER JOIN sys.indexes AS i
                ON i.object_id = kc.parent_object_id
               AND i.index_id = kc.unique_index_id
            WHERE kc.type = 'PK'
              AND s.name = @SchemaName
              AND t.name = @TableName;
            """;
        command.Parameters.AddWithValue("@SchemaName", schemaName);
        command.Parameters.AddWithValue("@TableName", tableName);
        var value = command.ExecuteScalar();
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Primary key for '{schemaName}.{tableName}' was not found.");
        }

        return string.Equals(Convert.ToString(value), "CLUSTERED", StringComparison.OrdinalIgnoreCase);
    }

    private static bool GetPrimaryKeyKeyIsDescending(string connectionString, string schemaName, string tableName, string columnName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT ic.is_descending_key
            FROM sys.key_constraints AS kc
            INNER JOIN sys.tables AS t ON t.object_id = kc.parent_object_id
            INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
            INNER JOIN sys.index_columns AS ic
                ON ic.object_id = kc.parent_object_id
               AND ic.index_id = kc.unique_index_id
            INNER JOIN sys.columns AS c
                ON c.object_id = ic.object_id
               AND c.column_id = ic.column_id
            WHERE kc.type = 'PK'
              AND s.name = @SchemaName
              AND t.name = @TableName
              AND c.name = @ColumnName
              AND ic.key_ordinal > 0;
            """;
        command.Parameters.AddWithValue("@SchemaName", schemaName);
        command.Parameters.AddWithValue("@TableName", tableName);
        command.Parameters.AddWithValue("@ColumnName", columnName);
        var value = command.ExecuteScalar();
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Primary key key column '{schemaName}.{tableName}.{columnName}' was not found.");
        }

        return Convert.ToBoolean(value);
    }

    private static bool GetIndexIsUnique(string connectionString, string indexName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT i.is_unique
            FROM sys.indexes AS i
            WHERE i.name = @IndexName;
            """;
        command.Parameters.AddWithValue("@IndexName", indexName);
        var value = command.ExecuteScalar();
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Index '{indexName}' was not found.");
        }

        return Convert.ToBoolean(value);
    }

    private static bool GetIndexIsDescending(string connectionString, string indexName, string columnName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT ic.is_descending_key
            FROM sys.indexes AS i
            INNER JOIN sys.index_columns AS ic
                ON ic.object_id = i.object_id
               AND ic.index_id = i.index_id
            INNER JOIN sys.columns AS c
                ON c.object_id = i.object_id
               AND c.column_id = ic.column_id
            WHERE i.name = @IndexName
              AND c.name = @ColumnName
              AND ic.key_ordinal > 0;
            """;
        command.Parameters.AddWithValue("@IndexName", indexName);
        command.Parameters.AddWithValue("@ColumnName", columnName);
        var value = command.ExecuteScalar();
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException($"Index key '{indexName}.{columnName}' was not found.");
        }

        return Convert.ToBoolean(value);
    }

    private static void AddUnsupportedActionKindToManifestModel(string manifestWorkspacePath, string unsupportedEntityName)
    {
        var modelPath = Path.Combine(manifestWorkspacePath, "model.xml");
        var document = XDocument.Load(modelPath);
        var model = document.Root ?? throw new InvalidOperationException("Manifest model.xml root is missing.");
        var entityList = model.Element("EntityList") ?? throw new InvalidOperationException("Manifest model.xml EntityList is missing.");
        entityList.Add(
            new XElement("Entity",
                new XAttribute("name", unsupportedEntityName),
                new XElement("PropertyList",
                    new XElement("Property", new XAttribute("name", "UnsupportedId"))),
                new XElement("RelationshipList",
                    new XElement("Relationship", new XAttribute("entity", "DeployManifest")))));
        document.Save(modelPath);
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
    private static (int ExitCode, string Output) RunProcess(ProcessStartInfo startInfo, string errorMessage)
    {
        using var process = new Process { StartInfo = startInfo };
        if (!process.Start())
        {
            throw new InvalidOperationException(errorMessage);
        }

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        return (process.ExitCode, output + error);
    }

    private static void AssertOutputLineContains(string output, string prefix, params string[] fragments)
    {
        var line = output
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .FirstOrDefault(item => item.StartsWith(prefix, StringComparison.Ordinal));

        Assert.False(string.IsNullOrWhiteSpace(line), $"Expected an output line starting with '{prefix}', but none was found in:{Environment.NewLine}{output}");
        foreach (var fragment in fragments)
        {
            Assert.Contains(fragment, line, StringComparison.Ordinal);
        }
    }

    private static void AssertPlanChanges(string output, params string[] fragments)
    {
        AssertOutputLineContains(output, "Changes:", fragments);
    }

    private static void AssertAppliedChanges(string output, params string[] fragments)
    {
        AssertOutputLineContains(output, "Deployed:", fragments);
    }

    private static void DeleteIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
