// Derived typed member access. Keep outside authored workspace source sets.
#nullable enable
namespace MetaSqlScript;
public interface IMetaSqlScriptCollectionVisitor
{
    void Visit<TEntity>(string entityName, global::System.Func<global::MetaSqlScript.MetaSqlScriptModel, global::System.Collections.Generic.IList<TEntity>> rows, global::System.Func<TEntity, string> readId, global::System.Action<TEntity, string> writeId)
        where TEntity : class;
}

public interface IMetaSqlScriptRelationshipVisitor
{
    void Visit<TRow, TTarget>(string entityName, string navigationName, string targetEntityName, bool isNullable, global::System.Func<global::MetaSqlScript.MetaSqlScriptModel, global::System.Collections.Generic.IList<TRow>> rows, global::System.Func<TRow, TTarget?> read, global::System.Action<TRow, TTarget?> write, global::System.Func<TTarget, string> readTargetId)
        where TRow : class where TTarget : class;
}

public static class MetaSqlScriptModelAccess
{
    public static void VisitCollections(IMetaSqlScriptCollectionVisitor visitor)
    {
        global::System.ArgumentNullException.ThrowIfNull(visitor);
        visitor.Visit<global::MetaSqlScript.SqlScript>("SqlScript", static model => model.SqlScriptList, static row => row.Id, static (row, id) => row.Id = id);
    }

    public static void VisitRelationships(IMetaSqlScriptRelationshipVisitor visitor)
    {
        global::System.ArgumentNullException.ThrowIfNull(visitor);
    }
}