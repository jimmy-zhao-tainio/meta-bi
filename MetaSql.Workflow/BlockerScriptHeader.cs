namespace MetaSql.Workflow;

public sealed record BlockerScriptHeader(
    string BlockerId,
    BlockerKind Kind,
    string ObjectName);
