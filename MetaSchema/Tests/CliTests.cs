using System.Globalization;
using MetaSchema.Core;
using Microsoft.Data.SqlClient;
using MetaSchema.Extractors.SqlServer;

using MetaBi.Tests.Common;

namespace MetaSchema.Tests;

public sealed class CliTests
{
    [Fact]
    public void Help_ShowsExtractCommand()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-schema <command> [options]", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("extract", result.Output);
    }

    [Fact]
    public void ExtractSqlServer_Help_ShowsRequiredOptions()
    {
        var result = RunCli("extract sqlserver --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--new-workspace <path>", result.Output);
        Assert.Contains("--connection-env <value>", result.Output);
        Assert.Contains("Options:", result.Output);
        Assert.Contains("--system", result.Output);
        Assert.Contains("--schema <value>", result.Output);
        Assert.Contains("--all-schemas", result.Output);
        Assert.Contains("--table <value>", result.Output);
        Assert.Contains("--all-tables", result.Output);
    }

    [Fact]
    public void ExtractSqlServer_FailsWhenConnectionEnvironmentVariableIsMissing()
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "metaschema-tests", Guid.NewGuid().ToString("N"));
        var environmentVariableName = "META_SCHEMA_TEST_" + Guid.NewGuid().ToString("N").ToUpperInvariant();
        var originalValue = Environment.GetEnvironmentVariable(environmentVariableName);

        try
        {
            Environment.SetEnvironmentVariable(environmentVariableName, null);

            var result = RunCli($"extract sqlserver --new-workspace \"{workspacePath}\" --connection-env {environmentVariableName} --system TestSystem --schema dbo --table Cube");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("Cannot extract schema.", result.Output);
            Assert.Contains($"Connection environment variable '{environmentVariableName}' was not found", result.Output);
            Assert.False(Directory.Exists(workspacePath));
        }
        finally
        {
            Environment.SetEnvironmentVariable(environmentVariableName, originalValue);
            DeleteDirectoryIfExists(workspacePath);
        }
    }

    [Fact]
    public void MetaSchemaModel_ModelsTablesViewsAndKeysWithoutTextDiscriminators()
    {
        var model = MetaSchemaModels.CreateMetaSchemaModel();

        var field = Assert.Single(model.Entities, entity => entity.Name == "Field");
        var fieldDataTypeDetail = Assert.Single(model.Entities, entity => entity.Name == "FieldDataTypeDetail");
        var schemaObject = Assert.Single(model.Entities, entity => entity.Name == "SchemaObject");
        var table = Assert.Single(model.Entities, entity => entity.Name == "Table");
        var view = Assert.Single(model.Entities, entity => entity.Name == "View");
        var key = Assert.Single(model.Entities, entity => entity.Name == "Key");
        var primaryKey = Assert.Single(model.Entities, entity => entity.Name == "PrimaryKey");
        var uniqueKey = Assert.Single(model.Entities, entity => entity.Name == "UniqueKey");
        var keyField = Assert.Single(model.Entities, entity => entity.Name == "KeyField");
        var tableRelationship = Assert.Single(model.Entities, entity => entity.Name == "TableRelationship");
        var tableRelationshipField = Assert.Single(model.Entities, entity => entity.Name == "TableRelationshipField");
        Assert.DoesNotContain(model.Entities, entity => entity.Name == "FieldType");
        Assert.DoesNotContain(model.Entities, entity => entity.Name == "TableKey");
        Assert.DoesNotContain(model.Entities, entity => entity.Name == "TableKeyField");
        Assert.Contains(field.Properties, property => property.Name == "MetaDataTypeId");
        Assert.Contains(field.Properties, property => property.Name == "IsIdentity");
        Assert.Contains(field.Properties, property => property.Name == "IdentitySeed");
        Assert.Contains(field.Properties, property => property.Name == "IdentityIncrement");
        Assert.DoesNotContain(field.Properties, property => property.Name == "Length");
        Assert.DoesNotContain(field.Properties, property => property.Name == "NumericPrecision");
        Assert.DoesNotContain(field.Properties, property => property.Name == "Scale");
        Assert.DoesNotContain(field.Relationships, relationship => relationship.Entity == "FieldType");
        Assert.Contains(field.Relationships, relationship => relationship.Entity == "SchemaObject");
        Assert.Contains(fieldDataTypeDetail.Relationships, relationship => relationship.Entity == "Field");
        Assert.Contains(schemaObject.Properties, property => property.Name == "Name");
        Assert.Contains(schemaObject.Relationships, relationship => relationship.Entity == "Schema");
        Assert.Contains(table.Relationships, relationship => relationship.Entity == "SchemaObject");
        Assert.Contains(view.Relationships, relationship => relationship.Entity == "SchemaObject");
        Assert.DoesNotContain(table.Properties, property => property.Name == "ObjectType");
        Assert.Contains(key.Relationships, relationship => relationship.Entity == "Table");
        Assert.DoesNotContain(key.Properties, property => property.Name == "KeyType");
        Assert.Contains(primaryKey.Relationships, relationship => relationship.Entity == "Key");
        Assert.Contains(uniqueKey.Relationships, relationship => relationship.Entity == "Key");
        Assert.Contains(keyField.Relationships, relationship => relationship.Entity == "Key");
        Assert.Contains(keyField.Relationships, relationship => relationship.Entity == "Field");
        Assert.DoesNotContain(keyField.Properties, property => property.Name == "FieldName");
        Assert.Contains(tableRelationship.Relationships, relationship => relationship.Entity == "Table" && string.Equals(relationship.Role, "SourceTable", StringComparison.Ordinal));
        Assert.Contains(tableRelationship.Relationships, relationship => relationship.Entity == "Table" && string.Equals(relationship.Role, "TargetTable", StringComparison.Ordinal));
        Assert.Contains(tableRelationshipField.Relationships, relationship => relationship.Entity == "TableRelationship");
        Assert.Contains(tableRelationshipField.Relationships, relationship => relationship.Entity == "Field" && string.Equals(relationship.Role, "SourceField", StringComparison.Ordinal));
        Assert.Contains(tableRelationshipField.Relationships, relationship => relationship.Entity == "Field" && string.Equals(relationship.Role, "TargetField", StringComparison.Ordinal));
        Assert.DoesNotContain(tableRelationship.Properties, property => property.Name == "TargetSchemaName");
        Assert.DoesNotContain(tableRelationship.Properties, property => property.Name == "TargetTableName");
        Assert.DoesNotContain(tableRelationshipField.Properties, property => property.Name == "SourceFieldName");
        Assert.DoesNotContain(tableRelationshipField.Properties, property => property.Name == "TargetFieldName");
    }

    [Fact]
    public void SqlServerExtractor_ExtractsMetaSqlCompatibleFieldTypeDetails()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "metaschema-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(tempRoot, "MetaSchemaWorkspace");
        var databaseName = $"MetaSchemaTypeDetails_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, """
                CREATE TABLE dbo.TypeDetailCase
                (
                    Id int NOT NULL,
                    LoadTimestamp datetime2(7) NOT NULL,
                    Amount decimal(18,2) NOT NULL,
                    AuditId int NOT NULL,
                    CONSTRAINT PK_TypeDetailCase PRIMARY KEY (Id)
                );
                """);

            var extractor = new SqlServerSchemaExtractor();
            var extracted = extractor.ExtractMetaSchemaModel(new SqlServerExtractRequest
            {
                NewWorkspacePath = workspacePath,
                ConnectionString = databaseConnectionString,
                SystemName = databaseName,
                SchemaName = "dbo",
                TableName = "TypeDetailCase",
            });

            var fieldsByName = extracted.FieldList
                .ToDictionary(row => row.Name, StringComparer.Ordinal);
            var detailNamesByFieldId = extracted.FieldDataTypeDetailList
                .GroupBy(row => row.Field.Id, StringComparer.Ordinal)
                .ToDictionary(
                    group => group.Key,
                    group => group.ToDictionary(row => row.Name, row => row.Value, StringComparer.Ordinal),
                    StringComparer.Ordinal);

            var loadTimestampDetails = detailNamesByFieldId[fieldsByName["LoadTimestamp"].Id];
            Assert.Equal("7", loadTimestampDetails["Precision"]);

            var amountDetails = detailNamesByFieldId[fieldsByName["Amount"].Id];
            Assert.Equal("18", amountDetails["Precision"]);
            Assert.Equal("2", amountDetails["Scale"]);

            Assert.False(detailNamesByFieldId.ContainsKey(fieldsByName["AuditId"].Id));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void SqlServerExtractor_ExtractsPhysicalDetailsForAliasTypes()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "metaschema-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(tempRoot, "MetaSchemaWorkspace");
        var databaseName = $"MetaSchemaAliasTypes_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, """
                CREATE TYPE dbo.Name FROM nvarchar(50) NOT NULL;
                CREATE TYPE dbo.Flag FROM bit NOT NULL;
                """);
            ExecuteSql(databaseConnectionString, """
                CREATE TABLE dbo.AliasTypeDetailCase
                (
                    Id int NOT NULL,
                    CustomerName dbo.Name NOT NULL,
                    IsActive dbo.Flag NOT NULL,
                    CONSTRAINT PK_AliasTypeDetailCase PRIMARY KEY (Id)
                );
                """);

            var extractor = new SqlServerSchemaExtractor();
            var extracted = extractor.ExtractMetaSchemaModel(new SqlServerExtractRequest
            {
                NewWorkspacePath = workspacePath,
                ConnectionString = databaseConnectionString,
                SystemName = databaseName,
                SchemaName = "dbo",
                TableName = "AliasTypeDetailCase",
            });

            var fieldsByName = extracted.FieldList
                .ToDictionary(row => row.Name, StringComparer.Ordinal);
            var detailNamesByFieldId = extracted.FieldDataTypeDetailList
                .GroupBy(row => row.Field.Id, StringComparer.Ordinal)
                .ToDictionary(
                    group => group.Key,
                    group => group.ToDictionary(row => row.Name, row => row.Value, StringComparer.Ordinal),
                    StringComparer.Ordinal);

            var customerName = fieldsByName["CustomerName"];
            Assert.Equal("sqlserver:type:Name", customerName.MetaDataTypeId);
            Assert.Equal("50", detailNamesByFieldId[customerName.Id]["Length"]);

            var isActive = fieldsByName["IsActive"];
            Assert.Equal("sqlserver:type:Flag", isActive.MetaDataTypeId);
            Assert.False(detailNamesByFieldId.ContainsKey(isActive.Id));
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void SqlServerExtractor_ExtractsIdentityColumnMetadata()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "metaschema-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(tempRoot, "MetaSchemaWorkspace");
        var databaseName = $"MetaSchemaIdentity_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, """
                CREATE TABLE dbo.IdentityCase
                (
                    Id int IDENTITY(100,5) NOT NULL,
                    CustomerName nvarchar(100) NOT NULL,
                    CONSTRAINT PK_IdentityCase PRIMARY KEY (Id)
                );
                """);

            var extractor = new SqlServerSchemaExtractor();
            var extracted = extractor.ExtractMetaSchemaModel(new SqlServerExtractRequest
            {
                NewWorkspacePath = workspacePath,
                ConnectionString = databaseConnectionString,
                SystemName = databaseName,
                SchemaName = "dbo",
                TableName = "IdentityCase",
            });

            var fieldsByName = extracted.FieldList
                .ToDictionary(row => row.Name, StringComparer.Ordinal);

            var schemaObject = Assert.Single(extracted.SchemaObjectList);
            var table = Assert.Single(extracted.TableList);
            Assert.Same(schemaObject, table.SchemaObject);
            Assert.Equal(table.Id, schemaObject.Id);
            Assert.Contains(":table:IdentityCase", schemaObject.Id, StringComparison.Ordinal);

            var primaryKey = Assert.Single(extracted.PrimaryKeyList);
            var key = primaryKey.Key;
            Assert.Equal(key.Id, primaryKey.Id);
            Assert.Same(table, key.Table);

            var keyField = Assert.Single(extracted.KeyFieldList);
            Assert.Same(key, keyField.Key);

            var identityField = fieldsByName["Id"];
            Assert.Same(identityField, keyField.Field);
            Assert.Equal("true", identityField.IsIdentity);
            Assert.Equal("100", identityField.IdentitySeed);
            Assert.Equal("5", identityField.IdentityIncrement);

            var nonIdentityField = fieldsByName["CustomerName"];
            Assert.Null(nonIdentityField.IsIdentity);
            Assert.Null(nonIdentityField.IdentitySeed);
            Assert.Null(nonIdentityField.IdentityIncrement);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void SqlServerExtractor_PreservesKeySpecializationAndColumnOrder()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "metaschema-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(tempRoot, "MetaSchemaWorkspace");
        var databaseName = $"MetaSchemaKeys_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, """
                CREATE TABLE dbo.KeyCase
                (
                    FirstPart int NOT NULL,
                    SecondPart int NOT NULL,
                    AlternatePart int NOT NULL,
                    CONSTRAINT PK_KeyCase PRIMARY KEY (SecondPart, FirstPart),
                    CONSTRAINT UQ_KeyCase_Alternate UNIQUE (AlternatePart, FirstPart)
                );
                """);

            var extracted = new SqlServerSchemaExtractor().ExtractMetaSchemaModel(new SqlServerExtractRequest
            {
                NewWorkspacePath = workspacePath,
                ConnectionString = databaseConnectionString,
                SystemName = databaseName,
                SchemaName = "dbo",
                TableName = "KeyCase",
            });

            var primaryKey = Assert.Single(extracted.PrimaryKeyList);
            var uniqueKey = Assert.Single(extracted.UniqueKeyList);
            Assert.Equal("PK_KeyCase", primaryKey.Key.Name);
            Assert.Equal("UQ_KeyCase_Alternate", uniqueKey.Key.Name);
            Assert.Equal(primaryKey.Key.Id, primaryKey.Id);
            Assert.Equal(uniqueKey.Key.Id, uniqueKey.Id);

            var primaryKeyFields = extracted.KeyFieldList
                .Where(keyField => string.Equals(keyField.Key.Id, primaryKey.Key.Id, StringComparison.Ordinal))
                .OrderBy(keyField => int.Parse(keyField.Ordinal, CultureInfo.InvariantCulture))
                .Select(keyField => keyField.Field.Name)
                .ToList();
            var uniqueKeyFields = extracted.KeyFieldList
                .Where(keyField => string.Equals(keyField.Key.Id, uniqueKey.Key.Id, StringComparison.Ordinal))
                .OrderBy(keyField => int.Parse(keyField.Ordinal, CultureInfo.InvariantCulture))
                .Select(keyField => keyField.Field.Name)
                .ToList();

            Assert.Equal(["SecondPart", "FirstPart"], primaryKeyFields);
            Assert.Equal(["AlternatePart", "FirstPart"], uniqueKeyFields);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void SqlServerExtractor_ExtractsViewColumns()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "metaschema-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(tempRoot, "MetaSchemaWorkspace");
        var databaseName = $"MetaSchemaViewColumns_{Guid.NewGuid():N}";
        var masterConnectionString = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        var databaseConnectionString = $"Server=.;Database={databaseName};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";

        try
        {
            CreateDatabase(masterConnectionString, databaseName);
            ExecuteSql(databaseConnectionString, """
                CREATE TABLE dbo.ViewSource
                (
                    SourceId int NOT NULL,
                    SourceCode varchar(50) NOT NULL,
                    SourceName nvarchar(50) NOT NULL
                );
                """);
            ExecuteSql(databaseConnectionString, """
                CREATE VIEW dbo.ViewCase
                AS
                SELECT
                    SourceId,
                    SourceCode,
                    SourceName AS AliasName
                FROM dbo.ViewSource;
                """);

            var extractor = new SqlServerSchemaExtractor();
            var extracted = extractor.ExtractMetaSchemaModel(new SqlServerExtractRequest
            {
                NewWorkspacePath = workspacePath,
                ConnectionString = databaseConnectionString,
                SystemName = databaseName,
                SchemaName = "dbo",
                TableName = "ViewCase",
            });

            Assert.Empty(extracted.TableList);
            var view = Assert.Single(extracted.ViewList);
            var schemaObject = Assert.Single(extracted.SchemaObjectList);
            Assert.Same(schemaObject, view.SchemaObject);
            Assert.Equal("ViewCase", schemaObject.Name);

            var fieldsByName = extracted.FieldList
                .ToDictionary(row => row.Name, StringComparer.Ordinal);

            Assert.Equal(3, fieldsByName.Count);
            Assert.Contains("SourceId", fieldsByName.Keys);
            Assert.Contains("SourceCode", fieldsByName.Keys);
            Assert.Contains("AliasName", fieldsByName.Keys);
            Assert.Equal("sqlserver:type:int", fieldsByName["SourceId"].MetaDataTypeId);
            Assert.Equal("sqlserver:type:varchar", fieldsByName["SourceCode"].MetaDataTypeId);
            Assert.Equal("sqlserver:type:nvarchar", fieldsByName["AliasName"].MetaDataTypeId);
            Assert.All(fieldsByName.Values, field =>
            {
                Assert.Same(schemaObject, field.SchemaObject);
                Assert.Null(field.IsNullable);
                Assert.Null(field.IsIdentity);
                Assert.Null(field.IdentitySeed);
                Assert.Null(field.IdentityIncrement);
            });
            Assert.Empty(extracted.KeyList);
            Assert.Empty(extracted.TableRelationshipList);
        }
        finally
        {
            DropDatabase(masterConnectionString, databaseName);
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    private static (int ExitCode, string Output) RunCli(string arguments) =>
        CliTestRunner.RunStandardCli("MetaSchema", "meta-schema.exe", arguments);

    private static void CreateDatabase(string masterConnectionString, string databaseName)
    {
        using var connection = new SqlConnection(masterConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"IF DB_ID(N'{databaseName.Replace("'", "''", StringComparison.Ordinal)}') IS NULL CREATE DATABASE [{databaseName}]";
        command.ExecuteNonQuery();
    }

    private static void DropDatabase(string masterConnectionString, string databaseName)
    {
        using var connection = new SqlConnection(masterConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"""
            IF DB_ID(N'{databaseName.Replace("'", "''", StringComparison.Ordinal)}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{databaseName}];
            END
            """;
        command.ExecuteNonQuery();
    }

    private static void ExecuteSql(string connectionString, string sql)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}


