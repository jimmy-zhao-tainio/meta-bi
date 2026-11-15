// Derived typed member access. Keep outside authored workspace source sets.
#nullable enable
namespace MetaTransformPatternInstance;
public interface IMetaTransformPatternInstanceCollectionVisitor
{
    void Visit<TEntity>(string entityName, global::System.Func<global::MetaTransformPatternInstance.MetaTransformPatternInstanceModel, global::System.Collections.Generic.IList<TEntity>> rows, global::System.Func<TEntity, string> readId, global::System.Action<TEntity, string> writeId)
        where TEntity : class;
}

public interface IMetaTransformPatternInstanceRelationshipVisitor
{
    void Visit<TRow, TTarget>(string entityName, string navigationName, string targetEntityName, bool isNullable, global::System.Func<global::MetaTransformPatternInstance.MetaTransformPatternInstanceModel, global::System.Collections.Generic.IList<TRow>> rows, global::System.Func<TRow, TTarget?> read, global::System.Action<TRow, TTarget?> write, global::System.Func<TTarget, string> readTargetId)
        where TRow : class where TTarget : class;
}

public static class MetaTransformPatternInstanceModelAccess
{
    public static void VisitCollections(IMetaTransformPatternInstanceCollectionVisitor visitor)
    {
        global::System.ArgumentNullException.ThrowIfNull(visitor);
        visitor.Visit<global::MetaTransformPatternInstance.TransformPatternInstance>("TransformPatternInstance", static model => model.TransformPatternInstanceList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaTransformPatternInstance.TransformPatternInstancePlaceholder>("TransformPatternInstancePlaceholder", static model => model.TransformPatternInstancePlaceholderList, static row => row.Id, static (row, id) => row.Id = id);
    }

    public static void VisitRelationships(IMetaTransformPatternInstanceRelationshipVisitor visitor)
    {
        global::System.ArgumentNullException.ThrowIfNull(visitor);
        visitor.Visit<global::MetaTransformPatternInstance.TransformPatternInstancePlaceholder, global::MetaTransformPatternInstance.TransformPatternInstance>("TransformPatternInstancePlaceholder", "TransformPatternInstance", "TransformPatternInstance", false, static model => model.TransformPatternInstancePlaceholderList, static row => row.TransformPatternInstance, static (row, target) => row.TransformPatternInstance = target!, static target => target.Id);
    }
}