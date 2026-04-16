public sealed class GeneratedCliInvariantTests
{
    [Fact]
    public void GeneratedCliScenarios_PrGate_AreDeterministic()
    {
        var scenarioCount = ReadEnvInt("META_TRANSFORM_CLI_PR_SCENARIO_COUNT", fallback: 64);
        var seed = ReadEnvInt("META_TRANSFORM_CLI_PR_SEED", fallback: 420001);

        RunGeneratedCliScenarios(scenarioCount, seed);
    }

    [Fact]
    public void GeneratedCliScenarios_ThousandLane_WhenEnabled()
    {
        var enabled = string.Equals(
            Environment.GetEnvironmentVariable("META_TRANSFORM_CLI_ENABLE_THOUSAND_LANE"),
            "1",
            StringComparison.Ordinal);
        if (!enabled)
        {
            return;
        }

        var scenarioCount = ReadEnvInt("META_TRANSFORM_CLI_THOUSAND_SCENARIO_COUNT", fallback: 2000);
        var seed = ReadEnvInt("META_TRANSFORM_CLI_THOUSAND_SEED", fallback: 904213);
        RunGeneratedCliScenarios(scenarioCount, seed);
    }

    private static void RunGeneratedCliScenarios(int scenarioCount, int seed)
    {
        if (scenarioCount <= 0)
        {
            throw new InvalidOperationException("Scenario count must be greater than zero.");
        }

        var scenarios = GeneratedCliScenarioGenerator.CreateScenarios(scenarioCount, seed);

        var repoRoot = FindRepositoryRoot();
        var scriptCliPath = ResolveCliPath(
            repoRoot,
            "MetaTransform",
            "Script",
            "Cli",
            "bin",
            "Debug",
            "net8.0",
            "meta-transform-script.exe");
        var bindingCliPath = ResolveCliPath(
            repoRoot,
            "MetaTransform",
            "Binding",
            "Cli",
            "bin",
            "Debug",
            "net8.0",
            "meta-transform-binding.exe");

        var tempRoot = Path.Combine(
            Path.GetTempPath(),
            "meta-bi",
            "generated-cli-scenarios",
            Guid.NewGuid().ToString("N"));

        try
        {
            var schemaWorkspacePath = Path.Combine(tempRoot, "SchemaWS");
            GeneratedCliSchemaWorkspaceBuilder.SaveWorkspace(schemaWorkspacePath, scenarios);

            var failures = new List<string>();

            foreach (var scenario in scenarios)
            {
                var scenarioFolder = Path.Combine(tempRoot, $"Scenario_{scenario.ScenarioNumber:D5}");
                var transformWorkspacePath = Path.Combine(scenarioFolder, "TransformWS");
                var bindingWorkspacePath = Path.Combine(scenarioFolder, "BindingWS");
                var validatedWorkspacePath = Path.Combine(scenarioFolder, "ValidatedWS");
                var sqlCodeArgument = CollapseSqlForCliArgument(scenario.SqlCode);

                var importResult = GeneratedCliProcessRunner.Run(
                    scriptCliPath,
                    $"from sql-code --code \"{sqlCodeArgument}\" --new-workspace \"{transformWorkspacePath}\"",
                    repoRoot,
                    timeout: TimeSpan.FromMinutes(2));
                if (importResult.ExitCode != 0)
                {
                    failures.Add($"Scenario {scenario.ScenarioNumber:D5} import failed with exit {importResult.ExitCode}.\n{importResult.Output}");
                    if (failures.Count >= 25)
                    {
                        break;
                    }

                    continue;
                }

                var bindResult = GeneratedCliProcessRunner.Run(
                    bindingCliPath,
                    $"bind --transform-workspace \"{transformWorkspacePath}\" --new-workspace \"{bindingWorkspacePath}\"",
                    repoRoot,
                    timeout: TimeSpan.FromMinutes(2));
                if (bindResult.ExitCode != 0)
                {
                    failures.Add($"Scenario {scenario.ScenarioNumber:D5} bind failed with exit {bindResult.ExitCode}.\n{bindResult.Output}");
                    if (failures.Count >= 25)
                    {
                        break;
                    }

                    continue;
                }

                var validateResult = GeneratedCliProcessRunner.Run(
                    bindingCliPath,
                    $"validate --binding-workspace \"{bindingWorkspacePath}\" --schema-workspace \"{schemaWorkspacePath}\" --new-workspace \"{validatedWorkspacePath}\"",
                    repoRoot,
                    timeout: TimeSpan.FromMinutes(2));

                if (scenario.ValidationExpectation == GeneratedCliValidationExpectationKind.Success)
                {
                    if (validateResult.ExitCode != 0)
                    {
                        failures.Add($"Scenario {scenario.ScenarioNumber:D5} expected validate success but got exit {validateResult.ExitCode}.\n{validateResult.Output}");
                    }
                    else if (!File.Exists(Path.Combine(validatedWorkspacePath, "model.xml")))
                    {
                        failures.Add($"Scenario {scenario.ScenarioNumber:D5} expected validated workspace at '{validatedWorkspacePath}' but model.xml was missing.");
                    }
                }
                else
                {
                    if (validateResult.ExitCode != 5)
                    {
                        failures.Add($"Scenario {scenario.ScenarioNumber:D5} expected validate failure exit 5 but got {validateResult.ExitCode}.\n{validateResult.Output}");
                    }
                    else if (!validateResult.Output.Contains($"Code: {scenario.ExpectedValidationCode}", StringComparison.Ordinal))
                    {
                        failures.Add($"Scenario {scenario.ScenarioNumber:D5} expected validation code '{scenario.ExpectedValidationCode}' but output was:\n{validateResult.Output}");
                    }
                }

                if (failures.Count >= 25)
                {
                    break;
                }
            }

            Assert.True(
                failures.Count == 0,
                $"Generated CLI invariant failures ({failures.Count}):\n{string.Join("\n\n", failures)}");
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    private static string CollapseSqlForCliArgument(string sql)
    {
        return string.Join(
            " ",
            sql.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(static line => line.Trim())
                .Where(static line => line.Length > 0));
    }

    private static int ReadEnvInt(string name, int fallback)
    {
        var raw = Environment.GetEnvironmentVariable(name);
        return int.TryParse(raw, out var parsed) && parsed > 0
            ? parsed
            : fallback;
    }

    private static string ResolveCliPath(string repoRoot, params string[] relativeSegments)
    {
        var cliPath = Path.Combine(new[] { repoRoot }.Concat(relativeSegments).ToArray());
        if (!File.Exists(cliPath))
        {
            throw new FileNotFoundException(
                $"Could not find CLI executable at '{cliPath}'. Build the CLI projects before running generated CLI tests.");
        }

        return cliPath;
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var hasGitFolder = Directory.Exists(Path.Combine(current.FullName, ".git"));
            var hasSln = File.Exists(Path.Combine(current.FullName, "meta-bi.sln"));
            if (hasGitFolder || hasSln)
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root from test base directory.");
    }

    private static void DeleteDirectoryIfExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            return;
        }

        Directory.Delete(directoryPath, recursive: true);
    }
}
