using MetaSql.App;

public sealed class ArtifactWriterTests
{
    [Fact]
    public void ArtifactWriterPackagesOnlySelectedTargets()
    {
        var repoRoot = TestSupport.CreateTempDirectory();
        var artifactRoot = TestSupport.CreateTempDirectory();
        const string envVarName = "PROD_CONN";
        try
        {
            Environment.SetEnvironmentVariable(envVarName, "Server=.;Database=Db;Integrated Security=true");
            Directory.CreateDirectory(Path.Combine(repoRoot, "sql", "qa"));
            Directory.CreateDirectory(Path.Combine(repoRoot, "sql", "prod"));
            Directory.CreateDirectory(Path.Combine(repoRoot, "deploy", "migrate", "baseline"));
            Directory.CreateDirectory(Path.Combine(repoRoot, "deploy", "migrate", "target", "qa"));
            Directory.CreateDirectory(Path.Combine(repoRoot, "deploy", "migrate", "target", "prod"));
            File.WriteAllText(Path.Combine(repoRoot, "sql", "qa", "H_QA.sql"), "CREATE TABLE [dbo].[H_QA] ([Id] int NOT NULL);");
            File.WriteAllText(Path.Combine(repoRoot, "sql", "prod", "H_PROD.sql"), "CREATE TABLE [dbo].[H_PROD] ([Id] int NOT NULL);");
            File.WriteAllText(Path.Combine(repoRoot, "deploy", "migrate", "baseline", "001.sql"), """
                -- meta-sql blocker-id: blk_base
                -- meta-sql blocker-kind: manual-required
                -- meta-sql object: dbo.S_Common

                ALTER TABLE dbo.S_Common DROP COLUMN LegacyName;
                """);
            File.WriteAllText(Path.Combine(repoRoot, "deploy", "migrate", "target", "prod", "002.sql"), """
                -- meta-sql blocker-id: blk_prod
                -- meta-sql blocker-kind: manual-required
                -- meta-sql object: dbo.S_Prod

                ALTER TABLE dbo.S_Prod DROP COLUMN LegacyName;
                """);
            File.WriteAllText(Path.Combine(repoRoot, "deploy", "migrate", "target", "qa", "003.sql"), """
                -- meta-sql blocker-id: blk_qa
                -- meta-sql blocker-kind: manual-required
                -- meta-sql object: dbo.S_Qa

                ALTER TABLE dbo.S_Qa DROP COLUMN LegacyName;
                """);
            File.WriteAllText(
                Path.Combine(repoRoot, "meta-sql.json"),
                """
                {
                  "targets": {
                    "qa": {
                      "desiredSql": "sql/qa",
                      "connectionStringEnvVar": "QA_CONN"
                    },
                    "prod": {
                      "desiredSql": "sql/prod",
                      "connectionStringEnvVar": "PROD_CONN"
                    }
                  }
                }
                """);

            new MetaSqlArtifactWriter().Write(repoRoot, artifactRoot, ["prod"]);

            Assert.True(File.Exists(Path.Combine(artifactRoot, "meta-sql", "desired-sql", "prod", "H_PROD.sql")));
            Assert.False(Directory.Exists(Path.Combine(artifactRoot, "meta-sql", "desired-sql", "qa")));
            Assert.True(File.Exists(Path.Combine(artifactRoot, "meta-sql", "migrate", "baseline", "001.sql")));
            Assert.True(File.Exists(Path.Combine(artifactRoot, "meta-sql", "migrate", "target", "prod", "002.sql")));
            Assert.False(Directory.Exists(Path.Combine(artifactRoot, "meta-sql", "migrate", "target", "qa")));

            var previousDirectory = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(artifactRoot);
                var context = new MetaSqlTargetContextLoader().Load("prod");
                Assert.Equal(MetaSqlRootMode.Artifact, context.Mode);
                Assert.Equal(Path.Combine(artifactRoot, "meta-sql", "desired-sql", "prod"), context.DesiredSqlPath);

                var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlTargetContextLoader().Load("qa"));
                Assert.Contains("not packaged in this artifact", exception.Message, StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                Directory.SetCurrentDirectory(previousDirectory);
            }
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVarName, null);
            Directory.Delete(repoRoot, recursive: true);
            Directory.Delete(artifactRoot, recursive: true);
        }
    }

    [Fact]
    public void ArtifactWriterCanPackageMultipleTargets()
    {
        var repoRoot = TestSupport.CreateTempDirectory();
        var artifactRoot = TestSupport.CreateTempDirectory();
        try
        {
            Directory.CreateDirectory(Path.Combine(repoRoot, "sql", "qa"));
            Directory.CreateDirectory(Path.Combine(repoRoot, "sql", "prod"));
            Directory.CreateDirectory(Path.Combine(repoRoot, "deploy", "migrate", "baseline"));
            Directory.CreateDirectory(Path.Combine(repoRoot, "deploy", "migrate", "target", "qa"));
            Directory.CreateDirectory(Path.Combine(repoRoot, "deploy", "migrate", "target", "prod"));
            File.WriteAllText(Path.Combine(repoRoot, "sql", "qa", "H_QA.sql"), "CREATE TABLE [dbo].[H_QA] ([Id] int NOT NULL);");
            File.WriteAllText(Path.Combine(repoRoot, "sql", "prod", "H_PROD.sql"), "CREATE TABLE [dbo].[H_PROD] ([Id] int NOT NULL);");
            File.WriteAllText(
                Path.Combine(repoRoot, "meta-sql.json"),
                """
                {
                  "targets": {
                    "qa": {
                      "desiredSql": "sql/qa",
                      "connectionStringEnvVar": "QA_CONN"
                    },
                    "prod": {
                      "desiredSql": "sql/prod",
                      "connectionStringEnvVar": "PROD_CONN"
                    }
                  }
                }
                """);

            new MetaSqlArtifactWriter().Write(repoRoot, artifactRoot, ["prod", "qa"]);

            Assert.True(File.Exists(Path.Combine(artifactRoot, "meta-sql", "desired-sql", "prod", "H_PROD.sql")));
            Assert.True(File.Exists(Path.Combine(artifactRoot, "meta-sql", "desired-sql", "qa", "H_QA.sql")));
        }
        finally
        {
            Directory.Delete(repoRoot, recursive: true);
            Directory.Delete(artifactRoot, recursive: true);
        }
    }
}
