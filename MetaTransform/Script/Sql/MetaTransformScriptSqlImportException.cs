namespace MetaTransformScript.Sql;

public enum MetaTransformScriptSqlImportFailureKind
{
    SourcePathNotFound,
    ParseFailed,
    UnsupportedSql,
    UnsupportedFunctionWrapper,
    LikelyTextEncodingMismatch,
    InvalidSqlInput
}

public sealed class MetaTransformScriptSqlImportException : InvalidOperationException
{
    public MetaTransformScriptSqlImportException(
        MetaTransformScriptSqlImportFailureKind kind,
        string message,
        Exception? innerException = null,
        int? line = null,
        int? column = null,
        int? offset = null)
        : base(message, innerException)
    {
        Kind = kind;
        Line = line;
        Column = column;
        Offset = offset;
    }

    public MetaTransformScriptSqlImportFailureKind Kind { get; }

    public int? Line { get; }

    public int? Column { get; }

    public int? Offset { get; }
}
