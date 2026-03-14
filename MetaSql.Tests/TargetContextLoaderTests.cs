using MetaSql.App;

public sealed class TargetContextLoaderTests
{
    [Fact]
    public void MissingMetaSqlJsonFails()
    {
        var root = TestSupport.CreateTempDirectory();
        var previousDirectory = Directory.GetCurrentDirectory();
        try
        {
            Directory.SetCurrentDirectory(root);

            var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlTargetContextLoader().Load("prod"));

            Assert.Contains("could not find 'meta-sql.json'", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void InvalidRootModeFails()
    {
        var root = TestSupport.CreateTempDirectory();
        var previousDirectory = Directory.GetCurrentDirectory();
        try
        {
            File.WriteAllText(
                Path.Combine(root, "meta-sql.json"),
                """
                {
                  "rootMode": "weird",
                  "targets": {
                    "prod": {
                      "desiredSql": "sql/prod",
                      "connectionString": "Server=.;Database=Db;Integrated Security=true"
                    }
                  }
                }
                """);

            Directory.SetCurrentDirectory(root);
            var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlTargetContextLoader().Load("prod"));

            Assert.Contains("unsupported rootMode", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void RepoModeUsesDefaultMigrationLayout()
    {
        var root = TestSupport.CreateTempDirectory();
        var previousDirectory = Directory.GetCurrentDirectory();
        try
        {
            File.WriteAllText(
                Path.Combine(root, "meta-sql.json"),
                """
                {
                  "targets": {
                    "prod": {
                      "desiredSql": "sql/prod",
                      "connectionString": "Server=.;Database=Db;Integrated Security=true"
                    }
                  }
                }
                """);

            Directory.SetCurrentDirectory(root);
            var context = new MetaSqlTargetContextLoader().Load("prod");

            Assert.Equal(MetaSqlRootMode.Repo, context.Mode);
            Assert.Equal(Path.Combine(root, "deploy", "migrate"), context.MigrationRootPath);
            Assert.Equal(Path.Combine(root, "deploy", "migrate", "baseline"), context.BaselinePath);
            Assert.Equal(Path.Combine(root, "deploy", "migrate", "target", "prod"), context.TargetPath);
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void RepoModeUsesEnvVarWhenPresent()
    {
        const string envVarName = "META_SQL_TEST_CONN";
        var root = TestSupport.CreateTempDirectory();
        var previousDirectory = Directory.GetCurrentDirectory();
        try
        {
            Environment.SetEnvironmentVariable(envVarName, "Server=.;Database=Db;Integrated Security=true");
            File.WriteAllText(
                Path.Combine(root, "meta-sql.json"),
                $$"""
                {
                  "targets": {
                    "prod": {
                      "desiredSql": "sql/prod",
                      "connectionStringEnvVar": "{{envVarName}}"
                    }
                  }
                }
                """);

            Directory.SetCurrentDirectory(root);
            var context = new MetaSqlTargetContextLoader().Load("prod");

            Assert.Equal("Server=.;Database=Db;Integrated Security=true", context.ConnectionString);
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVarName, null);
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void RepoModeMissingEnvVarFails()
    {
        const string envVarName = "META_SQL_TEST_CONN";
        var root = TestSupport.CreateTempDirectory();
        var previousDirectory = Directory.GetCurrentDirectory();
        try
        {
            Environment.SetEnvironmentVariable(envVarName, null);
            File.WriteAllText(
                Path.Combine(root, "meta-sql.json"),
                $$"""
                {
                  "targets": {
                    "prod": {
                      "desiredSql": "sql/prod",
                      "connectionStringEnvVar": "{{envVarName}}"
                    }
                  }
                }
                """);

            Directory.SetCurrentDirectory(root);
            var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlTargetContextLoader().Load("prod"));

            Assert.Contains(envVarName, exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void RepoModeWithoutConnectionSourceFails()
    {
        var root = TestSupport.CreateTempDirectory();
        var previousDirectory = Directory.GetCurrentDirectory();
        try
        {
            File.WriteAllText(
                Path.Combine(root, "meta-sql.json"),
                """
                {
                  "targets": {
                    "prod": {
                      "desiredSql": "sql/prod"
                    }
                  }
                }
                """);

            Directory.SetCurrentDirectory(root);
            var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlTargetContextLoader().Load("prod"));

            Assert.Contains("must define connectionString or connectionStringEnvVar", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ArtifactModeDetectsDefaultLayoutAndEnvVarConnection()
    {
        const string envVarName = "ARTIFACT_CONN";
        var root = TestSupport.CreateTempDirectory();
        var previousDirectory = Directory.GetCurrentDirectory();
        try
        {
            Environment.SetEnvironmentVariable(envVarName, "Server=.;Database=ArtifactDb;Integrated Security=true");
            File.WriteAllText(
                Path.Combine(root, "meta-sql.json"),
                $$"""
                {
                  "rootMode": "artifact",
                  "targets": {
                    "prod": {
                      "desiredSql": "meta-sql/desired-sql/prod",
                      "connectionStringEnvVar": "{{envVarName}}"
                    }
                  }
                }
                """);

            Directory.SetCurrentDirectory(root);
            var context = new MetaSqlTargetContextLoader().Load("prod");

            Assert.Equal(MetaSqlRootMode.Artifact, context.Mode);
            Assert.Equal(Path.Combine(root, "meta-sql", "migrate"), context.MigrationRootPath);
            Assert.Equal(Path.Combine(root, "meta-sql", "migrate", "baseline"), context.BaselinePath);
            Assert.Equal("Server=.;Database=ArtifactDb;Integrated Security=true", context.ConnectionString);
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVarName, null);
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ArtifactModeInlineConnectionOnlyIsAllowed()
    {
        var root = TestSupport.CreateTempDirectory();
        var previousDirectory = Directory.GetCurrentDirectory();
        try
        {
            File.WriteAllText(
                Path.Combine(root, "meta-sql.json"),
                """
                {
                  "rootMode": "artifact",
                  "targets": {
                    "prod": {
                      "desiredSql": "meta-sql/desired-sql/prod",
                      "connectionString": "Server=.;Database=ArtifactDb;Integrated Security=true"
                    }
                  }
                }
                """);

            Directory.SetCurrentDirectory(root);
            var context = new MetaSqlTargetContextLoader().Load("prod");

            Assert.Equal("Server=.;Database=ArtifactDb;Integrated Security=true", context.ConnectionString);
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ArtifactModePrefersEnvVarOverInlineConnection()
    {
        const string envVarName = "ARTIFACT_CONN";
        var root = TestSupport.CreateTempDirectory();
        var previousDirectory = Directory.GetCurrentDirectory();
        try
        {
            Environment.SetEnvironmentVariable(envVarName, "Server=.;Database=Injected;Integrated Security=true");
            File.WriteAllText(
                Path.Combine(root, "meta-sql.json"),
                $$"""
                {
                  "rootMode": "artifact",
                  "targets": {
                    "prod": {
                      "desiredSql": "meta-sql/desired-sql/prod",
                      "connectionString": "Server=.;Database=Inline;Integrated Security=true",
                      "connectionStringEnvVar": "{{envVarName}}"
                    }
                  }
                }
                """);

            Directory.SetCurrentDirectory(root);
            var context = new MetaSqlTargetContextLoader().Load("prod");

            Assert.Equal("Server=.;Database=Injected;Integrated Security=true", context.ConnectionString);
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVarName, null);
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ArtifactModeMissingConnectionSourceFailsPlainly()
    {
        var root = TestSupport.CreateTempDirectory();
        var previousDirectory = Directory.GetCurrentDirectory();
        try
        {
            File.WriteAllText(
                Path.Combine(root, "meta-sql.json"),
                """
                {
                  "rootMode": "artifact",
                  "targets": {
                    "prod": {
                      "desiredSql": "meta-sql/desired-sql/prod"
                    }
                  }
                }
                """);

            Directory.SetCurrentDirectory(root);
            var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlTargetContextLoader().Load("prod"));

            Assert.Contains("artifact mode requires a connection source", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ArtifactModeTargetAbsentFailsPlainly()
    {
        var root = TestSupport.CreateTempDirectory();
        var previousDirectory = Directory.GetCurrentDirectory();
        try
        {
            File.WriteAllText(
                Path.Combine(root, "meta-sql.json"),
                """
                {
                  "rootMode": "artifact",
                  "targets": {
                    "qa": {
                      "desiredSql": "meta-sql/desired-sql/qa",
                      "connectionString": "Server=.;Database=Db;Integrated Security=true"
                    }
                  }
                }
                """);

            Directory.SetCurrentDirectory(root);
            var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlTargetContextLoader().Load("prod"));

            Assert.Contains("not packaged in this artifact", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ResolveGuardRefusesArtifactMode()
    {
        var context = new MetaSqlTargetContext(
            MetaSqlRootMode.Artifact,
            "prod",
            @"C:\artifact",
            @"C:\artifact\meta-sql.json",
            @"C:\artifact\meta-sql\desired-sql\prod",
            null,
            "Server=.;Database=Db;Integrated Security=true",
            @"C:\artifact\meta-sql\migrate",
            @"C:\artifact\meta-sql\migrate\baseline",
            @"C:\artifact\meta-sql\migrate\target\prod",
            @"C:\artifact\meta-sql\migrate\archive");

        var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlResolveGuard().EnsureResolveAllowed(context));

        Assert.Contains("resolve is repo-mode only", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
