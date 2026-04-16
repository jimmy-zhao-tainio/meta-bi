internal enum GeneratedCliValidationExpectationKind
{
    Success = 0,
    TargetRequiredColumnMissing = 1,
    TargetOutputColumnNotInSchema = 2,
    TargetOutputColumnDuplicateMapping = 3
}

internal sealed record GeneratedCliScenario(
    int ScenarioNumber,
    int Seed,
    string ViewName,
    string SourceTableName,
    string LookupTableName,
    string TargetTableName,
    string SqlFileName,
    string SqlCode,
    GeneratedCliValidationExpectationKind ValidationExpectation)
{
    public string ViewSqlIdentifier => $"dbo.{ViewName}";
    public string SourceSqlIdentifier => $"dbo.{SourceTableName}";
    public string LookupSqlIdentifier => $"dbo.{LookupTableName}";
    public string TargetSqlIdentifier => $"dbo.{TargetTableName}";

    public string? ExpectedValidationCode => ValidationExpectation switch
    {
        GeneratedCliValidationExpectationKind.Success => null,
        GeneratedCliValidationExpectationKind.TargetRequiredColumnMissing => "TargetRequiredColumnMissing",
        GeneratedCliValidationExpectationKind.TargetOutputColumnNotInSchema => "TargetOutputColumnNotInSchema",
        GeneratedCliValidationExpectationKind.TargetOutputColumnDuplicateMapping => "TargetOutputColumnDuplicateMapping",
        _ => null
    };
}
