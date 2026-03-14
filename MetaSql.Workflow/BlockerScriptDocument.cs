namespace MetaSql.Workflow;

public sealed record BlockerScriptDocument(
    string Path,
    BlockerScriptHeader Header,
    string SqlBody);
