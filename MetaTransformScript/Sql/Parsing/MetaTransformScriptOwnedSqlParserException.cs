namespace MetaTransformScript.Sql.Parsing;

public enum MetaTransformScriptOwnedSqlParserFailureKind
{
    ParseError,
    UnsupportedSyntax
}

public sealed class MetaTransformScriptOwnedSqlParserException : Exception
{
    public MetaTransformScriptOwnedSqlParserException(
        MetaTransformScriptOwnedSqlParserFailureKind failureKind,
        string message,
        int line,
        int column,
        int offset)
        : base($"{message} (line {line}, column {column})")
    {
        FailureKind = failureKind;
        Line = line;
        Column = column;
        Offset = offset;
    }

    public MetaTransformScriptOwnedSqlParserFailureKind FailureKind { get; }

    public int Line { get; }

    public int Column { get; }

    public int Offset { get; }
}
