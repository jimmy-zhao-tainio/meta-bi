using System.Diagnostics;
using Meta.Integration;
using MetaSql.Extractors.SqlServer;
using MetaSqlDeployManifest;
using Microsoft.Data.SqlClient;
using static MetaSql.Tests.CliDiffTestSupport;

namespace MetaSql.Tests;

[Trait("Category", "Integration")]
public sealed class SqlServerSchemaDeploymentIntegrationTests
{
    [Fact]
    public async Task ScopedManifest_ReloadsAndDeploysWithoutChangingOtherSchemas()
    {
        using var fixture = new Fixture();
        await fixture.SaveSourceAsync();
        fixture.Sql("CREATE VIEW dw.Obsolete AS SELECT 1 AS Value;");
        var plan = fixture.Plan("--schema dw");
        Assert.True(plan.ExitCode == 0, plan.Output);
        var saved = await TypedWorkspaceModelMapper.LoadStateAsync(fixture.Manifest);
        Assert.Equal("dw", Assert.Single(Meta.TypedModels.TypedWorkspaceBindingValidation.Records(saved, "DeployManifest")).Values["ManagedSchemaName"]);
        var manifest = TypedWorkspaceModelMapper.FromInMemoryWorkspace(saved, new MetaSqlDeployManifestWorkspaceBinding());
        Assert.Single(manifest.DropViewList);
        fixture.Sql("CREATE VIEW dq.AddedAfterPlan AS SELECT 2 AS Value;");

        var deploy = fixture.Deploy();

        Assert.True(deploy.ExitCode == 0, deploy.Output);
        Assert.Null(fixture.Scalar("SELECT OBJECT_ID(N'dw.Obsolete');"));
        Assert.Equal("Åsa äter räksmörgås.", fixture.Scalar("SELECT Proof FROM dq.Preserved;"));
        Assert.Equal(2, fixture.Scalar("SELECT Value FROM dq.AddedAfterPlan;"));
    }

