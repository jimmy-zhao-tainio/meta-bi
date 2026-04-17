namespace MetaTransformScript.Sql;

public enum MetaTransformScriptSqlImportFailureKind
{
    SourcePathNotFound,
    ParseFailed,
    UnsupportedSql,
    InvalidSqlInput
}

public sealed class MetaTransformScriptSqlImportException : InvalidOperationException
{
    public MetaTransformScriptSqlImportException(
        MetaTransformScriptSqlImportFailureKind kind,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Kind = kind;
    }

    public MetaTransformScriptSqlImportFailureKind Kind { get; }
}
