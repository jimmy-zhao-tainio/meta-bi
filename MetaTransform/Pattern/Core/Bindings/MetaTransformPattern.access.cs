// Derived typed member access. Keep outside authored workspace source sets.
#nullable enable
namespace MetaTransformPattern;
public interface IMetaTransformPatternCollectionVisitor
{
    void Visit<TEntity>(string entityName, global::System.Func<global::MetaTransformPattern.MetaTransformPatternModel, global::System.Collections.Generic.IList<TEntity>> rows, global::System.Func<TEntity, string> readId, global::System.Action<TEntity, string> writeId)
        where TEntity : class;
}

public interface IMetaTransformPatternRelationshipVisitor
{
    void Visit<TRow, TTarget>(string entityName, string navigationName, string targetEntityName, bool isNullable, global::System.Func<global::MetaTransformPattern.MetaTransformPatternModel, global::System.Collections.Generic.IList<TRow>> rows, global::System.Func<TRow, TTarget?> read, global::System.Action<TRow, TTarget?> write, global::System.Func<TTarget, string> readTargetId)
        where TRow : class where TTarget : class;
}

public static class MetaTransformPatternModelAccess
{
    public static void VisitCollections(IMetaTransformPatternCollectionVisitor visitor)
    {
        global::System.ArgumentNullException.ThrowIfNull(visitor);
        visitor.Visit<global::MetaTransformPattern.TransformPattern>("TransformPattern", static model => model.TransformPatternList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaTransformPattern.TransformPatternItem>("TransformPatternItem", static model => model.TransformPatternItemList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaTransformPattern.TransformPatternPlaceholder>("TransformPatternPlaceholder", static model => model.TransformPatternPlaceholderList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaTransformPattern.TransformPatternPlaceholderItem>("TransformPatternPlaceholderItem", static model => model.TransformPatternPlaceholderItemList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaTransformPattern.TransformPatternText>("TransformPatternText", static model => model.TransformPatternTextList, static row => row.Id, static (row, id) => row.Id = id);
    }

    public static void VisitRelationships(IMetaTransformPatternRelationshipVisitor visitor)
    {
        global::System.ArgumentNullException.ThrowIfNull(visitor);
        visitor.Visit<global::MetaTransformPattern.TransformPatternItem, global::MetaTransformPattern.TransformPatternItem>("TransformPatternItem", "PreviousItem", "TransformPatternItem", true, static model => model.TransformPatternItemList, static row => row.PreviousItem, static (row, target) => row.PreviousItem = target, static target => target.Id);
        visitor.Visit<global::MetaTransformPattern.TransformPatternItem, global::MetaTransformPattern.TransformPattern>("TransformPatternItem", "TransformPattern", "TransformPattern", false, static model => model.TransformPatternItemList, static row => row.TransformPattern, static (row, target) => row.TransformPattern = target!, static target => target.Id);
        visitor.Visit<global::MetaTransformPattern.TransformPatternPlaceholder, global::MetaTransformPattern.TransformPattern>("TransformPatternPlaceholder", "TransformPattern", "TransformPattern", false, static model => model.TransformPatternPlaceholderList, static row => row.TransformPattern, static (row, target) => row.TransformPattern = target!, static target => target.Id);
        visitor.Visit<global::MetaTransformPattern.TransformPatternPlaceholderItem, global::MetaTransformPattern.TransformPatternItem>("TransformPatternPlaceholderItem", "TransformPatternItem", "TransformPatternItem", false, static model => model.TransformPatternPlaceholderItemList, static row => row.TransformPatternItem, static (row, target) => row.TransformPatternItem = target!, static target => target.Id);
        visitor.Visit<global::MetaTransformPattern.TransformPatternPlaceholderItem, global::MetaTransformPattern.TransformPatternPlaceholder>("TransformPatternPlaceholderItem", "TransformPatternPlaceholder", "TransformPatternPlaceholder", false, static model => model.TransformPatternPlaceholderItemList, static row => row.TransformPatternPlaceholder, static (row, target) => row.TransformPatternPlaceholder = target!, static target => target.Id);
        visitor.Visit<global::MetaTransformPattern.TransformPatternText, global::MetaTransformPattern.TransformPatternItem>("TransformPatternText", "TransformPatternItem", "TransformPatternItem", false, static model => model.TransformPatternTextList, static row => row.TransformPatternItem, static (row, target) => row.TransformPatternItem = target!, static target => target.Id);
    }
}