    [Fact]
    public async Task ScopedManifest_RejectsOwnedSchemaDriftBeforeExecutingAnyActions()
    {
        using var fixture = new Fixture();
        await fixture.SaveSourceAsync();
        fixture.Sql("CREATE VIEW dw.Obsolete AS SELECT 1 AS Value;");
        var plan = fixture.Plan("--schema dw");
        Assert.True(plan.ExitCode == 0, plan.Output);
        fixture.Sql("ALTER TABLE dw.Fact ADD ChangedAfterPlan int NULL;");

        var deploy = fixture.Deploy();

        Assert.NotEqual(0, deploy.ExitCode);
        Assert.Contains("live fingerprint mismatch", deploy.Output, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(fixture.Scalar("SELECT OBJECT_ID(N'dw.Obsolete');"));
        Assert.Equal("Åsa äter räksmörgås.", fixture.Scalar("SELECT Proof FROM dq.Preserved;"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ScopedPlanning_RejectsCrossSchemaForeignKeys(bool outbound)
    {
        using var fixture = new Fixture();
        await fixture.SaveSourceAsync();
        fixture.Sql("CREATE TABLE dq.OtherFact(Id int NOT NULL PRIMARY KEY, FactId int NULL);");
        fixture.Sql(outbound
            ? "ALTER TABLE dw.Fact ADD ExternalId int NULL REFERENCES dq.OtherFact(Id);"
            : "ALTER TABLE dq.OtherFact ADD CONSTRAINT FK_External_Fact FOREIGN KEY(FactId) REFERENCES dw.Fact(Id);");

        var plan = fixture.Plan("--schema dw");

        Assert.NotEqual(0, plan.ExitCode);
        Assert.Contains("cross-schema foreign key", plan.Output, StringComparison.OrdinalIgnoreCase);
        Assert.False(Directory.Exists(fixture.Manifest));
    }

    [Fact]
    public async Task ScopedDeployment_RejectsCrossSchemaForeignKeyAddedAfterPlanning()
    {
        using var fixture = new Fixture();
        await fixture.SaveSourceAsync();
        fixture.Sql("CREATE VIEW dw.Obsolete AS SELECT 1 AS Value;");
        var plan = fixture.Plan("--schema dw");
        Assert.True(plan.ExitCode == 0, plan.Output);
        fixture.Sql("CREATE TABLE dq.OtherFact(Id int NOT NULL PRIMARY KEY, FactId int REFERENCES dw.Fact(Id));");

        var deploy = fixture.Deploy();

        Assert.NotEqual(0, deploy.ExitCode);
        Assert.Contains("cross-schema foreign key", deploy.Output, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(fixture.Scalar("SELECT OBJECT_ID(N'dw.Obsolete');"));
    }

    [Fact]
    public async Task ScopedPlanning_RejectsSourceObjectsOutsideTheDeclaredScope()
    {
        using var fixture = new Fixture();
        await fixture.SaveSourceAsync();

        var plan = fixture.Plan("--schema dq");

        Assert.NotEqual(0, plan.ExitCode);
        Assert.Contains("dw.Fact", plan.Output, StringComparison.Ordinal);
        Assert.Contains("scope", plan.Output, StringComparison.OrdinalIgnoreCase);
        Assert.False(Directory.Exists(fixture.Manifest));
    }

    [Fact]
    public async Task OmittedScope_RetainsDatabaseWideReconciliation()
    {
        using var fixture = new Fixture();
        await fixture.SaveSourceAsync();

        var plan = fixture.Plan("");

        Assert.True(plan.ExitCode == 0, plan.Output);
        var manifest = TypedWorkspaceModelMapper.Load(fixture.Manifest, new MetaSqlDeployManifestWorkspaceBinding());
        Assert.Single(manifest.DropViewList);
        var state = await TypedWorkspaceModelMapper.LoadStateAsync(fixture.Manifest);
        Assert.False(Assert.Single(Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DeployManifest")).Values.TryGetValue("ManagedSchemaName", out var scope)
            && !string.IsNullOrEmpty(scope));
    }

    private sealed class Fixture : IDisposable
    {
        private const string Master = "Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        private readonly string database = "MetaSqlSchemaScope_" + Guid.NewGuid().ToString("N");
        private readonly string root = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        private string Connection => new SqlConnectionStringBuilder(Master) { InitialCatalog = database }.ConnectionString;
        private string Source => Path.Combine(root, "source");
        public string Manifest => Path.Combine(root, "manifest");

        public Fixture()
        {
            CreateDatabase(Master, database);
            Sql("CREATE SCHEMA dw;");
            Sql("CREATE SCHEMA dq;");
            Sql("CREATE TABLE dw.Fact(Id int NOT NULL PRIMARY KEY, Name nvarchar(50) NULL);");
            Sql("CREATE VIEW dq.Preserved AS SELECT N'Åsa äter räksmörgås.' AS Proof;");
        }

        public Task SaveSourceAsync() => MetaSqlTestSupport.SaveXmlAsync(
            new SqlServerMetaSqlExtractor().ExtractMetaSqlModel(new SqlServerExtractRequest
            {
                ConnectionString = Connection,
                SchemaName = "dw"
            }), Source);

        public (int ExitCode, string Output) Plan(string options) => Cli(
            $"deploy-plan --source-workspace \"{Source}\" --output-xml \"{Manifest}\" {options}");

        public (int ExitCode, string Output) Deploy() => Cli(
            $"deploy --source-workspace \"{Source}\" --manifest-workspace \"{Manifest}\"");

        private (int ExitCode, string Output) Cli(string arguments)
        {
            var start = new ProcessStartInfo("meta-sql", arguments + " --connection-env META_SQL_SCOPE_TEST")
            {
                WorkingDirectory = FindRepositoryRoot(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            start.Environment["META_SQL_SCOPE_TEST"] = Connection;
            return RunProcess(start, "Could not start the SQL CLI.");
        }

        public void Sql(string sql) => ExecuteSql(Connection, sql);

        public object? Scalar(string sql)
        {
            using var connection = new SqlConnection(Connection);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            var value = command.ExecuteScalar();
            return value is DBNull ? null : value;
        }

        public void Dispose()
        {
            DropDatabase(Master, database);
            DeleteIfExists(root);
        }
    }
}
