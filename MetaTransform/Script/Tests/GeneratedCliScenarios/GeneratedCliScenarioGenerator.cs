internal static class GeneratedCliScenarioGenerator
{
    public static IReadOnlyList<GeneratedCliScenario> CreateScenarios(int count, int seed)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, "Scenario count must be positive.");
        }

        var scenarios = new List<GeneratedCliScenario>(capacity: count);
        for (var i = 1; i <= count; i++)
        {
            var scenarioSeed = unchecked(seed + (i * 7919));
            scenarios.Add(CreateScenario(i, scenarioSeed));
        }

        return scenarios;
    }

    private static GeneratedCliScenario CreateScenario(int scenarioNumber, int seed)
    {
        var random = new Random(seed);
        var suffix = scenarioNumber.ToString("D5");
        var viewName = $"v_gen_{suffix}";
        var sourceTableName = $"Source_{suffix}";
        var lookupTableName = $"Lookup_{suffix}";
        var targetTableName = viewName;

        var validationExpectation = (GeneratedCliValidationExpectationKind)(scenarioNumber % 4);
        var sqlCode = RenderSql(
            random,
            viewSqlIdentifier: $"dbo.{viewName}",
            sourceSqlIdentifier: $"dbo.{sourceTableName}",
            lookupSqlIdentifier: $"dbo.{lookupTableName}",
            validationExpectation);

        return new GeneratedCliScenario(
            ScenarioNumber: scenarioNumber,
            Seed: seed,
            ViewName: viewName,
            SourceTableName: sourceTableName,
            LookupTableName: lookupTableName,
            TargetTableName: targetTableName,
            SqlFileName: $"{scenarioNumber:D5}_{viewName}.sql",
            SqlCode: sqlCode,
            ValidationExpectation: validationExpectation);
    }

    private static string RenderSql(
        Random random,
        string viewSqlIdentifier,
        string sourceSqlIdentifier,
        string lookupSqlIdentifier,
        GeneratedCliValidationExpectationKind validationExpectation)
    {
        var cteDepth = random.Next(minValue: 1, maxValue: 5);
        var includeJoin = random.NextDouble() >= 0.25d;
        var includeTautology = random.NextDouble() >= 0.35d;
        var includeOptionalAuditTag = random.NextDouble() >= 0.5d;
        var wrapperStyle = random.Next(minValue: 0, maxValue: 3);

        var sourceAlias = "src";
        var lookupAlias = "lkp";

        var cteLines = new List<string>(capacity: cteDepth);
        cteLines.Add($"""
base_0 AS
(
    SELECT
        {Wrap("CustomerId", sourceAlias, wrapperStyle)} AS CustomerId,
        {Wrap("OrderAmount", sourceAlias, wrapperStyle)} AS OrderAmount,
        {Wrap("CreatedAt", sourceAlias, wrapperStyle)} AS CreatedAt,
        {Wrap("RegionCode", sourceAlias, wrapperStyle)} AS RegionCode
    FROM {sourceSqlIdentifier} AS {sourceAlias}
{RenderJoin(includeJoin, lookupSqlIdentifier, lookupAlias, sourceAlias)}
{RenderWhere(includeTautology, sourceAlias)}
)
""");

        for (var i = 1; i < cteDepth; i++)
        {
            cteLines.Add($"""
base_{i} AS
(
    SELECT
        ({Wrap("CustomerId", $"b{i - 1}", wrapperStyle)}) AS CustomerId,
        ({Wrap("OrderAmount", $"b{i - 1}", wrapperStyle)}) AS OrderAmount,
        ({Wrap("CreatedAt", $"b{i - 1}", wrapperStyle)}) AS CreatedAt,
        ({Wrap("RegionCode", $"b{i - 1}", wrapperStyle)}) AS RegionCode
    FROM base_{i - 1} AS b{i - 1}
)
""");
        }

        var finalAlias = "f";
        var outputColumns = new List<string>(capacity: 6)
        {
            $"    {Wrap("CustomerId", finalAlias, wrapperStyle)} AS CustomerId",
            $"    {Wrap("OrderAmount", finalAlias, wrapperStyle)} AS OrderAmount",
            $"    {Wrap("CreatedAt", finalAlias, wrapperStyle)} AS CreatedAt"
        };

        if (includeOptionalAuditTag)
        {
            outputColumns.Add($"    'seed_{Math.Abs(random.Next()) % 100000:D5}' AS AuditTag");
        }

        switch (validationExpectation)
        {
            case GeneratedCliValidationExpectationKind.TargetRequiredColumnMissing:
                outputColumns.RemoveAll(item => item.Contains(" AS OrderAmount", StringComparison.Ordinal));
                break;

            case GeneratedCliValidationExpectationKind.TargetOutputColumnNotInSchema:
                outputColumns.Add($"    {Wrap("RegionCode", finalAlias, wrapperStyle)} AS UnexpectedRegionCode");
                break;

            case GeneratedCliValidationExpectationKind.TargetOutputColumnDuplicateMapping:
                outputColumns.RemoveAll(item => item.Contains(" AS CreatedAt", StringComparison.Ordinal));
                outputColumns.RemoveAll(item => item.Contains(" AS AuditTag", StringComparison.Ordinal));
                outputColumns.Add($"    {Wrap("CustomerId", finalAlias, wrapperStyle)} AS CustomerId");
                break;
        }

        return $"""
CREATE VIEW {viewSqlIdentifier}
AS
WITH
{string.Join(",\n", cteLines)}
SELECT
{string.Join(",\n", outputColumns)}
FROM base_{cteDepth - 1} AS {finalAlias};
""";
    }

    private static string RenderJoin(bool includeJoin, string lookupSqlIdentifier, string lookupAlias, string sourceAlias)
    {
        if (!includeJoin)
        {
            return string.Empty;
        }

        return $"""
    LEFT JOIN {lookupSqlIdentifier} AS {lookupAlias}
        ON {lookupAlias}.CustomerId = {sourceAlias}.CustomerId
""";
    }

    private static string RenderWhere(bool includeTautology, string sourceAlias)
    {
        if (!includeTautology)
        {
            return string.Empty;
        }

        return $"    WHERE ({sourceAlias}.CustomerId >= 0 OR {sourceAlias}.CustomerId = {sourceAlias}.CustomerId)";
    }

    private static string Wrap(string columnName, string alias, int wrapperStyle)
    {
        var reference = $"{alias}.{columnName}";
        return wrapperStyle switch
        {
            0 => reference,
            1 => $"(({reference}))",
            2 => $"COALESCE({reference}, {reference})",
            _ => reference
        };
    }
}
