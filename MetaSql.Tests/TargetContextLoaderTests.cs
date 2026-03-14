using MetaSql.App;

public sealed class TargetContextLoaderTests
{
    [Fact]
    public void MissingDeployWorkspaceFails()
    {
        var root = TestSupport.CreateTempDirectory();
        var previousDirectory = Directory.GetCurrentDirectory();
        try
        {
            Directory.SetCurrentDirectory(root);

            var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlTargetContextLoader().Load("prod"));

            Assert.Contains("could not find 'deploy/workspace.xml'", exception.Message, StringComparison.OrdinalIgnoreCase);
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
            TestSupport.WriteDeployWorkspace(
                root,
                targets:
                [
                    new DeployWorkspaceTargetSpec("prod", "sql/prod", ConnectionString: "Server=.;Database=Db;Integrated Security=true")
                ]);
            File.WriteAllText(
                Path.Combine(root, "deploy", "metadata", "instance", "DeployConfiguration.xml"),
                """
                <?xml version="1.0" encoding="utf-8"?>
                <MetaSqlDeploy>
                  <DeployConfigurationList>
                    <DeployConfiguration Id="deploy-config">
                      <RootMode>weird</RootMode>
                      <MigrationRoot>deploy/migrate</MigrationRoot>
                    </DeployConfiguration>
                  </DeployConfigurationList>
                </MetaSqlDeploy>
                """);

            Directory.SetCurrentDirectory(root);
            var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlTargetContextLoader().Load("prod"));

