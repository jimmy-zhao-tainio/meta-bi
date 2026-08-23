namespace MetaTransformScript;

public enum TransformScriptStatementKind
{
    Unsupported,
    ScalarFunction,
    StoredProcedure,
    Select,
    Insert,
    Update,
    Delete,
    Truncate,
    Merge
}
