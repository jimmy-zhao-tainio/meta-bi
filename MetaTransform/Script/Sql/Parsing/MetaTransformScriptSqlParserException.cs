namespace MetaTransformScript.Sql.Parsing;

public enum MetaTransformScriptSqlParserFailureKind
{
    ParseError,
    UnsupportedSyntax
}

public sealed class MetaTransformScriptSqlParserException : Exception
{
    public MetaTransformScriptSqlParserException(
        MetaTransformScriptSqlParserFailureKind failureKind,
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

    public MetaTransformScriptSqlParserFailureKind FailureKind { get; }

    public int Line { get; }

    public int Column { get; }

    public int Offset { get; }
}