            Assert.Contains("unsupported RootMode", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void MissingDeployConfigurationFails()
    {
        var root = TestSupport.CreateTempDirectory();
        var previousDirectory = Directory.GetCurrentDirectory();
        try
        {
            TestSupport.WriteDeployWorkspace(
                root,
                targets:
                [
                    new DeployWorkspaceTargetSpec("prod", "sql/prod", ConnectionString: "Server=.;Database=Db;Integrated Security=true")
                ]);
            File.WriteAllText(
                Path.Combine(root, "deploy", "metadata", "instance", "DeployConfiguration.xml"),
                """
                <?xml version="1.0" encoding="utf-8"?>
                <MetaSqlDeploy>
                  <DeployConfigurationList />
                </MetaSqlDeploy>
                """);
            File.WriteAllText(
                Path.Combine(root, "deploy", "metadata", "instance", "DeployTarget.xml"),
                """
                <?xml version="1.0" encoding="utf-8"?>
                <MetaSqlDeploy>
                  <DeployTargetList />
                </MetaSqlDeploy>
                """);

            Directory.SetCurrentDirectory(root);
            var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlTargetContextLoader().Load("prod"));

            Assert.Contains("does not define a DeployConfiguration row", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDirectory);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void MultipleDeployConfigurationRowsFail()
    {
        var root = TestSupport.CreateTempDirectory();
        var previousDirectory = Directory.GetCurrentDirectory();
        try
        {
            TestSupport.WriteDeployWorkspace(
                root,
                targets:
                [
                    new DeployWorkspaceTargetSpec("prod", "sql/prod", ConnectionString: "Server=.;Database=Db;Integrated Security=true")
                ]);
            File.WriteAllText(
                Path.Combine(root, "deploy", "metadata", "instance", "DeployConfiguration.xml"),
                """
                <?xml version="1.0" encoding="utf-8"?>
                <MetaSqlDeploy>
                  <DeployConfigurationList>
                    <DeployConfiguration Id="deploy-config-1">
                      <RootMode>repo</RootMode>
                      <MigrationRoot>deploy/migrate</MigrationRoot>
                    </DeployConfiguration>
                    <DeployConfiguration Id="deploy-config-2">
                      <RootMode>repo</RootMode>
                      <MigrationRoot>deploy/migrate</MigrationRoot>
                    </DeployConfiguration>
                  </DeployConfigurationList>
                </MetaSqlDeploy>
                """);
            File.WriteAllText(
                Path.Combine(root, "deploy", "metadata", "instance", "DeployTarget.xml"),
                """
                <?xml version="1.0" encoding="utf-8"?>
                <MetaSqlDeploy>
                  <DeployTargetList />
                </MetaSqlDeploy>
                """);

            Directory.SetCurrentDirectory(root);
            var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlTargetContextLoader().Load("prod"));

            Assert.Contains("defines multiple DeployConfiguration rows", exception.Message, StringComparison.OrdinalIgnoreCase);
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
            TestSupport.WriteDeployWorkspace(
                root,
                targets:
                [
                    new DeployWorkspaceTargetSpec("prod", "sql/prod", ConnectionString: "Server=.;Database=Db;Integrated Security=true")
                ]);

            Directory.SetCurrentDirectory(root);
            var context = new MetaSqlTargetContextLoader().Load("prod");

            Assert.Equal(MetaSqlRootMode.Repo, context.Mode);
            Assert.Equal(Path.Combine(root, "deploy", "migrate"), context.MigrationRootPath);
            Assert.Equal(Path.Combine(root, "deploy", "migrate", "baseline"), context.BaselinePath);
            Assert.Equal(Path.Combine(root, "deploy", "migrate", "target", "prod"), context.TargetPath);
            Assert.Equal(Path.Combine(root, "deploy", "workspace.xml"), context.ConfigPath);
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
            TestSupport.WriteDeployWorkspace(
                root,
                targets:
                [
                    new DeployWorkspaceTargetSpec("prod", "sql/prod", ConnectionStringEnvVar: envVarName)
                ]);

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
            TestSupport.WriteDeployWorkspace(
                root,
                targets:
                [
                    new DeployWorkspaceTargetSpec("prod", "sql/prod", ConnectionStringEnvVar: envVarName)
                ]);

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
            TestSupport.WriteDeployWorkspace(
                root,
                targets:
                [
                    new DeployWorkspaceTargetSpec("prod", "sql/prod")
                ]);

            Directory.SetCurrentDirectory(root);
            var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlTargetContextLoader().Load("prod"));

            Assert.Contains("must define ConnectionString or ConnectionStringEnvVar", exception.Message, StringComparison.OrdinalIgnoreCase);
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
            TestSupport.WriteDeployWorkspace(
                root,
                mode: MetaSqlRootMode.Artifact,
                targets:
                [
                    new DeployWorkspaceTargetSpec("prod", "deploy/desired-sql/prod", ConnectionStringEnvVar: envVarName)
                ]);

            Directory.SetCurrentDirectory(root);
            var context = new MetaSqlTargetContextLoader().Load("prod");

            Assert.Equal(MetaSqlRootMode.Artifact, context.Mode);
            Assert.Equal(Path.Combine(root, "deploy", "migrate"), context.MigrationRootPath);
            Assert.Equal(Path.Combine(root, "deploy", "migrate", "baseline"), context.BaselinePath);
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
            TestSupport.WriteDeployWorkspace(
                root,
                mode: MetaSqlRootMode.Artifact,
                targets:
                [
                    new DeployWorkspaceTargetSpec("prod", "deploy/desired-sql/prod", ConnectionString: "Server=.;Database=ArtifactDb;Integrated Security=true")
                ]);

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
            TestSupport.WriteDeployWorkspace(
                root,
                mode: MetaSqlRootMode.Artifact,
                targets:
                [
                    new DeployWorkspaceTargetSpec(
                        "prod",
                        "deploy/desired-sql/prod",
                        ConnectionString: "Server=.;Database=Inline;Integrated Security=true",
                        ConnectionStringEnvVar: envVarName)
                ]);

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
            TestSupport.WriteDeployWorkspace(
                root,
                mode: MetaSqlRootMode.Artifact,
                targets:
                [
                    new DeployWorkspaceTargetSpec("prod", "deploy/desired-sql/prod")
                ]);

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
            TestSupport.WriteDeployWorkspace(
                root,
                mode: MetaSqlRootMode.Artifact,
                targets:
                [
                    new DeployWorkspaceTargetSpec("qa", "deploy/desired-sql/qa", ConnectionString: "Server=.;Database=Db;Integrated Security=true")
                ]);

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
            @"C:\artifact\deploy\workspace.xml",
            @"C:\artifact\deploy\desired-sql\prod",
            null,
            "Server=.;Database=Db;Integrated Security=true",
            @"C:\artifact\deploy\migrate",
            @"C:\artifact\deploy\migrate\baseline",
            @"C:\artifact\deploy\migrate\target\prod",
            @"C:\artifact\deploy\migrate\archive");

        var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlResolveGuard().EnsureResolveAllowed(context));

        Assert.Contains("resolve is repo-mode only", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
